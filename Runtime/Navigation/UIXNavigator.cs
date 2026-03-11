using System;
using System.Collections.Generic;

namespace UIX.Navigation
{
    /// <summary>
    /// Manages navigation between screens.
    /// </summary>
    public class UIXNavigator
    {
        private readonly ScreenStack _stack = new ScreenStack();
        private readonly ModalManager _modals = new ModalManager();

        public Binding.ViewModel CurrentScreen => _stack.Current;
        public IReadOnlyList<Binding.ViewModel> ScreenStack => _stack.All;
        public bool HasModal => _modals.HasActive;

        public event Action<Binding.ViewModel> OnScreenPushed;
        public event Action<Binding.ViewModel> OnScreenPopped;
        public event Action<Binding.ViewModel> OnModalShown;
        public event Action<Binding.ViewModel> OnModalClosed;

        public void Push<T>() where T : Binding.ViewModel, new()
        {
            var vm = new T();
            _stack.Push(vm);
            OnScreenPushed?.Invoke(vm);
        }

        public void Push<T>(object props) where T : Binding.ViewModel, new()
        {
            Push<T>();
        }

        public void Replace<T>() where T : Binding.ViewModel, new()
        {
            _stack.Pop();
            Push<T>();
        }

        public void Replace<T>(object props) where T : Binding.ViewModel, new()
        {
            Replace<T>();
        }

        public void Pop()
        {
            var vm = _stack.Pop();
            if (vm != null)
                OnScreenPopped?.Invoke(vm);
        }

        public void PopToRoot()
        {
            while (_stack.Count > 1)
                Pop();
        }

        public void PopTo<T>() where T : Binding.ViewModel
        {
            while (_stack.Current != null && _stack.Current is not T)
                Pop();
        }

        public void ShowModal<T>() where T : Binding.ViewModel, new()
        {
            var vm = new T();
            _modals.Show(vm);
            OnModalShown?.Invoke(vm);
        }

        public void ShowModal<T>(object props) where T : Binding.ViewModel, new()
        {
            ShowModal<T>();
        }

        public void CloseModal()
        {
            var vm = _modals.Close();
            if (vm != null)
                OnModalClosed?.Invoke(vm);
        }

        public void CloseAllModals()
        {
            while (_modals.HasActive)
                CloseModal();
        }

        public void ShowOverlay<T>() where T : Binding.ViewModel, new() { }
        public void HideOverlay<T>() where T : Binding.ViewModel { }
    }
}
