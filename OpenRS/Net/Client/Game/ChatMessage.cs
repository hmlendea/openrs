using System;

namespace OpenRS.Net.Client.Game
{

public sealed class ChatMessage
{
    private static readonly char[] validChars =
    [
        ' ', 'e', 't', 'a', 'o', 'i', 'h', 'n', 's', 'r',
        'd', 'l', 'u', 'm', 'w', 'c', 'y', 'f', 'g', 'p',
        'b', 'v', 'k', 'x', 'j', 'q', 'z', '0', '1', '2',
        '3', '4', '5', '6', '7', '8', '9', ' ', '!', '?',
        '.', ',', ':', ';', '(', ')', '-', '&', '*', '\\',
        '\'', '@', '#', '+', '=', '§', '$', '%', '"', '[',
        ']'
    ];

    public static byte[] LastChat = new byte[100];
    private static char[] decodedMessageBuffer = new char[100];

    private static int MaxMessageLength => 80;

    private static int SingleNibbleThreshold => 13;

    private static int DoubleNibbleOffset => 195;

    private static int NibbleMask => 0xf;

    private static int ByteMask => 0xff;

    private static int MaxColourTagLength => 5;

    public static string BytesToString(sbyte[] encodedBytes, int readOffset, int byteCount)
    {
        byte[] convertedBytes = Array.ConvertAll(encodedBytes, signedByte => (byte)signedByte);

        return BytesToString(convertedBytes, readOffset, byteCount);
    }

    public static string BytesToString(byte[] encodedBytes, int readOffset, int byteCount)
    {
        try
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
                        decodedMessageBuffer[outputIndex++] = validChars[upperNibble];
                    }
                    else
                    {
                        pendingNibble = upperNibble;
                    }
                }
                else
                {
                    int encodedUpperChar = (pendingNibble << 4) + upperNibble - DoubleNibbleOffset;
                    decodedMessageBuffer[outputIndex++] = validChars[encodedUpperChar];
                    pendingNibble = -1;
                }

                int lowerNibble = currentByte & NibbleMask;

                if (pendingNibble == -1)
                {
                    if (lowerNibble < SingleNibbleThreshold)
                    {
                        decodedMessageBuffer[outputIndex++] = validChars[lowerNibble];
                    }
                    else
                    {
                        pendingNibble = lowerNibble;
                    }
                }
                else
                {
                    int encodedLowerChar = (pendingNibble << 4) + lowerNibble - DoubleNibbleOffset;
                    decodedMessageBuffer[outputIndex++] = validChars[encodedLowerChar];
                    pendingNibble = -1;
                }
            }

            bool isSentenceStart = true;

            for (int charIndex = 0; charIndex < outputIndex; charIndex += 1)
            {
                char currentChar = decodedMessageBuffer[charIndex];

                if (charIndex >= MaxColourTagLength && currentChar == '@')
                {
                    decodedMessageBuffer[charIndex] = ' ';
                }
                if (currentChar == '%')
                {
                    decodedMessageBuffer[charIndex] = ' ';
                }
                if (isSentenceStart &&
                    currentChar >= 'a' &&
                    currentChar <= 'z')
                {
                    decodedMessageBuffer[charIndex] += '\uFFE0';
                    isSentenceStart = false;
                }
                if (currentChar == '.' || currentChar == '!')
                {
                    isSentenceStart = true;
                }
            }

            return new string(decodedMessageBuffer, 0, outputIndex);
        }
        catch (Exception)
        {
            return ".";
        }
    }

    public static int StringToBytes(string message)
    {
        if (message.Length > MaxMessageLength)
        {
            message = message[..MaxMessageLength];
        }

        message = message.ToLower();
        int outputIndex = 0;
        int pendingNibble = -1;

        for (int charIndex = 0; charIndex < message.Length; charIndex += 1)
        {
            char currentChar = message[charIndex];
            int charCode = Array.IndexOf(validChars, currentChar);

            if (charCode < 0)
            {
                charCode = 0;
            }
            if (charCode >= SingleNibbleThreshold)
            {
                charCode += DoubleNibbleOffset;
            }
            if (pendingNibble == -1)
            {
                if (charCode < SingleNibbleThreshold)
                {
                    pendingNibble = charCode;
                }
                else
                {
                    LastChat[outputIndex++] = (byte)charCode;
                }
            }
            else if (charCode < SingleNibbleThreshold)
            {
                LastChat[outputIndex++] = (byte)((pendingNibble << 4) + charCode);
                pendingNibble = -1;
            }
            else
            {
                LastChat[outputIndex++] = (byte)((pendingNibble << 4) + (charCode >> 4));
                pendingNibble = charCode & NibbleMask;
            }
        }
        if (pendingNibble != -1)
        {
            LastChat[outputIndex++] = (byte)(pendingNibble << 4);
        }

        return outputIndex;
    }
}

}
