using System.Linq;

using NUnit.Framework;

using Microsoft.Xna.Framework.Input;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Game;
using OpenRS.UnitTests.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    [NonParallelizable]
    public sealed class MenuRenderingTests
    {
        private static int ImageWidth => 160;

        private static int ImageHeight => 128;

        private static int PictureCapacity => 8;

        private static int ComponentCapacity => 16;

        private static int FontIndex => 0;

        private GameImage gameImage = null!;
        private Menu menu = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp() => SyntheticGameFont.EnsureRegistered();

        [SetUp]
        public void SetUp()
        {
            Menu.baseScrollPic = 0;
            Menu.chatMenuTextHeightMod = 0;
            Menu.isBackgroundPatternEnabled = true;
            gameImage = new GameImage(ImageWidth, ImageHeight, PictureCapacity);
            RegisterMenuPictures();
            menu = new Menu(gameImage, ComponentCapacity);
        }

        [TearDown]
        public void TearDown()
        {
            Menu.baseScrollPic = 0;
            Menu.chatMenuTextHeightMod = 0;
            Menu.isBackgroundPatternEnabled = true;
        }

        [Test]
        public void GivenEveryPublicRenderableComponent_WhenDrawingTheMenu_ThenEachComponentContributesPixels()
        {
            int scrollableIndex = menu.CreateScrollableTextBox(4, 42, 48, 36, FontIndex, 8, true);
            int listIndex = menu.CreateList(108, 42, 48, 36, FontIndex, 8, false);
            int leftInputIndex = menu.CreateTextInput(4, 96, 48, 16, FontIndex, 16, true, true);
            int centredInputIndex = menu.CreateInput(80, 96, 48, 16, FontIndex, 16, false, false);
            menu.DrawText(80, 12, "RuneScape", FontIndex, true);
            menu.DrawButton(28, 28, 40, 20);
            menu.DrawCurvedBox(80, 28, 32, 20);
            menu.DrawArrow(140, 16, 0);
            AddListEntries(scrollableIndex, 6);
            AddListEntries(listIndex, 6);
            menu.UpdateText(leftInputIndex, "NucileRullz!");
            menu.UpdateText(centredInputIndex, "Dark Souls III");
            menu.SetFocus(leftInputIndex);

            menu.DrawMenu();

            Assert.That(gameImage.Pixels, Has.Some.Not.Zero);
            Assert.That(menu.listShownEntries[scrollableIndex], Is.Zero);
            Assert.That(menu.listShownEntries[listIndex], Is.Zero);
        }

        [Test]
        public void GivenPatternModes_WhenDrawingBackgroundPanels_ThenBothModesRenderAndResetClipping()
        {
            menu.DrawBackgroundPanel(4, 8, 48, 32);
            Menu.isBackgroundPatternEnabled = false;
            menu.DrawBackgroundPanel(60, 8, 48, 32);
            gameImage.DrawBox(0, 0, ImageWidth, 1, 42);

            Assert.That(gameImage.Pixels.Take(ImageWidth), Has.All.EqualTo(42));
            Assert.That(gameImage.Pixels.Skip(ImageWidth * 8).Take(ImageWidth), Has.Some.Not.Zero);
        }

        [Test]
        public void GivenMenuCornerPictures_WhenDrawingACurvedPanel_ThenItsInteriorAndCornersRender()
        {
            menu.DrawScrollCornerPanel(16, 24, 48, 32);

            Assert.That(gameImage.Pixels, Has.Some.Not.Zero);
            Assert.That(GetPixel(16, 24), Is.Not.Zero);
            Assert.That(GetPixel(57, 49), Is.Not.Zero);
        }

        [Test]
        public void GivenScrollableContent_WhenUsingBothArrowButtons_ThenTheVisibleEntryChanges()
        {
            int componentIndex = menu.CreateScrollableTextBox(8, 16, 64, 36, FontIndex, 8, true);
            AddListEntries(componentIndex, 8);
            menu.listShownEntries[componentIndex] = 2;
            int scrollbarX = 8 + 64 - 12;

            menu.MouseClick(scrollbarX + 1, 17, 0, 1);
            menu.DrawMenu();
            int afterUpArrow = menu.listShownEntries[componentIndex];
            menu.MouseClick(scrollbarX + 1, 16 + 35, 0, 1);
            menu.DrawMenu();

            Assert.That(afterUpArrow, Is.EqualTo(1));
            Assert.That(menu.listShownEntries[componentIndex], Is.EqualTo(2));
        }

        [Test]
        public void GivenScrollableContent_WhenDraggingAndReleasingTheThumb_ThenDragStateTracksTheMouse()
        {
            int componentIndex = menu.CreateScrollableTextBox(8, 16, 64, 60, FontIndex, 16, false);
            AddListEntries(componentIndex, 12);
            int scrollbarX = 8 + 64 - 12;

            menu.MouseClick(scrollbarX + 1, 48, 0, 1);
            menu.DrawMenu();

            Assert.That(menu.isScrollDragging[componentIndex]);
            Assert.That(menu.listShownEntries[componentIndex], Is.GreaterThan(0));

            menu.MouseClick(0, 0, 0, 0);
            menu.DrawMenu();

            Assert.That(menu.isScrollDragging[componentIndex], Is.False);
        }

        [Test]
        public void GivenASelectableList_WhenHoveringAndClickingAnEntry_ThenHighlightAndSelectionAreRecorded()
        {
            int componentIndex = menu.CreateList(8, 16, 72, 48, FontIndex, 8, true);
            AddListEntries(componentIndex, 3);

            menu.MouseClick(11, 25, 1, 0);
            menu.DrawMenu();

            Assert.That(menu.GetEntryHighlighted(componentIndex), Is.Zero);
            Assert.That(menu.componentSelectedIndex[componentIndex], Is.Zero);
            Assert.That(menu.IsClicked(componentIndex));
        }

        [Test]
        public void GivenASelectableListWithoutOverflow_WhenDrawingIt_ThenScrollAndHighlightStateAreNormalised()
        {
            int componentIndex = menu.CreateList(8, 16, 72, 72, FontIndex, 8, false);
            AddListEntries(componentIndex, 2);
            menu.listShownEntries[componentIndex] = 42;
            menu.componentHighlightedIndex[componentIndex] = 4;

            menu.DrawMenu();

            Assert.That(menu.listShownEntries[componentIndex], Is.Zero);
            Assert.That(menu.GetEntryHighlighted(componentIndex), Is.EqualTo(-1));
        }

        [Test]
        public void GivenBothInputAlignments_WhenClickingAndTyping_ThenFocusMovesToEachInput()
        {
            int leftInputIndex = menu.CreateTextInput(8, 24, 48, 16, FontIndex, 16, false, true);
            int centredInputIndex = menu.CreateInput(96, 24, 48, 16, FontIndex, 16, true, false);
            menu.UpdateText(centredInputIndex, "RuneScape");

            menu.MouseClick(12, 24, 1, 0);
            menu.DrawMenu();
            menu.KeyPress(Keys.A, 'a');
            menu.MouseClick(96, 24, 1, 0);
            menu.DrawMenu();
            menu.KeyPress(Keys.Z, 'z');

            Assert.That(menu.GetText(leftInputIndex), Is.EqualTo("a"));
            Assert.That(menu.GetText(centredInputIndex), Is.EqualTo("RuneScapez"));
        }

        [Test]
        public void GivenADisabledRenderableComponent_WhenDrawingTheMenu_ThenItIsSkipped()
        {
            int componentIndex = menu.DrawButton(32, 32, 32, 16);
            menu.DisableInput(componentIndex);

            menu.DrawMenu();

            Assert.That(gameImage.Pixels, Has.All.Zero);
        }

        private void AddListEntries(int componentIndex, int entryCount)
        {
            for (int entryIndex = 0; entryIndex < entryCount; entryIndex += 1)
            {
                menu.AddListItem(componentIndex, entryIndex, $"Entry {entryIndex}");
            }
        }

        private int GetPixel(int positionX, int positionY)
            => gameImage.Pixels[positionX + positionY * gameImage.GameWidth];

        private void RegisterMenuPictures()
        {
            for (int pictureIndex = 0; pictureIndex < PictureCapacity; pictureIndex += 1)
            {
                gameImage.PictureColours[pictureIndex] = [0x010101 * (pictureIndex + 1)];
                gameImage.PictureWidth[pictureIndex] = 1;
                gameImage.PictureHeight[pictureIndex] = 1;
                gameImage.PictureAssumedWidth[pictureIndex] = 1;
                gameImage.PictureAssumedHeight[pictureIndex] = 1;
            }
        }
    }
}