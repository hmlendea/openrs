using System.Numerics;
using System.Security.Cryptography;

namespace OpenRS.Net.Client.Net
{
    public sealed class LoginEncryptor
    {
        public void AddByte(int i)
            => LoginPacketBuffer.AddByte(this, i);

        public void AddInt(int i)
            => LoginPacketBuffer.AddInt(this, i);

        public void AddString(string s)
            => LoginPacketBuffer.AddString(this, s);

        public void AddBytes(byte[] bytes, int off, int length)
            => LoginPacketBuffer.AddBytes(this, bytes, off, length);

        public int GetByte()
            => LoginPacketBuffer.GetByte(this);

        public int GetShort()
            => LoginPacketBuffer.GetShort(this);

        public int GetInt()
            => LoginPacketBuffer.GetInt(this);

        public void GetBytes(byte[] outputBuffer, int startIndex, int byteCount)
            => LoginPacketBuffer.GetBytes(this, outputBuffer, startIndex, byteCount);

        public byte[] Encrypt(byte[] text) => text;

        public void EncryptPacket(BigInteger key, BigInteger modulus)
            => LoginPacketRsaEncryptor.EncryptPacket(this, key, modulus);

        public LoginEncryptor(byte[] keyBytes)
        {
            packet = keyBytes;
            offset = 0;
            try
            {
                Crypto = new RSACryptoServiceProvider();
                _ = Crypto.ExportParameters(false);
            }
            catch { }
        }

        public byte[] packet;
        public int offset;
        public RSACryptoServiceProvider Crypto;
    }
}