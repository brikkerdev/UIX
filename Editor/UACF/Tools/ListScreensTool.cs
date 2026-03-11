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
    public static class ListScreensTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var filter = p["filter"]?.ToString();
                var basePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/UI/Screens");
                var screens = new List<object>();

                if (Directory.Exists(basePath))
                {
                    foreach (var dir in Directory.GetDirectories(basePath))
                    {
                        var name = Path.GetFileName(dir);
                        if (!string.IsNullOrEmpty(filter) && !name.Contains(filter)) continue;
                        var xmlPath = Path.Combine(dir, name + ".xml");
                        if (File.Exists(xmlPath))
                            screens.Add(new { name, path = "Assets/UI/Screens/" + name + "/" });
                    }
                }

                return UacfResponse.Success(new { screens }, 0);
            });
        }
    }
}
