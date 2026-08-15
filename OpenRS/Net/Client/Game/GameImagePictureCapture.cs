namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePictureCapture(GameImage gameImage)
    {
        internal void CaptureColumnMajor(
            int pictureIndex,
            int positionX,
            int positionY,
            int width,
            int height)
        {
            int[] pictureColours = InitialisePicture(pictureIndex, width, height);
            int pixelIndex = 0;

            for (int column = positionX; column < positionX + width; column += 1)
            {
                for (int row = positionY; row < positionY + height; row += 1)
                {
                    pictureColours[pixelIndex] = gameImage.Pixels[column + row * gameImage.GameWidth];
                    pixelIndex += 1;
                }
            }
        }

        internal void CaptureRowMajor(
            int pictureIndex,
            int positionX,
            int positionY,
            int width,
            int height)
        {
            int[] pictureColours = InitialisePicture(pictureIndex, width, height);
            int pixelIndex = 0;

            for (int row = positionY; row < positionY + height; row += 1)
            {
                for (int column = positionX; column < positionX + width; column += 1)
                {
                    pictureColours[pixelIndex] = gameImage.Pixels[column + row * gameImage.GameWidth];
                    pixelIndex += 1;
                }
            }
        }

        private int[] InitialisePicture(int pictureIndex, int width, int height)
        {
            gameImage.PictureWidth[pictureIndex] = width;
            gameImage.PictureHeight[pictureIndex] = height;
            gameImage.HasTransparentBackground[pictureIndex] = false;
            gameImage.PictureOffsetX[pictureIndex] = 0;
            gameImage.PictureOffsetY[pictureIndex] = 0;
            gameImage.PictureAssumedWidth[pictureIndex] = width;
            gameImage.PictureAssumedHeight[pictureIndex] = height;
            gameImage.PictureColours[pictureIndex] = new int[width * height];

            return gameImage.PictureColours[pictureIndex];
        }
    }
}