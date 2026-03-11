using System;
using System.Collections.Generic;

namespace UIX.Binding
{
    /// <summary>
    /// Manages data bindings between ViewModel and UI.
    /// </summary>
    public class BindingEngine
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public void Bind(object viewModel, Action<object, string> onPropertyChanged)
        {
        }

        public void Unbind()
        {
            foreach (var s in _subscriptions)
                s?.Dispose();
            _subscriptions.Clear();
        }
    }
}
