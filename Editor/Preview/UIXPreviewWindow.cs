using UnityEditor;
using UnityEngine;

namespace UIX.Editor.Preview
{
    public class UIXPreviewWindow : EditorWindow
    {
        [MenuItem("UIX/Preview Window")]
        public static void ShowWindow()
        {
            GetWindow<UIXPreviewWindow>("UIX Preview");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UIX Preview", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Select a screen or component to preview. Use UIX → Create Component or Create Screen to add new UI.", MessageType.Info);
        }
    }
}
