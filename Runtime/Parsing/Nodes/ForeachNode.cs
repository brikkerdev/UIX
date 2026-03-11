using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Represents a foreach loop over a collection.
    /// </summary>
    public class ForeachNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Foreach;

        public string ItemsExpression { get; set; }
        public string VarName { get; set; }
        public string IndexName { get; set; }
        public List<UIXNode> Children { get; set; } = new List<UIXNode>();
    }
}
