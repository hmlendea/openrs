using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageMinimapRenderer
    {
        private static int RotationTableSize => 512;
        private static int CosineTableOffset => 256;
        private static double RotationAngleStep => 0.02454369;
        private static double SineScaleFactor => 32768.0;
        private static int RotationMask => 0xff;
        private static int AlignmentMask => 0x3f;
        private static int SpiralScale => 192;
        private static int StandardScale => 128;
        private static int ProjectionBitShift => 22;
        private static int ScanlinePositionShift => 8;
        private static int TextureCoordinateShift => 9;
        private static int ScanlineSentinelValue => 0x5f5e0ff;

        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageMinimapRenderer>();

        internal GameImageMinimapRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void DrawMinimapPic(int centreX, int centreY, int pictureIndex, int rotation, int scale)
        {
            try
            {
                DrawMinimapPicInternal(centreX, centreY, pictureIndex, rotation, scale);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderMinimap, "Error drawing minimap picture.", exception);
                throw;
            }
        }

        private void DrawMinimapPicInternal(
            int centreX,
            int centreY,
            int pictureIndex,
            int rotation,
            int scale)
        {
            int gameWidth = gameImage.GameWidth;
            int gameHeight = gameImage.GameHeight;

            InitialiseRotationTable();

            int pictureLeft = -gameImage.PictureAssumedWidth[pictureIndex] / 2;
            int pictureTop = -gameImage.PictureAssumedHeight[pictureIndex] / 2;

            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                pictureLeft += gameImage.PictureOffsetX[pictureIndex];
                pictureTop += gameImage.PictureOffsetY[pictureIndex];
            }

            int pictureRight = pictureLeft + gameImage.PictureWidth[pictureIndex];
            int pictureBottom = pictureTop + gameImage.PictureHeight[pictureIndex];

            rotation &= RotationMask;
            int sinScaled = gameImage.CharacterRotationTable[rotation] * scale;
            int cosScaled = gameImage.CharacterRotationTable[rotation + CosineTableOffset] * scale;

            int screenTopLeftX = centreX +
                (pictureTop * sinScaled + pictureLeft * cosScaled >> ProjectionBitShift);
            int screenTopLeftY = centreY +
                (pictureTop * cosScaled - pictureLeft * sinScaled >> ProjectionBitShift);
            int screenTopRightX = centreX +
                (pictureTop * sinScaled + pictureRight * cosScaled >> ProjectionBitShift);
            int screenTopRightY = centreY +
                (pictureTop * cosScaled - pictureRight * sinScaled >> ProjectionBitShift);
            int screenBottomRightX = centreX +
                (pictureBottom * sinScaled + pictureRight * cosScaled >> ProjectionBitShift);
            int screenBottomRightY = centreY +
                (pictureBottom * cosScaled - pictureRight * sinScaled >> ProjectionBitShift);
            int screenBottomLeftX = centreX +
                (pictureBottom * sinScaled + pictureLeft * cosScaled >> ProjectionBitShift);
            int screenBottomLeftY = centreY +
                (pictureBottom * cosScaled - pictureLeft * sinScaled >> ProjectionBitShift);

            UpdateDrawCounters(scale, rotation);

            int scanlineMinY = screenTopLeftY;
            int scanlineMaxY = screenTopLeftY;

            if (screenTopRightY < scanlineMinY)
            {
                scanlineMinY = screenTopRightY;
            }
            else if (screenTopRightY > scanlineMaxY)
            {
                scanlineMaxY = screenTopRightY;
            }

            if (screenBottomRightY < scanlineMinY)
            {
                scanlineMinY = screenBottomRightY;
            }
            else if (screenBottomRightY > scanlineMaxY)
            {
                scanlineMaxY = screenBottomRightY;
            }

            if (screenBottomLeftY < scanlineMinY)
            {
                scanlineMinY = screenBottomLeftY;
            }
            else if (screenBottomLeftY > scanlineMaxY)
            {
                scanlineMaxY = screenBottomLeftY;
            }

            if (scanlineMinY < gameImage.ImageY)
            {
                scanlineMinY = gameImage.ImageY;
            }

            if (scanlineMaxY > gameImage.ImageHeight)
            {
                scanlineMaxY = gameImage.ImageHeight;
            }

            EnsureScanlineArrayCapacity(gameHeight);
            InitialiseScanlineRanges(scanlineMinY, scanlineMaxY);

            int pictureWidth = gameImage.PictureWidth[pictureIndex];
            int textureRight = pictureWidth - 1;
            int textureBottom = gameImage.PictureHeight[pictureIndex] - 1;

            // Left edge: top-left → bottom-left; U=0, V varies (0 → textureBottom).
            RasteriseEdge(
                screenTopLeftX,
                screenTopLeftY,
                screenBottomLeftX,
                screenBottomLeftY,
                0,
                0,
                0,
                textureBottom,
                gameHeight,
                true);

            // Top edge: top-left → top-right; V=0, U varies (0 → textureRight).
            RasteriseEdge(
                screenTopLeftX,
                screenTopLeftY,
                screenTopRightX,
                screenTopRightY,
                0,
                0,
                textureRight,
                0,
                gameHeight,
                false);

            // Right edge: top-right → bottom-right; U=textureRight, V varies (0 → textureBottom).
            RasteriseEdge(
                screenTopRightX,
                screenTopRightY,
                screenBottomRightX,
                screenBottomRightY,
                textureRight,
                0,
                textureRight,
                textureBottom,
                gameHeight,
                false);

            // Bottom edge: bottom-right → bottom-left; V=textureBottom, U varies (textureRight → 0).
            RasteriseEdge(
                screenBottomRightX,
                screenBottomRightY,
                screenBottomLeftX,
                screenBottomLeftY,
                textureRight,
                textureBottom,
                0,
                textureBottom,
                gameHeight,
                false);

            RenderScanlines(scanlineMinY, scanlineMaxY, gameWidth, pictureWidth, pictureIndex);
        }

        private void InitialiseRotationTable()
        {
            if (gameImage.CharacterRotationTable is not null)
            {
                return;
            }

            gameImage.CharacterRotationTable = new int[RotationTableSize];

            for (int index = 0; index < CosineTableOffset; index += 1)
            {
                gameImage.CharacterRotationTable[index] =
                    (int)(Math.Sin(index * RotationAngleStep) * SineScaleFactor);
                gameImage.CharacterRotationTable[index + CosineTableOffset] =
                    (int)(Math.Cos(index * RotationAngleStep) * SineScaleFactor);
            }
        }

        private void UpdateDrawCounters(int scale, int rotation)
        {
            bool isAlignedWithLastRotation =
                (rotation & AlignmentMask) == (GameImage.LastCharacterRotation & AlignmentMask);

            if (scale == SpiralScale && isAlignedWithLastRotation)
            {
                GameImage.SpiralDrawCount += 1;
            }
            else if (scale == StandardScale)
            {
                GameImage.LastCharacterRotation = rotation;
            }
            else
            {
                GameImage.CharacterDrawCount += 1;
            }
        }

        private void EnsureScanlineArrayCapacity(int gameHeight)
        {
            if (gameImage.EntityScanlineMinX is not null &&
                gameImage.EntityScanlineMinX.Length == gameHeight + 1)
            {
                return;
            }

            gameImage.EntityScanlineMinX = new int[gameHeight + 1];
            gameImage.EntityScanlineMaxX = new int[gameHeight + 1];
            gameImage.EntityScanlineMinValue = new int[gameHeight + 1];
            gameImage.EntityScanlineMaxValue = new int[gameHeight + 1];
            gameImage.EntityScanlineMinExtra = new int[gameHeight + 1];
            gameImage.EntityScanlineMaxExtra = new int[gameHeight + 1];
        }

        private void InitialiseScanlineRanges(int scanlineMinY, int scanlineMaxY)
        {
            for (int scanlineY = scanlineMinY; scanlineY <= scanlineMaxY; scanlineY += 1)
            {
                gameImage.EntityScanlineMinX[scanlineY] = ScanlineSentinelValue;
                gameImage.EntityScanlineMaxX[scanlineY] = -ScanlineSentinelValue;
            }
        }

        private void RasteriseEdge(
            int screenStartX,
            int screenStartY,
            int screenEndX,
            int screenEndY,
            int textureStartU,
            int textureStartV,
            int textureEndU,
            int textureEndV,
            int gameHeight,
            bool shouldInitialise)
        {
            int edgeStepX = 0;
            int edgeStepU = 0;
            int edgeStepV = 0;

            if (screenEndY != screenStartY)
            {
                int deltaY = screenEndY - screenStartY;
                edgeStepX = (screenEndX - screenStartX << ScanlinePositionShift) / deltaY;
                edgeStepU = (textureEndU - textureStartU << ScanlinePositionShift) / deltaY;
                edgeStepV = (textureEndV - textureStartV << ScanlinePositionShift) / deltaY;
            }

            int edgeCurrentX;
            int edgeCurrentU;
            int edgeCurrentV;
            int edgeStartY;
            int edgeEndY;

            if (screenStartY > screenEndY)
            {
                edgeCurrentX = screenEndX << ScanlinePositionShift;
                edgeCurrentU = textureEndU << ScanlinePositionShift;
                edgeCurrentV = textureEndV << ScanlinePositionShift;
                edgeStartY = screenEndY;
                edgeEndY = screenStartY;
            }
            else
            {
                edgeCurrentX = screenStartX << ScanlinePositionShift;
                edgeCurrentU = textureStartU << ScanlinePositionShift;
                edgeCurrentV = textureStartV << ScanlinePositionShift;
                edgeStartY = screenStartY;
                edgeEndY = screenEndY;
            }

            if (edgeStartY < 0)
            {
                edgeCurrentX -= edgeStepX * edgeStartY;
                edgeCurrentU -= edgeStepU * edgeStartY;
                edgeCurrentV -= edgeStepV * edgeStartY;
                edgeStartY = 0;
            }

            if (edgeEndY > gameHeight - 1)
            {
                edgeEndY = gameHeight - 1;
            }

            for (int scanlineY = edgeStartY; scanlineY <= edgeEndY; scanlineY += 1)
            {
                if (shouldInitialise)
                {
                    gameImage.EntityScanlineMinX[scanlineY] = edgeCurrentX;
                    gameImage.EntityScanlineMaxX[scanlineY] = edgeCurrentX;
                    gameImage.EntityScanlineMinValue[scanlineY] = edgeCurrentU;
                    gameImage.EntityScanlineMaxValue[scanlineY] = edgeCurrentU;
                    gameImage.EntityScanlineMinExtra[scanlineY] = edgeCurrentV;
                    gameImage.EntityScanlineMaxExtra[scanlineY] = edgeCurrentV;
                }
                else
                {
                    if (edgeCurrentX < gameImage.EntityScanlineMinX[scanlineY])
                    {
                        gameImage.EntityScanlineMinX[scanlineY] = edgeCurrentX;
                        gameImage.EntityScanlineMinValue[scanlineY] = edgeCurrentU;
                        gameImage.EntityScanlineMinExtra[scanlineY] = edgeCurrentV;
                    }

                    if (edgeCurrentX > gameImage.EntityScanlineMaxX[scanlineY])
                    {
                        gameImage.EntityScanlineMaxX[scanlineY] = edgeCurrentX;
                        gameImage.EntityScanlineMaxValue[scanlineY] = edgeCurrentU;
                        gameImage.EntityScanlineMaxExtra[scanlineY] = edgeCurrentV;
                    }
                }

                edgeCurrentX += edgeStepX;
                edgeCurrentU += edgeStepU;
                edgeCurrentV += edgeStepV;
            }
        }

        private void RenderScanlines(
            int scanlineMinY,
            int scanlineMaxY,
            int gameWidth,
            int pictureWidth,
            int pictureIndex)
        {
            int[] pictureColours = gameImage.PictureColours[pictureIndex];
            int[] pixels = gameImage.Pixels;
            int imageX = gameImage.ImageX;
            int imageWidth = gameImage.ImageWidth;
            int pixelRowOffset = scanlineMinY * gameWidth;

            for (int scanlineY = scanlineMinY; scanlineY < scanlineMaxY; scanlineY += 1)
            {
                int scanlineLeftX = gameImage.EntityScanlineMinX[scanlineY] >> ScanlinePositionShift;
                int scanlineRightX = gameImage.EntityScanlineMaxX[scanlineY] >> ScanlinePositionShift;

                if (scanlineRightX - scanlineLeftX <= 0)
                {
                    pixelRowOffset += gameWidth;
                }
                else
                {
                    int spanWidth = scanlineRightX - scanlineLeftX;
                    int textureLeftU =
                        gameImage.EntityScanlineMinValue[scanlineY] << TextureCoordinateShift;
                    int textureRightU =
                        gameImage.EntityScanlineMaxValue[scanlineY] << TextureCoordinateShift;
                    int textureStepU = (textureRightU - textureLeftU) / spanWidth;
                    int textureLeftV =
                        gameImage.EntityScanlineMinExtra[scanlineY] << TextureCoordinateShift;
                    int textureRightV =
                        gameImage.EntityScanlineMaxExtra[scanlineY] << TextureCoordinateShift;
                    int textureStepV = (textureRightV - textureLeftV) / spanWidth;

                    if (scanlineLeftX < imageX)
                    {
                        textureLeftU += (imageX - scanlineLeftX) * textureStepU;
                        textureLeftV += (imageX - scanlineLeftX) * textureStepV;
                        scanlineLeftX = imageX;
                    }

                    if (scanlineRightX > imageWidth)
                    {
                        scanlineRightX = imageWidth;
                    }

                    if (!gameImage.IsInterlaced || (scanlineY & 1) == 0)
                    {
                        if (!gameImage.HasTransparentBackground[pictureIndex])
                        {
                            GameImageSpriteBlitter.DrawSpriteAlpha(
                                pixels,
                                pictureColours,
                                0,
                                pixelRowOffset + scanlineLeftX,
                                textureLeftU,
                                textureLeftV,
                                textureStepU,
                                textureStepV,
                                scanlineLeftX - scanlineRightX,
                                pictureWidth);
                        }
                        else
                        {
                            GameImageSpriteBlitter.DrawSpriteAlphaColorShifted(
                                pixels,
                                pictureColours,
                                0,
                                pixelRowOffset + scanlineLeftX,
                                textureLeftU,
                                textureLeftV,
                                textureStepU,
                                textureStepV,
                                scanlineLeftX - scanlineRightX,
                                pictureWidth);
                        }
                    }

                    pixelRowOffset += gameWidth;
                }
            }
        }
    }
}

