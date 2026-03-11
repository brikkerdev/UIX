using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Wraps content that is conditionally rendered based on if="{expression}".
    /// </summary>
    public class ConditionalNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Conditional;

        public string ConditionExpression { get; set; }
        public List<UIXNode> Children { get; set; } = new List<UIXNode>();
    }
}
