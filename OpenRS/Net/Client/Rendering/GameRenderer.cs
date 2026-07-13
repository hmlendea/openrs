using System;
using System.Collections.Generic;

using OpenRS.Models;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;
using OpenRS.Net.Client.Utilities;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class GameRenderer(GameClient client)
    {
        private List<ItemSpriteDrawCall> pendingItemSpriteDrawCalls = [];

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

        public void DrawNpc(int x, int y, int width, int height, int npcIndex, int cameraXOffset, int scalePercentage)
            => DrawNPC(x, y, width, height, npcIndex, cameraXOffset, scalePercentage);

        public Models.Enumerations.CombatStyle CombatStyle
        {
            get => (Models.Enumerations.CombatStyle)client.combatStyle;
            set => client.combatStyle = (int)value;
        }
        public void DrawReportAbuseBox1()
        {
            client.reportAbuseOptionSelected = 0;
            int yOffset = 135;
            for (int option = 0; option < 12; option += 1)
            {
                if (client.mouseX > 66 && client.mouseX < 446 && client.mouseY >= yOffset - 12 && client.mouseY < yOffset + 3)
                {
                    client.reportAbuseOptionSelected = option + 1;
                }

                yOffset += 14;
            }

            if (client.mouseButtonClick != 0 && client.reportAbuseOptionSelected != 0)
            {
                client.mouseButtonClick = 0;
                client.showAbuseBox = 2;
                client.inputText = "";
                client.enteredInputText = "";
                return;
            }
            yOffset += 15;
            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;
                if (client.mouseX < 56 || client.mouseY < 35 || client.mouseX > 456 || client.mouseY > 325)
                {
                    client.showAbuseBox = 0;
                    return;
                }
                if (client.mouseX > 66 && client.mouseX < 446 && client.mouseY >= yOffset - 15 && client.mouseY < yOffset + 5)
                {
                    client.showAbuseBox = 0;
                    return;
                }
            }
            client.gameGraphics.DrawBox(56, 35, 400, 290, 0);
            client.gameGraphics.DrawBoxEdge(56, 35, 400, 290, 0xffffff);
            yOffset = 50;
            client.gameGraphics.DrawText("This form is for reporting players who are breaking our rules", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            client.gameGraphics.DrawText("Using it sends a snapshot of the last 60 secs of activity to us", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            client.gameGraphics.DrawText("If you misuse this form, you will be banned.", 256, yOffset, 1, 0xff8000);
            yOffset += 15;
            yOffset += 10;
            client.gameGraphics.DrawText("First indicate which of our 12 rules is being broken. For a detailed", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            client.gameGraphics.DrawText("explanation of each rule please read the manual on our website.", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            int j1;
            if (client.reportAbuseOptionSelected == 1)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("1: Offensive language", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 2)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("2: Item scamming", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 3)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("3: Password scamming", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 4)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("4: Bug abuse", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 5)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("5: Jagex Staff impersonation", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 6)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("6: Account sharing/trading", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 7)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("7: Macroing", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 8)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("8: Mutiple logging in", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 9)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("9: Encouraging others to break rules", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 10)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("10: Misuse of customer support", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 11)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("11: Advertising / website", 256, yOffset, 1, j1);
            yOffset += 14;
            if (client.reportAbuseOptionSelected == 12)
            {
                client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            client.gameGraphics.DrawText("12: Real world item trading", 256, yOffset, 1, j1);
            yOffset += 14;
            yOffset += 15;
            j1 = 0xffffff;
            if (client.mouseX > 196 && client.mouseX < 316 && client.mouseY > yOffset - 15 && client.mouseY < yOffset + 5)
            {
                j1 = 0xffff00;
            }

            client.gameGraphics.DrawText("Click here to cancel", 256, yOffset, 1, j1);
        }
        public void DrawModel(int l, string s1)
        {
            int i1 = client.objectX[l];
            int j1 = client.objectY[l];
            int k1 = i1 - client.ourPlayer.currentX / 128;
            int l1 = j1 - client.ourPlayer.currentY / 128;
            byte byte0 = 7;
            if (i1 >= 0 && j1 >= 0 && i1 < 96 && j1 < 96 && k1 > -byte0 && k1 < byte0 && l1 > -byte0 && l1 < byte0)
            {
                client.gameCamera.RemoveModel(client.objectArray[l]);
                int i2 = GameData.GetModelNameIndex(s1);
                GameObject j2 = client.gameDataObjects[i2].CreateParent();
                client.gameCamera.AddModel(j2);
                j2.UpdateShading(true, 48, 48, -50, -10, -50);
                j2.CopyTranslation(client.objectArray[l]);
                j2.index = l;
                client.objectArray[l] = j2;
            }
        }
        public void DrawPlayer(int x, int y, int width, int height, int playerIndex, int cameraXOffset, int scalePercentage)
        {
            ClientMob f1 = client.playerArray[playerIndex];
            if (f1.bottomColour == 255) // TODO: This checks if the player is an invisible moderator.
            {
                return;
            }

            int direction = f1.currentSprite + (client.cameraRotation + 16) / 32 & 7;
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
            int j1 = direction2 * 3 + client.walkModel[f1.stepCount / 6 % 4];
            if (f1.currentSprite == 8)
            {
                direction2 = 5;
                direction = 2;
                flag = false;
                x -= 5 * scalePercentage / 100;
                j1 = direction2 * 3 + client.combatModelArray1[client.tick / 5 % 8];
            }
            else
                if (f1.currentSprite == 9)
                {
                    direction2 = 5;
                    direction = 2;
                    flag = true;
                    x += 5 * scalePercentage / 100;
                    j1 = direction2 * 3 + client.combatModelArray2[client.tick / 6 % 8];
                }
            for (int k1 = 0; k1 < 12; k1 += 1)
            {
                int l1 = client.animationModelArray[direction][k1];
                int l2 = f1.appearanceItems[l1] - 1;
                if (l2 > client.entityManager.AnimationCount - 1)
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
                        if (client.entityManager.GetAnimation(l2).HasF == 1)
                        {
                            j4 += 15;
                        }
                        else if (l1 == 4 && direction2 == 1)
                        {
                            k3 = -22;
                            i4 = -3;
                            j4 = direction2 * 3 + client.walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = -8;
                            j4 = direction2 * 3 + client.walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 3)
                        {
                            k3 = 26;
                            i4 = -5;
                            j4 = direction2 * 3 + client.walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 1)
                        {
                            k3 = 22;
                            i4 = 3;
                            j4 = direction2 * 3 + client.walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = 8;
                            j4 = direction2 * 3 + client.walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 3)
                        {
                            k3 = -26;
                            i4 = 5;
                            j4 = direction2 * 3 + client.walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                    }

                    if (direction2 != 5 || client.entityManager.GetAnimation(l2).HasA == 1)
                    {
                        int k4 = j4 + client.entityManager.GetAnimation(l2).Number;
                        k3 = k3 * width / client.gameGraphics.pictureAssumedWidth[k4];
                        i4 = i4 * height / client.gameGraphics.pictureAssumedHeight[k4];
                        int l4 = width * client.gameGraphics.pictureAssumedWidth[k4] / client.gameGraphics.pictureAssumedWidth[client.entityManager.GetAnimation(l2).Number];
                        k3 -= (l4 - width) / 2;
                        int i5 = client.entityManager.GetAnimation(l2).CharacterColour;
                        int j5 = client.appearanceSkinColours[f1.skinColour];
                        if (i5 == 1)
                        {
                            i5 = client.appearanceHairColours[f1.hairColour];
                        }
                        else
                            if (i5 == 2)
                        {
                            i5 = client.appearanceTopBottomColours[f1.topColour];
                        }
                        else
                                if (i5 == 3)
                        {
                            i5 = client.appearanceTopBottomColours[f1.bottomColour];
                        }

                        client.gameGraphics.DrawImage(x + k3, y + i4, l4, height, k4, i5, j5, cameraXOffset, flag);
                    }
                }
            }

            if (f1.lastMessageTimeout > 0)
            {
                client.receivedMessageMidPoint[client.receivedMessagesCount] = client.gameGraphics.TextWidth(f1.lastMessage, 1) / 2;

                if (client.receivedMessageMidPoint[client.receivedMessagesCount] > 150)
                {
                    client.receivedMessageMidPoint[client.receivedMessagesCount] = 150;
                }

                client.receivedMessageHeight[client.receivedMessagesCount] = client.gameGraphics.TextWidth(f1.lastMessage, 1) / 300 * client.gameGraphics.TextHeightNumber(1);
                client.receivedMessageX[client.receivedMessagesCount] = x + width / 2;
                client.receivedMessageY[client.receivedMessagesCount] = y;
                client.receivedMessages[client.receivedMessagesCount++] = f1.lastMessage;
            }
            if (f1.playerSkullTimeout > 0)
            {
                client.itemAboveHeadX[client.itemsAboveHeadCount] = x + width / 2;
                client.itemAboveHeadY[client.itemsAboveHeadCount] = y;
                client.itemAboveHeadScale[client.itemsAboveHeadCount] = scalePercentage;
                client.itemAboveHeadID[client.itemsAboveHeadCount++] = f1.itemAboveHeadID;
            }
            if (f1.currentSprite == 8 || f1.currentSprite == 9 || f1.combatTimer != 0)
            {
                if (f1.combatTimer > 0)
                {
                    int i2 = x;
                    if (f1.currentSprite == 8)
                    {
                        i2 -= 20 * scalePercentage / 100;
                    }
                    else
                        if (f1.currentSprite == 9)
                    {
                        i2 += 20 * scalePercentage / 100;
                    }

                    int i3 = f1.currentHits * 30 / f1.baseHits;
                    client.healthBarX[client.healthBarVisibleCount] = i2 + width / 2;
                    client.healthBarY[client.healthBarVisibleCount] = y;
                    client.healthBarMissing[client.healthBarVisibleCount++] = i3;
                }
                if (f1.combatTimer > 150)
                {
                    int j2 = x;
                    if (f1.currentSprite == 8)
                    {
                        j2 -= 10 * scalePercentage / 100;
                    }
                    else
                        if (f1.currentSprite == 9)
                    {
                        j2 += 10 * scalePercentage / 100;
                    }

                    client.gameGraphics.DrawPicture(j2 + width / 2 - 12, y + height / 2 - 12, client.baseInventoryPic + 11);
                    client.gameGraphics.DrawText(f1.lastDamageCount.ToString(), j2 + width / 2 - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
            if (f1.playerSkulled == 1 && f1.playerSkullTimeout == 0)
            {
                int k2 = cameraXOffset + x + width / 2;
                if (f1.currentSprite == 8)
                {
                    k2 -= 20 * scalePercentage / 100;
                }
                else
                    if (f1.currentSprite == 9)
                {
                    k2 += 20 * scalePercentage / 100;
                }

                int j3 = 16 * scalePercentage / 100;
                int l3 = 16 * scalePercentage / 100;
                client.gameGraphics.DrawEntity(k2 - j3 / 2, y - l3 / 2 - 10 * scalePercentage / 100, j3, l3, client.baseInventoryPic + 13);
            }
        }
        public void DrawDuelConfirmBox()
        {
            sbyte byte0 = 22;
            sbyte byte1 = 36;
            client.gameGraphics.DrawBox(byte0, byte1, 468, 16, 192);
            int l = 0x989898;
            client.gameGraphics.DrawBoxAlpha(byte0, byte1 + 16, 468, 246, l, 160);
            client.gameGraphics.DrawText("Please confirm your duel with @yel@" + DataOperations.HashToName(client.duelOpponentHash), byte0 + 234, byte1 + 12, 1, 0xffffff);
            client.gameGraphics.DrawText("Your stake:", byte0 + 117, byte1 + 30, 1, 0xffff00);
            for (int i1 = 0; i1 < client.duelOurStakeCount; i1 += 1)
            {
                Item stakeItem = client.entityManager.GetItem(client.duelOurStakeItem[i1]);
                string s1 = stakeItem.Name;
                if (stakeItem.IsStackable == 0)
                {
                    s1 = s1 + " x " + GameClientUtilities.FormatItemCount(client.duelOurStakeItemCount[i1]);
                }

                client.gameGraphics.DrawText(s1, byte0 + 117, byte1 + 42 + i1 * 12, 1, 0xffffff);
            }

            if (client.duelOurStakeCount == 0)
            {
                client.gameGraphics.DrawText("Nothing!", byte0 + 117, byte1 + 42, 1, 0xffffff);
            }

            client.gameGraphics.DrawText("Your opponent's stake:", byte0 + 351, byte1 + 30, 1, 0xffff00);
            for (int j1 = 0; j1 < client.duelOpponentStakeCount; j1 += 1)
            {
                Item stakeItem = client.entityManager.GetItem(client.duelOpponentStakeItem[j1]);
                string s2 = stakeItem.Name;
                if (stakeItem.IsStackable == 0)
                {
                    s2 = s2 + " x " + GameClientUtilities.FormatItemCount(client.duelOutStakeItemCount[j1]);
                }

                client.gameGraphics.DrawText(s2, byte0 + 351, byte1 + 42 + j1 * 12, 1, 0xffffff);
            }

            if (client.duelOpponentStakeCount == 0)
            {
                client.gameGraphics.DrawText("Nothing!", byte0 + 351, byte1 + 42, 1, 0xffffff);
            }

            if (client.duelRetreat == 0)
            {
                client.gameGraphics.DrawText("You can retreat from this duel", byte0 + 234, byte1 + 180, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText("No retreat is possible!", byte0 + 234, byte1 + 180, 1, 0xff0000);
            }

            if (client.duelMagic == 0)
            {
                client.gameGraphics.DrawText("Magic may be used", byte0 + 234, byte1 + 192, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText("Magic cannot be used", byte0 + 234, byte1 + 192, 1, 0xff0000);
            }

            if (client.duelPrayer == 0)
            {
                client.gameGraphics.DrawText("Prayer may be used", byte0 + 234, byte1 + 204, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText("Prayer cannot be used", byte0 + 234, byte1 + 204, 1, 0xff0000);
            }

            if (client.duelWeapons == 0)
            {
                client.gameGraphics.DrawText("Weapons may be used", byte0 + 234, byte1 + 216, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText("Weapons cannot be used", byte0 + 234, byte1 + 216, 1, 0xff0000);
            }

            client.gameGraphics.DrawText("If you are sure click 'Accept' to begin the duel", byte0 + 234, byte1 + 230, 1, 0xffffff);
            if (!client.duelConfirmOurAccepted)
            {
                client.gameGraphics.DrawPicture(byte0 + 118 - 35, byte1 + 238, client.baseInventoryPic + 25);
                client.gameGraphics.DrawPicture(byte0 + 352 - 35, byte1 + 238, client.baseInventoryPic + 26);
            }
            else
            {
                client.gameGraphics.DrawText("Waiting for other player...", byte0 + 234, byte1 + 250, 1, 0xffff00);
            }
            if (client.mouseButtonClick == 1)
            {
                if (client.mouseX < byte0 || client.mouseY < byte1 || client.mouseX > byte0 + 468 || client.mouseY > byte1 + 262)
                {
                    client.showDuelConfirmBox = false;
                    client.streamClass.CreatePacket(35);
                    client.streamClass.FormatPacket();
                }
                if (client.mouseX >= byte0 + 118 - 35 && client.mouseX <= byte0 + 118 + 70 && client.mouseY >= byte1 + 238 && client.mouseY <= byte1 + 238 + 21)
                {
                    client.duelConfirmOurAccepted = true;
                    client.streamClass.CreatePacket(87);
                    client.streamClass.FormatPacket();
                }
                if (client.mouseX >= byte0 + 352 - 35 && client.mouseX <= byte0 + 353 + 70 && client.mouseY >= byte1 + 238 && client.mouseY <= byte1 + 238 + 21)
                {
                    client.showDuelConfirmBox = false;
                    client.streamClass.CreatePacket(35);
                    client.streamClass.FormatPacket();
                }
                client.mouseButtonClick = 0;
            }
        }
        public void DrawInventoryMenu(bool canRightClick)
        {
            int l = client.gameGraphics.gameWidth - 248;
            client.gameGraphics.DrawPicture(l, 3, client.baseInventoryPic + 1);
            for (int i1 = 0; i1 < client.maxInventoryItems; i1 += 1)
            {
                int j1 = l + i1 % 5 * 49;
                int l1 = 36 + i1 / 5 * 34;
                if (i1 < client.inventoryItemsCount && client.inventoryItemEquipped[i1] == 1)
                {
                    client.gameGraphics.DrawBoxAlpha(j1, l1, 49, 34, 0xff0000, 128);
                }
                else
                {
                    client.gameGraphics.DrawBoxAlpha(j1, l1, 49, 34, GameImage.RgbToInt(181, 181, 181), 128);
                }

                if (i1 < client.inventoryItemsCount)
                {
                    Item inventoryItem = client.entityManager.GetItem(client.inventoryItems[i1]);
                    RecordItemSprite(j1, l1, 48, 32, inventoryItem);
                    if (inventoryItem.IsStackable == 0)
                    {
                        client.gameGraphics.DrawString(client.inventoryItemCount[i1].ToString(), j1 + 1, l1 + 10, 1, 0xffff00);
                    }
                }
            }

            for (int k1 = 1; k1 <= 4; k1 += 1)
            {
                client.gameGraphics.DrawLineY(l + k1 * 49, 36, client.maxInventoryItems / 5 * 34, 0);
            }

            for (int i2 = 1; i2 <= client.maxInventoryItems / 5 - 1; i2 += 1)
            {
                client.gameGraphics.DrawLineX(l, 36 + i2 * 34, 245, 0);
            }

            if (!canRightClick)
            {
                return;
            }

            l = client.mouseX - (client.gameGraphics.gameWidth - 248);
            int j2 = client.mouseY - 36;
            if (l >= 0 && j2 >= 0 && l < 248 && j2 < client.maxInventoryItems / 5 * 34)
            {
                int k2 = l / 49 + j2 / 34 * 5;
                if (k2 < client.inventoryItemsCount)
                {
                    int l2 = client.inventoryItems[k2];
                    Item inventoryItem = client.entityManager.GetItem(l2);
                    if (client.selectedSpell >= 0)
                    {
                        if (client.entityManager.GetSpell(client.selectedSpell).Type == 3)
                        {
                            client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 600;
                            client.menuActionType[client.menuOptionsCount] = k2;
                            client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                            client.menuOptionsCount += 1;
                            return;
                        }
                    }
                    else
                    {
                        if (client.selectedItem >= 0)
                        {
                            client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 610;
                            client.menuActionType[client.menuOptionsCount] = k2;
                            client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                            client.menuOptionsCount += 1;
                            return;
                        }
                        if (client.inventoryItemEquipped[k2] == 1)
                        {
                            client.menuText1[client.menuOptionsCount] = "Remove";
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 620;
                            client.menuActionType[client.menuOptionsCount] = k2;
                            client.menuOptionsCount += 1;
                        }
                        else
                            if (inventoryItem.IsEquipable != 0)
                            {
                                if ((inventoryItem.IsEquipable & 0x18) != 0)
                            {
                                client.menuText1[client.menuOptionsCount] = "Wield";
                            }
                            else
                            {
                                client.menuText1[client.menuOptionsCount] = "Wear";
                            }

                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                                client.menuActionID[client.menuOptionsCount] = 630;
                                client.menuActionType[client.menuOptionsCount] = k2;
                                client.menuOptionsCount += 1;
                            }
                        if (inventoryItem.Command != "")
                        {
                            client.menuText1[client.menuOptionsCount] = inventoryItem.Command;
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 640;
                            client.menuActionType[client.menuOptionsCount] = k2;
                            client.menuOptionsCount += 1;
                        }
                        client.menuText1[client.menuOptionsCount] = "Use";
                        client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                        client.menuActionID[client.menuOptionsCount] = 650;
                        client.menuActionType[client.menuOptionsCount] = k2;
                        client.menuOptionsCount += 1;
                        client.menuText1[client.menuOptionsCount] = "Drop";
                        client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                        client.menuActionID[client.menuOptionsCount] = 660;
                        client.menuActionType[client.menuOptionsCount] = k2;
                        client.menuOptionsCount += 1;
                        client.menuText1[client.menuOptionsCount] = "Examine";
                        client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                        client.menuActionID[client.menuOptionsCount] = 3600;
                        client.menuActionType[client.menuOptionsCount] = l2;
                        client.menuOptionsCount += 1;
                    }
                }
            }
        }
        public void DrawMinimapMenu(bool canClick)
        {
            int l = client.gameGraphics.gameWidth - 199;
            int c1 = 156;//'æ';//(char)234;//'\u234';
            int c3 = 152;// '~';//(char)230;//'\u230';
            client.gameGraphics.DrawPicture(l - 49, 3, client.baseInventoryPic + 2);
            l += 40;
            client.gameGraphics.DrawBox(l, 36, c1, c3, 0);
            client.gameGraphics.SetDimensions(l, 36, l + c1, 36 + c3);
            int j1 = 192 + client.minimapRandomRotationY;
            int l1 = client.cameraRotation + client.minimapRandomRotationX & 0xff;
            int j2 = (client.ourPlayer.currentX - 6040) * 3 * j1 / 2048;
            int l3 = (client.ourPlayer.currentY - 6040) * 3 * j1 / 2048;
            int j5 = Camera.trigonometryTable[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.trigonometryTable[(1024 - l1 * 4 & 0x3ff) + 1024];
            int j6 = l3 * j5 + j2 * l5 >> 18;
            l3 = l3 * l5 - j2 * j5 >> 18;
            j2 = j6;
            client.gameGraphics.DrawMinimapPic(l + c1 / 2 - j2, 36 + c3 / 2 + l3, client.baseInventoryPic - 1, l1 + 64 & 0xff, j1);
            for (int l7 = 0; l7 < client.objectCount; l7 += 1)
            {
                int k2 = (client.objectX[l7] * client.gridSize + 64 - client.ourPlayer.currentX) * 3 * j1 / 2048;
                int i4 = (client.objectY[l7] * client.gridSize + 64 - client.ourPlayer.currentY) * 3 * j1 / 2048;
                int k6 = i4 * j5 + k2 * l5 >> 18;
                i4 = i4 * l5 - k2 * j5 >> 18;
                k2 = k6;
                DrawMinimapObject(l + c1 / 2 + k2, 36 + c3 / 2 - i4, 65535);
            }

            for (int i8 = 0; i8 < client.groundItemCount; i8 += 1)
            {
                int l2 = (client.groundItemX[i8] * client.gridSize + 64 - client.ourPlayer.currentX) * 3 * j1 / 2048;
                int j4 = (client.groundItemY[i8] * client.gridSize + 64 - client.ourPlayer.currentY) * 3 * j1 / 2048;
                int l6 = j4 * j5 + l2 * l5 >> 18;
                j4 = j4 * l5 - l2 * j5 >> 18;
                l2 = l6;
                DrawMinimapObject(l + c1 / 2 + l2, 36 + c3 / 2 - j4, 0xff0000);
            }

            for (int j8 = 0; j8 < client.npcCount; j8 += 1)
            {
                ClientMob f1 = client.npcArray[j8];
                int i3 = (f1.currentX - client.ourPlayer.currentX) * 3 * j1 / 2048;
                int k4 = (f1.currentY - client.ourPlayer.currentY) * 3 * j1 / 2048;
                int i7 = k4 * j5 + i3 * l5 >> 18;
                k4 = k4 * l5 - i3 * j5 >> 18;
                i3 = i7;
                DrawMinimapObject(l + c1 / 2 + i3, 36 + c3 / 2 - k4, 0xffff00);
            }

            for (int k8 = 0; k8 < client.playerCount; k8 += 1)
            {
                ClientMob f2 = client.playerArray[k8];
                int j3 = (f2.currentX - client.ourPlayer.currentX) * 3 * j1 / 2048;
                int l4 = (f2.currentY - client.ourPlayer.currentY) * 3 * j1 / 2048;
                int j7 = l4 * j5 + j3 * l5 >> 18;
                l4 = l4 * l5 - j3 * j5 >> 18;
                j3 = j7;
                int i9 = 0xffffff;
                for (int j9 = 0; j9 < client.friendsCount; j9 += 1)
                {
                    if (f2.nameHash != client.friendsList[j9] || client.friendsWorld[j9] != 99)
                    {
                        continue;
                    }

                    i9 = 65280;
                    break;
                }

                DrawMinimapObject(l + c1 / 2 + j3, 36 + c3 / 2 - l4, i9);
            }

            // compass
            client.gameGraphics.DrawCircle(l + c1 / 2, 36 + c3 / 2, 2, 0xffffff, 255);
            client.gameGraphics.DrawMinimapPic(l + 19, 55, client.baseInventoryPic + 24, client.cameraRotation + 128 & 0xff, 128);
            client.gameGraphics.SetDimensions(0, 0, client.windowWidth, client.windowHeight + 12);
            if (!canClick)
            {
                return;
            }

            l = client.mouseX - (client.gameGraphics.gameWidth - 199);
            int l8 = client.mouseY - 36;
            if (l >= 40 && l8 >= 0 && l < 196 && l8 < 152)
            {
                int c2 = 156;//'\u234';
                int c4 = 152;//'\u230';
                int k1 = 192 + client.minimapRandomRotationY;
                int i2 = client.cameraRotation + client.minimapRandomRotationX & 0xff;
                int i1 = client.gameGraphics.gameWidth - 199;
                i1 += 40;
                int k3 = (client.mouseX - (i1 + c2 / 2)) * 16384 / (3 * k1);
                int i5 = (client.mouseY - (36 + c4 / 2)) * 16384 / (3 * k1);
                int k5 = Camera.trigonometryTable[1024 - i2 * 4 & 0x3ff];
                int i6 = Camera.trigonometryTable[(1024 - i2 * 4 & 0x3ff) + 1024];
                int k7 = i5 * k5 + k3 * i6 >> 15;
                i5 = i5 * i6 - k3 * k5 >> 15;
                k3 = k7;
                k3 += client.ourPlayer.currentX;
                i5 = client.ourPlayer.currentY - i5;
                if (client.mouseButtonClick == 1)
                {
                    client.WalkTo1Tile(client.sectionX, client.sectionY, k3 / 128, i5 / 128, false);
                }

                client.mouseButtonClick = 0;
            }
        }
        public void DrawWelcomeBox()
        {
            int l = 65;
            if (client.lastLoginAddress != "0.0.0.0")
            {
                l += 30;
            }

            if (client.subDaysLeft > 0)
            {
                l += 15;
            }

            if (client.lastLoginDays >= 0)
            {
                l += 15;
            }

            int i1 = 167 - l / 2;
            client.gameGraphics.DrawBox(56, 167 - l / 2, 400, l, 0);
            client.gameGraphics.DrawBoxEdge(56, 167 - l / 2, 400, l, 0xffffff);
            i1 += 20;
            client.gameGraphics.DrawText("Welcome to RuneScape " + client.loginUsername, 256, i1, 4, 0xffff00);
            i1 += 30;
            string s1;
            // lastLoginDays    subDaysLeft    lastLoginAddress
            if (client.lastLoginDays == 0)
            {
                s1 = "earlier today";
            }
            else
                if (client.lastLoginDays == 1)
            {
                s1 = "yesterday";
            }
            else
            {
                s1 = client.lastLoginDays + " days ago";
            }

            if (client.lastLoginAddress != "0.0.0.0")
            {
                client.gameGraphics.DrawText("You last logged in " + s1, 256, i1, 1, 0xffffff);
                i1 += 15;
                client.gameGraphics.DrawText("from: " + client.lastLoginAddress, 256, i1, 1, 0xffffff);
                i1 += 15;
            }
            if (client.subDaysLeft > 0)
            {
                client.gameGraphics.DrawText("Subscription left: " + client.subDaysLeft + " days", 256, i1, 1, 0xffffff);
                i1 += 15;
            }
            /*if(unreadMessages > 0) {
                int j1 = 0xffffff;
                client.gameGraphics.DrawText("Jagex staff will NEVER email you. We use the", 256, i1, 1, j1);
                i1 += 15;
                client.gameGraphics.DrawText("message-centre on this website instead.", 256, i1, 1, j1);
                i1 += 15;
                if(unreadMessages == 1)
                    client.gameGraphics.DrawText("You have @yel@0@whi@ unread messages in your message-centre", 256, i1, 1, 0xffffff);
                else
                    client.gameGraphics.DrawText("You have @gre@" + (unreadMessages - 1) + " unread messages @whi@in your message-centre", 256, i1, 1, 0xffffff);
                i1 += 15;
                i1 += 15;
            }
            if(lastChangedRecoveryDays != 201) {
                if(lastChangedRecoveryDays == 200) {
                    client.gameGraphics.DrawText("You have not yet set any password recovery questions.", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    client.gameGraphics.DrawText("We strongly recommend you do so now to secure your account.", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    client.gameGraphics.DrawText("Do this from the 'account management' area on our front webpage", 256, i1, 1, 0xff8000);
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
                    client.gameGraphics.DrawText(s2 + " you changed your recovery questions", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    client.gameGraphics.DrawText("If you do not remember making this change then cancel it immediately", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    client.gameGraphics.DrawText("Do this from the 'account management' area on our front webpage", 256, i1, 1, 0xff8000);
                    i1 += 15;
                }
                i1 += 15;
            }*/
            int k1 = 0xffffff;
            if (client.mouseY > i1 - 12 && client.mouseY <= i1 && client.mouseX > 106 && client.mouseX < 406)
            {
                k1 = 0xff0000;
            }

            client.gameGraphics.DrawText("Click here to close window", 256, i1, 1, k1);
            if (client.mouseButtonClick == 1)
            {
                if (k1 == 0xff0000)
                {
                    client.showWelcomeBox = false;
                }

                if ((client.mouseX < 86 || client.mouseX > 426) && (client.mouseY < 167 - l / 2 || client.mouseY > 167 + l / 2))
                {
                    client.showWelcomeBox = false;
                }
            }
            client.mouseButtonClick = 0;
        }
        public void DrawOptionsMenu(bool canClick)
        {
            int l = client.gameGraphics.gameWidth - 199;
            int i1 = 36;
            client.gameGraphics.DrawPicture(l - 49, 3, client.baseInventoryPic + 6);
            int c1 = 196;
            client.gameGraphics.DrawBoxAlpha(l, 36, c1, 62, GameImage.RgbToInt(181, 181, 181), 160);
            client.gameGraphics.DrawBoxAlpha(l, 98, c1, 92, GameImage.RgbToInt(201, 201, 201), 160);
            client.gameGraphics.DrawBoxAlpha(l, 190, c1, 90, GameImage.RgbToInt(181, 181, 181), 160);
            client.gameGraphics.DrawBoxAlpha(l, 280, c1, 40, GameImage.RgbToInt(201, 201, 201), 160);
            int j1 = l + 3;
            int l1 = i1 + 15;
            client.gameGraphics.DrawString("Game options - click to toggle", j1, l1, 1, 0);
            l1 += 15;
            if (client.configCameraAutoAngle)
            {
                client.gameGraphics.DrawString("Camera angle mode - @gre@Auto", j1, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Camera angle mode - @red@Manual", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (client.configOneMouseButton)
            {
                client.gameGraphics.DrawString("Mouse buttons - @red@One", j1, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Mouse buttons - @gre@Two", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (Config.MembersFeatures)
            {
                if (client.configSoundOff)
                {
                    client.gameGraphics.DrawString("Sound effects - @red@off", j1, l1, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString("Sound effects - @gre@on", j1, l1, 1, 0xffffff);
                }
            }

            l1 += 15;
            client.gameGraphics.DrawString("Client assists - click to toggle", j1, l1, 1, 0);
            l1 += 15;
            if (client.showRoofs)
            {
                client.gameGraphics.DrawString("Roofs - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Roofs - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (client.showCombatWindow)
            {
                client.gameGraphics.DrawString("Fight mode window - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Fight mode window - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;

            if (client.fogOfWar)
            {
                client.gameGraphics.DrawString("Fog of war - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Fog of war - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (client.autoScreenshot)
            {
                client.gameGraphics.DrawString("Automatic screenshots - @gre@on", j1, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Automatic screenshots - @red@off", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (client.useChatFilter)
            {
                client.gameGraphics.DrawString("Chat filter: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Chat filter: @red@<off>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            client.gameGraphics.DrawString("Privacy settings. Will be applied to", j1, l1, 1, 0);
            l1 += 15;
            client.gameGraphics.DrawString("all people not on your friends list", j1, l1, 1, 0);
            l1 += 15;
            if (client.blockChat == 0)
            {
                client.gameGraphics.DrawString("Block chat messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Block chat messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (client.blockPrivate == 0)
            {
                client.gameGraphics.DrawString("Block public messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Block public messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (client.blockTrade == 0)
            {
                client.gameGraphics.DrawString("Block trade requests: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Block trade requests: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (Config.MembersFeatures)
            {
                if (client.blockDuel == 0)
                {
                    client.gameGraphics.DrawString("Block duel requests: @red@<off>", l + 3, l1, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString("Block duel requests: @gre@<on>", l + 3, l1, 1, 0xffffff);
                }
            }

            l1 += 15;
            l1 += 5;
            client.gameGraphics.DrawString("Always logout when you finish", j1, l1, 1, 0);
            l1 += 15;
            int j2 = 0xffffff;
            if (client.mouseX > j1 && client.mouseX < j1 + c1 && client.mouseY > l1 - 12 && client.mouseY < l1 + 4)
            {
                j2 = 0xffff00;
            }

            client.gameGraphics.DrawString("Click here to logout", l + 3, l1, 1, j2);
            if (!canClick)
            {
                return;
            }

            l = client.mouseX - (client.gameGraphics.gameWidth - 199);
            i1 = client.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 280)
            {
                int k2 = client.gameGraphics.gameWidth - 199;
                sbyte byte0 = 36;
                int c2 = 196;
                int k1 = k2 + 3;
                int i2 = byte0 + 30;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.configCameraAutoAngle = !client.configCameraAutoAngle;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(0);
                    int configCameraAutoAngleByte = 0;
                    if (client.configCameraAutoAngle)
                    {
                        configCameraAutoAngleByte = 1;
                    }
                    client.streamClass.AddByte(configCameraAutoAngleByte);
                    client.streamClass.FormatPacket();
                }
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.configOneMouseButton = !client.configOneMouseButton;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(2);
                    int configOneMouseButtonByte = 0;
                    if (client.configOneMouseButton)
                    {
                        configOneMouseButtonByte = 1;
                    }
                    client.streamClass.AddByte(configOneMouseButtonByte);
                    client.streamClass.FormatPacket();
                }
                i2 += 15;
                if (Config.MembersFeatures && client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.configSoundOff = !client.configSoundOff;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(3);
                    int configSoundOffByte = 0;
                    if (client.configSoundOff)
                    {
                        configSoundOffByte = 1;
                    }
                    client.streamClass.AddByte(configSoundOffByte);
                    client.streamClass.FormatPacket();
                }
                i2 += 15;
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.showRoofs = !client.showRoofs;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(4);
                    int showRoofsByte = 0;
                    if (client.showRoofs)
                    {
                        showRoofsByte = 1;
                    }
                    client.streamClass.AddByte(showRoofsByte);
                    client.streamClass.FormatPacket();
                }
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.showCombatWindow = !client.showCombatWindow;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(6);
                    int showCombatWindowByte = 0;
                    if (client.showCombatWindow)
                    {
                        showCombatWindowByte = 1;
                    }
                    client.streamClass.AddByte(showCombatWindowByte);
                    client.streamClass.FormatPacket();
                }
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.fogOfWar = !client.fogOfWar;
                }
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.autoScreenshot = !client.autoScreenshot;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(5);
                    int autoScreenshotByte = 0;
                    if (client.autoScreenshot)
                    {
                        autoScreenshotByte = 1;
                    }
                    client.streamClass.AddByte(autoScreenshotByte);
                    client.streamClass.FormatPacket();
                }
                bool flag = false;
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.useChatFilter = !client.useChatFilter;
                }
                i2 += 15;
                i2 += 15;
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.blockChat = 1 - client.blockChat;
                    flag = true;
                }
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.blockPrivate = 1 - client.blockPrivate;
                    flag = true;
                }
                i2 += 15;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.blockTrade = 1 - client.blockTrade;
                    flag = true;
                }
                i2 += 15;
                if (Config.MembersFeatures && client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.blockDuel = 1 - client.blockDuel;
                    flag = true;
                }
                i2 += 15;
                if (flag)
                {
                    client.CallSendUpdatedPrivacyInfo(client.blockChat, client.blockPrivate, client.blockTrade, client.blockDuel);
                }

                i2 += 20;
                if (client.mouseX > k1 && client.mouseX < k1 + c2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseButtonClick == 1)
                {
                    client.SendLogout();
                }

                client.mouseButtonClick = 0;
            }
        }
        public void DrawCombatStyleBox()
        {
            sbyte byte0 = 7;
            sbyte byte1 = 15;
            int c1 = 175; ;//'\u257';
            if (client.mouseButtonClick != 0)
            {
                for (int l = 0; l < 5; l += 1)
                {
                    if (l <= 0 || client.mouseX <= byte0 || client.mouseX >= byte0 + c1 || client.mouseY <= byte1 + l * 20 || client.mouseY >= byte1 + l * 20 + 20)
                    {
                        continue;
                    }

                    client.combatStyle = l - 1;
                    client.mouseButtonClick = 0;
                    client.streamClass.CreatePacket(42);
                    client.streamClass.AddByte(client.combatStyle);
                    client.streamClass.FormatPacket();
                    break;
                }

            }
            for (int i1 = 0; i1 < 5; i1 += 1)
            {
                if (i1 == client.combatStyle + 1)
                {
                    client.gameGraphics.DrawBoxAlpha(byte0, byte1 + i1 * 20, c1, 20, GameImage.RgbToInt(255, 0, 0), 128);
                }
                else
                {
                    client.gameGraphics.DrawBoxAlpha(byte0, byte1 + i1 * 20, c1, 20, GameImage.RgbToInt(190, 190, 190), 128);
                }

                client.gameGraphics.DrawLineX(byte0, byte1 + i1 * 20, c1, 0);
                client.gameGraphics.DrawLineX(byte0, byte1 + i1 * 20 + 20, c1, 0);
            }

            client.gameGraphics.DrawText("Select combat style", byte0 + c1 / 2, byte1 + 16, 3, 0xffffff);
            client.gameGraphics.DrawText("Controlled (+1 of each)", byte0 + c1 / 2, byte1 + 36, 3, 0);
            client.gameGraphics.DrawText("Aggressive (+3 strength)", byte0 + c1 / 2, byte1 + 56, 3, 0);
            client.gameGraphics.DrawText("Accurate   (+3 attack)", byte0 + c1 / 2, byte1 + 76, 3, 0);
            client.gameGraphics.DrawText("Defensive  (+3 defense)", byte0 + c1 / 2, byte1 + 96, 3, 0);
        }
        public void DrawTradeBox()
        {
            if (client.mouseButtonClick != 0)
            {
                int mx = client.mouseX - 22;
                int my = client.mouseY - 36;
                if (mx >= 0 && my >= 30 && mx < 462 && my < 262)
                {
                    if (mx > 216 && my > 30 && mx < 462 && my < 235)
                    {
                        int curItem = (mx - 217) / 49 + (my - 31) / 34 * 5;
                        if (curItem >= 0 && curItem < client.inventoryItemsCount)
                        {
                            int item = client.inventoryItems[curItem];
                            client.mouseClickedHeldInTradeDuelBox = 1;
                            bool ourTradeItemsChanged = false;
                            int someInt = 0;
                            for (int tradeItem = 0; tradeItem < client.tradeItemsOurCount; tradeItem += 1)
                            {
                                if (client.tradeItemsOur[tradeItem] == item)
                                {
                                    if (client.entityManager.GetItem(item).IsStackable == 0)
                                    {
                                        for (int i = 0; i < client.mouseClickedHeldInTradeDuelBox; i += 1)
                                        {
                                            if (client.tradeItemOurCount[tradeItem] < client.inventoryItemCount[curItem])
                                            {
                                                client.tradeItemOurCount[tradeItem] += 1;
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

                            if (client.GetInventoryItemTotalCount(item) <= someInt)
                            {
                                ourTradeItemsChanged = true;
                            }

                            if (client.entityManager.GetItem(item).IsSpecial == 1)
                            {
                                client.DisplayMessage("This object cannot be traded with other players", 3);
                                ourTradeItemsChanged = true;
                            }
                            if (!ourTradeItemsChanged && client.tradeItemsOurCount < 12)
                            {
                                client.tradeItemsOur[client.tradeItemsOurCount] = item;
                                client.tradeItemOurCount[client.tradeItemsOurCount] = 1;
                                client.tradeItemsOurCount += 1;
                                ourTradeItemsChanged = true;
                            }
                            if (ourTradeItemsChanged)
                            {
                                client.streamClass.CreatePacket(70);
                                client.streamClass.AddByte(client.tradeItemsOurCount);
                                for (int i = 0; i < client.tradeItemsOurCount; i += 1)
                                {
                                    client.streamClass.AddShort(client.tradeItemsOur[i]);
                                    client.streamClass.AddInt(client.tradeItemOurCount[i]);
                                }
                                client.streamClass.FormatPacket();
                                client.tradeOtherAccepted = false;
                                client.tradeWeAccepted = false;
                            }
                        }
                    }
                    else if (mx > 8 && my > 30 && mx < 205 && my < 133)
                    {
                        int curItem = (mx - 9) / 49 + (my - 31) / 34 * 4;
                        if (curItem >= 0 && curItem < client.tradeItemsOurCount)
                        {
                            int item = client.tradeItemsOur[curItem];
                            for (int i = 0; i < client.mouseClickedHeldInTradeDuelBox; i += 1)
                            {
                                if (client.entityManager.GetItem(item).IsStackable == 0 && client.tradeItemOurCount[curItem] > 1)
                                {
                                    client.tradeItemOurCount[curItem] -= 1;
                                    continue;
                                }
                                client.tradeItemsOurCount -= 1;
                                client.mouseButtonHeldTime = 0;
                                for (int j = curItem; j < client.tradeItemsOurCount; j += 1)
                                {
                                    client.tradeItemsOur[j] = client.tradeItemsOur[j + 1];
                                    client.tradeItemOurCount[j] = client.tradeItemOurCount[j + 1];
                                }
                                break;
                            }
                            client.streamClass.CreatePacket(70);
                            client.streamClass.AddByte(client.tradeItemsOurCount);
                            for (int i = 0; i < client.tradeItemsOurCount; i += 1)
                            {
                                client.streamClass.AddShort(client.tradeItemsOur[i]);
                                client.streamClass.AddInt(client.tradeItemOurCount[i]);
                            }
                            client.streamClass.FormatPacket();
                            client.tradeOtherAccepted = false;
                            client.tradeWeAccepted = false;
                        }
                    }
                    if (mx >= 217 && my >= 238 && mx <= 286 && my <= 259)
                    {
                        client.tradeWeAccepted = true;
                        client.streamClass.CreatePacket(211);
                        client.streamClass.FormatPacket();
                    }
                    if (mx >= 394 && my >= 238 && mx < 463 && my < 259)
                    {
                        client.showTradeBox = false;
                        client.streamClass.CreatePacket(216);
                        client.streamClass.FormatPacket();
                    }
                }
                else
                {
                    //showTradeBox = false;
                    //base.streamClass.CreatePacket(216);
                    //base.streamClass.FormatPacket();
                }
                client.mouseButtonClick = 0;
                client.mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!client.showTradeBox)
            {
                return;
            }

            sbyte byte0 = 22;
            sbyte byte1 = 36;
            client.gameGraphics.DrawBox(byte0, byte1, 468, 12, 192);
            int l1 = 0x989898;
            client.gameGraphics.DrawBoxAlpha(byte0, byte1 + 12, 468, 18, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0, byte1 + 30, 8, 248, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 205, byte1 + 30, 11, 248, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 462, byte1 + 30, 6, 248, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 133, 197, 22, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 258, 197, 20, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 235, 246, 43, l1, 160);
            int j2 = 0xd0d0d0;
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 30, 197, 103, j2, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 155, 197, 103, j2, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 30, 246, 205, j2, 160);
            for (int i3 = 0; i3 < 4; i3 += 1)
            {
                client.gameGraphics.DrawLineX(byte0 + 8, byte1 + 30 + i3 * 34, 197, 0);
            }

            for (int i4 = 0; i4 < 4; i4 += 1)
            {
                client.gameGraphics.DrawLineX(byte0 + 8, byte1 + 155 + i4 * 34, 197, 0);
            }

            for (int k4 = 0; k4 < 7; k4 += 1)
            {
                client.gameGraphics.DrawLineX(byte0 + 216, byte1 + 30 + k4 * 34, 246, 0);
            }

            for (int j5 = 0; j5 < 6; j5 += 1)
            {
                if (j5 < 5)
                {
                    client.gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 30, 103, 0);
                }

                if (j5 < 5)
                {
                    client.gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 155, 103, 0);
                }

                client.gameGraphics.DrawLineY(byte0 + 216 + j5 * 49, byte1 + 30, 205, 0);
            }

            client.gameGraphics.DrawString("Trading with: " + client.tradeOtherName, byte0 + 1, byte1 + 10, 1, 0xffffff);
            client.gameGraphics.DrawString("Your Offer", byte0 + 9, byte1 + 27, 4, 0xffffff);
            client.gameGraphics.DrawString("Opponent's Offer", byte0 + 9, byte1 + 152, 4, 0xffffff);
            client.gameGraphics.DrawString("Your Inventory", byte0 + 216, byte1 + 27, 4, 0xffffff);
            if (!client.tradeWeAccepted)
            {
                client.gameGraphics.DrawPicture(byte0 + 217, byte1 + 238, client.baseInventoryPic + 25);
            }

            client.gameGraphics.DrawPicture(byte0 + 394, byte1 + 238, client.baseInventoryPic + 26);
            if (client.tradeOtherAccepted)
            {
                client.gameGraphics.DrawText("Other player", byte0 + 341, byte1 + 246, 1, 0xffffff);
                client.gameGraphics.DrawText("has accepted", byte0 + 341, byte1 + 256, 1, 0xffffff);
            }
            if (client.tradeWeAccepted)
            {
                client.gameGraphics.DrawText("Waiting for", byte0 + 217 + 35, byte1 + 246, 1, 0xffffff);
                client.gameGraphics.DrawText("other player", byte0 + 217 + 35, byte1 + 256, 1, 0xffffff);
            }
            for (int k5 = 0; k5 < client.inventoryItemsCount; k5 += 1)
            {
                int l5 = 217 + byte0 + k5 % 5 * 49;
                int j6 = 31 + byte1 + k5 / 5 * 34;
                client.gameGraphics.DrawImage(l5, j6, 48, 32, client.baseItemPicture + client.entityManager.GetItem(client.inventoryItems[k5]).InventoryPicture, client.entityManager.GetItem(client.inventoryItems[k5]).PictureMask, 0, 0, false);
                if (client.entityManager.GetItem(client.inventoryItems[k5]).IsStackable == 0)
                {
                    client.gameGraphics.DrawString(client.inventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < client.tradeItemsOurCount; i6 += 1)
            {
                int k6 = 9 + byte0 + i6 % 4 * 49;
                int i7 = 31 + byte1 + i6 / 4 * 34;
                RecordItemSprite(k6, i7, 48, 32, client.entityManager.GetItem(client.tradeItemsOur[i6]));
                if (client.entityManager.GetItem(client.tradeItemsOur[i6]).IsStackable == 0)
                {
                    client.gameGraphics.DrawString(client.tradeItemOurCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (client.mouseX > k6 && client.mouseX < k6 + 48 && client.mouseY > i7 && client.mouseY < i7 + 32)
                {
                    Item tradeOurItem = client.entityManager.GetItem(client.tradeItemsOur[i6]);
                    client.gameGraphics.DrawString(tradeOurItem.Name + ": @whi@" + tradeOurItem.Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < client.tradeItemsOtherCount; l6 += 1)
            {
                int j7 = 9 + byte0 + l6 % 4 * 49;
                int k7 = 156 + byte1 + l6 / 4 * 34;
                RecordItemSprite(j7, k7, 48, 32, client.entityManager.GetItem(client.tradeItemsOther[l6]));
                if (client.entityManager.GetItem(client.tradeItemsOther[l6]).IsStackable == 0)
                {
                    client.gameGraphics.DrawString(client.tradeItemOtherCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (client.mouseX > j7 && client.mouseX < j7 + 48 && client.mouseY > k7 && client.mouseY < k7 + 32)
                {
                    Item tradeOtherItem = client.entityManager.GetItem(client.tradeItemsOther[l6]);
                    client.gameGraphics.DrawString(tradeOtherItem.Name + ": @whi@" + tradeOtherItem.Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

        }
        public void DrawLogoutBox()
        {
            client.gameGraphics.DrawBox(126, 137, 260, 60, 0);
            client.gameGraphics.DrawBoxEdge(126, 137, 260, 60, 0xffffff);
            client.gameGraphics.DrawText("Logging out...", 256, 173, 5, 0xffffff);
        }
        public void LoginScreenPrint(string s1, string s2)
        {
            if (client.loginScreen == 2 && client.loginMenuLogin is not null)
            {
                client.loginMenuLogin.UpdateText(client.loginMenuStatusText, s1 + " " + s2);
            }

            DrawLoginScreens();
            client.ResetTimings();
        }
        public void DrawTeleBubble(int x, int y, int j1, int k1, int l1, int i2, int j2)
        {
            int type = client.teleBubbleType[l1];
            int time = client.teleBubbleTime[l1];
            if (type == 0)
            {
                int i3 = 255 + time * 5 * 256;
                client.gameGraphics.DrawCircle(x + j1 / 2, y + k1 / 2, 20 + time * 2, i3, 255 - time * 5);
            }
            if (type == 1)
            {
                int j3 = 0xff0000 + time * 5 * 256;
                client.gameGraphics.DrawCircle(x + j1 / 2, y + k1 / 2, 10 + time, j3, 255 - time * 5);
            }
        }
        public void DrawWindow()
        {
            client.Paint(GameClient.graphics);

            if (client.errorLoading)
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
                client.SetRefreshRate(1);
                return;
            }
            if (client.memoryError)
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
                if (!client.loggedIn)
                {
                    client.gameGraphics.loggedIn = false;
                    DrawLoginScreens();

                }
                if (client.loggedIn)
                {
                    client.gameGraphics.loggedIn = true;
                    DrawGame();

                    return;
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"[Paint EXCEPTION] {_ex.GetType().Name}: {_ex.Message}");
                Console.WriteLine(_ex.StackTrace);
                client.CleanUp();
                client.memoryError = true;
            }
        }
        public void DrawQuestionMenu()
        {
            if (client.mouseButtonClick != 0)
            {
                for (int l = 0; l < client.questionMenuCount; l += 1)
                {
                    if (client.mouseX >= client.gameGraphics.TextWidth(client.questionMenuAnswer[l], 1) || client.mouseY <= l * 12 || client.mouseY >= 12 + l * 12)
                    {
                        continue;
                    }

                    client.streamClass.CreatePacket(154);
                    client.streamClass.AddByte(l);
                    client.streamClass.FormatPacket();
                    break;
                }

                client.mouseButtonClick = 0;
                client.showQuestionMenu = false;
                return;
            }
            for (int i1 = 0; i1 < client.questionMenuCount; i1 += 1)
            {
                int j1 = 65535;
                if (client.mouseX < client.gameGraphics.TextWidth(client.questionMenuAnswer[i1], 1) && client.mouseY > i1 * 12 && client.mouseY < 12 + i1 * 12)
                {
                    j1 = 0xff0000;
                }

                client.gameGraphics.DrawString(client.questionMenuAnswer[i1], 6, 12 + i1 * 12, 1, j1);
            }

        }
        public void DrawTradeConfirmBox()
        {
            sbyte byte0 = 22;
            sbyte byte1 = 36;
            client.gameGraphics.DrawBox(byte0, byte1, 468, 16, 192);
            int l = 0x989898;
            client.gameGraphics.DrawBoxAlpha(byte0, byte1 + 16, 468, 246, l, 160);
            client.gameGraphics.DrawText("Please confirm your trade with @yel@" + DataOperations.HashToName(client.tradeConfirmOtherNameLong), byte0 + 234, byte1 + 12, 1, 0xffffff);
            client.gameGraphics.DrawText("You are about to give:", byte0 + 117, byte1 + 30, 1, 0xffff00);
            for (int i1 = 0; i1 < client.tradeConfigItemCount; i1 += 1)
            {
                Item tradeConfirmItem = client.entityManager.GetItem(client.tradeConfirmItems[i1]);
                string s1 = tradeConfirmItem.Name;
                if (tradeConfirmItem.IsStackable == 0)
                {
                    s1 = s1 + " x " + GameClientUtilities.FormatItemCount(client.tradeConfigItemsCount[i1]);
                }

                client.gameGraphics.DrawText(s1, byte0 + 117, byte1 + 42 + i1 * 12, 1, 0xffffff);
            }

            if (client.tradeConfigItemCount == 0)
            {
                client.gameGraphics.DrawText("Nothing!", byte0 + 117, byte1 + 42, 1, 0xffffff);
            }

            client.gameGraphics.DrawText("In return you will receive:", byte0 + 351, byte1 + 30, 1, 0xffff00);
            for (int j1 = 0; j1 < client.tradeConfirmOtherItemCount; j1 += 1)
            {
                Item tradeConfirmOtherItem = client.entityManager.GetItem(client.tradeConfirmOtherItems[j1]);
                string s2 = tradeConfirmOtherItem.Name;
                if (tradeConfirmOtherItem.IsStackable == 0)
                {
                    s2 = s2 + " x " + GameClientUtilities.FormatItemCount(client.tradeConfirmOtherItemsCount[j1]);
                }

                client.gameGraphics.DrawText(s2, byte0 + 351, byte1 + 42 + j1 * 12, 1, 0xffffff);
            }

            if (client.tradeConfirmOtherItemCount == 0)
            {
                client.gameGraphics.DrawText("Nothing!", byte0 + 351, byte1 + 42, 1, 0xffffff);
            }

            client.gameGraphics.DrawText("Are you sure you want to do this?", byte0 + 234, byte1 + 200, 4, 65535);
            client.gameGraphics.DrawText("There is NO WAY to reverse a trade if you change your mind.", byte0 + 234, byte1 + 215, 1, 0xffffff);
            client.gameGraphics.DrawText("Remember that not all players are trustworthy", byte0 + 234, byte1 + 230, 1, 0xffffff);
            if (!client.tradeConfirmAccepted)
            {
                client.gameGraphics.DrawPicture(byte0 + 118 - 35, byte1 + 238, client.baseInventoryPic + 25);
                client.gameGraphics.DrawPicture(byte0 + 352 - 35, byte1 + 238, client.baseInventoryPic + 26);
            }
            else
            {
                client.gameGraphics.DrawText("Waiting for other player...", byte0 + 234, byte1 + 250, 1, 0xffff00);
            }
            if (client.mouseButtonClick == 1)
            {
                if (client.mouseX < byte0 || client.mouseY < byte1 || client.mouseX > byte0 + 468 || client.mouseY > byte1 + 262)
                {
                    //showTradeConfirmBox = false;
                    //base.streamClass.CreatePacket(216);
                    //base.streamClass.FormatPacket();
                }
                if (client.mouseX >= byte0 + 118 - 35 && client.mouseX <= byte0 + 118 + 70 && client.mouseY >= byte1 + 238 && client.mouseY <= byte1 + 238 + 21)
                {
                    client.tradeConfirmAccepted = true;
                    client.streamClass.CreatePacket(53);
                    client.streamClass.FormatPacket();
                }
                if (client.mouseX >= byte0 + 352 - 35 && client.mouseX <= byte0 + 353 + 70 && client.mouseY >= byte1 + 238 && client.mouseY <= byte1 + 238 + 21)
                {
                    client.showTradeConfirmBox = false;
                    client.streamClass.CreatePacket(216);
                    client.streamClass.FormatPacket();
                }
                client.mouseButtonClick = 0;
            }
        }
        public void DrawLoginScreens()
        {
            client.loginScreenShown = false;
            if (client.gameGraphics is null)
            {
                return;
            }

            client.gameGraphics.interlace = false;
            client.gameGraphics.ClearScreen();
            if (client.loginScreen == 0 || client.loginScreen == 1 || client.loginScreen == 2 || client.loginScreen == 3)
            {
                int l = client.tick * 2 % 3072;
                if (l < 1024)
                {
                    client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic);
                    if (l > 768)
                    {
                        client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic + 1, l - 768);
                    }
                }
                else if (l < 2048)
                {
                    client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic + 1);
                    if (l > 1792)
                    {
                        client.gameGraphics.DrawPicture(0, 10, client.baseInventoryPic + 10, l - 1792);
                    }
                }
                else
                {
                    client.gameGraphics.DrawPicture(0, 10, client.baseInventoryPic + 10);
                    if (l > 2816)
                    {
                        client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic, l - 2816);
                    }
                }
            }
            if (client.loginMenuFirst is null)
            {
                return;
            }

            if (client.loginScreen == 0)
            {
                client.loginMenuFirst.DrawMenu();
            }

            if (client.loginScreen == 1)
            {
                client.loginNewUser.DrawMenu();
            }

            if (client.loginScreen == 2)
            {
                client.loginMenuLogin.DrawMenu();
            }

            client.gameGraphics.DrawPicture(0, client.windowHeight, client.baseInventoryPic + 22);

            //gameGraphics.UpdateGameImage();
            client.OnDrawDone();//client.gameGraphics.DrawImage(spriteBatch, 0, 0);
        }
        public void DrawItem(int x, int y, int width, int height, int itemID, int i2, int j2)
        {
            int picture = client.entityManager.GetItem(itemID).InventoryPicture + client.baseItemPicture;
            int mask = client.entityManager.GetItem(itemID).PictureMask;
            client.gameGraphics.DrawImage(x, y, width, height, picture, mask, 0, 0, false);
        }
        public void DrawFriendsMenu(bool canClick)
        {
            int l = client.gameGraphics.gameWidth - 199;
            int i1 = 36;
            client.gameGraphics.DrawPicture(l - 49, 3, client.baseInventoryPic + 5);
            int c1 = 196;//(char)304;//'\u304';
            int c2 = 182;//(char)266;//'\u266';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (client.friendsIgnoreMenuSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            client.gameGraphics.DrawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            client.gameGraphics.DrawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            client.gameGraphics.DrawBoxAlpha(l, i1 + 24, c1, c2 - 24, GameImage.RgbToInt(220, 220, 220), 128);
            client.gameGraphics.DrawLineX(l, i1 + 24, c1, 0);
            client.gameGraphics.DrawLineY(l + c1 / 2, i1, 24, 0);
            client.gameGraphics.DrawLineX(l, i1 + c2 - 16, c1, 0);
            client.gameGraphics.DrawText("Friends", l + c1 / 4, i1 + 16, 4, 0);
            client.gameGraphics.DrawText("Ignore", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            client.friendsMenu.ClearList(client.friendsMenuHandle);
            if (client.friendsIgnoreMenuSelected == 0)
            {
                for (int l1 = 0; l1 < client.friendsCount; l1 += 1)
                {
                    string s1;
                    if (client.friendsWorld[l1] == 99)
                    {
                        s1 = "@gre@";
                    }
                    else
                        if (client.friendsWorld[l1] > 0)
                    {
                        s1 = "@yel@";
                    }
                    else
                    {
                        s1 = "@red@";
                    }

                    client.friendsMenu.AddListItem(client.friendsMenuHandle, l1, s1 + DataOperations.HashToName(client.friendsList[l1]) + "~439~@whi@Remove         WWWWWWWWWW");
                }

            }
            if (client.friendsIgnoreMenuSelected == 1)
            {
                for (int i2 = 0; i2 < client.ignoresCount; i2 += 1)
                {
                    client.friendsMenu.AddListItem(client.friendsMenuHandle, i2, "@yel@" + DataOperations.HashToName(client.ignoresList[i2]) + "~439~@whi@Remove         WWWWWWWWWW");
                }
            }
            client.friendsMenu.DrawMenu();
            if (client.friendsIgnoreMenuSelected == 0)
            {
                int j2 = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);
                if (j2 >= 0 && client.mouseX < 489)
                {
                    if (client.mouseX > 429)
                    {
                        client.gameGraphics.DrawText("Click to remove " + DataOperations.HashToName(client.friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                        if (client.friendsWorld[j2] == 99)
                    {
                        client.gameGraphics.DrawText("Click to message " + DataOperations.HashToName(client.friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                            if (client.friendsWorld[j2] > 0)
                    {
                        client.gameGraphics.DrawText(DataOperations.HashToName(client.friendsList[j2]) + " is on world " + client.friendsWorld[j2], l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                    {
                        client.gameGraphics.DrawText(DataOperations.HashToName(client.friendsList[j2]) + " is offline", l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    client.gameGraphics.DrawText("Click a name to send a message", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int j3;
                if (client.mouseX > l && client.mouseX < l + c1 && client.mouseY > i1 + c2 - 16 && client.mouseY < i1 + c2)
                {
                    j3 = 0xffff00;
                }
                else
                {
                    j3 = 0xffffff;
                }

                client.gameGraphics.DrawText("Click here to add a friend", l + c1 / 2, i1 + c2 - 3, 1, j3);
            }
            if (client.friendsIgnoreMenuSelected == 1)
            {
                int k2 = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);
                if (k2 >= 0 && client.mouseX < 489 && client.mouseX > 429)
                {
                    if (client.mouseX > 429)
                    {
                        client.gameGraphics.DrawText("Click to remove " + DataOperations.HashToName(client.ignoresList[k2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    client.gameGraphics.DrawText("Blocking messages from:", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int k3;
                if (client.mouseX > l && client.mouseX < l + c1 && client.mouseY > i1 + c2 - 16 && client.mouseY < i1 + c2)
                {
                    k3 = 0xffff00;
                }
                else
                {
                    k3 = 0xffffff;
                }

                client.gameGraphics.DrawText("Click here to add a name", l + c1 / 2, i1 + c2 - 3, 1, k3);
            }
            if (!canClick)
            {
                return;
            }

            l = client.mouseX - (client.gameGraphics.gameWidth - 199);
            i1 = client.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                client.friendsMenu.MouseClick(l + (client.gameGraphics.gameWidth - 199), i1 + 36, client.lastMouseButton, client.mouseButton);
                if (i1 <= 24 && client.mouseButtonClick == 1)
                {
                    if (l < 98 && client.friendsIgnoreMenuSelected == 1)
                    {
                        client.friendsIgnoreMenuSelected = 0;
                        client.friendsMenu.SwitchList(client.friendsMenuHandle);
                    }
                    else
                        if (l > 98 && client.friendsIgnoreMenuSelected == 0)
                        {
                            client.friendsIgnoreMenuSelected = 1;
                            client.friendsMenu.SwitchList(client.friendsMenuHandle);
                        }
                }

                if (client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 0)
                {
                    int l2 = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);
                    if (l2 >= 0 && client.mouseX < 489)
                    {
                        if (client.mouseX > 429)
                        {
                            client.CallRemoveFriend(client.friendsList[l2]);
                        }
                        else
                            if (client.friendsWorld[l2] != 0)
                            {
                                client.showFriendsBox = 2;
                                client.pmTarget = client.friendsList[l2];
                                client.pmText = "";
                                client.enteredPMText = "";
                            }
                    }
                }
                if (client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 1)
                {
                    int i3 = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);
                    if (i3 >= 0 && client.mouseX < 489 && client.mouseX > 429)
                    {
                        client.CallRemoveIgnore(client.ignoresList[i3]);
                    }
                }
                if (i1 > 166 && client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 0)
                {
                    client.showFriendsBox = 1;
                    client.inputText = "";
                    client.enteredInputText = "";
                }
                if (i1 > 166 && client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 1)
                {
                    client.showFriendsBox = 3;
                    client.inputText = "";
                    client.enteredInputText = "";
                }
                client.mouseButtonClick = 0;
            }
        }
        public void DrawPrayerMagicMenu(bool canClick)
        {
            int l = client.gameGraphics.gameWidth - 199;
            int i1 = 36;
            client.gameGraphics.DrawPicture(l - 49, 3, client.baseInventoryPic + 4);
            int c1 = 196;//'\u304';
            int c2 = 182;//'\u266';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (client.menuMagicPrayersSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            client.gameGraphics.DrawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            client.gameGraphics.DrawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            client.gameGraphics.DrawBoxAlpha(l, i1 + 24, c1, 90, GameImage.RgbToInt(220, 220, 220), 128);
            client.gameGraphics.DrawBoxAlpha(l, i1 + 24 + 90, c1, c2 - 90 - 24, GameImage.RgbToInt(160, 160, 160), 128);
            client.gameGraphics.DrawLineX(l, i1 + 24, c1, 0);
            client.gameGraphics.DrawLineY(l + c1 / 2, i1, 24, 0);
            client.gameGraphics.DrawLineX(l, i1 + 113, c1, 0);
            client.gameGraphics.DrawText("Magic", l + c1 / 4, i1 + 16, 4, 0);
            client.gameGraphics.DrawText("Prayers", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);

            if (client.menuMagicPrayersSelected == 0)
            {
                client.spellMenu.ClearList(client.spellMenuHandle);
                int l1 = 0;

                for (int l2 = 0; l2 < client.entityManager.SpellCount; l2 += 1)
                {
                    string s1 = "@yel@";
                    for (int k4 = 0; k4 < client.entityManager.GetSpell(l2).RuneCount; k4 += 1)
                    {
                        int j5 = client.entityManager.GetSpell(l2).RequiredRunesIds[k4];
                        if (client.HasRequiredRunes(j5, client.entityManager.GetSpell(l2).RequiredRunesCounts[k4]))
                        {
                            continue;
                        }

                        s1 = "@whi@";
                        break;
                    }

                    int k5 = client.playerStatCurrent[6];
                    if (client.entityManager.GetSpell(l2).RequiredLevel > k5)
                    {
                        s1 = "@normalZ@";
                    }

                    client.spellMenu.AddListItem(client.spellMenuHandle, l1++, s1 + "Level " + client.entityManager.GetSpell(l2).RequiredLevel + ": " + client.entityManager.GetSpell(l2).Name);
                }

                client.spellMenu.DrawMenu();
                int l3 = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);
                if (l3 != -1)
                {
                    client.gameGraphics.DrawString("Level " + client.entityManager.GetSpell(l3).RequiredLevel + ": " + client.entityManager.GetSpell(l3).Name, l + 2, i1 + 124, 1, 0xffff00);
                    client.gameGraphics.DrawString(client.entityManager.GetSpell(l3).Description, l + 2, i1 + 136, 0, 0xffffff);
                    for (int l4 = 0; l4 < client.entityManager.GetSpell(l3).RuneCount; l4 += 1)
                    {
                        int l5 = client.entityManager.GetSpell(l3).RequiredRunesIds[l4];
                        RecordItemSprite(l + 2 + l4 * 44, i1 + 150, 48, 32, client.entityManager.GetItem(l5));
                        int i6 = client.GetInventoryItemTotalCount(l5);
                        int j6 = client.entityManager.GetSpell(l3).RequiredRunesCounts[l4];
                        string s3 = "@red@";
                        if (client.HasRequiredRunes(l5, j6))
                        {
                            s3 = "@gre@";
                        }

                        client.gameGraphics.DrawString(s3 + i6 + "/" + j6, l + 2 + l4 * 44, i1 + 150, 1, 0xffffff);
                    }

                }
                else
                {
                    client.gameGraphics.DrawString("Point at a spell for a description", l + 2, i1 + 124, 1, 0);
                }
            }
            if (client.menuMagicPrayersSelected == 1)
            {
                client.spellMenu.ClearList(client.spellMenuHandle);
                int i2 = 0;
                for (int i3 = 0; i3 < client.entityManager.PrayerCount; i3 += 1)
                {
                    Prayer prayer = client.entityManager.GetPrayer(i3);
                    string s2 = "@whi@";
                    if (prayer.RequiredLevel > client.playerStatBase[5])
                    {
                        s2 = "@normalZ@";
                    }

                    if (client.prayerOn[i3])
                    {
                        s2 = "@gre@";
                    }

                    client.spellMenu.AddListItem(client.spellMenuHandle, i2++, s2 + "Level " + prayer.RequiredLevel + ": " + prayer.Name);
                }

                client.spellMenu.DrawMenu();
                int i4 = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);
                if (i4 != -1)
                {
                    Prayer highlightedPrayer = client.entityManager.GetPrayer(i4);
                    client.gameGraphics.DrawText("Level " + highlightedPrayer.RequiredLevel + ": " + highlightedPrayer.Name, l + c1 / 2, i1 + 130, 1, 0xffff00);
                    client.gameGraphics.DrawText(highlightedPrayer.Description, l + c1 / 2, i1 + 145, 0, 0xffffff);
                    client.gameGraphics.DrawText("Drain rate: " + highlightedPrayer.DrainRate, l + c1 / 2, i1 + 160, 1, 0);
                }
                else
                {
                    client.gameGraphics.DrawString("Point at a prayer for a description", l + 2, i1 + 124, 1, 0);
                }
            }
            if (!canClick)
            {
                return;
            }

            l = client.mouseX - (client.gameGraphics.gameWidth - 199);
            i1 = client.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                client.spellMenu.MouseClick(l + (client.gameGraphics.gameWidth - 199), i1 + 36, client.lastMouseButton, client.mouseButton);
                if (i1 <= 24 && client.mouseButtonClick == 1)
                {
                    if (l < 98 && client.menuMagicPrayersSelected == 1)
                    {
                        client.menuMagicPrayersSelected = 0;
                        client.spellMenu.SwitchList(client.spellMenuHandle);
                    }
                    else
                        if (l > 98 && client.menuMagicPrayersSelected == 0)
                        {
                            client.menuMagicPrayersSelected = 1;
                            client.spellMenu.SwitchList(client.spellMenuHandle);
                        }
                }

                if (client.mouseButtonClick == 1 && client.menuMagicPrayersSelected == 0)
                {
                    int j2 = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);
                    if (j2 != -1)
                    {
                        int j3 = client.playerStatCurrent[6];
                        if (client.entityManager.GetSpell(j2).RequiredLevel > j3)
                        {
                            client.DisplayMessage("Your magic ability is not high enough for this spell", 3);
                        }
                        else
                        {
                            int j4;
                            for (j4 = 0; j4 < client.entityManager.GetSpell(j2).RuneCount; j4 += 1)
                            {
                                int i5 = client.entityManager.GetSpell(j2).RequiredRunesIds[j4];
                                if (client.HasRequiredRunes(i5, client.entityManager.GetSpell(j2).RequiredRunesCounts[j4]))
                                {
                                    continue;
                                }

                                client.DisplayMessage("You don't have all the reagents you need for this spell", 3);
                                j4 = -1;
                                break;
                            }

                            if (j4 == client.entityManager.GetSpell(j2).RuneCount)
                            {
                                client.selectedSpell = j2;
                                client.selectedItem = -1;
                            }
                        }
                    }
                }
                if (client.mouseButtonClick == 1 && client.menuMagicPrayersSelected == 1)
                {
                    int k2 = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);
                    if (k2 != -1)
                    {
                        int k3 = client.playerStatBase[5];
                        if (client.entityManager.GetPrayer(k2).RequiredLevel > k3)
                        {
                            client.DisplayMessage("Your prayer ability is not high enough for this prayer", 3);
                        }
                        else
                            if (client.playerStatCurrent[5] == 0)
                        {
                            client.DisplayMessage("You have Run out of prayer points. Return to a church to recharge", 3);
                        }
                        else
                                if (client.prayerOn[k2])
                                {
                                    client.streamClass.CreatePacket(248);
                                    client.streamClass.AddByte(k2);
                                    client.streamClass.FormatPacket();
                                    client.prayerOn[k2] = false;
                                    client.PlaySound("prayeroff");
                                }
                                else
                                {
                                    client.streamClass.CreatePacket(56);
                                    client.streamClass.AddByte(k2);
                                    client.streamClass.FormatPacket();
                                    client.prayerOn[k2] = true;
                                    client.PlaySound("prayeron");
                                }
                    }
                }
                client.mouseButtonClick = 0;
            }
        }
        public void DrawChatMessageTabs()
        {
            client.gameGraphics.DrawPicture(0, client.windowHeight - 4, client.baseInventoryPic + 23);
            int l = GameImage.RgbToInt(200, 200, 255);
            if (client.messagesTab == 0)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabAllMsgFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText("All messages", 54, client.windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (client.messagesTab == 1)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabHistoryFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText("Chat history", 155, client.windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (client.messagesTab == 2)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabQuestFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText("Quest history", 255, client.windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (client.messagesTab == 3)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabPrivateFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText("Private history", 355, client.windowHeight + 6, 0, l);
            client.gameGraphics.DrawText("Report abuse", 457, client.windowHeight + 6, 0, 0xffffff);
        }
        public void DrawShopBox()
        {
            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;
                int l = client.mouseX - 52;
                int i1 = client.mouseY - 44;
                if (l >= 0 && i1 >= 12 && l < 408 && i1 < 246)
                {
                    int j1 = 0;
                    for (int l1 = 0; l1 < 5; l1 += 1)
                    {
                        for (int l2 = 0; l2 < 8; l2 += 1)
                        {
                            int k3 = 7 + l2 * 49;
                            int k4 = 28 + l1 * 34;
                            if (l > k3 && l < k3 + 49 && i1 > k4 && i1 < k4 + 34 && client.shopItems[j1] != -1)
                            {
                                client.selectedShopItemIndex = j1;
                                client.selectedShopItemType = client.shopItems[j1];
                            }
                            j1 += 1;
                        }

                    }

                    if (client.selectedShopItemIndex >= 0)
                    {
                        int i3 = client.shopItems[client.selectedShopItemIndex];
                        if (i3 != -1)
                        {
                            if (client.shopItemCount[client.selectedShopItemIndex] > 0 && l > 298 && i1 >= 204 && l < 408 && i1 <= 215)
                            {
                                client.streamClass.CreatePacket(128);
                                client.streamClass.AddShort(client.shopItems[client.selectedShopItemIndex]);
                                client.streamClass.AddInt(client.shopItemBuyPrice[client.selectedShopItemIndex]);
                                client.streamClass.FormatPacket();
                            }
                            if (client.GetInventoryItemTotalCount(i3) > 0 && l > 2 && i1 >= 229 && l < 112 && i1 <= 240)
                            {
                                client.streamClass.CreatePacket(255);
                                client.streamClass.AddShort(client.shopItems[client.selectedShopItemIndex]);
                                client.streamClass.AddInt(client.shopItemSellPrice[client.selectedShopItemIndex]);
                                client.streamClass.FormatPacket();
                            }
                        }
                    }
                }
                else
                {
                    client.streamClass.CreatePacket(253);
                    client.streamClass.FormatPacket();
                    client.showShopBox = false;
                    return;
                }
            }
            sbyte _offsetX = 52;
            sbyte _offsetY = 44;
            client.gameGraphics.DrawBox(_offsetX, _offsetY, 408, 12, 192);
            int k1 = 0x989898;
            client.gameGraphics.DrawBoxAlpha(_offsetX, _offsetY + 12, 408, 17, k1, 160);
            client.gameGraphics.DrawBoxAlpha(_offsetX, _offsetY + 29, 8, 170, k1, 160);
            client.gameGraphics.DrawBoxAlpha(_offsetX + 399, _offsetY + 29, 9, 170, k1, 160);
            client.gameGraphics.DrawBoxAlpha(_offsetX, _offsetY + 199, 408, 47, k1, 160);
            client.gameGraphics.DrawString("Buying and selling items", _offsetX + 1, _offsetY + 10, 1, 0xffffff);
            int i2 = 0xffffff;

            if (client.mouseX > _offsetX + 320 && client.mouseY >= _offsetY && client.mouseX < _offsetX + 408 && client.mouseY < _offsetY + 12)
            {
                i2 = 0xff0000;
            }

            client.gameGraphics.DrawLabel("Close window", _offsetX + 406, _offsetY + 10, 1, i2);
            client.gameGraphics.DrawString("Shops stock in green", _offsetX + 2, _offsetY + 24, 1, 65280);
            client.gameGraphics.DrawString("Number you own in blue", _offsetX + 135, _offsetY + 24, 1, 65535);
            client.gameGraphics.DrawString("Your money: " + client.GetInventoryItemTotalCount(10) + "gp", _offsetX + 280, _offsetY + 24, 1, 0xffff00);
            int j3 = 0xd0d0d0;
            int j4 = 0;
            for (int boxRow = 0; boxRow < 5; boxRow += 1)
            {
                for (int boxRowColumn = 0; boxRowColumn < 8; boxRowColumn += 1)
                {
                    int i6 = _offsetX + 7 + boxRowColumn * 49;
                    int l6 = _offsetY + 28 + boxRow * 34;
                    if (client.selectedShopItemIndex == j4)
                    {
                        client.gameGraphics.DrawBoxAlpha(i6, l6, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        client.gameGraphics.DrawBoxAlpha(i6, l6, 49, 34, j3, 160);
                    }

                    client.gameGraphics.DrawBoxEdge(i6, l6, 50, 35, 0);
                    if (client.shopItems[j4] != -1)
                    {
                        RecordItemSprite(i6, l6, 48, 32, client.entityManager.GetItem(client.shopItems[j4]));
                        client.gameGraphics.DrawString(client.shopItemCount[j4].ToString(), i6 + 1, l6 + 10, 1, 65280);
                        client.gameGraphics.DrawLabel(client.GetInventoryItemTotalCount(client.shopItems[j4]).ToString(), i6 + 47, l6 + 10, 1, 65535);
                    }
                    j4 += 1;
                }

            }

            client.gameGraphics.DrawLineX(_offsetX + 5, _offsetY + 222, 398, 0);
            if (client.selectedShopItemIndex == -1)
            {
                client.gameGraphics.DrawText("Select an object to buy or sell", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                return;
            }
            int l5 = client.shopItems[client.selectedShopItemIndex];
            if (l5 != -1)
            {
                if (client.shopItemCount[client.selectedShopItemIndex] > 0)
                {
                    int j6 = client.shopItemBuyPriceModifier + client.shopItemBasePriceModifier[client.selectedShopItemIndex];
                    if (j6 < 10)
                    {
                        j6 = 10;
                    }

                    int i7 = j6 * client.entityManager.GetItem(l5).BasePrice / 100;
                    client.gameGraphics.DrawString("Buy a new " + client.entityManager.GetItem(l5).Name + " for " + i7 + "gp", _offsetX + 2, _offsetY + 214, 1, 0xffff00);
                    int j2 = 0xffffff;
                    if (client.mouseX > _offsetX + 298 && client.mouseY >= _offsetY + 204 && client.mouseX < _offsetX + 408 && client.mouseY <= _offsetY + 215)
                    {
                        j2 = 0xff0000;
                    }

                    client.gameGraphics.DrawLabel("Click here to buy", _offsetX + 405, _offsetY + 214, 3, j2);
                }
                else
                {
                    client.gameGraphics.DrawText("This item is not currently available to buy", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                }
                if (client.GetInventoryItemTotalCount(l5) > 0)
                {
                    int k6 = client.shopItemSellPriceModifier + client.shopItemBasePriceModifier[client.selectedShopItemIndex];
                    if (k6 < 10)
                    {
                        k6 = 10;
                    }

                    int j7 = k6 * client.entityManager.GetItem(l5).BasePrice / 100;
                    client.gameGraphics.DrawLabel("Sell your " + client.entityManager.GetItem(l5).Name + " for " + j7 + "gp", _offsetX + 405, _offsetY + 239, 1, 0xffff00);
                    int k2 = 0xffffff;
                    if (client.mouseX > _offsetX + 2 && client.mouseY >= _offsetY + 229 && client.mouseX < _offsetX + 112 && client.mouseY <= _offsetY + 240)
                    {
                        k2 = 0xff0000;
                    }

                    client.gameGraphics.DrawString("Click here to sell", _offsetX + 2, _offsetY + 239, 3, k2);
                    return;
                }
                client.gameGraphics.DrawText("You do not have any of this item to sell", _offsetX + 204, _offsetY + 239, 3, 0xffff00);
            }
        }
        public void DrawAppearanceWindow()
        {
            client.gameGraphics.interlace = false;
            client.gameGraphics.ClearScreen();
            client.appearanceMenu.DrawMenu();
            int l = 140;
            int i1 = 50;
            l += 116;
            i1 -= 25;
            client.gameGraphics.DrawCharacterLegs(l - 32 - 55, i1, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).Number, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).Number, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).Number, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawCharacterLegs(l - 32, i1, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).Number + 6, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(l - 32, i1, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).Number + 6, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(l - 32, i1, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).Number + 6, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawCharacterLegs(l - 32 + 55, i1, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).Number + 12, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(l - 32 + 55, i1, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).Number + 12, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(l - 32 + 55, i1, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).Number + 12, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawPicture(0, client.windowHeight, client.baseInventoryPic + 22);
            //gameGraphics.UpdateGameImage();
            client.OnDrawDone();//client.gameGraphics.DrawImage(spriteBatch, 0, 0);
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
                client.gameGraphics.ScreenFadeToBlack();
                client.gameGraphics.DrawText("Oh dear! You are dead...", client.windowWidth / 2, client.windowHeight / 2, 7, 0xff0000);
                client.DrawChatMessageTabs();
                //gameGraphics.UpdateGameImage();
                client.OnDrawDone();//client.gameGraphics.DrawImage(spriteBatch, 0, 0);
                return;
            }
            if (client.showAppearanceWindow)
            {
                DrawAppearanceWindow();
                return;
            }
            if (client.isSleeping)
            {
                client.gameGraphics.ScreenFadeToBlack();
                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    client.gameGraphics.DrawText("ZZZ", (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    client.gameGraphics.DrawText("ZZZ", 512 - (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                client.gameGraphics.DrawBox(client.windowWidth / 2 - 100, 160, 200, 40, 0);
                client.gameGraphics.DrawText("You are sleeping", client.windowWidth / 2, 50, 7, 0xffff00);
                client.gameGraphics.DrawText("Fatigue: " + client.fatigue * 100 / 750 + "%", client.windowWidth / 2, 90, 7, 0xffff00);
                client.gameGraphics.DrawText("When you want to wake up just use your", client.windowWidth / 2, 140, 5, 0xffffff);
                client.gameGraphics.DrawText("keyboard to type the word in the box below", client.windowWidth / 2, 160, 5, 0xffffff);
                client.gameGraphics.DrawText(client.inputText + "*", client.windowWidth / 2, 180, 5, 65535);
                if (client.sleepingStatusText is null)
                {
                    client.gameGraphics.DrawPixels(client.captchaPixels, client.windowWidth / 2 - 127, 230, client.captchaWidth, client.captchaHeight);
                }
                else
                {
                    client.gameGraphics.DrawText(client.sleepingStatusText, client.windowWidth / 2, 260, 5, 0xff0000);
                }

                client.gameGraphics.DrawBoxEdge(client.windowWidth / 2 - 128, 229, 257, 42, 0xffffff);
                client.DrawChatMessageTabs();
                client.gameGraphics.DrawText("If you can't read the word", client.windowWidth / 2, 290, 1, 0xffffff);
                client.gameGraphics.DrawText("@yel@click here@whi@ to get a different one", client.windowWidth / 2, 305, 1, 0xffffff);

                //gameGraphics.UpdateGameImage();
                client.OnDrawDone();//client.gameGraphics.DrawImage(spriteBatch, 0, 0);
                return;
            }
            if (!client.engineHandle.playerIsAlive)
            {
                return;
            }

            for (int l = 0; l < 64; l += 1)
            {
                client.gameCamera.RemoveModel(client.engineHandle.roofObject[client.lastLayerIndex][l]);
                if (client.lastLayerIndex == 0)
                {
                    client.gameCamera.RemoveModel(client.engineHandle.wallObject[1][l]);
                    client.gameCamera.RemoveModel(client.engineHandle.roofObject[1][l]);
                    client.gameCamera.RemoveModel(client.engineHandle.wallObject[2][l]);
                    client.gameCamera.RemoveModel(client.engineHandle.roofObject[2][l]);
                }
                client.cameraZoom = true;
                if (client.lastLayerIndex == 0 && (client.engineHandle.tiles[client.ourPlayer.currentX / 128][client.ourPlayer.currentY / 128] & 0x80) == 0)
                {
                    if (client.showRoofs)
                    {
                        client.gameCamera.AddModel(client.engineHandle.roofObject[client.lastLayerIndex][l]);
                        if (client.lastLayerIndex == 0)
                        {
                            // draw wall object at lv 1 / second layer
                            client.gameCamera.AddModel(client.engineHandle.wallObject[1][l]);
                            // draw roof object at lv 1 / second layer
                            GameObject roof1 = client.engineHandle.roofObject[1][l];
                            client.gameCamera.AddModel(roof1);

                            // draw wall object at lv 2 / third layer
                            client.gameCamera.AddModel(client.engineHandle.wallObject[2][l]);

                            // draw roof object at lv 2 / third layer
                            GameObject roof2 = client.engineHandle.roofObject[2][l];
                            client.gameCamera.AddModel(client.engineHandle.roofObject[2][l]);
                        }
                    }
                    client.cameraZoom = false;
                }
            }

            if (client.modelFireLightningSpellNumber != client.lastModelFireLightningSpellNumber)
            {
                client.lastModelFireLightningSpellNumber = client.modelFireLightningSpellNumber;
                for (int i1 = 0; i1 < client.objectCount; i1 += 1)
                {
                    if (client.objectType[i1] == 97)
                    {
                        DrawModel(i1, "firea" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[i1] == 274)
                    {
                        DrawModel(i1, "fireplacea" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[i1] == 1031)
                    {
                        DrawModel(i1, "lightning" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[i1] == 1036)
                    {
                        DrawModel(i1, "firespell" + (client.modelFireLightningSpellNumber + 1));
                    }

                    if (client.objectType[i1] == 1147)
                    {
                        DrawModel(i1, "spellcharge" + (client.modelFireLightningSpellNumber + 1));
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
                        DrawModel(j1, "torcha" + (client.modelTorchNumber + 1));
                    }

                    if (client.objectType[j1] == 143)
                    {
                        DrawModel(j1, "skulltorcha" + (client.modelTorchNumber + 1));
                    }
                }

            }
            if (client.modelClawSpellNumber != client.lastModelClawSpellNumber)
            {
                client.lastModelClawSpellNumber = client.modelClawSpellNumber;
                for (int k1 = 0; k1 < client.objectCount; k1 += 1)
                {
                    if (client.objectType[k1] == 1142)
                    {
                        DrawModel(k1, "clawspell" + (client.modelClawSpellNumber + 1));
                    }
                }
            }
            client.gameCamera.RemoveLastUpdates(client.drawUpdatesPerformed);
            client.drawUpdatesPerformed = 0;
            for (int l1 = 0; l1 < client.playerCount; l1 += 1)
            {
                ClientMob player = client.playerArray[l1];
                if (player.bottomColour != 255)
                {
                    int j2 = player.currentX;
                    int l2 = player.currentY;
                    int j3 = -client.engineHandle.GetAveragedElevation(j2, l2);
                    int k4 = client.gameCamera.AddSpriteToScene(5000 + l1, j2, j3, l2, 145, 220, l1 + 10000);
                    client.drawUpdatesPerformed += 1;
                    if (player == client.ourPlayer)
                    {
                        client.gameCamera.RemoveSprite(k4);
                    }

                    if (player.currentSprite == 8)
                    {
                        client.gameCamera.UpdateSpritePosition(k4, -30);
                    }

                    if (player.currentSprite == 9)
                    {
                        client.gameCamera.UpdateSpritePosition(k4, 30);
                    }
                }
            }

            for (int i2 = 0; i2 < client.playerCount; i2 += 1)
            {
                ClientMob player = client.playerArray[i2];
                if (player.projectileDistance > 0)
                {
                    ClientMob targetMob = null;
                    if (player.attackingNpcIndex != -1)
                    {
                        targetMob = client.npcAttackingArray[player.attackingNpcIndex];
                    }
                    else if (player.attackingPlayerIndex != -1)
                    {
                        targetMob = client.playerBufferArray[player.attackingPlayerIndex];
                    }

                    if (targetMob is not null)
                    {
                        int k3 = player.currentX;
                        int l4 = player.currentY;
                        int k7 = -client.engineHandle.GetAveragedElevation(k3, l4) - 110;
                        int k9 = targetMob.currentX;
                        int j10 = targetMob.currentY;
                        int k10 = -client.engineHandle.GetAveragedElevation(k9, j10) - client.entityManager.GetNpc(targetMob.npcId).Camera2 / 2;
                        int l10 = (k3 * player.projectileDistance + k9 * (client.projectileRange - player.projectileDistance)) / client.projectileRange;
                        int i11 = (k7 * player.projectileDistance + k10 * (client.projectileRange - player.projectileDistance)) / client.projectileRange;
                        int j11 = (l4 * player.projectileDistance + j10 * (client.projectileRange - player.projectileDistance)) / client.projectileRange;
                        client.gameCamera.AddSpriteToScene(client.baseProjectilePic + player.projectileType, l10, i11, j11, 32, 32, 0);
                        client.drawUpdatesPerformed += 1;
                    }
                }
            }

            for (int k2 = 0; k2 < client.npcCount; k2 += 1)
            {
                ClientMob npc = client.npcArray[k2];
                int x1 = npc.currentX;
                int z1 = npc.currentY;
                int y1 = -client.engineHandle.GetAveragedElevation(x1, z1);
                int l9 = client.gameCamera.AddSpriteToScene(20000 + k2, x1, y1, z1, client.entityManager.GetNpc(npc.npcId).Camera1, client.entityManager.GetNpc(npc.npcId).Camera2, k2 + 30000);
                client.drawUpdatesPerformed += 1;
                if (npc.currentSprite == 8)
                {
                    client.gameCamera.UpdateSpritePosition(l9, -30);
                }

                if (npc.currentSprite == 9)
                {
                    client.gameCamera.UpdateSpritePosition(l9, 30);
                }
            }

            for (int i3 = 0; i3 < client.groundItemCount; i3 += 1)
            {
                int x = client.groundItemX[i3] * client.gridSize + 64;
                int y = client.groundItemY[i3] * client.gridSize + 64;
                client.gameCamera.AddSpriteToScene(40000 + client.groundItemID[i3], x, -client.engineHandle.GetAveragedElevation(x, y) - client.groundItemObjectVar[i3], y, 96, 64, i3 + 20000);
                client.drawUpdatesPerformed += 1;
            }

            for (int j4 = 0; j4 < client.teleBubbleCount; j4 += 1)
            {
                int k5 = client.teleBubbleX[j4] * client.gridSize + 64;
                int i8 = client.teleBubbleY[j4] * client.gridSize + 64;
                int i10 = client.teleBubbleType[j4];
                if (i10 == 0)
                {
                    client.gameCamera.AddSpriteToScene(50000 + j4, k5, -client.engineHandle.GetAveragedElevation(k5, i8), i8, 128, 256, j4 + 50000);
                    client.drawUpdatesPerformed += 1;
                }
                if (i10 == 1)
                {
                    client.gameCamera.AddSpriteToScene(50000 + j4, k5, -client.engineHandle.GetAveragedElevation(k5, i8), i8, 128, 64, j4 + 50000);
                    client.drawUpdatesPerformed += 1;
                }
            }

            client.gameGraphics.interlace = false;
            client.gameGraphics.ClearScreen();
            client.gameGraphics.interlace = client.keyF1Toggle;
            if (client.lastLayerIndex == 3)
            {
                int l5 = 40 + (int)(Helper.Random.NextDouble() * 3D);
                int j8 = 40 + (int)(Helper.Random.NextDouble() * 7D);
                client.gameCamera.SetAllModelColours(l5, j8, -50, -10, -50);
            }
            client.itemsAboveHeadCount = 0;
            client.receivedMessagesCount = 0;
            client.healthBarVisibleCount = 0;
            if (client.cameraAutoAngleDebug)
            {
                if (client.configCameraAutoAngle && !client.cameraZoom)
                {
                    int i6 = client.cameraAutoAngle;
                    client.AutoRotateCamera();
                    if (client.cameraAutoAngle != i6)
                    {
                        client.cameraAutoRotatePlayerX = client.ourPlayer.currentX;
                        client.cameraAutoRotatePlayerY = client.ourPlayer.currentY;
                    }
                }
                if (client.fogOfWar)
                {
                    client.gameCamera.zoom1 = 3000;
                    client.gameCamera.zoom2 = 3000;
                    client.gameCamera.zoom3 = 1;
                    client.gameCamera.zoom4 = 2800;
                }
                else
                {
                    client.gameCamera.zoom1 = 40000;
                    client.gameCamera.zoom2 = 40000;
                    client.gameCamera.zoom3 = 40000;
                    client.gameCamera.zoom4 = 40000;
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
                        client.gameCamera.zoom1 = 2400;
                        client.gameCamera.zoom2 = 2400;
                        client.gameCamera.zoom3 = 1;
                        client.gameCamera.zoom4 = 2300;
                    }
                    else
                    {
                        client.gameCamera.zoom1 = 2200;
                        client.gameCamera.zoom2 = 2200;
                        client.gameCamera.zoom3 = 1;
                        client.gameCamera.zoom4 = 2100;
                    }
                }
                else
                {
                    client.gameCamera.zoom1 = 40000;
                    client.gameCamera.zoom2 = 40000;
                    client.gameCamera.zoom3 = 40000;
                    client.gameCamera.zoom4 = 40000;
                }
                int k6 = client.cameraAutoRotatePlayerX + client.cameraRotationXAmount;
                int l8 = client.cameraAutoRotatePlayerY + client.cameraRotationYAmount;
                client.gameCamera.SetCameraTransform(k6, -client.engineHandle.GetAveragedElevation(k6, l8), l8, 912, client.cameraRotation * 4, 0, client.cameraDistance * 2);
            }
            client.gameCamera.FinishCamera();
            DrawAboveHeadThings();
            if (client.actionPictureType > 0)
            {
                client.gameGraphics.DrawPicture(client.walkMouseX - 8, client.walkMouseY - 8, client.baseInventoryPic + 14 + (24 - client.actionPictureType) / 6);
            }

            if (client.actionPictureType < 0)
            {
                client.gameGraphics.DrawPicture(client.walkMouseX - 8, client.walkMouseY - 8, client.baseInventoryPic + 18 + (24 + client.actionPictureType) / 6);
            }

            if (client.systemUpdate != 0)
            {
                int seconds = client.systemUpdate / 50;
                int minutes = seconds / 60;
                seconds %= 60;
                if (seconds < 10)
                {
                    client.gameGraphics.DrawText("System update in: " + minutes + ":0" + seconds, 256, client.windowHeight - 7, 1, 0xffff00);
                }
                else
                {
                    client.gameGraphics.DrawText("System update in: " + minutes + ":" + seconds, 256, client.windowHeight - 7, 1, 0xffff00);
                }
            }
            if (!client.loadArea)
            {
                int i7 = 2203 - (client.sectionY + client.wildY + client.areaY);
                if (client.sectionX + client.wildX + client.areaX >= 2640)
                {
                    i7 = -50;
                }

                if (i7 > 0)
                {
                    int j9 = 1 + i7 / 6;
                    client.gameGraphics.DrawPicture(453, client.windowHeight - 56, client.baseInventoryPic + 13);
                    client.gameGraphics.DrawText("Wilderness", 465, client.windowHeight - 20, 1, 0xffff00);
                    client.gameGraphics.DrawText("Level: " + j9, 465, client.windowHeight - 7, 1, 0xffff00);
                    if (client.wildType == 0)
                    {
                        client.wildType = 2;
                    }
                }
                if (client.wildType == 0 && i7 > -10 && i7 <= 0)
                {
                    client.wildType = 1;
                }
            }
            if (client.messagesTab == 0)
            {
                for (int j7 = 0; j7 < 5; j7 += 1)
                {
                    if (client.messagesTimeout[j7] > 0)
                    {
                        string s1 = client.messagesArray[j7];
                        client.gameGraphics.DrawString(s1, 7, client.windowHeight - 18 - j7 * 12, 1, 0xffff00);
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
            client.gameGraphics.DrawPicture(client.gameGraphics.gameWidth - 3 - 197, 3, client.baseInventoryPic, 128);

#warning play with this! Create a new menu of choice :)

            DrawMenus();

            client.gameGraphics.loggedIn = false;
            client.DrawChatMessageTabs();

            string text = "Coordinates: ( " + (client.sectionX + client.areaX) + "," + (client.sectionY + client.areaY) + " ) Section: (" + client.sectionX + "," + client.sectionY + ") Area: (" + client.areaX + "," + client.areaY + ")";
            // Text shadow
            client.gameGraphics.DrawString(text, 10 + 11, 10 + 11, 1, 0x000000);
            client.gameGraphics.DrawString(text, 10 + 10, 10 + 10, 1, 0xffffff);

            //gameGraphics.UpdateGameImage();
            client.OnDrawDone();//client.gameGraphics.DrawImage(spriteBatch, 0, 0);
        }
        public void DrawReportAbuseBox2()
        {
            if (client.enteredInputText.Length > 0)
            {
                string s1 = client.enteredInputText.Trim();
                client.inputText = "";
                client.enteredInputText = "";
                if (s1.Length > 0)
                {
                    long l1 = DataOperations.NameToHash(s1);
                    client.streamClass.CreatePacket(7);
                    client.streamClass.AddLong(l1);
                    client.streamClass.AddByte(client.reportAbuseOptionSelected);
                    //base.streamClass.AddByte(dia ? 1 : 0);
                    client.streamClass.FormatPacket();
                }
                client.showAbuseBox = 0;
                return;
            }
            client.gameGraphics.DrawBox(56, 130, 400, 100, 0);
            client.gameGraphics.DrawBoxEdge(56, 130, 400, 100, 0xffffff);
            int l = 160;
            client.gameGraphics.DrawText("Now type the name of the offending player, and press enter", 256, l, 1, 0xffff00);
            l += 18;
            client.gameGraphics.DrawText("Name: " + client.inputText + "*", 256, l, 4, 0xffffff);
            l = 222;
            int i1 = 0xffffff;
            if (client.mouseX > 196 && client.mouseX < 316 && client.mouseY > l - 13 && client.mouseY < l + 2)
            {
                i1 = 0xffff00;
                if (client.mouseButtonClick == 1)
                {
                    client.mouseButtonClick = 0;
                    client.showAbuseBox = 0;
                }
            }
            client.gameGraphics.DrawText("Click here to cancel", 256, l, 1, i1);
            if (client.mouseButtonClick == 1 && (client.mouseX < 56 || client.mouseX > 456 || client.mouseY < 130 || client.mouseY > 230))
            {
                client.mouseButtonClick = 0;
                client.showAbuseBox = 0;
            }
        }
        public void DrawMenus()
        {
            if (client.logoutTimer != 0)
            {
                DrawLogoutBox();
            }
            else if (client.showWelcomeBox)
            {
                DrawWelcomeBox();
            }
            else if (client.showServerMessageBox)
            {
                DrawServerMessageBox();
            }
            else if (client.wildType == 1)
            {
                DrawWildernessAlertBox();
            }
            else if (client.showBankBox && client.combatTimeout == 0)
            {
                DrawBankBox();
            }
            else if (client.showShopBox && client.combatTimeout == 0)
            {
                DrawShopBox();
            }
            else if (client.showTradeConfirmBox)
            {
                DrawTradeConfirmBox();
            }
            else if (client.showTradeBox)
            {
                DrawTradeBox();
            }
            else if (client.showDuelConfirmBox)
            {
                DrawDuelConfirmBox();
            }
            else if (client.showDuelBox)
            {
                DrawDuelBox();
            }
            else if (client.showAbuseBox == 1)
            {
                DrawReportAbuseBox1();
            }
            else if (client.showAbuseBox == 2)
            {
                DrawReportAbuseBox2();
            }
            else if (client.showFriendsBox != 0)
            {
                DrawFriendsBox();
            }
            else
            {
                if (client.showQuestionMenu)
                {
                    DrawQuestionMenu();
                }

                if (client.showCombatWindow || client.ourPlayer.currentSprite == 8 || client.ourPlayer.currentSprite == 9)
                {
                    DrawCombatStyleBox();
                }

                client.GetMenuHighlighted();
                bool flag = !client.showQuestionMenu && !client.menuShow;

                if (flag)
                {
                    client.menuOptionsCount = 0;
                }

                if (client.drawMenuTab == 0 && flag)
                {
                    client.GenerateWorldRightClickMenu();
                }

                if (client.drawMenuTab == 1)
                {
                    DrawInventoryMenu(flag);
                }

                if (client.drawMenuTab == 2)
                {
                    DrawMinimapMenu(flag);
                }

                if (client.drawMenuTab == 3)
                {
                    DrawStatsQuestsMenu(flag);
                }

                if (client.drawMenuTab == 4)
                {
                    DrawPrayerMagicMenu(flag);
                }

                if (client.drawMenuTab == 5)
                {
                    DrawFriendsMenu(flag);
                }

                if (client.drawMenuTab == 6)
                {
                    DrawOptionsMenu(flag);
                }

                if (!client.menuShow && !client.showQuestionMenu)
                {
                    client.CheckMouseStatus();
                }

                if (client.menuShow && !client.showQuestionMenu)
                {
                    DrawRightClickMenu();
                }
            }
            client.mouseButtonClick = 0;
        }
        public void DrawDuelBox()
        {
            if (client.mouseButtonClick != 0 && client.mouseClickedHeldInTradeDuelBox == 0)
            {
                client.mouseClickedHeldInTradeDuelBox = 1;
            }

            if (client.mouseClickedHeldInTradeDuelBox > 0)
            {
                int l = client.mouseX - 22;
                int i1 = client.mouseY - 36;
                if (l >= 0 && i1 >= 0 && l < 468 && i1 < 262)
                {
                    if (l > 216 && i1 > 30 && l < 462 && i1 < 235)
                    {
                        int j1 = (l - 217) / 49 + (i1 - 31) / 34 * 5;
                        if (j1 >= 0 && j1 < client.inventoryItemsCount)
                        {
                            bool flag1 = false;
                            int k2 = 0;
                            int j3 = client.inventoryItems[j1];
                            for (int j4 = 0; j4 < client.duelMyItemCount; j4 += 1)
                            {
                                if (client.duelMyItems[j4] == j3)
                                {
                                    if (client.entityManager.GetItem(j3).IsStackable == 0)
                                    {
                                        for (int l4 = 0; l4 < client.mouseClickedHeldInTradeDuelBox; l4 += 1)
                                        {
                                            if (client.duelMyItemsCount[j4] < client.inventoryItemCount[j1])
                                            {
                                                client.duelMyItemsCount[j4] += 1;
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

                            if (client.GetInventoryItemTotalCount(j3) <= k2)
                            {
                                flag1 = true;
                            }

                            if (client.entityManager.GetItem(j3).IsSpecial == 1)
                            {
                                client.DisplayMessage("This object cannot be added to a duel offer", 3);
                                flag1 = true;
                            }
                            if (!flag1 && client.duelMyItemCount < 8)
                            {
                                client.duelMyItems[client.duelMyItemCount] = j3;
                                client.duelMyItemsCount[client.duelMyItemCount] = 1;
                                client.duelMyItemCount += 1;
                                flag1 = true;
                            }
                            if (flag1)
                            {
                                client.streamClass.CreatePacket(123);
                                client.streamClass.AddByte(client.duelMyItemCount);
                                for (int i5 = 0; i5 < client.duelMyItemCount; i5 += 1)
                                {
                                    client.streamClass.AddShort(client.duelMyItems[i5]);
                                    client.streamClass.AddInt(client.duelMyItemsCount[i5]);
                                }

                                client.streamClass.FormatPacket();
                                client.duelOpponentAccepted = false;
                                client.duelMyAccepted = false;
                            }
                        }
                    }
                    if (l > 8 && i1 > 30 && l < 205 && i1 < 129)
                    {
                        int k1 = (l - 9) / 49 + (i1 - 31) / 34 * 4;
                        if (k1 >= 0 && k1 < client.duelMyItemCount)
                        {
                            int i2 = client.duelMyItems[k1];
                            for (int l2 = 0; l2 < client.mouseClickedHeldInTradeDuelBox; l2 += 1)
                            {
                                if (client.entityManager.GetItem(i2).IsStackable == 0 && client.duelMyItemsCount[k1] > 1)
                                {
                                    client.duelMyItemsCount[k1] -= 1;
                                    continue;
                                }
                                client.duelMyItemCount -= 1;
                                client.mouseButtonHeldTime = 0;
                                for (int k3 = k1; k3 < client.duelMyItemCount; k3 += 1)
                                {
                                    client.duelMyItems[k3] = client.duelMyItems[k3 + 1];
                                    client.duelMyItemsCount[k3] = client.duelMyItemsCount[k3 + 1];
                                }

                                break;
                            }

                            client.streamClass.CreatePacket(123);
                            client.streamClass.AddByte(client.duelMyItemCount);
                            for (int l3 = 0; l3 < client.duelMyItemCount; l3 += 1)
                            {
                                client.streamClass.AddShort(client.duelMyItems[l3]);
                                client.streamClass.AddInt(client.duelMyItemsCount[l3]);
                            }

                            client.streamClass.FormatPacket();
                            client.duelOpponentAccepted = false;
                            client.duelMyAccepted = false;
                        }
                    }
                    bool flag = false;
                    if (l >= 93 && i1 >= 221 && l <= 104 && i1 <= 232)
                    {
                        client.duelNoRetreating = !client.duelNoRetreating;
                        flag = true;
                    }
                    if (l >= 93 && i1 >= 240 && l <= 104 && i1 <= 251)
                    {
                        client.duelNoMagic = !client.duelNoMagic;
                        flag = true;
                    }
                    if (l >= 191 && i1 >= 221 && l <= 202 && i1 <= 232)
                    {
                        client.duelNoPrayer = !client.duelNoPrayer;
                        flag = true;
                    }
                    if (l >= 191 && i1 >= 240 && l <= 202 && i1 <= 251)
                    {
                        client.duelNoWeapons = !client.duelNoWeapons;
                        flag = true;
                    }
                    if (flag)
                    {
                        client.streamClass.CreatePacket(225);
                        int duelNoRetreatingByte = 0;
                        if (client.duelNoRetreating)
                        {
                            duelNoRetreatingByte = 1;
                        }
                        client.streamClass.AddByte(duelNoRetreatingByte);
                        int duelNoMagicByte = 0;
                        if (client.duelNoMagic)
                        {
                            duelNoMagicByte = 1;
                        }
                        client.streamClass.AddByte(duelNoMagicByte);
                        int duelNoPrayerByte = 0;
                        if (client.duelNoPrayer)
                        {
                            duelNoPrayerByte = 1;
                        }
                        client.streamClass.AddByte(duelNoPrayerByte);
                        int duelNoWeaponsByte = 0;
                        if (client.duelNoWeapons)
                        {
                            duelNoWeaponsByte = 1;
                        }
                        client.streamClass.AddByte(duelNoWeaponsByte);
                        client.streamClass.FormatPacket();
                        client.duelOpponentAccepted = false;
                        client.duelMyAccepted = false;
                    }
                    if (l >= 217 && i1 >= 238 && l <= 286 && i1 <= 259)
                    {
                        client.duelMyAccepted = true;
                        client.streamClass.CreatePacket(252);
                        client.streamClass.FormatPacket();
                    }
                    if (l >= 394 && i1 >= 238 && l < 463 && i1 < 259)
                    {
                        client.showDuelBox = false;
                        client.streamClass.CreatePacket(35);
                        client.streamClass.FormatPacket();
                    }
                }
                else
                    if (client.mouseButtonClick != 0)
                    {
                        client.showDuelBox = false;
                        client.streamClass.CreatePacket(35);
                        client.streamClass.FormatPacket();
                    }
                client.mouseButtonClick = 0;
                client.mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!client.showDuelBox)
            {
                return;
            }

            sbyte byte0 = 22;
            sbyte byte1 = 36;
            client.gameGraphics.DrawBox(byte0, byte1, 468, 12, 0xc90b1d);
            int l1 = 0x989898;
            client.gameGraphics.DrawBoxAlpha(byte0, byte1 + 12, 468, 18, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0, byte1 + 30, 8, 248, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 205, byte1 + 30, 11, 248, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 462, byte1 + 30, 6, 248, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 99, 197, 24, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 192, 197, 23, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 258, 197, 20, l1, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 235, 246, 43, l1, 160);
            int j2 = 0xd0d0d0;
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 30, 197, 69, j2, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 123, 197, 69, j2, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 8, byte1 + 215, 197, 43, j2, 160);
            client.gameGraphics.DrawBoxAlpha(byte0 + 216, byte1 + 30, 246, 205, j2, 160);
            for (int i3 = 0; i3 < 3; i3 += 1)
            {
                client.gameGraphics.DrawLineX(byte0 + 8, byte1 + 30 + i3 * 34, 197, 0);
            }

            for (int i4 = 0; i4 < 3; i4 += 1)
            {
                client.gameGraphics.DrawLineX(byte0 + 8, byte1 + 123 + i4 * 34, 197, 0);
            }

            for (int k4 = 0; k4 < 7; k4 += 1)
            {
                client.gameGraphics.DrawLineX(byte0 + 216, byte1 + 30 + k4 * 34, 246, 0);
            }

            for (int j5 = 0; j5 < 6; j5 += 1)
            {
                if (j5 < 5)
                {
                    client.gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 30, 69, 0);
                }

                if (j5 < 5)
                {
                    client.gameGraphics.DrawLineY(byte0 + 8 + j5 * 49, byte1 + 123, 69, 0);
                }

                client.gameGraphics.DrawLineY(byte0 + 216 + j5 * 49, byte1 + 30, 205, 0);
            }

            client.gameGraphics.DrawLineX(byte0 + 8, byte1 + 215, 197, 0);
            client.gameGraphics.DrawLineX(byte0 + 8, byte1 + 257, 197, 0);
            client.gameGraphics.DrawLineY(byte0 + 8, byte1 + 215, 43, 0);
            client.gameGraphics.DrawLineY(byte0 + 204, byte1 + 215, 43, 0);
            client.gameGraphics.DrawString("Preparing to duel with: " + client.duelOpponent, byte0 + 1, byte1 + 10, 1, 0xffffff);
            client.gameGraphics.DrawString("Your Stake", byte0 + 9, byte1 + 27, 4, 0xffffff);
            client.gameGraphics.DrawString("Opponent's Stake", byte0 + 9, byte1 + 120, 4, 0xffffff);
            client.gameGraphics.DrawString("Duel Options", byte0 + 9, byte1 + 212, 4, 0xffffff);
            client.gameGraphics.DrawString("Your Inventory", byte0 + 216, byte1 + 27, 4, 0xffffff);
            client.gameGraphics.DrawString("No retreating", byte0 + 8 + 1, byte1 + 215 + 16, 3, 0xffff00);
            client.gameGraphics.DrawString("No magic", byte0 + 8 + 1, byte1 + 215 + 35, 3, 0xffff00);
            client.gameGraphics.DrawString("No prayer", byte0 + 8 + 102, byte1 + 215 + 16, 3, 0xffff00);
            client.gameGraphics.DrawString("No weapons", byte0 + 8 + 102, byte1 + 215 + 35, 3, 0xffff00);
            client.gameGraphics.DrawBoxEdge(byte0 + 93, byte1 + 215 + 6, 11, 11, 0xffff00);
            if (client.duelNoRetreating)
            {
                client.gameGraphics.DrawBox(byte0 + 95, byte1 + 215 + 8, 7, 7, 0xffff00);
            }

            client.gameGraphics.DrawBoxEdge(byte0 + 93, byte1 + 215 + 25, 11, 11, 0xffff00);
            if (client.duelNoMagic)
            {
                client.gameGraphics.DrawBox(byte0 + 95, byte1 + 215 + 27, 7, 7, 0xffff00);
            }

            client.gameGraphics.DrawBoxEdge(byte0 + 191, byte1 + 215 + 6, 11, 11, 0xffff00);
            if (client.duelNoPrayer)
            {
                client.gameGraphics.DrawBox(byte0 + 193, byte1 + 215 + 8, 7, 7, 0xffff00);
            }

            client.gameGraphics.DrawBoxEdge(byte0 + 191, byte1 + 215 + 25, 11, 11, 0xffff00);
            if (client.duelNoWeapons)
            {
                client.gameGraphics.DrawBox(byte0 + 193, byte1 + 215 + 27, 7, 7, 0xffff00);
            }

            if (!client.duelMyAccepted)
            {
                client.gameGraphics.DrawPicture(byte0 + 217, byte1 + 238, client.baseInventoryPic + 25);
            }

            client.gameGraphics.DrawPicture(byte0 + 394, byte1 + 238, client.baseInventoryPic + 26);
            if (client.duelOpponentAccepted)
            {
                client.gameGraphics.DrawText("Other player", byte0 + 341, byte1 + 246, 1, 0xffffff);
                client.gameGraphics.DrawText("has accepted", byte0 + 341, byte1 + 256, 1, 0xffffff);
            }
            if (client.duelMyAccepted)
            {
                client.gameGraphics.DrawText("Waiting for", byte0 + 217 + 35, byte1 + 246, 1, 0xffffff);
                client.gameGraphics.DrawText("other player", byte0 + 217 + 35, byte1 + 256, 1, 0xffffff);
            }
            for (int k5 = 0; k5 < client.inventoryItemsCount; k5 += 1)
            {
                int l5 = 217 + byte0 + k5 % 5 * 49;
                int j6 = 31 + byte1 + k5 / 5 * 34;
                RecordItemSprite(l5, j6, 48, 32, client.entityManager.GetItem(client.inventoryItems[k5]));
                if (client.entityManager.GetItem(client.inventoryItems[k5]).IsStackable == 0)
                {
                    client.gameGraphics.DrawString(client.inventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < client.duelMyItemCount; i6 += 1)
            {
                int k6 = 9 + byte0 + i6 % 4 * 49;
                int i7 = 31 + byte1 + i6 / 4 * 34;
                RecordItemSprite(k6, i7, 48, 32, client.entityManager.GetItem(client.duelMyItems[i6]));
                if (client.entityManager.GetItem(client.duelMyItems[i6]).IsStackable == 0)
                {
                    client.gameGraphics.DrawString(client.duelMyItemsCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (client.mouseX > k6 && client.mouseX < k6 + 48 && client.mouseY > i7 && client.mouseY < i7 + 32)
                {
                    Item duelMyItem = client.entityManager.GetItem(client.duelMyItems[i6]);
                    client.gameGraphics.DrawString(duelMyItem.Name + ": @whi@" + duelMyItem.Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < client.duelOpponentItemCount; l6 += 1)
            {
                int j7 = 9 + byte0 + l6 % 4 * 49;
                int k7 = 124 + byte1 + l6 / 4 * 34;
                RecordItemSprite(j7, k7, 48, 32, client.entityManager.GetItem(client.duelOpponentItems[l6]));
                if (client.entityManager.GetItem(client.duelOpponentItems[l6]).IsStackable == 0)
                {
                    client.gameGraphics.DrawString(client.duelOpponentItemsCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (client.mouseX > j7 && client.mouseX < j7 + 48 && client.mouseY > k7 && client.mouseY < k7 + 32)
                {
                    Item duelOpponentItem = client.entityManager.GetItem(client.duelOpponentItems[l6]);
                    client.gameGraphics.DrawString(duelOpponentItem.Name + ": @whi@" + duelOpponentItem.Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

        }
        public void DrawWildernessAlertBox()
        {
            int l = 97;
            client.gameGraphics.DrawBox(86, 77, 340, 180, 0);
            client.gameGraphics.DrawBoxEdge(86, 77, 340, 180, 0xffffff);
            client.gameGraphics.DrawText("Warning! Proceed with caution", 256, l, 4, 0xff0000);
            l += 26;
            client.gameGraphics.DrawText("If you go much further north you will enter the", 256, l, 1, 0xffffff);
            l += 13;
            client.gameGraphics.DrawText("wilderness. This a very dangerous area where", 256, l, 1, 0xffffff);
            l += 13;
            client.gameGraphics.DrawText("other players can attack you!", 256, l, 1, 0xffffff);
            l += 22;
            client.gameGraphics.DrawText("The further north you go the more dangerous it", 256, l, 1, 0xffffff);
            l += 13;
            client.gameGraphics.DrawText("becomes, but the more treasure you will find.", 256, l, 1, 0xffffff);
            l += 22;
            client.gameGraphics.DrawText("In the wilderness an indicator at the bottom-right", 256, l, 1, 0xffffff);
            l += 13;
            client.gameGraphics.DrawText("of the screen will show the current level of danger", 256, l, 1, 0xffffff);
            l += 22;
            int i1 = 0xffffff;
            if (client.mouseY > l - 12 && client.mouseY <= l && client.mouseX > 181 && client.mouseX < 331)
            {
                i1 = 0xff0000;
            }

            client.gameGraphics.DrawText("Click here to close window", 256, l, 1, i1);
            if (client.mouseButtonClick != 0)
            {
                if (client.mouseY > l - 12 && client.mouseY <= l && client.mouseX > 181 && client.mouseX < 331)
                {
                    client.wildType = 2;
                }

                if (client.mouseX < 86 || client.mouseX > 426 || client.mouseY < 77 || client.mouseY > 257)
                {
                    client.wildType = 2;
                }

                client.mouseButtonClick = 0;
            }
        }
        public void DrawNPC(int x, int y, int width, int height, int index, int unknown1, int unknown2)
        {
            ClientMob npc = client.npcArray[index];
            int frameIndex = npc.currentSprite + (client.cameraRotation + 16) / 32 & 7;
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
            int j1 = newFrameIndex * 3 + client.walkModel[npc.stepCount / client.entityManager.GetNpc(npc.npcId).WalkModel % 4];
            if (npc.currentSprite == 8)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = false;
                x -= client.entityManager.GetNpc(npc.npcId).CombatSprite * unknown2 / 100;
                j1 = newFrameIndex * 3 + client.combatModelArray1[client.tick / (client.entityManager.GetNpc(npc.npcId).CombatModel - 1) % 8];
            }
            else
                if (npc.currentSprite == 9)
                {
                    newFrameIndex = 5;
                    frameIndex = 2;
                    flag = true;
                    x += client.entityManager.GetNpc(npc.npcId).CombatSprite * unknown2 / 100;
                    j1 = newFrameIndex * 3 + client.combatModelArray2[client.tick / client.entityManager.GetNpc(npc.npcId).CombatModel % 8];
                }
            for (int k1 = 0; k1 < 12; k1 += 1)
            {
                int l1 = client.animationModelArray[frameIndex][k1];
                int k2 = client.entityManager.GetNpc(npc.npcId).Sprites[l1];
                if (k2 >= 0)
                {
                    int i3 = 0;
                    int j3 = 0;
                    int k3 = j1;
                    if (flag && newFrameIndex >= 1 && newFrameIndex <= 3 && client.entityManager.GetAnimation(k2).HasF == 1)
                    {
                        k3 += 15;
                    }

                    if (newFrameIndex != 5 || client.entityManager.GetAnimation(k2).HasA == 1)
                    {
                        int l3 = k3 + client.entityManager.GetAnimation(k2).Number;
                        i3 = i3 * width / client.gameGraphics.pictureAssumedWidth[l3];
                        j3 = j3 * height / client.gameGraphics.pictureAssumedHeight[l3];
                        int i4 = width * client.gameGraphics.pictureAssumedWidth[l3] / client.gameGraphics.pictureAssumedWidth[client.entityManager.GetAnimation(k2).Number];
                        i3 -= (i4 - width) / 2;
                        int j4 = client.entityManager.GetAnimation(k2).CharacterColour;
                        int k4 = 0;
                        if (j4 == 1)
                        {
                            j4 = client.entityManager.GetNpc(npc.npcId).Appearance.HairColour;
                            k4 = client.entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }
                        else
                            if (j4 == 2)
                            {
                                j4 = client.entityManager.GetNpc(npc.npcId).Appearance.TopColour;
                                k4 = client.entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                            }
                            else
                                if (j4 == 3)
                                {
                                    j4 = client.entityManager.GetNpc(npc.npcId).Appearance.TrousersColour;
                                    k4 = client.entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                                }
                        client.gameGraphics.DrawImage(x + i3, y + j3, i4, height, l3, j4, k4, unknown1, flag);
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
                    int i2 = x;
                    if (npc.currentSprite == 8)
                    {
                        i2 -= 20 * unknown2 / 100;
                    }
                    else
                        if (npc.currentSprite == 9)
                    {
                        i2 += 20 * unknown2 / 100;
                    }

                    int l2 = npc.currentHits * 30 / npc.baseHits;
                    client.healthBarX[client.healthBarVisibleCount] = i2 + width / 2;
                    client.healthBarY[client.healthBarVisibleCount] = y;
                    client.healthBarMissing[client.healthBarVisibleCount++] = l2;
                }
                if (npc.combatTimer > 150)
                {
                    int j2 = x;
                    if (npc.currentSprite == 8)
                    {
                        j2 -= 10 * unknown2 / 100;
                    }
                    else
                        if (npc.currentSprite == 9)
                    {
                        j2 += 10 * unknown2 / 100;
                    }

                    client.gameGraphics.DrawPicture(j2 + width / 2 - 12, y + height / 2 - 12, client.baseInventoryPic + 12);
                    client.gameGraphics.DrawText(npc.lastDamageCount.ToString(), j2 + width / 2 - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
        }
        public void DrawAboveHeadThings()
        {
            for (int l = 0; l < client.receivedMessagesCount; l += 1)
            {
                int height = client.gameGraphics.TextHeightNumber(1);
                int x = client.receivedMessageX[l];
                int y = client.receivedMessageY[l];
                int midpoint = client.receivedMessageMidPoint[l];
                int l3 = client.receivedMessageHeight[l];
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int l4 = 0; l4 < l; l4 += 1)
                    {
                        if (y + l3 > client.receivedMessageY[l4] - height && y - height < client.receivedMessageY[l4] + client.receivedMessageHeight[l4] && x - midpoint < client.receivedMessageX[l4] + client.receivedMessageMidPoint[l4] && x + midpoint > client.receivedMessageX[l4] - client.receivedMessageMidPoint[l4] && client.receivedMessageY[l4] - height - l3 < y)
                        {
                            y = client.receivedMessageY[l4] - height - l3;
                            flag = true;
                        }
                    }
                }
                client.receivedMessageY[l] = y;
                client.gameGraphics.DrawFloatingText(client.receivedMessages[l], x, y, 1, 0xffff00, 300);
            }

            for (int j1 = 0; j1 < client.itemsAboveHeadCount; j1 += 1)
            {
                int x = client.itemAboveHeadX[j1];
                int y = client.itemAboveHeadY[j1];
                int scale = client.itemAboveHeadScale[j1];
                int id = client.itemAboveHeadID[j1];
                int width = 39 * scale / 100;
                int height = 27 * scale / 100;
                int j5 = y - height;
                client.gameGraphics.DrawTransparentImage(x - width / 2, j5, width, height, client.baseInventoryPic + 9, 85);
                int k5 = 36 * scale / 100;
                int l5 = 24 * scale / 100;
                RecordItemSprite(x - k5 / 2, j5 + height / 2 - l5 / 2, k5, l5, client.entityManager.GetItem(id));
            }

            for (int i2 = 0; i2 < client.healthBarVisibleCount; i2 += 1)
            {
                int x = client.healthBarX[i2];
                int y = client.healthBarY[i2];
                int missing = client.healthBarMissing[i2];
                client.gameGraphics.DrawBoxAlpha(x - 15, y - 3, missing, 5, 65280, 192);
                client.gameGraphics.DrawBoxAlpha(x - 15 + missing, y - 3, 30 - missing, 5, 0xff0000, 192);
            }

        }
        public void DrawBankBox()
        {
            char c1 = '\u0198';
            char c2 = '\u014E';
            if (client.bankPage > 0 && client.bankItemsCount <= 48)
            {
                client.bankPage = 0;
            }

            if (client.bankPage > 1 && client.bankItemsCount <= 96)
            {
                client.bankPage = 1;
            }

            if (client.bankPage > 2 && client.bankItemsCount <= 144)
            {
                client.bankPage = 2;
            }

            if (client.selectedBankItem >= client.bankItemsCount || client.selectedBankItem < 0)
            {
                client.selectedBankItem = -1;
            }

            if (client.selectedBankItem != -1 && client.bankItems[client.selectedBankItem] != client.selectedBankItemType)
            {
                client.selectedBankItem = -1;
                client.selectedBankItemType = -2;
            }
            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;
                int l = client.mouseX - (256 - c1 / 2);
                int j1 = client.mouseY - (170 - c2 / 2);
                if (l >= 0 && j1 >= 12 && l < 408 && j1 < 280)
                {
                    int l1 = client.bankPage * 48;
                    for (int k2 = 0; k2 < 6; k2 += 1)
                    {
                        for (int i3 = 0; i3 < 8; i3 += 1)
                        {
                            int k7 = 7 + i3 * 49;
                            int i8 = 28 + k2 * 34;
                            if (l > k7 && l < k7 + 49 && j1 > i8 && j1 < i8 + 34 && l1 < client.bankItemsCount && client.bankItems[l1] != -1)
                            {
                                client.selectedBankItemType = client.bankItems[l1];
                                client.selectedBankItem = l1;
                            }
                            l1 += 1;
                        }

                    }

                    l = 256 - c1 / 2;
                    j1 = 170 - c2 / 2;
                    int id;
                    if (client.selectedBankItem < 0)
                    {
                        id = -1;
                    }
                    else
                    {
                        id = client.bankItems[client.selectedBankItem];
                    }

                    if (id != -1)
                    {
                        int count = client.bankItemCount[client.selectedBankItem];
                        if (client.entityManager.GetItem(id).IsStackable == 1 && count > 1)
                        {
                            count = 1;
                        }

                        if (count >= 1 && client.mouseX >= l + 220 && client.mouseY >= j1 + 238 && client.mouseX < l + 250 && client.mouseY <= j1 + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(1);
                            client.streamClass.FormatPacket();
                        }
                        if (count >= 5 && client.mouseX >= l + 250 && client.mouseY >= j1 + 238 && client.mouseX < l + 280 && client.mouseY <= j1 + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(5);
                            client.streamClass.FormatPacket();
                        }
                        if (count >= 25 && client.mouseX >= l + 280 && client.mouseY >= j1 + 238 && client.mouseX < l + 305 && client.mouseY <= j1 + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(25);
                            client.streamClass.FormatPacket();
                        }
                        if (count >= 100 && client.mouseX >= l + 305 && client.mouseY >= j1 + 238 && client.mouseX < l + 335 && client.mouseY <= j1 + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(100);
                            client.streamClass.FormatPacket();
                        }
                        if (count >= 500 && client.mouseX >= l + 335 && client.mouseY >= j1 + 238 && client.mouseX < l + 368 && client.mouseY <= j1 + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(500);
                            client.streamClass.FormatPacket();
                        }
                        if (count >= 2500 && client.mouseX >= l + 370 && client.mouseY >= j1 + 238 && client.mouseX < l + 400 && client.mouseY <= j1 + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(2500);
                            client.streamClass.FormatPacket();
                        }
                        if (client.GetInventoryItemTotalCount(id) >= 1 && client.mouseX >= l + 220 && client.mouseY >= j1 + 263 && client.mouseX < l + 250 && client.mouseY <= j1 + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(1);
                            client.streamClass.FormatPacket();
                        }
                        if (client.GetInventoryItemTotalCount(id) >= 5 && client.mouseX >= l + 250 && client.mouseY >= j1 + 263 && client.mouseX < l + 280 && client.mouseY <= j1 + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(5);
                            client.streamClass.FormatPacket();
                        }
                        if (client.GetInventoryItemTotalCount(id) >= 25 && client.mouseX >= l + 280 && client.mouseY >= j1 + 263 && client.mouseX < l + 305 && client.mouseY <= j1 + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(25);
                            client.streamClass.FormatPacket();
                        }
                        if (client.GetInventoryItemTotalCount(id) >= 100 && client.mouseX >= l + 305 && client.mouseY >= j1 + 263 && client.mouseX < l + 335 && client.mouseY <= j1 + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(100);
                            client.streamClass.FormatPacket();
                        }
                        if (client.GetInventoryItemTotalCount(id) >= 500 && client.mouseX >= l + 335 && client.mouseY >= j1 + 263 && client.mouseX < l + 368 && client.mouseY <= j1 + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(500);
                            client.streamClass.FormatPacket();
                        }
                        if (client.GetInventoryItemTotalCount(id) >= 2500 && client.mouseX >= l + 370 && client.mouseY >= j1 + 263 && client.mouseX < l + 400 && client.mouseY <= j1 + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(2500);
                            client.streamClass.FormatPacket();
                        }
                    }
                }
                else
                    if (client.bankItemsCount > 48 && l >= 50 && l <= 115 && j1 <= 12)
                {
                    client.bankPage = 0;
                }
                else
                        if (client.bankItemsCount > 48 && l >= 115 && l <= 180 && j1 <= 12)
                {
                    client.bankPage = 1;
                }
                else
                            if (client.bankItemsCount > 96 && l >= 180 && l <= 245 && j1 <= 12)
                {
                    client.bankPage = 2;
                }
                else
                                if (client.bankItemsCount > 144 && l >= 245 && l <= 310 && j1 <= 12)
                                {
                                    client.bankPage = 3;
                                }
                                else
                                {
                                    client.streamClass.CreatePacket(48);
                                    client.streamClass.FormatPacket();
                                    client.showBankBox = false;
                                    return;
                                }
            }
            int i1 = 256 - c1 / 2;
            int k1 = 170 - c2 / 2;
            client.gameGraphics.DrawBox(i1, k1, 408, 12, 192);
            int j2 = 0x989898;
            client.gameGraphics.DrawBoxAlpha(i1, k1 + 12, 408, 17, j2, 160);
            client.gameGraphics.DrawBoxAlpha(i1, k1 + 29, 8, 204, j2, 160);
            client.gameGraphics.DrawBoxAlpha(i1 + 399, k1 + 29, 9, 204, j2, 160);
            client.gameGraphics.DrawBoxAlpha(i1, k1 + 233, 408, 47, j2, 160);
            client.gameGraphics.DrawString("Bank", i1 + 1, k1 + 10, 1, 0xffffff);
            int l2 = 50;
            if (client.bankItemsCount > 48)
            {
                int k3 = 0xffffff;
                if (client.bankPage == 0)
                {
                    k3 = 0xff0000;
                }
                else
                    if (client.mouseX > i1 + l2 && client.mouseY >= k1 && client.mouseX < i1 + l2 + 65 && client.mouseY < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 1>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
                k3 = 0xffffff;
                if (client.bankPage == 1)
                {
                    k3 = 0xff0000;
                }
                else
                    if (client.mouseX > i1 + l2 && client.mouseY >= k1 && client.mouseX < i1 + l2 + 65 && client.mouseY < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 2>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
            }
            if (client.bankItemsCount > 96)
            {
                int l3 = 0xffffff;
                if (client.bankPage == 2)
                {
                    l3 = 0xff0000;
                }
                else
                    if (client.mouseX > i1 + l2 && client.mouseY >= k1 && client.mouseX < i1 + l2 + 65 && client.mouseY < k1 + 12)
                {
                    l3 = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 3>", i1 + l2, k1 + 10, 1, l3);
                l2 += 65;
            }
            if (client.bankItemsCount > 144)
            {
                int i4 = 0xffffff;
                if (client.bankPage == 3)
                {
                    i4 = 0xff0000;
                }
                else
                    if (client.mouseX > i1 + l2 && client.mouseY >= k1 && client.mouseX < i1 + l2 + 65 && client.mouseY < k1 + 12)
                {
                    i4 = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 4>", i1 + l2, k1 + 10, 1, i4);
            }
            int j4 = 0xffffff;
            if (client.mouseX > i1 + 320 && client.mouseY >= k1 && client.mouseX < i1 + 408 && client.mouseY < k1 + 12)
            {
                j4 = 0xff0000;
            }

            client.gameGraphics.DrawLabel("Close window", i1 + 406, k1 + 10, 1, j4);
            client.gameGraphics.DrawString("Number in bank in green", i1 + 7, k1 + 24, 1, 65280);
            client.gameGraphics.DrawString("Number held in blue", i1 + 289, k1 + 24, 1, 65535);
            int l7 = 0xd0d0d0;
            int j8 = client.bankPage * 48;
            for (int l8 = 0; l8 < 6; l8 += 1)
            {
                for (int i9 = 0; i9 < 8; i9 += 1)
                {
                    int k9 = i1 + 7 + i9 * 49;
                    int l9 = k1 + 28 + l8 * 34;
                    if (client.selectedBankItem == j8)
                    {
                        client.gameGraphics.DrawBoxAlpha(k9, l9, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        client.gameGraphics.DrawBoxAlpha(k9, l9, 49, 34, l7, 160);
                    }

                    client.gameGraphics.DrawBoxEdge(k9, l9, 50, 35, 0);

                    if (j8 < client.bankItemsCount && client.bankItems[j8] != -1)
                    {
                        RecordItemSprite(k9, l9, 48, 32, client.entityManager.GetItem(client.bankItems[j8]));
                        client.gameGraphics.DrawString(client.bankItemCount[j8].ToString(), k9 + 1, l9 + 10, 1, 65280);
                        client.gameGraphics.DrawLabel(client.GetInventoryItemTotalCount(client.bankItems[j8]).ToString(), k9 + 47, l9 + 29, 1, 65535);
                    }

                    j8 += 1;
                }

            }

            client.gameGraphics.DrawLineX(i1 + 5, k1 + 256, 398, 0);
            if (client.selectedBankItem == -1)
            {
                client.gameGraphics.DrawText("Select an object to withdraw or deposit", i1 + 204, k1 + 248, 3, 0xffff00);
                return;
            }
            int j9;
            if (client.selectedBankItem < 0)
            {
                j9 = -1;
            }
            else
            {
                j9 = client.bankItems[client.selectedBankItem];
            }

            if (j9 != -1)
            {
                int k8 = client.bankItemCount[client.selectedBankItem];
                if (client.entityManager.GetItem(j9).IsStackable == 1 && k8 > 1)
                {
                    k8 = 1;
                }

                if (k8 > 0)
                {
                    client.gameGraphics.DrawString("Withdraw " + client.entityManager.GetItem(j9).Name, i1 + 2, k1 + 248, 1, 0xffffff);
                    int k4 = 0xffffff;
                    if (client.mouseX >= i1 + 220 && client.mouseY >= k1 + 238 && client.mouseX < i1 + 250 && client.mouseY <= k1 + 249)
                    {
                        k4 = 0xff0000;
                    }

                    client.gameGraphics.DrawString("One", i1 + 222, k1 + 248, 1, k4);
                    if (k8 >= 5)
                    {
                        int l4 = 0xffffff;
                        if (client.mouseX >= i1 + 250 && client.mouseY >= k1 + 238 && client.mouseX < i1 + 280 && client.mouseY <= k1 + 249)
                        {
                            l4 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("Five", i1 + 252, k1 + 248, 1, l4);
                    }
                    if (k8 >= 25)
                    {
                        int i5 = 0xffffff;
                        if (client.mouseX >= i1 + 280 && client.mouseY >= k1 + 238 && client.mouseX < i1 + 305 && client.mouseY <= k1 + 249)
                        {
                            i5 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("25", i1 + 282, k1 + 248, 1, i5);
                    }
                    if (k8 >= 100)
                    {
                        int j5 = 0xffffff;
                        if (client.mouseX >= i1 + 305 && client.mouseY >= k1 + 238 && client.mouseX < i1 + 335 && client.mouseY <= k1 + 249)
                        {
                            j5 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("100", i1 + 307, k1 + 248, 1, j5);
                    }
                    if (k8 >= 500)
                    {
                        int k5 = 0xffffff;
                        if (client.mouseX >= i1 + 335 && client.mouseY >= k1 + 238 && client.mouseX < i1 + 368 && client.mouseY <= k1 + 249)
                        {
                            k5 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("500", i1 + 337, k1 + 248, 1, k5);
                    }
                    if (k8 >= 2500)
                    {
                        int l5 = 0xffffff;
                        if (client.mouseX >= i1 + 370 && client.mouseY >= k1 + 238 && client.mouseX < i1 + 400 && client.mouseY <= k1 + 249)
                        {
                            l5 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("2500", i1 + 370, k1 + 248, 1, l5);
                    }
                }
                if (client.GetInventoryItemTotalCount(j9) > 0)
                {
                    client.gameGraphics.DrawString("Deposit " + client.entityManager.GetItem(j9).Name, i1 + 2, k1 + 273, 1, 0xffffff);
                    int i6 = 0xffffff;
                    if (client.mouseX >= i1 + 220 && client.mouseY >= k1 + 263 && client.mouseX < i1 + 250 && client.mouseY <= k1 + 274)
                    {
                        i6 = 0xff0000;
                    }

                    client.gameGraphics.DrawString("One", i1 + 222, k1 + 273, 1, i6);
                    if (client.GetInventoryItemTotalCount(j9) >= 5)
                    {
                        int j6 = 0xffffff;
                        if (client.mouseX >= i1 + 250 && client.mouseY >= k1 + 263 && client.mouseX < i1 + 280 && client.mouseY <= k1 + 274)
                        {
                            j6 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("Five", i1 + 252, k1 + 273, 1, j6);
                    }
                    if (client.GetInventoryItemTotalCount(j9) >= 25)
                    {
                        int k6 = 0xffffff;
                        if (client.mouseX >= i1 + 280 && client.mouseY >= k1 + 263 && client.mouseX < i1 + 305 && client.mouseY <= k1 + 274)
                        {
                            k6 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("25", i1 + 282, k1 + 273, 1, k6);
                    }
                    if (client.GetInventoryItemTotalCount(j9) >= 100)
                    {
                        int l6 = 0xffffff;
                        if (client.mouseX >= i1 + 305 && client.mouseY >= k1 + 263 && client.mouseX < i1 + 335 && client.mouseY <= k1 + 274)
                        {
                            l6 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("100", i1 + 307, k1 + 273, 1, l6);
                    }
                    if (client.GetInventoryItemTotalCount(j9) >= 500)
                    {
                        int i7 = 0xffffff;
                        if (client.mouseX >= i1 + 335 && client.mouseY >= k1 + 263 && client.mouseX < i1 + 368 && client.mouseY <= k1 + 274)
                        {
                            i7 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("500", i1 + 337, k1 + 273, 1, i7);
                    }
                    if (client.GetInventoryItemTotalCount(j9) >= 2500)
                    {
                        int j7 = 0xffffff;
                        if (client.mouseX >= i1 + 370 && client.mouseY >= k1 + 263 && client.mouseX < i1 + 400 && client.mouseY <= k1 + 274)
                        {
                            j7 = 0xff0000;
                        }

                        client.gameGraphics.DrawString("2500", i1 + 370, k1 + 273, 1, j7);
                    }
                }
            }
        }
        public void DrawMinimapObject(int x, int y, int color)
        {
            client.gameGraphics.DrawMinimapPixel(x, y, color);
            client.gameGraphics.DrawMinimapPixel(x - 1, y, color);
            client.gameGraphics.DrawMinimapPixel(x + 1, y, color);
            client.gameGraphics.DrawMinimapPixel(x, y - 1, color);
            client.gameGraphics.DrawMinimapPixel(x, y + 1, color);
        }
        public void DrawServerMessageBox()
        {
            char c1 = '\u0190';
            char c2 = 'd';
            if (client.serverMessageBoxTop)
            {
                c2 = '\u012C';
            }
            client.gameGraphics.DrawBox(256 - c1 / 2, 167 - c2 / 2, c1, c2, 0);
            client.gameGraphics.DrawBoxEdge(256 - c1 / 2, 167 - c2 / 2, c1, c2, 0xffffff);
            client.gameGraphics.DrawFloatingText(client.serverMessage, 256, 167 - c2 / 2 + 20, 1, 0xffffff, c1 - 40);
            int l = 157 + c2 / 2;
            int i1 = 0xffffff;
            if (client.mouseY > l - 12 && client.mouseY <= l && client.mouseX > 106 && client.mouseX < 406)
            {
                i1 = 0xff0000;
            }

            client.gameGraphics.DrawText("Click here to close window", 256, l, 1, i1);
            if (client.mouseButtonClick == 1)
            {
                if (i1 == 0xff0000)
                {
                    client.showServerMessageBox = false;
                }

                if ((client.mouseX < 256 - c1 / 2 || client.mouseX > 256 + c1 / 2) && (client.mouseY < 167 - c2 / 2 || client.mouseY > 167 + c2 / 2))
                {
                    client.showServerMessageBox = false;
                }
            }
            client.mouseButtonClick = 0;
        }
        public void DrawStatsQuestsMenu(bool canClick)
        {
            int l = client.gameGraphics.gameWidth - 199; //199
            int i1 = 36;
            client.gameGraphics.DrawPicture(l - 49, 3, client.baseInventoryPic + 3);
            int c1 = 196;//'\u304';
            int c2 = 275;//113;//'\u0113';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (client.questMenuSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            client.gameGraphics.DrawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            client.gameGraphics.DrawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            client.gameGraphics.DrawBoxAlpha(l, i1 + 24, c1, c2 - 24, GameImage.RgbToInt(220, 220, 220), 128);
            client.gameGraphics.DrawLineX(l, i1 + 24, c1, 0);
            client.gameGraphics.DrawLineY(l + c1 / 2, i1, 24, 0);
            client.gameGraphics.DrawText("Stats", l + c1 / 4, i1 + 16, 4, 0);
            client.gameGraphics.DrawText("Quests", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            if (client.questMenuSelected == 0)
            {
                int l1 = 72;
                int j2 = -1;
                client.gameGraphics.DrawString("Skills", l + 5, l1, 3, 0xffff00);
                l1 += 13;
                for (int k2 = 0; k2 < 9; k2 += 1)
                {
                    int l2 = 0xffffff;
                    if (client.mouseX > l + 3 && client.mouseY >= l1 - 11 && client.mouseY < l1 + 2 && client.mouseX < l + 90)
                    {
                        l2 = 0xff0000;
                        j2 = k2;
                    }
                    client.gameGraphics.DrawString(client.skillName[k2] + ":@yel@" + client.playerStatCurrent[k2] + "/" + client.playerStatBase[k2], l + 5, l1, 1, l2);
                    l2 = 0xffffff;
                    if (client.mouseX >= l + 90 && client.mouseY >= l1 - 13 - 11 && client.mouseY < l1 - 13 + 2 && client.mouseX < l + 196)
                    {
                        l2 = 0xff0000;
                        j2 = k2 + 9;
                    }
                    client.gameGraphics.DrawString(client.skillName[k2 + 9] + ":@yel@" + client.playerStatCurrent[k2 + 9] + "/" + client.playerStatBase[k2 + 9], l + c1 / 2 - 5, l1 - 13, 1, l2);
                    l1 += 13;
                }

                client.gameGraphics.DrawString("Quest Points:@yel@" + client.questPoints, l + c1 / 2 - 5, l1 - 13, 1, 0xffffff);
                l1 += 12;
                client.gameGraphics.DrawString("Fatigue: @yel@" + client.fatigue * 100 / 750 + "%", l + 5, l1 - 13, 1, 0xffffff);
                l1 += 8;
                client.gameGraphics.DrawString("Equipment Status", l + 5, l1, 3, 0xffff00);
                l1 += 12;
                for (int i3 = 0; i3 < 3; i3 += 1)
                {
                    client.gameGraphics.DrawString(client.gearStats[i3] + ":@yel@" + client.equipmentStatus[i3], l + 5, l1, 1, 0xffffff);
                    if (i3 < 2)
                    {
                        client.gameGraphics.DrawString(client.gearStats[i3 + 3] + ":@yel@" + client.equipmentStatus[i3 + 3], l + c1 / 2 + 25, l1, 1, 0xffffff);
                    }

                    l1 += 13;
                }

                l1 += 6;
                client.gameGraphics.DrawLineX(l, l1 - 15, c1, 0);
                if (j2 != -1)
                {
                    client.gameGraphics.DrawString(client.skillNameVerb[j2] + " skill", l + 5, l1, 1, 0xffff00);
                    l1 += 12;
                    int j3 = client.experienceList[0];
                    for (int l3 = 0; l3 < 98; l3 += 1)
                    {
                        if (client.playerStatExp[j2] >= client.experienceList[l3])
                        {
                            j3 = client.experienceList[l3 + 1];
                        }
                    }

                    client.gameGraphics.DrawString("Total xp: " + client.playerStatExp[j2], l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                    client.gameGraphics.DrawString("Next level at: " + j3, l + 5, l1, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString("Overall levels", l + 5, l1, 1, 0xffff00);
                    l1 += 12;
                    int k3 = 0;
                    for (int i4 = 0; i4 < 18; i4 += 1)
                    {
                        k3 += client.playerStatBase[i4];
                    }

                    client.gameGraphics.DrawString("Skill total: " + k3, l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                    client.gameGraphics.DrawString("Combat level: " + client.ourPlayer.level, l + 5, l1, 1, 0xffffff);
                }
            }
            if (client.questMenuSelected == 1)
            {
                client.questMenu.ClearList(client.questMenuHandle);
                client.questMenu.AddListItem(client.questMenuHandle, 0, "@whi@Quest-list (green=completed)");
                for (int i2 = 0; i2 < client.usedQuestName.Length; i2 += 1)
                {
                    string questColor;

                    if (client.questStage[i2] == 0)
                    {
                        questColor = "@red@";
                    }
                    else if (client.questStage[i2] == 1)
                    {
                        questColor = "@yel@";
                    }
                    else
                    {
                        questColor = "@gre@";
                    }

                    client.questMenu.AddListItem(client.questMenuHandle, i2 + 1, questColor + client.usedQuestName[i2]);
                }

                client.questMenu.DrawMenu();
            }
            if (!canClick)
            {
                return;
            }

            l = client.mouseX - (client.gameGraphics.gameWidth - 199);
            i1 = client.mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < c1 && i1 < c2)
            {
                if (client.questMenuSelected == 1)
                {
                    client.questMenu.MouseClick(l + (client.gameGraphics.gameWidth - 199), i1 + 36, client.lastMouseButton, client.mouseButton);
                }

                if (i1 <= 24 && client.mouseButtonClick == 1)
                {
                    if (l < 98)
                    {
                        client.questMenuSelected = 0;
                        return;
                    }
                    if (l > 98)
                    {
                        client.questMenuSelected = 1;
                    }
                }
            }
        }
        public void DrawFriendsBox()
        {
            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;
                if (client.showFriendsBox == 1 && (client.mouseX < 106 || client.mouseY < 145 || client.mouseX > 406 || client.mouseY > 215))
                {
                    client.showFriendsBox = 0;
                    return;
                }
                if (client.showFriendsBox == 2 && (client.mouseX < 6 || client.mouseY < 145 || client.mouseX > 506 || client.mouseY > 215))
                {
                    client.showFriendsBox = 0;
                    return;
                }
                if (client.showFriendsBox == 3 && (client.mouseX < 106 || client.mouseY < 145 || client.mouseX > 406 || client.mouseY > 215))
                {
                    client.showFriendsBox = 0;
                    return;
                }
                if (client.mouseX > 236 && client.mouseX < 276 && client.mouseY > 193 && client.mouseY < 213)
                {
                    client.showFriendsBox = 0;
                    return;
                }
            }
            int l = 145;
            if (client.showFriendsBox == 1)
            {
                client.gameGraphics.DrawBox(106, l, 300, 70, 0);
                client.gameGraphics.DrawBoxEdge(106, l, 300, 70, 0xffffff);
                l += 20;
                client.gameGraphics.DrawText("Enter name to add to friends list", 256, l, 4, 0xffffff);
                l += 20;
                client.gameGraphics.DrawText(client.inputText + "*", 256, l, 4, 0xffffff);
                if (client.enteredInputText.Length > 0)
                {
                    string s1 = client.enteredInputText.Trim();
                    client.inputText = "";
                    client.enteredInputText = "";
                    client.showFriendsBox = 0;
                    if (s1.Length > 0 && DataOperations.NameToHash(s1) != client.ourPlayer.nameHash)
                    {
                        client.CallAddFriend(s1);
                    }
                }
            }
            if (client.showFriendsBox == 2)
            {
                client.gameGraphics.DrawBox(6, l, 500, 70, 0);
                client.gameGraphics.DrawBoxEdge(6, l, 500, 70, 0xffffff);
                l += 20;
                client.gameGraphics.DrawText("Enter message to send to " + DataOperations.HashToName(client.pmTarget), 256, l, 4, 0xffffff);
                l += 20;
                client.gameGraphics.DrawText(client.pmText + "*", 256, l, 4, 0xffffff);
                if (client.enteredPMText.Length > 0)
                {
                    string s2 = client.enteredPMText;
                    client.pmText = "";
                    client.enteredPMText = "";
                    client.showFriendsBox = 0;
                    int j1 = ChatMessage.StringToBytes(s2);
                    client.CallSendPrivateMessage(client.pmTarget, ChatMessage.lastChat, j1);
                    s2 = ChatMessage.BytesToString(ChatMessage.lastChat, 0, j1);
                    //if (useChatFilter)
                    // s2 = ChatFilter.filterChat(s2);
                    client.DisplayMessage("@pri@You tell " + DataOperations.HashToName(client.pmTarget) + ": " + s2);
                }
            }
            if (client.showFriendsBox == 3)
            {
                client.gameGraphics.DrawBox(106, l, 300, 70, 0);
                client.gameGraphics.DrawBoxEdge(106, l, 300, 70, 0xffffff);
                l += 20;
                client.gameGraphics.DrawText("Enter name to add to ignore list", 256, l, 4, 0xffffff);
                l += 20;
                client.gameGraphics.DrawText(client.inputText + "*", 256, l, 4, 0xffffff);
                if (client.enteredInputText.Length > 0)
                {
                    string s3 = client.enteredInputText.Trim();
                    client.inputText = "";
                    client.enteredInputText = "";
                    client.showFriendsBox = 0;
                    if (s3.Length > 0 && DataOperations.NameToHash(s3) != client.ourPlayer.nameHash)
                    {
                        client.CallAddIgnore(s3);
                    }
                }
            }
            int i1 = 0xffffff;
            if (client.mouseX > 236 && client.mouseX < 276 && client.mouseY > 193 && client.mouseY < 213)
            {
                i1 = 0xffff00;
            }

            client.gameGraphics.DrawText("Cancel", 256, 208, 1, i1);
        }
        public void DrawRightClickMenu()
        {
            if (client.mouseButtonClick != 0)
            {
                for (int l = 0; l < client.menuOptionsCount; l += 1)
                {
                    int j1 = client.menuX + 2;
                    int l1 = client.menuY + 27 + l * 15;
                    if (client.mouseX <= j1 - 2 || client.mouseY <= l1 - 12 || client.mouseY >= l1 + 4 || client.mouseX >= j1 - 3 + client.menuWidth)
                    {
                        continue;
                    }

                    client.MenuClick(client.menuIndexes[l]);
                    break;
                }

                client.mouseButtonClick = 0;
                client.menuShow = false;
                return;
            }
            if (client.mouseX < client.menuX - 10 || client.mouseY < client.menuY - 10 || client.mouseX > client.menuX + client.menuWidth + 10 || client.mouseY > client.menuY + client.menuHeight + 10)
            {
                client.menuShow = false;
                return;
            }
            client.gameGraphics.DrawBoxAlpha(client.menuX, client.menuY, client.menuWidth, client.menuHeight, 0xd0d0d0, 160);
            client.gameGraphics.DrawString("Choose option", client.menuX + 2, client.menuY + 12, 1, 65535);
            for (int i1 = 0; i1 < client.menuOptionsCount; i1 += 1)
            {
                int k1 = client.menuX + 2;
                int i2 = client.menuY + 27 + i1 * 15;
                int j2 = 0xffffff;
                if (client.mouseX > k1 - 2 && client.mouseY > i2 - 12 && client.mouseY < i2 + 4 && client.mouseX < k1 - 3 + client.menuWidth)
                {
                    j2 = 0xffff00;
                }

                string menuSecondaryText = client.menuText2[client.menuIndexes[i1]];
                client.gameGraphics.DrawString(client.menuText1[client.menuIndexes[i1]] + " " + client.menuText2[client.menuIndexes[i1]], k1, i2, 1, j2);
            }

        }

    }

}
