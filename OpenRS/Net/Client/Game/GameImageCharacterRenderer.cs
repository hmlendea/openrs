using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageCharacterRenderer
    {
        private static int DefaultColour => 0xffffff;

        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageCharacterRenderer>();

        internal GameImageCharacterRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void DrawTransparentImage(
            int x,
            int y,
            int drawWidth,
            int drawHeight,
            int pictureIndex,
            int colourTint)
        {
            try
            {
                DrawFlippedSpriteInternal(
                    x,
                    y,
                    drawWidth,
                    drawHeight,
                    pictureIndex,
                    colourTint,
                    false);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.", exception);
                throw;
            }
        }

        internal void DrawCharacterLegs(
            int x,
            int y,
            int drawWidth,
            int drawHeight,
            int pictureIndex,
            int colourTint)
        {
            try
            {
                DrawFlippedSpriteInternal(
                    x,
                    y,
                    drawWidth,
                    drawHeight,
                    pictureIndex,
                    colourTint,
                    true);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderCharacter, "Error in sprite clipping routine.", exception);
                throw;
            }
        }

        internal void DrawImage(
            int x,
            int y,
            int width,
            int height,
            int pictureIndex,
            int primaryColour,
            int secondaryColour,
            int shearFactor,
            bool isFlipped)
        {
            try
            {
                int[] pixels = gameImage.Pixels;
                int gameWidth = gameImage.GameWidth;
                int imageX = gameImage.ImageX;
                int imageY = gameImage.ImageY;
                int imageWidth = gameImage.ImageWidth;
                int imageHeight = gameImage.ImageHeight;

                if (primaryColour == 0)
                {
                    primaryColour = DefaultColour;
                }

                if (secondaryColour == 0)
                {
                    secondaryColour = DefaultColour;
                }

                int pictureWidth = gameImage.PictureWidth[pictureIndex];
                int pictureHeight = gameImage.PictureHeight[pictureIndex];
                int sourceStartX = 0;
                int sourceStartY = 0;
                int xPositionAccumulator = shearFactor << 16;
                int scaleX = (pictureWidth << 16) / width;
                int scaleY = (pictureHeight << 16) / height;
                int xShearPerRow = -(shearFactor << 16) / height;

                if (gameImage.HasTransparentBackground[pictureIndex])
                {
                    int assumedWidth = gameImage.PictureAssumedWidth[pictureIndex];
                    int assumedHeight = gameImage.PictureAssumedHeight[pictureIndex];
                    scaleX = (assumedWidth << 16) / width;
                    scaleY = (assumedHeight << 16) / height;
                    int pictureOffsetX = gameImage.PictureOffsetX[pictureIndex];
                    int pictureOffsetY = gameImage.PictureOffsetY[pictureIndex];

                    if (isFlipped)
                    {
                        pictureOffsetX = assumedWidth - pictureWidth - pictureOffsetX;
                    }

                    x += (pictureOffsetX * width + assumedWidth - 1) / assumedWidth;
                    int adjustedVerticalOffset = (pictureOffsetY * height + assumedHeight - 1) / assumedHeight;
                    y += adjustedVerticalOffset;
                    xPositionAccumulator += adjustedVerticalOffset * xShearPerRow;

                    if (pictureOffsetX * width % assumedWidth != 0)
                    {
                        sourceStartX = (assumedWidth - pictureOffsetX * width % assumedWidth << 16) / width;
                    }

                    if (pictureOffsetY * height % assumedHeight != 0)
                    {
                        sourceStartY = (assumedHeight - pictureOffsetY * height % assumedHeight << 16) / height;
                    }

                    width = ((pictureWidth << 16) - sourceStartX + scaleX - 1) / scaleX;
                    height = ((pictureHeight << 16) - sourceStartY + scaleY - 1) / scaleY;
                }

                int destinationIndex = y * gameWidth;
                xPositionAccumulator += x << 16;

                if (y < imageY)
                {
                    int topClip = imageY - y;
                    height -= topClip;
                    y = imageY;
                    destinationIndex += topClip * gameWidth;
                    sourceStartY += scaleY * topClip;
                    xPositionAccumulator += xShearPerRow * topClip;
                }

                if (y + height >= imageHeight)
                {
                    height -= y + height - imageHeight + 1;
                }

                int scanlineMode = 2;

                if (gameImage.IsInterlaced)
                {
                    scanlineMode = destinationIndex / gameWidth & 1;
                }

                if (secondaryColour == DefaultColour)
                {
                    if (gameImage.PictureColours[pictureIndex] is not null)
                    {
                        if (!isFlipped)
                        {
                            GameImageScaledSpriteBlitter.DrawSpriteFlatShaded(
                                pixels,
                                gameImage.PictureColours[pictureIndex],
                                0,
                                sourceStartX,
                                sourceStartY,
                                destinationIndex,
                                width,
                                height,
                                scaleX,
                                scaleY,
                                pictureWidth,
                                primaryColour,
                                xPositionAccumulator,
                                xShearPerRow,
                                scanlineMode,
                                imageX,
                                imageWidth,
                                gameWidth);

                            return;
                        }

                        GameImageScaledSpriteBlitter.DrawSpriteFlatShaded(
                            pixels,
                            gameImage.PictureColours[pictureIndex],
                            0,
                            (pictureWidth << 16) - sourceStartX - 1,
                            sourceStartY,
                            destinationIndex,
                            width,
                            height,
                            -scaleX,
                            scaleY,
                            pictureWidth,
                            primaryColour,
                            xPositionAccumulator,
                            xShearPerRow,
                            scanlineMode,
                            imageX,
                            imageWidth,
                            gameWidth);

                        return;
                    }

                    if (!isFlipped)
                    {
                        GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTextured(
                            pixels,
                            gameImage.PictureColourIndexes[pictureIndex],
                            gameImage.PictureColour[pictureIndex],
                            0,
                            sourceStartX,
                            sourceStartY,
                            destinationIndex,
                            width,
                            height,
                            scaleX,
                            scaleY,
                            pictureWidth,
                            primaryColour,
                            xPositionAccumulator,
                            xShearPerRow,
                            scanlineMode,
                            imageX,
                            imageWidth,
                            gameWidth);

                        return;
                    }

                    GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTextured(
                        pixels,
                        gameImage.PictureColourIndexes[pictureIndex],
                        gameImage.PictureColour[pictureIndex],
                        0,
                        (pictureWidth << 16) - sourceStartX - 1,
                        sourceStartY,
                        destinationIndex,
                        width,
                        height,
                        -scaleX,
                        scaleY,
                        pictureWidth,
                        primaryColour,
                        xPositionAccumulator,
                        xShearPerRow,
                        scanlineMode,
                        imageX,
                        imageWidth,
                        gameWidth);

                    return;
                }

                if (gameImage.PictureColours[pictureIndex] is not null)
                {
                    if (!isFlipped)
                    {
                        GameImageScaledSpriteBlitter.DrawSpriteFlatShadedAlt(
                            pixels,
                            gameImage.PictureColours[pictureIndex],
                            0,
                            sourceStartX,
                            sourceStartY,
                            destinationIndex,
                            width,
                            height,
                            scaleX,
                            scaleY,
                            pictureWidth,
                            primaryColour,
                            secondaryColour,
                            xPositionAccumulator,
                            xShearPerRow,
                            scanlineMode,
                            imageX,
                            imageWidth,
                            gameWidth);

                        return;
                    }

                    GameImageScaledSpriteBlitter.DrawSpriteFlatShadedAlt(
                        pixels,
                        gameImage.PictureColours[pictureIndex],
                        0,
                        (pictureWidth << 16) - sourceStartX - 1,
                        sourceStartY,
                        destinationIndex,
                        width,
                        height,
                        -scaleX,
                        scaleY,
                        pictureWidth,
                        primaryColour,
                        secondaryColour,
                        xPositionAccumulator,
                        xShearPerRow,
                        scanlineMode,
                        imageX,
                        imageWidth,
                        gameWidth);

                    return;
                }

                if (!isFlipped)
                {
                    GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTexturedAlt(
                        pixels,
                        gameImage.PictureColourIndexes[pictureIndex],
                        gameImage.PictureColour[pictureIndex],
                        0,
                        sourceStartX,
                        sourceStartY,
                        destinationIndex,
                        width,
                        height,
                        scaleX,
                        scaleY,
                        pictureWidth,
                        primaryColour,
                        secondaryColour,
                        xPositionAccumulator,
                        xShearPerRow,
                        scanlineMode,
                        imageX,
                        imageWidth,
                        gameWidth);

                    return;
                }

                GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTexturedAlt(
                    pixels,
                    gameImage.PictureColourIndexes[pictureIndex],
                    gameImage.PictureColour[pictureIndex],
                    0,
                    (pictureWidth << 16) - sourceStartX - 1,
                    sourceStartY,
                    destinationIndex,
                    width,
                    height,
                    -scaleX,
                    scaleY,
                    pictureWidth,
                    primaryColour,
                    secondaryColour,
                    xPositionAccumulator,
                    xShearPerRow,
                    scanlineMode,
                    imageX,
                    imageWidth,
                    gameWidth);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.", exception);
                throw;
            }
        }

        private void DrawFlippedSpriteInternal(
            int x,
            int y,
            int drawWidth,
            int drawHeight,
            int pictureIndex,
            int colourTint,
            bool isColourShifted)
        {
            int[] pixels = gameImage.Pixels;
            int gameWidth = gameImage.GameWidth;
            int imageX = gameImage.ImageX;
            int imageY = gameImage.ImageY;
            int imageWidth = gameImage.ImageWidth;
            int imageHeight = gameImage.ImageHeight;
            int pictureWidth = gameImage.PictureWidth[pictureIndex];
            int pictureHeight = gameImage.PictureHeight[pictureIndex];
            int sourceStartX = 0;
            int sourceStartY = 0;
            int scaleX = (pictureWidth << 16) / drawWidth;
            int scaleY = (pictureHeight << 16) / drawHeight;

            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                int assumedWidth = gameImage.PictureAssumedWidth[pictureIndex];
                int assumedHeight = gameImage.PictureAssumedHeight[pictureIndex];
                scaleX = (assumedWidth << 16) / drawWidth;
                scaleY = (assumedHeight << 16) / drawHeight;
                x += (gameImage.PictureOffsetX[pictureIndex] * drawWidth + assumedWidth - 1) / assumedWidth;
                y += (gameImage.PictureOffsetY[pictureIndex] * drawHeight + assumedHeight - 1) / assumedHeight;

                if (gameImage.PictureOffsetX[pictureIndex] * drawWidth % assumedWidth != 0)
                {
                    sourceStartX = (assumedWidth - gameImage.PictureOffsetX[pictureIndex] * drawWidth % assumedWidth << 16) / drawWidth;
                }

                if (gameImage.PictureOffsetY[pictureIndex] * drawHeight % assumedHeight != 0)
                {
                    sourceStartY = (assumedHeight - gameImage.PictureOffsetY[pictureIndex] * drawHeight % assumedHeight << 16) / drawHeight;
                }

                drawWidth = drawWidth * (pictureWidth - (sourceStartX >> 16)) / assumedWidth;
                drawHeight = drawHeight * (pictureHeight - (sourceStartY >> 16)) / assumedHeight;
            }

            int destinationIndex = x + y * gameWidth;
            int destinationRowStride = gameWidth - drawWidth;

            if (y < imageY)
            {
                int topClip = imageY - y;
                drawHeight -= topClip;
                y = 0;
                destinationIndex += topClip * gameWidth;
                sourceStartY += scaleY * topClip;
            }

            if (y + drawHeight >= imageHeight)
            {
                drawHeight -= y + drawHeight - imageHeight + 1;
            }

            if (x < imageX)
            {
                int leftClip = imageX - x;
                drawWidth -= leftClip;
                x = 0;
                destinationIndex += leftClip;
                sourceStartX += scaleX * leftClip;
                destinationRowStride += leftClip;
            }

            if (x + drawWidth >= imageWidth)
            {
                int rightClip = x + drawWidth - imageWidth + 1;
                drawWidth -= rightClip;
                destinationRowStride += rightClip;
            }

            byte scanlineStep = 1;

            if (gameImage.IsInterlaced)
            {
                scanlineStep = 2;
                destinationRowStride += gameWidth;
                scaleY *= 2;

                if ((y & 1) != 0)
                {
                    destinationIndex += gameWidth;
                    drawHeight -= 1;
                }
            }

            if (isColourShifted)
            {
                GameImageSpriteBlitter.DrawSpriteFlippedColorShifted(
                    pixels,
                    gameImage.PictureColours[pictureIndex],
                    0,
                    sourceStartX,
                    sourceStartY,
                    destinationIndex,
                    destinationRowStride,
                    drawWidth,
                    drawHeight,
                    scaleX,
                    scaleY,
                    pictureWidth,
                    scanlineStep,
                    colourTint);

                return;
            }

            GameImageSpriteBlitter.DrawSpriteFlipped(
                pixels,
                gameImage.PictureColours[pictureIndex],
                0,
                sourceStartX,
                sourceStartY,
                destinationIndex,
                destinationRowStride,
                drawWidth,
                drawHeight,
                scaleX,
                scaleY,
                pictureWidth,
                scanlineStep,
                colourTint);
        }
    }
}
