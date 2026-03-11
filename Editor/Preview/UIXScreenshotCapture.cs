using UnityEngine;

namespace UIX.Editor.Preview
{
    public static class UIXScreenshotCapture
    {
        public static Texture2D CaptureScreen(string screenName, int width, int height)
        {
            return CaptureScreen(screenName, null, width, height);
        }

        public static Texture2D CaptureScreen(string screenName, string themeName, int width, int height)
        {
            return new Texture2D(width, height);
        }

        public static string CaptureToFile(string screenName, string outputPath, int width, int height)
        {
            var tex = CaptureScreen(screenName, width, height);
            if (tex == null) return null;
            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(outputPath, bytes);
            return outputPath;
        }
    }
}
