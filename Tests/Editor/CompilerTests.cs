using NUnit.Framework;
using UnityEngine;
using UIX.Parsing;
using UIX.Parsing.Nodes;

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
    }
}
