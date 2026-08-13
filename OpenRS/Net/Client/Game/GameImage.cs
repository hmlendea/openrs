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
        private readonly GameImagePixelRasteriser pixelRasteriser;
        private readonly GameImageShapeRasteriser shapeRasteriser;
        private readonly GameImageBlurProcessor blurProcessor;

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
            pixelRasteriser = new GameImagePixelRasteriser(this);
            shapeRasteriser = new GameImageShapeRasteriser(this);
            blurProcessor = new GameImageBlurProcessor(this);
        }

        public static int AddFont(sbyte[] bytes)
            => GameImageTextRenderer.AddFont(bytes);

        public void SetDimensions(int x, int y, int width, int height)
            => pixelRasteriser.SetDimensions(x, y, width, height);

        public void ResetDimensions() => pixelRasteriser.ResetDimensions();

        public void ClearScreen() => pixelRasteriser.ClearScreen();

        public void DrawCircle(int centreX, int centreY, int radius, int colour, int alpha)
            => shapeRasteriser.DrawCircle(centreX, centreY, radius, colour, alpha);

        public void DrawBoxAlpha(int x, int y, int width, int height, int colour, int alpha)
            => shapeRasteriser.DrawBoxAlpha(x, y, width, height, colour, alpha);

        public void DrawGradientBox(int x, int y, int width, int height, int startColour, int endColour)
            => shapeRasteriser.DrawGradientBox(x, y, width, height, startColour, endColour);

        public void DrawBox(int x, int y, int width, int height, int colour)
            => pixelRasteriser.DrawBox(x, y, width, height, colour);

        public void DrawBoxEdge(int x, int y, int width, int height, int colour)
            => pixelRasteriser.DrawBoxEdge(x, y, width, height, colour);

        public void DrawLineX(int x, int y, int length, int colour)
            => pixelRasteriser.DrawLineX(x, y, length, colour);

        public void DrawLineY(int x, int y, int length, int colour)
            => pixelRasteriser.DrawLineY(x, y, length, colour);

        public void DrawMinimapPixel(int x, int y, int colour)
            => pixelRasteriser.DrawMinimapPixel(x, y, colour);

        public void ScreenFadeToBlack() => pixelRasteriser.FadeToBlack();

        public void DrawTransparentLine(
            int blurRadiusX,
            int blurRadiusY,
            int destX,
            int destY,
            int areaWidth,
            int areaHeight)
            => blurProcessor.BlurArea(
                blurRadiusX,
                blurRadiusY,
                destX,
                destY,
                areaWidth,
                areaHeight);

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
            => pixelRasteriser.DrawPixels(pixelGrid, drawX, drawY, width, height);

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
