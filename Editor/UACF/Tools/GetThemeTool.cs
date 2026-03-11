using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class GetThemeTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var themeName = p["theme_name"]?.ToString();
                return UacfResponse.Success(new
                {
                    theme_name = themeName ?? "DefaultTheme",
                    variables = new object()
                }, 0);
            });
        }
    }
}
