using System;
using System.Globalization;

namespace UIX.Utilities
{
    /// <summary>
    /// Converts CSS-like units (px, %, auto) to layout values.
    /// </summary>
    public static class UnitConverter
    {
        public const float Auto = -1f;

        public static bool TryParseLength(string value, out float result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(value)) return false;

            value = value.Trim();

            if (string.Equals(value, "auto", StringComparison.OrdinalIgnoreCase))
            {
                result = Auto;
                return true;
            }

            if (value.EndsWith("%"))
            {
                if (float.TryParse(value.Substring(0, value.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out var pct))
                {
                    result = -pct;
                    return true;
                }
            }

            if (value.EndsWith("px"))
            {
                if (float.TryParse(value.Substring(0, value.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out var px))
                {
                    result = px;
                    return true;
                }
            }

            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var num))
            {
                result = num;
                return true;
            }

            return false;
        }

        public static float ParseLength(string value, float defaultValue = 0)
        {
            return TryParseLength(value, out var r) ? r : defaultValue;
        }

        public static bool IsPercent(float value)
        {
            return value < 0 && value != Auto;
        }

        public static bool IsAuto(float value)
        {
            return value == Auto;
        }

        public static float GetPercent(float value)
        {
            return IsPercent(value) ? -value : 0;
        }
    }
}
