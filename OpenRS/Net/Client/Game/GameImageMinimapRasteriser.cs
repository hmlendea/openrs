namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageMinimapRasteriser(GameImage gameImage)
    {
        private static int ScanlinePositionShift => 8;

        private static int TextureCoordinateShift => 9;

        private readonly GameImageMinimapScanlineBuilder scanlineBuilder =
            new(gameImage);

        internal void Rasterise(
            GameImageMinimapProjection projection,
            int pictureIndex,
            int minimumY,
            int maximumY)
        {
            scanlineBuilder.Build(
                projection,
                pictureIndex,
                minimumY,
                maximumY);
            int pictureWidth = gameImage.PictureWidth[pictureIndex];
            RenderScanlines(minimumY, maximumY, pictureWidth, pictureIndex);
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