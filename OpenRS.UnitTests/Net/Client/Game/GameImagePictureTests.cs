using System;
using System.Collections.Generic;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class GameImagePictureTests
    {
        private static int PictureCapacity => 4;

        private static int SleepSpriteWidth => 255;

        private static int SleepSpriteHeight => 40;

        private GameImage image = null!;

        [SetUp]
        public void SetUp()
        {
            image = new GameImage(4, 3, PictureCapacity);
        }

        [Test]
        public void GivenLinearSpriteData_WhenUnpackingIt_ThenMetadataPaletteAndPixelsAreStored()
        {
            sbyte[] imageData = [0, 0, 1, 2, 0, 1];
            sbyte[] metadata = BuildMetadata(
                4,
                8,
                0,
                0,
                2,
                2,
                0,
                0x040810,
                0x203040);

            image.UnpackImageData(1, imageData, metadata, 1);

            Assert.That(image.PictureOffsetX[1], Is.Zero);
            Assert.That(image.PictureOffsetY[1], Is.Zero);
            Assert.That(image.PictureWidth[1], Is.EqualTo(2));
            Assert.That(image.PictureHeight[1], Is.EqualTo(2));
            Assert.That(image.PictureAssumedWidth[1], Is.EqualTo(4));
            Assert.That(image.PictureAssumedHeight[1], Is.EqualTo(8));
            Assert.That(image.PictureColourIndexes[1], Is.EqualTo(new sbyte[] { 1, 2, 0, 1 }));
            Assert.That(image.PictureColour[1], Is.EqualTo(new[] { 0xff00ff, 0x040810, 0x203040 }));
            Assert.That(image.PictureColours[1], Is.Null);
            Assert.That(image.HasTransparentBackground[1]);
        }

        [Test]
        public void GivenColumnMajorSpriteData_WhenUnpackingIt_ThenPixelsAreConvertedToRowMajorStorage()
        {
            sbyte[] imageData = [0, 0, 1, 2, 3, 4, 5, 6];
            sbyte[] metadata = BuildMetadata(
                2,
                3,
                0,
                0,
                2,
                3,
                1,
                0x040404,
                0x080808,
                0x101010,
                0x202020,
                0x404040,
                0x808080);

            image.UnpackImageData(0, imageData, metadata, 1);

            Assert.That(
                image.PictureColourIndexes[0],
                Is.EqualTo(new sbyte[] { 1, 4, 2, 5, 3, 6 }));
            Assert.That(image.HasTransparentBackground[0], Is.False);
        }

        [Test]
        public void GivenATransparentColumnMajorPixel_WhenUnpackingIt_ThenTransparencyIsRecorded()
        {
            sbyte[] imageData = [0, 0, 1, 0, 1, 1];
            sbyte[] metadata = BuildMetadata(2, 2, 0, 0, 2, 2, 1, 0x040810);

            image.UnpackImageData(0, imageData, metadata, 1);

            Assert.That(image.PictureColourIndexes[0], Is.EqualTo(new sbyte[] { 1, 1, 0, 1 }));
            Assert.That(image.HasTransparentBackground[0]);
        }

        [Test]
        public void GivenNonZeroSpriteOffsets_WhenUnpackingIt_ThenTransparencyIsRecorded()
        {
            sbyte[] imageData = [0, 0, 1];
            sbyte[] metadata = BuildMetadata(4, 8, 16, 32, 1, 1, 0, 0x040810);

            image.UnpackImageData(0, imageData, metadata, 1);

            Assert.That(image.PictureOffsetX[0], Is.EqualTo(16));
            Assert.That(image.PictureOffsetY[0], Is.EqualTo(32));
            Assert.That(image.HasTransparentBackground[0]);
        }

        [Test]
        public void GivenAnUnsupportedScanOrder_WhenUnpackingIt_ThenAllocatedPixelsRemainZero()
        {
            sbyte[] imageData = [0, 0];
            sbyte[] metadata = BuildMetadata(2, 2, 0, 0, 2, 2, 42, 0x040810);

            image.UnpackImageData(0, imageData, metadata, 1);

            Assert.That(image.PictureColourIndexes[0], Is.EqualTo(new sbyte[4]));
            Assert.That(image.HasTransparentBackground[0], Is.False);
        }

        [Test]
        public void GivenMoreSpritesThanCapacity_WhenUnpackingThem_ThenProcessingStopsAtCapacity()
        {
            sbyte[] imageData = [0, 0, 1];
            sbyte[] metadata = BuildMetadata(1, 1, 0, 0, 1, 1, 0, 0x040810);

            image.UnpackImageData(PictureCapacity - 1, imageData, metadata, 42);

            Assert.That(image.PictureColourIndexes[PictureCapacity - 1], Is.EqualTo(new sbyte[] { 1 }));
        }

        [Test]
        public void GivenANegativePictureIndex_WhenUnpackingData_ThenAnIndexExceptionIsThrown()
        {
            sbyte[] imageData = [0, 0, 1];
            sbyte[] metadata = BuildMetadata(1, 1, 0, 0, 1, 1, 0, 0x040810);

            Assert.That(
                () => image.UnpackImageData(-1, imageData, metadata, 1),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenNullImageData_WhenUnpackingIt_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => image.UnpackImageData(0, null!, [], 1),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenNullMetadata_WhenUnpackingIt_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => image.UnpackImageData(0, [0, 0], null!, 1),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenDirectColours_WhenApplyingAnImage_ThenAnIndexedPaletteIsCreated()
        {
            image.PictureWidth[0] = 2;
            image.PictureHeight[0] = 2;
            image.PictureColours[0] = [0x123456, 0x123456, 0x123456, 0x123456];

            image.ApplyImage(0);

            Assert.That(image.PictureColours[0], Is.Null);
            Assert.That(image.PictureColourIndexes[0], Is.EqualTo(new sbyte[] { 1, 1, 1, 1 }));
            Assert.That(image.PictureColour[0], Has.Length.EqualTo(256));
            Assert.That(image.PictureColour[0][0], Is.EqualTo(0xff00ff));
            Assert.That(image.PictureColour[0][1], Is.EqualTo(0x143454));
        }

        [Test]
        public void GivenColoursWithDifferentFrequencies_WhenApplyingAnImage_ThenPaletteRankIsPreserved()
        {
            image.PictureWidth[0] = 3;
            image.PictureHeight[0] = 1;
            image.PictureColours[0] = [0x00f800, 0x00f800, 0xf80000];

            image.ApplyImage(0);

            Assert.That(image.PictureColourIndexes[0], Is.EqualTo(new sbyte[] { 1, 1, 2 }));
            Assert.That(image.PictureColour[0][1], Is.EqualTo(0x04fc04));
            Assert.That(image.PictureColour[0][2], Is.EqualTo(0xfc0404));
        }

        [Test]
        public void GivenAnEmptyDirectImage_WhenApplyingIt_ThenEmptyIndexedStorageIsCreated()
        {
            image.PictureWidth[0] = 0;
            image.PictureHeight[0] = 0;
            image.PictureColours[0] = null!;

            image.ApplyImage(0);

            Assert.That(image.PictureColourIndexes[0], Is.Empty);
            Assert.That(image.PictureColour[0], Has.Length.EqualTo(256));
            Assert.That(image.PictureColours[0], Is.Null);
        }

        [Test]
        public void GivenIndexedColours_WhenLoadingAnImage_ThenSentinelsAndColoursAreResolved()
        {
            image.PictureWidth[0] = 3;
            image.PictureHeight[0] = 1;
            image.PictureColourIndexes[0] = [0, 1, 2];
            image.PictureColour[0] = [0x000000, 0xff00ff, 0x040810];

            image.LoadImage(0);

            Assert.That(image.PictureColours[0], Is.EqualTo(new[] { 1, 0, 0x040810 }));
            Assert.That(image.PictureColourIndexes[0], Is.Null);
            Assert.That(image.PictureColour[0], Is.Null);
        }

        [Test]
        public void GivenNoIndexedColours_WhenLoadingAnImage_ThenExistingDirectColoursRemainUnchanged()
        {
            int[] expectedColours = [4, 8, 16];
            image.PictureColours[0] = expectedColours;

            image.LoadImage(0);

            Assert.That(image.PictureColours[0], Is.SameAs(expectedColours));
        }

        [Test]
        public void GivenImagePixels_WhenFillingAPicture_ThenColumnMajorCaptureOrderIsUsed()
        {
            image.Pixels =
            [
                4, 8, 16, 32,
                42, 48, 64, 96,
                128, 256, 512, 1024,
            ];

            image.FillPicture(0, 1, 0, 2, 3);

            Assert.That(image.PictureColours[0], Is.EqualTo(new[] { 8, 48, 256, 16, 64, 512 }));
            AssertPictureMetadata(0, 2, 3);
        }

        [Test]
        public void GivenImagePixels_WhenDrawingAnImage_ThenRowMajorCaptureOrderIsUsed()
        {
            image.Pixels =
            [
                4, 8, 16, 32,
                42, 48, 64, 96,
                128, 256, 512, 1024,
            ];

            image.DrawImage(0, 1, 0, 2, 3);

            Assert.That(image.PictureColours[0], Is.EqualTo(new[] { 8, 16, 48, 64, 256, 512 }));
            AssertPictureMetadata(0, 2, 3);
        }

        [Test]
        public void GivenAColumnAtTheImageWidth_WhenFillingAPicture_ThenTheNextLinearRowIsCaptured()
        {
            image.Pixels[image.GameWidth] = 42;

            image.FillPicture(0, image.GameWidth, 0, 1, 1);

            Assert.That(image.PictureColours[0], Is.EqualTo(new[] { 42 }));
        }

        [Test]
        public void GivenARowAtTheImageHeight_WhenDrawingAnImage_ThenAnIndexExceptionIsThrown()
        {
            Assert.That(
                () => image.DrawImage(0, 0, image.GameHeight, 1, 1),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenACompleteSleepSprite_WhenSettingIt_ThenAZeroColourImageIsCreated()
        {
            sbyte[] spriteData = new sbyte[SleepSpriteHeight + 1];
            Array.Fill(spriteData, unchecked((sbyte)255));

            image.SetSleepSprite(0, spriteData);

            Assert.That(image.PictureColours[0], Has.Length.EqualTo(SleepSpriteWidth * SleepSpriteHeight));
            Assert.That(image.PictureColours[0], Has.All.Zero);
            AssertPictureMetadata(0, SleepSpriteWidth, SleepSpriteHeight);
        }

        [Test]
        public void GivenAnAlternatingFirstSleepSpriteRow_WhenSettingIt_ThenSubsequentRowsCopyIt()
        {
            sbyte[] spriteData = new sbyte[SleepSpriteHeight + 2];
            spriteData[1] = 1;
            spriteData[2] = unchecked((sbyte)254);

            for (int dataIndex = 3; dataIndex < spriteData.Length; dataIndex += 1)
            {
                spriteData[dataIndex] = unchecked((sbyte)255);
            }

            image.SetSleepSprite(0, spriteData);

            Assert.That(image.PictureColours[0][0], Is.Zero);
            Assert.That(image.PictureColours[0][1], Is.EqualTo(0xffffff));
            Assert.That(image.PictureColours[0][SleepSpriteWidth], Is.Zero);
            Assert.That(image.PictureColours[0][SleepSpriteWidth + 1], Is.EqualTo(0xffffff));
        }

        [Test]
        public void GivenADifferentialSleepSpriteRow_WhenSettingIt_ThenAChangedPixelPrecedesCopiedPixels()
        {
            sbyte[] spriteData = new sbyte[SleepSpriteHeight + 2];
            spriteData[1] = unchecked((sbyte)255);
            spriteData[2] = 0;
            spriteData[3] = unchecked((sbyte)254);

            for (int dataIndex = 4; dataIndex < spriteData.Length; dataIndex += 1)
            {
                spriteData[dataIndex] = unchecked((sbyte)255);
            }

            image.SetSleepSprite(0, spriteData);

            Assert.That(image.PictureColours[0][SleepSpriteWidth], Is.EqualTo(0xffffff));
            Assert.That(image.PictureColours[0][SleepSpriteWidth + 1], Is.Zero);
            Assert.That(image.PictureColours[0][SleepSpriteWidth * 2], Is.EqualTo(0xffffff));
        }

        [Test]
        public void GivenTruncatedSleepSpriteData_WhenSettingIt_ThenPartialStorageIsRetainedWithoutThrowing()
        {
            Assert.That(() => image.SetSleepSprite(0, []), Throws.Nothing);
            Assert.That(image.PictureColours[0], Has.Length.EqualTo(SleepSpriteWidth * SleepSpriteHeight));
            AssertPictureMetadata(0, SleepSpriteWidth, SleepSpriteHeight);
        }

        [Test]
        public void GivenStoredPictures_WhenCleaningUp_ThenAllocatedPictureDataAndDimensionsAreCleared()
        {
            image.PictureColours[0] = [4, 8];
            image.PictureColourIndexes[0] = [1, 2];
            image.PictureColour[0] = [0x040810];
            image.PictureWidth[0] = 16;
            image.PictureHeight[0] = 32;
            image.PictureOffsetX[0] = 42;
            image.PictureAssumedWidth[0] = 64;
            image.HasTransparentBackground[0] = true;

            image.CleanUp();

            Assert.That(image.PictureColours, Has.All.Null);
            Assert.That(image.PictureColourIndexes, Has.All.Null);
            Assert.That(image.PictureColour, Has.All.Null);
            Assert.That(image.PictureWidth, Has.All.Zero);
            Assert.That(image.PictureHeight, Has.All.Zero);
            Assert.That(image.PictureOffsetX[0], Is.EqualTo(42));
            Assert.That(image.PictureAssumedWidth[0], Is.EqualTo(64));
            Assert.That(image.HasTransparentBackground[0]);
        }

        private void AssertPictureMetadata(int pictureIndex, int expectedWidth, int expectedHeight)
        {
            Assert.That(image.PictureWidth[pictureIndex], Is.EqualTo(expectedWidth));
            Assert.That(image.PictureHeight[pictureIndex], Is.EqualTo(expectedHeight));
            Assert.That(image.PictureOffsetX[pictureIndex], Is.Zero);
            Assert.That(image.PictureOffsetY[pictureIndex], Is.Zero);
            Assert.That(image.PictureAssumedWidth[pictureIndex], Is.EqualTo(expectedWidth));
            Assert.That(image.PictureAssumedHeight[pictureIndex], Is.EqualTo(expectedHeight));
            Assert.That(image.HasTransparentBackground[pictureIndex], Is.False);
        }

        private static sbyte[] BuildMetadata(
            int assumedWidth,
            int assumedHeight,
            int offsetX,
            int offsetY,
            int width,
            int height,
            int scanOrder,
            params int[] paletteColours)
        {
            List<sbyte> metadata =
            [
                (sbyte)(assumedWidth >> 8),
                (sbyte)assumedWidth,
                (sbyte)(assumedHeight >> 8),
                (sbyte)assumedHeight,
                (sbyte)(paletteColours.Length + 1),
            ];

            foreach (int colour in paletteColours)
            {
                metadata.Add((sbyte)(colour >> 16));
                metadata.Add((sbyte)(colour >> 8));
                metadata.Add((sbyte)colour);
            }

            metadata.Add((sbyte)offsetX);
            metadata.Add((sbyte)offsetY);
            metadata.Add((sbyte)(width >> 8));
            metadata.Add((sbyte)width);
            metadata.Add((sbyte)(height >> 8));
            metadata.Add((sbyte)height);
            metadata.Add((sbyte)scanOrder);

            return [.. metadata];
        }
    }
}