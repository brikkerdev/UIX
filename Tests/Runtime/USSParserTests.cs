using NUnit.Framework;
using UIX.Parsing;

namespace UIX.Tests.Runtime
{
    public class USSParserTests
    {
        [Test]
        public void Parse_RootVariables()
        {
            var uss = @":root {
    --color-bg: #0D0D0D;
    --spacing-md: 16;
}";
            var result = USSParser.Parse(uss);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Variables.Count);
            Assert.AreEqual("--color-bg", result.Variables[0].Name);
            Assert.AreEqual("#0D0D0D", result.Variables[0].Value);
            Assert.AreEqual("--spacing-md", result.Variables[1].Name);
            Assert.AreEqual("16", result.Variables[1].Value);
        }

        [Test]
        public void Parse_StyleRules()
        {
            var uss = @".title {
    font-size: 28;
    color: #333333;
}";
            var result = USSParser.Parse(uss);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Rules.Count);
            Assert.AreEqual(2, result.Rules[0].Properties.Count);
            Assert.IsTrue(result.Rules[0].Properties.ContainsKey("font-size"));
            Assert.AreEqual("28", result.Rules[0].Properties["font-size"]);
            Assert.AreEqual("#333333", result.Rules[0].Properties["color"]);
        }

        [Test]
        public void ResolveVar_WithVariable()
        {
            var vars = new System.Collections.Generic.Dictionary<string, string>
            {
                ["--color-bg"] = "#FFFFFF"
            };
            var result = USSParser.ResolveVar("background-color: var(--color-bg)", vars);
            Assert.AreEqual("background-color: #FFFFFF", result);
        }

        [Test]
        public void ResolveVar_WithFallback()
        {
            var vars = new System.Collections.Generic.Dictionary<string, string>();
            var result = USSParser.ResolveVar("color: var(--missing, #000000)", vars);
            Assert.AreEqual("color: #000000", result);
        }

        [Test]
        public void Parse_IgnoresComments()
        {
            var uss = @"/* comment */
.title { /* inline */ font-size: 16; }";
            var result = USSParser.Parse(uss);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Rules.Count);
            Assert.AreEqual("16", result.Rules[0].Properties["font-size"]);
        }

        [Test]
        public void Parse_RootAndRule_SameAsCompilerTestInput()
        {
            var uss = @":root { --color: #FFF; }
.title { font-size: 24; }";
            var result = USSParser.Parse(uss);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Variables.Count);
            Assert.AreEqual("--color", result.Variables[0].Name);
            Assert.AreEqual(1, result.Rules.Count);
            Assert.AreEqual("24", result.Rules[0].Properties["font-size"]);
        }
    }
}
