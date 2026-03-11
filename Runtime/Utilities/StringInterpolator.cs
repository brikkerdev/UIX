using System;
using System.Text.RegularExpressions;

namespace UIX.Utilities
{
    /// <summary>
    /// Interpolates {expression} placeholders in strings.
    /// </summary>
    public static class StringInterpolator
    {
        private static readonly Regex BindingRegex = new Regex(@"\{([^}]+)\}", RegexOptions.Compiled);

        public static bool HasBindings(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Contains("{") && value.Contains("}");
        }

        public static string[] ExtractBindings(string value)
        {
            var matches = BindingRegex.Matches(value);
            var result = new string[matches.Count];
            for (var i = 0; i < matches.Count; i++)
                result[i] = matches[i].Groups[1].Value.Trim();
            return result;
        }

        public static string Interpolate(string template, Func<string, object> valueResolver)
        {
            if (string.IsNullOrEmpty(template)) return template;
            return BindingRegex.Replace(template, m =>
            {
                var expr = m.Groups[1].Value.Trim();
                var isTwoWay = expr.StartsWith("=");
                var key = isTwoWay ? expr.Substring(1).Trim() : expr;
                var value = valueResolver(key);
                return value?.ToString() ?? "";
            });
        }

        public static bool IsTwoWayBinding(string expression)
        {
            return !string.IsNullOrEmpty(expression) && expression.TrimStart().StartsWith("=");
        }

        public static string GetOneWayExpression(string expression)
        {
            var trimmed = expression?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return trimmed;
            return trimmed.StartsWith("=") ? trimmed.Substring(1).Trim() : trimmed;
        }
    }
}
