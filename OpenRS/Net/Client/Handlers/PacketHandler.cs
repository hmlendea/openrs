using System;
using System.Linq;

using NuciLog.Core;

using OpenRS.Localisation;
using OpenRS.Logging;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Handlers
{
    public sealed class PacketHandler(GameClient client)
    {
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<PacketHandler>();

        public void HandlePacket(int packetID, int packetLength, sbyte[] packetData)
        {
            try
            {
                //base.handlePacket(packetID, packetLength, packetData);
                if (packetID == 145)
                {
                    if (!client.hasWorldInfo)
                    {
                        return;
                    }

                    client.lastPlayerCount = client.playerCount;
                    for (int l = 0; l < client.lastPlayerCount; l += 1)
                    {
                        client.lastPlayerArray[l] = client.playerArray[l];
                    }

                    int off = 8;
                    client.sectionX = DataOperations.GetBits(client.packetData, off, 11);
                    off += 11;
                    client.sectionY = DataOperations.GetBits(client.packetData, off, 13);
                    off += 13;
                    int sprite = DataOperations.GetBits(client.packetData, off, 4);
                    off += 4;
                    bool sectionLoaded = client.LoadSection(client.sectionX, client.sectionY);
                    client.sectionX -= client.areaX;
                    client.sectionY -= client.areaY;
                    int mapEnterX = client.sectionX * client.gridSize + 64;
                    int mapEnterY = client.sectionY * client.gridSize + 64;
                    if (sectionLoaded)
                    {
                        client.ourPlayer.waypointCurrent = 0;
                        client.ourPlayer.waypointsEndSprite = 0;
                        client.ourPlayer.currentX = client.ourPlayer.waypointsX[0] = mapEnterX;
                        client.ourPlayer.currentY = client.ourPlayer.waypointsY[0] = mapEnterY;
                    }
                    client.playerCount = 0;
                    client.ourPlayer = client.CreatePlayer(client.serverIndex, mapEnterX, mapEnterY, sprite);
                    int newPlayerCount = DataOperations.GetBits(client.packetData, off, 8);
                    off += 8;
                    for (int currentNewPlayer = 0; currentNewPlayer < newPlayerCount; currentNewPlayer += 1)
                    {
                        //ClientMob mob = lastPlayerArray[currentNewPlayer + 1];
                        ClientMob mob = client.GetLastPlayer(DataOperations.GetBits(client.packetData, off, 16));
                        off += 16;
                        int playerAtTile = DataOperations.GetBits(client.packetData, off, 1);
                        off += 1;
                        if (playerAtTile != 0)
                        {
                            int waypointsLeft = DataOperations.GetBits(client.packetData, off, 1);
                            off += 1;
                            if (waypointsLeft == 0)
                            {
                                int currentNextSprite = DataOperations.GetBits(client.packetData, off, 3);
                                off += 3;
                                if (mob is not null)
                                {
                                    int currentWaypoint = mob.waypointCurrent;
                                    int newWaypointX = mob.waypointsX[currentWaypoint];
                                    int newWaypointY = mob.waypointsY[currentWaypoint];
                                    if (currentNextSprite == 2 || currentNextSprite == 1 || currentNextSprite == 3)
                                    {
                                        newWaypointX += client.gridSize;
                                    }

                                    if (currentNextSprite == 6 || currentNextSprite == 5 || currentNextSprite == 7)
                                    {
                                        newWaypointX -= client.gridSize;
                                    }

                                    if (currentNextSprite == 4 || currentNextSprite == 3 || currentNextSprite == 5)
                                    {
                                        newWaypointY += client.gridSize;
                                    }

                                    if (currentNextSprite == 0 || currentNextSprite == 1 || currentNextSprite == 7)
                                    {
                                        newWaypointY -= client.gridSize;
                                    }

                                    mob.nextSprite = currentNextSprite;
                                    mob.waypointCurrent = currentWaypoint = (currentWaypoint + 1) % 10;
                                    mob.waypointsX[currentWaypoint] = newWaypointX;
                                    mob.waypointsY[currentWaypoint] = newWaypointY;
                                }
                            }
                            else
                            {
                                int needsNextSprite = DataOperations.GetBits(client.packetData, off, 4);
                                off += 4;
                                if ((needsNextSprite & 0xc) == 12)
                                {
                                    continue;
                                }
                                mob?.nextSprite = needsNextSprite;
                            }
                        }
                        if (mob is not null)
                        {
                            client.playerArray[client.playerCount++] = mob;
                        }
                    }

                    int mobCount = 0;
                    while (off + 24 < packetLength * 8)
                    {
                        int mobIndex = DataOperations.GetBits(client.packetData, off, 16);
                        off += 16;
                        int areaMobX = DataOperations.GetBits(client.packetData, off, 5);
                        off += 5;
                        if (areaMobX > 15)
                        {
                            areaMobX -= 32;
                        }

                        int areaMobY = DataOperations.GetBits(client.packetData, off, 5);
                        off += 5;
                        if (areaMobY > 15)
                        {
                            areaMobY -= 32;
                        }

                        int mobSprite = DataOperations.GetBits(client.packetData, off, 4);
                        off += 4;
                        int addIndex = DataOperations.GetBits(client.packetData, off, 1);
                        off += 1;
                        int mobX = (client.sectionX + areaMobX) * client.gridSize + 64;
                        int mobY = (client.sectionY + areaMobY) * client.gridSize + 64;
                        client.CreatePlayer(mobIndex, mobX, mobY, mobSprite);
                        if (addIndex == 0)
                        {
                            client.playerBufferArrayIndexes[mobCount++] = mobIndex;
                        }
                    }
                    if (mobCount > 0)
                    {
                        client.streamClass.CreatePacket(83);
                        client.streamClass.AddShort(mobCount);
                        for (int k40 = 0; k40 < mobCount; k40 += 1)
                        {
                            ClientMob f5 = client.playerBufferArray[client.playerBufferArrayIndexes[k40]];
                            client.streamClass.AddShort(f5.serverIndex);
                            client.streamClass.AddShort(f5.serverID);
                        }

                        client.streamClass.FormatPacket();
                        mobCount = 0;
                    }
                    return;
                }
                if (packetID == 109)
                {
                    if (client.needsClear)
                    {
                        for (int i = 0; i < client.groundItemID.Length; i += 1)
                        {
                            client.groundItemX[i] = -1;
                            client.groundItemY[i] = -1;
                            client.groundItemID[i] = -1;
                            client.groundItemObjectVar[i] = -1;
                        }
                        client.groundItemCount = 0;
                        client.needsClear = false;
                    }
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(client.packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = client.sectionX + client.packetData[off + 1] >> 3;
                            int newSectionY = client.sectionY + client.packetData[off + 2] >> 3;
                            off += 3;
                            for (int groundItem = 0; groundItem < client.groundItemCount; groundItem += 1)
                            {
                                int newX = (client.groundItemX[groundItem] >> 3) - newSectionX;
                                int newY = (client.groundItemY[groundItem] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (groundItem != newCount)
                                    {
                                        client.groundItemX[newCount] = client.groundItemX[groundItem];
                                        client.groundItemY[newCount] = client.groundItemY[groundItem];
                                        client.groundItemID[newCount] = client.groundItemID[groundItem];
                                        client.groundItemObjectVar[newCount] = client.groundItemObjectVar[groundItem];
                                    }
                                    newCount += 1;
                                }
                            }

                            client.groundItemCount = newCount;
                        }
                        else
                        {
                            int newID = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            int newX = client.sectionX + client.packetData[off++];
                            int newY = client.sectionY + client.packetData[off++];
                            if ((newID & 0x8000) == 0)
                            {
                                client.groundItemX[client.groundItemCount] = newX;
                                client.groundItemY[client.groundItemCount] = newY;
                                client.groundItemID[client.groundItemCount] = newID;
                                client.groundItemObjectVar[client.groundItemCount] = 0;
                                for (int l23 = 0; l23 < client.objectCount; l23 += 1)
                                {
                                    if (client.objectX[l23] != newX || client.objectY[l23] != newY)
                                    {
                                        continue;
                                    }

                                    client.groundItemObjectVar[client.groundItemCount] = client.entityManager.GetWorldObject(client.objectType[l23]).GroundItemVar;
                                    break;
                                }

                                client.groundItemCount += 1;
                            }
                            else
                            {
                                newID &= 0x7fff;
                                int updateIndex = 0;
                                for (int currentItemIndex = 0; currentItemIndex < client.groundItemCount; currentItemIndex += 1)
                                {
                                    if (client.groundItemX[currentItemIndex] != newX || client.groundItemY[currentItemIndex] != newY || client.groundItemID[currentItemIndex] != newID)
                                    {
                                        if (currentItemIndex != updateIndex)
                                        {
                                            client.groundItemX[updateIndex] = client.groundItemX[currentItemIndex];
                                            client.groundItemY[updateIndex] = client.groundItemY[currentItemIndex];
                                            client.groundItemID[updateIndex] = client.groundItemID[currentItemIndex];
                                            client.groundItemObjectVar[updateIndex] = client.groundItemObjectVar[currentItemIndex];
                                        }
                                        updateIndex += 1;
                                    }
                                    else
                                    {
                                        newID = -123;
                                    }
                                }

                                client.groundItemCount = updateIndex;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 27)
                {
                    for (int off = 1; off < packetLength; )
                    {
                        if (DataOperations.GetByte(client.packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = client.sectionX + client.packetData[off + 1] >> 3;
                            int newSectionY = client.sectionY + client.packetData[off + 2] >> 3;
                            off += 3;
                            for (int _obj = 0; _obj < client.objectCount; _obj += 1)
                            {
                                int newX = (client.objectX[_obj] >> 3) - newSectionX;
                                int newY = (client.objectY[_obj] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (_obj != newCount)
                                    {
                                        client.objectArray[newCount] = client.objectArray[_obj];
                                        client.objectArray[newCount].index = newCount;
                                        client.objectX[newCount] = client.objectX[_obj];
                                        client.objectY[newCount] = client.objectY[_obj];
                                        client.objectType[newCount] = client.objectType[_obj];
                                        client.objectRotation[newCount] = client.objectRotation[_obj];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    client.gameCamera.RemoveModel(client.objectArray[_obj]);
                                    client.engineHandle.RemoveObject(client.objectX[_obj], client.objectY[_obj], client.objectType[_obj], client.objectRotation[_obj]);
                                }
                            }

                            client.objectCount = newCount;
                        }
                        else
                        {
                            int index = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            int newSectionX = client.sectionX + client.packetData[off++];
                            int newSectionY = client.sectionY + client.packetData[off++];
                            int rotation = client.packetData[off++];
                            int newCount = 0;
                            for (int _obj = 0; _obj < client.objectCount; _obj += 1)
                            {
                                if (client.objectX[_obj] != newSectionX || client.objectY[_obj] != newSectionY || client.objectRotation[_obj] != rotation)
                                {
                                    if (_obj != newCount)
                                    {
                                        client.objectArray[newCount] = client.objectArray[_obj];
                                        client.objectArray[newCount].index = newCount;
                                        client.objectX[newCount] = client.objectX[_obj];
                                        client.objectY[newCount] = client.objectY[_obj];
                                        client.objectType[newCount] = client.objectType[_obj];
                                        client.objectRotation[newCount] = client.objectRotation[_obj];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    client.gameCamera.RemoveModel(client.objectArray[_obj]);
                                    client.engineHandle.RemoveObject(client.objectX[_obj], client.objectY[_obj], client.objectType[_obj], client.objectRotation[_obj]);
                                }
                            }

                            client.objectCount = newCount;
                            if (index != 60000)
                            {
                                if (index >= client.entityManager.WorldObjectCount)
                                {
                                    int worldObjectCount = client.entityManager.WorldObjectCount;
                                    logger.Warn(
                                        GameOperation.HandlePacket,
                                        "Skipping unknown object.",
                                        new LogInfo(GameLogInfoKey.ObjectIndex, index),
                                        new LogInfo(GameLogInfoKey.ObjectCount, worldObjectCount));
                                }
                                else
                                {
                                    client.engineHandle.RegisterObjectDir(newSectionX, newSectionY, rotation);
                                    int width;
                                    int height;
                                    if (rotation == 0 || rotation == 4)
                                    {
                                        width = client.entityManager.GetWorldObject(index).Width;
                                        height = client.entityManager.GetWorldObject(index).Height;
                                    }
                                    else
                                    {
                                        height = client.entityManager.GetWorldObject(index).Width;
                                        width = client.entityManager.GetWorldObject(index).Height;
                                    }
                                    int l40 = (newSectionX + newSectionX + width) * client.gridSize / 2;
                                    int k42 = (newSectionY + newSectionY + height) * client.gridSize / 2;
                                    int model = client.entityManager.GetWorldObject(index).ModelId;
                                    GameObject gameObject = client.gameDataObjects[model].CreateParent();
    #warning object not being added to camera.
                                    client.gameCamera.AddModel(gameObject);

                                    gameObject.index = client.objectCount;
                                    gameObject.OffsetMiniPosition(0, rotation * 32, 0);
                                    gameObject.OffsetPosition(l40, -client.engineHandle.GetAveragedElevation(l40, k42), k42);
                                    gameObject.UpdateShading(true, 48, 48, -50, -10, -50);
                                    client.engineHandle.CreateObject(newSectionX, newSectionY, index, rotation);
                                    if (index == 74)
                                    {
                                        gameObject.OffsetPosition(0, -480, 0);
                                    }

                                    client.objectX[client.objectCount] = newSectionX;
                                    client.objectY[client.objectCount] = newSectionY;
                                    client.objectType[client.objectCount] = index;
                                    client.objectRotation[client.objectCount] = rotation;
                                    client.objectArray[client.objectCount++] = gameObject;
                                }
                            }
                        }
                    }

                    return;
                }
                if (packetID == 114)
                {
                    int off = 1;
                    client.inventoryItemsCount = client.packetData[off++] & 0xff;
                    for (int item = 0; item < client.inventoryItemsCount; item += 1)
                    {
                        int data = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.inventoryItems[item] = data & 0x7fff;
                        client.inventoryItemEquipped[item] = data / 32768;
                        if (!client.entityManager.GetItem(data & 0x7fff).IsStackable)
                        {
                            client.inventoryItemCount[item] = DataOperations.GetInt(client.packetData, off);
                            off += 4;
                        }
                        else
                        {
                            client.inventoryItemCount[item] = 1;
                        }
                    }

                    return;
                }
                if (packetID == 53)
                {
                    int newMobCount = DataOperations.GetShort(client.packetData, 1);
                    int off = 3;
                    for (int current = 0; current < newMobCount; current += 1)
                    {
                        int index = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        if (index < 0 || index > client.playerBufferArray.Length)
                        {
                            return;
                        }

                        ClientMob mob = client.playerBufferArray[index];
                        if (mob is null)
                        {
                            return;
                        }

                        sbyte mobUpdateType = client.packetData[off];
                        off += 1;
                        if (mobUpdateType == 0)
                        {
                            int j30 = DataOperations.GetShort(client.packetData, off);
                            off += 2;

                            mob.playerSkullTimeout = 150;
                            mob.itemAboveHeadID = j30;

                        }
                        else if (mobUpdateType == 1)
                        {
                            sbyte byte7 = client.packetData[off];
                            off += 1;
                            string s3 = ChatMessage.BytesToString(client.packetData, off, byte7);

                            if (client.useChatFilter)
                            {
                                s3 = ChatFilter.FilterChat(s3);
                            }

                            bool ignore = false;
                            for (int i41 = 0; i41 < client.ignoresCount; i41 += 1)
                            {
                                if (client.ignoresList[i41] == mob.nameHash)
                                {
                                    ignore = true;
                                }
                            }

                            if (!ignore)
                            {
                                mob.lastMessageTimeout = 150;
                                mob.lastMessage = s3;
                                client.DisplayMessage(mob.username + ": " + mob.lastMessage, 2);
                            }
                            off += byte7;
                        }
                        else if (mobUpdateType == 2)
                        {
                            int lastDamageCount = DataOperations.GetByte(client.packetData[off]);
                            off += 1;
                            int currentHits = DataOperations.GetByte(client.packetData[off]);
                            off += 1;
                            int baseHits = DataOperations.GetByte(client.packetData[off]);
                            off += 1;
                            mob.lastDamageCount = lastDamageCount;
                            mob.currentHits = currentHits;
                            mob.baseHits = baseHits;
                            mob.combatTimer = 200;
                            if (mob == client.ourPlayer)
                            {
                                client.playerStatCurrent[3] = currentHits;
                                client.playerStatBase[3] = baseHits;
                                client.showWelcomeBox = false;
                                client.showServerMessageBox = false;
                            }
                        }
                        else if (mobUpdateType == 3)
                        {
                            int l30 = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            int l34 = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            mob.projectileType = l30;
                            mob.attackingNpcIndex = l34;
                            mob.attackingPlayerIndex = -1;
                            mob.projectileDistance = client.projectileRange;
                        }
                        else if (mobUpdateType == 4)
                        {
                            int i31 = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            int i35 = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            mob.projectileType = i31;
                            mob.attackingPlayerIndex = i35;
                            mob.attackingNpcIndex = -1;
                            mob.projectileDistance = client.projectileRange;
                        }
                        else if (mobUpdateType == 5)
                        {
                            mob.serverID = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            mob.nameHash = DataOperations.GetLong(client.packetData, off);
                            off += 8;
                            mob.username = DataOperations.HashToName(mob.nameHash);
                            int appearanceCount = DataOperations.GetByte(client.packetData[off]);
                            off += 1;
                            for (int j35 = 0; j35 < appearanceCount; j35 += 1)
                            {
                                mob.appearanceItems[j35] = DataOperations.GetByte(client.packetData[off]);
                                off += 1;
                            }

                            for (int j38 = appearanceCount; j38 < 12; j38 += 1)
                            {
                                mob.appearanceItems[j38] = 0;
                            }

                            mob.hairColour = client.packetData[off++] & 0xff;
                            mob.topColour = client.packetData[off++] & 0xff;
                            mob.bottomColour = client.packetData[off++] & 0xff;
                            mob.skinColour = client.packetData[off++] & 0xff;
                            mob.level = client.packetData[off++] & 0xff;
                            mob.playerSkulled = client.packetData[off++] & 0xff;
                            mob.Admin = client.packetData[off++] & 0xff;
                        }
                        else if (mobUpdateType == 6)
                        {
                            sbyte byte8 = client.packetData[off];
                            off += 1;
                            string s4 = ChatMessage.BytesToString(client.packetData, off, byte8);
                            mob.lastMessageTimeout = 150;
                            mob.lastMessage = s4;
                            if (mob == client.ourPlayer)
                            {
                                client.DisplayMessage(mob.username + ": " + mob.lastMessage, 5);
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
                        if (DataOperations.GetByte(client.packetData[off]) == 255)
                        {
                            int newCount = 0;
                            int newSectionX = client.sectionX + client.packetData[off + 1] >> 3;
                            int newSectionY = client.sectionY + client.packetData[off + 2] >> 3;
                            off += 3;
                            for (int current = 0; current < client.wallObjectCount; current += 1)
                            {
                                int newX = (client.wallObjectX[current] >> 3) - newSectionX;
                                int newY = (client.wallObjectY[current] >> 3) - newSectionY;
                                if (newX != 0 || newY != 0)
                                {
                                    if (current != newCount)
                                    {
                                        client.wallObjectArray[newCount] = client.wallObjectArray[current];
                                        client.wallObjectArray[newCount].index = newCount + 10000;
                                        client.wallObjectX[newCount] = client.wallObjectX[current];
                                        client.wallObjectY[newCount] = client.wallObjectY[current];
                                        client.wallObjectDirection[newCount] = client.wallObjectDirection[current];
                                        client.wallObjectID[newCount] = client.wallObjectID[current];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    client.gameCamera.RemoveModel(client.wallObjectArray[current]);
                                    client.engineHandle.RemoveWallObject(client.wallObjectX[current], client.wallObjectY[current], client.wallObjectDirection[current], client.wallObjectID[current]);
                                }
                            }

                            client.wallObjectCount = newCount;
                        }
                        else
                        {
                            int newID = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            int newSectionX = client.sectionX + client.packetData[off++];
                            int newSectionY = client.sectionY + client.packetData[off++];
                            sbyte direction = client.packetData[off++];
                            int newCount = 0;
                            for (int current = 0; current < client.wallObjectCount; current += 1)
                            {
                                if (client.wallObjectX[current] != newSectionX || client.wallObjectY[current] != newSectionY || client.wallObjectDirection[current] != direction)
                                {
                                    if (current != newCount)
                                    {
                                        client.wallObjectArray[newCount] = client.wallObjectArray[current];
                                        client.wallObjectArray[newCount].index = newCount + 10000;
                                        client.wallObjectX[newCount] = client.wallObjectX[current];
                                        client.wallObjectY[newCount] = client.wallObjectY[current];
                                        client.wallObjectDirection[newCount] = client.wallObjectDirection[current];
                                        client.wallObjectID[newCount] = client.wallObjectID[current];
                                    }
                                    newCount += 1;
                                }
                                else
                                {
                                    client.gameCamera.RemoveModel(client.wallObjectArray[current]);
                                    client.engineHandle.RemoveWallObject(client.wallObjectX[current], client.wallObjectY[current], client.wallObjectDirection[current], client.wallObjectID[current]);
                                }
                            }

                            client.wallObjectCount = newCount;
                            if (newID != 60000)
                            {
                                client.engineHandle.CreateWall(newSectionX, newSectionY, direction, newID);
                                GameObject k35 = client.CreateWallObject(newSectionX, newSectionY, direction, newID, client.wallObjectCount);
                                client.wallObjectArray[client.wallObjectCount] = k35;
                                client.wallObjectX[client.wallObjectCount] = newSectionX;
                                client.wallObjectY[client.wallObjectCount] = newSectionY;
                                client.wallObjectID[client.wallObjectCount] = newID;
                                client.wallObjectDirection[client.wallObjectCount++] = direction;
                            }
                        }
                    }

                    return;
                }
                if (packetID == 77)
                {
                    client.lastNpcCount = client.npcCount;
                    client.npcCount = 0;
                    for (int j2 = 0; j2 < client.lastNpcCount; j2 += 1)
                    {
                        client.lastNpcArray[j2] = client.npcArray[j2];
                    }

                    int off = 8;
                    int newCount = DataOperations.GetBits(client.packetData, off, 8);
                    off += 8;
                    for (int current = 0; current < newCount; current += 1)
                    {
                        ClientMob newNpc = client.GetLastNpc(DataOperations.GetBits(client.packetData, off, 16));
                        off += 16;
                        int needsUpdate = DataOperations.GetBits(client.packetData, off, 1);
                        off += 1;
                        if (needsUpdate != 0)
                        {
                            int j32 = DataOperations.GetBits(client.packetData, off, 1);
                            off += 1;
                            if (j32 == 0)
                            {
                                int nextSprite = DataOperations.GetBits(client.packetData, off, 3);
                                off += 3;
                                int waypointCurrent = newNpc.waypointCurrent;
                                int waypointX = newNpc.waypointsX[waypointCurrent];
                                int waypointY = newNpc.waypointsY[waypointCurrent];
                                if (nextSprite == 2 || nextSprite == 1 || nextSprite == 3)
                                {
                                    waypointX += client.gridSize;
                                }

                                if (nextSprite == 6 || nextSprite == 5 || nextSprite == 7)
                                {
                                    waypointX -= client.gridSize;
                                }

                                if (nextSprite == 4 || nextSprite == 3 || nextSprite == 5)
                                {
                                    waypointY += client.gridSize;
                                }

                                if (nextSprite == 0 || nextSprite == 1 || nextSprite == 7)
                                {
                                    waypointY -= client.gridSize;
                                }

                                newNpc.nextSprite = nextSprite;
                                newNpc.waypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                                newNpc.waypointsX[waypointCurrent] = waypointX;
                                newNpc.waypointsY[waypointCurrent] = waypointY;
                            }
                            else
                            {
                                int nextSprite = DataOperations.GetBits(client.packetData, off, 4);
                                off += 4;
                                if ((nextSprite & 0xc) == 12)
                                {
                                    continue;
                                }
                                newNpc.nextSprite = nextSprite;
                            }
                        }
                        client.npcArray[client.npcCount++] = newNpc;
                    }

                    while (off + 34 < packetLength * 8)
                    {
                        int mobIndex = DataOperations.GetBits(client.packetData, off, 16);
                        off += 16;
                        int areaMobX = DataOperations.GetBits(client.packetData, off, 5);
                        off += 5;
                        if (areaMobX > 15)
                        {
                            areaMobX -= 32;
                        }

                        int areaMobY = DataOperations.GetBits(client.packetData, off, 5);
                        off += 5;
                        if (areaMobY > 15)
                        {
                            areaMobY -= 32;
                        }

                        int mobSprite = DataOperations.GetBits(client.packetData, off, 4);
                        off += 4;
                        int mobX = (client.sectionX + areaMobX) * client.gridSize + 64;
                        int mobY = (client.sectionY + areaMobY) * client.gridSize + 64;
                        int addIndex = DataOperations.GetBits(client.packetData, off, 10);
                        off += 10;
                        if (addIndex >= client.entityManager.NpcCount)
                        {
                            addIndex = 24;
                        }

                        client.CreateNpc(mobIndex, mobX, mobY, mobSprite, addIndex);
                    }
                    return;
                }
                if (packetID == 190)
                {
                    int newCount = DataOperations.GetShort(client.packetData, 1);
                    int off = 3;
                    for (int l16 = 0; l16 < newCount; l16 += 1)
                    {
                        int npcIndex = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        ClientMob mob = client.npcAttackingArray[npcIndex];
                        int updateType = DataOperations.GetByte(client.packetData[off]);
                        off += 1;
                        if (updateType == 1)
                        {
                            int playerIndex = DataOperations.GetShort(client.packetData, off);
                            off += 2;
                            sbyte messageLength = client.packetData[off];
                            off += 1;
                            if (mob is not null)
                            {
                                string s5 = ChatMessage.BytesToString(client.packetData, off, messageLength);
                                mob.lastMessageTimeout = 150;
                                mob.lastMessage = s5;
                                if (playerIndex == client.ourPlayer.serverIndex)
                                {
                                    client.DisplayMessage(string.Format(LocalisationManager.GetString("social.npc_message"), client.entityManager.GetNpc(mob.npcId).Name, mob.lastMessage), 5);
                                }
                            }
                            off += messageLength;
                        }
                        else
                            if (updateType == 2)
                            {
                                int lastDamageCount = DataOperations.GetByte(client.packetData[off]);
                                off += 1;
                                int currentHits = DataOperations.GetByte(client.packetData[off]);
                                off += 1;
                                int baseHits = DataOperations.GetByte(client.packetData[off]);
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
                    client.showQuestionMenu = true;
                    int count = DataOperations.GetByte(client.packetData[1]);
                    client.questionMenuCount = count;
                    int off = 2;
                    for (int index = 0; index < count; index += 1)
                    {
                        int optionLength = DataOperations.GetByte(client.packetData[off]);
                        off += 1;
                        client.questionMenuAnswer[index] = new string(client.packetData.Select(byteValue => (char)byteValue).ToArray(), off, optionLength);
                        off += optionLength;
                    }

                    return;
                }
                if (packetID == 127)
                {
                    client.showQuestionMenu = false;
                    return;
                }
                if (packetID == 131)
                {
                    client.loadArea = true;
                    client.serverIndex = DataOperations.GetShort(client.packetData, 1);
                    client.wildX = DataOperations.GetShort(client.packetData, 3);
                    client.wildY = DataOperations.GetShort(client.packetData, 5);
                    client.layerIndex = DataOperations.GetShort(client.packetData, 7);
                    client.layerModifier = DataOperations.GetShort(client.packetData, 9);
                    client.wildY -= client.layerIndex * client.layerModifier;
                    client.needsClear = true;
                    client.hasWorldInfo = true;
                    return;
                }
                if (packetID == 180)
                {
                    int off = 1;
                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        client.playerStatCurrent[stat] = DataOperations.GetByte(client.packetData[off++]);
                    }

                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        client.playerStatBase[stat] = DataOperations.GetByte(client.packetData[off++]);
                    }

                    for (int stat = 0; stat < 18; stat += 1)
                    {
                        client.playerStatExp[stat] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }
                    return;
                }
                if (packetID == 177)
                {
                    int off = 1;
                    for (int j3 = 0; j3 < 5; j3 += 1)
                    {
                        client.equipmentStatus[j3] = DataOperations.GetSignedShort(client.packetData, off);
                        off += 2;
                    }
                    return;
                }
                if (packetID == 165)
                {
                    client.playerAliveTimeout = 250;
                    return;
                }
                if (packetID == 115)
                {
                    int k3 = (packetLength - 1) / 4;
                    for (int i11 = 0; i11 < k3; i11 += 1)
                    {
                        int k17 = client.sectionX + DataOperations.GetSignedShort(client.packetData, 1 + i11 * 4) >> 3;
                        int i22 = client.sectionY + DataOperations.GetSignedShort(client.packetData, 3 + i11 * 4) >> 3;
                        int j25 = 0;
                        for (int l28 = 0; l28 < client.groundItemCount; l28 += 1)
                        {
                            int j33 = (client.groundItemX[l28] >> 3) - k17;
                            int l36 = (client.groundItemY[l28] >> 3) - i22;
                            if (j33 != 0 || l36 != 0)
                            {
                                if (l28 != j25)
                                {
                                    client.groundItemX[j25] = client.groundItemX[l28];
                                    client.groundItemY[j25] = client.groundItemY[l28];
                                    client.groundItemID[j25] = client.groundItemID[l28];
                                    client.groundItemObjectVar[j25] = client.groundItemObjectVar[l28];
                                }
                                j25 += 1;
                            }
                        }

                        client.groundItemCount = j25;
                        j25 = 0;
                        for (int k33 = 0; k33 < client.objectCount; k33 += 1)
                        {
                            int i37 = (client.objectX[k33] >> 3) - k17;
                            int j39 = (client.objectY[k33] >> 3) - i22;
                            if (i37 != 0 || j39 != 0)
                            {
                                if (k33 != j25)
                                {
                                    client.objectArray[j25] = client.objectArray[k33];
                                    client.objectArray[j25].index = j25;
                                    client.objectX[j25] = client.objectX[k33];
                                    client.objectY[j25] = client.objectY[k33];
                                    client.objectType[j25] = client.objectType[k33];
                                    client.objectRotation[j25] = client.objectRotation[k33];
                                }
                                j25 += 1;
                            }
                            else
                            {
                                client.gameCamera.RemoveModel(client.objectArray[k33]);
                                client.engineHandle.RemoveObject(client.objectX[k33], client.objectY[k33], client.objectType[k33], client.objectRotation[k33]);
                            }
                        }

                        client.objectCount = j25;
                        j25 = 0;
                        for (int j37 = 0; j37 < client.wallObjectCount; j37 += 1)
                        {
                            int k39 = (client.wallObjectX[j37] >> 3) - k17;
                            int l41 = (client.wallObjectY[j37] >> 3) - i22;
                            if (k39 != 0 || l41 != 0)
                            {
                                if (j37 != j25)
                                {
                                    client.wallObjectArray[j25] = client.wallObjectArray[j37];
                                    client.wallObjectArray[j25].index = j25 + 10000;
                                    client.wallObjectX[j25] = client.wallObjectX[j37];
                                    client.wallObjectY[j25] = client.wallObjectY[j37];
                                    client.wallObjectDirection[j25] = client.wallObjectDirection[j37];
                                    client.wallObjectID[j25] = client.wallObjectID[j37];
                                }
                                j25 += 1;
                            }
                            else
                            {
                                client.gameCamera.RemoveModel(client.wallObjectArray[j37]);
                                client.engineHandle.RemoveWallObject(client.wallObjectX[j37], client.wallObjectY[j37], client.wallObjectDirection[j37], client.wallObjectID[j37]);
                            }
                        }

                        client.wallObjectCount = j25;
                    }

                    return;
                }
                if (packetID == 207)
                {
                    client.showAppearanceWindow = true;
                    return;
                }
                if (packetID == 4)
                {
                    int tradeOther = DataOperations.GetShort(client.packetData, 1);
                    if (client.playerBufferArray[tradeOther] is not null)
                    {
                        client.tradeOtherName = client.playerBufferArray[tradeOther].username;
                    }

                    client.showTradeBox = true;
                    client.tradeOtherAccepted = false;
                    client.tradeWeAccepted = false;
                    client.tradeItemsOurCount = 0;
                    client.tradeItemsOtherCount = 0;
                    return;
                }
                if (packetID == 187)
                {
                    client.showTradeBox = false;
                    client.showTradeConfirmBox = false;
                    return;
                }
                if (packetID == 250)
                {
                    client.tradeItemsOtherCount = client.packetData[1] & 0xff;
                    int i4 = 2;
                    for (int j11 = 0; j11 < client.tradeItemsOtherCount; j11 += 1)
                    {
                        client.tradeItemsOther[j11] = DataOperations.GetShort(client.packetData, i4);
                        i4 += 2;
                        client.tradeItemOtherCount[j11] = DataOperations.GetInt(client.packetData, i4);
                        i4 += 4;
                    }

                    client.tradeOtherAccepted = false;
                    client.tradeWeAccepted = false;
                    return;
                }
                if (packetID == 92)
                {
                    sbyte byte0 = client.packetData[1];
                    if (byte0 == 1)
                    {
                        client.tradeOtherAccepted = true;
                        return;
                    }
                    else
                    {
                        client.tradeOtherAccepted = false;
                        return;
                    }
                }
                if (packetID == 253)
                {
                    client.showShopBox = true;
                    int off = 1;
                    int newShopItemCount = client.packetData[off++] & 0xff;
                    sbyte isGeneralShop = client.packetData[off++];
                    client.shopItemSellPriceModifier = client.packetData[off++] & 0xff;
                    client.shopItemBuyPriceModifier = client.packetData[off++] & 0xff;
                    for (int j22 = 0; j22 < 40; j22 += 1)
                    {
                        client.shopItems[j22] = -1;
                    }

                    for (int item = 0; item < newShopItemCount; item += 1)
                    {
                        client.shopItems[item] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.shopItemCount[item] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.shopItemBuyPrice[item] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                        client.shopItemSellPrice[item] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    if (isGeneralShop == 1)
                    {
                        int i29 = 39;
                        for (int l33 = 0; l33 < client.inventoryItemsCount; l33 += 1)
                        {
                            if (i29 < newShopItemCount)
                            {
                                break;
                            }

                            bool flag2 = false;
                            for (int l39 = 0; l39 < 40; l39 += 1)
                            {
                                if (client.shopItems[l39] != client.inventoryItems[l33])
                                {
                                    continue;
                                }

                                flag2 = true;
                                break;
                            }

                            if (client.inventoryItems[l33] == 10)
                            {
                                flag2 = true;
                            }

                            if (!flag2)
                            {
                                client.shopItems[i29] = client.inventoryItems[l33] & 0x7fff;
                                client.shopItemCount[i29] = 0;
                                client.shopItemSellPrice[i29] = client.entityManager.GetItem(client.shopItems[i29]).BasePrice - (int)(client.entityManager.GetItem(client.shopItems[i29]).BasePrice / 2.5);
                                client.shopItemSellPrice[i29] -= (int)(client.shopItemSellPrice[i29] * 0.10);
                                i29 -= 1;
                            }
                        }

                    }
                    if (client.selectedShopItemIndex >= 0 && client.selectedShopItemIndex < 40 && client.shopItems[client.selectedShopItemIndex] != client.selectedShopItemType)
                    {
                        client.selectedShopItemIndex = -1;
                        client.selectedShopItemType = -2;
                    }
                    return;
                }
                if (packetID == 220)
                {
                    client.showShopBox = false;
                    return;
                }
                if (packetID == 18)
                {
                    sbyte byte1 = client.packetData[1];
                    if (byte1 == 1)
                    {
                        client.tradeWeAccepted = true;
                        return;
                    }
                    else
                    {
                        client.tradeWeAccepted = false;
                        return;
                    }
                }
                if (packetID == 152)
                {
                    client.configCameraAutoAngle = DataOperations.GetByte(client.packetData[1]) == 1;
                    client.configOneMouseButton = DataOperations.GetByte(client.packetData[2]) == 1;
                    client.configSoundOff = DataOperations.GetByte(client.packetData[3]) == 1;
                    client.showRoofs = DataOperations.GetByte(client.packetData[4]) == 1;
                    client.autoScreenshot = DataOperations.GetByte(client.packetData[5]) == 1;
                    client.showCombatWindow = DataOperations.GetByte(client.packetData[6]) == 1;
                    return;
                }
                if (packetID == 209)
                {
                    for (int k4 = 0; k4 < packetLength - 1; k4 += 1)
                    {
                        bool flag = client.packetData[k4 + 1] == 1;
                        if (!client.prayerOn[k4] && flag)
                        {
                            client.PlaySound("prayeron");
                        }

                        if (client.prayerOn[k4] && !flag)
                        {
                            client.PlaySound("prayeroff");
                        }

                        client.prayerOn[k4] = flag;
                    }

                    return;
                }
                if (packetID == 93)
                {
                    client.showBankBox = true;
                    int off = 1;
                    client.serverBankItemsCount = client.packetData[off++] & 0xff;
                    client.maxBankItems = client.packetData[off++] & 0xff;
                    for (int l11 = 0; l11 < client.serverBankItemsCount; l11 += 1)
                    {
                        client.serverBankItems[l11] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.serverBankItemCount[l11] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    client.UpdateBankItems();
                    return;
                }
                if (packetID == 171)
                {
                    client.showBankBox = false;
                    return;
                }
                if (packetID == 211)
                {
                    int j5 = client.packetData[1] & 0xff;
                    client.playerStatExp[j5] = DataOperations.GetInt(client.packetData, 2);
                    return;
                }
                if (packetID == 229)
                {
                    int k5 = DataOperations.GetShort(client.packetData, 1);
                    if (client.playerBufferArray[k5] is not null)
                    {
                        client.duelOpponent = client.playerBufferArray[k5].username;
                    }

                    client.showDuelBox = true;
                    client.duelMyItemCount = 0;
                    client.duelOpponentItemCount = 0;
                    client.duelOpponentAccepted = false;
                    client.duelMyAccepted = false;
                    client.duelNoRetreating = false;
                    client.duelNoMagic = false;
                    client.duelNoPrayer = false;
                    client.duelNoWeapons = false;
                    return;
                }
                if (packetID == 160)
                {
                    client.showDuelBox = false;
                    client.showDuelConfirmBox = false;
                    return;
                }

#warning have not fixed the following yet....
                if (packetID == 251)
                {
                    client.showTradeConfirmBox = true;
                    client.tradeConfirmAccepted = false;
                    client.showTradeBox = false;
                    int off = 1;
                    client.tradeConfirmOtherNameLong = DataOperations.GetLong(client.packetData, off);
                    off += 8;
                    client.tradeConfirmOtherItemCount = client.packetData[off++] & 0xff;
                    for (int i12 = 0; i12 < client.tradeConfirmOtherItemCount; i12 += 1)
                    {
                        client.tradeConfirmOtherItems[i12] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.tradeConfirmOtherItemsCount[i12] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    client.tradeConfigItemCount = client.packetData[off++] & 0xff;
                    for (int l17 = 0; l17 < client.tradeConfigItemCount; l17 += 1)
                    {
                        client.tradeConfirmItems[l17] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.tradeConfigItemsCount[l17] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    return;
                }
                if (packetID == 63)
                {
                    client.duelOpponentItemCount = client.packetData[1] & 0xff;
                    int off = 2;
                    for (int j12 = 0; j12 < client.duelOpponentItemCount; j12 += 1)
                    {
                        client.duelOpponentItems[j12] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.duelOpponentItemsCount[j12] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    client.duelOpponentAccepted = false;
                    client.duelMyAccepted = false;
                    return;
                }
                if (packetID == 198)
                {
                    if (client.packetData[1] == 1)
                    {
                        client.duelNoRetreating = true;
                    }
                    else
                    {
                        client.duelNoRetreating = false;
                    }

                    if (client.packetData[2] == 1)
                    {
                        client.duelNoMagic = true;
                    }
                    else
                    {
                        client.duelNoMagic = false;
                    }

                    if (client.packetData[3] == 1)
                    {
                        client.duelNoPrayer = true;
                    }
                    else
                    {
                        client.duelNoPrayer = false;
                    }

                    if (client.packetData[4] == 1)
                    {
                        client.duelNoWeapons = true;
                    }
                    else
                    {
                        client.duelNoWeapons = false;
                    }

                    client.duelOpponentAccepted = false;
                    client.duelMyAccepted = false;
                    return;
                }
                if (packetID == 139)
                {
                    int off = 1;
                    int itemSlot = client.packetData[off++] & 0xff;
                    int itemID = DataOperations.GetShort(client.packetData, off);
                    off += 2;
                    int itemCount = DataOperations.GetInt(client.packetData, off);
                    off += 4;
                    if (itemCount == 0)
                    {
                        client.serverBankItemsCount -= 1;
                        for (int l25 = itemSlot; l25 < client.serverBankItemsCount; l25 += 1)
                        {
                            client.serverBankItems[l25] = client.serverBankItems[l25 + 1];
                            client.serverBankItemCount[l25] = client.serverBankItemCount[l25 + 1];
                        }

                    }
                    else
                    {
                        client.serverBankItems[itemSlot] = itemID;
                        client.serverBankItemCount[itemSlot] = itemCount;
                        if (itemSlot >= client.serverBankItemsCount)
                        {
                            client.serverBankItemsCount = itemSlot + 1;
                        }
                    }
                    client.UpdateBankItems();
                    return;
                }
                if (packetID == 228)
                {
                    int off = 1;
                    int count = 1;
                    int newCount = client.packetData[off++] & 0xff;
                    int data = DataOperations.GetShort(client.packetData, off);
                    off += 2;
                    if (!client.entityManager.GetItem(data & 0x7fff).IsStackable)
                    {
                        count = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }
                    client.inventoryItems[newCount] = data & 0x7fff;
                    client.inventoryItemEquipped[newCount] = data / 32768;
                    client.inventoryItemCount[newCount] = count;
                    if (newCount >= client.inventoryItemsCount)
                    {
                        client.inventoryItemsCount = newCount + 1;
                    }

                    return;
                }
                if (packetID == 191)
                {
                    int l6 = client.packetData[1] & 0xff;
                    client.inventoryItemsCount -= 1;
                    for (int i13 = l6; i13 < client.inventoryItemsCount; i13 += 1)
                    {
                        client.inventoryItems[i13] = client.inventoryItems[i13 + 1];
                        client.inventoryItemCount[i13] = client.inventoryItemCount[i13 + 1];
                        client.inventoryItemEquipped[i13] = client.inventoryItemEquipped[i13 + 1];
                    }

                    return;
                }
                if (packetID == 208)
                {
                    int off = 1;
                    int stat = client.packetData[off++] & 0xff;
                    client.playerStatCurrent[stat] = DataOperations.GetByte(client.packetData[off++]);
                    client.playerStatBase[stat] = DataOperations.GetByte(client.packetData[off++]);
                    client.playerStatExp[stat] = DataOperations.GetInt(client.packetData, off);
                    off += 4;
                    return;
                }
                if (packetID == 65)
                {
                    sbyte byte2 = client.packetData[1];
                    if (byte2 == 1)
                    {
                        client.duelOpponentAccepted = true;
                        return;
                    }
                    else
                    {
                        client.duelOpponentAccepted = false;
                        return;
                    }
                }
                if (packetID == 197)
                {
                    sbyte byte3 = client.packetData[1];
                    if (byte3 == 1)
                    {
                        client.duelMyAccepted = true;
                        return;
                    }
                    else
                    {
                        client.duelMyAccepted = false;
                        return;
                    }
                }
                if (packetID == 147)
                {
                    client.showDuelConfirmBox = true;
                    client.duelConfirmOurAccepted = false;
                    client.showDuelBox = false;
                    int off = 1;
                    client.duelOpponentHash = DataOperations.GetLong(client.packetData, off);
                    off += 8;
                    client.duelOpponentStakeCount = client.packetData[off++] & 0xff;
                    for (int k13 = 0; k13 < client.duelOpponentStakeCount; k13 += 1)
                    {
                        client.duelOpponentStakeItem[k13] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.duelOutStakeItemCount[k13] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    client.duelOurStakeCount = client.packetData[off++] & 0xff;
                    for (int k18 = 0; k18 < client.duelOurStakeCount; k18 += 1)
                    {
                        client.duelOurStakeItem[k18] = DataOperations.GetShort(client.packetData, off);
                        off += 2;
                        client.duelOurStakeItemCount[k18] = DataOperations.GetInt(client.packetData, off);
                        off += 4;
                    }

                    client.duelRetreat = client.packetData[off++] & 0xff;
                    client.duelMagic = client.packetData[off++] & 0xff;
                    client.duelPrayer = client.packetData[off++] & 0xff;
                    client.duelWeapons = client.packetData[off++] & 0xff;
                    return;
                }
                if (packetID == 11)
                {
                    string s1 = new(client.packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    client.PlaySound(s1);
                    return;
                }
                if (packetID == 23)
                {
                    if (client.teleBubbleCount < 50)
                    {
                        int type = client.packetData[1] & 0xff;
                        int x = client.packetData[2] + client.sectionX;
                        int y = client.packetData[3] + client.sectionY;
                        client.teleBubbleType[client.teleBubbleCount] = type;
                        client.teleBubbleTime[client.teleBubbleCount] = 0;
                        client.teleBubbleX[client.teleBubbleCount] = x;
                        client.teleBubbleY[client.teleBubbleCount] = y;
                        client.teleBubbleCount += 1;
                    }
                    return;
                }
                if (packetID == 248)
                {
                    if (!client.loginScreenShown)
                    {
                        client.lastLoginDays = DataOperations.GetShort(client.packetData, 1);
                        client.subDaysLeft = DataOperations.GetShort(client.packetData, 3);
                        client.lastLoginAddress = new string(client.packetData.Select(byteValue => (char)byteValue).ToArray(), 5, packetLength - 5);
                        client.showWelcomeBox = true;
                        client.loginScreenShown = true;
                    }
                    return;
                }
                if (packetID == 148)
                {
                    client.serverMessage = new string(client.packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    client.showServerMessageBox = true;
                    client.serverMessageBoxTop = false;
                    return;
                }
                if (packetID == 64)
                {
                    client.serverMessage = new string(client.packetData.Select(byteValue => (char)byteValue).ToArray(), 1, packetLength - 1);
                    client.showServerMessageBox = true;
                    client.serverMessageBoxTop = true;
                    return;
                }
                if (packetID == 126)
                {
                    client.fatigue = DataOperations.GetShort(client.packetData, 1);
                    return;
                }
                if (packetID == 206)
                {
                    client.killingSpree = DataOperations.GetShort(client.packetData, 1);
                    return;
                }
                if (packetID == 224)
                {
                    client.isSleeping = false;
                    return;
                }
                if (packetID == 225)
                {
                    client.sleepingStatusText = "Incorrect - Please wait...";
                    return;
                }
                if (packetID == 172)
                {
                    client.systemUpdate = DataOperations.GetShort(client.packetData, 1) * 32;
                    return;
                }
                if (packetID == 181)
                {
                    if (client.autoScreenshot)
                    {
                        client.TakeScreenshot(false);
                    }

                    return;
                }
                if (packetID == 182)
                {
                    int off = 1;
                    client.questPoints = DataOperations.GetShort(client.packetData, off);
                    off += 2;
                    for (int l4 = 0; l4 < client.questName.Length; l4 += 1)
                    {
                        client.questStage[l4] = client.packetData[l4 + 1];
                    }

                    return;
                }
                if (packetID == 233)
                {
                    client.questPoints = DataOperations.GetByte(client.packetData[1]);
                    int count = DataOperations.GetByte(client.packetData[2]);
                    int off = 3;
                    string[] newQuestNames = new string[count];
                    int[] newQuestStage = new int[count];
                    for (int i = 0; i < count; i += 1)
                    {
                        newQuestNames[i] = client.questName[DataOperations.GetByte(client.packetData[off++])];
                        newQuestStage[i] = DataOperations.GetByte(client.packetData[off++]);
                    }
                    client.usedQuestName = newQuestNames;
                    client.questStage = newQuestStage;
                    return;
                }
                if (packetID == 129)
                {
                    client.combatStyle = DataOperations.GetByte(client.packetData[1]);
                    return;
                }
                if (packetID == 110)
                {
                    // TODO: Determine if this packet should be removed.
                    logger.Debug(GameOperation.HandlePacket, "Received packet 110 (server info).");
                    return;
                }
                // Spell counts and quest progress - 2-byte value packets, silently accepted.
                if (packetID == 210 || packetID == 212 || packetID == 213)
                {
                    // Guthix/Zamorak/Saradomin spells.
                    return;
                }

                if (packetID == 128)
                {
                    // QuestPointsChange.
                    return;
                }

                if (packetID == 134)
                {
                    // Deaths.
                    return;
                }

                if (packetID == 137)
                {
                    // DruidicRitual.
                    return;
                }

                if (packetID == 138)
                {
                    // ImpCatcher.
                    return;
                }

                if (packetID == 141)
                {
                    // SheepShearer.
                    return;
                }

                if (packetID == 143)
                {
                    // DoricQuest.
                    return;
                }

                if (packetID == 132)
                {
                    // Kills.
                    return;
                }

                if (packetID == 140)
                {
                    // RomeoAndJuliet.
                    return;
                }

                if (packetID == 142)
                {
                    // WitchPotion.
                    return;
                }

                if (packetID == 144)
                {
                    // CookAssistant.
                    return;
                }

                if (packetID == 133)
                {
                    // PirateTreasure.
                    return;
                }

                if (packetID == 139)
                {
                    // BlackKnightsForte.
                    return;
                }

                if (packetID == 146)
                {
                    // RestlessGhost.
                    return;
                }

                if (packetID == 149)
                {
                    // ErnestTheChicken.
                    return;
                }

                if (packetID == 150)
                {
                    // Goblintown.
                    return;
                }

                if (packetID == 151)
                {
                    // MinersBlazonQuest.
                    return;
                }

                if (packetID == 173)
                {
                    // VampireSlayer.
                    return;
                }

                if (packetID == 174)
                {
                    // MilleniumItemQuest.
                    return;
                }

                if (packetID == 175)
                {
                    // KnightsSword.
                    return;
                }

                if (packetID == 202)
                {
                    // GoblinDiplomacy.
                    return;
                }

                if (packetID == 203)
                {
                    // TheGrandTree.
                    return;
                }

                if (packetID == 204)
                {
                    // FightArena.
                    return;
                }

                if (packetID == 205)
                {
                    // Hazeel.
                    return;
                }

                if (packetID == 214)
                {
                    // HolyGrail.
                    return;
                }

                if (packetID == 215)
                {
                    // MerlinsCrystal.
                    return;
                }

                if (packetID == 216)
                {
                    // LostCity.
                    return;
                }

                if (packetID == 217)
                {
                    // WitchHouse.
                    return;
                }
                logger.Warn(
                    GameOperation.HandlePacket,
                    "Unhandled packet.",
                    new LogInfo(GameLogInfoKey.PacketId, packetID),
                    new LogInfo(GameLogInfoKey.PacketLength, packetLength));
            }
            catch (Exception ex)
            {
                logger.Error(GameOperation.HandlePacket, "Packet handling has failed.", ex);
                // ex.printStackTrace();
            }
        }

    }

}
