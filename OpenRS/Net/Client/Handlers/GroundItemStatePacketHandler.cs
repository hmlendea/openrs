using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class GroundItemStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int StackFlagMask => 0x8000;
        private static int StackFlagValue => 0x7fff;
        private static int ResetValue => -1;

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (command != ServerCommand.GroundItems)
            {
                return false;
            }

            HandleGroundItems(packetLength, packetData);
            return true;
        }

        private void HandleGroundItems(int packetLength, sbyte[] packetData)
        {
            if (Client.needsClear)
            {
                ResetGroundItems();
            }

            int offset = 1;

            while (offset < packetLength)
            {
                if (BinaryDataReader.GetByte(packetData[offset]) ==
                    PacketHandlerConstants.SectionResetMarker)
                {
                    offset = ResetGroundItemSection(packetData, offset);
                    continue;
                }

                offset = UpdateGroundItem(packetData, offset);
            }
        }

        private void ResetGroundItems()
        {
            for (int itemIndex = 0; itemIndex < Client.groundItemID.Length; itemIndex += 1)
            {
                Client.groundItemX[itemIndex] = ResetValue;
                Client.groundItemY[itemIndex] = ResetValue;
                Client.groundItemID[itemIndex] = ResetValue;
                Client.groundItemObjectVar[itemIndex] = ResetValue;
            }

            Client.groundItemCount = 0;
            Client.needsClear = false;
        }

        private int ResetGroundItemSection(sbyte[] packetData, int offset)
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

            for (int itemIndex = 0; itemIndex < Client.groundItemCount; itemIndex += 1)
            {
                if (IsOutsideSection(itemIndex, sectionX, sectionY))
                {
                    CopyGroundItem(itemIndex, retainedCount);
                    retainedCount += 1;
                }
            }

            Client.groundItemCount = retainedCount;
            return nextOffset;
        }

        private int UpdateGroundItem(sbyte[] packetData, int offset)
        {
            int itemId = BinaryDataReader.GetShort(packetData, offset);
            int nextOffset = offset + 2;
            int itemX = Client.sectionX + packetData[nextOffset++];
            int itemY = Client.sectionY + packetData[nextOffset++];

            if ((itemId & StackFlagMask) == 0)
            {
                AddGroundItem(itemId, itemX, itemY);
                return nextOffset;
            }

            RemoveGroundItem(itemId & StackFlagValue, itemX, itemY);
            return nextOffset;
        }

        private void AddGroundItem(int itemId, int itemX, int itemY)
        {
            int itemIndex = Client.groundItemCount;
            Client.groundItemX[itemIndex] = itemX;
            Client.groundItemY[itemIndex] = itemY;
            Client.groundItemID[itemIndex] = itemId;
            Client.groundItemObjectVar[itemIndex] = GetGroundItemElevationOffset(itemX, itemY);
            Client.groundItemCount += 1;
        }

        private int GetGroundItemElevationOffset(int itemX, int itemY)
        {
            for (int objectIndex = 0; objectIndex < Client.objectCount; objectIndex += 1)
            {
                if (Client.objectX[objectIndex] != itemX || Client.objectY[objectIndex] != itemY)
                {
                    continue;
                }

                return Client.entityManager.GetWorldObject(Client.objectType[objectIndex])
                    .GroundItemElevationOffset;
            }

            return 0;
        }

        private void RemoveGroundItem(int itemId, int itemX, int itemY)
        {
            int retainedCount = 0;

            for (int itemIndex = 0; itemIndex < Client.groundItemCount; itemIndex += 1)
            {
                if (ShouldRetainGroundItem(itemIndex, itemId, itemX, itemY))
                {
                    CopyGroundItem(itemIndex, retainedCount);
                    retainedCount += 1;
                }
            }

            Client.groundItemCount = retainedCount;
        }

        private bool ShouldRetainGroundItem(int itemIndex, int itemId, int itemX, int itemY)
        {
            if (Client.groundItemX[itemIndex] != itemX || Client.groundItemY[itemIndex] != itemY)
            {
                return true;
            }

            return Client.groundItemID[itemIndex] != itemId;
        }

        private bool IsOutsideSection(int itemIndex, int sectionX, int sectionY)
            => SectionPacketCoordinates.IsOutsideSection(
                Client.groundItemX[itemIndex],
                Client.groundItemY[itemIndex],
                sectionX,
                sectionY);

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
    }
}