namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraPolygonDrawer
    {
        internal static int Texture64RowMask
            => CameraTexture64SpanDrawer.TextureRowMask;

        internal static void DrawFlatPolygon(
            int[] pixels,
            int[] scanlines,
            int colour,
            int startScanline,
            int endScanline,
            int startPixelX,
            int destOffset,
            int scanlineCount,
            int scanlineStep,
            int interlaceMode,
            int pixelSpanWidth,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            => CameraTexture128SpanDrawer.Draw<CameraOpaquePolygonPixelComposer>(
                pixels,
                scanlines,
                colour,
                startScanline,
                endScanline,
                startPixelX,
                destOffset,
                scanlineCount,
                scanlineStep,
                interlaceMode,
                pixelSpanWidth,
                pixelBufferOffset,
                shadingValue,
                shadingDelta);

        internal static void DrawShadedPolygon(
            int[] pixels,
            int[] scanlines,
            int colour,
            int startScanline,
            int endScanline,
            int startPixelX,
            int destOffset,
            int scanlineCount,
            int scanlineStep,
            int interlaceMode,
            int pixelSpanWidth,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            => CameraTexture128SpanDrawer.Draw<CameraHalfBlendPolygonPixelComposer>(
                pixels,
                scanlines,
                colour,
                startScanline,
                endScanline,
                startPixelX,
                destOffset,
                scanlineCount,
                scanlineStep,
                interlaceMode,
                pixelSpanWidth,
                pixelBufferOffset,
                shadingValue,
                shadingDelta);

        internal static void DrawTexturedPolygon(
            int[] pixels,
            int texturePixels,
            int textureColumn,
            int textureRow,
            int[] textureData,
            int startScanline,
            int destOffset,
            int scanlineCount,
            int scanlineStep,
            int columnNumeratorStep,
            int rowNumeratorStep,
            int interlaceMode,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            => CameraPerspectiveTextureSpanDrawer.Draw<
                CameraTexture128CoordinateFormat>(
                pixels,
                texturePixels,
                textureColumn,
                textureRow,
                textureData,
                startScanline,
                destOffset,
                scanlineCount,
                scanlineStep,
                columnNumeratorStep,
                rowNumeratorStep,
                interlaceMode,
                pixelBufferOffset,
                shadingValue,
                shadingDelta);

        internal static void DrawTransparentPolygon(
            int[] pixels,
            int[] scanlines,
            int colour,
            int startScanline,
            int endScanline,
            int startPixelX,
            int destOffset,
            int scanlineCount,
            int scanlineStep,
            int interlaceMode,
            int pixelSpanWidth,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            => CameraTexture64SpanDrawer.Draw<CameraOpaquePolygonPixelComposer>(
                pixels,
                scanlines,
                colour,
                startScanline,
                endScanline,
                startPixelX,
                destOffset,
                scanlineCount,
                scanlineStep,
                interlaceMode,
                pixelSpanWidth,
                pixelBufferOffset,
                shadingValue,
                shadingDelta);

        internal static void DrawMaskedPolygon(
            int[] pixels,
            int[] scanlines,
            int colour,
            int startScanline,
            int endScanline,
            int startPixelX,
            int destOffset,
            int scanlineCount,
            int scanlineStep,
            int interlaceMode,
            int pixelSpanWidth,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            => CameraTexture64SpanDrawer.Draw<CameraHalfBlendPolygonPixelComposer>(
                pixels,
                scanlines,
                colour,
                startScanline,
                endScanline,
                startPixelX,
                destOffset,
                scanlineCount,
                scanlineStep,
                interlaceMode,
                pixelSpanWidth,
                pixelBufferOffset,
                shadingValue,
                shadingDelta);

        internal static void DrawFlatTexturedPolygon(
            int[] pixels,
            int texturePixels,
            int textureColumn,
            int textureRow,
            int[] textureData,
            int startScanline,
            int destOffset,
            int scanlineCount,
            int scanlineStep,
            int columnNumeratorStep,
            int rowNumeratorStep,
            int interlaceMode,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            => CameraPerspectiveTextureSpanDrawer.Draw<
                CameraTexture64CoordinateFormat>(
                pixels,
                texturePixels,
                textureColumn,
                textureRow,
                textureData,
                startScanline,
                destOffset,
                scanlineCount,
                scanlineStep,
                columnNumeratorStep,
                rowNumeratorStep,
                interlaceMode,
                pixelBufferOffset,
                shadingValue,
                shadingDelta);

        internal static void DrawVertexColourPolygon(
            int[] pixels,
            int startColour,
            int pixelBufferIndex,
            int shadeColour,
            int[] shadeData,
            int startScanline,
            int destOffset)
        {
            if (startColour >= 0)
            {
                return;
            }

            destOffset <<= 1;
            shadeColour = shadeData[startScanline >> 8 & 0xff];
            startScanline += destOffset;
            int loopCounter = startColour / 8;

            for (int blockIndex = loopCounter; blockIndex < 0; blockIndex += 1)
            {
                for (int pairIndex = 0; pairIndex < 4; pairIndex += 1)
                {
                    pixels[pixelBufferIndex] = shadeColour;
                    pixelBufferIndex += 1;
                    pixels[pixelBufferIndex] = shadeColour;
                    pixelBufferIndex += 1;
                    shadeColour = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }

            loopCounter = -(startColour % 8);

            for (int pixelIndex = 0; pixelIndex < loopCounter; pixelIndex += 1)
            {
                pixels[pixelBufferIndex] = shadeColour;
                pixelBufferIndex += 1;

                if ((pixelIndex & 1) == 1)
                {
                    shadeColour = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }
        }

        internal static void DrawShiftColourPolygon(
            int[] pixels,
            int startColour,
            int pixelBufferIndex,
            int shadeColour,
            int[] shadeData,
            int startScanline,
            int destOffset)
            => CameraShade4SpanDrawer.Draw<
                CameraHalfBlendPolygonPixelComposer>(
                pixels,
                startColour,
                pixelBufferIndex,
                shadeColour,
                shadeData,
                startScanline,
                destOffset,
                2);

        internal static void DrawGradientPolygon(
            int[] pixels,
            int startColour,
            int pixelBufferIndex,
            int shadeColour,
            int[] shadeData,
            int startScanline,
            int destOffset)
            => CameraShade4SpanDrawer.Draw<CameraOpaquePolygonPixelComposer>(
                pixels,
                startColour,
                pixelBufferIndex,
                shadeColour,
                shadeData,
                startScanline,
                destOffset,
                1);

    }
}
