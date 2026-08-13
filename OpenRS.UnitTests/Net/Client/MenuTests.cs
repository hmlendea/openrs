using System;

using NUnit.Framework;

using Microsoft.Xna.Framework.Input;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    [NonParallelizable]
    public sealed class MenuTests
    {
        private static int Capacity => 8;

        private GameImage gameImage = null!;
        private Menu menu = null!;

        [SetUp]
        public void SetUp()
        {
            Menu.redMod = 114;
            Menu.greenMod = 114;
            Menu.blueMod = 176;
            gameImage = new GameImage(128, 96, 8);
            menu = new Menu(gameImage, Capacity);
        }

        [TearDown]
        public void TearDown()
        {
            Menu.redMod = 114;
            Menu.greenMod = 114;
            Menu.blueMod = 176;
        }

        [Test]
        public void GivenACapacity_WhenConstructingAMenu_ThenPublicComponentStateArraysAreInitialised()
        {
            Assert.That(menu.componentAcceptsInput, Has.Length.EqualTo(Capacity));
            Assert.That(menu.isScrollDragging, Has.Length.EqualTo(Capacity));
            Assert.That(menu.componentIsPasswordField, Has.Length.EqualTo(Capacity));
            Assert.That(menu.componentSkip, Has.Length.EqualTo(Capacity));
            Assert.That(menu.listShownEntries, Has.Length.EqualTo(Capacity));
            Assert.That(menu.listLength, Has.Length.EqualTo(Capacity));
            Assert.That(menu.componentSelectedIndex, Has.Length.EqualTo(Capacity));
            Assert.That(menu.componentHighlightedIndex, Has.Length.EqualTo(Capacity));
            Assert.That(menu.isListSelectionHighlighted);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(18, 52, 86, 0x123456)]
        [TestCase(255, 255, 255, 0xffffff)]
        [TestCase(4, 8, 16, 0x040810)]
        public void GivenDefaultColourModifiers_WhenConvertingRgb_ThenChannelsArePreserved(
            int red,
            int green,
            int blue,
            int expectedColour)
            => Assert.That(menu.RgbToInt(red, green, blue), Is.EqualTo(expectedColour));

        [Test]
        public void GivenCustomColourModifiers_WhenConvertingRgb_ThenEachChannelIsScaled()
        {
            Menu.redMod = 57;
            Menu.greenMod = 228;
            Menu.blueMod = 88;

            int colour = menu.RgbToInt(114, 57, 176);

            Assert.That(colour, Is.EqualTo(0x397258));
        }

        [Test]
        public void GivenDifferentComponentTypes_WhenCreatingThem_ThenSequentialIndicesAreReturned()
        {
            gameImage.PictureWidth[0] = 16;
            gameImage.PictureHeight[0] = 32;

            Assert.That(menu.DrawText(4, 8, "RuneScape", 0, true), Is.Zero);
            Assert.That(menu.DrawButton(16, 32, 42, 48), Is.EqualTo(1));
            Assert.That(menu.DrawCurvedBox(16, 32, 42, 48), Is.EqualTo(2));
            Assert.That(menu.DrawArrow(16, 32, 0), Is.EqualTo(3));
            Assert.That(menu.CreateScrollableTextBox(4, 8, 16, 32, 0, 42, true), Is.EqualTo(4));
            Assert.That(menu.CreateTextInput(4, 8, 16, 32, 0, 42, false, true), Is.EqualTo(5));
            Assert.That(menu.CreateInput(4, 8, 16, 32, 0, 42, true, false), Is.EqualTo(6));
            Assert.That(menu.CreateButton(16, 32, 42, 48), Is.EqualTo(7));
            Assert.That(menu.componentAcceptsInput, Has.All.True);
        }

        [Test]
        public void GivenFullComponentCapacity_WhenCreatingAnotherComponent_ThenAnIndexExceptionIsThrown()
        {
            for (int componentIndex = 0; componentIndex < Capacity; componentIndex += 1)
            {
                menu.CreateButton(16, 32, 42, 48);
            }

            Assert.That(
                () => menu.CreateButton(16, 32, 42, 48),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenATextComponent_WhenUpdatingAndReadingIt_ThenTheTextIsRetained()
        {
            int componentIndex = menu.DrawText(4, 8, "RuneScape", 0, true);

            menu.UpdateText(componentIndex, "Dark Souls III");

            Assert.That(menu.GetText(componentIndex), Is.EqualTo("Dark Souls III"));
        }

        [Test]
        public void GivenNullText_WhenUpdatingAComponent_ThenTheCompatibilityTextIsReturned()
        {
            int componentIndex = menu.DrawText(4, 8, "RuneScape", 0, true);

            menu.UpdateText(componentIndex, null!);

            Assert.That(menu.GetText(componentIndex), Is.EqualTo("null"));
        }

        [Test]
        public void GivenANonTextComponent_WhenReadingItsText_ThenTheCompatibilityTextIsReturned()
        {
            int componentIndex = menu.CreateButton(16, 32, 42, 48);

            Assert.That(menu.GetText(componentIndex), Is.EqualTo("null"));
        }

        [TestCase(Keys.A, 'a')]
        [TestCase(Keys.Z, 'Z')]
        [TestCase(Keys.D4, '4')]
        [TestCase(Keys.Space, ' ')]
        [TestCase(Keys.OemPlus, '+')]
        [TestCase(Keys.OemQuestion, '?')]
        public void GivenAnAllowedCharacterAndFocusedInput_WhenPressingIt_ThenItIsAppended(
            Keys key,
            char character)
        {
            int componentIndex = CreateFocusedTextInput(8);

            menu.KeyPress(key, character);

            Assert.That(menu.GetText(componentIndex), Is.EqualTo(character.ToString()));
        }

        [TestCase('\0')]
        [TestCase('`')]
        [TestCase('§')]
        [TestCase('€')]
        public void GivenAnUnsupportedCharacter_WhenPressingIt_ThenTextIsUnchanged(char character)
        {
            int componentIndex = CreateFocusedTextInput(8);

            menu.KeyPress(Keys.A, character);

            Assert.That(menu.GetText(componentIndex), Is.Empty);
        }

        [Test]
        public void GivenAZeroKey_WhenPressingAnAllowedCharacter_ThenTextIsUnchanged()
        {
            int componentIndex = CreateFocusedTextInput(8);

            menu.KeyPress(0, 'a');

            Assert.That(menu.GetText(componentIndex), Is.Empty);
        }

        [Test]
        public void GivenNoFocusedInput_WhenPressingAnAllowedCharacter_ThenNoComponentChanges()
        {
            int componentIndex = menu.CreateTextInput(4, 8, 16, 32, 0, 8, false, true);

            menu.KeyPress(Keys.A, 'a');

            Assert.That(menu.GetText(componentIndex), Is.Empty);
        }

        [Test]
        public void GivenADisabledFocusedInput_WhenPressingAnAllowedCharacter_ThenTextIsUnchanged()
        {
            int componentIndex = CreateFocusedTextInput(8);
            menu.DisableInput(componentIndex);

            menu.KeyPress(Keys.A, 'a');

            Assert.That(menu.GetText(componentIndex), Is.Empty);
        }

        [Test]
        public void GivenTextAtMaximumLength_WhenPressingAnotherCharacter_ThenItIsNotAppended()
        {
            int componentIndex = CreateFocusedTextInput(4);
            menu.UpdateText(componentIndex, "Test");

            menu.KeyPress(Keys.Y, 'y');

            Assert.That(menu.GetText(componentIndex), Is.EqualTo("Test"));
        }

        [Test]
        public void GivenNonEmptyText_WhenPressingBackspace_ThenTheLastCharacterIsRemoved()
        {
            int componentIndex = CreateFocusedTextInput(8);
            menu.UpdateText(componentIndex, "Test");

            menu.KeyPress(Keys.Back, '\0');

            Assert.That(menu.GetText(componentIndex), Is.EqualTo("Tes"));
        }

        [Test]
        public void GivenEmptyText_WhenPressingBackspace_ThenItRemainsEmpty()
        {
            int componentIndex = CreateFocusedTextInput(8);

            menu.KeyPress(Keys.Back, '\0');

            Assert.That(menu.GetText(componentIndex), Is.Empty);
        }

        [Test]
        public void GivenNonEmptyText_WhenPressingEnter_ThenTheComponentReportsOneClick()
        {
            int componentIndex = CreateFocusedTextInput(8);
            menu.UpdateText(componentIndex, "Test");

            menu.KeyPress(Keys.Enter, '\0');

            Assert.That(menu.IsClicked(componentIndex));
            Assert.That(menu.IsClicked(componentIndex), Is.False);
        }

        [Test]
        public void GivenEmptyText_WhenPressingEnter_ThenTheComponentDoesNotReportAClick()
        {
            int componentIndex = CreateFocusedTextInput(8);

            menu.KeyPress(Keys.Enter, '\0');

            Assert.That(menu.IsClicked(componentIndex), Is.False);
        }

        [Test]
        public void GivenTwoInputComponents_WhenPressingTabAndTyping_ThenFocusAdvances()
        {
            int firstComponent = menu.CreateTextInput(4, 8, 16, 32, 0, 8, false, true);
            int secondComponent = menu.CreateInput(16, 32, 42, 48, 0, 8, false, true);
            menu.SetFocus(firstComponent);

            menu.KeyPress(Keys.Tab, '\0');
            menu.KeyPress(Keys.A, 'a');

            Assert.That(menu.GetText(firstComponent), Is.Empty);
            Assert.That(menu.GetText(secondComponent), Is.EqualTo("a"));
        }

        [Test]
        public void GivenAnEnabledButton_WhenClickingInsideItsBounds_ThenItReportsOneClick()
        {
            int componentIndex = menu.CreateButton(32, 42, 16, 8);

            menu.MouseClick(32, 42, 1, 0);

            Assert.That(menu.IsClicked(componentIndex));
            Assert.That(menu.IsClicked(componentIndex), Is.False);
        }

        [TestCase(23, 38)]
        [TestCase(24, 37)]
        [TestCase(41, 42)]
        [TestCase(32, 47)]
        public void GivenAButton_WhenClickingOutsideItsBounds_ThenItDoesNotReportAClick(
            int mouseX,
            int mouseY)
        {
            int componentIndex = menu.CreateButton(32, 42, 16, 8);

            menu.MouseClick(mouseX, mouseY, 1, 0);

            Assert.That(menu.IsClicked(componentIndex), Is.False);
        }

        [TestCase(24, 38)]
        [TestCase(40, 38)]
        [TestCase(24, 46)]
        [TestCase(40, 46)]
        public void GivenAButton_WhenClickingOnAnInclusiveBoundary_ThenItReportsAClick(
            int mouseX,
            int mouseY)
        {
            int componentIndex = menu.CreateButton(32, 42, 16, 8);

            menu.MouseClick(mouseX, mouseY, 1, 0);

            Assert.That(menu.IsClicked(componentIndex));
        }

        [Test]
        public void GivenADisabledButton_WhenClickingInsideItsBounds_ThenItDoesNotReportAClick()
        {
            int componentIndex = menu.CreateButton(32, 42, 16, 8);
            menu.DisableInput(componentIndex);

            menu.MouseClick(32, 42, 1, 0);

            Assert.That(menu.IsClicked(componentIndex), Is.False);
        }

        [Test]
        public void GivenADisabledButton_WhenEnablingAndClickingIt_ThenItReportsAClick()
        {
            int componentIndex = menu.CreateButton(32, 42, 16, 8);
            menu.DisableInput(componentIndex);
            menu.EnableInput(componentIndex);

            menu.MouseClick(32, 42, 1, 0);

            Assert.That(menu.IsClicked(componentIndex));
        }

        [Test]
        public void GivenAList_WhenAddingSparseItems_ThenItsLengthIncludesTheHighestIndex()
        {
            int componentIndex = menu.CreateList(4, 8, 16, 32, 0, 8, true);

            menu.AddListItem(componentIndex, 4, "RuneScape");
            menu.AddListItem(componentIndex, 1, "Dark Souls III");

            Assert.That(menu.listLength[componentIndex], Is.EqualTo(5));
        }

        [Test]
        public void GivenAListWithState_WhenSwitchingIt_ThenScrollAndHighlightStateAreReset()
        {
            int componentIndex = menu.CreateList(4, 8, 16, 32, 0, 8, true);
            menu.listShownEntries[componentIndex] = 42;
            menu.componentHighlightedIndex[componentIndex] = 4;

            menu.SwitchList(componentIndex);

            Assert.That(menu.listShownEntries[componentIndex], Is.Zero);
            Assert.That(menu.GetEntryHighlighted(componentIndex), Is.EqualTo(-1));
        }

        [Test]
        public void GivenAListWithItems_WhenClearingIt_ThenItsLengthIsZero()
        {
            int componentIndex = menu.CreateList(4, 8, 16, 32, 0, 8, true);
            menu.AddListItem(componentIndex, 4, "RuneScape");

            menu.ClearList(componentIndex);

            Assert.That(menu.listLength[componentIndex], Is.Zero);
        }

        [TestCase(-1)]
        [TestCase(8)]
        [TestCase(42)]
        public void GivenAnInvalidComponentIndex_WhenReadingClickState_ThenAnIndexExceptionIsThrown(
            int componentIndex)
            => Assert.That(
                () => menu.IsClicked(componentIndex),
                Throws.TypeOf<IndexOutOfRangeException>());

        private int CreateFocusedTextInput(int maximumLength)
        {
            int componentIndex = menu.CreateTextInput(
                4,
                8,
                16,
                32,
                0,
                maximumLength,
                false,
                true);
            menu.SetFocus(componentIndex);

            return componentIndex;
        }
    }
}