namespace OpenRS.Net.Client.Game
{
    internal static class GameImageSampledSpriteBlitter
    {
        internal static void Draw<TPixelComposer>(
            int[] pixels,
            int[] colours,
            int sourceX,
            int sourceY,
            int destinationOffset,
            int destinationStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int sourceWidth,
            int rowStep,
            TPixelComposer pixelComposer)
            where TPixelComposer : struct, IGameImageSampledPixelComposer
        {
            int initialSourceX = sourceX;

            for (int rowIteration = -height;
                rowIteration < 0;
                rowIteration += rowStep)
            {
                int sourceRowOffset = (sourceY >> 16) * sourceWidth;

                for (int columnIteration = -width;
                    columnIteration < 0;
                    columnIteration += 1)
                {
                    int sourceColour = colours[
                        (sourceX >> 16) + sourceRowOffset];

                    if (sourceColour != 0)
                    {
                        pixels[destinationOffset] = pixelComposer.Compose(
                            sourceColour,
                            pixels,
                            destinationOffset);
                    }

                    destinationOffset += 1;
                    sourceX += xStep;
                }

                sourceY += yStep;
                sourceX = initialSourceX;
                destinationOffset += destinationStride;
            }
        }
    }
}