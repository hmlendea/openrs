using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    [NonParallelizable]
    public sealed class GameImageMinimapTests
    {
        private static int Width => 10;

        private static int Height => 10;

        private static int StandardScale => 128;

        private static int SpiralScale => 192;

        private GameImage image = null!;

        [SetUp]
        public void SetUp()
        {
            GameImage.SpiralDrawCount = 0;
            GameImage.CharacterDrawCount = 0;
            GameImage.LastCharacterRotation = 0;
            image = BuildImage(42);
        }

        [TearDown]
        public void TearDown()
        {
            GameImage.SpiralDrawCount = 0;
            GameImage.CharacterDrawCount = 0;
            GameImage.LastCharacterRotation = 0;
        }

        [Test]
        public void GivenASolidPicture_WhenDrawingItWithoutRotation_ThenItsProjectedSquareIsFilled()
        {
            image.DrawMinimapPic(5, 5, 0, 0, StandardScale);

            Assert.That(image.Pixels.Count(pixel => pixel == 42), Is.EqualTo(16));

            for (int positionY = 3; positionY < 7; positionY += 1)
            {
                for (int positionX = 3; positionX < 7; positionX += 1)
                {
                    Assert.That(GetPixel(positionX, positionY), Is.EqualTo(42));
                }
            }
        }

        [Test]
        public void GivenEquivalentMaskedRotations_WhenDrawingThem_ThenTheirPixelsAreIdentical()
        {
            GameImage firstImage = BuildImage(42);
            GameImage secondImage = BuildImage(42);

            firstImage.DrawMinimapPic(5, 5, 0, 0, StandardScale);
            secondImage.DrawMinimapPic(5, 5, 0, 256, StandardScale);

            Assert.That(secondImage.Pixels, Is.EqualTo(firstImage.Pixels));
        }

        [Test]
        public void GivenATransparentZeroPicture_WhenDrawingIt_ThenExistingPixelsArePreserved()
        {
            Array.Fill(image.Pixels, 64);
            Array.Fill(image.PictureColours[0], 0);
            image.HasTransparentBackground[0] = true;

            image.DrawMinimapPic(5, 5, 0, 0, StandardScale);

            Assert.That(image.Pixels, Has.All.EqualTo(64));
        }

        [Test]
        public void GivenAnOpaqueZeroPicture_WhenDrawingIt_ThenProjectedPixelsAreCleared()
        {
            Array.Fill(image.Pixels, 64);
            Array.Fill(image.PictureColours[0], 0);

            image.DrawMinimapPic(5, 5, 0, 0, StandardScale);

            Assert.That(image.Pixels.Count(pixel => pixel == 0), Is.EqualTo(16));
        }

        [Test]
        public void GivenAnInterlacedPicture_WhenDrawingIt_ThenOnlyEvenScanlinesChange()
        {
            image.IsInterlaced = true;

            image.DrawMinimapPic(5, 5, 0, 0, StandardScale);

            Assert.That(image.Pixels.Count(pixel => pixel == 42), Is.EqualTo(8));
            Assert.That(image.Pixels.Skip(3 * Width).Take(Width), Has.All.Zero);
            Assert.That(image.Pixels.Skip(4 * Width).Take(Width), Has.Some.EqualTo(42));
            Assert.That(image.Pixels.Skip(5 * Width).Take(Width), Has.All.Zero);
            Assert.That(image.Pixels.Skip(6 * Width).Take(Width), Has.Some.EqualTo(42));
        }

        [Test]
        public void GivenNoRotationStorage_WhenDrawingAPicture_ThenTrigonometryAndScanlinesAreInitialised()
        {
            Assert.That(image.CharacterRotationTable, Is.Null);
            Assert.That(image.EntityScanlineMinX, Is.Null);

            image.DrawMinimapPic(5, 5, 0, 0, StandardScale);

            Assert.That(image.CharacterRotationTable, Has.Length.EqualTo(512));
            Assert.That(image.EntityScanlineMinX, Has.Length.EqualTo(Height + 1));
            Assert.That(image.EntityScanlineMaxX, Has.Length.EqualTo(Height + 1));
            Assert.That(image.EntityScanlineMinValue, Has.Length.EqualTo(Height + 1));
            Assert.That(image.EntityScanlineMaxValue, Has.Length.EqualTo(Height + 1));
            Assert.That(image.EntityScanlineMinExtra, Has.Length.EqualTo(Height + 1));
            Assert.That(image.EntityScanlineMaxExtra, Has.Length.EqualTo(Height + 1));
        }

        [Test]
        public void GivenExistingRotationAndScanlineStorage_WhenDrawingAgain_ThenTheArraysAreReused()
        {
            image.DrawMinimapPic(5, 5, 0, 0, StandardScale);
            int[] expectedRotationTable = image.CharacterRotationTable;
            int[] expectedScanlines = image.EntityScanlineMinX;

            image.DrawMinimapPic(5, 5, 0, 64, StandardScale);

            Assert.That(image.CharacterRotationTable, Is.SameAs(expectedRotationTable));
            Assert.That(image.EntityScanlineMinX, Is.SameAs(expectedScanlines));
        }

        [Test]
        public void GivenAStandardScaleDraw_WhenDrawingIt_ThenTheLastRotationIsRecorded()
        {
            image.DrawMinimapPic(5, 5, 0, 42, StandardScale);

            Assert.That(GameImage.LastCharacterRotation, Is.EqualTo(42));
            Assert.That(GameImage.SpiralDrawCount, Is.Zero);
            Assert.That(GameImage.CharacterDrawCount, Is.Zero);
        }

        [Test]
        public void GivenAnAlignedSpiralDraw_WhenDrawingIt_ThenTheSpiralCounterIncreases()
        {
            GameImage.LastCharacterRotation = 4;

            image.DrawMinimapPic(5, 5, 0, 68, SpiralScale);

            Assert.That(GameImage.SpiralDrawCount, Is.EqualTo(1));
            Assert.That(GameImage.CharacterDrawCount, Is.Zero);
            Assert.That(GameImage.LastCharacterRotation, Is.EqualTo(4));
        }

        [TestCase(64, 42)]
        [TestCase(192, 43)]
        [TestCase(256, 42)]
        public void GivenANonStandardUnalignedDraw_WhenDrawingIt_ThenTheCharacterCounterIncreases(
            int scale,
            int rotation)
        {
            GameImage.LastCharacterRotation = 42;

            image.DrawMinimapPic(5, 5, 0, rotation, scale);

            Assert.That(GameImage.CharacterDrawCount, Is.EqualTo(1));
            Assert.That(GameImage.SpiralDrawCount, Is.Zero);
        }

        [Test]
        public void GivenAnInvalidPictureIndex_WhenDrawingIt_ThenAnIndexExceptionIsThrown()
            => Assert.That(
                () => image.DrawMinimapPic(5, 5, 42, 0, StandardScale),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenNullPictureColours_WhenDrawingIt_ThenANullReferenceExceptionIsThrown()
        {
            image.PictureColours[0] = null!;

            Assert.That(
                () => image.DrawMinimapPic(5, 5, 0, 0, StandardScale),
                Throws.TypeOf<NullReferenceException>());
        }

        private static GameImage BuildImage(int colour)
        {
            GameImage targetImage = new(Width, Height, 1);
            targetImage.PictureWidth[0] = 4;
            targetImage.PictureHeight[0] = 4;
            targetImage.PictureAssumedWidth[0] = 4;
            targetImage.PictureAssumedHeight[0] = 4;
            targetImage.PictureColours[0] = new int[16];
            Array.Fill(targetImage.PictureColours[0], colour);

            return targetImage;
        }

        private int GetPixel(int positionX, int positionY)
            => image.Pixels[positionX + positionY * image.GameWidth];
    }
}