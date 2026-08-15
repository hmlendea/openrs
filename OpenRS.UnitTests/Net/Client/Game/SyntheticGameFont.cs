using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    internal static class SyntheticGameFont
    {
        private static int FontCount => 9;

        private static int GlyphDataOffset => 1024;

        private static int CharacterEntryCount => 96;

        private static bool isRegistered;

        internal static void EnsureRegistered()
        {
            if (isRegistered)
            {
                return;
            }

            for (int fontIndex = 0; fontIndex < FontCount; fontIndex += 1)
            {
                GameImage.AddFont(BuildFont());
            }

            isRegistered = true;
        }

        private static sbyte[] BuildFont()
        {
            sbyte[] font = new sbyte[GlyphDataOffset + 1];

            for (int characterIndex = 0; characterIndex < CharacterEntryCount; characterIndex += 1)
            {
                int metadataOffset = characterIndex * 9;
                font[metadataOffset] = 0;
                font[metadataOffset + 1] = 8;
                font[metadataOffset + 2] = 0;
                font[metadataOffset + 3] = 1;
                font[metadataOffset + 4] = 1;
                font[metadataOffset + 5] = 0;
                font[metadataOffset + 6] = 0;
                font[metadataOffset + 7] = 1;
                font[metadataOffset + 8] = 12;
            }

            font[GlyphDataOffset] = unchecked((sbyte)255);

            return font;
        }
    }
}