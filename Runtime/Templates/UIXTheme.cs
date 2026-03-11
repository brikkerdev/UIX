using System.Collections.Generic;
using UnityEngine;

namespace UIX.Templates
{
    /// <summary>
    /// ScriptableObject holding theme variables (from :root in theme.uss).
    /// </summary>
    [CreateAssetMenu(menuName = "UIX/Theme")]
    public class UIXTheme : ScriptableObject
    {
        public string ThemeName;
        public List<ThemeVariable> Variables = new List<ThemeVariable>();
        public List<ThemeResource> Resources = new List<ThemeResource>();
    }

    [System.Serializable]
    public class ThemeVariable
    {
        public string Name;
        public string Value;
    }

    [System.Serializable]
    public class ThemeResource
    {
        public string Name;
        public string Path;
        public Object Asset;
    }
}
