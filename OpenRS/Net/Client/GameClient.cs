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

    public sealed class GameClient : GameAppletMiddleMan
    {
        public int killingSpree;
        public event EventHandler OnContentLoadedCompleted;
        public event EventHandler<ContentLoadedEventArgs> OnContentLoaded;

        public static Microsoft.Xna.Framework.GameWindow GameWindow;

        public event ChatMessageEventHandler OnChatMessageReceived;

        public static GameClient CreateMudclient(string title, int width, int height)
        {
            GameClient mud = new()
            {
                windowWidth = width,
                windowHeight = height
            };
            mud.CreateWindow(mud.windowWidth, mud.windowHeight + 11, title, false);
            mud.gameMinThreadSleepTime = 10;

            return mud;
        }

        public static GameClient CreateMudclient(string title)
        {
            return CreateMudclient(title, 512, 346);
        }

        public static GameClient CreateMudclient()
        {
            return CreateMudclient("RuneScape", 512, 346);
        }

        public static GameClient CreateGameClient(string username, string password, int width, int height)
        {
            return CreateMudclient("RuneScape", width, height);
        }

        public static GameClient CreateGameClient(string username, string password)
        {
            return CreateMudclient("RuneScape", 512, 346);
        }

        public new void Dispose()
        {
            Destroy();
        }

        public void Paint() { }

        public void UnloadContent() { }

        public void DrawNpc(int x, int y, int width, int height, int npcIndex, int cameraXOffset, int scalePercentage) { }

        public Models.Enumerations.CombatStyle CombatStyle
        {
            get => (Models.Enumerations.CombatStyle)combatStyle;
            set => combatStyle = (int)value;
        }

        public void SetCombatStyle(Models.Enumerations.CombatStyle style)
        {
            combatStyle = (int)style;
        }

        public ClientMob CurrentPlayer
        {
            get
            {
                if (playerArray is null || playerArray.Length == 0)
                {
                    return null;
                }

                return playerArray[0];
            }
        }

        public ClientMob[] Players => playerArray;

        public ClientMob[] Npcs => npcArray;

        public int GridSize => gridSize;

        public int GroundItemCount => groundItemCount;

        public int PlayerFatigue => fatigue;

        public NuciXNA.Primitives.Point2D[] GroundItemLocations
        {
            get
            {
                NuciXNA.Primitives.Point2D[] locs = new NuciXNA.Primitives.Point2D[groundItemCount];
                for (int i = 0; i < groundItemCount; i += 1)
                {
                    locs[i] = new NuciXNA.Primitives.Point2D(groundItemX[i], groundItemY[i]);
                }
                return locs;
            }
        }

        public Models.Skill[] Skills
        {
            get
            {
                if (playerStatBase is null)
                {
                    return [];
                }

                Models.Skill[] skills = new Models.Skill[playerStatBase.Length];

                for (int i = 0; i < playerStatBase.Length; i += 1)
                {
                    int currentLevel = 0;

                    if (playerStatCurrent is not null)
                    {
                        currentLevel = playerStatCurrent[i];
                    }

                    int experience = 0;

                    if (playerStatExp is not null)
                    {
                        experience = playerStatExp[i];
                    }

                    string name = string.Empty;

                    if (skillName is not null && i < skillName.Length)
                    {
                        name = skillName[i];
                    }

                    skills[i] = new Models.Skill
                    {
                        BaseLevel = playerStatBase[i],
                        CurrentLevel = currentLevel,
                        Experience = experience,
                        Name = name
                    };
                }
                return skills;
            }
        }

        public int GameDisplayOffsetX { get; set; }

        public int GameDisplayOffsetY { get; set; }

        public float GameDisplayScaleX { get; set; } = 1.0f;

        public float GameDisplayScaleY { get; set; } = 1.0f;

        public GameLogic.GameManagers.EntityManager entityManager;

        public GameLogic.GameManagers.InventoryManager inventoryManager;

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


        public void MenuClick(int l)
        {
            int actionX = menuActionX[l];
            int actionY = menuActionY[l];
            int actionType = menuActionType[l];
            int actionVar1 = menuActionVar1[l];
            int actionVar2 = menuActionVar2[l];
            int actionID = menuActionID[l];
            if (actionID == 200)
            {
                WalkToGroundItem(sectionX, sectionY, actionX, actionY, true);
                base.streamClass.CreatePacket(104);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 210)
            {
                WalkToGroundItem(sectionX, sectionY, actionX, actionY, true);
                base.streamClass.CreatePacket(34);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddShort(actionType);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 220)
            {
                WalkToGroundItem(sectionX, sectionY, actionX, actionY, true);
                base.streamClass.CreatePacket(245);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddShort(actionType);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.FormatPacket();
            }
            if (actionID == 3200)
            {
                DisplayMessage(Data.GameData.itemDescription[actionType], 3);
            }

            if (actionID == 300)
            {
                WalkToWallObject(actionX, actionY, actionType);
                base.streamClass.CreatePacket(67);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddByte(actionType);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 310)
            {
                WalkToWallObject(actionX, actionY, actionType);
                base.streamClass.CreatePacket(36);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddByte(actionType);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 320)
            {
                WalkToWallObject(actionX, actionY, actionType);
                base.streamClass.CreatePacket(126);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddByte(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 2300)
            {
                WalkToWallObject(actionX, actionY, actionType);
                base.streamClass.CreatePacket(235);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddByte(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 3300)
            {
                DisplayMessage(Data.GameData.wallObjectDescription[actionType], 3);
            }

            if (actionID == 400)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                base.streamClass.CreatePacket(17);
                base.streamClass.AddShort(actionVar2);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);

                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 410)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                base.streamClass.CreatePacket(94);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.AddShort(actionVar2);
                base.streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 420)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                base.streamClass.CreatePacket(51);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.FormatPacket();
            }
            if (actionID == 2400)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                base.streamClass.CreatePacket(40);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.FormatPacket();
            }
            if (actionID == 3400)
            {
                DisplayMessage(Data.GameData.objectDescription[actionType], 3);
            }

            if (actionID == 600)
            {
                base.streamClass.CreatePacket(49);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 610)
            {
                base.streamClass.CreatePacket(27);
                base.streamClass.AddShort(actionType);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 620)
            {
                base.streamClass.CreatePacket(92);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 630)
            {
                base.streamClass.CreatePacket(181);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 640)
            {
                base.streamClass.CreatePacket(89);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 650)
            {
                selectedItem = actionType;
                drawMenuTab = 0;
                selectedItemName = Data.GameData.itemName[inventoryItems[selectedItem]];
            }
            if (actionID == 660)
            {
                base.streamClass.CreatePacket(147);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
                selectedItem = -1;
                drawMenuTab = 0;
                DisplayMessage("Dropping " + Data.GameData.itemName[inventoryItems[actionType]], 4);
            }
            if (actionID == 3600)
            {
                DisplayMessage(Data.GameData.itemDescription[actionType], 3);
            }

            if (actionID == 700)
            {
                int k2 = (actionX - 64) / gridSize;
                int k4 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, k2, k4, true);
                base.streamClass.CreatePacket(71);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 710)
            {
                int l2 = (actionX - 64) / gridSize;
                int l4 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, l2, l4, true);
                base.streamClass.CreatePacket(142);
                base.streamClass.AddShort(actionType);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 720)
            {
                int i3 = (actionX - 64) / gridSize;
                int i5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, i3, i5, true);
                base.streamClass.CreatePacket(177);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 725)
            {
                int j3 = (actionX - 64) / gridSize;
                int j5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, j3, j5, true);
                base.streamClass.CreatePacket(74);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 715 || actionID == 2715)
            {
                int k3 = (actionX - 64) / gridSize;
                int k5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, k3, k5, true);
                base.streamClass.CreatePacket(73);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 3700)
            {
                DisplayMessage(Data.GameData.npcDescription[actionType], 3);
            }

            if (actionID == 800)
            {
                int l3 = (actionX - 64) / gridSize;
                int l5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, l3, l5, true);
                base.streamClass.CreatePacket(55);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 810)
            {
                int i4 = (actionX - 64) / gridSize;
                int i6 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, i4, i6, true);
                base.streamClass.CreatePacket(16);
                base.streamClass.AddShort(actionType);
                base.streamClass.AddShort(actionVar1);
                base.streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 805 || actionID == 2805)
            {
                int j4 = (actionX - 64) / gridSize;
                int j6 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, j4, j6, true);
                base.streamClass.CreatePacket(57);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 2806)
            {
                base.streamClass.CreatePacket(222);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 2810)
            {
                base.streamClass.CreatePacket(166);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 2820)
            {
                base.streamClass.CreatePacket(68);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
            }
            if (actionID == 900)
            {
                WalkTo1Tile(sectionX, sectionY, actionX, actionY, true);
                base.streamClass.CreatePacket(232);
                base.streamClass.AddShort(actionType);
                base.streamClass.AddShort(actionX + areaX);
                base.streamClass.AddShort(actionY + areaY);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 920)
            {
                WalkTo1Tile(sectionX, sectionY, actionX, actionY, false);
                if (actionPictureType == -24)
                {
                    actionPictureType = 24;
                }
            }
            if (actionID == 1000)
            {
                base.streamClass.CreatePacket(206);
                base.streamClass.AddShort(actionType);
                base.streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 4000)
            {
                selectedItem = -1;
                selectedSpell = -1;
            }
        }

        public override void ResetIntVars()
        {
            systemUpdate = 0;
            loginScreen = 0;
            loggedIn = false;
            logoutTimer = 0;
        }

        public void DrawReportAbuseBox1()
        {
            reportAbuseOptionSelected = 0;
            int yOffset = 135;
            for (int option = 0; option < 12; option += 1)
            {
                if (base.mouseX > 66 && base.mouseX < 446 && base.mouseY >= yOffset - 12 && base.mouseY < yOffset + 3)
                {
                    reportAbuseOptionSelected = option + 1;
                }

                yOffset += 14;
            }

            if (mouseButtonClick != 0 && reportAbuseOptionSelected != 0)
            {
                mouseButtonClick = 0;
                showAbuseBox = 2;
                base.inputText = "";
                base.enteredInputText = "";
                return;
            }
            yOffset += 15;
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                if (base.mouseX < 56 || base.mouseY < 35 || base.mouseX > 456 || base.mouseY > 325)
                {
                    showAbuseBox = 0;
                    return;
                }
                if (base.mouseX > 66 && base.mouseX < 446 && base.mouseY >= yOffset - 15 && base.mouseY < yOffset + 5)
                {
                    showAbuseBox = 0;
                    return;
                }
            }
            gameGraphics.DrawBox(56, 35, 400, 290, 0);
            gameGraphics.DrawBoxEdge(56, 35, 400, 290, 0xffffff);
            yOffset = 50;
            gameGraphics.DrawText("This form is for reporting players who are breaking our rules", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            gameGraphics.DrawText("Using it sends a snapshot of the last 60 secs of activity to us", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            gameGraphics.DrawText("If you misuse this form, you will be banned.", 256, yOffset, 1, 0xff8000);
            yOffset += 15;
            yOffset += 10;
            gameGraphics.DrawText("First indicate which of our 12 rules is being broken. For a detailed", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            gameGraphics.DrawText("explanation of each rule please read the manual on our website.", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            int j1;
            if (reportAbuseOptionSelected == 1)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("1: Offensive language", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 2)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("2: Item scamming", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 3)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("3: Password scamming", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 4)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("4: Bug abuse", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 5)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("5: Jagex Staff impersonation", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 6)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("6: Account sharing/trading", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 7)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("7: Macroing", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 8)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("8: Mutiple logging in", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 9)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("9: Encouraging others to break rules", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 10)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("10: Misuse of customer support", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 11)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("11: Advertising / website", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 12)
            {
                gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.DrawText("12: Real world item trading", 256, yOffset, 1, j1);
            yOffset += 14;
            yOffset += 15;
            j1 = 0xffffff;
            if (base.mouseX > 196 && base.mouseX < 316 && base.mouseY > yOffset - 15 && base.mouseY < yOffset + 5)
            {
                j1 = 0xffff00;
            }

            gameGraphics.DrawText("Click here to cancel", 256, yOffset, 1, j1);
        }

        public void LoadMap()
        {
            engineHandle.mapsFree = UnpackData("maps.jag", "map", 70);
            engineHandle.mapsMembers = UnpackData("maps.mem", "members map", 75);
            engineHandle.landscapeFree = UnpackData("land.jag", "landscape", 80);
            engineHandle.landscapeMembers = UnpackData("land.mem", "members landscape", 85);
        }

        public void DrawModel(int l, string s1)
        {
            int i1 = objectX[l];
            int j1 = objectY[l];
            int k1 = i1 - ourPlayer.currentX / 128;
            int l1 = j1 - ourPlayer.currentY / 128;
            byte byte0 = 7;
            if (i1 >= 0 && j1 >= 0 && i1 < 96 && j1 < 96 && k1 > -byte0 && k1 < byte0 && l1 > -byte0 && l1 < byte0)
            {
                gameCamera.RemoveModel(objectArray[l]);
                int i2 = Data.GameData.GetModelNameIndex(s1);
                GameObject j2 = gameDataObjects[i2].CreateParent();
                gameCamera.AddModel(j2);
                j2.UpdateShading(true, 48, 48, -50, -10, -50);
                j2.CopyTranslation(objectArray[l]);
                j2.index = l;
                objectArray[l] = j2;
            }
        }

        public void DrawPlayer(int x, int y, int width, int height, int playerIndex, int cameraXOffset, int scalePercentage)
        {
            ClientMob f1 = playerArray[playerIndex];
            if (f1.bottomColour == 255)// TODO this checks if the player is an invisible moderator
            {
                return;
            }

            int direction = f1.currentSprite + (cameraRotation + 16) / 32 & 7;
            bool flag = false;
            int direction2 = direction;
            if (direction2 == 5)
            {
                direction2 = 3;
                flag = true;
            }
            else if (direction2 == 6)
            {
                direction2 = 2;
                flag = true;
            }
            else if (direction2 == 7)
            {
                direction2 = 1;
                flag = true;
            }
            int j1 = direction2 * 3 + walkModel[(f1.stepCount / 6) % 4];
            if (f1.currentSprite == 8)
            {
                direction2 = 5;
                direction = 2;
                flag = false;
                x -= (5 * scalePercentage) / 100;
                j1 = direction2 * 3 + combatModelArray1[(tick / 5) % 8];
            }
            else
                if (f1.currentSprite == 9)
                {
                    direction2 = 5;
                    direction = 2;
                    flag = true;
                    x += (5 * scalePercentage) / 100;
                    j1 = direction2 * 3 + combatModelArray2[(tick / 6) % 8];
                }
            for (int k1 = 0; k1 < 12; k1 += 1)
            {
                int l1 = animationModelArray[direction][k1];
                int l2 = f1.appearanceItems[l1] - 1;
                if (l2 > Data.GameData.animationCount - 1)
                {
                    continue;
                }

                if (l2 >= 0)
                {
                    int k3 = 0;
                    int i4 = 0;
                    int j4 = j1;
                    if (flag && direction2 >= 1 && direction2 <= 3)
                    {
                        if (Data.GameData.animationHasF[l2] == 1)
                        {
                            j4 += 15;
                        }
                        else if (l1 == 4 && direction2 == 1)
                        {
                            k3 = -22;
                            i4 = -3;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = -8;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 3)
                        {
                            k3 = 26;
                            i4 = -5;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 1)
                        {
                            k3 = 22;
                            i4 = 3;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = 8;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 3)
                        {
                            k3 = -26;
                            i4 = 5;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                    }

                    if (direction2 != 5 || Data.GameData.animationHasA[l2] == 1)
                    {
                        int k4 = j4 + Data.GameData.animationNumber[l2];
                        k3 = (k3 * width) / ((GameImage)(gameGraphics)).pictureAssumedWidth[k4];
                        i4 = (i4 * height) / ((GameImage)(gameGraphics)).pictureAssumedHeight[k4];
                        int l4 = (width * ((GameImage)(gameGraphics)).pictureAssumedWidth[k4]) / ((GameImage)(gameGraphics)).pictureAssumedWidth[Data.GameData.animationNumber[l2]];
                        k3 -= (l4 - width) / 2;
                        int i5 = Data.GameData.animationCharacterColor[l2];
                        int j5 = appearanceSkinColours[f1.skinColour];
                        if (i5 == 1)
                        {
                            i5 = appearanceHairColours[f1.hairColour];
                        }
                        else
                            if (i5 == 2)
                        {
                            i5 = appearanceTopBottomColours[f1.topColour];
                        }
                        else
                                if (i5 == 3)
                        {
                            i5 = appearanceTopBottomColours[f1.bottomColour];
                        }

                        gameGraphics.DrawImage(x + k3, y + i4, l4, height, k4, i5, j5, cameraXOffset, flag);
                    }
                }
            }

            if (f1.lastMessageTimeout > 0)
            {
                receivedMessageMidPoint[receivedMessagesCount] = gameGraphics.TextWidth(f1.lastMessage, 1) / 2;
                if (receivedMessageMidPoint[receivedMessagesCount] > 150)
                {
                    receivedMessageMidPoint[receivedMessagesCount] = 150;
                }

                receivedMessageHeight[receivedMessagesCount] = (gameGraphics.TextWidth(f1.lastMessage, 1) / 300) * gameGraphics.TextHeightNumber(1);
                receivedMessageX[receivedMessagesCount] = x + width / 2;
                receivedMessageY[receivedMessagesCount] = y;
                receivedMessages[receivedMessagesCount++] = f1.lastMessage;
            }
            if (f1.playerSkullTimeout > 0)
            {
                itemAboveHeadX[itemsAboveHeadCount] = x + width / 2;
                itemAboveHeadY[itemsAboveHeadCount] = y;
                itemAboveHeadScale[itemsAboveHeadCount] = scalePercentage;
                itemAboveHeadID[itemsAboveHeadCount++] = f1.itemAboveHeadID;
            }
            if (f1.currentSprite == 8 || f1.currentSprite == 9 || f1.combatTimer != 0)
            {
                if (f1.combatTimer > 0)
                {
                    int i2 = x;
                    if (f1.currentSprite == 8)
                    {
                        i2 -= (20 * scalePercentage) / 100;
                    }
                    else
                        if (f1.currentSprite == 9)
                    {
                        i2 += (20 * scalePercentage) / 100;
                    }

                    int i3 = (f1.currentHits * 30) / f1.baseHits;
                    healthBarX[healthBarVisibleCount] = i2 + width / 2;
                    healthBarY[healthBarVisibleCount] = y;
                    healthBarMissing[healthBarVisibleCount++] = i3;
                }
                if (f1.combatTimer > 150)
                {
                    int j2 = x;
                    if (f1.currentSprite == 8)
                    {
                        j2 -= (10 * scalePercentage) / 100;
                    }
                    else
                        if (f1.currentSprite == 9)
                    {
                        j2 += (10 * scalePercentage) / 100;
                    }

                    gameGraphics.DrawPicture((j2 + width / 2) - 12, (y + height / 2) - 12, baseInventoryPic + 11);
                    gameGraphics.DrawText(f1.lastDamageCount.ToString(), (j2 + width / 2) - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
            if (f1.playerSkulled == 1 && f1.playerSkullTimeout == 0)
            {
                int k2 = cameraXOffset + x + width / 2;
                if (f1.currentSprite == 8)
                {
                    k2 -= (20 * scalePercentage) / 100;
                }
                else
                    if (f1.currentSprite == 9)
                {
                    k2 += (20 * scalePercentage) / 100;
                }

                int j3 = (16 * scalePercentage) / 100;
                int l3 = (16 * scalePercentage) / 100;
                gameGraphics.DrawEntity(k2 - j3 / 2, y - l3 / 2 - (10 * scalePercentage) / 100, j3, l3, baseInventoryPic + 13);
            }
        }

        public void WalkToWallObject(int x, int y, int direction)
        {
            if (direction == 0)
            {
                WalkTo(sectionX, sectionY, x, y - 1, x, y, false, true);
                return;
            }
            if (direction == 1)
            {
                WalkTo(sectionX, sectionY, x - 1, y, x, y, false, true);
                return;
            }
            else
            {
                WalkTo(sectionX, sectionY, x, y, x, y, true, true);
                return;
            }
        }

        public void DrawDuelConfirmBox()
        {
            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.DrawBox(byte0, byte1, 468, 16, 192);
            int l = 0x989898;
            gameGraphics.DrawBoxAlpha(byte0, byte1 + 16, 468, 246, l, 160);
            gameGraphics.DrawText("Please confirm your duel with @yel@" + DataOperations.HashToName(duelOpponentHash), byte0 + 234, byte1 + 12, 1, 0xffffff);
            gameGraphics.DrawText("Your stake:", byte0 + 117, byte1 + 30, 1, 0xffff00);
            for (int i1 = 0; i1 < duelOurStakeCount; i1 += 1)
            {
                string s1 = Data.GameData.itemName[duelOurStakeItem[i1]];
                if (Data.GameData.itemStackable[duelOurStakeItem[i1]] == 0)
                {
                    s1 = s1 + " x " + formatItemCount(duelOurStakeItemCount[i1]);
                }

                gameGraphics.DrawText(s1, byte0 + 117, byte1 + 42 + i1 * 12, 1, 0xffffff);
            }

            if (duelOurStakeCount == 0)
            {
                gameGraphics.DrawText("Nothing!", byte0 + 117, byte1 + 42, 1, 0xffffff);
            }

            gameGraphics.DrawText("Your opponent's stake:", byte0 + 351, byte1 + 30, 1, 0xffff00);
            for (int j1 = 0; j1 < duelOpponentStakeCount; j1 += 1)
            {
                string s2 = Data.GameData.itemName[duelOpponentStakeItem[j1]];
                if (Data.GameData.itemStackable[duelOpponentStakeItem[j1]] == 0)
                {
                    s2 = s2 + " x " + formatItemCount(duelOutStakeItemCount[j1]);
                }

                gameGraphics.DrawText(s2, byte0 + 351, byte1 + 42 + j1 * 12, 1, 0xffffff);
            }

            if (duelOpponentStakeCount == 0)
            {
                gameGraphics.DrawText("Nothing!", byte0 + 351, byte1 + 42, 1, 0xffffff);
            }

            if (duelRetreat == 0)
            {
                gameGraphics.DrawText("You can retreat from this duel", byte0 + 234, byte1 + 180, 1, 65280);
            }
            else
            {
                gameGraphics.DrawText("No retreat is possible!", byte0 + 234, byte1 + 180, 1, 0xff0000);
            }

            if (duelMagic == 0)
            {
                gameGraphics.DrawText("Magic may be used", byte0 + 234, byte1 + 192, 1, 65280);
            }
            else
            {
                gameGraphics.DrawText("Magic cannot be used", byte0 + 234, byte1 + 192, 1, 0xff0000);
            }

            if (duelPrayer == 0)
            {
                gameGraphics.DrawText("Prayer may be used", byte0 + 234, byte1 + 204, 1, 65280);
            }
            else
            {
                gameGraphics.DrawText("Prayer cannot be used", byte0 + 234, byte1 + 204, 1, 0xff0000);
            }

            if (duelWeapons == 0)
            {
                gameGraphics.DrawText("Weapons may be used", byte0 + 234, byte1 + 216, 1, 65280);
            }
            else
            {
                gameGraphics.DrawText("Weapons cannot be used", byte0 + 234, byte1 + 216, 1, 0xff0000);
            }

            gameGraphics.DrawText("If you are sure click 'Accept' to begin the duel", byte0 + 234, byte1 + 230, 1, 0xffffff);
            if (!duelConfirmOurAccepted)
            {
                gameGraphics.DrawPicture((byte0 + 118) - 35, byte1 + 238, baseInventoryPic + 25);
                gameGraphics.DrawPicture((byte0 + 352) - 35, byte1 + 238, baseInventoryPic + 26);
            }
            else
            {
                gameGraphics.DrawText("Waiting for other player...", byte0 + 234, byte1 + 250, 1, 0xffff00);
            }
            if (mouseButtonClick == 1)
            {
                if (base.mouseX < byte0 || base.mouseY < byte1 || base.mouseX > byte0 + 468 || base.mouseY > byte1 + 262)
                {
                    showDuelConfirmBox = false;
                    base.streamClass.CreatePacket(35);
                    base.streamClass.FormatPacket();
                }
                if (base.mouseX >= (byte0 + 118) - 35 && base.mouseX <= byte0 + 118 + 70 && base.mouseY >= byte1 + 238 && base.mouseY <= byte1 + 238 + 21)
                {
                    duelConfirmOurAccepted = true;
                    base.streamClass.CreatePacket(87);
                    base.streamClass.FormatPacket();
                }
                if (base.mouseX >= (byte0 + 352) - 35 && base.mouseX <= byte0 + 353 + 70 && base.mouseY >= byte1 + 238 && base.mouseY <= byte1 + 238 + 21)
                {
                    showDuelConfirmBox = false;
                    base.streamClass.CreatePacket(35);
                    base.streamClass.FormatPacket();
                }
                mouseButtonClick = 0;
            }
        }

        public void SetLoginVars()
        {
            loggedIn = false;
            loginScreen = 0;
            loginUsername = "";
            loginPassword = "";
            /*dja = "Please enter a username:";
            djb = "*" + loginUsername + "*";*/
            playerCount = 0;
            npcCount = 0;
        }

        public override void Close()
        {
            RequestLogout();
            CleanUp();
            if (audioPlayer is not null)
            {
                audioPlayer.Stop();
            }
        }

        //protected TcpClient makeSocket(string address, int port) {

        //    if(Link.gameApplet is not null) {
        //        Socket socket = Link.getSocket(port);
        //        if(socket is null)
        //            throw new IOException();
        //        else
        //            return socket;
        //    }
        //    Socket socket1 = new Socket(InetAddress.getByName(address), port);
        //    socket1.setSoTimeout(30000);
        //    socket1.setTcpNoDelay(true);
        //    return socket1;
        //}

        public void DrawInventoryMenu(bool canRightClick)
        {
            int l = ((GameImage)(gameGraphics)).gameWidth - 248;
            gameGraphics.DrawPicture(l, 3, baseInventoryPic + 1);
            for (int i1 = 0; i1 < maxInventoryItems; i1 += 1)
            {
                int j1 = l + (i1 % 5) * 49;
                int l1 = 36 + (i1 / 5) * 34;
                if (i1 < inventoryItemsCount && inventoryItemEquipped[i1] == 1)
                {
                    gameGraphics.DrawBoxAlpha(j1, l1, 49, 34, 0xff0000, 128);
                }
                else
                {
                    gameGraphics.DrawBoxAlpha(j1, l1, 49, 34, GameImage.RgbToInt(181, 181, 181), 128);
                }

                if (i1 < inventoryItemsCount)
                {
                    gameGraphics.DrawImage(j1, l1, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[inventoryItems[i1]], Data.GameData.itemPictureMask[inventoryItems[i1]], 0, 0, false);
                    if (Data.GameData.itemStackable[inventoryItems[i1]] == 0)
                    {
                        gameGraphics.DrawString(inventoryItemCount[i1].ToString(), j1 + 1, l1 + 10, 1, 0xffff00);
                    }
                }
            }

            for (int k1 = 1; k1 <= 4; k1 += 1)
            {
                gameGraphics.DrawLineY(l + k1 * 49, 36, (maxInventoryItems / 5) * 34, 0);
            }

            for (int i2 = 1; i2 <= maxInventoryItems / 5 - 1; i2 += 1)
            {
                gameGraphics.DrawLineX(l, 36 + i2 * 34, 245, 0);
            }

            if (!canRightClick)
            {
                return;
            }

            l = base.mouseX - (((GameImage)(gameGraphics)).gameWidth - 248);
            int j2 = base.mouseY - 36;
            if (l >= 0 && j2 >= 0 && l < 248 && j2 < (maxInventoryItems / 5) * 34)
            {
                int k2 = l / 49 + (j2 / 34) * 5;
                if (k2 < inventoryItemsCount)
                {
                    int l2 = inventoryItems[k2];
                    if (selectedSpell >= 0)
                    {
                        if (Data.GameData.spellType[selectedSpell] == 3)
                        {
                            menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on";
                            menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 600;
                            menuActionType[menuOptionsCount] = k2;
                            menuActionVar1[menuOptionsCount] = selectedSpell;
                            menuOptionsCount += 1;
                            return;
                        }
                    }
                    else
                    {
                        if (selectedItem >= 0)
                        {
                            menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                            menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 610;
                            menuActionType[menuOptionsCount] = k2;
                            menuActionVar1[menuOptionsCount] = selectedItem;
                            menuOptionsCount += 1;
                            return;
                        }
                        if (inventoryItemEquipped[k2] == 1)
                        {
                            menuText1[menuOptionsCount] = "Remove";
                            menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 620;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount += 1;
                        }
                        else
                            if (Data.GameData.itemIsEquippable[l2] != 0)
                            {
                                if ((Data.GameData.itemIsEquippable[l2] & 0x18) != 0)
                            {
                                menuText1[menuOptionsCount] = "Wield";
                            }
                            else
                            {
                                menuText1[menuOptionsCount] = "Wear";
                            }

                            menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                                menuActionID[menuOptionsCount] = 630;
                                menuActionType[menuOptionsCount] = k2;
                                menuOptionsCount += 1;
                            }
                        if (Data.GameData.itemCommand[l2] != "")
                        {
                            menuText1[menuOptionsCount] = Data.GameData.itemCommand[l2];
                            menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 640;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount += 1;
                        }
                        menuText1[menuOptionsCount] = "Use";
                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                        menuActionID[menuOptionsCount] = 650;
                        menuActionType[menuOptionsCount] = k2;
                        menuOptionsCount += 1;
                        menuText1[menuOptionsCount] = "Drop";
                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                        menuActionID[menuOptionsCount] = 660;
                        menuActionType[menuOptionsCount] = k2;
                        menuOptionsCount += 1;
                        menuText1[menuOptionsCount] = "Examine";
                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[l2];
                        menuActionID[menuOptionsCount] = 3600;
                        menuActionType[menuOptionsCount] = l2;
                        menuOptionsCount += 1;
                    }
                }
            }
        }
        public bool DoNotDrawLogo { get; set; }
        public void CreateLoginScreenBackgrounds()
        {
            int _bgScreenWidth = windowWidth;
            if (this.OnLoadingSection is not null)
            {
                this.OnLoadingSection(this, new EventArgs());
            }

            int l = 0;
            sbyte byte0 = 50;
            sbyte byte1 = 50;

            engineHandle.LoadSection(byte0 * 48 + 23, byte1 * 48 + 23, l);
            engineHandle.AddObjects(gameDataObjects);

            //char c1 = '\u2600';
            //char c2 = '\u1900';
            //char c3 = '\u044C';
            //char c4 = '\u0378';

            int cameraX = 9728;
            int cameraY = 6400;
            int cameraDistance = 1100;
            int cameraRotation = 888;

            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(cameraX, -engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.FinishCamera();
            gameGraphics.ScreenFadeToBlack();
            gameGraphics.ScreenFadeToBlack();



            gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0x000000); //_bgScreenWidth=512
            for (int i1 = 6; i1 >= 1; i1 -= 1)
            {
                gameGraphics.DrawTransparentLine(0, i1, 0, i1, _bgScreenWidth, 8);
            }

            gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0x000000);

            for (int j1 = 6; j1 >= 1; j1 -= 1)
            {
                gameGraphics.DrawTransparentLine(0, j1, 0, 194 - j1, _bgScreenWidth, 8);
            }



#warning draws logo

            if (!DoNotDrawLogo)
            {
                if (bgPixels is null)
                {
                    gameGraphics.DrawPicture(15, 15, baseInventoryPic + 10);
                }
                else
                {
                    gameGraphics.DrawPixels(bgPixels, 0, 0, bgPixels.Length, bgPixels[0].Length);
                }
            }


            gameGraphics.DrawImage(baseLoginScreenBackgroundPic, 0, 0, _bgScreenWidth, 200);
            gameGraphics.ApplyImage(baseLoginScreenBackgroundPic);



            cameraX = 9216;
            cameraY = 9216;
            cameraDistance = 1100;
            cameraRotation = 888;
            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(cameraX, -engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.FinishCamera();
            gameGraphics.ScreenFadeToBlack();
            gameGraphics.ScreenFadeToBlack();



            gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int k1 = 6; k1 >= 1; k1 -= 1)
            {
                gameGraphics.DrawTransparentLine(0, k1, 0, k1, _bgScreenWidth, 8);
            }

            gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int l1 = 6; l1 >= 1; l1 -= 1)
            {
                gameGraphics.DrawTransparentLine(0, l1, 0, 194 - l1, _bgScreenWidth, 8);
            }

            if (!DoNotDrawLogo)
            {
                if (bgPixels is null)
                {
                    gameGraphics.DrawPicture(15, 15, baseInventoryPic + 10);
                }
                else
                {
                    gameGraphics.DrawPixels(bgPixels, 0, 0, bgPixels.Length, bgPixels[0].Length);
                }
            }

            gameGraphics.DrawImage(baseLoginScreenBackgroundPic + 1, 0, 0, _bgScreenWidth, 200);
            gameGraphics.ApplyImage(baseLoginScreenBackgroundPic + 1);

            // Remove buildings
            for (int i2 = 0; i2 < 64; i2 += 1)
            {

                gameCamera.RemoveModel(engineHandle.roofObject[0][i2]);
                gameCamera.RemoveModel(engineHandle.wallObject[0][i2]);
                gameCamera.RemoveModel(engineHandle.wallObject[1][i2]);
                gameCamera.RemoveModel(engineHandle.roofObject[1][i2]);
                gameCamera.RemoveModel(engineHandle.wallObject[2][i2]);
                gameCamera.RemoveModel(engineHandle.roofObject[2][i2]);
            }

            cameraX = 11136;//'\u2B80';
            cameraY = 10368;//'\u2880';
            cameraDistance = 500;//'\u01F4';
            cameraRotation = 376;//'\u0178';
            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(cameraX, -engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.FinishCamera();
            gameGraphics.ScreenFadeToBlack();
            gameGraphics.ScreenFadeToBlack();



            gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int j2 = 6; j2 >= 1; j2 -= 1)
            {
                gameGraphics.DrawTransparentLine(0, j2, 0, j2, _bgScreenWidth, 8);
            }

            gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int k2 = 6; k2 >= 1; k2 -= 1)
            {
                gameGraphics.DrawTransparentLine(0, k2, 0, 194, _bgScreenWidth, 8);
            }

            if (!DoNotDrawLogo)
            {
                if (bgPixels is null)
                {
                    gameGraphics.DrawPicture(15, 15, baseInventoryPic + 10);
                }
                else
                {
                    gameGraphics.DrawPixels(bgPixels, 0, 0, bgPixels.Length, bgPixels[0].Length);
                }
            }

            gameGraphics.DrawImage(baseInventoryPic + 10, 0, 0, _bgScreenWidth, 200);
            gameGraphics.ApplyImage(baseInventoryPic + 10);

            if (this.OnLoadingSectionCompleted is not null)
            {
                this.OnLoadingSectionCompleted(this, new EventArgs());
            }
        }

        public override void HandlePacket(int packetID, int packetLength, sbyte[] packetData)
        {
            try
            {
                //base.handlePacket(packetID, packetLength, packetData);
                if (packetID == 145)
                {
                    if (!hasWorldInfo)
                    {
                        return;
                    }

                    lastPlayerCount = playerCount;
                    for (int l = 0; l < lastPlayerCount; l += 1)
                    {
                        lastPlayerArray[l] = playerArray[l];
                    }

                    int off = 8;
                    sectionX = DataOperations.GetBits(packetData, off, 11);
                    off += 11;
                    sectionY = DataOperations.GetBits(packetData, off, 13);
                    off += 13;
                    int sprite = DataOperations.GetBits(packetData, off, 4);
                    off += 4;
                    bool sectionLoaded = LoadSection(sectionX, sectionY);
                    sectionX -= areaX;
                    sectionY -= areaY;
                    int mapEnterX = sectionX * gridSize + 64;
                    int mapEnterY = sectionY * gridSize + 64;
                    if (sectionLoaded)
                    {
                        ourPlayer.waypointCurrent = 0;
                        ourPlayer.waypointsEndSprite = 0;
                        ourPlayer.currentX = ourPlayer.waypointsX[0] = mapEnterX;
                        ourPlayer.currentY = ourPlayer.waypointsY[0] = mapEnterY;
                    }
                    playerCount = 0;
                    ourPlayer = CreatePlayer(serverIndex, mapEnterX, mapEnterY, sprite);
                    int newPlayerCount = DataOperations.GetBits(packetData, off, 8);
                    off += 8;
                    for (int currentNewPlayer = 0; currentNewPlayer < newPlayerCount; currentNewPlayer += 1)
                    {
                        //ClientMob mob = lastPlayerArray[currentNewPlayer + 1];
                        ClientMob mob = GetLastPlayer(DataOperations.GetBits(packetData, off, 16));
                        off += 16;
                        int playerAtTile = DataOperations.GetBits(packetData, off, 1);
                        off += 1;
                        if (playerAtTile != 0)
                        {
                            int waypointsLeft = DataOperations.GetBits(packetData, off, 1);
                            off += 1;
                            if (waypointsLeft == 0)
                            {
                                int currentNextSprite = DataOperations.GetBits(packetData, off, 3);
                                off += 3;
                                int currentWaypoint = mob.waypointCurrent;
                                int newWaypointX = mob.waypointsX[currentWaypoint];
                                int newWaypointY = mob.waypointsY[currentWaypoint];
                                if (currentNextSprite == 2 || currentNextSprite == 1 || currentNextSprite == 3)
                                {
                                    newWaypointX += gridSize;
                                }

                                if (currentNextSprite == 6 || currentNextSprite == 5 || currentNextSprite == 7)
                                {
                                    newWaypointX -= gridSize;
                                }

                                if (currentNextSprite == 4 || currentNextSprite == 3 || currentNextSprite == 5)
                                {
                                    newWaypointY += gridSize;
                                }

                                if (currentNextSprite == 0 || currentNextSprite == 1 || currentNextSprite == 7)
                                {
                                    newWaypointY -= gridSize;
                                }

                                mob.nextSprite = currentNextSprite;
                                mob.waypointCurrent = currentWaypoint = (currentWaypoint + 1) % 10;
                                mob.waypointsX[currentWaypoint] = newWaypointX;
                                mob.waypointsY[currentWaypoint] = newWaypointY;
                            }
                            else
                            {
                                int needsNextSprite = DataOperations.GetBits(packetData, off, 4);
                                off += 4;
                                if ((needsNextSprite & 0xc) == 12)
                                {
                                    continue;
                                }
                                mob.nextSprite = needsNextSprite;
                            }
                        }
                        playerArray[playerCount++] = mob;
                    }

                    int mobCount = 0;
                    while (off + 24 < packetLength * 8)
                    {
                        int mobIndex = DataOperations.GetBits(packetData, off, 16);
                        off += 16;
                        int areaMobX = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobX > 15)
                        {
                            areaMobX -= 32;
                        }

                        int areaMobY = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobY > 15)
                        {
                            areaMobY -= 32;
                        }

                        int mobSprite = DataOperations.GetBits(packetData, off, 4);
                        off += 4;
                        int addIndex = DataOperations.GetBits(packetData, off, 1);
                        off += 1;
                        int mobX = (sectionX + areaMobX) * gridSize + 64;
                        int mobY = (sectionY + areaMobY) * gridSize + 64;
                        CreatePlayer(mobIndex, mobX, mobY, mobSprite);
                        if (addIndex == 0)
                        {
                            playerBufferArrayIndexes[mobCount++] = mobIndex;
                        }
                    }
                    if (mobCount > 0)
                    {
                        base.streamClass.CreatePacket(83);
                        base.streamClass.AddShort(mobCount);
                        for (int k40 = 0; k40 < mobCount; k40 += 1)
                        {
                            ClientMob f5 = playerBufferArray[playerBufferArrayIndexes[k40]];
                            base.streamClass.AddShort(f5.serverIndex);
                            base.streamClass.AddShort(f5.serverID);
                        }

                        base.streamClass.FormatPacket();
                        mobCount = 0;
                    }
                    return;
                }
                if (packetID == 109)
                {
                    if (needsClear)
                    {
                        for (int i = 0; i < groundItemID.Length; i += 1)
                        {
                            groundItemX[i] = -1;
                            groundItemY[i] = -1;
                            groundItemID[i] = -1;
                            groundItemObjectVar[i] = -1;
                        }
                        groundItemCount = 0;
                        needsClear = false;
                    }
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = sectionX + packetData[off + 1] >> 3;
                            int newSectionY = sectionY + packetData[off + 2] >> 3;
                            off += 3;
                            for (int groundItem = 0; groundItem < groundItemCount; groundItem += 1)
                            {
                                int newX = (groundItemX[groundItem] >> 3) - newSectionX;
                                int newY = (groundItemY[groundItem] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (groundItem != newCount)
                                    {
                                        groundItemX[newCount] = groundItemX[groundItem];
                                        groundItemY[newCount] = groundItemY[groundItem];
                                        groundItemID[newCount] = groundItemID[groundItem];
                                        groundItemObjectVar[newCount] = groundItemObjectVar[groundItem];
                                    }
                                    newCount += 1;
                                }
                            }

                            groundItemCount = newCount;
                        }
                        else
                        {
                            int newID = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int newX = sectionX + packetData[off++];
                            int newY = sectionY + packetData[off++];
                            if ((newID & 0x8000) == 0)
                            {
                                groundItemX[groundItemCount] = newX;
                                groundItemY[groundItemCount] = newY;
                                groundItemID[groundItemCount] = newID;
                                groundItemObjectVar[groundItemCount] = 0;
                                for (int l23 = 0; l23 < objectCount; l23 += 1)
                                {
                                    if (objectX[l23] != newX || objectY[l23] != newY)
                                    {
                                        continue;
                                    }

                                    groundItemObjectVar[groundItemCount] = Data.GameData.objectGroundItemVar[objectType[l23]];
                                    break;
                                }

                                groundItemCount += 1;
                            }
                            else
                            {
                                newID &= 0x7fff;
                                int updateIndex = 0;
                                for (int currentItemIndex = 0; currentItemIndex < groundItemCount; currentItemIndex += 1)
                                {
                                    if (groundItemX[currentItemIndex] != newX || groundItemY[currentItemIndex] != newY || groundItemID[currentItemIndex] != newID)
                                    {
                                        if (currentItemIndex != updateIndex)
                                        {
                                            groundItemX[updateIndex] = groundItemX[currentItemIndex];
                                            groundItemY[updateIndex] = groundItemY[currentItemIndex];
                                            groundItemID[updateIndex] = groundItemID[currentItemIndex];
                                            groundItemObjectVar[updateIndex] = groundItemObjectVar[currentItemIndex];
                                        }
                                        updateIndex += 1;
                                    }
                                    else
                                    {
                                        newID = -123;
                                    }
                                }

                                groundItemCount = updateIndex;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 27)
                {
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = sectionX + packetData[off + 1] >> 3;
                            int newSectionY = sectionY + packetData[off + 2] >> 3;
                            off += 3;
                            for (int _obj = 0; _obj < objectCount; _obj += 1)
                            {
                                int newX = (objectX[_obj] >> 3) - newSectionX;
                                int newY = (objectY[_obj] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (_obj != newCount)
                                    {
                                        objectArray[newCount] = objectArray[_obj];
                                        objectArray[newCount].index = newCount;
                                        objectX[newCount] = objectX[_obj];
                                        objectY[newCount] = objectY[_obj];
                                        objectType[newCount] = objectType[_obj];
                                        objectRotation[newCount] = objectRotation[_obj];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(objectArray[_obj]);
                                    engineHandle.RemoveObject(objectX[_obj], objectY[_obj], objectType[_obj], objectRotation[_obj]);
                                }
                            }

                            objectCount = newCount;
                        }
                        else
                        {
                            int index = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int newSectionX = sectionX + packetData[off++];
                            int newSectionY = sectionY + packetData[off++];
                            int rotation = packetData[off++];
                            int newCount = 0;
                            for (int _obj = 0; _obj < objectCount; _obj += 1)
                            {
                                if (objectX[_obj] != newSectionX || objectY[_obj] != newSectionY || objectRotation[_obj] != rotation)
                                {
                                    if (_obj != newCount)
                                    {
                                        objectArray[newCount] = objectArray[_obj];
                                        objectArray[newCount].index = newCount;
                                        objectX[newCount] = objectX[_obj];
                                        objectY[newCount] = objectY[_obj];
                                        objectType[newCount] = objectType[_obj];
                                        objectRotation[newCount] = objectRotation[_obj];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(objectArray[_obj]);
                                    engineHandle.RemoveObject(objectX[_obj], objectY[_obj], objectType[_obj], objectRotation[_obj]);
                                }
                            }

                            objectCount = newCount;
                            if (index != 60000)
                            {
                                engineHandle.RegisterObjectDir(newSectionX, newSectionY, rotation);
                                int width;
                                int height;
                                if (rotation == 0 || rotation == 4)
                                {
                                    width = Data.GameData.objectWidth[index];
                                    height = Data.GameData.objectHeight[index];
                                }
                                else
                                {
                                    height = Data.GameData.objectWidth[index];
                                    width = Data.GameData.objectHeight[index];
                                }
                                int l40 = ((newSectionX + newSectionX + width) * gridSize) / 2;
                                int k42 = ((newSectionY + newSectionY + height) * gridSize) / 2;
                                int model = Data.GameData.objectModelNumber[index];
                                GameObject gameObject = gameDataObjects[model].CreateParent();
#warning object not being added to camera.
                                gameCamera.AddModel(gameObject);

                                gameObject.index = objectCount;
                                gameObject.OffsetMiniPosition(0, rotation * 32, 0);
                                gameObject.OffsetPosition(l40, -engineHandle.GetAveragedElevation(l40, k42), k42);
                                gameObject.UpdateShading(true, 48, 48, -50, -10, -50);
                                engineHandle.CreateObject(newSectionX, newSectionY, index, rotation);
                                if (index == 74)
                                {
                                    gameObject.OffsetPosition(0, -480, 0);
                                }

                                objectX[objectCount] = newSectionX;
                                objectY[objectCount] = newSectionY;
                                objectType[objectCount] = index;
                                objectRotation[objectCount] = rotation;
                                objectArray[objectCount++] = gameObject;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 114)
                {
                    int off = 1;
                    inventoryItemsCount = packetData[off++] & 0xff;
                    for (int item = 0; item < inventoryItemsCount; item += 1)
                    {
                        int data = DataOperations.GetShort(packetData, off);
                        off += 2;
                        inventoryItems[item] = data & 0x7fff;
                        inventoryItemEquipped[item] = data / 32768;
                        if (Data.GameData.itemStackable[data & 0x7fff] == 0)
                        {
                            inventoryItemCount[item] = DataOperations.GetInt(packetData, off);
                            off += 4;
                        }
                        else
                        {
                            inventoryItemCount[item] = 1;
                        }
                    }

                    return;
                }
                if (packetID == 53)
                {
                    int newMobCount = DataOperations.GetShort(packetData, 1);
                    int off = 3;
                    for (int current = 0; current < newMobCount; current += 1)
                    {
                        int index = DataOperations.GetShort(packetData, off);
                        off += 2;
                        if (index < 0 || index > playerBufferArray.Length)
                        {
                            return;
                        }

                        ClientMob mob = playerBufferArray[index];
                        if (mob is null)
                        {
                            return;
                        }

                        sbyte mobUpdateType = packetData[off];
                        off += 1;
                        if (mobUpdateType == 0)
                        {
                            int j30 = DataOperations.GetShort(packetData, off);
                            off += 2;

                            mob.playerSkullTimeout = 150;
                            mob.itemAboveHeadID = j30;

                        }
                        else if (mobUpdateType == 1)
                        {
                            sbyte byte7 = packetData[off];
                            off += 1;
                            string s3 = ChatMessage.BytesToString(packetData, off, byte7);
                            //if (useChatFilter)
                            //    s3 = ChatFilter.filterChat(s3);
                            bool ignore = false;
                            for (int i41 = 0; i41 < base.ignoresCount; i41 += 1)
                            {
                                if (base.ignoresList[i41] == mob.nameHash)
                                {
                                    ignore = true;
                                }
                            }

                            if (!ignore)
                            {
                                mob.lastMessageTimeout = 150;
                                mob.lastMessage = s3;
                                DisplayMessage(mob.username + ": " + mob.lastMessage, 2);
                            }
                            off += byte7;
                        }
                        else if (mobUpdateType == 2)
                        {
                            int lastDamageCount = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            int currentHits = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            int baseHits = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            mob.lastDamageCount = lastDamageCount;
                            mob.currentHits = currentHits;
                            mob.baseHits = baseHits;
                            mob.combatTimer = 200;
                            if (mob == ourPlayer)
                            {
                                playerStatCurrent[3] = currentHits;
                                playerStatBase[3] = baseHits;
                                showWelcomeBox = false;
                                showServerMessageBox = false;
                            }
                        }
                        else if (mobUpdateType == 3)
                        {
                            int l30 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int l34 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            mob.projectileType = l30;
                            mob.attackingNpcIndex = l34;
                            mob.attackingPlayerIndex = -1;
                            mob.projectileDistance = projectileRange;
                        }
                        else if (mobUpdateType == 4)
                        {
                            int i31 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int i35 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            mob.projectileType = i31;
                            mob.attackingPlayerIndex = i35;
                            mob.attackingNpcIndex = -1;
                            mob.projectileDistance = projectileRange;
                        }
                        else if (mobUpdateType == 5)
                        {
                            mob.serverID = DataOperations.GetShort(packetData, off);
                            off += 2;
                            mob.nameHash = DataOperations.GetLong(packetData, off);
                            off += 8;
                            mob.username = DataOperations.HashToName(mob.nameHash);
                            int appearanceCount = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            for (int j35 = 0; j35 < appearanceCount; j35 += 1)
                            {
                                mob.appearanceItems[j35] = DataOperations.GetByte(packetData[off]);
                                off += 1;
                            }

                            for (int j38 = appearanceCount; j38 < 12; j38 += 1)
                            {
                                mob.appearanceItems[j38] = 0;
                            }

                            mob.hairColour = packetData[off++] & 0xff;
                            mob.topColour = packetData[off++] & 0xff;
                            mob.bottomColour = packetData[off++] & 0xff;
                            mob.skinColour = packetData[off++] & 0xff;
                            mob.level = packetData[off++] & 0xff;
                            mob.playerSkulled = packetData[off++] & 0xff;
                            off += 1;// TODO to skip the admin flag (should it be removed)
                        }
                        else if (mobUpdateType == 6)
                        {
                            sbyte byte8 = packetData[off];
                            off += 1;
                            string s4 = ChatMessage.BytesToString(packetData, off, byte8);
                            mob.lastMessageTimeout = 150;
                            mob.lastMessage = s4;
                            if (mob == ourPlayer)
                            {
                                DisplayMessage(mob.username + ": " + mob.lastMessage, 5);
                            }

                            off += byte8;
                        }
                    }

                    return;
                }
                if (packetID == 95)
                {
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = sectionX + packetData[off + 1] >> 3;
                            int newSectionY = sectionY + packetData[off + 2] >> 3;
                            off += 3;
                            for (int current = 0; current < wallObjectCount; current += 1)
                            {
                                int newX = (wallObjectX[current] >> 3) - newSectionX;
                                int newY = (wallObjectY[current] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (current != newCount)
                                    {
                                        wallObjectArray[newCount] = wallObjectArray[current];
                                        wallObjectArray[newCount].index = newCount + 10000;
                                        wallObjectX[newCount] = wallObjectX[current];
                                        wallObjectY[newCount] = wallObjectY[current];
                                        wallObjectDirection[newCount] = wallObjectDirection[current];
                                        wallObjectID[newCount] = wallObjectID[current];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(wallObjectArray[current]);
                                    engineHandle.RemoveWallObject(wallObjectX[current], wallObjectY[current], wallObjectDirection[current], wallObjectID[current]);
                                }
                            }

                            wallObjectCount = newCount;
                        }
                        else
                        {
                            int newID = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int newSectionX = sectionX + packetData[off++];
                            int newSectionY = sectionY + packetData[off++];
                            sbyte direction = packetData[off++];
                            int newCount = 0;
                            for (int current = 0; current < wallObjectCount; current += 1)
                            {
                                if (wallObjectX[current] != newSectionX || wallObjectY[current] != newSectionY || wallObjectDirection[current] != direction)
                                {
                                    if (current != newCount)
                                    {
                                        wallObjectArray[newCount] = wallObjectArray[current];
                                        wallObjectArray[newCount].index = newCount + 10000;
                                        wallObjectX[newCount] = wallObjectX[current];
                                        wallObjectY[newCount] = wallObjectY[current];
                                        wallObjectDirection[newCount] = wallObjectDirection[current];
                                        wallObjectID[newCount] = wallObjectID[current];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(wallObjectArray[current]);
                                    engineHandle.RemoveWallObject(wallObjectX[current], wallObjectY[current], wallObjectDirection[current], wallObjectID[current]);
                                }
                            }

                            wallObjectCount = newCount;
                            if (newID != 60000)
                            {
                                engineHandle.CreateWall(newSectionX, newSectionY, direction, newID);
                                GameObject k35 = CreateWallObject(newSectionX, newSectionY, direction, newID, wallObjectCount);
                                wallObjectArray[wallObjectCount] = k35;
                                wallObjectX[wallObjectCount] = newSectionX;
                                wallObjectY[wallObjectCount] = newSectionY;
                                wallObjectID[wallObjectCount] = newID;
                                wallObjectDirection[wallObjectCount++] = direction;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 77)
                {
                    lastNpcCount = npcCount;
                    npcCount = 0;
                    for (int j2 = 0; j2 < lastNpcCount; j2 += 1)
                    {
                        lastNpcArray[j2] = npcArray[j2];
                    }

                    int off = 8;
                    int newCount = DataOperations.GetBits(packetData, off, 8);
                    off += 8;
                    for (int current = 0; current < newCount; current += 1)
                    {
                        ClientMob newNpc = GetLastNpc(DataOperations.GetBits(packetData, off, 16));
                        off += 16;
                        int needsUpdate = DataOperations.GetBits(packetData, off, 1);
                        off += 1;
                        if (needsUpdate != 0)
                        {
                            int j32 = DataOperations.GetBits(packetData, off, 1);
                            off += 1;
                            if (j32 == 0)
                            {
                                int nextSprite = DataOperations.GetBits(packetData, off, 3);
                                off += 3;
                                int waypointCurrent = newNpc.waypointCurrent;
                                int waypointX = newNpc.waypointsX[waypointCurrent];
                                int waypointY = newNpc.waypointsY[waypointCurrent];
                                if (nextSprite == 2 || nextSprite == 1 || nextSprite == 3)
                                {
                                    waypointX += gridSize;
                                }

                                if (nextSprite == 6 || nextSprite == 5 || nextSprite == 7)
                                {
                                    waypointX -= gridSize;
                                }

                                if (nextSprite == 4 || nextSprite == 3 || nextSprite == 5)
                                {
                                    waypointY += gridSize;
                                }

                                if (nextSprite == 0 || nextSprite == 1 || nextSprite == 7)
                                {
                                    waypointY -= gridSize;
                                }

                                newNpc.nextSprite = nextSprite;
                                newNpc.waypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                                newNpc.waypointsX[waypointCurrent] = waypointX;
                                newNpc.waypointsY[waypointCurrent] = waypointY;
                            }
                            else
                            {
                                int nextSprite = DataOperations.GetBits(packetData, off, 4);
                                off += 4;
                                if ((nextSprite & 0xc) == 12)
                                {
                                    continue;
                                }
                                newNpc.nextSprite = nextSprite;
                            }
                        }
                        npcArray[npcCount++] = newNpc;
                    }

                    while (off + 34 < packetLength * 8)
                    {
                        int mobIndex = DataOperations.GetBits(packetData, off, 16);
                        off += 16;
                        int areaMobX = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobX > 15)
                        {
                            areaMobX -= 32;
                        }

                        int areaMobY = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobY > 15)
                        {
                            areaMobY -= 32;
                        }

                        int mobSprite = DataOperations.GetBits(packetData, off, 4);
                        off += 4;
                        int mobX = (sectionX + areaMobX) * gridSize + 64;
                        int mobY = (sectionY + areaMobY) * gridSize + 64;
                        int addIndex = DataOperations.GetBits(packetData, off, 10);
                        off += 10;
                        if (addIndex >= Data.GameData.npcCount)
                        {
                            addIndex = 24;
                        }

                        CreateNpc(mobIndex, mobX, mobY, mobSprite, addIndex);
                    }
                    return;
                }
                if (packetID == 190)
                {
                    int newCount = DataOperations.GetShort(packetData, 1);
                    int off = 3;
                    for (int l16 = 0; l16 < newCount; l16 += 1)
                    {
                        int npcIndex = DataOperations.GetShort(packetData, off);
                        off += 2;
                        ClientMob mob = npcAttackingArray[npcIndex];
                        int updateType = DataOperations.GetByte(packetData[off]);
                        off += 1;
                        if (updateType == 1)
                        {
                            int playerIndex = DataOperations.GetShort(packetData, off);
                            off += 2;
                            sbyte messageLength = packetData[off];
                            off += 1;
                            if (mob is not null)
                            {
                                string s5 = ChatMessage.BytesToString(packetData, off, messageLength);
                                mob.lastMessageTimeout = 150;
                                mob.lastMessage = s5;
                                if (playerIndex == ourPlayer.serverIndex)
                                {
                                    DisplayMessage("@yel@" + Data.GameData.npcName[mob.npcId] + ": " + mob.lastMessage, 5);
                                }
                            }
                            off += messageLength;
                        }
                        else
                            if (updateType == 2)
                            {
                                int lastDamageCount = DataOperations.GetByte(packetData[off]);
                                off += 1;
                                int currentHits = DataOperations.GetByte(packetData[off]);
                                off += 1;
                                int baseHits = DataOperations.GetByte(packetData[off]);
                                off += 1;
                                if (mob is not null)
                                {
                                    mob.lastDamageCount = lastDamageCount;
                                    mob.currentHits = currentHits;
                                    mob.baseHits = baseHits;
                                    mob.combatTimer = 200;
                                }
                            }
                    }

                    return;
                }
                if (packetID == 223)
                {
                    showQuestionMenu = true;
                    int count = DataOperations.GetByte(packetData[1]);
                    questionMenuCount = count;
                    int off = 2;
                    for (int index = 0; index < count; index += 1)
                    {
                        int optionLength = DataOperations.GetByte(packetData[off]);
                        off += 1;
                        questionMenuAnswer[index] = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), off, optionLength);
                        off += optionLength;
                    }

                    return;
                }
                if (packetID == 127)
                {
                    showQuestionMenu = false;
                    return;
                }
                if (packetID == 131)
                {
                    loadArea = true;
                    serverIndex = DataOperations.GetShort(packetData, 1);
                    wildX = DataOperations.GetShort(packetData, 3);
                    wildY = DataOperations.GetShort(packetData, 5);
                    layerIndex = DataOperations.GetShort(packetData, 7);
                    layerModifier = DataOperations.GetShort(packetData, 9);
                    wildY -= layerIndex * layerModifier;
                    needsClear = true;
                    hasWorldInfo = true;
                    return;
                }
                if (packetID == 180)
                {
                    int off = 1;
                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        playerStatCurrent[stat] = DataOperations.GetByte(packetData[off++]);
                    }

                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        playerStatBase[stat] = DataOperations.GetByte(packetData[off++]);
                    }

                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        playerStatExp[stat] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }
                    return;
                }
                if (packetID == 177)
                {
                    int off = 1;
                    for (int j3 = 0; j3 < 5; j3 += 1)
                    {
                        equipmentStatus[j3] = DataOperations.GetSignedShort(packetData, off);
                        off += 2;
                    }
                    return;
                }
                if (packetID == 165)
                {
                    playerAliveTimeout = 250;
                    return;
                }
                if (packetID == 115)
                {
                    int k3 = (packetLength - 1) / 4;
                    for (int i11 = 0; i11 < k3; i11 += 1)
                    {
                        int k17 = sectionX + DataOperations.GetSignedShort(packetData, 1 + i11 * 4) >> 3;
                        int i22 = sectionY + DataOperations.GetSignedShort(packetData, 3 + i11 * 4) >> 3;
                        int j25 = 0;
                        for (int l28 = 0; l28 < groundItemCount; l28 += 1)
                        {
                            int j33 = (groundItemX[l28] >> 3) - k17;
                            int l36 = (groundItemY[l28] >> 3) - i22;
                            if (j33 != 0 || l36 != 0)
                            {
                                if (l28 != j25)
                                {
                                    groundItemX[j25] = groundItemX[l28];
                                    groundItemY[j25] = groundItemY[l28];
                                    groundItemID[j25] = groundItemID[l28];
                                    groundItemObjectVar[j25] = groundItemObjectVar[l28];
                                }
                                j25 += 1;
                            }
                        }

                        groundItemCount = j25;
                        j25 = 0;
                        for (int k33 = 0; k33 < objectCount; k33 += 1)
                        {
                            int i37 = (objectX[k33] >> 3) - k17;
                            int j39 = (objectY[k33] >> 3) - i22;
                            if (i37 != 0 || j39 != 0)
                            {
                                if (k33 != j25)
                                {
                                    objectArray[j25] = objectArray[k33];
                                    objectArray[j25].index = j25;
                                    objectX[j25] = objectX[k33];
                                    objectY[j25] = objectY[k33];
                                    objectType[j25] = objectType[k33];
                                    objectRotation[j25] = objectRotation[k33];
                                }
                                j25 += 1;
                            }
                            else
                            {
                                gameCamera.RemoveModel(objectArray[k33]);
                                engineHandle.RemoveObject(objectX[k33], objectY[k33], objectType[k33], objectRotation[k33]);
                            }
                        }

                        objectCount = j25;
                        j25 = 0;
                        for (int j37 = 0; j37 < wallObjectCount; j37 += 1)
                        {
                            int k39 = (wallObjectX[j37] >> 3) - k17;
                            int l41 = (wallObjectY[j37] >> 3) - i22;
                            if (k39 != 0 || l41 != 0)
                            {
                                if (j37 != j25)
                                {
                                    wallObjectArray[j25] = wallObjectArray[j37];
                                    wallObjectArray[j25].index = j25 + 10000;
                                    wallObjectX[j25] = wallObjectX[j37];
                                    wallObjectY[j25] = wallObjectY[j37];
                                    wallObjectDirection[j25] = wallObjectDirection[j37];
                                    wallObjectID[j25] = wallObjectID[j37];
                                }
                                j25 += 1;
                            }
                            else
                            {
                                gameCamera.RemoveModel(wallObjectArray[j37]);
                                engineHandle.RemoveWallObject(wallObjectX[j37], wallObjectY[j37], wallObjectDirection[j37], wallObjectID[j37]);
                            }
                        }

                        wallObjectCount = j25;
                    }

                    return;
                }
                if (packetID == 207)
                {
                    showAppearanceWindow = true;
                    return;
                }
                if (packetID == 4)
                {
                    int tradeOther = DataOperations.GetShort(packetData, 1);
                    if (playerBufferArray[tradeOther] is not null)
                    {
                        tradeOtherName = playerBufferArray[tradeOther].username;
                    }

                    showTradeBox = true;
                    tradeOtherAccepted = false;
                    tradeWeAccepted = false;
                    tradeItemsOurCount = 0;
                    tradeItemsOtherCount = 0;
                    return;
                }
                if (packetID == 187)
                {
                    showTradeBox = false;
                    showTradeConfirmBox = false;
                    return;
                }
                if (packetID == 250)
                {
                    tradeItemsOtherCount = packetData[1] & 0xff;
                    int i4 = 2;
                    for (int j11 = 0; j11 < tradeItemsOtherCount; j11 += 1)
                    {
                        tradeItemsOther[j11] = DataOperations.GetShort(packetData, i4);
                        i4 += 2;
                        tradeItemOtherCount[j11] = DataOperations.GetInt(packetData, i4);
                        i4 += 4;
                    }

                    tradeOtherAccepted = false;
                    tradeWeAccepted = false;
                    return;
                }
                if (packetID == 92)
                {
                    sbyte byte0 = packetData[1];
                    if (byte0 == 1)
                    {
                        tradeOtherAccepted = true;
                        return;
                    }
                    else
                    {
                        tradeOtherAccepted = false;
                        return;
                    }
                }
                if (packetID == 253)
                {
                    showShopBox = true;
                    int off = 1;
                    int newShopItemCount = packetData[off++] & 0xff;
                    sbyte isGeneralShop = packetData[off++];
                    shopItemSellPriceModifier = packetData[off++] & 0xff;
                    shopItemBuyPriceModifier = packetData[off++] & 0xff;
                    for (int j22 = 0; j22 < 40; j22 += 1)
                    {
                        shopItems[j22] = -1;
                    }

                    for (int item = 0; item < newShopItemCount; item += 1)
                    {
                        shopItems[item] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        shopItemCount[item] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        shopItemBuyPrice[item] = DataOperations.GetInt(packetData, off);
                        off += 4;
                        shopItemSellPrice[item] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    if (isGeneralShop == 1)
                    {
                        int i29 = 39;
                        for (int l33 = 0; l33 < inventoryItemsCount; l33 += 1)
                        {
                            if (i29 < newShopItemCount)
                            {
                                break;
                            }

                            bool flag2 = false;
                            for (int l39 = 0; l39 < 40; l39 += 1)
                            {
                                if (shopItems[l39] != inventoryItems[l33])
                                {
                                    continue;
                                }

                                flag2 = true;
                                break;
                            }

                            if (inventoryItems[l33] == 10)
                            {
                                flag2 = true;
                            }

                            if (!flag2)
                            {
                                shopItems[i29] = inventoryItems[l33] & 0x7fff;
                                shopItemCount[i29] = 0;
                                shopItemSellPrice[i29] = Data.GameData.itemBasePrice[shopItems[i29]] - (int)(Data.GameData.itemBasePrice[shopItems[i29]] / 2.5);
                                shopItemSellPrice[i29] -= (int)(shopItemSellPrice[i29] * 0.10);
                                i29 -= 1;
                            }
                        }

                    }
                    if (selectedShopItemIndex >= 0 && selectedShopItemIndex < 40 && shopItems[selectedShopItemIndex] != selectedShopItemType)
                    {
                        selectedShopItemIndex = -1;
                        selectedShopItemType = -2;
                    }
                    return;
                }
                if (packetID == 220)
                {
                    showShopBox = false;
                    return;
                }
                if (packetID == 18)
                {
                    sbyte byte1 = packetData[1];
                    if (byte1 == 1)
                    {
                        tradeWeAccepted = true;
                        return;
                    }
                    else
                    {
                        tradeWeAccepted = false;
                        return;
                    }
                }
                if (packetID == 152)
                {
                    configCameraAutoAngle = DataOperations.GetByte(packetData[1]) == 1;
                    configOneMouseButton = DataOperations.GetByte(packetData[2]) == 1;
                    configSoundOff = DataOperations.GetByte(packetData[3]) == 1;
                    showRoofs = DataOperations.GetByte(packetData[4]) == 1;
                    autoScreenshot = DataOperations.GetByte(packetData[5]) == 1;
                    showCombatWindow = DataOperations.GetByte(packetData[6]) == 1;
                    return;
                }
                if (packetID == 209)
                {
                    for (int k4 = 0; k4 < packetLength - 1; k4 += 1)
                    {
                        bool flag = packetData[k4 + 1] == 1;
                        if (!prayerOn[k4] && flag)
                        {
                            PlaySound("prayeron");
                        }

                        if (prayerOn[k4] && !flag)
                        {
                            PlaySound("prayeroff");
                        }

                        prayerOn[k4] = flag;
                    }

                    return;
                }
                if (packetID == 93)
                {
                    showBankBox = true;
                    int off = 1;
                    serverBankItemsCount = packetData[off++] & 0xff;
                    maxBankItems = packetData[off++] & 0xff;
                    for (int l11 = 0; l11 < serverBankItemsCount; l11 += 1)
                    {
                        serverBankItems[l11] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        serverBankItemCount[l11] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    UpdateBankItems();
                    return;
                }
                if (packetID == 171)
                {
                    showBankBox = false;
                    return;
                }
                if (packetID == 211)
                {
                    int j5 = packetData[1] & 0xff;
                    playerStatExp[j5] = DataOperations.GetInt(packetData, 2);
                    return;
                }
                if (packetID == 229)
                {
                    int k5 = DataOperations.GetShort(packetData, 1);
                    if (playerBufferArray[k5] is not null)
                    {
                        duelOpponent = playerBufferArray[k5].username;
                    }

                    showDuelBox = true;
                    duelMyItemCount = 0;
                    duelOpponentItemCount = 0;
                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    duelNoRetreating = false;
                    duelNoMagic = false;
                    duelNoPrayer = false;
                    duelNoWeapons = false;
                    return;
                }
                if (packetID == 160)
                {
                    showDuelBox = false;
                    showDuelConfirmBox = false;
                    return;
                }

#warning have not fixed the following yet....
                if (packetID == 251)
                {
                    showTradeConfirmBox = true;
                    tradeConfirmAccepted = false;
                    showTradeBox = false;
                    int off = 1;
                    tradeConfirmOtherNameLong = DataOperations.GetLong(packetData, off);
                    off += 8;
                    tradeConfirmOtherItemCount = packetData[off++] & 0xff;
                    for (int i12 = 0; i12 < tradeConfirmOtherItemCount; i12 += 1)
                    {
                        tradeConfirmOtherItems[i12] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        tradeConfirmOtherItemsCount[i12] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    tradeConfigItemCount = packetData[off++] & 0xff;
                    for (int l17 = 0; l17 < tradeConfigItemCount; l17 += 1)
                    {
                        tradeConfirmItems[l17] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        tradeConfigItemsCount[l17] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    return;
                }
                if (packetID == 63)
                {
                    duelOpponentItemCount = packetData[1] & 0xff;
                    int off = 2;
                    for (int j12 = 0; j12 < duelOpponentItemCount; j12 += 1)
                    {
                        duelOpponentItems[j12] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        duelOpponentItemsCount[j12] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    return;
                }
                if (packetID == 198)
                {
                    if (packetData[1] == 1)
                    {
                        duelNoRetreating = true;
                    }
                    else
                    {
                        duelNoRetreating = false;
                    }

                    if (packetData[2] == 1)
                    {
                        duelNoMagic = true;
                    }
                    else
                    {
                        duelNoMagic = false;
                    }

                    if (packetData[3] == 1)
                    {
                        duelNoPrayer = true;
                    }
                    else
                    {
                        duelNoPrayer = false;
                    }

                    if (packetData[4] == 1)
                    {
                        duelNoWeapons = true;
                    }
                    else
                    {
                        duelNoWeapons = false;
                    }

                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    return;
                }
                if (packetID == 139)
                {
                    int off = 1;
                    int itemSlot = packetData[off++] & 0xff;
                    int itemID = DataOperations.GetShort(packetData, off);
                    off += 2;
                    int itemCount = DataOperations.GetInt(packetData, off);
                    off += 4;
                    if (itemCount == 0)
                    {
                        serverBankItemsCount -= 1;
                        for (int l25 = itemSlot; l25 < serverBankItemsCount; l25 += 1)
                        {
                            serverBankItems[l25] = serverBankItems[l25 + 1];
                            serverBankItemCount[l25] = serverBankItemCount[l25 + 1];
                        }

                    }
                    else
                    {
                        serverBankItems[itemSlot] = itemID;
                        serverBankItemCount[itemSlot] = itemCount;
                        if (itemSlot >= serverBankItemsCount)
                        {
                            serverBankItemsCount = itemSlot + 1;
                        }
                    }
                    UpdateBankItems();
                    return;
                }
                if (packetID == 228)
                {
                    int off = 1;
                    int count = 1;
                    int newCount = packetData[off++] & 0xff;
                    int data = DataOperations.GetShort(packetData, off);
                    off += 2;
                    if (Data.GameData.itemStackable[data & 0x7fff] == 0)
                    {
                        count = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }
                    inventoryItems[newCount] = data & 0x7fff;
                    inventoryItemEquipped[newCount] = data / 32768;
                    inventoryItemCount[newCount] = count;
                    if (newCount >= inventoryItemsCount)
                    {
                        inventoryItemsCount = newCount + 1;
                    }

                    return;
                }
                if (packetID == 191)
                {
                    int l6 = packetData[1] & 0xff;
                    inventoryItemsCount -= 1;
                    for (int i13 = l6; i13 < inventoryItemsCount; i13 += 1)
                    {
                        inventoryItems[i13] = inventoryItems[i13 + 1];
                        inventoryItemCount[i13] = inventoryItemCount[i13 + 1];
                        inventoryItemEquipped[i13] = inventoryItemEquipped[i13 + 1];
                    }

                    return;
                }
                if (packetID == 208)
                {
                    int off = 1;
                    int stat = packetData[off++] & 0xff;
                    playerStatCurrent[stat] = DataOperations.GetByte(packetData[off++]);
                    playerStatBase[stat] = DataOperations.GetByte(packetData[off++]);
                    playerStatExp[stat] = DataOperations.GetInt(packetData, off);
                    off += 4;
                    return;
                }
                if (packetID == 65)
                {
                    sbyte byte2 = packetData[1];
                    if (byte2 == 1)
                    {
                        duelOpponentAccepted = true;
                        return;
                    }
                    else
                    {
                        duelOpponentAccepted = false;
                        return;
                    }
                }
                if (packetID == 197)
                {
                    sbyte byte3 = packetData[1];
                    if (byte3 == 1)
                    {
                        duelMyAccepted = true;
                        return;
                    }
                    else
                    {
                        duelMyAccepted = false;
                        return;
                    }
                }
                if (packetID == 147)
                {
                    showDuelConfirmBox = true;
                    duelConfirmOurAccepted = false;
                    showDuelBox = false;
                    int off = 1;
                    duelOpponentHash = DataOperations.GetLong(packetData, off);
                    off += 8;
                    duelOpponentStakeCount = packetData[off++] & 0xff;
                    for (int k13 = 0; k13 < duelOpponentStakeCount; k13 += 1)
                    {
                        duelOpponentStakeItem[k13] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        duelOutStakeItemCount[k13] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    duelOurStakeCount = packetData[off++] & 0xff;
                    for (int k18 = 0; k18 < duelOurStakeCount; k18 += 1)
                    {
                        duelOurStakeItem[k18] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        duelOurStakeItemCount[k18] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    duelRetreat = packetData[off++] & 0xff;
                    duelMagic = packetData[off++] & 0xff;
                    duelPrayer = packetData[off++] & 0xff;
                    duelWeapons = packetData[off++] & 0xff;
                    return;
                }
                if (packetID == 11)
                {
                    string s1 = new(packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    PlaySound(s1);
                    return;
                }
                if (packetID == 23)
                {
                    if (teleBubbleCount < 50)
                    {
                        int type = packetData[1] & 0xff;
                        int x = packetData[2] + sectionX;
                        int y = packetData[3] + sectionY;
                        teleBubbleType[teleBubbleCount] = type;
                        teleBubbleTime[teleBubbleCount] = 0;
                        teleBubbleX[teleBubbleCount] = x;
                        teleBubbleY[teleBubbleCount] = y;
                        teleBubbleCount += 1;
                    }
                    return;
                }
                if (packetID == 248)
                {
                    if (!loginScreenShown)
                    {
                        lastLoginDays = DataOperations.GetShort(packetData, 1);
                        subDaysLeft = DataOperations.GetShort(packetData, 3);
                        lastLoginAddress = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), 5, packetLength - 5);
                        showWelcomeBox = true;
                        loginScreenShown = true;
                    }
                    return;
                }
                if (packetID == 148)
                {
                    serverMessage = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    showServerMessageBox = true;
                    serverMessageBoxTop = false;
                    return;
                }
                if (packetID == 64)
                {
                    serverMessage = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    showServerMessageBox = true;
                    serverMessageBoxTop = true;
                    return;
                }
                if (packetID == 126)
                {
                    fatigue = DataOperations.GetShort(packetData, 1);
                    return;
                }
                if (packetID == 206)
                {
                    killingSpree = DataOperations.GetShort(packetData, 1);
                    return;
                }
                if (packetID == 224)
                {
                    isSleeping = false;
                    return;
                }
                if (packetID == 225)
                {
                    sleepingStatusText = "Incorrect - Please wait...";
                    return;
                }
                if (packetID == 172)
                {
                    systemUpdate = DataOperations.GetShort(packetData, 1) * 32;
                    return;
                }
                if (packetID == 181)
                {
                    if (autoScreenshot)
                    {
                        TakeScreenshot(false);
                    }

                    return;
                }
                if (packetID == 182)
                {
                    int off = 1;
                    questPoints = DataOperations.GetShort(packetData, off);
                    off += 2;
                    for (int l4 = 0; l4 < questName.Length; l4 += 1)
                    {
                        questStage[l4] = packetData[l4 + 1];
                    }

                    return;
                }
                if (packetID == 233)
                {
                    questPoints = DataOperations.GetByte(packetData[1]);
                    int count = DataOperations.GetByte(packetData[2]);
                    int off = 3;
                    string[] newQuestNames = new string[count];
                    int[] newQuestStage = new int[count];
                    for (int i = 0; i < count; i += 1)
                    {
                        newQuestNames[i] = questName[DataOperations.GetByte(packetData[off++])];
                        newQuestStage[i] = DataOperations.GetByte(packetData[off++]);
                    }
                    usedQuestName = newQuestNames;
                    questStage = newQuestStage;
                    return;
                }
                if (packetID == 129)
                {
                    combatStyle = DataOperations.GetByte(packetData[1]);
                    return;
                }
                if (packetID == 110)
                {// TODO remove?
                    Console.WriteLine("RECEIVED PACKET 110 (SERVER INFO)");
                    return;
                }
                // Spell counts and quest progress — 2-byte value packets, silently accepted
                if (packetID == 210 || packetID == 212 || packetID == 213) { return; } // Guthix/Zamorak/Saradomin spells
                if (packetID == 128) { return; } // QuestPointsChange
                if (packetID == 134) { return; } // Deaths
                if (packetID == 137) { return; } // DruidicRitual
                if (packetID == 138) { return; } // ImpCatcher
                if (packetID == 141) { return; } // SheepShearer
                if (packetID == 143) { return; } // DoricQuest
                if (packetID == 132) { return; } // Kills
                if (packetID == 140) { return; } // RomeoAndJuliet
                if (packetID == 142) { return; } // WitchPotion
                if (packetID == 144) { return; } // CookAssistant
                if (packetID == 133) { return; } // PirateTreasure
                if (packetID == 139) { return; } // BlackKnightsForte
                if (packetID == 146) { return; } // RestlessGhost
                if (packetID == 149) { return; } // ErnestTheChicken
                if (packetID == 150) { return; } // Goblintown
                if (packetID == 151) { return; } // MinersBlazonQuest
                if (packetID == 173) { return; } // VampireSlayer
                if (packetID == 174) { return; } // MilleniumItemQuest
                if (packetID == 175) { return; } // KnightsSword
                if (packetID == 202) { return; } // GoblinDiplomacy
                if (packetID == 203) { return; } // TheGrandTree
                if (packetID == 204) { return; } // FightArena
                if (packetID == 205) { return; } // Hazeel
                if (packetID == 214) { return; } // HolyGrail
                if (packetID == 215) { return; } // MerlinsCrystal
                if (packetID == 216) { return; } // LostCity
                if (packetID == 217) { return; } // WitchHouse
                Console.WriteLine("UNHANDLED PACKET:" + packetID + " LEN:" + packetLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // ex.printStackTrace();
            }
        }

        //protected void startThread(Runnable runnable) {
        //    if(Link.gameApplet is not null) {
        //        Link.thread(runnable);
        //        return;
        //    } else {
        //        Thread thread = new Thread(runnable);
        //        thread.setDaemon(true);
        //        thread.start();
        //        return;
        //    }
        //}

        public override void InitVars()
        {
            systemUpdate = 0;
            combatStyle = 0;
            logoutTimer = 0;
            loginScreen = 0;
            loggedIn = true;
            ResetPrivateMessages();
            gameGraphics.ClearScreen();
            // gameGraphics.UpdateGameImage();
            //gameGraphics.DrawImage(spriteBatch, 0, 0);
            OnDrawDone();

            for (int l = 0; l < objectCount; l += 1)
            {
                gameCamera.RemoveModel(objectArray[l]);
                engineHandle.RemoveObject(objectX[l], objectY[l], objectType[l], objectRotation[l]);
            }

            for (int i1 = 0; i1 < wallObjectCount; i1 += 1)
            {
                gameCamera.RemoveModel(wallObjectArray[i1]);
                engineHandle.RemoveWallObject(wallObjectX[i1], wallObjectY[i1], wallObjectDirection[i1], wallObjectID[i1]);
            }

            objectCount = 0;
            wallObjectCount = 0;
            groundItemCount = 0;
            playerCount = 0;
            for (int j1 = 0; j1 < 4000; j1 += 1)
            {
                playerBufferArray[j1] = null;
            }

            for (int k1 = 0; k1 < 500; k1 += 1)
            {
                playerArray[k1] = null;
            }

            npcCount = 0;
            for (int l1 = 0; l1 < 5000; l1 += 1)
            {
                npcAttackingArray[l1] = null;
            }

            for (int i2 = 0; i2 < 500; i2 += 1)
            {
                npcArray[i2] = null;
            }

            for (int j2 = 0; j2 < 50; j2 += 1)
            {
                prayerOn[j2] = false;
            }

            mouseButtonClick = 0;
            base.lastMouseButton = 0;
            base.mouseButton = 0;
            showShopBox = false;
            showBankBox = false;
            isSleeping = false;
            base.friendsCount = 0;
        }

        public void DrawMinimapMenu(bool canClick)
        {
            int l = ((GameImage)(gameGraphics)).gameWidth - 199;
            int c1 = 156;//'æ';//(char)234;//'\u234';
            int c3 = 152;// '~';//(char)230;//'\u230';
            gameGraphics.DrawPicture(l - 49, 3, baseInventoryPic + 2);
            l += 40;
            gameGraphics.DrawBox(l, 36, c1, c3, 0);
            gameGraphics.SetDimensions(l, 36, l + c1, 36 + c3);
            int j1 = 192 + minimapRandomRotationY;
            int l1 = cameraRotation + minimapRandomRotationX & 0xff;
            int j2 = ((ourPlayer.currentX - 6040) * 3 * j1) / 2048;
            int l3 = ((ourPlayer.currentY - 6040) * 3 * j1) / 2048;
            int j5 = Camera.trigonometryTable[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.trigonometryTable[(1024 - l1 * 4 & 0x3ff) + 1024];
            int j6 = l3 * j5 + j2 * l5 >> 18;
            l3 = l3 * l5 - j2 * j5 >> 18;
            j2 = j6;
            gameGraphics.DrawMinimapPic((l + c1 / 2) - j2, 36 + c3 / 2 + l3, baseInventoryPic - 1, l1 + 64 & 0xff, j1);
            for (int l7 = 0; l7 < objectCount; l7 += 1)
            {
                int k2 = (((objectX[l7] * gridSize + 64) - ourPlayer.currentX) * 3 * j1) / 2048;
                int i4 = (((objectY[l7] * gridSize + 64) - ourPlayer.currentY) * 3 * j1) / 2048;
                int k6 = i4 * j5 + k2 * l5 >> 18;
                i4 = i4 * l5 - k2 * j5 >> 18;
                k2 = k6;
                DrawMinimapObject(l + c1 / 2 + k2, (36 + c3 / 2) - i4, 65535);
            }

            for (int i8 = 0; i8 < groundItemCount; i8 += 1)
            {
                int l2 = (((groundItemX[i8] * gridSize + 64) - ourPlayer.currentX) * 3 * j1) / 2048;
                int j4 = (((groundItemY[i8] * gridSize + 64) - ourPlayer.currentY) * 3 * j1) / 2048;
                int l6 = j4 * j5 + l2 * l5 >> 18;
                j4 = j4 * l5 - l2 * j5 >> 18;
                l2 = l6;
                DrawMinimapObject(l + c1 / 2 + l2, (36 + c3 / 2) - j4, 0xff0000);
            }

            for (int j8 = 0; j8 < npcCount; j8 += 1)
            {
                ClientMob f1 = npcArray[j8];
                int i3 = ((f1.currentX - ourPlayer.currentX) * 3 * j1) / 2048;
                int k4 = ((f1.currentY - ourPlayer.currentY) * 3 * j1) / 2048;
                int i7 = k4 * j5 + i3 * l5 >> 18;
                k4 = k4 * l5 - i3 * j5 >> 18;
                i3 = i7;
                DrawMinimapObject(l + c1 / 2 + i3, (36 + c3 / 2) - k4, 0xffff00);
            }

            for (int k8 = 0; k8 < playerCount; k8 += 1)
            {
                ClientMob f2 = playerArray[k8];
                int j3 = ((f2.currentX - ourPlayer.currentX) * 3 * j1) / 2048;
                int l4 = ((f2.currentY - ourPlayer.currentY) * 3 * j1) / 2048;
                int j7 = l4 * j5 + j3 * l5 >> 18;
                l4 = l4 * l5 - j3 * j5 >> 18;
                j3 = j7;
                int i9 = 0xffffff;
                for (int j9 = 0; j9 < base.friendsCount; j9 += 1)
                {
                    if (f2.nameHash != base.friendsList[j9] || base.friendsWorld[j9] != 99)
                    {
                        continue;
                    }

                    i9 = 65280;
                    break;
                }

                DrawMinimapObject(l + c1 / 2 + j3, (36 + c3 / 2) - l4, i9);
            }

            // compass
            gameGraphics.DrawCircle(l + c1 / 2, 36 + c3 / 2, 2, 0xffffff, 255);
            gameGraphics.DrawMinimapPic(l + 19, 55, baseInventoryPic + 24, cameraRotation + 128 & 0xff, 128);
            gameGraphics.SetDimensions(0, 0, windowWidth, windowHeight + 12);
            if (!canClick)
            {
                return;
            }

            l = base.mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            int l8 = base.mouseY - 36;
            if (l >= 40 && l8 >= 0 && l < 196 && l8 < 152)
            {
                int c2 = 156;//'\u234';
                int c4 = 152;//'\u230';
                int k1 = 192 + minimapRandomRotationY;
                int i2 = cameraRotation + minimapRandomRotationX & 0xff;
                int i1 = ((GameImage)(gameGraphics)).gameWidth - 199;
                i1 += 40;
                int k3 = ((base.mouseX - (i1 + c2 / 2)) * 16384) / (3 * k1);
                int i5 = ((base.mouseY - (36 + c4 / 2)) * 16384) / (3 * k1);
                int k5 = Camera.trigonometryTable[1024 - i2 * 4 & 0x3ff];
                int i6 = Camera.trigonometryTable[(1024 - i2 * 4 & 0x3ff) + 1024];
                int k7 = i5 * k5 + k3 * i6 >> 15;
                i5 = i5 * i6 - k3 * k5 >> 15;
                k3 = k7;
                k3 += ourPlayer.currentX;
                i5 = ourPlayer.currentY - i5;
                if (mouseButtonClick == 1)
                {
                    WalkTo1Tile(sectionX, sectionY, k3 / 128, i5 / 128, false);
                }

                mouseButtonClick = 0;
            }
        }

        public bool IsValidCameraAngle(int cameraDirection)
        {
            int l = ourPlayer.currentX / 128;
            int i1 = ourPlayer.currentY / 128;
            for (int j1 = 2; j1 >= 1; j1 -= 1)
            {
                if (cameraDirection == 1 && ((engineHandle.tiles[l][i1 - j1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1 - j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 3 && ((engineHandle.tiles[l][i1 + j1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1 + j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 5 && ((engineHandle.tiles[l][i1 + j1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1 + j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 7 && ((engineHandle.tiles[l][i1 - j1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1 - j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 0 && (engineHandle.tiles[l][i1 - j1] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 2 && (engineHandle.tiles[l - j1][i1] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 4 && (engineHandle.tiles[l][i1 + j1] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 6 && (engineHandle.tiles[l + j1][i1] & 0x80) == 128)
                {
                    return false;
                }
            }

            return true;
        }

        public void LoadSounds()
        {
            try
            {
                soundData = UnpackData("sounds.mem", "Sound effects", 90);
                audioPlayer = new AudioReader();
                return;
            }
            catch (Exception throwable)
            {
                Console.WriteLine("Unable to init sounds:" + throwable);
            }
        }

        public override void LoadGame()
        {
            int l = 0;
            for (int i1 = 0; i1 < 99; i1 += 1)
            {
                int j1 = i1 + 1;
                int l1 = (int)((double)j1 + 300D * Math.Pow(2D, (double)j1 / 7D));
                l += l1;
                experienceList[i1] = (l & 0xffffffc) / 4;
            }
            LoadConfig();
            if (errorLoading)
            {
                return;
            }

            GameAppletMiddleMan.maxPacketReadCount = 500;
            baseInventoryPic = 2000;
            baseScrollPic = baseInventoryPic + 100;
            baseItemPicture = baseScrollPic + 50;
            baseLoginScreenBackgroundPic = baseItemPicture + 1000;
            baseProjectilePic = baseLoginScreenBackgroundPic + 10;
            baseTexturePic = baseProjectilePic + 50;
            subTexturePic = baseTexturePic + 10;
            graphics = GetGraphics();
            SetRefreshRate(50);
            gameGraphics = new GameImageMiddleMan(windowWidth, windowHeight + 12, 4000)
            {
                gameReference = this
            };
            gameGraphics.SetDimensions(0, 0, windowWidth, windowHeight + 12);
            Menu.isBackgroundPatternEnabled = false;
            Menu.baseScrollPic = baseScrollPic;
            spellMenu = new Menu(gameGraphics, 5);
            int k1 = ((GameImage)(gameGraphics)).gameWidth - 199;
            sbyte byte0 = 36;
            spellMenuHandle = spellMenu.CreateList(k1, byte0 + 24, 196, 90, 1, 500, true);
            friendsMenu = new Menu(gameGraphics, 5);
            friendsMenuHandle = friendsMenu.CreateList(k1, byte0 + 40, 196, 126, 1, 500, true);
            questMenu = new Menu(gameGraphics, 5);
            questMenuHandle = questMenu.CreateList(k1, byte0 + 24, 196, 251, 1, 500, true);
            LoadMedia();
            if (errorLoading)
            {
                return;
            }

            LoadAnimations();
            if (errorLoading)
            {
                return;
            }

            gameCamera = new Camera(gameGraphics, 15000, 15000, 1000);

            gameCamera.SetCameraSize(windowWidth / 2, windowHeight / 2, windowWidth / 2, windowHeight / 2, windowWidth, cameraFieldOfView);
            gameCamera.zoom1 = 2400;
            gameCamera.zoom2 = 2400;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 2300;
            gameCamera.OffsetAllModelColours(-50, -10, -50);
            engineHandle = new EngineHandle(gameCamera, gameGraphics)
            {
                baseInventoryPic = baseInventoryPic
            };
            LoadTextures();
            if (errorLoading)
            {
                return;
            }

            LoadModels();
            if (errorLoading)
            {
                return;
            }

            LoadMap();
            if (errorLoading)
            {
                return;
            }

            LoadSounds();
            if (!errorLoading)
            {
                if (OnContentLoaded is not null)
                {
                    OnContentLoaded(this, new ContentLoadedEventArgs("Starting game...", 100));
                }
                DrawLoadingBarText(100, "Starting game...");
                CreateChatInputMenu();
                CreateLoginMenus();
                CreateAppearanceWindow();
                SetLoginVars();

                string[] modelNames = Data.GameData.modelName;

                if (OnContentLoadedCompleted is not null)
                {
                    OnContentLoadedCompleted(this, new EventArgs());
                }

                CreateLoginScreenBackgrounds();
                return;
            }
        }

        public void CreateLoginMenus()
        {
            loginMenuFirst = new Menu(gameGraphics, 50);
            int l = 40;
            if (!Config.MembersFeatures)
            {
                loginMenuFirst.DrawText(256, 200 + l, "Click on an option", 5, true);
                loginMenuFirst.DrawButton(156, 240 + l, 120, 35);
                loginMenuFirst.DrawButton(356, 240 + l, 120, 35);
                loginMenuFirst.DrawText(156, 240 + l, "New User", 5, false);
                loginMenuFirst.DrawText(356, 240 + l, "Existing User", 5, false);
                loginButtonNewUser = loginMenuFirst.CreateButton(156, 240 + l, 120, 35);
                loginMenuLoginButton = loginMenuFirst.CreateButton(356, 240 + l, 120, 35);
            }
            else
            {
                loginMenuFirst.DrawText(256, 200 + l, "Welcome to RuneScape", 4, true);
                loginMenuFirst.DrawText(256, 215 + l, "You need a member account to use this server", 4, true);
                loginMenuFirst.DrawButton(256, 250 + l, 200, 35);
                loginMenuFirst.DrawText(256, 250 + l, "Click here to login", 5, false);
                loginMenuLoginButton = loginMenuFirst.CreateButton(256, 250 + l, 200, 35);
            }
            loginNewUser = new Menu(gameGraphics, 50);
            l = 230;
            loginNewUser.DrawText(256, l + 8, "To create an account please go back to the", 4, true);
            l += 20;
            loginNewUser.DrawText(256, l + 8, "www.runescape.com front page, and choose 'create account'", 4, true);
            l += 30;
            loginNewUser.DrawButton(256, l + 17, 150, 34);
            loginNewUser.DrawText(256, l + 17, "Ok", 5, false);
            loginMenuOkButton = loginNewUser.CreateButton(256, l + 17, 150, 34);
            loginMenuLogin = new Menu(gameGraphics, 50);
            l = 230;
            loginMenuStatusText = loginMenuLogin.DrawText(256, l - 10, "Please enter your username and password", 4, true);
            l += 28;
            loginMenuLogin.DrawButton(140, l, 200, 40);
            loginMenuLogin.DrawText(140, l - 10, "Username:", 4, false);
            loginMenuUserText = loginMenuLogin.CreateInput(140, l + 10, 200, 40, 4, 12, false, false);
            l += 47;
            loginMenuLogin.DrawButton(190, l, 200, 40);
            loginMenuLogin.DrawText(190, l - 10, "Password:", 4, false);
            loginMenuPasswordText = loginMenuLogin.CreateInput(190, l + 10, 200, 40, 4, 20, true, false);
            l -= 55;
            loginMenuLogin.DrawButton(410, l, 120, 25);
            loginMenuLogin.DrawText(410, l, "Ok", 4, false);
            loginMenuOkLoginButton = loginMenuLogin.CreateButton(410, l, 120, 25);
            l += 30;
            loginMenuLogin.DrawButton(410, l, 120, 25);
            loginMenuLogin.DrawText(410, l, "Cancel", 4, false);
            loginMenuCancelButton = loginMenuLogin.CreateButton(410, l, 120, 25);
            l += 30;
            loginMenuLogin.SetFocus(loginMenuUserText);
        }

        public override void LostConnection()
        {
            systemUpdate = 0;
            if (logoutTimer != 0)
            {
                ResetIntVars();
                return;
            }
            else
            {
                base.LostConnection();
                return;
            }
        }

        public void LoadMedia()
        {
            sbyte[] media = UnpackData("media.jag", "2d graphics", 20);
            if (media is null)
            {
                errorLoading = true;
                return;
            }
            sbyte[] abyte1 = DataOperations.LoadData("index.dat", 0, media);
            gameGraphics.UnpackImageData(baseInventoryPic, DataOperations.LoadData("inv1.dat", 0, media), abyte1, 1);
            gameGraphics.UnpackImageData(baseInventoryPic + 1, DataOperations.LoadData("inv2.dat", 0, media), abyte1, 6);
            gameGraphics.UnpackImageData(baseInventoryPic + 9, DataOperations.LoadData("bubble.dat", 0, media), abyte1, 1);
            gameGraphics.UnpackImageData(baseInventoryPic + 10, DataOperations.LoadData("runescape.dat", 0, media), abyte1, 1);
            gameGraphics.UnpackImageData(baseInventoryPic + 11, DataOperations.LoadData("splat.dat", 0, media), abyte1, 3);
            gameGraphics.UnpackImageData(baseInventoryPic + 14, DataOperations.LoadData("icon.dat", 0, media), abyte1, 8);
            gameGraphics.UnpackImageData(baseInventoryPic + 22, DataOperations.LoadData("hbar.dat", 0, media), abyte1, 1);
            gameGraphics.UnpackImageData(baseInventoryPic + 23, DataOperations.LoadData("hbar2.dat", 0, media), abyte1, 1);
            gameGraphics.UnpackImageData(baseInventoryPic + 24, DataOperations.LoadData("compass.dat", 0, media), abyte1, 1);
            gameGraphics.UnpackImageData(baseInventoryPic + 25, DataOperations.LoadData("buttons.dat", 0, media), abyte1, 2);
            gameGraphics.UnpackImageData(baseScrollPic, DataOperations.LoadData("scrollbar.dat", 0, media), abyte1, 2);
            gameGraphics.UnpackImageData(baseScrollPic + 2, DataOperations.LoadData("corners.dat", 0, media), abyte1, 4);
            gameGraphics.UnpackImageData(baseScrollPic + 6, DataOperations.LoadData("arrows.dat", 0, media), abyte1, 2);
            gameGraphics.UnpackImageData(baseProjectilePic, DataOperations.LoadData("projectile.dat", 0, media), abyte1, Data.GameData.spellProjectileCount);
            int l = Data.GameData.highestLoadedPicture;
            for (int i1 = 1; l > 0; i1 += 1)
            {
                int j1 = l;
                l -= 30;
                if (j1 > 30)
                {
                    j1 = 30;
                }

                gameGraphics.UnpackImageData(baseItemPicture + (i1 - 1) * 30, DataOperations.LoadData("objects" + i1 + ".dat", 0, media), abyte1, j1);
            }
            //gameGraphics.UpdateGameImage();
            gameGraphics.LoadImage(baseInventoryPic);
            gameGraphics.LoadImage(baseInventoryPic + 9);
            for (int k1 = 11; k1 <= 26; k1 += 1)
            {
                gameGraphics.LoadImage(baseInventoryPic + k1);
            }

            for (int l1 = 0; l1 < Data.GameData.spellProjectileCount; l1 += 1)
            {
                gameGraphics.LoadImage(baseProjectilePic + l1);
            }

            for (int i2 = 0; i2 < Data.GameData.highestLoadedPicture; i2 += 1)
            {
                gameGraphics.LoadImage(baseProjectilePic + i2);
                //var w = ((GameImage)(gameGraphics)).pictureWidth[baseProjectilePic + i2];
                //var h = ((GameImage)(gameGraphics)).pictureHeight[baseProjectilePic + i2];
                //var texture = GameImage.UnpackedImages[baseProjectilePic + i2];
                //if (texture is not null)
                //    texture.SaveAsJpeg(System.IO.File.OpenWrite("c:/jpg/" + baseProjectilePic + i2 + ".jpg"), w, h);
            }


        }

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
                base.lastMouseButton = 0;
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

        public void LoadAnimations()
        {
            StringBuilder sb = new();
            sbyte[] abyte0 = null;
            sbyte[] abyte1 = null;
            abyte0 = UnpackData("entity.jag", "people and monsters", 30);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            abyte1 = DataOperations.LoadData("index.dat", 0, abyte0);
            sbyte[] abyte2 = null;
            sbyte[] abyte3 = null;
            abyte2 = UnpackData("entity.mem", "member graphics", 45);
            if (abyte2 is null)
            {
                errorLoading = true;
                return;
            }
            abyte3 = DataOperations.LoadData("index.dat", 0, abyte2);
            int l = 0;
            animationNumber = 0;
            //label0:
            for (int i1 = 0; i1 < Data.GameData.animationCount; i1 += 1)
            {
                //   label4:
                bool breakThis = false;
                string s1 = Data.GameData.animationName[i1];
                for (int j1 = 0; j1 < i1; j1 += 1)
                {
                    if (Data.GameData.animationName[j1].ToLower() != s1)
                    {
                        continue;
                    }

                    Data.GameData.animationNumber[i1] = Data.GameData.animationNumber[j1];

                    // i1 += 1;
                    // goto label0;
                    //break;
                    breakThis = true;
                    break;
                }
                if (breakThis)
                {
                    continue;
                }

                //label4:
                sbyte[] abyte7 = DataOperations.LoadData(s1 + ".dat", 0, abyte0);
                sbyte[] abyte4 = abyte1;
                if (abyte7 is null)
                {
                    abyte7 = DataOperations.LoadData(s1 + ".dat", 0, abyte2);
                    abyte4 = abyte3;
                }
                if (abyte7 is not null)
                {
                    try
                    {
                        gameGraphics.UnpackImageData(animationNumber, abyte7, abyte4, 15);
                        l += 15;
                        if (Data.GameData.animationHasA[i1] == 1)
                        {
                            sbyte[] abyte8 = DataOperations.LoadData(s1 + "a.dat", 0, abyte0);
                            sbyte[] abyte5 = abyte1;
                            if (abyte8 is null)
                            {
                                abyte8 = DataOperations.LoadData(s1 + "a.dat", 0, abyte2);
                                abyte5 = abyte3;
                            }
                            gameGraphics.UnpackImageData(animationNumber + 15, abyte8, abyte5, 3);
                            l += 3;
                        }
                        if (Data.GameData.animationHasF[i1] == 1)
                        {
                            sbyte[] abyte9 = DataOperations.LoadData(s1 + "f.dat", 0, abyte0);
                            sbyte[] abyte6 = abyte1;
                            if (abyte9 is null)
                            {
                                abyte9 = DataOperations.LoadData(s1 + "f.dat", 0, abyte2);
                                abyte6 = abyte3;
                            }
                            gameGraphics.UnpackImageData(animationNumber + 18, abyte9, abyte6, 9);
                            l += 9;
                        }
                        if (Data.GameData.animationGenderModels[i1] != 0)
                        {
                            for (int k1 = animationNumber; k1 < animationNumber + 27; k1 += 1)
                            {
                                gameGraphics.LoadImage(k1);
                            }
                        }
                    }
                    catch { }
                }
                Data.GameData.animationNumber[i1] = animationNumber;
                animationNumber += 27;




                //if (File.Exists("animations-loaded.txt")) File.Delete("animations-loaded.txt");
                //if (!File.Exists("animations-loaded.txt")) File.Create("animations-loaded.txt").Close();
                sb.AppendLine("Loaded: " + l + " frames of animation");

#warning ugly fix for forcing animation count to 1143.
                if (l == 1143)
                {
                    break;
                }
            }
            string animationsOutput = sb.ToString();
            Console.WriteLine("Loaded: " + l + " frames of animation");
        }

        public void UpdateAppearanceWindow()
        {
            appearanceMenu.MouseClick(base.mouseX, base.mouseY, base.lastMouseButton, base.mouseButton);
            if (appearanceMenu.IsClicked(appearanceHeadLeftArrow))
            {
                do
                {
                    appearanceHeadType = ((appearanceHeadType - 1) + Data.GameData.animationCount) % Data.GameData.animationCount;
                }
                while ((Data.GameData.animationGenderModels[appearanceHeadType] & 3) != 1 || (Data.GameData.animationGenderModels[appearanceHeadType] & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.IsClicked(appearanceHeadRightArrow))
            {
                do
                {
                    appearanceHeadType = (appearanceHeadType + 1) % Data.GameData.animationCount;
                }
                while ((Data.GameData.animationGenderModels[appearanceHeadType] & 3) != 1 || (Data.GameData.animationGenderModels[appearanceHeadType] & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.IsClicked(appearanceHairLeftArrow))
            {
                appearanceHairColour = ((appearanceHairColour - 1) + appearanceHairColours.Length) % appearanceHairColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceHairRightArrow))
            {
                appearanceHairColour = (appearanceHairColour + 1) % appearanceHairColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceGenderLeftArrow) || appearanceMenu.IsClicked(appearanceGenderRightArrow))
            {
                for (appearanceHeadGender = 3 - appearanceHeadGender; (Data.GameData.animationGenderModels[appearanceHeadType] & 3) != 1 || (Data.GameData.animationGenderModels[appearanceHeadType] & 4 * appearanceHeadGender) == 0; appearanceHeadType = (appearanceHeadType + 1) % Data.GameData.animationCount)
                {
                    ;
                }

                for (; (Data.GameData.animationGenderModels[appearanceBodyGender] & 3) != 2 || (Data.GameData.animationGenderModels[appearanceBodyGender] & 4 * appearanceHeadGender) == 0; appearanceBodyGender = (appearanceBodyGender + 1) % Data.GameData.animationCount)
                {
                    ;
                }
            }
            if (appearanceMenu.IsClicked(appearanceTopLeftArrow))
            {
                appearanceTopColour = ((appearanceTopColour - 1) + appearanceTopBottomColours.Length) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceTopRightArrow))
            {
                appearanceTopColour = (appearanceTopColour + 1) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceSkinLeftArrow))
            {
                appearanceSkinColour = ((appearanceSkinColour - 1) + appearanceSkinColours.Length) % appearanceSkinColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceSkingRightArrow))
            {
                appearanceSkinColour = (appearanceSkinColour + 1) % appearanceSkinColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceBottomLeftArrow))
            {
                appearanceBottomColour = ((appearanceBottomColour - 1) + appearanceTopBottomColours.Length) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceBottomRightArrow))
            {
                appearanceBottomColour = (appearanceBottomColour + 1) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceAcceptButton))
            {
                base.streamClass.CreatePacket(218);
                base.streamClass.AddByte(appearanceHeadGender);
                base.streamClass.AddByte(appearanceHeadType);
                base.streamClass.AddByte(appearanceBodyGender);
                base.streamClass.AddByte(appearance2Colour);
                base.streamClass.AddByte(appearanceHairColour);
                base.streamClass.AddByte(appearanceTopColour);
                base.streamClass.AddByte(appearanceBottomColour);
                base.streamClass.AddByte(appearanceSkinColour);
                base.streamClass.FormatPacket();
                gameGraphics.ClearScreen();
                showAppearanceWindow = false;
            }
        }

        public void DrawWelcomeBox()
        {
            int l = 65;
            if (lastLoginAddress != "0.0.0.0")
            {
                l += 30;
            }

            if (subDaysLeft > 0)
            {
                l += 15;
            }

            if (lastLoginDays >= 0)
            {
                l += 15;
            }

            int i1 = 167 - l / 2;
            gameGraphics.DrawBox(56, 167 - l / 2, 400, l, 0);
            gameGraphics.DrawBoxEdge(56, 167 - l / 2, 400, l, 0xffffff);
            i1 += 20;
            gameGraphics.DrawText("Welcome to RuneScape " + loginUsername, 256, i1, 4, 0xffff00);
            i1 += 30;
            string s1;
            // lastLoginDays    subDaysLeft    lastLoginAddress
            if (lastLoginDays == 0)
            {
                s1 = "earlier today";
            }
            else
                if (lastLoginDays == 1)
            {
                s1 = "yesterday";
            }
            else
            {
                s1 = lastLoginDays + " days ago";
            }

            if (lastLoginAddress != "0.0.0.0")
            {
                gameGraphics.DrawText("You last logged in " + s1, 256, i1, 1, 0xffffff);
                i1 += 15;
                gameGraphics.DrawText("from: " + lastLoginAddress, 256, i1, 1, 0xffffff);
                i1 += 15;
            }
            if (subDaysLeft > 0)
            {
                gameGraphics.DrawText("Subscription left: " + subDaysLeft + " days", 256, i1, 1, 0xffffff);
                i1 += 15;
            }
            /*if(unreadMessages > 0) {
                int j1 = 0xffffff;
                gameGraphics.DrawText("Jagex staff will NEVER email you. We use the", 256, i1, 1, j1);
                i1 += 15;
                gameGraphics.DrawText("message-centre on this website instead.", 256, i1, 1, j1);
                i1 += 15;
                if(unreadMessages == 1)
                    gameGraphics.DrawText("You have @yel@0@whi@ unread messages in your message-centre", 256, i1, 1, 0xffffff);
                else
                    gameGraphics.DrawText("You have @gre@" + (unreadMessages - 1) + " unread messages @whi@in your message-centre", 256, i1, 1, 0xffffff);
                i1 += 15;
                i1 += 15;
            }
            if(lastChangedRecoveryDays != 201) {
                if(lastChangedRecoveryDays == 200) {
                    gameGraphics.DrawText("You have not yet set any password recovery questions.", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.DrawText("We strongly recommend you do so now to secure your account.", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.DrawText("Do this from the 'account management' area on our front webpage", 256, i1, 1, 0xff8000);
                    i1 += 15;
                } else {
                    string s2;
                    if(lastChangedRecoveryDays == 0)
                        s2 = "Earlier today";
                    else
                    if(lastChangedRecoveryDays == 1)
                        s2 = "Yesterday";
                    else
                        s2 = lastChangedRecoveryDays + " days ago";
                    gameGraphics.DrawText(s2 + " you changed your recovery questions", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.DrawText("If you do not remember making this change then cancel it immediately", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.DrawText("Do this from the 'account management' area on our front webpage", 256, i1, 1, 0xff8000);
                    i1 += 15;
                }
                i1 += 15;
            }*/
            int k1 = 0xffffff;
            if (base.mouseY > i1 - 12 && base.mouseY <= i1 && base.mouseX > 106 && base.mouseX < 406)
            {
                k1 = 0xff0000;
            }

            gameGraphics.DrawText("Click here to close window", 256, i1, 1, k1);
            if (mouseButtonClick == 1)
            {
                if (k1 == 0xff0000)
                {
                    showWelcomeBox = false;
                }

                if ((base.mouseX < 86 || base.mouseX > 426) && (base.mouseY < 167 - l / 2 || base.mouseY > 167 + l / 2))
                {
                    showWelcomeBox = false;
                }
            }
            mouseButtonClick = 0;
        }

        public int GetInventoryItemTotalCount(int itemId)
        {
            int l = 0;
            for (int i1 = 0; i1 < inventoryItemsCount; i1 += 1)
            {
                if (inventoryItems[i1] == itemId)
                {
                    if (Data.GameData.itemStackable[itemId] == 1)
                    {
                        l += 1;
                    }
                    else
                    {
                        l += inventoryItemCount[i1];
                    }
                }
            }

            return l;
        }

        public void SendLogout()
        {
            if (!loggedIn)
            {
                return;
            }

            if (combatTimeout > 450)
            {
                DisplayMessage("@cya@You can't logout during combat!", 3);
                return;
            }
            if (combatTimeout > 0)
            {
                DisplayMessage("@cya@You can't logout for 10 seconds after combat", 3);
                return;
            }
            else
            {
                base.streamClass.CreatePacket(129);
                base.streamClass.FormatPacket();
                logoutTimer = 1000;

                base.streamClass.CloseStream();
                return;
            }
        }

        //public Uri getCodeBase() {
        //    if(Link.gameApplet is not null)
        //        return Link.gameApplet.getCodeBase();
        //    else
        //        return base.getCodeBase();
        //}

        public bool WalkTo(int startX, int startY, int destBottomX, int destBottomY, int destTopX, int destTopY, bool checkForObjects,
                bool walkToACommand)
        {
            int stepCount = engineHandle.GeneratePath(startX, startY, destBottomX, destBottomY, destTopX, destTopY, walkArrayX, walkArrayY, checkForObjects);
            if (stepCount == -1)
            {
                if (walkToACommand)
                {
                    stepCount = 1;
                    walkArrayX[0] = destBottomX;
                    walkArrayY[0] = destBottomY;
                }
                else
                {
                    return false;
                }
            }

            stepCount -= 1;
            startX = walkArrayX[stepCount];
            startY = walkArrayY[stepCount];
            stepCount -= 1;

            if (walkToACommand)
            {
                base.streamClass.CreatePacket(246);
            }
            else
            {
                base.streamClass.CreatePacket(132);
            }

            base.streamClass.AddShort(startX + areaX);
            base.streamClass.AddShort(startY + areaY);

            if (walkToACommand && stepCount == -1 && (startX + areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1 -= 1)
            {
                base.streamClass.AddByte(walkArrayX[i1] - startX);
                base.streamClass.AddByte(walkArrayY[i1] - startY);
            }

            base.streamClass.FormatPacket();
            //base.streamClass.Flush();

            actionPictureType = -24;
            walkMouseX = base.mouseX;
            walkMouseY = base.mouseY;
            return true;
        }

        public bool WalkToAlternate(int startX, int startY, int destBottomX, int destBottomY, int destTopX, int destTopY, bool unknownDifferent,
                bool walkToACommand)
        {
            int stepCount = engineHandle.GeneratePath(startX, startY, destBottomX, destBottomY, destTopX, destTopY, walkArrayX, walkArrayY, unknownDifferent);
            if (stepCount == -1)
            {
                return false;
            }

            stepCount -= 1;
            startX = walkArrayX[stepCount];
            startY = walkArrayY[stepCount];
            stepCount -= 1;
            if (walkToACommand)
            {
                base.streamClass.CreatePacket(246);
            }
            else
            {
                base.streamClass.CreatePacket(132);
            }

            base.streamClass.AddShort(startX + areaX);
            base.streamClass.AddShort(startY + areaY);
            if (walkToACommand && stepCount == -1 && (startX + areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1 -= 1)
            {
                base.streamClass.AddByte(walkArrayX[i1] - startX);
                base.streamClass.AddByte(walkArrayY[i1] - startY);
            }

            base.streamClass.FormatPacket();
            actionPictureType = -24;
            walkMouseX = base.mouseX;
            walkMouseY = base.mouseY;
            return true;
        }

        public void DrawOptionsMenu(bool canClick)
        {
            int l = ((GameImage)(gameGraphics)).gameWidth - 199;
            int i1 = 36;
            gameGraphics.DrawPicture(l - 49, 3, baseInventoryPic + 6);
            int c1 = 196;
            gameGraphics.DrawBoxAlpha(l, 36, c1, 62, GameImage.RgbToInt(181, 181, 181), 160);
            gameGraphics.DrawBoxAlpha(l, 98, c1, 92, GameImage.RgbToInt(201, 201, 201), 160);
            gameGraphics.DrawBoxAlpha(l, 190, c1, 90, GameImage.RgbToInt(181, 181, 181), 160);
            gameGraphics.DrawBoxAlpha(l, 280, c1, 40, GameImage.RgbToInt(201, 201, 201), 160);
            int j1 = l + 3;
            int l1 = i1 + 15;
            gameGraphics.DrawString("Game options - click to toggle", j1, l1, 1, 0);
            l1 += 15;
            if (configCameraAutoAngle)
            {
                gameGraphics.DrawString("Camera angle mode - @gre@Auto", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Camera angle mode - @red@Manual", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (configOneMouseButton)
            {
                gameGraphics.DrawString("Mouse buttons - @red@One", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Mouse buttons - @gre@Two", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (Config.MembersFeatures)
            {
                if (configSoundOff)
                {
                    gameGraphics.DrawString("Sound effects - @red@off", j1, l1, 1, 0xffffff);
                }
                else
                {
                    gameGraphics.DrawString("Sound effects - @gre@on", j1, l1, 1, 0xffffff);
                }
            }

            l1 += 15;
            gameGraphics.DrawString("Client assists - click to toggle", j1, l1, 1, 0);
            l1 += 15;
            if (showRoofs)
            {
                gameGraphics.DrawString("Roofs - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Roofs - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (showCombatWindow)
            {
                gameGraphics.DrawString("Fight mode window - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Fight mode window - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (fogOfWar)
            {
                gameGraphics.DrawString("Fog of war - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Fog of war - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (autoScreenshot)
            {
                gameGraphics.DrawString("Automatic screenshots - @gre@on", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Automatic screenshots - @red@off", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (useChatFilter)
            {
                gameGraphics.DrawString("Chat filter: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Chat filter: @red@<off>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            gameGraphics.DrawString("Privacy settings. Will be applied to", j1, l1, 1, 0);
            l1 += 15;
            gameGraphics.DrawString("all people not on your friends list", j1, l1, 1, 0);
            l1 += 15;
            if (base.blockChat == 0)
            {
                gameGraphics.DrawString("Block chat messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Block chat messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (base.blockPrivate == 0)
            {
                gameGraphics.DrawString("Block public messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Block public messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (base.blockTrade == 0)
            {
                gameGraphics.DrawString("Block trade requests: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Block trade requests: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (Config.MembersFeatures)
            {
                if (base.blockDuel == 0)
                {
                    gameGraphics.DrawString("Block duel requests: @red@<off>", l + 3, l1, 1, 0xffffff);
                }
                else
                {
                    gameGraphics.DrawString("Block duel requests: @gre@<on>", l + 3, l1, 1, 0xffffff);
                }
            }

            l1 += 15;
            l1 += 5;
            gameGraphics.DrawString("Always logout when you finish", j1, l1, 1, 0);
            l1 += 15;
            int j2 = 0xffffff;
            if (base.mouseX > j1 && base.mouseX < j1 + c1 && base.mouseY > l1 - 12 && base.mouseY < l1 + 4)
            {
                j2 = 0xffff00;
            }

            gameGraphics.DrawString("Click here to logout", l + 3, l1, 1, j2);
            if (!canClick)
            {
                return;
            }

            l = base.mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = base.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 280)
            {
                int k2 = ((GameImage)(gameGraphics)).gameWidth - 199;
                sbyte byte0 = 36;
                int c2 = 196;
                int k1 = k2 + 3;
                int i2 = byte0 + 30;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    configCameraAutoAngle = !configCameraAutoAngle;
                    base.streamClass.CreatePacket(157);
                    base.streamClass.AddByte(0);
                    int configCameraAutoAngleByte = 0;
                    if (configCameraAutoAngle)
                    {
                        configCameraAutoAngleByte = 1;
                    }
                    base.streamClass.AddByte(configCameraAutoAngleByte);
                    base.streamClass.FormatPacket();
                }
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    configOneMouseButton = !configOneMouseButton;
                    base.streamClass.CreatePacket(157);
                    base.streamClass.AddByte(2);
                    int configOneMouseButtonByte = 0;
                    if (configOneMouseButton)
                    {
                        configOneMouseButtonByte = 1;
                    }
                    base.streamClass.AddByte(configOneMouseButtonByte);
                    base.streamClass.FormatPacket();
                }
                i2 += 15;
                if (Config.MembersFeatures && base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    configSoundOff = !configSoundOff;
                    base.streamClass.CreatePacket(157);
                    base.streamClass.AddByte(3);
                    int configSoundOffByte = 0;
                    if (configSoundOff)
                    {
                        configSoundOffByte = 1;
                    }
                    base.streamClass.AddByte(configSoundOffByte);
                    base.streamClass.FormatPacket();
                }
                i2 += 15;
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    showRoofs = !showRoofs;
                    base.streamClass.CreatePacket(157);
                    base.streamClass.AddByte(4);
                    int showRoofsByte = 0;
                    if (showRoofs)
                    {
                        showRoofsByte = 1;
                    }
                    base.streamClass.AddByte(showRoofsByte);
                    base.streamClass.FormatPacket();
                }
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    showCombatWindow = !showCombatWindow;
                    base.streamClass.CreatePacket(157);
                    base.streamClass.AddByte(6);
                    int showCombatWindowByte = 0;
                    if (showCombatWindow)
                    {
                        showCombatWindowByte = 1;
                    }
                    base.streamClass.AddByte(showCombatWindowByte);
                    base.streamClass.FormatPacket();
                }
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    fogOfWar = !fogOfWar;
                }
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    autoScreenshot = !autoScreenshot;
                    base.streamClass.CreatePacket(157);
                    base.streamClass.AddByte(5);
                    int autoScreenshotByte = 0;
                    if (autoScreenshot)
                    {
                        autoScreenshotByte = 1;
                    }
                    base.streamClass.AddByte(autoScreenshotByte);
                    base.streamClass.FormatPacket();
                }
                bool flag = false;
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    useChatFilter = !useChatFilter;
                }
                i2 += 15;
                i2 += 15;
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    base.blockChat = 1 - base.blockChat;
                    flag = true;
                }
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    base.blockPrivate = 1 - base.blockPrivate;
                    flag = true;
                }
                i2 += 15;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    base.blockTrade = 1 - base.blockTrade;
                    flag = true;
                }
                i2 += 15;
                if (Config.MembersFeatures && base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    base.blockDuel = 1 - base.blockDuel;
                    flag = true;
                }
                i2 += 15;
                if (flag)
                {
                    SendUpdatedPrivacyInfo(base.blockChat, base.blockPrivate, base.blockTrade, base.blockDuel);
                }

                i2 += 20;
                if (base.mouseX > k1 && base.mouseX < k1 + c2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    SendLogout();
                }

                mouseButtonClick = 0;
            }
        }

        public void WalkToObject(int objectX, int objectY, int facingDirection, int objectIndex)
        {
            int l;
            int i1;
            if (facingDirection == 0 || facingDirection == 4)
            {
                l = Data.GameData.objectWidth[objectIndex];
                i1 = Data.GameData.objectHeight[objectIndex];
            }
            else
            {
                i1 = Data.GameData.objectWidth[objectIndex];
                l = Data.GameData.objectHeight[objectIndex];
            }
            if (Data.GameData.objectType[objectIndex] == 2 || Data.GameData.objectType[objectIndex] == 3)
            {
                if (facingDirection == 0)
                {
                    objectX -= 1;
                    l += 1;
                }
                if (facingDirection == 2)
                {
                    i1 += 1;
                }

                if (facingDirection == 4)
                {
                    l += 1;
                }

                if (facingDirection == 6)
                {
                    objectY -= 1;
                    i1 += 1;
                }
                WalkTo(sectionX, sectionY, objectX, objectY, (objectX + l) - 1, (objectY + i1) - 1, false, true);
                return;
            }
            else
            {
                WalkTo(sectionX, sectionY, objectX, objectY, (objectX + l) - 1, (objectY + i1) - 1, true, true);
                return;
            }
        }

        public void CreateChatInputMenu()
        {
            chatInputMenu = new Menu(gameGraphics, 10);
            messagesHandleType2 = chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            chatInputBox = chatInputMenu.CreateTextInput(7, 324, 498, 14, 1, 80, false, true);
            messagesHandleType5 = chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            messagesHandleType6 = chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            chatInputMenu.SetFocus(chatInputBox);
        }

        public void DrawCombatStyleBox()
        {
            sbyte byte0 = 7;
            sbyte byte1 = 15;
            int c1 = 175; ;//'\u257';
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < 5; l += 1)
                {
                    if (l <= 0 || base.mouseX <= byte0 || base.mouseX >= byte0 + c1 || base.mouseY <= byte1 + l * 20 || base.mouseY >= byte1 + l * 20 + 20)
                    {
                        continue;
                    }

                    combatStyle = l - 1;
                    mouseButtonClick = 0;
                    base.streamClass.CreatePacket(42);
                    base.streamClass.AddByte(combatStyle);
                    base.streamClass.FormatPacket();
                    break;
                }

            }
            for (int i1 = 0; i1 < 5; i1 += 1)
            {
                if (i1 == combatStyle + 1)
                {
                    gameGraphics.DrawBoxAlpha(byte0, byte1 + i1 * 20, c1, 20, GameImage.RgbToInt(255, 0, 0), 128);
                }
                else
                {
                    gameGraphics.DrawBoxAlpha(byte0, byte1 + i1 * 20, c1, 20, GameImage.RgbToInt(190, 190, 190), 128);
                }

                gameGraphics.DrawLineX(byte0, byte1 + i1 * 20, c1, 0);
                gameGraphics.DrawLineX(byte0, byte1 + i1 * 20 + 20, c1, 0);
            }

            gameGraphics.DrawText("Select combat style", byte0 + c1 / 2, byte1 + 16, 3, 0xffffff);
            gameGraphics.DrawText("Controlled (+1 of each)", byte0 + c1 / 2, byte1 + 36, 3, 0);
            gameGraphics.DrawText("Aggressive (+3 strength)", byte0 + c1 / 2, byte1 + 56, 3, 0);
            gameGraphics.DrawText("Accurate   (+3 attack)", byte0 + c1 / 2, byte1 + 76, 3, 0);
            gameGraphics.DrawText("Defensive  (+3 defense)", byte0 + c1 / 2, byte1 + 96, 3, 0);
        }

        public void DrawTradeBox()
        {
            if (mouseButtonClick != 0)
            {
                int mx = base.mouseX - 22;
                int my = base.mouseY - 36;
                if (mx >= 0 && my >= 30 && mx < 462 && my < 262)
                {
                    if (mx > 216 && my > 30 && mx < 462 && my < 235)
                    {
                        int curItem = (mx - 217) / 49 + ((my - 31) / 34) * 5;
                        if (curItem >= 0 && curItem < inventoryItemsCount)
                        {
                            int item = inventoryItems[curItem];
                            mouseClickedHeldInTradeDuelBox = 1;
                            bool ourTradeItemsChanged = false;
                            int someInt = 0;
                            for (int tradeItem = 0; tradeItem < tradeItemsOurCount; tradeItem += 1)
                            {
                                if (tradeItemsOur[tradeItem] == item)
                                {
                                    if (Data.GameData.itemStackable[item] == 0)
                                    {
                                        for (int i = 0; i < mouseClickedHeldInTradeDuelBox; i += 1)
                                        {
                                            if (tradeItemOurCount[tradeItem] < inventoryItemCount[curItem])
                                            {
                                                tradeItemOurCount[tradeItem] += 1;
                                            }

                                            ourTradeItemsChanged = true;
                                        }
                                    }
                                    else
                                    {
                                        someInt += 1;
                                    }
                                }
                            }

                            if (GetInventoryItemTotalCount(item) <= someInt)
                            {
                                ourTradeItemsChanged = true;
                            }

                            if (Data.GameData.itemSpecial[item] == 1)
                            {
                                DisplayMessage("This object cannot be traded with other players", 3);
                                ourTradeItemsChanged = true;
                            }
                            if (!ourTradeItemsChanged && tradeItemsOurCount < 12)
                            {
                                tradeItemsOur[tradeItemsOurCount] = item;
                                tradeItemOurCount[tradeItemsOurCount] = 1;
                                tradeItemsOurCount += 1;
                                ourTradeItemsChanged = true;
                            }
                            if (ourTradeItemsChanged)
                            {
                                base.streamClass.CreatePacket(70);
                                base.streamClass.AddByte(tradeItemsOurCount);
                                for (int i = 0; i < tradeItemsOurCount; i += 1)
                                {
                                    base.streamClass.AddShort(tradeItemsOur[i]);
                                    base.streamClass.AddInt(tradeItemOurCount[i]);
                                }
                                base.streamClass.FormatPacket();
                                tradeOtherAccepted = false;
                                tradeWeAccepted = false;
                            }
                        }
                    }
                    else if (mx > 8 && my > 30 && mx < 205 && my < 133)
                    {
                        int curItem = (mx - 9) / 49 + ((my - 31) / 34) * 4;
                        if (curItem >= 0 && curItem < tradeItemsOurCount)
                        {
                            int item = tradeItemsOur[curItem];
                            for (int i = 0; i < mouseClickedHeldInTradeDuelBox; i += 1)
                            {
                                if (Data.GameData.itemStackable[item] == 0 && tradeItemOurCount[curItem] > 1)
                                {
                                    tradeItemOurCount[curItem] -= 1;
                                    continue;
                                }
                                tradeItemsOurCount -= 1;
                                mouseButtonHeldTime = 0;
                                for (int j = curItem; j < tradeItemsOurCount; j += 1)
                                {
                                    tradeItemsOur[j] = tradeItemsOur[j + 1];
                                    tradeItemOurCount[j] = tradeItemOurCount[j + 1];
                                }
                                break;
                            }
                            base.streamClass.CreatePacket(70);
                            base.streamClass.AddByte(tradeItemsOurCount);
                            for (int i = 0; i < tradeItemsOurCount; i += 1)
                            {
                                base.streamClass.AddShort(tradeItemsOur[i]);
                                base.streamClass.AddInt(tradeItemOurCount[i]);
                            }
                            base.streamClass.FormatPacket();
                            tradeOtherAccepted = false;
                            tradeWeAccepted = false;
                        }
                    }
                    if (mx >= 217 && my >= 238 && mx <= 286 && my <= 259)
                    {
                        tradeWeAccepted = true;
                        base.streamClass.CreatePacket(211);
                        base.streamClass.FormatPacket();
                    }
                    if (mx >= 394 && my >= 238 && mx < 463 && my < 259)
                    {
                        showTradeBox = false;
                        base.streamClass.CreatePacket(216);
                        base.streamClass.FormatPacket();
                    }
                }
                else
                {
                    //showTradeBox = false;
                    //base.streamClass.CreatePacket(216);
                    //base.streamClass.FormatPacket();
                }
                mouseButtonClick = 0;
                mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!showTradeBox)
            {
                return;
            }

            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.DrawBox(byte0, byte1, 468, 12, 192);
            int l1 = 0x989898;
            gameGraphics.DrawBoxAlpha(byte0, byte1 + 12, 468, 18, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0, byte1 + 30, 8, 248, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 205, byte1 + 30, 11, 248, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 462, byte1 + 30, 6, 248, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 133, 197, 22, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 258, 197, 20, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 235, 246, 43, l1, 160);
            int j2 = 0xd0d0d0;
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 30, 197, 103, j2, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 155, 197, 103, j2, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 30, 246, 205, j2, 160);
            for (int i3 = 0; i3 < 4; i3 += 1)
            {
                gameGraphics.DrawLineX(byte0 + 8, byte1 + 30 + i3 * 34, 197, 0);
            }

            for (int i4 = 0; i4 < 4; i4 += 1)
            {
                gameGraphics.DrawLineX(byte0 + 8, byte1 + 155 + i4 * 34, 197, 0);
            }

            for (int k4 = 0; k4 < 7; k4 += 1)
            {
                gameGraphics.DrawLineX(byte0 + 216, byte1 + 30 + k4 * 34, 246, 0);
            }

            for (int j5 = 0; j5 < 6; j5 += 1)
            {
                if (j5 < 5)
                {
                    gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 30, 103, 0);
                }

                if (j5 < 5)
                {
                    gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 155, 103, 0);
                }

                gameGraphics.DrawLineY(byte0 + 216 + j5 * 49, byte1 + 30, 205, 0);
            }

            gameGraphics.DrawString("Trading with: " + tradeOtherName, byte0 + 1, byte1 + 10, 1, 0xffffff);
            gameGraphics.DrawString("Your Offer", byte0 + 9, byte1 + 27, 4, 0xffffff);
            gameGraphics.DrawString("Opponent's Offer", byte0 + 9, byte1 + 152, 4, 0xffffff);
            gameGraphics.DrawString("Your Inventory", byte0 + 216, byte1 + 27, 4, 0xffffff);
            if (!tradeWeAccepted)
            {
                gameGraphics.DrawPicture(byte0 + 217, byte1 + 238, baseInventoryPic + 25);
            }

            gameGraphics.DrawPicture(byte0 + 394, byte1 + 238, baseInventoryPic + 26);
            if (tradeOtherAccepted)
            {
                gameGraphics.DrawText("Other player", byte0 + 341, byte1 + 246, 1, 0xffffff);
                gameGraphics.DrawText("has accepted", byte0 + 341, byte1 + 256, 1, 0xffffff);
            }
            if (tradeWeAccepted)
            {
                gameGraphics.DrawText("Waiting for", byte0 + 217 + 35, byte1 + 246, 1, 0xffffff);
                gameGraphics.DrawText("other player", byte0 + 217 + 35, byte1 + 256, 1, 0xffffff);
            }
            for (int k5 = 0; k5 < inventoryItemsCount; k5 += 1)
            {
                int l5 = 217 + byte0 + (k5 % 5) * 49;
                int j6 = 31 + byte1 + (k5 / 5) * 34;
                gameGraphics.DrawImage(l5, j6, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[inventoryItems[k5]], Data.GameData.itemPictureMask[inventoryItems[k5]], 0, 0, false);
                if (Data.GameData.itemStackable[inventoryItems[k5]] == 0)
                {
                    gameGraphics.DrawString(inventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < tradeItemsOurCount; i6 += 1)
            {
                int k6 = 9 + byte0 + (i6 % 4) * 49;
                int i7 = 31 + byte1 + (i6 / 4) * 34;
                gameGraphics.DrawImage(k6, i7, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[tradeItemsOur[i6]], Data.GameData.itemPictureMask[tradeItemsOur[i6]], 0, 0, false);
                if (Data.GameData.itemStackable[tradeItemsOur[i6]] == 0)
                {
                    gameGraphics.DrawString(tradeItemOurCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (base.mouseX > k6 && base.mouseX < k6 + 48 && base.mouseY > i7 && base.mouseY < i7 + 32)
                {
                    gameGraphics.DrawString(Data.GameData.itemName[tradeItemsOur[i6]] + ": @whi@" + Data.GameData.itemDescription[tradeItemsOur[i6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < tradeItemsOtherCount; l6 += 1)
            {
                int j7 = 9 + byte0 + (l6 % 4) * 49;
                int k7 = 156 + byte1 + (l6 / 4) * 34;
                gameGraphics.DrawImage(j7, k7, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[tradeItemsOther[l6]], Data.GameData.itemPictureMask[tradeItemsOther[l6]], 0, 0, false);
                if (Data.GameData.itemStackable[tradeItemsOther[l6]] == 0)
                {
                    gameGraphics.DrawString(tradeItemOtherCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (base.mouseX > j7 && base.mouseX < j7 + 48 && base.mouseY > k7 && base.mouseY < k7 + 32)
                {
                    gameGraphics.DrawString(Data.GameData.itemName[tradeItemsOther[l6]] + ": @whi@" + Data.GameData.itemDescription[tradeItemsOther[l6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

        }

        public void AutoRotateCamera()
        {
            if ((cameraAutoAngle & 1) == 1 && IsValidCameraAngle(cameraAutoAngle))
            {
                return;
            }

            if ((cameraAutoAngle & 1) == 0 && IsValidCameraAngle(cameraAutoAngle))
            {
                if (IsValidCameraAngle(cameraAutoAngle + 1 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 1 & 7;
                    return;
                }
                if (IsValidCameraAngle(cameraAutoAngle + 7 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 7 & 7;
                }

                return;
            }
            int[] ai = [
            1, -1, 2, -2, 3, -3, 4
        ];
            for (int l = 0; l < 7; l += 1)
            {
                if (!IsValidCameraAngle(cameraAutoAngle + ai[l] + 8 & 7))
                {
                    continue;
                }

                cameraAutoAngle = cameraAutoAngle + ai[l] + 8 & 7;
                break;
            }

            if ((cameraAutoAngle & 1) == 0 && IsValidCameraAngle(cameraAutoAngle))
            {
                if (IsValidCameraAngle(cameraAutoAngle + 1 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 1 & 7;
                    return;
                }
                if (IsValidCameraAngle(cameraAutoAngle + 7 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 7 & 7;
                }

                return;
            }
            else
            {
                return;
            }
        }

        //public string getParameter(string s1) {
        //    if(Link.gameApplet is not null)
        //        return Link.gameApplet.getParameter(s1);
        //    else
        //        return base.getParameter(s1);
        //}

        public void DrawLogoutBox()
        {
            gameGraphics.DrawBox(126, 137, 260, 60, 0);
            gameGraphics.DrawBoxEdge(126, 137, 260, 60, 0xffffff);
            gameGraphics.DrawText("Logging out...", 256, 173, 5, 0xffffff);
        }

        public void WalkToGroundItem(int l, int i1, int j1, int k1, bool flag)
        {
            if (WalkToAlternate(l, i1, j1, k1, j1, k1, false, flag))
            {
                return;
            }
            else
            {
                WalkTo(l, i1, j1, k1, j1, k1, true, flag);
                return;
            }
        }

        public override void LoginScreenPrint(string s1, string s2)
        {
            if (loginScreen == 2 && loginMenuLogin is not null)
            {
                loginMenuLogin.UpdateText(loginMenuStatusText, s1 + " " + s2);
            }

            DrawLoginScreens();
            ResetTimings();
        }

        public void DrawTeleBubble(int x, int y, int j1, int k1, int l1, int i2, int j2)
        {
            int type = teleBubbleType[l1];
            int time = teleBubbleTime[l1];
            if (type == 0)
            {
                int i3 = 255 + time * 5 * 256;
                gameGraphics.DrawCircle(x + j1 / 2, y + k1 / 2, 20 + time * 2, i3, 255 - time * 5);
            }
            if (type == 1)
            {
                int j3 = 0xff0000 + time * 5 * 256;
                gameGraphics.DrawCircle(x + j1 / 2, y + k1 / 2, 10 + time, j3, 255 - time * 5);
            }
        }

        public void CheckLoginScreenInputs()
        {
            if (base.socketTimeout > 0)
            {
                base.socketTimeout -= 1;
            }

            if (loginScreen == 0)
            {
                if (loginMenuFirst is null)
                {
                    return;
                }

                if (base.lastMouseButton != 0 || base.mouseButton != 0)
                {
                    Console.WriteLine($"[CLICK] mouseX={base.mouseX} mouseY={base.mouseY} lastMouseButton={base.lastMouseButton} mouseButton={base.mouseButton}");
                }

                loginMenuFirst.MouseClick(base.mouseX, base.mouseY, base.lastMouseButton, base.mouseButton);
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

                loginNewUser.MouseClick(base.mouseX, base.mouseY, base.lastMouseButton, base.mouseButton);
                    if (loginNewUser.IsClicked(loginMenuOkButton))
                    {
                        loginScreen = 0;
                        return;
                    }
                }
                else
                    if (loginScreen == 2)
                    {
                        loginMenuLogin.MouseClick(base.mouseX, base.mouseY, base.lastMouseButton, base.mouseButton);
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

        public bool IsItemEquipped(int itemId)
        {
            for (int l = 0; l < inventoryItemsCount; l += 1)
            {
                if (inventoryItems[l] == itemId && inventoryItemEquipped[l] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public override void DrawWindow()
        {

            Paint(graphics);

            if (errorLoading)
            {
#warning add error loading event
                //var g1 = spriteBatch;//GetGraphics();
                ////g1.SetColor();
                //// g1.FillRect(0, 0, 512, 356, Color.Black);

                //// g1.SetFont(gameFont16);
                //g1.SetColor(Color.Yellow);
                //int l = 35;
                //g1.DrawString("Sorry, an error has occured whilst loading", 30, l);
                //l += 50;
                //g1.SetColor(Color.White);
                //g1.DrawString("To fix this try the following (in order):", 30, l);
                //l += 50;
                //g1.SetColor(Color.White);
                ////g1.SetFont(gameFont12);
                //g1.DrawString("1: Try closing ALL open web-browser windows, and reloading", 30, l);
                //l += 30;
                //g1.DrawString("2: Try clearing your web-browsers cache from tools->internet options", 30, l);
                //l += 30;
                //g1.DrawString("3: Try using a different game-world", 30, l);
                //l += 30;
                //g1.DrawString("4: Try rebooting your computer", 30, l);
                //l += 30;
                //g1.DrawString("5: Try selecting a different version of Java from the play-game menu", 30, l);
                SetRefreshRate(1);
                return;
            }
            if (memoryError)
            {
#warning add memory exception event
                //var g3 = spriteBatch;//GetGraphics();
                ////g3.SetColor(Color.Black);
                ////g3.FillRect(0, 0, 512, 356, Color.Black);
                ////g3.SetFont(gameFont20);
                //g3.SetColor(Color.White);
                //g3.DrawString("Error - out of memory!", 50, 50);
                //g3.DrawString("Close ALL unnecessary programs", 50, 100);
                //g3.DrawString("and windows before loading the game", 50, 150);
                //g3.DrawString("this game needs about 48meg of spare RAM", 50, 200);
                //SetRefreshRate(1);
                return;
            }
            try
            {
                if (!loggedIn)
                {
                    gameGraphics.loggedIn = false;
                    DrawLoginScreens();


                }
                if (loggedIn)
                {
                    gameGraphics.loggedIn = true;
                    DrawGame();


                    return;
                }
            }
            catch (Exception _ex)
            {
                CleanUp();
                memoryError = true;
            }
        }


        public void CleanUp()
        {
            try
            {
                if (gameGraphics is not null)
                {
                    gameGraphics.CleanUp();
                    gameGraphics.pixels = null;
                    gameGraphics = null;
                }
                if (gameCamera is not null)
                {
                    gameCamera.CleanUp();
                    gameCamera = null;
                }
                gameDataObjects = null;
                objectArray = null;
                wallObjectArray = null;
                playerBufferArray = null;
                playerArray = null;
                npcAttackingArray = null;
                npcArray = null;
                ourPlayer = null;
                if (engineHandle is not null)
                {
                    engineHandle.TileChunks = null;
                    engineHandle.wallObject = null;
                    engineHandle.roofObject = null;
                    engineHandle.currentSectionObject = null;
                    engineHandle = null;
                }
                //System.gc();
                System.GC.Collect();
                return;
            }
            catch (Exception _ex)
            {
                return;
            }
        }

        public void DrawQuestionMenu()
        {
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < questionMenuCount; l += 1)
                {
                    if (base.mouseX >= gameGraphics.TextWidth(questionMenuAnswer[l], 1) || base.mouseY <= l * 12 || base.mouseY >= 12 + l * 12)
                    {
                        continue;
                    }

                    base.streamClass.CreatePacket(154);
                    base.streamClass.AddByte(l);
                    base.streamClass.FormatPacket();
                    break;
                }

                mouseButtonClick = 0;
                showQuestionMenu = false;
                return;
            }
            for (int i1 = 0; i1 < questionMenuCount; i1 += 1)
            {
                int j1 = 65535;
                if (base.mouseX < gameGraphics.TextWidth(questionMenuAnswer[i1], 1) && base.mouseY > i1 * 12 && base.mouseY < 12 + i1 * 12)
                {
                    j1 = 0xff0000;
                }

                gameGraphics.DrawString(questionMenuAnswer[i1], 6, 12 + i1 * 12, 1, j1);
            }

        }

        public void DrawTradeConfirmBox()
        {
            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.DrawBox(byte0, byte1, 468, 16, 192);
            int l = 0x989898;
            gameGraphics.DrawBoxAlpha(byte0, byte1 + 16, 468, 246, l, 160);
            gameGraphics.DrawText("Please confirm your trade with @yel@" + DataOperations.HashToName(tradeConfirmOtherNameLong), byte0 + 234, byte1 + 12, 1, 0xffffff);
            gameGraphics.DrawText("You are about to give:", byte0 + 117, byte1 + 30, 1, 0xffff00);
            for (int i1 = 0; i1 < tradeConfigItemCount; i1 += 1)
            {
                string s1 = Data.GameData.itemName[tradeConfirmItems[i1]];
                if (Data.GameData.itemStackable[tradeConfirmItems[i1]] == 0)
                {
                    s1 = s1 + " x " + formatItemCount(tradeConfigItemsCount[i1]);
                }

                gameGraphics.DrawText(s1, byte0 + 117, byte1 + 42 + i1 * 12, 1, 0xffffff);
            }

            if (tradeConfigItemCount == 0)
            {
                gameGraphics.DrawText("Nothing!", byte0 + 117, byte1 + 42, 1, 0xffffff);
            }

            gameGraphics.DrawText("In return you will receive:", byte0 + 351, byte1 + 30, 1, 0xffff00);
            for (int j1 = 0; j1 < tradeConfirmOtherItemCount; j1 += 1)
            {
                string s2 = Data.GameData.itemName[tradeConfirmOtherItems[j1]];
                if (Data.GameData.itemStackable[tradeConfirmOtherItems[j1]] == 0)
                {
                    s2 = s2 + " x " + formatItemCount(tradeConfirmOtherItemsCount[j1]);
                }

                gameGraphics.DrawText(s2, byte0 + 351, byte1 + 42 + j1 * 12, 1, 0xffffff);
            }

            if (tradeConfirmOtherItemCount == 0)
            {
                gameGraphics.DrawText("Nothing!", byte0 + 351, byte1 + 42, 1, 0xffffff);
            }

            gameGraphics.DrawText("Are you sure you want to do this?", byte0 + 234, byte1 + 200, 4, 65535);
            gameGraphics.DrawText("There is NO WAY to reverse a trade if you change your mind.", byte0 + 234, byte1 + 215, 1, 0xffffff);
            gameGraphics.DrawText("Remember that not all players are trustworthy", byte0 + 234, byte1 + 230, 1, 0xffffff);
            if (!tradeConfirmAccepted)
            {
                gameGraphics.DrawPicture((byte0 + 118) - 35, byte1 + 238, baseInventoryPic + 25);
                gameGraphics.DrawPicture((byte0 + 352) - 35, byte1 + 238, baseInventoryPic + 26);
            }
            else
            {
                gameGraphics.DrawText("Waiting for other player...", byte0 + 234, byte1 + 250, 1, 0xffff00);
            }
            if (mouseButtonClick == 1)
            {
                if (base.mouseX < byte0 || base.mouseY < byte1 || base.mouseX > byte0 + 468 || base.mouseY > byte1 + 262)
                {
                    //showTradeConfirmBox = false;
                    //base.streamClass.CreatePacket(216);
                    //base.streamClass.FormatPacket();
                }
                if (base.mouseX >= (byte0 + 118) - 35 && base.mouseX <= byte0 + 118 + 70 && base.mouseY >= byte1 + 238 && base.mouseY <= byte1 + 238 + 21)
                {
                    tradeConfirmAccepted = true;
                    base.streamClass.CreatePacket(53);
                    base.streamClass.FormatPacket();
                }
                if (base.mouseX >= (byte0 + 352) - 35 && base.mouseX <= byte0 + 353 + 70 && base.mouseY >= byte1 + 238 && base.mouseY <= byte1 + 238 + 21)
                {
                    showTradeConfirmBox = false;
                    base.streamClass.CreatePacket(216);
                    base.streamClass.FormatPacket();
                }
                mouseButtonClick = 0;
            }
        }

        public void DrawLoginScreens()
        {
            loginScreenShown = false;
            if (gameGraphics is null)
            {
                return;
            }

            gameGraphics.interlace = false;
            gameGraphics.ClearScreen();
            if (loginScreen == 0 || loginScreen == 1 || loginScreen == 2 || loginScreen == 3)
            {
                int l = (tick * 2) % 3072;
                if (l < 1024)
                {
                    gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic);
                    if (l > 768)
                    {
                        gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic + 1, l - 768);
                    }
                }
                else if (l < 2048)
                {
                    gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic + 1);
                    if (l > 1792)
                    {
                        gameGraphics.DrawPicture(0, 10, baseInventoryPic + 10, l - 1792);
                    }
                }
                else
                {
                    gameGraphics.DrawPicture(0, 10, baseInventoryPic + 10);
                    if (l > 2816)
                    {
                        gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic, l - 2816);
                    }
                }
            }
            if (loginMenuFirst is null)
            {
                return;
            }

            if (loginScreen == 0)
            {
                loginMenuFirst.DrawMenu();
            }

            if (loginScreen == 1)
            {
                loginNewUser.DrawMenu();
            }

            if (loginScreen == 2)
            {
                loginMenuLogin.DrawMenu();
            }

            gameGraphics.DrawPicture(0, windowHeight, baseInventoryPic + 22);



            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.DrawImage(spriteBatch, 0, 0);
        }

        public void DrawItem(int x, int y, int width, int height, int itemID, int i2, int j2)
        {
            int picture = Data.GameData.itemInventoryPicture[itemID] + baseItemPicture;
            int mask = Data.GameData.itemPictureMask[itemID];
            gameGraphics.DrawImage(x, y, width, height, picture, mask, 0, 0, false);
        }

        public ClientMob CreatePlayer(int index, int x, int y, int sprite)
        {
            if (playerBufferArray[index] is null)
            {
                playerBufferArray[index] = new ClientMob
                {
                    serverIndex = index,
                    serverID = 0
                };
            }
            ClientMob existingPlayer = playerBufferArray[index];
            bool flag = false;
            for (int l = 0; l < lastPlayerCount; l += 1)
            {
                if (lastPlayerArray[l].serverIndex != index)
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (flag)
            {
                existingPlayer.nextSprite = sprite;
                int i1 = existingPlayer.waypointCurrent;
                if (x != existingPlayer.waypointsX[i1] || y != existingPlayer.waypointsY[i1])
                {
                    existingPlayer.waypointCurrent = i1 = (i1 + 1) % 10;
                    existingPlayer.waypointsX[i1] = x;
                    existingPlayer.waypointsY[i1] = y;
                }
            }
            else
            {
                existingPlayer.serverIndex = index;
                existingPlayer.waypointsEndSprite = 0;
                existingPlayer.waypointCurrent = 0;
                existingPlayer.waypointsX[0] = existingPlayer.currentX = x;
                existingPlayer.waypointsY[0] = existingPlayer.currentY = y;
                existingPlayer.nextSprite = existingPlayer.currentSprite = sprite;
                existingPlayer.stepCount = 0;
            }
            playerArray[playerCount++] = existingPlayer;
            return existingPlayer;
        }

        public void WalkTo1Tile(int l, int i1, int j1, int k1, bool flag)
        {
            WalkTo(l, i1, j1, k1, j1, k1, false, flag);
        }

        public void LoadConfig()
        {
            sbyte[] abyte0 = UnpackData("config.jag", "Configuration", 10);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            Data.GameData.Load(abyte0);
            sbyte[] abyte1 = UnpackData("filter.jag", "Chat system", 15);
            if (abyte1 is null)
            {
                errorLoading = true;
                return;
            }
            else
            {
                sbyte[] abyte2 = DataOperations.LoadData("fragmentsenc.txt", 0, abyte1);
                sbyte[] abyte3 = DataOperations.LoadData("badenc.txt", 0, abyte1);
                sbyte[] abyte4 = DataOperations.LoadData("hostenc.txt", 0, abyte1);
                sbyte[] abyte5 = DataOperations.LoadData("tldlist.txt", 0, abyte1);
                //ChatFilter.addFilterData(new DataEncryption(abyte2), new DataEncryption(abyte3), new DataEncryption(abyte4), new DataEncryption(abyte5));
                return;
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

        public void DrawFriendsMenu(bool canClick)
        {
            int l = ((GameImage)(gameGraphics)).gameWidth - 199;
            int i1 = 36;
            gameGraphics.DrawPicture(l - 49, 3, baseInventoryPic + 5);
            int c1 = 196;//(char)304;//'\u304';
            int c2 = 182;//(char)266;//'\u266';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (friendsIgnoreMenuSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            gameGraphics.DrawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.DrawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.DrawBoxAlpha(l, i1 + 24, c1, c2 - 24, GameImage.RgbToInt(220, 220, 220), 128);
            gameGraphics.DrawLineX(l, i1 + 24, c1, 0);
            gameGraphics.DrawLineY(l + c1 / 2, i1, 24, 0);
            gameGraphics.DrawLineX(l, (i1 + c2) - 16, c1, 0);
            gameGraphics.DrawText("Friends", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.DrawText("Ignore", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            friendsMenu.ClearList(friendsMenuHandle);
            if (friendsIgnoreMenuSelected == 0)
            {
                for (int l1 = 0; l1 < base.friendsCount; l1 += 1)
                {
                    string s1;
                    if (base.friendsWorld[l1] == 99)
                    {
                        s1 = "@gre@";
                    }
                    else
                        if (base.friendsWorld[l1] > 0)
                    {
                        s1 = "@yel@";
                    }
                    else
                    {
                        s1 = "@red@";
                    }

                    friendsMenu.AddListItem(friendsMenuHandle, l1, s1 + DataOperations.HashToName(base.friendsList[l1]) + "~439~@whi@Remove         WWWWWWWWWW");
                }

            }
            if (friendsIgnoreMenuSelected == 1)
            {
                for (int i2 = 0; i2 < base.ignoresCount; i2 += 1)
                {
                    friendsMenu.AddListItem(friendsMenuHandle, i2, "@yel@" + DataOperations.HashToName(base.ignoresList[i2]) + "~439~@whi@Remove         WWWWWWWWWW");
                }
            }
            friendsMenu.DrawMenu();
            if (friendsIgnoreMenuSelected == 0)
            {
                int j2 = friendsMenu.GetEntryHighlighted(friendsMenuHandle);
                if (j2 >= 0 && base.mouseX < 489)
                {
                    if (base.mouseX > 429)
                    {
                        gameGraphics.DrawText("Click to remove " + DataOperations.HashToName(base.friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                        if (base.friendsWorld[j2] == 99)
                    {
                        gameGraphics.DrawText("Click to message " + DataOperations.HashToName(base.friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                            if (base.friendsWorld[j2] > 0)
                    {
                        gameGraphics.DrawText(DataOperations.HashToName(base.friendsList[j2]) + " is on world " + base.friendsWorld[j2], l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                    {
                        gameGraphics.DrawText(DataOperations.HashToName(base.friendsList[j2]) + " is offline", l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    gameGraphics.DrawText("Click a name to send a message", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int j3;
                if (base.mouseX > l && base.mouseX < l + c1 && base.mouseY > (i1 + c2) - 16 && base.mouseY < i1 + c2)
                {
                    j3 = 0xffff00;
                }
                else
                {
                    j3 = 0xffffff;
                }

                gameGraphics.DrawText("Click here to add a friend", l + c1 / 2, (i1 + c2) - 3, 1, j3);
            }
            if (friendsIgnoreMenuSelected == 1)
            {
                int k2 = friendsMenu.GetEntryHighlighted(friendsMenuHandle);
                if (k2 >= 0 && base.mouseX < 489 && base.mouseX > 429)
                {
                    if (base.mouseX > 429)
                    {
                        gameGraphics.DrawText("Click to remove " + DataOperations.HashToName(base.ignoresList[k2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    gameGraphics.DrawText("Blocking messages from:", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int k3;
                if (base.mouseX > l && base.mouseX < l + c1 && base.mouseY > (i1 + c2) - 16 && base.mouseY < i1 + c2)
                {
                    k3 = 0xffff00;
                }
                else
                {
                    k3 = 0xffffff;
                }

                gameGraphics.DrawText("Click here to add a name", l + c1 / 2, (i1 + c2) - 3, 1, k3);
            }
            if (!canClick)
            {
                return;
            }

            l = base.mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = base.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                friendsMenu.MouseClick(l + (((GameImage)(gameGraphics)).gameWidth - 199), i1 + 36, base.lastMouseButton, base.mouseButton);
                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98 && friendsIgnoreMenuSelected == 1)
                    {
                        friendsIgnoreMenuSelected = 0;
                        friendsMenu.SwitchList(friendsMenuHandle);
                    }
                    else
                        if (l > 98 && friendsIgnoreMenuSelected == 0)
                        {
                            friendsIgnoreMenuSelected = 1;
                            friendsMenu.SwitchList(friendsMenuHandle);
                        }
                }

                if (mouseButtonClick == 1 && friendsIgnoreMenuSelected == 0)
                {
                    int l2 = friendsMenu.GetEntryHighlighted(friendsMenuHandle);
                    if (l2 >= 0 && base.mouseX < 489)
                    {
                        if (base.mouseX > 429)
                        {
                            RemoveFriend(base.friendsList[l2]);
                        }
                        else
                            if (base.friendsWorld[l2] != 0)
                            {
                                showFriendsBox = 2;
                                pmTarget = base.friendsList[l2];
                                base.pmText = "";
                                base.enteredPMText = "";
                            }
                    }
                }
                if (mouseButtonClick == 1 && friendsIgnoreMenuSelected == 1)
                {
                    int i3 = friendsMenu.GetEntryHighlighted(friendsMenuHandle);
                    if (i3 >= 0 && base.mouseX < 489 && base.mouseX > 429)
                    {
                        RemoveIgnore(base.ignoresList[i3]);
                    }
                }
                if (i1 > 166 && mouseButtonClick == 1 && friendsIgnoreMenuSelected == 0)
                {
                    showFriendsBox = 1;
                    base.inputText = "";
                    base.enteredInputText = "";
                }
                if (i1 > 166 && mouseButtonClick == 1 && friendsIgnoreMenuSelected == 1)
                {
                    showFriendsBox = 3;
                    base.inputText = "";
                    base.enteredInputText = "";
                }
                mouseButtonClick = 0;
            }
        }

        public void DrawPrayerMagicMenu(bool canClick)
        {
            int l = ((GameImage)(gameGraphics)).gameWidth - 199;
            int i1 = 36;
            gameGraphics.DrawPicture(l - 49, 3, baseInventoryPic + 4);
            int c1 = 196;//'\u304';
            int c2 = 182;//'\u266';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (menuMagicPrayersSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            gameGraphics.DrawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.DrawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.DrawBoxAlpha(l, i1 + 24, c1, 90, GameImage.RgbToInt(220, 220, 220), 128);
            gameGraphics.DrawBoxAlpha(l, i1 + 24 + 90, c1, c2 - 90 - 24, GameImage.RgbToInt(160, 160, 160), 128);
            gameGraphics.DrawLineX(l, i1 + 24, c1, 0);
            gameGraphics.DrawLineY(l + c1 / 2, i1, 24, 0);
            gameGraphics.DrawLineX(l, i1 + 113, c1, 0);
            gameGraphics.DrawText("Magic", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.DrawText("Prayers", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            if (menuMagicPrayersSelected == 0)
            {
                spellMenu.ClearList(spellMenuHandle);
                int l1 = 0;
                for (int l2 = 0; l2 < Data.GameData.spellCount; l2 += 1)
                {
                    string s1 = "@yel@";
                    for (int k4 = 0; k4 < Data.GameData.spellDifferentRuneCount[l2]; k4 += 1)
                    {
                        int j5 = Data.GameData.spellRequiredRuneIds[l2][k4];
                        if (HasRequiredRunes(j5, Data.GameData.spellRequiredRuneCount[l2][k4]))
                        {
                            continue;
                        }

                        s1 = "@whi@";
                        break;
                    }

                    int k5 = playerStatCurrent[6];
                    if (Data.GameData.spellRequiredLevel[l2] > k5)
                    {
                        s1 = "@normalZ@";
                    }

                    spellMenu.AddListItem(spellMenuHandle, l1++, s1 + "Level " + Data.GameData.spellRequiredLevel[l2] + ": " + Data.GameData.spellName[l2]);
                }

                spellMenu.DrawMenu();
                int l3 = spellMenu.GetEntryHighlighted(spellMenuHandle);
                if (l3 != -1)
                {
                    gameGraphics.DrawString("Level " + Data.GameData.spellRequiredLevel[l3] + ": " + Data.GameData.spellName[l3], l + 2, i1 + 124, 1, 0xffff00);
                    gameGraphics.DrawString(Data.GameData.spellDescription[l3], l + 2, i1 + 136, 0, 0xffffff);
                    for (int l4 = 0; l4 < Data.GameData.spellDifferentRuneCount[l3]; l4 += 1)
                    {
                        int l5 = Data.GameData.spellRequiredRuneIds[l3][l4];
                        gameGraphics.DrawPicture(l + 2 + l4 * 44, i1 + 150, baseItemPicture + Data.GameData.itemInventoryPicture[l5]);
                        int i6 = GetInventoryItemTotalCount(l5);
                        int j6 = Data.GameData.spellRequiredRuneCount[l3][l4];
                        string s3 = "@red@";
                        if (HasRequiredRunes(l5, j6))
                        {
                            s3 = "@gre@";
                        }

                        gameGraphics.DrawString(s3 + i6 + "/" + j6, l + 2 + l4 * 44, i1 + 150, 1, 0xffffff);
                    }

                }
                else
                {
                    gameGraphics.DrawString("Point at a spell for a description", l + 2, i1 + 124, 1, 0);
                }
            }
            if (menuMagicPrayersSelected == 1)
            {
                spellMenu.ClearList(spellMenuHandle);
                int i2 = 0;
                for (int i3 = 0; i3 < Data.GameData.prayerCount; i3 += 1)
                {
                    string s2 = "@whi@";
                    if (Data.GameData.prayerRequiredLevel[i3] > playerStatBase[5])
                    {
                        s2 = "@normalZ@";
                    }

                    if (prayerOn[i3])
                    {
                        s2 = "@gre@";
                    }

                    spellMenu.AddListItem(spellMenuHandle, i2++, s2 + "Level " + Data.GameData.prayerRequiredLevel[i3] + ": " + Data.GameData.prayerName[i3]);
                }

                spellMenu.DrawMenu();
                int i4 = spellMenu.GetEntryHighlighted(spellMenuHandle);
                if (i4 != -1)
                {
                    gameGraphics.DrawText("Level " + Data.GameData.prayerRequiredLevel[i4] + ": " + Data.GameData.prayerName[i4], l + c1 / 2, i1 + 130, 1, 0xffff00);
                    gameGraphics.DrawText(Data.GameData.prayerDescription[i4], l + c1 / 2, i1 + 145, 0, 0xffffff);
                    gameGraphics.DrawText("Drain rate: " + Data.GameData.prayerDrainRate[i4], l + c1 / 2, i1 + 160, 1, 0);
                }
                else
                {
                    gameGraphics.DrawString("Point at a prayer for a description", l + 2, i1 + 124, 1, 0);
                }
            }
            if (!canClick)
            {
                return;
            }

            l = base.mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = base.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                spellMenu.MouseClick(l + (((GameImage)(gameGraphics)).gameWidth - 199), i1 + 36, base.lastMouseButton, base.mouseButton);
                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98 && menuMagicPrayersSelected == 1)
                    {
                        menuMagicPrayersSelected = 0;
                        spellMenu.SwitchList(spellMenuHandle);
                    }
                    else
                        if (l > 98 && menuMagicPrayersSelected == 0)
                        {
                            menuMagicPrayersSelected = 1;
                            spellMenu.SwitchList(spellMenuHandle);
                        }
                }

                if (mouseButtonClick == 1 && menuMagicPrayersSelected == 0)
                {
                    int j2 = spellMenu.GetEntryHighlighted(spellMenuHandle);
                    if (j2 != -1)
                    {
                        int j3 = playerStatCurrent[6];
                        if (Data.GameData.spellRequiredLevel[j2] > j3)
                        {
                            DisplayMessage("Your magic ability is not high enough for this spell", 3);
                        }
                        else
                        {
                            int j4;
                            for (j4 = 0; j4 < Data.GameData.spellDifferentRuneCount[j2]; j4 += 1)
                            {
                                int i5 = Data.GameData.spellRequiredRuneIds[j2][j4];
                                if (HasRequiredRunes(i5, Data.GameData.spellRequiredRuneCount[j2][j4]))
                                {
                                    continue;
                                }

                                DisplayMessage("You don't have all the reagents you need for this spell", 3);
                                j4 = -1;
                                break;
                            }

                            if (j4 == Data.GameData.spellDifferentRuneCount[j2])
                            {
                                selectedSpell = j2;
                                selectedItem = -1;
                            }
                        }
                    }
                }
                if (mouseButtonClick == 1 && menuMagicPrayersSelected == 1)
                {
                    int k2 = spellMenu.GetEntryHighlighted(spellMenuHandle);
                    if (k2 != -1)
                    {
                        int k3 = playerStatBase[5];
                        if (Data.GameData.prayerRequiredLevel[k2] > k3)
                        {
                            DisplayMessage("Your prayer ability is not high enough for this prayer", 3);
                        }
                        else
                            if (playerStatCurrent[5] == 0)
                        {
                            DisplayMessage("You have Run out of prayer points. Return to a church to recharge", 3);
                        }
                        else
                                if (prayerOn[k2])
                                {
                                    base.streamClass.CreatePacket(248);
                                    base.streamClass.AddByte(k2);
                                    base.streamClass.FormatPacket();
                                    prayerOn[k2] = false;
                                    PlaySound("prayeroff");
                                }
                                else
                                {
                                    base.streamClass.CreatePacket(56);
                                    base.streamClass.AddByte(k2);
                                    base.streamClass.FormatPacket();
                                    prayerOn[k2] = true;
                                    PlaySound("prayeron");
                                }
                    }
                }
                mouseButtonClick = 0;
            }
        }

        public override sbyte[] UnpackData(string fileName, string fileTitle, int progressPercentage)
        {
            sbyte[] abyte0 = Link.GetFile(fileName);
            if (abyte0 is not null)
            {
                int l = ((abyte0[0] & 0xff) << 16) + ((abyte0[1] & 0xff) << 8) + (abyte0[2] & 0xff);
                int i1 = ((abyte0[3] & 0xff) << 16) + ((abyte0[4] & 0xff) << 8) + (abyte0[5] & 0xff);

                sbyte[] abyte1 = new sbyte[abyte0.Length - 6];
                for (int j1 = 0; j1 < abyte0.Length - 6; j1 += 1)
                {
                    abyte1[j1] = abyte0[j1 + 6];
                }

                DrawLoadingBarText(progressPercentage, "Unpacking " + fileTitle);
                if (i1 != l)
                {
                    sbyte[] abyte2 = new sbyte[l];
                    DataFileDecrypter.UnpackData(abyte2, l, abyte1, i1, 0);
                    if (OnContentLoaded is not null)
                    {
                        OnContentLoaded(this, new ContentLoadedEventArgs("Unpacking " + fileTitle, progressPercentage));
                    }
                    return abyte2;
                }
                else
                {
                    if (OnContentLoaded is not null)
                    {
                        OnContentLoaded(this, new ContentLoadedEventArgs("Unpacking " + fileTitle, progressPercentage));
                    }
                    return abyte1;
                }
            }
            else
            {
                if (OnContentLoaded is not null)
                {
                    OnContentLoaded(this, new ContentLoadedEventArgs("Unpacking " + fileTitle, progressPercentage));
                }
                return base.UnpackData(fileName, fileTitle, progressPercentage);
            }
        }

        public void DrawChatMessageTabs()
        {
            gameGraphics.DrawPicture(0, windowHeight - 4, baseInventoryPic + 23);
            int l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 0)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabAllMsgFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.DrawText("All messages", 54, windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 1)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabHistoryFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.DrawText("Chat history", 155, windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 2)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabQuestFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.DrawText("Quest history", 255, windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 3)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabPrivateFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.DrawText("Private history", 355, windowHeight + 6, 0, l);
            gameGraphics.DrawText("Report abuse", 457, windowHeight + 6, 0, 0xffffff);
        }

        //public URL getDocumentBase() {
        //    if(Link.gameApplet is not null)
        //        return Link.gameApplet.getDocumentBase();
        //    else
        //        return base.getDocumentBase();
        //}

        private readonly Lock _sync = new();
        public static bool sendingPing = false;
        public void SendPingPacketAsync()
        {
            lock (_sync)
            {
                if (sendingPing)
                {
                    return;
                }

                sendingPing = true;
            }

            Task.Run(() =>
            {
                try
                {
                    SendPingPacket();
                }
                finally
                {
                    lock (_sync)
                    {
                        sendingPing = false;
                    }
                    OnMyTaskCompleted(new AsyncCompletedEventArgs(null, false, null));
                }
            });
        }

        public event AsyncCompletedEventHandler MyTaskCompleted;

        protected void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted is not null)
            {
                MyTaskCompleted(this, e);
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
                if (base.enteredInputText.Length > 0)
                {
                    if (base.enteredInputText.ToLower() == "::lostcon")
                    {
                        base.streamClass.CloseStream();
                    }
                    else
                        if (base.enteredInputText.ToLower() == "::closecon")
                        {
                            RequestLogout();
                        }
                        else
                        {
                            base.streamClass.CreatePacket(200);
                            base.streamClass.AddString(base.enteredInputText);
                            if (!sleepWordDelay)
                            {
                                base.streamClass.AddByte(0);
                                sleepWordDelay = true;
                            }
                            base.streamClass.FormatPacket();
                            base.inputText = "";
                            base.enteredInputText = "";
                            sleepingStatusText = "Please wait...";
                        }
                }

                if (base.lastMouseButton == 1 && base.mouseY > 275 && base.mouseY < 310 && base.mouseX > 56 && base.mouseX < 456)
                {
                    base.streamClass.CreatePacket(200);
                    base.streamClass.AddString("-null-");
                    if (!sleepWordDelay)
                    {
                        base.streamClass.AddByte(0);
                        sleepWordDelay = true;
                    }
                    base.streamClass.FormatPacket();
                    base.inputText = "";
                    base.enteredInputText = "";
                    sleepingStatusText = "Please wait...";
                }
                base.lastMouseButton = 0;
                return;
            }
            if (base.mouseY > windowHeight - 4)
            {
                if (base.mouseX > 15 && base.mouseX < 96 && base.lastMouseButton == 1)
                {
                    messagesTab = 0;
                }

                if (base.mouseX > 110 && base.mouseX < 194 && base.lastMouseButton == 1)
                {
                    messagesTab = 1;
                    chatInputMenu.listShownEntries[messagesHandleType2] = 0xf423f;
                }
                if (base.mouseX > 215 && base.mouseX < 295 && base.lastMouseButton == 1)
                {
                    messagesTab = 2;
                    chatInputMenu.listShownEntries[messagesHandleType5] = 0xf423f;
                }
                if (base.mouseX > 315 && base.mouseX < 395 && base.lastMouseButton == 1)
                {
                    messagesTab = 3;
                    chatInputMenu.listShownEntries[messagesHandleType6] = 0xf423f;
                }
                if (base.mouseX > 417 && base.mouseX < 497 && base.lastMouseButton == 1)
                {
                    showAbuseBox = 1;
                    reportAbuseOptionSelected = 0;
                    base.inputText = "";
                    base.enteredInputText = "";
                }
                base.lastMouseButton = 0;
                base.mouseButton = 0;
            }
            chatInputMenu.MouseClick(base.mouseX, base.mouseY, base.lastMouseButton, base.mouseButton);
            if (messagesTab > 0 && base.mouseX >= 494 && base.mouseY >= windowHeight - 66)
            {
                base.lastMouseButton = 0;
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
                base.lastMouseButton = 0;
            }

            if (showTradeBox || showDuelBox)
            {
                if (base.mouseButton != 0)
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
            if (base.lastMouseButton == 1)
            {
                mouseButtonClick = 1;
            }
            else if (base.lastMouseButton == 2)
            {
                mouseButtonClick = 2;
            }

            gameCamera.SetMousePosition(base.mouseX, base.mouseY);
            base.lastMouseButton = 0;
            if (configCameraAutoAngle)
            {
                if (cameraAutoRotationAmount == 0 || cameraAutoAngleDebug)
                {
                    if (base.keyLeftDown)
                    {
                        cameraAutoAngle = cameraAutoAngle + 1 & 7;
                        base.keyLeftDown = false;
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
                    if (base.keyRightDown)
                    {
                        cameraAutoAngle = cameraAutoAngle + 7 & 7;
                        base.keyRightDown = false;
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
            else if (base.keyLeftDown)
            {
                cameraRotation = cameraRotation + 2 & 0xff;
            }
            else if (base.keyRightDown)
            {
                cameraRotation = cameraRotation - 2 & 0xff;
            }

            if (base.keyUpDown && cameraDistance > 550)
            {
                cameraDistance -= 4;
            }
            else if (base.keyDownDown && cameraDistance < 1250)
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

        public void CreateAppearanceWindow()
        {
            appearanceMenu = new Menu(gameGraphics, 100);
            appearanceMenu.DrawText(256, 10, "Please design Your Character", 4, true);
            int l = 140;
            int i1 = 34;
            l += 116;
            i1 -= 10;
            appearanceMenu.DrawText(l - 55, i1 + 110, "Front", 3, true);
            appearanceMenu.DrawText(l, i1 + 110, "Side", 3, true);
            appearanceMenu.DrawText(l + 55, i1 + 110, "Back", 3, true);
            sbyte byte0 = 54;
            i1 += 145;
            appearanceMenu.DrawCurvedBox(l - byte0, i1, 53, 41);
            appearanceMenu.DrawText(l - byte0, i1 - 8, "Head", 1, true);
            appearanceMenu.DrawText(l - byte0, i1 + 8, "Type", 1, true);
            appearanceMenu.DrawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            appearanceHeadLeftArrow = appearanceMenu.CreateButton(l - byte0 - 40, i1, 20, 20);
            appearanceMenu.DrawArrow((l - byte0) + 40, i1, Menu.baseScrollPic + 6);
            appearanceHeadRightArrow = appearanceMenu.CreateButton((l - byte0) + 40, i1, 20, 20);
            appearanceMenu.DrawCurvedBox(l + byte0, i1, 53, 41);
            appearanceMenu.DrawText(l + byte0, i1 - 8, "Hair", 1, true);
            appearanceMenu.DrawText(l + byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.DrawArrow((l + byte0) - 40, i1, Menu.baseScrollPic + 7);
            appearanceHairLeftArrow = appearanceMenu.CreateButton((l + byte0) - 40, i1, 20, 20);
            appearanceMenu.DrawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            appearanceHairRightArrow = appearanceMenu.CreateButton(l + byte0 + 40, i1, 20, 20);
            i1 += 50;
            appearanceMenu.DrawCurvedBox(l - byte0, i1, 53, 41);
            appearanceMenu.DrawText(l - byte0, i1, "Gender", 1, true);
            appearanceMenu.DrawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            appearanceGenderLeftArrow = appearanceMenu.CreateButton(l - byte0 - 40, i1, 20, 20);
            appearanceMenu.DrawArrow((l - byte0) + 40, i1, Menu.baseScrollPic + 6);
            appearanceGenderRightArrow = appearanceMenu.CreateButton((l - byte0) + 40, i1, 20, 20);
            appearanceMenu.DrawCurvedBox(l + byte0, i1, 53, 41);
            appearanceMenu.DrawText(l + byte0, i1 - 8, "Top", 1, true);
            appearanceMenu.DrawText(l + byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.DrawArrow((l + byte0) - 40, i1, Menu.baseScrollPic + 7);
            appearanceTopLeftArrow = appearanceMenu.CreateButton((l + byte0) - 40, i1, 20, 20);
            appearanceMenu.DrawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            appearanceTopRightArrow = appearanceMenu.CreateButton(l + byte0 + 40, i1, 20, 20);
            i1 += 50;
            appearanceMenu.DrawCurvedBox(l - byte0, i1, 53, 41);
            appearanceMenu.DrawText(l - byte0, i1 - 8, "Skin", 1, true);
            appearanceMenu.DrawText(l - byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.DrawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            appearanceSkinLeftArrow = appearanceMenu.CreateButton(l - byte0 - 40, i1, 20, 20);
            appearanceMenu.DrawArrow((l - byte0) + 40, i1, Menu.baseScrollPic + 6);
            appearanceSkingRightArrow = appearanceMenu.CreateButton((l - byte0) + 40, i1, 20, 20);
            appearanceMenu.DrawCurvedBox(l + byte0, i1, 53, 41);
            appearanceMenu.DrawText(l + byte0, i1 - 8, "Bottom", 1, true);
            appearanceMenu.DrawText(l + byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.DrawArrow((l + byte0) - 40, i1, Menu.baseScrollPic + 7);
            appearanceBottomLeftArrow = appearanceMenu.CreateButton((l + byte0) - 40, i1, 20, 20);
            appearanceMenu.DrawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            appearanceBottomRightArrow = appearanceMenu.CreateButton(l + byte0 + 40, i1, 20, 20);
            i1 += 82;
            i1 -= 35;
            appearanceMenu.DrawButton(l, i1, 200, 30);
            appearanceMenu.DrawText(l, i1, "Accept", 4, false);
            appearanceAcceptButton = appearanceMenu.CreateButton(l, i1, 200, 30);
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

        public void GenerateWorldRightClickMenu()
        {
            int l = 2203 - (sectionY + wildY + areaY);
            if (sectionX + wildX + areaX >= 2640)
            {
                l = -50;
            }

            int ground = -1;
            for (int j1 = 0; j1 < objectCount; j1 += 1)
            {
                objectAlreadyInMenu[j1] = false;
            }

            for (int k1 = 0; k1 < wallObjectCount; k1 += 1)
            {
                wallObjectAlreadyInMenu[k1] = false;
            }

            int optionCount = gameCamera.GetOptionCount();
            GameObject[] objects = gameCamera.GetHighlightedObjects();
            int[] players = gameCamera.GetHighlightedPlayers();
            for (int i2 = 0; i2 < optionCount; i2 += 1)
            {
                if (menuOptionsCount > 200)
                {
                    break;
                }

                int player = players[i2];
                GameObject _obj = objects[i2];
                if (_obj.entityType[player] <= 65535 || _obj.entityType[player] >= 0x30d40 && _obj.entityType[player] <= 0x493e0)
                {
                    if (_obj == gameCamera.highlightedObject)
                    {
                        int index = _obj.entityType[player] % 10000;
                        int type = _obj.entityType[player] / 10000;
                        if (type == 1)
                        {
                            string s1 = "";
                            int k4 = 0;
                            if (ourPlayer.level > 0 && playerArray[index].level > 0)
                            {
                                k4 = ourPlayer.level - playerArray[index].level;
                            }

                            if (k4 < 0)
                            {
                                s1 = "@or1@";
                            }

                            if (k4 < -3)
                            {
                                s1 = "@or2@";
                            }

                            if (k4 < -6)
                            {
                                s1 = "@or3@";
                            }

                            if (k4 < -9)
                            {
                                s1 = "@red@";
                            }

                            if (k4 > 0)
                            {
                                s1 = "@gr1@";
                            }

                            if (k4 > 3)
                            {
                                s1 = "@gr2@";
                            }

                            if (k4 > 6)
                            {
                                s1 = "@gr3@";
                            }

                            if (k4 > 9)
                            {
                                s1 = "@gre@";
                            }

                            s1 = " " + s1 + "(level-" + playerArray[index].level + ")";
                            if (selectedSpell >= 0)
                            {
                                if (Data.GameData.spellType[selectedSpell] == 1 || Data.GameData.spellType[selectedSpell] == 2)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on";
                                    menuText2[menuOptionsCount] = "@whi@" + playerArray[index].username + s1;
                                    menuActionID[menuOptionsCount] = 800;
                                    menuActionX[menuOptionsCount] = playerArray[index].currentX;
                                    menuActionY[menuOptionsCount] = playerArray[index].currentY;
                                    menuActionType[menuOptionsCount] = playerArray[index].serverIndex;
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount += 1;
                                }
                            }
                            else
                                if (selectedItem >= 0)
                                {
                                    menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                    menuText2[menuOptionsCount] = "@whi@" + playerArray[index].username + s1;
                                    menuActionID[menuOptionsCount] = 810;
                                    menuActionX[menuOptionsCount] = playerArray[index].currentX;
                                    menuActionY[menuOptionsCount] = playerArray[index].currentY;
                                    menuActionType[menuOptionsCount] = playerArray[index].serverIndex;
                                    menuActionVar1[menuOptionsCount] = selectedItem;
                                    menuOptionsCount += 1;
                                }
                                else
                                {
                                    if (l > 0 && (playerArray[index].currentY - 64) / gridSize + wildY + areaY < 2203)
                                    {
                                        menuText1[menuOptionsCount] = "Attack";
                                        menuText2[menuOptionsCount] = "@whi@" + playerArray[index].username + s1;
                                        if (k4 >= 0 && k4 < 5)
                                    {
                                        menuActionID[menuOptionsCount] = 805;
                                    }
                                    else
                                    {
                                        menuActionID[menuOptionsCount] = 2805;
                                    }

                                    menuActionX[menuOptionsCount] = playerArray[index].currentX;
                                        menuActionY[menuOptionsCount] = playerArray[index].currentY;
                                        menuActionType[menuOptionsCount] = playerArray[index].serverIndex;
                                        menuOptionsCount += 1;
                                    }
                                    else
                                        if (Config.MembersFeatures)
                                        {
                                            menuText1[menuOptionsCount] = "Duel with";
                                            menuText2[menuOptionsCount] = "@whi@" + playerArray[index].username + s1;
                                            menuActionX[menuOptionsCount] = playerArray[index].currentX;
                                            menuActionY[menuOptionsCount] = playerArray[index].currentY;
                                            menuActionID[menuOptionsCount] = 2806;
                                            menuActionType[menuOptionsCount] = playerArray[index].serverIndex;
                                            menuOptionsCount += 1;
                                        }
                                    menuText1[menuOptionsCount] = "Trade with";
                                    menuText2[menuOptionsCount] = "@whi@" + playerArray[index].username + s1;
                                    menuActionID[menuOptionsCount] = 2810;
                                    menuActionType[menuOptionsCount] = playerArray[index].serverIndex;
                                    menuOptionsCount += 1;
                                    menuText1[menuOptionsCount] = "Follow";
                                    menuText2[menuOptionsCount] = "@whi@" + playerArray[index].username + s1;
                                    menuActionID[menuOptionsCount] = 2820;
                                    menuActionType[menuOptionsCount] = playerArray[index].serverIndex;
                                    menuOptionsCount += 1;
                                }
                        }
                        else
                            if (type == 2)
                            {
                                if (selectedSpell >= 0)
                                {
                                    if (Data.GameData.spellType[selectedSpell] == 3)
                                    {
                                        menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on";
                                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[groundItemID[index]];
                                        menuActionID[menuOptionsCount] = 200;
                                        menuActionX[menuOptionsCount] = groundItemX[index];
                                        menuActionY[menuOptionsCount] = groundItemY[index];
                                        menuActionType[menuOptionsCount] = groundItemID[index];
                                        menuActionVar1[menuOptionsCount] = selectedSpell;
                                        menuOptionsCount += 1;
                                    }
                                }
                                else
                                    if (selectedItem >= 0)
                                    {
                                        menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[groundItemID[index]];
                                        menuActionID[menuOptionsCount] = 210;
                                        menuActionX[menuOptionsCount] = groundItemX[index];
                                        menuActionY[menuOptionsCount] = groundItemY[index];
                                        menuActionType[menuOptionsCount] = groundItemID[index];
                                        menuActionVar1[menuOptionsCount] = selectedItem;
                                        menuOptionsCount += 1;
                                    }
                                    else
                                    {
                                        menuText1[menuOptionsCount] = "Take";
                                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[groundItemID[index]];
                                        menuActionID[menuOptionsCount] = 220;
                                        menuActionX[menuOptionsCount] = groundItemX[index];
                                        menuActionY[menuOptionsCount] = groundItemY[index];
                                        menuActionType[menuOptionsCount] = groundItemID[index];
                                        menuOptionsCount += 1;
                                        menuText1[menuOptionsCount] = "Examine";
                                        menuText2[menuOptionsCount] = "@lre@" + Data.GameData.itemName[groundItemID[index]];
                                        menuActionID[menuOptionsCount] = 3200;
                                        menuActionType[menuOptionsCount] = groundItemID[index];
                                        menuOptionsCount += 1;
                                    }
                            }
                            else
                                if (type == 3)
                                {
                                    string s2 = "";
                                    int l4 = -1;
                                    int id = npcArray[index].npcId;
                                    if (Data.GameData.npcAttackable[id] > 0)
                                    {
                                        int j5 = (Data.GameData.npcAttack[id] + Data.GameData.npcDefense[id] + Data.GameData.npcStrength[id] + Data.GameData.npcHits[id]) / 4;
                                        int k5 = (playerStatBase[0] + playerStatBase[1] + playerStatBase[2] + playerStatBase[3] + 27) / 4;
                                        l4 = k5 - j5;
                                        s2 = "@yel@";
                                        if (l4 < 0)
                                {
                                    s2 = "@or1@";
                                }

                                if (l4 < -3)
                                {
                                    s2 = "@or2@";
                                }

                                if (l4 < -6)
                                {
                                    s2 = "@or3@";
                                }

                                if (l4 < -9)
                                {
                                    s2 = "@red@";
                                }

                                if (l4 > 0)
                                {
                                    s2 = "@gr1@";
                                }

                                if (l4 > 3)
                                {
                                    s2 = "@gr2@";
                                }

                                if (l4 > 6)
                                {
                                    s2 = "@gr3@";
                                }

                                if (l4 > 9)
                                {
                                    s2 = "@gre@";
                                }

                                s2 = " " + s2 + "(level-" + j5 + ")";
                                    }
                                    if (selectedSpell >= 0)
                                    {
                                        if (Data.GameData.spellType[selectedSpell] == 2)
                                        {
                                            menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on";
                                            menuText2[menuOptionsCount] = "@yel@" + Data.GameData.npcName[npcArray[index].npcId];
                                            menuActionID[menuOptionsCount] = 700;
                                            menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                            menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                            menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                            menuActionVar1[menuOptionsCount] = selectedSpell;
                                            menuOptionsCount += 1;
                                        }
                                    }
                                    else
                                        if (selectedItem >= 0)
                                        {
                                            menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                            menuText2[menuOptionsCount] = "@yel@" + Data.GameData.npcName[npcArray[index].npcId];
                                            menuActionID[menuOptionsCount] = 710;
                                            menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                            menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                            menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                            menuActionVar1[menuOptionsCount] = selectedItem;
                                            menuOptionsCount += 1;
                                        }
                                        else
                                        {
                                            if (Data.GameData.npcAttackable[id] > 0)
                                            {
                                                menuText1[menuOptionsCount] = "Attack";
                                                menuText2[menuOptionsCount] = "@yel@" + Data.GameData.npcName[npcArray[index].npcId] + s2;
                                                if (l4 >= 0)
                                    {
                                        menuActionID[menuOptionsCount] = 715;
                                    }
                                    else
                                    {
                                        menuActionID[menuOptionsCount] = 2715;
                                    }

                                    menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                                menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                                menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                                menuOptionsCount += 1;
                                            }
                                            menuText1[menuOptionsCount] = "Talk-to";
                                            menuText2[menuOptionsCount] = "@yel@" + Data.GameData.npcName[npcArray[index].npcId];
                                            menuActionID[menuOptionsCount] = 720;
                                            menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                            menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                            menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                            menuOptionsCount += 1;
                                            if (Data.GameData.npcCommand[id] != "")
                                            {
                                                menuText1[menuOptionsCount] = Data.GameData.npcCommand[id];
                                                menuText2[menuOptionsCount] = "@yel@" + Data.GameData.npcName[npcArray[index].npcId];
                                                menuActionID[menuOptionsCount] = 725;
                                                menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                                menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                                menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                                menuOptionsCount += 1;
                                            }
                                            menuText1[menuOptionsCount] = "Examine";
                                            menuText2[menuOptionsCount] = "@yel@" + Data.GameData.npcName[npcArray[index].npcId];
                                            menuActionID[menuOptionsCount] = 3700;
                                            menuActionType[menuOptionsCount] = npcArray[index].npcId;
                                            menuOptionsCount += 1;
                                        }
                                }
                    }
                    else
                        if (_obj is not null && _obj.index >= 10000)
                        {
                            int j3 = _obj.index - 10000;
                            int i4 = wallObjectID[j3];
                            if (!wallObjectAlreadyInMenu[j3])
                            {
                                if (selectedSpell >= 0)
                                {
                                    if (Data.GameData.spellType[selectedSpell] == 4)
                                    {
                                        menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on";
                                        menuText2[menuOptionsCount] = "@cya@" + Data.GameData.wallObjectName[i4];
                                        menuActionID[menuOptionsCount] = 300;
                                        menuActionX[menuOptionsCount] = wallObjectX[j3];
                                        menuActionY[menuOptionsCount] = wallObjectY[j3];
                                        menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                        menuActionVar1[menuOptionsCount] = selectedSpell;
                                        menuOptionsCount += 1;
                                    }
                                }
                                else
                                    if (selectedItem >= 0)
                                    {
                                        menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                        menuText2[menuOptionsCount] = "@cya@" + Data.GameData.wallObjectName[i4];
                                        menuActionID[menuOptionsCount] = 310;
                                        menuActionX[menuOptionsCount] = wallObjectX[j3];
                                        menuActionY[menuOptionsCount] = wallObjectY[j3];
                                        menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                        menuActionVar1[menuOptionsCount] = selectedItem;
                                        menuOptionsCount += 1;
                                    }
                                    else
                                    {
                                        if (Data.GameData.wallObjectCommand1[i4].ToLower() != "WalkTo")
                                        {
                                            menuText1[menuOptionsCount] = Data.GameData.wallObjectCommand1[i4];
                                            menuText2[menuOptionsCount] = "@cya@" + Data.GameData.wallObjectName[i4];
                                            menuActionID[menuOptionsCount] = 320;
                                            menuActionX[menuOptionsCount] = wallObjectX[j3];
                                            menuActionY[menuOptionsCount] = wallObjectY[j3];
                                            menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                            menuOptionsCount += 1;
                                        }
                                        if (Data.GameData.wallObjectCommand2[i4].ToLower() != "Examine")
                                        {
                                            menuText1[menuOptionsCount] = Data.GameData.wallObjectCommand2[i4];
                                            menuText2[menuOptionsCount] = "@cya@" + Data.GameData.wallObjectName[i4];
                                            menuActionID[menuOptionsCount] = 2300;
                                            menuActionX[menuOptionsCount] = wallObjectX[j3];
                                            menuActionY[menuOptionsCount] = wallObjectY[j3];
                                            menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                            menuOptionsCount += 1;
                                        }
                                        menuText1[menuOptionsCount] = "Examine";
                                        menuText2[menuOptionsCount] = "@cya@" + Data.GameData.wallObjectName[i4];
                                        menuActionID[menuOptionsCount] = 3300;
                                        menuActionType[menuOptionsCount] = i4;
                                        menuOptionsCount += 1;
                                    }
                                wallObjectAlreadyInMenu[j3] = true;
                            }
                        }
                        else
                            if (_obj is not null && _obj.index >= 0)
                            {
                                int k3 = _obj.index;
                                int j4 = objectType[k3];
                                if (!objectAlreadyInMenu[k3])
                                {
                                    if (selectedSpell >= 0)
                                    {
                                        if (Data.GameData.spellType[selectedSpell] == 5)
                                        {
                                            menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on";
                                            menuText2[menuOptionsCount] = "@cya@" + Data.GameData.objectName[j4];
                                            menuActionID[menuOptionsCount] = 400;
                                            menuActionX[menuOptionsCount] = objectX[k3];
                                            menuActionY[menuOptionsCount] = objectY[k3];
                                            menuActionType[menuOptionsCount] = objectRotation[k3];
                                            menuActionVar1[menuOptionsCount] = objectType[k3];
                                            menuActionVar2[menuOptionsCount] = selectedSpell;
                                            menuOptionsCount += 1;
                                        }
                                    }
                                    else
                                        if (selectedItem >= 0)
                                        {
                                            menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                            menuText2[menuOptionsCount] = "@cya@" + Data.GameData.objectName[j4];
                                            menuActionID[menuOptionsCount] = 410;
                                            menuActionX[menuOptionsCount] = objectX[k3];
                                            menuActionY[menuOptionsCount] = objectY[k3];
                                            menuActionType[menuOptionsCount] = objectRotation[k3];
                                            menuActionVar1[menuOptionsCount] = objectType[k3];
                                            menuActionVar2[menuOptionsCount] = selectedItem;
                                            menuOptionsCount += 1;
                                        }
                                        else
                                        {
                                            if (Data.GameData.objectCommand1[j4].ToLower() != "WalkTo")
                                            {
                                                menuText1[menuOptionsCount] = Data.GameData.objectCommand1[j4];
                                                menuText2[menuOptionsCount] = "@cya@" + Data.GameData.objectName[j4];
                                                menuActionID[menuOptionsCount] = 420;
                                                menuActionX[menuOptionsCount] = objectX[k3];
                                                menuActionY[menuOptionsCount] = objectY[k3];
                                                menuActionType[menuOptionsCount] = objectRotation[k3];
                                                menuActionVar1[menuOptionsCount] = objectType[k3];
                                                menuOptionsCount += 1;
                                            }
                                            if (Data.GameData.objectCommand2[j4].ToLower() != "Examine")
                                            {
                                                menuText1[menuOptionsCount] = Data.GameData.objectCommand2[j4];
                                                menuText2[menuOptionsCount] = "@cya@" + Data.GameData.objectName[j4];
                                                menuActionID[menuOptionsCount] = 2400;
                                                menuActionX[menuOptionsCount] = objectX[k3];
                                                menuActionY[menuOptionsCount] = objectY[k3];
                                                menuActionType[menuOptionsCount] = objectRotation[k3];
                                                menuActionVar1[menuOptionsCount] = objectType[k3];
                                                menuOptionsCount += 1;
                                            }
                                            menuText1[menuOptionsCount] = "Examine";
                                            menuText2[menuOptionsCount] = "@cya@" + Data.GameData.objectName[j4];
                                            menuActionID[menuOptionsCount] = 3400;
                                            menuActionType[menuOptionsCount] = j4;
                                            menuOptionsCount += 1;
                                        }
                                    objectAlreadyInMenu[k3] = true;
                                }
                            }
                            else
                            {
                                if (player >= 0)
                        {
                            player = _obj.entityType[player] - 0x30d40;
                        }

                        if (player >= 0)
                        {
                            ground = player;
                        }
                    }
                }
            }

            if (selectedSpell >= 0 && Data.GameData.spellType[selectedSpell] <= 1)
            {
                menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on self";
                menuText2[menuOptionsCount] = "";
                menuActionID[menuOptionsCount] = 1000;
                menuActionType[menuOptionsCount] = selectedSpell;
                menuOptionsCount += 1;
            }
            if (ground != -1)
            {
                if (selectedSpell >= 0)
                {
                    if (Data.GameData.spellType[selectedSpell] == 6)
                    {
                        menuText1[menuOptionsCount] = "Cast " + Data.GameData.spellName[selectedSpell] + " on ground";
                        menuText2[menuOptionsCount] = "";
                        menuActionID[menuOptionsCount] = 900;
                        menuActionX[menuOptionsCount] = engineHandle.selectedX[ground];
                        menuActionY[menuOptionsCount] = engineHandle.selectedY[ground];
                        menuActionType[menuOptionsCount] = selectedSpell;
                        menuOptionsCount += 1;
                        return;
                    }
                }
                else
                    if (selectedItem < 0)
                    {
                        menuText1[menuOptionsCount] = "Walk here";
                        menuText2[menuOptionsCount] = "";
                        menuActionID[menuOptionsCount] = 920;
                        menuActionX[menuOptionsCount] = engineHandle.selectedX[ground];
                        menuActionY[menuOptionsCount] = engineHandle.selectedY[ground];
                        menuOptionsCount += 1;
                    }
            }
        }

        public void DrawShopBox()
        {
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                int l = base.mouseX - 52;
                int i1 = base.mouseY - 44;
                if (l >= 0 && i1 >= 12 && l < 408 && i1 < 246)
                {
                    int j1 = 0;
                    for (int l1 = 0; l1 < 5; l1 += 1)
                    {
                        for (int l2 = 0; l2 < 8; l2 += 1)
                        {
                            int k3 = 7 + l2 * 49;
                            int k4 = 28 + l1 * 34;
                            if (l > k3 && l < k3 + 49 && i1 > k4 && i1 < k4 + 34 && shopItems[j1] != -1)
                            {
                                selectedShopItemIndex = j1;
                                selectedShopItemType = shopItems[j1];
                            }
                            j1 += 1;
                        }

                    }

                    if (selectedShopItemIndex >= 0)
                    {
                        int i3 = shopItems[selectedShopItemIndex];
                        if (i3 != -1)
                        {
                            if (shopItemCount[selectedShopItemIndex] > 0 && l > 298 && i1 >= 204 && l < 408 && i1 <= 215)
                            {
                                base.streamClass.CreatePacket(128);
                                base.streamClass.AddShort(shopItems[selectedShopItemIndex]);
                                base.streamClass.AddInt(shopItemBuyPrice[selectedShopItemIndex]);
                                base.streamClass.FormatPacket();
                            }
                            if (GetInventoryItemTotalCount(i3) > 0 && l > 2 && i1 >= 229 && l < 112 && i1 <= 240)
                            {
                                base.streamClass.CreatePacket(255);
                                base.streamClass.AddShort(shopItems[selectedShopItemIndex]);
                                base.streamClass.AddInt(shopItemSellPrice[selectedShopItemIndex]);
                                base.streamClass.FormatPacket();
                            }
                        }
                    }
                }
                else
                {
                    base.streamClass.CreatePacket(253);
                    base.streamClass.FormatPacket();
                    showShopBox = false;
                    return;
                }
            }
            sbyte _offsetX = 52;
            sbyte _offsetY = 44;
            gameGraphics.DrawBox(_offsetX, _offsetY, 408, 12, 192);
            int k1 = 0x989898;
            gameGraphics.DrawBoxAlpha(_offsetX, _offsetY + 12, 408, 17, k1, 160);
            gameGraphics.DrawBoxAlpha(_offsetX, _offsetY + 29, 8, 170, k1, 160);
            gameGraphics.DrawBoxAlpha(_offsetX + 399, _offsetY + 29, 9, 170, k1, 160);
            gameGraphics.DrawBoxAlpha(_offsetX, _offsetY + 199, 408, 47, k1, 160);
            gameGraphics.DrawString("Buying and selling items", _offsetX + 1, _offsetY + 10, 1, 0xffffff);
            int i2 = 0xffffff;
            if (base.mouseX > _offsetX + 320 && base.mouseY >= _offsetY && base.mouseX < _offsetX + 408 && base.mouseY < _offsetY + 12)
            {
                i2 = 0xff0000;
            }

            gameGraphics.DrawLabel("Close window", _offsetX + 406, _offsetY + 10, 1, i2);
            gameGraphics.DrawString("Shops stock in green", _offsetX + 2, _offsetY + 24, 1, 65280);
            gameGraphics.DrawString("Number you own in blue", _offsetX + 135, _offsetY + 24, 1, 65535);
            gameGraphics.DrawString("Your money: " + GetInventoryItemTotalCount(10) + "gp", _offsetX + 280, _offsetY + 24, 1, 0xffff00);
            int j3 = 0xd0d0d0;
            int j4 = 0;
            for (int boxRow = 0; boxRow < 5; boxRow += 1)
            {
                for (int boxRowColumn = 0; boxRowColumn < 8; boxRowColumn += 1)
                {
                    int i6 = _offsetX + 7 + boxRowColumn * 49;
                    int l6 = _offsetY + 28 + boxRow * 34;
                    if (selectedShopItemIndex == j4)
                    {
                        gameGraphics.DrawBoxAlpha(i6, l6, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        gameGraphics.DrawBoxAlpha(i6, l6, 49, 34, j3, 160);
                    }

                    gameGraphics.DrawBoxEdge(i6, l6, 50, 35, 0);
                    if (shopItems[j4] != -1)
                    {
                        gameGraphics.DrawImage(i6, l6, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[shopItems[j4]], Data.GameData.itemPictureMask[shopItems[j4]], 0, 0, false);
                        gameGraphics.DrawString(shopItemCount[j4].ToString(), i6 + 1, l6 + 10, 1, 65280);
                        gameGraphics.DrawLabel(GetInventoryItemTotalCount(shopItems[j4]).ToString(), i6 + 47, l6 + 10, 1, 65535);
                    }
                    j4 += 1;
                }

            }

            gameGraphics.DrawLineX(_offsetX + 5, _offsetY + 222, 398, 0);
            if (selectedShopItemIndex == -1)
            {
                gameGraphics.DrawText("Select an object to buy or sell", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                return;
            }
            int l5 = shopItems[selectedShopItemIndex];
            if (l5 != -1)
            {
                if (shopItemCount[selectedShopItemIndex] > 0)
                {
                    int j6 = shopItemBuyPriceModifier + shopItemBasePriceModifier[selectedShopItemIndex];
                    if (j6 < 10)
                    {
                        j6 = 10;
                    }

                    int i7 = (j6 * Data.GameData.itemBasePrice[l5]) / 100;
                    gameGraphics.DrawString("Buy a new " + Data.GameData.itemName[l5] + " for " + i7 + "gp", _offsetX + 2, _offsetY + 214, 1, 0xffff00);
                    int j2 = 0xffffff;
                    if (base.mouseX > _offsetX + 298 && base.mouseY >= _offsetY + 204 && base.mouseX < _offsetX + 408 && base.mouseY <= _offsetY + 215)
                    {
                        j2 = 0xff0000;
                    }

                    gameGraphics.DrawLabel("Click here to buy", _offsetX + 405, _offsetY + 214, 3, j2);
                }
                else
                {
                    gameGraphics.DrawText("This item is not currently available to buy", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                }
                if (GetInventoryItemTotalCount(l5) > 0)
                {
                    int k6 = shopItemSellPriceModifier + shopItemBasePriceModifier[selectedShopItemIndex];
                    if (k6 < 10)
                    {
                        k6 = 10;
                    }

                    int j7 = (k6 * Data.GameData.itemBasePrice[l5]) / 100;
                    gameGraphics.DrawLabel("Sell your " + Data.GameData.itemName[l5] + " for " + j7 + "gp", _offsetX + 405, _offsetY + 239, 1, 0xffff00);
                    int k2 = 0xffffff;
                    if (base.mouseX > _offsetX + 2 && base.mouseY >= _offsetY + 229 && base.mouseX < _offsetX + 112 && base.mouseY <= _offsetY + 240)
                    {
                        k2 = 0xff0000;
                    }

                    gameGraphics.DrawString("Click here to sell", _offsetX + 2, _offsetY + 239, 3, k2);
                    return;
                }
                gameGraphics.DrawText("You do not have any of this item to sell", _offsetX + 204, _offsetY + 239, 3, 0xffff00);
            }
        }

        public void LoadTextures()
        {
            sbyte[] abyte0 = UnpackData("textures.jag", "Textures", 50);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            sbyte[] abyte1 = DataOperations.LoadData("index.dat", 0, abyte0);
            gameCamera.CreateTexture(Data.GameData.textureCount, 7, 11);
            for (int l = 0; l < Data.GameData.textureCount; l += 1)
            {
                string s1 = Data.GameData.textureName[l];
                sbyte[] abyte2 = DataOperations.LoadData(s1 + ".dat", 0, abyte0);
                gameGraphics.UnpackImageData(baseTexturePic, abyte2, abyte1, 1);
                gameGraphics.DrawBox(0, 0, 128, 128, 0xff00ff);
                gameGraphics.DrawPicture(0, 0, baseTexturePic);
                int i1 = ((GameImage)(gameGraphics)).pictureAssumedWidth[baseTexturePic];
                string s2 = Data.GameData.textureSubName[l];
                if (s2 is not null && s2.Length > 0)
                {
                    sbyte[] abyte3 = DataOperations.LoadData(s2 + ".dat", 0, abyte0);
                    gameGraphics.UnpackImageData(baseTexturePic, abyte3, abyte1, 1);
                    gameGraphics.DrawPicture(0, 0, baseTexturePic);
                }
                gameGraphics.DrawImage(subTexturePic + l, 0, 0, i1, i1);
                int j1 = i1 * i1;
                for (int k1 = 0; k1 < j1; k1 += 1)
                {
                    if (((GameImage)(gameGraphics)).pictureColors[subTexturePic + l][k1] == 65280)
                    {
                        ((GameImage)(gameGraphics)).pictureColors[subTexturePic + l][k1] = 0xff00ff;
                    }
                }

                gameGraphics.ApplyImage(subTexturePic + l);
                gameCamera.SetTexture(l, ((GameImage)(gameGraphics)).pictureColorIndexes[subTexturePic + l], ((GameImage)(gameGraphics)).pictureColor[subTexturePic + l], i1 / 64 - 1);
            }
        }

        public void DrawAppearanceWindow()
        {
            gameGraphics.interlace = false;
            gameGraphics.ClearScreen();
            appearanceMenu.DrawMenu();
            int l = 140;
            int i1 = 50;
            l += 116;
            i1 -= 25;
            gameGraphics.DrawCharacterLegs(l - 32 - 55, i1, 64, 102, Data.GameData.animationNumber[appearance2Colour], appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, Data.GameData.animationNumber[appearanceBodyGender], appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, Data.GameData.animationNumber[appearanceHeadType], appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawCharacterLegs(l - 32, i1, 64, 102, Data.GameData.animationNumber[appearance2Colour] + 6, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage(l - 32, i1, 64, 102, Data.GameData.animationNumber[appearanceBodyGender] + 6, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage(l - 32, i1, 64, 102, Data.GameData.animationNumber[appearanceHeadType] + 6, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawCharacterLegs((l - 32) + 55, i1, 64, 102, Data.GameData.animationNumber[appearance2Colour] + 12, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage((l - 32) + 55, i1, 64, 102, Data.GameData.animationNumber[appearanceBodyGender] + 12, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage((l - 32) + 55, i1, 64, 102, Data.GameData.animationNumber[appearanceHeadType] + 12, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawPicture(0, windowHeight, baseInventoryPic + 22);
            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.DrawImage(spriteBatch, 0, 0);
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

                    menuX = base.mouseX - menuWidth / 2;
                    menuY = base.mouseY - 7;
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

        public void DrawGame()
        {
            if (playerAliveTimeout != 0)
            {
                gameGraphics.ScreenFadeToBlack();
                gameGraphics.DrawText("Oh dear! You are dead...", windowWidth / 2, windowHeight / 2, 7, 0xff0000);
                DrawChatMessageTabs();
                //gameGraphics.UpdateGameImage();
                OnDrawDone();//gameGraphics.DrawImage(spriteBatch, 0, 0);
                return;
            }
            if (showAppearanceWindow)
            {
                DrawAppearanceWindow();
                return;
            }
            if (isSleeping)
            {
                gameGraphics.ScreenFadeToBlack();
                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    gameGraphics.DrawText("ZZZ", (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    gameGraphics.DrawText("ZZZ", 512 - (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                gameGraphics.DrawBox(windowWidth / 2 - 100, 160, 200, 40, 0);
                gameGraphics.DrawText("You are sleeping", windowWidth / 2, 50, 7, 0xffff00);
                gameGraphics.DrawText("Fatigue: " + (fatigue * 100) / 750 + "%", windowWidth / 2, 90, 7, 0xffff00);
                gameGraphics.DrawText("When you want to wake up just use your", windowWidth / 2, 140, 5, 0xffffff);
                gameGraphics.DrawText("keyboard to type the word in the box below", windowWidth / 2, 160, 5, 0xffffff);
                gameGraphics.DrawText(base.inputText + "*", windowWidth / 2, 180, 5, 65535);
                if (sleepingStatusText is null)
                {
                    gameGraphics.DrawPixels(captchaPixels, windowWidth / 2 - 127, 230, captchaWidth, captchaHeight);
                }
                else
                {
                    gameGraphics.DrawText(sleepingStatusText, windowWidth / 2, 260, 5, 0xff0000);
                }

                gameGraphics.DrawBoxEdge(windowWidth / 2 - 128, 229, 257, 42, 0xffffff);
                DrawChatMessageTabs();
                gameGraphics.DrawText("If you can't read the word", windowWidth / 2, 290, 1, 0xffffff);
                gameGraphics.DrawText("@yel@click here@whi@ to get a different one", windowWidth / 2, 305, 1, 0xffffff);

                //gameGraphics.UpdateGameImage();
                OnDrawDone();//gameGraphics.DrawImage(spriteBatch, 0, 0);
                return;
            }
            if (!engineHandle.playerIsAlive)
            {
                return;
            }

            for (int l = 0; l < 64; l += 1)
            {
                gameCamera.RemoveModel(engineHandle.roofObject[lastLayerIndex][l]);
                if (lastLayerIndex == 0)
                {
                    gameCamera.RemoveModel(engineHandle.wallObject[1][l]);
                    gameCamera.RemoveModel(engineHandle.roofObject[1][l]);
                    gameCamera.RemoveModel(engineHandle.wallObject[2][l]);
                    gameCamera.RemoveModel(engineHandle.roofObject[2][l]);
                }
                cameraZoom = true;
                if (lastLayerIndex == 0 && (engineHandle.tiles[ourPlayer.currentX / 128][ourPlayer.currentY / 128] & 0x80) == 0)
                {
                    if (showRoofs)
                    {
                        gameCamera.AddModel(engineHandle.roofObject[lastLayerIndex][l]);
                        if (lastLayerIndex == 0)
                        {
                            // draw wall object at lv 1 / second layer
                            gameCamera.AddModel(engineHandle.wallObject[1][l]);
                            // draw roof object at lv 1 / second layer
                            GameObject roof1 = engineHandle.roofObject[1][l];
                            gameCamera.AddModel(roof1);


                            // draw wall object at lv 2 / third layer
                            gameCamera.AddModel(engineHandle.wallObject[2][l]);

                            // draw roof object at lv 2 / third layer
                            GameObject roof2 = engineHandle.roofObject[2][l];
                            gameCamera.AddModel(engineHandle.roofObject[2][l]);
                        }
                    }
                    cameraZoom = false;
                }
            }

            if (modelFireLightningSpellNumber != lastModelFireLightningSpellNumber)
            {
                lastModelFireLightningSpellNumber = modelFireLightningSpellNumber;
                for (int i1 = 0; i1 < objectCount; i1 += 1)
                {
                    if (objectType[i1] == 97)
                    {
                        DrawModel(i1, "firea" + (modelFireLightningSpellNumber + 1));
                    }

                    if (objectType[i1] == 274)
                    {
                        DrawModel(i1, "fireplacea" + (modelFireLightningSpellNumber + 1));
                    }

                    if (objectType[i1] == 1031)
                    {
                        DrawModel(i1, "lightning" + (modelFireLightningSpellNumber + 1));
                    }

                    if (objectType[i1] == 1036)
                    {
                        DrawModel(i1, "firespell" + (modelFireLightningSpellNumber + 1));
                    }

                    if (objectType[i1] == 1147)
                    {
                        DrawModel(i1, "spellcharge" + (modelFireLightningSpellNumber + 1));
                    }
                }

            }
            if (modelTorchNumber != lastModelTorchNumber)
            {
                lastModelTorchNumber = modelTorchNumber;
                for (int j1 = 0; j1 < objectCount; j1 += 1)
                {
                    if (objectType[j1] == 51)
                    {
                        DrawModel(j1, "torcha" + (modelTorchNumber + 1));
                    }

                    if (objectType[j1] == 143)
                    {
                        DrawModel(j1, "skulltorcha" + (modelTorchNumber + 1));
                    }
                }

            }
            if (modelClawSpellNumber != lastModelClawSpellNumber)
            {
                lastModelClawSpellNumber = modelClawSpellNumber;
                for (int k1 = 0; k1 < objectCount; k1 += 1)
                {
                    if (objectType[k1] == 1142)
                    {
                        DrawModel(k1, "clawspell" + (modelClawSpellNumber + 1));
                    }
                }
            }
            gameCamera.RemoveLastUpdates(drawUpdatesPerformed);
            drawUpdatesPerformed = 0;
            for (int l1 = 0; l1 < playerCount; l1 += 1)
            {
                ClientMob player = playerArray[l1];
                if (player.bottomColour != 255)
                {
                    int j2 = player.currentX;
                    int l2 = player.currentY;
                    int j3 = -engineHandle.GetAveragedElevation(j2, l2);
                    int k4 = gameCamera.AddSpriteToScene(5000 + l1, j2, j3, l2, 145, 220, l1 + 10000);
                    drawUpdatesPerformed += 1;
                    if (player == ourPlayer)
                    {
                        gameCamera.RemoveSprite(k4);
                    }

                    if (player.currentSprite == 8)
                    {
                        gameCamera.UpdateSpritePosition(k4, -30);
                    }

                    if (player.currentSprite == 9)
                    {
                        gameCamera.UpdateSpritePosition(k4, 30);
                    }
                }
            }

            for (int i2 = 0; i2 < playerCount; i2 += 1)
            {
                ClientMob player = playerArray[i2];
                if (player.projectileDistance > 0)
                {
                    ClientMob targetMob = null;
                    if (player.attackingNpcIndex != -1)
                    {
                        targetMob = npcAttackingArray[player.attackingNpcIndex];
                    }
                    else if (player.attackingPlayerIndex != -1)
                    {
                        targetMob = playerBufferArray[player.attackingPlayerIndex];
                    }

                    if (targetMob is not null)
                    {
                        int k3 = player.currentX;
                        int l4 = player.currentY;
                        int k7 = -engineHandle.GetAveragedElevation(k3, l4) - 110;
                        int k9 = targetMob.currentX;
                        int j10 = targetMob.currentY;
                        int k10 = -engineHandle.GetAveragedElevation(k9, j10) - Data.GameData.npcCameraArray2[targetMob.npcId] / 2;
                        int l10 = (k3 * player.projectileDistance + k9 * (projectileRange - player.projectileDistance)) / projectileRange;
                        int i11 = (k7 * player.projectileDistance + k10 * (projectileRange - player.projectileDistance)) / projectileRange;
                        int j11 = (l4 * player.projectileDistance + j10 * (projectileRange - player.projectileDistance)) / projectileRange;
                        gameCamera.AddSpriteToScene(baseProjectilePic + player.projectileType, l10, i11, j11, 32, 32, 0);
                        drawUpdatesPerformed += 1;
                    }
                }
            }

            for (int k2 = 0; k2 < npcCount; k2 += 1)
            {
                ClientMob npc = npcArray[k2];
                int x1 = npc.currentX;
                int z1 = npc.currentY;
                int y1 = -engineHandle.GetAveragedElevation(x1, z1);
                int l9 = gameCamera.AddSpriteToScene(20000 + k2, x1, y1, z1, Data.GameData.npcCameraArray1[npc.npcId], Data.GameData.npcCameraArray2[npc.npcId], k2 + 30000);
                drawUpdatesPerformed += 1;
                if (npc.currentSprite == 8)
                {
                    gameCamera.UpdateSpritePosition(l9, -30);
                }

                if (npc.currentSprite == 9)
                {
                    gameCamera.UpdateSpritePosition(l9, 30);
                }
            }

            for (int i3 = 0; i3 < groundItemCount; i3 += 1)
            {
                int x = groundItemX[i3] * gridSize + 64;
                int y = groundItemY[i3] * gridSize + 64;
                gameCamera.AddSpriteToScene(40000 + groundItemID[i3], x, -engineHandle.GetAveragedElevation(x, y) - groundItemObjectVar[i3], y, 96, 64, i3 + 20000);
                drawUpdatesPerformed += 1;
            }

            for (int j4 = 0; j4 < teleBubbleCount; j4 += 1)
            {
                int k5 = teleBubbleX[j4] * gridSize + 64;
                int i8 = teleBubbleY[j4] * gridSize + 64;
                int i10 = teleBubbleType[j4];
                if (i10 == 0)
                {
                    gameCamera.AddSpriteToScene(50000 + j4, k5, -engineHandle.GetAveragedElevation(k5, i8), i8, 128, 256, j4 + 50000);
                    drawUpdatesPerformed += 1;
                }
                if (i10 == 1)
                {
                    gameCamera.AddSpriteToScene(50000 + j4, k5, -engineHandle.GetAveragedElevation(k5, i8), i8, 128, 64, j4 + 50000);
                    drawUpdatesPerformed += 1;
                }
            }

            gameGraphics.interlace = false;
            gameGraphics.ClearScreen();
            gameGraphics.interlace = base.keyF1Toggle;
            if (lastLayerIndex == 3)
            {
                int l5 = 40 + (int)(Helper.Random.NextDouble() * 3D);
                int j8 = 40 + (int)(Helper.Random.NextDouble() * 7D);
                gameCamera.SetAllModelColours(l5, j8, -50, -10, -50);
            }
            itemsAboveHeadCount = 0;
            receivedMessagesCount = 0;
            healthBarVisibleCount = 0;
            if (cameraAutoAngleDebug)
            {
                if (configCameraAutoAngle && !cameraZoom)
                {
                    int i6 = cameraAutoAngle;
                    AutoRotateCamera();
                    if (cameraAutoAngle != i6)
                    {
                        cameraAutoRotatePlayerX = ourPlayer.currentX;
                        cameraAutoRotatePlayerY = ourPlayer.currentY;
                    }
                }
                if (fogOfWar)
                {
                    gameCamera.zoom1 = 3000;
                    gameCamera.zoom2 = 3000;
                    gameCamera.zoom3 = 1;
                    gameCamera.zoom4 = 2800;
                }
                else
                {
                    gameCamera.zoom1 = 40000;
                    gameCamera.zoom2 = 40000;
                    gameCamera.zoom3 = 40000;
                    gameCamera.zoom4 = 40000;
                }
                cameraRotation = cameraAutoAngle * 32;
                int newCameraPosX = cameraAutoRotatePlayerX + cameraRotationXAmount;
                int newCameraPosY = cameraAutoRotatePlayerY + cameraRotationYAmount;
                gameCamera.SetCameraTransform(newCameraPosX, -engineHandle.GetAveragedElevation(newCameraPosX, newCameraPosY), newCameraPosY, 912, cameraRotation * 4, 0, 2000);
            }
            else
            {
                if (configCameraAutoAngle && !cameraZoom)
                {
                    AutoRotateCamera();
                }

                if (fogOfWar)
                {
                    if (!base.keyF1Toggle)
                    {
                        gameCamera.zoom1 = 2400;
                        gameCamera.zoom2 = 2400;
                        gameCamera.zoom3 = 1;
                        gameCamera.zoom4 = 2300;
                    }
                    else
                    {
                        gameCamera.zoom1 = 2200;
                        gameCamera.zoom2 = 2200;
                        gameCamera.zoom3 = 1;
                        gameCamera.zoom4 = 2100;
                    }
                }
                else
                {
                    gameCamera.zoom1 = 40000;
                    gameCamera.zoom2 = 40000;
                    gameCamera.zoom3 = 40000;
                    gameCamera.zoom4 = 40000;
                }
                int k6 = cameraAutoRotatePlayerX + cameraRotationXAmount;
                int l8 = cameraAutoRotatePlayerY + cameraRotationYAmount;
                gameCamera.SetCameraTransform(k6, -engineHandle.GetAveragedElevation(k6, l8), l8, 912, cameraRotation * 4, 0, cameraDistance * 2);
            }
            gameCamera.FinishCamera();
            DrawAboveHeadThings();
            if (actionPictureType > 0)
            {
                gameGraphics.DrawPicture(walkMouseX - 8, walkMouseY - 8, baseInventoryPic + 14 + (24 - actionPictureType) / 6);
            }

            if (actionPictureType < 0)
            {
                gameGraphics.DrawPicture(walkMouseX - 8, walkMouseY - 8, baseInventoryPic + 18 + (24 + actionPictureType) / 6);
            }

            if (systemUpdate != 0)
            {
                int seconds = systemUpdate / 50;
                int minutes = seconds / 60;
                seconds %= 60;
                if (seconds < 10)
                {
                    gameGraphics.DrawText("System update in: " + minutes + ":0" + seconds, 256, windowHeight - 7, 1, 0xffff00);
                }
                else
                {
                    gameGraphics.DrawText("System update in: " + minutes + ":" + seconds, 256, windowHeight - 7, 1, 0xffff00);
                }
            }
            if (!loadArea)
            {
                int i7 = 2203 - (sectionY + wildY + areaY);
                if (sectionX + wildX + areaX >= 2640)
                {
                    i7 = -50;
                }

                if (i7 > 0)
                {
                    int j9 = 1 + i7 / 6;
                    gameGraphics.DrawPicture(453, windowHeight - 56, baseInventoryPic + 13);
                    gameGraphics.DrawText("Wilderness", 465, windowHeight - 20, 1, 0xffff00);
                    gameGraphics.DrawText("Level: " + j9, 465, windowHeight - 7, 1, 0xffff00);
                    if (wildType == 0)
                    {
                        wildType = 2;
                    }
                }
                if (wildType == 0 && i7 > -10 && i7 <= 0)
                {
                    wildType = 1;
                }
            }
            if (messagesTab == 0)
            {
                for (int j7 = 0; j7 < 5; j7 += 1)
                {
                    if (messagesTimeout[j7] > 0)
                    {
                        string s1 = messagesArray[j7];
                        gameGraphics.DrawString(s1, 7, windowHeight - 18 - j7 * 12, 1, 0xffff00);
                    }
                }
            }
            chatInputMenu.DisableInput(messagesHandleType2);
            chatInputMenu.DisableInput(messagesHandleType5);
            chatInputMenu.DisableInput(messagesHandleType6);
            if (messagesTab == 1)
            {
                chatInputMenu.EnableInput(messagesHandleType2);
            }
            else if (messagesTab == 2)
            {
                chatInputMenu.EnableInput(messagesHandleType5);
            }
            else if (messagesTab == 3)
            {
                chatInputMenu.EnableInput(messagesHandleType6);
            }

            Menu.chatMenuTextHeightMod = 2;
            chatInputMenu.DrawMenu();
            Menu.chatMenuTextHeightMod = 0;
            gameGraphics.DrawPicture(((GameImage)(gameGraphics)).gameWidth - 3 - 197, 3, baseInventoryPic, 128);

#warning play with this! Create a new menu of choice :)


            DrawMenus();

            gameGraphics.loggedIn = false;
            DrawChatMessageTabs();


            string text = "Coordinates: ( " + (sectionX + areaX) + "," + (sectionY + areaY) + " ) Section: (" + sectionX + "," + sectionY + ") Area: (" + areaX + "," + areaY + ")";
            // Text shadow
            gameGraphics.DrawString(text, 10 + 11, 10 + 11, 1, 0x000000);
            gameGraphics.DrawString(text, 10 + 10, 10 + 10, 1, 0xffffff);

            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.DrawImage(spriteBatch, 0, 0);
        }

        //	public bool DrawCustomMenus { get; set; }
        //    public event EventHandler OnDrawMenus;

        public void DrawReportAbuseBox2()
        {
            if (base.enteredInputText.Length > 0)
            {
                string s1 = base.enteredInputText.Trim();
                base.inputText = "";
                base.enteredInputText = "";
                if (s1.Length > 0)
                {
                    long l1 = DataOperations.NameToHash(s1);
                    base.streamClass.CreatePacket(7);
                    base.streamClass.AddLong(l1);
                    base.streamClass.AddByte(reportAbuseOptionSelected);
                    //base.streamClass.AddByte(dia ? 1 : 0);
                    base.streamClass.FormatPacket();
                }
                showAbuseBox = 0;
                return;
            }
            gameGraphics.DrawBox(56, 130, 400, 100, 0);
            gameGraphics.DrawBoxEdge(56, 130, 400, 100, 0xffffff);
            int l = 160;
            gameGraphics.DrawText("Now type the name of the offending player, and press enter", 256, l, 1, 0xffff00);
            l += 18;
            gameGraphics.DrawText("Name: " + base.inputText + "*", 256, l, 4, 0xffffff);
            l = 222;
            int i1 = 0xffffff;
            if (base.mouseX > 196 && base.mouseX < 316 && base.mouseY > l - 13 && base.mouseY < l + 2)
            {
                i1 = 0xffff00;
                if (mouseButtonClick == 1)
                {
                    mouseButtonClick = 0;
                    showAbuseBox = 0;
                }
            }
            gameGraphics.DrawText("Click here to cancel", 256, l, 1, i1);
            if (mouseButtonClick == 1 && (base.mouseX < 56 || base.mouseX > 456 || base.mouseY < 130 || base.mouseY > 230))
            {
                mouseButtonClick = 0;
                showAbuseBox = 0;
            }
        }

        public void DrawMenus()
        {
            if (logoutTimer != 0)
            {
                DrawLogoutBox();
            }
            else if (showWelcomeBox)
            {
                DrawWelcomeBox();
            }
            else if (showServerMessageBox)
            {
                DrawServerMessageBox();
            }
            else if (wildType == 1)
            {
                DrawWildernessAlertBox();
            }
            else if (showBankBox && combatTimeout == 0)
            {
                DrawBankBox();
            }
            else if (showShopBox && combatTimeout == 0)
            {
                DrawShopBox();
            }
            else if (showTradeConfirmBox)
            {
                DrawTradeConfirmBox();
            }
            else if (showTradeBox)
            {
                DrawTradeBox();
            }
            else if (showDuelConfirmBox)
            {
                DrawDuelConfirmBox();
            }
            else if (showDuelBox)
            {
                DrawDuelBox();
            }
            else if (showAbuseBox == 1)
            {
                DrawReportAbuseBox1();
            }
            else if (showAbuseBox == 2)
            {
                DrawReportAbuseBox2();
            }
            else if (showFriendsBox != 0)
            {
                DrawFriendsBox();
            }
            else
            {
                if (showQuestionMenu)
                {
                    DrawQuestionMenu();
                }

                if (showCombatWindow || ourPlayer.currentSprite == 8 || ourPlayer.currentSprite == 9)
                {
                    DrawCombatStyleBox();
                }

                GetMenuHighlighted();
                bool flag = !showQuestionMenu && !menuShow;
                if (flag)
                {
                    menuOptionsCount = 0;
                }

                if (drawMenuTab == 0 && flag)
                {
                    GenerateWorldRightClickMenu();
                }

                if (drawMenuTab == 1)
                {
                    DrawInventoryMenu(flag);
                }

                if (drawMenuTab == 2)
                {
                    DrawMinimapMenu(flag);
                }

                if (drawMenuTab == 3)
                {
                    DrawStatsQuestsMenu(flag);
                }

                if (drawMenuTab == 4)
                {
                    DrawPrayerMagicMenu(flag);
                }

                if (drawMenuTab == 5)
                {
                    DrawFriendsMenu(flag);
                }

                if (drawMenuTab == 6)
                {
                    DrawOptionsMenu(flag);
                }

                if (!menuShow && !showQuestionMenu)
                {
                    CheckMouseStatus();
                }

                if (menuShow && !showQuestionMenu)
                {
                    DrawRightClickMenu();
                }
            }
            mouseButtonClick = 0;
        }

        public void LoadModels()
        {
            Data.GameData.GetModelNameIndex("torcha2");
            Data.GameData.GetModelNameIndex("torcha3");
            Data.GameData.GetModelNameIndex("torcha4");
            Data.GameData.GetModelNameIndex("skulltorcha2");
            Data.GameData.GetModelNameIndex("skulltorcha3");
            Data.GameData.GetModelNameIndex("skulltorcha4");
            Data.GameData.GetModelNameIndex("firea2");
            Data.GameData.GetModelNameIndex("firea3");
            Data.GameData.GetModelNameIndex("fireplacea2");
            Data.GameData.GetModelNameIndex("fireplacea3");
            Data.GameData.GetModelNameIndex("firespell2");
            Data.GameData.GetModelNameIndex("firespell3");
            Data.GameData.GetModelNameIndex("lightning2");
            Data.GameData.GetModelNameIndex("lightning3");
            Data.GameData.GetModelNameIndex("clawspell2");
            Data.GameData.GetModelNameIndex("clawspell3");
            Data.GameData.GetModelNameIndex("clawspell4");
            Data.GameData.GetModelNameIndex("clawspell5");
            Data.GameData.GetModelNameIndex("spellcharge2");
            Data.GameData.GetModelNameIndex("spellcharge3");
            sbyte[] abyte0 = UnpackData("models.jag", "3d models", 60);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            for (int i1 = 0; i1 < Data.GameData.modelCount; i1 += 1)
            {
                try
                {
                    long j1 = DataOperations.GetObjectOffset(Data.GameData.modelName[i1] + ".ob3", abyte0);
                    if (j1 != 0)
                    {
                        gameDataObjects[i1] = new GameObject(abyte0, (int)j1, true);
                    }
                    else
                    {
                        gameDataObjects[i1] = new GameObject(1, 1);
                    }

                    if (Data.GameData.modelName[i1] == "giantcrystal")
                    {
                        gameDataObjects[i1].isGiantCrystal = true;
                    }
                }
                catch { }
            }
        }

        public void DrawDuelBox()
        {
            if (mouseButtonClick != 0 && mouseClickedHeldInTradeDuelBox == 0)
            {
                mouseClickedHeldInTradeDuelBox = 1;
            }

            if (mouseClickedHeldInTradeDuelBox > 0)
            {
                int l = base.mouseX - 22;
                int i1 = base.mouseY - 36;
                if (l >= 0 && i1 >= 0 && l < 468 && i1 < 262)
                {
                    if (l > 216 && i1 > 30 && l < 462 && i1 < 235)
                    {
                        int j1 = (l - 217) / 49 + ((i1 - 31) / 34) * 5;
                        if (j1 >= 0 && j1 < inventoryItemsCount)
                        {
                            bool flag1 = false;
                            int k2 = 0;
                            int j3 = inventoryItems[j1];
                            for (int j4 = 0; j4 < duelMyItemCount; j4 += 1)
                            {
                                if (duelMyItems[j4] == j3)
                                {
                                    if (Data.GameData.itemStackable[j3] == 0)
                                    {
                                        for (int l4 = 0; l4 < mouseClickedHeldInTradeDuelBox; l4 += 1)
                                        {
                                            if (duelMyItemsCount[j4] < inventoryItemCount[j1])
                                            {
                                                duelMyItemsCount[j4] += 1;
                                            }

                                            flag1 = true;
                                        }

                                    }
                                    else
                                    {
                                        k2 += 1;
                                    }
                                }
                            }

                            if (GetInventoryItemTotalCount(j3) <= k2)
                            {
                                flag1 = true;
                            }

                            if (Data.GameData.itemSpecial[j3] == 1)
                            {
                                DisplayMessage("This object cannot be added to a duel offer", 3);
                                flag1 = true;
                            }
                            if (!flag1 && duelMyItemCount < 8)
                            {
                                duelMyItems[duelMyItemCount] = j3;
                                duelMyItemsCount[duelMyItemCount] = 1;
                                duelMyItemCount += 1;
                                flag1 = true;
                            }
                            if (flag1)
                            {
                                base.streamClass.CreatePacket(123);
                                base.streamClass.AddByte(duelMyItemCount);
                                for (int i5 = 0; i5 < duelMyItemCount; i5 += 1)
                                {
                                    base.streamClass.AddShort(duelMyItems[i5]);
                                    base.streamClass.AddInt(duelMyItemsCount[i5]);
                                }

                                base.streamClass.FormatPacket();
                                duelOpponentAccepted = false;
                                duelMyAccepted = false;
                            }
                        }
                    }
                    if (l > 8 && i1 > 30 && l < 205 && i1 < 129)
                    {
                        int k1 = (l - 9) / 49 + ((i1 - 31) / 34) * 4;
                        if (k1 >= 0 && k1 < duelMyItemCount)
                        {
                            int i2 = duelMyItems[k1];
                            for (int l2 = 0; l2 < mouseClickedHeldInTradeDuelBox; l2 += 1)
                            {
                                if (Data.GameData.itemStackable[i2] == 0 && duelMyItemsCount[k1] > 1)
                                {
                                    duelMyItemsCount[k1] -= 1;
                                    continue;
                                }
                                duelMyItemCount -= 1;
                                mouseButtonHeldTime = 0;
                                for (int k3 = k1; k3 < duelMyItemCount; k3 += 1)
                                {
                                    duelMyItems[k3] = duelMyItems[k3 + 1];
                                    duelMyItemsCount[k3] = duelMyItemsCount[k3 + 1];
                                }

                                break;
                            }

                            base.streamClass.CreatePacket(123);
                            base.streamClass.AddByte(duelMyItemCount);
                            for (int l3 = 0; l3 < duelMyItemCount; l3 += 1)
                            {
                                base.streamClass.AddShort(duelMyItems[l3]);
                                base.streamClass.AddInt(duelMyItemsCount[l3]);
                            }

                            base.streamClass.FormatPacket();
                            duelOpponentAccepted = false;
                            duelMyAccepted = false;
                        }
                    }
                    bool flag = false;
                    if (l >= 93 && i1 >= 221 && l <= 104 && i1 <= 232)
                    {
                        duelNoRetreating = !duelNoRetreating;
                        flag = true;
                    }
                    if (l >= 93 && i1 >= 240 && l <= 104 && i1 <= 251)
                    {
                        duelNoMagic = !duelNoMagic;
                        flag = true;
                    }
                    if (l >= 191 && i1 >= 221 && l <= 202 && i1 <= 232)
                    {
                        duelNoPrayer = !duelNoPrayer;
                        flag = true;
                    }
                    if (l >= 191 && i1 >= 240 && l <= 202 && i1 <= 251)
                    {
                        duelNoWeapons = !duelNoWeapons;
                        flag = true;
                    }
                    if (flag)
                    {
                        base.streamClass.CreatePacket(225);
                        int duelNoRetreatingByte = 0;
                        if (duelNoRetreating)
                        {
                            duelNoRetreatingByte = 1;
                        }
                        base.streamClass.AddByte(duelNoRetreatingByte);
                        int duelNoMagicByte = 0;
                        if (duelNoMagic)
                        {
                            duelNoMagicByte = 1;
                        }
                        base.streamClass.AddByte(duelNoMagicByte);
                        int duelNoPrayerByte = 0;
                        if (duelNoPrayer)
                        {
                            duelNoPrayerByte = 1;
                        }
                        base.streamClass.AddByte(duelNoPrayerByte);
                        int duelNoWeaponsByte = 0;
                        if (duelNoWeapons)
                        {
                            duelNoWeaponsByte = 1;
                        }
                        base.streamClass.AddByte(duelNoWeaponsByte);
                        base.streamClass.FormatPacket();
                        duelOpponentAccepted = false;
                        duelMyAccepted = false;
                    }
                    if (l >= 217 && i1 >= 238 && l <= 286 && i1 <= 259)
                    {
                        duelMyAccepted = true;
                        base.streamClass.CreatePacket(252);
                        base.streamClass.FormatPacket();
                    }
                    if (l >= 394 && i1 >= 238 && l < 463 && i1 < 259)
                    {
                        showDuelBox = false;
                        base.streamClass.CreatePacket(35);
                        base.streamClass.FormatPacket();
                    }
                }
                else
                    if (mouseButtonClick != 0)
                    {
                        showDuelBox = false;
                        base.streamClass.CreatePacket(35);
                        base.streamClass.FormatPacket();
                    }
                mouseButtonClick = 0;
                mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!showDuelBox)
            {
                return;
            }

            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.DrawBox(byte0, byte1, 468, 12, 0xc90b1d);
            int l1 = 0x989898;
            gameGraphics.DrawBoxAlpha(byte0, byte1 + 12, 468, 18, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0, byte1 + 30, 8, 248, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 205, byte1 + 30, 11, 248, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 462, byte1 + 30, 6, 248, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 99, 197, 24, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 192, 197, 23, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 258, 197, 20, l1, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 235, 246, 43, l1, 160);
            int j2 = 0xd0d0d0;
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 30, 197, 69, j2, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 123, 197, 69, j2, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 215, 197, 43, j2, 160);
            gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 30, 246, 205, j2, 160);
            for (int i3 = 0; i3 < 3; i3 += 1)
            {
                gameGraphics.DrawLineX(byte0 + 8, byte1 + 30 + i3 * 34, 197, 0);
            }

            for (int i4 = 0; i4 < 3; i4 += 1)
            {
                gameGraphics.DrawLineX(byte0 + 8, byte1 + 123 + i4 * 34, 197, 0);
            }

            for (int k4 = 0; k4 < 7; k4 += 1)
            {
                gameGraphics.DrawLineX(byte0 + 216, byte1 + 30 + k4 * 34, 246, 0);
            }

            for (int j5 = 0; j5 < 6; j5 += 1)
            {
                if (j5 < 5)
                {
                    gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 30, 69, 0);
                }

                if (j5 < 5)
                {
                    gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 123, 69, 0);
                }

                gameGraphics.DrawLineY(byte0 + 216 + j5 * 49, byte1 + 30, 205, 0);
            }

            gameGraphics.DrawLineX(byte0 + 8, byte1 + 215, 197, 0);
            gameGraphics.DrawLineX(byte0 + 8, byte1 + 257, 197, 0);
            gameGraphics.DrawLineY(byte0 + 8, byte1 + 215, 43, 0);
            gameGraphics.DrawLineY(byte0 + 204, byte1 + 215, 43, 0);
            gameGraphics.DrawString("Preparing to duel with: " + duelOpponent, byte0 + 1, byte1 + 10, 1, 0xffffff);
            gameGraphics.DrawString("Your Stake", byte0 + 9, byte1 + 27, 4, 0xffffff);
            gameGraphics.DrawString("Opponent's Stake", byte0 + 9, byte1 + 120, 4, 0xffffff);
            gameGraphics.DrawString("Duel Options", byte0 + 9, byte1 + 212, 4, 0xffffff);
            gameGraphics.DrawString("Your Inventory", byte0 + 216, byte1 + 27, 4, 0xffffff);
            gameGraphics.DrawString("No retreating", byte0 + 8 + 1, byte1 + 215 + 16, 3, 0xffff00);
            gameGraphics.DrawString("No magic", byte0 + 8 + 1, byte1 + 215 + 35, 3, 0xffff00);
            gameGraphics.DrawString("No prayer", byte0 + 8 + 102, byte1 + 215 + 16, 3, 0xffff00);
            gameGraphics.DrawString("No weapons", byte0 + 8 + 102, byte1 + 215 + 35, 3, 0xffff00);
            gameGraphics.DrawBoxEdge(byte0 + 93, byte1 + 215 + 6, 11, 11, 0xffff00);
            if (duelNoRetreating)
            {
                gameGraphics.DrawBox(byte0 + 95, byte1 + 215 + 8, 7, 7, 0xffff00);
            }

            gameGraphics.DrawBoxEdge(byte0 + 93, byte1 + 215 + 25, 11, 11, 0xffff00);
            if (duelNoMagic)
            {
                gameGraphics.DrawBox(byte0 + 95, byte1 + 215 + 27, 7, 7, 0xffff00);
            }

            gameGraphics.DrawBoxEdge(byte0 + 191, byte1 + 215 + 6, 11, 11, 0xffff00);
            if (duelNoPrayer)
            {
                gameGraphics.DrawBox(byte0 + 193, byte1 + 215 + 8, 7, 7, 0xffff00);
            }

            gameGraphics.DrawBoxEdge(byte0 + 191, byte1 + 215 + 25, 11, 11, 0xffff00);
            if (duelNoWeapons)
            {
                gameGraphics.DrawBox(byte0 + 193, byte1 + 215 + 27, 7, 7, 0xffff00);
            }

            if (!duelMyAccepted)
            {
                gameGraphics.DrawPicture(byte0 + 217, byte1 + 238, baseInventoryPic + 25);
            }

            gameGraphics.DrawPicture(byte0 + 394, byte1 + 238, baseInventoryPic + 26);
            if (duelOpponentAccepted)
            {
                gameGraphics.DrawText("Other player", byte0 + 341, byte1 + 246, 1, 0xffffff);
                gameGraphics.DrawText("has accepted", byte0 + 341, byte1 + 256, 1, 0xffffff);
            }
            if (duelMyAccepted)
            {
                gameGraphics.DrawText("Waiting for", byte0 + 217 + 35, byte1 + 246, 1, 0xffffff);
                gameGraphics.DrawText("other player", byte0 + 217 + 35, byte1 + 256, 1, 0xffffff);
            }
            for (int k5 = 0; k5 < inventoryItemsCount; k5 += 1)
            {
                int l5 = 217 + byte0 + (k5 % 5) * 49;
                int j6 = 31 + byte1 + (k5 / 5) * 34;
                gameGraphics.DrawImage(l5, j6, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[inventoryItems[k5]], Data.GameData.itemPictureMask[inventoryItems[k5]], 0, 0, false);
                if (Data.GameData.itemStackable[inventoryItems[k5]] == 0)
                {
                    gameGraphics.DrawString(inventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < duelMyItemCount; i6 += 1)
            {
                int k6 = 9 + byte0 + (i6 % 4) * 49;
                int i7 = 31 + byte1 + (i6 / 4) * 34;
                gameGraphics.DrawImage(k6, i7, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[duelMyItems[i6]], Data.GameData.itemPictureMask[duelMyItems[i6]], 0, 0, false);
                if (Data.GameData.itemStackable[duelMyItems[i6]] == 0)
                {
                    gameGraphics.DrawString(duelMyItemsCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (base.mouseX > k6 && base.mouseX < k6 + 48 && base.mouseY > i7 && base.mouseY < i7 + 32)
                {
                    gameGraphics.DrawString(Data.GameData.itemName[duelMyItems[i6]] + ": @whi@" + Data.GameData.itemDescription[duelMyItems[i6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < duelOpponentItemCount; l6 += 1)
            {
                int j7 = 9 + byte0 + (l6 % 4) * 49;
                int k7 = 124 + byte1 + (l6 / 4) * 34;
                gameGraphics.DrawImage(j7, k7, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[duelOpponentItems[l6]], Data.GameData.itemPictureMask[duelOpponentItems[l6]], 0, 0, false);
                if (Data.GameData.itemStackable[duelOpponentItems[l6]] == 0)
                {
                    gameGraphics.DrawString(duelOpponentItemsCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (base.mouseX > j7 && base.mouseX < j7 + 48 && base.mouseY > k7 && base.mouseY < k7 + 32)
                {
                    gameGraphics.DrawString(Data.GameData.itemName[duelOpponentItems[l6]] + ": @whi@" + Data.GameData.itemDescription[duelOpponentItems[l6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

        }

        public void DrawWildernessAlertBox()
        {
            int l = 97;
            gameGraphics.DrawBox(86, 77, 340, 180, 0);
            gameGraphics.DrawBoxEdge(86, 77, 340, 180, 0xffffff);
            gameGraphics.DrawText("Warning! Proceed with caution", 256, l, 4, 0xff0000);
            l += 26;
            gameGraphics.DrawText("If you go much further north you will enter the", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.DrawText("wilderness. This a very dangerous area where", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.DrawText("other players can attack you!", 256, l, 1, 0xffffff);
            l += 22;
            gameGraphics.DrawText("The further north you go the more dangerous it", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.DrawText("becomes, but the more treasure you will find.", 256, l, 1, 0xffffff);
            l += 22;
            gameGraphics.DrawText("In the wilderness an indicator at the bottom-right", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.DrawText("of the screen will show the current level of danger", 256, l, 1, 0xffffff);
            l += 22;
            int i1 = 0xffffff;
            if (base.mouseY > l - 12 && base.mouseY <= l && base.mouseX > 181 && base.mouseX < 331)
            {
                i1 = 0xff0000;
            }

            gameGraphics.DrawText("Click here to close window", 256, l, 1, i1);
            if (mouseButtonClick != 0)
            {
                if (base.mouseY > l - 12 && base.mouseY <= l && base.mouseX > 181 && base.mouseX < 331)
                {
                    wildType = 2;
                }

                if (base.mouseX < 86 || base.mouseX > 426 || base.mouseY < 77 || base.mouseY > 257)
                {
                    wildType = 2;
                }

                mouseButtonClick = 0;
            }
        }

        public void DrawNPC(int x, int y, int width, int height, int index, int unknown1, int unknown2)
        {
            ClientMob npc = npcArray[index];
            int frameIndex = npc.currentSprite + (cameraRotation + 16) / 32 & 7;
            bool flag = false;
            int newFrameIndex = frameIndex;
            if (newFrameIndex == 5)
            {
                newFrameIndex = 3;
                flag = true;
            }
            else if (newFrameIndex == 6)
            {
                newFrameIndex = 2;
                flag = true;
            }
            else if (newFrameIndex == 7)
            {
                newFrameIndex = 1;
                flag = true;
            }
            int j1 = newFrameIndex * 3 + walkModel[(npc.stepCount / Data.GameData.npcWalkModelArray[npc.npcId]) % 4];
            if (npc.currentSprite == 8)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = false;
                x -= (Data.GameData.npcCombatSprite[npc.npcId] * unknown2) / 100;
                j1 = newFrameIndex * 3 + combatModelArray1[(tick / (Data.GameData.npcCombatModel[npc.npcId] - 1)) % 8];
            }
            else
                if (npc.currentSprite == 9)
                {
                    newFrameIndex = 5;
                    frameIndex = 2;
                    flag = true;
                    x += (Data.GameData.npcCombatSprite[npc.npcId] * unknown2) / 100;
                    j1 = newFrameIndex * 3 + combatModelArray2[(tick / Data.GameData.npcCombatModel[npc.npcId]) % 8];
                }
            for (int k1 = 0; k1 < 12; k1 += 1)
            {
                int l1 = animationModelArray[frameIndex][k1];
                int k2 = Data.GameData.npcAnimationCount[npc.npcId][l1];
                if (k2 >= 0)
                {
                    int i3 = 0;
                    int j3 = 0;
                    int k3 = j1;
                    if (flag && newFrameIndex >= 1 && newFrameIndex <= 3 && Data.GameData.animationHasF[k2] == 1)
                    {
                        k3 += 15;
                    }

                    if (newFrameIndex != 5 || Data.GameData.animationHasA[k2] == 1)
                    {
                        int l3 = k3 + Data.GameData.animationNumber[k2];
                        i3 = (i3 * width) / ((GameImage)(gameGraphics)).pictureAssumedWidth[l3];
                        j3 = (j3 * height) / ((GameImage)(gameGraphics)).pictureAssumedHeight[l3];
                        int i4 = (width * ((GameImage)(gameGraphics)).pictureAssumedWidth[l3]) / ((GameImage)(gameGraphics)).pictureAssumedWidth[Data.GameData.animationNumber[k2]];
                        i3 -= (i4 - width) / 2;
                        int j4 = Data.GameData.animationCharacterColor[k2];
                        int k4 = 0;
                        if (j4 == 1)
                        {
                            j4 = Data.GameData.npcHairColor[npc.npcId];
                            k4 = Data.GameData.npcSkinColor[npc.npcId];
                        }
                        else
                            if (j4 == 2)
                            {
                                j4 = Data.GameData.npcTopColor[npc.npcId];
                                k4 = Data.GameData.npcSkinColor[npc.npcId];
                            }
                            else
                                if (j4 == 3)
                                {
                                    j4 = Data.GameData.npcBottomColor[npc.npcId];
                                    k4 = Data.GameData.npcSkinColor[npc.npcId];
                                }
                        gameGraphics.DrawImage(x + i3, y + j3, i4, height, l3, j4, k4, unknown1, flag);
                    }
                }
            }

            if (npc.lastMessageTimeout > 0)
            {
                receivedMessageMidPoint[receivedMessagesCount] = gameGraphics.TextWidth(npc.lastMessage, 1) / 2;
                if (receivedMessageMidPoint[receivedMessagesCount] > 150)
                {
                    receivedMessageMidPoint[receivedMessagesCount] = 150;
                }

                receivedMessageHeight[receivedMessagesCount] = (gameGraphics.TextWidth(npc.lastMessage, 1) / 300) * gameGraphics.TextHeightNumber(1);
                receivedMessageX[receivedMessagesCount] = x + width / 2;
                receivedMessageY[receivedMessagesCount] = y;
                receivedMessages[receivedMessagesCount++] = npc.lastMessage;
            }
            if (npc.currentSprite == 8 || npc.currentSprite == 9 || npc.combatTimer != 0)
            {
                if (npc.combatTimer > 0)
                {
                    int i2 = x;
                    if (npc.currentSprite == 8)
                    {
                        i2 -= (20 * unknown2) / 100;
                    }
                    else
                        if (npc.currentSprite == 9)
                    {
                        i2 += (20 * unknown2) / 100;
                    }

                    int l2 = (npc.currentHits * 30) / npc.baseHits;
                    healthBarX[healthBarVisibleCount] = i2 + width / 2;
                    healthBarY[healthBarVisibleCount] = y;
                    healthBarMissing[healthBarVisibleCount++] = l2;
                }
                if (npc.combatTimer > 150)
                {
                    int j2 = x;
                    if (npc.currentSprite == 8)
                    {
                        j2 -= (10 * unknown2) / 100;
                    }
                    else
                        if (npc.currentSprite == 9)
                    {
                        j2 += (10 * unknown2) / 100;
                    }

                    gameGraphics.DrawPicture((j2 + width / 2) - 12, (y + height / 2) - 12, baseInventoryPic + 12);
                    gameGraphics.DrawText(npc.lastDamageCount.ToString(), (j2 + width / 2) - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
        }

        public override void DisplayMessage(string s1)
        {
            if (s1.StartsWith("@bor@"))
            {
                DisplayMessage(s1, 4);
                return;
            }
            if (s1.StartsWith("@que@"))
            {
                DisplayMessage("@whi@" + s1, 5);
                return;
            }
            if (s1.StartsWith("@pri@"))
            {
                DisplayMessage(s1, 6);
                return;
            }
            else
            {
                DisplayMessage(s1, 3);
                return;
            }
        }

        public void DrawAboveHeadThings()
        {
            for (int l = 0; l < receivedMessagesCount; l += 1)
            {
                int height = gameGraphics.TextHeightNumber(1);
                int x = receivedMessageX[l];
                int y = receivedMessageY[l];
                int midpoint = receivedMessageMidPoint[l];
                int l3 = receivedMessageHeight[l];
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int l4 = 0; l4 < l; l4 += 1)
                    {
                        if (y + l3 > receivedMessageY[l4] - height && y - height < receivedMessageY[l4] + receivedMessageHeight[l4] && x - midpoint < receivedMessageX[l4] + receivedMessageMidPoint[l4] && x + midpoint > receivedMessageX[l4] - receivedMessageMidPoint[l4] && receivedMessageY[l4] - height - l3 < y)
                        {
                            y = receivedMessageY[l4] - height - l3;
                            flag = true;
                        }
                    }
                }
                receivedMessageY[l] = y;
                gameGraphics.DrawFloatingText(receivedMessages[l], x, y, 1, 0xffff00, 300);
            }

            for (int j1 = 0; j1 < itemsAboveHeadCount; j1 += 1)
            {
                int x = itemAboveHeadX[j1];
                int y = itemAboveHeadY[j1];
                int scale = itemAboveHeadScale[j1];
                int id = itemAboveHeadID[j1];
                int width = (39 * scale) / 100;
                int height = (27 * scale) / 100;
                int j5 = y - height;
                gameGraphics.DrawTransparentImage(x - width / 2, j5, width, height, baseInventoryPic + 9, 85);
                int k5 = (36 * scale) / 100;
                int l5 = (24 * scale) / 100;
                gameGraphics.DrawImage(x - k5 / 2, (j5 + height / 2) - l5 / 2, k5, l5, Data.GameData.itemInventoryPicture[id] + baseItemPicture, Data.GameData.itemPictureMask[id], 0, 0, false);
            }

            for (int i2 = 0; i2 < healthBarVisibleCount; i2 += 1)
            {
                int x = healthBarX[i2];
                int y = healthBarY[i2];
                int missing = healthBarMissing[i2];
                gameGraphics.DrawBoxAlpha(x - 15, y - 3, missing, 5, 65280, 192);
                gameGraphics.DrawBoxAlpha((x - 15) + missing, y - 3, 30 - missing, 5, 0xff0000, 192);
            }

        }

        public override void CantLogout()
        {
            logoutTimer = 0;
            DisplayMessage("@cya@Sorry, you can't logout at the moment", 3);
        }

        public void DrawBankBox()
        {
            char c1 = '\u0198';
            char c2 = '\u014E';
            if (bankPage > 0 && bankItemsCount <= 48)
            {
                bankPage = 0;
            }

            if (bankPage > 1 && bankItemsCount <= 96)
            {
                bankPage = 1;
            }

            if (bankPage > 2 && bankItemsCount <= 144)
            {
                bankPage = 2;
            }

            if (selectedBankItem >= bankItemsCount || selectedBankItem < 0)
            {
                selectedBankItem = -1;
            }

            if (selectedBankItem != -1 && bankItems[selectedBankItem] != selectedBankItemType)
            {
                selectedBankItem = -1;
                selectedBankItemType = -2;
            }
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                int l = base.mouseX - (256 - c1 / 2);
                int j1 = base.mouseY - (170 - c2 / 2);
                if (l >= 0 && j1 >= 12 && l < 408 && j1 < 280)
                {
                    int l1 = bankPage * 48;
                    for (int k2 = 0; k2 < 6; k2 += 1)
                    {
                        for (int i3 = 0; i3 < 8; i3 += 1)
                        {
                            int k7 = 7 + i3 * 49;
                            int i8 = 28 + k2 * 34;
                            if (l > k7 && l < k7 + 49 && j1 > i8 && j1 < i8 + 34 && l1 < bankItemsCount && bankItems[l1] != -1)
                            {
                                selectedBankItemType = bankItems[l1];
                                selectedBankItem = l1;
                            }
                            l1 += 1;
                        }

                    }

                    l = 256 - c1 / 2;
                    j1 = 170 - c2 / 2;
                    int id;
                    if (selectedBankItem < 0)
                    {
                        id = -1;
                    }
                    else
                    {
                        id = bankItems[selectedBankItem];
                    }

                    if (id != -1)
                    {
                        int count = bankItemCount[selectedBankItem];
                        if (Data.GameData.itemStackable[id] == 1 && count > 1)
                        {
                            count = 1;
                        }

                        if (count >= 1 && base.mouseX >= l + 220 && base.mouseY >= j1 + 238 && base.mouseX < l + 250 && base.mouseY <= j1 + 249)
                        {
                            base.streamClass.CreatePacket(183);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(1);
                            base.streamClass.FormatPacket();
                        }
                        if (count >= 5 && base.mouseX >= l + 250 && base.mouseY >= j1 + 238 && base.mouseX < l + 280 && base.mouseY <= j1 + 249)
                        {
                            base.streamClass.CreatePacket(183);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(5);
                            base.streamClass.FormatPacket();
                        }
                        if (count >= 25 && base.mouseX >= l + 280 && base.mouseY >= j1 + 238 && base.mouseX < l + 305 && base.mouseY <= j1 + 249)
                        {
                            base.streamClass.CreatePacket(183);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(25);
                            base.streamClass.FormatPacket();
                        }
                        if (count >= 100 && base.mouseX >= l + 305 && base.mouseY >= j1 + 238 && base.mouseX < l + 335 && base.mouseY <= j1 + 249)
                        {
                            base.streamClass.CreatePacket(183);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(100);
                            base.streamClass.FormatPacket();
                        }
                        if (count >= 500 && base.mouseX >= l + 335 && base.mouseY >= j1 + 238 && base.mouseX < l + 368 && base.mouseY <= j1 + 249)
                        {
                            base.streamClass.CreatePacket(183);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(500);
                            base.streamClass.FormatPacket();
                        }
                        if (count >= 2500 && base.mouseX >= l + 370 && base.mouseY >= j1 + 238 && base.mouseX < l + 400 && base.mouseY <= j1 + 249)
                        {
                            base.streamClass.CreatePacket(183);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(2500);
                            base.streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 1 && base.mouseX >= l + 220 && base.mouseY >= j1 + 263 && base.mouseX < l + 250 && base.mouseY <= j1 + 274)
                        {
                            base.streamClass.CreatePacket(198);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(1);
                            base.streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 5 && base.mouseX >= l + 250 && base.mouseY >= j1 + 263 && base.mouseX < l + 280 && base.mouseY <= j1 + 274)
                        {
                            base.streamClass.CreatePacket(198);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(5);
                            base.streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 25 && base.mouseX >= l + 280 && base.mouseY >= j1 + 263 && base.mouseX < l + 305 && base.mouseY <= j1 + 274)
                        {
                            base.streamClass.CreatePacket(198);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(25);
                            base.streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 100 && base.mouseX >= l + 305 && base.mouseY >= j1 + 263 && base.mouseX < l + 335 && base.mouseY <= j1 + 274)
                        {
                            base.streamClass.CreatePacket(198);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(100);
                            base.streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 500 && base.mouseX >= l + 335 && base.mouseY >= j1 + 263 && base.mouseX < l + 368 && base.mouseY <= j1 + 274)
                        {
                            base.streamClass.CreatePacket(198);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(500);
                            base.streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 2500 && base.mouseX >= l + 370 && base.mouseY >= j1 + 263 && base.mouseX < l + 400 && base.mouseY <= j1 + 274)
                        {
                            base.streamClass.CreatePacket(198);
                            base.streamClass.AddShort(id);
                            base.streamClass.AddInt(2500);
                            base.streamClass.FormatPacket();
                        }
                    }
                }
                else
                    if (bankItemsCount > 48 && l >= 50 && l <= 115 && j1 <= 12)
                {
                    bankPage = 0;
                }
                else
                        if (bankItemsCount > 48 && l >= 115 && l <= 180 && j1 <= 12)
                {
                    bankPage = 1;
                }
                else
                            if (bankItemsCount > 96 && l >= 180 && l <= 245 && j1 <= 12)
                {
                    bankPage = 2;
                }
                else
                                if (bankItemsCount > 144 && l >= 245 && l <= 310 && j1 <= 12)
                                {
                                    bankPage = 3;
                                }
                                else
                                {
                                    base.streamClass.CreatePacket(48);
                                    base.streamClass.FormatPacket();
                                    showBankBox = false;
                                    return;
                                }
            }
            int i1 = 256 - c1 / 2;
            int k1 = 170 - c2 / 2;
            gameGraphics.DrawBox(i1, k1, 408, 12, 192);
            int j2 = 0x989898;
            gameGraphics.DrawBoxAlpha(i1, k1 + 12, 408, 17, j2, 160);
            gameGraphics.DrawBoxAlpha(i1, k1 + 29, 8, 204, j2, 160);
            gameGraphics.DrawBoxAlpha(i1 + 399, k1 + 29, 9, 204, j2, 160);
            gameGraphics.DrawBoxAlpha(i1, k1 + 233, 408, 47, j2, 160);
            gameGraphics.DrawString("Bank", i1 + 1, k1 + 10, 1, 0xffffff);
            int l2 = 50;
            if (bankItemsCount > 48)
            {
                int k3 = 0xffffff;
                if (bankPage == 0)
                {
                    k3 = 0xff0000;
                }
                else
                    if (base.mouseX > i1 + l2 && base.mouseY >= k1 && base.mouseX < i1 + l2 + 65 && base.mouseY < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                gameGraphics.DrawString("<page 1>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
                k3 = 0xffffff;
                if (bankPage == 1)
                {
                    k3 = 0xff0000;
                }
                else
                    if (base.mouseX > i1 + l2 && base.mouseY >= k1 && base.mouseX < i1 + l2 + 65 && base.mouseY < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                gameGraphics.DrawString("<page 2>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
            }
            if (bankItemsCount > 96)
            {
                int l3 = 0xffffff;
                if (bankPage == 2)
                {
                    l3 = 0xff0000;
                }
                else
                    if (base.mouseX > i1 + l2 && base.mouseY >= k1 && base.mouseX < i1 + l2 + 65 && base.mouseY < k1 + 12)
                {
                    l3 = 0xffff00;
                }

                gameGraphics.DrawString("<page 3>", i1 + l2, k1 + 10, 1, l3);
                l2 += 65;
            }
            if (bankItemsCount > 144)
            {
                int i4 = 0xffffff;
                if (bankPage == 3)
                {
                    i4 = 0xff0000;
                }
                else
                    if (base.mouseX > i1 + l2 && base.mouseY >= k1 && base.mouseX < i1 + l2 + 65 && base.mouseY < k1 + 12)
                {
                    i4 = 0xffff00;
                }

                gameGraphics.DrawString("<page 4>", i1 + l2, k1 + 10, 1, i4);
                l2 += 65;
            }
            int j4 = 0xffffff;
            if (base.mouseX > i1 + 320 && base.mouseY >= k1 && base.mouseX < i1 + 408 && base.mouseY < k1 + 12)
            {
                j4 = 0xff0000;
            }

            gameGraphics.DrawLabel("Close window", i1 + 406, k1 + 10, 1, j4);
            gameGraphics.DrawString("Number in bank in green", i1 + 7, k1 + 24, 1, 65280);
            gameGraphics.DrawString("Number held in blue", i1 + 289, k1 + 24, 1, 65535);
            int l7 = 0xd0d0d0;
            int j8 = bankPage * 48;
            for (int l8 = 0; l8 < 6; l8 += 1)
            {
                for (int i9 = 0; i9 < 8; i9 += 1)
                {
                    int k9 = i1 + 7 + i9 * 49;
                    int l9 = k1 + 28 + l8 * 34;
                    if (selectedBankItem == j8)
                    {
                        gameGraphics.DrawBoxAlpha(k9, l9, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        gameGraphics.DrawBoxAlpha(k9, l9, 49, 34, l7, 160);
                    }

                    gameGraphics.DrawBoxEdge(k9, l9, 50, 35, 0);
                    if (j8 < bankItemsCount && bankItems[j8] != -1)
                    {
                        gameGraphics.DrawImage(k9, l9, 48, 32, baseItemPicture + Data.GameData.itemInventoryPicture[bankItems[j8]], Data.GameData.itemPictureMask[bankItems[j8]], 0, 0, false);
                        gameGraphics.DrawString(bankItemCount[j8].ToString(), k9 + 1, l9 + 10, 1, 65280);
                        gameGraphics.DrawLabel(GetInventoryItemTotalCount(bankItems[j8]).ToString(), k9 + 47, l9 + 29, 1, 65535);
                    }
                    j8 += 1;
                }

            }

            gameGraphics.DrawLineX(i1 + 5, k1 + 256, 398, 0);
            if (selectedBankItem == -1)
            {
                gameGraphics.DrawText("Select an object to withdraw or deposit", i1 + 204, k1 + 248, 3, 0xffff00);
                return;
            }
            int j9;
            if (selectedBankItem < 0)
            {
                j9 = -1;
            }
            else
            {
                j9 = bankItems[selectedBankItem];
            }

            if (j9 != -1)
            {
                int k8 = bankItemCount[selectedBankItem];
                if (Data.GameData.itemStackable[j9] == 1 && k8 > 1)
                {
                    k8 = 1;
                }

                if (k8 > 0)
                {
                    gameGraphics.DrawString("Withdraw " + Data.GameData.itemName[j9], i1 + 2, k1 + 248, 1, 0xffffff);
                    int k4 = 0xffffff;
                    if (base.mouseX >= i1 + 220 && base.mouseY >= k1 + 238 && base.mouseX < i1 + 250 && base.mouseY <= k1 + 249)
                    {
                        k4 = 0xff0000;
                    }

                    gameGraphics.DrawString("One", i1 + 222, k1 + 248, 1, k4);
                    if (k8 >= 5)
                    {
                        int l4 = 0xffffff;
                        if (base.mouseX >= i1 + 250 && base.mouseY >= k1 + 238 && base.mouseX < i1 + 280 && base.mouseY <= k1 + 249)
                        {
                            l4 = 0xff0000;
                        }

                        gameGraphics.DrawString("Five", i1 + 252, k1 + 248, 1, l4);
                    }
                    if (k8 >= 25)
                    {
                        int i5 = 0xffffff;
                        if (base.mouseX >= i1 + 280 && base.mouseY >= k1 + 238 && base.mouseX < i1 + 305 && base.mouseY <= k1 + 249)
                        {
                            i5 = 0xff0000;
                        }

                        gameGraphics.DrawString("25", i1 + 282, k1 + 248, 1, i5);
                    }
                    if (k8 >= 100)
                    {
                        int j5 = 0xffffff;
                        if (base.mouseX >= i1 + 305 && base.mouseY >= k1 + 238 && base.mouseX < i1 + 335 && base.mouseY <= k1 + 249)
                        {
                            j5 = 0xff0000;
                        }

                        gameGraphics.DrawString("100", i1 + 307, k1 + 248, 1, j5);
                    }
                    if (k8 >= 500)
                    {
                        int k5 = 0xffffff;
                        if (base.mouseX >= i1 + 335 && base.mouseY >= k1 + 238 && base.mouseX < i1 + 368 && base.mouseY <= k1 + 249)
                        {
                            k5 = 0xff0000;
                        }

                        gameGraphics.DrawString("500", i1 + 337, k1 + 248, 1, k5);
                    }
                    if (k8 >= 2500)
                    {
                        int l5 = 0xffffff;
                        if (base.mouseX >= i1 + 370 && base.mouseY >= k1 + 238 && base.mouseX < i1 + 400 && base.mouseY <= k1 + 249)
                        {
                            l5 = 0xff0000;
                        }

                        gameGraphics.DrawString("2500", i1 + 370, k1 + 248, 1, l5);
                    }
                }
                if (GetInventoryItemTotalCount(j9) > 0)
                {
                    gameGraphics.DrawString("Deposit " + Data.GameData.itemName[j9], i1 + 2, k1 + 273, 1, 0xffffff);
                    int i6 = 0xffffff;
                    if (base.mouseX >= i1 + 220 && base.mouseY >= k1 + 263 && base.mouseX < i1 + 250 && base.mouseY <= k1 + 274)
                    {
                        i6 = 0xff0000;
                    }

                    gameGraphics.DrawString("One", i1 + 222, k1 + 273, 1, i6);
                    if (GetInventoryItemTotalCount(j9) >= 5)
                    {
                        int j6 = 0xffffff;
                        if (base.mouseX >= i1 + 250 && base.mouseY >= k1 + 263 && base.mouseX < i1 + 280 && base.mouseY <= k1 + 274)
                        {
                            j6 = 0xff0000;
                        }

                        gameGraphics.DrawString("Five", i1 + 252, k1 + 273, 1, j6);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 25)
                    {
                        int k6 = 0xffffff;
                        if (base.mouseX >= i1 + 280 && base.mouseY >= k1 + 263 && base.mouseX < i1 + 305 && base.mouseY <= k1 + 274)
                        {
                            k6 = 0xff0000;
                        }

                        gameGraphics.DrawString("25", i1 + 282, k1 + 273, 1, k6);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 100)
                    {
                        int l6 = 0xffffff;
                        if (base.mouseX >= i1 + 305 && base.mouseY >= k1 + 263 && base.mouseX < i1 + 335 && base.mouseY <= k1 + 274)
                        {
                            l6 = 0xff0000;
                        }

                        gameGraphics.DrawString("100", i1 + 307, k1 + 273, 1, l6);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 500)
                    {
                        int i7 = 0xffffff;
                        if (base.mouseX >= i1 + 335 && base.mouseY >= k1 + 263 && base.mouseX < i1 + 368 && base.mouseY <= k1 + 274)
                        {
                            i7 = 0xff0000;
                        }

                        gameGraphics.DrawString("500", i1 + 337, k1 + 273, 1, i7);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 2500)
                    {
                        int j7 = 0xffffff;
                        if (base.mouseX >= i1 + 370 && base.mouseY >= k1 + 263 && base.mouseX < i1 + 400 && base.mouseY <= k1 + 274)
                        {
                            j7 = 0xff0000;
                        }

                        gameGraphics.DrawString("2500", i1 + 370, k1 + 273, 1, j7);
                    }
                }
            }
        }

        public GraphicsDevice GetGraphics()
        {
            //if(GameApplet.gameFrame is not null)
            //    return GameApplet.gameFrame.GetGraphics();
            //if(Link.gameApplet is not null)
            //    return Link.gameApplet.GetGraphics();
            //else
            //    return base.GetGraphics();
            return graphics;
        }
        public event EventHandler OnLoadingSection;
        public event EventHandler OnLoadingSectionCompleted;
        public bool LoadSection(int x, int y)
        {
            if (playerAliveTimeout != 0)
            {
                engineHandle.playerIsAlive = false;
                return false;
            }
            loadArea = false;
            x += wildX;
            y += wildY;
            if (lastLayerIndex == layerIndex && x > sectionWidth && x < sectionPosX && y > sectionHeight && y < sectionPosY)
            {
                engineHandle.playerIsAlive = true;
                return false;
            }
            if (OnLoadingSection is not null)
            {
                OnLoadingSection(this, new EventArgs());
            }

            gameGraphics.DrawText("Loading... Please wait", 256, 192, 1, 0xffffff);
            DrawChatMessageTabs();


            //gameGraphics.DrawImage(spriteBatch, 0, 0);
            int l = areaX;
            int i1 = areaY;
            int xBase = (x + 24) / 48;
            int yBase = (y + 24) / 48;
            lastLayerIndex = layerIndex;
            areaX = xBase * 48 - 48;
            areaY = yBase * 48 - 48;
            sectionWidth = xBase * 48 - 32;
            sectionHeight = yBase * 48 - 32;
            sectionPosX = xBase * 48 + 32;
            sectionPosY = yBase * 48 + 32;
            engineHandle.LoadSection(x, y, lastLayerIndex);


            areaX -= wildX;
            areaY -= wildY;
            int offsetX = areaX - l;
            int offsetY = areaY - i1;
            for (int j2 = 0; j2 < objectCount; j2 += 1)
            {
                objectX[j2] -= offsetX;
                objectY[j2] -= offsetY;
                int objX = objectX[j2];
                int objY = objectY[j2];
                int objType = objectType[j2];
                GameObject _obj = objectArray[j2];
                try
                {
                    int objDir = objectRotation[j2];
                    int objWidth;
                    int objHeight;
                    if (objDir == 0 || objDir == 4)
                    {
                        objWidth = Data.GameData.objectWidth[objType];
                        objHeight = Data.GameData.objectHeight[objType];
                    }
                    else
                    {
                        objHeight = Data.GameData.objectWidth[objType];
                        objWidth = Data.GameData.objectHeight[objType];
                    }
                    int flatObjX = ((objX + objX + objWidth) * gridSize) / 2;
                    int flatObjY = ((objY + objY + objHeight) * gridSize) / 2;
                    if (objX >= 0 && objY >= 0 && objX < 96 && objY < 96)
                    {
                        gameCamera.AddModel(_obj);
                        _obj.SetPosition(flatObjX, -engineHandle.GetAveragedElevation(flatObjX, flatObjY), flatObjY);
                        engineHandle.CreateObject(objX, objY, objType, objDir);
                        if (objType == 74)
                        {
                            _obj.OffsetPosition(0, -480, 0);
                        }
                    }
                }
                catch (Exception runtimeexception)
                {
                    Console.WriteLine("Loc Error: " + runtimeexception.ToString());
                    Console.WriteLine("x:" + j2 + " obj:" + _obj);
                    //runtimeexception.printStackTrace();
                }
            }


            for (int wallIndex = 0; wallIndex < wallObjectCount; wallIndex += 1)
            {
                wallObjectX[wallIndex] -= offsetX;
                wallObjectY[wallIndex] -= offsetY;
                int wallX = wallObjectX[wallIndex];
                int wallY = wallObjectY[wallIndex];
                int wallId = wallObjectID[wallIndex];
                int wallDir = wallObjectDirection[wallIndex];
                try
                {
                    engineHandle.CreateWall(wallX, wallY, wallDir, wallId);
                    GameObject wallObject = CreateWallObject(wallX, wallY, wallDir, wallId, wallIndex);
                    wallObjectArray[wallIndex] = wallObject;
                }
                catch (Exception runtimeexception1)
                {
                    Console.WriteLine("Bound Error: " + runtimeexception1.ToString());
                    //runtimeexception1.printStackTrace();
                }
            }

            for (int k3 = 0; k3 < groundItemCount; k3 += 1)
            {
                groundItemX[k3] -= offsetX;
                groundItemY[k3] -= offsetY;
            }

            for (int j4 = 0; j4 < playerCount; j4 += 1)
            {
                ClientMob f1 = playerArray[j4];
                f1.currentX -= offsetX * gridSize;
                f1.currentY -= offsetY * gridSize;
                for (int l5 = 0; l5 <= f1.waypointCurrent; l5 += 1)
                {
                    f1.waypointsX[l5] -= offsetX * gridSize;
                    f1.waypointsY[l5] -= offsetY * gridSize;
                }

            }

            for (int i5 = 0; i5 < npcCount; i5 += 1)
            {
                ClientMob f2 = npcArray[i5];
                f2.currentX -= offsetX * gridSize;
                f2.currentY -= offsetY * gridSize;
                for (int k6 = 0; k6 <= f2.waypointCurrent; k6 += 1)
                {
                    f2.waypointsX[k6] -= offsetX * gridSize;
                    f2.waypointsY[k6] -= offsetY * gridSize;
                }

            }

            engineHandle.playerIsAlive = true;
            if (OnLoadingSectionCompleted is not null)
            {
                OnLoadingSectionCompleted(this, new EventArgs());
            }

            OnDrawDone();


            return true;
        }

        public static string formatItemCount(int itemCount)
        {
            string s1 = itemCount.ToString();
            for (int l = s1.Length - 3; l > 0; l -= 3)
            {
                s1 = s1.Substring(0, l) + "," + s1.Substring(l);
            }

            if (s1.Length > 8)
            {
                s1 = "@gre@" + s1.Substring(0, s1.Length - 8) + " million @whi@(" + s1 + ")";
            }
            else
                if (s1.Length > 4)
            {
                s1 = "@cya@" + s1.Substring(0, s1.Length - 4) + "K @whi@(" + s1 + ")";
            }

            return s1;
        }

        public bool HasRequiredRunes(int l, int i1)
        {
            if (l == 31 && (IsItemEquipped(197) || IsItemEquipped(615) || IsItemEquipped(682)))
            {
                return true;
            }

            if (l == 32 && (IsItemEquipped(102) || IsItemEquipped(616) || IsItemEquipped(683)))
            {
                return true;
            }

            if (l == 33 && (IsItemEquipped(101) || IsItemEquipped(617) || IsItemEquipped(684)))
            {
                return true;
            }

            if (l == 34 && (IsItemEquipped(103) || IsItemEquipped(618) || IsItemEquipped(685)))
            {
                return true;
            }

            return GetInventoryItemTotalCount(l) >= i1;
        }

        public void DisplayMessage(string message, int type)
        {
            if (type == 2 || type == 4 || type == 6)
            {
                for (; message.Length > 5 && message[0] == '@' && message[4] == '@'; message = message.Substring(5))
                {
                    ;
                }

                int l = message.IndexOf(":");
                if (l != -1)
                {
                    string s1 = message.Substring(0, l);
                    long l1 = DataOperations.NameToHash(s1);
                    for (int j1 = 0; j1 < base.ignoresCount; j1 += 1)
                    {
                        if (base.ignoresList[j1] == l1)
                        {
                            return;
                        }
                    }
                }
            }
            if (type == 2)
            {
                message = "@yel@" + message;
            }

            if (type == 3 || type == 4)
            {
                message = "@whi@" + message;
            }

            if (type == 6)
            {
                message = "@cya@" + message;
            }

            if (messagesTab != 0)
            {
                if (type == 4 || type == 3)
                {
                    chatTabAllMsgFlash = 200;
                }

                if (type == 2 && messagesTab != 1)
                {
                    chatTabHistoryFlash = 200;
                }

                if (type == 5 && messagesTab != 2)
                {
                    chatTabQuestFlash = 200;
                }

                if (type == 6 && messagesTab != 3)
                {
                    chatTabPrivateFlash = 200;
                }

                if (type == 3 && messagesTab != 0)
                {
                    messagesTab = 0;
                }

                if (type == 6 && messagesTab != 3 && messagesTab != 0)
                {
                    messagesTab = 0;
                }
            }
            for (int i1 = 4; i1 > 0; i1 -= 1)
            {
                messagesArray[i1] = messagesArray[i1 - 1];
                messagesTimeout[i1] = messagesTimeout[i1 - 1];
            }

            messagesArray[0] = message;
            messagesTimeout[0] = 300;
            if (type == 2)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType2] == chatInputMenu.listLength[messagesHandleType2] - 4)
                {
                    chatInputMenu.AddMessage(messagesHandleType2, message, true);
                }
                else
                {
                    chatInputMenu.AddMessage(messagesHandleType2, message, false);
                }
            }

            if (type == 5)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType5] == chatInputMenu.listLength[messagesHandleType5] - 4)
                {
                    chatInputMenu.AddMessage(messagesHandleType5, message, true);
                }
                else
                {
                    chatInputMenu.AddMessage(messagesHandleType5, message, false);
                }
            }

            if (type == 6)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType6] == chatInputMenu.listLength[messagesHandleType6] - 4)
                {
                    chatInputMenu.AddMessage(messagesHandleType6, message, true);
                    return;
                }
                chatInputMenu.AddMessage(messagesHandleType6, message, false);
            }
        }

        public void DrawMinimapObject(int x, int y, int color)
        {
            gameGraphics.DrawMinimapPixel(x, y, color);
            gameGraphics.DrawMinimapPixel(x - 1, y, color);
            gameGraphics.DrawMinimapPixel(x + 1, y, color);
            gameGraphics.DrawMinimapPixel(x, y - 1, color);
            gameGraphics.DrawMinimapPixel(x, y + 1, color);
        }

        public void DrawServerMessageBox()
        {
            char c1 = '\u0190';
            char c2 = 'd';
            if (serverMessageBoxTop)
            {
                c2 = '\u01C2';
                c2 = '\u012C';
            }
            gameGraphics.DrawBox(256 - c1 / 2, 167 - c2 / 2, c1, c2, 0);
            gameGraphics.DrawBoxEdge(256 - c1 / 2, 167 - c2 / 2, c1, c2, 0xffffff);
            gameGraphics.DrawFloatingText(serverMessage, 256, (167 - c2 / 2) + 20, 1, 0xffffff, c1 - 40);
            int l = 157 + c2 / 2;
            int i1 = 0xffffff;
            if (base.mouseY > l - 12 && base.mouseY <= l && base.mouseX > 106 && base.mouseX < 406)
            {
                i1 = 0xff0000;
            }

            gameGraphics.DrawText("Click here to close window", 256, l, 1, i1);
            if (mouseButtonClick == 1)
            {
                if (i1 == 0xff0000)
                {
                    showServerMessageBox = false;
                }

                if ((base.mouseX < 256 - c1 / 2 || base.mouseX > 256 + c1 / 2) && (base.mouseY < 167 - c2 / 2 || base.mouseY > 167 + c2 / 2))
                {
                    showServerMessageBox = false;
                }
            }
            mouseButtonClick = 0;
        }

        //public Texture2D createImage(int l, int i1)
        //{
        //    //if(GameApplet.gameFrame is not null)
        //    //    return GameApplet.gameFrame.createImage(l, i1);
        //    //if(Link.gameApplet is not null)
        //    //    return Link.gameApplet.createImage(l, i1);
        //    //else
        //    return base.createImage(l, i1);
        //}

        public GameObject CreateWallObject(int x, int y, int dir, int type, int totalCount)
        {

            int tileX = x;
            int tileY = y;
            int destTileX = x;
            int destTileY = y;
            int textureBack = Data.GameData.wallObjectModel_FaceBack[type];
            int textureFront = Data.GameData.wallObjectModel_FaceFront[type];
            int wallHeight = Data.GameData.wallObjectModelHeight[type];
            GameObject wallModel = new(4, 1);

            //
            //    _ _ _ _
            //
            //
            if (dir == 0)
            {
                destTileX = x + 1;
            }

            //    |
            //    |
            //    |
            //    |
            if (dir == 1)
            {
                destTileY = y + 1;
            }

            //       /
            //      /
            //     /
            //    /
            if (dir == 2)
            {
                tileX = x + 1;
                destTileY = y + 1;
            }

            //    \
            //     \
            //      \
            //       \
            if (dir == 3)
            {
                destTileX = x + 1;
                destTileY = y + 1;
            }
            tileX *= gridSize;
            tileY *= gridSize;
            destTileX *= gridSize;
            destTileY *= gridSize;

            // add vertex index bottomLeft
            int bLeft = wallModel.GetVertexIndex(tileX, -engineHandle.GetAveragedElevation(tileX, tileY), tileY);

            // add vertex index topLeft
            int tLeft = wallModel.GetVertexIndex(tileX, -engineHandle.GetAveragedElevation(tileX, tileY) - wallHeight, tileY);

            // add vertex index topRight
            int tRight = wallModel.GetVertexIndex(destTileX, -engineHandle.GetAveragedElevation(destTileX, destTileY) - wallHeight, destTileY);

            // vertex index bottomRight
            int bRight = wallModel.GetVertexIndex(destTileX, -engineHandle.GetAveragedElevation(destTileX, destTileY), destTileY);
            int[] faceVertices = [
            bLeft, tLeft, tRight, bRight
        ];
            wallModel.AddFaceVertices(4, faceVertices, textureBack, textureFront);
            wallModel.UpdateShading(false, 60, 24, -50, -10, -50);
            if (x >= 0 && y >= 0 && x < 96 && y < 96)
            {
                gameCamera.AddModel(wallModel);
            }

            wallModel.index = totalCount + 10000;
            return wallModel;
        }

        public void ResetPrivateMessages()
        {
            base.pmText = "";
            base.enteredPMText = "";
        }

        public ClientMob CreateNpc(int index, int x, int y, int sprite, int id)
        {
            if (npcAttackingArray[index] is null)
            {
                npcAttackingArray[index] = new ClientMob
                {
                    serverIndex = index
                };
            }
            ClientMob f1 = npcAttackingArray[index];
            bool flag = false;
            for (int l = 0; l < lastNpcCount; l += 1)
            {
                if (lastNpcArray[l].serverIndex != index)
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (flag)
            {
                f1.npcId = id;
                f1.nextSprite = sprite;
                int i1 = f1.waypointCurrent;
                if (x != f1.waypointsX[i1] || y != f1.waypointsY[i1])
                {
                    f1.waypointCurrent = i1 = (i1 + 1) % 10;
                    f1.waypointsX[i1] = x;
                    f1.waypointsY[i1] = y;
                }
            }
            else
            {
                f1.serverIndex = index;
                f1.waypointsEndSprite = 0;
                f1.waypointCurrent = 0;
                f1.waypointsX[0] = f1.currentX = x;
                f1.waypointsY[0] = f1.currentY = y;
                f1.npcId = id;
                f1.nextSprite = f1.currentSprite = sprite;
                f1.stepCount = 0;
            }
            npcArray[npcCount++] = f1;
            return f1;
        }

        public void UpdateBankItems()
        {
            bankItemsCount = serverBankItemsCount;
            for (int l = 0; l < serverBankItemsCount; l += 1)
            {
                bankItems[l] = serverBankItems[l];
                bankItemCount[l] = serverBankItemCount[l];
            }

            for (int i1 = 0; i1 < inventoryItemsCount; i1 += 1)
            {
                if (bankItemsCount >= maxBankItems)
                {
                    break;
                }

                int j1 = inventoryItems[i1];
                bool flag = false;
                for (int k1 = 0; k1 < bankItemsCount; k1 += 1)
                {
                    if (bankItems[k1] != j1)
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    bankItems[bankItemsCount] = j1;
                    bankItemCount[bankItemsCount] = 0;
                    bankItemsCount += 1;
                }
            }

        }

        public void DrawStatsQuestsMenu(bool canClick)
        {
            int l = ((GameImage)(gameGraphics)).gameWidth - 199; //199
            int i1 = 36;
            gameGraphics.DrawPicture(l - 49, 3, baseInventoryPic + 3);
            int c1 = 196;//'\u304';
            int c2 = 275;//113;//'\u0113';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (questMenuSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            gameGraphics.DrawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.DrawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.DrawBoxAlpha(l, i1 + 24, c1, c2 - 24, GameImage.RgbToInt(220, 220, 220), 128);
            gameGraphics.DrawLineX(l, i1 + 24, c1, 0);
            gameGraphics.DrawLineY(l + c1 / 2, i1, 24, 0);
            gameGraphics.DrawText("Stats", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.DrawText("Quests", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            if (questMenuSelected == 0)
            {
                int l1 = 72;
                int j2 = -1;
                gameGraphics.DrawString("Skills", l + 5, l1, 3, 0xffff00);
                l1 += 13;
                for (int k2 = 0; k2 < 9; k2 += 1)
                {
                    int l2 = 0xffffff;
                    if (base.mouseX > l + 3 && base.mouseY >= l1 - 11 && base.mouseY < l1 + 2 && base.mouseX < l + 90)
                    {
                        l2 = 0xff0000;
                        j2 = k2;
                    }
                    gameGraphics.DrawString(skillName[k2] + ":@yel@" + playerStatCurrent[k2] + "/" + playerStatBase[k2], l + 5, l1, 1, l2);
                    l2 = 0xffffff;
                    if (base.mouseX >= l + 90 && base.mouseY >= l1 - 13 - 11 && base.mouseY < (l1 - 13) + 2 && base.mouseX < l + 196)
                    {
                        l2 = 0xff0000;
                        j2 = k2 + 9;
                    }
                    gameGraphics.DrawString(skillName[k2 + 9] + ":@yel@" + playerStatCurrent[k2 + 9] + "/" + playerStatBase[k2 + 9], (l + c1 / 2) - 5, l1 - 13, 1, l2);
                    l1 += 13;
                }

                gameGraphics.DrawString("Quest Points:@yel@" + questPoints, (l + c1 / 2) - 5, l1 - 13, 1, 0xffffff);
                l1 += 12;
                gameGraphics.DrawString("Fatigue: @yel@" + (fatigue * 100) / 750 + "%", l + 5, l1 - 13, 1, 0xffffff);
                l1 += 8;
                gameGraphics.DrawString("Equipment Status", l + 5, l1, 3, 0xffff00);
                l1 += 12;
                for (int i3 = 0; i3 < 3; i3 += 1)
                {
                    gameGraphics.DrawString(gearStats[i3] + ":@yel@" + equipmentStatus[i3], l + 5, l1, 1, 0xffffff);
                    if (i3 < 2)
                    {
                        gameGraphics.DrawString(gearStats[i3 + 3] + ":@yel@" + equipmentStatus[i3 + 3], l + c1 / 2 + 25, l1, 1, 0xffffff);
                    }

                    l1 += 13;
                }

                l1 += 6;
                gameGraphics.DrawLineX(l, l1 - 15, c1, 0);
                if (j2 != -1)
                {
                    gameGraphics.DrawString(skillNameVerb[j2] + " skill", l + 5, l1, 1, 0xffff00);
                    l1 += 12;
                    int j3 = experienceList[0];
                    for (int l3 = 0; l3 < 98; l3 += 1)
                    {
                        if (playerStatExp[j2] >= experienceList[l3])
                        {
                            j3 = experienceList[l3 + 1];
                        }
                    }

                    gameGraphics.DrawString("Total xp: " + playerStatExp[j2], l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                    gameGraphics.DrawString("Next level at: " + j3, l + 5, l1, 1, 0xffffff);
                }
                else
                {
                    gameGraphics.DrawString("Overall levels", l + 5, l1, 1, 0xffff00);
                    l1 += 12;
                    int k3 = 0;
                    for (int i4 = 0; i4 < 18; i4 += 1)
                    {
                        k3 += playerStatBase[i4];
                    }

                    gameGraphics.DrawString("Skill total: " + k3, l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                    gameGraphics.DrawString("Combat level: " + ourPlayer.level, l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                }
            }
            if (questMenuSelected == 1)
            {
                questMenu.ClearList(questMenuHandle);
                questMenu.AddListItem(questMenuHandle, 0, "@whi@Quest-list (green=completed)");
                for (int i2 = 0; i2 < usedQuestName.Length; i2 += 1)
                {
                    string questColor;

                    if (questStage[i2] == 0)
                    {
                        questColor = "@red@";
                    }
                    else if (questStage[i2] == 1)
                    {
                        questColor = "@yel@";
                    }
                    else
                    {
                        questColor = "@gre@";
                    }

                    questMenu.AddListItem(questMenuHandle, i2 + 1, questColor + usedQuestName[i2]);
                }

                questMenu.DrawMenu();
            }
            if (!canClick)
            {
                return;
            }

            l = base.mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = base.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < c1 && i1 < c2)
            {
                if (questMenuSelected == 1)
                {
                    questMenu.MouseClick(l + (((GameImage)(gameGraphics)).gameWidth - 199), i1 + 36, base.lastMouseButton, base.mouseButton);
                }

                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98)
                    {
                        questMenuSelected = 0;
                        return;
                    }
                    if (l > 98)
                    {
                        questMenuSelected = 1;
                    }
                }
            }
        }

        public void DrawFriendsBox()
        {
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                if (showFriendsBox == 1 && (base.mouseX < 106 || base.mouseY < 145 || base.mouseX > 406 || base.mouseY > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (showFriendsBox == 2 && (base.mouseX < 6 || base.mouseY < 145 || base.mouseX > 506 || base.mouseY > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (showFriendsBox == 3 && (base.mouseX < 106 || base.mouseY < 145 || base.mouseX > 406 || base.mouseY > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (base.mouseX > 236 && base.mouseX < 276 && base.mouseY > 193 && base.mouseY < 213)
                {
                    showFriendsBox = 0;
                    return;
                }
            }
            int l = 145;
            if (showFriendsBox == 1)
            {
                gameGraphics.DrawBox(106, l, 300, 70, 0);
                gameGraphics.DrawBoxEdge(106, l, 300, 70, 0xffffff);
                l += 20;
                gameGraphics.DrawText("Enter name to add to friends list", 256, l, 4, 0xffffff);
                l += 20;
                gameGraphics.DrawText(base.inputText + "*", 256, l, 4, 0xffffff);
                if (base.enteredInputText.Length > 0)
                {
                    string s1 = base.enteredInputText.Trim();
                    base.inputText = "";
                    base.enteredInputText = "";
                    showFriendsBox = 0;
                    if (s1.Length > 0 && DataOperations.NameToHash(s1) != ourPlayer.nameHash)
                    {
                        AddFriend(s1);
                    }
                }
            }
            if (showFriendsBox == 2)
            {
                gameGraphics.DrawBox(6, l, 500, 70, 0);
                gameGraphics.DrawBoxEdge(6, l, 500, 70, 0xffffff);
                l += 20;
                gameGraphics.DrawText("Enter message to send to " + DataOperations.HashToName(pmTarget), 256, l, 4, 0xffffff);
                l += 20;
                gameGraphics.DrawText(base.pmText + "*", 256, l, 4, 0xffffff);
                if (base.enteredPMText.Length > 0)
                {
                    string s2 = base.enteredPMText;
                    base.pmText = "";
                    base.enteredPMText = "";
                    showFriendsBox = 0;
                    int j1 = ChatMessage.StringToBytes(s2);
                    SendPrivateMessage(pmTarget, ChatMessage.lastChat, j1);
                    s2 = ChatMessage.BytesToString(ChatMessage.lastChat, 0, j1);
                    //if (useChatFilter)
                    // s2 = ChatFilter.filterChat(s2);
                    DisplayMessage("@pri@You tell " + DataOperations.HashToName(pmTarget) + ": " + s2);
                }
            }
            if (showFriendsBox == 3)
            {
                gameGraphics.DrawBox(106, l, 300, 70, 0);
                gameGraphics.DrawBoxEdge(106, l, 300, 70, 0xffffff);
                l += 20;
                gameGraphics.DrawText("Enter name to add to ignore list", 256, l, 4, 0xffffff);
                l += 20;
                gameGraphics.DrawText(base.inputText + "*", 256, l, 4, 0xffffff);
                if (base.enteredInputText.Length > 0)
                {
                    string s3 = base.enteredInputText.Trim();
                    base.inputText = "";
                    base.enteredInputText = "";
                    showFriendsBox = 0;
                    if (s3.Length > 0 && DataOperations.NameToHash(s3) != ourPlayer.nameHash)
                    {
                        AddIgnore(s3);
                    }
                }
            }
            int i1 = 0xffffff;
            if (base.mouseX > 236 && base.mouseX < 276 && base.mouseY > 193 && base.mouseY < 213)
            {
                i1 = 0xffff00;
            }

            gameGraphics.DrawText("Cancel", 256, 208, 1, i1);
        }

        public void PlaySound(string s1)
        {
            if (audioPlayer is null || !Config.MembersFeatures)
            {
                return;
            }

            if (!configSoundOff)
            {
                int off = (int)DataOperations.GetObjectOffset(s1 + ".pcm", soundData);
                int len = DataOperations.GetSoundLength(s1 + ".pcm", soundData);
                audioPlayer.Play(soundData, off, len);
            }
        }

        public void DrawRightClickMenu()
        {
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < menuOptionsCount; l += 1)
                {
                    int j1 = menuX + 2;
                    int l1 = menuY + 27 + l * 15;
                    if (base.mouseX <= j1 - 2 || base.mouseY <= l1 - 12 || base.mouseY >= l1 + 4 || base.mouseX >= (j1 - 3) + menuWidth)
                    {
                        continue;
                    }

                    MenuClick(menuIndexes[l]);
                    break;
                }

                mouseButtonClick = 0;
                menuShow = false;
                return;
            }
            if (base.mouseX < menuX - 10 || base.mouseY < menuY - 10 || base.mouseX > menuX + menuWidth + 10 || base.mouseY > menuY + menuHeight + 10)
            {
                menuShow = false;
                return;
            }
            gameGraphics.DrawBoxAlpha(menuX, menuY, menuWidth, menuHeight, 0xd0d0d0, 160);
            gameGraphics.DrawString("Choose option", menuX + 2, menuY + 12, 1, 65535);
            for (int i1 = 0; i1 < menuOptionsCount; i1 += 1)
            {
                int k1 = menuX + 2;
                int i2 = menuY + 27 + i1 * 15;
                int j2 = 0xffffff;
                if (base.mouseX > k1 - 2 && base.mouseY > i2 - 12 && base.mouseY < i2 + 4 && base.mouseX < (k1 - 3) + menuWidth)
                {
                    j2 = 0xffff00;
                }

                string menuSecondaryText = menuText2[menuIndexes[i1]];
                gameGraphics.DrawString(menuText1[menuIndexes[i1]] + " " + menuText2[menuIndexes[i1]], k1, i2, 1, j2);
            }

        }

        public void GetMenuHighlighted()
        {
            if (drawMenuTab == 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 && base.mouseY < 35)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab == 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 33 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 33 && base.mouseY < 35)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab == 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 66 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 66 && base.mouseY < 35)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab == 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 99 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 99 && base.mouseY < 35)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab == 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 132 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 132 && base.mouseY < 35)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab == 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 165 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 165 && base.mouseY < 35)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab != 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 && base.mouseY < 26)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab != 0 && drawMenuTab != 2 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 33 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 33 && base.mouseY < 26)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab != 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 66 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 66 && base.mouseY < 26)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab != 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 99 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 99 && base.mouseY < 26)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab != 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 132 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 132 && base.mouseY < 26)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab != 0 && base.mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 165 && base.mouseY >= 3 && base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 165 && base.mouseY < 26)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab == 1 && (base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 248 || base.mouseY > 36 + (maxInventoryItems / 5) * 34))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 3 && (base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 199 || base.mouseY > 316))
            {
                drawMenuTab = 0;
            }

            if ((drawMenuTab == 2 || drawMenuTab == 4 || drawMenuTab == 5) && (base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 199 || base.mouseY > 240))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 6 && (base.mouseX < ((GameImage)(gameGraphics)).gameWidth - 199 || base.mouseY > 326))
            {
                drawMenuTab = 0;
            }
        }

        protected int GetUID()
        {
            return Link.userId;
        }

        public bool TakeScreenshot(bool verb)
        {
            //try
            //{
            //    string charName = DataOperations.hashToName(DataOperations.nameToHash(username));
            //    File dir = new File(Config.MEDIA_DIR + "/" + charName);
            //    if (!dir.exists() || !dir.isDirectory())
            //        dir.mkdir();
            //    string folder = dir.getPath() + "/";
            //    File file = null;
            //    for (int count = 0; file is null || file.exists(); count += 1)
            //        file = new File(folder + "screenshot" + count + ".png");
            //    BufferedImage bi = new BufferedImage(windowWidth, windowHeight + 11, BufferedImage.TYPE_INT_RGB);
            //    Graphics2D g2d = bi.createGraphics();
            //    g2d.DrawImage(gameGraphics.image, 0, 0, this);
            //    g2d.dispose();
            //    ImageIO.write(bi, "png", file);
            //    if (verb)
            //        DisplayMessage("Screenshot saved as " + file.getName());
            //    return true;
            //}
            //catch (IOException ioe)
            //{
            //    if (verb)
            //        DisplayMessage("Error saving screenshot");
            //    return false;
            //}
            return true;
        }

        public ClientMob GetLastPlayer(int serverIndex)
        {
            for (int i1 = 0; i1 < lastPlayerCount; i1 += 1)
            {
                if (lastPlayerArray[i1].serverIndex == serverIndex)
                {
                    return lastPlayerArray[i1];
                }
            }
            return null;
        }

        public ClientMob GetLastNpc(int serverIndex)
        {
            for (int i1 = 0; i1 < lastNpcCount; i1 += 1)
            {
                if (lastNpcArray[i1].serverIndex == serverIndex)
                {
                    return lastNpcArray[i1];
                }
            }
            return null;
        }

        public bool HandleCommand(string command)
        {
            try
            {
                int firstSpace = command.IndexOf(' ');
                string cmd = command;
                string[] args = [];
                if (firstSpace != -1)
                {
                    cmd = command.Substring(0, firstSpace).Trim();
                    args = command.Substring(firstSpace).Trim().Split(' ');
                }
                if (cmd == "closecon")
                {
                    base.streamClass.CloseStream();
                    return true;
                }
                if (cmd == "logout")
                {
                    SendLogout();
                    return true;
                }
                if (cmd == "lostcon")
                {
                    LostConnection();
                    return true;
                }
                if (cmd == "tell")
                {
                    long recipient = DataOperations.NameToHash(args[0]);
                    string message = JoinString(args, " ", 1).Trim();
                    if (message == "")
                    {
                        return true;
                    }

                    int len = ChatMessage.StringToBytes(message);
                    SendPrivateMessage(recipient, ChatMessage.lastChat, len);
                    message = ChatMessage.BytesToString(ChatMessage.lastChat, 0, len);
                    //  if (useChatFilter)
                    //      message = ChatFilter.filterChat(message);
                    DisplayMessage("@pri@You tell " + DataOperations.HashToName(recipient) + ": " + message);
                    return true;
                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
            return false;
        }

        public string JoinString(string[] hay, string glue, int start)
        {
            string ret = "";

            for (int i = start; i < hay.Length; i += 1)
            {
                ret += hay[i];

                if (i != hay.Length - 1)
                {
                    ret += glue;
                }
            }

            return ret;
        }

        public string JoinString(string[] hay, string glue)
        {
            return JoinString(hay, glue, 0);
        }

        public GameClient()
        {
            tradeOtherName = "";


            windowWidth = 512;
            windowHeight = 334;


            cameraFieldOfView = 9;
            showQuestionMenu = false;
            loginScreenShown = false;
            questionMenuAnswer = new string[10];
            appearanceBodyGender = 1;
            appearance2Colour = 2;
            appearanceHairColour = 2;
            appearanceTopColour = 8;
            appearanceBottomColour = 14;
            appearanceHeadGender = 1;
            menuIndexes = new int[250];
            duelMyItems = new int[8];
            duelMyItemsCount = new int[8];
            playerArray = new ClientMob[500];
            selectedShopItemIndex = -1;
            selectedShopItemType = -2;
            menuText1 = new string[250];
            isSleeping = false;
            tradeItemsOther = new int[14];
            tradeItemOtherCount = new int[14];
            tradeOtherAccepted = false;
            tradeWeAccepted = false;
            itemAboveHeadScale = new int[50];
            itemAboveHeadID = new int[50];
            playerStatCurrent = new int[18];
            menuActionX = new int[250];
            menuActionY = new int[250];
            menuActionID = new int[250];
            showTradeBox = false;
            npcArray = new ClientMob[500];
            duelNoRetreating = false;
            duelNoMagic = false;
            duelNoPrayer = false;
            duelNoWeapons = false;
            playerBufferArray = new ClientMob[4000];
            serverMessage = "";
            duelOpponentAccepted = false;
            duelMyAccepted = false;
            wallObjectX = new int[500];
            wallObjectY = new int[500];
            serverMessageBoxTop = false;
            cameraRotationYIncrement = 2;
            wallObjectArray = new GameObject[500];
            messagesArray = new string[5];
            objectAlreadyInMenu = new bool[1500];
            objectArray = new GameObject[1500];
            selectedSpell = -1;
            cameraAutoAngleDebug = false;
            ourPlayer = new ClientMob();
            serverIndex = -1;
            tradeItemsOur = new int[14];
            tradeItemOurCount = new int[14];
            showWelcomeBox = false;
            menuActionType = new int[250];
            menuActionVar1 = new int[250];
            menuActionVar2 = new int[250];
            sleepWordDelay = true;
            configCameraAutoAngle = true;
            cameraRotation = 128;
            configSoundOff = false;
            menuShow = false;
            duelOpponentItems = new int[8];
            duelOpponentItemsCount = new int[8];
            showBankBox = false;
            playerStatBase = new int[18];
            serverBankItems = new int[256];
            serverBankItemCount = new int[256];
            showShopBox = false;
            groundItemX = new int[5000];
            groundItemY = new int[5000];
            groundItemID = new int[5000];
            groundItemObjectVar = new int[5000];
            maxBankItems = 48;
            tradeConfirmOtherItems = new int[14];
            tradeConfirmOtherItemsCount = new int[14];
            layerIndex = -1;
            walkArrayX = new int[8000];
            walkArrayY = new int[8000];
            cameraDistance = 550;
            receivedMessageX = new int[50];
            receivedMessageY = new int[50];
            receivedMessageMidPoint = new int[50];
            receivedMessageHeight = new int[50];
            wallObjectAlreadyInMenu = new bool[500];
            lastLayerIndex = -1;
            bankItems = new int[256];
            bankItemCount = new int[256];
            maxInventoryItems = 30;
            errorLoading = false;
            itemAboveHeadX = new int[50];
            itemAboveHeadY = new int[50];
            showServerMessageBox = false;
            playerBufferArrayIndexes = new int[500];
            tradeConfirmItems = new int[14];
            tradeConfigItemsCount = new int[14];
            selectedBankItem = -1;
            selectedBankItemType = -2;
            showDuelConfirmBox = false;
            duelConfirmOurAccepted = false;
            wallObjectDirection = new int[500];
            wallObjectID = new int[500];
            gameDataObjects = new GameObject[1000];
            lastNpcArray = new ClientMob[500];
            inventoryItems = new int[35];
            inventoryItemCount = new int[35];
            inventoryItemEquipped = new int[35];
            selectedItem = -1;
            selectedItemName = "";
            lastPlayerArray = new ClientMob[500];
            showTradeConfirmBox = false;
            tradeConfirmAccepted = false;
            playerStatExp = new int[18];
            mouseTrailX = new int[8192];
            mouseTrailY = new int[8192];
            configOneMouseButton = false;
            prayerOn = new bool[50];
            shopItems = new int[256];
            shopItemCount = new int[256];
            shopItemBasePriceModifier = new int[256];
            duelOpponentStakeItem = new int[8];
            duelOutStakeItemCount = new int[8];
            equipmentStatus = new int[5];
            receivedMessages = new string[50];
            cameraRotationXIncrement = 2;
            teleBubbleTime = new int[50];
            gridSize = 128;
            questStage = new int[questName.Length];
            teleBubbleType = new int[50];
            experienceList = new int[99];
            lastModelFireLightningSpellNumber = -1;
            lastModelTorchNumber = -1;
            lastModelClawSpellNumber = -1;
            messagesTimeout = new int[5];
            projectileRange = 40;
            memoryError = false;
            duelOurStakeItem = new int[8];
            duelOurStakeItemCount = new int[8];
            menuText2 = new string[250];
            loginUsername = "";
            loginPassword = "";
            duelOpponent = "";
            healthBarX = new int[50];
            healthBarY = new int[50];
            healthBarMissing = new int[50];
            objectX = new int[1500];
            objectY = new int[1500];
            objectType = new int[1500];
            objectRotation = new int[1500];
            showDuelBox = false;
            npcAttackingArray = new ClientMob[5000];
            teleBubbleY = new int[50];
            cameraAutoAngle = 1;
            loadArea = false;
            teleBubbleX = new int[50];
            showAppearanceWindow = false;
            cameraZoom = false;

            fogOfWar = true;
            showCombatWindow = false;
            showRoofs = true;
            autoScreenshot = false;
            useChatFilter = true;
            usedQuestName = [];
            subDaysLeft = 0;
            shopItemSellPrice = new int[256];
            shopItemBuyPrice = new int[256];
            captchaPixels = [];
            captchaWidth = 0;
            captchaHeight = 0;
            needsClear = false;
            hasWorldInfo = false;
            //ImageIO.setCacheDirectory(new File(Config.CONF_DIR));
        }

        public string tradeOtherName;
        public int windowWidth;
        public int windowHeight;
        public int cameraFieldOfView;
        public bool showQuestionMenu;
        public bool loginScreenShown;
        public string[] questionMenuAnswer;
        public int appearanceHeadType;
        public int appearanceBodyGender;
        public int appearance2Colour;
        public int appearanceHairColour;
        public int appearanceTopColour;
        public int appearanceBottomColour;
        public int appearanceSkinColour;
        public int appearanceHeadGender;
        public Menu chatInputMenu;
        private int messagesHandleType2;
        private int chatInputBox;
        private int messagesHandleType5;
        private int messagesHandleType6;
        private int messagesTab;
        public int[] menuIndexes;
        public int duelMyItemCount;
        public int[] duelMyItems;
        public int[] duelMyItemsCount;
        public int systemUpdate;
        public ClientMob[] playerArray;
        public string[] questName = [// TODO really?... needs to be done better imho
            "Cook's Assistant", "Sheep Shearer", "Black knight's fortress", "Imp catcher", "Vampire slayer",
            "Romeo & Juliet", "The restless ghost", "Doric's quest", "The knight's sword", "Witch's potion",
            "Goblin diplomacy", "Ernest the chicken", "Demon Slayer", "Pirate's treasure", "Prince Ali Rescue",
            "Shield of Arrav", "Dragon Slayer"
        /*"Black knight's fortress", "Cook's assistant", "Demon slayer", "Doric's quest", "The restless ghost", "Goblin diplomacy", "Ernest the chicken",
        "Imp catcher", "Pirate's treasure", "Prince Ali rescue",
        "Romeo & Juliet", "Sheep shearer", "Shield of Arrav", "The knight's sword", "Vampire slayer", "Witch's potion", "Dragon slayer", "Witch's house (members)",
        "Lost city (members)", "Hero's quest (members)",
        "Druidic ritual (members)", "Merlin's crystal (members)", "Scorpion catcher (members)", "Family crest (members)", "Tribal totem (members)",
        "Fishing contest (members)", "Monk's friend (members)", "Temple of Ikov (members)", "Clock tower (members)", "The Holy Grail (members)",
        "Fight Arena (members)", "Tree Gnome Village (members)", "The Hazeel Cult (members)", "Sheep Herder (members)", "Plague City (members)",
        "Sea Slug (members)", "Waterfall quest (members)", "Biohazard (members)", "Jungle potion (members)", "Grand tree (members)",
        "Shilo village (members)", "Underground pass (members)", "Observatory quest (members)", "Tourist trap (members)", "Watchtower (members)",
        "Dwarf Cannon (members)", "Murder Mystery (members)", "Digsite (members)", "Gertrude's Cat (members)", "Legend's Quest (members)"*/
    ];
        public int selectedShopItemIndex;
        public int selectedShopItemType;
        public string sleepingStatusText;
        public string[] menuText1;
        public bool isSleeping;
        public int modelFireLightningSpellNumber;
        public int modelTorchNumber;
        public int modelClawSpellNumber;
        public int tradeItemsOtherCount;
        public int[] tradeItemsOther;
        public int[] tradeItemOtherCount;
        public bool tradeOtherAccepted;
        public bool tradeWeAccepted;
        public int[] itemAboveHeadScale;
        public int[] itemAboveHeadID;
        public int[] playerStatCurrent;
        public int[] menuActionX;
        public int[] menuActionY;
        public string[] skillNameVerb = [
        "Attack", "Defense", "Strength", "Hits", "Ranged", "Prayer", "Magic", "Cooking", "Woodcutting", "Fletching",
        "Fishing", "Firemaking", "Crafting", "Smithing", "Mining", "Herblaw", "Agility", "Thieving"
    ];
        public int[] menuActionID;
        public int playerAliveTimeout;
        public int cameraAutoRotatePlayerX;
        public int cameraAutoRotatePlayerY;
        public bool showTradeBox;
        public ClientMob[] npcArray;
        public bool duelNoRetreating;
        public bool duelNoMagic;
        public bool duelNoPrayer;
        public bool duelNoWeapons;
        public Menu appearanceMenu;
        public int[][] animationModelArray =
        [ [
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            3, 4
        ], [
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            3, 4
        ], [
            11, 3, 2, 9, 7, 1, 6, 10, 0, 5,
            8, 4
        ], [
            3, 4, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        ], [
            3, 4, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        ], [
            4, 3, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        ], [
            11, 4, 2, 9, 7, 1, 6, 10, 0, 5,
            8, 3
        ], [
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            4, 3
        ]
    ];
        public int playerCount;
        public int lastPlayerCount;
        public int drawUpdatesPerformed;
        public ClientMob[] playerBufferArray;
        public string serverMessage;
        public int groundItemCount;
        public bool duelOpponentAccepted;
        public bool duelMyAccepted;
        public int[] wallObjectX;
        public int[] wallObjectY;
        public bool serverMessageBoxTop;
        public int fatigue;
        public int cameraRotationYAmount;
        public int cameraRotationYIncrement;
        public int[] walkModel = [
        0, 1, 2, 1
    ];
        public int itemsAboveHeadCount;
        public AudioReader audioPlayer;
        public GameObject[] wallObjectArray;
        public string[] messagesArray;
        public long duelOpponentHash;
        public Menu questMenu;
        private int questMenuHandle;
        private int questMenuSelected;
        public bool[] objectAlreadyInMenu;
        public GameObject[] objectArray;
        public int selectedSpell;
        public bool cameraAutoAngleDebug;
        public string lastLoginAddress;
        public ClientMob ourPlayer;
        private int sectionX;
        private int sectionY;
        private int serverIndex;
        public int tradeItemsOurCount;
        public int[] tradeItemsOur;
        public int[] tradeItemOurCount;
        public bool showWelcomeBox;
        public int[] menuActionType;
        public int[] menuActionVar1;
        public int[] menuActionVar2;
        public bool sleepWordDelay;
        public bool configCameraAutoAngle;
        public int minimapRandomRotationX;
        public int minimapRandomRotationY;
        public int loginMenuOkButton;
        public int cameraRotation;
        public int combatStyle;
        public int[] appearanceSkinColours = [
        0xecded0, 0xccb366, 0xb38c40, 0x997326, 0x906020
    ];
        public bool configSoundOff;
        public bool menuShow;
        public int duelOpponentItemCount;
        public int[] duelOpponentItems;
        public int[] duelOpponentItemsCount;
        public Menu loginMenuLogin;
        public int appearanceHeadLeftArrow;
        public int appearanceHeadRightArrow;
        public int appearanceHairLeftArrow;
        public int appearanceHairRightArrow;
        public int appearanceGenderLeftArrow;
        public int appearanceGenderRightArrow;
        public int appearanceTopLeftArrow;
        public int appearanceTopRightArrow;
        public int appearanceSkinLeftArrow;
        public int appearanceSkingRightArrow;
        public int appearanceBottomLeftArrow;
        public int appearanceBottomRightArrow;
        public int appearanceAcceptButton;
        public sbyte[] soundData;
        public bool showBankBox;
        public int shopItemSellPriceModifier;
        public int shopItemBuyPriceModifier;
        public int wildType;
        public int[] playerStatBase;
        public long tradeConfirmOtherNameLong;
        public int showAbuseBox;
        public int[] serverBankItems;
        public int[] serverBankItemCount;
        public bool showShopBox;
        public int[] groundItemX;
        public int[] groundItemY;
        public int[] groundItemID;
        public int[] groundItemObjectVar;
        public GameImageMiddleMan gameGraphics;
        public int maxBankItems;
        public int tradeConfirmOtherItemCount;
        public int[] tradeConfirmOtherItems;
        public int[] tradeConfirmOtherItemsCount;
        public int tick;
        public EngineHandle engineHandle;
        public int areaX;
        public int areaY;
        public int layerIndex;
        public int mouseButtonClick;
        public Menu loginNewUser;
        public int[] walkArrayX;
        public int[] walkArrayY;
        public int[] combatModelArray2 = [
        0, 0, 0, 0, 0, 1, 2, 1
    ];
        public int cameraDistance;
        public int[] receivedMessageX;
        public int[] receivedMessageY;
        public int[] receivedMessageMidPoint;
        public int[] receivedMessageHeight;
        public bool[] wallObjectAlreadyInMenu;
        public int wildX;
        public int wildY;
        public int layerModifier;
        public int lastLayerIndex;
        public int[] bankItems;
        public int[] bankItemCount;
        public string[] skillName = [
        "Attack", "Defense", "Strength", "Hits", "Ranged", "Prayer", "Magic", "Cooking", "Woodcut", "Fletching",
        "Fishing", "Firemaking", "Crafting", "Smithing", "Mining", "Herblaw", "Agility", "Thieving"
    ];
        public int npcCount;
        public int lastNpcCount;
        public int combatTimeout;
        public int maxInventoryItems;
        public static GraphicsDevice graphics;
        public bool errorLoading;
        public int animationNumber;
        public int[] itemAboveHeadX;
        public int[] itemAboveHeadY;
        public int duelRetreat;
        public int duelMagic;
        public int duelPrayer;
        public int duelWeapons;
        public bool showServerMessageBox;
        public int[] playerBufferArrayIndexes;
        public int loginScreen;
        public int tradeConfigItemCount;
        public int[] tradeConfirmItems;
        public int[] tradeConfigItemsCount;
        public int selectedBankItem;
        public int selectedBankItemType;
        public bool showDuelConfirmBox;
        public bool duelConfirmOurAccepted;
        public int[] wallObjectDirection;
        public int[] wallObjectID;
        public GameObject[] gameDataObjects;
        public ClientMob[] lastNpcArray;
        public int modelUpdatingTimer;
        public int inventoryItemsCount;
        public int[] inventoryItems;
        public int[] inventoryItemCount;
        public int[] inventoryItemEquipped;
        public int selectedItem;
        private string selectedItemName;
        public ClientMob[] lastPlayerArray;
        public bool showTradeConfirmBox;
        public bool tradeConfirmAccepted;
        public int[] playerStatExp;
        public int loginButtonNewUser;
        public int loginMenuLoginButton;
        public int mouseTrailIndex;
        private int[] mouseTrailX;
        private int[] mouseTrailY;
        public bool configOneMouseButton;
        public bool[] prayerOn;
        public int lastLoginDays;
        public int loginMenuStatusText;
        public int loginMenuUserText;
        public int loginMenuPasswordText;
        public int loginMenuOkLoginButton;
        public int loginMenuCancelButton;
        public int[] shopItems;
        public int[] shopItemCount;
        public int[] shopItemBasePriceModifier;
        public int objectCount;
        public int duelOpponentStakeCount;
        public int[] duelOpponentStakeItem;
        public int[] duelOutStakeItemCount;
        public int baseInventoryPic;
        public int baseScrollPic;
        public int baseItemPicture;
        public int baseProjectilePic;
        public int baseTexturePic;
        public int subTexturePic;
        public int baseLoginScreenBackgroundPic;
        public int sectionWidth;
        public int sectionHeight;
        public int sectionPosX;
        public int sectionPosY;
        public int[] equipmentStatus;
        public int drawMenuTab;
        public int receivedMessagesCount;
        private string[] receivedMessages;
        public int cameraRotateTime;
        public int questionMenuCount;
        public int cameraRotationXAmount;
        public int cameraRotationXIncrement;
        public int[] teleBubbleTime;
        public string[] gearStats = [
        "Armour", "WeaponAim", "WeaponPower", "Magic", "Prayer"
    ];
        public int logoutTimer;
        public int wallObjectCount;
        public int gridSize;
        public bool loggedIn;
        public int[] questStage;
        public int[] teleBubbleType;
        public int[] experienceList;
        public int lastModelFireLightningSpellNumber;
        public int lastModelTorchNumber;
        public int lastModelClawSpellNumber;
        public int chatTabAllMsgFlash;
        public int chatTabHistoryFlash;
        public int chatTabQuestFlash;
        public int chatTabPrivateFlash;
        public int[] messagesTimeout;
        public int projectileRange;
        public int[] appearanceTopBottomColours = [
        0xff0000, 0xff8000, 0xffe000, 0xa0e000, 57344, 32768, 41088, 45311, 33023, 12528,
        0xe000e0, 0x303030, 0x604000, 0x805000, 0xffffff
    ];
        public int showFriendsBox;
        public int teleBubbleCount;
        public bool memoryError;
        public int[] appearanceHairColours = [
        0xffc030, 0xffa040, 0x805030, 0x604020, 0x303030, 0xff6020, 0xff4000, 0xffffff, 65280, 65535
    ];
        public Menu spellMenu;
        private int spellMenuHandle;
        private int menuMagicPrayersSelected;
        public int duelOurStakeCount;
        public int[] duelOurStakeItem;
        public int[] duelOurStakeItemCount;
        public int menuX;
        public int menuY;
        public int menuWidth;
        public int menuHeight;
        public int menuOptionsCount;
        public Camera gameCamera;
        public Menu friendsMenu;
        private int friendsMenuHandle;
        private int friendsIgnoreMenuSelected;
        private long pmTarget;
        public int healthBarVisibleCount;
        public string[] menuText2;
        public int sleepWordDelayTimer;
        public int mouseButtonHeldTime;
        public int mouseClickedHeldInTradeDuelBox;
        public string loginUsername;
        public string loginPassword;
        public string duelOpponent;
        public int bankPage;
        public Menu loginMenuFirst;
        public int[] healthBarX;
        public int[] healthBarY;
        public int[] healthBarMissing;
        public int[] objectX;
        public int[] objectY;
        public int[] objectType;
        public int[] objectRotation;
        public int reportAbuseOptionSelected;
        public bool showDuelBox;
        public ClientMob[] npcAttackingArray;
        public int serverBankItemsCount;
        public int[] teleBubbleY;
        public int cameraAutoAngle;
        public int cameraAutoRotationAmount;
        public bool loadArea;
        public int[] teleBubbleX;
        public int bankItemsCount;
        public bool showAppearanceWindow;
        public int questPoints;
        public int actionPictureType;
        private int walkMouseX;
        private int walkMouseY;
        public int[] combatModelArray1 = [
        0, 1, 2, 1, 0, 0, 0, 0
    ];
        public bool cameraZoom;

        public bool fogOfWar;
        public bool showCombatWindow;
        public bool showRoofs;
        public bool autoScreenshot;
        public bool useChatFilter;
        public string[] usedQuestName;
        public int subDaysLeft;
        public int[] shopItemSellPrice;
        public int[] shopItemBuyPrice;
        public int[][] captchaPixels;
        public int captchaWidth;
        public int captchaHeight;
        public bool needsClear;
        public bool hasWorldInfo;

        //public void LoadContent()
        //{
        //    throw new NotImplementedException();
        //}
    }

}
