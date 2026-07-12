using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OpenRS.Net.Client.Net
{
    public class PacketConstruction
    {

        public virtual void closeStream()
        {
        }

        public void createPacket(int id)
        {
            if (packetStart > (maxPacketLength * 4) / 5)
                try
                {
                    writePacket(0);
                }
                catch (IOException ioexception)
                {
                    error = true;
                    errorText = ioexception.ToString();//ioexception.getMessage();
                }
            if (packetData == null)
                packetData = new byte[maxPacketLength];
            packetData[packetStart + 2] = (byte)id;
            packetData[packetStart + 3] = 0;
            packetOffset = packetStart + 3;
            skipOffset = 8;
        }

        public void writePacket(int packetId)
        {
            if (error)
            {
                packetStart = 0;
                packetOffset = 3;
                error = false;
                throw new IOException(errorText);
            }
            packetCount++;
            if (packetCount < packetId)
                return;
            if (packetStart > 0)
            {
                packetCount = 0;
                writeToBuffer(packetData, 0, packetStart);
            }
            packetStart = 0;
            packetOffset = 3;
        }

        public void addByte(int i)
        {
            packetData[packetOffset++] = (byte)i;
        }

        public void addString(String s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);

            //s.getBytes(0, s.length(), packetData, packetOffset);

            Array.Copy(bytes, 0, packetData, packetOffset, bytes.Length);

            packetOffset += bytes.Length;//s.length();
        }

        public void addLong(long l)
        {
            addInt((int)(l >> 32));
            addInt((int)(l & -1L));
        }

        public virtual void writeToBuffer(byte[] abyte0, int i, int j)
        {
        }

        public virtual void readInputStream(int i, int j, sbyte[] abyte0)
        {
        }

        public int readShort()
        {
            int i = readByte();
            int j = readByte();
            return i * 256 + j;
        }

        public virtual int read()
        {
            return 0;
        }

        public void read(int i, sbyte[] abyte0)
        {
            readInputStream(i, 0, abyte0);
        }

        public void addInt(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 24);
            packetData[packetOffset++] = (byte)(i >> 16);
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public void flush(bool format = true)
        {
            if (format)
                formatPacket();
            writePacket(0);
        }

        // bad
        //public virtual int available()
        //{
        //    Console.WriteLine("packetconstruction.available WRONG");
        //    return 0;
        //}

        public void addShort(int i)
        {
            packetData[packetOffset++] = (byte)(i >> 8);
            packetData[packetOffset++] = (byte)i;
        }

        public long readLong()
        {
            long l = readShort();
            long l1 = readShort();
            long l2 = readShort();
            long l3 = readShort();
            return (l << 48) + (l1 << 32) + (l2 << 16) + l3;
        }

        public void formatPacket()
        {
            if (skipOffset != 8)
                packetOffset++;
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

            flush(false);
        }

        public void addBytes(byte[] data, int off, int len)
        {
            for (int i = 0; i < len; i++)
                packetData[packetOffset++] = data[off + i];

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
                    int b0 = read() & 0xff;
                    if (b0 < 160)
                    {
                        // compact: b0 = payload length, b1 = last payload byte moved to header
                        int b1 = read() & 0xff;
                        length = b0; // b0 IS the payload length
                        _swappedByte = b1;
                        _hasSwappedByte = true;
                    }
                    else
                    {
                        // extended: length = (b0-160)*256 + b1
                        int b1 = read() & 0xff;
                        length = (b0 - 160) * 256 + b1;
                        _hasSwappedByte = false;
                    }
                }
                if (length > 0 /*&& available() >= length*/)
                {
                    if (_hasSwappedByte)
                    {
                        // read length-1 bytes (cmd + payload without last byte), then append swapped byte
                        read(length - 1, arg0);
                        arg0[length - 1] = (sbyte)_swappedByte;
                        _hasSwappedByte = false;
                    }
                    else
                    {
                        read(length, arg0);
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

        public int readByte()
        {
            return read();
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
        public String errorText;
        public bool error;
    }

}
