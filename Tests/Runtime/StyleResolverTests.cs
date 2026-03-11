using System.Collections.Generic;
using NUnit.Framework;
using UIX.Parsing.Tokens;
using UIX.Styling;

namespace UIX.Tests.Runtime
{
    public class StyleResolverTests
    {
        [Test]
        public void ResolveEmptyStyles()
        {
            var vr = new VariableResolver();
            var sr = new StyleResolver(vr);
            var result = sr.Resolve("text", "", new[] { "title" });
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Resolve_ClassSelector()
        {
            var vr = new VariableResolver();
            var sr = new StyleResolver(vr);
            sr.AddRules(new[]
            {
                new StyleRule
                {
                    Selector = new StyleSelector { Classes = new List<string> { "title" } },
                    Properties = new Dictionary<string, string> { ["font-size"] = "24", ["color"] = "#333333" }
                }
            });
            var result = sr.Resolve("text", "", new[] { "title" });
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("24", result["font-size"]);
            Assert.AreEqual("#333333", result["color"]);
        }

        [Test]
        public void Resolve_ElementTypeSelector()
        {
            var vr = new VariableResolver();
            var sr = new StyleResolver(vr);
            sr.AddRules(new[]
            {
                new StyleRule
                {
                    Selector = new StyleSelector { ElementType = "button" },
                    Properties = new Dictionary<string, string> { ["font-size"] = "16" }
                }
            });
            var result = sr.Resolve("button", "", new string[0]);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("16", result["font-size"]);
        }

        [Test]
        public void Resolve_VariableInValue()
        {
            var vr = new VariableResolver();
            vr.SetVariable("--color-bg", "#FFFFFF");
            var sr = new StyleResolver(vr);
            sr.AddRules(new[]
            {
                new StyleRule
                {
                    Selector = new StyleSelector { Classes = new List<string> { "box" } },
                    Properties = new Dictionary<string, string> { ["background-color"] = "var(--color-bg)" }
                }
            });
            var result = sr.Resolve("container", "", new[] { "box" });
            Assert.AreEqual("#FFFFFF", result["background-color"]);
        }
    }
}
