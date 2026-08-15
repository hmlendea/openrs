using System;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageShapeRasteriser(GameImage image)
    {
        private static int ByteMask => 0xff;

        internal void DrawCircle(int centreX, int centreY, int radius, int colour, int alpha)
        {
            GameImageAlphaBlender alphaBlender = new(colour, alpha);
            int topY = centreY - radius;

            if (topY < 0)
            {
                topY = 0;
            }

            int bottomY = centreY + radius;

            if (bottomY >= image.GameHeight)
            {
                bottomY = image.GameHeight - 1;
            }

            byte rowStep = 1;

            if (image.IsInterlaced)
            {
                rowStep = 2;

                if ((topY & 1) != 0)
                {
                    topY += 1;
                }
            }

            for (int scanY = topY; scanY <= bottomY; scanY += rowStep)
            {
                int relativeY = scanY - centreY;
                int halfWidth = (int)Math.Sqrt(radius * radius - relativeY * relativeY);
                int leftX = centreX - halfWidth;

                if (leftX < 0)
                {
                    leftX = 0;
                }

                int rightX = centreX + halfWidth;

                if (rightX >= image.GameWidth)
                {
                    rightX = image.GameWidth - 1;
                }

                int pixelIndex = leftX + scanY * image.GameWidth;

                for (int columnX = leftX; columnX <= rightX; columnX += 1)
                {
                    image.Pixels[pixelIndex] =
                        alphaBlender.Blend(image.Pixels[pixelIndex]);
                    pixelIndex += 1;
                }
            }
        }

        internal void DrawBoxAlpha(int x, int y, int width, int height, int colour, int alpha)
        {
            GameImageRectangleClip rectangleClip =
                GameImageRectangleClip.Calculate(image, x, y, width, height);
            GameImageAlphaBlender alphaBlender = new(colour, alpha);
            int pixelIndex = rectangleClip.DestinationOffset;

            for (int rowIndex = 0;
                rowIndex < rectangleClip.Height;
                rowIndex += rectangleClip.RowStep)
            {
                for (int columnOffset = -rectangleClip.Width;
                    columnOffset < 0;
                    columnOffset += 1)
                {
                    image.Pixels[pixelIndex] =
                        alphaBlender.Blend(image.Pixels[pixelIndex]);
                    pixelIndex += 1;
                }

                pixelIndex += rectangleClip.RowStride;
            }
        }

        internal void DrawGradientBox(
            int x,
            int y,
            int width,
            int height,
            int startColour,
            int endColour)
        {
            if (x < image.ImageX)
            {
                width -= image.ImageX - x;
                x = image.ImageX;
            }

            if (x + width > image.ImageWidth)
            {
                width = image.ImageWidth - x;
            }

            int endBlue = endColour >> 16 & ByteMask;
            int endGreen = endColour >> 8 & ByteMask;
            int endRed = endColour & ByteMask;
            int startBlue = startColour >> 16 & ByteMask;
            int startGreen = startColour >> 8 & ByteMask;
            int startRed = startColour & ByteMask;
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

            for (int rowIndex = 0; rowIndex < height; rowIndex += rowStep)
            {
                if (rowIndex + y >= image.ImageY && rowIndex + y < image.ImageHeight)
                {
                    int rowColour =
                        ((endBlue * rowIndex + startBlue * (height - rowIndex)) / height << 16) +
                        ((endGreen * rowIndex + startGreen * (height - rowIndex)) / height << 8) +
                        (endRed * rowIndex + startRed * (height - rowIndex)) / height;

                    for (int columnOffset = -width; columnOffset < 0; columnOffset += 1)
                    {
                        image.Pixels[pixelIndex++] = rowColour;
                    }

                    pixelIndex += rowStride;
                }
                else
                {
                    pixelIndex += image.GameWidth;
                }
            }
        }
    }
}