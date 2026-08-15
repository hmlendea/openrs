using System;

namespace OpenRS.Net.Client.Game
{
    internal static class ChatMessageNibbleAlphabet
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

        internal static int SingleNibbleThreshold => 13;

        internal static int DoubleNibbleOffset => 195;

        internal static char GetCharacter(int characterCode)
            => ValidCharacters[characterCode];

        internal static int GetCharacterCode(char character)
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