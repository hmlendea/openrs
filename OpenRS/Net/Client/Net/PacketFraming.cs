namespace OpenRS.Net.Client.Net
{
    internal static class PacketFraming
    {
        private static int ByteMask => 0xff;

        private static int CompactPacketLengthLimit => 160;

        internal static bool IsCompact(int firstHeaderByte)
            => firstHeaderByte < CompactPacketLengthLimit;

        internal static int DecodeExtendedLength(int firstHeaderByte, int secondHeaderByte) =>
            (firstHeaderByte - CompactPacketLengthLimit) * 256 +
            secondHeaderByte;

        internal static int WriteLength(
            byte[] packetData,
            int packetStart,
            int packetOffset,
            int packetLength)
        {
            if (packetLength >= CompactPacketLengthLimit)
            {
                packetData[packetStart] =
                    (byte)(CompactPacketLengthLimit + packetLength / 256);
                packetData[packetStart + 1] = (byte)(packetLength & ByteMask);

                return packetOffset;
            }

            packetData[packetStart] = (byte)packetLength;
            packetOffset -= 1;
            packetData[packetStart + 1] = packetData[packetOffset];

            return packetOffset;
        }
    }
}