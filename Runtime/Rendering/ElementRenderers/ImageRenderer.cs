using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    public class ImageRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject("image");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(100, 100);

            var image = go.AddComponent<Image>();
            image.color = Color.white;

            var spriteAttr = node.GetAttribute("sprite");
            if (!string.IsNullOrEmpty(spriteAttr))
            {
                var path = spriteAttr.Trim(' ', '{', '}');
                if (!path.StartsWith("theme:"))
                {
                    var sprite = Resources.Load<Sprite>(path);
                    if (sprite != null)
                        image.sprite = sprite;
                }
            }

            if (context?.ResolvedStyles != null)
            {
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
                StyleApplicator.ApplyToElement(go, context.ResolvedStyles);
            }

            return go;
        }
    }
}
