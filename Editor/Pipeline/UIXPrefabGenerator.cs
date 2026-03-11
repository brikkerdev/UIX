using UnityEngine;
using UnityEditor;
using UIX.Parsing;
using UIX.Parsing.Nodes;
using UIX.Rendering;
using UIX.Styling;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Generates uGUI prefabs from UIX templates.
    /// </summary>
    public static class UIXPrefabGenerator
    {
        public static GameObject Generate(UIX.Templates.UIXTemplate template)
        {
            if (template == null) return null;

            var fullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), template.SourcePath);
            var xml = System.IO.File.Exists(fullPath) ? System.IO.File.ReadAllText(fullPath) : "";
            var root = XMLParser.Parse(xml, template.SourcePath);
            if (root == null) return null;

            var variableResolver = new VariableResolver();
            var styleResolver = new StyleResolver(variableResolver);

            if (!string.IsNullOrEmpty(template.SourcePath))
            {
                var ussPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), template.SourcePath.Replace(".xml", ".uss"));
                if (System.IO.File.Exists(ussPath))
                {
                    var uss = System.IO.File.ReadAllText(ussPath);
                    var parseResult = USSParser.Parse(uss, ussPath);
                    styleResolver.AddRules(parseResult.Rules);
                }
            }

            var renderer = new UIXRenderer(styleResolver, variableResolver);
            var rootGo = new GameObject(template.TemplateName);
            var rect = rootGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var child = renderer.Render(root, rootGo.transform);
            if (child != null && child.transform.parent != rootGo.transform)
                child.transform.SetParent(rootGo.transform, false);

            return rootGo;
        }
    }
}
