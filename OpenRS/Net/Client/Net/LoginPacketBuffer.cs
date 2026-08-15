using System;
using System.Text;

namespace OpenRS.Net.Client.Net
{
    internal static class LoginPacketBuffer
    {
        private static byte StringTerminator => 10;

        internal static void AddByte(LoginEncryptor encryptor, int value)
            => encryptor.packet[encryptor.offset++] = (byte)value;

        internal static void AddInt(LoginEncryptor encryptor, int value)
        {
            encryptor.packet[encryptor.offset++] = (byte)(value >> 24);
            encryptor.packet[encryptor.offset++] = (byte)(value >> 16);
            encryptor.packet[encryptor.offset++] = (byte)(value >> 8);
            encryptor.packet[encryptor.offset++] = (byte)value;
        }

        internal static void AddString(LoginEncryptor encryptor, string text)
        {
            byte[] encodedBytes = Encoding.UTF8.GetBytes(text);
            Array.Copy(
                encodedBytes,
                0,
                encryptor.packet,
                encryptor.offset,
                encodedBytes.Length);
            encryptor.offset += encodedBytes.Length;
            encryptor.packet[encryptor.offset++] = StringTerminator;
        }

        internal static void AddBytes(
            LoginEncryptor encryptor,
            byte[] bytes,
            int offset,
            int length)
        {
            for (int byteIndex = offset;
                byteIndex < offset + length;
                byteIndex += 1)
            {
                encryptor.packet[encryptor.offset++] = bytes[byteIndex];
            }
        }

        internal static int GetByte(LoginEncryptor encryptor)
            => encryptor.packet[encryptor.offset++] & 0xff;

        internal static int GetShort(LoginEncryptor encryptor)
        {
            encryptor.offset += 2;

            return ((encryptor.packet[encryptor.offset - 2] & 0xff) << 8) +
                (encryptor.packet[encryptor.offset - 1] & 0xff);
        }

        internal static int GetInt(LoginEncryptor encryptor)
        {
            encryptor.offset += 4;

            return ((encryptor.packet[encryptor.offset - 4] & 0xff) << 24) +
                ((encryptor.packet[encryptor.offset - 3] & 0xff) << 16) +
                ((encryptor.packet[encryptor.offset - 2] & 0xff) << 8) +
                (encryptor.packet[encryptor.offset - 1] & 0xff);
        }

        internal static void GetBytes(
            LoginEncryptor encryptor,
            byte[] outputBuffer,
            int startIndex,
            int byteCount)
        {
            for (int outputIndex = startIndex;
                outputIndex < startIndex + byteCount;
                outputIndex += 1)
            {
                outputBuffer[outputIndex] = encryptor.packet[encryptor.offset];
                encryptor.offset += 1;
            }
        }
    }
}