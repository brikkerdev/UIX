using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class GetScreenTool
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

                var templateXml = File.ReadAllText(fullPath);
                var ussPath = $"Assets/UI/Screens/{name}/{name}.uss";
                var ussFullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), ussPath);
                var stylesUss = File.Exists(ussFullPath) ? File.ReadAllText(ussFullPath) : "";

                return UacfResponse.Success(new
                {
                    name,
                    template_xml = templateXml,
                    styles_uss = stylesUss,
                    viewmodel = name + "ViewModel",
                    prefab_path = $"Assets/UI/_Generated/Screens/{name}.prefab"
                }, 0);
            });
        }
    }
}
