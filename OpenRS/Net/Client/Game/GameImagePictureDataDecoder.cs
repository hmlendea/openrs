using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePictureDataDecoder(GameImage gameImage)
    {
        private static int LinearScanOrder => 0;

        private static int ColumnMajorScanOrder => 1;

        internal void Decode(int startIndex, sbyte[] imageData, sbyte[] metadata, int count)
        {
            int metadataOffset = BinaryDataReader.GetShort(imageData, 0);
            int assumedWidth = BinaryDataReader.GetShort(metadata, metadataOffset);
            metadataOffset += 2;
            int assumedHeight = BinaryDataReader.GetShort(metadata, metadataOffset);
            metadataOffset += 2;
            int paletteSize = metadata[metadataOffset] & 0xff;
            metadataOffset += 1;
            int[] palette = new int[paletteSize];
            palette[0] = GameImagePaletteConverter.TransparentColour;

            for (int paletteIndex = 0; paletteIndex < paletteSize - 1; paletteIndex += 1)
            {
                palette[paletteIndex + 1] =
                    ((metadata[metadataOffset] & 0xff) << 16) +
                    ((metadata[metadataOffset + 1] & 0xff) << 8) +
                    (metadata[metadataOffset + 2] & 0xff);
                metadataOffset += 3;
            }

            int imageDataOffset = 2;

            for (int pictureIndex = startIndex; pictureIndex < startIndex + count; pictureIndex += 1)
            {
                if (pictureIndex >= gameImage.PictureOffsetX.Length)
                {
                    break;
                }

                gameImage.PictureOffsetX[pictureIndex] = metadata[metadataOffset] & 0xff;
                metadataOffset += 1;
                gameImage.PictureOffsetY[pictureIndex] = metadata[metadataOffset] & 0xff;
                metadataOffset += 1;
                gameImage.PictureWidth[pictureIndex] = BinaryDataReader.GetShort(metadata, metadataOffset);
                metadataOffset += 2;
                gameImage.PictureHeight[pictureIndex] = BinaryDataReader.GetShort(metadata, metadataOffset);
                metadataOffset += 2;
                int scanOrder = metadata[metadataOffset] & 0xff;
                metadataOffset += 1;
                int pixelCount = gameImage.PictureWidth[pictureIndex] * gameImage.PictureHeight[pictureIndex];
                gameImage.PictureColourIndexes[pictureIndex] = new sbyte[pixelCount];
                gameImage.PictureColour[pictureIndex] = palette;
                gameImage.PictureAssumedWidth[pictureIndex] = assumedWidth;
                gameImage.PictureAssumedHeight[pictureIndex] = assumedHeight;
                gameImage.PictureColours[pictureIndex] = null;
                gameImage.HasTransparentBackground[pictureIndex] = false;

                if (gameImage.PictureOffsetX[pictureIndex] != 0 ||
                    gameImage.PictureOffsetY[pictureIndex] != 0)
                {
                    gameImage.HasTransparentBackground[pictureIndex] = true;
                }

                if (scanOrder == LinearScanOrder)
                {
                    imageDataOffset = DecodeLinearScan(
                        pictureIndex,
                        imageData,
                        pixelCount,
                        imageDataOffset);
                }
                else if (scanOrder == ColumnMajorScanOrder)
                {
                    imageDataOffset = DecodeColumnMajorScan(
                        pictureIndex,
                        imageData,
                        imageDataOffset);
                }
            }
        }

        private int DecodeLinearScan(
            int pictureIndex,
            sbyte[] imageData,
            int pixelCount,
            int imageDataOffset)
        {
            for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
            {
                StorePixel(pictureIndex, pixelIndex, imageData[imageDataOffset]);
                imageDataOffset += 1;
            }

            return imageDataOffset;
        }

        private int DecodeColumnMajorScan(
            int pictureIndex,
            sbyte[] imageData,
            int imageDataOffset)
        {
            for (int column = 0; column < gameImage.PictureWidth[pictureIndex]; column += 1)
            {
                for (int row = 0; row < gameImage.PictureHeight[pictureIndex]; row += 1)
                {
                    int pixelIndex = column + row * gameImage.PictureWidth[pictureIndex];
                    StorePixel(pictureIndex, pixelIndex, imageData[imageDataOffset]);
                    imageDataOffset += 1;
                }
            }

            return imageDataOffset;
        }

        private void StorePixel(int pictureIndex, int pixelIndex, sbyte colourIndex)
        {
            gameImage.PictureColourIndexes[pictureIndex][pixelIndex] = colourIndex;

            if (colourIndex == 0)
            {
                gameImage.HasTransparentBackground[pictureIndex] = true;
            }
        }
    }
}