using NUnit.Framework;

using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    public sealed class ItemSpriteDrawCallTests
    {
        [Test]
        public void GivenADefaultDrawCall_WhenReadingIt_ThenNumericValuesAreZeroAndTheNameIsNull()
        {
            ItemSpriteDrawCall drawCall = default;

            Assert.That(drawCall.PixelX, Is.Zero);
            Assert.That(drawCall.PixelY, Is.Zero);
            Assert.That(drawCall.PixelWidth, Is.Zero);
            Assert.That(drawCall.PixelHeight, Is.Zero);
            Assert.That(drawCall.SpriteName, Is.Null);
        }

        [TestCase(4, 8, 16, 32, "RuneScape")]
        [TestCase(-42, -64, -96, -128, "Dark Souls III")]
        [TestCase(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, "Minecraft")]
        public void GivenDrawCallValues_WhenInitialisingIt_ThenEveryValueIsRetained(
            int pixelX,
            int pixelY,
            int pixelWidth,
            int pixelHeight,
            string spriteName)
        {
            ItemSpriteDrawCall drawCall = new()
            {
                PixelX = pixelX,
                PixelY = pixelY,
                PixelWidth = pixelWidth,
                PixelHeight = pixelHeight,
                SpriteName = spriteName,
            };

            Assert.That(drawCall.PixelX, Is.EqualTo(pixelX));
            Assert.That(drawCall.PixelY, Is.EqualTo(pixelY));
            Assert.That(drawCall.PixelWidth, Is.EqualTo(pixelWidth));
            Assert.That(drawCall.PixelHeight, Is.EqualTo(pixelHeight));
            Assert.That(drawCall.SpriteName, Is.EqualTo(spriteName));
        }
    }
}