using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    [NonParallelizable]
    public sealed class GameImageTextTests
    {
        private static int FontIndex => 0;

        private static int FallbackHeightFontIndex => 8;

        private GameImage image = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp() => SyntheticGameFont.EnsureRegistered();

        [SetUp]
        public void SetUp()
        {
            image = new GameImage(32, 40, 0);
        }

        [TestCase(0, 12)]
        [TestCase(1, 14)]
        [TestCase(2, 14)]
        [TestCase(3, 15)]
        [TestCase(4, 15)]
        [TestCase(5, 19)]
        [TestCase(6, 24)]
        [TestCase(7, 29)]
        [TestCase(8, 11)]
        public void GivenAFontIndex_WhenReadingTextHeight_ThenTheCompatibleHeightIsReturned(
            int fontIndex,
            int expectedHeight)
            => Assert.That(image.TextHeightNumber(fontIndex), Is.EqualTo(expectedHeight));

        [TestCase(0, 10)]
        [TestCase(8, 11)]
        public void GivenAFontIndex_WhenReadingCharacterWidth_ThenTheCompatibleAdjustmentIsApplied(
            int fontIndex,
            int expectedWidth)
            => Assert.That(image.GetCharacterWidth(fontIndex), Is.EqualTo(expectedWidth));

        [TestCase("", 0)]
        [TestCase("RuneScape", 9)]
        [TestCase("@red@RuneScape", 9)]
        [TestCase("~123~RuneScape", 9)]
        [TestCase("@red@Rune~123~Scape", 9)]
        public void GivenTextAndFormattingTags_WhenMeasuringIt_ThenOnlyGlyphAdvancesAreCounted(
            string text,
            int expectedWidth)
            => Assert.That(image.TextWidth(text, FontIndex), Is.EqualTo(expectedWidth));

        [Test]
        public void GivenPlainText_WhenDrawingIt_ThenEachGlyphUsesItsAdvance()
        {
            image.DrawString("abc", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(42));
            Assert.That(GetPixel(5, 8), Is.EqualTo(42));
            Assert.That(GetPixel(6, 8), Is.EqualTo(42));
            Assert.That(image.Pixels.Count(pixel => pixel == 42), Is.EqualTo(3));
        }

        [TestCase("red", 0xff0000)]
        [TestCase("lre", 0xff9040)]
        [TestCase("yel", 0xffff00)]
        [TestCase("gre", 0x00ff00)]
        [TestCase("blu", 0x0000ff)]
        [TestCase("cya", 0x00ffff)]
        [TestCase("mag", 0xff00ff)]
        [TestCase("whi", 0xffffff)]
        [TestCase("nor", 0x000000)]
        [TestCase("dre", 0xc00000)]
        [TestCase("ora", 0xff9040)]
        [TestCase("or1", 0xffb000)]
        [TestCase("or2", 0xff7000)]
        [TestCase("or3", 0xff3000)]
        [TestCase("gr1", 0xc0ff00)]
        [TestCase("gr2", 0x80ff00)]
        [TestCase("gr3", 0x40ff00)]
        public void GivenAColourTag_WhenDrawingText_ThenTheTaggedColourIsUsed(
            string colourCode,
            int expectedColour)
        {
            image.Pixels[4 + 8 * image.GameWidth] = 64;

            image.DrawString($"@{colourCode}@a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(expectedColour));
        }

        [Test]
        public void GivenAnUppercaseColourTag_WhenDrawingText_ThenItIsMatchedCaseInsensitively()
        {
            image.DrawString("@RED@a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(0xff0000));
        }

        [Test]
        public void GivenAnUnknownColourTag_WhenDrawingText_ThenTheExistingColourIsRetained()
        {
            image.DrawString("@xyz@a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(42));
        }

        [Test]
        public void GivenARandomColourTag_WhenDrawingText_ThenAnRgbColourIsGenerated()
        {
            image.DrawString("@ran@a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.InRange(0, 0xfffffe));
        }

        [Test]
        public void GivenAPositionTag_WhenDrawingText_ThenTheFollowingGlyphUsesThatPosition()
        {
            image.DrawString("~016~a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(16, 8), Is.EqualTo(42));
            Assert.That(GetPixel(4, 8), Is.Zero);
        }

        [Test]
        public void GivenANonNumericPositionTag_WhenDrawingText_ThenThePositionRemainsUnchanged()
        {
            image.DrawString("~abc~a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(42));
        }

        [Test]
        public void GivenLiteralTagMarkers_WhenDrawingText_ThenTheyAreNotRendered()
        {
            image.DrawString("@~a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(42));
            Assert.That(image.Pixels.Count(pixel => pixel == 42), Is.EqualTo(1));
        }

        [Test]
        public void GivenLoggedInText_WhenDrawingIt_ThenTwoBlackShadowPixelsAreAdded()
        {
            Array.Fill(image.Pixels, 64);
            image.IsLoggedIn = true;

            image.DrawString("a", 4, 8, FontIndex, 42);

            Assert.That(GetPixel(4, 8), Is.EqualTo(42));
            Assert.That(GetPixel(5, 8), Is.Zero);
            Assert.That(GetPixel(4, 9), Is.Zero);
        }

        [Test]
        public void GivenZeroColourWhileLoggedIn_WhenDrawingText_ThenNoAdditionalShadowPixelsAreAdded()
        {
            Array.Fill(image.Pixels, 64);
            image.IsLoggedIn = true;

            image.DrawString("a", 4, 8, FontIndex, 0);

            Assert.That(GetPixel(4, 8), Is.Zero);
            Assert.That(GetPixel(5, 8), Is.EqualTo(64));
            Assert.That(GetPixel(4, 9), Is.EqualTo(64));
        }

        [Test]
        public void GivenText_WhenDrawingALabel_ThenItsRightEdgeUsesTheProvidedPosition()
        {
            image.DrawLabel("abc", 8, 8, FontIndex, 42);

            Assert.That(GetPixel(5, 8), Is.EqualTo(42));
            Assert.That(GetPixel(7, 8), Is.EqualTo(42));
        }

        [Test]
        public void GivenText_WhenDrawingCentredText_ThenItsMidpointUsesTheProvidedPosition()
        {
            image.DrawText("abcd", 8, 8, FontIndex, 42);

            Assert.That(GetPixel(6, 8), Is.EqualTo(42));
            Assert.That(GetPixel(9, 8), Is.EqualTo(42));
        }

        [Test]
        public void GivenTextBeyondEveryDrawingEdge_WhenDrawingIt_ThenGlyphsAreClipped()
        {
            image.SetDimensions(4, 4, 8, 8);

            image.DrawString("a", 3, 3, FontIndex, 42);
            image.DrawString("a", 7, 4, FontIndex, 64);
            image.DrawString("a", 4, 7, FontIndex, 96);
            image.DrawString("a", 4, 4, FontIndex, 128);

            Assert.That(image.Pixels.Count(pixel => pixel == 42), Is.Zero);
            Assert.That(image.Pixels.Count(pixel => pixel == 64), Is.Zero);
            Assert.That(image.Pixels.Count(pixel => pixel == 96), Is.Zero);
            Assert.That(GetPixel(4, 4), Is.EqualTo(128));
        }

        [Test]
        public void GivenWordsBeyondMaximumWidth_WhenDrawingFloatingText_ThenTheyWrapToAnotherLine()
        {
            image.DrawFloatingText("aa aa", 8, 8, FontIndex, 42, 2);

            Assert.That(image.Pixels.Count(pixel => pixel == 42), Is.GreaterThanOrEqualTo(4));
            Assert.That(
                image.Pixels.Skip(8 * image.GameWidth).Take(image.GameWidth),
                Has.Some.EqualTo(42));
            Assert.That(
                image.Pixels.Skip(20 * image.GameWidth).Take(image.GameWidth),
                Has.Some.EqualTo(42));
        }

        [Test]
        public void GivenAForcedBreakMarker_WhenDrawingFloatingText_ThenFollowingTextUsesAnotherLine()
        {
            image.DrawFloatingText("aa%aa", 8, 8, FontIndex, 42, 32);

            Assert.That(
                image.Pixels.Skip(8 * image.GameWidth).Take(image.GameWidth),
                Has.Some.EqualTo(42));
            Assert.That(
                image.Pixels.Skip(20 * image.GameWidth).Take(image.GameWidth),
                Has.Some.EqualTo(42));
        }

        [Test]
        public void GivenFormattingTags_WhenDrawingFloatingText_ThenTagsDoNotConsumeWidth()
        {
            image.DrawFloatingText("@red@aa ~016~aa", 8, 8, FontIndex, 42, 8);

            Assert.That(image.Pixels, Has.Some.Not.Zero);
        }

        [Test]
        public void GivenInvalidTextInput_WhenDrawingIt_ThenRendererFailuresAreContained()
        {
            Assert.That(() => image.DrawString(null!, 4, 8, FontIndex, 42), Throws.Nothing);
            Assert.That(() => image.DrawFloatingText(null!, 4, 8, FontIndex, 42, 8), Throws.Nothing);
            Assert.That(() => image.DrawString("a", 4, 8, 42, 42), Throws.Nothing);
            Assert.That(() => image.DrawFloatingText("a", 4, 8, 42, 42, 8), Throws.Nothing);
        }

        [Test]
        public void GivenInvalidMeasurementInput_WhenMeasuringIt_ThenTheUnderlyingExceptionIsPreserved()
        {
            Assert.That(
                () => image.TextWidth(null!, FontIndex),
                Throws.TypeOf<NullReferenceException>());
            Assert.That(
                () => image.TextWidth("a", 42),
                Throws.TypeOf<NullReferenceException>());
        }

        private int GetPixel(int positionX, int positionY)
            => image.Pixels[positionX + positionY * image.GameWidth];
    }
}