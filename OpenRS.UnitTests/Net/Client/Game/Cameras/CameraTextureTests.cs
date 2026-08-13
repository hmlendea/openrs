using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    [NonParallelizable]
    public sealed class CameraTextureTests
    {
        private static int Texture64PixelCount => 4096;

        private static int Texture128PixelCount => 16384;

        private static int Texture64FrameType => 0;

        private static int Texture128FrameType => 1;

        private static int NotSetTexture => 0xbc614e;

        private Camera camera = null!;

        [SetUp]
        public void SetUp()
        {
            camera = new Camera(new GameImage(8, 8, 0), 1, 1, 1);
        }

        [Test]
        public void GivenA64PixelTexture_WhenSettingIt_ThenItsFirstQuantisedPixelIsReturned()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[Texture64PixelCount], [0xf80000], Texture64FrameType);

            int colour = camera.ApplyTextureSmoothing(0);

            Assert.That(colour, Is.EqualTo(0xf80000));
        }

        [Test]
        public void GivenA128PixelTexture_WhenSettingIt_ThenItsFirstQuantisedPixelIsReturned()
        {
            camera.CreateTexture(1, 0, 1);
            camera.SetTexture(0, new sbyte[Texture128PixelCount], [0x00f800], Texture128FrameType);

            int colour = camera.ApplyTextureSmoothing(0);

            Assert.That(colour, Is.EqualTo(0x00f800));
        }

        [Test]
        public void GivenAnUnalignedTextureColour_WhenSettingIt_ThenRedAndGreenAreQuantised()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[Texture64PixelCount], [0x123456], Texture64FrameType);

            int colour = camera.ApplyTextureSmoothing(0);

            Assert.That(colour, Is.EqualTo(0x103056));
        }

        [Test]
        public void GivenABlackTextureColour_WhenSettingIt_ThenTheOpaqueBlackSentinelIsReturned()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[Texture64PixelCount], [0x000000], Texture64FrameType);

            int colour = camera.ApplyTextureSmoothing(0);

            Assert.That(colour, Is.EqualTo(1));
        }

        [Test]
        public void GivenTheTransparentTextureColour_WhenSettingIt_ThenAZeroPixelIsReturned()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[Texture64PixelCount], [0xf800ff], Texture64FrameType);

            int colour = camera.ApplyTextureSmoothing(0);

            Assert.That(colour, Is.Zero);
        }

        [TestCase(0, 0, 0, 0x000000)]
        [TestCase(255, 0, 0, 0xf80000)]
        [TestCase(0, 255, 0, 0x00f800)]
        [TestCase(0, 0, 255, 0x0000f8)]
        [TestCase(255, 255, 255, 0xf8f8f8)]
        [TestCase(128, 64, 32, 0x804020)]
        public void GivenANegativeTextureColour_WhenSmoothingIt_ThenFiveBitChannelsAreExpanded(
            int red,
            int green,
            int blue,
            int expectedColour)
        {
            int textureColour = Camera.GetTextureColour(red, green, blue);

            Assert.That(camera.ApplyTextureSmoothing(textureColour), Is.EqualTo(expectedColour));
        }

        [Test]
        public void GivenTheNotSetTexture_WhenSmoothingIt_ThenZeroIsReturned()
            => Assert.That(camera.ApplyTextureSmoothing(NotSetTexture), Is.Zero);

        [Test]
        public void GivenAnAllocatedButUnsetTexture_WhenUpdatingItsLighting_ThenNoExceptionIsThrown()
        {
            camera.CreateTexture(1, 1, 0);

            Assert.That(() => camera.UpdateLighting(0), Throws.Nothing);
        }

        [Test]
        public void GivenANegativeTextureIndex_WhenUpdatingItsSmoothing_ThenNoExceptionIsThrown()
            => Assert.That(() => camera.UpdateTextureSmoothing(-1), Throws.Nothing);

        [Test]
        public void GivenA64PixelTexture_WhenUpdatingItsLighting_ThenTheLastRowMovesToTheFirstRow()
        {
            sbyte[] colourIndices = new sbyte[Texture64PixelCount];
            colourIndices[Texture64PixelCount - 64] = 1;
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(
                0,
                colourIndices,
                [0xf80000, 0x00f800],
                Texture64FrameType);

            camera.UpdateLighting(0);

            Assert.That(camera.ApplyTextureSmoothing(0), Is.EqualTo(0x00f800));
        }

        [Test]
        public void GivenTwo64PixelTexturesAndOneBuffer_WhenSettingTheSecond_ThenTheOldestBufferIsReused()
        {
            camera.CreateTexture(2, 1, 0);
            camera.SetTexture(0, new sbyte[Texture64PixelCount], [0xf80000], Texture64FrameType);

            camera.SetTexture(1, new sbyte[Texture64PixelCount], [0x00f800], Texture64FrameType);

            Assert.That(camera.ApplyTextureSmoothing(1), Is.EqualTo(0x00f800));
            Assert.That(camera.ApplyTextureSmoothing(0), Is.EqualTo(0xf80000));
        }

        [Test]
        public void GivenTwo128PixelTexturesAndOneBuffer_WhenSettingTheSecond_ThenTheOldestBufferIsReused()
        {
            camera.CreateTexture(2, 0, 1);
            camera.SetTexture(0, new sbyte[Texture128PixelCount], [0xf80000], Texture128FrameType);

            camera.SetTexture(1, new sbyte[Texture128PixelCount], [0x00f800], Texture128FrameType);

            Assert.That(camera.ApplyTextureSmoothing(1), Is.EqualTo(0x00f800));
            Assert.That(camera.ApplyTextureSmoothing(0), Is.EqualTo(0xf80000));
        }

        [Test]
        public void GivenMultipleTextureColours_WhenSettingASelectedIndex_ThenThatPaletteColourIsUsed()
        {
            sbyte[] colourIndices = new sbyte[Texture64PixelCount];
            Array.Fill(colourIndices, (sbyte)1);
            camera.CreateTexture(1, 1, 0);

            camera.SetTexture(
                0,
                colourIndices,
                [0xf80000, 0x00f800],
                Texture64FrameType);

            Assert.That(camera.ApplyTextureSmoothing(0), Is.EqualTo(0x00f800));
        }

        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(42)]
        public void GivenAnInvalidTextureIndex_WhenSettingIt_ThenAnIndexExceptionIsThrown(int textureIndex)
        {
            camera.CreateTexture(1, 1, 0);

            Assert.That(
                () => camera.SetTexture(
                    textureIndex,
                    new sbyte[Texture64PixelCount],
                    [0xf80000],
                    Texture64FrameType),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenNullColourIndices_WhenSettingATexture_ThenANullReferenceExceptionIsThrown()
        {
            camera.CreateTexture(1, 1, 0);

            Assert.That(
                () => camera.SetTexture(0, null!, [0xf80000], Texture64FrameType),
                Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void GivenInsufficientColourIndices_WhenSettingATexture_ThenAnIndexExceptionIsThrown()
        {
            camera.CreateTexture(1, 1, 0);

            Assert.That(
                () => camera.SetTexture(0, [], [0xf80000], Texture64FrameType),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenAnInvalidPaletteIndex_WhenSettingATexture_ThenAnIndexExceptionIsThrown()
        {
            sbyte[] colourIndices = new sbyte[Texture64PixelCount];
            Array.Fill(colourIndices, (sbyte)1);
            camera.CreateTexture(1, 1, 0);

            Assert.That(
                () => camera.SetTexture(0, colourIndices, [0xf80000], Texture64FrameType),
                Throws.TypeOf<IndexOutOfRangeException>());
        }
    }
}