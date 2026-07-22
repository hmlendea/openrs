using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Input
{
    public sealed class InputHandler(GameClient client)
    {
        private readonly ContextMenuInputHandler contextMenuInputHandler = new(client);
        private readonly GameInputHandler gameInputHandler = new(client);
        private readonly LoginScreenInputHandler loginScreenInputHandler = new(client);
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<InputHandler>();
        private readonly List<Keys> lastPressedKeys = [];
        private int lastMouseX;
        private int lastMouseY;
        private bool lastLeftDown;
        private bool lastRightDown;
        private bool shiftKeyIsDown;
        private bool ctrlKeyIsDown;
        private bool altKeyIsDown;
        private TimeSpan timeLapse;

        private static int CameraRotationDriftThreshold => 500;

        private static int ChatTabFlashMinimum => 0;

        private static TimeSpan KeyRepeatThreshold => TimeSpan.FromMilliseconds(150);

        private static int MouseTrailIndexMask => 0x1fff;

        private static int MouseTrailLookbackMaximum => 4000;

        private static int MouseTrailLookbackMinimum => 10;

        public void CheckGameInputs() => gameInputHandler.CheckGameInputs();

        public void CheckInputs()
        {
            if (client.memoryError || client.errorLoading)
            {
                return;
            }

            try
            {
                client.tick += 1;
                ProcessCurrentInputState();
                client.lastMouseButton = 0;
                UpdateCameraRotationDrift();
                UpdateChatTabFlashTimers();
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.ProcessInput,
                    "The CheckInputs call has failed.",
                    exception);
                client.CleanUp();
                client.memoryError = true;
            }
        }

        public void CheckLoginScreenInputs() =>
            loginScreenInputHandler.CheckLoginScreenInputs();

        public void CheckMouseStatus() => contextMenuInputHandler.CheckMouseStatus();

        public void HandleKeyDown(Keys key, char character)
        {
            if (IsArrowKey(key))
            {
                return;
            }

            if (!client.loggedIn)
            {
                HandleLoginKeyDown(key, character);
            }

            if (client.loggedIn)
            {
                HandleGameKeyDown(key, character);
            }
        }

        public void HandleMouseDown(
            int pressedMouseButton,
            int mouseXPosition,
            int mouseYPosition)
        {
            RecordMouseTrailPosition(mouseXPosition, mouseYPosition);
            TryLogoutFromRepeatedMouseTrail(mouseXPosition, mouseYPosition);
        }

        public char TranslateOemKeys(Keys key)
        {
            if (key == Keys.OemPeriod)
            {
                return '.';
            }

            if (shiftKeyIsDown)
            {
                return TranslateShiftedKey(key);
            }

            if (altKeyIsDown && ctrlKeyIsDown)
            {
                return TranslateAltGrKey(key);
            }

            return char.ToLower((char)key);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            List<Keys> pressedKeys = [.. keyboardState.GetPressedKeys()];
            MouseState adjustedMouseState = BuildAdjustedMouseState(Mouse.GetState());

            UpdateModifierKeyStates(pressedKeys);
            ProcessPressedKeys(pressedKeys);
            ProcessReleasedKeys(pressedKeys);
            UpdateLastPressedKeys(pressedKeys);
            timeLapse += gameTime.ElapsedGameTime;
            HandleMouseMovement(adjustedMouseState);
            HandleMouseButtons(adjustedMouseState);
        }

        private static bool IsArrowKey(Keys key)
        {
            if (key == Keys.Left || key == Keys.Right)
            {
                return true;
            }

            return key == Keys.Up || key == Keys.Down;
        }

        private static char TranslateAltGrKey(Keys key)
            => key switch
            {
                Keys.NumPad2 or Keys.D2 => '@',
                Keys.NumPad3 or Keys.D3 => '£',
                Keys.NumPad4 or Keys.D4 => '$',
                Keys.NumPad7 or Keys.D7 => '{',
                Keys.NumPad8 or Keys.D8 => '[',
                Keys.NumPad9 or Keys.D9 => ']',
                Keys.NumPad0 or Keys.D0 => '}',
                Keys.OemPlus => '\\',
                _ => (char)key
            };

        private static char TranslateShiftedKey(Keys key)
            => key switch
            {
                Keys.NumPad1 or Keys.D1 => '!',
                Keys.NumPad2 or Keys.D2 => '"',
                Keys.NumPad3 or Keys.D3 => '#',
                Keys.NumPad4 or Keys.D4 => '¤',
                Keys.NumPad5 or Keys.D5 => '%',
                Keys.NumPad6 or Keys.D6 => '&',
                Keys.NumPad7 or Keys.D7 => '/',
                Keys.NumPad8 or Keys.D8 => '(',
                Keys.NumPad9 or Keys.D9 => ')',
                Keys.NumPad0 or Keys.D0 => '=',
                Keys.OemPlus => '?',
                _ => (char)key
            };

        private void ApplyCameraRotationDriftStep()
        {
            client.cameraRotateTime = 0;
            int randomFlags = (int)(Helper.Random.NextDouble() * 4D);

            if ((randomFlags & 1) == 1)
            {
                client.cameraRotationXAmount += client.cameraRotationXIncrement;
            }

            if ((randomFlags & 2) == 2)
            {
                client.cameraRotationYAmount += client.cameraRotationYIncrement;
            }
        }

        private MouseState BuildAdjustedMouseState(MouseState mouseState)
        {
            int adjustedMouseX = mouseState.X;
            int adjustedMouseY = mouseState.Y;

            if (GameClient.GameWindow is not null)
            {
                adjustedMouseX += GameClient.GameWindow.ClientBounds.X;
                adjustedMouseY += GameClient.GameWindow.ClientBounds.Y;
            }

            return new MouseState(
                adjustedMouseX,
                adjustedMouseY,
                mouseState.ScrollWheelValue,
                mouseState.LeftButton,
                mouseState.MiddleButton,
                mouseState.RightButton,
                mouseState.XButton1,
                mouseState.XButton2);
        }

        private int GetWrappedTrailIndex(int trailIndex) => trailIndex & MouseTrailIndexMask;

        private void HandleGameKeyDown(Keys key, char character)
        {
            if (key == Keys.F12)
            {
                client.TakeScreenshot(true);
                return;
            }

            if (client.showAppearanceWindow && client.appearanceMenu is not null)
            {
                client.appearanceMenu.KeyPress(key, character);
                return;
            }

            if (client.showFriendsBox == 0 &&
                client.showAbuseBox == 0 &&
                !client.isSleeping &&
                client.chatInputMenu is not null)
            {
                client.chatInputMenu.KeyPress(key, character);
            }
        }

        private void HandleLeftMouseButton(MouseState adjustedMouseState)
        {
            if (adjustedMouseState.LeftButton == ButtonState.Pressed && !lastLeftDown)
            {
                lastLeftDown = true;
                client.MouseDown(adjustedMouseState.X, adjustedMouseState.Y, false);
            }

            if (adjustedMouseState.LeftButton == ButtonState.Released && lastLeftDown)
            {
                lastLeftDown = false;
                client.MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }
        }

        private void HandleLoginKeyDown(Keys key, char character)
        {
            if (client.loginScreen == 0 && client.loginMenuFirst is not null)
            {
                client.loginMenuFirst.KeyPress(key, character);
            }

            if (client.loginScreen == 1 && client.loginNewUser is not null)
            {
                client.loginNewUser.KeyPress(key, character);
            }

            if (client.loginScreen == 2 && client.loginMenuLogin is not null)
            {
                client.loginMenuLogin.KeyPress(key, character);
            }
        }

        private void HandleMouseButtons(MouseState adjustedMouseState)
        {
            HandleRightMouseButton(adjustedMouseState);
            HandleLeftMouseButton(adjustedMouseState);
        }

        private void HandleMouseMovement(MouseState adjustedMouseState)
        {
            if (adjustedMouseState.X == lastMouseX && adjustedMouseState.Y == lastMouseY)
            {
                return;
            }

            client.MouseMove(adjustedMouseState.X, adjustedMouseState.Y);
            lastMouseX = adjustedMouseState.X;
            lastMouseY = adjustedMouseState.Y;
        }

        private void HandleRightMouseButton(MouseState adjustedMouseState)
        {
            if (adjustedMouseState.RightButton == ButtonState.Pressed && !lastRightDown)
            {
                lastRightDown = true;
                client.MouseDown(
                    adjustedMouseState.X,
                    adjustedMouseState.Y,
                    adjustedMouseState.LeftButton == ButtonState.Pressed);
                client.MousePressed(adjustedMouseState);
            }

            if (adjustedMouseState.RightButton == ButtonState.Released && lastRightDown)
            {
                lastRightDown = false;
                client.MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }
        }

        private bool HasRepeatedTrailMismatch(
            int historicTrailIndex,
            int lookbackLength,
            int mouseXPosition,
            int mouseYPosition)
        {
            bool hasTrailMismatch = false;

            for (int trailSearchIndex = 1;
                trailSearchIndex < lookbackLength;
                trailSearchIndex += 1)
            {
                int recentTrailIndex = GetWrappedTrailIndex(
                    client.mouseTrailIndex - trailSearchIndex);
                int olderTrailIndex = GetWrappedTrailIndex(
                    historicTrailIndex - trailSearchIndex);

                if (client.mouseTrailX[olderTrailIndex] != mouseXPosition ||
                    client.mouseTrailY[olderTrailIndex] != mouseYPosition)
                {
                    hasTrailMismatch = true;
                }

                if (client.mouseTrailX[recentTrailIndex] !=
                    client.mouseTrailX[olderTrailIndex] ||
                    client.mouseTrailY[recentTrailIndex] !=
                    client.mouseTrailY[olderTrailIndex])
                {
                    return false;
                }
            }

            return hasTrailMismatch;
        }

        private bool IsHistoricTrailMatch(
            int lookbackLength,
            int mouseXPosition,
            int mouseYPosition)
        {
            int historicTrailIndex = GetWrappedTrailIndex(client.mouseTrailIndex - lookbackLength);

            if (client.mouseTrailX[historicTrailIndex] != mouseXPosition)
            {
                return false;
            }

            return client.mouseTrailY[historicTrailIndex] == mouseYPosition;
        }

        private bool IsKeyHeldDown(Keys pressedKey) => lastPressedKeys.Contains(pressedKey);

        private void ProcessCurrentInputState()
        {
            if (!client.loggedIn)
            {
                CheckLoginScreenInputs();
            }

            if (client.loggedIn)
            {
                CheckGameInputs();
            }
        }

        private void ProcessPressedKeys(IEnumerable<Keys> pressedKeys)
        {
            foreach (Keys pressedKey in pressedKeys)
            {
                if (!ShouldRepeatKeyPress(pressedKey))
                {
                    continue;
                }

                client.KeyDown(pressedKey, TranslateOemKeys(pressedKey));
                timeLapse = TimeSpan.Zero;
            }
        }

        private void ProcessReleasedKeys(IEnumerable<Keys> pressedKeys)
        {
            foreach (Keys lastPressedKey in lastPressedKeys)
            {
                if (pressedKeys.Contains(lastPressedKey))
                {
                    continue;
                }

                client.KeyUp(lastPressedKey, TranslateOemKeys(lastPressedKey));
            }
        }

        private void RecordMouseTrailPosition(int mouseXPosition, int mouseYPosition)
        {
            client.mouseTrailX[client.mouseTrailIndex] = mouseXPosition;
            client.mouseTrailY[client.mouseTrailIndex] = mouseYPosition;
            client.mouseTrailIndex = GetWrappedTrailIndex(client.mouseTrailIndex + 1);
        }

        private bool ShouldRepeatKeyPress(Keys pressedKey)
        {
            if (!IsKeyHeldDown(pressedKey))
            {
                return true;
            }

            return timeLapse > KeyRepeatThreshold;
        }

        private void TryLogoutFromRepeatedMouseTrail(
            int mouseXPosition,
            int mouseYPosition)
        {
            for (int lookbackLength = MouseTrailLookbackMinimum;
                lookbackLength < MouseTrailLookbackMaximum;
                lookbackLength += 1)
            {
                if (!IsHistoricTrailMatch(lookbackLength, mouseXPosition, mouseYPosition))
                {
                    continue;
                }

                int historicTrailIndex = GetWrappedTrailIndex(
                    client.mouseTrailIndex - lookbackLength);

                if (!HasRepeatedTrailMismatch(
                    historicTrailIndex,
                    lookbackLength,
                    mouseXPosition,
                    mouseYPosition))
                {
                    continue;
                }

                if (client.combatTimeout == 0 && client.logoutTimer == 0)
                {
                    client.SendLogout();
                    return;
                }
            }
        }

        private void UpdateCameraRotationDrift()
        {
            client.cameraRotateTime += 1;

            if (client.cameraRotateTime > CameraRotationDriftThreshold)
            {
                ApplyCameraRotationDriftStep();
            }

            if (client.cameraRotationXAmount < -50)
            {
                client.cameraRotationXIncrement = 2;
            }

            if (client.cameraRotationXAmount > 50)
            {
                client.cameraRotationXIncrement = -2;
            }

            if (client.cameraRotationYAmount < -50)
            {
                client.cameraRotationYIncrement = 2;
            }

            if (client.cameraRotationYAmount > 50)
            {
                client.cameraRotationYIncrement = -2;
            }
        }

        private void UpdateChatTabFlashTimers()
        {
            if (client.chatTabAllMsgFlash > ChatTabFlashMinimum)
            {
                client.chatTabAllMsgFlash -= 1;
            }

            if (client.chatTabHistoryFlash > ChatTabFlashMinimum)
            {
                client.chatTabHistoryFlash -= 1;
            }

            if (client.chatTabQuestFlash > ChatTabFlashMinimum)
            {
                client.chatTabQuestFlash -= 1;
            }

            if (client.chatTabPrivateFlash > ChatTabFlashMinimum)
            {
                client.chatTabPrivateFlash -= 1;
            }
        }

        private void UpdateLastPressedKeys(IEnumerable<Keys> pressedKeys)
        {
            lastPressedKeys.Clear();
            lastPressedKeys.AddRange(pressedKeys);
        }

        private void UpdateModifierKeyStates(IEnumerable<Keys> pressedKeys)
        {
            shiftKeyIsDown = pressedKeys.Any(
                pressedKey => pressedKey == Keys.LeftShift || pressedKey == Keys.RightShift);
            ctrlKeyIsDown = pressedKeys.Any(
                pressedKey =>
                    pressedKey == Keys.LeftControl ||
                    pressedKey == Keys.RightControl);
            altKeyIsDown = pressedKeys.Any(
                pressedKey => pressedKey == Keys.LeftAlt || pressedKey == Keys.RightAlt);
        }
    }
}