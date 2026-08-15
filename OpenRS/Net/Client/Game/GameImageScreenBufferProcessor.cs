namespace OpenRS.Net.Client.Game
{
    internal static class GameImageScreenBufferProcessor
    {
        private static int RgbMask => 0xffffff;

        private static int ScreenFadeHalfMask => 0x7f7f7f;

        private static int ScreenFadeQuarterMask => 0x3f3f3f;

        private static int ScreenFadeEighthMask => 0x1f1f1f;

        private static int ScreenFadeSixteenthMask => 0x0f0f0f;

        internal static void Clear(GameImage image)
        {
            int pixelCount = image.GameWidth * image.GameHeight;

            if (!image.IsInterlaced)
            {
                for (int pixelIndex = 0;
                    pixelIndex < pixelCount;
                    pixelIndex += 1)
                {
                    image.Pixels[pixelIndex] = 0;
                }

                return;
            }

            int currentPixelIndex = 0;

            for (int rowOffset = -image.GameHeight;
                rowOffset < 0;
                rowOffset += 2)
            {
                for (int columnOffset = -image.GameWidth;
                    columnOffset < 0;
                    columnOffset += 1)
                {
                    image.Pixels[currentPixelIndex++] = 0;
                }

                currentPixelIndex += image.GameWidth;
            }
        }

        internal static void FadeToBlack(GameImage image)
        {
            int pixelCount = image.GameWidth * image.GameHeight;

            for (int pixelIndex = 0;
                pixelIndex < pixelCount;
                pixelIndex += 1)
            {
                int pixelValue = image.Pixels[pixelIndex] & RgbMask;
                image.Pixels[pixelIndex] = (int)(
                    ((uint)pixelValue >> 1 & ScreenFadeHalfMask) +
                    ((uint)pixelValue >> 2 & ScreenFadeQuarterMask) +
                    ((uint)pixelValue >> 3 & ScreenFadeEighthMask) +
                    ((uint)pixelValue >> 4 & ScreenFadeSixteenthMask));
            }
        }
    }
}