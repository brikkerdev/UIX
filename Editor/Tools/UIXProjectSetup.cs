using UnityEditor;

namespace UIX.Editor.Tools
{
    public static class UIXProjectSetup
    {
        [MenuItem("UIX/Project Setup")]
        public static void Setup()
        {
            EditorUtility.DisplayDialog("UIX", "Create folders: Assets/UI/Components, Assets/UI/Screens, Assets/UI/Themes, Assets/UI/_Generated", "OK");
        }
    }
}
