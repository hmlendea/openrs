using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal static class PacketCursorDataReader
    {
        internal static int ReadBits(
            sbyte[] packetData,
            PacketReadCursor cursor,
            int bitCount)
        {
            int value = BinaryDataReader.GetBits(packetData, cursor.Index, bitCount);
            cursor.Index += bitCount;
            return value;
        }

        internal static int ReadByte(sbyte[] packetData, PacketReadCursor cursor)
        {
            int value = BinaryDataReader.GetByte(packetData[cursor.Index]);
            cursor.Index += 1;
            return value;
        }

        internal static int ReadInt(sbyte[] packetData, PacketReadCursor cursor)
        {
            int value = BinaryDataReader.GetInt(packetData, cursor.Index);
            cursor.Index += 4;
            return value;
        }

        internal static long ReadLong(sbyte[] packetData, PacketReadCursor cursor)
        {
            long value = BinaryDataReader.GetLong(packetData, cursor.Index);
            cursor.Index += 8;
            return value;
        }

        internal static int ReadShort(sbyte[] packetData, PacketReadCursor cursor)
        {
            int value = BinaryDataReader.GetShort(packetData, cursor.Index);
            cursor.Index += 2;
            return value;
        }
    }
}