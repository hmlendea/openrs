using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageScaledSpriteBlitter
    {


        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageScaledSpriteBlitter>();

        internal static void DrawSpriteFlatShaded(
            int[] pixels,
            int[] colours,
            int currentColour,
            int srcX,
            int srcY,
            int dstOffset,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int primaryColour,
            int xPosFixed,
            int xPosStep,
            int interlaceFlag,
            int imageX,
            int imageWidth,
            int gameWidth)
        {
            int primaryRed = primaryColour >> 16 & 0xff;
            int primaryGreen = primaryColour >> 8 & 0xff;
            int primaryBlue = primaryColour & 0xff;

            try
            {
                int initialSrcX = srcX;

                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    int currentX = xPosFixed >> 16;
                    int clippedWidth = width;

                    if (currentX < imageX)
                    {
                        int leftClipAmount = imageX - currentX;
                        clippedWidth -= leftClipAmount;
                        currentX = imageX;
                        srcX += xStep * leftClipAmount;
                    }

                    if (currentX + clippedWidth >= imageWidth)
                    {
                        int rightClipAmount = currentX + clippedWidth - imageWidth;
                        clippedWidth -= rightClipAmount;
                    }

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int pixelEndX = currentX + clippedWidth;

                        for (int pixelX = currentX; pixelX < pixelEndX; pixelX += 1)
                        {
                            currentColour = colours[(srcX >> 16) + srcRowOffset];

                            if (currentColour != 0)
                            {
                                int red = currentColour >> 16 & 0xff;
                                int green = currentColour >> 8 & 0xff;
                                int blue = currentColour & 0xff;

                                if (red == green && green == blue)
                                {
                                    pixels[pixelX + dstOffset] =
                                        ((red * primaryRed >> 8) << 16) +
                                        ((green * primaryGreen >> 8) << 8) +
                                        (blue * primaryBlue >> 8);
                                }
                                else
                                {
                                    pixels[pixelX + dstOffset] = currentColour;
                                }
                            }

                            srcX += xStep;
                        }
                    }

                    srcY += yStep;
                    srcX = initialSrcX;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.",
                    exception);

                throw;
            }
        }

        internal static void DrawSpriteFlatShadedAlt(
            int[] pixels,
            int[] colours,
            int currentColour,
            int srcX,
            int srcY,
            int dstOffset,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int primaryColour,
            int secondaryColour,
            int xPosFixed,
            int xPosStep,
            int interlaceFlag,
            int imageX,
            int imageWidth,
            int gameWidth)
        {
            int primaryRed = primaryColour >> 16 & 0xff;
            int primaryGreen = primaryColour >> 8 & 0xff;
            int primaryBlue = primaryColour & 0xff;
            int secondaryRed = secondaryColour >> 16 & 0xff;
            int secondaryGreen = secondaryColour >> 8 & 0xff;
            int secondaryBlue = secondaryColour & 0xff;

            try
            {
                int initialSrcX = srcX;

                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    int currentX = xPosFixed >> 16;
                    int clippedWidth = width;

                    if (currentX < imageX)
                    {
                        int leftClipAmount = imageX - currentX;
                        clippedWidth -= leftClipAmount;
                        currentX = imageX;
                        srcX += xStep * leftClipAmount;
                    }

                    if (currentX + clippedWidth >= imageWidth)
                    {
                        int rightClipAmount = currentX + clippedWidth - imageWidth;
                        clippedWidth -= rightClipAmount;
                    }

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int pixelEndX = currentX + clippedWidth;

                        for (int pixelX = currentX; pixelX < pixelEndX; pixelX += 1)
                        {
                            currentColour = colours[(srcX >> 16) + srcRowOffset];

                            if (currentColour != 0)
                            {
                                int red = currentColour >> 16 & 0xff;
                                int green = currentColour >> 8 & 0xff;
                                int blue = currentColour & 0xff;

                                if (red == green && green == blue)
                                {
                                    pixels[pixelX + dstOffset] =
                                        ((red * primaryRed >> 8) << 16) +
                                        ((green * primaryGreen >> 8) << 8) +
                                        (blue * primaryBlue >> 8);
                                }
                                else if (red == 255 && green == blue)
                                {
                                    pixels[pixelX + dstOffset] =
                                        ((red * secondaryRed >> 8) << 16) +
                                        ((green * secondaryGreen >> 8) << 8) +
                                        (blue * secondaryBlue >> 8);
                                }
                                else
                                {
                                    pixels[pixelX + dstOffset] = currentColour;
                                }
                            }

                            srcX += xStep;
                        }
                    }

                    srcY += yStep;
                    srcX = initialSrcX;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.",
                    exception);

                throw;
            }
        }

        internal static void DrawSpriteFlatShadedTextured(
            int[] pixels,
            sbyte[] colourIndexes,
            int[] colourLookup,
            int currentColour,
            int srcX,
            int srcY,
            int dstOffset,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int primaryColour,
            int xPosFixed,
            int xPosStep,
            int interlaceFlag,
            int imageX,
            int imageWidth,
            int gameWidth)
        {
            int primaryRed = primaryColour >> 16 & 0xff;
            int primaryGreen = primaryColour >> 8 & 0xff;
            int primaryBlue = primaryColour & 0xff;

            try
            {
                int initialSrcX = srcX;

                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    int currentX = xPosFixed >> 16;
                    int clippedWidth = width;

                    if (currentX < imageX)
                    {
                        int leftClipAmount = imageX - currentX;
                        clippedWidth -= leftClipAmount;
                        currentX = imageX;
                        srcX += xStep * leftClipAmount;
                    }

                    if (currentX + clippedWidth >= imageWidth)
                    {
                        int rightClipAmount = currentX + clippedWidth - imageWidth;
                        clippedWidth -= rightClipAmount;
                    }

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int pixelEndX = currentX + clippedWidth;

                        for (int pixelX = currentX; pixelX < pixelEndX; pixelX += 1)
                        {
                            currentColour = colourIndexes[(srcX >> 16) + srcRowOffset] & 0xff;

                            if (currentColour != 0)
                            {
                                currentColour = colourLookup[currentColour];
                                int red = currentColour >> 16 & 0xff;
                                int green = currentColour >> 8 & 0xff;
                                int blue = currentColour & 0xff;

                                if (red == green && green == blue)
                                {
                                    pixels[pixelX + dstOffset] =
                                        ((red * primaryRed >> 8) << 16) +
                                        ((green * primaryGreen >> 8) << 8) +
                                        (blue * primaryBlue >> 8);
                                }
                                else
                                {
                                    pixels[pixelX + dstOffset] = currentColour;
                                }
                            }

                            srcX += xStep;
                        }
                    }

                    srcY += yStep;
                    srcX = initialSrcX;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.",
                    exception);

                throw;
            }
        }

        internal static void DrawSpriteFlatShadedTexturedAlt(
            int[] pixels,
            sbyte[] colourIndexes,
            int[] colourLookup,
            int currentColour,
            int srcX,
            int srcY,
            int dstOffset,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int primaryColour,
            int secondaryColour,
            int xPosFixed,
            int xPosStep,
            int interlaceFlag,
            int imageX,
            int imageWidth,
            int gameWidth)
        {
            int primaryRed = primaryColour >> 16 & 0xff;
            int primaryGreen = primaryColour >> 8 & 0xff;
            int primaryBlue = primaryColour & 0xff;
            int secondaryRed = secondaryColour >> 16 & 0xff;
            int secondaryGreen = secondaryColour >> 8 & 0xff;
            int secondaryBlue = secondaryColour & 0xff;

            try
            {
                int initialSrcX = srcX;

                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    int currentX = xPosFixed >> 16;
                    int clippedWidth = width;

                    if (currentX < imageX)
                    {
                        int leftClipAmount = imageX - currentX;
                        clippedWidth -= leftClipAmount;
                        currentX = imageX;
                        srcX += xStep * leftClipAmount;
                    }

                    if (currentX + clippedWidth >= imageWidth)
                    {
                        int rightClipAmount = currentX + clippedWidth - imageWidth;
                        clippedWidth -= rightClipAmount;
                    }

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int pixelEndX = currentX + clippedWidth;

                        for (int pixelX = currentX; pixelX < pixelEndX; pixelX += 1)
                        {
                            currentColour = colourIndexes[(srcX >> 16) + srcRowOffset] & 0xff;

                            if (currentColour != 0)
                            {
                                currentColour = colourLookup[currentColour];
                                int red = currentColour >> 16 & 0xff;
                                int green = currentColour >> 8 & 0xff;
                                int blue = currentColour & 0xff;

                                if (red == green && green == blue)
                                {
                                    pixels[pixelX + dstOffset] =
                                        ((red * primaryRed >> 8) << 16) +
                                        ((green * primaryGreen >> 8) << 8) +
                                        (blue * primaryBlue >> 8);
                                }
                                else if (red == 255 && green == blue)
                                {
                                    pixels[pixelX + dstOffset] =
                                        ((red * secondaryRed >> 8) << 16) +
                                        ((green * secondaryGreen >> 8) << 8) +
                                        (blue * secondaryBlue >> 8);
                                }
                                else
                                {
                                    pixels[pixelX + dstOffset] = currentColour;
                                }
                            }

                            srcX += xStep;
                        }
                    }

                    srcY += yStep;
                    srcX = initialSrcX;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.",
                    exception);

                throw;
            }
        }
    }
}
