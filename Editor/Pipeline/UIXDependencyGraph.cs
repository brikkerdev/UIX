using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Tracks dependencies between components, screens, and themes for incremental compilation.
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Dependency Graph")]
    public class UIXDependencyGraph : ScriptableObject
    {
        [System.Serializable]
        public class ComponentDeps
        {
            public string ComponentName;
            public List<string> UsedByScreens = new List<string>();
            public List<string> UsedByComponents = new List<string>();
        }

        [System.Serializable]
        public class ScreenDeps
        {
            public string ScreenName;
            public List<string> UsesComponents = new List<string>();
            public string ThemeName;
        }

        [System.Serializable]
        public class ThemeDeps
        {
            public string ThemeName;
            public List<string> AffectedScreens = new List<string>();
            public List<string> AffectedComponents = new List<string>();
        }

        public List<ComponentDeps> ComponentDependencies = new List<ComponentDeps>();
        public List<ScreenDeps> ScreenDependencies = new List<ScreenDeps>();
        public List<ThemeDeps> ThemeDependencies = new List<ThemeDeps>();

        public string LastBuiltPath;

        public void GetScreensUsingComponent(string componentName, List<string> outScreens)
        {
            outScreens.Clear();
            foreach (var cd in ComponentDependencies)
            {
                if (cd.ComponentName == componentName)
                {
                    outScreens.AddRange(cd.UsedByScreens);
                    break;
                }
            }
        }

        public void GetComponentsUsedByScreen(string screenName, List<string> outComponents)
        {
            outComponents.Clear();
            foreach (var sd in ScreenDependencies)
            {
                if (sd.ScreenName == screenName)
                {
                    outComponents.AddRange(sd.UsesComponents);
                    break;
                }
            }
        }

        public void GetAffectedByTheme(string themeName, List<string> outScreens, List<string> outComponents)
        {
            outScreens.Clear();
            outComponents.Clear();
            foreach (var td in ThemeDependencies)
            {
                if (td.ThemeName == themeName)
                {
                    outScreens.AddRange(td.AffectedScreens);
                    outComponents.AddRange(td.AffectedComponents);
                    break;
                }
            }
        }

        private static readonly Regex ComponentTagRegex = new Regex(@"<([A-Z][a-zA-Z0-9]*)(?:\s|>|/)", RegexOptions.Compiled);

        public static void BuildGraph(string projectRoot, UIXDependencyGraph graph)
        {
            if (graph == null) return;

            graph.ComponentDependencies.Clear();
            graph.ScreenDependencies.Clear();
            graph.ThemeDependencies.Clear();

            var componentRefs = new Dictionary<string, HashSet<string>>();
            var screenComponents = new Dictionary<string, HashSet<string>>();

            foreach (var basePath in new[]
            {
                Path.Combine(projectRoot, "Assets/UI/Screens"),
                Path.Combine(projectRoot, "Assets/Resources/UI/Screens")
            })
            {
                if (!Directory.Exists(basePath)) continue;
                foreach (var dir in Directory.GetDirectories(basePath))
                {
                    var name = Path.GetFileName(dir);
                    var xmlPath = Path.Combine(dir, name + ".xml");
                    if (!File.Exists(xmlPath)) continue;

                    var xml = File.ReadAllText(xmlPath);
                    var used = ExtractComponentNames(xml);
                    screenComponents[name] = used;
                    foreach (var comp in used)
                    {
                        if (!componentRefs.ContainsKey(comp))
                            componentRefs[comp] = new HashSet<string>();
                        componentRefs[comp].Add(name);
                    }
                }
            }

            foreach (var kv in componentRefs)
            {
                graph.ComponentDependencies.Add(new ComponentDeps
                {
                    ComponentName = kv.Key,
                    UsedByScreens = new List<string>(kv.Value)
                });
            }

            foreach (var kv in screenComponents)
            {
                graph.ScreenDependencies.Add(new ScreenDeps
                {
                    ScreenName = kv.Key,
                    UsesComponents = new List<string>(kv.Value)
                });
            }

            graph.LastBuiltPath = projectRoot;
        }

        private static HashSet<string> ExtractComponentNames(string xml)
        {
            var result = new HashSet<string>();
            foreach (Match m in ComponentTagRegex.Matches(xml))
            {
                if (m.Groups[1].Success)
                {
                    var name = m.Groups[1].Value;
                    if (!name.StartsWith("UIX"))
                        result.Add(name);
                }
            }
            return result;
        }
    }
}
