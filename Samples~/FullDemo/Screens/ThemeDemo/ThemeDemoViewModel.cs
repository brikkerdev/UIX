using UIX.Binding;
using UIX.Core;

namespace UIX.Samples.FullDemo
{
    public class ThemeDemoViewModel : ViewModel
    {
        public void OnToggleTheme()
        {
            var current = UIXEngine.Themes.CurrentTheme?.ThemeName ?? "";
            if (current.Contains("Dark"))
                UIXEngine.Themes.SetTheme("LightTheme");
            else
                UIXEngine.Themes.SetTheme("DarkTheme");
        }
    }
}
