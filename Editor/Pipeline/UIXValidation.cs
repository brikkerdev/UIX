using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UIX.Components;
using UIX.Parsing;
using UIX.Parsing.Nodes;
using UIX.Templates;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// Validates UIX XML and USS files.
    /// </summary>
    public static class UIXValidation
    {
        private static readonly HashSet<string> ValidPropTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "string", "int", "float", "bool", "sprite", "texture", "color", "font"
        };

        private static readonly Regex BindingRegex = new Regex(@"\{([^}]*)\}", RegexOptions.Compiled);
        private static readonly Regex VarRegex = new Regex(@"var\s*\(\s*--([a-zA-Z0-9_-]+)\s*(?:,\s*(.+?))?\s*\)", RegexOptions.Compiled);

        public class ValidationResult
        {
            public bool Valid;
            public List<ValidationError> Errors = new List<ValidationError>();
            public List<ValidationError> Warnings = new List<ValidationError>();
        }

        public class ValidationError
        {
            public string File;
            public int Line;
            public string Message;
            public string Severity;
        }

        public static ComponentRegistry GetComponentRegistry()
        {
            var guids = AssetDatabase.FindAssets("t:ComponentRegistry");
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                var reg = AssetDatabase.LoadAssetAtPath<ComponentRegistry>(p);
                if (reg != null) return reg;
            }
            return null;
        }

        public static ValidationResult ValidateXml(string path, string content)
        {
            var result = new ValidationResult { Valid = true };

            try
            {
                var root = XMLParser.Parse(content, path);
                if (root == null)
                {
                    result.Errors.Add(new ValidationError { File = path, Message = "Failed to parse XML", Severity = "error" });
                    result.Valid = false;
                    return result;
                }

                var registry = GetComponentRegistry();

                // 1. Components in Registry
                var componentNames = CollectComponentNames(root);
                foreach (var name in componentNames)
                {
                    if (registry == null) break;
                    if (registry.Find(name) == null)
                    {
                        result.Errors.Add(new ValidationError { File = path, Message = $"Component '{name}' not found in registry", Severity = "error" });
                        result.Valid = false;
                    }
                }

                // 2. Required props, 3. Prop types - validate component usages
                ValidateComponentUsages(root, registry, result, path);

                // 4. Binding expressions
                ValidateBindingExpressions(root, result, path);

                // 5. ViewModel and members
                if (!string.IsNullOrEmpty(root.ViewModelType))
                {
                    ValidateViewModel(root, result, path);
                }

                // 6. Slots
                ValidateSlots(root, registry, result, path);
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationError { File = path, Message = ex.Message, Severity = "error" });
                result.Valid = false;
            }

            return result;
        }

        private static HashSet<string> CollectComponentNames(UIXNode node)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            CollectComponentNamesRecursive(node, names);
            return names;
        }

        private static void CollectComponentNamesRecursive(UIXNode node, HashSet<string> names)
        {
            if (node is ComponentNode cn)
            {
                names.Add(cn.ComponentName);
            }

            if (node is RootNode rn)
            {
                foreach (var c in rn.Children)
                    CollectComponentNamesRecursive(c, names);
            }
            else if (node is ElementNode en)
            {
                foreach (var c in en.Children)
                    CollectComponentNamesRecursive(c, names);
            }
            else if (node is ComponentNode compNode)
            {
                foreach (var slotContent in compNode.DefaultSlotContent)
                    CollectComponentNamesRecursive(slotContent, names);
                foreach (var kv in compNode.Slots)
                {
                    foreach (var slotChild in kv.Value)
                        CollectComponentNamesRecursive(slotChild, names);
                }
            }
            else if (node is SlotContentNode scn)
            {
                foreach (var c in scn.Children)
                    CollectComponentNamesRecursive(c, names);
            }
            else if (node is ConditionalNode condNode)
            {
                foreach (var c in condNode.Children)
                    CollectComponentNamesRecursive(c, names);
            }
            else if (node is ForeachNode feNode)
            {
                foreach (var c in feNode.Children)
                    CollectComponentNamesRecursive(c, names);
            }
        }

        private static void ValidateComponentUsages(UIXNode node, ComponentRegistry registry, ValidationResult result, string path)
        {
            if (node is ComponentNode cn && registry != null)
            {
                var entry = registry.Find(cn.ComponentName);
                if (entry != null)
                {
                    foreach (var prop in entry.Props)
                    {
                        if (!prop.Optional)
                        {
                            var hasValue = cn.Props.TryGetValue(prop.Name, out var val) && !string.IsNullOrEmpty(val);
                            if (!hasValue)
                            {
                                result.Errors.Add(new ValidationError { File = path, Message = $"Required prop '{prop.Name}' is missing for component '{cn.ComponentName}'", Severity = "error" });
                                result.Valid = false;
                            }
                        }
                    }
                    foreach (var prop in entry.Props)
                    {
                        if (!IsValidPropType(prop.Type))
                        {
                            result.Errors.Add(new ValidationError { File = path, Message = $"Invalid prop type '{prop.Type}' in component '{cn.ComponentName}'", Severity = "error" });
                            result.Valid = false;
                        }
                    }
                }

                foreach (var slotContent in cn.DefaultSlotContent)
                    ValidateComponentUsages(slotContent, registry, result, path);
                foreach (var kv in cn.Slots)
                {
                    foreach (var slotChild in kv.Value)
                        ValidateComponentUsages(slotChild, registry, result, path);
                }
            }

            if (node is RootNode rn)
            {
                foreach (var p in rn.Props)
                {
                    if (!IsValidPropType(p.Type))
                    {
                        result.Errors.Add(new ValidationError { File = path, Message = $"Invalid prop type '{p.Type}' in definition", Severity = "error" });
                        result.Valid = false;
                    }
                }
                foreach (var c in rn.Children)
                    ValidateComponentUsages(c, registry, result, path);
            }
            else if (node is ElementNode en)
            {
                foreach (var c in en.Children)
                    ValidateComponentUsages(c, registry, result, path);
            }
            else if (node is SlotContentNode scn)
            {
                foreach (var c in scn.Children)
                    ValidateComponentUsages(c, registry, result, path);
            }
            else if (node is ConditionalNode condNode)
            {
                foreach (var c in condNode.Children)
                    ValidateComponentUsages(c, registry, result, path);
            }
            else if (node is ForeachNode feNode)
            {
                foreach (var c in feNode.Children)
                    ValidateComponentUsages(c, registry, result, path);
            }
        }

        private static bool IsValidPropType(string type)
        {
            if (string.IsNullOrEmpty(type)) return false;
            if (ValidPropTypes.Contains(type)) return true;
            if (type.StartsWith("enum(", StringComparison.OrdinalIgnoreCase) && type.EndsWith(")")) return true;
            if (type.StartsWith("list<", StringComparison.OrdinalIgnoreCase) && type.EndsWith(">")) return true;
            if (type.Equals("event", StringComparison.OrdinalIgnoreCase)) return true;
            if (type.StartsWith("event<", StringComparison.OrdinalIgnoreCase) && type.EndsWith(">")) return true;
            return false;
        }

        private static void ValidateBindingExpressions(UIXNode node, ValidationResult result, string path)
        {
            void CheckExpression(string expr, string attrName)
            {
                if (string.IsNullOrEmpty(expr)) return;
                var open = expr.Count(c => c == '(' || c == '[' || c == '{');
                var close = expr.Count(c => c == ')' || c == ']' || c == '}');
                if (open != close)
                {
                    result.Errors.Add(new ValidationError { File = path, Message = $"Unbalanced brackets in binding '{attrName}': {expr}", Severity = "error" });
                    result.Valid = false;
                }
            }

            if (node is ElementNode en)
            {
                foreach (var kv in en.Attributes)
                {
                    var matches = BindingRegex.Matches(kv.Value);
                    foreach (Match m in matches)
                    {
                        var expr = m.Groups[1].Value.Trim();
                        CheckExpression(expr, kv.Key);
                    }
                }
                foreach (var c in en.Children)
                    ValidateBindingExpressions(c, result, path);
            }
            else if (node is ComponentNode cn)
            {
                foreach (var kv in cn.Props)
                {
                    var matches = BindingRegex.Matches(kv.Value);
                    foreach (Match m in matches)
                    {
                        var expr = m.Groups[1].Value.Trim();
                        CheckExpression(expr, kv.Key);
                    }
                }
                foreach (var slotContent in cn.DefaultSlotContent)
                    ValidateBindingExpressions(slotContent, result, path);
                foreach (var kv in cn.Slots)
                {
                    foreach (var slotChild in kv.Value)
                        ValidateBindingExpressions(slotChild, result, path);
                }
            }
            else if (node is RootNode rn)
            {
                foreach (var c in rn.Children)
                    ValidateBindingExpressions(c, result, path);
            }
            else if (node is SlotContentNode scn)
            {
                foreach (var c in scn.Children)
                    ValidateBindingExpressions(c, result, path);
            }
            else if (node is ConditionalNode condNode)
            {
                CheckExpression(condNode.ConditionExpression ?? "", "if");
                foreach (var c in condNode.Children)
                    ValidateBindingExpressions(c, result, path);
            }
            else if (node is ForeachNode feNode)
            {
                foreach (var c in feNode.Children)
                    ValidateBindingExpressions(c, result, path);
            }
        }

        private static void ValidateViewModel(RootNode root, ValidationResult result, string path)
        {
            var vmTypeName = root.ViewModelType;
            var vmType = FindTypeByName(vmTypeName);
            if (vmType == null)
            {
                result.Errors.Add(new ValidationError { File = path, Message = $"ViewModel type '{vmTypeName}' not found", Severity = "error" });
                result.Valid = false;
                return;
            }

            var bindingRefs = CollectBindingReferences(root);
            foreach (var refName in bindingRefs)
            {
                var memberName = refName.Split('.')[0].Trim();
                if (string.IsNullOrEmpty(memberName) || memberName.StartsWith("()") || memberName.StartsWith("=")) continue;
                if (memberName.StartsWith("=")) memberName = memberName.Substring(1);
                var prop = vmType.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var method = vmType.GetMethod(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null && method == null)
                {
                    result.Errors.Add(new ValidationError { File = path, Message = $"ViewModel '{vmTypeName}' does not contain member '{memberName}'", Severity = "error" });
                    result.Valid = false;
                }
            }
        }

        private static Type FindTypeByName(string name)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var t = asm.GetType(name);
                    if (t != null) return t;
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.Name == name) return type;
                    }
                }
                catch { }
            }
            return null;
        }

        private static HashSet<string> CollectBindingReferences(UIXNode node)
        {
            var refs = new HashSet<string>();
            CollectBindingRefsRecursive(node, refs);
            return refs;
        }

        private static void CollectBindingRefsRecursive(UIXNode node, HashSet<string> refs)
        {
            if (node is ElementNode en)
            {
                foreach (var kv in en.Attributes)
                {
                    foreach (Match m in BindingRegex.Matches(kv.Value))
                    {
                        var expr = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(expr)) refs.Add(expr);
                    }
                }
                foreach (var c in en.Children)
                    CollectBindingRefsRecursive(c, refs);
            }
            else if (node is ComponentNode cn)
            {
                foreach (var kv in cn.Props)
                {
                    foreach (Match m in BindingRegex.Matches(kv.Value))
                    {
                        var expr = m.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(expr)) refs.Add(expr);
                    }
                }
                foreach (var slotContent in cn.DefaultSlotContent)
                    CollectBindingRefsRecursive(slotContent, refs);
                foreach (var kv in cn.Slots)
                {
                    foreach (var slotChild in kv.Value)
                        CollectBindingRefsRecursive(slotChild, refs);
                }
            }
            else if (node is RootNode rn)
            {
                foreach (var c in rn.Children)
                    CollectBindingRefsRecursive(c, refs);
            }
            else if (node is SlotContentNode scn)
            {
                foreach (var c in scn.Children)
                    CollectBindingRefsRecursive(c, refs);
            }
            else if (node is ConditionalNode condNode)
            {
                if (!string.IsNullOrEmpty(condNode.ConditionExpression))
                    refs.Add(condNode.ConditionExpression);
                foreach (var c in condNode.Children)
                    CollectBindingRefsRecursive(c, refs);
            }
            else if (node is ForeachNode feNode)
            {
                foreach (var c in feNode.Children)
                    CollectBindingRefsRecursive(c, refs);
            }
        }

        private static void ValidateSlots(RootNode root, ComponentRegistry registry, ValidationResult result, string path)
        {
            if (registry == null) return;
            ValidateSlotsRecursive(root.Children, registry, result, path);
        }

        private static void ValidateSlotsRecursive(List<UIXNode> nodes, ComponentRegistry registry, ValidationResult result, string path)
        {
            foreach (var node in nodes)
            {
                if (node is ComponentNode cn)
                {
                    var entry = registry.Find(cn.ComponentName);
                    if (entry != null)
                    {
                        foreach (var slotName in cn.Slots.Keys)
                        {
                            var name = slotName ?? "default";
                            var hasSlot = entry.Slots.Any(s => string.Equals(s.Name ?? "default", name, StringComparison.OrdinalIgnoreCase));
                            if (!hasSlot)
                            {
                                result.Errors.Add(new ValidationError { File = path, Message = $"Slot '{name}' has no matching slot in component '{cn.ComponentName}'", Severity = "error" });
                                result.Valid = false;
                            }
                        }
                    }
                    foreach (var slotContent in cn.DefaultSlotContent)
                        ValidateSlotsRecursive(new List<UIXNode> { slotContent }, registry, result, path);
                    foreach (var kv in cn.Slots)
                        ValidateSlotsRecursive(kv.Value, registry, result, path);
                }
                else if (node is ElementNode en)
                {
                    ValidateSlotsRecursive(en.Children, registry, result, path);
                }
                else if (node is SlotContentNode scn)
                {
                    ValidateSlotsRecursive(scn.Children, registry, result, path);
                }
                else if (node is ConditionalNode condNode)
                {
                    ValidateSlotsRecursive(condNode.Children, registry, result, path);
                }
                else if (node is ForeachNode feNode)
                {
                    ValidateSlotsRecursive(feNode.Children, registry, result, path);
                }
            }
        }

        public static ValidationResult ValidateUss(string path, string content)
        {
            var result = new ValidationResult { Valid = true };

            try
            {
                var parseResult = USSParser.Parse(content, path);
                var themeVars = GetThemeVariables(parseResult);
                foreach (var rule in parseResult.Rules)
                {
                    foreach (var prop in rule.Properties)
                    {
                        if (!Styling.CSSProperties.IsSupported(prop.Key))
                            result.Warnings.Add(new ValidationError { File = path, Line = rule.LineNumber, Message = $"Unknown property: {prop.Key}", Severity = "warning" });

                        // 7. var() in USS
                        var varMatches = VarRegex.Matches(prop.Value);
                        foreach (Match m in varMatches)
                        {
                            var varName = "--" + m.Groups[1].Value;
                            var fallback = m.Groups[2].Success ? m.Groups[2].Value.Trim() : null;
                            if (!themeVars.ContainsKey(varName) && string.IsNullOrEmpty(fallback))
                            {
                                result.Errors.Add(new ValidationError { File = path, Line = rule.LineNumber, Message = $"Variable '{varName}' is not defined", Severity = "error" });
                                result.Valid = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationError { File = path, Message = ex.Message, Severity = "error" });
                result.Valid = false;
            }

            return result;
        }

        private static Dictionary<string, string> GetThemeVariables(USSParseResult parseResult = null)
        {
            var vars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (parseResult?.Variables != null)
            {
                foreach (var v in parseResult.Variables)
                    vars[v.Name] = v.Value;
            }
            var guids = AssetDatabase.FindAssets("t:UIXTheme");
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                var theme = AssetDatabase.LoadAssetAtPath<UIXTheme>(p);
                if (theme != null)
                {
                    foreach (var v in theme.Variables)
                        vars[v.Name] = v.Value;
                }
            }
            return vars;
        }
    }
}
