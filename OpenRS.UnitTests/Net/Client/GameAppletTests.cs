using System;

using NUnit.Framework;

using Microsoft.Xna.Framework.Input;

using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    [NonParallelizable]
    public sealed class GameAppletTests
    {
        private GameAppletTestDouble applet = null!;

        [SetUp]
        public void SetUp()
        {
            GameApplet.gameFrame = null!;
            applet = new GameAppletTestDouble();
            applet.InitGameApplet();
        }

        [TearDown]
        public void TearDown() => GameApplet.gameFrame = null!;

        [Test]
        public void GivenANewInitialisedApplet_WhenReadingItsState_ThenCompatibleDefaultsAreUsed()
        {
            Assert.That(applet.gameLoadingScreen, Is.EqualTo(1));
            Assert.That(applet.gameLoadingFileTitle, Is.EqualTo("Loading"));
            Assert.That(applet.gameMinThreadSleepTime, Is.EqualTo(1));
            Assert.That(applet.keyLeftDown, Is.False);
            Assert.That(applet.keyRightDown, Is.False);
            Assert.That(applet.keyUpDown, Is.False);
            Assert.That(applet.keyDownDown, Is.False);
            Assert.That(applet.keySpaceDown, Is.False);
            Assert.That(applet.keyNMDown, Is.False);
            Assert.That(applet.keyF1Toggle, Is.False);
            Assert.That(applet.inputText, Is.Empty);
            Assert.That(applet.enteredInputText, Is.Empty);
            Assert.That(applet.pmText, Is.Empty);
            Assert.That(applet.enteredPMText, Is.Empty);
        }

        [TestCase(Keys.Left)]
        [TestCase(Keys.Right)]
        [TestCase(Keys.Up)]
        [TestCase(Keys.Down)]
        [TestCase(Keys.Space)]
        [TestCase(Keys.N)]
        [TestCase(Keys.M)]
        public void GivenAStateKey_WhenPressingAndReleasingIt_ThenItsStateIsUpdated(Keys key)
        {
            applet.KeyDown(key, '\0');

            Assert.That(GetKeyState(key));

            applet.KeyUp(key, '\0');

            Assert.That(GetKeyState(key), Is.False);
        }

        [Test]
        public void GivenF1_WhenPressingItTwice_ThenTheToggleChangesOnEachPress()
        {
            applet.KeyDown(Keys.F1, '\0');
            Assert.That(applet.keyF1Toggle);

            applet.KeyDown(Keys.F1, '\0');
            Assert.That(applet.keyF1Toggle, Is.False);
        }

        [Test]
        public void GivenAnAllowedCharacter_WhenTypingIt_ThenBothInputBuffersReceiveIt()
        {
            applet.KeyDown(Keys.A, 'a');

            Assert.That(applet.inputText, Is.EqualTo("a"));
            Assert.That(applet.pmText, Is.EqualTo("a"));
            Assert.That(applet.LastHandledKey, Is.EqualTo(Keys.A));
            Assert.That(applet.LastHandledCharacter, Is.EqualTo('a'));
        }

        [Test]
        public void GivenAnUnsupportedCharacter_WhenTypingIt_ThenInputBuffersRemainUnchanged()
        {
            applet.KeyDown(Keys.A, '€');

            Assert.That(applet.inputText, Is.Empty);
            Assert.That(applet.pmText, Is.Empty);
        }

        [Test]
        public void GivenFullInputBuffers_WhenTypingACharacter_ThenTheirIndependentLimitsAreApplied()
        {
            applet.inputText = new string('a', 20);
            applet.pmText = new string('b', 79);

            applet.KeyDown(Keys.C, 'c');

            Assert.That(applet.inputText, Has.Length.EqualTo(20));
            Assert.That(applet.pmText, Is.EqualTo(new string('b', 79) + "c"));
        }

        [Test]
        public void GivenText_WhenPressingBackspace_ThenTheLastCharacterIsRemovedFromBothBuffers()
        {
            applet.inputText = "RuneScape";
            applet.pmText = "Dark Souls III";

            applet.KeyDown(Keys.Back, '\0');

            Assert.That(applet.inputText, Is.EqualTo("RuneScap"));
            Assert.That(applet.pmText, Is.EqualTo("Dark Souls II"));
        }

        [Test]
        public void GivenEmptyText_WhenPressingBackspace_ThenItRemainsEmpty()
        {
            applet.KeyDown(Keys.Back, '\0');

            Assert.That(applet.inputText, Is.Empty);
            Assert.That(applet.pmText, Is.Empty);
        }

        [Test]
        public void GivenInputText_WhenPressingEnter_ThenSubmittedCopiesAreStored()
        {
            applet.inputText = "RuneScape";
            applet.pmText = "Dark Souls III";

            applet.KeyDown(Keys.Enter, '\0');

            Assert.That(applet.enteredInputText, Is.EqualTo("RuneScape"));
            Assert.That(applet.enteredPMText, Is.EqualTo("Dark Souls III"));
        }

        [Test]
        public void GivenKeyWrappers_WhenPressingAndReleasing_ThenTheyDelegateToKeyStateMethods()
        {
            applet.KeyPressed(Keys.Left);
            Assert.That(applet.keyLeftDown);

            applet.KeyReleased(Keys.Left);
            Assert.That(applet.keyLeftDown, Is.False);
        }

        [Test]
        public void GivenMouseCoordinates_WhenMovingAndReleasing_ThenCoordinatesAndButtonStateAreStored()
        {
            applet.mouseYOffset = 8;

            Assert.That(applet.MouseMove(42, 64));
            Assert.That(applet.mouseX, Is.EqualTo(42));
            Assert.That(applet.mouseY, Is.EqualTo(56));
            Assert.That(applet.mouseButton, Is.Zero);

            Assert.That(applet.MouseUp(16, 32));
            Assert.That(applet.mouseX, Is.EqualTo(16));
            Assert.That(applet.mouseY, Is.EqualTo(24));
            Assert.That(applet.mouseButton, Is.Zero);
        }

        [TestCase(false, 1)]
        [TestCase(true, 2)]
        public void GivenAMouseButton_WhenPressingIt_ThenItsStateAndCallbackAreUpdated(
            bool isMetaDown,
            int expectedButton)
        {
            applet.mouseYOffset = 8;

            Assert.That(applet.MouseDown(42, 64, isMetaDown));

            Assert.That(applet.mouseX, Is.EqualTo(42));
            Assert.That(applet.mouseY, Is.EqualTo(56));
            Assert.That(applet.mouseButton, Is.EqualTo(expectedButton));
            Assert.That(applet.lastMouseButton, Is.EqualTo(expectedButton));
            Assert.That(applet.LastPressedMouseButton, Is.EqualTo(expectedButton));
            Assert.That(applet.LastMouseX, Is.EqualTo(42));
            Assert.That(applet.LastMouseY, Is.EqualTo(64));
        }

        [TestCase(false, 1)]
        [TestCase(true, 2)]
        public void GivenAMouseButton_WhenDraggingIt_ThenCoordinatesAndButtonAreUpdated(
            bool isMetaDown,
            int expectedButton)
        {
            applet.mouseYOffset = 8;

            Assert.That(applet.MouseDrag(42, 64, isMetaDown));

            Assert.That(applet.mouseX, Is.EqualTo(42));
            Assert.That(applet.mouseY, Is.EqualTo(56));
            Assert.That(applet.mouseButton, Is.EqualTo(expectedButton));
        }

        [Test]
        public void GivenMouseStateWrappers_WhenInvoked_ThenTheyDelegateToMouseStateMethods()
        {
            MouseState leftPressed = BuildMouseState(ButtonState.Pressed, ButtonState.Released);
            MouseState rightPressed = BuildMouseState(ButtonState.Released, ButtonState.Pressed);

            applet.MouseEntered(leftPressed);
            Assert.That(applet.mouseX, Is.EqualTo(42));
            Assert.That(applet.mouseY, Is.EqualTo(64));

            applet.MouseExited(leftPressed);
            applet.MouseMoved(leftPressed);
            applet.MousePressed(rightPressed);
            Assert.That(applet.mouseButton, Is.EqualTo(2));

            applet.MouseReleased(leftPressed);
            Assert.That(applet.mouseButton, Is.Zero);

            applet.MouseDragged(rightPressed);
            Assert.That(applet.mouseX, Is.EqualTo(64));
            Assert.That(applet.mouseY, Is.EqualTo(42));
            Assert.That(applet.mouseButton, Is.EqualTo(2));
        }

        [Test]
        public void GivenARefreshRate_WhenStoppingAndStarting_ThenRunStatusUsesTheCompatibleDelay()
        {
            applet.SetRefreshRate(50);
            applet.Stop();
            Assert.That(applet.runStatus, Is.EqualTo(200));

            applet.Start();
            Assert.That(applet.runStatus, Is.Zero);
        }

        [Test]
        public void GivenANegativeRunStatus_WhenStartingOrStopping_ThenItRemainsUnchanged()
        {
            applet.runStatus = -2;
            applet.Start();
            applet.Stop();

            Assert.That(applet.runStatus, Is.EqualTo(-2));
        }

        [Test]
        public void GivenAWindowRequest_WhenCreatingIt_ThenTheFrameAndLoadingStateAreInitialised()
        {
            applet.CreateWindow(42, 64, "OpenRS", true);

            Assert.That(GameApplet.gameFrame, Is.Not.Null);
            Assert.That(GameApplet.gameFrame.frameWidth, Is.EqualTo(42));
            Assert.That(GameApplet.gameFrame.frameHeight, Is.EqualTo(64));
            Assert.That(GameApplet.gameFrame.gameApplet, Is.SameAs(applet));
            Assert.That(applet.gameLoadingScreen, Is.EqualTo(1));
        }

        [Test]
        public void GivenLoadingProgress_WhenUpdatingIt_ThenStatusFieldsAreStored()
        {
            applet.DrawLoadingBarText(42, "RuneScape");

            Assert.That(applet.gameLoadingPercentage, Is.EqualTo(42));
            Assert.That(applet.gameLoadingFileTitle, Is.EqualTo("RuneScape"));
        }

        [Test]
        public void GivenAnApplet_WhenInvokingNoOpCompatibilityMethods_ThenNoExceptionIsThrown()
        {
            Assert.That(() => applet.LoadGame(), Throws.Nothing);
            Assert.That(() => applet.CheckInputs(), Throws.Nothing);
            Assert.That(() => applet.Close(), Throws.Nothing);
            Assert.That(() => applet.KeyTyped(EventArgs.Empty), Throws.Nothing);
            Assert.That(() => applet.MouseClicked(EventArgs.Empty), Throws.Nothing);
            Assert.That(() => applet.LoadApp(), Throws.Nothing);
            Assert.That(() => applet.DrawWindow(), Throws.Nothing);
            Assert.That(() => applet.Paint(null!), Throws.Nothing);
            Assert.That(() => applet.MouseScroll(true, 42), Throws.Nothing);
            Assert.That(applet.GetCodeBase(), Is.Null);
            Assert.That(applet.GetDocumentBase(), Is.Null);
            Assert.That(applet.GetParameter("RuneScape"), Is.Empty);
        }

        [Test]
        public void GivenAnApplet_WhenMarkingDrawComplete_ThenDrawingIsRequested()
        {
            applet.OnDrawDone();

            Assert.That(applet.DrawIsNecessary);
        }

        [Test]
        public void GivenAnInitialisedTimingLoop_WhenUpdatingOnce_ThenInputIsCheckedAndAnimationAdvances()
        {
            applet.loadingAnimationCounter = 8;

            applet.UpdateGame(0, 256, 1, 0);

            Assert.That(applet.CheckInputsInvocationCount, Is.EqualTo(1));
            Assert.That(applet.loadingAnimationCounter, Is.EqualTo(7));
        }

        [Test]
        public void GivenTheClock_WhenReadingMilliseconds_ThenAPositiveUnixValueIsReturned()
            => Assert.That(GameApplet.CurrentTimeMillis(), Is.GreaterThan(0));

        [Test]
        public void GivenTimingStorage_WhenResettingIt_ThenNoExceptionIsThrown()
            => Assert.That(() => applet.ResetTimings(), Throws.Nothing);

        [Test]
        public void GivenANewApplet_WhenInitialisingIt_ThenAppletDefaultsAndCodeBaseAreAssigned()
        {
            GameApplet freshApplet = new();

            freshApplet.Init();

            Assert.That(freshApplet.gameLoadingScreen, Is.EqualTo(1));
            Assert.That(freshApplet.inputText, Is.Null);
            Assert.That(freshApplet.pmText, Is.Null);
        }

        [Test]
        public void GivenADirectionalKeyAndUnsupportedCharacter_WhenTypingIt_ThenTheDirectionStillPermitsTheCharacter()
        {
            applet.KeyDown(Keys.Left, '€');

            Assert.That(applet.inputText, Is.EqualTo("€"));
            Assert.That(applet.pmText, Is.EqualTo("€"));
        }

        [Test]
        public void GivenALoopWithZeroTimingMultiplier_WhenUpdatingIt_ThenTheMinimumMultiplierIsApplied()
        {
            applet.loadingAnimationCounter = 20;

            applet.UpdateGame(0, 0, 0, 0);

            Assert.That(applet.CheckInputsInvocationCount, Is.EqualTo(11));
            Assert.That(applet.keyF1Toggle, Is.False);
            Assert.That(applet.loadingAnimationCounter, Is.EqualTo(19));
        }

        [Test]
        public void GivenALoopAccumulatorAtTheThreshold_WhenUpdatingIt_ThenInputIsNotChecked()
        {
            applet.UpdateGame(0, 256, 1, 256);

            Assert.That(applet.CheckInputsInvocationCount, Is.Zero);
            Assert.That(applet.loadingAnimationCounter, Is.EqualTo(-1));
        }

        [Test]
        public void GivenAPositiveRunStatus_WhenUpdatingIt_ThenTheCountdownDecreases()
        {
            applet.runStatus = 2;

            applet.UpdateGame(0, 256, 1, 256);

            Assert.That(applet.runStatus, Is.EqualTo(1));
        }

        [Test]
        public void GivenLoadingScreenState_WhenPaintingIt_ThenLoadingValuesRemainAvailable()
        {
            applet.gameLoadingScreen = 2;
            applet.gameLoadingPercentage = 42;
            applet.gameLoadingFileTitle = "RuneScape";

            applet.Paint(null!);

            Assert.That(applet.gameLoadingPercentage, Is.EqualTo(42));
            Assert.That(applet.gameLoadingFileTitle, Is.EqualTo("RuneScape"));
        }

        [Test]
        public void GivenAZeroRefreshRate_WhenSettingIt_ThenADivideByZeroExceptionIsThrown()
            => Assert.That(
                () => applet.SetRefreshRate(0),
                Throws.TypeOf<DivideByZeroException>());

        [Test]
        public void GivenAStoppedApplet_WhenRunningIt_ThenTimingStateInitialisesAndReturns()
        {
            applet.gameLoadingScreen = 0;
            applet.runStatus = -2;

            applet.Run();

            Assert.That(applet.runStatus, Is.EqualTo(-2));
            Assert.That(applet.CloseInvocationCount, Is.Zero);
        }

        [Test]
        public void GivenACachedDataFile_WhenUnpackingIt_ThenTheRegisteredArrayIsReturned()
        {
            sbyte[] expectedData = [4, 8, 16, 32, 42];
            string fileName = $"coverage-{Guid.NewGuid():N}.jag";
            Link.AddFile(fileName, expectedData);

            sbyte[] data = applet.UnpackData(fileName, "RuneScape", 42);

            Assert.That(data, Is.SameAs(expectedData));
            Assert.That(applet.gameLoadingPercentage, Is.EqualTo(42));
            Assert.That(applet.gameLoadingFileTitle, Is.EqualTo("Unpacking RuneScape"));
        }

        [Test]
        public void GivenTheGraphicsCompatibilityConstructor_WhenConstructingWithNullDevices_ThenDefaultsAreInitialised()
        {
            GameApplet graphicsApplet = new(null!, null!);

            Assert.That(graphicsApplet.gameLoadingScreen, Is.EqualTo(1));
            Assert.That(graphicsApplet.inputText, Is.Empty);
            Assert.That(graphicsApplet.pmText, Is.Empty);
        }

        [Test]
        public void GivenAnApplet_WhenClosingTheProgram_ThenCloseIsInvokedAndStatusIsTerminal()
        {
            applet.CloseProgram();

            Assert.That(applet.runStatus, Is.EqualTo(-2));
            Assert.That(applet.CloseInvocationCount, Is.EqualTo(1));
        }

        private bool GetKeyState(Keys key) => key switch
        {
            Keys.Left => applet.keyLeftDown,
            Keys.Right => applet.keyRightDown,
            Keys.Up => applet.keyUpDown,
            Keys.Down => applet.keyDownDown,
            Keys.Space => applet.keySpaceDown,
            Keys.N => applet.keyNMDown,
            Keys.M => applet.keyNMDown,
            _ => false,
        };

        private static MouseState BuildMouseState(
            ButtonState leftButton,
            ButtonState rightButton)
            => new(
                42,
                64,
                0,
                leftButton,
                ButtonState.Released,
                rightButton,
                ButtonState.Released,
                ButtonState.Released);
    }

}