using System;
using System.Collections.Generic;

namespace UIX.Binding
{
    /// <summary>
    /// Observable value that notifies on change.
    /// </summary>
    public class ReactiveProperty<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                _value = value;
                OnChanged?.Invoke(value);
            }
        }

        public event Action<T> OnChanged;

        public ReactiveProperty(T initialValue = default)
        {
            _value = initialValue;
        }

        public static implicit operator T(ReactiveProperty<T> prop) => prop != null ? prop.Value : default!;
    }
}
