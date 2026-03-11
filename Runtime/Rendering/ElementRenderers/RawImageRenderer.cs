using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;

namespace UIX.Rendering.ElementRenderers
{
    /// <summary>
    /// Renders a raw-image element for render textures.
    /// </summary>
    public class RawImageRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject("raw-image");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(100, 100);

            var rawImage = go.AddComponent<RawImage>();
            rawImage.color = Color.white;

            var textureAttr = node.GetAttribute("texture");
            if (!string.IsNullOrEmpty(textureAttr))
            {
                var path = textureAttr.Trim(' ', '{', '}');
                if (!path.StartsWith("theme:"))
                {
                    var texture = Resources.Load<Texture>(path);
                    if (texture != null)
                        rawImage.texture = texture;
                }
            }

            if (context?.ResolvedStyles != null)
            {
                StyleApplicator.ApplyToRawImage(rawImage, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
