using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageTextRenderer
    {
        private static int MaxFontCount => 50;
        private static int CharacterTableSize => 256;
        private static int FontEntryStride => 9;
        private static int FontFallbackIndex => 74;
        private static int MaxFontShadowCount => 12;
        private static int GlyphDataBitShiftHigh => 14;
        private static int GlyphDataBitShiftMid => 7;
        private static int GlyphDataMidMultiplier => 16384;
        private static int GlyphDataLowMultiplier => 128;
        private static int BlendThresholdLow => 30;
        private static int BlendThresholdHigh => 230;
        private static int BlendChannelScale => 256;
        private static int FloatingTextMaxWidthOverride => 1000;
        private static int ColourMaxRgb => 16777215;

        private static readonly sbyte[][] gameFonts = new sbyte[MaxFontCount][];
        private static readonly int[] characterFontOffsetTable;
        private static readonly bool[] fontShadowEnabled = new bool[MaxFontShadowCount];
        private static int currentFont;

        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageTextRenderer>();

        static GameImageTextRenderer()
        {
            string characterSet =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz" +
                "0123456789!\"!$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
            characterFontOffsetTable = new int[CharacterTableSize];

            for (int charIndex = 0; charIndex < CharacterTableSize; charIndex += 1)
            {
                int setIndex = characterSet.IndexOf((char)charIndex);

                if (setIndex == -1)
                {
                    setIndex = FontFallbackIndex;
                }

                characterFontOffsetTable[charIndex] = setIndex * FontEntryStride;
            }
        }

        internal GameImageTextRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal static int AddFont(sbyte[] bytes)
        {
            gameFonts[currentFont] = bytes;
            currentFont += 1;

            return currentFont - 1;
        }

        internal void DrawLabel(string text, int x, int y, int fontIndex, int colour)
            => DrawString(text, x - TextWidth(text, fontIndex), y, fontIndex, colour);

        internal void DrawText(string text, int x, int y, int fontIndex, int colour)
            => DrawString(text, x - TextWidth(text, fontIndex) / 2, y, fontIndex, colour);

        internal void DrawFloatingText(string text, int x, int y, int fontIndex, int colour, int maxWidth)
        {
            try
            {
                sbyte[] fontData = gameFonts[fontIndex];
                int currentWidth = 0;
                int lineStartIndex = 0;
                int lastWordBreak = 0;

                for (int charIndex = 0; charIndex < text.Length; charIndex += 1)
                {
                    if (text[charIndex] == '@' && charIndex + 4 < text.Length && text[charIndex + 4] == '@')
                    {
                        charIndex += 4;
                    }
                    else if (text[charIndex] == '~' && charIndex + 4 < text.Length && text[charIndex + 4] == '~')
                    {
                        charIndex += 4;
                    }
                    else
                    {
                        currentWidth += fontData[characterFontOffsetTable[text[charIndex]] + 7];
                    }

                    if (text[charIndex] == ' ')
                    {
                        lastWordBreak = charIndex;
                    }

                    if (text[charIndex] == '%')
                    {
                        lastWordBreak = charIndex;
                        currentWidth = FloatingTextMaxWidthOverride;
                    }

                    if (currentWidth > maxWidth)
                    {
                        if (lastWordBreak <= lineStartIndex)
                        {
                            lastWordBreak = charIndex;
                        }

                        DrawText(text.Substring(lineStartIndex, lastWordBreak), x, y, fontIndex, colour);
                        currentWidth = 0;
                        lineStartIndex = charIndex = lastWordBreak + 1;
                        y += TextHeightNumber(fontIndex);
                    }
                }

                if (currentWidth > 0)
                {
                    DrawText(text[lineStartIndex..], x, y, fontIndex, colour);
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderText, "Error in the centrepara routine.", exception);
            }
        }

        internal void DrawString(string text, int x, int y, int fontIndex, int colour)
        {
            try
            {
                sbyte[] fontData = gameFonts[fontIndex];

                for (int charIndex = 0; charIndex < text.Length; charIndex += 1)
                {
                    if (text[charIndex] == '@' && charIndex + 4 < text.Length && text[charIndex + 4] == '@')
                    {
                        string colourCode = text.Substring(charIndex + 1, 3).ToLower();

                        colour = colourCode switch
                        {
                            "red" => 0xff0000,
                            "lre" => 0xff9040,
                            "yel" => 0xffff00,
                            "gre" => 0x00ff00,
                            "blu" => 0x0000ff,
                            "cya" => 0x00ffff,
                            "mag" => 0xff00ff,
                            "whi" => 0xffffff,
                            "nor" => 0,
                            "dre" => 0xc00000,
                            "ora" => 0xff9040,
                            "ran" => (int)(new Random().NextDouble() * ColourMaxRgb),
                            "or1" => 0xffb000,
                            "or2" => 0xff7000,
                            "or3" => 0xff3000,
                            "gr1" => 0xc0ff00,
                            "gr2" => 0x80ff00,
                            "gr3" => 0x40ff00,
                            _ => colour
                        };

                        charIndex += 3;
                        continue;
                    }
                    else if (text[charIndex] == '~' && charIndex + 4 < text.Length && text[charIndex + 4] == '~')
                    {
                        char digit0 = text[charIndex + 1];
                        char digit1 = text[charIndex + 2];
                        char digit2 = text[charIndex + 3];

                        if (digit0 >= '0' && digit0 <= '9' &&
                            digit1 >= '0' && digit1 <= '9' &&
                            digit2 >= '0' && digit2 <= '9')
                        {
                            x = int.Parse(text.Substring(charIndex + 1, 3));
                        }

                        charIndex += 3;
                    }
                    else if (text[charIndex] != '@' && text[charIndex] != '~')
                    {
                        int glyphOffset = characterFontOffsetTable[text[charIndex]];
                        bool hasShadow = fontShadowEnabled[fontIndex];

                        if (gameImage.IsLoggedIn && !hasShadow && colour != 0)
                        {
                            UnpackSpriteRow(glyphOffset, x + 1, y, 0, fontData, hasShadow);
                        }

                        if (gameImage.IsLoggedIn && !hasShadow && colour != 0)
                        {
                            UnpackSpriteRow(glyphOffset, x, y + 1, 0, fontData, hasShadow);
                        }

                        UnpackSpriteRow(glyphOffset, x, y, colour, fontData, hasShadow);
                        x += fontData[glyphOffset + 7];
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderText, "Error in the drawstring routine.", exception);
            }
        }

        internal int TextHeightNumber(int fontIndex)
        {
            if (fontIndex == 0)
            {
                return 12;
            }

            if (fontIndex == 1)
            {
                return 14;
            }

            if (fontIndex == 2)
            {
                return 14;
            }

            if (fontIndex == 3)
            {
                return 15;
            }

            if (fontIndex == 4)
            {
                return 15;
            }

            if (fontIndex == 5)
            {
                return 19;
            }

            if (fontIndex == 6)
            {
                return 24;
            }

            if (fontIndex == 7)
            {
                return 29;
            }

            return GetCharacterWidth(fontIndex);
        }

        internal int GetCharacterWidth(int fontIndex)
        {
            if (fontIndex == 0)
            {
                return gameFonts[fontIndex][8] - 2;
            }

            return gameFonts[fontIndex][8] - 1;
        }

        internal int TextWidth(string text, int fontIndex)
        {
            int totalWidth = 0;
            sbyte[] fontData = gameFonts[fontIndex];

            for (int charIndex = 0; charIndex < text.Length; charIndex += 1)
            {
                if (text[charIndex] == '@' && charIndex + 4 < text.Length && text[charIndex + 4] == '@')
                {
                    charIndex += 4;
                }
                else if (text[charIndex] == '~' && charIndex + 4 < text.Length && text[charIndex + 4] == '~')
                {
                    charIndex += 4;
                }
                else
                {
                    totalWidth += fontData[characterFontOffsetTable[text[charIndex]] + 7];
                }
            }

            return totalWidth;
        }

        private void UnpackSpriteRow(int charOffset, int x, int y, int colour, sbyte[] fontData, bool useShadow)
        {
            int[] pixels = gameImage.Pixels;
            int gameWidth = gameImage.GameWidth;

            int drawX = x + fontData[charOffset + 5];
            int drawY = y - fontData[charOffset + 6];
            int glyphWidth = fontData[charOffset + 3];
            int glyphHeight = fontData[charOffset + 4];
            int dataOffset =
                fontData[charOffset] * GlyphDataMidMultiplier +
                fontData[charOffset + 1] * GlyphDataLowMultiplier +
                fontData[charOffset + 2];
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

            if (glyphWidth > 0 && glyphHeight > 0)
            {
                if (useShadow)
                {
                    DrawCharacterRow(
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

                PlotLetter(
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
        }

        private void PlotLetter(
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

                    for (int remainderIndex = remainder; remainderIndex < 0; remainderIndex += 1)
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
                logger.Error(GameOperation.RenderText, "Error in the plotletter routine.", exception);
            }
        }

        private void DrawCharacterRow(
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
                for (int colIndex = -glyphWidth; colIndex < 0; colIndex += 1)
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
                                ((colour & 0xff00ff) * alpha + (existingPixel & 0xff00ff) * (BlendChannelScale - alpha) & 0xff00ff00) +
                                ((colour & 0xff00) * alpha + (existingPixel & 0xff00) * (BlendChannelScale - alpha) & 0xff0000) >> 8);
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
