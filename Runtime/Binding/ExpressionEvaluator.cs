using System;
using System.Reflection;

namespace UIX.Binding
{
    /// <summary>
    /// Evaluates binding expressions against a context object (ViewModel).
    /// </summary>
    public static class ExpressionEvaluator
    {
        public static object Evaluate(string expression, object context)
        {
            if (string.IsNullOrWhiteSpace(expression) || context == null)
                return null;

            var parts = expression.Split('.');
            object current = context;

            foreach (var part in parts)
            {
                if (current == null) return null;
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                current = GetMember(current, trimmed);
            }

            return current;
        }

        private static object GetMember(object obj, string name)
        {
            var type = obj.GetType();

            var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null)
                return prop.GetValue(obj);

            var field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field.GetValue(obj);

            return null;
        }
    }
}
