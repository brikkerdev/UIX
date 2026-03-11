using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class ListThemesTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                return UacfResponse.Success(new { themes = new[] { "DefaultTheme" } }, 0);
            });
        }
    }
}
