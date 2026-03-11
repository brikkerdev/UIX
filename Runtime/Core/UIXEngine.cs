using UnityEngine;
using UIX.Components;
using UIX.Navigation;
using UIX.Rendering;
using UIX.Styling;
using UIX.Templates;

namespace UIX.Core
{
    /// <summary>
    /// Main entry point for UIX Framework.
    /// </summary>
    public static class UIXEngine
    {
        private static UIXConfiguration _config;
        private static UIXNavigator _navigator;
        private static UIXThemeManager _themes;
        private static ComponentRegistry _registry;
        private static ComponentResolver _componentResolver;
        private static UIXRenderer _renderer;
        private static VariableResolver _variableResolver;
        private static StyleResolver _styleResolver;

        public static UIXNavigator Navigator => _navigator;
        public static UIXThemeManager Themes => _themes;
        public static ComponentRegistry Registry => _registry;
        public static ComponentResolver ComponentResolver => _componentResolver;
        public static UIXRenderer Renderer => _renderer;
        public static RectTransform OverlayContainer => _config?.OverlayContainer;
        internal static StyleResolver StyleResolver => _styleResolver;
        internal static VariableResolver VariableResolver => _variableResolver;

        public static void Initialize(UIXConfiguration config)
        {
            _config = config;
            _variableResolver = new VariableResolver();
            _styleResolver = new StyleResolver(_variableResolver);
            _themes = new UIXThemeManager(_variableResolver);
            _navigator = new UIXNavigator();
            _registry = config?.ComponentRegistry;
            _componentResolver = _registry != null ? new ComponentResolver(_registry) : null;
            _renderer = new UIXRenderer(_styleResolver, _variableResolver, _componentResolver);

            if (config?.DefaultTheme != null)
                _themes.SetTheme(config.DefaultTheme);
        }
    }
}
