using System;
using System.Text;
using System.IO;

namespace OpenRS.Net.Client.Net
{
    public class PacketConstruction
    {

        public virtual void CloseStream()
        {
        }

        public void CreatePacket(int id)
        {
            if (packetStart > (maxPacketLength * 4) / 5)
            {
                try
                {
                    WritePacket(0);
                }
                catch (IOException ioexception)
                {
                    error = true;
                    errorText = ioexception.ToString();//ioexception.getMessage();
                }
            }

            if (packetData is null)
            {
                packetData = new byte[maxPacketLength];
            }

            packetData[packetStart + 2] = (byte)id;
            packetData[packetStart + 3] = 0;
            packetOffset = packetStart + 3;
            skipOffset = 8;
        }

        public void WritePacket(int packetId)
        {
            if (error)
            {
                packetStart = 0;
                packetOffset = 3;
                error = false;
                throw new IOException(errorText);
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
            packetOffset = 3;
        }

        public void AddByte(int i)
        {
            packetData[packetOffset++] = (byte)i;
        }

        public void AddString(string s)
        {
            byte[] encodedBytes = Encoding.UTF8.GetBytes(s);

            //s.GetBytes(0, s.length(), packetData, packetOffset);

            Array.Copy(encodedBytes, 0, packetData, packetOffset, encodedBytes.Length);

            packetOffset += encodedBytes.Length;//s.length();
        }

        public void AddLong(long l)
        {
            AddInt((int)(l >> 32));
            AddInt((int)(l & -1L));
        }

        public virtual void WriteToBuffer(byte[] buffer, int offset, int length)
        {
        }

        public virtual void ReadInputStream(int size, int type, sbyte[] buffer)
        {
        }

        public int ReadShort()
        {
            int i = ReadByte();
            int j = ReadByte();
            return i * 256 + j;
        }

        public virtual int Read()
        {
            return 0;
        }

        public void Read(int size, sbyte[] buffer)
        {
            ReadInputStream(size, 0, buffer);
        }

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

        // bad
        //public virtual int available()
        //{
        //    Console.WriteLine("packetconstruction.available WRONG");
        //    return 0;
        //}

        public void AddShort(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public long ReadLong()
        {
            long l = ReadShort();
            long l1 = ReadShort();
            long l2 = ReadShort();
            long l3 = ReadShort();
            return (l << 48) + (l1 << 32) + (l2 << 16) + l3;
        }

        public void FormatPacket()
        {
            if (skipOffset != 8)
            {
                packetOffset += 1;
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
                packetOffset -= 1;
                packetData[packetStart + 1] = packetData[packetOffset];
            }
            if (maxPacketLength <= 10000)
            {
                int k = packetData[packetStart + 2] & 0xff;
                packetCommandCount[k] += 1;
                packetLengthCount[k] += packetOffset - packetStart;
            }
            packetStart = packetOffset;

            Flush(false);
        }

        public void AddBytes(byte[] data, int off, int len)
        {
            for (int i = 0; i < len; i += 1)
            {
                packetData[packetOffset++] = data[off + i];
            }
        }

        public bool HasData()
        {
            return packetStart > 0;
        }

        public int ReadPacket(sbyte[] packetBuffer)
        {
            try
            {
                _read += 1;
                if (maxPacketReadCount > 0 && _read > maxPacketReadCount)
                {
                    error = true;
                    errorText = "time-out";
                    maxPacketReadCount += maxPacketReadCount;
                    return 0;
                }
                if (length == 0 /*&& available() >= 2*/)
                {
                    int b0 = Read() & 0xff;
                    if (b0 < 160)
                    {
                        // compact: b0 = payload length, b1 = last payload byte moved to header
                        int b1 = Read() & 0xff;
                        length = b0; // b0 IS the payload length
                        _swappedByte = b1;
                        _hasSwappedByte = true;
                    }
                    else
                    {
                        // extended: length = (b0-160)*256 + b1
                        int b1 = Read() & 0xff;
                        length = (b0 - 160) * 256 + b1;
                        _hasSwappedByte = false;
                    }
                }
                if (length > 0 /*&& available() >= length*/)
                {
                    if (_hasSwappedByte)
                    {
                        // read length-1 bytes (cmd + payload without last byte), then append swapped byte
                        Read(length - 1, packetBuffer);
                        packetBuffer[length - 1] = (sbyte)_swappedByte;
                        _hasSwappedByte = false;
                    }
                    else
                    {
                        Read(length, packetBuffer);
                    }
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

        public int ReadByte()
        {
            return Read();
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
        private int _swappedByte;
        private bool _hasSwappedByte;
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
