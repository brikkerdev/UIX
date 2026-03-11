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
    }
}
