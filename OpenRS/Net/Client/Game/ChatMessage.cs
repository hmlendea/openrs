using System;

namespace OpenRS.Net.Client.Game
{
    public sealed class ChatMessage
    {
        public static byte[] LastChat = new byte[100];

        public static string BytesToString(sbyte[] encodedBytes, int readOffset, int byteCount)
        {
            byte[] convertedBytes = Array.ConvertAll(encodedBytes, signedByte => (byte)signedByte);

            return ChatMessageCodec.Decode(convertedBytes, readOffset, byteCount);
        }

        public static string BytesToString(byte[] encodedBytes, int readOffset, int byteCount)
            => ChatMessageCodec.Decode(encodedBytes, readOffset, byteCount);

        public static int StringToBytes(string message)
            => ChatMessageCodec.Encode(message, LastChat);
    }
}
