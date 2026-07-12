using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using OpenRS.Net.Client.Events;
using OpenRS.Settings;
using System.Threading;


namespace OpenRS.Net.Client
{
    public sealed partial class GameClient
    {
        private bool leftMouseDown = false;
        private bool rightMouseDown = false;
        private List<Keys> lastPressedKeys = [];
        private int lastMouseX = 0;
        private int lastMouseY = 0;
        private bool lastLeftDown = false;
        private bool lastRightDown = false;
        private bool shiftKeyIsDown = false;
        private bool ctrlKeyIsDown = false;
        private bool altKeyIsDown = false;
        private TimeSpan timeLapse = TimeSpan.Zero;

        public char TranslateOemKeys(Keys k)
        {
            //   if (k == Keys.1)
            //  { }
            if (k == Keys.OemPeriod)
            {
                return '.';
            }
            else if (shiftKeyIsDown)
            {
                if (k == Keys.NumPad1 || k == Keys.D1)
                {
                    return '!';
                }
                else if (k == Keys.NumPad2 || k == Keys.D2)
                {
                    return '"';
                }
                else if (k == Keys.NumPad3 || k == Keys.D3)
                {
                    return '#';
                }
                else if (k == Keys.NumPad4 || k == Keys.D4)
                {
                    return '¤';
                }
                else if (k == Keys.NumPad5 || k == Keys.D5)
                {
                    return '%';
                }
                else if (k == Keys.NumPad6 || k == Keys.D6)
                {
                    return '&';
                }
                else if (k == Keys.NumPad7 || k == Keys.D7)
                {
                    return '/';
                }
                else if (k == Keys.NumPad8 || k == Keys.D8)
                {
                    return '(';
                }
                else if (k == Keys.NumPad9 || k == Keys.D9)
                {
                    return ')';
                }
                else if (k == Keys.NumPad0 || k == Keys.D0)
                {
                    return '=';
                }
                else if (k == Keys.OemPlus)
                {
                    return '?';
                }

                return (char)k;
            }
            else if (altKeyIsDown && ctrlKeyIsDown) // alt Gr
            {
                if (k == Keys.NumPad2 || k == Keys.D2)
                {
                    return '@';
                }
                else if (k == Keys.NumPad3 || k == Keys.D3)
                {
                    return '£';
                }
                else if (k == Keys.NumPad4 || k == Keys.D4)
                {
                    return '$';
                }
                else if (k == Keys.NumPad7 || k == Keys.D7)
                {
                    return '{';
                }
                else if (k == Keys.NumPad8 || k == Keys.D8)
                {
                    return '[';
                }
                else if (k == Keys.NumPad9 || k == Keys.D9)
                {
                    return ']';
                }
                else if (k == Keys.NumPad0 || k == Keys.D0)
                {
                    return '}';
                }
                else if (k == Keys.OemPlus)
                {
                    return '\\';
                }
            }
            else
            {
                return ((char)k + "").ToLower()[0];
            }
            return (char)k;
        }
        public void Update(GameTime gt)
        {
            TimeSpan lastUpdate = gt.ElapsedGameTime;

            KeyboardState keyboardState = Keyboard.GetState();

            MouseState mouseState = Mouse.GetState();
            int rawX = mouseState.X;
            int rawY = mouseState.Y;
            Rectangle? bounds = GameWindow?.ClientBounds;
            if (GameWindow is not null)
            {
                rawX += GameWindow.ClientBounds.X;
                rawY += GameWindow.ClientBounds.Y;
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
                    KeyDown(pressedKey, TranslateOemKeys(pressedKey));
                    timeLapse = TimeSpan.Zero;
                }
                else if (timeLapse > TimeSpan.FromMilliseconds(150))
                {
                    KeyDown(pressedKey, TranslateOemKeys(pressedKey));
                    timeLapse = TimeSpan.Zero;
                }
                // HandleKeyDown(k, c[0]);
            }

            foreach (Keys lastKey in lastPressedKeys)
            {
                if (!keysPressedDown.Contains(lastKey))
                {
                    KeyUp(lastKey, TranslateOemKeys(lastKey));
                }
            }


            lastPressedKeys.Clear();
            lastPressedKeys.AddRange(keyboardState.GetPressedKeys());

            timeLapse += lastUpdate;

            //mouseEntered(mouseState);
            if (adjustedMouseState.X != lastMouseX || adjustedMouseState.Y != lastMouseY)
            {
                MouseMove(adjustedMouseState.X, adjustedMouseState.Y);
                lastMouseX = adjustedMouseState.X;
                lastMouseY = adjustedMouseState.Y;
                //mouseButtonClick = 0;
            }

            if (adjustedMouseState.RightButton == ButtonState.Pressed && !lastRightDown)
            {
                lastRightDown = true;
                MouseDown(adjustedMouseState.X, adjustedMouseState.Y, adjustedMouseState.LeftButton == ButtonState.Pressed);
                MousePressed(adjustedMouseState);
            }


            if (adjustedMouseState.LeftButton == ButtonState.Pressed && !lastLeftDown)
            {
                lastLeftDown = true;
                Console.WriteLine($"[MOUSEDOWN] screenX={mouseState.X} screenY={mouseState.Y} adjX={rawX} adjY={rawY} windowBounds={bounds}");
                MouseDown(adjustedMouseState.X, adjustedMouseState.Y, false);
            }

            if (adjustedMouseState.RightButton == ButtonState.Released && lastRightDown)
            {
                lastRightDown = false;
                // MousePressed(mouseState);
                MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }
            if (adjustedMouseState.LeftButton == ButtonState.Released && lastLeftDown)
            {
                lastLeftDown = false;

                MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }

            //uglyHack = false;
            //if ((!rightMouseDown && !leftMouseDown) && (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed))
            //{
            //    if (!uglyHack)
            //    {
            //        uglyHack = true;
            //        leftMouseDown = mouseState.LeftButton == ButtonState.Pressed;
            //        rightMouseDown = mouseState.RightButton == ButtonState.Pressed;
            //        //MouseDown(
            //        MouseDown(mouseState.X, mouseState.Y, mouseState.LeftButton != ButtonState.Pressed);
            //        //handleMouseDown(mouseState.X, mouseState.Y, 1);
            //    }
            //}

            //if ((leftMouseDown || rightMouseDown) && mouseState.LeftButton == ButtonState.Released && mouseState.RightButton == ButtonState.Released && !uglyHack)
            //{

            //    leftMouseDown = false;
            //    rightMouseDown = false;
            //    MouseUp(mouseState.X, mouseState.Y);
            //    MousePressed(mouseState);

            //}



        }
        private bool uglyHack = false;
        //public void Draw(GameTime gt)
        //{
        //    if (gameGraphics is not null)
        //    {
        //        try
        //        {
        //            //   gameGraphics.UpdateGameImage();

        //            //  drawWindow();

        //            gameGraphics.DrawImage(spriteBatch, 0, 0);

        //            //    //GameClient.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
        //            //    foreach (var str in GameImage.stringsToDraw)
        //            //    {

        //            //        //GameClient.gameFont12
        //            //        if (!GameClient.spriteBatch.BeginIsActive()) return;
        //            //        //var color = new Color(startColor >> 0x0000ff, startColor >> 0x00ff00, startColor >> 0xff0000, 255);

        //            //        Color clr = str.forecolor;
        //            //        SpriteFont font = GameClient.gameFont12;

        //            //        //if (clr.A == 0 || clr.A < 255)
        //            //        //    clr = new Color(255, 255, 255, 255);

        //            //        if (str.font is not null)
        //            //        {
        //            //            font = str.font;
        //            //        }
        //            //        var textToRender = str.text;
        //            //        //textToRender = textToRender.Replace("@gre@", "");
        //            //        //textToRender = textToRender.Replace("@yel@", "");
        //            //        //textToRender = textToRender.Replace("@whi@", "");
        //            //        //textToRender = textToRender.Replace("@normalZ@", "");
        //            //        //textToRender = textToRender.Replace("@ran@", "");
        //            //        //textToRender = textToRender.Replace("@red@", "");

        //            //        GameClient.spriteBatch.DrawString(font, textToRender, str.pos - new Vector2(0f, (float)gameFrame.yOffset / 2.5f), clr);


        //            //    }
        //        }
        //        catch { }

        //        ////GameClient.spriteBatch.End();

        //        //GameImage.stringsToDraw.Clear();
        //    }
        //}


        public override void CheckInputs()
        {
            if (memoryError)
            {
                return;
            }

            if (errorLoading)
            {
                return;
            }

            try
            {
                tick += 1;
                if (!loggedIn)
                {
                    CheckLoginScreenInputs();
                }
                if (loggedIn)
                {
                    CheckGameInputs();
                }
                lastMouseButton = 0;
                cameraRotateTime += 1;
                if (cameraRotateTime > 500)
                {
                    cameraRotateTime = 0;
                    int l = (int)(Helper.Random.NextDouble() * 4D);
                    if ((l & 1) == 1)
                    {
                        cameraRotationXAmount += cameraRotationXIncrement;
                    }

                    if ((l & 2) == 2)
                    {
                        cameraRotationYAmount += cameraRotationYIncrement;
                    }
                }
                if (cameraRotationXAmount < -50)
                {
                    cameraRotationXIncrement = 2;
                }

                if (cameraRotationXAmount > 50)
                {
                    cameraRotationXIncrement = -2;
                }

                if (cameraRotationYAmount < -50)
                {
                    cameraRotationYIncrement = 2;
                }

                if (cameraRotationYAmount > 50)
                {
                    cameraRotationYIncrement = -2;
                }

                if (chatTabAllMsgFlash > 0)
                {
                    chatTabAllMsgFlash -= 1;
                }

                if (chatTabHistoryFlash > 0)
                {
                    chatTabHistoryFlash -= 1;
                }

                if (chatTabQuestFlash > 0)
                {
                    chatTabQuestFlash -= 1;
                }

                if (chatTabPrivateFlash > 0)
                {
                    chatTabPrivateFlash -= 1;
                }
            }
            catch (Exception _ex)
            {
                CleanUp();
                memoryError = true;
            }
        }
        public void CheckLoginScreenInputs()
        {
            if (socketTimeout > 0)
            {
                socketTimeout -= 1;
            }

            if (loginScreen == 0)
            {
                if (loginMenuFirst is null)
                {
                    return;
                }

                if (lastMouseButton != 0 || mouseButton != 0)
                {
                    Console.WriteLine($"[CLICK] mouseX={mouseX} mouseY={mouseY} lastMouseButton={lastMouseButton} mouseButton={mouseButton}");
                }

                loginMenuFirst.MouseClick(mouseX, mouseY, lastMouseButton, mouseButton);
                if (loginMenuFirst.IsClicked(loginButtonNewUser))
                {
                    loginScreen = 1;
                }

                if (loginMenuFirst.IsClicked(loginMenuLoginButton))
                {
                    loginScreen = 2;
                    loginMenuLogin.UpdateText(loginMenuStatusText, "Please enter your username and password");
                    loginMenuLogin.UpdateText(loginMenuUserText, "");
                    loginMenuLogin.UpdateText(loginMenuPasswordText, "");
                    loginMenuLogin.SetFocus(loginMenuUserText);
                    return;
                }
            }
            else
                if (loginScreen == 1)
                {
                    if (loginNewUser is null)
                {
                    return;
                }

                loginNewUser.MouseClick(mouseX, mouseY, lastMouseButton, mouseButton);
                    if (loginNewUser.IsClicked(loginMenuOkButton))
                    {
                        loginScreen = 0;
                        return;
                    }
                }
                else
                    if (loginScreen == 2)
                    {
                        loginMenuLogin.MouseClick(mouseX, mouseY, lastMouseButton, mouseButton);
                        if (loginMenuLogin.IsClicked(loginMenuCancelButton))
                {
                    loginScreen = 0;
                }

                if (loginMenuLogin.IsClicked(loginMenuUserText))
                {
                    loginMenuLogin.SetFocus(loginMenuPasswordText);
                }

                if (loginMenuLogin.IsClicked(loginMenuPasswordText) || loginMenuLogin.IsClicked(loginMenuOkLoginButton))
                        {
                            loginUsername = loginMenuLogin.GetText(loginMenuUserText);
                            loginPassword = loginMenuLogin.GetText(loginMenuPasswordText);
                            Connect(loginUsername, loginPassword, false);
                        }
                    }
        }
        public override void HandleMouseDown(int mouseButtonPressed, int mouseXPosition, int mouseYPosition)
        {
            mouseTrailX[mouseTrailIndex] = mouseXPosition;
            mouseTrailY[mouseTrailIndex] = mouseYPosition;
            mouseTrailIndex = mouseTrailIndex + 1 & 0x1fff;
            for (int l = 10; l < 4000; l += 1)
            {
                int lastMouseTrailIndex = mouseTrailIndex - l & 0x1fff;
                if (mouseTrailX[lastMouseTrailIndex] == mouseXPosition && mouseTrailY[lastMouseTrailIndex] == mouseYPosition)
                {
                    bool flag = false;
                    for (int j1 = 1; j1 < l; j1 += 1)
                    {
                        int mouseNew = mouseTrailIndex - j1 & 0x1fff;
                        int mouseOld = lastMouseTrailIndex - j1 & 0x1fff;
                        if (mouseTrailX[mouseOld] != mouseXPosition || mouseTrailY[mouseOld] != mouseYPosition)
                        {
                            flag = true;
                        }

                        if (mouseTrailX[mouseNew] != mouseTrailX[mouseOld] || mouseTrailY[mouseNew] != mouseTrailY[mouseOld])
                        {
                            break;
                        }

                        if (j1 == l - 1 && flag && combatTimeout == 0 && logoutTimer == 0)
                        {
                            SendLogout();
                            return;
                        }
                    }

                }
            }

        }
        public void CheckGameInputs()
        {
            if (systemUpdate > 1)
            {
                systemUpdate -= 1;
            }

            SendPingPacketAsync();




            if (logoutTimer > 0)
            {
                logoutTimer -= 1;
            }

            if (ourPlayer.currentSprite == 8 || ourPlayer.currentSprite == 9)
            {
                combatTimeout = 500;
            }

            if (combatTimeout > 0)
            {
                combatTimeout -= 1;
            }

            if (showAppearanceWindow)
            {
                UpdateAppearanceWindow();
                return;
            }
            for (int l = 0; l < playerCount; l += 1)
            {
                ClientMob player = playerArray[l];
                int j1 = (player.waypointCurrent + 1) % 10;
                if (player.waypointsEndSprite != j1)
                {
                    int direction = -1;
                    int targetSprite = player.waypointsEndSprite;
                    int i5;
                    if (targetSprite < j1)
                    {
                        i5 = j1 - targetSprite;
                    }
                    else
                    {
                        i5 = (10 + j1) - targetSprite;
                    }

                    int i6 = 4;
                    if (i5 > 2)
                    {
                        i6 = (i5 - 1) * 4;
                    }

                    if (player.waypointsX[targetSprite] - player.currentX > gridSize * 3 || player.waypointsY[targetSprite] - player.currentY > gridSize * 3 || player.waypointsX[targetSprite] - player.currentX < -gridSize * 3 || player.waypointsY[targetSprite] - player.currentY < -gridSize * 3 || i5 > 8)
                    {
                        player.currentX = player.waypointsX[targetSprite];
                        player.currentY = player.waypointsY[targetSprite];
                    }
                    else
                    {
                        if (player.currentX < player.waypointsX[targetSprite])
                        {
                            player.currentX += i6;
                            player.stepCount += 1;
                            direction = 2;
                        }
                        else
                            if (player.currentX > player.waypointsX[targetSprite])
                            {
                                player.currentX -= i6;
                                player.stepCount += 1;
                                direction = 6;
                            }
                        if (player.currentX - player.waypointsX[targetSprite] < i6 && player.currentX - player.waypointsX[targetSprite] > -i6)
                        {
                            player.currentX = player.waypointsX[targetSprite];
                        }

                        if (player.currentY < player.waypointsY[targetSprite])
                        {
                            player.currentY += i6;
                            player.stepCount += 1;
                            if (direction == -1)
                            {
                                direction = 4;
                            }
                            else
                                if (direction == 2)
                            {
                                direction = 3;
                            }
                            else
                            {
                                direction = 5;
                            }
                        }
                        else
                            if (player.currentY > player.waypointsY[targetSprite])
                            {
                                player.currentY -= i6;
                                player.stepCount += 1;
                                if (direction == -1)
                            {
                                direction = 0;
                            }
                            else
                                    if (direction == 2)
                            {
                                direction = 1;
                            }
                            else
                            {
                                direction = 7;
                            }
                        }
                        if (player.currentY - player.waypointsY[targetSprite] < i6 && player.currentY - player.waypointsY[targetSprite] > -i6)
                        {
                            player.currentY = player.waypointsY[targetSprite];
                        }
                    }
                    if (direction != -1)
                    {
                        player.currentSprite = direction;
                    }

                    if (player.currentX == player.waypointsX[targetSprite] && player.currentY == player.waypointsY[targetSprite])
                    {
                        player.waypointsEndSprite = (targetSprite + 1) % 10;
                    }
                }
                else
                {
                    player.currentSprite = player.nextSprite;
                }
                if (player.lastMessageTimeout > 0)
                {
                    player.lastMessageTimeout -= 1;
                }

                if (player.playerSkullTimeout > 0)
                {
                    player.playerSkullTimeout -= 1;
                }

                if (player.combatTimer > 0)
                {
                    player.combatTimer -= 1;
                }

                if (playerAliveTimeout > 0)
                {
                    playerAliveTimeout -= 1;
                    if (playerAliveTimeout == 0)
                    {
                        DisplayMessage("You have been granted another life. Be more careful this time!", 3);
                    }

                    if (playerAliveTimeout == 0)
                    {
                        DisplayMessage("You retain your skills. Your objects land where you died", 3);
                    }
                }
            }

            for (int i1 = 0; i1 < npcCount; i1 += 1)
            {
                ClientMob f2 = npcArray[i1];
                int i2 = (f2.waypointCurrent + 1) % 10;
                if (f2.waypointsEndSprite != i2)
                {
                    int l3 = -1;
                    int j5 = f2.waypointsEndSprite;
                    int j6;
                    if (j5 < i2)
                    {
                        j6 = i2 - j5;
                    }
                    else
                    {
                        j6 = (10 + i2) - j5;
                    }

                    int k6 = 4;
                    if (j6 > 2)
                    {
                        k6 = (j6 - 1) * 4;
                    }

                    if (f2.waypointsX[j5] - f2.currentX > gridSize * 3 || f2.waypointsY[j5] - f2.currentY > gridSize * 3 || f2.waypointsX[j5] - f2.currentX < -gridSize * 3 || f2.waypointsY[j5] - f2.currentY < -gridSize * 3 || j6 > 8)
                    {
                        f2.currentX = f2.waypointsX[j5];
                        f2.currentY = f2.waypointsY[j5];
                    }
                    else
                    {
                        if (f2.currentX < f2.waypointsX[j5])
                        {
                            f2.currentX += k6;
                            f2.stepCount += 1;
                            l3 = 2;
                        }
                        else
                            if (f2.currentX > f2.waypointsX[j5])
                            {
                                f2.currentX -= k6;
                                f2.stepCount += 1;
                                l3 = 6;
                            }
                        if (f2.currentX - f2.waypointsX[j5] < k6 && f2.currentX - f2.waypointsX[j5] > -k6)
                        {
                            f2.currentX = f2.waypointsX[j5];
                        }

                        if (f2.currentY < f2.waypointsY[j5])
                        {
                            f2.currentY += k6;
                            f2.stepCount += 1;
                            if (l3 == -1)
                            {
                                l3 = 4;
                            }
                            else
                                if (l3 == 2)
                            {
                                l3 = 3;
                            }
                            else
                            {
                                l3 = 5;
                            }
                        }
                        else
                            if (f2.currentY > f2.waypointsY[j5])
                            {
                                f2.currentY -= k6;
                                f2.stepCount += 1;
                                if (l3 == -1)
                            {
                                l3 = 0;
                            }
                            else
                                    if (l3 == 2)
                            {
                                l3 = 1;
                            }
                            else
                            {
                                l3 = 7;
                            }
                        }
                        if (f2.currentY - f2.waypointsY[j5] < k6 && f2.currentY - f2.waypointsY[j5] > -k6)
                        {
                            f2.currentY = f2.waypointsY[j5];
                        }
                    }
                    if (l3 != -1)
                    {
                        f2.currentSprite = l3;
                    }

                    if (f2.currentX == f2.waypointsX[j5] && f2.currentY == f2.waypointsY[j5])
                    {
                        f2.waypointsEndSprite = (j5 + 1) % 10;
                    }
                }
                else
                {
                    f2.currentSprite = f2.nextSprite;
                    if (f2.npcId == 43)
                    {
                        f2.stepCount += 1;
                    }
                }
                if (f2.lastMessageTimeout > 0)
                {
                    f2.lastMessageTimeout -= 1;
                }

                if (f2.playerSkullTimeout > 0)
                {
                    f2.playerSkullTimeout -= 1;
                }

                if (f2.combatTimer > 0)
                {
                    f2.combatTimer -= 1;
                }
            }

            if (drawMenuTab != 2)
            {
                if (GameImage.spiralDrawCount > 0)
                {
                    sleepWordDelayTimer += 1;
                }

                if (GameImage.characterDrawCount > 0)
                {
                    sleepWordDelayTimer = 0;
                }

                GameImage.spiralDrawCount = 0;
                GameImage.characterDrawCount = 0;
            }
            for (int k1 = 0; k1 < playerCount; k1 += 1)
            {
                ClientMob f3 = playerArray[k1];
                if (f3.projectileDistance > 0)
                {
                    f3.projectileDistance -= 1;
                }
            }

            if (cameraAutoAngleDebug)
            {
                if (cameraAutoRotatePlayerX - ourPlayer.currentX < -500 || cameraAutoRotatePlayerX - ourPlayer.currentX > 500 || cameraAutoRotatePlayerY - ourPlayer.currentY < -500 || cameraAutoRotatePlayerY - ourPlayer.currentY > 500)
                {
                    cameraAutoRotatePlayerX = ourPlayer.currentX;
                    cameraAutoRotatePlayerY = ourPlayer.currentY;
                }
            }
            else
            {
                if (cameraAutoRotatePlayerX - ourPlayer.currentX < -500 || cameraAutoRotatePlayerX - ourPlayer.currentX > 500 || cameraAutoRotatePlayerY - ourPlayer.currentY < -500 || cameraAutoRotatePlayerY - ourPlayer.currentY > 500)
                {
                    cameraAutoRotatePlayerX = ourPlayer.currentX;
                    cameraAutoRotatePlayerY = ourPlayer.currentY;
                }
                if (cameraAutoRotatePlayerX != ourPlayer.currentX)
                {
                    cameraAutoRotatePlayerX += (ourPlayer.currentX - cameraAutoRotatePlayerX) / (16 + (cameraDistance - 500) / 15);
                }

                if (cameraAutoRotatePlayerY != ourPlayer.currentY)
                {
                    cameraAutoRotatePlayerY += (ourPlayer.currentY - cameraAutoRotatePlayerY) / (16 + (cameraDistance - 500) / 15);
                }

                if (configCameraAutoAngle)
                {
                    int j2 = cameraAutoAngle * 32;
                    int i4 = j2 - cameraRotation;
                    int byte0 = 1;
                    if (i4 != 0)
                    {
                        cameraAutoRotationAmount += 1;
                        if (i4 > 128)
                        {
                            byte0 = -1;
                            i4 = 256 - i4;
                        }
                        else
                            if (i4 > 0)
                        {
                            byte0 = 1;
                        }
                        else
                                if (i4 < -128)
                                {
                                    byte0 = 1;
                                    i4 = 256 + i4;
                                }
                                else
                                    if (i4 < 0)
                                    {
                                        byte0 = -1;
                                        i4 = -i4;
                                    }
                        cameraRotation += ((cameraAutoRotationAmount * i4 + 255) / 256) * byte0;
                        cameraRotation &= 0xff;
                    }
                    else
                    {
                        cameraAutoRotationAmount = 0;
                    }
                }
            }
            if (sleepWordDelayTimer > 20)
            {
                sleepWordDelay = false;
                sleepWordDelayTimer = 0;
            }
            if (isSleeping)
            {
                if (enteredInputText.Length > 0)
                {
                    if (enteredInputText.ToLower() == "::lostcon")
                    {
                        streamClass.CloseStream();
                    }
                    else
                        if (enteredInputText.ToLower() == "::closecon")
                        {
                            RequestLogout();
                        }
                        else
                        {
                            streamClass.CreatePacket(200);
                            streamClass.AddString(enteredInputText);
                            if (!sleepWordDelay)
                            {
                                streamClass.AddByte(0);
                                sleepWordDelay = true;
                            }
                            streamClass.FormatPacket();
                            inputText = "";
                            enteredInputText = "";
                            sleepingStatusText = "Please wait...";
                        }
                }

                if (lastMouseButton == 1 && mouseY > 275 && mouseY < 310 && mouseX > 56 && mouseX < 456)
                {
                    streamClass.CreatePacket(200);
                    streamClass.AddString("-null-");
                    if (!sleepWordDelay)
                    {
                        streamClass.AddByte(0);
                        sleepWordDelay = true;
                    }
                    streamClass.FormatPacket();
                    inputText = "";
                    enteredInputText = "";
                    sleepingStatusText = "Please wait...";
                }
                lastMouseButton = 0;
                return;
            }
            if (mouseY > windowHeight - 4)
            {
                if (mouseX > 15 && mouseX < 96 && lastMouseButton == 1)
                {
                    messagesTab = 0;
                }

                if (mouseX > 110 && mouseX < 194 && lastMouseButton == 1)
                {
                    messagesTab = 1;
                    chatInputMenu.listShownEntries[messagesHandleType2] = 0xf423f;
                }
                if (mouseX > 215 && mouseX < 295 && lastMouseButton == 1)
                {
                    messagesTab = 2;
                    chatInputMenu.listShownEntries[messagesHandleType5] = 0xf423f;
                }
                if (mouseX > 315 && mouseX < 395 && lastMouseButton == 1)
                {
                    messagesTab = 3;
                    chatInputMenu.listShownEntries[messagesHandleType6] = 0xf423f;
                }
                if (mouseX > 417 && mouseX < 497 && lastMouseButton == 1)
                {
                    showAbuseBox = 1;
                    reportAbuseOptionSelected = 0;
                    inputText = "";
                    enteredInputText = "";
                }
                lastMouseButton = 0;
                mouseButton = 0;
            }
            chatInputMenu.MouseClick(mouseX, mouseY, lastMouseButton, mouseButton);
            if (messagesTab > 0 && mouseX >= 494 && mouseY >= windowHeight - 66)
            {
                lastMouseButton = 0;
            }

            if (chatInputMenu.IsClicked(chatInputBox))
            {
                string input = chatInputMenu.GetText(chatInputBox);
                chatInputMenu.UpdateText(chatInputBox, "");
                if (input.StartsWith("::"))
                {
                    if (!HandleCommand(input.Substring(2)))
                    {
                        SendCommand(input.Substring(2));
                    }
                }
                else
                {
                    int len = ChatMessage.StringToBytes(input);
                    SendChatMessage(ChatMessage.lastChat, len);
                    input = ChatMessage.BytesToString(ChatMessage.lastChat, 0, len);
                    //if (useChatFilter)
                    //input = ChatFilter.filterChat(input);
                    ourPlayer.lastMessageTimeout = 150;
                    ourPlayer.lastMessage = input;
                    DisplayMessage(ourPlayer.username + ": " + input, 2);
                }
            }
            if (messagesTab == 0)
            {
                for (int k2 = 0; k2 < 5; k2 += 1)
                {
                    if (messagesTimeout[k2] > 0)
                    {
                        messagesTimeout[k2] -= 1;
                    }
                }
            }
            if (playerAliveTimeout != 0)
            {
                lastMouseButton = 0;
            }

            if (showTradeBox || showDuelBox)
            {
                if (mouseButton != 0)
                {
                    mouseButtonHeldTime += 1;
                }
                else
                {
                    mouseButtonHeldTime = 0;
                }

                if (mouseButtonHeldTime > 500)
                {
                    mouseClickedHeldInTradeDuelBox += 100000;
                }
                else if (mouseButtonHeldTime > 350)
                {
                    mouseClickedHeldInTradeDuelBox += 10000;
                }
                else if (mouseButtonHeldTime > 250)
                {
                    mouseClickedHeldInTradeDuelBox += 1000;
                }
                else if (mouseButtonHeldTime > 150)
                {
                    mouseClickedHeldInTradeDuelBox += 100;
                }
                else if (mouseButtonHeldTime > 100)
                {
                    mouseClickedHeldInTradeDuelBox += 10;
                }
                else if (mouseButtonHeldTime > 50)
                {
                    mouseClickedHeldInTradeDuelBox += 1;
                }
                else if (mouseButtonHeldTime > 20 && (mouseButtonHeldTime & 5) == 0)
                {
                    mouseClickedHeldInTradeDuelBox += 1;
                }
            }
            else
            {
                mouseButtonHeldTime = 0;
                mouseClickedHeldInTradeDuelBox = 0;
            }
            if (lastMouseButton == 1)
            {
                mouseButtonClick = 1;
            }
            else if (lastMouseButton == 2)
            {
                mouseButtonClick = 2;
            }

            gameCamera.SetMousePosition(mouseX, mouseY);
            lastMouseButton = 0;
            if (configCameraAutoAngle)
            {
                if (cameraAutoRotationAmount == 0 || cameraAutoAngleDebug)
                {
                    if (keyLeftDown)
                    {
                        cameraAutoAngle = cameraAutoAngle + 1 & 7;
                        keyLeftDown = false;
                        if (!cameraZoom)
                        {
                            if ((cameraAutoAngle & 1) == 0)
                            {
                                cameraAutoAngle = cameraAutoAngle + 1 & 7;
                            }

                            for (int l2 = 0; l2 < 8; l2 += 1)
                            {
                                if (IsValidCameraAngle(cameraAutoAngle))
                                {
                                    break;
                                }

                                cameraAutoAngle = cameraAutoAngle + 1 & 7;
                            }
                        }
                    }
                    if (keyRightDown)
                    {
                        cameraAutoAngle = cameraAutoAngle + 7 & 7;
                        keyRightDown = false;
                        if (!cameraZoom)
                        {
                            if ((cameraAutoAngle & 1) == 0)
                            {
                                cameraAutoAngle = cameraAutoAngle + 7 & 7;
                            }

                            for (int i3 = 0; i3 < 8; i3 += 1)
                            {
                                if (IsValidCameraAngle(cameraAutoAngle))
                                {
                                    break;
                                }

                                cameraAutoAngle = cameraAutoAngle + 7 & 7;
                            }
                        }
                    }
                }
            }
            else if (keyLeftDown)
            {
                cameraRotation = cameraRotation + 2 & 0xff;
            }
            else if (keyRightDown)
            {
                cameraRotation = cameraRotation - 2 & 0xff;
            }

            if (keyUpDown && cameraDistance > 550)
            {
                cameraDistance -= 4;
            }
            else if (keyDownDown && cameraDistance < 1250)
            {
                cameraDistance += 4;
            }

            if (fogOfWar)
            {
                if ((cameraZoom && cameraDistance > 550) || cameraDistance > 750)
                {
                    cameraDistance -= 4;
                }

                if (!cameraZoom && cameraDistance < 750)
                {
                    cameraDistance += 4;
                }
            }
            if (actionPictureType > 0)
            {
                actionPictureType -= 1;
            }
            else
                if (actionPictureType < 0)
            {
                actionPictureType += 1;
            }

            gameCamera.UpdateLighting(17);
            modelUpdatingTimer += 1;
            if (modelUpdatingTimer > 5)
            {
                modelUpdatingTimer = 0;
                modelFireLightningSpellNumber = (modelFireLightningSpellNumber + 1) % 3;
                modelTorchNumber = (modelTorchNumber + 1) % 4;
                modelClawSpellNumber = (modelClawSpellNumber + 1) % 5;
            }
            for (int j3 = 0; j3 < objectCount; j3 += 1)
            {
                int k4 = objectX[j3];
                int k5 = objectY[j3];
                if (k4 >= 0 && k5 >= 0 && k4 < 96 && k5 < 96 && objectType[j3] == 74)
                {
                    objectArray[j3].OffsetMiniPosition(1, 0, 0);
                }
            }

            for (int l4 = 0; l4 < teleBubbleCount; l4 += 1)
            {
                teleBubbleTime[l4] += 1;
                if (teleBubbleTime[l4] > 50)
                {
                    teleBubbleCount -= 1;
                    for (int l5 = l4; l5 < teleBubbleCount; l5 += 1)
                    {
                        teleBubbleX[l5] = teleBubbleX[l5 + 1];
                        teleBubbleY[l5] = teleBubbleY[l5 + 1];
                        teleBubbleTime[l5] = teleBubbleTime[l5 + 1];
                        teleBubbleType[l5] = teleBubbleType[l5 + 1];
                    }

                }
            }

        }
        public override void HandleKeyDown(Keys key, char c)
        {
            if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
            {
                return;
            }

            if (!loggedIn)
            {
                if (loginScreen == 0 && loginMenuFirst is not null)
                {
                    loginMenuFirst.KeyPress(key, c);
                }

                if (loginScreen == 1 && loginNewUser is not null)
                {
                    loginNewUser.KeyPress(key, c);
                }

                if (loginScreen == 2 && loginMenuLogin is not null)
                {
                    loginMenuLogin.KeyPress(key, c);
                }
            }
            if (loggedIn)
            {
                if (key == Keys.F12)
                {
                    TakeScreenshot(true);
                }
                else if (showAppearanceWindow && appearanceMenu is not null)
                {
                    appearanceMenu.KeyPress(key, c);
                }
                else if (showFriendsBox == 0 && showAbuseBox == 0 && !isSleeping && chatInputMenu is not null)
                {
                    chatInputMenu.KeyPress(key, c);
                }
            }
        }
        public void CheckMouseStatus()
        {
            if (selectedSpell >= 0 || selectedItem >= 0)
            {
                menuText1[menuOptionsCount] = "Cancel";
                menuText2[menuOptionsCount] = "";
                menuActionID[menuOptionsCount] = 4000;
                menuOptionsCount += 1;
            }
            for (int l = 0; l < menuOptionsCount; l += 1)
            {
                menuIndexes[l] = l;
            }

            for (bool flag = false; !flag; )
            {
                flag = true;
                for (int i1 = 0; i1 < menuOptionsCount - 1; i1 += 1)
                {
                    int k1 = menuIndexes[i1];
                    int i2 = menuIndexes[i1 + 1];
                    if (menuActionID[k1] > menuActionID[i2])
                    {
                        menuIndexes[i1] = i2;
                        menuIndexes[i1 + 1] = k1;
                        flag = false;
                    }
                }

            }

            if (menuOptionsCount > 20)
            {
                menuOptionsCount = 20;
            }

            if (menuOptionsCount > 0)
            {
                int j1 = -1;
                for (int l1 = 0; l1 < menuOptionsCount; l1 += 1)
                {
                    if (menuText2[menuIndexes[l1]] is null || menuText2[menuIndexes[l1]].Length <= 0)
                    {
                        continue;
                    }

                    j1 = l1;
                    break;
                }

                string s1 = null;
                if ((selectedItem >= 0 || selectedSpell >= 0) && menuOptionsCount == 1)
                {
                    s1 = "Choose a target";
                }
                else
                    if ((selectedItem >= 0 || selectedSpell >= 0) && menuOptionsCount > 1)
                {
                    s1 = "@whi@" + menuText1[menuIndexes[0]] + " " + menuText2[menuIndexes[0]];
                }
                else
                        if (j1 != -1)
                {
                    s1 = menuText2[menuIndexes[j1]] + ": @whi@" + menuText1[menuIndexes[0]];
                }

                if (menuOptionsCount == 2 && s1 is not null)
                {
                    s1 += "@whi@ / 1 more option";
                }

                if (menuOptionsCount > 2 && s1 is not null)
                {
                    s1 = s1 + "@whi@ / " + (menuOptionsCount - 1) + " more options";
                }

                if (s1 is not null)
                {
                    gameGraphics.DrawString(s1, 6, 14, 1, 0xffff00);
                }

                if (!configOneMouseButton && mouseButtonClick == 1 || configOneMouseButton && mouseButtonClick == 1 && menuOptionsCount == 1)
                {
                    MenuClick(menuIndexes[0]);
                    mouseButtonClick = 0;
                    return;
                }
                if (!configOneMouseButton && mouseButtonClick == 2 || configOneMouseButton && mouseButtonClick == 1)
                {
                    menuHeight = (menuOptionsCount + 1) * 15;
                    menuWidth = gameGraphics.TextWidth("Choose option", 1) + 5;
                    for (int j2 = 0; j2 < menuOptionsCount; j2 += 1)
                    {
                        int k2 = gameGraphics.TextWidth(menuText1[j2] + " " + menuText2[j2], 1) + 5;
                        if (k2 > menuWidth)
                        {
                            menuWidth = k2;
                        }
                    }

                    menuX = mouseX - menuWidth / 2;
                    menuY = mouseY - 7;
                    menuShow = true;
                    if (menuX < 0)
                    {
                        menuX = 0;
                    }

                    if (menuY < 0)
                    {
                        menuY = 0;
                    }

                    if (menuX + menuWidth > 510)
                    {
                        menuX = 510 - menuWidth;
                    }

                    if (menuY + menuHeight > 315)
                    {
                        menuY = 315 - menuHeight;
                    }

                    mouseButtonClick = 0;
                }
            }
        }
    }

}
