using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageTextRenderer
    {
        private static int FloatingTextMaxWidthOverride => 1000;

        private readonly GameImage gameImage;
        private readonly GameImageGlyphRasteriser glyphRasteriser;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageTextRenderer>();

        internal GameImageTextRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
            glyphRasteriser = new GameImageGlyphRasteriser(gameImage, logger);
        }

        internal static int AddFont(sbyte[] bytes)
            => GameImageFontRegistry.AddFont(bytes);

        internal void DrawLabel(string text, int x, int y, int fontIndex, int colour)
            => DrawString(text, x - TextWidth(text, fontIndex), y, fontIndex, colour);

        internal void DrawText(string text, int x, int y, int fontIndex, int colour)
            => DrawString(text, x - TextWidth(text, fontIndex) / 2, y, fontIndex, colour);

        internal void DrawFloatingText(string text, int x, int y, int fontIndex, int colour, int maxWidth)
        {
            try
            {
                sbyte[] fontData = GameImageFontRegistry.GetFont(fontIndex);
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
                        currentWidth += fontData[
                            GameImageFontRegistry.GetCharacterOffset(text[charIndex]) + 7];
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
                sbyte[] fontData = GameImageFontRegistry.GetFont(fontIndex);

                for (int charIndex = 0; charIndex < text.Length; charIndex += 1)
                {
                    if (text[charIndex] == '@' && charIndex + 4 < text.Length && text[charIndex + 4] == '@')
                    {
                        string colourCode = text.Substring(charIndex + 1, 3).ToLower();
                        colour = GameImageTextColourResolver.Resolve(
                            colourCode,
                            colour);

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
                        int glyphOffset = GameImageFontRegistry.GetCharacterOffset(
                            text[charIndex]);
                        bool hasShadow = GameImageFontRegistry.IsShadowEnabled(fontIndex);

                        if (gameImage.IsLoggedIn && !hasShadow && colour != 0)
                        {
                            glyphRasteriser.Draw(
                                glyphOffset,
                                x + 1,
                                y,
                                0,
                                fontData,
                                hasShadow);
                        }

                        if (gameImage.IsLoggedIn && !hasShadow && colour != 0)
                        {
                            glyphRasteriser.Draw(
                                glyphOffset,
                                x,
                                y + 1,
                                0,
                                fontData,
                                hasShadow);
                        }

                        glyphRasteriser.Draw(
                            glyphOffset,
                            x,
                            y,
                            colour,
                            fontData,
                            hasShadow);
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
                return GameImageFontRegistry.GetFont(fontIndex)[8] - 2;
            }

            return GameImageFontRegistry.GetFont(fontIndex)[8] - 1;
        }

        internal int TextWidth(string text, int fontIndex)
        {
            int totalWidth = 0;
            sbyte[] fontData = GameImageFontRegistry.GetFont(fontIndex);

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
                    totalWidth += fontData[
                        GameImageFontRegistry.GetCharacterOffset(text[charIndex]) + 7];
                }
            }

            return totalWidth;
        }

    }
}
