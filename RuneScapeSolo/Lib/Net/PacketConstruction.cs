using System;
using System.Text;
using System.IO;

namespace RuneScapeSolo.Lib.Net
{
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

        public int MaximumPacketCount { get; set; }
        public int MaximumPacketReadCount { get; set; }

        public string ErrorMessage { get; set; }
        public bool HasErrors { get; set; }

        public PacketConstruction()
        {
            packetOffset = 3;
            skipOffset = 8;
            MaximumPacketCount = 5000;
            ErrorMessage = "";
            HasErrors = false;
        }

        public virtual void CloseStream()
        {
        }

        public void CreatePacket(int i)
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

            packetData[packetStart + 2] = (byte)i;
            packetData[packetStart + 3] = 0;

            packetOffset = packetStart + 3;
            skipOffset = 8;
        }

        public void WritePacket(int i)
        {
            if (HasErrors)
            {
                packetStart = 0;
                packetOffset = 3;
                HasErrors = false;
                throw new IOException(ErrorMessage);
            }

            PacketCount++;

            if (PacketCount < i)
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

        public void AddInt8(int i)
        {
            packetData[packetOffset++] = (byte)i;
        }

        public void AddInt16(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public void AddInt32(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 24);
            packetData[packetOffset++] = (byte)(i >> 16);
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public void AddInt64(long l)
        {
            AddInt32((int)(l >> 32));
            AddInt32((int)(l & -1L));
        }

        public void AddString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            Array.Copy(bytes, 0, packetData, packetOffset, bytes.Length);

            packetOffset += bytes.Length;
        }

        public void AddBytes(byte[] data)
        {
            AddBytes(data, 0, data.Length);
        }

        public void AddBytes(byte[] data, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                packetData[packetOffset++] = data[offset + i];
            }
        }

        public virtual void WriteToBuffer(byte[] abyte0, int i, int j)
        {
        }

        public int ReadInt8()
        {
            return ReadInputStream();
        }

        public int ReadInt16()
        {
            int i = ReadInt8();
            int j = ReadInt8();

            return i * 256 + j;
        }

        public long ReadInt64()
        {
            long q1 = ReadInt16();
            long q2 = ReadInt16();
            long q3 = ReadInt16();
            long q4 = ReadInt16();

            return (q1 << 48) + (q2 << 32) + (q3 << 16) + q4;
        }

        public void Read(int i, sbyte[] abyte0)
        {
            ReadInputStream(i, 0, abyte0);
        }

        public virtual int ReadInputStream()
        {
            return 0;
        }

        public virtual void ReadInputStream(int length, sbyte[] data)
        {
            ReadInputStream(length, 0, data);
        }

        public virtual void ReadInputStream(int length, int offset, sbyte[] data)
        {
        }

        public void FinalisePacket(bool format = true)
        {
            if (format)
            {
                FormatPacket();
            }

            WritePacket(0);
        }

        public virtual int available()
        {
            return 0;
        }

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

        public bool hasData()
        {
            return packetStart > 0;
        }

        public int readPacket(sbyte[] data)
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
            catch (IOException ioexception)
            {
                HasErrors = true;
                ErrorMessage = ioexception.Message;
            }

            return 0;
        }
    }
}
