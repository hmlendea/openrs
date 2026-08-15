using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraPerspectiveTextureSpanDrawer
    {
        internal static void Draw<TTextureFormat>(
            int[] pixels,
            int texturePixels,
            int textureColumn,
            int textureRow,
            int[] textureData,
            int startScanline,
            int destinationOffset,
            int scanlineCount,
            int scanlineStep,
            int columnNumeratorStep,
            int rowNumeratorStep,
            int interlaceMode,
            int pixelBufferOffset,
            int shadingValue,
            int shadingDelta)
            where TTextureFormat : struct, ICameraTextureCoordinateFormat
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
                farTextureColumn =
                    startScanline / scanlineCount <<
                    TTextureFormat.CoordinateShift;
                farTextureRow =
                    destinationOffset / scanlineCount <<
                    TTextureFormat.CoordinateShift;
            }

            farTextureColumn = ClampToTextureRange<TTextureFormat>(
                farTextureColumn);

            for (int blockIterationCount = interlaceMode;
                blockIterationCount > 0;
                blockIterationCount -= 16)
            {
                startScanline += scanlineStep;
                destinationOffset += columnNumeratorStep;
                scanlineCount += rowNumeratorStep;
                textureColumn = farTextureColumn;
                textureRow = farTextureRow;

                if (scanlineCount != 0)
                {
                    farTextureColumn =
                        startScanline / scanlineCount <<
                        TTextureFormat.CoordinateShift;
                    farTextureRow =
                        destinationOffset / scanlineCount <<
                        TTextureFormat.CoordinateShift;
                }

                farTextureColumn = ClampToTextureRange<TTextureFormat>(
                    farTextureColumn);

                int textureColumnStep =
                    farTextureColumn - textureColumn >> 4;
                int textureRowStep = farTextureRow - textureRow >> 4;
                textureColumn =
                    (textureColumn & TTextureFormat.CoordinateWrapMask) +
                    (shadingValue & TTextureFormat.ShadingBandMask);
                int lightLevel =
                    shadingValue >> TTextureFormat.ShadingLightShift;
                shadingValue += shadingDelta;

                if (blockIterationCount < 16)
                {
                    for (int remainderIndex = 0;
                        remainderIndex < blockIterationCount;
                        remainderIndex += 1)
                    {
                        texturePixels = SampleTexture<TTextureFormat>(
                            textureData,
                            textureColumn,
                            textureRow,
                            lightLevel);

                        if (texturePixels != 0)
                        {
                            pixels[pixelBufferOffset] = texturePixels;
                        }

                        pixelBufferOffset += 1;
                        textureColumn += textureColumnStep;
                        textureRow += textureRowStep;

                        if ((remainderIndex & 3) == 3)
                        {
                            textureColumn =
                                (textureColumn &
                                    TTextureFormat.CoordinateWrapMask) +
                                (shadingValue &
                                    TTextureFormat.ShadingBandMask);
                            lightLevel =
                                shadingValue >>
                                TTextureFormat.ShadingLightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
                else
                {
                    for (int quadIndex = 0; quadIndex < 4; quadIndex += 1)
                    {
                        for (int pixelIndex = 0;
                            pixelIndex < 4;
                            pixelIndex += 1)
                        {
                            texturePixels = SampleTexture<TTextureFormat>(
                                textureData,
                                textureColumn,
                                textureRow,
                                lightLevel);

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
                            textureColumn =
                                (textureColumn &
                                    TTextureFormat.CoordinateWrapMask) +
                                (shadingValue &
                                    TTextureFormat.ShadingBandMask);
                            lightLevel =
                                shadingValue >>
                                TTextureFormat.ShadingLightShift;
                            shadingValue += shadingDelta;
                        }
                    }
                }
            }
        }

        private static int ClampToTextureRange<TTextureFormat>(int value)
            where TTextureFormat : struct, ICameraTextureCoordinateFormat
            => Math.Clamp(value, 0, TTextureFormat.RowMask);

        private static int SampleTexture<TTextureFormat>(
            int[] textureData,
            int textureColumn,
            int textureRow,
            int lightLevel)
            where TTextureFormat : struct, ICameraTextureCoordinateFormat
            => textureData[
                (textureRow & TTextureFormat.RowMask) +
                (textureColumn >> TTextureFormat.CoordinateShift)] >> lightLevel;
    }
}