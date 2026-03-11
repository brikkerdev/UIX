using System.Collections.Generic;
using System.Linq;
using UIX.Parsing.Tokens;

namespace UIX.Styling
{
    /// <summary>
    /// Context for style resolution (parent, siblings).
    /// </summary>
    public struct StyleResolveContext
    {
        public string ParentElementType;
        public List<string> ParentClasses;
        public string ParentId;
        public bool IsDirectChild;
        public int SiblingIndex;
        public int SiblingCount;
    }

    /// <summary>
    /// Resolves which styles apply to an element based on selector matching and specificity.
    /// </summary>
    public class StyleResolver
    {
        private readonly List<StyleRuleEntry> _rules = new List<StyleRuleEntry>();
        private readonly VariableResolver _variableResolver;

        public StyleResolver(VariableResolver variableResolver)
        {
            _variableResolver = variableResolver;
        }

        public void AddRules(IEnumerable<StyleRule> rules)
        {
            foreach (var rule in rules)
            {
                _rules.Add(new StyleRuleEntry
                {
                    Rule = rule,
                    Specificity = rule.Selector?.Specificity ?? 0
                });
            }
        }

        public void ClearRules()
        {
            _rules.Clear();
        }

        public Dictionary<string, string> Resolve(string elementType, string id, IEnumerable<string> classes, string[] pseudoClasses = null)
        {
            return Resolve(elementType, id, classes, pseudoClasses, default);
        }

        public Dictionary<string, string> Resolve(string elementType, string id, IEnumerable<string> classes, string[] pseudoClasses, StyleResolveContext context)
        {
            var result = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            var classList = classes?.ToList() ?? new List<string>();
            var pseudoList = pseudoClasses?.ToList() ?? new List<string>();

            var matchingRules = _rules
                .Where(r => Matches(r.Rule.Selector, elementType, id, classList, pseudoList, context))
                .OrderBy(r => r.Specificity)
                .ThenBy(r => _rules.IndexOf(r));

            foreach (var entry in matchingRules)
            {
                foreach (var prop in entry.Rule.Properties)
                {
                    var resolved = _variableResolver.Resolve(prop.Value);
                    result[prop.Key] = resolved;
                }
            }

            return result;
        }

        private static bool Matches(StyleSelector selector, string elementType, string id, List<string> classes, List<string> pseudoClasses, StyleResolveContext context)
        {
            if (selector == null) return false;

            if (selector.Parent != null)
            {
                if (!MatchesParent(selector.Parent, context))
                    return false;
                if (selector.DirectChild && !context.IsDirectChild)
                    return false;
            }

            if (!string.IsNullOrEmpty(selector.ElementType) && !string.Equals(selector.ElementType, elementType, System.StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && selector.Id != id)
                return false;

            foreach (var c in selector.Classes)
            {
                if (!classes.Contains(c))
                    return false;
            }

            foreach (var p in selector.PseudoClasses)
            {
                if (p == "first-child" && context.SiblingIndex != 0)
                    return false;
                if (p == "last-child" && context.SiblingIndex != context.SiblingCount - 1)
                    return false;
                if (p != "first-child" && p != "last-child" && !pseudoClasses.Contains(p))
                    return false;
            }

            return true;
        }

        private static bool MatchesParent(StyleSelector parent, StyleResolveContext context)
        {
            if (!string.IsNullOrEmpty(parent.ElementType) && !string.Equals(parent.ElementType, context.ParentElementType, System.StringComparison.OrdinalIgnoreCase))
                return false;
            if (!string.IsNullOrEmpty(parent.Id) && parent.Id != context.ParentId)
                return false;
            if (context.ParentClasses != null)
            {
                foreach (var c in parent.Classes)
                {
                    if (!context.ParentClasses.Contains(c))
                        return false;
                }
            }
            return true;
        }

        private class StyleRuleEntry
        {
            public StyleRule Rule;
            public int Specificity;
        }
    }
}
