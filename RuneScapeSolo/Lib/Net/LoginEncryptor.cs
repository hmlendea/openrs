using System;
using System.Numerics;
using System.Text;

namespace RuneScapeSolo
{
    public class LoginEncryptor
    {
        public byte[] Packet { get; private set; }
        public int Offset { get; private set; }

        public LoginEncryptor(byte[] packet)
        {
            Packet = packet;
            Offset = 0;
        }

        public void AddInt8(int value)
        {
            Packet[Offset++] = (byte)value;
        }

        public void AddInt32(int value)
        {
            Packet[Offset++] = (byte)(value >> 24);
            Packet[Offset++] = (byte)(value >> 16);
            Packet[Offset++] = (byte)(value >> 8);
            Packet[Offset++] = (byte)value;
        }

        public void AddString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            Array.Copy(bytes, 0, Packet, Offset, bytes.Length);

            Offset += str.Length;

            Packet[Offset++] = 10;
        }

        public void AddBytes(byte[] bytes, int index, int length)
        {
            for (int i = index; i < index + length; i++)
            {
                Packet[Offset++] = bytes[i];
            }
        }

        public int GetInt8()
        {
            return Packet[Offset++] & 0xFF;
        }

        public int GetInt16()
        {
            Offset += 2;

            return ((Packet[Offset - 2] & 0xFF) << 8) +
                   (Packet[Offset - 1] & 0xFF);
        }

        public int GetInt32()
        {
            Offset += 4;

            return ((Packet[Offset - 4] & 0xFF) << 24) +
                   ((Packet[Offset - 3] & 0xFF) << 16) +
                   ((Packet[Offset - 2] & 0xFF) << 8) +
                   (Packet[Offset - 1] & 0xFF);
        }

        public void GetBytes(byte[] data, int index, int length)
        {
            for (int i = index; i < index + length; i++)
            {
                data[i] = Packet[Offset++];
            }
        }

        public void EncryptPacket(BigInteger bigInt, BigInteger bigInt2)
        {
            int i = Offset;

            Offset = 0;

            byte[] dummyPacket = new byte[i];
            GetBytes(dummyPacket, 0, i);

            Array.Reverse(dummyPacket);

            BigInteger bigInt3 = new BigInteger(dummyPacket);
            bigInt3 = BigInteger.ModPow(bigInt3, bigInt, bigInt2);

            byte[] encryptedPacket = bigInt3.ToByteArray();

            Array.Reverse(encryptedPacket);

            Offset = 0;

            AddInt8(encryptedPacket.Length);
            AddBytes(encryptedPacket, 0, encryptedPacket.Length);
        }
    }
}
