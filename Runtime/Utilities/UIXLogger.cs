using UnityEngine;

namespace UIX.Utilities
{
    public enum UIXLogLevel
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error
    }

    public static class UIXLogger
    {
        public static UIXLogLevel MinLevel = UIXLogLevel.Warning;

        public static void Log(string message, UIXLogLevel level = UIXLogLevel.Info)
        {
            if (level < MinLevel) return;

            switch (level)
            {
                case UIXLogLevel.Error:
                    Debug.LogError($"[UIX] {message}");
                    break;
                case UIXLogLevel.Warning:
                    Debug.LogWarning($"[UIX] {message}");
                    break;
                default:
                    Debug.Log($"[UIX] {message}");
                    break;
            }
        }
    }
}
