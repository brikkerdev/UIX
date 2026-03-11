using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UIX.Styling
{
    /// <summary>
    /// Tracks pseudo-class state (hover, pressed, disabled, checked, focused) and triggers style reapplication.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CSSPseudoClassTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Selectable _selectable;
        private Toggle _toggle;
        private TMP_InputField _inputField;
        private bool _hover;
        private bool _pressed;
        private string[] _lastPseudoClasses;

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();
            _toggle = GetComponent<Toggle>();
            _inputField = GetComponent<TMP_InputField>();
        }

        private void Update()
        {
            var pseudo = GetPseudoClasses();
            if (!SamePseudoClasses(_lastPseudoClasses, pseudo))
            {
                _lastPseudoClasses = pseudo;
                ReapplyStyles();
            }
        }

        public void OnPointerEnter(PointerEventData eventData) { _hover = true; }
        public void OnPointerExit(PointerEventData eventData) { _hover = false; }
        public void OnPointerDown(PointerEventData eventData) { _pressed = true; }
        public void OnPointerUp(PointerEventData eventData) { _pressed = false; }

        private string[] GetPseudoClasses()
        {
            var list = new List<string>();
            if (_selectable != null && !_selectable.interactable)
                list.Add("disabled");
            if (_toggle != null && _toggle.isOn)
                list.Add("checked");
            if (_inputField != null && _inputField.isFocused)
                list.Add("focused");
            if (_hover)
                list.Add("hover");
            if (_pressed)
                list.Add("pressed");
            return list.ToArray();
        }

        private static bool SamePseudoClasses(string[] a, string[] b)
        {
            if (a == null || b == null) return a == b;
            if (a.Length != b.Length) return false;
            var set = new HashSet<string>(b);
            foreach (var x in a)
                if (!set.Contains(x)) return false;
            return true;
        }

        private void ReapplyStyles()
        {
            var elementData = GetComponent<UIX.Rendering.UIXElementData>();
            if (elementData == null) return;

            var engine = UIX.Core.UIXEngine.StyleResolver;
            if (engine == null) return;

            var context = new StyleResolveContext
            {
                ParentElementType = elementData.ParentElementType,
                ParentClasses = elementData.ParentClasses ?? new List<string>(),
                ParentId = elementData.ParentId,
                IsDirectChild = elementData.IsDirectChild,
                SiblingIndex = elementData.SiblingIndex,
                SiblingCount = elementData.SiblingCount
            };

            var styles = engine.Resolve(elementData.ElementType, elementData.Id, elementData.Classes ?? new List<string>(), _lastPseudoClasses, context);
            UIX.Rendering.StyleApplicator.ReapplyStyles(gameObject, styles);
        }
    }
}
