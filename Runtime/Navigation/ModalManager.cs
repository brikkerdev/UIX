using System.Collections.Generic;

namespace UIX.Navigation
{
    public class ModalManager
    {
        private readonly Stack<Binding.ViewModel> _modals = new Stack<Binding.ViewModel>();

        public bool HasActive => _modals.Count > 0;

        public void Show(Binding.ViewModel vm)
        {
            _modals.Push(vm);
        }

        public Binding.ViewModel Close()
        {
            return _modals.Count > 0 ? _modals.Pop() : null;
        }
    }
}
