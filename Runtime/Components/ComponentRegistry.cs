using System.Collections.Generic;
using UnityEngine;
using UIX.Templates;

namespace UIX.Components
{
    /// <summary>
    /// Registry of all registered UIX components.
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Component Registry")]
    public class ComponentRegistry : ScriptableObject
    {
        public List<ComponentEntry> Components = new List<ComponentEntry>();

        public ComponentEntry Find(string name)
        {
            foreach (var c in Components)
                if (c.Name == name) return c;
            return null;
        }
    }

    [System.Serializable]
    public class ComponentEntry
    {
        public string Name;
        public string SourceXMLPath;
        public string SourceUSSPath;
        public string SourceCSPath;
        public UIXComponentDefinition Definition;
        public UIXTemplate Template;
        public UIXStyleSheet StyleSheet;
        public GameObject Prefab;
        public List<PropDefinition> Props = new List<PropDefinition>();
        public List<SlotDefinition> Slots = new List<SlotDefinition>();
        public List<string> UsedByScreens = new List<string>();
        public List<string> UsedByComponents = new List<string>();
    }
}
