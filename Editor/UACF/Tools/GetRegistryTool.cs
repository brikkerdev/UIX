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
    public static class GetRegistryTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var components = new List<object>();
                var screens = new List<object>();

                var projectRoot = Path.GetDirectoryName(Application.dataPath);

                var componentsPath = Path.Combine(projectRoot, "Assets/UI/Components");
                if (Directory.Exists(componentsPath))
                {
                    foreach (var dir in Directory.GetDirectories(componentsPath))
                    {
                        var name = Path.GetFileName(dir);
                        var xmlPath = Path.Combine(dir, name + ".xml");
                        if (File.Exists(xmlPath))
                            components.Add(new { name, props = new object[0], used_by = new string[0] });
                    }
                }

                var screensPath = Path.Combine(projectRoot, "Assets/UI/Screens");
                if (Directory.Exists(screensPath))
                {
                    foreach (var dir in Directory.GetDirectories(screensPath))
                    {
                        var name = Path.GetFileName(dir);
                        var xmlPath = Path.Combine(dir, name + ".xml");
                        if (File.Exists(xmlPath))
                            screens.Add(new { name, uses_components = new string[0], viewmodel = name + "ViewModel" });
                    }
                }

                return UacfResponse.Success(new
                {
                    components,
                    screens,
                    themes = new[] { "DefaultTheme" },
                    active_theme = "DefaultTheme",
                    uix_actions = new[]
                    {
                        new { action = "uix.create_component", description = "Create new UI component" },
                        new { action = "uix.create_screen", description = "Create new screen" },
                        new { action = "uix.get_registry", description = "Get full UIX registry" }
                    }
                }, 0);
            });
        }
    }
}
