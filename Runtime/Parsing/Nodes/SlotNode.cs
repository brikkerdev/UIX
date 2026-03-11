namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Represents a slot placeholder in a component template where content can be inserted.
    /// </summary>
    public class SlotNode : UIXNode
    {
        public override UIXNodeType NodeType => UIXNodeType.Slot;

        public string SlotName { get; set; }  // null = default slot
    }
}
