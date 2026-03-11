using UnityEngine;
using UIX.Templates;

namespace UIX.Components
{
    /// <summary>
    /// Resolves and instantiates components by name.
    /// </summary>
    public class ComponentResolver
    {
        private readonly ComponentRegistry _registry;

        public ComponentResolver(ComponentRegistry registry)
        {
            _registry = registry;
        }

        public GameObject Instantiate(string componentName, Transform parent)
        {
            var entry = _registry?.Find(componentName);
            if (entry?.Prefab == null)
                return null;

            var instance = Object.Instantiate(entry.Prefab, parent);
            return instance;
        }

        public UIXComponentDefinition GetDefinition(string componentName)
        {
            var entry = _registry?.Find(componentName);
            return entry?.Definition;
        }
    }
}
