using System.Collections;
using UnityEngine;

namespace UIX.Navigation
{
    public enum TransitionType
    {
        None,
        Fade,
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        Scale,
        Custom
    }

    /// <summary>
    /// Applies transition animations to screen RectTransforms.
    /// </summary>
    public static class TransitionAnimator
    {
        public static void AnimateIn(RectTransform rect, TransitionType type, float duration, System.Action onComplete = null)
        {
            if (rect == null) { onComplete?.Invoke(); return; }
            var runner = rect.GetComponent<TransitionRunner>() ?? rect.gameObject.AddComponent<TransitionRunner>();
            runner.StartCoroutine(DoAnimate(rect, type, duration, true, onComplete));
        }

        public static void AnimateOut(RectTransform rect, TransitionType type, float duration, System.Action onComplete = null)
        {
            if (rect == null) { onComplete?.Invoke(); return; }
            var runner = rect.GetComponent<TransitionRunner>() ?? rect.gameObject.AddComponent<TransitionRunner>();
            runner.StartCoroutine(DoAnimate(rect, type, duration, false, onComplete));
        }

        private static IEnumerator DoAnimate(RectTransform rect, TransitionType type, float duration, bool isIn, System.Action onComplete)
        {
            if (type == TransitionType.None || duration <= 0)
            {
                SetFinalState(rect, type, isIn);
                onComplete?.Invoke();
                yield break;
            }

            var cg = rect.GetComponent<CanvasGroup>();
            if (cg == null) cg = rect.gameObject.AddComponent<CanvasGroup>();

            var startAlpha = isIn ? 0f : 1f;
            var endAlpha = isIn ? 1f : 0f;

            Vector2 offscreenPos;
            Vector3 offscreenScale;
            GetOffscreenState(rect, type, out offscreenPos, out offscreenScale);

            var startPos = isIn ? offscreenPos : Vector2.zero;
            var endPos = isIn ? Vector2.zero : offscreenPos;
            var startScale = isIn ? offscreenScale : Vector3.one;
            var endScale = isIn ? Vector3.one : offscreenScale;

            cg.alpha = startAlpha;
            rect.anchoredPosition = startPos;
            rect.localScale = startScale;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                t = EaseOutCubic(t);

                cg.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                rect.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return null;
            }

            SetFinalState(rect, type, isIn);
            onComplete?.Invoke();
        }

        private static void GetOffscreenState(RectTransform rect, TransitionType type, out Vector2 pos, out Vector3 scale)
        {
            var width = rect.rect.width > 0 ? rect.rect.width : 400f;
            var height = rect.rect.height > 0 ? rect.rect.height : 600f;

            pos = Vector2.zero;
            scale = Vector3.one * 0.8f;

            switch (type)
            {
                case TransitionType.Fade:
                    pos = Vector2.zero;
                    break;
                case TransitionType.SlideLeft:
                    pos = new Vector2(-width, 0);
                    break;
                case TransitionType.SlideRight:
                    pos = new Vector2(width, 0);
                    break;
                case TransitionType.SlideUp:
                    pos = new Vector2(0, height);
                    break;
                case TransitionType.SlideDown:
                    pos = new Vector2(0, -height);
                    break;
                case TransitionType.Scale:
                    scale = Vector3.one * 0.8f;
                    break;
                default:
                    break;
            }
        }

        private static void SetFinalState(RectTransform rect, TransitionType type, bool isIn)
        {
            var cg = rect.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.alpha = isIn ? 1f : 0f;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        private static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private class TransitionRunner : MonoBehaviour { }
    }
}
