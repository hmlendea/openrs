using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraPolygonDrawer
    {
        internal static int Texture64RowMask => 0xfc0;

        private static int Texture128RowMask => 0x3f80;

        private static int Texture128UCoordWrapMask => 0x3fff;

        private static int Shading128BandMask => 0x600000;

        private static int Shading128LightShift => 23;

        private static int Texture64UCoordWrapMask => 0xfff;

        private static int Shading64BandMask => 0xc0000;

        private static int Shading64LightShift => 20;

        private static int HalfBlendChannelMask => 0x7f7f7f;

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
        {
            if (pixelSpanWidth <= 0)
            {
                return;
            }

            int farColour = 0;
            int farScanlinePos = 0;
            int lightLevel = 0;

            if (destOffset != 0)
            {
                colour = endScanline / destOffset << 7;
                startScanline = startPixelX / destOffset << 7;
            }

            colour = ClampToTexture128Range(colour);
            endScanline += scanlineCount;
            startPixelX += scanlineStep;
            destOffset += interlaceMode;

            if (destOffset != 0)
            {
                farColour = endScanline / destOffset << 7;
                farScanlinePos = startPixelX / destOffset << 7;
            }

            farColour = ClampToTexture128Range(farColour);

            int colourStep = farColour - colour >> 4;
            int positionStep = farScanlinePos - startScanline >> 4;

            for (int blockIndex = pixelSpanWidth >> 4; blockIndex > 0; blockIndex -= 1)
            {
                for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                {
                    colour = (colour & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                    lightLevel = shadingValue >> Shading128LightShift;
                    shadingValue += shadingDelta;

                    for (int pixelIndex = 0; pixelIndex < 4; pixelIndex += 1)
                    {
                        pixels[pixelBufferOffset] = scanlines[(startScanline & Texture128RowMask) + (colour >> 7)] >> lightLevel;
                        pixelBufferOffset += 1;
                        colour += colourStep;
                        startScanline += positionStep;
                    }
                }

                colour = farColour;
                startScanline = farScanlinePos;
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;

                if (destOffset != 0)
                {
                    farColour = endScanline / destOffset << 7;
                    farScanlinePos = startPixelX / destOffset << 7;
                }

                farColour = ClampToTexture128Range(farColour);
                colourStep = farColour - colour >> 4;
                positionStep = farScanlinePos - startScanline >> 4;
            }

            for (int remainderIndex = 0; remainderIndex < (pixelSpanWidth & 0xf); remainderIndex += 1)
            {
                if ((remainderIndex & 3) == 0)
                {
                    colour = (colour & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                    lightLevel = shadingValue >> Shading128LightShift;
                    shadingValue += shadingDelta;
                }

                pixels[pixelBufferOffset] = scanlines[(startScanline & Texture128RowMask) + (colour >> 7)] >> lightLevel;
                pixelBufferOffset += 1;
                colour += colourStep;
                startScanline += positionStep;
            }
        }

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
        {
            if (pixelSpanWidth <= 0)
            {
                return;
            }

            int farColour = 0;
            int farScanlinePos = 0;
            int lightLevel = 0;

            if (destOffset != 0)
            {
                colour = endScanline / destOffset << 7;
                startScanline = startPixelX / destOffset << 7;
            }

            colour = ClampToTexture128Range(colour);
            endScanline += scanlineCount;
            startPixelX += scanlineStep;
            destOffset += interlaceMode;

            if (destOffset != 0)
            {
                farColour = endScanline / destOffset << 7;
                farScanlinePos = startPixelX / destOffset << 7;
            }

            farColour = ClampToTexture128Range(farColour);

            int colourStep = farColour - colour >> 4;
            int positionStep = farScanlinePos - startScanline >> 4;

            for (int blockIndex = pixelSpanWidth >> 4; blockIndex > 0; blockIndex -= 1)
            {
                for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                {
                    colour = (colour & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                    lightLevel = shadingValue >> Shading128LightShift;
                    shadingValue += shadingDelta;

                    for (int pixelIndex = 0; pixelIndex < 4; pixelIndex += 1)
                    {
                        pixels[pixelBufferOffset] = (scanlines[(startScanline & Texture128RowMask) + (colour >> 7)] >> lightLevel) + (pixels[pixelBufferOffset + 1] >> 1 & HalfBlendChannelMask);
                        pixelBufferOffset += 1;
                        colour += colourStep;
                        startScanline += positionStep;
                    }
                }

                colour = farColour;
                startScanline = farScanlinePos;
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;

                if (destOffset != 0)
                {
                    farColour = endScanline / destOffset << 7;
                    farScanlinePos = startPixelX / destOffset << 7;
                }

                farColour = ClampToTexture128Range(farColour);
                colourStep = farColour - colour >> 4;
                positionStep = farScanlinePos - startScanline >> 4;
            }

            for (int remainderIndex = 0; remainderIndex < (pixelSpanWidth & 0xf); remainderIndex += 1)
            {
                if ((remainderIndex & 3) == 0)
                {
                    colour = (colour & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                    lightLevel = shadingValue >> Shading128LightShift;
                    shadingValue += shadingDelta;
                }

                pixels[pixelBufferOffset] = (scanlines[(startScanline & Texture128RowMask) + (colour >> 7)] >> lightLevel) + (pixels[pixelBufferOffset + 1] >> 1 & HalfBlendChannelMask);
                pixelBufferOffset += 1;
                colour += colourStep;
                startScanline += positionStep;
            }
        }

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
        {
            if (interlaceMode <= 0)
            {
                return;
            }

            int farTextureColumn = 0;
            int farTextureRow = 0;
            shadingDelta <<= 2;

            if (scanlineCount != 0)
            {
                farTextureColumn = startScanline / scanlineCount << 7;
                farTextureRow = destOffset / scanlineCount << 7;
            }

            farTextureColumn = ClampToTexture128Range(farTextureColumn);

            for (int blockIterCount = interlaceMode; blockIterCount > 0; blockIterCount -= 16)
            {
                startScanline += scanlineStep;
                destOffset += columnNumeratorStep;
                scanlineCount += rowNumeratorStep;
                textureColumn = farTextureColumn;
                textureRow = farTextureRow;

                if (scanlineCount != 0)
                {
                    farTextureColumn = startScanline / scanlineCount << 7;
                    farTextureRow = destOffset / scanlineCount << 7;
                }

                farTextureColumn = ClampToTexture128Range(farTextureColumn);

                int textureColumnStep = farTextureColumn - textureColumn >> 4;
                int textureRowStep = farTextureRow - textureRow >> 4;
                textureColumn = (textureColumn & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                int lightLevel = shadingValue >> Shading128LightShift;
                shadingValue += shadingDelta;

                if (blockIterCount < 16)
                {
                    for (int remainderIndex = 0; remainderIndex < blockIterCount; remainderIndex += 1)
                    {
                        texturePixels = textureData[(textureRow & Texture128RowMask) + (textureColumn >> 7)] >> lightLevel;

                        if (texturePixels != 0)
                        {
                            pixels[pixelBufferOffset] = texturePixels;
                        }

                        pixelBufferOffset += 1;
                        textureColumn += textureColumnStep;
                        textureRow += textureRowStep;

                        if ((remainderIndex & 3) == 3)
                        {
                            textureColumn = (textureColumn & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                            lightLevel = shadingValue >> Shading128LightShift;
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
                            texturePixels = textureData[(textureRow & Texture128RowMask) + (textureColumn >> 7)] >> lightLevel;

                            if (texturePixels != 0)
                            {
                                pixels[pixelBufferOffset] = texturePixels;
                            }

                            pixelBufferOffset += 1;
                            textureColumn += textureColumnStep;
                            textureRow += textureRowStep;
                        }

                        if (quadIndex < 3)
                        {
                            textureColumn = (textureColumn & Texture128UCoordWrapMask) + (shadingValue & Shading128BandMask);
                            lightLevel = shadingValue >> Shading128LightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
            }
        }

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
        {
            if (pixelSpanWidth <= 0)
            {
                return;
            }

            int farColour = 0;
            int farScanlinePos = 0;
            shadingDelta <<= 2;

            if (destOffset != 0)
            {
                farColour = endScanline / destOffset << 6;
                farScanlinePos = startPixelX / destOffset << 6;
            }

            farColour = ClampToTexture64Range(farColour);

            for (int remainingPixels = pixelSpanWidth; remainingPixels > 0; remainingPixels -= 16)
            {
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;
                colour = farColour;
                startScanline = farScanlinePos;

                if (destOffset != 0)
                {
                    farColour = endScanline / destOffset << 6;
                    farScanlinePos = startPixelX / destOffset << 6;
                }

                farColour = ClampToTexture64Range(farColour);

                int colourStep = farColour - colour >> 4;
                int positionStep = farScanlinePos - startScanline >> 4;
                int lightLevel = shadingValue >> Shading64LightShift;
                colour += shadingValue & Shading64BandMask;
                shadingValue += shadingDelta;

                if (remainingPixels < 16)
                {
                    for (int remainderIndex = 0; remainderIndex < remainingPixels; remainderIndex += 1)
                    {
                        pixels[pixelBufferOffset] = scanlines[(startScanline & Texture64RowMask) + (colour >> 6)] >> lightLevel;
                        pixelBufferOffset += 1;
                        colour += colourStep;
                        startScanline += positionStep;

                        if ((remainderIndex & 3) == 3)
                        {
                            colour = (colour & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                            lightLevel = shadingValue >> Shading64LightShift;
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
                            pixels[pixelBufferOffset] = scanlines[(startScanline & Texture64RowMask) + (colour >> 6)] >> lightLevel;
                            pixelBufferOffset += 1;
                            colour += colourStep;
                            startScanline += positionStep;
                        }

                        if (quadIndex < 3)
                        {
                            colour = (colour & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                            lightLevel = shadingValue >> Shading64LightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
            }
        }

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
        {
            if (pixelSpanWidth <= 0)
            {
                return;
            }

            int farColour = 0;
            int farScanlinePos = 0;
            shadingDelta <<= 2;

            if (destOffset != 0)
            {
                farColour = endScanline / destOffset << 6;
                farScanlinePos = startPixelX / destOffset << 6;
            }

            farColour = ClampToTexture64Range(farColour);

            for (int remainingPixels = pixelSpanWidth; remainingPixels > 0; remainingPixels -= 16)
            {
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;
                colour = farColour;
                startScanline = farScanlinePos;

                if (destOffset != 0)
                {
                    farColour = endScanline / destOffset << 6;
                    farScanlinePos = startPixelX / destOffset << 6;
                }

                farColour = ClampToTexture64Range(farColour);

                int colourStep = farColour - colour >> 4;
                int positionStep = farScanlinePos - startScanline >> 4;
                int lightLevel = shadingValue >> Shading64LightShift;
                colour += shadingValue & Shading64BandMask;
                shadingValue += shadingDelta;

                if (remainingPixels < 16)
                {
                    for (int remainderIndex = 0; remainderIndex < remainingPixels; remainderIndex += 1)
                    {
                        pixels[pixelBufferOffset] = (scanlines[(startScanline & Texture64RowMask) + (colour >> 6)] >> lightLevel) + (pixels[pixelBufferOffset + 1] >> 1 & HalfBlendChannelMask);
                        pixelBufferOffset += 1;
                        colour += colourStep;
                        startScanline += positionStep;

                        if ((remainderIndex & 3) == 3)
                        {
                            colour = (colour & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                            lightLevel = shadingValue >> Shading64LightShift;
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
                            pixels[pixelBufferOffset] = (scanlines[(startScanline & Texture64RowMask) + (colour >> 6)] >> lightLevel) + (pixels[pixelBufferOffset + 1] >> 1 & HalfBlendChannelMask);
                            pixelBufferOffset += 1;
                            colour += colourStep;
                            startScanline += positionStep;
                        }

                        if (quadIndex < 3)
                        {
                            colour = (colour & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                            lightLevel = shadingValue >> Shading64LightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
            }
        }

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
        {
            if (interlaceMode <= 0)
            {
                return;
            }

            int farTextureColumn = 0;
            int farTextureRow = 0;
            shadingDelta <<= 2;

            if (scanlineCount != 0)
            {
                farTextureColumn = startScanline / scanlineCount << 6;
                farTextureRow = destOffset / scanlineCount << 6;
            }

            farTextureColumn = ClampToTexture64Range(farTextureColumn);

            for (int blockIterCount = interlaceMode; blockIterCount > 0; blockIterCount -= 16)
            {
                startScanline += scanlineStep;
                destOffset += columnNumeratorStep;
                scanlineCount += rowNumeratorStep;
                textureColumn = farTextureColumn;
                textureRow = farTextureRow;

                if (scanlineCount != 0)
                {
                    farTextureColumn = startScanline / scanlineCount << 6;
                    farTextureRow = destOffset / scanlineCount << 6;
                }

                farTextureColumn = ClampToTexture64Range(farTextureColumn);

                int textureColumnStep = farTextureColumn - textureColumn >> 4;
                int textureRowStep = farTextureRow - textureRow >> 4;
                textureColumn = (textureColumn & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                int lightLevel = shadingValue >> Shading64LightShift;
                shadingValue += shadingDelta;

                if (blockIterCount < 16)
                {
                    for (int remainderIndex = 0; remainderIndex < blockIterCount; remainderIndex += 1)
                    {
                        texturePixels = textureData[(textureRow & Texture64RowMask) + (textureColumn >> 6)] >> lightLevel;

                        if (texturePixels != 0)
                        {
                            pixels[pixelBufferOffset] = texturePixels;
                        }

                        pixelBufferOffset += 1;
                        textureColumn += textureColumnStep;
                        textureRow += textureRowStep;

                        if ((remainderIndex & 3) == 3)
                        {
                            textureColumn = (textureColumn & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                            lightLevel = shadingValue >> Shading64LightShift;
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
                            texturePixels = textureData[(textureRow & Texture64RowMask) + (textureColumn >> 6)] >> lightLevel;

                            if (texturePixels != 0)
                            {
                                pixels[pixelBufferOffset] = texturePixels;
                            }

                            pixelBufferOffset += 1;
                            textureColumn += textureColumnStep;
                            textureRow += textureRowStep;
                        }

                        if (quadIndex < 3)
                        {
                            textureColumn = (textureColumn & Texture64UCoordWrapMask) + (shadingValue & Shading64BandMask);
                            lightLevel = shadingValue >> Shading64LightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
            }
        }

        internal static void DrawVertexColorPolygon(
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

        internal static void DrawShiftColorPolygon(
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

            destOffset <<= 2;
            shadeColour = shadeData[startScanline >> 8 & 0xff];
            startScanline += destOffset;
            int loopCounter = startColour / 16;

            for (int blockIndex = loopCounter; blockIndex < 0; blockIndex += 1)
            {
                for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                {
                    for (int pixelIndex = 0; pixelIndex < 4; pixelIndex += 1)
                    {
                        pixels[pixelBufferIndex] = shadeColour + (pixels[pixelBufferIndex + 1] >> 1 & HalfBlendChannelMask);
                        pixelBufferIndex += 1;
                    }

                    shadeColour = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }

            loopCounter = -(startColour % 16);

            for (int pixelIndex = 0; pixelIndex < loopCounter; pixelIndex += 1)
            {
                pixels[pixelBufferIndex] = shadeColour + (pixels[pixelBufferIndex + 1] >> 1 & HalfBlendChannelMask);
                pixelBufferIndex += 1;

                if ((pixelIndex & 3) == 3)
                {
                    shadeColour = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                    startScanline += destOffset;
                }
            }
        }

        internal static void DrawGradientPolygon(
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

            destOffset <<= 2;
            shadeColour = shadeData[startScanline >> 8 & 0xff];
            startScanline += destOffset;
            int loopCounter = startColour / 16;

            for (int blockIndex = loopCounter; blockIndex < 0; blockIndex += 1)
            {
                for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                {
                    for (int pixelIndex = 0; pixelIndex < 4; pixelIndex += 1)
                    {
                        pixels[pixelBufferIndex] = shadeColour;
                        pixelBufferIndex += 1;
                    }

                    shadeColour = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }

            loopCounter = -(startColour % 16);

            for (int pixelIndex = 0; pixelIndex < loopCounter; pixelIndex += 1)
            {
                pixels[pixelBufferIndex] = shadeColour;
                pixelBufferIndex += 1;

                if ((pixelIndex & 3) == 3)
                {
                    shadeColour = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }
        }

        private static int ClampToTexture128Range(int value) => Math.Clamp(value, 0, Texture128RowMask);

        private static int ClampToTexture64Range(int value) => Math.Clamp(value, 0, Texture64RowMask);
    }
}
