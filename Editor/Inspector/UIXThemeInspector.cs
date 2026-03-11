using UnityEditor;
using UnityEngine;

namespace UIX.Editor.Inspector
{
    [CustomEditor(typeof(UIX.Templates.UIXTheme))]
    public class UIXThemeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
