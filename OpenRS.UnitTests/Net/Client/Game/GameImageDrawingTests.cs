using System;
using System.Linq;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class GameImageDrawingTests
    {
        private static int Width => 5;

        private static int Height => 4;

        private static int PictureCapacity => 8;

        private GameImage image = null!;

        [SetUp]
        public void SetUp()
        {
            image = new GameImage(Width, Height, PictureCapacity);
        }

        [Test]
        public void GivenImageDimensions_WhenConstructingAnImage_ThenPixelAndPictureStorageAreInitialised()
        {
            Assert.That(image.GameWidth, Is.EqualTo(Width));
            Assert.That(image.GameHeight, Is.EqualTo(Height));
            Assert.That(image.Width, Is.EqualTo(Width));
            Assert.That(image.Height, Is.EqualTo(Height));
            Assert.That(image.Area, Is.EqualTo(Width * Height));
            Assert.That(image.GameSize, Is.EqualTo(new Size2D(Width, Height)));
            Assert.That(image.Pixels, Has.Length.EqualTo(Width * Height));
            Assert.That(image.Pixels, Has.All.Zero);
            Assert.That(image.PictureColours, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureColourIndexes, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureColour, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureWidth, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureHeight, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureOffsetX, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureOffsetY, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureAssumedWidth, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.PictureAssumedHeight, Has.Length.EqualTo(PictureCapacity));
            Assert.That(image.HasTransparentBackground, Has.Length.EqualTo(PictureCapacity));
        }

        [Test]
        public void GivenFilledPixels_WhenClearingANonInterlacedScreen_ThenEveryPixelIsZero()
        {
            FillPixels(42);

            image.ClearScreen();

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenFilledPixels_WhenClearingAnInterlacedScreen_ThenOnlyAlternatingRowsAreZero()
        {
            FillPixels(42);
            image.IsInterlaced = true;

            image.ClearScreen();

            AssertRows(
                [0, 0, 0, 0, 0],
                [42, 42, 42, 42, 42],
                [0, 0, 0, 0, 0],
                [42, 42, 42, 42, 42]);
        }

        [Test]
        public void GivenABoxWithinTheImage_WhenDrawingIt_ThenOnlyItsRectangleIsFilled()
        {
            image.DrawBox(1, 1, 3, 2, 42);

            AssertRows(
                [0, 0, 0, 0, 0],
                [0, 42, 42, 42, 0],
                [0, 42, 42, 42, 0],
                [0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenABoxBeyondEveryImageEdge_WhenDrawingIt_ThenItIsClippedToTheImage()
        {
            image.DrawBox(-2, -1, 9, 7, 42);

            Assert.That(image.Pixels, Has.All.EqualTo(42));
        }

        [Test]
        public void GivenARestrictedDrawingArea_WhenDrawingABox_ThenItIsClippedToThatArea()
        {
            image.SetDimensions(1, 1, 4, 3);

            image.DrawBox(0, 0, 5, 4, 42);

            AssertRows(
                [0, 0, 0, 0, 0],
                [0, 42, 42, 42, 0],
                [0, 42, 42, 42, 0],
                [0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenARestrictedDrawingArea_WhenResettingIt_ThenTheFullImageCanBeDrawnAgain()
        {
            image.SetDimensions(1, 1, 4, 3);
            image.ResetDimensions();

            image.DrawBox(0, 0, Width, Height, 42);

            Assert.That(image.Pixels, Has.All.EqualTo(42));
        }

        [Test]
        public void GivenNegativeAreaOrigins_WhenSettingDimensions_ThenTheyAreClampedToZero()
        {
            image.SetDimensions(-42, -64, Width, Height);

            image.DrawMinimapPixel(0, 0, 42);

            Assert.That(GetPixel(0, 0), Is.EqualTo(42));
        }

        [Test]
        public void GivenAreaBoundsBeyondTheImage_WhenSettingDimensions_ThenTheyAreClampedToTheImage()
        {
            image.SetDimensions(0, 0, 42, 64);

            image.DrawMinimapPixel(Width - 1, Height - 1, 42);
            image.DrawMinimapPixel(Width, Height, 64);

            Assert.That(GetPixel(Width - 1, Height - 1), Is.EqualTo(42));
            Assert.That(image.Pixels, Does.Not.Contain(64));
        }

        [Test]
        public void GivenAnInterlacedBoxStartingOnAnEvenRow_WhenDrawingIt_ThenOnlyEvenRowsAreFilled()
        {
            image.IsInterlaced = true;

            image.DrawBox(1, 0, 3, 4, 42);

            AssertRows(
                [0, 42, 42, 42, 0],
                [0, 0, 0, 0, 0],
                [0, 42, 42, 42, 0],
                [0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenAnInterlacedBoxStartingOnAnOddRow_WhenDrawingIt_ThenTheNextEvenRowsAreFilled()
        {
            image.IsInterlaced = true;

            image.DrawBox(1, 1, 3, 3, 42);

            AssertRows(
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
                [0, 42, 42, 42, 0],
                [0, 0, 0, 0, 0]);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-42)]
        public void GivenANonPositiveBoxWidth_WhenDrawingIt_ThenNoPixelIsModified(int width)
        {
            image.DrawBox(1, 1, width, 2, 42);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-42)]
        public void GivenANonPositiveBoxHeight_WhenDrawingIt_ThenNoPixelIsModified(int height)
        {
            image.DrawBox(1, 1, 2, height, 42);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenAFullAlphaBox_WhenDrawingIt_ThenTheSourceColourReplacesExistingPixels()
        {
            FillPixels(0x204060);

            image.DrawBoxAlpha(1, 1, 3, 2, 0x80c000, 256);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x80c000));
            Assert.That(GetPixel(3, 2), Is.EqualTo(0x80c000));
            Assert.That(GetPixel(0, 0), Is.EqualTo(0x204060));
        }

        [Test]
        public void GivenAZeroAlphaBox_WhenDrawingIt_ThenExistingPixelsRemainUnchanged()
        {
            FillPixels(0x204060);

            image.DrawBoxAlpha(0, 0, Width, Height, 0x80c000, 0);

            Assert.That(image.Pixels, Has.All.EqualTo(0x204060));
        }

        [Test]
        public void GivenAHalfAlphaBox_WhenDrawingIt_ThenChannelsAreBlendedIndependently()
        {
            FillPixels(0x204060);

            image.DrawBoxAlpha(1, 1, 1, 1, 0x80c000, 128);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x508030));
        }

        [Test]
        public void GivenAnInterlacedAlphaBox_WhenDrawingIt_ThenOnlyAlternatingRowsAreBlended()
        {
            FillPixels(0x204060);
            image.IsInterlaced = true;

            image.DrawBoxAlpha(0, 0, Width, Height, 0x80c000, 128);

            Assert.That(GetPixel(0, 0), Is.EqualTo(0x508030));
            Assert.That(GetPixel(0, 1), Is.EqualTo(0x204060));
            Assert.That(GetPixel(0, 2), Is.EqualTo(0x508030));
            Assert.That(GetPixel(0, 3), Is.EqualTo(0x204060));
        }

        [Test]
        public void GivenAHorizontalGradient_WhenDrawingIt_ThenEachRowInterpolatesFromTheStartColour()
        {
            image.DrawGradientBox(1, 0, 3, 4, 0x000000, 0xffffff);

            AssertRows(
                [0, 0x000000, 0x000000, 0x000000, 0],
                [0, 0x3f3f3f, 0x3f3f3f, 0x3f3f3f, 0],
                [0, 0x7f7f7f, 0x7f7f7f, 0x7f7f7f, 0],
                [0, 0xbfbfbf, 0xbfbfbf, 0xbfbfbf, 0]);
        }

        [Test]
        public void GivenAClippedGradient_WhenDrawingIt_ThenOnlyHorizontalAreaBoundsAreModified()
        {
            image.SetDimensions(2, 0, 4, Height);

            image.DrawGradientBox(0, 0, Width, Height, 0x000000, 0xffffff);

            Assert.That(GetPixel(0, 2), Is.Zero);
            Assert.That(GetPixel(1, 2), Is.Zero);
            Assert.That(GetPixel(2, 2), Is.EqualTo(0x7f7f7f));
            Assert.That(GetPixel(3, 2), Is.EqualTo(0x7f7f7f));
            Assert.That(GetPixel(4, 2), Is.Zero);
        }

        [Test]
        public void GivenAFullAlphaCircle_WhenDrawingIt_ThenTheIntegerRadiusShapeIsFilled()
        {
            GameImage circleImage = new(5, 5, 0);

            circleImage.DrawCircle(2, 2, 2, 42, 256);

            AssertPixels(
                circleImage,
                [0, 0, 42, 0, 0],
                [0, 42, 42, 42, 0],
                [42, 42, 42, 42, 42],
                [0, 42, 42, 42, 0],
                [0, 0, 42, 0, 0]);
        }

        [Test]
        public void GivenACircleBeyondTheImage_WhenDrawingIt_ThenTheShapeIsClipped()
        {
            image.DrawCircle(0, 0, 2, 42, 256);

            AssertRows(
                [42, 42, 42, 0, 0],
                [42, 42, 0, 0, 0],
                [42, 0, 0, 0, 0],
                [0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenAnInterlacedCircle_WhenDrawingIt_ThenOnlyAlternatingRowsAreFilled()
        {
            GameImage circleImage = new(5, 5, 0)
            {
                IsInterlaced = true,
            };

            circleImage.DrawCircle(2, 2, 2, 42, 256);

            AssertPixels(
                circleImage,
                [0, 0, 42, 0, 0],
                [0, 0, 0, 0, 0],
                [42, 42, 42, 42, 42],
                [0, 0, 0, 0, 0],
                [0, 0, 42, 0, 0]);
        }

        [Test]
        public void GivenAHorizontalLine_WhenDrawingIt_ThenItIsClippedToTheImage()
        {
            image.DrawLineX(-2, 1, 9, 42);

            AssertRows(
                [0, 0, 0, 0, 0],
                [42, 42, 42, 42, 42],
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0]);
        }

        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(42)]
        public void GivenAHorizontalLineOutsideTheImage_WhenDrawingIt_ThenNoPixelIsModified(int positionY)
        {
            image.DrawLineX(0, positionY, Width, 42);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenAVerticalLine_WhenDrawingIt_ThenItIsClippedToTheImage()
        {
            image.DrawLineY(2, -2, 8, 42);

            AssertRows(
                [0, 0, 42, 0, 0],
                [0, 0, 42, 0, 0],
                [0, 0, 42, 0, 0],
                [0, 0, 42, 0, 0]);
        }

        [Test]
        public void GivenANonSquareImageAndAContainedVerticalLine_WhenDrawingIt_ThenItsRequestedLengthIsUsed()
        {
            GameImage tallImage = new(4, 6, 0);

            tallImage.DrawLineY(1, 0, 5, 42);

            Assert.That(tallImage.Pixels.Count(pixel => pixel == 42), Is.EqualTo(5));
            Assert.That(tallImage.Pixels[1 + 5 * 4], Is.Zero);
        }

        [TestCase(-1)]
        [TestCase(5)]
        [TestCase(42)]
        public void GivenAVerticalLineOutsideTheImage_WhenDrawingIt_ThenNoPixelIsModified(int positionX)
        {
            image.DrawLineY(positionX, 0, Height, 42);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenABoxEdge_WhenDrawingIt_ThenOnlyItsPerimeterIsFilled()
        {
            image.DrawBoxEdge(1, 0, 3, 4, 42);

            AssertRows(
                [0, 42, 42, 42, 0],
                [0, 42, 0, 42, 0],
                [0, 42, 0, 42, 0],
                [0, 42, 42, 42, 0]);
        }

        [Test]
        public void GivenMinimapPixels_WhenDrawingInsideAndOutsideBounds_ThenOnlyTheInsidePixelChanges()
        {
            image.SetDimensions(1, 1, 4, 3);

            image.DrawMinimapPixel(1, 1, 42);
            image.DrawMinimapPixel(0, 1, 64);
            image.DrawMinimapPixel(1, 0, 96);
            image.DrawMinimapPixel(4, 1, 128);
            image.DrawMinimapPixel(1, 3, 256);

            Assert.That(GetPixel(1, 1), Is.EqualTo(42));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.EqualTo(1));
        }

        [TestCase(0x000000, 0x000000)]
        [TestCase(0xffffff, 0xececec)]
        [TestCase(0x808080, 0x787878)]
        [TestCase(0xff0000, 0xec0000)]
        [TestCase(0x00ff00, 0x00ec00)]
        [TestCase(0x0000ff, 0x0000ec)]
        [TestCase(unchecked((int)0xff123456), 0x10304f)]
        public void GivenAColour_WhenFadingTheScreen_ThenItsRgbChannelsUseTheCompatibleAttenuation(
            int colour,
            int expectedColour)
        {
            FillPixels(colour);

            image.ScreenFadeToBlack();

            Assert.That(image.Pixels, Has.All.EqualTo(expectedColour));
        }

        [Test]
        public void GivenZeroBlurRadii_WhenBlurringAnArea_ThenPixelsRemainUnchanged()
        {
            image.Pixels = Enumerable.Range(0, Width * Height).ToArray();
            int[] expectedPixels = [.. image.Pixels];

            image.DrawTransparentLine(0, 0, 0, 0, Width, Height);

            Assert.That(image.Pixels, Is.EqualTo(expectedPixels));
        }

        [Test]
        public void GivenThreePrimaryColours_WhenBlurringTheirCentre_ThenTheirAverageIsGrey()
        {
            GameImage lineImage = new(3, 1, 0)
            {
                Pixels = [0xff0000, 0x00ff00, 0x0000ff],
            };

            lineImage.DrawTransparentLine(1, 0, 1, 0, 1, 1);

            Assert.That(lineImage.Pixels[1], Is.EqualTo(0x555555));
        }

        [Test]
        public void GivenFourCornerColours_WhenBlurringACorner_ThenOnlyAvailableSamplesAreAveraged()
        {
            GameImage squareImage = new(2, 2, 0)
            {
                Pixels = [0xff0000, 0x00ff00, 0x0000ff, 0xffffff],
            };

            squareImage.DrawTransparentLine(1, 1, 0, 0, 1, 1);

            Assert.That(squareImage.Pixels[0], Is.EqualTo(0x7f7f7f));
        }

        [Test]
        public void GivenAColumnMajorPixelGrid_WhenDrawingIt_ThenCoordinatesMapToTheImage()
        {
            int[][] pixelGrid =
            [
                [4, 8],
                [16, 32],
                [42, 48],
            ];

            image.DrawPixels(pixelGrid, 1, 1, 3, 2);

            AssertRows(
                [0, 0, 0, 0, 0],
                [0, 4, 16, 42, 0],
                [0, 8, 32, 48, 0],
                [0, 0, 0, 0, 0]);
        }

        [Test]
        public void GivenZeroPixelGridDimensions_WhenDrawingIt_ThenNoGridAccessOccurs()
        {
            image.DrawPixels(null!, 1, 1, 0, 0);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenAPixelGridOutsideTheImage_WhenDrawingIt_ThenAnIndexExceptionIsThrown()
            => Assert.That(
                () => image.DrawPixels([[42]], Width, Height, 1, 1),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenAnOddHeightImage_WhenClearingAnInterlacedScreen_ThenEvenRowsAreZero()
        {
            GameImage oddHeightImage = new(3, 5, 0)
            {
                IsInterlaced = true,
            };
            Array.Fill(oddHeightImage.Pixels, 42);

            oddHeightImage.ClearScreen();

            AssertPixels(
                oddHeightImage,
                [0, 0, 0],
                [42, 42, 42],
                [0, 0, 0],
                [42, 42, 42],
                [0, 0, 0]);
        }

        [Test]
        public void GivenAnAlphaBoxBeyondEveryEdge_WhenDrawingIt_ThenTheEntireImageIsClippedAndFilled()
        {
            image.DrawBoxAlpha(-2, -1, 9, 7, 42, 256);

            Assert.That(image.Pixels, Has.All.EqualTo(42));
        }

        [Test]
        public void GivenAHalfAlphaZeroRadiusCircle_WhenDrawingIt_ThenOnlyTheCentrePixelIsBlended()
        {
            FillPixels(0x204060);

            image.DrawCircle(2, 2, 0, 0x80c000, 128);

            Assert.That(GetPixel(2, 2), Is.EqualTo(0x508030));
            Assert.That(image.Pixels.Count(pixel => pixel == 0x508030), Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel == 0x204060), Is.EqualTo(19));
        }

        [Test]
        public void GivenAZeroAlphaCircle_WhenDrawingIt_ThenExistingPixelsRemainUnchanged()
        {
            FillPixels(0x204060);

            image.DrawCircle(2, 2, 2, 0x80c000, 0);

            Assert.That(image.Pixels, Has.All.EqualTo(0x204060));
        }

        [Test]
        public void GivenAnInterlacedGradient_WhenDrawingIt_ThenOnlyAlternatingRowsAreFilled()
        {
            GameImage gradientImage = new(3, 5, 0)
            {
                IsInterlaced = true,
            };

            gradientImage.DrawGradientBox(0, 0, 3, 5, 0x000000, 0xffffff);

            AssertPixels(
                gradientImage,
                [0x000000, 0x000000, 0x000000],
                [0, 0, 0],
                [0x666666, 0x666666, 0x666666],
                [0, 0, 0],
                [0xcccccc, 0xcccccc, 0xcccccc]);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-42)]
        public void GivenANonPositiveLineLength_WhenDrawingLines_ThenNoPixelIsModified(int length)
        {
            image.DrawLineX(1, 1, length, 42);
            image.DrawLineY(1, 1, length, 42);

            Assert.That(image.Pixels, Has.All.Zero);
        }

        private static void AssertPixels(GameImage targetImage, params int[][] expectedRows)
        {
            Assert.That(expectedRows, Has.Length.EqualTo(targetImage.GameHeight));

            for (int positionY = 0; positionY < expectedRows.Length; positionY += 1)
            {
                Assert.That(
                    targetImage.Pixels[(positionY * targetImage.GameWidth)..((positionY + 1) * targetImage.GameWidth)],
                    Is.EqualTo(expectedRows[positionY]));
            }
        }

        private void AssertRows(params int[][] expectedRows) => AssertPixels(image, expectedRows);

        private void FillPixels(int colour) => Array.Fill(image.Pixels, colour);

        private int GetPixel(int positionX, int positionY)
            => image.Pixels[positionX + positionY * image.GameWidth];
    }
}