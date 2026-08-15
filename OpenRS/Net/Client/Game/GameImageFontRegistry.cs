namespace OpenRS.Net.Client.Game
{
    internal static class GameImageFontRegistry
    {
        private static int MaxFontCount => 50;

        private static int CharacterTableSize => 256;

        private static int FontEntryStride => 9;

        private static int FontFallbackIndex => 74;

        private static int MaxFontShadowCount => 12;

        private static readonly sbyte[][] gameFonts = new sbyte[MaxFontCount][];
        private static readonly int[] characterFontOffsetTable;
        private static readonly bool[] fontShadowEnabled = new bool[MaxFontShadowCount];
        private static int currentFont;

        static GameImageFontRegistry()
        {
            string characterSet =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                "0123456789!\"!$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
            characterFontOffsetTable = new int[CharacterTableSize];

            for (int characterIndex = 0;
                characterIndex < CharacterTableSize;
                characterIndex += 1)
            {
                int setIndex = characterSet.IndexOf((char)characterIndex);

                if (setIndex == -1)
                {
                    setIndex = FontFallbackIndex;
                }

                characterFontOffsetTable[characterIndex] =
                    setIndex * FontEntryStride;
            }
        }

        internal static int AddFont(sbyte[] bytes)
        {
            gameFonts[currentFont] = bytes;
            currentFont += 1;

            return currentFont - 1;
        }

        internal static sbyte[] GetFont(int fontIndex)
            => gameFonts[fontIndex];

        internal static int GetCharacterOffset(char character)
            => characterFontOffsetTable[character];

        internal static bool IsShadowEnabled(int fontIndex)
            => fontShadowEnabled[fontIndex];
    }
}