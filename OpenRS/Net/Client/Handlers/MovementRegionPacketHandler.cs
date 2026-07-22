using OpenRS.Net;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class MovementRegionPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static readonly int NewEntityBaseOffset = 8;
        private static readonly int PlayerStepHistorySize = 10;
        private static readonly int NegativeAreaOffsetThreshold = 15;
        private static readonly int NegativeAreaOffsetAdjustment = 32;
        private static readonly int UnknownNpcFallbackIndex = 24;

        private readonly MovementRegionSectionCleaner movementRegionSectionCleaner =
            new(client);

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (TryHandleRegionPacket(command, packetData))
            {
                return true;
            }

            if (TryHandleMovementPacket(command, packetLength, packetData))
            {
                return true;
            }

            return TryHandleGroundItemSectionPacket(command, packetLength, packetData);
        }

        private bool TryHandleRegionPacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.WorldInfo)
            {
                HandleWorldInfo(packetData);
                return true;
            }

            return false;
        }

        private bool TryHandleMovementPacket(
            ServerCommand command,
            int packetLength,
            sbyte[] packetData)
        {
            if (command == ServerCommand.PlayerPositions)
            {
                HandlePlayerPositions(packetLength, packetData);
                return true;
            }

            if (command == ServerCommand.NpcPositions)
            {
                HandleNpcPositions(packetLength, packetData);
                return true;
            }

            return false;
        }

        private bool TryHandleGroundItemSectionPacket(
            ServerCommand command,
            int packetLength,
            sbyte[] packetData)
        {
            if (command != ServerCommand.GroundItemSections)
            {
                return false;
            }

            movementRegionSectionCleaner.HandleGroundItemSections(packetLength, packetData);
            return true;
        }

        private void HandleWorldInfo(sbyte[] packetData)
        {
            Client.loadArea = true;
            Client.serverIndex = BinaryDataReader.GetShort(packetData, 1);
            Client.wildX = BinaryDataReader.GetShort(packetData, 3);
            Client.wildY = BinaryDataReader.GetShort(packetData, 5);
            Client.layerIndex = BinaryDataReader.GetShort(packetData, 7);
            Client.layerModifier = BinaryDataReader.GetShort(packetData, 9);
            Client.wildY -= Client.layerIndex * Client.layerModifier;
            Client.needsClear = true;
            Client.hasWorldInfo = true;
        }

        private void HandlePlayerPositions(int packetLength, sbyte[] packetData)
        {
            if (!Client.hasWorldInfo)
            {
                return;
            }

            SnapshotLastPlayers();

            int offset = NewEntityBaseOffset;
            int sectionX = BinaryDataReader.GetBits(packetData, offset, 11);
            offset += 11;
            int sectionY = BinaryDataReader.GetBits(packetData, offset, 13);
            offset += 13;
            int sprite = BinaryDataReader.GetBits(packetData, offset, 4);
            offset += 4;
            int mapEnterX = LoadPlayerSection(sectionX, sectionY, sprite);
            int mapEnterY = SectionPacketCoordinates.GetMapTileCoordinate(
                Client.sectionY,
                Client.gridSize);

            Client.playerCount = 0;
            Client.ourPlayer = Client.CreatePlayer(
                Client.serverIndex,
                mapEnterX,
                mapEnterY,
                sprite);

            int newPlayerCount = BinaryDataReader.GetBits(packetData, offset, 8);
            offset += 8;
            PacketReadCursor cursor = new(offset);
            ReadExistingPlayerPositions(newPlayerCount, packetData, cursor);
            int bufferCount = ReadNewPlayerPositions(packetLength, packetData, cursor);
            SendPlayerBufferRequest(bufferCount);
        }

        private void SnapshotLastPlayers()
        {
            Client.lastPlayerCount = Client.playerCount;

            for (int playerIndex = 0;
                playerIndex < Client.lastPlayerCount;
                playerIndex += 1)
            {
                Client.lastPlayerArray[playerIndex] = Client.playerArray[playerIndex];
            }
        }

        private int LoadPlayerSection(int sectionX, int sectionY, int sprite)
        {
            Client.sectionX = sectionX;
            Client.sectionY = sectionY;
            bool sectionLoaded = Client.LoadSection(Client.sectionX, Client.sectionY);
            Client.sectionX -= Client.areaX;
            Client.sectionY -= Client.areaY;
            int mapEnterX = SectionPacketCoordinates.GetMapTileCoordinate(
                Client.sectionX,
                Client.gridSize);
            int mapEnterY = SectionPacketCoordinates.GetMapTileCoordinate(
                Client.sectionY,
                Client.gridSize);

            if (sectionLoaded)
            {
                ResetOurPlayerPath(mapEnterX, mapEnterY);
            }

            return mapEnterX;
        }

        private void ResetOurPlayerPath(int mapEnterX, int mapEnterY)
        {
            Client.ourPlayer.WaypointCurrent = 0;
            Client.ourPlayer.WaypointsEndSprite = 0;
            Client.ourPlayer.LocationX = mapEnterX;
            Client.ourPlayer.LocationY = mapEnterY;
            Client.ourPlayer.WaypointXPositions[0] = mapEnterX;
            Client.ourPlayer.WaypointYPositions[0] = mapEnterY;
        }

        private void ReadExistingPlayerPositions(
            int newPlayerCount,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            for (int playerIndex = 0; playerIndex < newPlayerCount; playerIndex += 1)
            {
                ClientMob mob = Client.GetLastPlayer(
                    ReadBits(packetData, cursor, 16));
                int playerAtTile = ReadBits(packetData, cursor, 1);

                if (playerAtTile != 0)
                {
                    UpdateExistingPlayerMovement(mob, packetData, cursor);
                }

                if (mob is not null)
                {
                    Client.playerArray[Client.playerCount++] = mob;
                }
            }
        }

        private void UpdateExistingPlayerMovement(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            int waypointsLeft = ReadBits(packetData, cursor, 1);

            if (waypointsLeft == 0)
            {
                int nextSprite = ReadBits(packetData, cursor, 3);
                ApplyWalkingStep(mob, nextSprite);
                return;
            }

            int needsNextSprite = ReadBits(packetData, cursor, 4);

            if ((needsNextSprite & 0xc) != 12 && mob is not null)
            {
                mob.NextSprite = needsNextSprite;
            }
        }

        private int ReadNewPlayerPositions(
            int packetLength,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            int bufferCount = 0;

            while (cursor.Index + 24 < packetLength * 8)
            {
                int mobIndex = ReadBits(packetData, cursor, 16);
                int areaMobX = ReadSignedAreaOffset(packetData, cursor);
                int areaMobY = ReadSignedAreaOffset(packetData, cursor);
                int mobSprite = ReadBits(packetData, cursor, 4);
                int addIndex = ReadBits(packetData, cursor, 1);
                int mobX = SectionPacketCoordinates.GetWorldTileCoordinate(
                    Client.sectionX,
                    areaMobX,
                    Client.gridSize);
                int mobY = SectionPacketCoordinates.GetWorldTileCoordinate(
                    Client.sectionY,
                    areaMobY,
                    Client.gridSize);
                Client.CreatePlayer(mobIndex, mobX, mobY, mobSprite);

                if (addIndex == 0)
                {
                    Client.playerBufferArrayIndexes[bufferCount++] = mobIndex;
                }
            }

            return bufferCount;
        }

        private void SendPlayerBufferRequest(int bufferCount)
        {
            if (bufferCount <= 0)
            {
                return;
            }

            Client.streamClass.CreatePacket(83);
            Client.streamClass.AddShort(bufferCount);

            for (int bufferIndex = 0; bufferIndex < bufferCount; bufferIndex += 1)
            {
                ClientMob mob = Client.playerBufferArray[
                    Client.playerBufferArrayIndexes[bufferIndex]];
                Client.streamClass.AddShort(mob.ServerIndex);
                Client.streamClass.AddShort(mob.ServerId);
            }

            Client.streamClass.FormatPacket();
        }

        private void HandleNpcPositions(int packetLength, sbyte[] packetData)
        {
            SnapshotLastNpcs();
            int offset = NewEntityBaseOffset;
            int existingNpcCount = BinaryDataReader.GetBits(packetData, offset, 8);
            offset += 8;
            PacketReadCursor cursor = new(offset);
            ReadExistingNpcPositions(existingNpcCount, packetData, cursor);
            ReadNewNpcPositions(packetLength, packetData, cursor);
        }

        private void SnapshotLastNpcs()
        {
            Client.lastNpcCount = Client.npcCount;
            Client.npcCount = 0;

            for (int npcIndex = 0; npcIndex < Client.lastNpcCount; npcIndex += 1)
            {
                Client.lastNpcArray[npcIndex] = Client.npcArray[npcIndex];
            }
        }

        private void ReadExistingNpcPositions(
            int existingNpcCount,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            for (int npcIndex = 0; npcIndex < existingNpcCount; npcIndex += 1)
            {
                ClientMob npc = Client.GetLastNpc(ReadBits(packetData, cursor, 16));
                int needsUpdate = ReadBits(packetData, cursor, 1);

                if (needsUpdate != 0)
                {
                    UpdateExistingNpcPosition(npc, packetData, cursor);
                }

                Client.npcArray[Client.npcCount++] = npc;
            }
        }

        private void UpdateExistingNpcPosition(
            ClientMob npc,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            int hasWaypoint = ReadBits(packetData, cursor, 1);

            if (hasWaypoint == 0)
            {
                int nextSprite = ReadBits(packetData, cursor, 3);
                ApplyWalkingStep(npc, nextSprite);
                return;
            }

            int nextNpcSprite = ReadBits(packetData, cursor, 4);

            if ((nextNpcSprite & 0xc) != 12)
            {
                npc.NextSprite = nextNpcSprite;
            }
        }

        private void ReadNewNpcPositions(
            int packetLength,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            while (cursor.Index + 34 < packetLength * 8)
            {
                int mobIndex = ReadBits(packetData, cursor, 16);
                int areaMobX = ReadSignedAreaOffset(packetData, cursor);
                int areaMobY = ReadSignedAreaOffset(packetData, cursor);
                int mobSprite = ReadBits(packetData, cursor, 4);
                int mobX = SectionPacketCoordinates.GetWorldTileCoordinate(
                    Client.sectionX,
                    areaMobX,
                    Client.gridSize);
                int mobY = SectionPacketCoordinates.GetWorldTileCoordinate(
                    Client.sectionY,
                    areaMobY,
                    Client.gridSize);
                int addIndex = ReadBits(packetData, cursor, 10);

                if (addIndex >= Client.entityManager.NpcCount)
                {
                    addIndex = UnknownNpcFallbackIndex;
                }

                Client.CreateNpc(mobIndex, mobX, mobY, mobSprite, addIndex);
            }
        }

        private void ApplyWalkingStep(ClientMob mob, int nextSprite)
        {
            if (mob is null)
            {
                return;
            }

            int waypointCurrent = mob.WaypointCurrent;
            int waypointX = AdjustWaypointX(mob.WaypointXPositions[waypointCurrent], nextSprite);
            int waypointY = AdjustWaypointY(mob.WaypointYPositions[waypointCurrent], nextSprite);
            mob.NextSprite = nextSprite;
            waypointCurrent = (waypointCurrent + 1) % PlayerStepHistorySize;
            mob.WaypointCurrent = waypointCurrent;
            mob.WaypointXPositions[waypointCurrent] = waypointX;
            mob.WaypointYPositions[waypointCurrent] = waypointY;
        }

        private int AdjustWaypointX(int waypointX, int nextSprite)
        {
            if (nextSprite == 2 || nextSprite == 1 || nextSprite == 3)
            {
                waypointX += Client.gridSize;
            }

            if (nextSprite == 6 || nextSprite == 5 || nextSprite == 7)
            {
                waypointX -= Client.gridSize;
            }

            return waypointX;
        }

        private int AdjustWaypointY(int waypointY, int nextSprite)
        {
            if (nextSprite == 4 || nextSprite == 3 || nextSprite == 5)
            {
                waypointY += Client.gridSize;
            }

            if (nextSprite == 0 || nextSprite == 1 || nextSprite == 7)
            {
                waypointY -= Client.gridSize;
            }

            return waypointY;
        }

        private static int ReadSignedAreaOffset(sbyte[] packetData, PacketReadCursor cursor)
        {
            int areaOffset = ReadBits(packetData, cursor, 5);

            if (areaOffset > NegativeAreaOffsetThreshold)
            {
                areaOffset -= NegativeAreaOffsetAdjustment;
            }

            return areaOffset;
        }

        private static int ReadBits(sbyte[] packetData, PacketReadCursor cursor, int bitCount)
            => PacketCursorDataReader.ReadBits(packetData, cursor, bitCount);
    }
}
