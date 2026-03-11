using UnityEditor;
using UnityEngine;

namespace UIX.Editor.Inspector
{
    [CustomEditor(typeof(UIX.Templates.UIXTemplate))]
    public class UIXTemplateInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.LabelField("Template", EditorStyles.boldLabel);
        }
    }
}
