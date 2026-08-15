using System;
using System.IO;
using System.Text;

namespace OpenRS.Net.Client.Net
{
    internal sealed class PacketConstructionWriter
    {
        private static int BufferFlushThresholdNumerator => 4;

        private static int BufferFlushThresholdDenominator => 5;

        private static int ByteMask => 0xff;

        private static int CommandByteOffset => 2;

        private static int DefaultSkipOffset => 8;

        private static int InitialPacketOffset => 3;

        private static long LowBitsMask => -1L;

        private static int MaximumMetricPacketLength => 10000;

        private int packetOffset = InitialPacketOffset;
        private int skipOffset = DefaultSkipOffset;

        internal void CreatePacket(PacketConstruction packetConstruction, int id)
        {
            if (HasReachedBufferFlushThreshold(packetConstruction))
            {
                try
                {
                    packetConstruction.WritePacket(0);
                }
                catch (IOException exception)
                {
                    DeferError(packetConstruction, exception.ToString());
                }
            }

            packetConstruction.packetData ??=
                new byte[packetConstruction.maxPacketLength];

            packetConstruction.packetData[
                packetConstruction.packetStart + CommandByteOffset] = (byte)id;
            packetConstruction.packetData[
                packetConstruction.packetStart + InitialPacketOffset] = 0;
            packetOffset =
                packetConstruction.packetStart + InitialPacketOffset;
            skipOffset = DefaultSkipOffset;
        }

        internal void WritePacket(
            PacketConstruction packetConstruction,
            int packetId)
        {
            if (packetConstruction.error)
            {
                ThrowDeferredError(packetConstruction);
            }

            packetConstruction.packetCount += 1;

            if (packetConstruction.packetCount < packetId)
            {
                return;
            }

            if (packetConstruction.packetStart > 0)
            {
                packetConstruction.packetCount = 0;
                packetConstruction.WriteToBuffer(
                    packetConstruction.packetData,
                    0,
                    packetConstruction.packetStart);
            }

            packetConstruction.packetStart = 0;
            packetOffset = InitialPacketOffset;
        }

        internal void AddByte(PacketConstruction packetConstruction, int value)
            => packetConstruction.packetData[packetOffset++] = (byte)value;

        internal void AddString(PacketConstruction packetConstruction, string text)
        {
            byte[] encodedBytes = Encoding.UTF8.GetBytes(text);
            Array.Copy(
                encodedBytes,
                0,
                packetConstruction.packetData,
                packetOffset,
                encodedBytes.Length);
            packetOffset += encodedBytes.Length;
        }

        internal void AddLong(PacketConstruction packetConstruction, long value)
        {
            AddInt(packetConstruction, (int)(value >> 32));
            AddInt(packetConstruction, (int)(value & LowBitsMask));
        }

        internal void AddInt(PacketConstruction packetConstruction, int value)
        {
            packetConstruction.packetData[packetOffset++] = (byte)(value >> 24);
            packetConstruction.packetData[packetOffset++] = (byte)(value >> 16);
            packetConstruction.packetData[packetOffset++] = (byte)(value >> 8);
            packetConstruction.packetData[packetOffset++] = (byte)value;
        }

        internal void Flush(PacketConstruction packetConstruction, bool format)
        {
            if (format)
            {
                packetConstruction.FormatPacket();
            }

            packetConstruction.WritePacket(0);
        }

        internal void AddShort(PacketConstruction packetConstruction, int value)
        {
            packetConstruction.packetData[packetOffset++] = (byte)(value >> 8);
            packetConstruction.packetData[packetOffset++] = (byte)value;
        }

        internal void FormatPacket(PacketConstruction packetConstruction)
        {
            if (skipOffset != DefaultSkipOffset)
            {
                packetOffset += 1;
            }

            int packetLength =
                packetOffset - packetConstruction.packetStart - CommandByteOffset;
            packetOffset = PacketFraming.WriteLength(
                packetConstruction.packetData,
                packetConstruction.packetStart,
                packetOffset,
                packetLength);
            RecordPacketMetrics(packetConstruction);
            packetConstruction.packetStart = packetOffset;

            packetConstruction.Flush(false);
        }

        internal void AddBytes(
            PacketConstruction packetConstruction,
            byte[] data,
            int offset,
            int length)
        {
            for (int byteIndex = 0; byteIndex < length; byteIndex += 1)
            {
                packetConstruction.packetData[packetOffset++] =
                    data[offset + byteIndex];
            }
        }

        private static void DeferError(
            PacketConstruction packetConstruction,
            string deferredErrorText)
        {
            packetConstruction.error = true;
            packetConstruction.errorText = deferredErrorText;
        }

        private static bool HasReachedBufferFlushThreshold(
            PacketConstruction packetConstruction)
            => packetConstruction.packetStart >
                packetConstruction.maxPacketLength *
                BufferFlushThresholdNumerator /
                BufferFlushThresholdDenominator;

        private void RecordPacketMetrics(PacketConstruction packetConstruction)
        {
            if (packetConstruction.maxPacketLength > MaximumMetricPacketLength)
            {
                return;
            }

            int commandIdentifier = packetConstruction.packetData[
                packetConstruction.packetStart + CommandByteOffset] & ByteMask;
            PacketConstruction.packetCommandCount[commandIdentifier] += 1;
            PacketConstruction.packetLengthCount[commandIdentifier] +=
                packetOffset - packetConstruction.packetStart;
        }

        private void ThrowDeferredError(PacketConstruction packetConstruction)
        {
            packetConstruction.packetStart = 0;
            packetOffset = InitialPacketOffset;
            packetConstruction.error = false;

            throw new IOException(packetConstruction.errorText);
        }
    }
}