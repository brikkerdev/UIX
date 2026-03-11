using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;

namespace UIX.Editor.UACF.Tools
{
    public static class AddPropTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var componentName = p["component_name"]?.ToString();
                if (string.IsNullOrEmpty(componentName))
                    return UacfResponse.Fail("INVALID_REQUEST", "component_name is required", null, 0);

                return UacfResponse.Success(new
                {
                    success = true,
                    component_name = componentName,
                    total_props = 1,
                    screens_needing_update = new string[0]
                }, 0);
            });
        }
    }
}
