using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class DeleteComponentTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var name = p["name"]?.ToString();
                if (string.IsNullOrEmpty(name))
                    return UacfResponse.Fail("INVALID_REQUEST", "name is required", null, 0);

                var componentPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/UI/Components", name);
                if (!Directory.Exists(componentPath))
                    return UacfResponse.Fail("NOT_FOUND", $"Component {name} not found", null, 0);

                Directory.Delete(componentPath, true);
                AssetDatabase.Refresh();

                return UacfResponse.Success(new { success = true, deleted_files = new[] { "Assets/UI/Components/" + name } }, 0);
            });
        }
    }
}
