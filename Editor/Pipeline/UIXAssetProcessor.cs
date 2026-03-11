using UnityEditor;
using UnityEngine;

namespace UIX.Editor.Pipeline
{
    /// <summary>
    /// AssetPostprocessor for .xml and .uss files - triggers compilation.
    /// </summary>
    public class UIXAssetProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                if (path.EndsWith(".xml") || path.EndsWith(".uss"))
                {
                    UIXCompiler.CompileAsset(path);
                }
            }
        }
    }
}
