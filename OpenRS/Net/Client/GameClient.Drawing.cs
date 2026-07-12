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
        public void DrawNpc(int x, int y, int width, int height, int npcIndex, int cameraXOffset, int scalePercentage)
            => DrawNPC(x, y, width, height, npcIndex, cameraXOffset, scalePercentage);

        public Models.Enumerations.CombatStyle CombatStyle
        {
            get => (Models.Enumerations.CombatStyle)combatStyle;
            set => combatStyle = (int)value;
        }
        public void DrawReportAbuseBox1()
        {
            reportAbuseOptionSelected = 0;
            int yOffset = 135;
            for (int option = 0; option < 12; option += 1)
            {
                if (mouseX > 66 && mouseX < 446 && mouseY >= yOffset - 12 && mouseY < yOffset + 3)
                {
                    reportAbuseOptionSelected = option + 1;
                }

                yOffset += 14;
            }

            if (mouseButtonClick != 0 && reportAbuseOptionSelected != 0)
            {
                mouseButtonClick = 0;
                showAbuseBox = 2;
                inputText = "";
                enteredInputText = "";
                return;
            }
            yOffset += 15;
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                if (mouseX < 56 || mouseY < 35 || mouseX > 456 || mouseY > 325)
                {
                    showAbuseBox = 0;
                    return;
                }
                if (mouseX > 66 && mouseX < 446 && mouseY >= yOffset - 15 && mouseY < yOffset + 5)
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
            if (mouseX > 196 && mouseX < 316 && mouseY > yOffset - 15 && mouseY < yOffset + 5)
            {
                j1 = 0xffff00;
            }

            gameGraphics.DrawText("Click here to cancel", 256, yOffset, 1, j1);
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
                int i2 = GameData.GetModelNameIndex(s1);
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
                if (l2 > GameData.animationCount - 1)
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
                        if (GameData.animationHasF[l2] == 1)
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

                    if (direction2 != 5 || GameData.animationHasA[l2] == 1)
                    {
                        int k4 = j4 + GameData.animationNumber[l2];
                        k3 = (k3 * width) / ((GameImage)(gameGraphics)).pictureAssumedWidth[k4];
                        i4 = (i4 * height) / ((GameImage)(gameGraphics)).pictureAssumedHeight[k4];
                        int l4 = (width * ((GameImage)(gameGraphics)).pictureAssumedWidth[k4]) / ((GameImage)(gameGraphics)).pictureAssumedWidth[GameData.animationNumber[l2]];
                        k3 -= (l4 - width) / 2;
                        int i5 = GameData.animationCharacterColor[l2];
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
                string s1 = GameData.itemName[duelOurStakeItem[i1]];
                if (GameData.itemStackable[duelOurStakeItem[i1]] == 0)
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
                string s2 = GameData.itemName[duelOpponentStakeItem[j1]];
                if (GameData.itemStackable[duelOpponentStakeItem[j1]] == 0)
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
                if (mouseX < byte0 || mouseY < byte1 || mouseX > byte0 + 468 || mouseY > byte1 + 262)
                {
                    showDuelConfirmBox = false;
                    streamClass.CreatePacket(35);
                    streamClass.FormatPacket();
                }
                if (mouseX >= (byte0 + 118) - 35 && mouseX <= byte0 + 118 + 70 && mouseY >= byte1 + 238 && mouseY <= byte1 + 238 + 21)
                {
                    duelConfirmOurAccepted = true;
                    streamClass.CreatePacket(87);
                    streamClass.FormatPacket();
                }
                if (mouseX >= (byte0 + 352) - 35 && mouseX <= byte0 + 353 + 70 && mouseY >= byte1 + 238 && mouseY <= byte1 + 238 + 21)
                {
                    showDuelConfirmBox = false;
                    streamClass.CreatePacket(35);
                    streamClass.FormatPacket();
                }
                mouseButtonClick = 0;
            }
        }
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
                    gameGraphics.DrawImage(j1, l1, 48, 32, baseItemPicture + GameData.itemInventoryPicture[inventoryItems[i1]], GameData.itemPictureMask[inventoryItems[i1]], 0, 0, false);
                    if (GameData.itemStackable[inventoryItems[i1]] == 0)
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

            l = mouseX - (((GameImage)(gameGraphics)).gameWidth - 248);
            int j2 = mouseY - 36;
            if (l >= 0 && j2 >= 0 && l < 248 && j2 < (maxInventoryItems / 5) * 34)
            {
                int k2 = l / 49 + (j2 / 34) * 5;
                if (k2 < inventoryItemsCount)
                {
                    int l2 = inventoryItems[k2];
                    if (selectedSpell >= 0)
                    {
                        if (GameData.spellType[selectedSpell] == 3)
                        {
                            menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on";
                            menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
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
                            menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 610;
                            menuActionType[menuOptionsCount] = k2;
                            menuActionVar1[menuOptionsCount] = selectedItem;
                            menuOptionsCount += 1;
                            return;
                        }
                        if (inventoryItemEquipped[k2] == 1)
                        {
                            menuText1[menuOptionsCount] = "Remove";
                            menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 620;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount += 1;
                        }
                        else
                            if (GameData.itemIsEquippable[l2] != 0)
                            {
                                if ((GameData.itemIsEquippable[l2] & 0x18) != 0)
                            {
                                menuText1[menuOptionsCount] = "Wield";
                            }
                            else
                            {
                                menuText1[menuOptionsCount] = "Wear";
                            }

                            menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                                menuActionID[menuOptionsCount] = 630;
                                menuActionType[menuOptionsCount] = k2;
                                menuOptionsCount += 1;
                            }
                        if (GameData.itemCommand[l2] != "")
                        {
                            menuText1[menuOptionsCount] = GameData.itemCommand[l2];
                            menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                            menuActionID[menuOptionsCount] = 640;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount += 1;
                        }
                        menuText1[menuOptionsCount] = "Use";
                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                        menuActionID[menuOptionsCount] = 650;
                        menuActionType[menuOptionsCount] = k2;
                        menuOptionsCount += 1;
                        menuText1[menuOptionsCount] = "Drop";
                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                        menuActionID[menuOptionsCount] = 660;
                        menuActionType[menuOptionsCount] = k2;
                        menuOptionsCount += 1;
                        menuText1[menuOptionsCount] = "Examine";
                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[l2];
                        menuActionID[menuOptionsCount] = 3600;
                        menuActionType[menuOptionsCount] = l2;
                        menuOptionsCount += 1;
                    }
                }
            }
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
                for (int j9 = 0; j9 < friendsCount; j9 += 1)
                {
                    if (f2.nameHash != friendsList[j9] || friendsWorld[j9] != 99)
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

            l = mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            int l8 = mouseY - 36;
            if (l >= 40 && l8 >= 0 && l < 196 && l8 < 152)
            {
                int c2 = 156;//'\u234';
                int c4 = 152;//'\u230';
                int k1 = 192 + minimapRandomRotationY;
                int i2 = cameraRotation + minimapRandomRotationX & 0xff;
                int i1 = ((GameImage)(gameGraphics)).gameWidth - 199;
                i1 += 40;
                int k3 = ((mouseX - (i1 + c2 / 2)) * 16384) / (3 * k1);
                int i5 = ((mouseY - (36 + c4 / 2)) * 16384) / (3 * k1);
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
            if (mouseY > i1 - 12 && mouseY <= i1 && mouseX > 106 && mouseX < 406)
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

                if ((mouseX < 86 || mouseX > 426) && (mouseY < 167 - l / 2 || mouseY > 167 + l / 2))
                {
                    showWelcomeBox = false;
                }
            }
            mouseButtonClick = 0;
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
            if (blockChat == 0)
            {
                gameGraphics.DrawString("Block chat messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Block chat messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (blockPrivate == 0)
            {
                gameGraphics.DrawString("Block public messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.DrawString("Block public messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (blockTrade == 0)
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
                if (blockDuel == 0)
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
            if (mouseX > j1 && mouseX < j1 + c1 && mouseY > l1 - 12 && mouseY < l1 + 4)
            {
                j2 = 0xffff00;
            }

            gameGraphics.DrawString("Click here to logout", l + 3, l1, 1, j2);
            if (!canClick)
            {
                return;
            }

            l = mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 280)
            {
                int k2 = ((GameImage)(gameGraphics)).gameWidth - 199;
                sbyte byte0 = 36;
                int c2 = 196;
                int k1 = k2 + 3;
                int i2 = byte0 + 30;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    configCameraAutoAngle = !configCameraAutoAngle;
                    streamClass.CreatePacket(157);
                    streamClass.AddByte(0);
                    int configCameraAutoAngleByte = 0;
                    if (configCameraAutoAngle)
                    {
                        configCameraAutoAngleByte = 1;
                    }
                    streamClass.AddByte(configCameraAutoAngleByte);
                    streamClass.FormatPacket();
                }
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    configOneMouseButton = !configOneMouseButton;
                    streamClass.CreatePacket(157);
                    streamClass.AddByte(2);
                    int configOneMouseButtonByte = 0;
                    if (configOneMouseButton)
                    {
                        configOneMouseButtonByte = 1;
                    }
                    streamClass.AddByte(configOneMouseButtonByte);
                    streamClass.FormatPacket();
                }
                i2 += 15;
                if (Config.MembersFeatures && mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    configSoundOff = !configSoundOff;
                    streamClass.CreatePacket(157);
                    streamClass.AddByte(3);
                    int configSoundOffByte = 0;
                    if (configSoundOff)
                    {
                        configSoundOffByte = 1;
                    }
                    streamClass.AddByte(configSoundOffByte);
                    streamClass.FormatPacket();
                }
                i2 += 15;
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    showRoofs = !showRoofs;
                    streamClass.CreatePacket(157);
                    streamClass.AddByte(4);
                    int showRoofsByte = 0;
                    if (showRoofs)
                    {
                        showRoofsByte = 1;
                    }
                    streamClass.AddByte(showRoofsByte);
                    streamClass.FormatPacket();
                }
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    showCombatWindow = !showCombatWindow;
                    streamClass.CreatePacket(157);
                    streamClass.AddByte(6);
                    int showCombatWindowByte = 0;
                    if (showCombatWindow)
                    {
                        showCombatWindowByte = 1;
                    }
                    streamClass.AddByte(showCombatWindowByte);
                    streamClass.FormatPacket();
                }
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    fogOfWar = !fogOfWar;
                }
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    autoScreenshot = !autoScreenshot;
                    streamClass.CreatePacket(157);
                    streamClass.AddByte(5);
                    int autoScreenshotByte = 0;
                    if (autoScreenshot)
                    {
                        autoScreenshotByte = 1;
                    }
                    streamClass.AddByte(autoScreenshotByte);
                    streamClass.FormatPacket();
                }
                bool flag = false;
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    useChatFilter = !useChatFilter;
                }
                i2 += 15;
                i2 += 15;
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    blockChat = 1 - blockChat;
                    flag = true;
                }
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    blockPrivate = 1 - blockPrivate;
                    flag = true;
                }
                i2 += 15;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    blockTrade = 1 - blockTrade;
                    flag = true;
                }
                i2 += 15;
                if (Config.MembersFeatures && mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    blockDuel = 1 - blockDuel;
                    flag = true;
                }
                i2 += 15;
                if (flag)
                {
                    SendUpdatedPrivacyInfo(blockChat, blockPrivate, blockTrade, blockDuel);
                }

                i2 += 20;
                if (mouseX > k1 && mouseX < k1 + c2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseButtonClick == 1)
                {
                    SendLogout();
                }

                mouseButtonClick = 0;
            }
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
                    if (l <= 0 || mouseX <= byte0 || mouseX >= byte0 + c1 || mouseY <= byte1 + l * 20 || mouseY >= byte1 + l * 20 + 20)
                    {
                        continue;
                    }

                    combatStyle = l - 1;
                    mouseButtonClick = 0;
                    streamClass.CreatePacket(42);
                    streamClass.AddByte(combatStyle);
                    streamClass.FormatPacket();
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
                int mx = mouseX - 22;
                int my = mouseY - 36;
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
                                    if (GameData.itemStackable[item] == 0)
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

                            if (GameData.itemSpecial[item] == 1)
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
                                streamClass.CreatePacket(70);
                                streamClass.AddByte(tradeItemsOurCount);
                                for (int i = 0; i < tradeItemsOurCount; i += 1)
                                {
                                    streamClass.AddShort(tradeItemsOur[i]);
                                    streamClass.AddInt(tradeItemOurCount[i]);
                                }
                                streamClass.FormatPacket();
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
                                if (GameData.itemStackable[item] == 0 && tradeItemOurCount[curItem] > 1)
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
                            streamClass.CreatePacket(70);
                            streamClass.AddByte(tradeItemsOurCount);
                            for (int i = 0; i < tradeItemsOurCount; i += 1)
                            {
                                streamClass.AddShort(tradeItemsOur[i]);
                                streamClass.AddInt(tradeItemOurCount[i]);
                            }
                            streamClass.FormatPacket();
                            tradeOtherAccepted = false;
                            tradeWeAccepted = false;
                        }
                    }
                    if (mx >= 217 && my >= 238 && mx <= 286 && my <= 259)
                    {
                        tradeWeAccepted = true;
                        streamClass.CreatePacket(211);
                        streamClass.FormatPacket();
                    }
                    if (mx >= 394 && my >= 238 && mx < 463 && my < 259)
                    {
                        showTradeBox = false;
                        streamClass.CreatePacket(216);
                        streamClass.FormatPacket();
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
                gameGraphics.DrawImage(l5, j6, 48, 32, baseItemPicture + GameData.itemInventoryPicture[inventoryItems[k5]], GameData.itemPictureMask[inventoryItems[k5]], 0, 0, false);
                if (GameData.itemStackable[inventoryItems[k5]] == 0)
                {
                    gameGraphics.DrawString(inventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < tradeItemsOurCount; i6 += 1)
            {
                int k6 = 9 + byte0 + (i6 % 4) * 49;
                int i7 = 31 + byte1 + (i6 / 4) * 34;
                gameGraphics.DrawImage(k6, i7, 48, 32, baseItemPicture + GameData.itemInventoryPicture[tradeItemsOur[i6]], GameData.itemPictureMask[tradeItemsOur[i6]], 0, 0, false);
                if (GameData.itemStackable[tradeItemsOur[i6]] == 0)
                {
                    gameGraphics.DrawString(tradeItemOurCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (mouseX > k6 && mouseX < k6 + 48 && mouseY > i7 && mouseY < i7 + 32)
                {
                    gameGraphics.DrawString(GameData.itemName[tradeItemsOur[i6]] + ": @whi@" + GameData.itemDescription[tradeItemsOur[i6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < tradeItemsOtherCount; l6 += 1)
            {
                int j7 = 9 + byte0 + (l6 % 4) * 49;
                int k7 = 156 + byte1 + (l6 / 4) * 34;
                gameGraphics.DrawImage(j7, k7, 48, 32, baseItemPicture + GameData.itemInventoryPicture[tradeItemsOther[l6]], GameData.itemPictureMask[tradeItemsOther[l6]], 0, 0, false);
                if (GameData.itemStackable[tradeItemsOther[l6]] == 0)
                {
                    gameGraphics.DrawString(tradeItemOtherCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (mouseX > j7 && mouseX < j7 + 48 && mouseY > k7 && mouseY < k7 + 32)
                {
                    gameGraphics.DrawString(GameData.itemName[tradeItemsOther[l6]] + ": @whi@" + GameData.itemDescription[tradeItemsOther[l6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

        }
        public void DrawLogoutBox()
        {
            gameGraphics.DrawBox(126, 137, 260, 60, 0);
            gameGraphics.DrawBoxEdge(126, 137, 260, 60, 0xffffff);
            gameGraphics.DrawText("Logging out...", 256, 173, 5, 0xffffff);
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
        public void DrawQuestionMenu()
        {
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < questionMenuCount; l += 1)
                {
                    if (mouseX >= gameGraphics.TextWidth(questionMenuAnswer[l], 1) || mouseY <= l * 12 || mouseY >= 12 + l * 12)
                    {
                        continue;
                    }

                    streamClass.CreatePacket(154);
                    streamClass.AddByte(l);
                    streamClass.FormatPacket();
                    break;
                }

                mouseButtonClick = 0;
                showQuestionMenu = false;
                return;
            }
            for (int i1 = 0; i1 < questionMenuCount; i1 += 1)
            {
                int j1 = 65535;
                if (mouseX < gameGraphics.TextWidth(questionMenuAnswer[i1], 1) && mouseY > i1 * 12 && mouseY < 12 + i1 * 12)
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
                string s1 = GameData.itemName[tradeConfirmItems[i1]];
                if (GameData.itemStackable[tradeConfirmItems[i1]] == 0)
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
                string s2 = GameData.itemName[tradeConfirmOtherItems[j1]];
                if (GameData.itemStackable[tradeConfirmOtherItems[j1]] == 0)
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
                if (mouseX < byte0 || mouseY < byte1 || mouseX > byte0 + 468 || mouseY > byte1 + 262)
                {
                    //showTradeConfirmBox = false;
                    //base.streamClass.CreatePacket(216);
                    //base.streamClass.FormatPacket();
                }
                if (mouseX >= (byte0 + 118) - 35 && mouseX <= byte0 + 118 + 70 && mouseY >= byte1 + 238 && mouseY <= byte1 + 238 + 21)
                {
                    tradeConfirmAccepted = true;
                    streamClass.CreatePacket(53);
                    streamClass.FormatPacket();
                }
                if (mouseX >= (byte0 + 352) - 35 && mouseX <= byte0 + 353 + 70 && mouseY >= byte1 + 238 && mouseY <= byte1 + 238 + 21)
                {
                    showTradeConfirmBox = false;
                    streamClass.CreatePacket(216);
                    streamClass.FormatPacket();
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
            int picture = GameData.itemInventoryPicture[itemID] + baseItemPicture;
            int mask = GameData.itemPictureMask[itemID];
            gameGraphics.DrawImage(x, y, width, height, picture, mask, 0, 0, false);
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
                for (int l1 = 0; l1 < friendsCount; l1 += 1)
                {
                    string s1;
                    if (friendsWorld[l1] == 99)
                    {
                        s1 = "@gre@";
                    }
                    else
                        if (friendsWorld[l1] > 0)
                    {
                        s1 = "@yel@";
                    }
                    else
                    {
                        s1 = "@red@";
                    }

                    friendsMenu.AddListItem(friendsMenuHandle, l1, s1 + DataOperations.HashToName(friendsList[l1]) + "~439~@whi@Remove         WWWWWWWWWW");
                }

            }
            if (friendsIgnoreMenuSelected == 1)
            {
                for (int i2 = 0; i2 < ignoresCount; i2 += 1)
                {
                    friendsMenu.AddListItem(friendsMenuHandle, i2, "@yel@" + DataOperations.HashToName(ignoresList[i2]) + "~439~@whi@Remove         WWWWWWWWWW");
                }
            }
            friendsMenu.DrawMenu();
            if (friendsIgnoreMenuSelected == 0)
            {
                int j2 = friendsMenu.GetEntryHighlighted(friendsMenuHandle);
                if (j2 >= 0 && mouseX < 489)
                {
                    if (mouseX > 429)
                    {
                        gameGraphics.DrawText("Click to remove " + DataOperations.HashToName(friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                        if (friendsWorld[j2] == 99)
                    {
                        gameGraphics.DrawText("Click to message " + DataOperations.HashToName(friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                            if (friendsWorld[j2] > 0)
                    {
                        gameGraphics.DrawText(DataOperations.HashToName(friendsList[j2]) + " is on world " + friendsWorld[j2], l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                    {
                        gameGraphics.DrawText(DataOperations.HashToName(friendsList[j2]) + " is offline", l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    gameGraphics.DrawText("Click a name to send a message", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int j3;
                if (mouseX > l && mouseX < l + c1 && mouseY > (i1 + c2) - 16 && mouseY < i1 + c2)
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
                if (k2 >= 0 && mouseX < 489 && mouseX > 429)
                {
                    if (mouseX > 429)
                    {
                        gameGraphics.DrawText("Click to remove " + DataOperations.HashToName(ignoresList[k2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    gameGraphics.DrawText("Blocking messages from:", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int k3;
                if (mouseX > l && mouseX < l + c1 && mouseY > (i1 + c2) - 16 && mouseY < i1 + c2)
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

            l = mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                friendsMenu.MouseClick(l + (((GameImage)(gameGraphics)).gameWidth - 199), i1 + 36, lastMouseButton, mouseButton);
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
                    if (l2 >= 0 && mouseX < 489)
                    {
                        if (mouseX > 429)
                        {
                            RemoveFriend(friendsList[l2]);
                        }
                        else
                            if (friendsWorld[l2] != 0)
                            {
                                showFriendsBox = 2;
                                pmTarget = friendsList[l2];
                                pmText = "";
                                enteredPMText = "";
                            }
                    }
                }
                if (mouseButtonClick == 1 && friendsIgnoreMenuSelected == 1)
                {
                    int i3 = friendsMenu.GetEntryHighlighted(friendsMenuHandle);
                    if (i3 >= 0 && mouseX < 489 && mouseX > 429)
                    {
                        RemoveIgnore(ignoresList[i3]);
                    }
                }
                if (i1 > 166 && mouseButtonClick == 1 && friendsIgnoreMenuSelected == 0)
                {
                    showFriendsBox = 1;
                    inputText = "";
                    enteredInputText = "";
                }
                if (i1 > 166 && mouseButtonClick == 1 && friendsIgnoreMenuSelected == 1)
                {
                    showFriendsBox = 3;
                    inputText = "";
                    enteredInputText = "";
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
                for (int l2 = 0; l2 < GameData.spellCount; l2 += 1)
                {
                    string s1 = "@yel@";
                    for (int k4 = 0; k4 < GameData.spellDifferentRuneCount[l2]; k4 += 1)
                    {
                        int j5 = GameData.spellRequiredRuneIds[l2][k4];
                        if (HasRequiredRunes(j5, GameData.spellRequiredRuneCount[l2][k4]))
                        {
                            continue;
                        }

                        s1 = "@whi@";
                        break;
                    }

                    int k5 = playerStatCurrent[6];
                    if (GameData.spellRequiredLevel[l2] > k5)
                    {
                        s1 = "@normalZ@";
                    }

                    spellMenu.AddListItem(spellMenuHandle, l1++, s1 + "Level " + GameData.spellRequiredLevel[l2] + ": " + GameData.spellName[l2]);
                }

                spellMenu.DrawMenu();
                int l3 = spellMenu.GetEntryHighlighted(spellMenuHandle);
                if (l3 != -1)
                {
                    gameGraphics.DrawString("Level " + GameData.spellRequiredLevel[l3] + ": " + GameData.spellName[l3], l + 2, i1 + 124, 1, 0xffff00);
                    gameGraphics.DrawString(GameData.spellDescription[l3], l + 2, i1 + 136, 0, 0xffffff);
                    for (int l4 = 0; l4 < GameData.spellDifferentRuneCount[l3]; l4 += 1)
                    {
                        int l5 = GameData.spellRequiredRuneIds[l3][l4];
                        gameGraphics.DrawPicture(l + 2 + l4 * 44, i1 + 150, baseItemPicture + GameData.itemInventoryPicture[l5]);
                        int i6 = GetInventoryItemTotalCount(l5);
                        int j6 = GameData.spellRequiredRuneCount[l3][l4];
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
                for (int i3 = 0; i3 < GameData.prayerCount; i3 += 1)
                {
                    string s2 = "@whi@";
                    if (GameData.prayerRequiredLevel[i3] > playerStatBase[5])
                    {
                        s2 = "@normalZ@";
                    }

                    if (prayerOn[i3])
                    {
                        s2 = "@gre@";
                    }

                    spellMenu.AddListItem(spellMenuHandle, i2++, s2 + "Level " + GameData.prayerRequiredLevel[i3] + ": " + GameData.prayerName[i3]);
                }

                spellMenu.DrawMenu();
                int i4 = spellMenu.GetEntryHighlighted(spellMenuHandle);
                if (i4 != -1)
                {
                    gameGraphics.DrawText("Level " + GameData.prayerRequiredLevel[i4] + ": " + GameData.prayerName[i4], l + c1 / 2, i1 + 130, 1, 0xffff00);
                    gameGraphics.DrawText(GameData.prayerDescription[i4], l + c1 / 2, i1 + 145, 0, 0xffffff);
                    gameGraphics.DrawText("Drain rate: " + GameData.prayerDrainRate[i4], l + c1 / 2, i1 + 160, 1, 0);
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

            l = mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                spellMenu.MouseClick(l + (((GameImage)(gameGraphics)).gameWidth - 199), i1 + 36, lastMouseButton, mouseButton);
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
                        if (GameData.spellRequiredLevel[j2] > j3)
                        {
                            DisplayMessage("Your magic ability is not high enough for this spell", 3);
                        }
                        else
                        {
                            int j4;
                            for (j4 = 0; j4 < GameData.spellDifferentRuneCount[j2]; j4 += 1)
                            {
                                int i5 = GameData.spellRequiredRuneIds[j2][j4];
                                if (HasRequiredRunes(i5, GameData.spellRequiredRuneCount[j2][j4]))
                                {
                                    continue;
                                }

                                DisplayMessage("You don't have all the reagents you need for this spell", 3);
                                j4 = -1;
                                break;
                            }

                            if (j4 == GameData.spellDifferentRuneCount[j2])
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
                        if (GameData.prayerRequiredLevel[k2] > k3)
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
                                    streamClass.CreatePacket(248);
                                    streamClass.AddByte(k2);
                                    streamClass.FormatPacket();
                                    prayerOn[k2] = false;
                                    PlaySound("prayeroff");
                                }
                                else
                                {
                                    streamClass.CreatePacket(56);
                                    streamClass.AddByte(k2);
                                    streamClass.FormatPacket();
                                    prayerOn[k2] = true;
                                    PlaySound("prayeron");
                                }
                    }
                }
                mouseButtonClick = 0;
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
        public void DrawShopBox()
        {
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                int l = mouseX - 52;
                int i1 = mouseY - 44;
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
                                streamClass.CreatePacket(128);
                                streamClass.AddShort(shopItems[selectedShopItemIndex]);
                                streamClass.AddInt(shopItemBuyPrice[selectedShopItemIndex]);
                                streamClass.FormatPacket();
                            }
                            if (GetInventoryItemTotalCount(i3) > 0 && l > 2 && i1 >= 229 && l < 112 && i1 <= 240)
                            {
                                streamClass.CreatePacket(255);
                                streamClass.AddShort(shopItems[selectedShopItemIndex]);
                                streamClass.AddInt(shopItemSellPrice[selectedShopItemIndex]);
                                streamClass.FormatPacket();
                            }
                        }
                    }
                }
                else
                {
                    streamClass.CreatePacket(253);
                    streamClass.FormatPacket();
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
            if (mouseX > _offsetX + 320 && mouseY >= _offsetY && mouseX < _offsetX + 408 && mouseY < _offsetY + 12)
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
                        gameGraphics.DrawImage(i6, l6, 48, 32, baseItemPicture + GameData.itemInventoryPicture[shopItems[j4]], GameData.itemPictureMask[shopItems[j4]], 0, 0, false);
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

                    int i7 = (j6 * GameData.itemBasePrice[l5]) / 100;
                    gameGraphics.DrawString("Buy a new " + GameData.itemName[l5] + " for " + i7 + "gp", _offsetX + 2, _offsetY + 214, 1, 0xffff00);
                    int j2 = 0xffffff;
                    if (mouseX > _offsetX + 298 && mouseY >= _offsetY + 204 && mouseX < _offsetX + 408 && mouseY <= _offsetY + 215)
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

                    int j7 = (k6 * GameData.itemBasePrice[l5]) / 100;
                    gameGraphics.DrawLabel("Sell your " + GameData.itemName[l5] + " for " + j7 + "gp", _offsetX + 405, _offsetY + 239, 1, 0xffff00);
                    int k2 = 0xffffff;
                    if (mouseX > _offsetX + 2 && mouseY >= _offsetY + 229 && mouseX < _offsetX + 112 && mouseY <= _offsetY + 240)
                    {
                        k2 = 0xff0000;
                    }

                    gameGraphics.DrawString("Click here to sell", _offsetX + 2, _offsetY + 239, 3, k2);
                    return;
                }
                gameGraphics.DrawText("You do not have any of this item to sell", _offsetX + 204, _offsetY + 239, 3, 0xffff00);
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
            gameGraphics.DrawCharacterLegs(l - 32 - 55, i1, 64, 102, GameData.animationNumber[appearance2Colour], appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, GameData.animationNumber[appearanceBodyGender], appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, GameData.animationNumber[appearanceHeadType], appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawCharacterLegs(l - 32, i1, 64, 102, GameData.animationNumber[appearance2Colour] + 6, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage(l - 32, i1, 64, 102, GameData.animationNumber[appearanceBodyGender] + 6, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage(l - 32, i1, 64, 102, GameData.animationNumber[appearanceHeadType] + 6, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawCharacterLegs((l - 32) + 55, i1, 64, 102, GameData.animationNumber[appearance2Colour] + 12, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage((l - 32) + 55, i1, 64, 102, GameData.animationNumber[appearanceBodyGender] + 12, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage((l - 32) + 55, i1, 64, 102, GameData.animationNumber[appearanceHeadType] + 12, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawPicture(0, windowHeight, baseInventoryPic + 22);
            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.DrawImage(spriteBatch, 0, 0);
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
                gameGraphics.DrawText(inputText + "*", windowWidth / 2, 180, 5, 65535);
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
                        int k10 = -engineHandle.GetAveragedElevation(k9, j10) - GameData.npcCameraArray2[targetMob.npcId] / 2;
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
                int l9 = gameCamera.AddSpriteToScene(20000 + k2, x1, y1, z1, GameData.npcCameraArray1[npc.npcId], GameData.npcCameraArray2[npc.npcId], k2 + 30000);
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
            gameGraphics.interlace = keyF1Toggle;
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
                    if (!keyF1Toggle)
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
        public void DrawReportAbuseBox2()
        {
            if (enteredInputText.Length > 0)
            {
                string s1 = enteredInputText.Trim();
                inputText = "";
                enteredInputText = "";
                if (s1.Length > 0)
                {
                    long l1 = DataOperations.NameToHash(s1);
                    streamClass.CreatePacket(7);
                    streamClass.AddLong(l1);
                    streamClass.AddByte(reportAbuseOptionSelected);
                    //base.streamClass.AddByte(dia ? 1 : 0);
                    streamClass.FormatPacket();
                }
                showAbuseBox = 0;
                return;
            }
            gameGraphics.DrawBox(56, 130, 400, 100, 0);
            gameGraphics.DrawBoxEdge(56, 130, 400, 100, 0xffffff);
            int l = 160;
            gameGraphics.DrawText("Now type the name of the offending player, and press enter", 256, l, 1, 0xffff00);
            l += 18;
            gameGraphics.DrawText("Name: " + inputText + "*", 256, l, 4, 0xffffff);
            l = 222;
            int i1 = 0xffffff;
            if (mouseX > 196 && mouseX < 316 && mouseY > l - 13 && mouseY < l + 2)
            {
                i1 = 0xffff00;
                if (mouseButtonClick == 1)
                {
                    mouseButtonClick = 0;
                    showAbuseBox = 0;
                }
            }
            gameGraphics.DrawText("Click here to cancel", 256, l, 1, i1);
            if (mouseButtonClick == 1 && (mouseX < 56 || mouseX > 456 || mouseY < 130 || mouseY > 230))
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
        public void DrawDuelBox()
        {
            if (mouseButtonClick != 0 && mouseClickedHeldInTradeDuelBox == 0)
            {
                mouseClickedHeldInTradeDuelBox = 1;
            }

            if (mouseClickedHeldInTradeDuelBox > 0)
            {
                int l = mouseX - 22;
                int i1 = mouseY - 36;
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
                                    if (GameData.itemStackable[j3] == 0)
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

                            if (GameData.itemSpecial[j3] == 1)
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
                                streamClass.CreatePacket(123);
                                streamClass.AddByte(duelMyItemCount);
                                for (int i5 = 0; i5 < duelMyItemCount; i5 += 1)
                                {
                                    streamClass.AddShort(duelMyItems[i5]);
                                    streamClass.AddInt(duelMyItemsCount[i5]);
                                }

                                streamClass.FormatPacket();
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
                                if (GameData.itemStackable[i2] == 0 && duelMyItemsCount[k1] > 1)
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

                            streamClass.CreatePacket(123);
                            streamClass.AddByte(duelMyItemCount);
                            for (int l3 = 0; l3 < duelMyItemCount; l3 += 1)
                            {
                                streamClass.AddShort(duelMyItems[l3]);
                                streamClass.AddInt(duelMyItemsCount[l3]);
                            }

                            streamClass.FormatPacket();
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
                        streamClass.CreatePacket(225);
                        int duelNoRetreatingByte = 0;
                        if (duelNoRetreating)
                        {
                            duelNoRetreatingByte = 1;
                        }
                        streamClass.AddByte(duelNoRetreatingByte);
                        int duelNoMagicByte = 0;
                        if (duelNoMagic)
                        {
                            duelNoMagicByte = 1;
                        }
                        streamClass.AddByte(duelNoMagicByte);
                        int duelNoPrayerByte = 0;
                        if (duelNoPrayer)
                        {
                            duelNoPrayerByte = 1;
                        }
                        streamClass.AddByte(duelNoPrayerByte);
                        int duelNoWeaponsByte = 0;
                        if (duelNoWeapons)
                        {
                            duelNoWeaponsByte = 1;
                        }
                        streamClass.AddByte(duelNoWeaponsByte);
                        streamClass.FormatPacket();
                        duelOpponentAccepted = false;
                        duelMyAccepted = false;
                    }
                    if (l >= 217 && i1 >= 238 && l <= 286 && i1 <= 259)
                    {
                        duelMyAccepted = true;
                        streamClass.CreatePacket(252);
                        streamClass.FormatPacket();
                    }
                    if (l >= 394 && i1 >= 238 && l < 463 && i1 < 259)
                    {
                        showDuelBox = false;
                        streamClass.CreatePacket(35);
                        streamClass.FormatPacket();
                    }
                }
                else
                    if (mouseButtonClick != 0)
                    {
                        showDuelBox = false;
                        streamClass.CreatePacket(35);
                        streamClass.FormatPacket();
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
                gameGraphics.DrawImage(l5, j6, 48, 32, baseItemPicture + GameData.itemInventoryPicture[inventoryItems[k5]], GameData.itemPictureMask[inventoryItems[k5]], 0, 0, false);
                if (GameData.itemStackable[inventoryItems[k5]] == 0)
                {
                    gameGraphics.DrawString(inventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < duelMyItemCount; i6 += 1)
            {
                int k6 = 9 + byte0 + (i6 % 4) * 49;
                int i7 = 31 + byte1 + (i6 / 4) * 34;
                gameGraphics.DrawImage(k6, i7, 48, 32, baseItemPicture + GameData.itemInventoryPicture[duelMyItems[i6]], GameData.itemPictureMask[duelMyItems[i6]], 0, 0, false);
                if (GameData.itemStackable[duelMyItems[i6]] == 0)
                {
                    gameGraphics.DrawString(duelMyItemsCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (mouseX > k6 && mouseX < k6 + 48 && mouseY > i7 && mouseY < i7 + 32)
                {
                    gameGraphics.DrawString(GameData.itemName[duelMyItems[i6]] + ": @whi@" + GameData.itemDescription[duelMyItems[i6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < duelOpponentItemCount; l6 += 1)
            {
                int j7 = 9 + byte0 + (l6 % 4) * 49;
                int k7 = 124 + byte1 + (l6 / 4) * 34;
                gameGraphics.DrawImage(j7, k7, 48, 32, baseItemPicture + GameData.itemInventoryPicture[duelOpponentItems[l6]], GameData.itemPictureMask[duelOpponentItems[l6]], 0, 0, false);
                if (GameData.itemStackable[duelOpponentItems[l6]] == 0)
                {
                    gameGraphics.DrawString(duelOpponentItemsCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (mouseX > j7 && mouseX < j7 + 48 && mouseY > k7 && mouseY < k7 + 32)
                {
                    gameGraphics.DrawString(GameData.itemName[duelOpponentItems[l6]] + ": @whi@" + GameData.itemDescription[duelOpponentItems[l6]], byte0 + 8, byte1 + 273, 1, 0xffff00);
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
            if (mouseY > l - 12 && mouseY <= l && mouseX > 181 && mouseX < 331)
            {
                i1 = 0xff0000;
            }

            gameGraphics.DrawText("Click here to close window", 256, l, 1, i1);
            if (mouseButtonClick != 0)
            {
                if (mouseY > l - 12 && mouseY <= l && mouseX > 181 && mouseX < 331)
                {
                    wildType = 2;
                }

                if (mouseX < 86 || mouseX > 426 || mouseY < 77 || mouseY > 257)
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
            int j1 = newFrameIndex * 3 + walkModel[(npc.stepCount / GameData.npcWalkModelArray[npc.npcId]) % 4];
            if (npc.currentSprite == 8)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = false;
                x -= (GameData.npcCombatSprite[npc.npcId] * unknown2) / 100;
                j1 = newFrameIndex * 3 + combatModelArray1[(tick / (GameData.npcCombatModel[npc.npcId] - 1)) % 8];
            }
            else
                if (npc.currentSprite == 9)
                {
                    newFrameIndex = 5;
                    frameIndex = 2;
                    flag = true;
                    x += (GameData.npcCombatSprite[npc.npcId] * unknown2) / 100;
                    j1 = newFrameIndex * 3 + combatModelArray2[(tick / GameData.npcCombatModel[npc.npcId]) % 8];
                }
            for (int k1 = 0; k1 < 12; k1 += 1)
            {
                int l1 = animationModelArray[frameIndex][k1];
                int k2 = GameData.npcAnimationCount[npc.npcId][l1];
                if (k2 >= 0)
                {
                    int i3 = 0;
                    int j3 = 0;
                    int k3 = j1;
                    if (flag && newFrameIndex >= 1 && newFrameIndex <= 3 && GameData.animationHasF[k2] == 1)
                    {
                        k3 += 15;
                    }

                    if (newFrameIndex != 5 || GameData.animationHasA[k2] == 1)
                    {
                        int l3 = k3 + GameData.animationNumber[k2];
                        i3 = (i3 * width) / ((GameImage)(gameGraphics)).pictureAssumedWidth[l3];
                        j3 = (j3 * height) / ((GameImage)(gameGraphics)).pictureAssumedHeight[l3];
                        int i4 = (width * ((GameImage)(gameGraphics)).pictureAssumedWidth[l3]) / ((GameImage)(gameGraphics)).pictureAssumedWidth[GameData.animationNumber[k2]];
                        i3 -= (i4 - width) / 2;
                        int j4 = GameData.animationCharacterColor[k2];
                        int k4 = 0;
                        if (j4 == 1)
                        {
                            j4 = GameData.npcHairColor[npc.npcId];
                            k4 = GameData.npcSkinColor[npc.npcId];
                        }
                        else
                            if (j4 == 2)
                            {
                                j4 = GameData.npcTopColor[npc.npcId];
                                k4 = GameData.npcSkinColor[npc.npcId];
                            }
                            else
                                if (j4 == 3)
                                {
                                    j4 = GameData.npcBottomColor[npc.npcId];
                                    k4 = GameData.npcSkinColor[npc.npcId];
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
                gameGraphics.DrawImage(x - k5 / 2, (j5 + height / 2) - l5 / 2, k5, l5, GameData.itemInventoryPicture[id] + baseItemPicture, GameData.itemPictureMask[id], 0, 0, false);
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
                int l = mouseX - (256 - c1 / 2);
                int j1 = mouseY - (170 - c2 / 2);
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
                        if (GameData.itemStackable[id] == 1 && count > 1)
                        {
                            count = 1;
                        }

                        if (count >= 1 && mouseX >= l + 220 && mouseY >= j1 + 238 && mouseX < l + 250 && mouseY <= j1 + 249)
                        {
                            streamClass.CreatePacket(183);
                            streamClass.AddShort(id);
                            streamClass.AddInt(1);
                            streamClass.FormatPacket();
                        }
                        if (count >= 5 && mouseX >= l + 250 && mouseY >= j1 + 238 && mouseX < l + 280 && mouseY <= j1 + 249)
                        {
                            streamClass.CreatePacket(183);
                            streamClass.AddShort(id);
                            streamClass.AddInt(5);
                            streamClass.FormatPacket();
                        }
                        if (count >= 25 && mouseX >= l + 280 && mouseY >= j1 + 238 && mouseX < l + 305 && mouseY <= j1 + 249)
                        {
                            streamClass.CreatePacket(183);
                            streamClass.AddShort(id);
                            streamClass.AddInt(25);
                            streamClass.FormatPacket();
                        }
                        if (count >= 100 && mouseX >= l + 305 && mouseY >= j1 + 238 && mouseX < l + 335 && mouseY <= j1 + 249)
                        {
                            streamClass.CreatePacket(183);
                            streamClass.AddShort(id);
                            streamClass.AddInt(100);
                            streamClass.FormatPacket();
                        }
                        if (count >= 500 && mouseX >= l + 335 && mouseY >= j1 + 238 && mouseX < l + 368 && mouseY <= j1 + 249)
                        {
                            streamClass.CreatePacket(183);
                            streamClass.AddShort(id);
                            streamClass.AddInt(500);
                            streamClass.FormatPacket();
                        }
                        if (count >= 2500 && mouseX >= l + 370 && mouseY >= j1 + 238 && mouseX < l + 400 && mouseY <= j1 + 249)
                        {
                            streamClass.CreatePacket(183);
                            streamClass.AddShort(id);
                            streamClass.AddInt(2500);
                            streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 1 && mouseX >= l + 220 && mouseY >= j1 + 263 && mouseX < l + 250 && mouseY <= j1 + 274)
                        {
                            streamClass.CreatePacket(198);
                            streamClass.AddShort(id);
                            streamClass.AddInt(1);
                            streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 5 && mouseX >= l + 250 && mouseY >= j1 + 263 && mouseX < l + 280 && mouseY <= j1 + 274)
                        {
                            streamClass.CreatePacket(198);
                            streamClass.AddShort(id);
                            streamClass.AddInt(5);
                            streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 25 && mouseX >= l + 280 && mouseY >= j1 + 263 && mouseX < l + 305 && mouseY <= j1 + 274)
                        {
                            streamClass.CreatePacket(198);
                            streamClass.AddShort(id);
                            streamClass.AddInt(25);
                            streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 100 && mouseX >= l + 305 && mouseY >= j1 + 263 && mouseX < l + 335 && mouseY <= j1 + 274)
                        {
                            streamClass.CreatePacket(198);
                            streamClass.AddShort(id);
                            streamClass.AddInt(100);
                            streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 500 && mouseX >= l + 335 && mouseY >= j1 + 263 && mouseX < l + 368 && mouseY <= j1 + 274)
                        {
                            streamClass.CreatePacket(198);
                            streamClass.AddShort(id);
                            streamClass.AddInt(500);
                            streamClass.FormatPacket();
                        }
                        if (GetInventoryItemTotalCount(id) >= 2500 && mouseX >= l + 370 && mouseY >= j1 + 263 && mouseX < l + 400 && mouseY <= j1 + 274)
                        {
                            streamClass.CreatePacket(198);
                            streamClass.AddShort(id);
                            streamClass.AddInt(2500);
                            streamClass.FormatPacket();
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
                                    streamClass.CreatePacket(48);
                                    streamClass.FormatPacket();
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
                    if (mouseX > i1 + l2 && mouseY >= k1 && mouseX < i1 + l2 + 65 && mouseY < k1 + 12)
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
                    if (mouseX > i1 + l2 && mouseY >= k1 && mouseX < i1 + l2 + 65 && mouseY < k1 + 12)
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
                    if (mouseX > i1 + l2 && mouseY >= k1 && mouseX < i1 + l2 + 65 && mouseY < k1 + 12)
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
                    if (mouseX > i1 + l2 && mouseY >= k1 && mouseX < i1 + l2 + 65 && mouseY < k1 + 12)
                {
                    i4 = 0xffff00;
                }

                gameGraphics.DrawString("<page 4>", i1 + l2, k1 + 10, 1, i4);
                l2 += 65;
            }
            int j4 = 0xffffff;
            if (mouseX > i1 + 320 && mouseY >= k1 && mouseX < i1 + 408 && mouseY < k1 + 12)
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
                        gameGraphics.DrawImage(k9, l9, 48, 32, baseItemPicture + GameData.itemInventoryPicture[bankItems[j8]], GameData.itemPictureMask[bankItems[j8]], 0, 0, false);
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
                if (GameData.itemStackable[j9] == 1 && k8 > 1)
                {
                    k8 = 1;
                }

                if (k8 > 0)
                {
                    gameGraphics.DrawString("Withdraw " + GameData.itemName[j9], i1 + 2, k1 + 248, 1, 0xffffff);
                    int k4 = 0xffffff;
                    if (mouseX >= i1 + 220 && mouseY >= k1 + 238 && mouseX < i1 + 250 && mouseY <= k1 + 249)
                    {
                        k4 = 0xff0000;
                    }

                    gameGraphics.DrawString("One", i1 + 222, k1 + 248, 1, k4);
                    if (k8 >= 5)
                    {
                        int l4 = 0xffffff;
                        if (mouseX >= i1 + 250 && mouseY >= k1 + 238 && mouseX < i1 + 280 && mouseY <= k1 + 249)
                        {
                            l4 = 0xff0000;
                        }

                        gameGraphics.DrawString("Five", i1 + 252, k1 + 248, 1, l4);
                    }
                    if (k8 >= 25)
                    {
                        int i5 = 0xffffff;
                        if (mouseX >= i1 + 280 && mouseY >= k1 + 238 && mouseX < i1 + 305 && mouseY <= k1 + 249)
                        {
                            i5 = 0xff0000;
                        }

                        gameGraphics.DrawString("25", i1 + 282, k1 + 248, 1, i5);
                    }
                    if (k8 >= 100)
                    {
                        int j5 = 0xffffff;
                        if (mouseX >= i1 + 305 && mouseY >= k1 + 238 && mouseX < i1 + 335 && mouseY <= k1 + 249)
                        {
                            j5 = 0xff0000;
                        }

                        gameGraphics.DrawString("100", i1 + 307, k1 + 248, 1, j5);
                    }
                    if (k8 >= 500)
                    {
                        int k5 = 0xffffff;
                        if (mouseX >= i1 + 335 && mouseY >= k1 + 238 && mouseX < i1 + 368 && mouseY <= k1 + 249)
                        {
                            k5 = 0xff0000;
                        }

                        gameGraphics.DrawString("500", i1 + 337, k1 + 248, 1, k5);
                    }
                    if (k8 >= 2500)
                    {
                        int l5 = 0xffffff;
                        if (mouseX >= i1 + 370 && mouseY >= k1 + 238 && mouseX < i1 + 400 && mouseY <= k1 + 249)
                        {
                            l5 = 0xff0000;
                        }

                        gameGraphics.DrawString("2500", i1 + 370, k1 + 248, 1, l5);
                    }
                }
                if (GetInventoryItemTotalCount(j9) > 0)
                {
                    gameGraphics.DrawString("Deposit " + GameData.itemName[j9], i1 + 2, k1 + 273, 1, 0xffffff);
                    int i6 = 0xffffff;
                    if (mouseX >= i1 + 220 && mouseY >= k1 + 263 && mouseX < i1 + 250 && mouseY <= k1 + 274)
                    {
                        i6 = 0xff0000;
                    }

                    gameGraphics.DrawString("One", i1 + 222, k1 + 273, 1, i6);
                    if (GetInventoryItemTotalCount(j9) >= 5)
                    {
                        int j6 = 0xffffff;
                        if (mouseX >= i1 + 250 && mouseY >= k1 + 263 && mouseX < i1 + 280 && mouseY <= k1 + 274)
                        {
                            j6 = 0xff0000;
                        }

                        gameGraphics.DrawString("Five", i1 + 252, k1 + 273, 1, j6);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 25)
                    {
                        int k6 = 0xffffff;
                        if (mouseX >= i1 + 280 && mouseY >= k1 + 263 && mouseX < i1 + 305 && mouseY <= k1 + 274)
                        {
                            k6 = 0xff0000;
                        }

                        gameGraphics.DrawString("25", i1 + 282, k1 + 273, 1, k6);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 100)
                    {
                        int l6 = 0xffffff;
                        if (mouseX >= i1 + 305 && mouseY >= k1 + 263 && mouseX < i1 + 335 && mouseY <= k1 + 274)
                        {
                            l6 = 0xff0000;
                        }

                        gameGraphics.DrawString("100", i1 + 307, k1 + 273, 1, l6);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 500)
                    {
                        int i7 = 0xffffff;
                        if (mouseX >= i1 + 335 && mouseY >= k1 + 263 && mouseX < i1 + 368 && mouseY <= k1 + 274)
                        {
                            i7 = 0xff0000;
                        }

                        gameGraphics.DrawString("500", i1 + 337, k1 + 273, 1, i7);
                    }
                    if (GetInventoryItemTotalCount(j9) >= 2500)
                    {
                        int j7 = 0xffffff;
                        if (mouseX >= i1 + 370 && mouseY >= k1 + 263 && mouseX < i1 + 400 && mouseY <= k1 + 274)
                        {
                            j7 = 0xff0000;
                        }

                        gameGraphics.DrawString("2500", i1 + 370, k1 + 273, 1, j7);
                    }
                }
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
            if (mouseY > l - 12 && mouseY <= l && mouseX > 106 && mouseX < 406)
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

                if ((mouseX < 256 - c1 / 2 || mouseX > 256 + c1 / 2) && (mouseY < 167 - c2 / 2 || mouseY > 167 + c2 / 2))
                {
                    showServerMessageBox = false;
                }
            }
            mouseButtonClick = 0;
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
                    if (mouseX > l + 3 && mouseY >= l1 - 11 && mouseY < l1 + 2 && mouseX < l + 90)
                    {
                        l2 = 0xff0000;
                        j2 = k2;
                    }
                    gameGraphics.DrawString(skillName[k2] + ":@yel@" + playerStatCurrent[k2] + "/" + playerStatBase[k2], l + 5, l1, 1, l2);
                    l2 = 0xffffff;
                    if (mouseX >= l + 90 && mouseY >= l1 - 13 - 11 && mouseY < (l1 - 13) + 2 && mouseX < l + 196)
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

            l = mouseX - (((GameImage)(gameGraphics)).gameWidth - 199);
            i1 = mouseY - 36;
            if (l >= 0 && i1 >= 0 && l < c1 && i1 < c2)
            {
                if (questMenuSelected == 1)
                {
                    questMenu.MouseClick(l + (((GameImage)(gameGraphics)).gameWidth - 199), i1 + 36, lastMouseButton, mouseButton);
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
                if (showFriendsBox == 1 && (mouseX < 106 || mouseY < 145 || mouseX > 406 || mouseY > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (showFriendsBox == 2 && (mouseX < 6 || mouseY < 145 || mouseX > 506 || mouseY > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (showFriendsBox == 3 && (mouseX < 106 || mouseY < 145 || mouseX > 406 || mouseY > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (mouseX > 236 && mouseX < 276 && mouseY > 193 && mouseY < 213)
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
                gameGraphics.DrawText(inputText + "*", 256, l, 4, 0xffffff);
                if (enteredInputText.Length > 0)
                {
                    string s1 = enteredInputText.Trim();
                    inputText = "";
                    enteredInputText = "";
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
                gameGraphics.DrawText(pmText + "*", 256, l, 4, 0xffffff);
                if (enteredPMText.Length > 0)
                {
                    string s2 = enteredPMText;
                    pmText = "";
                    enteredPMText = "";
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
                gameGraphics.DrawText(inputText + "*", 256, l, 4, 0xffffff);
                if (enteredInputText.Length > 0)
                {
                    string s3 = enteredInputText.Trim();
                    inputText = "";
                    enteredInputText = "";
                    showFriendsBox = 0;
                    if (s3.Length > 0 && DataOperations.NameToHash(s3) != ourPlayer.nameHash)
                    {
                        AddIgnore(s3);
                    }
                }
            }
            int i1 = 0xffffff;
            if (mouseX > 236 && mouseX < 276 && mouseY > 193 && mouseY < 213)
            {
                i1 = 0xffff00;
            }

            gameGraphics.DrawText("Cancel", 256, 208, 1, i1);
        }
        public void DrawRightClickMenu()
        {
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < menuOptionsCount; l += 1)
                {
                    int j1 = menuX + 2;
                    int l1 = menuY + 27 + l * 15;
                    if (mouseX <= j1 - 2 || mouseY <= l1 - 12 || mouseY >= l1 + 4 || mouseX >= (j1 - 3) + menuWidth)
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
            if (mouseX < menuX - 10 || mouseY < menuY - 10 || mouseX > menuX + menuWidth + 10 || mouseY > menuY + menuHeight + 10)
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
                if (mouseX > k1 - 2 && mouseY > i2 - 12 && mouseY < i2 + 4 && mouseX < (k1 - 3) + menuWidth)
                {
                    j2 = 0xffff00;
                }

                string menuSecondaryText = menuText2[menuIndexes[i1]];
                gameGraphics.DrawString(menuText1[menuIndexes[i1]] + " " + menuText2[menuIndexes[i1]], k1, i2, 1, j2);
            }

        }
    }

}
