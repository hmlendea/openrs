namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraTextureRasteriser
    {
        private static int Texture64Size => 64;

        private static int Texture64PixelCount => 4096;

        private static int Texture128Size => 128;

        private static int TexturePixelColourMask => 0xf8f8ff;

        private static int TransparentPixelColour => 0xf800ff;

        private static int ColourIndexByteMask => 0xff;

        private static int Texture64FrameType => 0;

        internal static void Apply(
            CameraTextureManager textureManager,
            int textureIndex)
        {
            int textureSize = Texture128Size;

            if (textureManager.textureLastAccessFrame[textureIndex] ==
                Texture64FrameType)
            {
                textureSize = Texture64Size;
            }

            int[] texture = textureManager.objectTexturePixels[textureIndex];
            int pixelCount = 0;

            for (int x = 0; x < textureSize; x += 1)
            {
                for (int y = 0; y < textureSize; y += 1)
                {
                    int pixel = textureManager.texturePictureColourArray[
                        textureIndex][textureManager.texturePictureColourIndex[
                            textureIndex][y + x * textureSize] & ColourIndexByteMask];
                    pixel &= TexturePixelColourMask;

                    if (pixel == 0)
                    {
                        pixel = 1;
                    }
                    else if (pixel == TransparentPixelColour)
                    {
                        pixel = 0;
                        textureManager.textureIsTransparent[textureIndex] = true;
                    }

                    texture[pixelCount] = pixel;
                    pixelCount += 1;
                }
            }

            ApplyLightingVariants(texture, pixelCount);
        }

        internal static void RotateAndRelight(
            CameraTextureManager textureManager,
            int textureIndex)
        {
            if (textureManager.objectTexturePixels[textureIndex] is null)
            {
                return;
            }

            int[] objectLighting =
                textureManager.objectTexturePixels[textureIndex];

            for (int columnIndex = 0;
                columnIndex < Texture64Size;
                columnIndex += 1)
            {
                int lastRowOffset =
                    columnIndex + CameraPolygonDrawer.Texture64RowMask;
                int savedValue = objectLighting[lastRowOffset];

                for (int rowIndex = 0;
                    rowIndex < Texture64Size - 1;
                    rowIndex += 1)
                {
                    objectLighting[lastRowOffset] =
                        objectLighting[lastRowOffset - Texture64Size];
                    lastRowOffset -= Texture64Size;
                }

                textureManager.objectTexturePixels[textureIndex][lastRowOffset] =
                    savedValue;
            }

            ApplyLightingVariants(objectLighting, Texture64PixelCount);
        }

        private static void ApplyLightingVariants(
            int[] texture,
            int pixelCount)
        {
            for (int pixelIndex = 0;
                pixelIndex < pixelCount;
                pixelIndex += 1)
            {
                int basePixel = texture[pixelIndex];
                texture[pixelCount + pixelIndex] =
                    basePixel - (basePixel >> 3) & TexturePixelColourMask;
                texture[pixelCount * 2 + pixelIndex] =
                    basePixel - (basePixel >> 2) & TexturePixelColourMask;
                texture[pixelCount * 3 + pixelIndex] =
                    basePixel - (basePixel >> 2) - (basePixel >> 3) &
                    TexturePixelColourMask;
            }
        }
    }
}