using System;

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
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep)
        {
            int groupCount = -(width >> 2);
            int remainderCount = -(width & 3);

            for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
            {
                for (int groupIndex = groupCount; groupIndex < 0; groupIndex += 1)
                {
                    int currentColour = colours[srcOffset++];

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

                for (int remainderIndex = remainderCount; remainderIndex < 0; remainderIndex += 1)
                {
                    int currentColour = colours[srcOffset++];

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

        internal static void DrawSpriteTextured(
            int[] pixels,
            sbyte[] colourIndexes,
            int[] colourLookup,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep)
        {
            int groupCount = -(width >> 2);
            int remainderCount = -(width & 3);

            for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
            {
                for (int groupIndex = groupCount; groupIndex < 0; groupIndex += 1)
                {
                    sbyte colourIndex = colourIndexes[srcOffset++];

                    if (colourIndex != 0)
                    {
                        pixels[dstOffset++] = colourLookup[colourIndex & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    colourIndex = colourIndexes[srcOffset++];

                    if (colourIndex != 0)
                    {
                        pixels[dstOffset++] = colourLookup[colourIndex & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    colourIndex = colourIndexes[srcOffset++];

                    if (colourIndex != 0)
                    {
                        pixels[dstOffset++] = colourLookup[colourIndex & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    colourIndex = colourIndexes[srcOffset++];

                    if (colourIndex != 0)
                    {
                        pixels[dstOffset++] = colourLookup[colourIndex & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                for (int remainderIndex = remainderCount; remainderIndex < 0; remainderIndex += 1)
                {
                    sbyte colourIndex = colourIndexes[srcOffset++];

                    if (colourIndex != 0)
                    {
                        pixels[dstOffset++] = colourLookup[colourIndex & 0xff];
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

        internal static void DrawSpriteTransparent(
            int[] pixels,
            int[] colours,
            int srcX,
            int srcY,
            int dstOffset,
            int dstStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int rowStep)
        {
            try
            {
                int initialSrcX = srcX;

                for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;

                    for (int colIteration = -width; colIteration < 0; colIteration += 1)
                    {
                        int currentColour = colours[(srcX >> 16) + srcRowOffset];

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
                    srcX = initialSrcX;
                    dstOffset += dstStride;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "The transparent sprite rendering has failed.",
                    exception);

                throw;
            }
        }

        internal static void DrawSpriteColorShifted(
            int[] pixels,
            int[] colours,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep,
            int blendFactor)
        {
            int blendComplement = 256 - blendFactor;

            for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
            {
                for (int colIteration = -width; colIteration < 0; colIteration += 1)
                {
                    int currentColour = colours[srcOffset++];

                    if (currentColour != 0)
                    {
                        int backgroundPixel = pixels[dstOffset];
                        int blendedRedBlue =
                            (int)((currentColour & 0xff00ff) * blendFactor +
                            (backgroundPixel & 0xff00ff) * blendComplement &
                            0xff00ff00);
                        int blendedGreen =
                            (currentColour & 0xff00) * blendFactor +
                            (backgroundPixel & 0xff00) * blendComplement &
                            0xff0000;
                        pixels[dstOffset++] = (blendedRedBlue + blendedGreen) >> 8;
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

        internal static void DrawSpriteColorShiftedTextured(
            int[] pixels,
            sbyte[] colourIndexes,
            int[] colourLookup,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep,
            int blendFactor)
        {
            int blendComplement = 256 - blendFactor;

            for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
            {
                for (int colIteration = -width; colIteration < 0; colIteration += 1)
                {
                    int colourIndex = colourIndexes[srcOffset++];

                    if (colourIndex != 0)
                    {
                        int colour = colourLookup[colourIndex & 0xff];
                        int backgroundPixel = pixels[dstOffset];
                        int blendedRedBlue =
                            (int)((colour & 0xff00ff) * blendFactor +
                            (backgroundPixel & 0xff00ff) * blendComplement &
                            0xff00ff00);
                        int blendedGreen =
                            (colour & 0xff00) * blendFactor +
                            (backgroundPixel & 0xff00) * blendComplement &
                            0xff0000;
                        pixels[dstOffset++] = (blendedRedBlue + blendedGreen) >> 8;
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

        internal static void DrawSpriteFlipped(
            int[] pixels,
            int[] colours,
            int srcX,
            int srcY,
            int dstOffset,
            int dstStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int rowStep,
            int blendFactor)
        {
            int blendComplement = 256 - blendFactor;

            try
            {
                int initialSrcX = srcX;

                for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;

                    for (int colIteration = -width; colIteration < 0; colIteration += 1)
                    {
                        int currentColour = colours[(srcX >> 16) + srcRowOffset];

                        if (currentColour != 0)
                        {
                            int backgroundPixel = pixels[dstOffset];
                            int blendedRedBlue =
                                (int)((currentColour & 0xff00ff) * blendFactor +
                                (backgroundPixel & 0xff00ff) * blendComplement &
                                0xff00ff00);
                            int blendedGreen =
                                (currentColour & 0xff00) * blendFactor +
                                (backgroundPixel & 0xff00) * blendComplement &
                                0xff0000;
                            pixels[dstOffset++] = (blendedRedBlue + blendedGreen) >> 8;
                        }
                        else
                        {
                            dstOffset += 1;
                        }

                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = initialSrcX;
                    dstOffset += dstStride;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "The flipped sprite rendering has failed.",
                    exception);

                throw;
            }
        }

        internal static void DrawSpriteFlippedColorShifted(
            int[] pixels,
            int[] colours,
            int srcX,
            int srcY,
            int dstOffset,
            int dstStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int rowStep,
            int colour)
        {
            int red = colour >> 16 & 0xff;
            int green = colour >> 8 & 0xff;
            int blue = colour & 0xff;

            try
            {
                int initialSrcX = srcX;

                for (int rowIteration = -height; rowIteration < 0; rowIteration += rowStep)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;

                    for (int colIteration = -width; colIteration < 0; colIteration += 1)
                    {
                        int currentColour = colours[(srcX >> 16) + srcRowOffset];

                        if (currentColour != 0)
                        {
                            int redComponent = currentColour >> 16 & 0xff;
                            int greenComponent = currentColour >> 8 & 0xff;
                            int blueComponent = currentColour & 0xff;

                            if (redComponent == greenComponent && greenComponent == blueComponent)
                            {
                                pixels[dstOffset++] =
                                    ((redComponent * red >> 8) << 16) +
                                    ((greenComponent * green >> 8) << 8) +
                                    (blueComponent * blue >> 8);
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
                    srcX = initialSrcX;
                    dstOffset += dstStride;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "The colour-shifted flipped sprite rendering has failed.",
                    exception);

                throw;
            }
        }

        internal static void DrawSpriteAlpha(
            int[] pixels,
            int[] colours,
            int dstOffset,
            int srcX,
            int srcY,
            int xStep,
            int yStep,
            int count,
            int srcWidth)
        {
            for (int pixelIndex = count; pixelIndex < 0; pixelIndex += 1)
            {
                pixels[dstOffset++] = colours[(srcX >> 17) + (srcY >> 17) * srcWidth];
                srcX += xStep;
                srcY += yStep;
            }
        }

        internal static void DrawSpriteAlphaColorShifted(
            int[] pixels,
            int[] colours,
            int dstOffset,
            int srcX,
            int srcY,
            int xStep,
            int yStep,
            int count,
            int srcWidth)
        {
            for (int pixelIndex = count; pixelIndex < 0; pixelIndex += 1)
            {
                int currentColour = colours[(srcX >> 17) + (srcY >> 17) * srcWidth];

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
