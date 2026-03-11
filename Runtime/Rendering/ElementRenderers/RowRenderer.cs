using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    public class RowRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject(node.TagName);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 0);

            var hlg = go.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            if (context?.ResolvedStyles != null)
            {
                LayoutMapper.ApplyToLayoutGroup(hlg, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
                StyleApplicator.AddBackgroundIfNeeded(go, context.ResolvedStyles);
            }

            return go;
        }
    }
}
