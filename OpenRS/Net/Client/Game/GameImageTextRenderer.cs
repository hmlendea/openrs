using System;
using System.Collections.Generic;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageTextRenderer
    {
        private static readonly sbyte[][] gameFonts = new sbyte[50][];
        private static readonly int[] characterFontOffsetTable;
        private static readonly bool[] fontShadowEnabled = new bool[12];
        private static int currentFont;

        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageTextRenderer>();

        static GameImageTextRenderer()
        {
            string characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"!$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
            characterFontOffsetTable = new int[256];

            for (int i = 0; i < 256; i += 1)
            {
                int k = characterSet.IndexOf((char)i);

                if (k == -1)
                {
                    k = 74;
                }

                characterFontOffsetTable[i] = k * 9;
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
        {
            DrawString(text, x - TextWidth(text, fontIndex), y, fontIndex, colour);
        }

        internal void DrawText(string text, int x, int y, int fontIndex, int colour)
        {
            DrawString(text, x - TextWidth(text, fontIndex) / 2, y, fontIndex, colour);
        }

        internal void DrawFloatingText(string text, int x, int y, int fontIndex, int colour, int maxWidth)
        {
            try
            {
                int i = 0;
                sbyte[] abyte0 = gameFonts[fontIndex];
                int k = 0;
                int l = 0;

                for (int i1 = 0; i1 < text.Length; i1 += 1)
                {
                    if (text[i1] == '@' && i1 + 4 < text.Length && text[i1 + 4] == '@')
                    {
                        i1 += 4;
                    }
                    else if (text[i1] == '~' && i1 + 4 < text.Length && text[i1 + 4] == '~')
                    {
                        i1 += 4;
                    }
                    else
                    {
                        i += abyte0[characterFontOffsetTable[text[i1]] + 7];
                    }

                    if (text[i1] == ' ')
                    {
                        l = i1;
                    }

                    if (text[i1] == '%')
                    {
                        l = i1;
                        i = 1000;
                    }

                    if (i > maxWidth)
                    {
                        if (l <= k)
                        {
                            l = i1;
                        }

                        DrawText(text.Substring(k, l), x, y, fontIndex, colour);
                        i = 0;
                        k = i1 = l + 1;
                        y += TextHeightNumber(fontIndex);
                    }
                }

                if (i > 0)
                {
                    DrawText(text[k..], x, y, fontIndex, colour);
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
                sbyte[] abyte0 = gameFonts[fontIndex];

                try
                {
                    for (int i = 0; i < text.Length; i += 1)
                    {
                        if (text[i] == '@' && i + 4 < text.Length && text[i + 4] == '@')
                        {
                            string colourCode = text.Substring(i + 1, 3).ToLower();

                            if (colourCode == "red")
                            {
                                colour = 0xff0000;
                            }
                            else if (colourCode == "lre")
                            {
                                colour = 0xff9040;
                            }
                            else if (colourCode == "yel")
                            {
                                colour = 0xffff00;
                            }
                            else if (colourCode == "gre")
                            {
                                colour = 65280;
                            }
                            else if (colourCode == "blu")
                            {
                                colour = 255;
                            }
                            else if (colourCode == "cya")
                            {
                                colour = 65535;
                            }
                            else if (colourCode == "mag")
                            {
                                colour = 0xff00ff;
                            }
                            else if (colourCode == "whi")
                            {
                                colour = 0xffffff;
                            }
                            else if (colourCode == "nor")
                            {
                                colour = 0;
                            }
                            else if (colourCode == "dre")
                            {
                                colour = 0xc00000;
                            }
                            else if (colourCode == "ora")
                            {
                                colour = 0xff9040;
                            }
                            else if (colourCode == "ran")
                            {
                                colour = (int)(new Random().NextDouble() * 16777215D);
                            }
                            else if (colourCode == "or1")
                            {
                                colour = 0xffb000;
                            }
                            else if (colourCode == "or2")
                            {
                                colour = 0xff7000;
                            }
                            else if (colourCode == "or3")
                            {
                                colour = 0xff3000;
                            }
                            else if (colourCode == "gr1")
                            {
                                colour = 0xc0ff00;
                            }
                            else if (colourCode == "gr2")
                            {
                                colour = 0x80ff00;
                            }
                            else if (colourCode == "gr3")
                            {
                                colour = 0x40ff00;
                            }

                            i += 3;
                            continue;
                        }
                        else if (text[i] == '~' && i + 4 < text.Length && text[i + 4] == '~')
                        {
                            char c = text[i + 1];
                            char c1 = text[i + 2];
                            char c2 = text[i + 3];

                            if (c >= '0' && c <= '9' && c1 >= '0' && c1 <= '9' && c2 >= '0' && c2 <= '9')
                            {
                                x = int.Parse(text.Substring(i + 1, i + 4));
                            }

                            i += 3;
                        }
                        else if (text[i] != '@' && text[i] != '~')
                        {
                            int k = characterFontOffsetTable[text[i]];

                            if (gameImage.IsLoggedIn && !fontShadowEnabled[fontIndex] && colour != 0)
                            {
                                UnpackSpriteRow(k, x + 1, y, 0, abyte0, fontShadowEnabled[fontIndex]);
                            }

                            if (gameImage.IsLoggedIn && !fontShadowEnabled[fontIndex] && colour != 0)
                            {
                                UnpackSpriteRow(k, x, y + 1, 0, abyte0, fontShadowEnabled[fontIndex]);
                            }

                            UnpackSpriteRow(k, x, y, colour, abyte0, fontShadowEnabled[fontIndex]);
                            x += abyte0[k + 7];
                        }
                    }
                }
                catch { }
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
            else
            {
                return GetCharacterWidth(fontIndex);
            }
        }

        internal int GetCharacterWidth(int fontIndex)
        {
            if (fontIndex == 0)
            {
                return gameFonts[fontIndex][8] - 2;
            }
            else
            {
                return gameFonts[fontIndex][8] - 1;
            }
        }

        internal int TextWidth(string text, int fontIndex)
        {
            int i = 0;
            sbyte[] abyte0 = gameFonts[fontIndex];

            for (int k = 0; k < text.Length; k += 1)
            {
                if (text[k] == '@' && k + 4 < text.Length && text[k + 4] == '@')
                {
                    k += 4;
                }
                else if (text[k] == '~' && k + 4 < text.Length && text[k + 4] == '~')
                {
                    k += 4;
                }
                else
                {
                    i += abyte0[characterFontOffsetTable[text[k]] + 7];
                }
            }

            return i;
        }

        private void UnpackSpriteRow(int charOffset, int x, int y, int colour, sbyte[] fontData, bool useShadow)
        {
            int[] pixels = gameImage.Pixels;
            int gameWidth = gameImage.GameWidth;

            int j1 = x + fontData[charOffset + 5];
            int k1 = y - fontData[charOffset + 6];
            int l1 = fontData[charOffset + 3];
            int i2 = fontData[charOffset + 4];
            int j2 = fontData[charOffset] * 16384 + fontData[charOffset + 1] * 128 + fontData[charOffset + 2];
            int k2 = j1 + k1 * gameWidth;
            int l2 = gameWidth - l1;
            int i3 = 0;

            if (k1 < gameImage.ImageY)
            {
                int j3 = gameImage.ImageY - k1;
                i2 -= j3;
                k1 = gameImage.ImageY;
                j2 += j3 * l1;
                k2 += j3 * gameWidth;
            }

            if (k1 + i2 >= gameImage.ImageHeight)
            {
                i2 -= k1 + i2 - gameImage.ImageHeight + 1;
            }

            if (j1 < gameImage.ImageX)
            {
                int k3 = gameImage.ImageX - j1;
                l1 -= k3;
                j1 = gameImage.ImageX;
                j2 += k3;
                k2 += k3;
                i3 += k3;
                l2 += k3;
            }

            if (j1 + l1 >= gameImage.ImageWidth)
            {
                int l3 = j1 + l1 - gameImage.ImageWidth + 1;
                l1 -= l3;
                i3 += l3;
                l2 += l3;
            }

            if (l1 > 0 && i2 > 0)
            {
                if (useShadow)
                {
                    DrawCharacterRow(pixels, fontData, colour, j2, k2, l1, i2, l2, i3);
                    return;
                }

                PlotLetter(pixels, fontData, colour, j2, k2, l1, i2, l2, i3);
            }
        }

        private void PlotLetter(int[] screenPixels, sbyte[] fontData, int colour, int glyphOffset, int pixelOffset, int glyphWidth, int glyphHeight,
                int screenStride, int fontStride)
        {
            try
            {
                int i = -(glyphWidth >> 2);
                glyphWidth = -(glyphWidth & 3);

                for (int k = -glyphHeight; k < 0; k += 1)
                {
                    for (int l = i; l < 0; l += 1)
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

                    for (int i1 = glyphWidth; i1 < 0; i1 += 1)
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

        private void DrawCharacterRow(int[] pixels, sbyte[] fontData, int colour, int glyphOffset, int pixelOffset, int glyphWidth, int glyphHeight,
                int screenStride, int fontStride)
        {
            for (int i = -glyphHeight; i < 0; i += 1)
            {
                for (int k = -glyphWidth; k < 0; k += 1)
                {
                    int l = fontData[glyphOffset++] & 0xff;

                    if (l > 30)
                    {
                        if (l >= 230)
                        {
                            pixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            int i1 = pixels[pixelOffset];
                            pixels[pixelOffset++] = (int)(((colour & 0xff00ff) * l + (i1 & 0xff00ff) * (256 - l) & 0xff00ff00) + ((colour & 0xff00) * l + (i1 & 0xff00) * (256 - l) & 0xff0000) >> 8);
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
