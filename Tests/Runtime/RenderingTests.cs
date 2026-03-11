using NUnit.Framework;
using UnityEngine;
using UIX.Parsing;
using UIX.Parsing.Nodes;
using UIX.Rendering;
using UIX.Styling;

namespace UIX.Tests.Runtime
{
    public class RenderingTests
    {
        [Test]
        public void UIXRenderer_RendersColumn()
        {
            var vr = new VariableResolver();
            var sr = new StyleResolver(vr);
            var renderer = new UIXRenderer(sr, vr);

            var xml = @"<screen name=""Test"" viewmodel=""TestVM"">
  <template><column class=""screen""><text text=""Hello"" /></column></template>
</screen>";
            var root = XMLParser.Parse(xml);
            Assert.IsNotNull(root);

            var parent = new GameObject("Parent").transform;
            var result = renderer.Render(root, parent);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, parent.childCount);
            var column = parent.GetChild(0);
            Assert.IsNotNull(column.GetComponent<RectTransform>());
            Assert.IsNotNull(column.GetComponent<UnityEngine.UI.VerticalLayoutGroup>());
            Assert.Greater(column.childCount, 0);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void UIXRenderer_RendersButton()
        {
            var vr = new VariableResolver();
            var sr = new StyleResolver(vr);
            var renderer = new UIXRenderer(sr, vr);

            var xml = @"<screen name=""Test"">
  <template><button><text text=""Click"" /></button></template>
</screen>";
            var root = XMLParser.Parse(xml);
            var parent = new GameObject("Parent").transform;
            var result = renderer.Render(root, parent);

            Assert.IsNotNull(result);
            var button = parent.GetComponentInChildren<UnityEngine.UI.Button>();
            Assert.IsNotNull(button);

            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void UIXRenderer_RendersText()
        {
            var vr = new VariableResolver();
            var sr = new StyleResolver(vr);
            var renderer = new UIXRenderer(sr, vr);

            var xml = @"<screen name=""Test"">
  <template><text text=""Hello World"" /></template>
</screen>";
            var root = XMLParser.Parse(xml);
            var parent = new GameObject("Parent").transform;
            var result = renderer.Render(root, parent);

            Assert.IsNotNull(result);
            var tmp = parent.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            Assert.IsNotNull(tmp);
            Assert.AreEqual("Hello World", tmp.text);

            Object.DestroyImmediate(parent.gameObject);
        }
    }
}
