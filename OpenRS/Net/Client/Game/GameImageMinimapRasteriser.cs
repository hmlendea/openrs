namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageMinimapRasteriser(GameImage gameImage)
    {
        private static int ScanlinePositionShift => 8;

        private static int TextureCoordinateShift => 9;

        private static int ScanlineSentinelValue => 0x5f5e0ff;

        internal void Rasterise(
            GameImageMinimapProjection projection,
            int pictureIndex,
            int minimumY,
            int maximumY)
        {
            int gameHeight = gameImage.GameHeight;
            EnsureScanlineArrayCapacity(gameHeight);
            InitialiseScanlineRanges(minimumY, maximumY);
            int pictureWidth = gameImage.PictureWidth[pictureIndex];
            int textureRight = pictureWidth - 1;
            int textureBottom = gameImage.PictureHeight[pictureIndex] - 1;

            RasteriseEdge(
                projection.TopLeftX,
                projection.TopLeftY,
                projection.BottomLeftX,
                projection.BottomLeftY,
                0,
                0,
                0,
                textureBottom,
                gameHeight,
                true);
            RasteriseEdge(
                projection.TopLeftX,
                projection.TopLeftY,
                projection.TopRightX,
                projection.TopRightY,
                0,
                0,
                textureRight,
                0,
                gameHeight,
                false);
            RasteriseEdge(
                projection.TopRightX,
                projection.TopRightY,
                projection.BottomRightX,
                projection.BottomRightY,
                textureRight,
                0,
                textureRight,
                textureBottom,
                gameHeight,
                false);
            RasteriseEdge(
                projection.BottomRightX,
                projection.BottomRightY,
                projection.BottomLeftX,
                projection.BottomLeftY,
                textureRight,
                textureBottom,
                0,
                textureBottom,
                gameHeight,
                false);
            RenderScanlines(minimumY, maximumY, pictureWidth, pictureIndex);
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

        private void InitialiseScanlineRanges(int minimumY, int maximumY)
        {
            for (int scanlineY = minimumY; scanlineY <= maximumY; scanlineY += 1)
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
                    UpdateScanlineRange(
                        scanlineY,
                        edgeCurrentX,
                        edgeCurrentU,
                        edgeCurrentV);
                }

                edgeCurrentX += edgeStepX;
                edgeCurrentU += edgeStepU;
                edgeCurrentV += edgeStepV;
            }
        }

        private void UpdateScanlineRange(
            int scanlineY,
            int positionX,
            int textureU,
            int textureV)
        {
            if (positionX < gameImage.EntityScanlineMinX[scanlineY])
            {
                gameImage.EntityScanlineMinX[scanlineY] = positionX;
                gameImage.EntityScanlineMinValue[scanlineY] = textureU;
                gameImage.EntityScanlineMinExtra[scanlineY] = textureV;
            }

            if (positionX > gameImage.EntityScanlineMaxX[scanlineY])
            {
                gameImage.EntityScanlineMaxX[scanlineY] = positionX;
                gameImage.EntityScanlineMaxValue[scanlineY] = textureU;
                gameImage.EntityScanlineMaxExtra[scanlineY] = textureV;
            }
        }

        private void RenderScanlines(
            int minimumY,
            int maximumY,
            int pictureWidth,
            int pictureIndex)
        {
            int gameWidth = gameImage.GameWidth;
            int[] pictureColours = gameImage.PictureColours[pictureIndex];
            int pixelRowOffset = minimumY * gameWidth;

            for (int scanlineY = minimumY; scanlineY < maximumY; scanlineY += 1)
            {
                int leftX = gameImage.EntityScanlineMinX[scanlineY] >> ScanlinePositionShift;
                int rightX = gameImage.EntityScanlineMaxX[scanlineY] >> ScanlinePositionShift;

                if (rightX - leftX > 0)
                {
                    RenderScanline(
                        scanlineY,
                        pixelRowOffset,
                        pictureWidth,
                        pictureIndex,
                        pictureColours,
                        leftX,
                        rightX);
                }

                pixelRowOffset += gameWidth;
            }
        }

        private void RenderScanline(
            int scanlineY,
            int pixelRowOffset,
            int pictureWidth,
            int pictureIndex,
            int[] pictureColours,
            int leftX,
            int rightX)
        {
            int spanWidth = rightX - leftX;
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

            if (leftX < gameImage.ImageX)
            {
                textureLeftU += (gameImage.ImageX - leftX) * textureStepU;
                textureLeftV += (gameImage.ImageX - leftX) * textureStepV;
                leftX = gameImage.ImageX;
            }

            if (rightX > gameImage.ImageWidth)
            {
                rightX = gameImage.ImageWidth;
            }

            if (gameImage.IsInterlaced && (scanlineY & 1) != 0)
            {
                return;
            }

            if (!gameImage.HasTransparentBackground[pictureIndex])
            {
                GameImageSpriteBlitter.DrawSpriteAlpha(
                    gameImage.Pixels,
                    pictureColours,
                    pixelRowOffset + leftX,
                    textureLeftU,
                    textureLeftV,
                    textureStepU,
                    textureStepV,
                    leftX - rightX,
                    pictureWidth);

                return;
            }

            GameImageSpriteBlitter.DrawSpriteAlphaColorShifted(
                gameImage.Pixels,
                pictureColours,
                pixelRowOffset + leftX,
                textureLeftU,
                textureLeftV,
                textureStepU,
                textureStepV,
                leftX - rightX,
                pictureWidth);
        }
    }
}