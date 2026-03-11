using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    public class SliderRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject("slider");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 20);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f);

            var slider = go.AddComponent<Slider>();
            slider.fillRect = CreateFillRect(go.transform);
            slider.targetGraphic = image;

            if (float.TryParse(node.GetAttribute("min"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var min))
                slider.minValue = min;
            if (float.TryParse(node.GetAttribute("max"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var max))
                slider.maxValue = max;

            if (context?.ResolvedStyles != null)
            {
                StyleApplicator.ApplyToImage(image, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }

        private static RectTransform CreateFillRect(Transform parent)
        {
            var fill = new GameObject("Fill");
            fill.transform.SetParent(parent, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            fill.AddComponent<Image>().color = new Color(0.2f, 0.6f, 1f);
            return fillRect;
        }
    }
}
