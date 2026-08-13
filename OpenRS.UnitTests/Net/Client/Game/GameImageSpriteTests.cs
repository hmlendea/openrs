using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class GameImageSpriteTests
    {
        private static int Width => 6;

        private static int Height => 6;

        private GameImage image = null!;

        [SetUp]
        public void SetUp()
        {
            image = new GameImage(Width, Height, 2);
        }

        [Test]
        public void GivenADirectPicture_WhenDrawingIt_ThenNonZeroPixelsAreCopied()
        {
            SetDirectPicture(0, 3, 2, [4, 0, 8, 16, 32, 0]);

            image.DrawPicture(1, 1, 0);

            AssertRows(
                [0, 0, 0, 0, 0, 0],
                [0, 4, 0, 8, 0, 0],
                [0, 16, 32, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenAnIndexedPicture_WhenDrawingIt_ThenPaletteIndicesAreResolved()
        {
            SetIndexedPicture(0, 3, 2, [1, 0, 2, 2, 1, 0], [0, 42, 64]);

            image.DrawPicture(1, 1, 0);

            AssertRows(
                [0, 0, 0, 0, 0, 0],
                [0, 42, 0, 64, 0, 0],
                [0, 64, 42, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenATransparentPictureOffset_WhenDrawingIt_ThenTheOffsetMovesTheDestination()
        {
            SetDirectPicture(0, 2, 1, [42, 64]);
            image.HasTransparentBackground[0] = true;
            image.PictureOffsetX[0] = 1;
            image.PictureOffsetY[0] = 2;

            image.DrawPicture(1, 1, 0);

            Assert.That(GetPixel(2, 3), Is.EqualTo(42));
            Assert.That(GetPixel(3, 3), Is.EqualTo(64));
            Assert.That(image.Pixels, Has.Exactly(2).Not.Zero);
        }

        [Test]
        public void GivenAPictureBeyondTheTopLeft_WhenDrawingIt_ThenOnlyTheVisibleSourceAreaIsCopied()
        {
            SetDirectPicture(0, 3, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]);

            image.DrawPicture(-1, -1, 0);

            Assert.That(GetPixel(0, 0), Is.EqualTo(5));
            Assert.That(GetPixel(1, 0), Is.EqualTo(6));
            Assert.That(GetPixel(0, 1), Is.EqualTo(8));
            Assert.That(GetPixel(1, 1), Is.EqualTo(9));
            Assert.That(image.Pixels, Has.Exactly(4).Not.Zero);
        }

        [Test]
        public void GivenAPictureAtTheBottomRightBoundary_WhenDrawingIt_ThenTheCompatibleExclusiveEdgeIsUsed()
        {
            SetDirectPicture(0, 3, 3, [1, 2, 3, 4, 5, 6, 7, 8, 9]);

            image.DrawPicture(Width - 2, Height - 2, 0);

            Assert.That(GetPixel(Width - 2, Height - 2), Is.EqualTo(1));
            Assert.That(image.Pixels, Has.Exactly(1).EqualTo(1));
        }

        [Test]
        public void GivenAPictureEntirelyOutsideTheImage_WhenDrawingIt_ThenNoPixelChanges()
        {
            SetDirectPicture(0, 2, 2, [4, 8, 16, 32]);

            image.DrawPicture(-4, -4, 0);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenAnInterlacedPicture_WhenDrawingIt_ThenAlternatingSourceRowsAreCopied()
        {
            SetDirectPicture(0, 2, 4, [1, 2, 3, 4, 5, 6, 7, 8]);
            image.IsInterlaced = true;

            image.DrawPicture(1, 0, 0);

            Assert.That(GetPixel(1, 0), Is.EqualTo(1));
            Assert.That(GetPixel(2, 0), Is.EqualTo(2));
            Assert.That(GetPixel(1, 1), Is.Zero);
            Assert.That(GetPixel(1, 2), Is.EqualTo(5));
            Assert.That(GetPixel(2, 2), Is.EqualTo(6));
            Assert.That(GetPixel(1, 3), Is.Zero);
        }

        [Test]
        public void GivenAnInterlacedPictureStartingOnAnOddRow_WhenDrawingIt_ThenItStartsOnTheNextRow()
        {
            SetDirectPicture(0, 2, 4, [1, 2, 3, 4, 5, 6, 7, 8]);
            image.IsInterlaced = true;

            image.DrawPicture(1, 1, 0);

            Assert.That(GetPixel(1, 1), Is.Zero);
            Assert.That(GetPixel(1, 2), Is.EqualTo(1));
            Assert.That(GetPixel(2, 2), Is.EqualTo(2));
            Assert.That(GetPixel(1, 4), Is.EqualTo(5));
            Assert.That(GetPixel(2, 4), Is.EqualTo(6));
        }

        [TestCase(0, 0x204060)]
        [TestCase(128, 0x508030)]
        [TestCase(256, unchecked((int)0xff80c000))]
        public void GivenADirectPictureAndBlendFactor_WhenDrawingIt_ThenThePixelIsBlended(
            int blendFactor,
            int expectedColour)
        {
            Array.Fill(image.Pixels, 0x204060);
            SetDirectPicture(0, 1, 1, [0x80c000]);

            image.DrawPicture(1, 1, 0, blendFactor);

            Assert.That(GetPixel(1, 1), Is.EqualTo(expectedColour));
        }

        [Test]
        public void GivenAnIndexedPictureAndBlendFactor_WhenDrawingIt_ThenThePaletteColourIsBlended()
        {
            Array.Fill(image.Pixels, 0x204060);
            SetIndexedPicture(0, 1, 1, [1], [0, 0x80c000]);

            image.DrawPicture(1, 1, 0, 128);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x508030));
        }

        [Test]
        public void GivenATransparentPicturePixel_WhenDrawingWithBlend_ThenTheBackgroundIsPreserved()
        {
            Array.Fill(image.Pixels, 0x204060);
            SetDirectPicture(0, 1, 1, [0]);

            image.DrawPicture(1, 1, 0, 256);

            Assert.That(image.Pixels, Has.All.EqualTo(0x204060));
        }

        [Test]
        public void GivenA2By2Picture_WhenDrawingAnEntityAtDoubleSize_ThenNearestPixelsAreExpanded()
        {
            SetDirectPicture(0, 2, 2, [4, 8, 16, 32]);

            image.DrawEntity(1, 1, 4, 4, 0);

            AssertRows(
                [0, 0, 0, 0, 0, 0],
                [0, 4, 4, 8, 8, 0],
                [0, 4, 4, 8, 8, 0],
                [0, 16, 16, 32, 32, 0],
                [0, 16, 16, 32, 32, 0],
                [0, 0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenATransparentEntityPicture_WhenDrawingIt_ThenAssumedOffsetsAdjustItsArea()
        {
            SetDirectPicture(0, 2, 2, [4, 8, 16, 32]);
            image.HasTransparentBackground[0] = true;
            image.PictureOffsetX[0] = 1;
            image.PictureOffsetY[0] = 1;
            image.PictureAssumedWidth[0] = 4;
            image.PictureAssumedHeight[0] = 4;

            image.DrawEntity(0, 0, 4, 4, 0);

            Assert.That(GetPixel(1, 1), Is.EqualTo(4));
            Assert.That(GetPixel(2, 1), Is.EqualTo(8));
            Assert.That(GetPixel(1, 2), Is.EqualTo(16));
            Assert.That(GetPixel(2, 2), Is.EqualTo(32));
            Assert.That(image.Pixels, Has.Exactly(4).Not.Zero);
        }

        [Test]
        public void GivenAnEntityPicture_WhenDrawingItThroughTheVisibleEntityFacade_ThenPixelsAreScaled()
        {
            SetDirectPicture(0, 1, 1, [42]);

            image.DrawVisibleEntity(1, 1, 2, 2, 0, 64, 96);

            Assert.That(GetPixel(1, 1), Is.EqualTo(42));
            Assert.That(GetPixel(2, 1), Is.EqualTo(42));
            Assert.That(GetPixel(1, 2), Is.EqualTo(42));
            Assert.That(GetPixel(2, 2), Is.EqualTo(42));
        }

        [Test]
        public void GivenInvalidEntityDimensions_WhenDrawingIt_ThenTheRendererSwallowsTheFailure()
        {
            SetDirectPicture(0, 1, 1, [42]);

            Assert.That(() => image.DrawEntity(1, 1, 0, 1, 0), Throws.Nothing);
        }

        [Test]
        public void GivenAnInvalidEntityIndex_WhenDrawingIt_ThenTheRendererSwallowsTheFailure()
            => Assert.That(
                () => image.DrawEntity(1, 1, 1, 1, 42),
                Throws.Nothing);

        [Test]
        public void GivenAnInvalidPictureIndex_WhenDrawingAPicture_ThenAnIndexExceptionIsThrown()
            => Assert.That(
                () => image.DrawPicture(1, 1, 42),
                Throws.TypeOf<IndexOutOfRangeException>());

        private void AssertRows(params int[][] expectedRows)
        {
            Assert.That(expectedRows, Has.Length.EqualTo(image.GameHeight));

            for (int positionY = 0; positionY < expectedRows.Length; positionY += 1)
            {
                Assert.That(
                    image.Pixels[(positionY * image.GameWidth)..((positionY + 1) * image.GameWidth)],
                    Is.EqualTo(expectedRows[positionY]));
            }
        }

        private int GetPixel(int positionX, int positionY)
            => image.Pixels[positionX + positionY * image.GameWidth];

        private void SetDirectPicture(int pictureIndex, int width, int height, int[] colours)
        {
            image.PictureWidth[pictureIndex] = width;
            image.PictureHeight[pictureIndex] = height;
            image.PictureAssumedWidth[pictureIndex] = width;
            image.PictureAssumedHeight[pictureIndex] = height;
            image.PictureColours[pictureIndex] = colours;
        }

        private void SetIndexedPicture(
            int pictureIndex,
            int width,
            int height,
            sbyte[] colourIndices,
            int[] palette)
        {
            image.PictureWidth[pictureIndex] = width;
            image.PictureHeight[pictureIndex] = height;
            image.PictureAssumedWidth[pictureIndex] = width;
            image.PictureAssumedHeight[pictureIndex] = height;
            image.PictureColourIndexes[pictureIndex] = colourIndices;
            image.PictureColour[pictureIndex] = palette;
            image.PictureColours[pictureIndex] = null!;
        }
    }
}