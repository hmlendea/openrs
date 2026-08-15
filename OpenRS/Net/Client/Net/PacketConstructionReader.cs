using System.IO;

namespace OpenRS.Net.Client.Net
{
    internal sealed class PacketConstructionReader
    {
        private static int ByteMask => 0xff;

        private static string TimeoutErrorText => "time-out";

        private int swappedByte;
        private bool hasSwappedByte;

        internal int ReadPacket(
            PacketConstruction packetConstruction,
            sbyte[] packetBuffer)
        {
            try
            {
                packetConstruction._read += 1;

                if (HasReadTimedOut(packetConstruction))
                {
                    return 0;
                }

                if (packetConstruction.length == 0)
                {
                    ReadPacketHeader(packetConstruction);
                }

                if (packetConstruction.length > 0)
                {
                    return ReadPacketPayload(packetConstruction, packetBuffer);
                }
            }
            catch (IOException exception)
            {
                packetConstruction.error = true;
                packetConstruction.errorText = exception.ToString();
            }

            return 0;
        }

        private static bool HasReadTimedOut(
            PacketConstruction packetConstruction)
        {
            if (packetConstruction.maxPacketReadCount <= 0 ||
                packetConstruction._read <= packetConstruction.maxPacketReadCount)
            {
                return false;
            }

            packetConstruction.error = true;
            packetConstruction.errorText = TimeoutErrorText;
            packetConstruction.maxPacketReadCount +=
                packetConstruction.maxPacketReadCount;

            return true;
        }

        private void ReadPacketHeader(PacketConstruction packetConstruction)
        {
            int firstHeaderByte = packetConstruction.Read() & ByteMask;
            int secondHeaderByte = packetConstruction.Read() & ByteMask;

            if (PacketFraming.IsCompact(firstHeaderByte))
            {
                packetConstruction.length = firstHeaderByte;
                swappedByte = secondHeaderByte;
                hasSwappedByte = true;

                return;
            }

            packetConstruction.length = PacketFraming.DecodeExtendedLength(
                firstHeaderByte,
                secondHeaderByte);
            hasSwappedByte = false;
        }

        private int ReadPacketPayload(
            PacketConstruction packetConstruction,
            sbyte[] packetBuffer)
        {
            if (hasSwappedByte)
            {
                packetConstruction.Read(
                    packetConstruction.length - 1,
                    packetBuffer);
                packetBuffer[packetConstruction.length - 1] = (sbyte)swappedByte;
                hasSwappedByte = false;
            }
            else
            {
                packetConstruction.Read(packetConstruction.length, packetBuffer);
            }

            int packetLength = packetConstruction.length;
            packetConstruction.length = 0;
            packetConstruction._read = 0;

            return packetLength;
        }
    }
}