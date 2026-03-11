namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Represents static or dynamic text content.
    /// </summary>
    public class TextNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Text;

        public string Content { get; set; }
        public bool IsBinding => Content != null && Content.StartsWith("{") && Content.EndsWith("}");
    }
}
