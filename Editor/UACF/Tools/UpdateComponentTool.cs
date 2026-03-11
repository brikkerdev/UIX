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
    public static class UpdateComponentTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var name = p["name"]?.ToString();
                if (string.IsNullOrEmpty(name))
                    return UacfResponse.Fail("INVALID_REQUEST", "name is required", null, 0);

                var xmlPath = $"Assets/UI/Components/{name}/{name}.xml";
                var fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), xmlPath);
                if (!File.Exists(fullPath))
                    return UacfResponse.Fail("NOT_FOUND", $"Component {name} not found", null, 0);

                var templateXml = p["template_xml"]?.ToString();
                if (!string.IsNullOrEmpty(templateXml))
                    File.WriteAllText(fullPath, templateXml);

                var ussPath = $"Assets/UI/Components/{name}/{name}.uss";
                var ussFullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), ussPath);
                var stylesUss = p["styles_uss"]?.ToString();
                if (!string.IsNullOrEmpty(stylesUss) && File.Exists(ussFullPath))
                    File.WriteAllText(ussFullPath, stylesUss);

                AssetDatabase.Refresh();
                var xmlContent = File.ReadAllText(fullPath);
                var template = UIXCompiler.CompileXml(xmlPath, xmlContent);
                UIXCompiler.CompileAsset(ussPath);

                var prefabPath = (string)null;
                if (template != null)
                    prefabPath = UIXPrefabGenerator.SaveAsPrefab(template);
                AssetDatabase.Refresh();

                return UacfResponse.Success(new
                {
                    success = true,
                    updated_files = new[] { xmlPath },
                    regenerated_prefab = prefabPath,
                    validation_warnings = new string[0]
                }, 0);
            });
        }
    }
}
