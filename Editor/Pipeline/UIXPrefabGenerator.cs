using System.IO;
using UnityEngine;
using UnityEditor;
using UIX.Components;
using UIX.Parsing;
using UIX.Parsing.Nodes;
using UIX.Rendering;
using UIX.Styling;
using UIX.Core;
using UIX.Templates;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Generates uGUI prefabs from UIX templates.
    /// </summary>
    public static class UIXPrefabGenerator
    {
        private const string GeneratedPath = "Assets/UI/_Generated";

        public static GameObject Generate(UIXTemplate template)
        {
            if (template == null) return null;

            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            var fullPath = Path.Combine(projectRoot, template.SourcePath);
            var xml = File.Exists(fullPath) ? File.ReadAllText(fullPath) : "";
            var root = XMLParser.Parse(xml, template.SourcePath);
            if (root == null) return null;

            var variableResolver = new VariableResolver();
            var styleResolver = new StyleResolver(variableResolver);

            LoadThemeVariables(template.SourcePath, variableResolver);

            if (!string.IsNullOrEmpty(template.SourcePath))
            {
                var ussPath = Path.Combine(projectRoot, template.SourcePath.Replace(".xml", ".uss"));
                if (File.Exists(ussPath))
                {
                    var uss = File.ReadAllText(ussPath);
                    var parseResult = USSParser.Parse(uss, ussPath);
                    foreach (var v in parseResult.Variables)
                        variableResolver.SetVariable(v.Name, v.Value);
                    styleResolver.AddRules(parseResult.Rules);
                }
            }

            var componentResolver = LoadComponentResolver();
            var renderer = new UIXRenderer(styleResolver, variableResolver, componentResolver);
            var rootGo = new GameObject(template.TemplateName);
            var rect = rootGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            if (template.IsComponent)
                rootGo.AddComponent<UIXComponent>();
            else
                rootGo.AddComponent<UIXScreen>();

            var child = renderer.Render(root, rootGo.transform);
            if (child != null && child.transform.parent != rootGo.transform)
                child.transform.SetParent(rootGo.transform, false);

            return rootGo;
        }

        public static string SaveAsPrefab(UIXTemplate template)
        {
            var go = Generate(template);
            if (go == null) return null;

            var name = Path.GetFileNameWithoutExtension(template.SourcePath);
            var subdir = template.IsComponent ? "Components" : "Screens";
            var prefabPath = Path.Combine(GeneratedPath, subdir, name + ".prefab").Replace("\\", "/");

            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            var fullDir = Path.Combine(projectRoot, Path.GetDirectoryName(prefabPath));
            if (!Directory.Exists(fullDir))
                Directory.CreateDirectory(fullDir);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
            return prefab != null ? AssetDatabase.GetAssetPath(prefab) : null;
        }

        private static ComponentResolver LoadComponentResolver()
        {
            var guids = AssetDatabase.FindAssets("t:ComponentRegistry");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var registry = AssetDatabase.LoadAssetAtPath<ComponentRegistry>(path);
                if (registry != null)
                    return new ComponentResolver(registry);
            }
            return null;
        }

        private static void LoadThemeVariables(string sourcePath, VariableResolver variableResolver)
        {
            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            foreach (var themesPath in new[]
            {
                Path.Combine(projectRoot, "Assets", "UI", "Themes"),
                Path.Combine(projectRoot, "Assets", "Resources", "UI", "Themes")
            })
            {
                if (!Directory.Exists(themesPath)) continue;
                foreach (var themeDir in Directory.GetDirectories(themesPath))
                {
                    var themeFile = Path.Combine(themeDir, Path.GetFileName(themeDir) + ".uss");
                    if (!File.Exists(themeFile))
                    {
                        var anyUss = Directory.GetFiles(themeDir, "*.uss");
                        if (anyUss.Length > 0) themeFile = anyUss[0];
                    }
                    if (File.Exists(themeFile))
                    {
                        var uss = File.ReadAllText(themeFile);
                        var parseResult = USSParser.Parse(uss, themeFile);
                        foreach (var v in parseResult.Variables)
                            variableResolver.SetVariable(v.Name, v.Value);
                        return;
                    }
                }
            }
        }
    }
}
