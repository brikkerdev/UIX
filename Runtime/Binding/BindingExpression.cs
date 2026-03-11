using System;
using System.Text.RegularExpressions;

namespace UIX.Binding
{
    /// <summary>
    /// Parses and represents a binding expression like {PropertyName} or {=PropertyName}.
    /// </summary>
    public class BindingExpression
    {
        private static readonly Regex ExprRegex = new Regex(@"^\s*=\s*(.+)$", RegexOptions.Compiled);

        public string Expression { get; }
        public bool IsTwoWay { get; }

        public BindingExpression(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                Expression = "";
                IsTwoWay = false;
                return;
            }

            raw = raw.Trim();
            var twoWayMatch = ExprRegex.Match(raw);
            if (twoWayMatch.Success)
            {
                IsTwoWay = true;
                Expression = twoWayMatch.Groups[1].Value.Trim();
            }
            else
            {
                IsTwoWay = false;
                Expression = raw;
            }
        }

        public static BindingExpression Parse(string raw)
        {
            return new BindingExpression(raw);
        }
    }
}
