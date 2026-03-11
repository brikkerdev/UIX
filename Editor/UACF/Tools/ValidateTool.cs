using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;
using UIX.Editor.Pipeline;

namespace UIX.Editor.UACF.Tools
{
    public static class ValidateTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var valid = true;
                var errors = new System.Collections.Generic.List<object>();
                var warnings = new System.Collections.Generic.List<object>();

                var targets = p["targets"] as JArray;
                var targetAll = targets == null || (targets.Count == 1 && targets[0]?.ToString() == "all");

                if (targetAll)
                {
                    var componentsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/UI/Components");
                    if (Directory.Exists(componentsPath))
                    {
                        foreach (var dir in Directory.GetDirectories(componentsPath))
                        {
                            var xmlPath = Path.Combine(dir, Path.GetFileName(dir) + ".xml");
                            if (File.Exists(xmlPath))
                            {
                                var assetPath = "Assets/UI/Components/" + Path.GetFileName(dir) + "/" + Path.GetFileName(dir) + ".xml";
                                var result = UIXValidation.ValidateXml(assetPath, File.ReadAllText(xmlPath));
                                if (!result.Valid)
                                {
                                    valid = false;
                                    foreach (var e in result.Errors)
                                        errors.Add(new { file = e.File, line = e.Line, message = e.Message, severity = e.Severity });
                                }
                            }
                        }
                    }
                }

                return UacfResponse.Success(new { valid, errors, warnings }, 0);
            });
        }
    }
}
