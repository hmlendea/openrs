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
            GameImageColourTint primaryTint = new(primaryColour);

            try
            {
                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    GameImageScaledScanline scanline = GameImageScaledScanline.Calculate(
                        srcX,
                        xPosFixed,
                        width,
                        xStep,
                        imageX,
                        imageWidth);

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int sampleX = scanline.SourceX;

                        for (int pixelX = scanline.StartX; pixelX < scanline.EndX; pixelX += 1)
                        {
                            currentColour = colours[(sampleX >> 16) + srcRowOffset];

                            if (currentColour != 0)
                            {
                                pixels[pixelX + dstOffset] = primaryTint.ApplyPrimary(
                                    currentColour);
                            }

                            sampleX += xStep;
                        }
                    }

                    srcY += yStep;
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
            GameImageColourTint primaryTint = new(primaryColour);
            GameImageColourTint secondaryTint = new(secondaryColour);

            try
            {
                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    GameImageScaledScanline scanline = GameImageScaledScanline.Calculate(
                        srcX,
                        xPosFixed,
                        width,
                        xStep,
                        imageX,
                        imageWidth);

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int sampleX = scanline.SourceX;

                        for (int pixelX = scanline.StartX; pixelX < scanline.EndX; pixelX += 1)
                        {
                            currentColour = colours[(sampleX >> 16) + srcRowOffset];

                            if (currentColour != 0)
                            {
                                pixels[pixelX + dstOffset] = primaryTint.ApplyPrimaryAndSecondary(
                                    currentColour,
                                    secondaryTint);
                            }

                            sampleX += xStep;
                        }
                    }

                    srcY += yStep;
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
            GameImageColourTint primaryTint = new(primaryColour);

            try
            {
                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    GameImageScaledScanline scanline = GameImageScaledScanline.Calculate(
                        srcX,
                        xPosFixed,
                        width,
                        xStep,
                        imageX,
                        imageWidth);

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int sampleX = scanline.SourceX;

                        for (int pixelX = scanline.StartX; pixelX < scanline.EndX; pixelX += 1)
                        {
                            currentColour = colourIndexes[(sampleX >> 16) + srcRowOffset] & 0xff;

                            if (currentColour != 0)
                            {
                                currentColour = colourLookup[currentColour];
                                pixels[pixelX + dstOffset] = primaryTint.ApplyPrimary(
                                    currentColour);
                            }

                            sampleX += xStep;
                        }
                    }

                    srcY += yStep;
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
            GameImageColourTint primaryTint = new(primaryColour);
            GameImageColourTint secondaryTint = new(secondaryColour);

            try
            {
                for (int rowIndex = -height; rowIndex < 0; rowIndex += 1)
                {
                    int srcRowOffset = (srcY >> 16) * srcWidth;
                    GameImageScaledScanline scanline = GameImageScaledScanline.Calculate(
                        srcX,
                        xPosFixed,
                        width,
                        xStep,
                        imageX,
                        imageWidth);

                    interlaceFlag = 1 - interlaceFlag;

                    if (interlaceFlag != 0)
                    {
                        int sampleX = scanline.SourceX;

                        for (int pixelX = scanline.StartX; pixelX < scanline.EndX; pixelX += 1)
                        {
                            currentColour = colourIndexes[(sampleX >> 16) + srcRowOffset] & 0xff;

                            if (currentColour != 0)
                            {
                                currentColour = colourLookup[currentColour];
                                pixels[pixelX + dstOffset] = primaryTint.ApplyPrimaryAndSecondary(
                                    currentColour,
                                    secondaryTint);
                            }

                            sampleX += xStep;
                        }
                    }

                    srcY += yStep;
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
