using System.Collections.Generic;
using UnityEngine;
using UIX.Parsing.Nodes;

namespace UIX.Templates
{
    /// <summary>
    /// ScriptableObject holding compiled template (AST) from XML.
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Template")]
    public class UIXTemplate : ScriptableObject
    {
        [HideInInspector] public string SourcePath;
        [HideInInspector] public string TemplateName;
        [HideInInspector] public bool IsComponent;
        [HideInInspector] public string ViewModelType;
        [HideInInspector] public List<SerializedPropDef> Props = new List<SerializedPropDef>();
        [HideInInspector] public List<SerializedSlotDef> Slots = new List<SerializedSlotDef>();
        [HideInInspector] public string RootNodeJson;

        [System.Serializable]
        public class SerializedPropDef
        {
            public string Name;
            public string Type;
            public string Default;
            public bool Optional;
        }

        [System.Serializable]
        public class SerializedSlotDef
        {
            public string Name;
            public string Description;
        }
    }
}
