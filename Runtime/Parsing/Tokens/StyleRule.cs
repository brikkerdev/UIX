using System.Collections.Generic;

namespace UIX.Parsing.Tokens
{
    /// <summary>
    /// A CSS rule: selector { property: value; }
    /// </summary>
    public class StyleRule
    {
        public StyleSelector Selector { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public int LineNumber { get; set; }
    }
}
