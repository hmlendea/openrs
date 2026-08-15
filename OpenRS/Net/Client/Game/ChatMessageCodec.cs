namespace OpenRS.Net.Client.Game
{
    internal static class ChatMessageCodec
    {
        internal static string Decode(byte[] encodedBytes, int readOffset, int byteCount)
            => ChatMessageDecoder.Decode(encodedBytes, readOffset, byteCount);

        internal static int Encode(string message, byte[] outputBuffer)
            => ChatMessageEncoder.Encode(message, outputBuffer);
    }
}