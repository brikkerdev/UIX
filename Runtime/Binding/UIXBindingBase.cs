using UnityEngine;

namespace UIX.Binding
{
    public abstract class UIXBindingBase
    {
        public abstract void Bind(ViewModel viewModel, GameObject root);
        public abstract void Unbind();
    }
}
