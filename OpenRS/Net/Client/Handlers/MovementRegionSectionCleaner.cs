using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class MovementRegionSectionCleaner(GameClient client) : PacketHandlerBase(client)
    {
        internal void HandleGroundItemSections(int packetLength, sbyte[] packetData)
        {
            int offset = 1;

            while (offset < packetLength)
            {
                int sectionX = SectionPacketCoordinates.GetSectionCoordinateFromSignedShort(
                    Client.sectionX,
                    packetData,
                    offset);
                int sectionY = SectionPacketCoordinates.GetSectionCoordinateFromSignedShort(
                    Client.sectionY,
                    packetData,
                    offset + 2);
                offset += 4;
                RemoveGroundItemsInSection(sectionX, sectionY);
                RemoveObjectsInSection(sectionX, sectionY);
                RemoveWallObjectsInSection(sectionX, sectionY);
            }
        }

        private void RemoveGroundItemsInSection(int sectionX, int sectionY)
        {
            int retainedCount = 0;

            for (int itemIndex = 0; itemIndex < Client.groundItemCount; itemIndex += 1)
            {
                if (SectionPacketCoordinates.IsOutsideSection(
                    Client.groundItemX[itemIndex],
                    Client.groundItemY[itemIndex],
                    sectionX,
                    sectionY))
                {
                    CopyGroundItem(itemIndex, retainedCount);
                    retainedCount += 1;
                }
            }

            Client.groundItemCount = retainedCount;
        }

        private void RemoveObjectsInSection(int sectionX, int sectionY)
        {
            int retainedCount = 0;

            for (int objectIndex = 0; objectIndex < Client.objectCount; objectIndex += 1)
            {
                if (SectionPacketCoordinates.IsOutsideSection(
                    Client.objectX[objectIndex],
                    Client.objectY[objectIndex],
                    sectionX,
                    sectionY))
                {
                    CopyObject(objectIndex, retainedCount);
                    retainedCount += 1;
                    continue;
                }

                RemoveObjectAt(objectIndex);
            }

            Client.objectCount = retainedCount;
        }

        private void RemoveWallObjectsInSection(int sectionX, int sectionY)
        {
            int retainedCount = 0;

            for (int wallIndex = 0; wallIndex < Client.wallObjectCount; wallIndex += 1)
            {
                if (SectionPacketCoordinates.IsOutsideSection(
                    Client.wallObjectX[wallIndex],
                    Client.wallObjectY[wallIndex],
                    sectionX,
                    sectionY))
                {
                    CopyWallObject(wallIndex, retainedCount);
                    retainedCount += 1;
                    continue;
                }

                RemoveWallObjectAt(wallIndex);
            }

            Client.wallObjectCount = retainedCount;
        }

        private void RemoveObjectAt(int objectIndex)
        {
            Client.gameCamera.RemoveModel(Client.objectArray[objectIndex]);
            Client.engineHandle.RemoveObject(
                Client.objectX[objectIndex],
                Client.objectY[objectIndex],
                Client.objectType[objectIndex],
                Client.objectRotation[objectIndex]);
        }

        private void RemoveWallObjectAt(int wallIndex)
        {
            Client.gameCamera.RemoveModel(Client.wallObjectArray[wallIndex]);
            Client.engineHandle.RemoveWallObject(
                Client.wallObjectX[wallIndex],
                Client.wallObjectY[wallIndex],
                Client.wallObjectDirection[wallIndex],
                Client.wallObjectID[wallIndex]);
        }

        private void CopyGroundItem(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex == destinationIndex)
            {
                return;
            }

            Client.groundItemX[destinationIndex] = Client.groundItemX[sourceIndex];
            Client.groundItemY[destinationIndex] = Client.groundItemY[sourceIndex];
            Client.groundItemID[destinationIndex] = Client.groundItemID[sourceIndex];
            Client.groundItemObjectVar[destinationIndex] =
                Client.groundItemObjectVar[sourceIndex];
        }

        private void CopyObject(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex == destinationIndex)
            {
                return;
            }

            Client.objectArray[destinationIndex] = Client.objectArray[sourceIndex];
            Client.objectArray[destinationIndex].Index = destinationIndex;
            Client.objectX[destinationIndex] = Client.objectX[sourceIndex];
            Client.objectY[destinationIndex] = Client.objectY[sourceIndex];
            Client.objectType[destinationIndex] = Client.objectType[sourceIndex];
            Client.objectRotation[destinationIndex] = Client.objectRotation[sourceIndex];
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