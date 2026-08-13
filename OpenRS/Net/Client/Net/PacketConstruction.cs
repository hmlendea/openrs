using System;
using System.IO;
using System.Text;

namespace OpenRS.Net.Client.Net
{
    public class PacketConstruction
    {
        public static int[] packetCommandCount = new int[256];
        public static int[] packetLengthCount = new int[256];

        public int length;
        public int _read;
        public int maxPacketReadCount;
        public int packetStart;
        public byte[] packetData;
        public int maxPacketLength;
        public int packetCount;
        public string errorText;
        public bool error;

        private int packetOffset;
        private int skipOffset;
        private int swappedByte;
        private bool hasSwappedByte;

        private static int BufferFlushThresholdNumerator => 4;
        private static int BufferFlushThresholdDenominator => 5;
        private static int ByteMask => 0xff;
        private static int CommandByteOffset => 2;
        private static int DefaultMaximumPacketLength => 5000;
        private static int DefaultSkipOffset => 8;
        private static int InitialPacketOffset => 3;
        private static long LowBitsMask => -1L;
        private static int MaximumMetricPacketLength => 10000;
        private static string TimeoutErrorText => "time-out";

        public PacketConstruction()
        {
            packetOffset = InitialPacketOffset;
            skipOffset = DefaultSkipOffset;
            maxPacketLength = DefaultMaximumPacketLength;
            errorText = string.Empty;
            error = false;
        }

        public virtual void CloseStream()
        {
        }

        public void CreatePacket(int id)
        {
            if (HasReachedBufferFlushThreshold())
            {
                try
                {
                    WritePacket(0);
                }
                catch (IOException exception)
                {
                    DeferError(exception.ToString());
                }
            }

            packetData ??= new byte[maxPacketLength];

            packetData[packetStart + CommandByteOffset] = (byte)id;
            packetData[packetStart + InitialPacketOffset] = 0;
            packetOffset = packetStart + InitialPacketOffset;
            skipOffset = DefaultSkipOffset;
        }

        public void WritePacket(int packetId)
        {
            if (error)
            {
                ThrowDeferredError();
            }

            packetCount += 1;

            if (packetCount < packetId)
            {
                return;
            }

            if (packetStart > 0)
            {
                packetCount = 0;
                WriteToBuffer(packetData, 0, packetStart);
            }

            packetStart = 0;
            packetOffset = InitialPacketOffset;
        }

        public void AddByte(int i) => packetData[packetOffset++] = (byte)i;

        public void AddString(string s)
        {
            byte[] encodedBytes = Encoding.UTF8.GetBytes(s);
            Array.Copy(encodedBytes, 0, packetData, packetOffset, encodedBytes.Length);
            packetOffset += encodedBytes.Length;
        }

        public void AddLong(long l)
        {
            AddInt((int)(l >> 32));
            AddInt((int)(l & LowBitsMask));
        }

        public virtual void WriteToBuffer(byte[] buffer, int offset, int length)
        {
        }

        public virtual void ReadInputStream(int size, int type, sbyte[] buffer)
        {
        }

        public int ReadShort()
        {
            int highByte = ReadByte();
            int lowByte = ReadByte();

            return highByte * 256 + lowByte;
        }

        public virtual int Read()
        {
            return 0;
        }

        public void Read(int size, sbyte[] buffer) => ReadInputStream(size, 0, buffer);

        public void AddInt(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 24);
            packetData[packetOffset++] = (byte)(i >> 16);
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public void Flush(bool format = true)
        {
            if (format)
            {
                FormatPacket();
            }

            WritePacket(0);
        }

        public void AddShort(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public long ReadLong()
        {
            long firstSegment = ReadShort();
            long secondSegment = ReadShort();
            long thirdSegment = ReadShort();
            long fourthSegment = ReadShort();

            return
                (firstSegment << 48) +
                (secondSegment << 32) +
                (thirdSegment << 16) +
                fourthSegment;
        }

        public void FormatPacket()
        {
            if (skipOffset != DefaultSkipOffset)
            {
                packetOffset += 1;
            }

            int packetLength = packetOffset - packetStart - CommandByteOffset;
            packetOffset = PacketFraming.WriteLength(
                packetData,
                packetStart,
                packetOffset,
                packetLength);
            RecordPacketMetrics();
            packetStart = packetOffset;

            Flush(false);
        }

        public void AddBytes(byte[] data, int off, int len)
        {
            for (int byteIndex = 0; byteIndex < len; byteIndex += 1)
            {
                packetData[packetOffset++] = data[off + byteIndex];
            }
        }

        public bool HasData() => packetStart > 0;

        public int ReadPacket(sbyte[] packetBuffer)
        {
            try
            {
                _read += 1;

                if (HasReadTimedOut())
                {
                    return 0;
                }

                if (length == 0)
                {
                    ReadPacketHeader();
                }

                if (length > 0)
                {
                    return ReadPacketPayload(packetBuffer);
                }
            }
            catch (IOException exception)
            {
                DeferError(exception.ToString());
            }

            return 0;
        }

        public int ReadByte() => Read();

        private void DeferError(string deferredErrorText)
        {
            error = true;
            errorText = deferredErrorText;
        }

        private bool HasReachedBufferFlushThreshold() =>
            packetStart >
            maxPacketLength * BufferFlushThresholdNumerator / BufferFlushThresholdDenominator;

        private bool HasReadTimedOut()
        {
            if (maxPacketReadCount <= 0 || _read <= maxPacketReadCount)
            {
                return false;
            }

            DeferError(TimeoutErrorText);
            maxPacketReadCount += maxPacketReadCount;

            return true;
        }

        private void ReadPacketHeader()
        {
            int firstHeaderByte = Read() & ByteMask;
            int secondHeaderByte = Read() & ByteMask;

            if (PacketFraming.IsCompact(firstHeaderByte))
            {
                length = firstHeaderByte;
                swappedByte = secondHeaderByte;
                hasSwappedByte = true;
                return;
            }

            length = PacketFraming.DecodeExtendedLength(firstHeaderByte, secondHeaderByte);
            hasSwappedByte = false;
        }

        private int ReadPacketPayload(sbyte[] packetBuffer)
        {
            if (hasSwappedByte)
            {
                Read(length - 1, packetBuffer);
                packetBuffer[length - 1] = (sbyte)swappedByte;
                hasSwappedByte = false;
            }
            else
            {
                Read(length, packetBuffer);
            }

            int packetLength = length;
            length = 0;
            _read = 0;

            return packetLength;
        }

        private void RecordPacketMetrics()
        {
            if (maxPacketLength > MaximumMetricPacketLength)
            {
                return;
            }

            int commandIdentifier = packetData[packetStart + CommandByteOffset] & ByteMask;
            packetCommandCount[commandIdentifier] += 1;
            packetLengthCount[commandIdentifier] += packetOffset - packetStart;
        }

        private void ThrowDeferredError()
        {
            packetStart = 0;
            packetOffset = InitialPacketOffset;
            error = false;

            throw new IOException(errorText);
        }

    }
}
