using System;
using System.Collections.Generic;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Models;
using OpenRS.Net.Client.Game;
using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class GameRenderer
    {
        private readonly GameClient client;
        private readonly CharacterRenderer characterRenderer;
        private readonly CombatRenderer combatRenderer;
        private readonly InventoryRenderer inventoryRenderer;
        private readonly LoginRenderer loginRenderer;
        private readonly OverlayRenderer overlayRenderer;
        private readonly SocialRenderer socialRenderer;
        private readonly TradeRenderer tradeRenderer;
        private readonly WorldRenderer worldRenderer;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameRenderer>();

        private List<ItemSpriteDrawCall> pendingItemSpriteDrawCalls = [];

        public GameRenderer(GameClient client)
        {
            this.client = client;
            characterRenderer = new(client, RecordItemSprite);
            combatRenderer = new(client);
            inventoryRenderer = new(client, RecordItemSprite);
            loginRenderer = new(client);
            overlayRenderer = new(client);
            socialRenderer = new(client);
            tradeRenderer = new(client, RecordItemSprite);
            worldRenderer = new(client, RecordItemSprite);
        }

        private void RecordItemSprite(int x, int y, int width, int height, Item item)
        {
            if (item is null || string.IsNullOrEmpty(item.SpriteName))
            {
                return;
            }

            pendingItemSpriteDrawCalls.Add(new ItemSpriteDrawCall
            {
                PixelX = x,
                PixelY = y,
                PixelWidth = width,
                PixelHeight = height,
                SpriteName = item.SpriteName
            });
        }

        public CombatStyle CombatStyle
        {
            get => (CombatStyle)client.combatStyle;
            set => client.combatStyle = (int)value;
        }

        public void DrawNpc(int x, int y, int width, int height, int npcIndex, int cameraXOffset, int scalePercentage)
            => worldRenderer.DrawNPC(x, y, width, height, npcIndex, cameraXOffset, scalePercentage);

        public void DrawReportAbuseBox1() => overlayRenderer.DrawReportAbuseBox1();

        public void DrawModel(int objectIndex, string modelName) => worldRenderer.DrawModel(objectIndex, modelName);

        public void DrawPlayer(int x, int y, int width, int height, int playerIndex, int cameraXOffset, int scalePercentage)
            => worldRenderer.DrawPlayer(x, y, width, height, playerIndex, cameraXOffset, scalePercentage);

        public void DrawDuelConfirmBox() => tradeRenderer.DrawDuelConfirmBox();

        public void DrawInventoryMenu(bool canRightClick) => inventoryRenderer.DrawInventoryMenu(canRightClick);

        public void DrawMinimapMenu(bool canClick) => worldRenderer.DrawMinimapMenu(canClick);

        public void DrawWelcomeBox() => loginRenderer.DrawWelcomeBox();

        public void DrawOptionsMenu(bool canClick) => overlayRenderer.DrawOptionsMenu(canClick);

        public void DrawTradeBox() => tradeRenderer.DrawTradeBox();

        public void DrawLogoutBox() => overlayRenderer.DrawLogoutBox();

        public void DrawTeleBubble(int x, int y, int entityWidth, int entityHeight, int bubbleIndex, int cameraXOffset, int scalePercentage)
            => worldRenderer.DrawTeleBubble(x, y, entityWidth, entityHeight, bubbleIndex, cameraXOffset, scalePercentage);

        public void DrawQuestionMenu() => overlayRenderer.DrawQuestionMenu();

        public void DrawTradeConfirmBox() => tradeRenderer.DrawTradeConfirmBox();

        public void DrawLoginScreens() => loginRenderer.DrawLoginScreens();

        public void LoginScreenPrint(string statusLine1, string statusLine2) => loginRenderer.LoginScreenPrint(statusLine1, statusLine2);

        public void DrawItem(int x, int y, int width, int height, int itemID, int xOffset, int yOffset)
            => inventoryRenderer.DrawItem(x, y, width, height, itemID, xOffset, yOffset);

        public void DrawFriendsMenu(bool canClick) => socialRenderer.DrawFriendsMenu(canClick);

        public void DrawPrayerMagicMenu(bool canClick) => characterRenderer.DrawPrayerMagicMenu(canClick);

        public void DrawChatMessageTabs() => socialRenderer.DrawChatMessageTabs();

        public void DrawShopBox() => inventoryRenderer.DrawShopBox();

        public void DrawAppearanceWindow() => characterRenderer.DrawAppearanceWindow();

        public void DrawReportAbuseBox2() => overlayRenderer.DrawReportAbuseBox2();

        public void DrawDuelBox() => tradeRenderer.DrawDuelBox();

        public void DrawWildernessAlertBox() => combatRenderer.DrawWildernessAlertBox();

        public void DrawNPC(int x, int y, int width, int height, int index, int unknown1, int unknown2)
            => worldRenderer.DrawNPC(x, y, width, height, index, unknown1, unknown2);

        public void DrawAboveHeadThings() => worldRenderer.DrawAboveHeadThings();

        public void DrawBankBox() => inventoryRenderer.DrawBankBox();

        public void DrawMinimapObject(int x, int y, int colour) => worldRenderer.DrawMinimapObject(x, y, colour);

        public void DrawServerMessageBox() => socialRenderer.DrawServerMessageBox();

        public void DrawStatsQuestsMenu(bool canClick) => characterRenderer.DrawStatsQuestsMenu(canClick);

        public void DrawFriendsBox() => socialRenderer.DrawFriendsBox();

        public void DrawRightClickMenu() => overlayRenderer.DrawRightClickMenu();

        public void DrawWindow()
        {
            client.Paint(GameClient.graphics);

            if (client.errorLoading)
            {
                client.RaiseOnErrorLoading(this, EventArgs.Empty);
                client.SetRefreshRate(1);

                return;
            }

            if (client.memoryError)
            {
                client.RaiseOnMemoryError(this, EventArgs.Empty);
                return;
            }

            try
            {
                if (!client.loggedIn)
                {
                    client.GameGraphics.IsLoggedIn = false;
                    loginRenderer.DrawLoginScreens();
                }

                if (client.loggedIn)
                {
                    client.GameGraphics.IsLoggedIn = true;
                    DrawGame();

                    return;
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderGame, "The Paint call has failed.", exception);
                client.CleanUp();
                client.memoryError = true;
            }
        }

        public void DrawMenus()
        {
            if (client.logoutTimer != 0)
            {
                overlayRenderer.DrawLogoutBox();
            }
            else if (client.showWelcomeBox)
            {
                loginRenderer.DrawWelcomeBox();
            }
            else if (client.showServerMessageBox)
            {
                socialRenderer.DrawServerMessageBox();
            }
            else if (client.wildType == 1)
            {
                combatRenderer.DrawWildernessAlertBox();
            }
            else if (client.showBankBox && client.combatTimeout == 0)
            {
                inventoryRenderer.DrawBankBox();
            }
            else if (client.showShopBox && client.combatTimeout == 0)
            {
                inventoryRenderer.DrawShopBox();
            }
            else if (client.showTradeConfirmBox)
            {
                tradeRenderer.DrawTradeConfirmBox();
            }
            else if (client.showTradeBox)
            {
                tradeRenderer.DrawTradeBox();
            }
            else if (client.showDuelConfirmBox)
            {
                tradeRenderer.DrawDuelConfirmBox();
            }
            else if (client.showDuelBox)
            {
                tradeRenderer.DrawDuelBox();
            }
            else if (client.showAbuseBox == 1)
            {
                overlayRenderer.DrawReportAbuseBox1();
            }
            else if (client.showAbuseBox == 2)
            {
                overlayRenderer.DrawReportAbuseBox2();
            }
            else if (client.showFriendsBox != 0)
            {
                socialRenderer.DrawFriendsBox();
            }
            else
            {
                if (client.showQuestionMenu)
                {
                    overlayRenderer.DrawQuestionMenu();
                }

                client.GetMenuHighlighted();
                bool isMenuClosed = !client.showQuestionMenu && !client.menuShow;

                if (isMenuClosed)
                {
                    client.menuOptionsCount = 0;
                }

                if (client.drawMenuTab == 0 && isMenuClosed)
                {
                    client.GenerateWorldRightClickMenu();
                }

                if (client.drawMenuTab == 1)
                {
                    inventoryRenderer.DrawInventoryMenu(isMenuClosed);
                }

                if (client.drawMenuTab == 2)
                {
                    worldRenderer.DrawMinimapMenu(isMenuClosed);
                }

                if (client.drawMenuTab == 3)
                {
                    characterRenderer.DrawStatsQuestsMenu(isMenuClosed);
                }

                if (client.drawMenuTab == 4)
                {
                    characterRenderer.DrawPrayerMagicMenu(isMenuClosed);
                }

                if (client.drawMenuTab == 5)
                {
                    socialRenderer.DrawFriendsMenu(isMenuClosed);
                }

                if (client.drawMenuTab == 6)
                {
                    overlayRenderer.DrawOptionsMenu(isMenuClosed);
                }

                if (!client.menuShow && !client.showQuestionMenu)
                {
                    client.CheckMouseStatus();
                }

                if (client.menuShow && !client.showQuestionMenu)
                {
                    overlayRenderer.DrawRightClickMenu();
                }
            }
            client.mouseButtonClick = 0;
        }

        public void DrawGame()
        {
            pendingItemSpriteDrawCalls = [];
            client.PendingItemSpriteDrawCalls = pendingItemSpriteDrawCalls;

            if (client.ourPlayer is null)
            {
                return;
            }

            if (client.playerAliveTimeout != 0)
            {
                client.GameGraphics.ScreenFadeToBlack();
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.player_dead"), client.windowWidth / 2, client.windowHeight / 2, 7, 0xff0000);
                socialRenderer.DrawChatMessageTabs();
                client.OnDrawDone();

                return;
            }

            if (client.showAppearanceWindow)
            {
                characterRenderer.DrawAppearanceWindow();
                return;
            }
            if (client.isSleeping)
            {
                client.GameGraphics.ScreenFadeToBlack();
                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    client.GameGraphics.DrawText("ZZZ", (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    client.GameGraphics.DrawText("ZZZ", 512 - (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                client.GameGraphics.DrawBox(client.windowWidth / 2 - 100, 160, 200, 40, 0);
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.sleeping"), client.windowWidth / 2, 50, 7, 0xffff00);
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.sleeping_fatigue_prefix") + client.fatigue * 100 / 750 + "%", client.windowWidth / 2, 90, 7, 0xffff00);
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.sleeping_instruction1"), client.windowWidth / 2, 140, 5, 0xffffff);
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.sleeping_instruction2"), client.windowWidth / 2, 160, 5, 0xffffff);
                client.GameGraphics.DrawText(client.inputText + "*", client.windowWidth / 2, 180, 5, 65535);
                if (client.sleepingStatusText is null)
                {
                    client.GameGraphics.DrawPixels(client.captchaPixels, client.windowWidth / 2 - 127, 230, client.captchaWidth, client.captchaHeight);
                }
                else
                {
                    client.GameGraphics.DrawText(client.sleepingStatusText, client.windowWidth / 2, 260, 5, 0xff0000);
                }

                client.GameGraphics.DrawBoxEdge(client.windowWidth / 2 - 128, 229, 257, 42, 0xffffff);
                socialRenderer.DrawChatMessageTabs();
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.sleeping_captcha_alternative"), client.windowWidth / 2, 290, 1, 0xffffff);
                client.GameGraphics.DrawText(LocalisationManager.GetString("game.sleeping_captcha_action"), client.windowWidth / 2, 305, 1, 0xffffff);
                client.OnDrawDone();

                return;
            }

            if (!client.engineHandle.PlayerIsAlive)
            {
                return;
            }

            for (int modelIndex = 0; modelIndex < 64; modelIndex += 1)
            {
                client.gameCamera.RemoveModel(client.engineHandle.RoofObject[client.lastLayerIndex][modelIndex]);

                if (client.lastLayerIndex == 0)
                {
                    client.gameCamera.RemoveModel(client.engineHandle.WallObject[1][modelIndex]);
                    client.gameCamera.RemoveModel(client.engineHandle.RoofObject[1][modelIndex]);
                    client.gameCamera.RemoveModel(client.engineHandle.WallObject[2][modelIndex]);
                    client.gameCamera.RemoveModel(client.engineHandle.RoofObject[2][modelIndex]);
                }

                client.cameraZoom = true;

                if (client.lastLayerIndex == 0 &&
                    (client.engineHandle.Tiles[client.ourPlayer.LocationX / 128][client.ourPlayer.LocationY / 128] & 0x80) == 0)
                {
                    if (client.showRoofs)
                    {
                        client.gameCamera.AddModel(client.engineHandle.RoofObject[client.lastLayerIndex][modelIndex]);

                        if (client.lastLayerIndex == 0)
                        {
                            // draw wall object at lv 1 / second layer
                            client.gameCamera.AddModel(client.engineHandle.WallObject[1][modelIndex]);
                            // draw roof object at lv 1 / second layer
                            client.gameCamera.AddModel(client.engineHandle.RoofObject[1][modelIndex]);

                            // draw wall object at lv 2 / third layer
                            client.gameCamera.AddModel(client.engineHandle.WallObject[2][modelIndex]);

                            // draw roof object at lv 2 / third layer
                            client.gameCamera.AddModel(client.engineHandle.RoofObject[2][modelIndex]);
                        }
                    }

                    client.cameraZoom = false;
                }
            }

            if (client.modelFireLightningSpellNumber != client.lastModelFireLightningSpellNumber)
            {
                client.lastModelFireLightningSpellNumber = client.modelFireLightningSpellNumber;
                for (int objectSpriteIndex = 0; objectSpriteIndex < client.objectCount; objectSpriteIndex += 1)
                {
                    if (client.objectType[objectSpriteIndex] == 97)
                    {
                        worldRenderer.DrawModel(objectSpriteIndex, "firea" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[objectSpriteIndex] == 274)
                    {
                        worldRenderer.DrawModel(objectSpriteIndex, "fireplacea" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[objectSpriteIndex] == 1031)
                    {
                        worldRenderer.DrawModel(objectSpriteIndex, "lightning" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[objectSpriteIndex] == 1036)
                    {
                        worldRenderer.DrawModel(objectSpriteIndex, "firespell" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[objectSpriteIndex] == 1147)
                    {
                        worldRenderer.DrawModel(objectSpriteIndex, "spellcharge" + (client.modelFireLightningSpellNumber + 1));
                    }
                }

            }
            if (client.modelTorchNumber != client.lastModelTorchNumber)
            {
                client.lastModelTorchNumber = client.modelTorchNumber;
                for (int j1 = 0; j1 < client.objectCount; j1 += 1)
                {
                    if (client.objectType[j1] == 51)
                    {
                        worldRenderer.DrawModel(j1, "torcha" + (client.modelTorchNumber + 1));
                    }

                    if (client.objectType[j1] == 143)
                    {
                        worldRenderer.DrawModel(j1, "skulltorcha" + (client.modelTorchNumber + 1));
                    }
                }

            }
            if (client.modelClawSpellNumber != client.lastModelClawSpellNumber)
            {
                client.lastModelClawSpellNumber = client.modelClawSpellNumber;
                for (int clawSpellObjectIndex = 0; clawSpellObjectIndex < client.objectCount; clawSpellObjectIndex += 1)
                {
                    if (client.objectType[clawSpellObjectIndex] == 1142)
                    {
                        worldRenderer.DrawModel(clawSpellObjectIndex, "clawspell" + (client.modelClawSpellNumber + 1));
                    }
                }
            }
            client.gameCamera.RemoveLastUpdates(client.drawUpdatesPerformed);
            client.drawUpdatesPerformed = 0;
            for (int playerSpriteIndex = 0; playerSpriteIndex < client.playerCount; playerSpriteIndex += 1)
            {
                ClientMob player = client.playerArray[playerSpriteIndex];

                if (player.Appearance.TrousersColour != 255)
                {
                    int playerX = player.LocationX;
                    int playerY = player.LocationY;
                    int playerElevation = -client.engineHandle.GetAveragedElevation(playerX, playerY);
                    int spriteHandle = client.gameCamera.AddSpriteToScene(5000 + playerSpriteIndex, playerX, playerElevation, playerY, 145, 220, playerSpriteIndex + 10000);
                    client.drawUpdatesPerformed += 1;

                    if (player == client.ourPlayer)
                    {
                        client.gameCamera.RemoveSprite(spriteHandle);
                    }

                    if (player.CurrentSprite == 8)
                    {
                        client.gameCamera.UpdateSpritePosition(spriteHandle, -30);
                    }

                    if (player.CurrentSprite == 9)
                    {
                        client.gameCamera.UpdateSpritePosition(spriteHandle, 30);
                    }
                }
            }

            for (int playerProjectileIndex = 0; playerProjectileIndex < client.playerCount; playerProjectileIndex += 1)
            {
                ClientMob player = client.playerArray[playerProjectileIndex];

                if (player.ProjectileDistance > 0)
                {
                    ClientMob targetMob = null;

                    if (player.AttackingNpcIndex != -1)
                    {
                        targetMob = client.npcAttackingArray[player.AttackingNpcIndex];
                    }
                    else if (player.AttackingPlayerIndex != -1)
                    {
                        targetMob = client.playerBufferArray[player.AttackingPlayerIndex];
                    }

                    if (targetMob is not null)
                    {
                        int attackerX = player.LocationX;
                        int attackerY = player.LocationY;
                        int attackerElevation = -client.engineHandle.GetAveragedElevation(attackerX, attackerY) - 110;
                        int targetX = targetMob.LocationX;
                        int targetY = targetMob.LocationY;
                        int targetElevation = -client.engineHandle.GetAveragedElevation(targetX, targetY) - client.entityManager.GetNpc(targetMob.NpcIdentifier).SpriteHeight / 2;
                        int projectileX = (attackerX * player.ProjectileDistance + targetX * (client.projectileRange - player.ProjectileDistance)) / client.projectileRange;
                        int projectileElevation = (attackerElevation * player.ProjectileDistance + targetElevation * (client.projectileRange - player.ProjectileDistance)) / client.projectileRange;
                        int projectileY = (attackerY * player.ProjectileDistance + targetY * (client.projectileRange - player.ProjectileDistance)) / client.projectileRange;
                        client.gameCamera.AddSpriteToScene(client.baseProjectilePic + player.ProjectileType, projectileX, projectileElevation, projectileY, 32, 32, 0);
                        client.drawUpdatesPerformed += 1;
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < client.npcCount; npcIndex += 1)
            {
                ClientMob npc = client.npcArray[npcIndex];
                int npcWorldX = npc.LocationX;
                int npcWorldY = npc.LocationY;
                int npcElevation = -client.engineHandle.GetAveragedElevation(npcWorldX, npcWorldY);
                int npcSpriteHandle = client.gameCamera.AddSpriteToScene(20000 + npcIndex, npcWorldX, npcElevation, npcWorldY, client.entityManager.GetNpc(npc.NpcIdentifier).SpriteWidth, client.entityManager.GetNpc(npc.NpcIdentifier).SpriteHeight, npcIndex + 30000);
                client.drawUpdatesPerformed += 1;

                if (npc.CurrentSprite == 8)
                {
                    client.gameCamera.UpdateSpritePosition(npcSpriteHandle, -30);
                }

                if (npc.CurrentSprite == 9)
                {
                    client.gameCamera.UpdateSpritePosition(npcSpriteHandle, 30);
                }
            }

            for (int groundItemIndex = 0; groundItemIndex < client.groundItemCount; groundItemIndex += 1)
            {
                int groundItemWorldX = client.groundItemX[groundItemIndex] * client.gridSize + 64;
                int groundItemWorldY = client.groundItemY[groundItemIndex] * client.gridSize + 64;
                client.gameCamera.AddSpriteToScene(40000 + client.groundItemID[groundItemIndex], groundItemWorldX, -client.engineHandle.GetAveragedElevation(groundItemWorldX, groundItemWorldY) - client.groundItemObjectVar[groundItemIndex], groundItemWorldY, 96, 64, groundItemIndex + 20000);
                client.drawUpdatesPerformed += 1;
            }

            for (int bubbleIndex = 0; bubbleIndex < client.teleBubbleCount; bubbleIndex += 1)
            {
                int bubbleWorldX = client.teleBubbleX[bubbleIndex] * client.gridSize + 64;
                int bubbleWorldY = client.teleBubbleY[bubbleIndex] * client.gridSize + 64;
                int bubbleType = client.teleBubbleType[bubbleIndex];

                if (bubbleType == 0)
                {
                    client.gameCamera.AddSpriteToScene(50000 + bubbleIndex, bubbleWorldX, -client.engineHandle.GetAveragedElevation(bubbleWorldX, bubbleWorldY), bubbleWorldY, 128, 256, bubbleIndex + 50000);
                    client.drawUpdatesPerformed += 1;
                }

                if (bubbleType == 1)
                {
                    client.gameCamera.AddSpriteToScene(50000 + bubbleIndex, bubbleWorldX, -client.engineHandle.GetAveragedElevation(bubbleWorldX, bubbleWorldY), bubbleWorldY, 128, 64, bubbleIndex + 50000);
                    client.drawUpdatesPerformed += 1;
                }
            }

            client.GameGraphics.IsInterlaced = false;
            client.GameGraphics.ClearScreen();
            client.GameGraphics.IsInterlaced = client.keyF1Toggle;
            if (client.lastLayerIndex == 3)
            {
                int lightLevelR = 40 + (int)(Helper.Random.NextDouble() * 3D);
                int lightLevelG = 40 + (int)(Helper.Random.NextDouble() * 7D);
                client.gameCamera.SetAllModelColours(lightLevelR, lightLevelG, -50, -10, -50);
            }
            client.itemsAboveHeadCount = 0;
            client.receivedMessagesCount = 0;
            client.healthBarVisibleCount = 0;
            if (client.cameraAutoAngleDebug)
            {
                if (client.configCameraAutoAngle && !client.cameraZoom)
                {
                    int previousCameraAutoAngle = client.cameraAutoAngle;
                    client.AutoRotateCamera();

                    if (client.cameraAutoAngle != previousCameraAutoAngle)
                    {
                        client.cameraAutoRotatePlayerX = client.ourPlayer.LocationX;
                        client.cameraAutoRotatePlayerY = client.ourPlayer.LocationY;
                    }
                }
                if (client.fogOfWar)
                {
                    client.gameCamera.FarClipDistance = 3000;
                    client.gameCamera.SpriteFarClipDistance = 3000;
                    client.gameCamera.FogGradientStep = 1;
                    client.gameCamera.FogStartDistance = 2800;
                }
                else
                {
                    client.gameCamera.FarClipDistance = 40000;
                    client.gameCamera.SpriteFarClipDistance = 40000;
                    client.gameCamera.FogGradientStep = 40000;
                    client.gameCamera.FogStartDistance = 40000;
                }
                client.cameraRotation = client.cameraAutoAngle * 32;
                int newCameraPosX = client.cameraAutoRotatePlayerX + client.cameraRotationXAmount;
                int newCameraPosY = client.cameraAutoRotatePlayerY + client.cameraRotationYAmount;
                client.gameCamera.SetCameraTransform(newCameraPosX, -client.engineHandle.GetAveragedElevation(newCameraPosX, newCameraPosY), newCameraPosY, 912, client.cameraRotation * 4, 0, 2000);
            }
            else
            {
                if (client.configCameraAutoAngle && !client.cameraZoom)
                {
                    client.AutoRotateCamera();
                }

                if (client.fogOfWar)
                {
                    if (!client.keyF1Toggle)
                    {
                        client.gameCamera.FarClipDistance = 2400;
                        client.gameCamera.SpriteFarClipDistance = 2400;
                        client.gameCamera.FogGradientStep = 1;
                        client.gameCamera.FogStartDistance = 2300;
                    }
                    else
                    {
                        client.gameCamera.FarClipDistance = 2200;
                        client.gameCamera.SpriteFarClipDistance = 2200;
                        client.gameCamera.FogGradientStep = 1;
                        client.gameCamera.FogStartDistance = 2100;
                    }
                }
                else
                {
                    client.gameCamera.FarClipDistance = 40000;
                    client.gameCamera.SpriteFarClipDistance = 40000;
                    client.gameCamera.FogGradientStep = 40000;
                    client.gameCamera.FogStartDistance = 40000;
                }
                int cameraX = client.cameraAutoRotatePlayerX + client.cameraRotationXAmount;
                int cameraY = client.cameraAutoRotatePlayerY + client.cameraRotationYAmount;
                client.gameCamera.SetCameraTransform(cameraX, -client.engineHandle.GetAveragedElevation(cameraX, cameraY), cameraY, 912, client.cameraRotation * 4, 0, client.cameraDistance * 2);
            }
            client.gameCamera.FinishCamera();
            worldRenderer.DrawAboveHeadThings();
            if (client.actionPictureType > 0)
            {
                client.GameGraphics.DrawPicture(client.walkMouseX - 8, client.walkMouseY - 8, client.BaseInventoryPic + 14 + (24 - client.actionPictureType) / 6);
            }

            if (client.actionPictureType < 0)
            {
                client.GameGraphics.DrawPicture(client.walkMouseX - 8, client.walkMouseY - 8, client.BaseInventoryPic + 18 + (24 + client.actionPictureType) / 6);
            }

            if (client.systemUpdate != 0)
            {
                int seconds = client.systemUpdate / 50;
                int minutes = seconds / 60;
                seconds %= 60;
                if (seconds < 10)
                {
                    client.GameGraphics.DrawText(LocalisationManager.GetString("game.system_update_prefix") + minutes + ":0" + seconds, 256, client.windowHeight - 7, 1, 0xffff00);
                }
                else
                {
                    client.GameGraphics.DrawText(LocalisationManager.GetString("game.system_update_prefix") + minutes + ":" + seconds, 256, client.windowHeight - 7, 1, 0xffff00);
                }
            }
            if (!client.loadArea)
            {
                int wildernessYDistance = 2203 - (client.sectionY + client.wildY + client.areaY);

                if (client.sectionX + client.wildX + client.areaX >= 2640)
                {
                    wildernessYDistance = -50;
                }

                if (wildernessYDistance > 0)
                {
                    int wildernessLevel = 1 + wildernessYDistance / 6;
                    client.GameGraphics.DrawPicture(453, client.windowHeight - 56, client.BaseInventoryPic + 13);
                    client.GameGraphics.DrawText(LocalisationManager.GetString("game.wilderness"), 465, client.windowHeight - 20, 1, 0xffff00);
                    client.GameGraphics.DrawText(LocalisationManager.GetString("game.wilderness_level_prefix") + wildernessLevel, 465, client.windowHeight - 7, 1, 0xffff00);

                    if (client.wildType == 0)
                    {
                        client.wildType = 2;
                    }
                }

                if (client.wildType == 0 && wildernessYDistance > -10 && wildernessYDistance <= 0)
                {
                    client.wildType = 1;
                }
            }
            if (client.messagesTab == 0)
            {
                for (int messageIndex = 0; messageIndex < 5; messageIndex += 1)
                {
                    if (client.messagesTimeout[messageIndex] > 0)
                    {
                        string messageText = client.messagesArray[messageIndex];
                        client.GameGraphics.DrawString(messageText, 7, client.windowHeight - 18 - messageIndex * 12, 1, 0xffff00);
                    }
                }
            }
            client.chatInputMenu.DisableInput(client.messagesHandleType2);
            client.chatInputMenu.DisableInput(client.messagesHandleType5);
            client.chatInputMenu.DisableInput(client.messagesHandleType6);
            if (client.messagesTab == 1)
            {
                client.chatInputMenu.EnableInput(client.messagesHandleType2);
            }
            else if (client.messagesTab == 2)
            {
                client.chatInputMenu.EnableInput(client.messagesHandleType5);
            }
            else if (client.messagesTab == 3)
            {
                client.chatInputMenu.EnableInput(client.messagesHandleType6);
            }

            Menu.chatMenuTextHeightMod = 2;
            client.chatInputMenu.DrawMenu();
            Menu.chatMenuTextHeightMod = 0;
            client.GameGraphics.DrawPicture(client.GameGraphics.GameWidth - 3 - 197, 3, client.BaseInventoryPic, 128);

            DrawMenus();

            client.GameGraphics.IsLoggedIn = false;
            socialRenderer.DrawChatMessageTabs();

            string text = "Coordinates: ( " + (client.sectionX + client.areaX) + "," + (client.sectionY + client.areaY) + " ) Section: (" + client.sectionX + "," + client.sectionY + ") Area: (" + client.areaX + "," + client.areaY + ")";
            // Text shadow
            client.GameGraphics.DrawString(text, 10 + 11, 10 + 11, 1, 0x000000);
            client.GameGraphics.DrawString(text, 10 + 10, 10 + 10, 1, 0xffffff);
            client.OnDrawDone();
        }
    }
}
