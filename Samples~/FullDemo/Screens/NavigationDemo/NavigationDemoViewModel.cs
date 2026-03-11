using UIX.Binding;

namespace UIX.Samples.FullDemo
{
    public class NavigationDemoViewModel : ViewModel
    {
        public void OnThemeDemoClicked()
        {
            Navigator.Push<ThemeDemoViewModel>();
        }

        public void OnComponentsDemoClicked()
        {
            Navigator.Push<ComponentsDemoViewModel>();
        }

        public void OnBackClicked()
        {
            Navigator.Pop();
        }
    }
}
