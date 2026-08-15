using System;
using System.Numerics;

namespace OpenRS.Net.Client.Net
{
    internal static class LoginPacketRsaEncryptor
    {
        internal static void EncryptPacket(
            LoginEncryptor encryptor,
            BigInteger key,
            BigInteger modulus)
        {
            int plaintextLength = encryptor.offset;
            encryptor.offset = 0;
            byte[] plaintext = new byte[plaintextLength];

            for (int byteIndex = 0;
                byteIndex < plaintextLength;
                byteIndex += 1)
            {
                plaintext[byteIndex] = encryptor.packet[encryptor.offset++];
            }

            Array.Reverse(plaintext);
            byte[] unsignedPlaintext = new byte[plaintext.Length + 1];
            Array.Copy(plaintext, unsignedPlaintext, plaintext.Length);
            BigInteger plaintextValue = new(unsignedPlaintext);
            BigInteger encryptedValue = BigInteger.ModPow(
                plaintextValue,
                key,
                modulus);
            byte[] encryptedBytes = encryptedValue.ToByteArray();
            int encryptedLength = encryptedBytes.Length;

            if (encryptedLength > 1 && encryptedBytes[encryptedLength - 1] == 0)
            {
                encryptedLength -= 1;
            }

            Array.Reverse(encryptedBytes, 0, encryptedLength);
            encryptor.offset = 0;
            encryptor.packet[encryptor.offset++] = (byte)encryptedLength;

            for (int byteIndex = 0;
                byteIndex < encryptedLength;
                byteIndex += 1)
            {
                encryptor.packet[encryptor.offset++] = encryptedBytes[byteIndex];
            }
        }
    }
}