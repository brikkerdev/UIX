using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    public class ButtonRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject(!string.IsNullOrEmpty(node.Id) ? node.Id : "button");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(160, 30);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.5f, 1f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            if (context?.ResolvedStyles != null)
            {
                StyleApplicator.ApplyToElement(go, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
