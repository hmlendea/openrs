using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraTexture64SpanDrawer
    {
        internal static int TextureRowMask => 0xfc0;

        private static int TextureCoordinateWrapMask => 0xfff;

        private static int ShadingBandMask => 0xc0000;

        private static int ShadingLightShift => 20;

        internal static void Draw<TPixelComposer>(
            int[] pixels,
            int[] scanlines,
            int colour,
            int startScanline,
            int endScanline,
            int startPixelX,
            int destinationOffset,
            int scanlineCount,
            int scanlineStep,
            int interlaceMode,
            int pixelSpanWidth,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            where TPixelComposer : struct, ICameraPolygonPixelComposer
        {
            if (pixelSpanWidth <= 0)
            {
                return;
            }

            int farColour = 0;
            int farScanlinePosition = 0;
            shadingDelta <<= 2;

            if (destinationOffset != 0)
            {
                farColour = endScanline / destinationOffset << 6;
                farScanlinePosition = startPixelX / destinationOffset << 6;
            }

            farColour = ClampToTextureRange(farColour);

            for (int remainingPixels = pixelSpanWidth;
                remainingPixels > 0;
                remainingPixels -= 16)
            {
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destinationOffset += interlaceMode;
                colour = farColour;
                startScanline = farScanlinePosition;

                if (destinationOffset != 0)
                {
                    farColour = endScanline / destinationOffset << 6;
                    farScanlinePosition = startPixelX / destinationOffset << 6;
                }

                farColour = ClampToTextureRange(farColour);

                int colourStep = farColour - colour >> 4;
                int positionStep = farScanlinePosition - startScanline >> 4;
                int lightLevel = shadingValue >> ShadingLightShift;
                colour += shadingValue & ShadingBandMask;
                shadingValue += shadingDelta;

                if (remainingPixels < 16)
                {
                    for (int remainderIndex = 0;
                        remainderIndex < remainingPixels;
                        remainderIndex += 1)
                    {
                        int sourceColour = scanlines[
                            (startScanline & TextureRowMask) +
                            (colour >> 6)] >> lightLevel;
                        pixels[pixelBufferOffset] = TPixelComposer.Compose(
                            sourceColour,
                            pixels,
                            pixelBufferOffset);
                        pixelBufferOffset += 1;
                        colour += colourStep;
                        startScanline += positionStep;

                        if ((remainderIndex & 3) == 3)
                        {
                            colour =
                                (colour & TextureCoordinateWrapMask) +
                                (shadingValue & ShadingBandMask);
                            lightLevel = shadingValue >> ShadingLightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
                else
                {
                    for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                    {
                        for (int pixelIndex = 0; pixelIndex < 4; pixelIndex += 1)
                        {
                            int sourceColour = scanlines[
                                (startScanline & TextureRowMask) +
                                (colour >> 6)] >> lightLevel;
                            pixels[pixelBufferOffset] = TPixelComposer.Compose(
                                sourceColour,
                                pixels,
                                pixelBufferOffset);
                            pixelBufferOffset += 1;
                            colour += colourStep;
                            startScanline += positionStep;
                        }

                        if (quadIndex < 3)
                        {
                            colour =
                                (colour & TextureCoordinateWrapMask) +
                                (shadingValue & ShadingBandMask);
                            lightLevel = shadingValue >> ShadingLightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
            }
        }

        private static int ClampToTextureRange(int value)
            => Math.Clamp(value, 0, TextureRowMask);
    }
}