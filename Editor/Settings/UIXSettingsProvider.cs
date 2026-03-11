using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UIX.Core;
using UIX.Templates;

namespace UIX.Editor.Settings
{
    internal static class UIXSettingsProvider
    {
        private const string SettingsPath = "Project/UIX";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider(SettingsPath, SettingsScope.Project)
            {
                label = "UIX",
                keywords = new HashSet<string>(new[] { "UIX", "UI", "Components", "Screens", "Themes", "Generated" }),
                guiHandler = OnGUI
            };
        }

        private static void OnGUI(string searchContext)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("UIX Framework Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Configure paths and default settings for the UIX Framework. " +
                "For runtime configuration, use a UIXConfiguration ScriptableObject.",
                MessageType.Info);
            EditorGUILayout.Space(10);

            var config = FindDefaultConfiguration();
            if (config != null)
            {
                var so = new SerializedObject(config);
                EditorGUILayout.PropertyField(so.FindProperty("ComponentsSearchPath"), new GUIContent("Components Path"));
                EditorGUILayout.PropertyField(so.FindProperty("ScreensSearchPath"), new GUIContent("Screens Path"));
                EditorGUILayout.PropertyField(so.FindProperty("ThemesSearchPath"), new GUIContent("Themes Path"));
                EditorGUILayout.PropertyField(so.FindProperty("GeneratedAssetsPath"), new GUIContent("Generated Path"));
                EditorGUILayout.PropertyField(so.FindProperty("DefaultTheme"), new GUIContent("Default Theme"));
                EditorGUILayout.PropertyField(so.FindProperty("ComponentRegistry"), new GUIContent("Component Registry"));
                EditorGUILayout.PropertyField(so.FindProperty("EnableHotReload"), new GUIContent("Enable Hot Reload"));
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No UIXConfiguration found. Create one via Assets > Create > UIX > Configuration.",
                    MessageType.Warning);
                if (GUILayout.Button("Create UIX Configuration"))
                {
                    var newConfig = ScriptableObject.CreateInstance<UIXConfiguration>();
                    newConfig.ComponentsSearchPath = "Assets/UI/Components";
                    newConfig.ScreensSearchPath = "Assets/UI/Screens";
                    newConfig.ThemesSearchPath = "Assets/UI/Themes";
                    newConfig.GeneratedAssetsPath = "Assets/UI/_Generated";
                    var path = EditorUtility.SaveFilePanelInProject("Save UIX Configuration", "UIXConfiguration", "asset", "Save UIX Configuration");
                    if (!string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.CreateAsset(newConfig, path);
                        AssetDatabase.SaveAssets();
                        Selection.activeObject = newConfig;
                    }
                }
            }
        }

        private static UIXConfiguration FindDefaultConfiguration()
        {
            var guids = AssetDatabase.FindAssets("t:UIXConfiguration");
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                var c = AssetDatabase.LoadAssetAtPath<UIXConfiguration>(p);
                if (c != null) return c;
            }
            return null;
        }
    }
}
