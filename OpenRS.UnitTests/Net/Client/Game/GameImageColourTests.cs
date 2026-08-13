using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class GameImageColourTests
    {
        [TestCase(0, 0, 0, 0, 0L)]
        [TestCase(18, 52, 86, 120, 0x78563412L)]
        [TestCase(255, 255, 255, 255, 0xffffffffL)]
        [TestCase(-1, -1, -1, -1, 0L)]
        [TestCase(256, 256, 256, 256, 0xffffffffL)]
        [TestCase(-42, 42, 512, 128, 0x80ff2a00L)]
        public void GivenRgbaChannels_WhenConvertingThem_ThenTheCompatiblePackedColourIsReturned(
            int red,
            int green,
            int blue,
            int alpha,
            long expectedColour)
            => Assert.That(
                GameImage.RgbaToUInt(red, green, blue, alpha),
                Is.EqualTo((uint)expectedColour));

        [TestCase(0, 0, 0, 0)]
        [TestCase(18, 52, 86, 0x123456)]
        [TestCase(255, 255, 255, 0xffffff)]
        [TestCase(4, 8, 16, 0x040810)]
        public void GivenRgbChannels_WhenConvertingThem_ThenTheCompatiblePackedColourIsReturned(
            int red,
            int green,
            int blue,
            int expectedColour)
            => Assert.That(
                GameImage.RgbToInt(red, green, blue),
                Is.EqualTo(expectedColour));
    }
}