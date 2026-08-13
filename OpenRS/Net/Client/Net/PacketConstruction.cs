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

        private readonly PacketConstructionReader packetReader = new();
        private readonly PacketConstructionWriter packetWriter = new();

        private static int DefaultMaximumPacketLength => 5000;

        public PacketConstruction()
        {
            maxPacketLength = DefaultMaximumPacketLength;
            errorText = string.Empty;
            error = false;
        }

        public virtual void CloseStream()
        {
        }

        public void CreatePacket(int id)
            => packetWriter.CreatePacket(this, id);

        public void WritePacket(int packetId)
            => packetWriter.WritePacket(this, packetId);

        public void AddByte(int i) => packetWriter.AddByte(this, i);

        public void AddString(string s)
            => packetWriter.AddString(this, s);

        public void AddLong(long l)
            => packetWriter.AddLong(this, l);

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
            => packetWriter.AddInt(this, i);

        public void Flush(bool format = true)
            => packetWriter.Flush(this, format);

        public void AddShort(int i)
            => packetWriter.AddShort(this, i);

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
            => packetWriter.FormatPacket(this);

        public void AddBytes(byte[] data, int off, int len)
            => packetWriter.AddBytes(this, data, off, len);

        public bool HasData() => packetStart > 0;

        public int ReadPacket(sbyte[] packetBuffer)
            => packetReader.ReadPacket(this, packetBuffer);

        public int ReadByte() => Read();
    }
}
