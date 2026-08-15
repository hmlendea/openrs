namespace OpenRS.Net.Client.Game
{
    internal static class GameImageDirectColourResolver
    {
        internal static int[] Resolve(
            sbyte[] colourIndexes,
            int[] palette,
            int pixelCount)
        {
            int[] resolvedColours = new int[pixelCount];

            for (int pixelIndex = 0;
                pixelIndex < pixelCount;
                pixelIndex += 1)
            {
                int colour = palette[colourIndexes[pixelIndex] & 0xff];

                if (colour == 0)
                {
                    colour = 1;
                }
                else if (colour == GameImagePaletteConverter.TransparentColour)
                {
                    colour = 0;
                }

                resolvedColours[pixelIndex] = colour;
            }

            return resolvedColours;
        }
    }
}