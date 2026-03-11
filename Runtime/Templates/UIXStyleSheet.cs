using System.Collections.Generic;
using UnityEngine;

namespace UIX.Templates
{
    /// <summary>
    /// ScriptableObject holding compiled styles from USS.
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Style Sheet")]
    public class UIXStyleSheet : ScriptableObject
    {
        [HideInInspector] public string SourcePath;
        [HideInInspector] [TextArea(5, 20)] public string SourceUSS;
        [HideInInspector] public List<SerializedStyleRule> Rules = new List<SerializedStyleRule>();
        [HideInInspector] public List<SerializedVariable> Variables = new List<SerializedVariable>();

        [System.Serializable]
        public class SerializedStyleRule
        {
            public string SelectorJson;
            public List<SerializedProperty> Properties = new List<SerializedProperty>();
            public int LineNumber;
        }

        [System.Serializable]
        public class SerializedProperty
        {
            public string Name;
            public string Value;
        }

        [System.Serializable]
        public class SerializedVariable
        {
            public string Name;
            public string Value;
        }
    }
}
