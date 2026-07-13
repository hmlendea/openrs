using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;
using System;

namespace OpenRS.Net.Client.World
{
    public sealed class WorldInteractionHandler(GameClient client)
    {

        public void MenuClick(int l)
        {
            int actionX = client.menuActionX[l];
            int actionY = client.menuActionY[l];
            int actionType = client.menuActionType[l];
            int actionVar1 = client.menuActionVar1[l];
            int actionVar2 = client.menuActionVar2[l];
            int actionID = client.menuActionID[l];
            if (actionID == 200)
            {
                WalkToGroundItem(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket(104);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 210)
            {
                WalkToGroundItem(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket(34);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == 220)
            {
                WalkToGroundItem(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket(245);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
            }
            if (actionID == 3200)
            {
                client.DisplayMessage(GameData.itemDescription[actionType], 3);
            }

            if (actionID == 300)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket(67);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 310)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket(36);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == 320)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket(126);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 2300)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket(235);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 3300)
            {
                client.DisplayMessage(GameData.wallObjectDescription[actionType], 3);
            }

            if (actionID == 400)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket(17);
                client.streamClass.AddShort(actionVar2);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);

                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 410)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket(94);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionVar2);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == 420)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket(51);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.FormatPacket();
            }
            if (actionID == 2400)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket(40);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.FormatPacket();
            }
            if (actionID == 3400)
            {
                client.DisplayMessage(GameData.objectDescription[actionType], 3);
            }

            if (actionID == 600)
            {
                client.streamClass.CreatePacket(49);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 610)
            {
                client.streamClass.CreatePacket(27);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == 620)
            {
                client.streamClass.CreatePacket(92);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 630)
            {
                client.streamClass.CreatePacket(181);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 640)
            {
                client.streamClass.CreatePacket(89);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 650)
            {
                client.selectedItem = actionType;
                client.drawMenuTab = 0;
                client.selectedItemName = GameData.itemName[client.inventoryItems[client.selectedItem]];
            }
            if (actionID == 660)
            {
                client.streamClass.CreatePacket(147);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
                client.drawMenuTab = 0;
                client.DisplayMessage("Dropping " + GameData.itemName[client.inventoryItems[actionType]], 4);
            }
            if (actionID == 3600)
            {
                client.DisplayMessage(GameData.itemDescription[actionType], 3);
            }

            if (actionID == 700)
            {
                int k2 = (actionX - 64) / client.gridSize;
                int k4 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, k2, k4, true);
                client.streamClass.CreatePacket(71);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 710)
            {
                int l2 = (actionX - 64) / client.gridSize;
                int l4 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, l2, l4, true);
                client.streamClass.CreatePacket(142);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == 720)
            {
                int i3 = (actionX - 64) / client.gridSize;
                int i5 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, i3, i5, true);
                client.streamClass.CreatePacket(177);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 725)
            {
                int j3 = (actionX - 64) / client.gridSize;
                int j5 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, j3, j5, true);
                client.streamClass.CreatePacket(74);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 715 || actionID == 2715)
            {
                int k3 = (actionX - 64) / client.gridSize;
                int k5 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, k3, k5, true);
                client.streamClass.CreatePacket(73);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 3700)
            {
                client.DisplayMessage(GameData.npcDescription[actionType], 3);
            }

            if (actionID == 800)
            {
                int l3 = (actionX - 64) / client.gridSize;
                int l5 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, l3, l5, true);
                client.streamClass.CreatePacket(55);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 810)
            {
                int i4 = (actionX - 64) / client.gridSize;
                int i6 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, i4, i6, true);
                client.streamClass.CreatePacket(16);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == 805 || actionID == 2805)
            {
                int j4 = (actionX - 64) / client.gridSize;
                int j6 = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, j4, j6, true);
                client.streamClass.CreatePacket(57);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 2806)
            {
                client.streamClass.CreatePacket(222);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 2810)
            {
                client.streamClass.CreatePacket(166);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 2820)
            {
                client.streamClass.CreatePacket(68);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == 900)
            {
                client.WalkTo1Tile(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket(232);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 920)
            {
                client.WalkTo1Tile(client.sectionX, client.sectionY, actionX, actionY, false);
                if (client.actionPictureType == -24)
                {
                    client.actionPictureType = 24;
                }
            }
            if (actionID == 1000)
            {
                client.streamClass.CreatePacket(206);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == 4000)
            {
                client.selectedItem = -1;
                client.selectedSpell = -1;
            }
        }
        public void LoadMap()
        {
            client.engineHandle.mapsFree = client.UnpackData("maps.jag", "map", 70);
            client.engineHandle.mapsMembers = client.UnpackData("maps.mem", "members map", 75);
            client.engineHandle.landscapeFree = client.UnpackData("land.jag", "landscape", 80);
            client.engineHandle.landscapeMembers = client.UnpackData("land.mem", "members landscape", 85);
        }
        public void WalkToWallObject(int x, int y, int direction)
        {
            if (direction == 0)
            {
                client.WalkTo(client.sectionX, client.sectionY, x, y - 1, x, y, false, true);
                return;
            }
            if (direction == 1)
            {
                client.WalkTo(client.sectionX, client.sectionY, x - 1, y, x, y, false, true);
                return;
            }
            else
            {
                client.WalkTo(client.sectionX, client.sectionY, x, y, x, y, true, true);
                return;
            }
        }
        public bool IsValidCameraAngle(int cameraDirection)
        {
            int l = client.ourPlayer.currentX / 128;
            int i1 = client.ourPlayer.currentY / 128;
            for (int j1 = 2; j1 >= 1; j1 -= 1)
            {
                if (cameraDirection == 1 && ((client.engineHandle.tiles[l][i1 - j1] & 0x80) == 128 || (client.engineHandle.tiles[l - j1][i1] & 0x80) == 128 || (client.engineHandle.tiles[l - j1][i1 - j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 3 && ((client.engineHandle.tiles[l][i1 + j1] & 0x80) == 128 || (client.engineHandle.tiles[l - j1][i1] & 0x80) == 128 || (client.engineHandle.tiles[l - j1][i1 + j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 5 && ((client.engineHandle.tiles[l][i1 + j1] & 0x80) == 128 || (client.engineHandle.tiles[l + j1][i1] & 0x80) == 128 || (client.engineHandle.tiles[l + j1][i1 + j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 7 && ((client.engineHandle.tiles[l][i1 - j1] & 0x80) == 128 || (client.engineHandle.tiles[l + j1][i1] & 0x80) == 128 || (client.engineHandle.tiles[l + j1][i1 - j1] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 0 && (client.engineHandle.tiles[l][i1 - j1] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 2 && (client.engineHandle.tiles[l - j1][i1] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 4 && (client.engineHandle.tiles[l][i1 + j1] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 6 && (client.engineHandle.tiles[l + j1][i1] & 0x80) == 128)
                {
                    return false;
                }
            }

            return true;
        }
        public bool WalkTo(int startX, int startY, int destBottomX, int destBottomY, int destTopX, int destTopY, bool checkForObjects,
                bool walkToACommand)
        {
            int stepCount = client.engineHandle.GeneratePath(startX, startY, destBottomX, destBottomY, destTopX, destTopY, client.walkArrayX, client.walkArrayY, checkForObjects);
            if (stepCount == -1)
            {
                if (walkToACommand)
                {
                    stepCount = 1;
                    client.walkArrayX[0] = destBottomX;
                    client.walkArrayY[0] = destBottomY;
                }
                else
                {
                    // Local pathfinding blocked — send destination directly to server
                    stepCount = 1;
                    client.walkArrayX[0] = destBottomX;
                    client.walkArrayY[0] = destBottomY;
                }
            }

            stepCount -= 1;
            startX = client.walkArrayX[stepCount];
            startY = client.walkArrayY[stepCount];
            stepCount -= 1;

            if (walkToACommand)
            {
                client.streamClass.CreatePacket(246);
            }
            else
            {
                client.streamClass.CreatePacket(132);
            }

            client.streamClass.AddShort(startX + client.areaX);
            client.streamClass.AddShort(startY + client.areaY);

            if (walkToACommand && stepCount == -1 && (startX + client.areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1 -= 1)
            {
                client.streamClass.AddByte(client.walkArrayX[i1] - startX);
                client.streamClass.AddByte(client.walkArrayY[i1] - startY);
            }

            client.streamClass.FormatPacket();
            //base.streamClass.Flush();

            client.actionPictureType = -24;
            client.walkMouseX = client.mouseX;
            client.walkMouseY = client.mouseY;
            return true;
        }
        public bool WalkToAlternate(int startX, int startY, int destBottomX, int destBottomY, int destTopX, int destTopY, bool unknownDifferent,
                bool walkToACommand)
        {
            int stepCount = client.engineHandle.GeneratePath(startX, startY, destBottomX, destBottomY, destTopX, destTopY, client.walkArrayX, client.walkArrayY, unknownDifferent);
            if (stepCount == -1)
            {
                return false;
            }

            stepCount -= 1;
            startX = client.walkArrayX[stepCount];
            startY = client.walkArrayY[stepCount];
            stepCount -= 1;
            if (walkToACommand)
            {
                client.streamClass.CreatePacket(246);
            }
            else
            {
                client.streamClass.CreatePacket(132);
            }

            client.streamClass.AddShort(startX + client.areaX);
            client.streamClass.AddShort(startY + client.areaY);
            if (walkToACommand && stepCount == -1 && (startX + client.areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1 -= 1)
            {
                client.streamClass.AddByte(client.walkArrayX[i1] - startX);
                client.streamClass.AddByte(client.walkArrayY[i1] - startY);
            }

            client.streamClass.FormatPacket();
            client.actionPictureType = -24;
            client.walkMouseX = client.mouseX;
            client.walkMouseY = client.mouseY;
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
                client.WalkTo(client.sectionX, client.sectionY, objectX, objectY, (objectX + l) - 1, (objectY + i1) - 1, false, true);
                return;
            }
            else
            {
                client.WalkTo(client.sectionX, client.sectionY, objectX, objectY, (objectX + l) - 1, (objectY + i1) - 1, true, true);
                return;
            }
        }
        public void AutoRotateCamera()
        {
            if ((client.cameraAutoAngle & 1) == 1 && client.IsValidCameraAngle(client.cameraAutoAngle))
            {
                return;
            }

            if ((client.cameraAutoAngle & 1) == 0 && client.IsValidCameraAngle(client.cameraAutoAngle))
            {
                if (client.IsValidCameraAngle(client.cameraAutoAngle + 1 & 7))
                {
                    client.cameraAutoAngle = client.cameraAutoAngle + 1 & 7;
                    return;
                }
                if (client.IsValidCameraAngle(client.cameraAutoAngle + 7 & 7))
                {
                    client.cameraAutoAngle = client.cameraAutoAngle + 7 & 7;
                }

                return;
            }
            int[] ai = [
            1, -1, 2, -2, 3, -3, 4
        ];
            for (int l = 0; l < 7; l += 1)
            {
                if (!client.IsValidCameraAngle(client.cameraAutoAngle + ai[l] + 8 & 7))
                {
                    continue;
                }

                client.cameraAutoAngle = client.cameraAutoAngle + ai[l] + 8 & 7;
                break;
            }

            if ((client.cameraAutoAngle & 1) == 0 && client.IsValidCameraAngle(client.cameraAutoAngle))
            {
                if (client.IsValidCameraAngle(client.cameraAutoAngle + 1 & 7))
                {
                    client.cameraAutoAngle = client.cameraAutoAngle + 1 & 7;
                    return;
                }
                if (client.IsValidCameraAngle(client.cameraAutoAngle + 7 & 7))
                {
                    client.cameraAutoAngle = client.cameraAutoAngle + 7 & 7;
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
            if (client.WalkToAlternate(l, i1, j1, k1, j1, k1, false, flag))
            {
                return;
            }
            else
            {
                client.WalkTo(l, i1, j1, k1, j1, k1, true, flag);
                return;
            }
        }
        public void WalkTo1Tile(int l, int i1, int j1, int k1, bool flag)
        {
            client.WalkTo(l, i1, j1, k1, j1, k1, false, flag);
        }
        public void GenerateWorldRightClickMenu()
        {
            int l = 2203 - (client.sectionY + client.wildY + client.areaY);
            if (client.sectionX + client.wildX + client.areaX >= 2640)
            {
                l = -50;
            }

            int ground = -1;
            for (int j1 = 0; j1 < client.objectCount; j1 += 1)
            {
                client.objectAlreadyInMenu[j1] = false;
            }

            for (int k1 = 0; k1 < client.wallObjectCount; k1 += 1)
            {
                client.wallObjectAlreadyInMenu[k1] = false;
            }

            int optionCount = client.gameCamera.GetOptionCount();
            GameObject[] objects = client.gameCamera.GetHighlightedObjects();
            int[] players = client.gameCamera.GetHighlightedPlayers();
            for (int i2 = 0; i2 < optionCount; i2 += 1)
            {
                if (client.menuOptionsCount > 200)
                {
                    break;
                }

                int player = players[i2];
                GameObject _obj = objects[i2];
                if (_obj.entityType[player] <= 65535 || _obj.entityType[player] >= 0x30d40 && _obj.entityType[player] <= 0x493e0)
                {
                    if (_obj == client.gameCamera.highlightedObject)
                    {
                        int index = _obj.entityType[player] % 10000;
                        int type = _obj.entityType[player] / 10000;
                        if (type == 1)
                        {
                            string s1 = "";
                            int k4 = 0;
                            if (client.ourPlayer.level > 0 && client.playerArray[index].level > 0)
                            {
                                k4 = client.ourPlayer.level - client.playerArray[index].level;
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

                            s1 = " " + s1 + "(level-" + client.playerArray[index].level + ")";
                            if (client.selectedSpell >= 0)
                            {
                                if (GameData.spellType[client.selectedSpell] == 1 || GameData.spellType[client.selectedSpell] == 2)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[index].username + s1;
                                    client.menuActionID[client.menuOptionsCount] = 800;
                                    client.menuActionX[client.menuOptionsCount] = client.playerArray[index].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.playerArray[index].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[index].serverIndex;
                                    client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                    client.menuOptionsCount += 1;
                                }
                            }
                            else
                                if (client.selectedItem >= 0)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[index].username + s1;
                                    client.menuActionID[client.menuOptionsCount] = 810;
                                    client.menuActionX[client.menuOptionsCount] = client.playerArray[index].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.playerArray[index].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[index].serverIndex;
                                    client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                    client.menuOptionsCount += 1;
                                }
                                else
                                {
                                    if (l > 0 && (client.playerArray[index].currentY - 64) / client.gridSize + client.wildY + client.areaY < 2203)
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Attack";
                                        client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[index].username + s1;
                                        if (k4 >= 0 && k4 < 5)
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 805;
                                    }
                                    else
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 2805;
                                    }

                                    client.menuActionX[client.menuOptionsCount] = client.playerArray[index].currentX;
                                        client.menuActionY[client.menuOptionsCount] = client.playerArray[index].currentY;
                                        client.menuActionType[client.menuOptionsCount] = client.playerArray[index].serverIndex;
                                        client.menuOptionsCount += 1;
                                    }
                                    else
                                        if (Config.MembersFeatures)
                                        {
                                            client.menuText1[client.menuOptionsCount] = "Duel with";
                                            client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[index].username + s1;
                                            client.menuActionX[client.menuOptionsCount] = client.playerArray[index].currentX;
                                            client.menuActionY[client.menuOptionsCount] = client.playerArray[index].currentY;
                                            client.menuActionID[client.menuOptionsCount] = 2806;
                                            client.menuActionType[client.menuOptionsCount] = client.playerArray[index].serverIndex;
                                            client.menuOptionsCount += 1;
                                        }
                                    client.menuText1[client.menuOptionsCount] = "Trade with";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[index].username + s1;
                                    client.menuActionID[client.menuOptionsCount] = 2810;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[index].serverIndex;
                                    client.menuOptionsCount += 1;
                                    client.menuText1[client.menuOptionsCount] = "Follow";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[index].username + s1;
                                    client.menuActionID[client.menuOptionsCount] = 2820;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[index].serverIndex;
                                    client.menuOptionsCount += 1;
                                }
                        }
                        else
                            if (type == 2)
                            {
                                if (client.selectedSpell >= 0)
                                {
                                    if (GameData.spellType[client.selectedSpell] == 3)
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on";
                                        client.menuText2[client.menuOptionsCount] = "@lre@" + GameData.itemName[client.groundItemID[index]];
                                        client.menuActionID[client.menuOptionsCount] = 200;
                                        client.menuActionX[client.menuOptionsCount] = client.groundItemX[index];
                                        client.menuActionY[client.menuOptionsCount] = client.groundItemY[index];
                                        client.menuActionType[client.menuOptionsCount] = client.groundItemID[index];
                                        client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                        client.menuOptionsCount += 1;
                                    }
                                }
                                else
                                    if (client.selectedItem >= 0)
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                        client.menuText2[client.menuOptionsCount] = "@lre@" + GameData.itemName[client.groundItemID[index]];
                                        client.menuActionID[client.menuOptionsCount] = 210;
                                        client.menuActionX[client.menuOptionsCount] = client.groundItemX[index];
                                        client.menuActionY[client.menuOptionsCount] = client.groundItemY[index];
                                        client.menuActionType[client.menuOptionsCount] = client.groundItemID[index];
                                        client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                        client.menuOptionsCount += 1;
                                    }
                                    else
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Take";
                                        client.menuText2[client.menuOptionsCount] = "@lre@" + GameData.itemName[client.groundItemID[index]];
                                        client.menuActionID[client.menuOptionsCount] = 220;
                                        client.menuActionX[client.menuOptionsCount] = client.groundItemX[index];
                                        client.menuActionY[client.menuOptionsCount] = client.groundItemY[index];
                                        client.menuActionType[client.menuOptionsCount] = client.groundItemID[index];
                                        client.menuOptionsCount += 1;
                                        client.menuText1[client.menuOptionsCount] = "Examine";
                                        client.menuText2[client.menuOptionsCount] = "@lre@" + GameData.itemName[client.groundItemID[index]];
                                        client.menuActionID[client.menuOptionsCount] = 3200;
                                        client.menuActionType[client.menuOptionsCount] = client.groundItemID[index];
                                        client.menuOptionsCount += 1;
                                    }
                            }
                            else
                                if (type == 3)
                                {
                                    string s2 = "";
                                    int l4 = -1;
                                    int id = client.npcArray[index].npcId;
                                    if (GameData.npcAttackable[id] > 0)
                                    {
                                        int j5 = (GameData.npcAttack[id] + GameData.npcDefense[id] + GameData.npcStrength[id] + GameData.npcHits[id]) / 4;
                                        int k5 = (client.playerStatBase[0] + client.playerStatBase[1] + client.playerStatBase[2] + client.playerStatBase[3] + 27) / 4;
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
                                    if (client.selectedSpell >= 0)
                                    {
                                        if (GameData.spellType[client.selectedSpell] == 2)
                                        {
                                            client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on";
                                            client.menuText2[client.menuOptionsCount] = "@yel@" + GameData.npcName[client.npcArray[index].npcId];
                                            client.menuActionID[client.menuOptionsCount] = 700;
                                            client.menuActionX[client.menuOptionsCount] = client.npcArray[index].currentX;
                                            client.menuActionY[client.menuOptionsCount] = client.npcArray[index].currentY;
                                            client.menuActionType[client.menuOptionsCount] = client.npcArray[index].serverIndex;
                                            client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                            client.menuOptionsCount += 1;
                                        }
                                    }
                                    else
                                        if (client.selectedItem >= 0)
                                        {
                                            client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                            client.menuText2[client.menuOptionsCount] = "@yel@" + GameData.npcName[client.npcArray[index].npcId];
                                            client.menuActionID[client.menuOptionsCount] = 710;
                                            client.menuActionX[client.menuOptionsCount] = client.npcArray[index].currentX;
                                            client.menuActionY[client.menuOptionsCount] = client.npcArray[index].currentY;
                                            client.menuActionType[client.menuOptionsCount] = client.npcArray[index].serverIndex;
                                            client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                            client.menuOptionsCount += 1;
                                        }
                                        else
                                        {
                                            if (GameData.npcAttackable[id] > 0)
                                            {
                                                client.menuText1[client.menuOptionsCount] = "Attack";
                                                client.menuText2[client.menuOptionsCount] = "@yel@" + GameData.npcName[client.npcArray[index].npcId] + s2;
                                                if (l4 >= 0)
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 715;
                                    }
                                    else
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 2715;
                                    }

                                    client.menuActionX[client.menuOptionsCount] = client.npcArray[index].currentX;
                                                client.menuActionY[client.menuOptionsCount] = client.npcArray[index].currentY;
                                                client.menuActionType[client.menuOptionsCount] = client.npcArray[index].serverIndex;
                                                client.menuOptionsCount += 1;
                                            }
                                            client.menuText1[client.menuOptionsCount] = "Talk-to";
                                            client.menuText2[client.menuOptionsCount] = "@yel@" + GameData.npcName[client.npcArray[index].npcId];
                                            client.menuActionID[client.menuOptionsCount] = 720;
                                            client.menuActionX[client.menuOptionsCount] = client.npcArray[index].currentX;
                                            client.menuActionY[client.menuOptionsCount] = client.npcArray[index].currentY;
                                            client.menuActionType[client.menuOptionsCount] = client.npcArray[index].serverIndex;
                                            client.menuOptionsCount += 1;
                                            if (GameData.npcCommand[id] != "")
                                            {
                                                client.menuText1[client.menuOptionsCount] = GameData.npcCommand[id];
                                                client.menuText2[client.menuOptionsCount] = "@yel@" + GameData.npcName[client.npcArray[index].npcId];
                                                client.menuActionID[client.menuOptionsCount] = 725;
                                                client.menuActionX[client.menuOptionsCount] = client.npcArray[index].currentX;
                                                client.menuActionY[client.menuOptionsCount] = client.npcArray[index].currentY;
                                                client.menuActionType[client.menuOptionsCount] = client.npcArray[index].serverIndex;
                                                client.menuOptionsCount += 1;
                                            }
                                            client.menuText1[client.menuOptionsCount] = "Examine";
                                            client.menuText2[client.menuOptionsCount] = "@yel@" + GameData.npcName[client.npcArray[index].npcId];
                                            client.menuActionID[client.menuOptionsCount] = 3700;
                                            client.menuActionType[client.menuOptionsCount] = client.npcArray[index].npcId;
                                            client.menuOptionsCount += 1;
                                        }
                                }
                    }
                    else
                        if (_obj is not null && _obj.index >= 10000)
                        {
                            int j3 = _obj.index - 10000;
                            int i4 = client.wallObjectID[j3];
                            if (!client.wallObjectAlreadyInMenu[j3])
                            {
                                if (client.selectedSpell >= 0)
                                {
                                    if (GameData.spellType[client.selectedSpell] == 4)
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on";
                                        client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                        client.menuActionID[client.menuOptionsCount] = 300;
                                        client.menuActionX[client.menuOptionsCount] = client.wallObjectX[j3];
                                        client.menuActionY[client.menuOptionsCount] = client.wallObjectY[j3];
                                        client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[j3];
                                        client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                        client.menuOptionsCount += 1;
                                    }
                                }
                                else
                                    if (client.selectedItem >= 0)
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                        client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                        client.menuActionID[client.menuOptionsCount] = 310;
                                        client.menuActionX[client.menuOptionsCount] = client.wallObjectX[j3];
                                        client.menuActionY[client.menuOptionsCount] = client.wallObjectY[j3];
                                        client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[j3];
                                        client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                        client.menuOptionsCount += 1;
                                    }
                                    else
                                    {
                                        if (GameData.wallObjectCommand1[i4].ToLower() != "WalkTo")
                                        {
                                            client.menuText1[client.menuOptionsCount] = GameData.wallObjectCommand1[i4];
                                            client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                            client.menuActionID[client.menuOptionsCount] = 320;
                                            client.menuActionX[client.menuOptionsCount] = client.wallObjectX[j3];
                                            client.menuActionY[client.menuOptionsCount] = client.wallObjectY[j3];
                                            client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[j3];
                                            client.menuOptionsCount += 1;
                                        }
                                        if (GameData.wallObjectCommand2[i4].ToLower() != "Examine")
                                        {
                                            client.menuText1[client.menuOptionsCount] = GameData.wallObjectCommand2[i4];
                                            client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                            client.menuActionID[client.menuOptionsCount] = 2300;
                                            client.menuActionX[client.menuOptionsCount] = client.wallObjectX[j3];
                                            client.menuActionY[client.menuOptionsCount] = client.wallObjectY[j3];
                                            client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[j3];
                                            client.menuOptionsCount += 1;
                                        }
                                        client.menuText1[client.menuOptionsCount] = "Examine";
                                        client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.wallObjectName[i4];
                                        client.menuActionID[client.menuOptionsCount] = 3300;
                                        client.menuActionType[client.menuOptionsCount] = i4;
                                        client.menuOptionsCount += 1;
                                    }
                                client.wallObjectAlreadyInMenu[j3] = true;
                            }
                        }
                        else
                            if (_obj is not null && _obj.index >= 0)
                            {
                                int k3 = _obj.index;
                                int j4 = client.objectType[k3];
                                if (!client.objectAlreadyInMenu[k3])
                                {
                                    if (client.selectedSpell >= 0)
                                    {
                                        if (GameData.spellType[client.selectedSpell] == 5)
                                        {
                                            client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on";
                                            client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                            client.menuActionID[client.menuOptionsCount] = 400;
                                            client.menuActionX[client.menuOptionsCount] = client.objectX[k3];
                                            client.menuActionY[client.menuOptionsCount] = client.objectY[k3];
                                            client.menuActionType[client.menuOptionsCount] = client.objectRotation[k3];
                                            client.menuActionVar1[client.menuOptionsCount] = client.objectType[k3];
                                            client.menuActionVar2[client.menuOptionsCount] = client.selectedSpell;
                                            client.menuOptionsCount += 1;
                                        }
                                    }
                                    else
                                        if (client.selectedItem >= 0)
                                        {
                                            client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                            client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                            client.menuActionID[client.menuOptionsCount] = 410;
                                            client.menuActionX[client.menuOptionsCount] = client.objectX[k3];
                                            client.menuActionY[client.menuOptionsCount] = client.objectY[k3];
                                            client.menuActionType[client.menuOptionsCount] = client.objectRotation[k3];
                                            client.menuActionVar1[client.menuOptionsCount] = client.objectType[k3];
                                            client.menuActionVar2[client.menuOptionsCount] = client.selectedItem;
                                            client.menuOptionsCount += 1;
                                        }
                                        else
                                        {
                                            if (GameData.objectCommand1[j4].ToLower() != "WalkTo")
                                            {
                                                client.menuText1[client.menuOptionsCount] = GameData.objectCommand1[j4];
                                                client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                                client.menuActionID[client.menuOptionsCount] = 420;
                                                client.menuActionX[client.menuOptionsCount] = client.objectX[k3];
                                                client.menuActionY[client.menuOptionsCount] = client.objectY[k3];
                                                client.menuActionType[client.menuOptionsCount] = client.objectRotation[k3];
                                                client.menuActionVar1[client.menuOptionsCount] = client.objectType[k3];
                                                client.menuOptionsCount += 1;
                                            }
                                            if (GameData.objectCommand2[j4].ToLower() != "Examine")
                                            {
                                                client.menuText1[client.menuOptionsCount] = GameData.objectCommand2[j4];
                                                client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                                client.menuActionID[client.menuOptionsCount] = 2400;
                                                client.menuActionX[client.menuOptionsCount] = client.objectX[k3];
                                                client.menuActionY[client.menuOptionsCount] = client.objectY[k3];
                                                client.menuActionType[client.menuOptionsCount] = client.objectRotation[k3];
                                                client.menuActionVar1[client.menuOptionsCount] = client.objectType[k3];
                                                client.menuOptionsCount += 1;
                                            }
                                            client.menuText1[client.menuOptionsCount] = "Examine";
                                            client.menuText2[client.menuOptionsCount] = "@cya@" + GameData.objectName[j4];
                                            client.menuActionID[client.menuOptionsCount] = 3400;
                                            client.menuActionType[client.menuOptionsCount] = j4;
                                            client.menuOptionsCount += 1;
                                        }
                                    client.objectAlreadyInMenu[k3] = true;
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

            if (client.selectedSpell >= 0 && GameData.spellType[client.selectedSpell] <= 1)
            {
                client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on self";
                client.menuText2[client.menuOptionsCount] = "";
                client.menuActionID[client.menuOptionsCount] = 1000;
                client.menuActionType[client.menuOptionsCount] = client.selectedSpell;
                client.menuOptionsCount += 1;
            }
            if (ground != -1)
            {
                if (client.selectedSpell >= 0)
                {
                    if (GameData.spellType[client.selectedSpell] == 6)
                    {
                        client.menuText1[client.menuOptionsCount] = "Cast " + GameData.spellName[client.selectedSpell] + " on ground";
                        client.menuText2[client.menuOptionsCount] = "";
                        client.menuActionID[client.menuOptionsCount] = 900;
                        client.menuActionX[client.menuOptionsCount] = client.engineHandle.selectedX[ground];
                        client.menuActionY[client.menuOptionsCount] = client.engineHandle.selectedY[ground];
                        client.menuActionType[client.menuOptionsCount] = client.selectedSpell;
                        client.menuOptionsCount += 1;
                        return;
                    }
                }
                else
                    if (client.selectedItem < 0)
                    {
                        client.menuText1[client.menuOptionsCount] = "Walk here";
                        client.menuText2[client.menuOptionsCount] = "";
                        client.menuActionID[client.menuOptionsCount] = 920;
                        client.menuActionX[client.menuOptionsCount] = client.engineHandle.selectedX[ground];
                        client.menuActionY[client.menuOptionsCount] = client.engineHandle.selectedY[ground];
                        client.menuOptionsCount += 1;
                    }
            }
        }
        public void GetMenuHighlighted()
        {
            if (client.drawMenuTab == 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 && client.mouseY < 35)
            {
                client.drawMenuTab = 1;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 33 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 33 && client.mouseY < 35)
            {
                client.drawMenuTab = 2;
                client.minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                client.minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (client.drawMenuTab == 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 66 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 66 && client.mouseY < 35)
            {
                client.drawMenuTab = 3;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 99 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 99 && client.mouseY < 35)
            {
                client.drawMenuTab = 4;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 132 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 132 && client.mouseY < 35)
            {
                client.drawMenuTab = 5;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 165 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 165 && client.mouseY < 35)
            {
                client.drawMenuTab = 6;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 && client.mouseY < 26)
            {
                client.drawMenuTab = 1;
            }

            if (client.drawMenuTab != 0 && client.drawMenuTab != 2 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 33 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 33 && client.mouseY < 26)
            {
                client.drawMenuTab = 2;
                client.minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                client.minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (client.drawMenuTab != 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 66 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 66 && client.mouseY < 26)
            {
                client.drawMenuTab = 3;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 99 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 99 && client.mouseY < 26)
            {
                client.drawMenuTab = 4;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 132 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 132 && client.mouseY < 26)
            {
                client.drawMenuTab = 5;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= ((GameImage)(client.gameGraphics)).gameWidth - 35 - 165 && client.mouseY >= 3 && client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 3 - 165 && client.mouseY < 26)
            {
                client.drawMenuTab = 6;
            }

            if (client.drawMenuTab == 1 && (client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 248 || client.mouseY > 36 + (client.maxInventoryItems / 5) * 34))
            {
                client.drawMenuTab = 0;
            }

            if (client.drawMenuTab == 3 && (client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 199 || client.mouseY > 316))
            {
                client.drawMenuTab = 0;
            }

            if ((client.drawMenuTab == 2 || client.drawMenuTab == 4 || client.drawMenuTab == 5) && (client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 199 || client.mouseY > 240))
            {
                client.drawMenuTab = 0;
            }

            if (client.drawMenuTab == 6 && (client.mouseX < ((GameImage)(client.gameGraphics)).gameWidth - 199 || client.mouseY > 326))
            {
                client.drawMenuTab = 0;
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
                    client.streamClass.CloseStream();
                    return true;
                }
                if (cmd == "logout")
                {
                    client.SendLogout();
                    return true;
                }
                if (cmd == "lostcon")
                {
                    client.LostConnection();
                    return true;
                }
                if (cmd == "tell")
                {
                    long recipient = DataOperations.NameToHash(args[0]);
                    string message = client.JoinString(args, " ", 1).Trim();
                    if (message == "")
                    {
                        return true;
                    }

                    int len = ChatMessage.StringToBytes(message);
                    client.CallSendPrivateMessage(recipient, ChatMessage.lastChat, len);
                    message = ChatMessage.BytesToString(ChatMessage.lastChat, 0, len);
                    //  if (useChatFilter)
                    //      message = ChatFilter.filterChat(message);
                    client.DisplayMessage("@pri@You tell " + DataOperations.HashToName(recipient) + ": " + message);
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
