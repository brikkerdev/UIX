using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Root node for component or screen templates.
    /// </summary>
    public class RootNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Root;

        public string Name { get; set; }
        public bool IsComponent { get; set; }
        public string ViewModelType { get; set; }
        public List<PropDefinition> Props { get; set; } = new List<PropDefinition>();
        public List<SlotDefinition> Slots { get; set; } = new List<SlotDefinition>();
        public List<UIXNode> Children { get; set; } = new List<UIXNode>();
    }

    public class PropDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }
        public bool Optional { get; set; }
    }

    public class SlotDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
