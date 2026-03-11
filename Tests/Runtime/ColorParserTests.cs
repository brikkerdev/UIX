using NUnit.Framework;
using UnityEngine;
using UIX.Utilities;

namespace UIX.Tests.Runtime
{
    public class ColorParserTests
    {
        [Test]
        public void TryParse_Hex6()
        {
            Assert.IsTrue(ColorParser.TryParse("#FF0000", out var c));
            Assert.AreEqual(Color.red, c);
        }

        [Test]
        public void TryParse_Hex3()
        {
            Assert.IsTrue(ColorParser.TryParse("#F00", out var c));
            Assert.AreEqual(Color.red, c);
        }

        [Test]
        public void TryParse_Hex8()
        {
            Assert.IsTrue(ColorParser.TryParse("#FF000080", out var c));
            Assert.AreEqual(0.5f, c.a, 0.01f);
        }

        [Test]
        public void TryParse_Rgb()
        {
            Assert.IsTrue(ColorParser.TryParse("rgb(255, 0, 0)", out var c));
            Assert.AreEqual(Color.red, c);
        }

        [Test]
        public void TryParse_Rgba()
        {
            Assert.IsTrue(ColorParser.TryParse("rgba(255, 0, 0, 0.5)", out var c));
            Assert.AreEqual(0.5f, c.a, 0.01f);
        }

        [Test]
        public void TryParse_NamedColor()
        {
            Assert.IsTrue(ColorParser.TryParse("white", out var c));
            Assert.AreEqual(Color.white, c);
        }

        [Test]
        public void TryParse_Invalid_ReturnsFalse()
        {
            Assert.IsFalse(ColorParser.TryParse("invalid", out _));
        }

        [Test]
        public void Parse_ReturnsWhite_WhenInvalid()
        {
            var c = ColorParser.Parse("invalid");
            Assert.AreEqual(Color.white, c);
        }
    }
}
