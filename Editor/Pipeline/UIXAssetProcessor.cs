using System.IO;
using UnityEditor;
using UnityEngine;
using UIX.Templates;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// AssetPostprocessor for .xml and .uss files - triggers compilation and prefab generation.
    /// </summary>
    public class UIXAssetProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                if (path.EndsWith(".xml"))
                {
                    var content = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath), path));
                    var template = UIXCompiler.CompileXml(path, content);
                    if (template != null)
                    {
                        var prefabPath = UIXPrefabGenerator.SaveAsPrefab(template);
                        if (!string.IsNullOrEmpty(prefabPath))
                        {
                            AssetDatabase.Refresh();
                            if (!template.IsComponent && !string.IsNullOrEmpty(template.ViewModelType))
                            {
                                var name = Path.GetFileNameWithoutExtension(path);
                                var bindingsPath = Path.Combine("Assets/UI/_Generated/Screens", name + "_Bindings.generated.cs").Replace("\\", "/");
                                UIXBindingCodeGen.Generate(name, template.ViewModelType, Path.Combine(Path.GetDirectoryName(Application.dataPath), bindingsPath));
                            }
                        }
                    }
                }
                else if (path.EndsWith(".uss"))
                {
                    UIXCompiler.CompileAsset(path);
                    if (!UIXCompiler.IsThemePath(path))
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
