using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class GameImageCharacterTests
    {
        private static int Width => 8;

        private static int Height => 6;

        private GameImage image = null!;

        [SetUp]
        public void SetUp()
        {
            image = new GameImage(Width, Height, 2);
        }

        [Test]
        public void GivenADirectPicture_WhenDrawingItTransparently_ThenTheBackgroundIsBlended()
        {
            Array.Fill(image.Pixels, 0x204060);
            SetDirectPicture(0, 1, 1, [0x80c000]);

            image.DrawTransparentImage(1, 1, 1, 1, 0, 128);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x508030));
        }

        [Test]
        public void GivenATransparentDirectPixel_WhenDrawingItTransparently_ThenTheBackgroundIsPreserved()
        {
            Array.Fill(image.Pixels, 0x204060);
            SetDirectPicture(0, 1, 1, [0]);

            image.DrawTransparentImage(1, 1, 1, 1, 0, 256);

            Assert.That(image.Pixels, Has.All.EqualTo(0x204060));
        }

        [Test]
        public void GivenGrayscaleAndColouredLegPixels_WhenDrawingThem_ThenOnlyGrayscaleIsTinted()
        {
            SetDirectPicture(0, 2, 1, [0x808080, 0x804020]);

            image.DrawCharacterLegs(1, 1, 2, 1, 0, 0x80c000);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x406000));
            Assert.That(GetPixel(2, 1), Is.EqualTo(0x804020));
        }

        [Test]
        public void GivenDirectPixelsAndAPrimaryColour_WhenDrawingAnImage_ThenOnlyGrayscaleIsTinted()
        {
            SetDirectPicture(0, 3, 1, [0x808080, 0x804020, 0]);

            image.DrawImage(1, 1, 3, 1, 0, 0x80c000, 0, 0, false);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x406000));
            Assert.That(GetPixel(2, 1), Is.EqualTo(0x804020));
            Assert.That(GetPixel(3, 1), Is.Zero);
        }

        [Test]
        public void GivenDirectPixelsAndTwoColours_WhenDrawingAnImage_ThenBothSpecialColourClassesAreTinted()
        {
            SetDirectPicture(0, 4, 1, [0x808080, 0xff8080, 0x804020, 0]);

            image.DrawImage(1, 1, 4, 1, 0, 0x80c000, 0x0040c0, 0, false);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x406000));
            Assert.That(GetPixel(2, 1), Is.EqualTo(0x002060));
            Assert.That(GetPixel(3, 1), Is.EqualTo(0x804020));
            Assert.That(GetPixel(4, 1), Is.Zero);
        }

        [Test]
        public void GivenIndexedPixelsAndAPrimaryColour_WhenDrawingAnImage_ThenThePaletteIsResolvedAndTinted()
        {
            SetIndexedPicture(
                0,
                3,
                1,
                [1, 2, 0],
                [0, 0x808080, 0x804020]);

            image.DrawImage(1, 1, 3, 1, 0, 0x80c000, 0, 0, false);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x406000));
            Assert.That(GetPixel(2, 1), Is.EqualTo(0x804020));
            Assert.That(GetPixel(3, 1), Is.Zero);
        }

        [Test]
        public void GivenIndexedPixelsAndTwoColours_WhenDrawingAnImage_ThenBothPaletteClassesAreTinted()
        {
            SetIndexedPicture(
                0,
                4,
                1,
                [1, 2, 3, 0],
                [0, 0x808080, 0xff8080, 0x804020]);

            image.DrawImage(1, 1, 4, 1, 0, 0x80c000, 0x0040c0, 0, false);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x406000));
            Assert.That(GetPixel(2, 1), Is.EqualTo(0x002060));
            Assert.That(GetPixel(3, 1), Is.EqualTo(0x804020));
            Assert.That(GetPixel(4, 1), Is.Zero);
        }

        [Test]
        public void GivenADirectPicture_WhenDrawingItFlipped_ThenSourceColumnsAreReversed()
        {
            SetDirectPicture(0, 3, 1, [4, 8, 16]);

            image.DrawImage(1, 1, 3, 1, 0, 0xffffff, 0, 0, true);

            Assert.That(GetPixel(1, 1), Is.EqualTo(16));
            Assert.That(GetPixel(2, 1), Is.EqualTo(8));
            Assert.That(GetPixel(3, 1), Is.EqualTo(4));
        }

        [Test]
        public void GivenAnIndexedPicture_WhenDrawingItFlipped_ThenSourceColumnsAreReversed()
        {
            SetIndexedPicture(0, 3, 1, [1, 2, 3], [0, 4, 8, 16]);

            image.DrawImage(1, 1, 3, 1, 0, 0xffffff, 0, 0, true);

            Assert.That(GetPixel(1, 1), Is.EqualTo(16));
            Assert.That(GetPixel(2, 1), Is.EqualTo(8));
            Assert.That(GetPixel(3, 1), Is.EqualTo(4));
        }

        [Test]
        public void GivenZeroColours_WhenDrawingAGrayscaleImage_ThenTheDefaultWhiteTintIsUsed()
        {
            SetDirectPicture(0, 1, 1, [0x808080]);

            image.DrawImage(1, 1, 1, 1, 0, 0, 0, 0, false);

            Assert.That(GetPixel(1, 1), Is.EqualTo(0x7f7f7f));
        }

        [Test]
        public void GivenAShearFactor_WhenDrawingAnImage_ThenEachRowMovesTowardsTheBasePosition()
        {
            SetDirectPicture(0, 1, 2, [42, 64]);

            image.DrawImage(1, 1, 1, 2, 0, 0xffffff, 0, 2, false);

            Assert.That(GetPixel(3, 1), Is.EqualTo(42));
            Assert.That(GetPixel(2, 2), Is.EqualTo(64));
            Assert.That(image.Pixels, Has.Exactly(2).Not.Zero);
        }

        [Test]
        public void GivenAnInterlacedImage_WhenDrawingIt_ThenOnlyAlternatingRowsAreRendered()
        {
            SetDirectPicture(0, 1, 4, [4, 8, 16, 32]);
            image.IsInterlaced = true;

            image.DrawImage(1, 0, 1, 4, 0, 0xffffff, 0, 0, false);

            Assert.That(GetPixel(1, 0), Is.EqualTo(4));
            Assert.That(GetPixel(1, 1), Is.Zero);
            Assert.That(GetPixel(1, 2), Is.EqualTo(16));
            Assert.That(GetPixel(1, 3), Is.Zero);
        }

        [Test]
        public void GivenAssumedPictureOffsets_WhenDrawingAnImage_ThenTheVisibleAreaIsRepositioned()
        {
            SetDirectPicture(0, 2, 2, [4, 8, 16, 32]);
            image.HasTransparentBackground[0] = true;
            image.PictureOffsetX[0] = 1;
            image.PictureOffsetY[0] = 1;
            image.PictureAssumedWidth[0] = 4;
            image.PictureAssumedHeight[0] = 4;

            image.DrawImage(0, 0, 4, 4, 0, 0xffffff, 0, 0, false);

            Assert.That(GetPixel(1, 1), Is.EqualTo(4));
            Assert.That(GetPixel(2, 1), Is.EqualTo(8));
            Assert.That(GetPixel(1, 2), Is.EqualTo(16));
            Assert.That(GetPixel(2, 2), Is.EqualTo(32));
        }

        [Test]
        public void GivenZeroDrawWidth_WhenDrawingCharacterData_ThenADivideByZeroExceptionIsThrown()
        {
            SetDirectPicture(0, 1, 1, [42]);

            Assert.That(
                () => image.DrawTransparentImage(1, 1, 0, 1, 0, 128),
                Throws.TypeOf<DivideByZeroException>());
            Assert.That(
                () => image.DrawCharacterLegs(1, 1, 0, 1, 0, 0x80c000),
                Throws.TypeOf<DivideByZeroException>());
            Assert.That(
                () => image.DrawImage(1, 1, 0, 1, 0, 0xffffff, 0, 0, false),
                Throws.TypeOf<DivideByZeroException>());
        }

        [Test]
        public void GivenAnInvalidPictureIndex_WhenDrawingCharacterData_ThenAnIndexExceptionIsThrown()
        {
            Assert.That(
                () => image.DrawTransparentImage(1, 1, 1, 1, 42, 128),
                Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(
                () => image.DrawCharacterLegs(1, 1, 1, 1, 42, 0x80c000),
                Throws.TypeOf<IndexOutOfRangeException>());
            Assert.That(
                () => image.DrawImage(1, 1, 1, 1, 42, 0xffffff, 0, 0, false),
                Throws.TypeOf<IndexOutOfRangeException>());
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