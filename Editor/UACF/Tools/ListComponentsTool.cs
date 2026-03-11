using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class ListComponentsTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var filter = p["filter"]?.ToString();
                var basePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/UI/Components");
                var components = new List<object>();

                if (Directory.Exists(basePath))
                {
                    foreach (var dir in Directory.GetDirectories(basePath))
                    {
                        var name = Path.GetFileName(dir);
                        if (!string.IsNullOrEmpty(filter) && !name.Contains(filter)) continue;
                        var xmlPath = Path.Combine(dir, name + ".xml");
                        if (File.Exists(xmlPath))
                            components.Add(new { name, props_count = 0, used_by_count = 0, path = "Assets/UI/Components/" + name + "/" });
                    }
                }

                return UacfResponse.Success(new { components }, 0);
            });
        }
    }
}
