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
        public void UpdateAppearanceWindow()
        {
            appearanceMenu.MouseClick(mouseX, mouseY, lastMouseButton, mouseButton);
            if (appearanceMenu.IsClicked(appearanceHeadLeftArrow))
            {
                do
                {
                    appearanceHeadType = ((appearanceHeadType - 1) + GameData.animationCount) % GameData.animationCount;
                }
                while ((GameData.animationGenderModels[appearanceHeadType] & 3) != 1 || (GameData.animationGenderModels[appearanceHeadType] & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.IsClicked(appearanceHeadRightArrow))
            {
                do
                {
                    appearanceHeadType = (appearanceHeadType + 1) % GameData.animationCount;
                }
                while ((GameData.animationGenderModels[appearanceHeadType] & 3) != 1 || (GameData.animationGenderModels[appearanceHeadType] & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.IsClicked(appearanceHairLeftArrow))
            {
                appearanceHairColour = ((appearanceHairColour - 1) + appearanceHairColours.Length) % appearanceHairColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceHairRightArrow))
            {
                appearanceHairColour = (appearanceHairColour + 1) % appearanceHairColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceGenderLeftArrow) || appearanceMenu.IsClicked(appearanceGenderRightArrow))
            {
                for (appearanceHeadGender = 3 - appearanceHeadGender; (GameData.animationGenderModels[appearanceHeadType] & 3) != 1 || (GameData.animationGenderModels[appearanceHeadType] & 4 * appearanceHeadGender) == 0; appearanceHeadType = (appearanceHeadType + 1) % GameData.animationCount)
                {
                    ;
                }

                for (; (GameData.animationGenderModels[appearanceBodyGender] & 3) != 2 || (GameData.animationGenderModels[appearanceBodyGender] & 4 * appearanceHeadGender) == 0; appearanceBodyGender = (appearanceBodyGender + 1) % GameData.animationCount)
                {
                    ;
                }
            }
            if (appearanceMenu.IsClicked(appearanceTopLeftArrow))
            {
                appearanceTopColour = ((appearanceTopColour - 1) + appearanceTopBottomColours.Length) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceTopRightArrow))
            {
                appearanceTopColour = (appearanceTopColour + 1) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceSkinLeftArrow))
            {
                appearanceSkinColour = ((appearanceSkinColour - 1) + appearanceSkinColours.Length) % appearanceSkinColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceSkingRightArrow))
            {
                appearanceSkinColour = (appearanceSkinColour + 1) % appearanceSkinColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceBottomLeftArrow))
            {
                appearanceBottomColour = ((appearanceBottomColour - 1) + appearanceTopBottomColours.Length) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceBottomRightArrow))
            {
                appearanceBottomColour = (appearanceBottomColour + 1) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.IsClicked(appearanceAcceptButton))
            {
                streamClass.CreatePacket(218);
                streamClass.AddByte(appearanceHeadGender);
                streamClass.AddByte(appearanceHeadType);
                streamClass.AddByte(appearanceBodyGender);
                streamClass.AddByte(appearance2Colour);
                streamClass.AddByte(appearanceHairColour);
                streamClass.AddByte(appearanceTopColour);
                streamClass.AddByte(appearanceBottomColour);
                streamClass.AddByte(appearanceSkinColour);
                streamClass.FormatPacket();
                gameGraphics.ClearScreen();
                showAppearanceWindow = false;
            }
        }
        public void CleanUp()
        {
            try
            {
                if (gameGraphics is not null)
                {
                    gameGraphics.CleanUp();
                    gameGraphics.pixels = null;
                    gameGraphics = null;
                }
                if (gameCamera is not null)
                {
                    gameCamera.CleanUp();
                    gameCamera = null;
                }
                gameDataObjects = null;
                objectArray = null;
                wallObjectArray = null;
                playerBufferArray = null;
                playerArray = null;
                npcAttackingArray = null;
                npcArray = null;
                ourPlayer = null;
                if (engineHandle is not null)
                {
                    engineHandle.TileChunks = null;
                    engineHandle.wallObject = null;
                    engineHandle.roofObject = null;
                    engineHandle.currentSectionObject = null;
                    engineHandle = null;
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
            if (playerBufferArray[index] is null)
            {
                playerBufferArray[index] = new ClientMob
                {
                    serverIndex = index,
                    serverID = 0
                };
            }
            ClientMob existingPlayer = playerBufferArray[index];
            bool flag = false;
            for (int l = 0; l < lastPlayerCount; l += 1)
            {
                if (lastPlayerArray[l].serverIndex != index)
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
            playerArray[playerCount++] = existingPlayer;
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
            tileX *= gridSize;
            tileY *= gridSize;
            destTileX *= gridSize;
            destTileY *= gridSize;

            // add vertex index bottomLeft
            int bLeft = wallModel.GetVertexIndex(tileX, -engineHandle.GetAveragedElevation(tileX, tileY), tileY);

            // add vertex index topLeft
            int tLeft = wallModel.GetVertexIndex(tileX, -engineHandle.GetAveragedElevation(tileX, tileY) - wallHeight, tileY);

            // add vertex index topRight
            int tRight = wallModel.GetVertexIndex(destTileX, -engineHandle.GetAveragedElevation(destTileX, destTileY) - wallHeight, destTileY);

            // vertex index bottomRight
            int bRight = wallModel.GetVertexIndex(destTileX, -engineHandle.GetAveragedElevation(destTileX, destTileY), destTileY);
            int[] faceVertices = [
            bLeft, tLeft, tRight, bRight
        ];
            wallModel.AddFaceVertices(4, faceVertices, textureBack, textureFront);
            wallModel.UpdateShading(false, 60, 24, -50, -10, -50);
            if (x >= 0 && y >= 0 && x < 96 && y < 96)
            {
                gameCamera.AddModel(wallModel);
            }

            wallModel.index = totalCount + 10000;
            return wallModel;
        }
        public void ResetPrivateMessages()
        {
            pmText = "";
            enteredPMText = "";
        }
        public ClientMob CreateNpc(int index, int x, int y, int sprite, int id)
        {
            if (npcAttackingArray[index] is null)
            {
                npcAttackingArray[index] = new ClientMob
                {
                    serverIndex = index
                };
            }
            ClientMob f1 = npcAttackingArray[index];
            bool flag = false;
            for (int l = 0; l < lastNpcCount; l += 1)
            {
                if (lastNpcArray[l].serverIndex != index)
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
            npcArray[npcCount++] = f1;
            return f1;
        }
        public void UpdateBankItems()
        {
            bankItemsCount = serverBankItemsCount;
            for (int l = 0; l < serverBankItemsCount; l += 1)
            {
                bankItems[l] = serverBankItems[l];
                bankItemCount[l] = serverBankItemCount[l];
            }

            for (int i1 = 0; i1 < inventoryItemsCount; i1 += 1)
            {
                if (bankItemsCount >= maxBankItems)
                {
                    break;
                }

                int j1 = inventoryItems[i1];
                bool flag = false;
                for (int k1 = 0; k1 < bankItemsCount; k1 += 1)
                {
                    if (bankItems[k1] != j1)
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    bankItems[bankItemsCount] = j1;
                    bankItemCount[bankItemsCount] = 0;
                    bankItemsCount += 1;
                }
            }

        }
        public ClientMob GetLastPlayer(int serverIndex)
        {
            for (int i1 = 0; i1 < lastPlayerCount; i1 += 1)
            {
                if (lastPlayerArray[i1].serverIndex == serverIndex)
                {
                    return lastPlayerArray[i1];
                }
            }
            return null;
        }
        public ClientMob GetLastNpc(int serverIndex)
        {
            for (int i1 = 0; i1 < lastNpcCount; i1 += 1)
            {
                if (lastNpcArray[i1].serverIndex == serverIndex)
                {
                    return lastNpcArray[i1];
                }
            }
            return null;
        }
    }

}
