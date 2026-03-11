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
    public static class CreateScreenTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var name = p["name"]?.ToString();
                if (string.IsNullOrEmpty(name))
                    return UacfResponse.Fail("INVALID_REQUEST", "name is required", null, 0);

                var basePath = "Assets/UI/Screens";
                var screenPath = Path.Combine(basePath, name);
                var xmlPath = Path.Combine(screenPath, name + ".xml");
                var ussPath = Path.Combine(screenPath, name + ".uss");
                var vmPath = Path.Combine(screenPath, name + "ViewModel.cs");

                var templateXml = p["template_xml"]?.ToString() ?? $"<screen name=\"{name}\" viewmodel=\"{name}ViewModel\">\n  <template><column class=\"screen\"><text text=\"{name}\" /></column></template>\n</screen>";
                var stylesUss = p["styles_uss"]?.ToString() ?? ".screen { padding: 24; }";

                Directory.CreateDirectory(screenPath);
                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                File.WriteAllText(Path.Combine(projectRoot, xmlPath), templateXml);
                File.WriteAllText(Path.Combine(projectRoot, ussPath), stylesUss);
                File.WriteAllText(Path.Combine(projectRoot, vmPath), $"using UIX.Binding;\n\npublic class {name}ViewModel : ViewModel {{ }}\n");

                AssetDatabase.Refresh();
                UIXCompiler.CompileAsset(xmlPath);
                UIXCompiler.CompileAsset(ussPath);

                return UacfResponse.Success(new
                {
                    screen_name = name,
                    created_files = new[] { xmlPath, ussPath, vmPath },
                    generated_files = new[] { $"Assets/UI/_Generated/Screens/{name}.prefab" },
                    validation_warnings = new string[0]
                }, 0);
            });
        }
    }
}
