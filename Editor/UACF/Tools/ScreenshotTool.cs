using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UACF.Core;
using UACF.Models;
using UIX.Editor.Preview;

namespace UIX.Editor.UACF.Tools
{
    public static class ScreenshotTool
    {
        public static Task<UacfResponse> Handle(JObject p)
        {
            return MainThreadDispatcher.Enqueue(() =>
            {
                var screenName = p["screen_name"]?.ToString() ?? "SampleScreen";
                var width = p["width"]?.Value<int>() ?? 1920;
                var height = p["height"]?.Value<int>() ?? 1080;

                var tex = UIXScreenshotCapture.CaptureScreen(screenName, width, height);
                var base64 = "";
                if (tex != null)
                {
                    var bytes = tex.EncodeToPNG();
                    base64 = Convert.ToBase64String(bytes);
                }

                var outputPath = Path.Combine("Temp", "UIXScreenshots", screenName + ".png");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                UIXScreenshotCapture.CaptureToFile(screenName, outputPath, width, height);

                return UacfResponse.Success(new
                {
                    success = true,
                    screenshot_base64 = base64,
                    width,
                    height,
                    file_path = outputPath
                }, 0);
            });
        }
    }
}
