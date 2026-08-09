namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraTextureManager
    {
        private static long textureLastUsedTime;

        internal int maxTextureCount;
        internal int[] textureClipIds;
        internal int[][] textureClipData;
        internal int[] textureClipSizes;
        internal int textureCount;
        internal sbyte[][] texturePictureColourIndex;
        internal int[][] texturePictureColourArray;
        internal int[] textureLastAccessFrame;
        internal long[] textureLastAccessTimes;
        internal int[][] objectTexturePixels;
        internal bool[] textureIsTransparent;
        internal int[][] texturePixels;
        internal int[][] textureColourMaps;

        private static int Texture64Size => 64;
        private static int Texture64PixelCount => 4096;
        private static int Texture128Size => 128;
        private static int Texture128PixelCount => 16384;
        private static int ColourMapBufferSize => 0x10000;
        private static int TexturePixelColourMask => 0xf8f8ff;
        private static int TransparentPixelColour => 0xf800ff;
        private static int ColourIndexRedMultiplier => 1024;
        private static int ColourIndexGreenMultiplier => 32;
        private static int ColourChannelQuantisationDivisor => 8;
        private static int ColourIndexByteMask => 0xff;
        private static int ColourChannel5BitMask => 0x1f;
        private static long OldestTimeSentinel => 1L << 30;
        private static int Texture64FrameType => 0;
        private static int ColourMapFrameType => 1;

        internal CameraTextureManager(int maxTextureClipCount, int textureClipDataSize)
        {
            maxTextureCount = maxTextureClipCount;
            textureClipIds = new int[maxTextureCount];
            textureClipData = new int[maxTextureCount][];

            for (int textureClipIndex = 0; textureClipIndex < maxTextureCount; textureClipIndex += 1)
            {
                textureClipData[textureClipIndex] = new int[textureClipDataSize];
            }
        }

        internal void CreateTexture(int totalCount, int pixelBufferCount, int colourMapCount)
        {
            textureCount = totalCount;
            texturePictureColourIndex = new sbyte[totalCount][];
            texturePictureColourArray = new int[totalCount][];
            textureLastAccessFrame = new int[totalCount];
            textureLastAccessTimes = new long[totalCount];
            textureIsTransparent = new bool[totalCount];
            objectTexturePixels = new int[totalCount][];
            textureLastUsedTime = 0L;
            texturePixels = new int[pixelBufferCount][];
            textureColourMaps = new int[colourMapCount][];
        }

        internal void SetTexture(int textureIndex, sbyte[] colourIndices, int[] colourArray, int frameType)
        {
            texturePictureColourIndex[textureIndex] = colourIndices;
            texturePictureColourArray[textureIndex] = colourArray;
            textureLastAccessFrame[textureIndex] = frameType;
            textureLastAccessTimes[textureIndex] = 0L;
            textureIsTransparent[textureIndex] = false;
            objectTexturePixels[textureIndex] = null;
            UpdateTextureSmoothing(textureIndex);
        }

        internal void UpdateTextureSmoothing(int textureIndex)
        {
            if (textureIndex < 0)
            {
                return;
            }

            textureLastAccessTimes[textureIndex] = textureLastUsedTime += 1;

            if (objectTexturePixels[textureIndex] is not null)
            {
                return;
            }

            if (textureLastAccessFrame[textureIndex] == Texture64FrameType)
            {
                for (int bufferIndex = 0; bufferIndex < texturePixels.Length; bufferIndex += 1)
                {
                    if (texturePixels[bufferIndex] is null)
                    {
                        texturePixels[bufferIndex] = new int[Texture128PixelCount];
                        objectTexturePixels[textureIndex] = texturePixels[bufferIndex];
                        ApplyTexture(textureIndex);

                        return;
                    }
                }

                long oldestTime = OldestTimeSentinel;
                int oldestIndex = 0;

                for (int scanIndex = 0; scanIndex < textureCount; scanIndex += 1)
                {
                    if (scanIndex != textureIndex &&
                        textureLastAccessFrame[scanIndex] == Texture64FrameType &&
                        objectTexturePixels[scanIndex] is not null &&
                        textureLastAccessTimes[scanIndex] < oldestTime)
                    {
                        oldestTime = textureLastAccessTimes[scanIndex];
                        oldestIndex = scanIndex;
                    }
                }

                objectTexturePixels[textureIndex] = objectTexturePixels[oldestIndex];
                objectTexturePixels[oldestIndex] = null;
                ApplyTexture(textureIndex);

                return;
            }

            for (int colourMapIndex = 0; colourMapIndex < textureColourMaps.Length; colourMapIndex += 1)
            {
                if (textureColourMaps[colourMapIndex] is null)
                {
                    textureColourMaps[colourMapIndex] = new int[ColourMapBufferSize];
                    objectTexturePixels[textureIndex] = textureColourMaps[colourMapIndex];
                    ApplyTexture(textureIndex);

                    return;
                }
            }

            long oldestColourMapTime = OldestTimeSentinel;
            int oldestColourMapIndex = 0;

            for (int colourMapScanIndex = 0; colourMapScanIndex < textureCount; colourMapScanIndex += 1)
            {
                if (colourMapScanIndex != textureIndex &&
                    textureLastAccessFrame[colourMapScanIndex] == ColourMapFrameType &&
                    objectTexturePixels[colourMapScanIndex] is not null &&
                    textureLastAccessTimes[colourMapScanIndex] < oldestColourMapTime)
                {
                    oldestColourMapTime = textureLastAccessTimes[colourMapScanIndex];
                    oldestColourMapIndex = colourMapScanIndex;
                }
            }

            objectTexturePixels[textureIndex] = objectTexturePixels[oldestColourMapIndex];
            objectTexturePixels[oldestColourMapIndex] = null;
            ApplyTexture(textureIndex);
        }

        private void ApplyTexture(int textureIndex)
        {
            int textureSize = Texture128Size;

            if (textureLastAccessFrame[textureIndex] == Texture64FrameType)
            {
                textureSize = Texture64Size;
            }

            int[] texture = objectTexturePixels[textureIndex];
            int pixelCount = 0;

            for (int x = 0; x < textureSize; x += 1)
            {
                for (int y = 0; y < textureSize; y += 1)
                {
                    int pixel = texturePictureColourArray[textureIndex][texturePictureColourIndex[textureIndex][y + x * textureSize] & ColourIndexByteMask];
                    pixel &= TexturePixelColourMask;

                    if (pixel == 0)
                    {
                        pixel = 1;
                    }
                    else if (pixel == TransparentPixelColour)
                    {
                        pixel = 0;
                        textureIsTransparent[textureIndex] = true;
                    }

                    texture[pixelCount] = pixel;
                    pixelCount += 1;
                }
            }

            // Blend objects with correct lighting.
            for (int pixel = 0; pixel < pixelCount; pixel += 1)
            {
                int basePixel = texture[pixel];
                texture[pixelCount + pixel] = basePixel - (basePixel >> 3) & TexturePixelColourMask;
                texture[pixelCount * 2 + pixel] = basePixel - (basePixel >> 2) & TexturePixelColourMask;
                texture[pixelCount * 3 + pixel] = basePixel - (basePixel >> 2) - (basePixel >> 3) & TexturePixelColourMask;
            }
        }

        internal void UpdateLighting(int textureIndex)
        {
            if (objectTexturePixels[textureIndex] is null)
            {
                return;
            }

            int[] objLight = objectTexturePixels[textureIndex];

            for (int columnIndex = 0; columnIndex < Texture64Size; columnIndex += 1)
            {
                int lastRowOffset = columnIndex + CameraPolygonDrawer.Texture64RowMask;
                int savedValue = objLight[lastRowOffset];

                for (int rowLoopIndex = 0; rowLoopIndex < Texture64Size - 1; rowLoopIndex += 1)
                {
                    objLight[lastRowOffset] = objLight[lastRowOffset - Texture64Size];
                    lastRowOffset -= Texture64Size;
                }

                objectTexturePixels[textureIndex][lastRowOffset] = savedValue;
            }

            for (int pixelIndex = 0; pixelIndex < Texture64PixelCount; pixelIndex += 1)
            {
                int pixelValue = objLight[pixelIndex];
                objLight[Texture64PixelCount + pixelIndex] = pixelValue - (pixelValue >> 3) & TexturePixelColourMask;
                objLight[Texture64PixelCount * 2 + pixelIndex] = pixelValue - (pixelValue >> 2) & TexturePixelColourMask;
                objLight[Texture64PixelCount * 3 + pixelIndex] = pixelValue - (pixelValue >> 2) - (pixelValue >> 3) & TexturePixelColourMask;
            }
        }

        internal int ApplyTextureSmoothing(int index, int notSetSentinel)
        {
            if (index == notSetSentinel)
            {
                return 0;
            }

            UpdateTextureSmoothing(index);

            if (index >= 0)
            {
                return objectTexturePixels[index][0];
            }

            index = -(index + 1);
            int redComponent = index >> 10 & ColourChannel5BitMask;
            int greenComponent = index >> 5 & ColourChannel5BitMask;
            int blueComponent = index & ColourChannel5BitMask;

            return (redComponent << 19) + (greenComponent << 11) + (blueComponent << 3);
        }

        internal static int GetTextureColour(int r, int g, int b) =>
            -1 - r / ColourChannelQuantisationDivisor * ColourIndexRedMultiplier -
            g / ColourChannelQuantisationDivisor * ColourIndexGreenMultiplier -
            b / ColourChannelQuantisationDivisor;
    }
}
