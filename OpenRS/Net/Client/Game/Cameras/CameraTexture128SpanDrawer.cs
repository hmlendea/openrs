using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraTexture128SpanDrawer
    {
        private static int TextureRowMask => 0x3f80;

        private static int TextureCoordinateWrapMask => 0x3fff;

        private static int ShadingBandMask => 0x600000;

        private static int ShadingLightShift => 23;

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
            int lightLevel = 0;

            if (destinationOffset != 0)
            {
                colour = endScanline / destinationOffset << 7;
                startScanline = startPixelX / destinationOffset << 7;
            }

            colour = ClampToTextureRange(colour);
            endScanline += scanlineCount;
            startPixelX += scanlineStep;
            destinationOffset += interlaceMode;

            if (destinationOffset != 0)
            {
                farColour = endScanline / destinationOffset << 7;
                farScanlinePosition = startPixelX / destinationOffset << 7;
            }

            farColour = ClampToTextureRange(farColour);

            int colourStep = farColour - colour >> 4;
            int positionStep = farScanlinePosition - startScanline >> 4;

            for (int blockIndex = pixelSpanWidth >> 4;
                blockIndex > 0;
                blockIndex -= 1)
            {
                for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                {
                    colour =
                        (colour & TextureCoordinateWrapMask) +
                        (shadingValue & ShadingBandMask);
                    lightLevel = shadingValue >> ShadingLightShift;
                    shadingValue += shadingDelta;

                    for (int pixelIndex = 0; pixelIndex < 4; pixelIndex += 1)
                    {
                        int sourceColour = scanlines[
                            (startScanline & TextureRowMask) +
                            (colour >> 7)] >> lightLevel;
                        pixels[pixelBufferOffset] = TPixelComposer.Compose(
                            sourceColour,
                            pixels,
                            pixelBufferOffset);
                        pixelBufferOffset += 1;
                        colour += colourStep;
                        startScanline += positionStep;
                    }
                }

                colour = farColour;
                startScanline = farScanlinePosition;
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destinationOffset += interlaceMode;

                if (destinationOffset != 0)
                {
                    farColour = endScanline / destinationOffset << 7;
                    farScanlinePosition = startPixelX / destinationOffset << 7;
                }

                farColour = ClampToTextureRange(farColour);
                colourStep = farColour - colour >> 4;
                positionStep = farScanlinePosition - startScanline >> 4;
            }

            for (int remainderIndex = 0;
                remainderIndex < (pixelSpanWidth & 0xf);
                remainderIndex += 1)
            {
                if ((remainderIndex & 3) == 0)
                {
                    colour =
                        (colour & TextureCoordinateWrapMask) +
                        (shadingValue & ShadingBandMask);
                    lightLevel = shadingValue >> ShadingLightShift;
                    shadingValue += shadingDelta;
                }

                int sourceColour = scanlines[
                    (startScanline & TextureRowMask) +
                    (colour >> 7)] >> lightLevel;
                pixels[pixelBufferOffset] = TPixelComposer.Compose(
                    sourceColour,
                    pixels,
                    pixelBufferOffset);
                pixelBufferOffset += 1;
                colour += colourStep;
                startScanline += positionStep;
            }
        }

        private static int ClampToTextureRange(int value)
            => Math.Clamp(value, 0, TextureRowMask);
    }
}