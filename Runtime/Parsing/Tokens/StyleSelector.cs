using System.Collections.Generic;

namespace UIX.Parsing.Tokens
{
    /// <summary>
    /// CSS-like selector (e.g. .button, #main, text:hover).
    /// </summary>
    public class StyleSelector
    {
        public string ElementType { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public string Id { get; set; }
        public List<string> PseudoClasses { get; set; } = new List<string>();
        public StyleSelector Parent { get; set; }
        public bool DirectChild { get; set; }

        public int Specificity
        {
            get
            {
                int a = string.IsNullOrEmpty(Id) ? 0 : 1;
                int b = Classes.Count + PseudoClasses.Count;
                int c = string.IsNullOrEmpty(ElementType) ? 0 : 1;
                return a * 100 + b * 10 + c;
            }
        }

        public override string ToString()
        {
            var parts = new List<string>();
            if (Parent != null)
            {
                parts.Add(Parent.ToString());
                parts.Add(DirectChild ? " > " : " ");
            }
            if (!string.IsNullOrEmpty(ElementType))
                parts.Add(ElementType);
            foreach (var c in Classes)
                parts.Add("." + c);
            if (!string.IsNullOrEmpty(Id))
                parts.Add("#" + Id);
            foreach (var p in PseudoClasses)
                parts.Add(":" + p);
            return string.Join("", parts);
        }
    }
}
