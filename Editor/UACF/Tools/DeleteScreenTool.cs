using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class DeleteScreenTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var name = p["name"]?.ToString();
                if (string.IsNullOrEmpty(name))
                    return UacfResponse.Fail("INVALID_REQUEST", "name is required", null, 0);

                var screenPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/UI/Screens", name);
                if (!Directory.Exists(screenPath))
                    return UacfResponse.Fail("NOT_FOUND", $"Screen {name} not found", null, 0);

                Directory.Delete(screenPath, true);
                AssetDatabase.Refresh();

                return UacfResponse.Success(new { success = true, deleted_files = new[] { "Assets/UI/Screens/" + name } }, 0);
            });
        }
    }
}
