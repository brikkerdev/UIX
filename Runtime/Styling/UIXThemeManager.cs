using System;
using UnityEngine;

namespace UIX.Styling
{
    /// <summary>
    /// Manages current theme and theme switching.
    /// </summary>
    public class UIXThemeManager
    {
        private UIX.Templates.UIXTheme _currentTheme;
        private readonly VariableResolver _variableResolver;

        public UIX.Templates.UIXTheme CurrentTheme => _currentTheme;

        public event Action<UIX.Templates.UIXTheme, UIX.Templates.UIXTheme> OnThemeChanged;

        public UIXThemeManager(VariableResolver variableResolver)
        {
            _variableResolver = variableResolver;
        }

        public void SetTheme(string themeName)
        {
            var theme = Resources.Load<UIX.Templates.UIXTheme>($"Themes/{themeName}");
            if (theme != null)
                SetTheme(theme);
        }

        public void SetTheme(UIX.Templates.UIXTheme theme)
        {
            var oldTheme = _currentTheme;
            _currentTheme = theme;

            _variableResolver.SetVariables(GetThemeVariables(theme));
            OnThemeChanged?.Invoke(oldTheme, theme);
        }

        private static System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> GetThemeVariables(UIX.Templates.UIXTheme theme)
        {
            if (theme?.Variables == null) yield break;
            foreach (var v in theme.Variables)
            {
                if (!string.IsNullOrEmpty(v.Name))
                    yield return new System.Collections.Generic.KeyValuePair<string, string>(v.Name, v.Value);
            }
        }
    }
}
