using System.Text.RegularExpressions;

namespace UIX.Styling
{
    /// <summary>
    /// Transforms selectors for component style scoping (.btn -> .ComponentName__btn).
    /// </summary>
    public static class StyleScopeManager
    {
        private static readonly Regex ClassSelectorRegex = new Regex(@"\.([a-zA-Z0-9_-]+)", RegexOptions.Compiled);

        public static string ScopeSelector(string selector, string componentName)
        {
            if (string.IsNullOrEmpty(selector) || string.IsNullOrEmpty(componentName))
                return selector;

            return ClassSelectorRegex.Replace(selector, m => $".{componentName}__{m.Groups[1].Value}");
        }

        public static string ScopeClass(string className, string componentName)
        {
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(componentName))
                return className;
            return $"{componentName}__{className}";
        }
    }
}
