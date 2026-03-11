using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    public class InputRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject("input");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 30);

            var image = go.AddComponent<Image>();
            image.color = Color.white;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 6);
            textRect.offsetMax = new Vector2(-10, -6);
            var textComp = textGo.AddComponent<TextMeshProUGUI>();

            var input = go.AddComponent<TMP_InputField>();
            input.textViewport = rect;
            input.textComponent = textComp;

            if (context?.ResolvedStyles != null)
            {
                StyleApplicator.ApplyToElement(go, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
