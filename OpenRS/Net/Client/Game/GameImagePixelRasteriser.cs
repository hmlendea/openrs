namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePixelRasteriser(GameImage image)
    {
        private static int RgbMask => 0xffffff;

        private static int ScreenFadeHalfMask => 0x7f7f7f;

        private static int ScreenFadeQuarterMask => 0x3f3f3f;

        private static int ScreenFadeEighthMask => 0x1f1f1f;

        private static int ScreenFadeSixteenthMask => 0x0f0f0f;

        internal void SetDimensions(int x, int y, int width, int height)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (width > image.GameWidth)
            {
                width = image.GameWidth;
            }

            if (height > image.GameHeight)
            {
                height = image.GameHeight;
            }

            image.ImageX = x;
            image.ImageY = y;
            image.ImageWidth = width;
            image.ImageHeight = height;
        }

        internal void ResetDimensions()
        {
            image.ImageX = 0;
            image.ImageY = 0;
            image.ImageWidth = image.GameWidth;
            image.ImageHeight = image.GameHeight;
        }

        internal void ClearScreen()
        {
            int pixelCount = image.GameWidth * image.GameHeight;

            if (!image.IsInterlaced)
            {
                for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
                {
                    image.Pixels[pixelIndex] = 0;
                }

                return;
            }

            int currentPixelIndex = 0;

            for (int rowOffset = -image.GameHeight; rowOffset < 0; rowOffset += 2)
            {
                for (int columnOffset = -image.GameWidth; columnOffset < 0; columnOffset += 1)
                {
                    image.Pixels[currentPixelIndex++] = 0;
                }

                currentPixelIndex += image.GameWidth;
            }
        }

        internal void DrawBox(int x, int y, int width, int height, int colour)
        {
            if (x < image.ImageX)
            {
                width -= image.ImageX - x;
                x = image.ImageX;
            }

            if (y < image.ImageY)
            {
                height -= image.ImageY - y;
                y = image.ImageY;
            }

            if (x + width > image.ImageWidth)
            {
                width = image.ImageWidth - x;
            }

            if (y + height > image.ImageHeight)
            {
                height = image.ImageHeight - y;
            }

            int rowStride = image.GameWidth - width;
            byte rowStep = 1;

            if (image.IsInterlaced)
            {
                rowStep = 2;
                rowStride += image.GameWidth;

                if ((y & 1) != 0)
                {
                    y += 1;
                    height -= 1;
                }
            }

            int pixelIndex = x + y * image.GameWidth;

            for (int rowIndex = -height; rowIndex < 0; rowIndex += rowStep)
            {
                for (int columnOffset = -width; columnOffset < 0; columnOffset += 1)
                {
                    image.Pixels[pixelIndex++] = colour;
                }

                pixelIndex += rowStride;
            }
        }

        internal void DrawBoxEdge(int x, int y, int width, int height, int colour)
        {
            DrawLineX(x, y, width, colour);
            DrawLineX(x, y + height - 1, width, colour);
            DrawLineY(x, y, height, colour);
            DrawLineY(x + width - 1, y, height, colour);
        }

        internal void DrawLineX(int x, int y, int length, int colour)
        {
            if (y < image.ImageY || y >= image.ImageHeight)
            {
                return;
            }

            if (x < image.ImageX)
            {
                length -= image.ImageX - x;
                x = image.ImageX;
            }

            if (x + length > image.ImageWidth)
            {
                length = image.ImageWidth - x;
            }

            int startIndex = x + y * image.GameWidth;

            for (int offset = 0; offset < length; offset += 1)
            {
                image.Pixels[startIndex + offset] = colour;
            }
        }

        internal void DrawLineY(int x, int y, int length, int colour)
        {
            if (x < image.ImageX || x >= image.ImageWidth)
            {
                return;
            }

            if (y < image.ImageY)
            {
                length -= image.ImageY - y;
                y = image.ImageY;
            }

            if (y + length > image.ImageHeight)
            {
                length = image.ImageHeight - y;
            }

            int startIndex = x + y * image.GameWidth;

            for (int offset = 0; offset < length; offset += 1)
            {
                image.Pixels[startIndex + offset * image.GameWidth] = colour;
            }
        }

        internal void DrawMinimapPixel(int x, int y, int colour)
        {
            if (x < image.ImageX || y < image.ImageY || x >= image.ImageWidth || y >= image.ImageHeight)
            {
                return;
            }

            image.Pixels[x + y * image.GameWidth] = colour;
        }

        internal void DrawPixels(int[][] pixelGrid, int drawX, int drawY, int width, int height)
        {
            for (int positionX = drawX; positionX < drawX + width; positionX += 1)
            {
                for (int positionY = drawY; positionY < drawY + height; positionY += 1)
                {
                    image.Pixels[positionX + positionY * image.GameWidth] =
                        pixelGrid[positionX - drawX][positionY - drawY];
                }
            }
        }

        internal void FadeToBlack()
        {
            int pixelCount = image.GameWidth * image.GameHeight;

            for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
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