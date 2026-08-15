namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraTextureBufferPool
    {
        private static int Texture128PixelCount => 16384;

        private static int ColourMapBufferSize => 0x10000;

        private static long OldestTimeSentinel => 1L << 30;

        private static int Texture64FrameType => 0;

        private static int ColourMapFrameType => 1;

        private static long lastUsedTime;

        internal static void ResetClock() => lastUsedTime = 0L;

        internal static void EnsureLoaded(
            CameraTextureManager textureManager,
            int textureIndex)
        {
            if (textureIndex < 0)
            {
                return;
            }

            textureManager.textureLastAccessTimes[textureIndex] =
                lastUsedTime += 1;

            if (textureManager.objectTexturePixels[textureIndex] is not null)
            {
                return;
            }

            if (textureManager.textureLastAccessFrame[textureIndex] ==
                Texture64FrameType)
            {
                LoadTexture64(textureManager, textureIndex);

                return;
            }

            LoadColourMap(textureManager, textureIndex);
        }

        private static void LoadTexture64(
            CameraTextureManager textureManager,
            int textureIndex)
        {
            for (int bufferIndex = 0;
                bufferIndex < textureManager.texturePixels.Length;
                bufferIndex += 1)
            {
                if (textureManager.texturePixels[bufferIndex] is null)
                {
                    textureManager.texturePixels[bufferIndex] =
                        new int[Texture128PixelCount];
                    textureManager.objectTexturePixels[textureIndex] =
                        textureManager.texturePixels[bufferIndex];
                    CameraTextureRasteriser.Apply(textureManager, textureIndex);

                    return;
                }
            }

            long oldestTime = OldestTimeSentinel;
            int oldestIndex = 0;

            for (int scanIndex = 0;
                scanIndex < textureManager.textureCount;
                scanIndex += 1)
            {
                if (scanIndex != textureIndex &&
                    textureManager.textureLastAccessFrame[scanIndex] ==
                    Texture64FrameType &&
                    textureManager.objectTexturePixels[scanIndex] is not null &&
                    textureManager.textureLastAccessTimes[scanIndex] < oldestTime)
                {
                    oldestTime = textureManager.textureLastAccessTimes[scanIndex];
                    oldestIndex = scanIndex;
                }
            }

            textureManager.objectTexturePixels[textureIndex] =
                textureManager.objectTexturePixels[oldestIndex];
            textureManager.objectTexturePixels[oldestIndex] = null;
            CameraTextureRasteriser.Apply(textureManager, textureIndex);
        }

        private static void LoadColourMap(
            CameraTextureManager textureManager,
            int textureIndex)
        {
            for (int colourMapIndex = 0;
                colourMapIndex < textureManager.textureColourMaps.Length;
                colourMapIndex += 1)
            {
                if (textureManager.textureColourMaps[colourMapIndex] is null)
                {
                    textureManager.textureColourMaps[colourMapIndex] =
                        new int[ColourMapBufferSize];
                    textureManager.objectTexturePixels[textureIndex] =
                        textureManager.textureColourMaps[colourMapIndex];
                    CameraTextureRasteriser.Apply(textureManager, textureIndex);

                    return;
                }
            }

            long oldestTime = OldestTimeSentinel;
            int oldestIndex = 0;

            for (int scanIndex = 0;
                scanIndex < textureManager.textureCount;
                scanIndex += 1)
            {
                if (scanIndex != textureIndex &&
                    textureManager.textureLastAccessFrame[scanIndex] ==
                    ColourMapFrameType &&
                    textureManager.objectTexturePixels[scanIndex] is not null &&
                    textureManager.textureLastAccessTimes[scanIndex] < oldestTime)
                {
                    oldestTime = textureManager.textureLastAccessTimes[scanIndex];
                    oldestIndex = scanIndex;
                }
            }

            textureManager.objectTexturePixels[textureIndex] =
                textureManager.objectTexturePixels[oldestIndex];
            textureManager.objectTexturePixels[oldestIndex] = null;
            CameraTextureRasteriser.Apply(textureManager, textureIndex);
        }
    }
}