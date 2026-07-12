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
        public override void HandlePacket(int packetID, int packetLength, sbyte[] packetData)
        {
            try
            {
                //base.handlePacket(packetID, packetLength, packetData);
                if (packetID == 145)
                {
                    if (!hasWorldInfo)
                    {
                        return;
                    }

                    lastPlayerCount = playerCount;
                    for (int l = 0; l < lastPlayerCount; l += 1)
                    {
                        lastPlayerArray[l] = playerArray[l];
                    }

                    int off = 8;
                    sectionX = DataOperations.GetBits(packetData, off, 11);
                    off += 11;
                    sectionY = DataOperations.GetBits(packetData, off, 13);
                    off += 13;
                    int sprite = DataOperations.GetBits(packetData, off, 4);
                    off += 4;
                    bool sectionLoaded = LoadSection(sectionX, sectionY);
                    sectionX -= areaX;
                    sectionY -= areaY;
                    int mapEnterX = sectionX * gridSize + 64;
                    int mapEnterY = sectionY * gridSize + 64;
                    if (sectionLoaded)
                    {
                        ourPlayer.waypointCurrent = 0;
                        ourPlayer.waypointsEndSprite = 0;
                        ourPlayer.currentX = ourPlayer.waypointsX[0] = mapEnterX;
                        ourPlayer.currentY = ourPlayer.waypointsY[0] = mapEnterY;
                    }
                    playerCount = 0;
                    ourPlayer = CreatePlayer(serverIndex, mapEnterX, mapEnterY, sprite);
                    int newPlayerCount = DataOperations.GetBits(packetData, off, 8);
                    off += 8;
                    for (int currentNewPlayer = 0; currentNewPlayer < newPlayerCount; currentNewPlayer += 1)
                    {
                        //ClientMob mob = lastPlayerArray[currentNewPlayer + 1];
                        ClientMob mob = GetLastPlayer(DataOperations.GetBits(packetData, off, 16));
                        off += 16;
                        int playerAtTile = DataOperations.GetBits(packetData, off, 1);
                        off += 1;
                        if (playerAtTile != 0)
                        {
                            int waypointsLeft = DataOperations.GetBits(packetData, off, 1);
                            off += 1;
                            if (waypointsLeft == 0)
                            {
                                int currentNextSprite = DataOperations.GetBits(packetData, off, 3);
                                off += 3;
                                int currentWaypoint = mob.waypointCurrent;
                                int newWaypointX = mob.waypointsX[currentWaypoint];
                                int newWaypointY = mob.waypointsY[currentWaypoint];
                                if (currentNextSprite == 2 || currentNextSprite == 1 || currentNextSprite == 3)
                                {
                                    newWaypointX += gridSize;
                                }

                                if (currentNextSprite == 6 || currentNextSprite == 5 || currentNextSprite == 7)
                                {
                                    newWaypointX -= gridSize;
                                }

                                if (currentNextSprite == 4 || currentNextSprite == 3 || currentNextSprite == 5)
                                {
                                    newWaypointY += gridSize;
                                }

                                if (currentNextSprite == 0 || currentNextSprite == 1 || currentNextSprite == 7)
                                {
                                    newWaypointY -= gridSize;
                                }

                                mob.nextSprite = currentNextSprite;
                                mob.waypointCurrent = currentWaypoint = (currentWaypoint + 1) % 10;
                                mob.waypointsX[currentWaypoint] = newWaypointX;
                                mob.waypointsY[currentWaypoint] = newWaypointY;
                            }
                            else
                            {
                                int needsNextSprite = DataOperations.GetBits(packetData, off, 4);
                                off += 4;
                                if ((needsNextSprite & 0xc) == 12)
                                {
                                    continue;
                                }
                                mob.nextSprite = needsNextSprite;
                            }
                        }
                        playerArray[playerCount++] = mob;
                    }

                    int mobCount = 0;
                    while (off + 24 < packetLength * 8)
                    {
                        int mobIndex = DataOperations.GetBits(packetData, off, 16);
                        off += 16;
                        int areaMobX = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobX > 15)
                        {
                            areaMobX -= 32;
                        }

                        int areaMobY = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobY > 15)
                        {
                            areaMobY -= 32;
                        }

                        int mobSprite = DataOperations.GetBits(packetData, off, 4);
                        off += 4;
                        int addIndex = DataOperations.GetBits(packetData, off, 1);
                        off += 1;
                        int mobX = (sectionX + areaMobX) * gridSize + 64;
                        int mobY = (sectionY + areaMobY) * gridSize + 64;
                        CreatePlayer(mobIndex, mobX, mobY, mobSprite);
                        if (addIndex == 0)
                        {
                            playerBufferArrayIndexes[mobCount++] = mobIndex;
                        }
                    }
                    if (mobCount > 0)
                    {
                        streamClass.CreatePacket(83);
                        streamClass.AddShort(mobCount);
                        for (int k40 = 0; k40 < mobCount; k40 += 1)
                        {
                            ClientMob f5 = playerBufferArray[playerBufferArrayIndexes[k40]];
                            streamClass.AddShort(f5.serverIndex);
                            streamClass.AddShort(f5.serverID);
                        }

                        streamClass.FormatPacket();
                        mobCount = 0;
                    }
                    return;
                }
                if (packetID == 109)
                {
                    if (needsClear)
                    {
                        for (int i = 0; i < groundItemID.Length; i += 1)
                        {
                            groundItemX[i] = -1;
                            groundItemY[i] = -1;
                            groundItemID[i] = -1;
                            groundItemObjectVar[i] = -1;
                        }
                        groundItemCount = 0;
                        needsClear = false;
                    }
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = sectionX + packetData[off + 1] >> 3;
                            int newSectionY = sectionY + packetData[off + 2] >> 3;
                            off += 3;
                            for (int groundItem = 0; groundItem < groundItemCount; groundItem += 1)
                            {
                                int newX = (groundItemX[groundItem] >> 3) - newSectionX;
                                int newY = (groundItemY[groundItem] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (groundItem != newCount)
                                    {
                                        groundItemX[newCount] = groundItemX[groundItem];
                                        groundItemY[newCount] = groundItemY[groundItem];
                                        groundItemID[newCount] = groundItemID[groundItem];
                                        groundItemObjectVar[newCount] = groundItemObjectVar[groundItem];
                                    }
                                    newCount += 1;
                                }
                            }

                            groundItemCount = newCount;
                        }
                        else
                        {
                            int newID = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int newX = sectionX + packetData[off++];
                            int newY = sectionY + packetData[off++];
                            if ((newID & 0x8000) == 0)
                            {
                                groundItemX[groundItemCount] = newX;
                                groundItemY[groundItemCount] = newY;
                                groundItemID[groundItemCount] = newID;
                                groundItemObjectVar[groundItemCount] = 0;
                                for (int l23 = 0; l23 < objectCount; l23 += 1)
                                {
                                    if (objectX[l23] != newX || objectY[l23] != newY)
                                    {
                                        continue;
                                    }

                                    groundItemObjectVar[groundItemCount] = GameData.objectGroundItemVar[objectType[l23]];
                                    break;
                                }

                                groundItemCount += 1;
                            }
                            else
                            {
                                newID &= 0x7fff;
                                int updateIndex = 0;
                                for (int currentItemIndex = 0; currentItemIndex < groundItemCount; currentItemIndex += 1)
                                {
                                    if (groundItemX[currentItemIndex] != newX || groundItemY[currentItemIndex] != newY || groundItemID[currentItemIndex] != newID)
                                    {
                                        if (currentItemIndex != updateIndex)
                                        {
                                            groundItemX[updateIndex] = groundItemX[currentItemIndex];
                                            groundItemY[updateIndex] = groundItemY[currentItemIndex];
                                            groundItemID[updateIndex] = groundItemID[currentItemIndex];
                                            groundItemObjectVar[updateIndex] = groundItemObjectVar[currentItemIndex];
                                        }
                                        updateIndex += 1;
                                    }
                                    else
                                    {
                                        newID = -123;
                                    }
                                }

                                groundItemCount = updateIndex;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 27)
                {
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = sectionX + packetData[off + 1] >> 3;
                            int newSectionY = sectionY + packetData[off + 2] >> 3;
                            off += 3;
                            for (int _obj = 0; _obj < objectCount; _obj += 1)
                            {
                                int newX = (objectX[_obj] >> 3) - newSectionX;
                                int newY = (objectY[_obj] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (_obj != newCount)
                                    {
                                        objectArray[newCount] = objectArray[_obj];
                                        objectArray[newCount].index = newCount;
                                        objectX[newCount] = objectX[_obj];
                                        objectY[newCount] = objectY[_obj];
                                        objectType[newCount] = objectType[_obj];
                                        objectRotation[newCount] = objectRotation[_obj];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(objectArray[_obj]);
                                    engineHandle.RemoveObject(objectX[_obj], objectY[_obj], objectType[_obj], objectRotation[_obj]);
                                }
                            }

                            objectCount = newCount;
                        }
                        else
                        {
                            int index = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int newSectionX = sectionX + packetData[off++];
                            int newSectionY = sectionY + packetData[off++];
                            int rotation = packetData[off++];
                            int newCount = 0;
                            for (int _obj = 0; _obj < objectCount; _obj += 1)
                            {
                                if (objectX[_obj] != newSectionX || objectY[_obj] != newSectionY || objectRotation[_obj] != rotation)
                                {
                                    if (_obj != newCount)
                                    {
                                        objectArray[newCount] = objectArray[_obj];
                                        objectArray[newCount].index = newCount;
                                        objectX[newCount] = objectX[_obj];
                                        objectY[newCount] = objectY[_obj];
                                        objectType[newCount] = objectType[_obj];
                                        objectRotation[newCount] = objectRotation[_obj];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(objectArray[_obj]);
                                    engineHandle.RemoveObject(objectX[_obj], objectY[_obj], objectType[_obj], objectRotation[_obj]);
                                }
                            }

                            objectCount = newCount;
                            if (index != 60000)
                            {
                                engineHandle.RegisterObjectDir(newSectionX, newSectionY, rotation);
                                int width;
                                int height;
                                if (rotation == 0 || rotation == 4)
                                {
                                    width = GameData.objectWidth[index];
                                    height = GameData.objectHeight[index];
                                }
                                else
                                {
                                    height = GameData.objectWidth[index];
                                    width = GameData.objectHeight[index];
                                }
                                int l40 = ((newSectionX + newSectionX + width) * gridSize) / 2;
                                int k42 = ((newSectionY + newSectionY + height) * gridSize) / 2;
                                int model = GameData.objectModelNumber[index];
                                GameObject gameObject = gameDataObjects[model].CreateParent();
#warning object not being added to camera.
                                gameCamera.AddModel(gameObject);

                                gameObject.index = objectCount;
                                gameObject.OffsetMiniPosition(0, rotation * 32, 0);
                                gameObject.OffsetPosition(l40, -engineHandle.GetAveragedElevation(l40, k42), k42);
                                gameObject.UpdateShading(true, 48, 48, -50, -10, -50);
                                engineHandle.CreateObject(newSectionX, newSectionY, index, rotation);
                                if (index == 74)
                                {
                                    gameObject.OffsetPosition(0, -480, 0);
                                }

                                objectX[objectCount] = newSectionX;
                                objectY[objectCount] = newSectionY;
                                objectType[objectCount] = index;
                                objectRotation[objectCount] = rotation;
                                objectArray[objectCount++] = gameObject;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 114)
                {
                    int off = 1;
                    inventoryItemsCount = packetData[off++] & 0xff;
                    for (int item = 0; item < inventoryItemsCount; item += 1)
                    {
                        int data = DataOperations.GetShort(packetData, off);
                        off += 2;
                        inventoryItems[item] = data & 0x7fff;
                        inventoryItemEquipped[item] = data / 32768;
                        if (GameData.itemStackable[data & 0x7fff] == 0)
                        {
                            inventoryItemCount[item] = DataOperations.GetInt(packetData, off);
                            off += 4;
                        }
                        else
                        {
                            inventoryItemCount[item] = 1;
                        }
                    }

                    return;
                }
                if (packetID == 53)
                {
                    int newMobCount = DataOperations.GetShort(packetData, 1);
                    int off = 3;
                    for (int current = 0; current < newMobCount; current += 1)
                    {
                        int index = DataOperations.GetShort(packetData, off);
                        off += 2;
                        if (index < 0 || index > playerBufferArray.Length)
                        {
                            return;
                        }

                        ClientMob mob = playerBufferArray[index];
                        if (mob is null)
                        {
                            return;
                        }

                        sbyte mobUpdateType = packetData[off];
                        off += 1;
                        if (mobUpdateType == 0)
                        {
                            int j30 = DataOperations.GetShort(packetData, off);
                            off += 2;

                            mob.playerSkullTimeout = 150;
                            mob.itemAboveHeadID = j30;

                        }
                        else if (mobUpdateType == 1)
                        {
                            sbyte byte7 = packetData[off];
                            off += 1;
                            string s3 = ChatMessage.BytesToString(packetData, off, byte7);
                            //if (useChatFilter)
                            //    s3 = ChatFilter.filterChat(s3);
                            bool ignore = false;
                            for (int i41 = 0; i41 < ignoresCount; i41 += 1)
                            {
                                if (ignoresList[i41] == mob.nameHash)
                                {
                                    ignore = true;
                                }
                            }

                            if (!ignore)
                            {
                                mob.lastMessageTimeout = 150;
                                mob.lastMessage = s3;
                                DisplayMessage(mob.username + ": " + mob.lastMessage, 2);
                            }
                            off += byte7;
                        }
                        else if (mobUpdateType == 2)
                        {
                            int lastDamageCount = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            int currentHits = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            int baseHits = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            mob.lastDamageCount = lastDamageCount;
                            mob.currentHits = currentHits;
                            mob.baseHits = baseHits;
                            mob.combatTimer = 200;
                            if (mob == ourPlayer)
                            {
                                playerStatCurrent[3] = currentHits;
                                playerStatBase[3] = baseHits;
                                showWelcomeBox = false;
                                showServerMessageBox = false;
                            }
                        }
                        else if (mobUpdateType == 3)
                        {
                            int l30 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int l34 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            mob.projectileType = l30;
                            mob.attackingNpcIndex = l34;
                            mob.attackingPlayerIndex = -1;
                            mob.projectileDistance = projectileRange;
                        }
                        else if (mobUpdateType == 4)
                        {
                            int i31 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int i35 = DataOperations.GetShort(packetData, off);
                            off += 2;
                            mob.projectileType = i31;
                            mob.attackingPlayerIndex = i35;
                            mob.attackingNpcIndex = -1;
                            mob.projectileDistance = projectileRange;
                        }
                        else if (mobUpdateType == 5)
                        {
                            mob.serverID = DataOperations.GetShort(packetData, off);
                            off += 2;
                            mob.nameHash = DataOperations.GetLong(packetData, off);
                            off += 8;
                            mob.username = DataOperations.HashToName(mob.nameHash);
                            int appearanceCount = DataOperations.GetByte(packetData[off]);
                            off += 1;
                            for (int j35 = 0; j35 < appearanceCount; j35 += 1)
                            {
                                mob.appearanceItems[j35] = DataOperations.GetByte(packetData[off]);
                                off += 1;
                            }

                            for (int j38 = appearanceCount; j38 < 12; j38 += 1)
                            {
                                mob.appearanceItems[j38] = 0;
                            }

                            mob.hairColour = packetData[off++] & 0xff;
                            mob.topColour = packetData[off++] & 0xff;
                            mob.bottomColour = packetData[off++] & 0xff;
                            mob.skinColour = packetData[off++] & 0xff;
                            mob.level = packetData[off++] & 0xff;
                            mob.playerSkulled = packetData[off++] & 0xff;
                            off += 1;// TODO to skip the admin flag (should it be removed)
                        }
                        else if (mobUpdateType == 6)
                        {
                            sbyte byte8 = packetData[off];
                            off += 1;
                            string s4 = ChatMessage.BytesToString(packetData, off, byte8);
                            mob.lastMessageTimeout = 150;
                            mob.lastMessage = s4;
                            if (mob == ourPlayer)
                            {
                                DisplayMessage(mob.username + ": " + mob.lastMessage, 5);
                            }

                            off += byte8;
                        }
                    }

                    return;
                }
                if (packetID == 95)
                {
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = sectionX + packetData[off + 1] >> 3;
                            int newSectionY = sectionY + packetData[off + 2] >> 3;
                            off += 3;
                            for (int current = 0; current < wallObjectCount; current += 1)
                            {
                                int newX = (wallObjectX[current] >> 3) - newSectionX;
                                int newY = (wallObjectY[current] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (current != newCount)
                                    {
                                        wallObjectArray[newCount] = wallObjectArray[current];
                                        wallObjectArray[newCount].index = newCount + 10000;
                                        wallObjectX[newCount] = wallObjectX[current];
                                        wallObjectY[newCount] = wallObjectY[current];
                                        wallObjectDirection[newCount] = wallObjectDirection[current];
                                        wallObjectID[newCount] = wallObjectID[current];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(wallObjectArray[current]);
                                    engineHandle.RemoveWallObject(wallObjectX[current], wallObjectY[current], wallObjectDirection[current], wallObjectID[current]);
                                }
                            }

                            wallObjectCount = newCount;
                        }
                        else
                        {
                            int newID = DataOperations.GetShort(packetData, off);
                            off += 2;
                            int newSectionX = sectionX + packetData[off++];
                            int newSectionY = sectionY + packetData[off++];
                            sbyte direction = packetData[off++];
                            int newCount = 0;
                            for (int current = 0; current < wallObjectCount; current += 1)
                            {
                                if (wallObjectX[current] != newSectionX || wallObjectY[current] != newSectionY || wallObjectDirection[current] != direction)
                                {
                                    if (current != newCount)
                                    {
                                        wallObjectArray[newCount] = wallObjectArray[current];
                                        wallObjectArray[newCount].index = newCount + 10000;
                                        wallObjectX[newCount] = wallObjectX[current];
                                        wallObjectY[newCount] = wallObjectY[current];
                                        wallObjectDirection[newCount] = wallObjectDirection[current];
                                        wallObjectID[newCount] = wallObjectID[current];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    gameCamera.RemoveModel(wallObjectArray[current]);
                                    engineHandle.RemoveWallObject(wallObjectX[current], wallObjectY[current], wallObjectDirection[current], wallObjectID[current]);
                                }
                            }

                            wallObjectCount = newCount;
                            if (newID != 60000)
                            {
                                engineHandle.CreateWall(newSectionX, newSectionY, direction, newID);
                                GameObject k35 = CreateWallObject(newSectionX, newSectionY, direction, newID, wallObjectCount);
                                wallObjectArray[wallObjectCount] = k35;
                                wallObjectX[wallObjectCount] = newSectionX;
                                wallObjectY[wallObjectCount] = newSectionY;
                                wallObjectID[wallObjectCount] = newID;
                                wallObjectDirection[wallObjectCount++] = direction;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 77)
                {
                    lastNpcCount = npcCount;
                    npcCount = 0;
                    for (int j2 = 0; j2 < lastNpcCount; j2 += 1)
                    {
                        lastNpcArray[j2] = npcArray[j2];
                    }

                    int off = 8;
                    int newCount = DataOperations.GetBits(packetData, off, 8);
                    off += 8;
                    for (int current = 0; current < newCount; current += 1)
                    {
                        ClientMob newNpc = GetLastNpc(DataOperations.GetBits(packetData, off, 16));
                        off += 16;
                        int needsUpdate = DataOperations.GetBits(packetData, off, 1);
                        off += 1;
                        if (needsUpdate != 0)
                        {
                            int j32 = DataOperations.GetBits(packetData, off, 1);
                            off += 1;
                            if (j32 == 0)
                            {
                                int nextSprite = DataOperations.GetBits(packetData, off, 3);
                                off += 3;
                                int waypointCurrent = newNpc.waypointCurrent;
                                int waypointX = newNpc.waypointsX[waypointCurrent];
                                int waypointY = newNpc.waypointsY[waypointCurrent];
                                if (nextSprite == 2 || nextSprite == 1 || nextSprite == 3)
                                {
                                    waypointX += gridSize;
                                }

                                if (nextSprite == 6 || nextSprite == 5 || nextSprite == 7)
                                {
                                    waypointX -= gridSize;
                                }

                                if (nextSprite == 4 || nextSprite == 3 || nextSprite == 5)
                                {
                                    waypointY += gridSize;
                                }

                                if (nextSprite == 0 || nextSprite == 1 || nextSprite == 7)
                                {
                                    waypointY -= gridSize;
                                }

                                newNpc.nextSprite = nextSprite;
                                newNpc.waypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                                newNpc.waypointsX[waypointCurrent] = waypointX;
                                newNpc.waypointsY[waypointCurrent] = waypointY;
                            }
                            else
                            {
                                int nextSprite = DataOperations.GetBits(packetData, off, 4);
                                off += 4;
                                if ((nextSprite & 0xc) == 12)
                                {
                                    continue;
                                }
                                newNpc.nextSprite = nextSprite;
                            }
                        }
                        npcArray[npcCount++] = newNpc;
                    }

                    while (off + 34 < packetLength * 8)
                    {
                        int mobIndex = DataOperations.GetBits(packetData, off, 16);
                        off += 16;
                        int areaMobX = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobX > 15)
                        {
                            areaMobX -= 32;
                        }

                        int areaMobY = DataOperations.GetBits(packetData, off, 5);
                        off += 5;
                        if (areaMobY > 15)
                        {
                            areaMobY -= 32;
                        }

                        int mobSprite = DataOperations.GetBits(packetData, off, 4);
                        off += 4;
                        int mobX = (sectionX + areaMobX) * gridSize + 64;
                        int mobY = (sectionY + areaMobY) * gridSize + 64;
                        int addIndex = DataOperations.GetBits(packetData, off, 10);
                        off += 10;
                        if (addIndex >= GameData.npcCount)
                        {
                            addIndex = 24;
                        }

                        CreateNpc(mobIndex, mobX, mobY, mobSprite, addIndex);
                    }
                    return;
                }
                if (packetID == 190)
                {
                    int newCount = DataOperations.GetShort(packetData, 1);
                    int off = 3;
                    for (int l16 = 0; l16 < newCount; l16 += 1)
                    {
                        int npcIndex = DataOperations.GetShort(packetData, off);
                        off += 2;
                        ClientMob mob = npcAttackingArray[npcIndex];
                        int updateType = DataOperations.GetByte(packetData[off]);
                        off += 1;
                        if (updateType == 1)
                        {
                            int playerIndex = DataOperations.GetShort(packetData, off);
                            off += 2;
                            sbyte messageLength = packetData[off];
                            off += 1;
                            if (mob is not null)
                            {
                                string s5 = ChatMessage.BytesToString(packetData, off, messageLength);
                                mob.lastMessageTimeout = 150;
                                mob.lastMessage = s5;
                                if (playerIndex == ourPlayer.serverIndex)
                                {
                                    DisplayMessage("@yel@" + GameData.npcName[mob.npcId] + ": " + mob.lastMessage, 5);
                                }
                            }
                            off += messageLength;
                        }
                        else
                            if (updateType == 2)
                            {
                                int lastDamageCount = DataOperations.GetByte(packetData[off]);
                                off += 1;
                                int currentHits = DataOperations.GetByte(packetData[off]);
                                off += 1;
                                int baseHits = DataOperations.GetByte(packetData[off]);
                                off += 1;
                                if (mob is not null)
                                {
                                    mob.lastDamageCount = lastDamageCount;
                                    mob.currentHits = currentHits;
                                    mob.baseHits = baseHits;
                                    mob.combatTimer = 200;
                                }
                            }
                    }

                    return;
                }
                if (packetID == 223)
                {
                    showQuestionMenu = true;
                    int count = DataOperations.GetByte(packetData[1]);
                    questionMenuCount = count;
                    int off = 2;
                    for (int index = 0; index < count; index += 1)
                    {
                        int optionLength = DataOperations.GetByte(packetData[off]);
                        off += 1;
                        questionMenuAnswer[index] = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), off, optionLength);
                        off += optionLength;
                    }

                    return;
                }
                if (packetID == 127)
                {
                    showQuestionMenu = false;
                    return;
                }
                if (packetID == 131)
                {
                    loadArea = true;
                    serverIndex = DataOperations.GetShort(packetData, 1);
                    wildX = DataOperations.GetShort(packetData, 3);
                    wildY = DataOperations.GetShort(packetData, 5);
                    layerIndex = DataOperations.GetShort(packetData, 7);
                    layerModifier = DataOperations.GetShort(packetData, 9);
                    wildY -= layerIndex * layerModifier;
                    needsClear = true;
                    hasWorldInfo = true;
                    return;
                }
                if (packetID == 180)
                {
                    int off = 1;
                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        playerStatCurrent[stat] = DataOperations.GetByte(packetData[off++]);
                    }

                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        playerStatBase[stat] = DataOperations.GetByte(packetData[off++]);
                    }

                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        playerStatExp[stat] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }
                    return;
                }
                if (packetID == 177)
                {
                    int off = 1;
                    for (int j3 = 0; j3 < 5; j3 += 1)
                    {
                        equipmentStatus[j3] = DataOperations.GetSignedShort(packetData, off);
                        off += 2;
                    }
                    return;
                }
                if (packetID == 165)
                {
                    playerAliveTimeout = 250;
                    return;
                }
                if (packetID == 115)
                {
                    int k3 = (packetLength - 1) / 4;
                    for (int i11 = 0; i11 < k3; i11 += 1)
                    {
                        int k17 = sectionX + DataOperations.GetSignedShort(packetData, 1 + i11 * 4) >> 3;
                        int i22 = sectionY + DataOperations.GetSignedShort(packetData, 3 + i11 * 4) >> 3;
                        int j25 = 0;
                        for (int l28 = 0; l28 < groundItemCount; l28 += 1)
                        {
                            int j33 = (groundItemX[l28] >> 3) - k17;
                            int l36 = (groundItemY[l28] >> 3) - i22;
                            if (j33 != 0 || l36 != 0)
                            {
                                if (l28 != j25)
                                {
                                    groundItemX[j25] = groundItemX[l28];
                                    groundItemY[j25] = groundItemY[l28];
                                    groundItemID[j25] = groundItemID[l28];
                                    groundItemObjectVar[j25] = groundItemObjectVar[l28];
                                }
                                j25 += 1;
                            }
                        }

                        groundItemCount = j25;
                        j25 = 0;
                        for (int k33 = 0; k33 < objectCount; k33 += 1)
                        {
                            int i37 = (objectX[k33] >> 3) - k17;
                            int j39 = (objectY[k33] >> 3) - i22;
                            if (i37 != 0 || j39 != 0)
                            {
                                if (k33 != j25)
                                {
                                    objectArray[j25] = objectArray[k33];
                                    objectArray[j25].index = j25;
                                    objectX[j25] = objectX[k33];
                                    objectY[j25] = objectY[k33];
                                    objectType[j25] = objectType[k33];
                                    objectRotation[j25] = objectRotation[k33];
                                }
                                j25 += 1;
                            }
                            else
                            {
                                gameCamera.RemoveModel(objectArray[k33]);
                                engineHandle.RemoveObject(objectX[k33], objectY[k33], objectType[k33], objectRotation[k33]);
                            }
                        }

                        objectCount = j25;
                        j25 = 0;
                        for (int j37 = 0; j37 < wallObjectCount; j37 += 1)
                        {
                            int k39 = (wallObjectX[j37] >> 3) - k17;
                            int l41 = (wallObjectY[j37] >> 3) - i22;
                            if (k39 != 0 || l41 != 0)
                            {
                                if (j37 != j25)
                                {
                                    wallObjectArray[j25] = wallObjectArray[j37];
                                    wallObjectArray[j25].index = j25 + 10000;
                                    wallObjectX[j25] = wallObjectX[j37];
                                    wallObjectY[j25] = wallObjectY[j37];
                                    wallObjectDirection[j25] = wallObjectDirection[j37];
                                    wallObjectID[j25] = wallObjectID[j37];
                                }
                                j25 += 1;
                            }
                            else
                            {
                                gameCamera.RemoveModel(wallObjectArray[j37]);
                                engineHandle.RemoveWallObject(wallObjectX[j37], wallObjectY[j37], wallObjectDirection[j37], wallObjectID[j37]);
                            }
                        }

                        wallObjectCount = j25;
                    }

                    return;
                }
                if (packetID == 207)
                {
                    showAppearanceWindow = true;
                    return;
                }
                if (packetID == 4)
                {
                    int tradeOther = DataOperations.GetShort(packetData, 1);
                    if (playerBufferArray[tradeOther] is not null)
                    {
                        tradeOtherName = playerBufferArray[tradeOther].username;
                    }

                    showTradeBox = true;
                    tradeOtherAccepted = false;
                    tradeWeAccepted = false;
                    tradeItemsOurCount = 0;
                    tradeItemsOtherCount = 0;
                    return;
                }
                if (packetID == 187)
                {
                    showTradeBox = false;
                    showTradeConfirmBox = false;
                    return;
                }
                if (packetID == 250)
                {
                    tradeItemsOtherCount = packetData[1] & 0xff;
                    int i4 = 2;
                    for (int j11 = 0; j11 < tradeItemsOtherCount; j11 += 1)
                    {
                        tradeItemsOther[j11] = DataOperations.GetShort(packetData, i4);
                        i4 += 2;
                        tradeItemOtherCount[j11] = DataOperations.GetInt(packetData, i4);
                        i4 += 4;
                    }

                    tradeOtherAccepted = false;
                    tradeWeAccepted = false;
                    return;
                }
                if (packetID == 92)
                {
                    sbyte byte0 = packetData[1];
                    if (byte0 == 1)
                    {
                        tradeOtherAccepted = true;
                        return;
                    }
                    else
                    {
                        tradeOtherAccepted = false;
                        return;
                    }
                }
                if (packetID == 253)
                {
                    showShopBox = true;
                    int off = 1;
                    int newShopItemCount = packetData[off++] & 0xff;
                    sbyte isGeneralShop = packetData[off++];
                    shopItemSellPriceModifier = packetData[off++] & 0xff;
                    shopItemBuyPriceModifier = packetData[off++] & 0xff;
                    for (int j22 = 0; j22 < 40; j22 += 1)
                    {
                        shopItems[j22] = -1;
                    }

                    for (int item = 0; item < newShopItemCount; item += 1)
                    {
                        shopItems[item] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        shopItemCount[item] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        shopItemBuyPrice[item] = DataOperations.GetInt(packetData, off);
                        off += 4;
                        shopItemSellPrice[item] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    if (isGeneralShop == 1)
                    {
                        int i29 = 39;
                        for (int l33 = 0; l33 < inventoryItemsCount; l33 += 1)
                        {
                            if (i29 < newShopItemCount)
                            {
                                break;
                            }

                            bool flag2 = false;
                            for (int l39 = 0; l39 < 40; l39 += 1)
                            {
                                if (shopItems[l39] != inventoryItems[l33])
                                {
                                    continue;
                                }

                                flag2 = true;
                                break;
                            }

                            if (inventoryItems[l33] == 10)
                            {
                                flag2 = true;
                            }

                            if (!flag2)
                            {
                                shopItems[i29] = inventoryItems[l33] & 0x7fff;
                                shopItemCount[i29] = 0;
                                shopItemSellPrice[i29] = GameData.itemBasePrice[shopItems[i29]] - (int)(GameData.itemBasePrice[shopItems[i29]] / 2.5);
                                shopItemSellPrice[i29] -= (int)(shopItemSellPrice[i29] * 0.10);
                                i29 -= 1;
                            }
                        }

                    }
                    if (selectedShopItemIndex >= 0 && selectedShopItemIndex < 40 && shopItems[selectedShopItemIndex] != selectedShopItemType)
                    {
                        selectedShopItemIndex = -1;
                        selectedShopItemType = -2;
                    }
                    return;
                }
                if (packetID == 220)
                {
                    showShopBox = false;
                    return;
                }
                if (packetID == 18)
                {
                    sbyte byte1 = packetData[1];
                    if (byte1 == 1)
                    {
                        tradeWeAccepted = true;
                        return;
                    }
                    else
                    {
                        tradeWeAccepted = false;
                        return;
                    }
                }
                if (packetID == 152)
                {
                    configCameraAutoAngle = DataOperations.GetByte(packetData[1]) == 1;
                    configOneMouseButton = DataOperations.GetByte(packetData[2]) == 1;
                    configSoundOff = DataOperations.GetByte(packetData[3]) == 1;
                    showRoofs = DataOperations.GetByte(packetData[4]) == 1;
                    autoScreenshot = DataOperations.GetByte(packetData[5]) == 1;
                    showCombatWindow = DataOperations.GetByte(packetData[6]) == 1;
                    return;
                }
                if (packetID == 209)
                {
                    for (int k4 = 0; k4 < packetLength - 1; k4 += 1)
                    {
                        bool flag = packetData[k4 + 1] == 1;
                        if (!prayerOn[k4] && flag)
                        {
                            PlaySound("prayeron");
                        }

                        if (prayerOn[k4] && !flag)
                        {
                            PlaySound("prayeroff");
                        }

                        prayerOn[k4] = flag;
                    }

                    return;
                }
                if (packetID == 93)
                {
                    showBankBox = true;
                    int off = 1;
                    serverBankItemsCount = packetData[off++] & 0xff;
                    maxBankItems = packetData[off++] & 0xff;
                    for (int l11 = 0; l11 < serverBankItemsCount; l11 += 1)
                    {
                        serverBankItems[l11] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        serverBankItemCount[l11] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    UpdateBankItems();
                    return;
                }
                if (packetID == 171)
                {
                    showBankBox = false;
                    return;
                }
                if (packetID == 211)
                {
                    int j5 = packetData[1] & 0xff;
                    playerStatExp[j5] = DataOperations.GetInt(packetData, 2);
                    return;
                }
                if (packetID == 229)
                {
                    int k5 = DataOperations.GetShort(packetData, 1);
                    if (playerBufferArray[k5] is not null)
                    {
                        duelOpponent = playerBufferArray[k5].username;
                    }

                    showDuelBox = true;
                    duelMyItemCount = 0;
                    duelOpponentItemCount = 0;
                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    duelNoRetreating = false;
                    duelNoMagic = false;
                    duelNoPrayer = false;
                    duelNoWeapons = false;
                    return;
                }
                if (packetID == 160)
                {
                    showDuelBox = false;
                    showDuelConfirmBox = false;
                    return;
                }

#warning have not fixed the following yet....
                if (packetID == 251)
                {
                    showTradeConfirmBox = true;
                    tradeConfirmAccepted = false;
                    showTradeBox = false;
                    int off = 1;
                    tradeConfirmOtherNameLong = DataOperations.GetLong(packetData, off);
                    off += 8;
                    tradeConfirmOtherItemCount = packetData[off++] & 0xff;
                    for (int i12 = 0; i12 < tradeConfirmOtherItemCount; i12 += 1)
                    {
                        tradeConfirmOtherItems[i12] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        tradeConfirmOtherItemsCount[i12] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    tradeConfigItemCount = packetData[off++] & 0xff;
                    for (int l17 = 0; l17 < tradeConfigItemCount; l17 += 1)
                    {
                        tradeConfirmItems[l17] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        tradeConfigItemsCount[l17] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    return;
                }
                if (packetID == 63)
                {
                    duelOpponentItemCount = packetData[1] & 0xff;
                    int off = 2;
                    for (int j12 = 0; j12 < duelOpponentItemCount; j12 += 1)
                    {
                        duelOpponentItems[j12] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        duelOpponentItemsCount[j12] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    return;
                }
                if (packetID == 198)
                {
                    if (packetData[1] == 1)
                    {
                        duelNoRetreating = true;
                    }
                    else
                    {
                        duelNoRetreating = false;
                    }

                    if (packetData[2] == 1)
                    {
                        duelNoMagic = true;
                    }
                    else
                    {
                        duelNoMagic = false;
                    }

                    if (packetData[3] == 1)
                    {
                        duelNoPrayer = true;
                    }
                    else
                    {
                        duelNoPrayer = false;
                    }

                    if (packetData[4] == 1)
                    {
                        duelNoWeapons = true;
                    }
                    else
                    {
                        duelNoWeapons = false;
                    }

                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    return;
                }
                if (packetID == 139)
                {
                    int off = 1;
                    int itemSlot = packetData[off++] & 0xff;
                    int itemID = DataOperations.GetShort(packetData, off);
                    off += 2;
                    int itemCount = DataOperations.GetInt(packetData, off);
                    off += 4;
                    if (itemCount == 0)
                    {
                        serverBankItemsCount -= 1;
                        for (int l25 = itemSlot; l25 < serverBankItemsCount; l25 += 1)
                        {
                            serverBankItems[l25] = serverBankItems[l25 + 1];
                            serverBankItemCount[l25] = serverBankItemCount[l25 + 1];
                        }

                    }
                    else
                    {
                        serverBankItems[itemSlot] = itemID;
                        serverBankItemCount[itemSlot] = itemCount;
                        if (itemSlot >= serverBankItemsCount)
                        {
                            serverBankItemsCount = itemSlot + 1;
                        }
                    }
                    UpdateBankItems();
                    return;
                }
                if (packetID == 228)
                {
                    int off = 1;
                    int count = 1;
                    int newCount = packetData[off++] & 0xff;
                    int data = DataOperations.GetShort(packetData, off);
                    off += 2;
                    if (GameData.itemStackable[data & 0x7fff] == 0)
                    {
                        count = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }
                    inventoryItems[newCount] = data & 0x7fff;
                    inventoryItemEquipped[newCount] = data / 32768;
                    inventoryItemCount[newCount] = count;
                    if (newCount >= inventoryItemsCount)
                    {
                        inventoryItemsCount = newCount + 1;
                    }

                    return;
                }
                if (packetID == 191)
                {
                    int l6 = packetData[1] & 0xff;
                    inventoryItemsCount -= 1;
                    for (int i13 = l6; i13 < inventoryItemsCount; i13 += 1)
                    {
                        inventoryItems[i13] = inventoryItems[i13 + 1];
                        inventoryItemCount[i13] = inventoryItemCount[i13 + 1];
                        inventoryItemEquipped[i13] = inventoryItemEquipped[i13 + 1];
                    }

                    return;
                }
                if (packetID == 208)
                {
                    int off = 1;
                    int stat = packetData[off++] & 0xff;
                    playerStatCurrent[stat] = DataOperations.GetByte(packetData[off++]);
                    playerStatBase[stat] = DataOperations.GetByte(packetData[off++]);
                    playerStatExp[stat] = DataOperations.GetInt(packetData, off);
                    off += 4;
                    return;
                }
                if (packetID == 65)
                {
                    sbyte byte2 = packetData[1];
                    if (byte2 == 1)
                    {
                        duelOpponentAccepted = true;
                        return;
                    }
                    else
                    {
                        duelOpponentAccepted = false;
                        return;
                    }
                }
                if (packetID == 197)
                {
                    sbyte byte3 = packetData[1];
                    if (byte3 == 1)
                    {
                        duelMyAccepted = true;
                        return;
                    }
                    else
                    {
                        duelMyAccepted = false;
                        return;
                    }
                }
                if (packetID == 147)
                {
                    showDuelConfirmBox = true;
                    duelConfirmOurAccepted = false;
                    showDuelBox = false;
                    int off = 1;
                    duelOpponentHash = DataOperations.GetLong(packetData, off);
                    off += 8;
                    duelOpponentStakeCount = packetData[off++] & 0xff;
                    for (int k13 = 0; k13 < duelOpponentStakeCount; k13 += 1)
                    {
                        duelOpponentStakeItem[k13] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        duelOutStakeItemCount[k13] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    duelOurStakeCount = packetData[off++] & 0xff;
                    for (int k18 = 0; k18 < duelOurStakeCount; k18 += 1)
                    {
                        duelOurStakeItem[k18] = DataOperations.GetShort(packetData, off);
                        off += 2;
                        duelOurStakeItemCount[k18] = DataOperations.GetInt(packetData, off);
                        off += 4;
                    }

                    duelRetreat = packetData[off++] & 0xff;
                    duelMagic = packetData[off++] & 0xff;
                    duelPrayer = packetData[off++] & 0xff;
                    duelWeapons = packetData[off++] & 0xff;
                    return;
                }
                if (packetID == 11)
                {
                    string s1 = new(packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    PlaySound(s1);
                    return;
                }
                if (packetID == 23)
                {
                    if (teleBubbleCount < 50)
                    {
                        int type = packetData[1] & 0xff;
                        int x = packetData[2] + sectionX;
                        int y = packetData[3] + sectionY;
                        teleBubbleType[teleBubbleCount] = type;
                        teleBubbleTime[teleBubbleCount] = 0;
                        teleBubbleX[teleBubbleCount] = x;
                        teleBubbleY[teleBubbleCount] = y;
                        teleBubbleCount += 1;
                    }
                    return;
                }
                if (packetID == 248)
                {
                    if (!loginScreenShown)
                    {
                        lastLoginDays = DataOperations.GetShort(packetData, 1);
                        subDaysLeft = DataOperations.GetShort(packetData, 3);
                        lastLoginAddress = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), 5, packetLength - 5);
                        showWelcomeBox = true;
                        loginScreenShown = true;
                    }
                    return;
                }
                if (packetID == 148)
                {
                    serverMessage = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    showServerMessageBox = true;
                    serverMessageBoxTop = false;
                    return;
                }
                if (packetID == 64)
                {
                    serverMessage = new string(packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    showServerMessageBox = true;
                    serverMessageBoxTop = true;
                    return;
                }
                if (packetID == 126)
                {
                    fatigue = DataOperations.GetShort(packetData, 1);
                    return;
                }
                if (packetID == 206)
                {
                    killingSpree = DataOperations.GetShort(packetData, 1);
                    return;
                }
                if (packetID == 224)
                {
                    isSleeping = false;
                    return;
                }
                if (packetID == 225)
                {
                    sleepingStatusText = "Incorrect - Please wait...";
                    return;
                }
                if (packetID == 172)
                {
                    systemUpdate = DataOperations.GetShort(packetData, 1) * 32;
                    return;
                }
                if (packetID == 181)
                {
                    if (autoScreenshot)
                    {
                        TakeScreenshot(false);
                    }

                    return;
                }
                if (packetID == 182)
                {
                    int off = 1;
                    questPoints = DataOperations.GetShort(packetData, off);
                    off += 2;
                    for (int l4 = 0; l4 < questName.Length; l4 += 1)
                    {
                        questStage[l4] = packetData[l4 + 1];
                    }

                    return;
                }
                if (packetID == 233)
                {
                    questPoints = DataOperations.GetByte(packetData[1]);
                    int count = DataOperations.GetByte(packetData[2]);
                    int off = 3;
                    string[] newQuestNames = new string[count];
                    int[] newQuestStage = new int[count];
                    for (int i = 0; i < count; i += 1)
                    {
                        newQuestNames[i] = questName[DataOperations.GetByte(packetData[off++])];
                        newQuestStage[i] = DataOperations.GetByte(packetData[off++]);
                    }
                    usedQuestName = newQuestNames;
                    questStage = newQuestStage;
                    return;
                }
                if (packetID == 129)
                {
                    combatStyle = DataOperations.GetByte(packetData[1]);
                    return;
                }
                if (packetID == 110)
                {// TODO remove?
                    Console.WriteLine("RECEIVED PACKET 110 (SERVER INFO)");
                    return;
                }
                // Spell counts and quest progress — 2-byte value packets, silently accepted
                if (packetID == 210 || packetID == 212 || packetID == 213) { return; } // Guthix/Zamorak/Saradomin spells
                if (packetID == 128) { return; } // QuestPointsChange
                if (packetID == 134) { return; } // Deaths
                if (packetID == 137) { return; } // DruidicRitual
                if (packetID == 138) { return; } // ImpCatcher
                if (packetID == 141) { return; } // SheepShearer
                if (packetID == 143) { return; } // DoricQuest
                if (packetID == 132) { return; } // Kills
                if (packetID == 140) { return; } // RomeoAndJuliet
                if (packetID == 142) { return; } // WitchPotion
                if (packetID == 144) { return; } // CookAssistant
                if (packetID == 133) { return; } // PirateTreasure
                if (packetID == 139) { return; } // BlackKnightsForte
                if (packetID == 146) { return; } // RestlessGhost
                if (packetID == 149) { return; } // ErnestTheChicken
                if (packetID == 150) { return; } // Goblintown
                if (packetID == 151) { return; } // MinersBlazonQuest
                if (packetID == 173) { return; } // VampireSlayer
                if (packetID == 174) { return; } // MilleniumItemQuest
                if (packetID == 175) { return; } // KnightsSword
                if (packetID == 202) { return; } // GoblinDiplomacy
                if (packetID == 203) { return; } // TheGrandTree
                if (packetID == 204) { return; } // FightArena
                if (packetID == 205) { return; } // Hazeel
                if (packetID == 214) { return; } // HolyGrail
                if (packetID == 215) { return; } // MerlinsCrystal
                if (packetID == 216) { return; } // LostCity
                if (packetID == 217) { return; } // WitchHouse
                Console.WriteLine("UNHANDLED PACKET:" + packetID + " LEN:" + packetLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // ex.printStackTrace();
            }
        }
    }

}
