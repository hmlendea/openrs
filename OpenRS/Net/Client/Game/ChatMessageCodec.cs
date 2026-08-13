using System;

namespace OpenRS.Net.Client.Game
{
    internal static class ChatMessageCodec
    {
        private static readonly char[] ValidCharacters =
        [
            ' ', 'e', 't', 'a', 'o', 'i', 'h', 'n', 's', 'r',
            'd', 'l', 'u', 'm', 'w', 'c', 'y', 'f', 'g', 'p',
            'b', 'v', 'k', 'x', 'j', 'q', 'z', '0', '1', '2',
            '3', '4', '5', '6', '7', '8', '9', ' ', '!', '?',
            '.', ',', ':', ';', '(', ')', '-', '&', '*', '\\',
            '\'', '@', '#', '+', '=', '§', '$', '%', '"', '[',
            ']'
        ];

        private static readonly char[] DecodedMessageBuffer = new char[100];

        private static int MaximumMessageLength => 80;

        private static int SingleNibbleThreshold => 13;

        private static int DoubleNibbleOffset => 195;

        private static int NibbleMask => 0xf;

        private static int ByteMask => 0xff;

        private static int MaximumColourTagLength => 5;

        internal static string Decode(byte[] encodedBytes, int readOffset, int byteCount)
        {
            try
            {
                int outputLength = DecodeCharacters(encodedBytes, readOffset, byteCount);
                NormaliseDecodedCharacters(outputLength);

                return new string(DecodedMessageBuffer, 0, outputLength);
            }
            catch (Exception)
            {
                return ".";
            }
        }

        internal static int Encode(string message, byte[] outputBuffer)
        {
            if (message.Length > MaximumMessageLength)
            {
                message = message[..MaximumMessageLength];
            }

            message = message.ToLower();
            int outputIndex = 0;
            int pendingNibble = -1;

            for (int characterIndex = 0; characterIndex < message.Length; characterIndex += 1)
            {
                int characterCode = GetCharacterCode(message[characterIndex]);

                if (pendingNibble == -1)
                {
                    if (characterCode < SingleNibbleThreshold)
                    {
                        pendingNibble = characterCode;
                    }
                    else
                    {
                        outputBuffer[outputIndex++] = (byte)characterCode;
                    }
                }
                else if (characterCode < SingleNibbleThreshold)
                {
                    outputBuffer[outputIndex++] = (byte)((pendingNibble << 4) + characterCode);
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

        private static int DecodeCharacters(byte[] encodedBytes, int readOffset, int byteCount)
        {
            int outputIndex = 0;
            int pendingNibble = -1;

            for (int byteIndex = 0; byteIndex < byteCount; byteIndex += 1)
            {
                readOffset += 1;
                int currentByte = encodedBytes[readOffset] & ByteMask;
                int upperNibble = currentByte >> 4 & NibbleMask;

                if (pendingNibble == -1)
                {
                    if (upperNibble < SingleNibbleThreshold)
                    {
                        DecodedMessageBuffer[outputIndex++] = ValidCharacters[upperNibble];
                    }
                    else
                    {
                        pendingNibble = upperNibble;
                    }
                }
                else
                {
                    int characterCode =
                        (pendingNibble << 4) + upperNibble - DoubleNibbleOffset;
                    DecodedMessageBuffer[outputIndex++] = ValidCharacters[characterCode];
                    pendingNibble = -1;
                }

                int lowerNibble = currentByte & NibbleMask;

                if (pendingNibble == -1)
                {
                    if (lowerNibble < SingleNibbleThreshold)
                    {
                        DecodedMessageBuffer[outputIndex++] = ValidCharacters[lowerNibble];
                    }
                    else
                    {
                        pendingNibble = lowerNibble;
                    }
                }
                else
                {
                    int characterCode =
                        (pendingNibble << 4) + lowerNibble - DoubleNibbleOffset;
                    DecodedMessageBuffer[outputIndex++] = ValidCharacters[characterCode];
                    pendingNibble = -1;
                }
            }

            return outputIndex;
        }

        private static void NormaliseDecodedCharacters(int outputLength)
        {
            bool isSentenceStart = true;

            for (int characterIndex = 0; characterIndex < outputLength; characterIndex += 1)
            {
                char currentCharacter = DecodedMessageBuffer[characterIndex];

                if (characterIndex >= MaximumColourTagLength && currentCharacter == '@')
                {
                    DecodedMessageBuffer[characterIndex] = ' ';
                }

                if (currentCharacter == '%')
                {
                    DecodedMessageBuffer[characterIndex] = ' ';
                }

                if (isSentenceStart && currentCharacter >= 'a' && currentCharacter <= 'z')
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

        private static int GetCharacterCode(char character)
        {
            int characterCode = Array.IndexOf(ValidCharacters, character);

            if (characterCode < 0)
            {
                characterCode = 0;
            }

            if (characterCode >= SingleNibbleThreshold)
            {
                characterCode += DoubleNibbleOffset;
            }

            return characterCode;
        }
    }
}