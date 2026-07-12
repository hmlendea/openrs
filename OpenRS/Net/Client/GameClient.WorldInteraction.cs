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
                streamClass.CreatePacket(104);
                streamClass.AddShort(actionVar1);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 210)
            {
                WalkToGroundItem(sectionX, sectionY, actionX, actionY, true);
                streamClass.CreatePacket(34);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddShort(actionType);
                streamClass.AddShort(actionVar1);
                streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 220)
            {
                WalkToGroundItem(sectionX, sectionY, actionX, actionY, true);
                streamClass.CreatePacket(245);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddShort(actionType);
                streamClass.AddShort(actionVar1);
                streamClass.FormatPacket();
            }
            if (actionID == 3200)
            {
                DisplayMessage(GameData.itemDescription[actionType], 3);
            }

            if (actionID == 300)
            {
                WalkToWallObject(actionX, actionY, actionType);
                streamClass.CreatePacket(67);
                streamClass.AddShort(actionVar1);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddByte(actionType);
                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 310)
            {
                WalkToWallObject(actionX, actionY, actionType);
                streamClass.CreatePacket(36);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddByte(actionType);
                streamClass.AddShort(actionVar1);
                streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 320)
            {
                WalkToWallObject(actionX, actionY, actionType);
                streamClass.CreatePacket(126);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddByte(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 2300)
            {
                WalkToWallObject(actionX, actionY, actionType);
                streamClass.CreatePacket(235);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddByte(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 3300)
            {
                DisplayMessage(GameData.wallObjectDescription[actionType], 3);
            }

            if (actionID == 400)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                streamClass.CreatePacket(17);
                streamClass.AddShort(actionVar2);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);

                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 410)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                streamClass.CreatePacket(94);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.AddShort(actionVar2);
                streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 420)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                streamClass.CreatePacket(51);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.FormatPacket();
            }
            if (actionID == 2400)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                streamClass.CreatePacket(40);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.FormatPacket();
            }
            if (actionID == 3400)
            {
                DisplayMessage(GameData.objectDescription[actionType], 3);
            }

            if (actionID == 600)
            {
                streamClass.CreatePacket(49);
                streamClass.AddShort(actionVar1);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 610)
            {
                streamClass.CreatePacket(27);
                streamClass.AddShort(actionType);
                streamClass.AddShort(actionVar1);
                streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 620)
            {
                streamClass.CreatePacket(92);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 630)
            {
                streamClass.CreatePacket(181);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 640)
            {
                streamClass.CreatePacket(89);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 650)
            {
                selectedItem = actionType;
                drawMenuTab = 0;
                selectedItemName = GameData.itemName[inventoryItems[selectedItem]];
            }
            if (actionID == 660)
            {
                streamClass.CreatePacket(147);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
                selectedItem = -1;
                drawMenuTab = 0;
                DisplayMessage("Dropping " + GameData.itemName[inventoryItems[actionType]], 4);
            }
            if (actionID == 3600)
            {
                DisplayMessage(GameData.itemDescription[actionType], 3);
            }

            if (actionID == 700)
            {
                int k2 = (actionX - 64) / gridSize;
                int k4 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, k2, k4, true);
                streamClass.CreatePacket(71);
                streamClass.AddShort(actionVar1);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 710)
            {
                int l2 = (actionX - 64) / gridSize;
                int l4 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, l2, l4, true);
                streamClass.CreatePacket(142);
                streamClass.AddShort(actionType);
                streamClass.AddShort(actionVar1);
                streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 720)
            {
                int i3 = (actionX - 64) / gridSize;
                int i5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, i3, i5, true);
                streamClass.CreatePacket(177);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 725)
            {
                int j3 = (actionX - 64) / gridSize;
                int j5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, j3, j5, true);
                streamClass.CreatePacket(74);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 715 || actionID == 2715)
            {
                int k3 = (actionX - 64) / gridSize;
                int k5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, k3, k5, true);
                streamClass.CreatePacket(73);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 3700)
            {
                DisplayMessage(GameData.npcDescription[actionType], 3);
            }

            if (actionID == 800)
            {
                int l3 = (actionX - 64) / gridSize;
                int l5 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, l3, l5, true);
                streamClass.CreatePacket(55);
                streamClass.AddShort(actionVar1);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 810)
            {
                int i4 = (actionX - 64) / gridSize;
                int i6 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, i4, i6, true);
                streamClass.CreatePacket(16);
                streamClass.AddShort(actionType);
                streamClass.AddShort(actionVar1);
                streamClass.FormatPacket();
                selectedItem = -1;
            }
            if (actionID == 805 || actionID == 2805)
            {
                int j4 = (actionX - 64) / gridSize;
                int j6 = (actionY - 64) / gridSize;
                WalkTo1Tile(sectionX, sectionY, j4, j6, true);
                streamClass.CreatePacket(57);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 2806)
            {
                streamClass.CreatePacket(222);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 2810)
            {
                streamClass.CreatePacket(166);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 2820)
            {
                streamClass.CreatePacket(68);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
            }
            if (actionID == 900)
            {
                WalkTo1Tile(sectionX, sectionY, actionX, actionY, true);
                streamClass.CreatePacket(232);
                streamClass.AddShort(actionType);
                streamClass.AddShort(actionX + areaX);
                streamClass.AddShort(actionY + areaY);
                streamClass.FormatPacket();
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
                streamClass.CreatePacket(206);
                streamClass.AddShort(actionType);
                streamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (actionID == 4000)
            {
                selectedItem = -1;
                selectedSpell = -1;
            }
        }
        public void LoadMap()
        {
            engineHandle.mapsFree = UnpackData("maps.jag", "map", 70);
            engineHandle.mapsMembers = UnpackData("maps.mem", "members map", 75);
            engineHandle.landscapeFree = UnpackData("land.jag", "landscape", 80);
            engineHandle.landscapeMembers = UnpackData("land.mem", "members landscape", 85);
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
                    // Local pathfinding blocked — send destination directly to server
                    stepCount = 1;
                    walkArrayX[0] = destBottomX;
                    walkArrayY[0] = destBottomY;
                }
            }

            stepCount -= 1;
            startX = walkArrayX[stepCount];
            startY = walkArrayY[stepCount];
            stepCount -= 1;

            if (walkToACommand)
            {
                streamClass.CreatePacket(246);
            }
            else
            {
                streamClass.CreatePacket(132);
            }

            streamClass.AddShort(startX + areaX);
            streamClass.AddShort(startY + areaY);

            if (walkToACommand && stepCount == -1 && (startX + areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1 -= 1)
            {
                streamClass.AddByte(walkArrayX[i1] - startX);
                streamClass.AddByte(walkArrayY[i1] - startY);
            }

            streamClass.FormatPacket();
            //base.streamClass.Flush();

            actionPictureType = -24;
            walkMouseX = mouseX;
            walkMouseY = mouseY;
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
                streamClass.CreatePacket(246);
            }
            else
            {
                streamClass.CreatePacket(132);
            }

            streamClass.AddShort(startX + areaX);
            streamClass.AddShort(startY + areaY);
            if (walkToACommand && stepCount == -1 && (startX + areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1 -= 1)
            {
                streamClass.AddByte(walkArrayX[i1] - startX);
                streamClass.AddByte(walkArrayY[i1] - startY);
            }

            streamClass.FormatPacket();
            actionPictureType = -24;
            walkMouseX = mouseX;
            walkMouseY = mouseY;
            return true;
        }
        public void WalkToObject(int objectX, int objectY, int facingDirection, int objectIndex)
        {
            int l;
            int i1;
            if (facingDirection == 0 || facingDirection == 4)
            {
                l = GameData.objectWidth[objectIndex];
                i1 = GameData.objectHeight[objectIndex];
            }
            else
            {
                i1 = GameData.objectWidth[objectIndex];
                l = GameData.objectHeight[objectIndex];
            }
            if (GameData.objectType[objectIndex] == 2 || GameData.objectType[objectIndex] == 3)
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
        public void WalkTo1Tile(int l, int i1, int j1, int k1, bool flag)
        {
            WalkTo(l, i1, j1, k1, j1, k1, false, flag);
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
                                if (GameData.spellType[selectedSpell] == 1 || GameData.spellType[selectedSpell] == 2)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on";
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
                                    if (GameData.spellType[selectedSpell] == 3)
                                    {
                                        menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on";
                                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[groundItemID[index]];
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
                                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[groundItemID[index]];
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
                                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[groundItemID[index]];
                                        menuActionID[menuOptionsCount] = 220;
                                        menuActionX[menuOptionsCount] = groundItemX[index];
                                        menuActionY[menuOptionsCount] = groundItemY[index];
                                        menuActionType[menuOptionsCount] = groundItemID[index];
                                        menuOptionsCount += 1;
                                        menuText1[menuOptionsCount] = "Examine";
                                        menuText2[menuOptionsCount] = "@lre@" + GameData.itemName[groundItemID[index]];
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
                                    if (GameData.npcAttackable[id] > 0)
                                    {
                                        int j5 = (GameData.npcAttack[id] + GameData.npcDefense[id] + GameData.npcStrength[id] + GameData.npcHits[id]) / 4;
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
                                        if (GameData.spellType[selectedSpell] == 2)
                                        {
                                            menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on";
                                            menuText2[menuOptionsCount] = "@yel@" + GameData.npcName[npcArray[index].npcId];
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
                                            menuText2[menuOptionsCount] = "@yel@" + GameData.npcName[npcArray[index].npcId];
                                            menuActionID[menuOptionsCount] = 710;
                                            menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                            menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                            menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                            menuActionVar1[menuOptionsCount] = selectedItem;
                                            menuOptionsCount += 1;
                                        }
                                        else
                                        {
                                            if (GameData.npcAttackable[id] > 0)
                                            {
                                                menuText1[menuOptionsCount] = "Attack";
                                                menuText2[menuOptionsCount] = "@yel@" + GameData.npcName[npcArray[index].npcId] + s2;
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
                                            menuText2[menuOptionsCount] = "@yel@" + GameData.npcName[npcArray[index].npcId];
                                            menuActionID[menuOptionsCount] = 720;
                                            menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                            menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                            menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                            menuOptionsCount += 1;
                                            if (GameData.npcCommand[id] != "")
                                            {
                                                menuText1[menuOptionsCount] = GameData.npcCommand[id];
                                                menuText2[menuOptionsCount] = "@yel@" + GameData.npcName[npcArray[index].npcId];
                                                menuActionID[menuOptionsCount] = 725;
                                                menuActionX[menuOptionsCount] = npcArray[index].currentX;
                                                menuActionY[menuOptionsCount] = npcArray[index].currentY;
                                                menuActionType[menuOptionsCount] = npcArray[index].serverIndex;
                                                menuOptionsCount += 1;
                                            }
                                            menuText1[menuOptionsCount] = "Examine";
                                            menuText2[menuOptionsCount] = "@yel@" + GameData.npcName[npcArray[index].npcId];
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
                                    if (GameData.spellType[selectedSpell] == 4)
                                    {
                                        menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on";
                                        menuText2[menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
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
                                        menuText2[menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                        menuActionID[menuOptionsCount] = 310;
                                        menuActionX[menuOptionsCount] = wallObjectX[j3];
                                        menuActionY[menuOptionsCount] = wallObjectY[j3];
                                        menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                        menuActionVar1[menuOptionsCount] = selectedItem;
                                        menuOptionsCount += 1;
                                    }
                                    else
                                    {
                                        if (GameData.wallObjectCommand1[i4].ToLower() != "WalkTo")
                                        {
                                            menuText1[menuOptionsCount] = GameData.wallObjectCommand1[i4];
                                            menuText2[menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                            menuActionID[menuOptionsCount] = 320;
                                            menuActionX[menuOptionsCount] = wallObjectX[j3];
                                            menuActionY[menuOptionsCount] = wallObjectY[j3];
                                            menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                            menuOptionsCount += 1;
                                        }
                                        if (GameData.wallObjectCommand2[i4].ToLower() != "Examine")
                                        {
                                            menuText1[menuOptionsCount] = GameData.wallObjectCommand2[i4];
                                            menuText2[menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                            menuActionID[menuOptionsCount] = 2300;
                                            menuActionX[menuOptionsCount] = wallObjectX[j3];
                                            menuActionY[menuOptionsCount] = wallObjectY[j3];
                                            menuActionType[menuOptionsCount] = wallObjectDirection[j3];
                                            menuOptionsCount += 1;
                                        }
                                        menuText1[menuOptionsCount] = "Examine";
                                        menuText2[menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
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
                                        if (GameData.spellType[selectedSpell] == 5)
                                        {
                                            menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on";
                                            menuText2[menuOptionsCount] = "@cya@" + GameData.objectName[j4];
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
                                            menuText2[menuOptionsCount] = "@cya@" + GameData.objectName[j4];
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
                                            if (GameData.objectCommand1[j4].ToLower() != "WalkTo")
                                            {
                                                menuText1[menuOptionsCount] = GameData.objectCommand1[j4];
                                                menuText2[menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                                menuActionID[menuOptionsCount] = 420;
                                                menuActionX[menuOptionsCount] = objectX[k3];
                                                menuActionY[menuOptionsCount] = objectY[k3];
                                                menuActionType[menuOptionsCount] = objectRotation[k3];
                                                menuActionVar1[menuOptionsCount] = objectType[k3];
                                                menuOptionsCount += 1;
                                            }
                                            if (GameData.objectCommand2[j4].ToLower() != "Examine")
                                            {
                                                menuText1[menuOptionsCount] = GameData.objectCommand2[j4];
                                                menuText2[menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                                menuActionID[menuOptionsCount] = 2400;
                                                menuActionX[menuOptionsCount] = objectX[k3];
                                                menuActionY[menuOptionsCount] = objectY[k3];
                                                menuActionType[menuOptionsCount] = objectRotation[k3];
                                                menuActionVar1[menuOptionsCount] = objectType[k3];
                                                menuOptionsCount += 1;
                                            }
                                            menuText1[menuOptionsCount] = "Examine";
                                            menuText2[menuOptionsCount] = "@cya@" + GameData.objectName[j4];
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

            if (selectedSpell >= 0 && GameData.spellType[selectedSpell] <= 1)
            {
                menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on self";
                menuText2[menuOptionsCount] = "";
                menuActionID[menuOptionsCount] = 1000;
                menuActionType[menuOptionsCount] = selectedSpell;
                menuOptionsCount += 1;
            }
            if (ground != -1)
            {
                if (selectedSpell >= 0)
                {
                    if (GameData.spellType[selectedSpell] == 6)
                    {
                        menuText1[menuOptionsCount] = "Cast " + GameData.spellName[selectedSpell] + " on ground";
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
        public void GetMenuHighlighted()
        {
            if (drawMenuTab == 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 && mouseY < 35)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab == 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 33 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 33 && mouseY < 35)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab == 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 66 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 66 && mouseY < 35)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab == 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 99 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 99 && mouseY < 35)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab == 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 132 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 132 && mouseY < 35)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab == 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 165 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 165 && mouseY < 35)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab != 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 && mouseY < 26)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab != 0 && drawMenuTab != 2 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 33 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 33 && mouseY < 26)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab != 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 66 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 66 && mouseY < 26)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab != 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 99 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 99 && mouseY < 26)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab != 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 132 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 132 && mouseY < 26)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab != 0 && mouseX >= ((GameImage)(gameGraphics)).gameWidth - 35 - 165 && mouseY >= 3 && mouseX < ((GameImage)(gameGraphics)).gameWidth - 3 - 165 && mouseY < 26)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab == 1 && (mouseX < ((GameImage)(gameGraphics)).gameWidth - 248 || mouseY > 36 + (maxInventoryItems / 5) * 34))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 3 && (mouseX < ((GameImage)(gameGraphics)).gameWidth - 199 || mouseY > 316))
            {
                drawMenuTab = 0;
            }

            if ((drawMenuTab == 2 || drawMenuTab == 4 || drawMenuTab == 5) && (mouseX < ((GameImage)(gameGraphics)).gameWidth - 199 || mouseY > 240))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 6 && (mouseX < ((GameImage)(gameGraphics)).gameWidth - 199 || mouseY > 326))
            {
                drawMenuTab = 0;
            }
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
                    streamClass.CloseStream();
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
    }

}
