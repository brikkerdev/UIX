using System.Collections.Generic;

namespace UIX.Parsing.Nodes
{
    /// <summary>
    /// Base class for all UIX AST nodes.
    /// </summary>
    public abstract class UIXNode
    {
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string SourcePath { get; set; }

        public abstract UIXNodeType NodeType { get; }
    }

    public enum UIXNodeType
    {
        Element,
        Text,
        Component,
        Slot,
        SlotContent,
        Conditional,
        Foreach,
        Root
    }
}
