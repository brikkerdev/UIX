using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class GetComponentTool
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

                var templateXml = File.ReadAllText(fullPath);
                var ussPath = $"Assets/UI/Components/{name}/{name}.uss";
                var ussFullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), ussPath);
                var stylesUss = File.Exists(ussFullPath) ? File.ReadAllText(ussFullPath) : "";

                return UacfResponse.Success(new
                {
                    name,
                    props = new object[0],
                    slots = new object[0],
                    template_xml = templateXml,
                    styles_uss = stylesUss,
                    used_by_screens = new string[0],
                    used_by_components = new string[0],
                    prefab_path = $"Assets/UI/_Generated/Components/{name}.prefab"
                }, 0);
            });
        }
    }
}
