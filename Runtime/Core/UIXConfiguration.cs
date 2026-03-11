using UnityEngine;
using UIX.Components;
using UIX.Navigation;

namespace UIX.Core
{
    [CreateAssetMenu(menuName = "UIX/Configuration")]
    public class UIXConfiguration : ScriptableObject
    {
        public UIX.Templates.UIXTheme DefaultTheme;
        public UIX.Templates.UIXTheme[] AvailableThemes;
        public Canvas TargetCanvas;
        public RectTransform OverlayContainer;
        public TransitionType DefaultTransition = TransitionType.Fade;
        public float DefaultTransitionDuration = 0.3f;
        public bool EnableHotReload;
        public string GeneratedAssetsPath = "Assets/UI/_Generated";
        public string ComponentsSearchPath = "Assets/UI/Components";
        public string ScreensSearchPath = "Assets/UI/Screens";
        public string ThemesSearchPath = "Assets/UI/Themes";
        public UIX.Components.ComponentRegistry ComponentRegistry;
    }
}
