using System;

using OpenRS.Models;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class WorldRenderer(GameClient client, Action<int, int, int, int, Item> recordItemSprite)
    {

        public void DrawNpc(int x, int y, int width, int height, int npcIndex, int cameraXOffset, int scalePercentage)
            => DrawNPC(x, y, width, height, npcIndex, cameraXOffset, scalePercentage);


        public void DrawModel(int objectIndex, string modelName)
        {
            int tileX = client.objectX[objectIndex];
            int tileY = client.objectY[objectIndex];
            int relativeX = tileX - client.ourPlayer.currentX / 128;
            int relativeY = tileY - client.ourPlayer.currentY / 128;
            byte viewRadius = 7;

            if (tileX >= 0 &&
                tileY >= 0 &&
                tileX < 96 &&
                tileY < 96 &&
                relativeX > -viewRadius &&
                relativeX < viewRadius &&
                relativeY > -viewRadius &&
                relativeY < viewRadius)
            {
                client.gameCamera.RemoveModel(client.objectArray[objectIndex]);
                int modelNameIndex = GameData.GetModelNameIndex(modelName);
                GameObject newGameObject = client.gameDataObjects[modelNameIndex].CreateParent();
                client.gameCamera.AddModel(newGameObject);
                newGameObject.UpdateShading(true, 48, 48, -50, -10, -50);
                newGameObject.CopyTranslation(client.objectArray[objectIndex]);
                newGameObject.index = objectIndex;
                client.objectArray[objectIndex] = newGameObject;
            }
        }


        public void DrawPlayer(int x, int y, int width, int height, int playerIndex, int cameraXOffset, int scalePercentage)
        {
            ClientMob player = client.playerArray[playerIndex];

            if (player.Admin >= 2) // Admin level 2 or above marks an invisible moderator.
            {
                return;
            }

            int direction = player.currentSprite + (client.cameraRotation + 16) / 32 & 7;
            bool isMirrored = false;
            int effectiveDirection = direction;

            if (effectiveDirection == 5)
            {
                effectiveDirection = 3;
                isMirrored = true;
            }
            else if (effectiveDirection == 6)
            {
                effectiveDirection = 2;
                isMirrored = true;
            }
            else if (effectiveDirection == 7)
            {
                effectiveDirection = 1;
                isMirrored = true;
            }

            int walkAnimationIndex = effectiveDirection * 3 + client.walkModel[player.stepCount / 6 % 4];

            if (player.currentSprite == 8)
            {
                effectiveDirection = 5;
                direction = 2;
                isMirrored = false;
                x -= 5 * scalePercentage / 100;
                walkAnimationIndex = effectiveDirection * 3 + client.combatModelArray1[client.tick / 5 % 8];
            }
            else if (player.currentSprite == 9)
            {
                effectiveDirection = 5;
                direction = 2;
                isMirrored = true;
                x += 5 * scalePercentage / 100;
                walkAnimationIndex = effectiveDirection * 3 + client.combatModelArray2[client.tick / 6 % 8];
            }

            for (int animationLayerIndex = 0; animationLayerIndex < 12; animationLayerIndex += 1)
            {
                int animationModelIndex = client.animationModelArray[direction][animationLayerIndex];
                int appearanceItemIndex = player.appearanceItems[animationModelIndex] - 1;

                if (appearanceItemIndex > client.entityManager.AnimationCount - 1)
                {
                    continue;
                }

                if (appearanceItemIndex >= 0)
                {
                    int xOffset = 0;
                    int yOffset = 0;
                    int animationFrameIndex = walkAnimationIndex;

                    if (isMirrored && effectiveDirection >= 1 && effectiveDirection <= 3)
                    {
                        if (client.entityManager.GetAnimation(appearanceItemIndex).HasFemaleFrames)
                        {
                            animationFrameIndex += 15;
                        }
                        else if (animationModelIndex == 4 && effectiveDirection == 1)
                        {
                            xOffset = -22;
                            yOffset = -3;
                            animationFrameIndex = effectiveDirection * 3 + client.walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (animationModelIndex == 4 && effectiveDirection == 2)
                        {
                            xOffset = 0;
                            yOffset = -8;
                            animationFrameIndex = effectiveDirection * 3 + client.walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (animationModelIndex == 4 && effectiveDirection == 3)
                        {
                            xOffset = 26;
                            yOffset = -5;
                            animationFrameIndex = effectiveDirection * 3 + client.walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (animationModelIndex == 3 && effectiveDirection == 1)
                        {
                            xOffset = 22;
                            yOffset = 3;
                            animationFrameIndex = effectiveDirection * 3 + client.walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (animationModelIndex == 3 && effectiveDirection == 2)
                        {
                            xOffset = 0;
                            yOffset = 8;
                            animationFrameIndex = effectiveDirection * 3 + client.walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (animationModelIndex == 3 && effectiveDirection == 3)
                        {
                            xOffset = -26;
                            yOffset = 5;
                            animationFrameIndex = effectiveDirection * 3 + client.walkModel[(2 + player.stepCount / 6) % 4];
                        }
                    }

                    if (effectiveDirection != 5 || client.entityManager.GetAnimation(appearanceItemIndex).HasAttackFrames)
                    {
                        int pictureIndex = animationFrameIndex + client.entityManager.GetAnimation(appearanceItemIndex).SpriteIndex;
                        xOffset = xOffset * width / client.gameGraphics.pictureAssumedWidth[pictureIndex];
                        yOffset = yOffset * height / client.gameGraphics.pictureAssumedHeight[pictureIndex];
                        int pictureWidth = width * client.gameGraphics.pictureAssumedWidth[pictureIndex] / client.gameGraphics.pictureAssumedWidth[client.entityManager.GetAnimation(appearanceItemIndex).SpriteIndex];
                        xOffset -= (pictureWidth - width) / 2;
                        int characterColour = client.entityManager.GetAnimation(appearanceItemIndex).CharacterColour;
                        int skinColour = client.appearanceSkinColours[player.skinColour];

                        if (characterColour == 1)
                        {
                            characterColour = client.appearanceHairColours[player.hairColour];
                        }
                        else if (characterColour == 2)
                        {
                            characterColour = client.appearanceTopBottomColours[player.topColour];
                        }
                        else if (characterColour == 3)
                        {
                            characterColour = client.appearanceTopBottomColours[player.bottomColour];
                        }

                        client.gameGraphics.DrawImage(x + xOffset, y + yOffset, pictureWidth, height, pictureIndex, characterColour, skinColour, cameraXOffset, isMirrored);
                    }
                }
            }

            if (player.lastMessageTimeout > 0)
            {
                client.receivedMessageMidPoint[client.receivedMessagesCount] = client.gameGraphics.TextWidth(player.lastMessage, 1) / 2;

                if (client.receivedMessageMidPoint[client.receivedMessagesCount] > 150)
                {
                    client.receivedMessageMidPoint[client.receivedMessagesCount] = 150;
                }

                client.receivedMessageHeight[client.receivedMessagesCount] = client.gameGraphics.TextWidth(player.lastMessage, 1) / 300 * client.gameGraphics.TextHeightNumber(1);
                client.receivedMessageX[client.receivedMessagesCount] = x + width / 2;
                client.receivedMessageY[client.receivedMessagesCount] = y;
                client.receivedMessages[client.receivedMessagesCount] = player.lastMessage;
                client.receivedMessagesCount += 1;
            }

            if (player.playerSkullTimeout > 0)
            {
                client.itemAboveHeadX[client.itemsAboveHeadCount] = x + width / 2;
                client.itemAboveHeadY[client.itemsAboveHeadCount] = y;
                client.itemAboveHeadScale[client.itemsAboveHeadCount] = scalePercentage;
                client.itemAboveHeadID[client.itemsAboveHeadCount] = player.itemAboveHeadID;
                client.itemsAboveHeadCount += 1;
            }

            if (player.currentSprite == 8 || player.currentSprite == 9 || player.combatTimer != 0)
            {
                if (player.combatTimer > 0)
                {
                    int combatX = x;

                    if (player.currentSprite == 8)
                    {
                        combatX -= 20 * scalePercentage / 100;
                    }
                    else if (player.currentSprite == 9)
                    {
                        combatX += 20 * scalePercentage / 100;
                    }

                    int healthBarRatio = player.currentHits * 30 / player.baseHits;
                    client.healthBarX[client.healthBarVisibleCount] = combatX + width / 2;
                    client.healthBarY[client.healthBarVisibleCount] = y;
                    client.healthBarMissing[client.healthBarVisibleCount] = healthBarRatio;
                    client.healthBarVisibleCount += 1;
                }

                if (player.combatTimer > 150)
                {
                    int damageCombatX = x;

                    if (player.currentSprite == 8)
                    {
                        damageCombatX -= 10 * scalePercentage / 100;
                    }
                    else if (player.currentSprite == 9)
                    {
                        damageCombatX += 10 * scalePercentage / 100;
                    }

                    client.gameGraphics.DrawPicture(damageCombatX + width / 2 - 12, y + height / 2 - 12, client.baseInventoryPic + 11);
                    client.gameGraphics.DrawText(player.lastDamageCount.ToString(), damageCombatX + width / 2 - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }

            if (player.playerSkulled == 1 && player.playerSkullTimeout == 0)
            {
                int skullX = cameraXOffset + x + width / 2;

                if (player.currentSprite == 8)
                {
                    skullX -= 20 * scalePercentage / 100;
                }
                else if (player.currentSprite == 9)
                {
                    skullX += 20 * scalePercentage / 100;
                }

                int skullWidth = 16 * scalePercentage / 100;
                int skullHeight = 16 * scalePercentage / 100;
                client.gameGraphics.DrawEntity(skullX - skullWidth / 2, y - skullHeight / 2 - 10 * scalePercentage / 100, skullWidth, skullHeight, client.baseInventoryPic + 13);
            }
        }


        public void DrawMinimapMenu(bool canClick)
        {
            int minimapX = client.gameGraphics.gameWidth - 199;
            int minimapWidth = 156;
            int minimapHeight = 152;
            client.gameGraphics.DrawPicture(minimapX - 49, 3, client.baseInventoryPic + 2);
            minimapX += 40;
            client.gameGraphics.DrawBox(minimapX, 36, minimapWidth, minimapHeight, 0);
            client.gameGraphics.SetDimensions(minimapX, 36, minimapX + minimapWidth, 36 + minimapHeight);
            int zoomLevel = 192 + client.minimapRandomRotationY;
            int rotationAngle = client.cameraRotation + client.minimapRandomRotationX & 0xff;
            int mapOffsetX = (client.ourPlayer.currentX - 6040) * 3 * zoomLevel / 2048;
            int mapOffsetY = (client.ourPlayer.currentY - 6040) * 3 * zoomLevel / 2048;
            int cosine = Camera.trigonometryTable[1024 - rotationAngle * 4 & 0x3ff];
            int sine = Camera.trigonometryTable[(1024 - rotationAngle * 4 & 0x3ff) + 1024];
            int rotatedX = mapOffsetY * cosine + mapOffsetX * sine >> 18;
            mapOffsetY = mapOffsetY * sine - mapOffsetX * cosine >> 18;
            mapOffsetX = rotatedX;
            client.gameGraphics.DrawMinimapPic(minimapX + minimapWidth / 2 - mapOffsetX, 36 + minimapHeight / 2 + mapOffsetY, client.baseInventoryPic - 1, rotationAngle + 64 & 0xff, zoomLevel);

            for (int objectDrawIndex = 0; objectDrawIndex < client.objectCount; objectDrawIndex += 1)
            {
                int objectMapX = (client.objectX[objectDrawIndex] * client.gridSize + 64 - client.ourPlayer.currentX) * 3 * zoomLevel / 2048;
                int objectMapY = (client.objectY[objectDrawIndex] * client.gridSize + 64 - client.ourPlayer.currentY) * 3 * zoomLevel / 2048;
                int rotatedObjectX = objectMapY * cosine + objectMapX * sine >> 18;
                objectMapY = objectMapY * sine - objectMapX * cosine >> 18;
                objectMapX = rotatedObjectX;
                DrawMinimapObject(minimapX + minimapWidth / 2 + objectMapX, 36 + minimapHeight / 2 - objectMapY, 65535);
            }

            for (int groundItemIndex = 0; groundItemIndex < client.groundItemCount; groundItemIndex += 1)
            {
                int groundItemMapX = (client.groundItemX[groundItemIndex] * client.gridSize + 64 - client.ourPlayer.currentX) * 3 * zoomLevel / 2048;
                int groundItemMapY = (client.groundItemY[groundItemIndex] * client.gridSize + 64 - client.ourPlayer.currentY) * 3 * zoomLevel / 2048;
                int rotatedGroundItemX = groundItemMapY * cosine + groundItemMapX * sine >> 18;
                groundItemMapY = groundItemMapY * sine - groundItemMapX * cosine >> 18;
                groundItemMapX = rotatedGroundItemX;
                DrawMinimapObject(minimapX + minimapWidth / 2 + groundItemMapX, 36 + minimapHeight / 2 - groundItemMapY, 0xff0000);
            }

            for (int npcDrawIndex = 0; npcDrawIndex < client.npcCount; npcDrawIndex += 1)
            {
                ClientMob npc = client.npcArray[npcDrawIndex];
                int npcMapX = (npc.currentX - client.ourPlayer.currentX) * 3 * zoomLevel / 2048;
                int npcMapY = (npc.currentY - client.ourPlayer.currentY) * 3 * zoomLevel / 2048;
                int rotatedNpcX = npcMapY * cosine + npcMapX * sine >> 18;
                npcMapY = npcMapY * sine - npcMapX * cosine >> 18;
                npcMapX = rotatedNpcX;
                DrawMinimapObject(minimapX + minimapWidth / 2 + npcMapX, 36 + minimapHeight / 2 - npcMapY, 0xffff00);
            }

            for (int playerDrawIndex = 0; playerDrawIndex < client.playerCount; playerDrawIndex += 1)
            {
                ClientMob otherPlayer = client.playerArray[playerDrawIndex];
                int playerMapX = (otherPlayer.currentX - client.ourPlayer.currentX) * 3 * zoomLevel / 2048;
                int playerMapY = (otherPlayer.currentY - client.ourPlayer.currentY) * 3 * zoomLevel / 2048;
                int rotatedPlayerX = playerMapY * cosine + playerMapX * sine >> 18;
                playerMapY = playerMapY * sine - playerMapX * cosine >> 18;
                playerMapX = rotatedPlayerX;
                int playerDotColour = 0xffffff;

                for (int friendIndex = 0; friendIndex < client.friendsCount; friendIndex += 1)
                {
                    if (otherPlayer.nameHash != client.friendsList[friendIndex] || client.friendsWorld[friendIndex] != 99)
                    {
                        continue;
                    }

                    playerDotColour = 65280;
                    break;
                }

                DrawMinimapObject(minimapX + minimapWidth / 2 + playerMapX, 36 + minimapHeight / 2 - playerMapY, playerDotColour);
            }

            // Compass marker and player dot.
            client.gameGraphics.DrawCircle(minimapX + minimapWidth / 2, 36 + minimapHeight / 2, 2, 0xffffff, 255);
            client.gameGraphics.DrawMinimapPic(minimapX + 19, 55, client.baseInventoryPic + 24, client.cameraRotation + 128 & 0xff, 128);
            client.gameGraphics.SetDimensions(0, 0, client.windowWidth, client.windowHeight + 12);

            if (!canClick)
            {
                return;
            }

            int relativeMouseX = client.mouseX - (client.gameGraphics.gameWidth - 199);
            int relativeMouseY = client.mouseY - 36;

            if (relativeMouseX >= 40 && relativeMouseY >= 0 && relativeMouseX < 196 && relativeMouseY < 152)
            {
                int clickZoomLevel = 192 + client.minimapRandomRotationY;
                int clickRotationAngle = client.cameraRotation + client.minimapRandomRotationX & 0xff;
                int clickOriginX = client.gameGraphics.gameWidth - 199 + 40;
                int worldX = (client.mouseX - (clickOriginX + minimapWidth / 2)) * 16384 / (3 * clickZoomLevel);
                int worldY = (client.mouseY - (36 + minimapHeight / 2)) * 16384 / (3 * clickZoomLevel);
                int clickCosine = Camera.trigonometryTable[1024 - clickRotationAngle * 4 & 0x3ff];
                int clickSine = Camera.trigonometryTable[(1024 - clickRotationAngle * 4 & 0x3ff) + 1024];
                int rotatedWorldX = worldY * clickCosine + worldX * clickSine >> 15;
                worldY = worldY * clickSine - worldX * clickCosine >> 15;
                worldX = rotatedWorldX;
                worldX += client.ourPlayer.currentX;
                worldY = client.ourPlayer.currentY - worldY;

                if (client.mouseButtonClick == 1)
                {
                    client.WalkTo1Tile(client.sectionX, client.sectionY, worldX / 128, worldY / 128, false);
                }

                client.mouseButtonClick = 0;
            }
        }


        public void DrawTeleBubble(int x, int y, int entityWidth, int entityHeight, int bubbleIndex, int cameraXOffset, int scalePercentage)
        {
            int type = client.teleBubbleType[bubbleIndex];
            int time = client.teleBubbleTime[bubbleIndex];

            if (type == 0)
            {
                int normalBubbleColour = 255 + time * 5 * 256;
                client.gameGraphics.DrawCircle(x + entityWidth / 2, y + entityHeight / 2, 20 + time * 2, normalBubbleColour, 255 - time * 5);
            }

            if (type == 1)
            {
                int fireBubbleColour = 0xff0000 + time * 5 * 256;
                client.gameGraphics.DrawCircle(x + entityWidth / 2, y + entityHeight / 2, 10 + time, fireBubbleColour, 255 - time * 5);
            }
        }


        public void DrawNPC(int x, int y, int width, int height, int index, int unknown1, int unknown2)
        {
            ClientMob npc = client.npcArray[index];
            int frameIndex = npc.currentSprite + (client.cameraRotation + 16) / 32 & 7;
            bool isMirrored = false;
            int newFrameIndex = frameIndex;

            if (newFrameIndex == 5)
            {
                newFrameIndex = 3;
                isMirrored = true;
            }
            else if (newFrameIndex == 6)
            {
                newFrameIndex = 2;
                isMirrored = true;
            }
            else if (newFrameIndex == 7)
            {
                newFrameIndex = 1;
                isMirrored = true;
            }

            int animationFrameIndex = newFrameIndex * 3 + client.walkModel[npc.stepCount / client.entityManager.GetNpc(npc.npcId).WalkAnimationSpeed % 4];

            if (npc.currentSprite == 8)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                isMirrored = false;
                x -= client.entityManager.GetNpc(npc.npcId).CombatSwingOffset * unknown2 / 100;
                animationFrameIndex = newFrameIndex * 3 + client.combatModelArray1[client.tick / (client.entityManager.GetNpc(npc.npcId).CombatAnimationSpeed - 1) % 8];
            }
            else if (npc.currentSprite == 9)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                isMirrored = true;
                x += client.entityManager.GetNpc(npc.npcId).CombatSwingOffset * unknown2 / 100;
                animationFrameIndex = newFrameIndex * 3 + client.combatModelArray2[client.tick / client.entityManager.GetNpc(npc.npcId).CombatAnimationSpeed % 8];
            }

            for (int bodyPartIndex = 0; bodyPartIndex < 12; bodyPartIndex += 1)
            {
                int bodyPartType = client.animationModelArray[frameIndex][bodyPartIndex];
                int spriteId = client.entityManager.GetNpc(npc.npcId).Sprites[bodyPartType];

                if (spriteId >= 0)
                {
                    int drawOffsetX = 0;
                    int drawOffsetY = 0;
                    int frameNumber = animationFrameIndex;

                    if (isMirrored &&
                        newFrameIndex >= 1 &&
                        newFrameIndex <= 3 &&
                        client.entityManager.GetAnimation(spriteId).HasFemaleFrames)
                    {
                        frameNumber += 15;
                    }

                    if (newFrameIndex != 5 || client.entityManager.GetAnimation(spriteId).HasAttackFrames)
                    {
                        int actualFrame = frameNumber + client.entityManager.GetAnimation(spriteId).SpriteIndex;
                        drawOffsetX = drawOffsetX * width / client.gameGraphics.pictureAssumedWidth[actualFrame];
                        drawOffsetY = drawOffsetY * height / client.gameGraphics.pictureAssumedHeight[actualFrame];
                        int scaledWidth = width * client.gameGraphics.pictureAssumedWidth[actualFrame] / client.gameGraphics.pictureAssumedWidth[client.entityManager.GetAnimation(spriteId).SpriteIndex];
                        drawOffsetX -= (scaledWidth - width) / 2;
                        int bodyColour = client.entityManager.GetAnimation(spriteId).CharacterColour;
                        int skinColour = 0;

                        if (bodyColour == 1)
                        {
                            bodyColour = client.entityManager.GetNpc(npc.npcId).Appearance.HairColour;
                            skinColour = client.entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }
                        else if (bodyColour == 2)
                        {
                            bodyColour = client.entityManager.GetNpc(npc.npcId).Appearance.TopColour;
                            skinColour = client.entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }
                        else if (bodyColour == 3)
                        {
                            bodyColour = client.entityManager.GetNpc(npc.npcId).Appearance.TrousersColour;
                            skinColour = client.entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }

                        client.gameGraphics.DrawImage(x + drawOffsetX, y + drawOffsetY, scaledWidth, height, actualFrame, bodyColour, skinColour, unknown1, isMirrored);
                    }
                }
            }

            if (npc.lastMessageTimeout > 0)
            {
                client.receivedMessageMidPoint[client.receivedMessagesCount] = client.gameGraphics.TextWidth(npc.lastMessage, 1) / 2;

                if (client.receivedMessageMidPoint[client.receivedMessagesCount] > 150)
                {
                    client.receivedMessageMidPoint[client.receivedMessagesCount] = 150;
                }

                client.receivedMessageHeight[client.receivedMessagesCount] = client.gameGraphics.TextWidth(npc.lastMessage, 1) / 300 * client.gameGraphics.TextHeightNumber(1);
                client.receivedMessageX[client.receivedMessagesCount] = x + width / 2;
                client.receivedMessageY[client.receivedMessagesCount] = y;
                client.receivedMessages[client.receivedMessagesCount++] = npc.lastMessage;
            }

            if (npc.currentSprite == 8 || npc.currentSprite == 9 || npc.combatTimer != 0)
            {
                if (npc.combatTimer > 0)
                {
                    int combatBarX = x;

                    if (npc.currentSprite == 8)
                    {
                        combatBarX -= 20 * unknown2 / 100;
                    }
                    else if (npc.currentSprite == 9)
                    {
                        combatBarX += 20 * unknown2 / 100;
                    }

                    int currentHealthPercent = npc.currentHits * 30 / npc.baseHits;
                    client.healthBarX[client.healthBarVisibleCount] = combatBarX + width / 2;
                    client.healthBarY[client.healthBarVisibleCount] = y;
                    client.healthBarMissing[client.healthBarVisibleCount++] = currentHealthPercent;
                }

                if (npc.combatTimer > 150)
                {
                    int combatImageX = x;

                    if (npc.currentSprite == 8)
                    {
                        combatImageX -= 10 * unknown2 / 100;
                    }
                    else if (npc.currentSprite == 9)
                    {
                        combatImageX += 10 * unknown2 / 100;
                    }

                    client.gameGraphics.DrawPicture(combatImageX + width / 2 - 12, y + height / 2 - 12, client.baseInventoryPic + 12);
                    client.gameGraphics.DrawText(npc.lastDamageCount.ToString(), combatImageX + width / 2 - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
        }


        public void DrawAboveHeadThings()
        {
            for (int messageIndex = 0; messageIndex < client.receivedMessagesCount; messageIndex += 1)
            {
                int height = client.gameGraphics.TextHeightNumber(1);
                int x = client.receivedMessageX[messageIndex];
                int y = client.receivedMessageY[messageIndex];
                int midpoint = client.receivedMessageMidPoint[messageIndex];
                int messageHeight = client.receivedMessageHeight[messageIndex];
                bool hasOverlap = true;

                while (hasOverlap)
                {
                    hasOverlap = false;

                    for (int otherMessageIndex = 0; otherMessageIndex < messageIndex; otherMessageIndex += 1)
                    {
                        if (y + messageHeight > client.receivedMessageY[otherMessageIndex] - height &&
                            y - height < client.receivedMessageY[otherMessageIndex] + client.receivedMessageHeight[otherMessageIndex] &&
                            x - midpoint < client.receivedMessageX[otherMessageIndex] + client.receivedMessageMidPoint[otherMessageIndex] &&
                            x + midpoint > client.receivedMessageX[otherMessageIndex] - client.receivedMessageMidPoint[otherMessageIndex] &&
                            client.receivedMessageY[otherMessageIndex] - height - messageHeight < y)
                        {
                            y = client.receivedMessageY[otherMessageIndex] - height - messageHeight;
                            hasOverlap = true;
                        }
                    }
                }

                client.receivedMessageY[messageIndex] = y;
                client.gameGraphics.DrawFloatingText(client.receivedMessages[messageIndex], x, y, 1, 0xffff00, 300);
            }

            for (int itemAboveIndex = 0; itemAboveIndex < client.itemsAboveHeadCount; itemAboveIndex += 1)
            {
                int x = client.itemAboveHeadX[itemAboveIndex];
                int y = client.itemAboveHeadY[itemAboveIndex];
                int scale = client.itemAboveHeadScale[itemAboveIndex];
                int id = client.itemAboveHeadID[itemAboveIndex];
                int width = 39 * scale / 100;
                int height = 27 * scale / 100;
                int itemTopY = y - height;
                client.gameGraphics.DrawTransparentImage(x - width / 2, itemTopY, width, height, client.baseInventoryPic + 9, 85);
                int itemDisplayWidth = 36 * scale / 100;
                int itemDisplayHeight = 24 * scale / 100;
                recordItemSprite(x - itemDisplayWidth / 2, itemTopY + height / 2 - itemDisplayHeight / 2, itemDisplayWidth, itemDisplayHeight, client.entityManager.GetItem(id));
            }

            for (int healthBarIndex = 0; healthBarIndex < client.healthBarVisibleCount; healthBarIndex += 1)
            {
                int x = client.healthBarX[healthBarIndex];
                int y = client.healthBarY[healthBarIndex];
                int missing = client.healthBarMissing[healthBarIndex];
                client.gameGraphics.DrawBoxAlpha(x - 15, y - 3, missing, 5, 65280, 192);
                client.gameGraphics.DrawBoxAlpha(x - 15 + missing, y - 3, 30 - missing, 5, 0xff0000, 192);
            }
        }


        public void DrawMinimapObject(int x, int y, int colour)
        {
            client.gameGraphics.DrawMinimapPixel(x, y, colour);
            client.gameGraphics.DrawMinimapPixel(x - 1, y, colour);
            client.gameGraphics.DrawMinimapPixel(x + 1, y, colour);
            client.gameGraphics.DrawMinimapPixel(x, y - 1, colour);
            client.gameGraphics.DrawMinimapPixel(x, y + 1, colour);
        }


    }
}
