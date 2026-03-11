using System.Collections.Generic;

namespace UIX.Styling
{
    /// <summary>
    /// Registry of supported CSS properties and their uGUI mapping.
    /// </summary>
    public static class CSSProperties
    {
        public static readonly HashSet<string> SupportedProperties = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "width", "height", "min-width", "min-height", "max-width", "max-height",
            "flex", "padding", "padding-left", "padding-right", "padding-top", "padding-bottom",
            "gap", "align-items", "justify-content", "flex-direction", "overflow",
            "background-color", "background-image", "background-size", "opacity",
            "border-radius", "border", "border-color", "border-width", "tint",
            "material", "box-shadow",
            "scale", "rotation",
            "font-family", "font-size", "font-weight", "font-style", "color",
            "text-align", "text-overflow", "line-height", "letter-spacing", "white-space",
            "transition", "transition-property", "transition-duration", "transition-easing"
        };

        public static bool IsSupported(string propertyName)
        {
            return SupportedProperties.Contains(propertyName);
        }
    }
}
