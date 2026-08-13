namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectShadeDecoder
    {
        private static int AsciiLineFeed => 10;

        private static int AsciiCarriageReturn => 13;

        private static int ByteMaskValue => 0xff;

        private static int ShadeHighByteMultiplier => 4096;

        private static int ShadeMidByteMultiplier => 64;

        private static int ShadeValueDecodingOffset => 0x20000;

        private static int ShadeSpecialEncodedValue => 0x1e240;

        internal static int Decode(GameObject gameObject, sbyte[] buffer)
        {
            SkipLineBreaks(gameObject, buffer);

            int highByte = ReadNextShadeTableEntry(gameObject, buffer);
            int middleByte = ReadNextShadeTableEntry(gameObject, buffer);
            int lowByte = ReadNextShadeTableEntry(gameObject, buffer);
            int decodedValue =
                highByte * ShadeHighByteMultiplier +
                middleByte * ShadeMidByteMultiplier +
                lowByte -
                ShadeValueDecodingOffset;

            if (decodedValue == ShadeSpecialEncodedValue)
            {
                return GameObject.DefaultShadeValue;
            }

            return decodedValue;
        }

        private static void SkipLineBreaks(GameObject gameObject, sbyte[] buffer)
        {
            while (buffer[gameObject.ShadeBufferIndex] == AsciiLineFeed ||
                buffer[gameObject.ShadeBufferIndex] == AsciiCarriageReturn)
            {
                gameObject.ShadeBufferIndex += 1;
            }
        }

        private static int ReadNextShadeTableEntry(GameObject gameObject, sbyte[] buffer)
        {
            int value =
                GameObjectLookupTables.ShadeTable[
                    buffer[gameObject.ShadeBufferIndex] & ByteMaskValue];
            gameObject.ShadeBufferIndex += 1;

            return value;
        }
    }
}