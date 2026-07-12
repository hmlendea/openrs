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
            lastMouseButton = 0;
            mouseButton = 0;
            showShopBox = false;
            showBankBox = false;
            isSleeping = false;
            friendsCount = 0;
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

            maxPacketReadCount = 500;
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

                string[] modelNames = GameData.modelName;

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
            gameGraphics.UnpackImageData(baseProjectilePic, DataOperations.LoadData("projectile.dat", 0, media), abyte1, GameData.spellProjectileCount);
            int l = GameData.highestLoadedPicture;
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

            for (int l1 = 0; l1 < GameData.spellProjectileCount; l1 += 1)
            {
                gameGraphics.LoadImage(baseProjectilePic + l1);
            }

            for (int i2 = 0; i2 < GameData.highestLoadedPicture; i2 += 1)
            {
                gameGraphics.LoadImage(baseProjectilePic + i2);
                //var w = ((GameImage)(gameGraphics)).pictureWidth[baseProjectilePic + i2];
                //var h = ((GameImage)(gameGraphics)).pictureHeight[baseProjectilePic + i2];
                //var texture = GameImage.UnpackedImages[baseProjectilePic + i2];
                //if (texture is not null)
                //    texture.SaveAsJpeg(System.IO.File.OpenWrite("c:/jpg/" + baseProjectilePic + i2 + ".jpg"), w, h);
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
            for (int i1 = 0; i1 < GameData.animationCount; i1 += 1)
            {
                //   label4:
                bool breakThis = false;
                string s1 = GameData.animationName[i1];
                for (int j1 = 0; j1 < i1; j1 += 1)
                {
                    if (GameData.animationName[j1].ToLower() != s1)
                    {
                        continue;
                    }

                    GameData.animationNumber[i1] = GameData.animationNumber[j1];

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
                        if (GameData.animationHasA[i1] == 1)
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
                        if (GameData.animationHasF[i1] == 1)
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
                        if (GameData.animationGenderModels[i1] != 0)
                        {
                            for (int k1 = animationNumber; k1 < animationNumber + 27; k1 += 1)
                            {
                                gameGraphics.LoadImage(k1);
                            }
                        }
                    }
                    catch { }
                }
                GameData.animationNumber[i1] = animationNumber;
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
        public void CreateChatInputMenu()
        {
            chatInputMenu = new Menu(gameGraphics, 10);
            messagesHandleType2 = chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            chatInputBox = chatInputMenu.CreateTextInput(7, 324, 498, 14, 1, 80, false, true);
            messagesHandleType5 = chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            messagesHandleType6 = chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            chatInputMenu.SetFocus(chatInputBox);
        }
        public void LoadConfig()
        {
            sbyte[] abyte0 = UnpackData("config.jag", "Configuration", 10);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            GameData.Load(abyte0);
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
        public void LoadTextures()
        {
            sbyte[] abyte0 = UnpackData("textures.jag", "Textures", 50);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            sbyte[] abyte1 = DataOperations.LoadData("index.dat", 0, abyte0);
            gameCamera.CreateTexture(GameData.textureCount, 7, 11);
            for (int l = 0; l < GameData.textureCount; l += 1)
            {
                string s1 = GameData.textureName[l];
                sbyte[] abyte2 = DataOperations.LoadData(s1 + ".dat", 0, abyte0);
                gameGraphics.UnpackImageData(baseTexturePic, abyte2, abyte1, 1);
                gameGraphics.DrawBox(0, 0, 128, 128, 0xff00ff);
                gameGraphics.DrawPicture(0, 0, baseTexturePic);
                int i1 = ((GameImage)(gameGraphics)).pictureAssumedWidth[baseTexturePic];
                string s2 = GameData.textureSubName[l];
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
        public void LoadModels()
        {
            GameData.GetModelNameIndex("torcha2");
            GameData.GetModelNameIndex("torcha3");
            GameData.GetModelNameIndex("torcha4");
            GameData.GetModelNameIndex("skulltorcha2");
            GameData.GetModelNameIndex("skulltorcha3");
            GameData.GetModelNameIndex("skulltorcha4");
            GameData.GetModelNameIndex("firea2");
            GameData.GetModelNameIndex("firea3");
            GameData.GetModelNameIndex("fireplacea2");
            GameData.GetModelNameIndex("fireplacea3");
            GameData.GetModelNameIndex("firespell2");
            GameData.GetModelNameIndex("firespell3");
            GameData.GetModelNameIndex("lightning2");
            GameData.GetModelNameIndex("lightning3");
            GameData.GetModelNameIndex("clawspell2");
            GameData.GetModelNameIndex("clawspell3");
            GameData.GetModelNameIndex("clawspell4");
            GameData.GetModelNameIndex("clawspell5");
            GameData.GetModelNameIndex("spellcharge2");
            GameData.GetModelNameIndex("spellcharge3");
            sbyte[] abyte0 = UnpackData("models.jag", "3d models", 60);
            if (abyte0 is null)
            {
                errorLoading = true;
                return;
            }
            for (int i1 = 0; i1 < GameData.modelCount; i1 += 1)
            {
                try
                {
                    long j1 = DataOperations.GetObjectOffset(GameData.modelName[i1] + ".ob3", abyte0);
                    if (j1 != 0)
                    {
                        gameDataObjects[i1] = new GameObject(abyte0, (int)j1, true);
                    }
                    else
                    {
                        gameDataObjects[i1] = new GameObject(1, 1);
                    }

                    if (GameData.modelName[i1] == "giantcrystal")
                    {
                        gameDataObjects[i1].isGiantCrystal = true;
                    }
                }
                catch { }
            }
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
                        objWidth = GameData.objectWidth[objType];
                        objHeight = GameData.objectHeight[objType];
                    }
                    else
                    {
                        objHeight = GameData.objectWidth[objType];
                        objWidth = GameData.objectHeight[objType];
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
    }

}
