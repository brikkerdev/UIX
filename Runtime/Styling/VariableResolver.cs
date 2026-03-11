using System.Collections.Generic;

namespace UIX.Styling
{
    /// <summary>
    /// Resolves var(--name) and var(--name, fallback) in style values.
    /// </summary>
    public class VariableResolver
    {
        private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

        public void SetVariables(IEnumerable<KeyValuePair<string, string>> variables)
        {
            _variables.Clear();
            foreach (var kv in variables)
                _variables[kv.Key] = kv.Value;
        }

        public void SetVariable(string name, string value)
        {
            _variables[name] = value;
        }

        public string Resolve(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Parsing.USSParser.ResolveVar(value, _variables);
        }

        public bool TryGetVariable(string name, out string value)
        {
            return _variables.TryGetValue(name, out value);
        }
    }
}
