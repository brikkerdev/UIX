using UnityEngine;
using TMPro;
using UIX.Parsing.Nodes;
using UIX.Utilities;

namespace UIX.Rendering.ElementRenderers
{
    public class TextRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject("text");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 24);

            var text = go.AddComponent<TextMeshProUGUI>();
            var textAttr = node.GetAttribute("text");
            var content = "";
            if (!string.IsNullOrEmpty(textAttr))
            {
                if (StringInterpolator.HasBindings(textAttr) && context?.EvaluateBinding != null)
                    content = StringInterpolator.Interpolate(textAttr, k => context.EvaluateBinding(k));
                else
                    content = textAttr.Trim(' ', '{', '}');
            }
            text.text = content;

            if (context?.ResolvedStyles != null)
            {
                StyleApplicator.ApplyToText(text, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
