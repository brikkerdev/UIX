using NUnit.Framework;
using UIX.Editor.Pipeline;

namespace UIX.Tests.Editor
{
    public class UACFToolsTests
    {
        [Test]
        public void UIXCompiler_IsThemePath_ThemesFolder_ReturnsTrue()
        {
            Assert.IsTrue(UIXCompiler.IsThemePath("Assets/UI/Themes/DarkTheme/theme.uss"));
            Assert.IsTrue(UIXCompiler.IsThemePath("Assets/Resources/UI/Themes/Default/theme.uss"));
        }

        [Test]
        public void UIXCompiler_IsThemePath_ScreenUss_ReturnsFalse()
        {
            Assert.IsFalse(UIXCompiler.IsThemePath("Assets/UI/Screens/MainMenu/MainMenu.uss"));
            Assert.IsFalse(UIXCompiler.IsThemePath("Assets/UI/Components/Button/Button.uss"));
        }
    }
}
