using System;
using System.IO;
using System.Text;

using NuciLog.Core;

using OpenRS.GameLogic.GameManagers;
using OpenRS.Logging;
using OpenRS.Localisation;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Loading
{
    public sealed class GameLoader(GameClient client)
    {
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameLoader>();

        public void SetLoginVars()
        {
            client.loggedIn = false;
            client.loginScreen = 0;
            client.loginUsername = "";
            client.loginPassword = "";
            /*dja = "Please enter a username:";
            djb = "*" + client.loginUsername + "*";*/
            client.playerCount = 0;
            client.npcCount = 0;
        }

        public void Close()
        {
            client.CallRequestLogout();
            client.CleanUp();
        }

        public void CreateLoginScreenBackgrounds()
        {
            int _bgScreenWidth = client.windowWidth;
client.RaiseOnLoadingSection(this, new EventArgs());

            int l = 0;
            sbyte byte0 = 50;
            sbyte byte1 = 50;

            client.engineHandle.LoadSection(byte0 * 48 + 23, byte1 * 48 + 23, l);
            client.engineHandle.AddObjects(client.gameDataObjects);

            //char c1 = '\u2600';
            //char c2 = '\u1900';
            //char c3 = '\u044C';
            //char c4 = '\u0378';

            int cameraX = 9728;
            int cameraY = 6400;
            int cameraDistance = 1100;
            int cameraRotation = 888;

            client.gameCamera.zoom1 = 4100;
            client.gameCamera.zoom2 = 4100;
            client.gameCamera.zoom3 = 1;
            client.gameCamera.zoom4 = 4000;
            client.gameCamera.SetCameraTransform(cameraX, -client.engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            client.gameCamera.FinishCamera();
            client.gameGraphics.ScreenFadeToBlack();
            client.gameGraphics.ScreenFadeToBlack();

            client.gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0x000000); //_bgScreenWidth=512
            for (int i1 = 6; i1 >= 1; i1 -= 1)
            {
                client.gameGraphics.DrawTransparentLine(0, i1, 0, i1, _bgScreenWidth, 8);
            }

            client.gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0x000000);

            for (int j1 = 6; j1 >= 1; j1 -= 1)
            {
                client.gameGraphics.DrawTransparentLine(0, j1, 0, 194 - j1, _bgScreenWidth, 8);
            }

            if (!client.DoNotDrawLogo)
            {
                if (GameApplet.bgPixels is null)
                {
                    client.gameGraphics.DrawPicture(15, 15, client.baseInventoryPic + 10);
                }
                else
                {
                    client.gameGraphics.DrawPixels(GameApplet.bgPixels, 0, 0, GameApplet.bgPixels.Length, GameApplet.bgPixels[0].Length);
                }
            }

            client.gameGraphics.DrawImage(client.baseLoginScreenBackgroundPic, 0, 0, _bgScreenWidth, 200);
            client.gameGraphics.ApplyImage(client.baseLoginScreenBackgroundPic);

            cameraX = 9216;
            cameraY = 9216;
            cameraDistance = 1100;
            client.cameraRotation = 888;
            client.gameCamera.zoom1 = 4100;
            client.gameCamera.zoom2 = 4100;
            client.gameCamera.zoom3 = 1;
            client.gameCamera.zoom4 = 4000;
            client.gameCamera.SetCameraTransform(cameraX, -client.engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, client.cameraRotation, 0, cameraDistance * 2);
            client.gameCamera.FinishCamera();
            client.gameGraphics.ScreenFadeToBlack();
            client.gameGraphics.ScreenFadeToBlack();

            client.gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int k1 = 6; k1 >= 1; k1 -= 1)
            {
                client.gameGraphics.DrawTransparentLine(0, k1, 0, k1, _bgScreenWidth, 8);
            }

            client.gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int l1 = 6; l1 >= 1; l1 -= 1)
            {
                client.gameGraphics.DrawTransparentLine(0, l1, 0, 194 - l1, _bgScreenWidth, 8);
            }

            if (!client.DoNotDrawLogo)
            {
                if (GameApplet.bgPixels is null)
                {
                    client.gameGraphics.DrawPicture(15, 15, client.baseInventoryPic + 10);
                }
                else
                {
                    client.gameGraphics.DrawPixels(GameApplet.bgPixels, 0, 0, GameApplet.bgPixels.Length, GameApplet.bgPixels[0].Length);
                }
            }

            client.gameGraphics.DrawImage(client.baseLoginScreenBackgroundPic + 1, 0, 0, _bgScreenWidth, 200);
            client.gameGraphics.ApplyImage(client.baseLoginScreenBackgroundPic + 1);

            // Remove buildings
            for (int i2 = 0; i2 < 64; i2 += 1)
            {

                client.gameCamera.RemoveModel(client.engineHandle.roofObject[0][i2]);
                client.gameCamera.RemoveModel(client.engineHandle.wallObject[0][i2]);
                client.gameCamera.RemoveModel(client.engineHandle.wallObject[1][i2]);
                client.gameCamera.RemoveModel(client.engineHandle.roofObject[1][i2]);
                client.gameCamera.RemoveModel(client.engineHandle.wallObject[2][i2]);
                client.gameCamera.RemoveModel(client.engineHandle.roofObject[2][i2]);
            }

            cameraX = 11136;//'\u2B80';
            cameraY = 10368;//'\u2880';
            cameraDistance = 500;//'\u01F4';
            client.cameraRotation = 376;//'\u0178';
            client.gameCamera.zoom1 = 4100;
            client.gameCamera.zoom2 = 4100;
            client.gameCamera.zoom3 = 1;
            client.gameCamera.zoom4 = 4000;
            client.gameCamera.SetCameraTransform(cameraX, -client.engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, client.cameraRotation, 0, cameraDistance * 2);
            client.gameCamera.FinishCamera();
            client.gameGraphics.ScreenFadeToBlack();
            client.gameGraphics.ScreenFadeToBlack();

            client.gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int j2 = 6; j2 >= 1; j2 -= 1)
            {
                client.gameGraphics.DrawTransparentLine(0, j2, 0, j2, _bgScreenWidth, 8);
            }

            client.gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int k2 = 6; k2 >= 1; k2 -= 1)
            {
                client.gameGraphics.DrawTransparentLine(0, k2, 0, 194, _bgScreenWidth, 8);
            }

            if (!client.DoNotDrawLogo)
            {
                if (GameApplet.bgPixels is null)
                {
                    client.gameGraphics.DrawPicture(15, 15, client.baseInventoryPic + 10);
                }
                else
                {
                    client.gameGraphics.DrawPixels(GameApplet.bgPixels, 0, 0, GameApplet.bgPixels.Length, GameApplet.bgPixels[0].Length);
                }
            }

            client.gameGraphics.DrawImage(client.baseInventoryPic + 10, 0, 0, _bgScreenWidth, 200);
            client.gameGraphics.ApplyImage(client.baseInventoryPic + 10);

client.RaiseOnLoadingSectionCompleted(this, new EventArgs());
        }

        public void InitVars()
        {
            client.systemUpdate = 0;
            client.combatStyle = 0;
            client.logoutTimer = 0;
            client.loginScreen = 0;
            client.loggedIn = true;
            client.ResetPrivateMessages();
            client.gameGraphics.ClearScreen();
            // gameGraphics.UpdateGameImage();
            //gameGraphics.DrawImage(spriteBatch, 0, 0);
            client.OnDrawDone();

            for (int l = 0; l < client.objectCount; l += 1)
            {
                client.gameCamera.RemoveModel(client.objectArray[l]);
                client.engineHandle.RemoveObject(client.objectX[l], client.objectY[l], client.objectType[l], client.objectRotation[l]);
            }

            for (int i1 = 0; i1 < client.wallObjectCount; i1 += 1)
            {
                client.gameCamera.RemoveModel(client.wallObjectArray[i1]);
                client.engineHandle.RemoveWallObject(client.wallObjectX[i1], client.wallObjectY[i1], client.wallObjectDirection[i1], client.wallObjectID[i1]);
            }

            client.objectCount = 0;
            client.wallObjectCount = 0;
            client.groundItemCount = 0;
            client.playerCount = 0;
            for (int j1 = 0; j1 < 4000; j1 += 1)
            {
                client.playerBufferArray[j1] = null;
            }

            for (int k1 = 0; k1 < 500; k1 += 1)
            {
                client.playerArray[k1] = null;
            }

            client.npcCount = 0;
            for (int l1 = 0; l1 < 5000; l1 += 1)
            {
                client.npcAttackingArray[l1] = null;
            }

            for (int i2 = 0; i2 < 500; i2 += 1)
            {
                client.npcArray[i2] = null;
            }

            for (int j2 = 0; j2 < 50; j2 += 1)
            {
                client.prayerOn[j2] = false;
            }

            client.mouseButtonClick = 0;
            client.lastMouseButton = 0;
            client.mouseButton = 0;
            client.showShopBox = false;
            client.showBankBox = false;
            client.isSleeping = false;
            client.friendsCount = 0;
        }

        public void LoadSounds()
        {
        }

        public void LoadGame()
        {
            int l = 0;
            for (int i1 = 0; i1 < 99; i1 += 1)
            {
                int j1 = i1 + 1;
                int l1 = (int)(j1 + 300D * Math.Pow(2D, j1 / 7D));
                l += l1;
                client.experienceList[i1] = (l & 0xffffffc) / 4;
            }
            LoadConfig();
            if (client.errorLoading)
            {
                return;
            }

            GameAppletMiddleMan.maxPacketReadCount = 500;
            client.baseInventoryPic = 2000;
            client.baseScrollPic = client.baseInventoryPic + 100;
            client.baseItemPicture = client.baseScrollPic + 50;
            client.baseLoginScreenBackgroundPic = client.baseItemPicture + 1000;
            client.baseProjectilePic = client.baseLoginScreenBackgroundPic + 10;
            client.baseTexturePic = client.baseProjectilePic + 50;
            client.subTexturePic = client.baseTexturePic + 10;
            GameClient.graphics = client.GetGraphics();
            client.SetRefreshRate(50);
            client.gameGraphics = new GameImageMiddleMan(client.windowWidth, client.windowHeight + 12, 4000)
            {
                gameReference = client
            };
            client.gameGraphics.SetDimensions(0, 0, client.windowWidth, client.windowHeight + 12);
            Menu.isBackgroundPatternEnabled = false;
            Menu.baseScrollPic = client.baseScrollPic;
            client.spellMenu = new Menu(client.gameGraphics, 5);
            int k1 = client.gameGraphics.gameWidth - 199;
            sbyte byte0 = 36;
            client.spellMenuHandle = client.spellMenu.CreateList(k1, byte0 + 24, 196, 90, 1, 500, true);
            client.friendsMenu = new Menu(client.gameGraphics, 5);
            client.friendsMenuHandle = client.friendsMenu.CreateList(k1, byte0 + 40, 196, 126, 1, 500, true);
            client.questMenu = new Menu(client.gameGraphics, 5);
            client.questMenuHandle = client.questMenu.CreateList(k1, byte0 + 24, 196, 251, 1, 500, true);
            LoadMedia();
            if (client.errorLoading)
            {
                return;
            }

            LoadAnimations();
            if (client.errorLoading)
            {
                return;
            }

            client.gameCamera = new Camera(client.gameGraphics, 15000, 15000, 1000);

            client.gameCamera.SetCameraSize(client.windowWidth / 2, client.windowHeight / 2, client.windowWidth / 2, client.windowHeight / 2, client.windowWidth, client.cameraFieldOfView);
            client.gameCamera.zoom1 = 2400;
            client.gameCamera.zoom2 = 2400;
            client.gameCamera.zoom3 = 1;
            client.gameCamera.zoom4 = 2300;
            client.gameCamera.OffsetAllModelColours(-50, -10, -50);
            client.engineHandle = new EngineHandle(
                client.gameCamera,
                client.gameGraphics,
                client.entityManager)
            {
                baseInventoryPic = client.baseInventoryPic
            };

            LoadTextures();

            if (client.errorLoading)
            {
                return;
            }

            LoadModels();
            if (client.errorLoading)
            {
                return;
            }

            client.LoadMap();
            if (client.errorLoading)
            {
                return;
            }

            LoadSounds();
            if (!client.errorLoading)
            {
client.RaiseOnContentLoaded(this, new ContentLoadedEventArgs("Starting game...", 100));
                client.DrawLoadingBarText(100, "Starting game...");
                CreateChatInputMenu();
                CreateLoginMenus();
                CreateAppearanceWindow();
                SetLoginVars();
                client.RaiseOnContentLoadedCompleted(this, new EventArgs());

                CreateLoginScreenBackgrounds();
                return;
            }
        }

        public void CreateLoginMenus()
        {
            client.loginMenuFirst = new Menu(client.gameGraphics, 50);
            int l = 40;
            if (!Config.MembersFeatures)
            {
                client.loginMenuFirst.DrawText(256, 200 + l, "Click on an option", 5, true);
                client.loginMenuFirst.DrawButton(156, 240 + l, 120, 35);
                client.loginMenuFirst.DrawButton(356, 240 + l, 120, 35);
                client.loginMenuFirst.DrawText(156, 240 + l, "New User", 5, false);
                client.loginMenuFirst.DrawText(356, 240 + l, "Existing User", 5, false);
                client.loginButtonNewUser = client.loginMenuFirst.CreateButton(156, 240 + l, 120, 35);
                client.loginMenuLoginButton = client.loginMenuFirst.CreateButton(356, 240 + l, 120, 35);
            }
            else
            {
                client.loginMenuFirst.DrawText(256, 200 + l, "Welcome to RuneScape", 4, true);
                client.loginMenuFirst.DrawText(256, 215 + l, "You need a member account to use this server", 4, true);
                client.loginMenuFirst.DrawButton(256, 250 + l, 200, 35);
                client.loginMenuFirst.DrawText(256, 250 + l, "Click here to login", 5, false);
                client.loginMenuLoginButton = client.loginMenuFirst.CreateButton(256, 250 + l, 200, 35);
            }
            client.loginNewUser = new Menu(client.gameGraphics, 50);
            l = 230;
            client.loginNewUser.DrawText(256, l + 8, "To create an account please go back to the", 4, true);
            l += 20;
            client.loginNewUser.DrawText(256, l + 8, "www.runescape.com front page, and choose 'create account'", 4, true);
            l += 30;
            client.loginNewUser.DrawButton(256, l + 17, 150, 34);
            client.loginNewUser.DrawText(256, l + 17, "Ok", 5, false);
            client.loginMenuOkButton = client.loginNewUser.CreateButton(256, l + 17, 150, 34);
            client.loginMenuLogin = new Menu(client.gameGraphics, 50);
            l = 230;
            client.loginMenuStatusText = client.loginMenuLogin.DrawText(256, l - 10, "Please enter your username and password", 4, true);
            l += 28;
            client.loginMenuLogin.DrawButton(140, l, 200, 40);
            client.loginMenuLogin.DrawText(140, l - 10, "Username:", 4, false);
            client.loginMenuUserText = client.loginMenuLogin.CreateInput(140, l + 10, 200, 40, 4, 12, false, false);
            l += 47;
            client.loginMenuLogin.DrawButton(190, l, 200, 40);
            client.loginMenuLogin.DrawText(190, l - 10, "Password:", 4, false);
            client.loginMenuPasswordText = client.loginMenuLogin.CreateInput(190, l + 10, 200, 40, 4, 20, true, false);
            l -= 55;
            client.loginMenuLogin.DrawButton(410, l, 120, 25);
            client.loginMenuLogin.DrawText(410, l, "Ok", 4, false);
            client.loginMenuOkLoginButton = client.loginMenuLogin.CreateButton(410, l, 120, 25);
            l += 30;
            client.loginMenuLogin.DrawButton(410, l, 120, 25);
            client.loginMenuLogin.DrawText(410, l, "Cancel", 4, false);
            client.loginMenuCancelButton = client.loginMenuLogin.CreateButton(410, l, 120, 25);
            client.loginMenuLogin.SetFocus(client.loginMenuUserText);
        }

        public void LostConnection()
        {
            client.systemUpdate = 0;
            if (client.logoutTimer != 0)
            {
                client.ResetIntVars();
                return;
            }
            else
            {
                client.CallBaseLostConnection();
                return;
            }
        }

        public void LoadMedia()
        {
            sbyte[] indexData = LoadDataFile(ApplicationPaths.MediaDirectory, "index.dat");
            client.gameGraphics.UnpackImageData(client.baseInventoryPic, LoadDataFile(ApplicationPaths.MediaDirectory, "inv1.dat"), indexData, 1);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 1, LoadDataFile(ApplicationPaths.MediaDirectory, "inv2.dat"), indexData, 6);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 9, LoadDataFile(ApplicationPaths.MediaDirectory, "bubble.dat"), indexData, 1);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 10, LoadDataFile(ApplicationPaths.MediaDirectory, "runescape.dat"), indexData, 1);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 11, LoadDataFile(ApplicationPaths.MediaDirectory, "splat.dat"), indexData, 3);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 14, LoadDataFile(ApplicationPaths.MediaDirectory, "icon.dat"), indexData, 8);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 22, LoadDataFile(ApplicationPaths.MediaDirectory, "hbar.dat"), indexData, 1);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 23, LoadDataFile(ApplicationPaths.MediaDirectory, "hbar2.dat"), indexData, 1);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 24, LoadDataFile(ApplicationPaths.MediaDirectory, "compass.dat"), indexData, 1);
            client.gameGraphics.UnpackImageData(client.baseInventoryPic + 25, LoadDataFile(ApplicationPaths.MediaDirectory, "buttons.dat"), indexData, 2);
            client.gameGraphics.UnpackImageData(client.baseScrollPic, LoadDataFile(ApplicationPaths.MediaDirectory, "scrollbar.dat"), indexData, 2);
            client.gameGraphics.UnpackImageData(client.baseScrollPic + 2, LoadDataFile(ApplicationPaths.MediaDirectory, "corners.dat"), indexData, 4);
            client.gameGraphics.UnpackImageData(client.baseScrollPic + 6, LoadDataFile(ApplicationPaths.MediaDirectory, "arrows.dat"), indexData, 2);
            client.gameGraphics.UnpackImageData(client.baseProjectilePic, LoadDataFile(ApplicationPaths.MediaDirectory, "projectile.dat"), indexData, client.entityManager.SpellProjectileCount);

            int remainingPictures = client.entityManager.HighestLoadedPicture;

            for (int batchIndex = 1; remainingPictures > 0; batchIndex += 1)
            {
                int batchSize = remainingPictures;
                remainingPictures -= 30;

                if (batchSize > 30)
                {
                    batchSize = 30;
                }

                client.gameGraphics.UnpackImageData(
                    client.baseItemPicture + (batchIndex - 1) * 30,
                    LoadDataFile(ApplicationPaths.MediaDirectory, "objects" + batchIndex + ".dat"),
                    indexData,
                    batchSize);
            }

            client.gameGraphics.LoadImage(client.baseInventoryPic);
            client.gameGraphics.LoadImage(client.baseInventoryPic + 9);

            for (int imageIndex = 11; imageIndex <= 26; imageIndex += 1)
            {
                client.gameGraphics.LoadImage(client.baseInventoryPic + imageIndex);
            }

            for (int spellIndex = 0; spellIndex < client.entityManager.SpellProjectileCount; spellIndex += 1)
            {
                client.gameGraphics.LoadImage(client.baseProjectilePic + spellIndex);
            }

            for (int pictureIndex = 0; pictureIndex < client.entityManager.HighestLoadedPicture; pictureIndex += 1)
            {
                client.gameGraphics.LoadImage(client.baseProjectilePic + pictureIndex);
            }
        }

        public void LoadAnimations()
        {
            StringBuilder sb = new();
            sbyte[] indexData = LoadDataFile(ApplicationPaths.AnimationsDirectory, "index.dat");
            int frameCount = 0;
            client.animationNumber = 0;

            for (int animationIndex = 0; animationIndex < client.entityManager.AnimationCount; animationIndex += 1)
            {
                bool isDuplicate = false;
                string animationName = client.entityManager.GetAnimation(animationIndex).Name;

                for (int previousIndex = 0; previousIndex < animationIndex; previousIndex += 1)
                {
                    if (client.entityManager.GetAnimation(previousIndex).Name.ToLower() != animationName)
                    {
                        continue;
                    }

                    client.entityManager.GetAnimation(animationIndex).SpriteIndex = client.entityManager.GetAnimation(previousIndex).SpriteIndex;
                    isDuplicate = true;
                    break;
                }

                if (isDuplicate)
                {
                    continue;
                }

                string animationFilePath = Path.Combine(ApplicationPaths.AnimationsDirectory, animationName + ".dat");

                if (!File.Exists(animationFilePath))
                {
                    client.entityManager.GetAnimation(animationIndex).SpriteIndex = client.animationNumber;
                    client.animationNumber += 27;
                    continue;
                }

                sbyte[] animationData = LoadDataFile(ApplicationPaths.AnimationsDirectory, animationName + ".dat");

                try
                {
                    client.gameGraphics.UnpackImageData(client.animationNumber, animationData, indexData, 15);
                    frameCount += 15;

                    if (client.entityManager.GetAnimation(animationIndex).HasAttackFrames)
                    {
                        client.gameGraphics.UnpackImageData(
                            client.animationNumber + 15,
                            LoadDataFile(ApplicationPaths.AnimationsDirectory, animationName + "a.dat"),
                            indexData,
                            3);
                        frameCount += 3;
                    }

                    if (client.entityManager.GetAnimation(animationIndex).HasFemaleFrames)
                    {
                        client.gameGraphics.UnpackImageData(
                            client.animationNumber + 18,
                            LoadDataFile(ApplicationPaths.AnimationsDirectory, animationName + "f.dat"),
                            indexData,
                            9);
                        frameCount += 9;
                    }

                    if (client.entityManager.GetAnimation(animationIndex).GenderModel != 0)
                    {
                        for (int imageIndex = client.animationNumber; imageIndex < client.animationNumber + 27; imageIndex += 1)
                        {
                            client.gameGraphics.LoadImage(imageIndex);
                        }
                    }
                }
                catch { }

                client.entityManager.GetAnimation(animationIndex).SpriteIndex = client.animationNumber;
                client.animationNumber += 27;

                sb.AppendLine("Loaded: " + frameCount + " frames of animation");

            }

            logger.Info(
                GameOperation.LoadAnimations,
                "Loaded animation frames.",
                new LogInfo(GameLogInfoKey.AnimationFrameCount, frameCount));
        }

        public void CreateChatInputMenu()
        {
            client.chatInputMenu = new Menu(client.gameGraphics, 10);
            client.messagesHandleType2 = client.chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            client.chatInputBox = client.chatInputMenu.CreateTextInput(7, 324, 498, 14, 1, 80, false, true);
            client.messagesHandleType5 = client.chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            client.messagesHandleType6 = client.chatInputMenu.CreateScrollableTextBox(5, 269, 502, 56, 1, 20, true);
            client.chatInputMenu.SetFocus(client.chatInputBox);
        }

        public void LoadConfig()
        {
            client.entityManager = new EntityManager();
            LocalisationManager.Load(Language.English);
            client.entityManager.LoadPrayers();
            client.entityManager.LoadItems();
            client.entityManager.LoadSpells(GameDefines.SpellProjectileCount);
            client.entityManager.LoadNpcs();
            client.entityManager.LoadAnimations();
            client.entityManager.LoadWallObjects();
            client.entityManager.LoadWorldObjects();
            client.entityManager.LoadTextures();
            client.entityManager.LoadElevations();
            client.entityManager.LoadTiles();

            ChatFilter.Load();
            client.DrawLoadingBarText(15, "Chat system");
            client.RaiseOnContentLoaded(this, new ContentLoadedEventArgs("Chat system", 15));
        }

        public sbyte[] UnpackData(string fileName, string fileTitle, int progressPercentage)
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

                client.DrawLoadingBarText(progressPercentage, "Unpacking " + fileTitle);
                if (i1 != l)
                {
                    sbyte[] abyte2 = new sbyte[l];
                    DataFileDecrypter.UnpackData(abyte2, l, abyte1, i1, 0);
client.RaiseOnContentLoaded(this, new ContentLoadedEventArgs("Unpacking " + fileTitle, progressPercentage));
                    return abyte2;
                }
                else
                {
client.RaiseOnContentLoaded(this, new ContentLoadedEventArgs("Unpacking " + fileTitle, progressPercentage));
                    return abyte1;
                }
            }
            else
            {
client.RaiseOnContentLoaded(this, new ContentLoadedEventArgs("Unpacking " + fileTitle, progressPercentage));
                return client.CallBaseUnpackData(fileName, fileTitle, progressPercentage);
            }
        }

        public void CreateAppearanceWindow()
        {
            client.appearanceMenu = new Menu(client.gameGraphics, 100);
            client.appearanceMenu.DrawText(256, 10, "Please design Your Character", 4, true);
            int l = 140;
            int i1 = 34;
            l += 116;
            i1 -= 10;
            client.appearanceMenu.DrawText(l - 55, i1 + 110, "Front", 3, true);
            client.appearanceMenu.DrawText(l, i1 + 110, "Side", 3, true);
            client.appearanceMenu.DrawText(l + 55, i1 + 110, "Back", 3, true);
            sbyte byte0 = 54;
            i1 += 145;
            client.appearanceMenu.DrawCurvedBox(l - byte0, i1, 53, 41);
            client.appearanceMenu.DrawText(l - byte0, i1 - 8, "Head", 1, true);
            client.appearanceMenu.DrawText(l - byte0, i1 + 8, "Type", 1, true);
            client.appearanceMenu.DrawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            client.appearanceHeadLeftArrow = client.appearanceMenu.CreateButton(l - byte0 - 40, i1, 20, 20);
            client.appearanceMenu.DrawArrow(l - byte0 + 40, i1, Menu.baseScrollPic + 6);
            client.appearanceHeadRightArrow = client.appearanceMenu.CreateButton(l - byte0 + 40, i1, 20, 20);
            client.appearanceMenu.DrawCurvedBox(l + byte0, i1, 53, 41);
            client.appearanceMenu.DrawText(l + byte0, i1 - 8, "Hair", 1, true);
            client.appearanceMenu.DrawText(l + byte0, i1 + 8, "Color", 1, true);
            client.appearanceMenu.DrawArrow(l + byte0 - 40, i1, Menu.baseScrollPic + 7);
            client.appearanceHairLeftArrow = client.appearanceMenu.CreateButton(l + byte0 - 40, i1, 20, 20);
            client.appearanceMenu.DrawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            client.appearanceHairRightArrow = client.appearanceMenu.CreateButton(l + byte0 + 40, i1, 20, 20);
            i1 += 50;
            client.appearanceMenu.DrawCurvedBox(l - byte0, i1, 53, 41);
            client.appearanceMenu.DrawText(l - byte0, i1, "Gender", 1, true);
            client.appearanceMenu.DrawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            client.appearanceGenderLeftArrow = client.appearanceMenu.CreateButton(l - byte0 - 40, i1, 20, 20);
            client.appearanceMenu.DrawArrow(l - byte0 + 40, i1, Menu.baseScrollPic + 6);
            client.appearanceGenderRightArrow = client.appearanceMenu.CreateButton(l - byte0 + 40, i1, 20, 20);
            client.appearanceMenu.DrawCurvedBox(l + byte0, i1, 53, 41);
            client.appearanceMenu.DrawText(l + byte0, i1 - 8, "Top", 1, true);
            client.appearanceMenu.DrawText(l + byte0, i1 + 8, "Color", 1, true);
            client.appearanceMenu.DrawArrow(l + byte0 - 40, i1, Menu.baseScrollPic + 7);
            client.appearanceTopLeftArrow = client.appearanceMenu.CreateButton(l + byte0 - 40, i1, 20, 20);
            client.appearanceMenu.DrawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            client.appearanceTopRightArrow = client.appearanceMenu.CreateButton(l + byte0 + 40, i1, 20, 20);
            i1 += 50;
            client.appearanceMenu.DrawCurvedBox(l - byte0, i1, 53, 41);
            client.appearanceMenu.DrawText(l - byte0, i1 - 8, "Skin", 1, true);
            client.appearanceMenu.DrawText(l - byte0, i1 + 8, "Color", 1, true);
            client.appearanceMenu.DrawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            client.appearanceSkinLeftArrow = client.appearanceMenu.CreateButton(l - byte0 - 40, i1, 20, 20);
            client.appearanceMenu.DrawArrow(l - byte0 + 40, i1, Menu.baseScrollPic + 6);
            client.appearanceSkingRightArrow = client.appearanceMenu.CreateButton(l - byte0 + 40, i1, 20, 20);
            client.appearanceMenu.DrawCurvedBox(l + byte0, i1, 53, 41);
            client.appearanceMenu.DrawText(l + byte0, i1 - 8, "Bottom", 1, true);
            client.appearanceMenu.DrawText(l + byte0, i1 + 8, "Color", 1, true);
            client.appearanceMenu.DrawArrow(l + byte0 - 40, i1, Menu.baseScrollPic + 7);
            client.appearanceBottomLeftArrow = client.appearanceMenu.CreateButton(l + byte0 - 40, i1, 20, 20);
            client.appearanceMenu.DrawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            client.appearanceBottomRightArrow = client.appearanceMenu.CreateButton(l + byte0 + 40, i1, 20, 20);
            i1 += 82;
            i1 -= 35;
            client.appearanceMenu.DrawButton(l, i1, 200, 30);
            client.appearanceMenu.DrawText(l, i1, "Accept", 4, false);
            client.appearanceAcceptButton = client.appearanceMenu.CreateButton(l, i1, 200, 30);
        }

        public void LoadTextures()
        {
            sbyte[] textureIndexData = LoadDataFile(ApplicationPaths.TexturesDirectory, "index.dat");
            client.gameCamera.CreateTexture(client.entityManager.TextureCount, 7, 11);

            for (int textureIndex = 0; textureIndex < client.entityManager.TextureCount; textureIndex += 1)
            {
                string textureName = client.entityManager.GetTexture(textureIndex).Name;
                sbyte[] textureData = LoadDataFile(ApplicationPaths.TexturesDirectory, textureName + ".dat");
                client.gameGraphics.UnpackImageData(client.baseTexturePic, textureData, textureIndexData, 1);
                client.gameGraphics.DrawBox(0, 0, 128, 128, 0xff00ff);
                client.gameGraphics.DrawPicture(0, 0, client.baseTexturePic);
                int textureWidth = client.gameGraphics.pictureAssumedWidth[client.baseTexturePic];
                string subTextureName = client.entityManager.GetTexture(textureIndex).SubName;

                if (subTextureName is not null && subTextureName.Length > 0)
                {
                    sbyte[] subTextureData = LoadDataFile(ApplicationPaths.TexturesDirectory, subTextureName + ".dat");
                    client.gameGraphics.UnpackImageData(client.baseTexturePic, subTextureData, textureIndexData, 1);
                    client.gameGraphics.DrawPicture(0, 0, client.baseTexturePic);
                }

                client.gameGraphics.DrawImage(client.subTexturePic + textureIndex, 0, 0, textureWidth, textureWidth);
                int pixelCount = textureWidth * textureWidth;

                for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
                {
                    if (client.gameGraphics.pictureColors[client.subTexturePic + textureIndex][pixelIndex] == 65280)
                    {
                        client.gameGraphics.pictureColors[client.subTexturePic + textureIndex][pixelIndex] = 0xff00ff;
                    }
                }

                client.gameGraphics.ApplyImage(client.subTexturePic + textureIndex);
                client.gameCamera.SetTexture(
                    textureIndex,
                    client.gameGraphics.pictureColorIndexes[client.subTexturePic + textureIndex],
                    client.gameGraphics.pictureColor[client.subTexturePic + textureIndex],
                    textureWidth / 64 - 1);
            }
        }

        private static sbyte[] LoadDataFile(string directory, string fileName)
        {
            string filePath = Path.Combine(directory, fileName.ToLower());

            return (sbyte[])(Array)File.ReadAllBytes(filePath);
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

            for (int modelIndex = 0; modelIndex < GameData.modelCount; modelIndex += 1)
            {
                try
                {
                    string modelFilePath = Path.Combine(
                        ApplicationPaths.ModelsDirectory,
                        GameData.modelName[modelIndex].ToLower() + ".ob3");

                    if (File.Exists(modelFilePath))
                    {
                        sbyte[] modelData = (sbyte[])(Array)File.ReadAllBytes(modelFilePath);
                        client.gameDataObjects[modelIndex] = new GameObject(modelData, 0, true);
                    }
                    else
                    {
                        client.gameDataObjects[modelIndex] = new GameObject(1, 1);
                    }

                    if (GameData.modelName[modelIndex] == "giantcrystal")
                    {
                        client.gameDataObjects[modelIndex].isGiantCrystal = true;
                    }
                }
                catch { }
            }
        }

        public bool LoadSection(int x, int y)
        {
            if (client.playerAliveTimeout != 0)
            {
                client.engineHandle.playerIsAlive = false;
                return false;
            }
            client.loadArea = false;
            x += client.wildX;
            y += client.wildY;
            if (client.lastLayerIndex == client.layerIndex && x > client.sectionWidth && x < client.sectionPosX && y > client.sectionHeight && y < client.sectionPosY)
            {
                client.engineHandle.playerIsAlive = true;
                return false;
            }
client.RaiseOnLoadingSection(this, new EventArgs());

            client.gameGraphics.DrawText("Loading... Please wait", 256, 192, 1, 0xffffff);
            client.DrawChatMessageTabs();

            //gameGraphics.DrawImage(spriteBatch, 0, 0);
            int l = client.areaX;
            int i1 = client.areaY;
            int xBase = (x + 24) / 48;
            int yBase = (y + 24) / 48;
            client.lastLayerIndex = client.layerIndex;
            client.areaX = xBase * 48 - 48;
            client.areaY = yBase * 48 - 48;
            client.sectionWidth = xBase * 48 - 32;
            client.sectionHeight = yBase * 48 - 32;
            client.sectionPosX = xBase * 48 + 32;
            client.sectionPosY = yBase * 48 + 32;
            client.engineHandle.LoadSection(x, y, client.lastLayerIndex);

            client.areaX -= client.wildX;
            client.areaY -= client.wildY;
            int offsetX = client.areaX - l;
            int offsetY = client.areaY - i1;
            for (int j2 = 0; j2 < client.objectCount; j2 += 1)
            {
                client.objectX[j2] -= offsetX;
                client.objectY[j2] -= offsetY;
                int objX = client.objectX[j2];
                int objY = client.objectY[j2];
                int objType = client.objectType[j2];
                GameObject _obj = client.objectArray[j2];
                try
                {
                    int objDir = client.objectRotation[j2];
                    int objWidth;
                    int objHeight;
                    if (objDir == 0 || objDir == 4)
                    {
                        objWidth = client.entityManager.GetWorldObject(objType).Width;
                        objHeight = client.entityManager.GetWorldObject(objType).Height;
                    }
                    else
                    {
                        objHeight = client.entityManager.GetWorldObject(objType).Width;
                        objWidth = client.entityManager.GetWorldObject(objType).Height;
                    }
                    int flatObjX = (objX + objX + objWidth) * client.gridSize / 2;
                    int flatObjY = (objY + objY + objHeight) * client.gridSize / 2;
                    if (objX >= 0 && objY >= 0 && objX < 96 && objY < 96)
                    {
                        client.gameCamera.AddModel(_obj);
                        _obj.SetPosition(flatObjX, -client.engineHandle.GetAveragedElevation(flatObjX, flatObjY), flatObjY);
                        client.engineHandle.CreateObject(objX, objY, objType, objDir);
                        if (objType == 74)
                        {
                            _obj.OffsetPosition(0, -480, 0);
                        }
                    }
                }
                catch (Exception runtimeexception)
                {
                    logger.Error(
                        GameOperation.LoadSection,
                        "Location error for object.",
                        runtimeexception,
                        new LogInfo(GameLogInfoKey.CoordinateX, j2));
                    //runtimeexception.printStackTrace();
                }
            }

            for (int wallIndex = 0; wallIndex < client.wallObjectCount; wallIndex += 1)
            {
                client.wallObjectX[wallIndex] -= offsetX;
                client.wallObjectY[wallIndex] -= offsetY;
                int wallX = client.wallObjectX[wallIndex];
                int wallY = client.wallObjectY[wallIndex];
                int wallId = client.wallObjectID[wallIndex];
                int wallDir = client.wallObjectDirection[wallIndex];
                try
                {
                    client.engineHandle.CreateWall(wallX, wallY, wallDir, wallId);
                    GameObject wallObject = client.CreateWallObject(wallX, wallY, wallDir, wallId, wallIndex);
                    client.wallObjectArray[wallIndex] = wallObject;
                }
                catch (Exception runtimeexception1)
                {
                    logger.Error(
                        GameOperation.LoadSection,
                        "Boundary error while creating wall object.",
                        runtimeexception1);
                    //runtimeexception1.printStackTrace();
                }
            }

            for (int k3 = 0; k3 < client.groundItemCount; k3 += 1)
            {
                client.groundItemX[k3] -= offsetX;
                client.groundItemY[k3] -= offsetY;
            }

            for (int j4 = 0; j4 < client.playerCount; j4 += 1)
            {
                ClientMob f1 = client.playerArray[j4];
                f1.currentX -= offsetX * client.gridSize;
                f1.currentY -= offsetY * client.gridSize;
                for (int l5 = 0; l5 <= f1.waypointCurrent; l5 += 1)
                {
                    f1.waypointsX[l5] -= offsetX * client.gridSize;
                    f1.waypointsY[l5] -= offsetY * client.gridSize;
                }

            }

            for (int i5 = 0; i5 < client.npcCount; i5 += 1)
            {
                ClientMob f2 = client.npcArray[i5];
                f2.currentX -= offsetX * client.gridSize;
                f2.currentY -= offsetY * client.gridSize;
                for (int k6 = 0; k6 <= f2.waypointCurrent; k6 += 1)
                {
                    f2.waypointsX[k6] -= offsetX * client.gridSize;
                    f2.waypointsY[k6] -= offsetY * client.gridSize;
                }

            }

            client.engineHandle.playerIsAlive = true;
client.RaiseOnLoadingSectionCompleted(this, new EventArgs());

            client.OnDrawDone();

            return true;
        }

    }

}
