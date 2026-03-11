using NUnit.Framework;
using UIX.Parsing;
using UIX.Parsing.Nodes;

namespace UIX.Tests.Runtime
{
    public class ParsingTests
    {
        [Test]
        public void ParseSimpleComponent()
        {
            var xml = @"<component name=""Test"">
  <props><prop name=""text"" type=""string"" /></props>
  <template><column><text text=""Hello"" /></column></template>
</component>";
            var root = XMLParser.Parse(xml);
            Assert.IsNotNull(root);
            Assert.IsTrue(root.IsComponent);
            Assert.AreEqual("Test", root.Name);
            Assert.AreEqual(1, root.Children.Count);
        }

        [Test]
        public void ParseScreen()
        {
            var xml = @"<screen name=""Main"" viewmodel=""MainViewModel"">
  <template><column /></template>
</screen>";
            var root = XMLParser.Parse(xml);
            Assert.IsNotNull(root);
            Assert.IsFalse(root.IsComponent);
            Assert.AreEqual("Main", root.Name);
            Assert.AreEqual("MainViewModel", root.ViewModelType);
        }

        [Test]
        public void Parse_ElementWithIdAndClass()
        {
            var xml = @"<screen name=""Test"">
  <template><button id=""btn-1"" class=""primary""><text text=""OK"" /></button></template>
</screen>";
            var root = XMLParser.Parse(xml);
            Assert.IsNotNull(root);
            var child = root.Children[0] as Parsing.Nodes.ElementNode;
            Assert.IsNotNull(child);
            Assert.AreEqual("btn-1", child.Id);
            Assert.AreEqual("primary", child.Class);
        }

        [Test]
        public void Parse_Props()
        {
            var xml = @"<component name=""Card"">
  <props><prop name=""title"" type=""string"" default=""Untitled"" /></props>
  <template><column /></template>
</component>";
            var root = XMLParser.Parse(xml);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Props.Count);
            Assert.AreEqual("title", root.Props[0].Name);
            Assert.AreEqual("string", root.Props[0].Type);
            Assert.AreEqual("Untitled", root.Props[0].Default);
        }
    }
}
