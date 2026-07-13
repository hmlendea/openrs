using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using System;

namespace OpenRS.Net.Client.Entities
{
    public sealed class ClientEntityHandler(GameClient client)
    {

        public void UpdateAppearanceWindow()
        {
            client.appearanceMenu.MouseClick(client.mouseX, client.mouseY, client.lastMouseButton, client.mouseButton);
            if (client.appearanceMenu.IsClicked(client.appearanceHeadLeftArrow))
            {
                do
                {
                    client.appearanceHeadType = (client.appearanceHeadType - 1 + GameData.animationCount) % GameData.animationCount;
                }
                while ((GameData.animationGenderModels[client.appearanceHeadType] & 3) != 1 || (GameData.animationGenderModels[client.appearanceHeadType] & 4 * client.appearanceHeadGender) == 0);
            }

            if (client.appearanceMenu.IsClicked(client.appearanceHeadRightArrow))
            {
                do
                {
                    client.appearanceHeadType = (client.appearanceHeadType + 1) % GameData.animationCount;
                }
                while ((GameData.animationGenderModels[client.appearanceHeadType] & 3) != 1 || (GameData.animationGenderModels[client.appearanceHeadType] & 4 * client.appearanceHeadGender) == 0);
            }

            if (client.appearanceMenu.IsClicked(client.appearanceHairLeftArrow))
            {
                client.appearanceHairColour = (client.appearanceHairColour - 1 + client.appearanceHairColours.Length) % client.appearanceHairColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceHairRightArrow))
            {
                client.appearanceHairColour = (client.appearanceHairColour + 1) % client.appearanceHairColours.Length;
            }

            if (client.appearanceMenu.IsClicked(client.appearanceGenderLeftArrow) || client.appearanceMenu.IsClicked(client.appearanceGenderRightArrow))
            {
                for (client.appearanceHeadGender = 3 - client.appearanceHeadGender; (GameData.animationGenderModels[client.appearanceHeadType] & 3) != 1 || (GameData.animationGenderModels[client.appearanceHeadType] & 4 * client.appearanceHeadGender) == 0; client.appearanceHeadType = (client.appearanceHeadType + 1) % GameData.animationCount)
                {
                    ;
                }

                for (; (GameData.animationGenderModels[client.appearanceBodyGender] & 3) != 2 || (GameData.animationGenderModels[client.appearanceBodyGender] & 4 * client.appearanceHeadGender) == 0; client.appearanceBodyGender = (client.appearanceBodyGender + 1) % GameData.animationCount)
                {
                    ;
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
                client.streamClass.CreatePacket(218);
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
                if (client.gameCamera is not null)
                {
                    client.gameCamera.CleanUp();
                    client.gameCamera = null;
                }
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
                //System.gc();
                GC.Collect();
                return;
            }
            catch (Exception _ex)
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
            ClientMob existingPlayer = client.playerBufferArray[index];
            bool flag = false;
            for (int l = 0; l < client.lastPlayerCount; l += 1)
            {
                if (client.lastPlayerArray[l].serverIndex != index)
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (flag)
            {
                existingPlayer.nextSprite = sprite;
                int i1 = existingPlayer.waypointCurrent;
                if (x != existingPlayer.waypointsX[i1] || y != existingPlayer.waypointsY[i1])
                {
                    existingPlayer.waypointCurrent = i1 = (i1 + 1) % 10;
                    existingPlayer.waypointsX[i1] = x;
                    existingPlayer.waypointsY[i1] = y;
                }
            }
            else
            {
                existingPlayer.serverIndex = index;
                existingPlayer.waypointsEndSprite = 0;
                existingPlayer.waypointCurrent = 0;
                existingPlayer.waypointsX[0] = existingPlayer.currentX = x;
                existingPlayer.waypointsY[0] = existingPlayer.currentY = y;
                existingPlayer.nextSprite = existingPlayer.currentSprite = sprite;
                existingPlayer.stepCount = 0;
            }
            client.playerArray[client.playerCount++] = existingPlayer;
            return existingPlayer;
        }
        public GameObject CreateWallObject(int x, int y, int dir, int type, int totalCount)
        {

            int tileX = x;
            int tileY = y;
            int destTileX = x;
            int destTileY = y;
            int textureBack = GameData.wallObjectModel_FaceBack[type];
            int textureFront = GameData.wallObjectModel_FaceFront[type];
            int wallHeight = GameData.wallObjectModelHeight[type];
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

            // add vertex index bottomLeft
            int bLeft = wallModel.GetVertexIndex(tileX, -client.engineHandle.GetAveragedElevation(tileX, tileY), tileY);

            // add vertex index topLeft
            int tLeft = wallModel.GetVertexIndex(tileX, -client.engineHandle.GetAveragedElevation(tileX, tileY) - wallHeight, tileY);

            // add vertex index topRight
            int tRight = wallModel.GetVertexIndex(destTileX, -client.engineHandle.GetAveragedElevation(destTileX, destTileY) - wallHeight, destTileY);

            // vertex index bottomRight
            int bRight = wallModel.GetVertexIndex(destTileX, -client.engineHandle.GetAveragedElevation(destTileX, destTileY), destTileY);
            int[] faceVertices = [
            bLeft, tLeft, tRight, bRight
        ];
            wallModel.AddFaceVertices(4, faceVertices, textureBack, textureFront);
            wallModel.UpdateShading(false, 60, 24, -50, -10, -50);
            if (x >= 0 && y >= 0 && x < 96 && y < 96)
            {
                client.gameCamera.AddModel(wallModel);
            }

            wallModel.index = totalCount + 10000;
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
            ClientMob f1 = client.npcAttackingArray[index];
            bool flag = false;
            for (int l = 0; l < client.lastNpcCount; l += 1)
            {
                if (client.lastNpcArray[l].serverIndex != index)
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (flag)
            {
                f1.npcId = id;
                f1.nextSprite = sprite;
                int i1 = f1.waypointCurrent;
                if (x != f1.waypointsX[i1] || y != f1.waypointsY[i1])
                {
                    f1.waypointCurrent = i1 = (i1 + 1) % 10;
                    f1.waypointsX[i1] = x;
                    f1.waypointsY[i1] = y;
                }
            }
            else
            {
                f1.serverIndex = index;
                f1.waypointsEndSprite = 0;
                f1.waypointCurrent = 0;
                f1.waypointsX[0] = f1.currentX = x;
                f1.waypointsY[0] = f1.currentY = y;
                f1.npcId = id;
                f1.nextSprite = f1.currentSprite = sprite;
                f1.stepCount = 0;
            }
            client.npcArray[client.npcCount++] = f1;
            return f1;
        }
        public void UpdateBankItems()
        {
            client.bankItemsCount = client.serverBankItemsCount;
            for (int l = 0; l < client.serverBankItemsCount; l += 1)
            {
                client.bankItems[l] = client.serverBankItems[l];
                client.bankItemCount[l] = client.serverBankItemCount[l];
            }

            for (int i1 = 0; i1 < client.inventoryItemsCount; i1 += 1)
            {
                if (client.bankItemsCount >= client.maxBankItems)
                {
                    break;
                }

                int j1 = client.inventoryItems[i1];
                bool flag = false;
                for (int k1 = 0; k1 < client.bankItemsCount; k1 += 1)
                {
                    if (client.bankItems[k1] != j1)
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    client.bankItems[client.bankItemsCount] = j1;
                    client.bankItemCount[client.bankItemsCount] = 0;
                    client.bankItemsCount += 1;
                }
            }

        }
        public ClientMob GetLastPlayer(int serverIndex)
        {
            for (int i1 = 0; i1 < client.lastPlayerCount; i1 += 1)
            {
                if (client.lastPlayerArray[i1].serverIndex == serverIndex)
                {
                    return client.lastPlayerArray[i1];
                }
            }
            return null;
        }
        public ClientMob GetLastNpc(int serverIndex)
        {
            for (int i1 = 0; i1 < client.lastNpcCount; i1 += 1)
            {
                if (client.lastNpcArray[i1].serverIndex == serverIndex)
                {
                    return client.lastNpcArray[i1];
                }
            }
            return null;
        }

    }

}
