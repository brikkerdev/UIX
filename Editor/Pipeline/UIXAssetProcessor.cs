using System.IO;
using UnityEditor;
using UnityEngine;
using UIX.Templates;
using UIX.Editor.Settings;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// AssetPostprocessor for .xml and .uss files - triggers compilation and prefab generation.
    /// </summary>
    public class UIXAssetProcessor : AssetPostprocessor
    {
        private static bool AutoCompileEnabled
        {
            get
            {
                var guids = AssetDatabase.FindAssets("t:UIXEditorSettings");
                foreach (var g in guids)
                {
                    var s = AssetDatabase.LoadAssetAtPath<UIXEditorSettings>(AssetDatabase.GUIDToAssetPath(g));
                    if (s != null) return s.AutoCompile;
                }
                return true;
            }
        }

        private static bool AutoGeneratePrefabsEnabled
        {
            get
            {
                var guids = AssetDatabase.FindAssets("t:UIXEditorSettings");
                foreach (var g in guids)
                {
                    var s = AssetDatabase.LoadAssetAtPath<UIXEditorSettings>(AssetDatabase.GUIDToAssetPath(g));
                    if (s != null) return s.AutoGeneratePrefabs;
                }
                return true;
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!AutoCompileEnabled) return;

            foreach (var path in importedAssets)
            {
                if (path.EndsWith(".xml"))
                {
                    var content = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), path));
                    var validation = UIXValidation.ValidateXml(path, content);
                    if (!validation.Valid)
                    {
                        foreach (var e in validation.Errors)
                        {
                            var loc = e.Line > 0 ? $"{path}({e.Line}): " : $"{path}: ";
                            Debug.LogError($"[UIX] {loc}{e.Message}");
                        }
                        continue;
                    }
                    var template = UIXCompiler.CompileXml(path, content);
                    if (template != null && AutoGeneratePrefabsEnabled)
                    {
                        var prefabPath = UIXPrefabGenerator.SaveAsPrefab(template);
                        if (!string.IsNullOrEmpty(prefabPath))
                        {
                            AssetDatabase.Refresh();
                            if (!template.IsComponent && !string.IsNullOrEmpty(template.ViewModelType))
                            {
                                var name = Path.GetFileNameWithoutExtension(path);
                                var bindingsPath = Path.Combine("Assets/UI/_Generated/Screens", name + "_Bindings.generated.cs").Replace("\\", "/");
                                UIXBindingCodeGen.Generate(template, name, template.ViewModelType, Path.Combine(Path.GetDirectoryName(Application.dataPath), bindingsPath));
                            }
                        }
                    }
                }
                else if (path.EndsWith(".uss"))
                {
                    var fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), path);
                    var content = File.ReadAllText(fullPath);
                    var validation = UIXValidation.ValidateUss(path, content);
                    if (!validation.Valid)
                    {
                        foreach (var e in validation.Errors)
                        {
                            var loc = e.Line > 0 ? $"{path}({e.Line}): " : $"{path}: ";
                            Debug.LogError($"[UIX] {loc}{e.Message}");
                        }
                        continue;
                    }
                    UIXCompiler.CompileAsset(path);
                    if (!UIXCompiler.IsThemePath(path) && AutoGeneratePrefabsEnabled)
                        RegeneratePrefabForUss(path);
                }
            }
        }

        private static void RegeneratePrefabForUss(string ussPath)
        {
            var xmlPath = ussPath.Replace(".uss", ".xml");
            var fullXmlPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), xmlPath);
            if (!File.Exists(fullXmlPath)) return;

            var dir = Path.GetDirectoryName(xmlPath).Replace("\\", "/");
            var name = Path.GetFileNameWithoutExtension(xmlPath);
            var templatePath = $"{dir}/_Generated/{name}_Template.asset";
            var template = AssetDatabase.LoadAssetAtPath<UIXTemplate>(templatePath);
            if (template != null)
            {
                UIXPrefabGenerator.SaveAsPrefab(template);
                AssetDatabase.Refresh();
            }
        }
    }
}
