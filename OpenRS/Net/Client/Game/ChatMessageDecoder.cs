using System;

namespace OpenRS.Net.Client.Game
{
    internal static class ChatMessageDecoder
    {
        private static readonly char[] DecodedMessageBuffer = new char[100];

        private static int NibbleMask => 0xf;

        private static int ByteMask => 0xff;

        private static int MaximumColourTagLength => 5;

        internal static string Decode(
            byte[] encodedBytes,
            int readOffset,
            int byteCount)
        {
            try
            {
                int outputLength = DecodeCharacters(
                    encodedBytes,
                    readOffset,
                    byteCount);
                NormaliseDecodedCharacters(outputLength);

                return new string(DecodedMessageBuffer, 0, outputLength);
            }
            catch (Exception)
            {
                return ".";
            }
        }

        private static int DecodeCharacters(
            byte[] encodedBytes,
            int readOffset,
            int byteCount)
        {
            int outputIndex = 0;
            int pendingNibble = -1;

            for (int byteIndex = 0; byteIndex < byteCount; byteIndex += 1)
            {
                readOffset += 1;
                int currentByte = encodedBytes[readOffset] & ByteMask;
                int upperNibble = currentByte >> 4 & NibbleMask;
                DecodeNibble(upperNibble, ref pendingNibble, ref outputIndex);
                int lowerNibble = currentByte & NibbleMask;
                DecodeNibble(lowerNibble, ref pendingNibble, ref outputIndex);
            }

            return outputIndex;
        }

        private static void DecodeNibble(
            int nibble,
            ref int pendingNibble,
            ref int outputIndex)
        {
            if (pendingNibble == -1)
            {
                if (nibble < ChatMessageNibbleAlphabet.SingleNibbleThreshold)
                {
                    DecodedMessageBuffer[outputIndex++] =
                        ChatMessageNibbleAlphabet.GetCharacter(nibble);
                }
                else
                {
                    pendingNibble = nibble;
                }

                return;
            }

            int characterCode =
                (pendingNibble << 4) + nibble -
                ChatMessageNibbleAlphabet.DoubleNibbleOffset;
            DecodedMessageBuffer[outputIndex++] =
                ChatMessageNibbleAlphabet.GetCharacter(characterCode);
            pendingNibble = -1;
        }

        private static void NormaliseDecodedCharacters(int outputLength)
        {
            bool isSentenceStart = true;

            for (int characterIndex = 0;
                characterIndex < outputLength;
                characterIndex += 1)
            {
                char currentCharacter = DecodedMessageBuffer[characterIndex];

                if (characterIndex >= MaximumColourTagLength &&
                    currentCharacter == '@')
                {
                    DecodedMessageBuffer[characterIndex] = ' ';
                }

                if (currentCharacter == '%')
                {
                    DecodedMessageBuffer[characterIndex] = ' ';
                }

                if (isSentenceStart &&
                    currentCharacter >= 'a' &&
                    currentCharacter <= 'z')
                {
                    DecodedMessageBuffer[characterIndex] += '\uFFE0';
                    isSentenceStart = false;
                }

                if (currentCharacter == '.' || currentCharacter == '!')
                {
                    isSentenceStart = true;
                }
            }
        }
    }
}