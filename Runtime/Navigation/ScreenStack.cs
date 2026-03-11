using System.Collections.Generic;

namespace UIX.Navigation
{
    public class ScreenStack
    {
        private readonly Stack<Binding.ViewModel> _stack = new Stack<Binding.ViewModel>();

        public Binding.ViewModel Current => _stack.Count > 0 ? _stack.Peek() : null;
        public int Count => _stack.Count;

        public IReadOnlyList<Binding.ViewModel> All => new List<Binding.ViewModel>(_stack);

        public void Push(Binding.ViewModel vm)
        {
            _stack.Push(vm);
        }

        public Binding.ViewModel Pop()
        {
            return _stack.Count > 0 ? _stack.Pop() : null;
        }
    }
}
