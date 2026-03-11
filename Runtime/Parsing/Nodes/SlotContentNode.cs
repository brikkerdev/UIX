using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Wraps content destined for a named slot (slot-content name="header").
    /// </summary>
    public class SlotContentNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.SlotContent;

        public string SlotName { get; set; }
        public List<UIXNode> Children { get; set; } = new List<UIXNode>();
    }
}
