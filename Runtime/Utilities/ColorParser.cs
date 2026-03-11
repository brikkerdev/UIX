using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UIX.Utilities
{
    /// <summary>
    /// Parses CSS color values to Unity Color.
    /// </summary>
    public static class ColorParser
    {
        private static readonly Regex HexRegex = new Regex(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$", RegexOptions.Compiled);
        private static readonly Regex RgbRegex = new Regex(@"rgb\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.Compiled);
        private static readonly Regex RgbaRegex = new Regex(@"rgba\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*([\d.]+)\s*\)", RegexOptions.Compiled);

        private static readonly System.Collections.Generic.Dictionary<string, Color> NamedColors = new System.Collections.Generic.Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            ["black"] = Color.black,
            ["white"] = Color.white,
            ["red"] = Color.red,
            ["green"] = Color.green,
            ["blue"] = Color.blue,
            ["yellow"] = Color.yellow,
            ["cyan"] = Color.cyan,
            ["magenta"] = Color.magenta,
            ["gray"] = Color.gray,
            ["grey"] = Color.gray,
            ["transparent"] = new Color(0, 0, 0, 0)
        };

        public static bool TryParse(string value, out Color color)
        {
            color = Color.white;
            if (string.IsNullOrWhiteSpace(value)) return false;

            value = value.Trim();

            if (NamedColors.TryGetValue(value, out color))
                return true;

            var hexMatch = HexRegex.Match(value);
            if (hexMatch.Success)
            {
                var hex = hexMatch.Groups[1].Value;
                if (hex.Length == 3)
                {
                    var r = ParseHex2(hex[0].ToString() + hex[0]);
                    var g = ParseHex2(hex[1].ToString() + hex[1]);
                    var b = ParseHex2(hex[2].ToString() + hex[2]);
                    color = new Color(r / 255f, g / 255f, b / 255f, 1f);
                }
                else if (hex.Length == 6)
                {
                    var r = ParseHex2(hex.Substring(0, 2));
                    var g = ParseHex2(hex.Substring(2, 2));
                    var b = ParseHex2(hex.Substring(4, 2));
                    color = new Color(r / 255f, g / 255f, b / 255f, 1f);
                }
                else
                {
                    var r = ParseHex2(hex.Substring(0, 2));
                    var g = ParseHex2(hex.Substring(2, 2));
                    var b = ParseHex2(hex.Substring(4, 2));
                    var a = ParseHex2(hex.Substring(6, 2));
                    color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                }
                return true;
            }

            var rgbMatch = RgbRegex.Match(value);
            if (rgbMatch.Success)
            {
                var r = int.Parse(rgbMatch.Groups[1].Value);
                var g = int.Parse(rgbMatch.Groups[2].Value);
                var b = int.Parse(rgbMatch.Groups[3].Value);
                color = new Color(r / 255f, g / 255f, b / 255f, 1f);
                return true;
            }

            var rgbaMatch = RgbaRegex.Match(value);
            if (rgbaMatch.Success)
            {
                var r = int.Parse(rgbaMatch.Groups[1].Value);
                var g = int.Parse(rgbaMatch.Groups[2].Value);
                var b = int.Parse(rgbaMatch.Groups[3].Value);
                var a = float.Parse(rgbaMatch.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture);
                color = new Color(r / 255f, g / 255f, b / 255f, a);
                return true;
            }

            return false;
        }

        public static Color Parse(string value)
        {
            return TryParse(value, out var c) ? c : Color.white;
        }

        private static int ParseHex2(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }
    }
}
