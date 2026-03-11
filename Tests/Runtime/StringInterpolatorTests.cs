using NUnit.Framework;
using UIX.Utilities;

namespace UIX.Tests.Runtime
{
    public class StringInterpolatorTests
    {
        [Test]
        public void HasBindings()
        {
            Assert.IsTrue(StringInterpolator.HasBindings("{Name}"));
            Assert.IsTrue(StringInterpolator.HasBindings("Hello {Name}"));
            Assert.IsFalse(StringInterpolator.HasBindings("Hello"));
        }

        [Test]
        public void ExtractBindings()
        {
            var bindings = StringInterpolator.ExtractBindings("{A} and {B}");
            Assert.AreEqual(2, bindings.Length);
            Assert.AreEqual("A", bindings[0]);
            Assert.AreEqual("B", bindings[1]);
        }

        [Test]
        public void Interpolate()
        {
            var result = StringInterpolator.Interpolate("Hello {Name}", k => k == "Name" ? "World" : null);
            Assert.AreEqual("Hello World", result);
        }

        [Test]
        public void IsTwoWayBinding()
        {
            Assert.IsTrue(StringInterpolator.IsTwoWayBinding("=Value"));
            Assert.IsFalse(StringInterpolator.IsTwoWayBinding("Value"));
        }

        [Test]
        public void GetOneWayExpression()
        {
            Assert.AreEqual("Value", StringInterpolator.GetOneWayExpression("=Value"));
            Assert.AreEqual("Value", StringInterpolator.GetOneWayExpression("Value"));
        }
    }
}
