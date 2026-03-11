using System.Collections.Generic;
using UnityEngine;

namespace UIX.Templates
{
    /// <summary>
    /// ScriptableObject defining a UIX component (props, slots, template reference).
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Component Definition")]
    public class UIXComponentDefinition : ScriptableObject
    {
        public string ComponentName;
        public List<PropDefinition> Props = new List<PropDefinition>();
        public List<SlotDefinition> Slots = new List<SlotDefinition>();
        public UIXTemplate Template;
        public UIXStyleSheet StyleSheet;
    }

    [System.Serializable]
    public class PropDefinition
    {
        public string Name;
        public string Type = "string";
        public string Default;
        public bool Optional;
    }

    [System.Serializable]
    public class SlotDefinition
    {
        public string Name = "default";
        public string Description;
    }
}
