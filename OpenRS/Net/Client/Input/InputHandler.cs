using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NuciLog.Core;

using OpenRS.Localisation;
using OpenRS.Logging;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Input
{
    public sealed class InputHandler(GameClient client)
    {
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

        public char TranslateOemKeys(Keys key)
        {
            if (key == Keys.OemPeriod)
            {
                return '.';
            }
            else if (shiftKeyIsDown)
            {
                if (key == Keys.NumPad1 || key == Keys.D1)
                {
                    return '!';
                }
                else if (key == Keys.NumPad2 || key == Keys.D2)
                {
                    return '"';
                }
                else if (key == Keys.NumPad3 || key == Keys.D3)
                {
                    return '#';
                }
                else if (key == Keys.NumPad4 || key == Keys.D4)
                {
                    return '¤';
                }
                else if (key == Keys.NumPad5 || key == Keys.D5)
                {
                    return '%';
                }
                else if (key == Keys.NumPad6 || key == Keys.D6)
                {
                    return '&';
                }
                else if (key == Keys.NumPad7 || key == Keys.D7)
                {
                    return '/';
                }
                else if (key == Keys.NumPad8 || key == Keys.D8)
                {
                    return '(';
                }
                else if (key == Keys.NumPad9 || key == Keys.D9)
                {
                    return ')';
                }
                else if (key == Keys.NumPad0 || key == Keys.D0)
                {
                    return '=';
                }
                else if (key == Keys.OemPlus)
                {
                    return '?';
                }

                return (char)key;
            }
            else if (altKeyIsDown && ctrlKeyIsDown) // Alt Gr.
            {
                if (key == Keys.NumPad2 || key == Keys.D2)
                {
                    return '@';
                }
                else if (key == Keys.NumPad3 || key == Keys.D3)
                {
                    return '£';
                }
                else if (key == Keys.NumPad4 || key == Keys.D4)
                {
                    return '$';
                }
                else if (key == Keys.NumPad7 || key == Keys.D7)
                {
                    return '{';
                }
                else if (key == Keys.NumPad8 || key == Keys.D8)
                {
                    return '[';
                }
                else if (key == Keys.NumPad9 || key == Keys.D9)
                {
                    return ']';
                }
                else if (key == Keys.NumPad0 || key == Keys.D0)
                {
                    return '}';
                }
                else if (key == Keys.OemPlus)
                {
                    return '\\';
                }
            }
            else
            {
                return char.ToLower((char)key);
            }

            return (char)key;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            int rawX = mouseState.X;
            int rawY = mouseState.Y;

            if (GameClient.GameWindow is not null)
            {
                rawX += GameClient.GameWindow.ClientBounds.X;
                rawY += GameClient.GameWindow.ClientBounds.Y;
            }

            MouseState adjustedMouseState = new(
                rawX, rawY,
                mouseState.ScrollWheelValue,
                mouseState.LeftButton,
                mouseState.MiddleButton,
                mouseState.RightButton,
                mouseState.XButton1,
                mouseState.XButton2);
            List<Keys> keysPressedDown = [.. keyboardState.GetPressedKeys()];

            shiftKeyIsDown = keysPressedDown.Any(key => key == Keys.LeftShift || key == Keys.RightShift);
            ctrlKeyIsDown = keysPressedDown.Any(key => key == Keys.LeftControl || key == Keys.RightControl);
            altKeyIsDown = keysPressedDown.Any(key => key == Keys.LeftAlt || key == Keys.RightAlt);

            foreach (Keys pressedKey in keysPressedDown)
            {
                // if (timeLapse > TimeSpan.FromMilliseconds(100))
                if (!lastPressedKeys.Contains(pressedKey))
                {
                    client.KeyDown(pressedKey, TranslateOemKeys(pressedKey));
                    timeLapse = TimeSpan.Zero;
                }
                else if (timeLapse > TimeSpan.FromMilliseconds(150))
                {
                    client.KeyDown(pressedKey, TranslateOemKeys(pressedKey));
                    timeLapse = TimeSpan.Zero;
                }
                // HandleKeyDown(k, c[0]);
            }

            foreach (Keys lastKey in lastPressedKeys)
            {
                if (!keysPressedDown.Contains(lastKey))
                {
                    client.KeyUp(lastKey, TranslateOemKeys(lastKey));
                }
            }

            lastPressedKeys.Clear();
            lastPressedKeys.AddRange(keyboardState.GetPressedKeys());

            timeLapse += gameTime.ElapsedGameTime;

            //mouseEntered(mouseState);
            if (adjustedMouseState.X != lastMouseX || adjustedMouseState.Y != lastMouseY)
            {
                client.MouseMove(adjustedMouseState.X, adjustedMouseState.Y);
                lastMouseX = adjustedMouseState.X;
                lastMouseY = adjustedMouseState.Y;
                //mouseButtonClick = 0;
            }

            if (adjustedMouseState.RightButton == ButtonState.Pressed && !lastRightDown)
            {
                lastRightDown = true;
                client.MouseDown(adjustedMouseState.X, adjustedMouseState.Y, adjustedMouseState.LeftButton == ButtonState.Pressed);
                client.MousePressed(adjustedMouseState);
            }

            if (adjustedMouseState.LeftButton == ButtonState.Pressed && !lastLeftDown)
            {
                lastLeftDown = true;
                client.MouseDown(adjustedMouseState.X, adjustedMouseState.Y, false);
            }

            if (adjustedMouseState.RightButton == ButtonState.Released && lastRightDown)
            {
                lastRightDown = false;
                client.MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }
            if (adjustedMouseState.LeftButton == ButtonState.Released && lastLeftDown)
            {
                lastLeftDown = false;
                client.MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }
        }

        public void CheckInputs()
        {
            if (client.memoryError)
            {
                return;
            }

            if (client.errorLoading)
            {
                return;
            }

            try
            {
                client.tick += 1;
                if (!client.loggedIn)
                {
                    CheckLoginScreenInputs();
                }
                if (client.loggedIn)
                {
                    CheckGameInputs();
                }
                client.lastMouseButton = 0;
                client.cameraRotateTime += 1;
                if (client.cameraRotateTime > 500)
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

                if (client.chatTabAllMsgFlash > 0)
                {
                    client.chatTabAllMsgFlash -= 1;
                }

                if (client.chatTabHistoryFlash > 0)
                {
                    client.chatTabHistoryFlash -= 1;
                }

                if (client.chatTabQuestFlash > 0)
                {
                    client.chatTabQuestFlash -= 1;
                }

                if (client.chatTabPrivateFlash > 0)
                {
                    client.chatTabPrivateFlash -= 1;
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.ProcessInput, "The CheckInputs call has failed.", exception);
                client.CleanUp();
                client.memoryError = true;
            }
        }

        public void CheckLoginScreenInputs()
        {
            if (client.socketTimeout > 0)
            {
                client.socketTimeout -= 1;
            }

            if (client.loginScreen == 0)
            {
                if (client.loginMenuFirst is null)
                {
                    return;
                }

                if (client.lastMouseButton != 0 || client.mouseButton != 0)
                {
                }

                client.loginMenuFirst.MouseClick(client.mouseX, client.mouseY, client.lastMouseButton, client.mouseButton);
                if (client.loginMenuFirst.IsClicked(client.loginButtonNewUser))
                {
                    client.loginScreen = 1;
                }

                if (client.loginMenuFirst.IsClicked(client.loginMenuLoginButton))
                {
                    client.loginScreen = 2;
                    client.loginMenuLogin.UpdateText(client.loginMenuStatusText, "Please enter your username and password");
                    client.loginMenuLogin.UpdateText(client.loginMenuUserText, "");
                    client.loginMenuLogin.UpdateText(client.loginMenuPasswordText, "");
                    client.loginMenuLogin.SetFocus(client.loginMenuUserText);
                    return;
                }
            }
            else if (client.loginScreen == 1)
            {
                if (client.loginNewUser is null)
                {
                    return;
                }

                client.loginNewUser.MouseClick(client.mouseX, client.mouseY, client.lastMouseButton, client.mouseButton);

                if (client.loginNewUser.IsClicked(client.loginMenuOkButton))
                {
                    client.loginScreen = 0;
                    return;
                }
            }
            else if (client.loginScreen == 2)
            {
                client.loginMenuLogin.MouseClick(client.mouseX, client.mouseY, client.lastMouseButton, client.mouseButton);

                if (client.loginMenuLogin.IsClicked(client.loginMenuCancelButton))
                {
                    client.loginScreen = 0;
                }

                if (client.loginMenuLogin.IsClicked(client.loginMenuUserText))
                {
                    client.loginMenuLogin.SetFocus(client.loginMenuPasswordText);
                }

                if (client.loginMenuLogin.IsClicked(client.loginMenuPasswordText) || client.loginMenuLogin.IsClicked(client.loginMenuOkLoginButton))
                {
                    client.loginUsername = client.loginMenuLogin.GetText(client.loginMenuUserText);
                    client.loginPassword = client.loginMenuLogin.GetText(client.loginMenuPasswordText);
                    client.Connect(client.loginUsername, client.loginPassword, false);
                }
            }
        }

        public void HandleMouseDown(int pressedMouseButton, int mouseXPosition, int mouseYPosition)
        {
            client.mouseTrailX[client.mouseTrailIndex] = mouseXPosition;
            client.mouseTrailY[client.mouseTrailIndex] = mouseYPosition;
            client.mouseTrailIndex = client.mouseTrailIndex + 1 & 0x1fff;

            for (int lookbackLength = 10; lookbackLength < 4000; lookbackLength += 1)
            {
                int historicTrailIndex = client.mouseTrailIndex - lookbackLength & 0x1fff;

                if (client.mouseTrailX[historicTrailIndex] == mouseXPosition && client.mouseTrailY[historicTrailIndex] == mouseYPosition)
                {
                    bool hasTrailMismatch = false;

                    for (int trailSearchIndex = 1; trailSearchIndex < lookbackLength; trailSearchIndex += 1)
                    {
                        int recentTrailIndex = client.mouseTrailIndex - trailSearchIndex & 0x1fff;
                        int olderTrailIndex = historicTrailIndex - trailSearchIndex & 0x1fff;

                        if (client.mouseTrailX[olderTrailIndex] != mouseXPosition || client.mouseTrailY[olderTrailIndex] != mouseYPosition)
                        {
                            hasTrailMismatch = true;
                        }

                        if (client.mouseTrailX[recentTrailIndex] != client.mouseTrailX[olderTrailIndex] || client.mouseTrailY[recentTrailIndex] != client.mouseTrailY[olderTrailIndex])
                        {
                            break;
                        }

                        if (trailSearchIndex == lookbackLength - 1 && hasTrailMismatch && client.combatTimeout == 0 && client.logoutTimer == 0)
                        {
                            client.SendLogout();
                            return;
                        }
                    }
                }
            }
        }

        public void CheckGameInputs()
        {
            if (client.ourPlayer is null)
            {
                client.SendPingPacketAsync();
                return;
            }

            if (client.systemUpdate > 1)
            {
                client.systemUpdate -= 1;
            }

            client.SendPingPacketAsync();

            if (client.logoutTimer > 0)
            {
                client.logoutTimer -= 1;
            }

            if (client.ourPlayer.CurrentSprite == 8 || client.ourPlayer.CurrentSprite == 9)
            {
                client.combatTimeout = 500;
            }

            if (client.combatTimeout > 0)
            {
                client.combatTimeout -= 1;
            }

            if (client.showAppearanceWindow)
            {
                client.UpdateAppearanceWindow();
                return;
            }
            for (int playerIndex = 0; playerIndex < client.playerCount; playerIndex += 1)
            {
                ClientMob player = client.playerArray[playerIndex];

                if (player is null)
                {
                    continue;
                }

                int nextWaypointIndex = (player.WaypointCurrent + 1) % 10;

                if (player.WaypointsEndSprite != nextWaypointIndex)
                {
                    int direction = -1;
                    int targetSprite = player.WaypointsEndSprite;
                    int waypointDistance;

                    if (targetSprite < nextWaypointIndex)
                    {
                        waypointDistance = nextWaypointIndex - targetSprite;
                    }
                    else
                    {
                        waypointDistance = 10 + nextWaypointIndex - targetSprite;
                    }

                    int movementSpeed = 4;

                    if (waypointDistance > 2)
                    {
                        movementSpeed = (waypointDistance - 1) * 4;
                    }

                    if (player.WaypointXPositions[targetSprite] - player.LocationX > client.gridSize * 3 || player.WaypointYPositions[targetSprite] - player.LocationY > client.gridSize * 3 || player.WaypointXPositions[targetSprite] - player.LocationX < -client.gridSize * 3 || player.WaypointYPositions[targetSprite] - player.LocationY < -client.gridSize * 3 || waypointDistance > 8)
                    {
                        player.LocationX = player.WaypointXPositions[targetSprite];
                        player.LocationY = player.WaypointYPositions[targetSprite];
                    }
                    else
                    {
                        if (player.LocationX < player.WaypointXPositions[targetSprite])
                        {
                            player.LocationX += movementSpeed;
                            player.StepCount += 1;
                            direction = 2;
                        }
                        else if (player.LocationX > player.WaypointXPositions[targetSprite])
                        {
                            player.LocationX -= movementSpeed;
                            player.StepCount += 1;
                            direction = 6;
                        }

                        if (player.LocationX - player.WaypointXPositions[targetSprite] < movementSpeed && player.LocationX - player.WaypointXPositions[targetSprite] > -movementSpeed)
                        {
                            player.LocationX = player.WaypointXPositions[targetSprite];
                        }

                        if (player.LocationY < player.WaypointYPositions[targetSprite])
                        {
                            player.LocationY += movementSpeed;
                            player.StepCount += 1;

                            if (direction == -1)
                            {
                                direction = 4;
                            }
                            else if (direction == 2)
                            {
                                direction = 3;
                            }
                            else
                            {
                                direction = 5;
                            }
                        }
                        else if (player.LocationY > player.WaypointYPositions[targetSprite])
                        {
                            player.LocationY -= movementSpeed;
                            player.StepCount += 1;

                            if (direction == -1)
                            {
                                direction = 0;
                            }
                            else if (direction == 2)
                            {
                                direction = 1;
                            }
                            else
                            {
                                direction = 7;
                            }
                        }

                        if (player.LocationY - player.WaypointYPositions[targetSprite] < movementSpeed && player.LocationY - player.WaypointYPositions[targetSprite] > -movementSpeed)
                        {
                            player.LocationY = player.WaypointYPositions[targetSprite];
                        }
                    }
                    if (direction != -1)
                    {
                        player.CurrentSprite = direction;
                    }

                    if (player.LocationX == player.WaypointXPositions[targetSprite] && player.LocationY == player.WaypointYPositions[targetSprite])
                    {
                        player.WaypointsEndSprite = (targetSprite + 1) % 10;
                    }
                }
                else
                {
                    player.CurrentSprite = player.NextSprite;
                }
                if (player.LastMessageTimeout > 0)
                {
                    player.LastMessageTimeout -= 1;
                }

                if (player.PlayerSkullTimeout > 0)
                {
                    player.PlayerSkullTimeout -= 1;
                }

                if (player.CombatTimer > 0)
                {
                    player.CombatTimer -= 1;
                }

                if (client.playerAliveTimeout > 0)
                {
                    client.playerAliveTimeout -= 1;
                    if (client.playerAliveTimeout == 0)
                    {
                        client.DisplayMessage(LocalisationManager.GetString("social.respawn_granted"), 3);
                    }

                    if (client.playerAliveTimeout == 0)
                    {
                        client.DisplayMessage(LocalisationManager.GetString("social.respawn_retain_skills"), 3);
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < client.npcCount; npcIndex += 1)
            {
                ClientMob npcMob = client.npcArray[npcIndex];
                int nextWaypointIndex = (npcMob.WaypointCurrent + 1) % 10;

                if (npcMob.WaypointsEndSprite != nextWaypointIndex)
                {
                    int direction = -1;
                    int targetSprite = npcMob.WaypointsEndSprite;
                    int waypointDistance;

                    if (targetSprite < nextWaypointIndex)
                    {
                        waypointDistance = nextWaypointIndex - targetSprite;
                    }
                    else
                    {
                        waypointDistance = 10 + nextWaypointIndex - targetSprite;
                    }

                    int movementSpeed = 4;

                    if (waypointDistance > 2)
                    {
                        movementSpeed = (waypointDistance - 1) * 4;
                    }

                    if (npcMob.WaypointXPositions[targetSprite] - npcMob.LocationX > client.gridSize * 3 || npcMob.WaypointYPositions[targetSprite] - npcMob.LocationY > client.gridSize * 3 || npcMob.WaypointXPositions[targetSprite] - npcMob.LocationX < -client.gridSize * 3 || npcMob.WaypointYPositions[targetSprite] - npcMob.LocationY < -client.gridSize * 3 || waypointDistance > 8)
                    {
                        npcMob.LocationX = npcMob.WaypointXPositions[targetSprite];
                        npcMob.LocationY = npcMob.WaypointYPositions[targetSprite];
                    }
                    else
                    {
                        if (npcMob.LocationX < npcMob.WaypointXPositions[targetSprite])
                        {
                            npcMob.LocationX += movementSpeed;
                            npcMob.StepCount += 1;
                            direction = 2;
                        }
                        else if (npcMob.LocationX > npcMob.WaypointXPositions[targetSprite])
                        {
                            npcMob.LocationX -= movementSpeed;
                            npcMob.StepCount += 1;
                            direction = 6;
                        }

                        if (npcMob.LocationX - npcMob.WaypointXPositions[targetSprite] < movementSpeed && npcMob.LocationX - npcMob.WaypointXPositions[targetSprite] > -movementSpeed)
                        {
                            npcMob.LocationX = npcMob.WaypointXPositions[targetSprite];
                        }

                        if (npcMob.LocationY < npcMob.WaypointYPositions[targetSprite])
                        {
                            npcMob.LocationY += movementSpeed;
                            npcMob.StepCount += 1;

                            if (direction == -1)
                            {
                                direction = 4;
                            }
                            else if (direction == 2)
                            {
                                direction = 3;
                            }
                            else
                            {
                                direction = 5;
                            }
                        }
                        else if (npcMob.LocationY > npcMob.WaypointYPositions[targetSprite])
                        {
                            npcMob.LocationY -= movementSpeed;
                            npcMob.StepCount += 1;

                            if (direction == -1)
                            {
                                direction = 0;
                            }
                            else if (direction == 2)
                            {
                                direction = 1;
                            }
                            else
                            {
                                direction = 7;
                            }
                        }

                        if (npcMob.LocationY - npcMob.WaypointYPositions[targetSprite] < movementSpeed && npcMob.LocationY - npcMob.WaypointYPositions[targetSprite] > -movementSpeed)
                        {
                            npcMob.LocationY = npcMob.WaypointYPositions[targetSprite];
                        }
                    }

                    if (direction != -1)
                    {
                        npcMob.CurrentSprite = direction;
                    }

                    if (npcMob.LocationX == npcMob.WaypointXPositions[targetSprite] && npcMob.LocationY == npcMob.WaypointYPositions[targetSprite])
                    {
                        npcMob.WaypointsEndSprite = (targetSprite + 1) % 10;
                    }
                }
                else
                {
                    npcMob.CurrentSprite = npcMob.NextSprite;

                    if (npcMob.NpcIdentifier == 43)
                    {
                        npcMob.StepCount += 1;
                    }
                }

                if (npcMob.LastMessageTimeout > 0)
                {
                    npcMob.LastMessageTimeout -= 1;
                }

                if (npcMob.PlayerSkullTimeout > 0)
                {
                    npcMob.PlayerSkullTimeout -= 1;
                }

                if (npcMob.CombatTimer > 0)
                {
                    npcMob.CombatTimer -= 1;
                }
            }

            if (client.drawMenuTab != 2)
            {
                if (GameImage.SpiralDrawCount > 0)
                {
                    client.sleepWordDelayTimer += 1;
                }

                if (GameImage.CharacterDrawCount > 0)
                {
                    client.sleepWordDelayTimer = 0;
                }

                GameImage.SpiralDrawCount = 0;
                GameImage.CharacterDrawCount = 0;
            }
            for (int playerIndex = 0; playerIndex < client.playerCount; playerIndex += 1)
            {
                ClientMob player = client.playerArray[playerIndex];

                if (player.ProjectileDistance > 0)
                {
                    player.ProjectileDistance -= 1;
                }
            }

            if (client.cameraAutoAngleDebug)
            {
                if (client.cameraAutoRotatePlayerX - client.ourPlayer.LocationX < -500 || client.cameraAutoRotatePlayerX - client.ourPlayer.LocationX > 500 || client.cameraAutoRotatePlayerY - client.ourPlayer.LocationY < -500 || client.cameraAutoRotatePlayerY - client.ourPlayer.LocationY > 500)
                {
                    client.cameraAutoRotatePlayerX = client.ourPlayer.LocationX;
                    client.cameraAutoRotatePlayerY = client.ourPlayer.LocationY;
                }
            }
            else
            {
                if (client.cameraAutoRotatePlayerX - client.ourPlayer.LocationX < -500 || client.cameraAutoRotatePlayerX - client.ourPlayer.LocationX > 500 || client.cameraAutoRotatePlayerY - client.ourPlayer.LocationY < -500 || client.cameraAutoRotatePlayerY - client.ourPlayer.LocationY > 500)
                {
                    client.cameraAutoRotatePlayerX = client.ourPlayer.LocationX;
                    client.cameraAutoRotatePlayerY = client.ourPlayer.LocationY;
                }
                if (client.cameraAutoRotatePlayerX != client.ourPlayer.LocationX)
                {
                    client.cameraAutoRotatePlayerX += (client.ourPlayer.LocationX - client.cameraAutoRotatePlayerX) / (16 + (client.cameraDistance - 500) / 15);
                }

                if (client.cameraAutoRotatePlayerY != client.ourPlayer.LocationY)
                {
                    client.cameraAutoRotatePlayerY += (client.ourPlayer.LocationY - client.cameraAutoRotatePlayerY) / (16 + (client.cameraDistance - 500) / 15);
                }

                if (client.configCameraAutoAngle)
                {
                    int targetCameraRotation = client.cameraAutoAngle * 32;
                    int rotationDifference = targetCameraRotation - client.cameraRotation;
                    int rotationDirection = 1;

                    if (rotationDifference != 0)
                    {
                        client.cameraAutoRotationAmount += 1;

                        if (rotationDifference > 128)
                        {
                            rotationDirection = -1;
                            rotationDifference = 256 - rotationDifference;
                        }
                        else if (rotationDifference > 0)
                        {
                            rotationDirection = 1;
                        }
                        else if (rotationDifference < -128)
                        {
                            rotationDirection = 1;
                            rotationDifference = 256 + rotationDifference;
                        }
                        else if (rotationDifference < 0)
                        {
                            rotationDirection = -1;
                            rotationDifference = -rotationDifference;
                        }

                        client.cameraRotation += (client.cameraAutoRotationAmount * rotationDifference + 255) / 256 * rotationDirection;
                        client.cameraRotation &= 0xff;
                    }
                    else
                    {
                        client.cameraAutoRotationAmount = 0;
                    }
                }
            }
            if (client.sleepWordDelayTimer > 20)
            {
                client.sleepWordDelay = false;
                client.sleepWordDelayTimer = 0;
            }
            if (client.isSleeping)
            {
                if (client.enteredInputText.Length > 0)
                {
                    if (client.enteredInputText.ToLower() == "::lostcon")
                    {
                        client.streamClass.CloseStream();
                    }
                    else if (client.enteredInputText.ToLower() == "::closecon")
                    {
                        client.CallRequestLogout();
                    }
                        else
                        {
                            client.streamClass.CreatePacket(200);
                            client.streamClass.AddString(client.enteredInputText);
                            if (!client.sleepWordDelay)
                            {
                                client.streamClass.AddByte(0);
                                client.sleepWordDelay = true;
                            }
                            client.streamClass.FormatPacket();
                            client.inputText = "";
                            client.enteredInputText = "";
                            client.sleepingStatusText = "Please wait...";
                        }
                }

                if (client.lastMouseButton == 1 && client.mouseY > 275 && client.mouseY < 310 && client.mouseX > 56 && client.mouseX < 456)
                {
                    client.streamClass.CreatePacket(200);
                    client.streamClass.AddString("-null-");
                    if (!client.sleepWordDelay)
                    {
                        client.streamClass.AddByte(0);
                        client.sleepWordDelay = true;
                    }
                    client.streamClass.FormatPacket();
                    client.inputText = "";
                    client.enteredInputText = "";
                    client.sleepingStatusText = "Please wait...";
                }
                client.lastMouseButton = 0;
                return;
            }
            if (client.mouseY > client.windowHeight - 4)
            {
                if (client.mouseX > 15 && client.mouseX < 96 && client.lastMouseButton == 1)
                {
                    client.messagesTab = 0;
                }

                if (client.mouseX > 110 && client.mouseX < 194 && client.lastMouseButton == 1)
                {
                    client.messagesTab = 1;
                    client.chatInputMenu.listShownEntries[client.messagesHandleType2] = 0xf423f;
                }
                if (client.mouseX > 215 && client.mouseX < 295 && client.lastMouseButton == 1)
                {
                    client.messagesTab = 2;
                    client.chatInputMenu.listShownEntries[client.messagesHandleType5] = 0xf423f;
                }
                if (client.mouseX > 315 && client.mouseX < 395 && client.lastMouseButton == 1)
                {
                    client.messagesTab = 3;
                    client.chatInputMenu.listShownEntries[client.messagesHandleType6] = 0xf423f;
                }
                if (client.mouseX > 417 && client.mouseX < 497 && client.lastMouseButton == 1)
                {
                    client.showAbuseBox = 1;
                    client.reportAbuseOptionSelected = 0;
                    client.inputText = "";
                    client.enteredInputText = "";
                }
                client.lastMouseButton = 0;
                client.mouseButton = 0;
            }
            client.chatInputMenu.MouseClick(client.mouseX, client.mouseY, client.lastMouseButton, client.mouseButton);
            if (client.messagesTab > 0 && client.mouseX >= 494 && client.mouseY >= client.windowHeight - 66)
            {
                client.lastMouseButton = 0;
            }

            if (client.chatInputMenu.IsClicked(client.chatInputBox))
            {
                string input = client.chatInputMenu.GetText(client.chatInputBox);
                client.chatInputMenu.UpdateText(client.chatInputBox, "");
                if (input.StartsWith("::"))
                {
                    if (!client.HandleCommand(input[2..]))
                    {
                        client.CallSendCommand(input[2..]);
                    }
                }
                else
                {
                    int chatMessageLength = ChatMessage.StringToBytes(input);
                    client.CallSendChatMessage(ChatMessage.LastChat, chatMessageLength);
                    input = ChatMessage.BytesToString(ChatMessage.LastChat, 0, chatMessageLength);
                    client.ourPlayer.LastMessageTimeout = 150;
                    client.ourPlayer.LastMessage = input;
                    client.DisplayMessage(client.ourPlayer.Username + ": " + input, 2);
                }
            }
            if (client.messagesTab == 0)
            {
                for (int messageTabIndex = 0; messageTabIndex < 5; messageTabIndex += 1)
                {
                    if (client.messagesTimeout[messageTabIndex] > 0)
                    {
                        client.messagesTimeout[messageTabIndex] -= 1;
                    }
                }
            }
            if (client.playerAliveTimeout != 0)
            {
                client.lastMouseButton = 0;
            }

            if (client.showTradeBox || client.showDuelBox)
            {
                if (client.mouseButton != 0)
                {
                    client.mouseButtonHeldTime += 1;
                }
                else
                {
                    client.mouseButtonHeldTime = 0;
                }

                if (client.mouseButtonHeldTime > 500)
                {
                    client.mouseClickedHeldInTradeDuelBox += 100000;
                }
                else if (client.mouseButtonHeldTime > 350)
                {
                    client.mouseClickedHeldInTradeDuelBox += 10000;
                }
                else if (client.mouseButtonHeldTime > 250)
                {
                    client.mouseClickedHeldInTradeDuelBox += 1000;
                }
                else if (client.mouseButtonHeldTime > 150)
                {
                    client.mouseClickedHeldInTradeDuelBox += 100;
                }
                else if (client.mouseButtonHeldTime > 100)
                {
                    client.mouseClickedHeldInTradeDuelBox += 10;
                }
                else if (client.mouseButtonHeldTime > 50)
                {
                    client.mouseClickedHeldInTradeDuelBox += 1;
                }
                else if (client.mouseButtonHeldTime > 20 && (client.mouseButtonHeldTime & 5) == 0)
                {
                    client.mouseClickedHeldInTradeDuelBox += 1;
                }
            }
            else
            {
                client.mouseButtonHeldTime = 0;
                client.mouseClickedHeldInTradeDuelBox = 0;
            }
            if (client.lastMouseButton == 1)
            {
                client.mouseButtonClick = 1;
            }
            else if (client.lastMouseButton == 2)
            {
                client.mouseButtonClick = 2;
            }

            client.gameCamera.SetMousePosition(client.mouseX, client.mouseY);
            client.lastMouseButton = 0;
            if (client.configCameraAutoAngle)
            {
                if (client.cameraAutoRotationAmount == 0 || client.cameraAutoAngleDebug)
                {
                    if (client.keyLeftDown)
                    {
                        client.cameraAutoAngle = client.cameraAutoAngle + 1 & 7;
                        client.keyLeftDown = false;
                        if (!client.cameraZoom)
                        {
                            if ((client.cameraAutoAngle & 1) == 0)
                            {
                                client.cameraAutoAngle = client.cameraAutoAngle + 1 & 7;
                            }

                            for (int angleCheckIndex = 0; angleCheckIndex < 8; angleCheckIndex += 1)
                            {
                                if (client.IsValidCameraAngle(client.cameraAutoAngle))
                                {
                                    break;
                                }

                                client.cameraAutoAngle = client.cameraAutoAngle + 1 & 7;
                            }
                        }
                    }
                    if (client.keyRightDown)
                    {
                        client.cameraAutoAngle = client.cameraAutoAngle + 7 & 7;
                        client.keyRightDown = false;
                        if (!client.cameraZoom)
                        {
                            if ((client.cameraAutoAngle & 1) == 0)
                            {
                                client.cameraAutoAngle = client.cameraAutoAngle + 7 & 7;
                            }

                            for (int angleCheckIndex = 0; angleCheckIndex < 8; angleCheckIndex += 1)
                            {
                                if (client.IsValidCameraAngle(client.cameraAutoAngle))
                                {
                                    break;
                                }

                                client.cameraAutoAngle = client.cameraAutoAngle + 7 & 7;
                            }
                        }
                    }
                }
            }
            else if (client.keyLeftDown)
            {
                client.cameraRotation = client.cameraRotation + 2 & 0xff;
            }
            else if (client.keyRightDown)
            {
                client.cameraRotation = client.cameraRotation - 2 & 0xff;
            }

            if (client.keyUpDown && client.cameraDistance > 550)
            {
                client.cameraDistance -= 4;
            }
            else if (client.keyDownDown && client.cameraDistance < 1250)
            {
                client.cameraDistance += 4;
            }

            if (client.fogOfWar)
            {
                if ((client.cameraZoom && client.cameraDistance > 550) || client.cameraDistance > 750)
                {
                    client.cameraDistance -= 4;
                }

                if (!client.cameraZoom && client.cameraDistance < 750)
                {
                    client.cameraDistance += 4;
                }
            }
            if (client.actionPictureType > 0)
            {
                client.actionPictureType -= 1;
            }
            else if (client.actionPictureType < 0)
            {
                client.actionPictureType += 1;
            }

            client.gameCamera.UpdateLighting(17);
            client.modelUpdatingTimer += 1;
            if (client.modelUpdatingTimer > 5)
            {
                client.modelUpdatingTimer = 0;
                client.modelFireLightningSpellNumber = (client.modelFireLightningSpellNumber + 1) % 3;
                client.modelTorchNumber = (client.modelTorchNumber + 1) % 4;
                client.modelClawSpellNumber = (client.modelClawSpellNumber + 1) % 5;
            }
            for (int objectIndex = 0; objectIndex < client.objectCount; objectIndex += 1)
            {
                int objectXCoord = client.objectX[objectIndex];
                int objectYCoord = client.objectY[objectIndex];

                if (objectXCoord >= 0 && objectYCoord >= 0 && objectXCoord < 96 && objectYCoord < 96 && client.objectType[objectIndex] == 74)
                {
                    client.objectArray[objectIndex].OffsetMiniPosition(1, 0, 0);
                }
            }

            for (int bubbleIndex = 0; bubbleIndex < client.teleBubbleCount; bubbleIndex += 1)
            {
                client.teleBubbleTime[bubbleIndex] += 1;

                if (client.teleBubbleTime[bubbleIndex] > 50)
                {
                    client.teleBubbleCount -= 1;

                    for (int shiftIndex = bubbleIndex; shiftIndex < client.teleBubbleCount; shiftIndex += 1)
                    {
                        client.teleBubbleX[shiftIndex] = client.teleBubbleX[shiftIndex + 1];
                        client.teleBubbleY[shiftIndex] = client.teleBubbleY[shiftIndex + 1];
                        client.teleBubbleTime[shiftIndex] = client.teleBubbleTime[shiftIndex + 1];
                        client.teleBubbleType[shiftIndex] = client.teleBubbleType[shiftIndex + 1];
                    }
                }
            }
        }

        public void HandleKeyDown(Keys key, char character)
        {
            if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
            {
                return;
            }

            if (!client.loggedIn)
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
            if (client.loggedIn)
            {
                if (key == Keys.F12)
                {
                    client.TakeScreenshot(true);
                }
                else if (client.showAppearanceWindow && client.appearanceMenu is not null)
                {
                    client.appearanceMenu.KeyPress(key, character);
                }
                else if (client.showFriendsBox == 0 && client.showAbuseBox == 0 && !client.isSleeping && client.chatInputMenu is not null)
                {
                    client.chatInputMenu.KeyPress(key, character);
                }
            }
        }

        public void CheckMouseStatus()
        {
            if (client.selectedSpell >= 0 || client.selectedItem >= 0)
            {
                client.menuText1[client.menuOptionsCount] = "Cancel";
                client.menuText2[client.menuOptionsCount] = "";
                client.menuActionID[client.menuOptionsCount] = 4000;
                client.menuOptionsCount += 1;
            }
            for (int menuIndex = 0; menuIndex < client.menuOptionsCount; menuIndex += 1)
            {
                client.menuIndexes[menuIndex] = menuIndex;
            }

            for (bool isSorted = false; !isSorted;)
            {
                isSorted = true;

                for (int menuIndex = 0; menuIndex < client.menuOptionsCount - 1; menuIndex += 1)
                {
                    int firstIndex = client.menuIndexes[menuIndex];
                    int secondIndex = client.menuIndexes[menuIndex + 1];

                    if (client.menuActionID[firstIndex] > client.menuActionID[secondIndex])
                    {
                        client.menuIndexes[menuIndex] = secondIndex;
                        client.menuIndexes[menuIndex + 1] = firstIndex;
                        isSorted = false;
                    }
                }
            }

            if (client.menuOptionsCount > 20)
            {
                client.menuOptionsCount = 20;
            }

            if (client.menuOptionsCount > 0)
            {
                int firstTextEntryIndex = -1;

                for (int menuSearchIndex = 0; menuSearchIndex < client.menuOptionsCount; menuSearchIndex += 1)
                {
                    if (client.menuText2[client.menuIndexes[menuSearchIndex]] is null || client.menuText2[client.menuIndexes[menuSearchIndex]].Length <= 0)
                    {
                        continue;
                    }

                    firstTextEntryIndex = menuSearchIndex;
                    break;
                }

                string statusText = null;

                if ((client.selectedItem >= 0 || client.selectedSpell >= 0) && client.menuOptionsCount == 1)
                {
                    statusText = "Choose a target";
                }
                else if ((client.selectedItem >= 0 || client.selectedSpell >= 0) && client.menuOptionsCount > 1)
                {
                    statusText = "@whi@" + client.menuText1[client.menuIndexes[0]] + " " + client.menuText2[client.menuIndexes[0]];
                }
                else if (firstTextEntryIndex != -1)
                {
                    statusText = client.menuText2[client.menuIndexes[firstTextEntryIndex]] + ": @whi@" + client.menuText1[client.menuIndexes[0]];
                }

                if (client.menuOptionsCount == 2 && statusText is not null)
                {
                    statusText += "@whi@ / 1 more option";
                }

                if (client.menuOptionsCount > 2 && statusText is not null)
                {
                    statusText = statusText + "@whi@ / " + (client.menuOptionsCount - 1) + " more options";
                }

                if (statusText is not null)
                {
                    client.GameGraphics.DrawString(statusText, 6, 14, 1, 0xffff00);
                }

                if (!client.configOneMouseButton && client.mouseButtonClick == 1 || client.configOneMouseButton && client.mouseButtonClick == 1 && client.menuOptionsCount == 1)
                {
                    client.MenuClick(client.menuIndexes[0]);
                    client.mouseButtonClick = 0;
                    return;
                }
                if (!client.configOneMouseButton && client.mouseButtonClick == 2 || client.configOneMouseButton && client.mouseButtonClick == 1)
                {
                    client.menuHeight = (client.menuOptionsCount + 1) * 15;
                    client.menuWidth = client.GameGraphics.TextWidth("Choose option", 1) + 5;
                    for (int menuIndex = 0; menuIndex < client.menuOptionsCount; menuIndex += 1)
                    {
                        int entryTextWidth = client.GameGraphics.TextWidth(client.menuText1[menuIndex] + " " + client.menuText2[menuIndex], 1) + 5;

                        if (entryTextWidth > client.menuWidth)
                        {
                            client.menuWidth = entryTextWidth;
                        }
                    }

                    client.menuX = client.mouseX - client.menuWidth / 2;
                    client.menuY = client.mouseY - 7;
                    client.menuShow = true;
                    if (client.menuX < 0)
                    {
                        client.menuX = 0;
                    }

                    if (client.menuY < 0)
                    {
                        client.menuY = 0;
                    }

                    if (client.menuX + client.menuWidth > 510)
                    {
                        client.menuX = 510 - client.menuWidth;
                    }

                    if (client.menuY + client.menuHeight > 315)
                    {
                        client.menuY = 315 - client.menuHeight;
                    }

                    client.mouseButtonClick = 0;
                }
            }
        }

    }

}
