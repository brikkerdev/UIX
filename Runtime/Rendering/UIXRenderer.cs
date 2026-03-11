using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UIX.Components;
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
        private readonly ComponentResolver _componentResolver;

        public UIXRenderer(StyleResolver styleResolver, VariableResolver variableResolver, ComponentResolver componentResolver = null)
        {
            _styleResolver = styleResolver;
            _variableResolver = variableResolver;
            _componentResolver = componentResolver;

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
            Register("mask", new MaskRenderer());
            Register("raw-image", new RawImageRenderer());
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
                    if (go != null)
                    {
                        ApplyVisibleEnabled(go, elem, evaluateBinding);
                        if (elem.Children.Count > 0)
                        {
                            var childParent = elementType == "scroll" ? go.transform.Find("Viewport/Content") : go.transform;
                            if (childParent != null)
                                RenderNodes(elem.Children, childParent, scope, viewModel, evaluateBinding);
                        }
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
            if (_componentResolver == null) return null;

            var go = _componentResolver.Instantiate(comp.ComponentName, parent);
            if (go == null) return null;

            if (comp.DefaultSlotContent.Count > 0)
                RenderNodes(comp.DefaultSlotContent, go.transform, scope ?? comp.ComponentName, viewModel, evaluateBinding);
            foreach (var kv in comp.Slots)
            {
                var slotMarker = go.transform.Find("Slot_" + kv.Key) ?? go.transform;
                RenderNodes(kv.Value, slotMarker, scope ?? comp.ComponentName, viewModel, evaluateBinding);
            }

            return go;
        }

        private GameObject RenderForeach(ForeachNode fe, Transform parent, string scope, object viewModel, System.Func<string, object> evaluateBinding)
        {
            if (evaluateBinding == null || string.IsNullOrEmpty(fe.ItemsExpression)) return null;

            var items = evaluateBinding(fe.ItemsExpression);
            if (items == null) return null;

            var list = items as IList;
            if (list == null && items is IEnumerable en)
            {
                var temp = new List<object>();
                foreach (var item in en) temp.Add(item);
                list = temp;
            }
            if (list == null) return null;

            var varName = fe.VarName ?? "item";
            var indexName = fe.IndexName;
            GameObject firstChild = null;

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var loopEvaluate = CreateLoopEvaluateBinding(varName, indexName, item, i, evaluateBinding);
                var child = RenderNodes(fe.Children, parent, scope, viewModel, loopEvaluate);
                if (firstChild == null && child != null)
                    firstChild = child;
            }

            return firstChild;
        }

        private static System.Func<string, object> CreateLoopEvaluateBinding(string varName, string indexName, object item, int index, System.Func<string, object> parentEvaluate)
        {
            return expr =>
            {
                if (string.IsNullOrEmpty(expr)) return null;
                var trimmed = expr.Trim();
                if (trimmed == varName)
                    return item;
                if (trimmed.StartsWith(varName + "."))
                {
                    var path = trimmed.Substring(varName.Length + 1);
                    return GetNestedProperty(item, path);
                }
                if (!string.IsNullOrEmpty(indexName) && trimmed == indexName)
                    return index;
                return parentEvaluate?.Invoke(expr);
            };
        }

        private static void ApplyVisibleEnabled(GameObject go, ElementNode elem, System.Func<string, object> evaluateBinding)
        {
            var visibleAttr = elem.GetAttribute("visible");
            if (!string.IsNullOrEmpty(visibleAttr) && evaluateBinding != null)
            {
                var expr = visibleAttr.Trim(' ', '{', '}');
                var val = evaluateBinding(expr);
                var visible = val is bool b ? b : (val != null && !string.IsNullOrEmpty(val?.ToString()));
                go.SetActive(visible);
            }

            var enabledAttr = elem.GetAttribute("enabled");
            if (!string.IsNullOrEmpty(enabledAttr) && evaluateBinding != null)
            {
                var expr = enabledAttr.Trim(' ', '{', '}');
                var val = evaluateBinding(expr);
                var enabled = val is bool b ? b : (val != null && !string.IsNullOrEmpty(val?.ToString()));
                var cg = go.GetComponent<UnityEngine.CanvasGroup>();
                if (cg == null) cg = go.AddComponent<UnityEngine.CanvasGroup>();
                cg.interactable = enabled;
                cg.blocksRaycasts = enabled;
            }
        }

        private static object GetNestedProperty(object obj, string path)
        {
            if (obj == null || string.IsNullOrEmpty(path)) return null;
            var parts = path.Split('.');
            var current = obj;
            foreach (var part in parts)
            {
                if (current == null) return null;
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                var type = current.GetType();
                var prop = type.GetProperty(trimmed, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    current = prop.GetValue(current);
                    continue;
                }
                var field = type.GetField(trimmed, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (field != null)
                    current = field.GetValue(current);
                else
                    return null;
            }
            return current;
        }
    }
}
