using System.IO;
using UnityEditor;
using UnityEngine;
using UIX.Parsing;
using UIX.Parsing.Tokens;
using UIX.Templates;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Compiles XML and USS to ScriptableObject assets.
    /// </summary>
    public static class UIXCompiler
    {
        public static void CompileAsset(string assetPath)
        {
            var fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), assetPath);
            if (!File.Exists(fullPath)) return;

            var content = File.ReadAllText(fullPath);
            var ext = Path.GetExtension(assetPath).ToLowerInvariant();

            if (ext == ".xml")
            {
                CompileXml(assetPath, content);
            }
            else if (ext == ".uss")
            {
                CompileUss(assetPath, content);
            }
        }

        public static UIXTemplate CompileXml(string path, string xml)
        {
            var result = UIXValidation.ValidateXml(path, xml);
            if (!result.Valid)
            {
                foreach (var e in result.Errors)
                    Debug.LogError($"[UIX] {e.File}: {e.Message}");
                return null;
            }

            var root = XMLParser.Parse(xml, path);
            if (root == null) return null;

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var outputPath = Path.Combine(dir, "_Generated", name + "_Template.asset");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            var template = ScriptableObject.CreateInstance<UIXTemplate>();
            template.SourcePath = path;
            template.TemplateName = root.Name;
            template.IsComponent = root.IsComponent;
            template.ViewModelType = root.ViewModelType;

            foreach (var p in root.Props)
            {
                template.Props.Add(new UIXTemplate.SerializedPropDef
                {
                    Name = p.Name,
                    Type = p.Type,
                    Default = p.Default,
                    Optional = p.Optional
                });
            }

            foreach (var s in root.Slots)
            {
                template.Slots.Add(new UIXTemplate.SerializedSlotDef
                {
                    Name = s.Name,
                    Description = s.Description
                });
            }

            var assetPath = "Assets" + outputPath.Replace(Application.dataPath, "").Replace("\\", "/");
            if (!assetPath.StartsWith("Assets/"))
                assetPath = path.Replace("\\", "/").Replace(Path.GetFileName(path), "_Generated/" + name + "_Template.asset");

            AssetDatabase.CreateAsset(template, assetPath);
            AssetDatabase.SaveAssets();

            return template;
        }

        public static UIXStyleSheet CompileUss(string path, string uss)
        {
            var result = UIXValidation.ValidateUss(path, uss);
            foreach (var w in result.Warnings)
                Debug.LogWarning($"[UIX] {w.File}:{w.Line} {w.Message}");
            if (!result.Valid)
            {
                foreach (var e in result.Errors)
                    Debug.LogError($"[UIX] {e.File}: {e.Message}");
                return null;
            }

            var parseResult = USSParser.Parse(uss, path);

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileNameWithoutExtension(path);
            var assetPath = path.Replace("\\", "/").Replace(Path.GetFileName(path), "_Generated/" + name + "_StyleSheet.asset");
            var assetDir = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(assetDir))
                Directory.CreateDirectory(assetDir);

            var styleSheet = ScriptableObject.CreateInstance<UIXStyleSheet>();
            styleSheet.SourcePath = path;
            styleSheet.SourceUSS = uss;

            foreach (var v in parseResult.Variables)
            {
                styleSheet.Variables.Add(new UIXStyleSheet.SerializedVariable { Name = v.Name, Value = v.Value });
            }

            foreach (var rule in parseResult.Rules)
            {
                var sr = new UIXStyleSheet.SerializedStyleRule { LineNumber = rule.LineNumber };
                foreach (var p in rule.Properties)
                    sr.Properties.Add(new UIXStyleSheet.SerializedProperty { Name = p.Key, Value = p.Value });
                styleSheet.Rules.Add(sr);
            }

            AssetDatabase.CreateAsset(styleSheet, assetPath);
            AssetDatabase.SaveAssets();

            return styleSheet;
        }
    }
}
