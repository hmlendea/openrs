using System;
using System.Text;
using System.IO;

namespace RuneScapeSolo.Lib.Net
{
    /// <summary>
    /// Packet construction.
    /// </summary>
    public class PacketConstruction
    {
        int length;
        int packetReadCount;
        int packetStart;
        int packetOffset;
        int skipOffset;
        byte[] packetData;
        int PacketCount { get; set; }

        static int[] packetCommandCount = new int[256];
        static int[] packetLengthCount = new int[256];

        /// <summary>
        /// Gets or sets the maximum packet count.
        /// </summary>
        /// <value>The maximum packet count.</value>
        public int MaximumPacketCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum packet read count.
        /// </summary>
        /// <value>The maximum packet read count.</value>
        public int MaximumPacketReadCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PacketConstruction"/> has data.
        /// </summary>
        /// <value><c>true</c> if has data; otherwise, <c>false</c>.</value>
        public bool HasData => packetStart > 0;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PacketConstruction"/> has errors.
        /// </summary>
        /// <value><c>true</c> if has errors; otherwise, <c>false</c>.</value>
        public bool HasErrors { get; protected set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketConstruction"/> class.
        /// </summary>
        public PacketConstruction()
        {
            packetOffset = 3;
            skipOffset = 8;
            MaximumPacketCount = 5000;
            ErrorMessage = "";
            HasErrors = false;
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        public virtual void CloseStream()
        {
        }

        /// <summary>
        /// Creates the packet.
        /// </summary>
        /// <param name="value">Value.</param>
        public void CreatePacket(int value)
        {
            if (packetStart > (MaximumPacketCount * 4) / 5)
            {
                try
                {
                    WritePacket(0);
                }
                catch (IOException ex)
                {
                    HasErrors = true;
                    ErrorMessage = ex.Message;
                }
            }

            if (packetData == null)
            {
                packetData = new byte[MaximumPacketCount];
            }

            packetData[packetStart + 2] = (byte)value;
            packetData[packetStart + 3] = 0;

            packetOffset = packetStart + 3;
            skipOffset = 8;
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="value">Value.</param>
        public void WritePacket(int value)
        {
            if (HasErrors)
            {
                packetStart = 0;
                packetOffset = 3;
                HasErrors = false;
                throw new IOException(ErrorMessage);
            }

            PacketCount++;

            if (PacketCount < value)
            {
                return;
            }
            if (packetStart > 0)
            {
                PacketCount = 0;
                WriteToBuffer(packetData, 0, packetStart);
            }

            packetStart = 0;
            packetOffset = 3;
        }

        /// <summary>
        /// Adds an 8-bit integer.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddInt8(int value)
        {
            packetData[packetOffset++] = (byte)value;
        }

        /// <summary>
        /// Adds a 16-bit integer.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddInt16(int value)
        {
            packetData[packetOffset++] = (byte)(value >> 8);
            packetData[packetOffset++] = (byte)value;
        }

        /// <summary>
        /// Adds an 32-bit integer.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddInt32(int value)
        {
            packetData[packetOffset++] = (byte)(value >> 24);
            packetData[packetOffset++] = (byte)(value >> 16);
            packetData[packetOffset++] = (byte)(value >> 8);
            packetData[packetOffset++] = (byte)value;
        }

        /// <summary>
        /// Adds an 64-bit integer.
        /// </summary>
        /// <param name="value">Value.</param>
        public void AddInt64(long value)
        {
            AddInt32((int)(value >> 32));
            AddInt32((int)(value & -1L));
        }

        /// <summary>
        /// Adds a string.
        /// </summary>
        /// <param name="str">Text.</param>
        public void AddString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            Array.Copy(bytes, 0, packetData, packetOffset, bytes.Length);

            packetOffset += bytes.Length;
        }

        /// <summary>
        /// Adds the bytes.
        /// </summary>
        /// <param name="data">Data.</param>
        public void AddBytes(byte[] data)
        {
            AddBytes(data, 0, data.Length);
        }

        /// <summary>
        /// Adds the bytes.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="length">Length.</param>
        public void AddBytes(byte[] data, int offset, int length)
        {
            Array.Copy(data, offset, packetData, packetOffset, length);
            packetOffset += length;
        }

        public virtual void WriteToBuffer(byte[] abyte0, int i, int j)
        {
        }

        /// <summary>
        /// Reads an 8-bit integer.
        /// </summary>
        public int ReadInt8()
        {
            return ReadInputStream();
        }

        /// <summary>
        /// Reads a 16-bit integer.
        /// </summary>
        public int ReadInt16()
        {
            int i = ReadInt8();
            int j = ReadInt8();

            return i * 256 + j;
        }

        /// <summary>
        /// Reads a 64-bit integer.
        /// </summary>
        public long ReadInt64()
        {
            long q1 = ReadInt16();
            long q2 = ReadInt16();
            long q3 = ReadInt16();
            long q4 = ReadInt16();

            return (q1 << 48) + (q2 << 32) + (q3 << 16) + q4;
        }

        /// <summary>
        /// Reads.
        /// </summary>
        /// <param name="length">The index.</param>
        /// <param name="data">Data.</param>
        public void Read(int length, sbyte[] data)
        {
            ReadInputStream(length, 0, data);
        }

        /// <summary>
        /// Reads the input stream.
        /// </summary>
        /// <returns>The input stream.</returns>
        public virtual int ReadInputStream()
        {
            return 0;
        }

        /// <summary>
        /// Reads the input stream.
        /// </summary>
        /// <param name="length">Length.</param>
        /// <param name="data">Data.</param>
        public virtual void ReadInputStream(int length, sbyte[] data)
        {
            ReadInputStream(length, 0, data);
        }

        public virtual void ReadInputStream(int length, int offset, sbyte[] data)
        {
        }

        /// <summary>
        /// Finalises the packet.
        /// </summary>
        /// <param name="format">If set to <c>true</c> format.</param>
        public void FinalisePacket(bool format = true)
        {
            if (format)
            {
                FormatPacket();
            }

            WritePacket(0);
        }

        /// <summary>
        /// Formats the packet.
        /// </summary>
        public void FormatPacket()
        {
            if (skipOffset != 8)
            {
                packetOffset++;
            }

            int j = packetOffset - packetStart - 2;

            if (j >= 160)
            {
                packetData[packetStart] = (byte)(160 + j / 256);
                packetData[packetStart + 1] = (byte)(j & 0xff);
            }
            else
            {
                packetData[packetStart] = (byte)j;
                packetOffset--;
                packetData[packetStart + 1] = packetData[packetOffset];
            }
            if (MaximumPacketCount <= 10000)
            {
                int k = packetData[packetStart + 2] & 0xff;

                packetCommandCount[k]++;
                packetLengthCount[k] += packetOffset - packetStart;
            }

            packetStart = packetOffset;
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="data">Data.</param>
        public int ReadPacket(sbyte[] data)
        {
            try
            {
                packetReadCount += 1;

                if (MaximumPacketReadCount > 0 && packetReadCount > MaximumPacketReadCount)
                {
                    HasErrors = true;
                    ErrorMessage = "time-out";
                    MaximumPacketReadCount += MaximumPacketReadCount;

                    return 0;
                }

                if (length == 0)// && available() >= 2)
                {
                    length = ReadInputStream();

                    if (length >= 160)
                    {
                        length = (length - 160) * 256 + ReadInputStream();
                    }
                }

                if (length > 0)// && available() >= length)
                {
                    if (length >= 160)
                    {
                        ReadInputStream(length, data);
                    }
                    else
                    {
                        data[length - 1] = (sbyte)ReadInputStream();

                        if (length > 1)
                        {
                            ReadInputStream(length - 1, data);
                        }
                    }

                    int readBytes = length;
                    length = 0;
                    packetReadCount = 0;

                    return readBytes;
                }
            }
            catch (IOException ex)
            {
                HasErrors = true;
                ErrorMessage = ex.Message;
            }

            return 0;
        }
    }
}
