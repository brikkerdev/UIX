using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Represents a custom component usage (e.g. &lt;Button /&gt;, &lt;PlayerCard /&gt;).
    /// </summary>
    public class ComponentNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Component;

        public string ComponentName { get; set; }
        public Dictionary<string, string> Props { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, List<UIXNode>> Slots { get; set; } = new Dictionary<string, List<UIXNode>>();
        public List<UIXNode> DefaultSlotContent { get; set; } = new List<UIXNode>();
    }
}
