using System;
using System.Text;
using System.IO;

namespace RuneScapeSolo.Lib.Net
{
    public class PacketConstruction
    {

        public virtual void closeStream()
        {
        }

        public void CreatePacket(int i)
        {
            if (packetStart > (maxPacketLength * 4) / 5)
            {
                try
                {
                    WritePacket(0);
                }
                catch (IOException ex)
                {
                    error = true;
                    errorText = ex.Message;
                }
            }

            if (packetData == null)
            {
                packetData = new byte[maxPacketLength];
            }

            packetData[packetStart + 2] = (byte)i;
            packetData[packetStart + 3] = 0;

            packetOffset = packetStart + 3;
            skipOffset = 8;
        }

        public void WritePacket(int i)
        {
            if (error)
            {
                packetStart = 0;
                packetOffset = 3;
                error = false;
                throw new IOException(errorText);
            }

            packetCount++;

            if (packetCount < i)
            {
                return;
            }
            if (packetStart > 0)
            {
                packetCount = 0;
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

        public void AddString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);

            Array.Copy(bytes, 0, packetData, packetOffset, bytes.Length);

            packetOffset += bytes.Length;
        }

        public void AddInt64(long l)
        {
            addInt((int)(l >> 32));
            addInt((int)(l & -1L));
        }

        public virtual void WriteToBuffer(byte[] abyte0, int i, int j)
        {
        }

        public virtual void ReadInputStream(int i, int j, sbyte[] abyte0)
        {
        }

        public int ReadInt16()
        {
            int i = ReadInt8();
            int j = ReadInt8();
            return i * 256 + j;
        }

        public virtual int ReadInputStream()
        {
            return 0;
        }

        public void Read(int i, sbyte[] abyte0)
        {
            ReadInputStream(i, 0, abyte0);
        }

        public void addInt(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 24);
            packetData[packetOffset++] = (byte)(i >> 16);
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public void FinalisePacket(bool format = true)
        {
            if (format)
            {
                FormatPacket();
            }

            WritePacket(0);
        }

        // bad
        //public virtual int available()
        //{
        //    Console.WriteLine("packetconstruction.available WRONG");
        //    return 0;
        //}

        public void SendInt16(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public long ReadInt64()
        {
            long q1 = ReadInt16();
            long q2 = ReadInt16();
            long q3 = ReadInt16();
            long q4 = ReadInt16();

            return (q1 << 48) + (q2 << 32) + (q3 << 16) + q4;
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
            if (maxPacketLength <= 10000)
            {
                int k = packetData[packetStart + 2] & 0xff;

                packetCommandCount[k]++;
                packetLengthCount[k] += packetOffset - packetStart;
            }

            packetStart = packetOffset;
        }

        public void AddBytes(byte[] data, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                packetData[packetOffset++] = data[offset + i];
            }
        }

        public bool hasData()
        {
            return packetStart > 0;
        }

        public int readPacket(sbyte[] arg0)
        {
            try
            {
                _read++;
                if (maxPacketReadCount > 0 && _read > maxPacketReadCount)
                {
                    error = true;
                    errorText = "time-out";
                    maxPacketReadCount += maxPacketReadCount;
                    return 0;
                }
                if (length == 0 /*&& available() >= 2*/)
                {
                    sbyte[] buf = new sbyte[2];
                    ReadInputStream(2, 0, buf);
                    length = ((short)((buf[0] & 0xff) << 8) | (short)(buf[1] & 0xff)) + 1;
                }
                if (length > 0 /*&& available() >= length*/)
                {
                    Read(length, arg0);
                    int i = length;
                    length = 0;
                    _read = 0;
                    return i;
                }
            }
            catch (IOException ioexception)
            {
                error = true;
                errorText = ioexception.ToString();//ioexception.getMessage();
            }
            return 0;
        }

        public int ReadInt8()
        {
            return ReadInputStream();
        }

        public PacketConstruction()
        {
            packetOffset = 3;
            skipOffset = 8;
            maxPacketLength = 5000;
            errorText = "";
            error = false;
        }

        public int length;
        public int _read;
        public int maxPacketReadCount;
        public int packetStart;
        private int packetOffset;
        private int skipOffset;
        public byte[] packetData;
        public static int[] packetCommandCount = new int[256];
        public int maxPacketLength;
        public static int[] packetLengthCount = new int[256];
        public int packetCount;
        public string errorText;
        public bool error;
    }

}
