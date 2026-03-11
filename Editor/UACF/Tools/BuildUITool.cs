using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;
using UIX.Editor.Pipeline;
using UIX.Templates;

namespace UIX.Editor.UACF.Tools
{
    public static class BuildUITool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                AssetDatabase.Refresh();

                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                var screensPath = Path.Combine(projectRoot, "Assets/UI/Screens");
                var componentsPath = Path.Combine(projectRoot, "Assets/UI/Components");
                var altScreensPath = Path.Combine(projectRoot, "Assets/Resources/UI/Screens");
                var altComponentsPath = Path.Combine(projectRoot, "Assets/Resources/UI/Components");

                var compiledScreens = 0;
                var compiledComponents = 0;
                var generatedPrefabs = 0;
                var errors = new List<string>();
                var warnings = new List<string>();

                void ProcessXml(string fullXmlPath)
                {
                    var relPath = fullXmlPath.Replace(projectRoot + Path.DirectorySeparatorChar, "")
                        .Replace(projectRoot + Path.AltDirectorySeparatorChar, "").Replace("\\", "/");
                    if (!relPath.StartsWith("Assets/")) relPath = "Assets/" + relPath.TrimStart('/', '\\');

                    var content = File.Exists(fullXmlPath) ? File.ReadAllText(fullXmlPath) : "";
                    var template = UIXCompiler.CompileXml(relPath, content);
                    if (template == null) return;

                    if (template.IsComponent) compiledComponents++; else compiledScreens++;

                    var prefabPath = UIXPrefabGenerator.SaveAsPrefab(template);
                    if (!string.IsNullOrEmpty(prefabPath))
                    {
                        generatedPrefabs++;
                        if (!template.IsComponent && !string.IsNullOrEmpty(template.ViewModelType))
                        {
                            var name = Path.GetFileNameWithoutExtension(fullXmlPath);
                            var bindingsPath = Path.Combine(projectRoot, "Assets/UI/_Generated/Screens", name + "_Bindings.generated.cs");
                            var bindingsDir = Path.GetDirectoryName(bindingsPath);
                            if (!Directory.Exists(bindingsDir)) Directory.CreateDirectory(bindingsDir);
                            UIXBindingCodeGen.Generate(name, template.ViewModelType, bindingsPath);
                        }
                    }
                }

                foreach (var basePath in new[] { screensPath, altScreensPath, componentsPath, altComponentsPath })
                {
                    if (!Directory.Exists(basePath)) continue;
                    foreach (var dir in Directory.GetDirectories(basePath))
                    {
                        var name = Path.GetFileName(dir);
                        var xmlPath = Path.Combine(dir, name + ".xml");
                        if (File.Exists(xmlPath))
                        {
                            try { ProcessXml(xmlPath); }
                            catch (System.Exception ex) { errors.Add($"{xmlPath}: {ex.Message}"); }
                        }
                    }
                }

                AssetDatabase.Refresh();

                return UacfResponse.Success(new
                {
                    success = errors.Count == 0,
                    compiled_components = compiledComponents,
                    compiled_screens = compiledScreens,
                    generated_prefabs = generatedPrefabs,
                    errors = errors.ToArray(),
                    warnings = warnings.ToArray()
                }, 0);
            });
        }
    }
}
