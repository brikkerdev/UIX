using NUnit.Framework;
using UIX.Utilities;

namespace UIX.Tests.Runtime
{
    public class UnitConverterTests
    {
        [Test]
        public void ParseLength_Number()
        {
            Assert.IsTrue(UnitConverter.TryParseLength("100", out var r));
            Assert.AreEqual(100f, r);
        }

        [Test]
        public void ParseLength_Px()
        {
            Assert.IsTrue(UnitConverter.TryParseLength("24px", out var r));
            Assert.AreEqual(24f, r);
        }

        [Test]
        public void ParseLength_Percent()
        {
            Assert.IsTrue(UnitConverter.TryParseLength("50%", out var r));
            Assert.AreEqual(-50f, r);
        }

        [Test]
        public void ParseLength_Auto()
        {
            Assert.IsTrue(UnitConverter.TryParseLength("auto", out var r));
            Assert.AreEqual(UnitConverter.Auto, r);
        }

        [Test]
        public void IsPercent()
        {
            Assert.IsTrue(UnitConverter.IsPercent(-50f));
            Assert.IsFalse(UnitConverter.IsPercent(50f));
            Assert.IsFalse(UnitConverter.IsPercent(UnitConverter.Auto));
        }

        [Test]
        public void IsAuto()
        {
            Assert.IsTrue(UnitConverter.IsAuto(UnitConverter.Auto));
        }

        [Test]
        public void GetPercent()
        {
            Assert.AreEqual(50f, UnitConverter.GetPercent(-50f));
            Assert.AreEqual(0f, UnitConverter.GetPercent(50f));
        }
    }
}
