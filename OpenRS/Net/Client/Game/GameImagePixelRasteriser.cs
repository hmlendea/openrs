namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePixelRasteriser(GameImage image)
    {
        internal void SetDimensions(int x, int y, int width, int height)
            => GameImageViewportController.Set(image, x, y, width, height);

        internal void ResetDimensions()
            => GameImageViewportController.Reset(image);

        internal void ClearScreen()
            => GameImageScreenBufferProcessor.Clear(image);

        internal void DrawBox(int x, int y, int width, int height, int colour)
        {
            GameImageRectangleClip rectangleClip =
                GameImageRectangleClip.Calculate(image, x, y, width, height);
            int pixelIndex = rectangleClip.DestinationOffset;

            for (int rowIndex = -rectangleClip.Height;
                rowIndex < 0;
                rowIndex += rectangleClip.RowStep)
            {
                for (int columnOffset = -rectangleClip.Width;
                    columnOffset < 0;
                    columnOffset += 1)
                {
                    image.Pixels[pixelIndex++] = colour;
                }

                pixelIndex += rectangleClip.RowStride;
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
            => GameImageScreenBufferProcessor.FadeToBlack(image);
    }
}