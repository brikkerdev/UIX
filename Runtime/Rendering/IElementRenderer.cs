using System.Collections.Generic;
using UnityEngine;
using UIX.Parsing.Nodes;

namespace UIX.Rendering
{
    /// <summary>
    /// Renders a UIX element node to uGUI.
    /// </summary>
    public interface IElementRenderer
    {
        GameObject Render(ElementNode node, Transform parent, RenderContext context);
    }

    public class RenderContext
    {
        public Dictionary<string, string> ResolvedStyles { get; set; }
        public object ViewModel { get; set; }
        public System.Func<string, object> EvaluateBinding { get; set; }
        public string ComponentScope { get; set; }
    }
}
