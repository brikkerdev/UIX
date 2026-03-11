using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    public class ToggleRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject("toggle");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 24);

            var image = go.AddComponent<Image>();
            image.color = Color.white;

            go.AddComponent<Toggle>().targetGraphic = image;

            if (context?.ResolvedStyles != null)
            {
                StyleApplicator.ApplyToElement(go, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
