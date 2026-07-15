using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageSpriteBlitter
    {
        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageSpriteBlitter>();

        internal static void DrawSpriteOpaque(
            int[] pixels,
            int[] colours,
            int currentColour,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep)
        {
            int i = -(width >> 2);
            width = -(width & 3);

            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = i; l < 0; l += 1)
                {
                    currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                for (int i1 = width; i1 < 0; i1 += 1)
                {
                    currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }
        }

        internal static void DrawSpriteTextured(int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep)
        {
            int i = -(width >> 2);
            width = -(width & 3);

            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = i; l < 0; l += 1)
                {
                    sbyte byte0 = colourIndexes[srcOffset++];

                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    byte0 = colourIndexes[srcOffset++];

                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    byte0 = colourIndexes[srcOffset++];

                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    byte0 = colourIndexes[srcOffset++];

                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                for (int i1 = width; i1 < 0; i1 += 1)
                {
                    sbyte byte1 = colourIndexes[srcOffset++];

                    if (byte1 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte1 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }
        }

        internal static void DrawSpriteTransparent(int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int dstStride,
                int width, int height, int xStep, int yStep, int srcWidth, int rowStep)
        {
            try
            {
                int i = srcX;

                for (int k = -height; k < 0; k += rowStep)
                {
                    int l = (srcY >> 16) * srcWidth;

                    for (int i1 = -width; i1 < 0; i1 += 1)
                    {
                        currentColour = colours[(srcX >> 16) + l];

                        if (currentColour != 0)
                        {
                            pixels[dstOffset++] = currentColour;
                        }
                        else
                        {
                            dstOffset += 1;
                        }

                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = i;
                    dstOffset += dstStride;
                }
            }
            catch (System.Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the plot_scale routine.");
            }
        }

        internal static void DrawSpriteColorShifted(int[] pixels, int[] colours, int currentColour, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep, int blendFactor)
        {
            int i = 256 - blendFactor;

            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = -width; l < 0; l += 1)
                {
                    currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        int i1 = pixels[dstOffset];
                        pixels[dstOffset++] = (int)(((currentColour & 0xff00ff) * blendFactor + (i1 & 0xff00ff) * i & 0xff00ff00) + ((currentColour & 0xff00) * blendFactor + (i1 & 0xff00) * i & 0xff0000) >> 8);
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }
        }

        internal static void DrawSpriteColorShiftedTextured(int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep, int blendFactor)
        {
            int i = 256 - blendFactor;

            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = -width; l < 0; l += 1)
                {
                    int i1 = colourIndexes[srcOffset++];

                    if (i1 != 0)
                    {
                        i1 = colourLookup[i1 & 0xff];
                        int j1 = pixels[dstOffset];
                        pixels[dstOffset++] = (int)(((i1 & 0xff00ff) * blendFactor + (j1 & 0xff00ff) * i & 0xff00ff00) + ((i1 & 0xff00) * blendFactor + (j1 & 0xff00) * i & 0xff0000) >> 8);
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }
        }

        internal static void DrawSpriteFlipped(int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int dstStride,
                int width, int height, int xStep, int yStep, int srcWidth, int rowStep, int blendFactor)
        {
            int i = 256 - blendFactor;

            try
            {
                int k = srcX;

                for (int l = -height; l < 0; l += rowStep)
                {
                    int i1 = (srcY >> 16) * srcWidth;

                    for (int j1 = -width; j1 < 0; j1 += 1)
                    {
                        currentColour = colours[(srcX >> 16) + i1];

                        if (currentColour != 0)
                        {
                            int k1 = pixels[dstOffset];
                            pixels[dstOffset++] = (int)(((currentColour & 0xff00ff) * blendFactor + (k1 & 0xff00ff) * i & 0xff00ff00) + ((currentColour & 0xff00) * blendFactor + (k1 & 0xff00) * i & 0xff0000) >> 8);
                        }
                        else
                        {
                            dstOffset += 1;
                        }

                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = k;
                    dstOffset += dstStride;
                }
            }
            catch (System.Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the tran_scale routine.");
            }
        }

        internal static void DrawSpriteFlippedColorShifted(int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int dstStride,
                int width, int height, int xStep, int yStep, int srcWidth, int rowStep, int color)
        {
            int red = color >> 16 & 0xff;
            int green = color >> 8 & 0xff;
            int blue = color & 0xff;

            try
            {
                int i1 = srcX;

                for (int j1 = -height; j1 < 0; j1 += rowStep)
                {
                    int k1 = (srcY >> 16) * srcWidth;

                    for (int l1 = -width; l1 < 0; l1 += 1)
                    {
                        currentColour = colours[(srcX >> 16) + k1];

                        if (currentColour != 0)
                        {
                            int i2 = currentColour >> 16 & 0xff;
                            int j2 = currentColour >> 8 & 0xff;
                            int k2 = currentColour & 0xff;

                            if (i2 == j2 && j2 == k2)
                            {
                                pixels[dstOffset++] = ((i2 * red >> 8) << 16) + ((j2 * green >> 8) << 8) + (k2 * blue >> 8);
                            }
                            else
                            {
                                pixels[dstOffset++] = currentColour;
                            }
                        }
                        else
                        {
                            dstOffset += 1;
                        }

                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = i1;
                    dstOffset += dstStride;
                }
            }
            catch (System.Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the plot_scale routine.");
            }
        }

        internal static void DrawSpriteAlpha(int[] pixels, int[] colours, int currentColour, int dstOffset, int srcX, int srcY, int xStep,
                int yStep, int count, int srcWidth)
        {
            for (currentColour = count; currentColour < 0; currentColour += 1)
            {
                pixels[dstOffset++] = colours[(srcX >> 17) + (srcY >> 17) * srcWidth];
                srcX += xStep;
                srcY += yStep;
            }
        }

        internal static void DrawSpriteAlphaColorShifted(int[] pixels, int[] colours, int currentColour, int dstOffset, int srcX, int srcY, int xStep,
                int yStep, int count, int srcWidth)
        {
            for (int i = count; i < 0; i += 1)
            {
                currentColour = colours[(srcX >> 17) + (srcY >> 17) * srcWidth];

                if (currentColour != 0)
                {
                    pixels[dstOffset++] = currentColour;
                }
                else
                {
                    dstOffset += 1;
                }

                srcX += xStep;
                srcY += yStep;
            }
        }
    }
}
