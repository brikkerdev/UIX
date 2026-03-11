using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Represents a built-in XML element (column, row, text, image, button, etc.).
    /// </summary>
    public class ElementNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Element;

        public string TagName { get; set; }
        public string Id { get; set; }
        public string Class { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<UIXNode> Children { get; set; } = new List<UIXNode>();

        public string GetAttribute(string name)
        {
            return Attributes.TryGetValue(name, out var value) ? value : null;
        }
    }
}
