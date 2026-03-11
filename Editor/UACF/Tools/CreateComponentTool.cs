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
    public static class CreateComponentTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var name = p["name"]?.ToString();
                if (string.IsNullOrEmpty(name))
                    return UacfResponse.Fail("INVALID_REQUEST", "name is required", "Provide component name in params.name", 0);

                var basePath = "Assets/UI/Components";
                var componentPath = Path.Combine(basePath, name);
                var xmlPath = Path.Combine(componentPath, name + ".xml");
                var ussPath = Path.Combine(componentPath, name + ".uss");

                var templateXml = p["template_xml"]?.ToString() ?? $"<component name=\"{name}\">\n  <props><prop name=\"text\" type=\"string\" /></props>\n  <template><column><text text=\"{{text}}\" /></column></template>\n</component>";
                var stylesUss = p["styles_uss"]?.ToString() ?? $".{name.ToLowerInvariant()} {{ padding: 16; }}";

                Directory.CreateDirectory(componentPath);
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), xmlPath), templateXml);
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), ussPath), stylesUss);

                AssetDatabase.Refresh();
                UIXCompiler.CompileAsset(xmlPath);
                UIXCompiler.CompileAsset(ussPath);

                return UacfResponse.Success(new
                {
                    component_name = name,
                    created_files = new[] { xmlPath, ussPath },
                    generated_prefab = $"Assets/UI/_Generated/Components/{name}.prefab",
                    validation_warnings = new string[0]
                }, 0);
            });
        }
    }
}
