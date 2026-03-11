using System;
using System.Collections.Generic;
using UnityEngine;
using UIX.Core;
using UIX.Parsing;
using UIX.Parsing.Nodes;

namespace UIX.Navigation
{
    /// <summary>
    /// Manages navigation between screens.
    /// </summary>
    public class UIXNavigator
    {
        public static TransitionType DefaultTransition { get; set; } = TransitionType.Fade;
        public static float DefaultTransitionDuration { get; set; } = 0.3f;

        private readonly ScreenStack _stack = new ScreenStack();
        private readonly ModalManager _modals = new ModalManager();
        private readonly Dictionary<Type, (Binding.ViewModel vm, GameObject root)> _overlays = new Dictionary<Type, (Binding.ViewModel, GameObject)>();

        public Binding.ViewModel CurrentScreen => _stack.Current;
        public IReadOnlyList<Binding.ViewModel> ScreenStack => _stack.All;
        public bool HasModal => _modals.HasActive;

        public event Action<Binding.ViewModel> OnScreenPushed;
        public event Action<Binding.ViewModel> OnScreenPopped;
        public event Action<Binding.ViewModel> OnModalShown;
        public event Action<Binding.ViewModel> OnModalClosed;

        public void Push<T>() where T : Binding.ViewModel, new()
        {
            Push<T>(null);
        }

        public void Push<T>(object props) where T : Binding.ViewModel, new()
        {
            var vm = new T();
            if (vm is Binding.IViewModelWithProps withProps && props != null)
                withProps.SetProps(props);
            _stack.Push(vm);
            OnScreenPushed?.Invoke(vm);
        }

        public void Replace<T>() where T : Binding.ViewModel, new()
        {
            Replace<T>(null);
        }

        public void Replace<T>(object props) where T : Binding.ViewModel, new()
        {
            _stack.Pop();
            Push<T>(props);
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
            ShowModal<T>(null);
        }

        public void ShowModal<T>(object props) where T : Binding.ViewModel, new()
        {
            var vm = new T();
            if (vm is Binding.IViewModelWithProps withProps && props != null)
                withProps.SetProps(props);
            _modals.Show(vm);
            OnModalShown?.Invoke(vm);
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

        public void ShowOverlay<T>() where T : Binding.ViewModel, new()
        {
            ShowOverlay<T>(null);
        }

        public void ShowOverlay<T>(object props) where T : Binding.ViewModel, new()
        {
            if (_overlays.ContainsKey(typeof(T)))
                HideOverlay<T>();

            var vm = new T();
            if (vm is Binding.IViewModelWithProps withProps && props != null)
                withProps.SetProps(props);

            GameObject root = null;
            var container = UIXEngine.OverlayContainer;
            var renderer = UIXEngine.Renderer;
            if (container != null && renderer != null)
            {
                var rootNode = LoadScreenRoot<T>();
                if (rootNode != null)
                {
                    var child = renderer.Render(rootNode, container, vm, null);
                    if (child != null)
                    {
                        root = child;
                        if (root.transform.parent != container)
                            root.transform.SetParent(container, false);
                    }
                }
            }
            _overlays[typeof(T)] = (vm, root);
        }

        public void HideOverlay<T>() where T : Binding.ViewModel
        {
            if (_overlays.TryGetValue(typeof(T), out var entry))
            {
                if (entry.root != null)
                    UnityEngine.Object.Destroy(entry.root);
                _overlays.Remove(typeof(T));
            }
        }

        private static RootNode LoadScreenRoot<T>()
        {
            var typeName = typeof(T).Name;
            var screenName = typeName.EndsWith("ViewModel") ? typeName.Substring(0, typeName.Length - 9) : typeName;
            var templates = Resources.LoadAll<UIX.Templates.UIXTemplate>("");
            foreach (var template in templates)
            {
                if (template != null && !template.IsComponent && template.ViewModelType == typeName && !string.IsNullOrEmpty(template.SourcePath))
                {
                    var path = template.SourcePath.Replace("\\", "/");
                    var resPath = path.Replace("Assets/Resources/", "").Replace(".xml", "");
                    var xmlAsset = Resources.Load<TextAsset>(resPath);
                    if (xmlAsset != null)
                        return XMLParser.Parse(xmlAsset.text, template.SourcePath);
                }
            }
            return null;
        }
    }
}
