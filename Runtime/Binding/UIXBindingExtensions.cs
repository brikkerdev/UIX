using UnityEngine;

namespace UIX.Binding
{
    /// <summary>
    /// Extension methods for UIX binding.
    /// </summary>
    public static class UIXBindingExtensions
    {
        /// <summary>
        /// Finds a component of type T in the hierarchy by GameObject name (id).
        /// </summary>
        public static T FindDeep<T>(this GameObject root, string id) where T : Component
        {
            if (root == null || string.IsNullOrEmpty(id)) return null;
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                if (t.gameObject.name == id)
                {
                    var c = t.GetComponent<T>();
                    if (c != null) return c;
                }
            }
            return null;
        }
    }
}
