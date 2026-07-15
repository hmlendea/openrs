namespace OpenRS.Net.Client.Data
{
    internal sealed class BinaryDataReader
    {
        private static readonly int[] bitMasks =
        [
            0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767,
            65535, 0x1ffff, 0x3ffff, 0x7ffff, 0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
            0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff, 0xfffffff, 0x1fffffff, 0x3fffffff,
            0x7fffffff, -1
        ];

        internal static int GetByte(sbyte value) => value & 0xff;

        internal static int GetShort(sbyte[] data, int offset) =>
            ((data[offset] & 0xff) << 8) + (data[offset + 1] & 0xff);

        internal static int GetInt16(sbyte[] data, int offset) => GetShort(data, offset);

        internal static long GetLong(sbyte[] data, int offset) =>
            ((GetInt(data, offset) & 0xffffffffL) << 32) + (GetInt(data, offset + 4) & 0xffffffffL);

        internal static long GetLong(byte[] data, int offset) =>
            ((GetInt(data, offset) & 0xffffffffL) << 32) + (GetInt(data, offset + 4) & 0xffffffffL);

        internal static int GetInt(sbyte[] data, int offset) =>
            ((data[offset] & 0xff) << 24) +
            ((data[offset + 1] & 0xff) << 16) +
            ((data[offset + 2] & 0xff) << 8) +
            (data[offset + 3] & 0xff);

        internal static int GetInt(byte[] data, int offset) =>
            ((data[offset] & 0xff) << 24) +
            ((data[offset + 1] & 0xff) << 16) +
            ((data[offset + 2] & 0xff) << 8) +
            (data[offset + 3] & 0xff);

        internal static int GetSignedShort(sbyte[] data, int offset)
        {
            int value = GetShort(data, offset);

            if (value > short.MaxValue)
            {
                value -= 1 << 16;
            }

            return value;
        }

        internal static int GetBits(sbyte[] data, int offset, int bitCount)
        {
            int byteOffset = offset >> 3;
            int bitModifier = 8 - (offset & 7);
            int result = 0;

            for (; bitCount > bitModifier; bitModifier = 8)
            {
                result += (data[byteOffset] & bitMasks[bitModifier]) << bitCount - bitModifier;
                byteOffset += 1;
                bitCount -= bitModifier;
            }

            if (bitCount == bitModifier)
            {
                result += data[byteOffset] & bitMasks[bitModifier];
            }
            else
            {
                result += data[byteOffset] >> bitModifier - bitCount & bitMasks[bitCount];
            }

            return result;
        }
    }
}
