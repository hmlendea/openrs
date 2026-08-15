using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Gui.Controls;
using OpenRS.Settings;

namespace OpenRS.UnitTests.Gui.Controls
{
    [TestFixture]
    public sealed class GuiControlStateTests
    {
        [Test]
        public void GivenANewButton_WhenReadingIt_ThenCompatibleDefaultsAreUsed()
        {
            GuiButton button = new();

            Assert.That(button.Texture, Is.EqualTo("Interface/button"));
            Assert.That(button.FontName, Is.EqualTo("ButtonFont"));
            Assert.That(button.ButtonTileSize, Is.EqualTo(new Size2D(GameDefines.GuiTileSize, GameDefines.GuiTileSize)));
        }

        [Test]
        public void GivenButtonDimensions_WhenReadingTileCount_ThenIntegerTileDimensionsAreReturned()
        {
            GuiButton button = new()
            {
                Size = new Size2D(96, 64),
                ButtonTileSize = new Size2D(32, 16),
            };

            Assert.That(button.ButtonTileCount, Is.EqualTo(new Size2D(3, 4)));
        }

        [Test]
        public void GivenThreeButtonSections_WhenCalculatingRectangles_ThenLeftMiddleAndRightOffsetsAreUsed()
        {
            GuiButtonTestDouble button = new()
            {
                Size = new Size2D(96, 32),
                ButtonTileSize = new Size2D(32, 32),
            };

            Assert.That(button.CalculateRectangle(0), Is.EqualTo(new Rectangle2D(0, 0, 32, 32)));
            Assert.That(button.CalculateRectangle(1), Is.EqualTo(new Rectangle2D(32, 0, 32, 32)));
            Assert.That(button.CalculateRectangle(2), Is.EqualTo(new Rectangle2D(64, 0, 32, 32)));
        }

        [Test]
        public void GivenASingleButtonSection_WhenCalculatingItsRectangle_ThenTheCentreOffsetIsUsed()
        {
            GuiButtonTestDouble button = new()
            {
                Size = new Size2D(32, 32),
                ButtonTileSize = new Size2D(32, 32),
            };

            Assert.That(button.CalculateRectangle(0), Is.EqualTo(new Rectangle2D(96, 0, 32, 32)));
        }

        [Test]
        public void GivenANewToggleButton_WhenReadingIt_ThenCompatibleDefaultsAreUsed()
        {
            GuiToggleButton button = new();

            Assert.That(button.IsToggled, Is.False);
            Assert.That(button.ToggleColour, Is.EqualTo(Colour.DarkRed));
        }

        [Test]
        public void GivenANewSkillCard_WhenReadingIt_ThenCompatibleSizeAndStateAreUsed()
        {
            GuiSkillCard card = new()
            {
                SkillIcon = "RuneScape",
                CurrentLevel = 4,
                BaseLevel = 8,
                Experience = 16,
            };

            Assert.That(card.Size, Is.EqualTo(new Size2D(60, 32)));
            Assert.That(card.SkillIcon, Is.EqualTo("RuneScape"));
            Assert.That(card.CurrentLevel, Is.EqualTo(4));
            Assert.That(card.BaseLevel, Is.EqualTo(8));
            Assert.That(card.Experience, Is.EqualTo(16));
        }

        [Test]
        public void GivenTheItemSpriteContract_WhenReadingCanvasSize_ThenInventoryDimensionsAreUsed()
            => Assert.That(GuiItemCard.SpriteCanvasSize, Is.EqualTo(new Size2D(48, 32)));

        [Test]
        public void GivenANewItemCard_WhenReadingIt_ThenStateIsRetained()
        {
            GuiItemCard card = new() { SpriteName = "RuneScape", Quantity = 42 };

            Assert.That(card.SpriteName, Is.EqualTo("RuneScape"));
            Assert.That(card.Quantity, Is.EqualTo(42));
        }

        [Test]
        public void GivenEqualMinimapIndicatorValues_WhenReadingFillLevel_ThenItIsFull()
        {
            GuiMinimapIndicator indicator = new() { CurrentValue = 42, BaseValue = 42 };

            Assert.That(indicator.Size, Is.EqualTo(new Size2D(22, 22)));
            Assert.That(indicator.BackgroundColour, Is.EqualTo(Colour.White));
            Assert.That(indicator.FillLevel, Is.EqualTo(1F));
        }

        [Test]
        public void GivenDifferentMinimapIndicatorValues_WhenReadingFillLevel_ThenTheirRatioIsReturned()
        {
            GuiMinimapIndicator indicator = new()
            {
                CurrentValue = 4,
                BaseValue = 8,
                Icon = "RuneScape",
                IconRotation = 3.14F,
            };

            Assert.That(indicator.FillLevel, Is.EqualTo(0.5F));
            Assert.That(indicator.Icon, Is.EqualTo("RuneScape"));
            Assert.That(indicator.IconRotation, Is.EqualTo(3.14F));
        }

        [Test]
        public void GivenCombatStyleValues_WhenAssigningThem_ThenTheyAreRetained()
        {
            GuiCombatStyleCard card = new()
            {
                IsToggled = true,
                Icon = "RuneScape",
                CombatStyleName = "Dark Souls III",
            };

            Assert.That(card.IsToggled);
            Assert.That(card.Icon, Is.EqualTo("RuneScape"));
            Assert.That(card.CombatStyleName, Is.EqualTo("Dark Souls III"));
        }
    }
}