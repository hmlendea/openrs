namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageBlurProcessor(GameImage image)
    {
        private static int ByteMask => 0xff;

        internal void BlurArea(
            int blurRadiusX,
            int blurRadiusY,
            int destinationX,
            int destinationY,
            int areaWidth,
            int areaHeight)
        {
            for (int columnX = destinationX; columnX < destinationX + areaWidth; columnX += 1)
            {
                for (int rowY = destinationY; rowY < destinationY + areaHeight; rowY += 1)
                {
                    int totalRed = 0;
                    int totalGreen = 0;
                    int totalBlue = 0;
                    int sampleCount = 0;

                    for (int sampleX = columnX - blurRadiusX; sampleX <= columnX + blurRadiusX; sampleX += 1)
                    {
                        if (sampleX >= 0 && sampleX < image.GameWidth)
                        {
                            for (int sampleY = rowY - blurRadiusY; sampleY <= rowY + blurRadiusY; sampleY += 1)
                            {
                                if (sampleY >= 0 && sampleY < image.GameHeight)
                                {
                                    int samplePixel = image.Pixels[sampleX + image.GameWidth * sampleY];
                                    totalRed += samplePixel >> 16 & ByteMask;
                                    totalGreen += samplePixel >> 8 & ByteMask;
                                    totalBlue += samplePixel & ByteMask;
                                    sampleCount += 1;
                                }
                            }
                        }
                    }

                    image.Pixels[columnX + image.GameWidth * rowY] =
                        (totalRed / sampleCount << 16) +
                        (totalGreen / sampleCount << 8) +
                        totalBlue / sampleCount;
                }
            }
        }
    }
}