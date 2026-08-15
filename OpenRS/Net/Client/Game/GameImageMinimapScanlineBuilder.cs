namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageMinimapScanlineBuilder(GameImage gameImage)
    {
        private static int ScanlinePositionShift => 8;

        private static int ScanlineSentinelValue => 0x5f5e0ff;

        internal void Build(
            GameImageMinimapProjection projection,
            int pictureIndex,
            int minimumY,
            int maximumY)
        {
            int gameHeight = gameImage.GameHeight;
            EnsureScanlineArrayCapacity(gameHeight);
            InitialiseScanlineRanges(minimumY, maximumY);
            int textureRight = gameImage.PictureWidth[pictureIndex] - 1;
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
            for (int scanlineY = minimumY;
                scanlineY <= maximumY;
                scanlineY += 1)
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
                edgeStepX =
                    (screenEndX - screenStartX << ScanlinePositionShift) /
                    deltaY;
                edgeStepU =
                    (textureEndU - textureStartU << ScanlinePositionShift) /
                    deltaY;
                edgeStepV =
                    (textureEndV - textureStartV << ScanlinePositionShift) /
                    deltaY;
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

            for (int scanlineY = edgeStartY;
                scanlineY <= edgeEndY;
                scanlineY += 1)
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
    }
}