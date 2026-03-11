using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UIX.Core;
using UIX.Templates;
using UIX.Utilities;

namespace UIX.Editor.Settings
{
    internal static class UIXSettingsProvider
    {
        private const string SettingsPath = "Project/UIX";
        private const string EditorSettingsGUID = "UIXEditorSettings";

        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider(SettingsPath, SettingsScope.Project)
            {
                label = "UIX",
                keywords = new HashSet<string>(new[] { "UIX", "UI", "Components", "Screens", "Themes", "Generated", "AutoCompile", "LogLevel" }),
                guiHandler = OnGUI
            };
        }

        private static UIXEditorSettings GetOrCreateEditorSettings()
        {
            var guids = AssetDatabase.FindAssets("t:UIXEditorSettings");
            foreach (var g in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(g);
                var s = AssetDatabase.LoadAssetAtPath<UIXEditorSettings>(p);
                if (s != null) return s;
            }
            var settings = ScriptableObject.CreateInstance<UIXEditorSettings>();
            var path = "Assets/UIX/Editor/UIXEditorSettings.asset";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            return settings;
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

            var editorSettings = GetOrCreateEditorSettings();
            var editorSo = new SerializedObject(editorSettings);
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(editorSo.FindProperty("AutoCompile"), new GUIContent("Auto Compile"));
            EditorGUILayout.PropertyField(editorSo.FindProperty("AutoGeneratePrefabs"), new GUIContent("Auto Generate Prefabs"));
            var logLevelProp = editorSo.FindProperty("LogLevel");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(logLevelProp, new GUIContent("Log Level"));
            if (EditorGUI.EndChangeCheck())
            {
                editorSo.ApplyModifiedPropertiesWithoutUndo();
                UIXLogger.MinLevel = editorSettings.LogLevel;
            }
            editorSo.ApplyModifiedPropertiesWithoutUndo();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Runtime Configuration", EditorStyles.boldLabel);
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
                EditorGUILayout.PropertyField(so.FindProperty("OverlayContainer"), new GUIContent("Overlay Container"));
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
