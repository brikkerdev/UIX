using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    /// <summary>
    /// Renders a mask element - uses RectMask2D for clipping child content.
    /// </summary>
    public class MaskRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject(node.TagName);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(200, 200);

            go.AddComponent<RectMask2D>();

            if (context?.ResolvedStyles != null)
            {
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
