using NUnit.Framework;
using UIX.Styling;

namespace UIX.Tests.Runtime
{
    public class VariableResolverTests
    {
        [Test]
        public void SetVariable_Resolve()
        {
            var vr = new VariableResolver();
            vr.SetVariable("--color-bg", "#0D0D0D");
            var result = vr.Resolve("var(--color-bg)");
            Assert.AreEqual("#0D0D0D", result);
        }

        [Test]
        public void Resolve_WithFallback_WhenVariableMissing()
        {
            var vr = new VariableResolver();
            var result = vr.Resolve("var(--missing, #000000)");
            Assert.AreEqual("#000000", result);
        }

        [Test]
        public void SetVariables_ReplacesAll()
        {
            var vr = new VariableResolver();
            vr.SetVariable("--a", "1");
            vr.SetVariables(new[] { new System.Collections.Generic.KeyValuePair<string, string>("--b", "2") });
            Assert.IsFalse(vr.TryGetVariable("--a", out _));
            Assert.IsTrue(vr.TryGetVariable("--b", out var v));
            Assert.AreEqual("2", v);
        }

        [Test]
        public void TryGetVariable()
        {
            var vr = new VariableResolver();
            vr.SetVariable("--x", "value");
            Assert.IsTrue(vr.TryGetVariable("--x", out var v));
            Assert.AreEqual("value", v);
            Assert.IsFalse(vr.TryGetVariable("--y", out _));
        }
    }
}
