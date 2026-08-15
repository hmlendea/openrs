namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraTextureManager
    {
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
            CameraTextureBufferPool.ResetClock();
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
            => CameraTextureBufferPool.EnsureLoaded(this, textureIndex);

        internal void UpdateLighting(int textureIndex)
            => CameraTextureRasteriser.RotateAndRelight(this, textureIndex);

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

            return CameraTextureColourCodec.Expand(index);
        }

        internal static int GetTextureColour(int r, int g, int b)
            => CameraTextureColourCodec.Pack(r, g, b);
    }
}
