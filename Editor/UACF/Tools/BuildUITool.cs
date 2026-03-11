using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;
using UIX.Editor.Pipeline;

namespace UIX.Editor.UACF.Tools
{
    public static class BuildUITool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                AssetDatabase.Refresh();
                return UacfResponse.Success(new
                {
                    success = true,
                    compiled_components = 0,
                    compiled_screens = 0,
                    compiled_themes = 0,
                    generated_prefabs = 0,
                    errors = new string[0],
                    warnings = new string[0]
                }, 0);
            });
        }
    }
}
