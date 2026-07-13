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
using System;

namespace OpenRS.Net.Client
{
    public sealed class InputHandler(GameClient client)
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
            Rectangle? bounds = GameClient.GameWindow?.ClientBounds;
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
                    client.KeyDown(pressedKey, client.TranslateOemKeys(pressedKey));
                    timeLapse = TimeSpan.Zero;
                }
                else if (timeLapse > TimeSpan.FromMilliseconds(150))
                {
                    client.KeyDown(pressedKey, client.TranslateOemKeys(pressedKey));
                    timeLapse = TimeSpan.Zero;
                }
                // HandleKeyDown(k, c[0]);
            }

            foreach (Keys lastKey in lastPressedKeys)
            {
                if (!keysPressedDown.Contains(lastKey))
                {
                    client.KeyUp(lastKey, client.TranslateOemKeys(lastKey));
                }
            }


            lastPressedKeys.Clear();
            lastPressedKeys.AddRange(keyboardState.GetPressedKeys());

            timeLapse += lastUpdate;

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
                Console.WriteLine($"[MOUSEDOWN] screenX={mouseState.X} screenY={mouseState.Y} adjX={rawX} adjY={rawY} windowBounds={bounds}");
                client.MouseDown(adjustedMouseState.X, adjustedMouseState.Y, false);
            }

            if (adjustedMouseState.RightButton == ButtonState.Released && lastRightDown)
            {
                lastRightDown = false;
                // MousePressed(mouseState);
                client.MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
            }
            if (adjustedMouseState.LeftButton == ButtonState.Released && lastLeftDown)
            {
                lastLeftDown = false;

                client.MouseUp(adjustedMouseState.X, adjustedMouseState.Y);
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
                    int l = (int)(Helper.Random.NextDouble() * 4D);
                    if ((l & 1) == 1)
                    {
                        client.cameraRotationXAmount += client.cameraRotationXIncrement;
                    }

                    if ((l & 2) == 2)
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
            catch (Exception _ex)
            {
                Console.WriteLine($"[CheckInputs EXCEPTION] {_ex.GetType().Name}: {_ex.Message}");
                Console.WriteLine(_ex.StackTrace);
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
                    Console.WriteLine($"[CLICK] client.mouseX={client.mouseX} client.mouseY={client.mouseY} client.lastMouseButton={client.lastMouseButton} client.mouseButton={client.mouseButton}");
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
            else
                if (client.loginScreen == 1)
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
                else
                    if (client.loginScreen == 2)
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
        public void HandleMouseDown(int mouseButtonPressed, int mouseXPosition, int mouseYPosition)
        {
            client.mouseTrailX[client.mouseTrailIndex] = mouseXPosition;
            client.mouseTrailY[client.mouseTrailIndex] = mouseYPosition;
            client.mouseTrailIndex = client.mouseTrailIndex + 1 & 0x1fff;
            for (int l = 10; l < 4000; l += 1)
            {
                int lastMouseTrailIndex = client.mouseTrailIndex - l & 0x1fff;
                if (client.mouseTrailX[lastMouseTrailIndex] == mouseXPosition && client.mouseTrailY[lastMouseTrailIndex] == mouseYPosition)
                {
                    bool flag = false;
                    for (int j1 = 1; j1 < l; j1 += 1)
                    {
                        int mouseNew = client.mouseTrailIndex - j1 & 0x1fff;
                        int mouseOld = lastMouseTrailIndex - j1 & 0x1fff;
                        if (client.mouseTrailX[mouseOld] != mouseXPosition || client.mouseTrailY[mouseOld] != mouseYPosition)
                        {
                            flag = true;
                        }

                        if (client.mouseTrailX[mouseNew] != client.mouseTrailX[mouseOld] || client.mouseTrailY[mouseNew] != client.mouseTrailY[mouseOld])
                        {
                            break;
                        }

                        if (j1 == l - 1 && flag && client.combatTimeout == 0 && client.logoutTimer == 0)
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
            if (client.systemUpdate > 1)
            {
                client.systemUpdate -= 1;
            }

            client.SendPingPacketAsync();




            if (client.logoutTimer > 0)
            {
                client.logoutTimer -= 1;
            }

            if (client.ourPlayer.currentSprite == 8 || client.ourPlayer.currentSprite == 9)
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
            for (int l = 0; l < client.playerCount; l += 1)
            {
                ClientMob player = client.playerArray[l];
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

                    if (player.waypointsX[targetSprite] - player.currentX > client.gridSize * 3 || player.waypointsY[targetSprite] - player.currentY > client.gridSize * 3 || player.waypointsX[targetSprite] - player.currentX < -client.gridSize * 3 || player.waypointsY[targetSprite] - player.currentY < -client.gridSize * 3 || i5 > 8)
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

                if (client.playerAliveTimeout > 0)
                {
                    client.playerAliveTimeout -= 1;
                    if (client.playerAliveTimeout == 0)
                    {
                        client.DisplayMessage("You have been granted another life. Be more careful this time!", 3);
                    }

                    if (client.playerAliveTimeout == 0)
                    {
                        client.DisplayMessage("You retain your skills. Your objects land where you died", 3);
                    }
                }
            }

            for (int i1 = 0; i1 < client.npcCount; i1 += 1)
            {
                ClientMob f2 = client.npcArray[i1];
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

                    if (f2.waypointsX[j5] - f2.currentX > client.gridSize * 3 || f2.waypointsY[j5] - f2.currentY > client.gridSize * 3 || f2.waypointsX[j5] - f2.currentX < -client.gridSize * 3 || f2.waypointsY[j5] - f2.currentY < -client.gridSize * 3 || j6 > 8)
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

            if (client.drawMenuTab != 2)
            {
                if (GameImage.spiralDrawCount > 0)
                {
                    client.sleepWordDelayTimer += 1;
                }

                if (GameImage.characterDrawCount > 0)
                {
                    client.sleepWordDelayTimer = 0;
                }

                GameImage.spiralDrawCount = 0;
                GameImage.characterDrawCount = 0;
            }
            for (int k1 = 0; k1 < client.playerCount; k1 += 1)
            {
                ClientMob f3 = client.playerArray[k1];
                if (f3.projectileDistance > 0)
                {
                    f3.projectileDistance -= 1;
                }
            }

            if (client.cameraAutoAngleDebug)
            {
                if (client.cameraAutoRotatePlayerX - client.ourPlayer.currentX < -500 || client.cameraAutoRotatePlayerX - client.ourPlayer.currentX > 500 || client.cameraAutoRotatePlayerY - client.ourPlayer.currentY < -500 || client.cameraAutoRotatePlayerY - client.ourPlayer.currentY > 500)
                {
                    client.cameraAutoRotatePlayerX = client.ourPlayer.currentX;
                    client.cameraAutoRotatePlayerY = client.ourPlayer.currentY;
                }
            }
            else
            {
                if (client.cameraAutoRotatePlayerX - client.ourPlayer.currentX < -500 || client.cameraAutoRotatePlayerX - client.ourPlayer.currentX > 500 || client.cameraAutoRotatePlayerY - client.ourPlayer.currentY < -500 || client.cameraAutoRotatePlayerY - client.ourPlayer.currentY > 500)
                {
                    client.cameraAutoRotatePlayerX = client.ourPlayer.currentX;
                    client.cameraAutoRotatePlayerY = client.ourPlayer.currentY;
                }
                if (client.cameraAutoRotatePlayerX != client.ourPlayer.currentX)
                {
                    client.cameraAutoRotatePlayerX += (client.ourPlayer.currentX - client.cameraAutoRotatePlayerX) / (16 + (client.cameraDistance - 500) / 15);
                }

                if (client.cameraAutoRotatePlayerY != client.ourPlayer.currentY)
                {
                    client.cameraAutoRotatePlayerY += (client.ourPlayer.currentY - client.cameraAutoRotatePlayerY) / (16 + (client.cameraDistance - 500) / 15);
                }

                if (client.configCameraAutoAngle)
                {
                    int j2 = client.cameraAutoAngle * 32;
                    int i4 = j2 - client.cameraRotation;
                    int byte0 = 1;
                    if (i4 != 0)
                    {
                        client.cameraAutoRotationAmount += 1;
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
                        client.cameraRotation += ((client.cameraAutoRotationAmount * i4 + 255) / 256) * byte0;
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
                    else
                        if (client.enteredInputText.ToLower() == "::closecon")
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
                    if (!client.HandleCommand(input.Substring(2)))
                    {
                        client.CallSendCommand(input.Substring(2));
                    }
                }
                else
                {
                    int len = ChatMessage.StringToBytes(input);
                    client.CallSendChatMessage(ChatMessage.lastChat, len);
                    input = ChatMessage.BytesToString(ChatMessage.lastChat, 0, len);
                    //if (useChatFilter)
                    //input = ChatFilter.filterChat(input);
                    client.ourPlayer.lastMessageTimeout = 150;
                    client.ourPlayer.lastMessage = input;
                    client.DisplayMessage(client.ourPlayer.username + ": " + input, 2);
                }
            }
            if (client.messagesTab == 0)
            {
                for (int k2 = 0; k2 < 5; k2 += 1)
                {
                    if (client.messagesTimeout[k2] > 0)
                    {
                        client.messagesTimeout[k2] -= 1;
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

                            for (int l2 = 0; l2 < 8; l2 += 1)
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

                            for (int i3 = 0; i3 < 8; i3 += 1)
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
            else
                if (client.actionPictureType < 0)
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
            for (int j3 = 0; j3 < client.objectCount; j3 += 1)
            {
                int k4 = client.objectX[j3];
                int k5 = client.objectY[j3];
                if (k4 >= 0 && k5 >= 0 && k4 < 96 && k5 < 96 && client.objectType[j3] == 74)
                {
                    client.objectArray[j3].OffsetMiniPosition(1, 0, 0);
                }
            }

            for (int l4 = 0; l4 < client.teleBubbleCount; l4 += 1)
            {
                client.teleBubbleTime[l4] += 1;
                if (client.teleBubbleTime[l4] > 50)
                {
                    client.teleBubbleCount -= 1;
                    for (int l5 = l4; l5 < client.teleBubbleCount; l5 += 1)
                    {
                        client.teleBubbleX[l5] = client.teleBubbleX[l5 + 1];
                        client.teleBubbleY[l5] = client.teleBubbleY[l5 + 1];
                        client.teleBubbleTime[l5] = client.teleBubbleTime[l5 + 1];
                        client.teleBubbleType[l5] = client.teleBubbleType[l5 + 1];
                    }

                }
            }

        }
        public void HandleKeyDown(Keys key, char c)
        {
            if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
            {
                return;
            }

            if (!client.loggedIn)
            {
                if (client.loginScreen == 0 && client.loginMenuFirst is not null)
                {
                    client.loginMenuFirst.KeyPress(key, c);
                }

                if (client.loginScreen == 1 && client.loginNewUser is not null)
                {
                    client.loginNewUser.KeyPress(key, c);
                }

                if (client.loginScreen == 2 && client.loginMenuLogin is not null)
                {
                    client.loginMenuLogin.KeyPress(key, c);
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
                    client.appearanceMenu.KeyPress(key, c);
                }
                else if (client.showFriendsBox == 0 && client.showAbuseBox == 0 && !client.isSleeping && client.chatInputMenu is not null)
                {
                    client.chatInputMenu.KeyPress(key, c);
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
            for (int l = 0; l < client.menuOptionsCount; l += 1)
            {
                client.menuIndexes[l] = l;
            }

            for (bool flag = false; !flag; )
            {
                flag = true;
                for (int i1 = 0; i1 < client.menuOptionsCount - 1; i1 += 1)
                {
                    int k1 = client.menuIndexes[i1];
                    int i2 = client.menuIndexes[i1 + 1];
                    if (client.menuActionID[k1] > client.menuActionID[i2])
                    {
                        client.menuIndexes[i1] = i2;
                        client.menuIndexes[i1 + 1] = k1;
                        flag = false;
                    }
                }

            }

            if (client.menuOptionsCount > 20)
            {
                client.menuOptionsCount = 20;
            }

            if (client.menuOptionsCount > 0)
            {
                int j1 = -1;
                for (int l1 = 0; l1 < client.menuOptionsCount; l1 += 1)
                {
                    if (client.menuText2[client.menuIndexes[l1]] is null || client.menuText2[client.menuIndexes[l1]].Length <= 0)
                    {
                        continue;
                    }

                    j1 = l1;
                    break;
                }

                string s1 = null;
                if ((client.selectedItem >= 0 || client.selectedSpell >= 0) && client.menuOptionsCount == 1)
                {
                    s1 = "Choose a target";
                }
                else
                    if ((client.selectedItem >= 0 || client.selectedSpell >= 0) && client.menuOptionsCount > 1)
                {
                    s1 = "@whi@" + client.menuText1[client.menuIndexes[0]] + " " + client.menuText2[client.menuIndexes[0]];
                }
                else
                        if (j1 != -1)
                {
                    s1 = client.menuText2[client.menuIndexes[j1]] + ": @whi@" + client.menuText1[client.menuIndexes[0]];
                }

                if (client.menuOptionsCount == 2 && s1 is not null)
                {
                    s1 += "@whi@ / 1 more option";
                }

                if (client.menuOptionsCount > 2 && s1 is not null)
                {
                    s1 = s1 + "@whi@ / " + (client.menuOptionsCount - 1) + " more options";
                }

                if (s1 is not null)
                {
                    client.gameGraphics.DrawString(s1, 6, 14, 1, 0xffff00);
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
                    client.menuWidth = client.gameGraphics.TextWidth("Choose option", 1) + 5;
                    for (int j2 = 0; j2 < client.menuOptionsCount; j2 += 1)
                    {
                        int k2 = client.gameGraphics.TextWidth(client.menuText1[j2] + " " + client.menuText2[j2], 1) + 5;
                        if (k2 > client.menuWidth)
                        {
                            client.menuWidth = k2;
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
