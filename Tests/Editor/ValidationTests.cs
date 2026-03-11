using NUnit.Framework;
using UIX.Editor.Pipeline;

namespace UIX.Tests.Editor
{
    public class ValidationTests
    {
        [Test]
        public void ValidateXml_ValidScreen_ReturnsValid()
        {
            var xml = @"<screen name=""Test"">
  <template><column><text text=""Hello"" /></column></template>
</screen>";
            var result = UIXValidation.ValidateXml("Assets/UI/Screens/Test/Test.xml", xml);
            Assert.IsTrue(result.Valid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ValidateXml_InvalidXml_ReturnsErrors()
        {
            var xml = "<screen name=\"Test\"><template><column>";
            var result = UIXValidation.ValidateXml("Test.xml", xml);
            Assert.IsFalse(result.Valid);
            Assert.Greater(result.Errors.Count, 0);
        }

        [Test]
        public void ValidateUss_ValidUss_ReturnsValid()
        {
            var uss = @":root { --color: #FFF; }
.title { font-size: 24; }";
            var result = UIXValidation.ValidateUss("Assets/UI/Screens/Test/Test.uss", uss);
            Assert.IsTrue(result.Valid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ValidateUss_UnknownProperty_AddsWarning()
        {
            var uss = ".test { unknown-prop: 1; }";
            var result = UIXValidation.ValidateUss("Test.uss", uss);
            Assert.IsTrue(result.Valid);
            Assert.Greater(result.Warnings.Count, 0);
        }
    }
}
