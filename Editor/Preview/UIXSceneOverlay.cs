using UnityEditor;
using UnityEngine;

namespace UIX.Editor.Preview
{
    /// <summary>
    /// Overlay in Scene View for UIX screens - shows screen labels.
    /// </summary>
    [InitializeOnLoad]
    public static class UIXSceneOverlay
    {
        static UIXSceneOverlay()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            var screens = Object.FindObjectsOfType<Core.UIXScreen>();
            foreach (var screen in screens)
            {
                if (screen == null || !screen.gameObject.activeInHierarchy) continue;

                var rect = screen.GetComponent<RectTransform>();
                if (rect == null) continue;

                var cam = sceneView.camera;
                if (cam == null) continue;

                Handles.BeginGUI();
                var center = RectTransformUtility.WorldToScreenPoint(cam, rect.position);
                center.y = sceneView.position.height - center.y;
                var labelRect = new Rect(center.x - 50, center.y - 10, 100, 20);
                GUI.Label(labelRect, "UIX: " + screen.name);
                Handles.EndGUI();
            }
        }
    }
}
