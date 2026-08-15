namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraColourTableCache(
        CameraTextureManager textureManager)
    {
        private static int SquaredIntensityDivisor => 0x10000;

        internal void Ensure(int textureIndex)
        {
            for (int clipIndex = 0;
                clipIndex < textureManager.maxTextureCount;
                clipIndex += 1)
            {
                if (textureManager.textureClipIds[clipIndex] == textureIndex)
                {
                    textureManager.textureClipSizes =
                        textureManager.textureClipData[clipIndex];

                    return;
                }

                if (clipIndex != textureManager.maxTextureCount - 1)
                {
                    continue;
                }

                double randomValue = Helper.Random.NextDouble();
                int randomSlot =
                    (int)(randomValue * textureManager.maxTextureCount);

                if (randomSlot >= textureManager.textureClipIds.Length)
                {
                    randomSlot -= 1;
                }

                textureManager.textureClipIds[randomSlot] = textureIndex;
                int encodedTextureIndex = -1 - textureIndex;
                int redChannel = (encodedTextureIndex >> 10 & 0x1f) * 8;
                int greenChannel = (encodedTextureIndex >> 5 & 0x1f) * 8;
                int blueChannel = (encodedTextureIndex & 0x1f) * 8;

                for (int colourTableIndex = 0;
                    colourTableIndex < 256;
                    colourTableIndex += 1)
                {
                    int squaredIntensity =
                        colourTableIndex * colourTableIndex;
                    int redScaled =
                        redChannel * squaredIntensity /
                        SquaredIntensityDivisor;
                    int greenScaled =
                        greenChannel * squaredIntensity /
                        SquaredIntensityDivisor;
                    int blueScaled =
                        blueChannel * squaredIntensity /
                        SquaredIntensityDivisor;
                    textureManager.textureClipData[randomSlot][
                        255 - colourTableIndex] =
                        (redScaled << 16) +
                        (greenScaled << 8) +
                        blueScaled;
                }

                textureManager.textureClipSizes =
                    textureManager.textureClipData[randomSlot];
            }
        }
    }
}