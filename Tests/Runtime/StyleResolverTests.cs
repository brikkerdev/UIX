using NUnit.Framework;
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
    }
}
