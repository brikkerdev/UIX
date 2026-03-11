namespace UIX.Binding
{
    /// <summary>
    /// Base class for screen ViewModels.
    /// </summary>
    public abstract class ViewModel
    {
        protected UIX.Navigation.UIXNavigator Navigator { get; private set; }

        public virtual void OnCreated() { }
        public virtual void OnShown() { }
        public virtual void OnHidden() { }
        public virtual void OnDestroyed() { }

        internal void SetNavigator(UIX.Navigation.UIXNavigator navigator)
        {
            Navigator = navigator;
        }
    }
}
