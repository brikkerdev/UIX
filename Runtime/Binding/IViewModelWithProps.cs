namespace UIX.Binding
{
    /// <summary>
    /// Interface for ViewModels that accept initialization props.
    /// </summary>
    public interface IViewModelWithProps
    {
        void SetProps(object props);
    }
}
