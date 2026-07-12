using System;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;

namespace OpenRS.Net.Client.Net
{
    public sealed class LoginEncryptor
    {
        public void AddByte(int i)
        {
            packet[offset++] = (byte)i;
        }

        public void AddInt(int i)
        {
            packet[offset++] = (byte)(i >> 24);
            packet[offset++] = (byte)(i >> 16);
            packet[offset++] = (byte)(i >> 8);
            packet[offset++] = (byte)i;
        }


        public void AddString(string s)
        {

            byte[] encodedBytes = Encoding.UTF8.GetBytes(s);
            Array.Copy(encodedBytes, 0, packet, offset, encodedBytes.Length);

            //s.GetBytes(0, s.length(), packet, offset);
            offset += encodedBytes.Length;
            packet[offset++] = 10;
        }

        public void AddBytes(byte[] bytes, int off, int length)
        {
            for (int i = off; i < off + length; i += 1)
            {
                packet[this.offset++] = bytes[i];
            }
        }

        public int GetByte()
        {
            return packet[offset++] & 0xff;
        }

        public int GetShort()
        {
            offset += 2;
            return ((packet[offset - 2] & 0xff) << 8) + (packet[offset - 1] & 0xff);
        }

        public int GetInt()
        {
            offset += 4;
            return ((packet[offset - 4] & 0xff) << 24) + ((packet[offset - 3] & 0xff) << 16) + ((packet[offset - 2] & 0xff) << 8) + (packet[offset - 1] & 0xff);
        }

        public void GetBytes(byte[] outputBuffer, int startIndex, int byteCount)
        {
            for (int i = startIndex; i < startIndex + byteCount; i += 1)
            {
                outputBuffer[i] = packet[offset += 1];
            }
        }

        public byte[] Encrypt(byte[] text) {
            byte[] cipherText = null;
        //Cipher cipher = Cipher.getInstance("RSA/ECB/PKCS1Padding");
        //cipher.init(Cipher.ENCRYPT_MODE, pubKey);
        //cipherText = cipher.doFinal(text);

           //return Crypto.Encrypt(text, false);
            return text;

        //return cipherText;
    }

        public void EncryptPacket(BigInteger key, BigInteger modulus)
        {
            int i = offset;
            offset = 0;
            byte[] plain = new byte[i];
            for (int k = 0; k < i; k += 1)
            {
                plain[k] = packet[offset++];
            }
            Array.Reverse(plain);
            // Append 0x00 to ensure BigInteger treats value as positive (two's complement, little-endian)
            byte[] plainUnsigned = new byte[plain.Length + 1];
            Array.Copy(plain, plainUnsigned, plain.Length);
            BigInteger plainInt = new(plainUnsigned);
            BigInteger encInt = BigInteger.ModPow(plainInt, key, modulus);
            byte[] enc = encInt.ToByteArray();
            // Strip trailing 0x00 sign byte if present (ToByteArray is little-endian)
            int encLen = enc.Length;
            if (encLen > 1 && enc[encLen - 1] == 0x00)
            {
                encLen -= 1;
            }
            Array.Reverse(enc, 0, encLen);
            offset = 0;
            packet[offset++] = (byte)encLen;
            for (int k = 0; k < encLen; k += 1)
            {
                packet[offset++] = enc[k];
            }
        }

        public LoginEncryptor(byte[] keyBytes)
        {
            packet = keyBytes;
            offset = 0;
            try
            {
                // keyFactory = KeyFactory.getInstance("RSA");



                Crypto = new RSACryptoServiceProvider();
                pubKey = Crypto.ExportParameters(false);


                //parms.
                //var key =
                //    BigInteger.Parse(
                //        "258483531987721813854435365666199783121097212864526576114955744050873252978581213214062885665119329089273296913884093898593877564098511382732309048889240854054459372263273672334107564088395710980478911359605768175143527864461996266529749955416370971506195317045377519645018157466830930794446490944537605962330090699836840861268493872513762630835769942133970804813091619416385064187784658945")
                //        .ToByteArray();
                //pubKey = keyFactory.generatePublic(new X509EncodedKeySpec());
            }
            catch (Exception e) { }
        }

        public byte[] packet;
        public int offset;
        public RSACryptoServiceProvider Crypto;
        //private KeyFactory keyFactory;
        private RSAParameters pubKey;
    }
}