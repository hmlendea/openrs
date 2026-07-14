using System;

using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;

namespace OpenRS.Net.Client.World
{
    public sealed class WorldInteractionHandler(GameClient client)
    {
        public void MenuClick(int menuIndex)
        {
            int actionX = client.menuActionX[menuIndex];
            int actionY = client.menuActionY[menuIndex];
            int actionType = client.menuActionType[menuIndex];
            int actionVar1 = client.menuActionVar1[menuIndex];
            int actionVar2 = client.menuActionVar2[menuIndex];
            int actionID = client.menuActionID[menuIndex];

            if (actionID == (int)MenuAction.CastSpellOnGroundItem)
            {
                WalkToGroundItem(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnGroundItem);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == (int)MenuAction.UseItemWithGroundItem)
            {
                WalkToGroundItem(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket((int)ClientPacket.UseItemWithGroundItem);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == (int)MenuAction.TakeItem)
            {
                WalkToGroundItem(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket((int)ClientPacket.TakeGroundItem);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.ExamineGroundItem)
            {
                client.DisplayMessage(client.entityManager.GetItem(actionType).Description, 3);
            }

            if (actionID == (int)MenuAction.CastSpellOnWallObject)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnWallObject);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == (int)MenuAction.UseItemWithWallObject)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket((int)ClientPacket.UseItemWithWallObject);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == (int)MenuAction.Command1OnWallObject)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket((int)ClientPacket.WallObjectCommand1);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.Command2OnWallObject)
            {
                WalkToWallObject(actionX, actionY, actionType);
                client.streamClass.CreatePacket((int)ClientPacket.WallObjectCommand2);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddByte(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.ExamineWallObject)
            {
                client.DisplayMessage(client.entityManager.GetWallObject(actionType).Description, 3);
            }

            if (actionID == (int)MenuAction.CastSpellOnModel)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnGameObject);
                client.streamClass.AddShort(actionVar2);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);

                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == (int)MenuAction.UseItemWithModel)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket((int)ClientPacket.UseItemWithGameObject);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.AddShort(actionVar2);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == (int)MenuAction.Command1OnModel)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket((int)ClientPacket.GameObjectCommand1);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.Command2OnModel)
            {
                WalkToObject(actionX, actionY, actionType, actionVar1);
                client.streamClass.CreatePacket((int)ClientPacket.GameObjectCommand2);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.ExamineModel)
            {
                client.DisplayMessage(client.entityManager.GetWorldObject(actionType).Description, 3);
            }

            if (actionID == (int)MenuAction.CastSpellOnItem)
            {
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnInventoryItem);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == (int)MenuAction.UseItemWithItem)
            {
                client.streamClass.CreatePacket((int)ClientPacket.UseItemWithInventoryItem);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }
            if (actionID == (int)MenuAction.RemoveItem)
            {
                client.streamClass.CreatePacket((int)ClientPacket.RemoveInventoryItem);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.EquipItem)
            {
                client.streamClass.CreatePacket((int)ClientPacket.EquipItem);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.CommandOnItem)
            {
                client.streamClass.CreatePacket((int)ClientPacket.InventoryItemCommand);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.UseItem)
            {
                client.selectedItem = actionType;
                client.drawMenuTab = 0;
                client.selectedItemName = client.entityManager.GetItem(client.inventoryItems[client.selectedItem]).Name;
            }
            if (actionID == (int)MenuAction.DropItem)
            {
                client.streamClass.CreatePacket((int)ClientPacket.DropItem);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
                client.drawMenuTab = 0;
                client.DisplayMessage("Dropping " + client.entityManager.GetItem(client.inventoryItems[actionType]).Name, 4);
            }
            if (actionID == (int)MenuAction.ExamineItem)
            {
                client.DisplayMessage(client.entityManager.GetItem(actionType).Description, 3);
            }

            if (actionID == (int)MenuAction.CastSpellOnNpc)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnNpc);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }

            if (actionID == (int)MenuAction.UseItemWithNpc)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.UseItemWithNpc);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }

            if (actionID == (int)MenuAction.TalkToNpc)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.TalkToNpc);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }

            if (actionID == (int)MenuAction.CommandOnNpc)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.NpcCommand);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }

            if (actionID == (int)MenuAction.AttackNpc || actionID == (int)MenuAction.AttackNpc2)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.AttackNpc);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.ExamineNpc)
            {
                client.DisplayMessage(client.entityManager.GetNpc(actionType).Description, 3);
            }

            if (actionID == (int)MenuAction.CastSpellOnPlayer)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnPlayer);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }

            if (actionID == (int)MenuAction.UseItemWithPlayer)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.UseItemWithPlayer);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionVar1);
                client.streamClass.FormatPacket();
                client.selectedItem = -1;
            }

            if (actionID == (int)MenuAction.AttackPlayerSafe || actionID == (int)MenuAction.AttackPlayerUnsafe)
            {
                int tileX = (actionX - 64) / client.gridSize;
                int tileY = (actionY - 64) / client.gridSize;
                client.WalkTo1Tile(client.sectionX, client.sectionY, tileX, tileY, true);
                client.streamClass.CreatePacket((int)ClientPacket.AttackPlayer);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.DuelWithPlayer)
            {
                client.streamClass.CreatePacket((int)ClientPacket.DuelWithPlayer);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.TradeWithPlayer)
            {
                client.streamClass.CreatePacket((int)ClientPacket.TradeWithPlayer);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.FollowPlayer)
            {
                client.streamClass.CreatePacket((int)ClientPacket.FollowPlayer);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
            }
            if (actionID == (int)MenuAction.CastSpellOnGround)
            {
                client.WalkTo1Tile(client.sectionX, client.sectionY, actionX, actionY, true);
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnGround);
                client.streamClass.AddShort(actionType);
                client.streamClass.AddShort(actionX + client.areaX);
                client.streamClass.AddShort(actionY + client.areaY);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == (int)MenuAction.WalkHere)
            {
                client.WalkTo1Tile(client.sectionX, client.sectionY, actionX, actionY, false);
                if (client.actionPictureType == -24)
                {
                    client.actionPictureType = 24;
                }
            }
            if (actionID == (int)MenuAction.CastSpellOnSelf)
            {
                client.streamClass.CreatePacket((int)ClientPacket.CastSpellOnSelf);
                client.streamClass.AddShort(actionType);
                client.streamClass.FormatPacket();
                client.selectedSpell = -1;
            }
            if (actionID == (int)MenuAction.Cancel)
            {
                client.selectedItem = -1;
                client.selectedSpell = -1;
            }
        }

        public void LoadMap()
        {
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

            client.WalkTo(client.sectionX, client.sectionY, x, y, x, y, true, true);
        }

        public bool IsValidCameraAngle(int cameraDirection)
        {
            int playerTileX = client.ourPlayer.currentX / 128;
            int playerTileY = client.ourPlayer.currentY / 128;

            for (int checkDistance = 2; checkDistance >= 1; checkDistance -= 1)
            {
                if (cameraDirection == 1 && ((client.engineHandle.tiles[playerTileX][playerTileY - checkDistance] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX - checkDistance][playerTileY] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX - checkDistance][playerTileY - checkDistance] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 3 && ((client.engineHandle.tiles[playerTileX][playerTileY + checkDistance] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX - checkDistance][playerTileY] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX - checkDistance][playerTileY + checkDistance] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 5 && ((client.engineHandle.tiles[playerTileX][playerTileY + checkDistance] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX + checkDistance][playerTileY] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX + checkDistance][playerTileY + checkDistance] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 7 && ((client.engineHandle.tiles[playerTileX][playerTileY - checkDistance] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX + checkDistance][playerTileY] & 0x80) == 128 || (client.engineHandle.tiles[playerTileX + checkDistance][playerTileY - checkDistance] & 0x80) == 128))
                {
                    return false;
                }

                if (cameraDirection == 0 && (client.engineHandle.tiles[playerTileX][playerTileY - checkDistance] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 2 && (client.engineHandle.tiles[playerTileX - checkDistance][playerTileY] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 4 && (client.engineHandle.tiles[playerTileX][playerTileY + checkDistance] & 0x80) == 128)
                {
                    return false;
                }

                if (cameraDirection == 6 && (client.engineHandle.tiles[playerTileX + checkDistance][playerTileY] & 0x80) == 128)
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
                client.streamClass.CreatePacket((int)ClientPacket.WalkToCommand);
            }
            else
            {
                client.streamClass.CreatePacket((int)ClientPacket.Walk);
            }

            client.streamClass.AddShort(startX + client.areaX);
            client.streamClass.AddShort(startY + client.areaY);

            if (walkToACommand && stepCount == -1 && (startX + client.areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int walkStepIndex = stepCount; walkStepIndex >= 0 && walkStepIndex > stepCount - 25; walkStepIndex -= 1)
            {
                client.streamClass.AddByte(client.walkArrayX[walkStepIndex] - startX);
                client.streamClass.AddByte(client.walkArrayY[walkStepIndex] - startY);
            }

            client.streamClass.FormatPacket();

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
                client.streamClass.CreatePacket((int)ClientPacket.WalkToCommand);
            }
            else
            {
                client.streamClass.CreatePacket((int)ClientPacket.Walk);
            }

            client.streamClass.AddShort(startX + client.areaX);
            client.streamClass.AddShort(startY + client.areaY);

            if (walkToACommand && stepCount == -1 && (startX + client.areaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int walkStepIndex = stepCount; walkStepIndex >= 0 && walkStepIndex > stepCount - 25; walkStepIndex -= 1)
            {
                client.streamClass.AddByte(client.walkArrayX[walkStepIndex] - startX);
                client.streamClass.AddByte(client.walkArrayY[walkStepIndex] - startY);
            }

            client.streamClass.FormatPacket();
            client.actionPictureType = -24;
            client.walkMouseX = client.mouseX;
            client.walkMouseY = client.mouseY;

            return true;
        }

        public void WalkToObject(int objectX, int objectY, int facingDirection, int objectIndex)
        {
            int adjustedWidth;
            int adjustedHeight;

            if (facingDirection == 0 || facingDirection == 4)
            {
                adjustedWidth = client.entityManager.GetWorldObject(objectIndex).Width;
                adjustedHeight = client.entityManager.GetWorldObject(objectIndex).Height;
            }
            else
            {
                adjustedHeight = client.entityManager.GetWorldObject(objectIndex).Width;
                adjustedWidth = client.entityManager.GetWorldObject(objectIndex).Height;
            }

            if (client.entityManager.GetWorldObject(objectIndex).Type == 2 || client.entityManager.GetWorldObject(objectIndex).Type == 3)
            {
                if (facingDirection == 0)
                {
                    objectX -= 1;
                    adjustedWidth += 1;
                }

                if (facingDirection == 2)
                {
                    adjustedHeight += 1;
                }

                if (facingDirection == 4)
                {
                    adjustedWidth += 1;
                }

                if (facingDirection == 6)
                {
                    objectY -= 1;
                    adjustedHeight += 1;
                }

                client.WalkTo(client.sectionX, client.sectionY, objectX, objectY, objectX + adjustedWidth - 1, objectY + adjustedHeight - 1, false, true);

                return;
            }

            client.WalkTo(client.sectionX, client.sectionY, objectX, objectY, objectX + adjustedWidth - 1, objectY + adjustedHeight - 1, true, true);
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

            int[] directionOffsets = [1, -1, 2, -2, 3, -3, 4];

            for (int offsetIndex = 0; offsetIndex < 7; offsetIndex += 1)
            {
                if (!client.IsValidCameraAngle(client.cameraAutoAngle + directionOffsets[offsetIndex] + 8 & 7))
                {
                    continue;
                }

                client.cameraAutoAngle = client.cameraAutoAngle + directionOffsets[offsetIndex] + 8 & 7;
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
            }
        }

        public void WalkToGroundItem(int sectionX, int sectionY, int destinationX, int destinationY, bool isWalkCommand)
        {
            if (client.WalkToAlternate(sectionX, sectionY, destinationX, destinationY, destinationX, destinationY, false, isWalkCommand))
            {
                return;
            }

            client.WalkTo(sectionX, sectionY, destinationX, destinationY, destinationX, destinationY, true, isWalkCommand);
        }

        public void WalkTo1Tile(int sectionX, int sectionY, int destinationX, int destinationY, bool isWalkCommand)
        {
            client.WalkTo(sectionX, sectionY, destinationX, destinationY, destinationX, destinationY, false, isWalkCommand);
        }

        public void GenerateWorldRightClickMenu()
        {
            int northernWildernessBoundary = 2203 - (client.sectionY + client.wildY + client.areaY);

            if (client.sectionX + client.wildX + client.areaX >= 2640)
            {
                northernWildernessBoundary = -50;
            }

            int groundEntityIndex = -1;

            for (int objectSlot = 0; objectSlot < client.objectCount; objectSlot += 1)
            {
                client.objectAlreadyInMenu[objectSlot] = false;
            }

            for (int wallObjectSlot = 0; wallObjectSlot < client.wallObjectCount; wallObjectSlot += 1)
            {
                client.wallObjectAlreadyInMenu[wallObjectSlot] = false;
            }

            int optionCount = client.gameCamera.GetOptionCount();
            GameObject[] objects = client.gameCamera.GetHighlightedObjects();
            int[] entitySlots = client.gameCamera.GetHighlightedPlayers();

            for (int optionIndex = 0; optionIndex < optionCount; optionIndex += 1)
            {
                if (client.menuOptionsCount > 200)
                {
                    break;
                }

                int entitySlotId = entitySlots[optionIndex];
                GameObject highlightedObject = objects[optionIndex];

                if (highlightedObject.entityType[entitySlotId] <= 65535 || highlightedObject.entityType[entitySlotId] >= 0x30d40 && highlightedObject.entityType[entitySlotId] <= 0x493e0)
                {
                    if (highlightedObject == client.gameCamera.highlightedObject)
                    {
                        int entityIndex = highlightedObject.entityType[entitySlotId] % 10000;
                        int entityKind = highlightedObject.entityType[entitySlotId] / 10000;

                        if (entityKind == 1)
                        {
                            string levelColourCode = "";
                            int levelDifference = 0;

                            if (client.ourPlayer.level > 0 && client.playerArray[entityIndex].level > 0)
                            {
                                levelDifference = client.ourPlayer.level - client.playerArray[entityIndex].level;
                            }

                            if (levelDifference < 0)
                            {
                                levelColourCode = "@or1@";
                            }

                            if (levelDifference < -3)
                            {
                                levelColourCode = "@or2@";
                            }

                            if (levelDifference < -6)
                            {
                                levelColourCode = "@or3@";
                            }

                            if (levelDifference < -9)
                            {
                                levelColourCode = "@red@";
                            }

                            if (levelDifference > 0)
                            {
                                levelColourCode = "@gr1@";
                            }

                            if (levelDifference > 3)
                            {
                                levelColourCode = "@gr2@";
                            }

                            if (levelDifference > 6)
                            {
                                levelColourCode = "@gr3@";
                            }

                            if (levelDifference > 9)
                            {
                                levelColourCode = "@gre@";
                            }

                            levelColourCode = " " + levelColourCode + "(level-" + client.playerArray[entityIndex].level + ")";
                            if (client.selectedSpell >= 0)
                            {
                                if (client.entityManager.GetSpell(client.selectedSpell).Type == 1 || client.entityManager.GetSpell(client.selectedSpell).Type == 2)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[entityIndex].username + levelColourCode;
                                    client.menuActionID[client.menuOptionsCount] = 800;
                                    client.menuActionX[client.menuOptionsCount] = client.playerArray[entityIndex].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.playerArray[entityIndex].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[entityIndex].serverIndex;
                                    client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                    client.menuOptionsCount += 1;
                                }
                            }
                            else if (client.selectedItem >= 0)
                            {
                                client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[entityIndex].username + levelColourCode;
                                client.menuActionID[client.menuOptionsCount] = 810;
                                client.menuActionX[client.menuOptionsCount] = client.playerArray[entityIndex].currentX;
                                client.menuActionY[client.menuOptionsCount] = client.playerArray[entityIndex].currentY;
                                client.menuActionType[client.menuOptionsCount] = client.playerArray[entityIndex].serverIndex;
                                client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                client.menuOptionsCount += 1;
                            }
                            else
                            {
                                if (northernWildernessBoundary > 0 && (client.playerArray[entityIndex].currentY - 64) / client.gridSize + client.wildY + client.areaY < 2203)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Attack";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[entityIndex].username + levelColourCode;

                                    if (levelDifference >= 0 && levelDifference < 5)
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 805;
                                    }
                                    else
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 2805;
                                    }

                                    client.menuActionX[client.menuOptionsCount] = client.playerArray[entityIndex].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.playerArray[entityIndex].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[entityIndex].serverIndex;
                                    client.menuOptionsCount += 1;
                                }
                                else if (Config.MembersFeatures)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Duel with";
                                    client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[entityIndex].username + levelColourCode;
                                    client.menuActionX[client.menuOptionsCount] = client.playerArray[entityIndex].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.playerArray[entityIndex].currentY;
                                    client.menuActionID[client.menuOptionsCount] = 2806;
                                    client.menuActionType[client.menuOptionsCount] = client.playerArray[entityIndex].serverIndex;
                                    client.menuOptionsCount += 1;
                                }

                                client.menuText1[client.menuOptionsCount] = "Trade with";
                                client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[entityIndex].username + levelColourCode;
                                client.menuActionID[client.menuOptionsCount] = 2810;
                                client.menuActionType[client.menuOptionsCount] = client.playerArray[entityIndex].serverIndex;
                                client.menuOptionsCount += 1;

                                client.menuText1[client.menuOptionsCount] = "Follow";
                                client.menuText2[client.menuOptionsCount] = "@whi@" + client.playerArray[entityIndex].username + levelColourCode;
                                client.menuActionID[client.menuOptionsCount] = 2820;
                                client.menuActionType[client.menuOptionsCount] = client.playerArray[entityIndex].serverIndex;
                                client.menuOptionsCount += 1;
                            }
                        }
                        else if (entityKind == 2)
                        {
                                if (client.selectedSpell >= 0)
                                {
                                    if (client.entityManager.GetSpell(client.selectedSpell).Type == 3)
                                    {
                                        client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                                        client.menuText2[client.menuOptionsCount] = "@lre@" + client.entityManager.GetItem(client.groundItemID[entityIndex]).Name;
                                        client.menuActionID[client.menuOptionsCount] = 200;
                                        client.menuActionX[client.menuOptionsCount] = client.groundItemX[entityIndex];
                                        client.menuActionY[client.menuOptionsCount] = client.groundItemY[entityIndex];
                                        client.menuActionType[client.menuOptionsCount] = client.groundItemID[entityIndex];
                                        client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                        client.menuOptionsCount += 1;
                                    }
                                }
                                else if (client.selectedItem >= 0)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                    client.menuText2[client.menuOptionsCount] = "@lre@" + client.entityManager.GetItem(client.groundItemID[entityIndex]).Name;
                                    client.menuActionID[client.menuOptionsCount] = 210;
                                    client.menuActionX[client.menuOptionsCount] = client.groundItemX[entityIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.groundItemY[entityIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.groundItemID[entityIndex];
                                    client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                    client.menuOptionsCount += 1;
                                }
                                else
                                {
                                    client.menuText1[client.menuOptionsCount] = "Take";
                                    client.menuText2[client.menuOptionsCount] = "@lre@" + client.entityManager.GetItem(client.groundItemID[entityIndex]).Name;
                                    client.menuActionID[client.menuOptionsCount] = 220;
                                    client.menuActionX[client.menuOptionsCount] = client.groundItemX[entityIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.groundItemY[entityIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.groundItemID[entityIndex];
                                    client.menuOptionsCount += 1;

                                    client.menuText1[client.menuOptionsCount] = "Examine";
                                    client.menuText2[client.menuOptionsCount] = "@lre@" + client.entityManager.GetItem(client.groundItemID[entityIndex]).Name;
                                    client.menuActionID[client.menuOptionsCount] = 3200;
                                    client.menuActionType[client.menuOptionsCount] = client.groundItemID[entityIndex];
                                    client.menuOptionsCount += 1;
                                }
                        }
                        else if (entityKind == 3)
                        {
                            string npcLevelColourCode = "";
                            int npcLevelDifference = -1;
                            int npcId = client.npcArray[entityIndex].npcId;

                            if (client.entityManager.GetNpc(npcId).IsAttackable > 0)
                            {
                                int npcAverageLevel = (client.entityManager.GetNpc(npcId).AttackLevel + client.entityManager.GetNpc(npcId).DefenceLevel + client.entityManager.GetNpc(npcId).StrengthLevel + client.entityManager.GetNpc(npcId).HealthLevel) / 4;
                                int playerAverageLevel = (client.playerStatBase[0] + client.playerStatBase[1] + client.playerStatBase[2] + client.playerStatBase[3] + 27) / 4;
                                npcLevelDifference = playerAverageLevel - npcAverageLevel;
                                npcLevelColourCode = "@yel@";

                                if (npcLevelDifference < 0)
                                {
                                    npcLevelColourCode = "@or1@";
                                }

                                if (npcLevelDifference < -3)
                                {
                                    npcLevelColourCode = "@or2@";
                                }

                                if (npcLevelDifference < -6)
                                {
                                    npcLevelColourCode = "@or3@";
                                }

                                if (npcLevelDifference < -9)
                                {
                                    npcLevelColourCode = "@red@";
                                }

                                if (npcLevelDifference > 0)
                                {
                                    npcLevelColourCode = "@gr1@";
                                }

                                if (npcLevelDifference > 3)
                                {
                                    npcLevelColourCode = "@gr2@";
                                }

                                if (npcLevelDifference > 6)
                                {
                                    npcLevelColourCode = "@gr3@";
                                }

                                if (npcLevelDifference > 9)
                                {
                                    npcLevelColourCode = "@gre@";
                                }

                                npcLevelColourCode = " " + npcLevelColourCode + "(level-" + npcAverageLevel + ")";
                            }

                            if (client.selectedSpell >= 0)
                            {
                                if (client.entityManager.GetSpell(client.selectedSpell).Type == 2)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                                    client.menuText2[client.menuOptionsCount] = "@yel@" + client.entityManager.GetNpc(client.npcArray[entityIndex].npcId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 700;
                                    client.menuActionX[client.menuOptionsCount] = client.npcArray[entityIndex].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.npcArray[entityIndex].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.npcArray[entityIndex].serverIndex;
                                    client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                    client.menuOptionsCount += 1;
                                }
                            }
                            else if (client.selectedItem >= 0)
                            {
                                client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                client.menuText2[client.menuOptionsCount] = "@yel@" + client.entityManager.GetNpc(client.npcArray[entityIndex].npcId).Name;
                                client.menuActionID[client.menuOptionsCount] = 710;
                                client.menuActionX[client.menuOptionsCount] = client.npcArray[entityIndex].currentX;
                                client.menuActionY[client.menuOptionsCount] = client.npcArray[entityIndex].currentY;
                                client.menuActionType[client.menuOptionsCount] = client.npcArray[entityIndex].serverIndex;
                                client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                client.menuOptionsCount += 1;
                            }
                            else
                            {
                                if (client.entityManager.GetNpc(npcId).IsAttackable > 0)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Attack";
                                    client.menuText2[client.menuOptionsCount] = "@yel@" + client.entityManager.GetNpc(client.npcArray[entityIndex].npcId).Name + npcLevelColourCode;

                                    if (npcLevelDifference >= 0)
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 715;
                                    }
                                    else
                                    {
                                        client.menuActionID[client.menuOptionsCount] = 2715;
                                    }

                                    client.menuActionX[client.menuOptionsCount] = client.npcArray[entityIndex].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.npcArray[entityIndex].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.npcArray[entityIndex].serverIndex;
                                    client.menuOptionsCount += 1;
                                }

                                client.menuText1[client.menuOptionsCount] = "Talk-to";
                                client.menuText2[client.menuOptionsCount] = "@yel@" + client.entityManager.GetNpc(client.npcArray[entityIndex].npcId).Name;
                                client.menuActionID[client.menuOptionsCount] = 720;
                                client.menuActionX[client.menuOptionsCount] = client.npcArray[entityIndex].currentX;
                                client.menuActionY[client.menuOptionsCount] = client.npcArray[entityIndex].currentY;
                                client.menuActionType[client.menuOptionsCount] = client.npcArray[entityIndex].serverIndex;
                                client.menuOptionsCount += 1;

                                if (client.entityManager.GetNpc(npcId).Command != "")
                                {
                                    client.menuText1[client.menuOptionsCount] = client.entityManager.GetNpc(npcId).Command;
                                    client.menuText2[client.menuOptionsCount] = "@yel@" + client.entityManager.GetNpc(client.npcArray[entityIndex].npcId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 725;
                                    client.menuActionX[client.menuOptionsCount] = client.npcArray[entityIndex].currentX;
                                    client.menuActionY[client.menuOptionsCount] = client.npcArray[entityIndex].currentY;
                                    client.menuActionType[client.menuOptionsCount] = client.npcArray[entityIndex].serverIndex;
                                    client.menuOptionsCount += 1;
                                }

                                client.menuText1[client.menuOptionsCount] = "Examine";
                                client.menuText2[client.menuOptionsCount] = "@yel@" + client.entityManager.GetNpc(client.npcArray[entityIndex].npcId).Name;
                                client.menuActionID[client.menuOptionsCount] = 3700;
                                client.menuActionType[client.menuOptionsCount] = client.npcArray[entityIndex].npcId;
                                client.menuOptionsCount += 1;
                            }
                        }
                    }
                    else if (highlightedObject is not null && highlightedObject.index >= 10000)
                    {
                        int wallObjectSlotIndex = highlightedObject.index - 10000;
                        int wallObjectId = client.wallObjectID[wallObjectSlotIndex];

                        if (!client.wallObjectAlreadyInMenu[wallObjectSlotIndex])
                        {
                            if (client.selectedSpell >= 0)
                            {
                                if (client.entityManager.GetSpell(client.selectedSpell).Type == 4)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                                    client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWallObject(wallObjectId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 300;
                                    client.menuActionX[client.menuOptionsCount] = client.wallObjectX[wallObjectSlotIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.wallObjectY[wallObjectSlotIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[wallObjectSlotIndex];
                                    client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                                    client.menuOptionsCount += 1;
                                }
                            }
                            else if (client.selectedItem >= 0)
                            {
                                client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWallObject(wallObjectId).Name;
                                client.menuActionID[client.menuOptionsCount] = 310;
                                client.menuActionX[client.menuOptionsCount] = client.wallObjectX[wallObjectSlotIndex];
                                client.menuActionY[client.menuOptionsCount] = client.wallObjectY[wallObjectSlotIndex];
                                client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[wallObjectSlotIndex];
                                client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                                client.menuOptionsCount += 1;
                            }
                            else
                            {
                                if (client.entityManager.GetWallObject(wallObjectId).Command1.ToLower() != "WalkTo")
                                {
                                    client.menuText1[client.menuOptionsCount] = client.entityManager.GetWallObject(wallObjectId).Command1;
                                    client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWallObject(wallObjectId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 320;
                                    client.menuActionX[client.menuOptionsCount] = client.wallObjectX[wallObjectSlotIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.wallObjectY[wallObjectSlotIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[wallObjectSlotIndex];
                                    client.menuOptionsCount += 1;
                                }

                                if (client.entityManager.GetWallObject(wallObjectId).Command2.ToLower() != "Examine")
                                {
                                    client.menuText1[client.menuOptionsCount] = client.entityManager.GetWallObject(wallObjectId).Command2;
                                    client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWallObject(wallObjectId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 2300;
                                    client.menuActionX[client.menuOptionsCount] = client.wallObjectX[wallObjectSlotIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.wallObjectY[wallObjectSlotIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.wallObjectDirection[wallObjectSlotIndex];
                                    client.menuOptionsCount += 1;
                                }

                                client.menuText1[client.menuOptionsCount] = "Examine";
                                client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWallObject(wallObjectId).Name;
                                client.menuActionID[client.menuOptionsCount] = 3300;
                                client.menuActionType[client.menuOptionsCount] = wallObjectId;
                                client.menuOptionsCount += 1;
                            }

                            client.wallObjectAlreadyInMenu[wallObjectSlotIndex] = true;
                        }
                    }
                    else if (highlightedObject is not null && highlightedObject.index >= 0)
                    {
                        int objectSlotIndex = highlightedObject.index;
                        int objectTypeId = client.objectType[objectSlotIndex];

                        if (!client.objectAlreadyInMenu[objectSlotIndex])
                        {
                            if (client.selectedSpell >= 0)
                            {
                                if (client.entityManager.GetSpell(client.selectedSpell).Type == 5)
                                {
                                    client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                                    client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWorldObject(objectTypeId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 400;
                                    client.menuActionX[client.menuOptionsCount] = client.objectX[objectSlotIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.objectY[objectSlotIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.objectRotation[objectSlotIndex];
                                    client.menuActionVar1[client.menuOptionsCount] = client.objectType[objectSlotIndex];
                                    client.menuActionVar2[client.menuOptionsCount] = client.selectedSpell;
                                    client.menuOptionsCount += 1;
                                }
                            }
                            else if (client.selectedItem >= 0)
                            {
                                client.menuText1[client.menuOptionsCount] = "Use " + client.selectedItemName + " with";
                                client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWorldObject(objectTypeId).Name;
                                client.menuActionID[client.menuOptionsCount] = 410;
                                client.menuActionX[client.menuOptionsCount] = client.objectX[objectSlotIndex];
                                client.menuActionY[client.menuOptionsCount] = client.objectY[objectSlotIndex];
                                client.menuActionType[client.menuOptionsCount] = client.objectRotation[objectSlotIndex];
                                client.menuActionVar1[client.menuOptionsCount] = client.objectType[objectSlotIndex];
                                client.menuActionVar2[client.menuOptionsCount] = client.selectedItem;
                                client.menuOptionsCount += 1;
                            }
                            else
                            {
                                if (client.entityManager.GetWorldObject(objectTypeId).Command1.ToLower() != "WalkTo")
                                {
                                    client.menuText1[client.menuOptionsCount] = client.entityManager.GetWorldObject(objectTypeId).Command1;
                                    client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWorldObject(objectTypeId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 420;
                                    client.menuActionX[client.menuOptionsCount] = client.objectX[objectSlotIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.objectY[objectSlotIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.objectRotation[objectSlotIndex];
                                    client.menuActionVar1[client.menuOptionsCount] = client.objectType[objectSlotIndex];
                                    client.menuOptionsCount += 1;
                                }

                                if (client.entityManager.GetWorldObject(objectTypeId).Command2.ToLower() != "Examine")
                                {
                                    client.menuText1[client.menuOptionsCount] = client.entityManager.GetWorldObject(objectTypeId).Command2;
                                    client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWorldObject(objectTypeId).Name;
                                    client.menuActionID[client.menuOptionsCount] = 2400;
                                    client.menuActionX[client.menuOptionsCount] = client.objectX[objectSlotIndex];
                                    client.menuActionY[client.menuOptionsCount] = client.objectY[objectSlotIndex];
                                    client.menuActionType[client.menuOptionsCount] = client.objectRotation[objectSlotIndex];
                                    client.menuActionVar1[client.menuOptionsCount] = client.objectType[objectSlotIndex];
                                    client.menuOptionsCount += 1;
                                }

                                client.menuText1[client.menuOptionsCount] = "Examine";
                                client.menuText2[client.menuOptionsCount] = "@cya@" + client.entityManager.GetWorldObject(objectTypeId).Name;
                                client.menuActionID[client.menuOptionsCount] = 3400;
                                client.menuActionType[client.menuOptionsCount] = objectTypeId;
                                client.menuOptionsCount += 1;
                            }

                            client.objectAlreadyInMenu[objectSlotIndex] = true;
                        }
                    }
                    else
                    {
                        int groundEntitySlotId = entitySlotId;

                        if (groundEntitySlotId >= 0)
                        {
                            groundEntitySlotId = highlightedObject.entityType[entitySlotId] - 0x30d40;
                        }

                        if (groundEntitySlotId >= 0)
                        {
                            groundEntityIndex = groundEntitySlotId;
                        }
                    }
                }
            }

            if (client.selectedSpell >= 0 && client.entityManager.GetSpell(client.selectedSpell).Type <= 1)
            {
                client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on self";
                client.menuText2[client.menuOptionsCount] = "";
                client.menuActionID[client.menuOptionsCount] = 1000;
                client.menuActionType[client.menuOptionsCount] = client.selectedSpell;
                client.menuOptionsCount += 1;
            }

            if (groundEntityIndex != -1)
            {
                if (client.selectedSpell >= 0)
                {
                    if (client.entityManager.GetSpell(client.selectedSpell).Type == 6)
                    {
                        client.menuText1[client.menuOptionsCount] = "Cast " + client.entityManager.GetSpell(client.selectedSpell).Name + " on ground";
                        client.menuText2[client.menuOptionsCount] = "";
                        client.menuActionID[client.menuOptionsCount] = 900;
                        client.menuActionX[client.menuOptionsCount] = client.engineHandle.selectedX[groundEntityIndex];
                        client.menuActionY[client.menuOptionsCount] = client.engineHandle.selectedY[groundEntityIndex];
                        client.menuActionType[client.menuOptionsCount] = client.selectedSpell;
                        client.menuOptionsCount += 1;
                        return;
                    }
                }
                else if (client.selectedItem < 0)
                {
                    client.menuText1[client.menuOptionsCount] = "Walk here";
                        client.menuText2[client.menuOptionsCount] = "";
                        client.menuActionID[client.menuOptionsCount] = 920;
                        client.menuActionX[client.menuOptionsCount] = client.engineHandle.selectedX[groundEntityIndex];
                        client.menuActionY[client.menuOptionsCount] = client.engineHandle.selectedY[groundEntityIndex];
                        client.menuOptionsCount += 1;
                }
            }
        }

        public void GetMenuHighlighted()
        {
            if (client.drawMenuTab == 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 && client.mouseY < 35)
            {
                client.drawMenuTab = 1;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 33 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 33 && client.mouseY < 35)
            {
                client.drawMenuTab = 2;
                client.minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                client.minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (client.drawMenuTab == 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 66 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 66 && client.mouseY < 35)
            {
                client.drawMenuTab = 3;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 99 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 99 && client.mouseY < 35)
            {
                client.drawMenuTab = 4;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 132 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 132 && client.mouseY < 35)
            {
                client.drawMenuTab = 5;
            }

            if (client.drawMenuTab == 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 165 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 165 && client.mouseY < 35)
            {
                client.drawMenuTab = 6;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 && client.mouseY < 26)
            {
                client.drawMenuTab = 1;
            }

            if (client.drawMenuTab != 0 && client.drawMenuTab != 2 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 33 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 33 && client.mouseY < 26)
            {
                client.drawMenuTab = 2;
                client.minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                client.minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (client.drawMenuTab != 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 66 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 66 && client.mouseY < 26)
            {
                client.drawMenuTab = 3;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 99 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 99 && client.mouseY < 26)
            {
                client.drawMenuTab = 4;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 132 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 132 && client.mouseY < 26)
            {
                client.drawMenuTab = 5;
            }

            if (client.drawMenuTab != 0 && client.mouseX >= client.gameGraphics.gameWidth - 35 - 165 && client.mouseY >= 3 && client.mouseX < client.gameGraphics.gameWidth - 3 - 165 && client.mouseY < 26)
            {
                client.drawMenuTab = 6;
            }

            if (client.drawMenuTab == 1 && (client.mouseX < client.gameGraphics.gameWidth - 248 || client.mouseY > 36 + client.maxInventoryItems / 5 * 34))
            {
                client.drawMenuTab = 0;
            }

            if (client.drawMenuTab == 3 && (client.mouseX < client.gameGraphics.gameWidth - 199 || client.mouseY > 316))
            {
                client.drawMenuTab = 0;
            }

            if ((client.drawMenuTab == 2 || client.drawMenuTab == 4 || client.drawMenuTab == 5) && (client.mouseX < client.gameGraphics.gameWidth - 199 || client.mouseY > 240))
            {
                client.drawMenuTab = 0;
            }

            if (client.drawMenuTab == 6 && (client.mouseX < client.gameGraphics.gameWidth - 199 || client.mouseY > 326))
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

                    if (client.useChatFilter)
                    {
                        message = ChatFilter.FilterChat(message);
                    }

                    client.DisplayMessage("@pri@You tell " + DataOperations.HashToName(recipient) + ": " + message);
                    return true;
                }
            }
            catch (Exception exception)
            {
            }

            return false;
        }

    }

}
