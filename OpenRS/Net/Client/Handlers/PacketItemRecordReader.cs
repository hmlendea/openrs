namespace OpenRS.Net.Client.Handlers
{
    internal static class PacketItemRecordReader
    {
        internal static int ReadCountPrefixedItemAndCountRecords(
            sbyte[] packetData,
            PacketReadCursor cursor,
            int[] itemIdentifiers,
            int[] itemCounts)
        {
            int recordCount = PacketCursorDataReader.ReadByte(packetData, cursor);
            ReadItemAndCountRecords(packetData, cursor, recordCount, itemIdentifiers, itemCounts);
            return recordCount;
        }

        internal static void ReadItemAndCountRecords(
            sbyte[] packetData,
            PacketReadCursor cursor,
            int recordCount,
            int[] itemIdentifiers,
            int[] itemCounts)
        {
            for (int itemIndex = 0; itemIndex < recordCount; itemIndex += 1)
            {
                itemIdentifiers[itemIndex] = PacketCursorDataReader.ReadShort(packetData, cursor);
                itemCounts[itemIndex] = PacketCursorDataReader.ReadInt(packetData, cursor);
            }
        }

        internal static void ReadShopItemRecords(
            sbyte[] packetData,
            PacketReadCursor cursor,
            int recordCount,
            int[] itemIdentifiers,
            int[] itemCounts,
            int[] buyPrices,
            int[] sellPrices)
        {
            for (int itemIndex = 0; itemIndex < recordCount; itemIndex += 1)
            {
                itemIdentifiers[itemIndex] = PacketCursorDataReader.ReadShort(packetData, cursor);
                itemCounts[itemIndex] = PacketCursorDataReader.ReadShort(packetData, cursor);
                buyPrices[itemIndex] = PacketCursorDataReader.ReadInt(packetData, cursor);
                sellPrices[itemIndex] = PacketCursorDataReader.ReadInt(packetData, cursor);
            }
        }
    }
}