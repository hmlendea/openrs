namespace OpenRS.Net.Client.Game
{
    internal static class GameImageIndexedPaletteBuilder
    {
        private static int Colour15BitTableSize => 32768;

        private static int MaximumPaletteSize => 256;

        private static int InitialDistanceSquared => 0x3b9ac9ff;

        private static int ColourChannelBias => 0x40404;

        private static int UnmappedPaletteEntry => -1;

        internal static GameImageIndexedPalette Build(
            int[] sourceColours,
            int pixelCount)
        {
            int[] colourFrequencies = CountColourFrequencies(
                sourceColours,
                pixelCount);
            int[] topPalette = BuildTopPalette(colourFrequencies);
            sbyte[] colourIndexes = BuildColourIndexBuffer(
                sourceColours,
                pixelCount,
                colourFrequencies,
                topPalette);

            return new GameImageIndexedPalette(colourIndexes, topPalette);
        }

        private static int[] CountColourFrequencies(
            int[] sourceColours,
            int pixelCount)
        {
            int[] colourFrequencies = new int[Colour15BitTableSize];

            for (int pixelIndex = 0;
                pixelIndex < pixelCount;
                pixelIndex += 1)
            {
                int quantisedIndex = QuantiseColour(sourceColours[pixelIndex]);
                colourFrequencies[quantisedIndex] += 1;
            }

            return colourFrequencies;
        }

        private static int[] BuildTopPalette(int[] colourFrequencies)
        {
            int[] topPalette = new int[MaximumPaletteSize];
            topPalette[0] = GameImagePaletteConverter.TransparentColour;
            int[] topPaletteCounts = new int[MaximumPaletteSize];

            for (int quantisedIndex = 0;
                quantisedIndex < Colour15BitTableSize;
                quantisedIndex += 1)
            {
                int frequency = colourFrequencies[quantisedIndex];

                if (frequency > topPaletteCounts[MaximumPaletteSize - 1])
                {
                    InsertPaletteEntry(
                        topPalette,
                        topPaletteCounts,
                        quantisedIndex,
                        frequency);
                }

                colourFrequencies[quantisedIndex] = UnmappedPaletteEntry;
            }

            return topPalette;
        }

        private static void InsertPaletteEntry(
            int[] topPalette,
            int[] topPaletteCounts,
            int quantisedIndex,
            int frequency)
        {
            for (int paletteSlot = 1;
                paletteSlot < MaximumPaletteSize;
                paletteSlot += 1)
            {
                if (frequency <= topPaletteCounts[paletteSlot])
                {
                    continue;
                }

                for (int shiftIndex = MaximumPaletteSize - 1;
                    shiftIndex > paletteSlot;
                    shiftIndex -= 1)
                {
                    topPalette[shiftIndex] = topPalette[shiftIndex - 1];
                    topPaletteCounts[shiftIndex] =
                        topPaletteCounts[shiftIndex - 1];
                }

                topPalette[paletteSlot] =
                    ((quantisedIndex & 0x7c00) << 9) +
                    ((quantisedIndex & 0x3e0) << 6) +
                    ((quantisedIndex & 0x1f) << 3) +
                    ColourChannelBias;
                topPaletteCounts[paletteSlot] = frequency;
                break;
            }
        }

        private static sbyte[] BuildColourIndexBuffer(
            int[] sourceColours,
            int pixelCount,
            int[] colourFrequencies,
            int[] topPalette)
        {
            sbyte[] colourIndexBuffer = new sbyte[pixelCount];

            for (int pixelIndex = 0;
                pixelIndex < pixelCount;
                pixelIndex += 1)
            {
                int colour = sourceColours[pixelIndex];
                int quantisedIndex = QuantiseColour(colour);
                int nearestPaletteEntry = colourFrequencies[quantisedIndex];

                if (nearestPaletteEntry == UnmappedPaletteEntry)
                {
                    nearestPaletteEntry = FindNearestPaletteEntry(
                        colour,
                        topPalette);
                    colourFrequencies[quantisedIndex] = nearestPaletteEntry;
                }

                colourIndexBuffer[pixelIndex] = (sbyte)nearestPaletteEntry;
            }

            return colourIndexBuffer;
        }

        private static int FindNearestPaletteEntry(
            int colour,
            int[] topPalette)
        {
            int minimumDistanceSquared = InitialDistanceSquared;
            int nearestPaletteEntry = UnmappedPaletteEntry;
            int blueChannel = colour >> 16 & 0xff;
            int greenChannel = colour >> 8 & 0xff;
            int redChannel = colour & 0xff;

            for (int paletteIndex = 0;
                paletteIndex < MaximumPaletteSize;
                paletteIndex += 1)
            {
                int paletteColour = topPalette[paletteIndex];
                int paletteBlue = paletteColour >> 16 & 0xff;
                int paletteGreen = paletteColour >> 8 & 0xff;
                int paletteRed = paletteColour & 0xff;
                int distanceSquared =
                    (blueChannel - paletteBlue) *
                    (blueChannel - paletteBlue) +
                    (greenChannel - paletteGreen) *
                    (greenChannel - paletteGreen) +
                    (redChannel - paletteRed) *
                    (redChannel - paletteRed);

                if (distanceSquared < minimumDistanceSquared)
                {
                    minimumDistanceSquared = distanceSquared;
                    nearestPaletteEntry = paletteIndex;
                }
            }

            return nearestPaletteEntry;
        }

        private static int QuantiseColour(int colour)
            => ((colour & 0xf80000) >> 9) +
                ((colour & 0xf800) >> 6) +
                ((colour & 0xf8) >> 3);
    }
}