using System;
using System.Collections;
using System.Collections.Generic;

namespace UIX.Binding
{
    /// <summary>
    /// Observable collection that notifies on changes.
    /// </summary>
    public class ReactiveCollection<T> : IReadOnlyList<T>
    {
        private readonly List<T> _items = new List<T>();

        public int Count => _items.Count;

        public event Action OnCollectionChanged;
        public event Action<int, T> OnItemAdded;
        public event Action<int, T> OnItemRemoved;
        public event Action<int, T, T> OnItemReplaced;

        public void Add(T item)
        {
            var index = _items.Count;
            _items.Add(item);
            OnItemAdded?.Invoke(index, item);
            OnCollectionChanged?.Invoke();
        }

        public void Remove(T item)
        {
            var index = _items.IndexOf(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                OnItemRemoved?.Invoke(index, item);
                OnCollectionChanged?.Invoke();
            }
        }

        public void RemoveAt(int index)
        {
            var item = _items[index];
            _items.RemoveAt(index);
            OnItemRemoved?.Invoke(index, item);
            OnCollectionChanged?.Invoke();
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            OnItemAdded?.Invoke(index, item);
            OnCollectionChanged?.Invoke();
        }

        public void Clear()
        {
            for (var i = _items.Count - 1; i >= 0; i--)
                OnItemRemoved?.Invoke(i, _items[i]);
            _items.Clear();
            OnCollectionChanged?.Invoke();
        }

        public T this[int index]
        {
            get => _items[index];
            set
            {
                var old = _items[index];
                _items[index] = value;
                OnItemReplaced?.Invoke(index, old, value);
                OnCollectionChanged?.Invoke();
            }
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
