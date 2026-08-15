namespace OpenRS.Net.Client.Game
{
    internal static class GameImageScaledTintedSpriteBlitter
    {
        internal static void Draw<TColourSource, TPixelComposer>(
            int[] pixels,
            TColourSource colourSource,
            int sourceX,
            int sourceY,
            int destinationOffset,
            int width,
            int height,
            int xStep,
            int yStep,
            int sourceWidth,
            int xPositionFixed,
            int xPositionStep,
            int interlaceFlag,
            int imageX,
            int imageWidth,
            int gameWidth,
            TPixelComposer pixelComposer)
            where TColourSource : struct, IGameImageSpriteColourSource
            where TPixelComposer : struct, IGameImageSampledPixelComposer
        {
            for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
            {
                int sourceRowOffset = (sourceY >> 16) * sourceWidth;
                GameImageScaledScanline scanline =
                    GameImageScaledScanline.Calculate(
                        sourceX,
                        xPositionFixed,
                        width,
                        xStep,
                        imageX,
                        imageWidth);

                interlaceFlag = 1 - interlaceFlag;

                if (interlaceFlag != 0)
                {
                    int sampleX = scanline.SourceX;

                    for (int pixelX = scanline.StartX;
                        pixelX < scanline.EndX;
                        pixelX += 1)
                    {
                        GameImageSpriteSample sample = colourSource.Read(
                            (sampleX >> 16) + sourceRowOffset);

                        if (sample.IsVisible)
                        {
                            int targetOffset = pixelX + destinationOffset;
                            pixels[targetOffset] = pixelComposer.Compose(
                                sample.Colour,
                                pixels,
                                targetOffset);
                        }

                        sampleX += xStep;
                    }
                }

                sourceY += yStep;
                destinationOffset += gameWidth;
                xPositionFixed += xPositionStep;
            }
        }
    }
}