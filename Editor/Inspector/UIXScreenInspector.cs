using UnityEditor;
using UnityEngine;
using UIX.Core;

namespace UIX.Editor.Inspector
{
    [CustomEditor(typeof(UIXScreen))]
    public class UIXScreenInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var screen = (UIXScreen)target;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("UIX Screen", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Screen root for UIX-rendered content.");
        }
    }
}
