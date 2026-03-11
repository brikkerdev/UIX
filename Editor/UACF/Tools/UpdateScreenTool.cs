using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;
using UIX.Editor.Pipeline;

namespace UIX.Editor.UACF.Tools
{
    public static class UpdateScreenTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var name = p["name"]?.ToString();
                if (string.IsNullOrEmpty(name))
                    return UacfResponse.Fail("INVALID_REQUEST", "name is required", null, 0);

                var xmlPath = $"Assets/UI/Screens/{name}/{name}.xml";
                var fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), xmlPath);
                if (!File.Exists(fullPath))
                    return UacfResponse.Fail("NOT_FOUND", $"Screen {name} not found", null, 0);

                var templateXml = p["template_xml"]?.ToString();
                if (!string.IsNullOrEmpty(templateXml))
                    File.WriteAllText(fullPath, templateXml);

                AssetDatabase.Refresh();
                var xmlContent = File.ReadAllText(fullPath);
                var template = UIXCompiler.CompileXml(xmlPath, xmlContent);
                UIXCompiler.CompileAsset($"Assets/UI/Screens/{name}/{name}.uss");

                var prefabPath = (string)null;
                if (template != null)
                {
                    prefabPath = UIXPrefabGenerator.SaveAsPrefab(template);
                    if (!string.IsNullOrEmpty(template.ViewModelType))
                    {
                        var bindingsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/UI/_Generated/Screens", name + "_Bindings.generated.cs");
                        var bindingsDir = Path.GetDirectoryName(bindingsPath);
                        if (!Directory.Exists(bindingsDir)) Directory.CreateDirectory(bindingsDir);
                        UIXBindingCodeGen.Generate(name, template.ViewModelType, bindingsPath);
                    }
                }
                AssetDatabase.Refresh();

                return UacfResponse.Success(new { success = true, updated_files = new[] { xmlPath }, regenerated_prefab = prefabPath }, 0);
            });
        }
    }
}
