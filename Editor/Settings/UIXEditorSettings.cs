using UnityEngine;
using UIX.Utilities;

namespace UIX.Editor.Settings
{
    /// <summary>
    /// Editor-only settings for UIX.
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Editor Settings", fileName = "UIXEditorSettings")]
    public class UIXEditorSettings : ScriptableObject
    {
        public bool AutoCompile = true;
        public bool AutoGeneratePrefabs = true;
        public UIXLogLevel LogLevel = UIXLogLevel.Warning;
    }
}
