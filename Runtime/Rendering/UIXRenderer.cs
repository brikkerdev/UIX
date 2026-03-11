using System;
using System.Collections.Generic;
using UnityEngine;
using UIX.Parsing;
using UIX.Parsing.Nodes;
using UIX.Rendering.ElementRenderers;
using UIX.Styling;

namespace UIX.Rendering
{
    /// <summary>
    /// Renders UIX template tree to uGUI hierarchy.
    /// </summary>
    public class UIXRenderer
    {
        private readonly Dictionary<string, IElementRenderer> _renderers = new Dictionary<string, IElementRenderer>(System.StringComparer.OrdinalIgnoreCase);
        private readonly StyleResolver _styleResolver;
        private readonly VariableResolver _variableResolver;

        public UIXRenderer(StyleResolver styleResolver, VariableResolver variableResolver)
        {
            _styleResolver = styleResolver;
            _variableResolver = variableResolver;

            RegisterDefaultRenderers();
        }

        private void RegisterDefaultRenderers()
        {
            Register("column", new ColumnRenderer());
            Register("row", new RowRenderer());
            Register("stack", new StackRenderer());
            Register("grid", new GridRenderer());
            Register("scroll", new ScrollRenderer());
            Register("text", new TextRenderer());
            Register("image", new ImageRenderer());
            Register("button", new ButtonRenderer());
            Register("toggle", new ToggleRenderer());
            Register("slider", new SliderRenderer());
            Register("input", new InputRenderer());
            Register("dropdown", new DropdownRenderer());
            Register("container", new ContainerRenderer());
            Register("canvas-group", new CanvasGroupRenderer());
        }

        public void Register(string tagName, IElementRenderer renderer)
        {
            _renderers[tagName] = renderer;
        }

        public GameObject Render(RootNode root, Transform parent, object viewModel = null, System.Func<string, object> evaluateBinding = null)
        {
            return RenderNodes(root.Children, parent, root.Name, viewModel, evaluateBinding);
        }

        public GameObject RenderNodes(List<UIXNode> nodes, Transform parent, string scope = null, object viewModel = null, System.Func<string, object> evaluateBinding = null)
        {
            GameObject firstRoot = null;
            foreach (var node in nodes)
            {
                var go = RenderNode(node, parent, scope, viewModel, evaluateBinding);
                if (firstRoot == null && go != null)
                    firstRoot = go;
            }
            return firstRoot ?? parent?.gameObject;
        }

        private GameObject RenderNode(UIXNode node, Transform parent, string scope, object viewModel, System.Func<string, object> evaluateBinding)
        {
            var context = new RenderContext
            {
                ViewModel = viewModel,
                EvaluateBinding = evaluateBinding,
                ComponentScope = scope
            };

            var classes = new List<string>();
            var elementType = "";
            var id = "";

            if (node is ElementNode elem)
            {
                elementType = elem.TagName;
                id = elem.Id ?? "";
                if (!string.IsNullOrEmpty(elem.Class))
                    classes.AddRange(elem.Class.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                context.ResolvedStyles = _styleResolver.Resolve(elementType, id, classes);

                if (_renderers.TryGetValue(elementType, out var renderer))
                {
                    var go = renderer.Render(elem, parent, context);
                if (go != null && elem.Children.Count > 0)
                {
                    var childParent = elementType == "scroll" ? go.transform.Find("Viewport/Content") : go.transform;
                    if (childParent != null)
                        RenderNodes(elem.Children, childParent, scope, viewModel, evaluateBinding);
                }
                    return go;
                }
            }

            if (node is ComponentNode comp)
            {
                return RenderComponent(comp, parent, scope, viewModel, evaluateBinding);
            }

            if (node is SlotNode slot)
            {
                return null;
            }

            if (node is SlotContentNode slotContent)
            {
                RenderNodes(slotContent.Children, parent, scope, viewModel, evaluateBinding);
                return null;
            }

            if (node is ConditionalNode cond)
            {
                if (evaluateBinding != null && evaluateBinding(cond.ConditionExpression) is bool b && b)
                    RenderNodes(cond.Children, parent, scope, viewModel, evaluateBinding);
                return null;
            }

            if (node is ForeachNode fe)
            {
                return RenderForeach(fe, parent, scope, viewModel, evaluateBinding);
            }

            return null;
        }

        private GameObject RenderComponent(ComponentNode comp, Transform parent, string scope, object viewModel, System.Func<string, object> evaluateBinding)
        {
            return null;
        }

        private GameObject RenderForeach(ForeachNode fe, Transform parent, string scope, object viewModel, System.Func<string, object> evaluateBinding)
        {
            return null;
        }
    }
}
