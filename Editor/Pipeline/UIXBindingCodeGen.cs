using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UIX.Templates;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Generates C# binding code for screens.
    /// </summary>
    public static class UIXBindingCodeGen
    {
        public static void Generate(string screenName, string viewModelType, string outputPath)
        {
            Generate(null, screenName, viewModelType, outputPath);
        }

        public static void Generate(UIXTemplate template, string screenName, string viewModelType, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED FILE. DO NOT EDIT.");
            sb.AppendLine();
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using TMPro;");
            sb.AppendLine("using UIX.Binding;");
            sb.AppendLine();
            sb.AppendLine($"public partial class {screenName}_Bindings : UIXBindingBase");
            sb.AppendLine("{");

            var fields = new List<string>();
            var bindStatements = new List<string>();
            var unbindStatements = new List<string>();

            var unbindIds = new HashSet<string>();
            if (template?.Bindings != null && template.Bindings.Count > 0)
            {
                var seenIds = new HashSet<string>();
                foreach (var b in template.Bindings)
                {
                    var fieldName = ToFieldName(b.ElementId);
                    var componentType = GetComponentType(b.AttributeName);
                    if (string.IsNullOrEmpty(componentType)) continue;

                    if (!seenIds.Contains(b.ElementId))
                    {
                        seenIds.Add(b.ElementId);
                        fields.Add($"    private {componentType} _{fieldName};");
                    }

                    var vmProp = ExtractPropertyPath(b.Expression);
                    var bindCode = GenerateBindCode(b, fieldName, componentType, viewModelType, vmProp);
                    if (!string.IsNullOrEmpty(bindCode))
                        bindStatements.Add(bindCode);

                    if (!unbindIds.Contains(b.ElementId))
                    {
                        unbindIds.Add(b.ElementId);
                        unbindStatements.Add($"_{fieldName} = null;");
                    }
                }
            }

            foreach (var f in fields)
                sb.AppendLine(f);
            sb.AppendLine();

            sb.AppendLine("    public override void Bind(ViewModel viewModel, GameObject root)");
            sb.AppendLine("    {");
            sb.AppendLine($"        var vm = ({viewModelType})viewModel;");
            if (template?.Bindings != null)
            {
                var seenIds = new HashSet<string>();
                foreach (var b in template.Bindings)
                {
                    var fieldName = ToFieldName(b.ElementId);
                    var componentType = GetComponentType(b.AttributeName);
                    if (string.IsNullOrEmpty(componentType) || seenIds.Contains(b.ElementId)) continue;
                    seenIds.Add(b.ElementId);
                    sb.AppendLine($"        _{fieldName} = root.FindDeep<{componentType}>(\"{b.ElementId}\");");
                }
            }
            foreach (var stmt in bindStatements)
                sb.AppendLine("        " + stmt);
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public override void Unbind()");
            sb.AppendLine("    {");
            foreach (var stmt in unbindStatements)
                sb.AppendLine("        " + stmt);
            sb.AppendLine("    }");
            sb.AppendLine("}");

            var dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(outputPath, sb.ToString());
        }

        private static string ToFieldName(string id)
        {
            if (string.IsNullOrEmpty(id)) return "element";
            var s = id.Substring(0, 1).ToLowerInvariant() + id.Substring(1);
            return s.Replace("-", "_").Replace(" ", "");
        }

        private static string GetComponentType(string attrName)
        {
            var k = attrName?.ToLowerInvariant() ?? "";
            return k switch
            {
                "text" => "TextMeshProUGUI",
                "value" => "Slider",
                "ison" => "Toggle",
                "onclick" => "Button",
                "onvaluechanged" => "Slider",
                "ontoggled" => "Toggle",
                "onendedit" => "TMP_InputField",
                "visible" => "RectTransform",
                "enabled" => "RectTransform",
                _ => "RectTransform"
            };
        }

        private static string ExtractPropertyPath(string expr)
        {
            if (string.IsNullOrEmpty(expr)) return "";
            var s = expr.Trim();
            if (s.StartsWith("=")) s = s.Substring(1).Trim();
            return s;
        }

        private static string GenerateBindCode(UIXTemplate.BindingInfo b, string fieldName, string componentType, string vmType, string vmProp)
        {
            if (string.IsNullOrEmpty(vmProp)) return "";

            var attr = b.AttributeName.ToLowerInvariant();

            if (b.Type == UIXTemplate.BindingType.OneWay)
            {
                if (attr == "text")
                    return $"if (_{fieldName} != null && vm.{vmProp} != null) {{ vm.{vmProp}.OnChanged += v => _{fieldName}.text = v?.ToString() ?? \"\"; _{fieldName}.text = vm.{vmProp}.Value?.ToString() ?? \"\"; }}";
                if (attr == "visible")
                    return $"if (_{fieldName} != null && vm.{vmProp} != null) {{ vm.{vmProp}.OnChanged += v => _{fieldName}.gameObject.SetActive(v is bool b && b); _{fieldName}.gameObject.SetActive(vm.{vmProp}.Value is bool b2 && b2); }}";
                if (attr == "enabled")
                    return $"if (_{fieldName} != null && vm.{vmProp} != null) {{ var cg = _{fieldName}.GetComponent<CanvasGroup>() ?? _{fieldName}.gameObject.AddComponent<CanvasGroup>(); vm.{vmProp}.OnChanged += v => {{ cg.interactable = v is bool b && b; cg.blocksRaycasts = cg.interactable; }}; cg.interactable = vm.{vmProp}.Value is bool b2 && b2; cg.blocksRaycasts = cg.interactable; }}";
            }
            else if (b.Type == UIXTemplate.BindingType.TwoWay)
            {
                if (attr == "value" && componentType == "Slider")
                    return $"if (_{fieldName} != null && vm.{vmProp} != null) {{ vm.{vmProp}.OnChanged += v => _{fieldName}.value = v; _{fieldName}.onValueChanged.AddListener(v => vm.{vmProp}.Value = v); _{fieldName}.value = vm.{vmProp}.Value; }}";
                if (attr == "ison" && componentType == "Toggle")
                    return $"if (_{fieldName} != null && vm.{vmProp} != null) {{ vm.{vmProp}.OnChanged += v => _{fieldName}.isOn = v; _{fieldName}.onValueChanged.AddListener(v => vm.{vmProp}.Value = v); _{fieldName}.isOn = vm.{vmProp}.Value; }}";
                if (attr == "text" && componentType == "TMP_InputField")
                    return $"if (_{fieldName} != null && vm.{vmProp} != null) {{ vm.{vmProp}.OnChanged += v => _{fieldName}.text = v?.ToString() ?? \"\"; _{fieldName}.onValueChanged.AddListener(v => vm.{vmProp}.Value = v); _{fieldName}.text = vm.{vmProp}.Value?.ToString() ?? \"\"; }}";
            }
            else if (b.Type == UIXTemplate.BindingType.Event)
            {
                if (attr == "onclick")
                    return $"if (_{fieldName} != null) _{fieldName}.onClick.AddListener(() => vm.{vmProp}());";
                if (attr == "onvaluechanged")
                    return $"if (_{fieldName} != null) _{fieldName}.onValueChanged.AddListener(v => vm.{vmProp}(v));";
                if (attr == "ontoggled")
                    return $"if (_{fieldName} != null) _{fieldName}.onValueChanged.AddListener(v => vm.{vmProp}());";
                if (attr == "onendedit")
                    return $"if (_{fieldName} != null) _{fieldName}.onEndEdit.AddListener(s => vm.{vmProp}(s));";
            }

            return "";
        }

    }
}
