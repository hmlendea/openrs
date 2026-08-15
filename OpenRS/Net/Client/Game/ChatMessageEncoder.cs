namespace OpenRS.Net.Client.Game
{
    internal static class ChatMessageEncoder
    {
        private static int MaximumMessageLength => 80;

        private static int NibbleMask => 0xf;

        internal static int Encode(string message, byte[] outputBuffer)
        {
            if (message.Length > MaximumMessageLength)
            {
                message = message[..MaximumMessageLength];
            }

            message = message.ToLower();
            int outputIndex = 0;
            int pendingNibble = -1;

            for (int characterIndex = 0;
                characterIndex < message.Length;
                characterIndex += 1)
            {
                int characterCode =
                    ChatMessageNibbleAlphabet.GetCharacterCode(
                        message[characterIndex]);

                if (pendingNibble == -1)
                {
                    if (characterCode <
                        ChatMessageNibbleAlphabet.SingleNibbleThreshold)
                    {
                        pendingNibble = characterCode;
                    }
                    else
                    {
                        outputBuffer[outputIndex++] = (byte)characterCode;
                    }
                }
                else if (characterCode <
                    ChatMessageNibbleAlphabet.SingleNibbleThreshold)
                {
                    outputBuffer[outputIndex++] =
                        (byte)((pendingNibble << 4) + characterCode);
                    pendingNibble = -1;
                }
                else
                {
                    outputBuffer[outputIndex++] =
                        (byte)((pendingNibble << 4) + (characterCode >> 4));
                    pendingNibble = characterCode & NibbleMask;
                }
            }

            if (pendingNibble != -1)
            {
                outputBuffer[outputIndex++] = (byte)(pendingNibble << 4);
            }

            return outputIndex;
        }
    }
}