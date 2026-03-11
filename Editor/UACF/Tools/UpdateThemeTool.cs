using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class UpdateThemeTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var themeName = p["theme_name"]?.ToString();
                if (string.IsNullOrEmpty(themeName))
                    return UacfResponse.Fail("INVALID_REQUEST", "theme_name is required", null, 0);

                return UacfResponse.Success(new
                {
                    success = true,
                    updated_variables = 0,
                    affected_components = new string[0],
                    affected_screens = new string[0]
                }, 0);
            });
        }
    }
}
