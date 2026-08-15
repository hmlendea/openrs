namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraShade4SpanDrawer
    {
        private static int PixelsPerShade => 4;

        private static int PixelsPerBlock => 16;

        private static int ShadeIndexMask => 0xff;

        internal static void Draw<TPixelComposer>(
            int[] pixels,
            int startColour,
            int pixelBufferIndex,
            int shadeColour,
            int[] shadeData,
            int startScanline,
            int destinationOffset,
            int remainderScanlineStepMultiplier)
            where TPixelComposer : struct, ICameraPolygonPixelComposer
        {
            if (startColour >= 0)
            {
                return;
            }

            destinationOffset <<= 2;
            shadeColour = shadeData[startScanline >> 8 & ShadeIndexMask];
            startScanline += destinationOffset;
            int loopCounter = startColour / PixelsPerBlock;

            for (int blockIndex = loopCounter;
                blockIndex < 0;
                blockIndex += 1)
            {
                for (int shadeIndex = 0; shadeIndex < 4; shadeIndex += 1)
                {
                    for (int pixelIndex = 0;
                        pixelIndex < PixelsPerShade;
                        pixelIndex += 1)
                    {
                        pixels[pixelBufferIndex] = TPixelComposer.Compose(
                            shadeColour,
                            pixels,
                            pixelBufferIndex);
                        pixelBufferIndex += 1;
                    }

                    shadeColour = shadeData[
                        startScanline >> 8 & ShadeIndexMask];
                    startScanline += destinationOffset;
                }
            }

            loopCounter = -(startColour % PixelsPerBlock);

            for (int pixelIndex = 0;
                pixelIndex < loopCounter;
                pixelIndex += 1)
            {
                pixels[pixelBufferIndex] = TPixelComposer.Compose(
                    shadeColour,
                    pixels,
                    pixelBufferIndex);
                pixelBufferIndex += 1;

                if ((pixelIndex & (PixelsPerShade - 1)) == PixelsPerShade - 1)
                {
                    shadeColour = shadeData[
                        startScanline >> 8 & ShadeIndexMask];
                    startScanline +=
                        destinationOffset * remainderScanlineStepMultiplier;
                }
            }
        }
    }
}