using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIX.Utilities;

namespace UIX.Rendering
{
    /// <summary>
    /// Maps CSS layout properties to uGUI LayoutGroup and LayoutElement.
    /// </summary>
    public static class LayoutMapper
    {
        public static void ApplyToRectTransform(RectTransform rect, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null) return;

            if (styles.TryGetValue("width", out var w))
            {
                var val = UnitConverter.ParseLength(w);
                if (!UnitConverter.IsAuto(val))
                {
                    rect.sizeDelta = new Vector2(UnitConverter.IsPercent(val) ? -1 : val, rect.sizeDelta.y);
                }
            }
            if (styles.TryGetValue("height", out var h))
            {
                var val = UnitConverter.ParseLength(h);
                if (!UnitConverter.IsAuto(val))
                {
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, UnitConverter.IsPercent(val) ? -1 : val);
                }
            }
        }

        public static void ApplyToLayoutElement(LayoutElement le, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || le == null) return;

            if (styles.TryGetValue("width", out var w))
            {
                var val = UnitConverter.ParseLength(w);
                if (UnitConverter.IsAuto(val))
                    le.preferredWidth = -1;
                else if (UnitConverter.IsPercent(val))
                    le.flexibleWidth = UnitConverter.GetPercent(val) / 100f;
                else
                    le.preferredWidth = val;
            }
            if (styles.TryGetValue("height", out var h))
            {
                var val = UnitConverter.ParseLength(h);
                if (UnitConverter.IsAuto(val))
                    le.preferredHeight = -1;
                else if (UnitConverter.IsPercent(val))
                    le.flexibleHeight = UnitConverter.GetPercent(val) / 100f;
                else
                    le.preferredHeight = val;
            }
            if (styles.TryGetValue("min-width", out var mw))
                le.minWidth = UnitConverter.ParseLength(mw);
            if (styles.TryGetValue("min-height", out var mh))
                le.minHeight = UnitConverter.ParseLength(mh);
            if (styles.TryGetValue("flex", out var flex))
            {
                var f = UnitConverter.ParseLength(flex);
                if (f > 0) { le.flexibleWidth = f; le.flexibleHeight = f; }
            }
        }

        public static void ApplyToLayoutGroup(HorizontalOrVerticalLayoutGroup group, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || group == null) return;

            if (styles.TryGetValue("padding", out var pad))
            {
                var p = UnitConverter.ParseLength(pad);
                group.padding = new RectOffset((int)p, (int)p, (int)p, (int)p);
            }
            if (styles.TryGetValue("padding-left", out var pl)) group.padding.left = (int)UnitConverter.ParseLength(pl);
            if (styles.TryGetValue("padding-right", out var pr)) group.padding.right = (int)UnitConverter.ParseLength(pr);
            if (styles.TryGetValue("padding-top", out var pt)) group.padding.top = (int)UnitConverter.ParseLength(pt);
            if (styles.TryGetValue("padding-bottom", out var pb)) group.padding.bottom = (int)UnitConverter.ParseLength(pb);
            if (styles.TryGetValue("gap", out var gap))
                group.spacing = UnitConverter.ParseLength(gap);
            if (styles.TryGetValue("align-items", out var align))
                group.childAlignment = ParseAlignment(align);
        }

        public static void ApplyToGridLayoutGroup(GridLayoutGroup grid, IReadOnlyDictionary<string, string> styles)
        {
            if (styles == null || grid == null) return;
            if (styles.TryGetValue("gap", out var gap))
            {
                var g = UnitConverter.ParseLength(gap);
                grid.spacing = new Vector2(g, g);
            }
        }

        private static TextAnchor ParseAlignment(string value)
        {
            if (string.IsNullOrEmpty(value)) return TextAnchor.MiddleCenter;
            switch (value.ToLowerInvariant())
            {
                case "flex-start": return TextAnchor.UpperLeft;
                case "center": return TextAnchor.MiddleCenter;
                case "flex-end": return TextAnchor.LowerRight;
                case "stretch": return TextAnchor.MiddleCenter;
                default: return TextAnchor.MiddleCenter;
            }
        }
    }
}
