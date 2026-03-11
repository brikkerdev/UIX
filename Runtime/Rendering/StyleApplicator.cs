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

            if (styles.TryGetValue("background-color", out var bg) || styles.TryGetValue("tint", out bg))
            {
                if (ColorParser.TryParse(bg, out var c))
                    image.color = c;
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
