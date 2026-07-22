using OpenRS.Net;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class WallObjectStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (command != ServerCommand.WallObjects)
            {
                return false;
            }

            HandleWallObjects(packetLength, packetData);
            return true;
        }

        private void HandleWallObjects(int packetLength, sbyte[] packetData)
        {
            int offset = 1;

            while (offset < packetLength)
            {
                if (BinaryDataReader.GetByte(packetData[offset]) ==
                    PacketHandlerConstants.SectionResetMarker)
                {
                    offset = ResetWallObjectSection(packetData, offset);
                    continue;
                }

                offset = UpdateWallObject(packetData, offset);
            }
        }

        private int ResetWallObjectSection(sbyte[] packetData, int offset)
        {
            int sectionX = SectionPacketCoordinates.GetSectionCoordinateFromByte(
                Client.sectionX,
                packetData,
                offset + 1);
            int sectionY = SectionPacketCoordinates.GetSectionCoordinateFromByte(
                Client.sectionY,
                packetData,
                offset + 2);
            int nextOffset = offset + 3;
            int retainedCount = 0;

            for (int wallIndex = 0; wallIndex < Client.wallObjectCount; wallIndex += 1)
            {
                if (IsOutsideSection(wallIndex, sectionX, sectionY))
                {
                    CopyWallObject(wallIndex, retainedCount);
                    retainedCount += 1;
                    continue;
                }

                RemoveWallObjectAt(wallIndex);
            }

            Client.wallObjectCount = retainedCount;
            return nextOffset;
        }

        private int UpdateWallObject(sbyte[] packetData, int offset)
        {
            int wallObjectId = BinaryDataReader.GetShort(packetData, offset);
            int nextOffset = offset + 2;
            int wallObjectX = Client.sectionX + packetData[nextOffset++];
            int wallObjectY = Client.sectionY + packetData[nextOffset++];
            int direction = packetData[nextOffset++];
            RemoveMatchingWallObjects(wallObjectX, wallObjectY, direction);

            if (wallObjectId != PacketHandlerConstants.ObjectRemovalIdentifier)
            {
                AddWallObject(wallObjectId, wallObjectX, wallObjectY, direction);
            }

            return nextOffset;
        }

        private void RemoveMatchingWallObjects(int wallObjectX, int wallObjectY, int direction)
        {
            int retainedCount = 0;

            for (int wallIndex = 0; wallIndex < Client.wallObjectCount; wallIndex += 1)
            {
                if (ShouldRetainWallObject(wallIndex, wallObjectX, wallObjectY, direction))
                {
                    CopyWallObject(wallIndex, retainedCount);
                    retainedCount += 1;
                    continue;
                }

                RemoveWallObjectAt(wallIndex);
            }

            Client.wallObjectCount = retainedCount;
        }

        private bool ShouldRetainWallObject(
            int wallIndex,
            int wallObjectX,
            int wallObjectY,
            int direction)
        {
            if (Client.wallObjectX[wallIndex] != wallObjectX ||
                Client.wallObjectY[wallIndex] != wallObjectY)
            {
                return true;
            }

            return Client.wallObjectDirection[wallIndex] != direction;
        }

        private bool IsOutsideSection(int wallIndex, int sectionX, int sectionY)
            => SectionPacketCoordinates.IsOutsideSection(
                Client.wallObjectX[wallIndex],
                Client.wallObjectY[wallIndex],
                sectionX,
                sectionY);

        private void RemoveWallObjectAt(int wallIndex)
        {
            Client.gameCamera.RemoveModel(Client.wallObjectArray[wallIndex]);
            Client.engineHandle.RemoveWallObject(
                Client.wallObjectX[wallIndex],
                Client.wallObjectY[wallIndex],
                Client.wallObjectDirection[wallIndex],
                Client.wallObjectID[wallIndex]);
        }

        private void AddWallObject(
            int wallObjectId,
            int wallObjectX,
            int wallObjectY,
            int direction)
        {
            Client.engineHandle.CreateWall(wallObjectX, wallObjectY, direction, wallObjectId);
            GameObject wallObject = Client.CreateWallObject(
                wallObjectX,
                wallObjectY,
                direction,
                wallObjectId,
                Client.wallObjectCount);
            Client.wallObjectArray[Client.wallObjectCount] = wallObject;
            Client.wallObjectX[Client.wallObjectCount] = wallObjectX;
            Client.wallObjectY[Client.wallObjectCount] = wallObjectY;
            Client.wallObjectID[Client.wallObjectCount] = wallObjectId;
            Client.wallObjectDirection[Client.wallObjectCount] = direction;
            Client.wallObjectCount += 1;
        }

        private void CopyWallObject(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex == destinationIndex)
            {
                return;
            }

            Client.wallObjectArray[destinationIndex] = Client.wallObjectArray[sourceIndex];
            Client.wallObjectArray[destinationIndex].Index =
                destinationIndex + PacketHandlerConstants.WallObjectIndexOffset;
            Client.wallObjectX[destinationIndex] = Client.wallObjectX[sourceIndex];
            Client.wallObjectY[destinationIndex] = Client.wallObjectY[sourceIndex];
            Client.wallObjectDirection[destinationIndex] =
                Client.wallObjectDirection[sourceIndex];
            Client.wallObjectID[destinationIndex] = Client.wallObjectID[sourceIndex];
        }
    }
}