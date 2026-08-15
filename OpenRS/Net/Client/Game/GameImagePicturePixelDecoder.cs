namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePicturePixelDecoder(GameImage gameImage)
    {
        private static int LinearScanOrder => 0;

        private static int ColumnMajorScanOrder => 1;

        internal int Decode(
            int pictureIndex,
            sbyte[] imageData,
            int imageDataOffset,
            int scanOrder)
        {
            if (scanOrder == LinearScanOrder)
            {
                return DecodeLinearScan(
                    pictureIndex,
                    imageData,
                    imageDataOffset);
            }

            if (scanOrder == ColumnMajorScanOrder)
            {
                return DecodeColumnMajorScan(
                    pictureIndex,
                    imageData,
                    imageDataOffset);
            }

            return imageDataOffset;
        }

        private int DecodeLinearScan(
            int pictureIndex,
            sbyte[] imageData,
            int imageDataOffset)
        {
            int pixelCount =
                gameImage.PictureWidth[pictureIndex] *
                gameImage.PictureHeight[pictureIndex];

            for (int pixelIndex = 0;
                pixelIndex < pixelCount;
                pixelIndex += 1)
            {
                StorePixel(
                    pictureIndex,
                    pixelIndex,
                    imageData[imageDataOffset]);
                imageDataOffset += 1;
            }

            return imageDataOffset;
        }

        private int DecodeColumnMajorScan(
            int pictureIndex,
            sbyte[] imageData,
            int imageDataOffset)
        {
            for (int column = 0;
                column < gameImage.PictureWidth[pictureIndex];
                column += 1)
            {
                for (int row = 0;
                    row < gameImage.PictureHeight[pictureIndex];
                    row += 1)
                {
                    int pixelIndex =
                        column + row * gameImage.PictureWidth[pictureIndex];
                    StorePixel(
                        pictureIndex,
                        pixelIndex,
                        imageData[imageDataOffset]);
                    imageDataOffset += 1;
                }
            }

            return imageDataOffset;
        }

        private void StorePixel(
            int pictureIndex,
            int pixelIndex,
            sbyte colourIndex)
        {
            gameImage.PictureColourIndexes[pictureIndex][pixelIndex] = colourIndex;

            if (colourIndex == 0)
            {
                gameImage.HasTransparentBackground[pictureIndex] = true;
            }
        }
    }
}