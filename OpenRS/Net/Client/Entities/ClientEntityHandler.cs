using System;

using OpenRS.Net;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Entities
{
    public sealed class ClientEntityHandler(GameClient client)
    {
        private static int WaypointRingSize => 10;
        private static int WallObjectIndexOffset => 10000;
        private static int TileBoundsMax => 96;

        public void UpdateAppearanceWindow()
        {
            client.appearanceMenu.MouseClick(client.mouseX, client.mouseY, client.lastMouseButton, client.mouseButton);

            if (client.appearanceMenu.IsClicked(client.appearanceHeadLeftArrow))
            {
                do
                {
                    client.appearanceHeadType = (client.appearanceHeadType - 1 + client.entityManager.AnimationCount) % client.entityManager.AnimationCount;
                }
                while ((client.entityManager.GetAnimation(client.appearanceHeadType).GenderModel & 3) != 1 ||
                       (client.entityManager.GetAnimation(client.appearanceHeadType).GenderModel & 4 * client.appearanceHeadGender) == 0);
            }

            if (client.appearanceMenu.IsClicked(client.appearanceHeadRightArrow))
            {
                do
                {
                    client.appearanceHeadType = (client.appearanceHeadType + 1) % client.entityManager.AnimationCount;
                }
                while ((client.entityManager.GetAnimation(client.appearanceHeadType).GenderModel & 3) != 1 ||
                       (client.entityManager.GetAnimation(client.appearanceHeadType).GenderModel & 4 * client.appearanceHeadGender) == 0);
            }

            if (client.appearanceMenu.IsClicked(client.appearanceHairLeftArrow))
            {
                client.appearanceHairColour = (client.appearanceHairColour - 1 + client.appearanceHairColours.Length) % client.appearanceHairColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceHairRightArrow))
            {
                client.appearanceHairColour = (client.appearanceHairColour + 1) % client.appearanceHairColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceGenderLeftArrow) ||
                client.appearanceMenu.IsClicked(client.appearanceGenderRightArrow))
            {
                client.appearanceHeadGender = 3 - client.appearanceHeadGender;

                while ((client.entityManager.GetAnimation(client.appearanceHeadType).GenderModel & 3) != 1 ||
                       (client.entityManager.GetAnimation(client.appearanceHeadType).GenderModel & 4 * client.appearanceHeadGender) == 0)
                {
                    client.appearanceHeadType = (client.appearanceHeadType + 1) % client.entityManager.AnimationCount;
                }

                while ((client.entityManager.GetAnimation(client.appearanceBodyGender).GenderModel & 3) != 2 ||
                       (client.entityManager.GetAnimation(client.appearanceBodyGender).GenderModel & 4 * client.appearanceHeadGender) == 0)
                {
                    client.appearanceBodyGender = (client.appearanceBodyGender + 1) % client.entityManager.AnimationCount;
                }
            }

            if (client.appearanceMenu.IsClicked(client.appearanceTopLeftArrow))
            {
                client.appearanceTopColour = (client.appearanceTopColour - 1 + client.appearanceTopBottomColours.Length) % client.appearanceTopBottomColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceTopRightArrow))
            {
                client.appearanceTopColour = (client.appearanceTopColour + 1) % client.appearanceTopBottomColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceSkinLeftArrow))
            {
                client.appearanceSkinColour = (client.appearanceSkinColour - 1 + client.appearanceSkinColours.Length) % client.appearanceSkinColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceSkingRightArrow))
            {
                client.appearanceSkinColour = (client.appearanceSkinColour + 1) % client.appearanceSkinColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceBottomLeftArrow))
            {
                client.appearanceBottomColour = (client.appearanceBottomColour - 1 + client.appearanceTopBottomColours.Length) % client.appearanceTopBottomColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceBottomRightArrow))
            {
                client.appearanceBottomColour = (client.appearanceBottomColour + 1) % client.appearanceTopBottomColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceAcceptButton))
            {
                client.streamClass.CreatePacket((int)ClientPacket.UpdateAppearance);
                client.streamClass.AddByte(client.appearanceHeadGender);
                client.streamClass.AddByte(client.appearanceHeadType);
                client.streamClass.AddByte(client.appearanceBodyGender);
                client.streamClass.AddByte(client.appearance2Colour);
                client.streamClass.AddByte(client.appearanceHairColour);
                client.streamClass.AddByte(client.appearanceTopColour);
                client.streamClass.AddByte(client.appearanceBottomColour);
                client.streamClass.AddByte(client.appearanceSkinColour);
                client.streamClass.FormatPacket();
                client.gameGraphics.ClearScreen();
                client.showAppearanceWindow = false;
            }
        }

        public void CleanUp()
        {
            try
            {
                if (client.gameGraphics is not null)
                {
                    client.gameGraphics.CleanUp();
                    client.gameGraphics.pixels = null;
                    client.gameGraphics = null;
                }

                client.gameCamera?.CleanUp();
                client.gameCamera = null;
                client.gameDataObjects = null;
                client.objectArray = null;
                client.wallObjectArray = null;
                client.playerBufferArray = null;
                client.playerArray = null;
                client.npcAttackingArray = null;
                client.npcArray = null;
                client.ourPlayer = null;

                if (client.engineHandle is not null)
                {
                    client.engineHandle.TileChunks = null;
                    client.engineHandle.wallObject = null;
                    client.engineHandle.roofObject = null;
                    client.engineHandle.currentSectionObject = null;
                    client.engineHandle = null;
                }

                GC.Collect();
            }
            catch (Exception)
            {
                return;
            }
        }

        public ClientMob CreatePlayer(int index, int x, int y, int sprite)
        {
            if (client.playerBufferArray[index] is null)
            {
                client.playerBufferArray[index] = new ClientMob
                {
                    serverIndex = index,
                    serverID = 0
                };
            }

            ClientMob player = client.playerBufferArray[index];
            bool playerWasKnown = IsInLastPlayerArray(index);

            if (playerWasKnown)
            {
                player.nextSprite = sprite;
                int waypointIndex = player.waypointCurrent;

                if (x != player.waypointsX[waypointIndex] || y != player.waypointsY[waypointIndex])
                {
                    player.waypointCurrent = waypointIndex = (waypointIndex + 1) % WaypointRingSize;
                    player.waypointsX[waypointIndex] = x;
                    player.waypointsY[waypointIndex] = y;
                }
            }
            else
            {
                player.serverIndex = index;
                player.waypointsEndSprite = 0;
                player.waypointCurrent = 0;
                player.waypointsX[0] = player.currentX = x;
                player.waypointsY[0] = player.currentY = y;
                player.nextSprite = player.currentSprite = sprite;
                player.stepCount = 0;
            }

            client.playerArray[client.playerCount] = player;
            client.playerCount += 1;

            return player;
        }

        public GameObject CreateWallObject(int x, int y, int dir, int type, int totalCount)
        {
            int tileX = x;
            int tileY = y;
            int destTileX = x;
            int destTileY = y;
            int textureBack = client.entityManager.GetWallObject(type).ModelFaceBack;
            int textureFront = client.entityManager.GetWallObject(type).ModelFaceFront;
            int wallHeight = client.entityManager.GetWallObject(type).ModelHeight;
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

            tileX *= client.gridSize;
            tileY *= client.gridSize;
            destTileX *= client.gridSize;
            destTileY *= client.gridSize;

            int bottomLeft = wallModel.GetVertexIndex(tileX, -client.engineHandle.GetAveragedElevation(tileX, tileY), tileY);
            int topLeft = wallModel.GetVertexIndex(tileX, -client.engineHandle.GetAveragedElevation(tileX, tileY) - wallHeight, tileY);
            int topRight = wallModel.GetVertexIndex(destTileX, -client.engineHandle.GetAveragedElevation(destTileX, destTileY) - wallHeight, destTileY);
            int bottomRight = wallModel.GetVertexIndex(destTileX, -client.engineHandle.GetAveragedElevation(destTileX, destTileY), destTileY);
            int[] faceVertices = [bottomLeft, topLeft, topRight, bottomRight];

            wallModel.AddFaceVertices(4, faceVertices, textureBack, textureFront);
            wallModel.UpdateShading(false, 60, 24, -50, -10, -50);

            if (x >= 0 && y >= 0 && x < TileBoundsMax && y < TileBoundsMax)
            {
                client.gameCamera.AddModel(wallModel);
            }

            wallModel.index = totalCount + WallObjectIndexOffset;

            return wallModel;
        }

        public void ResetPrivateMessages()
        {
            client.pmText = "";
            client.enteredPMText = "";
        }

        public ClientMob CreateNpc(int index, int x, int y, int sprite, int id)
        {
            if (client.npcAttackingArray[index] is null)
            {
                client.npcAttackingArray[index] = new ClientMob
                {
                    serverIndex = index
                };
            }

            ClientMob npc = client.npcAttackingArray[index];
            bool npcWasKnown = IsInLastNpcArray(index);

            if (npcWasKnown)
            {
                npc.npcId = id;
                npc.nextSprite = sprite;
                int waypointIndex = npc.waypointCurrent;

                if (x != npc.waypointsX[waypointIndex] || y != npc.waypointsY[waypointIndex])
                {
                    npc.waypointCurrent = waypointIndex = (waypointIndex + 1) % WaypointRingSize;
                    npc.waypointsX[waypointIndex] = x;
                    npc.waypointsY[waypointIndex] = y;
                }
            }
            else
            {
                npc.serverIndex = index;
                npc.waypointsEndSprite = 0;
                npc.waypointCurrent = 0;
                npc.waypointsX[0] = npc.currentX = x;
                npc.waypointsY[0] = npc.currentY = y;
                npc.npcId = id;
                npc.nextSprite = npc.currentSprite = sprite;
                npc.stepCount = 0;
            }

            client.npcArray[client.npcCount] = npc;
            client.npcCount += 1;

            return npc;
        }

        public void UpdateBankItems()
        {
            client.bankItemsCount = client.serverBankItemsCount;

            for (int bankIndex = 0; bankIndex < client.serverBankItemsCount; bankIndex += 1)
            {
                client.bankItems[bankIndex] = client.serverBankItems[bankIndex];
                client.bankItemCount[bankIndex] = client.serverBankItemCount[bankIndex];
            }

            for (int inventoryIndex = 0; inventoryIndex < client.inventoryItemsCount; inventoryIndex += 1)
            {
                if (client.bankItemsCount >= client.maxBankItems)
                {
                    break;
                }

                int itemId = client.inventoryItems[inventoryIndex];
                bool itemWasAlreadyBanked = false;

                for (int bankItemIndex = 0; bankItemIndex < client.bankItemsCount; bankItemIndex += 1)
                {
                    if (client.bankItems[bankItemIndex] != itemId)
                    {
                        continue;
                    }

                    itemWasAlreadyBanked = true;
                    break;
                }

                if (!itemWasAlreadyBanked)
                {
                    client.bankItems[client.bankItemsCount] = itemId;
                    client.bankItemCount[client.bankItemsCount] = 0;
                    client.bankItemsCount += 1;
                }
            }
        }

        public ClientMob GetLastPlayer(int serverIndex)
        {
            for (int playerIndex = 0; playerIndex < client.lastPlayerCount; playerIndex += 1)
            {
                if (client.lastPlayerArray[playerIndex].serverIndex == serverIndex)
                {
                    return client.lastPlayerArray[playerIndex];
                }
            }

            return null;
        }

        public ClientMob GetLastNpc(int serverIndex)
        {
            for (int npcIndex = 0; npcIndex < client.lastNpcCount; npcIndex += 1)
            {
                if (client.lastNpcArray[npcIndex].serverIndex == serverIndex)
                {
                    return client.lastNpcArray[npcIndex];
                }
            }

            return null;
        }

        private bool IsInLastPlayerArray(int serverIndex)
        {
            for (int playerIndex = 0; playerIndex < client.lastPlayerCount; playerIndex += 1)
            {
                if (client.lastPlayerArray[playerIndex].serverIndex == serverIndex)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInLastNpcArray(int serverIndex)
        {
            for (int npcIndex = 0; npcIndex < client.lastNpcCount; npcIndex += 1)
            {
                if (client.lastNpcArray[npcIndex].serverIndex == serverIndex)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
