using System;

using Microsoft.Xna.Framework.Graphics;

using NuciLog.Core;
using NuciXNA.Primitives;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    public class GameImage
    {
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImage>();
        private readonly GameImagePictureManager pictureManager;
        private readonly GameImageSpriteRenderer spriteRenderer;
        private readonly GameImageCharacterRenderer characterRenderer;
        private readonly GameImageTextRenderer textRenderer;
        private readonly GameImageMinimapRenderer minimapRenderer;

        public static int SpiralDrawCount { get; set; }

        public static int CharacterDrawCount { get; set; }

        public static int LastCharacterRotation { get; set; }

        public int GameWidth { get; set; }

        public int GameHeight { get; set; }

        public int Area { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int[] Pixels { get; set; }

        public Texture2D ImageTexture { get; set; }

        public int[][] PictureColours { get; set; }

        public sbyte[][] PictureColourIndexes { get; set; }

        public int[][] PictureColour { get; set; }

        public int[] PictureWidth { get; set; }

        public int[] PictureHeight { get; set; }

        public int[] PictureOffsetX { get; set; }

        public int[] PictureOffsetY { get; set; }

        public int[] PictureAssumedWidth { get; set; }

        public int[] PictureAssumedHeight { get; set; }

        public bool[] HasTransparentBackground { get; set; }

        public bool IsInterlaced { get; set; }

        public bool IsLoggedIn { get; set; }

        public int[] CharacterRotationTable { get; set; }

        public int[] EntityScanlineMinX { get; set; }

        public int[] EntityScanlineMaxX { get; set; }

        public int[] EntityScanlineMinValue { get; set; }

        public int[] EntityScanlineMaxValue { get; set; }

        public int[] EntityScanlineMinExtra { get; set; }

        public int[] EntityScanlineMaxExtra { get; set; }

        public Size2D GameSize => new(GameWidth, GameHeight);

        public GraphicsDevice Graphics { get; set; }

        internal int ImageX { get; set; }

        internal int ImageY { get; set; }

        internal int ImageWidth { get; set; }

        internal int ImageHeight { get; set; }

        private static int AlphaScale => 256;

        private static int ByteMask => 0xff;

        private static int RgbMask => 0xffffff;

        private static int ScreenFadeHalfMask => 0x7f7f7f;

        private static int ScreenFadeQuarterMask => 0x3f3f3f;

        private static int ScreenFadeEighthMask => 0x1f1f1f;

        private static int ScreenFadeSixteenthMask => 0x0f0f0f;

        public GameImage(int width, int height, int size)
        {
            ImageHeight = height;
            ImageWidth = width;
            Width = GameWidth = width;
            Height = GameHeight = height;
            Area = width * height;
            Pixels = new int[width * height];
            PictureColours = new int[size][];
            HasTransparentBackground = new bool[size];
            PictureColourIndexes = new sbyte[size][];
            PictureColour = new int[size][];
            PictureWidth = new int[size];
            PictureHeight = new int[size];
            PictureAssumedWidth = new int[size];
            PictureAssumedHeight = new int[size];
            PictureOffsetX = new int[size];
            PictureOffsetY = new int[size];
            pictureManager = new GameImagePictureManager(this);
            spriteRenderer = new GameImageSpriteRenderer(this);
            characterRenderer = new GameImageCharacterRenderer(this);
            textRenderer = new GameImageTextRenderer(this);
            minimapRenderer = new GameImageMinimapRenderer(this);
        }

        public static int AddFont(sbyte[] bytes)
            => GameImageTextRenderer.AddFont(bytes);

        public void SetDimensions(int x, int y, int width, int height)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (width > GameWidth)
            {
                width = GameWidth;
            }

            if (height > GameHeight)
            {
                height = GameHeight;
            }

            ImageX = x;
            ImageY = y;
            ImageWidth = width;
            ImageHeight = height;
        }

        public void ResetDimensions()
        {
            ImageX = 0;
            ImageY = 0;
            ImageWidth = GameWidth;
            ImageHeight = GameHeight;
        }

        public void ClearScreen()
        {
            int pixelCount = GameWidth * GameHeight;

            if (!IsInterlaced)
            {
                for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
                {
                    Pixels[pixelIndex] = 0;
                }

                return;
            }

            int index = 0;

            for (int rowOffset = -GameHeight; rowOffset < 0; rowOffset += 2)
            {
                for (int columnOffset = -GameWidth; columnOffset < 0; columnOffset += 1)
                {
                    Pixels[index++] = 0;
                }

                index += GameWidth;
            }
        }

        public void DrawCircle(int centreX, int centreY, int radius, int colour, int alpha)
        {
            int inverseAlpha = AlphaScale - alpha;
            int scaledRed = (colour >> 16 & ByteMask) * alpha;
            int scaledGreen = (colour >> 8 & ByteMask) * alpha;
            int scaledBlue = (colour & ByteMask) * alpha;
            int topY = centreY - radius;

            if (topY < 0)
            {
                topY = 0;
            }

            int bottomY = centreY + radius;

            if (bottomY >= GameHeight)
            {
                bottomY = GameHeight - 1;
            }

            byte rowStep = 1;

            if (IsInterlaced)
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

                if (rightX >= GameWidth)
                {
                    rightX = GameWidth - 1;
                }

                int pixelIndex = leftX + scanY * GameWidth;

                for (int columnX = leftX; columnX <= rightX; columnX += 1)
                {
                    int existingRed = (Pixels[pixelIndex] >> 16 & ByteMask) * inverseAlpha;
                    int existingGreen = (Pixels[pixelIndex] >> 8 & ByteMask) * inverseAlpha;
                    int existingBlue = (Pixels[pixelIndex] & ByteMask) * inverseAlpha;
                    int blendedColour = ((scaledRed + existingRed >> 8) << 16) + ((scaledGreen + existingGreen >> 8) << 8) + (scaledBlue + existingBlue >> 8);
                    Pixels[pixelIndex++] = blendedColour;
                }
            }
        }

        public void DrawBoxAlpha(int x, int y, int width, int height, int colour, int alpha)
        {
            if (x < ImageX)
            {
                width -= ImageX - x;
                x = ImageX;
            }

            if (y < ImageY)
            {
                height -= ImageY - y;
                y = ImageY;
            }

            if (x + width > ImageWidth)
            {
                width = ImageWidth - x;
            }

            if (y + height > ImageHeight)
            {
                height = ImageHeight - y;
            }

            int inverseAlpha = AlphaScale - alpha;
            int scaledRed = (colour >> 16 & ByteMask) * alpha;
            int scaledGreen = (colour >> 8 & ByteMask) * alpha;
            int scaledBlue = (colour & ByteMask) * alpha;
            int rowStride = GameWidth - width;
            byte rowStep = 1;

            if (IsInterlaced)
            {
                rowStep = 2;
                rowStride += GameWidth;

                if ((y & 1) != 0)
                {
                    y += 1;
                    height -= 1;
                }
            }

            int pixelIndex = x + y * GameWidth;

            for (int rowIndex = 0; rowIndex < height; rowIndex += rowStep)
            {
                for (int columnOffset = -width; columnOffset < 0; columnOffset += 1)
                {
                    int existingRed = (Pixels[pixelIndex] >> 16 & ByteMask) * inverseAlpha;
                    int existingGreen = (Pixels[pixelIndex] >> 8 & ByteMask) * inverseAlpha;
                    int existingBlue = (Pixels[pixelIndex] & ByteMask) * inverseAlpha;
                    int blendedColour = ((scaledRed + existingRed >> 8) << 16) + ((scaledGreen + existingGreen >> 8) << 8) + (scaledBlue + existingBlue >> 8);
                    Pixels[pixelIndex++] = blendedColour;
                }

                pixelIndex += rowStride;
            }
        }

        public void DrawGradientBox(int x, int y, int width, int height, int startColour, int endColour)
        {
            if (x < ImageX)
            {
                width -= ImageX - x;
                x = ImageX;
            }

            if (x + width > ImageWidth)
            {
                width = ImageWidth - x;
            }

            int endBlue = endColour >> 16 & ByteMask;
            int endGreen = endColour >> 8 & ByteMask;
            int endRed = endColour & ByteMask;
            int startBlue = startColour >> 16 & ByteMask;
            int startGreen = startColour >> 8 & ByteMask;
            int startRed = startColour & ByteMask;
            int rowStride = GameWidth - width;
            byte rowStep = 1;

            if (IsInterlaced)
            {
                rowStep = 2;
                rowStride += GameWidth;

                if ((y & 1) != 0)
                {
                    y += 1;
                    height -= 1;
                }
            }

            int pixelIndex = x + y * GameWidth;

            for (int rowIndex = 0; rowIndex < height; rowIndex += rowStep)
            {
                if (rowIndex + y >= ImageY && rowIndex + y < ImageHeight)
                {
                    int rowColour =
                        ((endBlue * rowIndex + startBlue * (height - rowIndex)) / height << 16) +
                        ((endGreen * rowIndex + startGreen * (height - rowIndex)) / height << 8) +
                        (endRed * rowIndex + startRed * (height - rowIndex)) / height;

                    for (int columnOffset = -width; columnOffset < 0; columnOffset += 1)
                    {
                        Pixels[pixelIndex++] = rowColour;
                    }

                    pixelIndex += rowStride;
                }
                else
                {
                    pixelIndex += GameWidth;
                }
            }
        }

        public void DrawBox(int x, int y, int width, int height, int colour)
        {
            if (x < ImageX)
            {
                width -= ImageX - x;
                x = ImageX;
            }

            if (y < ImageY)
            {
                height -= ImageY - y;
                y = ImageY;
            }

            if (x + width > ImageWidth)
            {
                width = ImageWidth - x;
            }

            if (y + height > ImageHeight)
            {
                height = ImageHeight - y;
            }

            int rowStride = GameWidth - width;
            byte rowStep = 1;

            if (IsInterlaced)
            {
                rowStep = 2;
                rowStride += GameWidth;

                if ((y & 1) != 0)
                {
                    y += 1;
                    height -= 1;
                }
            }

            int pixelIndex = x + y * GameWidth;

            for (int rowIndex = -height; rowIndex < 0; rowIndex += rowStep)
            {
                for (int columnOffset = -width; columnOffset < 0; columnOffset += 1)
                {
                    Pixels[pixelIndex++] = colour;
                }

                pixelIndex += rowStride;
            }
        }

        public void DrawBoxEdge(int x, int y, int width, int height, int colour)
        {
            DrawLineX(x, y, width, colour);
            DrawLineX(x, y + height - 1, width, colour);
            DrawLineY(x, y, height, colour);
            DrawLineY(x + width - 1, y, height, colour);
        }

        public void DrawLineX(int x, int y, int length, int colour)
        {
            if (y < ImageY || y >= ImageHeight)
            {
                return;
            }

            if (x < ImageX)
            {
                length -= ImageX - x;
                x = ImageX;
            }

            if (x + length > ImageWidth)
            {
                length = ImageWidth - x;
            }

            int startIndex = x + y * GameWidth;

            for (int offset = 0; offset < length; offset += 1)
            {
                Pixels[startIndex + offset] = colour;
            }
        }

        public void DrawLineY(int x, int y, int length, int colour)
        {
            if (x < ImageX || x >= ImageWidth)
            {
                return;
            }

            if (y < ImageY)
            {
                length -= ImageY - y;
                y = ImageY;
            }

            if (y + length > ImageWidth)
            {
                length = ImageHeight - y;
            }

            int startIndex = x + y * GameWidth;

            for (int offset = 0; offset < length; offset += 1)
            {
                Pixels[startIndex + offset * GameWidth] = colour;
            }
        }

        public void DrawMinimapPixel(int x, int y, int colour)
        {
            if (x < ImageX || y < ImageY || x >= ImageWidth || y >= ImageHeight)
            {
                return;
            }

            Pixels[x + y * GameWidth] = colour;
        }

        public void ScreenFadeToBlack()
        {
            int pixelCount = GameWidth * GameHeight;

            for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
            {
                int pixelValue = Pixels[pixelIndex] & RgbMask;
                Pixels[pixelIndex] = (int)(
                    ((uint)pixelValue >> 1 & ScreenFadeHalfMask) +
                    ((uint)pixelValue >> 2 & ScreenFadeQuarterMask) +
                    ((uint)pixelValue >> 3 & ScreenFadeEighthMask) +
                    ((uint)pixelValue >> 4 & ScreenFadeSixteenthMask));
            }
        }

        public void DrawTransparentLine(
            int blurRadiusX,
            int blurRadiusY,
            int destX,
            int destY,
            int areaWidth,
            int areaHeight)
        {
            for (int columnX = destX; columnX < destX + areaWidth; columnX += 1)
            {
                for (int rowY = destY; rowY < destY + areaHeight; rowY += 1)
                {
                    int totalRed = 0;
                    int totalGreen = 0;
                    int totalBlue = 0;
                    int sampleCount = 0;

                    for (int sampleX = columnX - blurRadiusX; sampleX <= columnX + blurRadiusX; sampleX += 1)
                    {
                        if (sampleX >= 0 && sampleX < GameWidth)
                        {
                            for (int sampleY = rowY - blurRadiusY; sampleY <= rowY + blurRadiusY; sampleY += 1)
                            {
                                if (sampleY >= 0 && sampleY < GameHeight)
                                {
                                    int samplePixel = Pixels[sampleX + GameWidth * sampleY];
                                    totalRed += samplePixel >> 16 & ByteMask;
                                    totalGreen += samplePixel >> 8 & ByteMask;
                                    totalBlue += samplePixel & ByteMask;
                                    sampleCount += 1;
                                }
                            }
                        }
                    }

                    Pixels[columnX + GameWidth * rowY] = (totalRed / sampleCount << 16) + (totalGreen / sampleCount << 8) + totalBlue / sampleCount;
                }
            }
        }

        public static uint RgbaToUInt(int red, int green, int blue, int alpha)
        {
            if (((red | green | blue | alpha) & -256) != 0)
            {
                red = ClampToByte32(red);
                green = ClampToByte32(green);
                blue = ClampToByte32(blue);
                alpha = ClampToByte32(alpha);
            }

            green <<= 8;
            blue <<= 0x10;
            alpha <<= 0x18;

            return (uint)(red | green | blue | alpha);
        }

        public static int RgbToInt(int red, int green, int blue)
            => (red << 16) + (green << 8) + blue;

        public void DrawPixels(int[][] pixelGrid, int drawX, int drawY, int width, int height)
        {
            for (int x = drawX; x < drawX + width; x += 1)
            {
                for (int y = drawY; y < drawY + height; y += 1)
                {
                    Pixels[x + y * GameWidth] = pixelGrid[x - drawX][y - drawY];
                }
            }
        }

        public void CleanUp()
            => pictureManager.CleanUp();

        public void UnpackImageData(int startIndex, sbyte[] imageData, sbyte[] metaData, int count)
            => pictureManager.UnpackImageData(startIndex, imageData, metaData, count);

        public void SetSleepSprite(int pictureIndex, sbyte[] spriteData)
            => pictureManager.SetSleepSprite(pictureIndex, spriteData);

        public void ApplyImage(int pictureIndex)
            => pictureManager.ApplyImage(pictureIndex);

        public void LoadImage(int pictureIndex)
            => pictureManager.LoadImage(pictureIndex);

        public void FillPicture(int pictureIndex, int x, int y, int width, int height)
            => pictureManager.FillPicture(pictureIndex, x, y, width, height);

        public void DrawImage(int pictureIndex, int x, int y, int width, int height)
            => pictureManager.DrawImage(pictureIndex, x, y, width, height);

        public void DrawPicture(int x, int y, int pictureIndex)
            => spriteRenderer.DrawPicture(x, y, pictureIndex);

        public void DrawPicture(int x, int y, int pictureIndex, int blendFactor)
            => spriteRenderer.DrawPicture(x, y, pictureIndex, blendFactor);

        public void DrawEntity(int x, int y, int width, int height, int index)
            => spriteRenderer.DrawEntity(x, y, width, height, index);

        public void DrawTransparentImage(int x, int y, int width, int height, int pictureIndex, int tintValue)
            => characterRenderer.DrawTransparentImage(x, y, width, height, pictureIndex, tintValue);

        public void DrawCharacterLegs(int x, int y, int width, int height, int animationSpriteIndex, int colour)
            => characterRenderer.DrawCharacterLegs(x, y, width, height, animationSpriteIndex, colour);

        public virtual void DrawVisibleEntity(int x, int y, int width, int height, int objectId, int unknownParam1, int unknownParam2)
            => spriteRenderer.DrawEntity(x, y, width, height, objectId);

        public virtual void DrawImage(
            int x,
            int y,
            int width,
            int height,
            int pictureIndex,
            int tint1,
            int tint2,
            int rotation,
            bool isFlipped)
            => characterRenderer.DrawImage(x, y, width, height, pictureIndex, tint1, tint2, rotation, isFlipped);

        public void DrawMinimapPic(int centreX, int centreY, int pictureIndex, int rotation, int scale)
            => minimapRenderer.DrawMinimapPic(centreX, centreY, pictureIndex, rotation, scale);

        public void DrawLabel(string text, int x, int y, int fontIndex, int colour)
            => textRenderer.DrawLabel(text, x, y, fontIndex, colour);

        public void DrawText(string text, int x, int y, int fontIndex, int colour)
            => textRenderer.DrawText(text, x, y, fontIndex, colour);

        public void DrawFloatingText(string text, int x, int y, int fontIndex, int colour, int maxWidth)
            => textRenderer.DrawFloatingText(text, x, y, fontIndex, colour, maxWidth);

        public void DrawString(string text, int x, int y, int fontIndex, int colour)
            => textRenderer.DrawString(text, x, y, fontIndex, colour);

        public int TextHeightNumber(int fontIndex)
            => textRenderer.TextHeightNumber(fontIndex);

        public int GetCharacterWidth(int fontIndex)
            => textRenderer.GetCharacterWidth(fontIndex);

        public int TextWidth(string text, int fontIndex)
            => textRenderer.TextWidth(text, fontIndex);

        private static int ClampToByte32(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 0xff)
            {
                return 0xff;
            }

            return value;
        }
    }
}
