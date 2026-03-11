using UnityEngine;
using UnityEngine.UI;
using UIX.Parsing.Nodes;
using UIX.Utilities;

namespace UIX.Rendering.ElementRenderers
{
    public class GridRenderer : IElementRenderer
    {
        public GameObject Render(ElementNode node, Transform parent, RenderContext context)
        {
            var go = new GameObject(node.TagName);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = Vector2.zero;

            var grid = go.AddComponent<GridLayoutGroup>();
            var cols = node.GetAttribute("columns");
            if (!string.IsNullOrEmpty(cols) && int.TryParse(cols, out var c))
                grid.constraintCount = c;
            var cellSize = node.GetAttribute("cellSize");
            if (!string.IsNullOrEmpty(cellSize))
            {
                var parts = cellSize.Split(',');
                if (parts.Length >= 2 && float.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var w)
                    && float.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var h))
                    grid.cellSize = new Vector2(w, h);
            }

            if (context?.ResolvedStyles != null)
            {
                LayoutMapper.ApplyToGridLayoutGroup(grid, context.ResolvedStyles);
                LayoutMapper.ApplyToRectTransform(rect, context.ResolvedStyles);
            }

            return go;
        }
    }
}
