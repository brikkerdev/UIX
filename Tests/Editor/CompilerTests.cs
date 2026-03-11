using System.IO;
using NUnit.Framework;
using UnityEngine;
using UIX.Parsing;
using UIX.Parsing.Nodes;
using UIX.Editor.Pipeline;
using UIX.Templates;

namespace UIX.Tests.Editor
{
    public class CompilerTests
    {
        [Test]
        public void XMLParser_ParsesComponent()
        {
            var xml = "<component name=\"Test\"><template><column /></template></component>";
            var root = XMLParser.Parse(xml);
            Assert.IsNotNull(root);
            Assert.AreEqual("Test", root.Name);
        }

        [Test]
        public void UIXCompiler_CompileXml_CreatesTemplate()
        {
            var xml = @"<screen name=""CompileTest"" viewmodel=""CompileTestVM"">
  <template><column><text text=""Hello"" /></column></template>
</screen>";
            var template = UIXCompiler.CompileXml("Assets/UI/Screens/CompileTest/CompileTest.xml", xml);
            Assert.IsNotNull(template);
            Assert.AreEqual("CompileTest", template.TemplateName);
            Assert.IsFalse(template.IsComponent);
            Assert.AreEqual("CompileTestVM", template.ViewModelType);
        }

        [Test]
        public void UIXCompiler_CompileUss_CreatesStyleSheet()
        {
            var uss = @":root { --color: #FFF; }
.title { font-size: 24; }";
            var result = UIXCompiler.CompileUss("Assets/UI/Screens/Test/Test.uss", uss);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Variables.Count);
            Assert.AreEqual(1, result.Rules.Count);
        }
    }
}
