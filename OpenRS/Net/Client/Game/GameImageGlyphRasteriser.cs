using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageGlyphRasteriser
    {
        private static int GlyphDataMidMultiplier => 16384;

        private static int GlyphDataLowMultiplier => 128;

        private static int BlendThresholdLow => 30;

        private static int BlendThresholdHigh => 230;

        private static int BlendChannelScale => 256;

        private readonly GameImage gameImage;
        private readonly ILogger logger;

        internal GameImageGlyphRasteriser(GameImage gameImage, ILogger logger)
        {
            this.gameImage = gameImage;
            this.logger = logger;
        }

        internal void Draw(
            int characterOffset,
            int x,
            int y,
            int colour,
            sbyte[] fontData,
            bool useShadow)
        {
            int[] pixels = gameImage.Pixels;
            int gameWidth = gameImage.GameWidth;

            int drawX = x + fontData[characterOffset + 5];
            int drawY = y - fontData[characterOffset + 6];
            int glyphWidth = fontData[characterOffset + 3];
            int glyphHeight = fontData[characterOffset + 4];
            int dataOffset =
                fontData[characterOffset] * GlyphDataMidMultiplier +
                fontData[characterOffset + 1] * GlyphDataLowMultiplier +
                fontData[characterOffset + 2];
            int pixelOffset = drawX + drawY * gameWidth;
            int screenStride = gameWidth - glyphWidth;
            int fontStride = 0;

            if (drawY < gameImage.ImageY)
            {
                int clippedRows = gameImage.ImageY - drawY;
                glyphHeight -= clippedRows;
                drawY = gameImage.ImageY;
                dataOffset += clippedRows * glyphWidth;
                pixelOffset += clippedRows * gameWidth;
            }

            if (drawY + glyphHeight >= gameImage.ImageHeight)
            {
                glyphHeight -= drawY + glyphHeight - gameImage.ImageHeight + 1;
            }

            if (drawX < gameImage.ImageX)
            {
                int clippedCols = gameImage.ImageX - drawX;
                glyphWidth -= clippedCols;
                drawX = gameImage.ImageX;
                dataOffset += clippedCols;
                pixelOffset += clippedCols;
                fontStride += clippedCols;
                screenStride += clippedCols;
            }

            if (drawX + glyphWidth >= gameImage.ImageWidth)
            {
                int overflow = drawX + glyphWidth - gameImage.ImageWidth + 1;
                glyphWidth -= overflow;
                fontStride += overflow;
                screenStride += overflow;
            }

            if (glyphWidth <= 0 || glyphHeight <= 0)
            {
                return;
            }

            if (useShadow)
            {
                DrawBlended(
                    pixels,
                    fontData,
                    colour,
                    dataOffset,
                    pixelOffset,
                    glyphWidth,
                    glyphHeight,
                    screenStride,
                    fontStride);

                return;
            }

            DrawOpaque(
                pixels,
                fontData,
                colour,
                dataOffset,
                pixelOffset,
                glyphWidth,
                glyphHeight,
                screenStride,
                fontStride);
        }

        private void DrawOpaque(
            int[] screenPixels,
            sbyte[] fontData,
            int colour,
            int glyphOffset,
            int pixelOffset,
            int glyphWidth,
            int glyphHeight,
            int screenStride,
            int fontStride)
        {
            try
            {
                int quadGroups = -(glyphWidth >> 2);
                int remainder = -(glyphWidth & 3);

                for (int rowIndex = -glyphHeight; rowIndex < 0; rowIndex += 1)
                {
                    for (int quadIndex = quadGroups; quadIndex < 0; quadIndex += 1)
                    {
                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }

                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }

                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }

                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }
                    }

                    for (int remainderIndex = remainder;
                        remainderIndex < 0;
                        remainderIndex += 1)
                    {
                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }
                    }

                    pixelOffset += screenStride;
                    glyphOffset += fontStride;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderText,
                    "Error in the plotletter routine.",
                    exception);
            }
        }

        private static void DrawBlended(
            int[] pixels,
            sbyte[] fontData,
            int colour,
            int glyphOffset,
            int pixelOffset,
            int glyphWidth,
            int glyphHeight,
            int screenStride,
            int fontStride)
        {
            for (int rowIndex = -glyphHeight; rowIndex < 0; rowIndex += 1)
            {
                for (int columnIndex = -glyphWidth;
                    columnIndex < 0;
                    columnIndex += 1)
                {
                    int alpha = fontData[glyphOffset++] & 0xff;

                    if (alpha > BlendThresholdLow)
                    {
                        if (alpha >= BlendThresholdHigh)
                        {
                            pixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            int existingPixel = pixels[pixelOffset];
                            pixels[pixelOffset++] = (int)(
                                ((colour & 0xff00ff) * alpha +
                                    (existingPixel & 0xff00ff) *
                                    (BlendChannelScale - alpha) & 0xff00ff00) +
                                ((colour & 0xff00) * alpha +
                                    (existingPixel & 0xff00) *
                                    (BlendChannelScale - alpha) & 0xff0000) >> 8);
                        }
                    }
                    else
                    {
                        pixelOffset += 1;
                    }
                }

                pixelOffset += screenStride;
                glyphOffset += fontStride;
            }
        }
    }
}