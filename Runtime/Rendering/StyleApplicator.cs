using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UIX.Utilities;

namespace UIX.Rendering
{
    /// <summary>
    /// Applies resolved CSS styles to uGUI components.
    /// </summary>
    public static class StyleApplicator
    {
        public static void ApplyToImage(Image image, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || image == null) return;

            var rect = image.rectTransform;
            var rectSize = rect.rect.size;
            if (rectSize.x <= 0 || rectSize.y <= 0) rectSize = new Vector2(100, 100);

            // 1. Custom material (highest priority)
            Material customMat = null;
            if (styles.TryGetValue("material", out var matPath) && !string.IsNullOrEmpty(matPath))
            {
                customMat = UIXMaterialRegistry.GetCustomMaterial(matPath.Trim());
            }

            // 2. background-image
            if (styles.TryGetValue("background-image", out var bgImage) && !string.IsNullOrEmpty(bgImage))
            {
                var path = bgImage.Trim().Trim('"', '\'');
                if (!path.StartsWith("theme:"))
                {
                    var sprite = Resources.Load<Sprite>(path);
                    if (sprite != null)
                        image.sprite = sprite;
                }
            }

            // 3. background-size
            if (styles.TryGetValue("background-size", out var bgSize))
            {
                switch (bgSize.Trim().ToLowerInvariant())
                {
                    case "cover":
                        image.type = Image.Type.Sliced;
                        break;
                    case "contain":
                        image.preserveAspect = true;
                        image.type = Image.Type.Simple;
                        break;
                    default:
                        image.type = Image.Type.Simple;
                        break;
                }
            }

            // 4. background-color / tint
            if (styles.TryGetValue("background-color", out var bg) || styles.TryGetValue("tint", out bg))
            {
                if (ColorParser.TryParse(bg, out var c))
                    image.color = c;
            }

            // 5. border-radius, border, material selection
            float radius = ParseBorderRadius(styles);
            float borderWidth = ParseFloat(styles, "border-width", 0f);
            Color borderColor = Color.black;
            if (styles.TryGetValue("border-color", out var bc) && ColorParser.TryParse(bc, out var bcParsed))
                borderColor = bcParsed;

            if (customMat != null)
            {
                image.material = customMat;
            }
            else if (radius > 0)
            {
                var normRadius = UIXMaterialRegistry.PixelRadiusToNormalized(radius, rectSize);
                Material mat;
                if (image.sprite != null)
                {
                    mat = UIXMaterialRegistry.GetRoundedImageMaterial(normRadius);
                }
                else
                {
                    mat = UIXMaterialRegistry.GetRoundedMaterial(normRadius);
                }
                if (mat != null)
                {
                    mat.SetFloat("_Radius", normRadius);
                    if (borderWidth > 0)
                    {
                        mat.SetFloat("_BorderWidth", borderWidth / Mathf.Min(rectSize.x, rectSize.y));
                        mat.SetColor("_BorderColor", borderColor);
                    }
                    image.material = mat;
                }
            }
            else if (image.sprite == null)
            {
                var solidMat = UIXMaterialRegistry.GetMaterial(UIXMaterialRegistry.MaterialType.Solid);
                if (solidMat != null)
                    image.material = solidMat;
            }
        }

        public static void ApplyToRawImage(RawImage rawImage, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || rawImage == null) return;

            if (styles.TryGetValue("background-color", out var bg) || styles.TryGetValue("tint", out bg))
            {
                if (ColorParser.TryParse(bg, out var c))
                    rawImage.color = c;
            }
            if (styles.TryGetValue("material", out var matPath) && !string.IsNullOrEmpty(matPath))
            {
                var mat = UIXMaterialRegistry.GetCustomMaterial(matPath.Trim());
                if (mat != null) rawImage.material = mat;
            }
        }

        public static void ApplyToText(TMP_Text text, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || text == null) return;

            if (styles.TryGetValue("color", out var color))
            {
                if (ColorParser.TryParse(color, out var c))
                    text.color = c;
            }
            if (styles.TryGetValue("font-size", out var size))
            {
                if (float.TryParse(size, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var fs))
                    text.fontSize = fs;
            }
            if (styles.TryGetValue("text-align", out var align))
            {
                text.alignment = ParseTextAlignment(align);
            }
        }

        public static void ApplyToCanvasGroup(CanvasGroup cg, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || cg == null) return;

            if (styles.TryGetValue("opacity", out var opacity))
            {
                if (float.TryParse(opacity, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var a))
                    cg.alpha = Mathf.Clamp01(a);
            }
        }

        public static void ApplyToRectTransform(RectTransform rect, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || rect == null) return;

            if (styles.TryGetValue("scale", out var scale))
            {
                if (float.TryParse(scale, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var s))
                    rect.localScale = Vector3.one * s;
            }
            if (styles.TryGetValue("rotation", out var rot))
            {
                if (float.TryParse(rot, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var r))
                    rect.localEulerAngles = new Vector3(0, 0, r);
            }
        }

        /// <summary>
        /// Reapplies styles to a GameObject (used when pseudo-classes change).
        /// </summary>
        public static void ReapplyStyles(GameObject go, IReadOnlyDictionary<string, string> styles)
        {
            ApplyToElement(go, styles);
        }

        /// <summary>
        /// Adds a background Image as first child if the element has background-color or background-image.
        /// </summary>
        public static void AddBackgroundIfNeeded(GameObject go, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null) return;
            if (!styles.ContainsKey("background-color") && !styles.ContainsKey("background-image")) return;

            var bgGo = new GameObject("background");
            bgGo.transform.SetParent(go.transform, false);
            bgGo.transform.SetAsFirstSibling();

            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var image = bgGo.AddComponent<Image>();
            image.color = Color.white;
            image.raycastTarget = false;

            ApplyToElement(bgGo, styles);
        }

        /// <summary>
        /// Applies styles to a GameObject and creates box-shadow child if needed.
        /// </summary>
        public static void ApplyToElement(GameObject go, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || go == null) return;

            var image = go.GetComponent<Image>();
            if (image != null) ApplyToImage(image, styles);

            var rawImage = go.GetComponent<RawImage>();
            if (rawImage != null) ApplyToRawImage(rawImage, styles);

            var text = go.GetComponent<TMP_Text>();
            if (text != null) ApplyToText(text, styles);

            var cg = go.GetComponent<CanvasGroup>();
            if (cg != null) ApplyToCanvasGroup(cg, styles);

            var rect = go.GetComponent<RectTransform>();
            if (rect != null) ApplyToRectTransform(rect, styles);

            // box-shadow: create child shadow element (only if not already present)
            if (styles.TryGetValue("box-shadow", out var boxShadow) && !string.IsNullOrEmpty(boxShadow))
            {
                var existing = go.transform.Find("box-shadow");
                if (existing == null)
                    CreateBoxShadow(go, boxShadow);
            }
        }

        private static void CreateBoxShadow(GameObject parent, string boxShadowValue)
        {
            // Parse simple format: "offsetX offsetY blur color" or "color"
            var parts = boxShadowValue.Trim().Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            float offsetX = 0, offsetY = 0, blur = 4;
            Color shadowColor = new Color(0, 0, 0, 0.5f);

            if (parts.Length >= 4)
            {
                float.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out offsetX);
                float.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out offsetY);
                float.TryParse(parts[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out blur);
                ColorParser.TryParse(parts[3], out shadowColor);
            }
            else if (parts.Length == 1)
            {
                ColorParser.TryParse(parts[0], out shadowColor);
            }

            var shadowGo = new GameObject("box-shadow");
            shadowGo.transform.SetParent(parent.transform, false);
            shadowGo.transform.SetAsFirstSibling();

            var shadowRect = shadowGo.AddComponent<RectTransform>();
            shadowRect.anchorMin = Vector2.zero;
            shadowRect.anchorMax = Vector2.one;
            shadowRect.offsetMin = new Vector2(-blur + offsetX, -blur + offsetY);
            shadowRect.offsetMax = new Vector2(blur + offsetX, blur + offsetY);

            var shadowImage = shadowGo.AddComponent<Image>();
            shadowImage.color = Color.white;
            shadowImage.raycastTarget = false;

            var mat = UIXMaterialRegistry.GetMaterial(UIXMaterialRegistry.MaterialType.Shadow);
            if (mat != null)
            {
                var shadowMat = new Material(mat);
                shadowMat.SetColor("_Color", shadowColor);
                shadowMat.SetFloat("_Blur", blur / 200f);
                shadowMat.SetFloat("_Radius", 0.1f);
                shadowImage.material = shadowMat;
            }
        }

        private static float ParseBorderRadius(IReadOnlyDictionary<string, string> styles)
        {
            if (!styles.TryGetValue("border-radius", out var val) || string.IsNullOrEmpty(val)) return 0f;
            var parts = val.Trim().Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return 0f;
            float maxR = 0f;
            foreach (var p in parts)
            {
                if (float.TryParse(p, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var r))
                    maxR = Mathf.Max(maxR, r);
            }
            return maxR;
        }

        private static float ParseFloat(IReadOnlyDictionary<string, string> styles, string key, float defaultValue)
        {
            if (!styles.TryGetValue(key, out var val) || string.IsNullOrEmpty(val)) return defaultValue;
            return float.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : defaultValue;
        }

        private static TextAlignmentOptions ParseTextAlignment(string value)
        {
            if (string.IsNullOrEmpty(value)) return TextAlignmentOptions.Left;
            switch (value.ToLowerInvariant())
            {
                case "left": return TextAlignmentOptions.Left;
                case "center": return TextAlignmentOptions.Center;
                case "right": return TextAlignmentOptions.Right;
                default: return TextAlignmentOptions.Left;
            }
        }
    }
}
