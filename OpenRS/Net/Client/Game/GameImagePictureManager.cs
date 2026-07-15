using System;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePictureManager
    {
        private static int TransparentColour => 0xff00ff;
        private static int WhiteColour => 0xffffff;
        private static int Colour15BitTableSize => 32768;
        private static int MaxPaletteSize => 256;
        private static int LargeDistanceInitial => 0x3b9ac9ff;
        private static int ColourChannelBias => 0x40404;
        private static int SleepSpriteWidth => 255;
        private static int SleepSpriteHeight => 40;
        private static int SleepSpritePixelCount => SleepSpriteWidth * SleepSpriteHeight;
        private static int LinearScanOrder => 0;
        private static int ColumnMajorScanOrder => 1;
        private static int UnmappedPaletteEntry => -1;

        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImagePictureManager>();

        internal GameImagePictureManager(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void CleanUp()
        {
            for (int pictureIndex = 0; pictureIndex < gameImage.PictureColours.Length; pictureIndex += 1)
            {
                gameImage.PictureColours[pictureIndex] = null;
                gameImage.PictureWidth[pictureIndex] = 0;
                gameImage.PictureHeight[pictureIndex] = 0;
                gameImage.PictureColourIndexes[pictureIndex] = null;
                gameImage.PictureColour[pictureIndex] = null;
            }
        }

        internal void UnpackImageData(int startIndex, sbyte[] imageData, sbyte[] metaData, int count)
        {
            int metaOffset = BinaryDataReader.GetShort(imageData, 0);
            int assumedWidth = BinaryDataReader.GetShort(metaData, metaOffset);
            metaOffset += 2;
            int assumedHeight = BinaryDataReader.GetShort(metaData, metaOffset);
            metaOffset += 2;
            int paletteSize = metaData[metaOffset] & 0xff;
            metaOffset += 1;
            int[] palette = new int[paletteSize];
            palette[0] = TransparentColour;

            for (int paletteIndex = 0; paletteIndex < paletteSize - 1; paletteIndex += 1)
            {
                palette[paletteIndex + 1] =
                    ((metaData[metaOffset] & 0xff) << 16) +
                    ((metaData[metaOffset + 1] & 0xff) << 8) +
                    (metaData[metaOffset + 2] & 0xff);
                metaOffset += 3;
            }

            int imageDataOffset = 2;

            for (int pictureIndex = startIndex; pictureIndex < startIndex + count; pictureIndex += 1)
            {
                if (pictureIndex >= gameImage.PictureOffsetX.Length)
                {
                    break;
                }

                gameImage.PictureOffsetX[pictureIndex] = metaData[metaOffset] & 0xff;
                metaOffset += 1;
                gameImage.PictureOffsetY[pictureIndex] = metaData[metaOffset] & 0xff;
                metaOffset += 1;
                gameImage.PictureWidth[pictureIndex] = BinaryDataReader.GetShort(metaData, metaOffset);
                metaOffset += 2;
                gameImage.PictureHeight[pictureIndex] = BinaryDataReader.GetShort(metaData, metaOffset);
                metaOffset += 2;
                int scanOrder = metaData[metaOffset] & 0xff;
                metaOffset += 1;
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
                    for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
                    {
                        gameImage.PictureColourIndexes[pictureIndex][pixelIndex] = imageData[imageDataOffset];
                        imageDataOffset += 1;

                        if (gameImage.PictureColourIndexes[pictureIndex][pixelIndex] == 0)
                        {
                            gameImage.HasTransparentBackground[pictureIndex] = true;
                        }
                    }
                }
                else if (scanOrder == ColumnMajorScanOrder)
                {
                    for (int column = 0; column < gameImage.PictureWidth[pictureIndex]; column += 1)
                    {
                        for (int row = 0; row < gameImage.PictureHeight[pictureIndex]; row += 1)
                        {
                            int pixelIndex = column + row * gameImage.PictureWidth[pictureIndex];
                            gameImage.PictureColourIndexes[pictureIndex][pixelIndex] = imageData[imageDataOffset];
                            imageDataOffset += 1;

                            if (gameImage.PictureColourIndexes[pictureIndex][pixelIndex] == 0)
                            {
                                gameImage.HasTransparentBackground[pictureIndex] = true;
                            }
                        }
                    }
                }
            }
        }

        internal void SetSleepSprite(int pictureIndex, sbyte[] spriteData)
        {
            int[] colours = gameImage.PictureColours[pictureIndex] = new int[SleepSpritePixelCount];
            gameImage.PictureWidth[pictureIndex] = SleepSpriteWidth;
            gameImage.PictureHeight[pictureIndex] = SleepSpriteHeight;
            gameImage.PictureOffsetX[pictureIndex] = 0;
            gameImage.PictureOffsetY[pictureIndex] = 0;
            gameImage.PictureAssumedWidth[pictureIndex] = SleepSpriteWidth;
            gameImage.PictureAssumedHeight[pictureIndex] = SleepSpriteHeight;
            gameImage.HasTransparentBackground[pictureIndex] = false;
            int currentColour = 0;
            int dataOffset = 1;
            int pixelIndex = 0;

            try
            {
                for (; pixelIndex < SleepSpriteWidth; )
                {
                    int runLength = spriteData[dataOffset] & 0xff;
                    dataOffset += 1;

                    for (int runPosition = 0; runPosition < runLength; runPosition += 1)
                    {
                        colours[pixelIndex] = currentColour;
                        pixelIndex += 1;
                    }

                    currentColour = WhiteColour - currentColour;
                }

                for (int row = 1; row < SleepSpriteHeight; row += 1)
                {
                    for (int columnIndex = 0; columnIndex < SleepSpriteWidth; )
                    {
                        int runLength = spriteData[dataOffset] & 0xff;
                        dataOffset += 1;

                        for (int runPosition = 0; runPosition < runLength; runPosition += 1)
                        {
                            colours[pixelIndex] = colours[pixelIndex - SleepSpriteWidth];
                            pixelIndex += 1;
                            columnIndex += 1;
                        }

                        if (columnIndex < SleepSpriteWidth)
                        {
                            colours[pixelIndex] = WhiteColour - colours[pixelIndex - SleepSpriteWidth];
                            pixelIndex += 1;
                            columnIndex += 1;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderSprite, "An error has occurred while applying the image.", exception);
            }
        }

        internal void ApplyImage(int pictureIndex)
        {
            int pixelCount = gameImage.PictureWidth[pictureIndex] * gameImage.PictureHeight[pictureIndex];
            int[] sourceColours = gameImage.PictureColours[pictureIndex];
            int[] colourFrequencies = new int[Colour15BitTableSize];

            for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
            {
                int colour = sourceColours[pixelIndex];
                int quantisedIndex =
                    ((colour & 0xf80000) >> 9) +
                    ((colour & 0xf800) >> 6) +
                    ((colour & 0xf8) >> 3);
                colourFrequencies[quantisedIndex] += 1;
            }

            int[] topPalette = new int[MaxPaletteSize];
            topPalette[0] = TransparentColour;
            int[] topPaletteCounts = new int[MaxPaletteSize];

            for (int quantisedIndex = 0; quantisedIndex < Colour15BitTableSize; quantisedIndex += 1)
            {
                int frequency = colourFrequencies[quantisedIndex];

                if (frequency > topPaletteCounts[MaxPaletteSize - 1])
                {
                    for (int paletteSlot = 1; paletteSlot < MaxPaletteSize; paletteSlot += 1)
                    {
                        if (frequency <= topPaletteCounts[paletteSlot])
                        {
                            continue;
                        }

                        for (int shiftIndex = MaxPaletteSize - 1; shiftIndex > paletteSlot; shiftIndex -= 1)
                        {
                            topPalette[shiftIndex] = topPalette[shiftIndex - 1];
                            topPaletteCounts[shiftIndex] = topPaletteCounts[shiftIndex - 1];
                        }

                        topPalette[paletteSlot] =
                            ((quantisedIndex & 0x7c00) << 9) +
                            ((quantisedIndex & 0x3e0) << 6) +
                            ((quantisedIndex & 0x1f) << 3) +
                            ColourChannelBias;
                        topPaletteCounts[paletteSlot] = frequency;
                        break;
                    }
                }

                colourFrequencies[quantisedIndex] = UnmappedPaletteEntry;
            }

            sbyte[] colourIndexBuffer = new sbyte[pixelCount];

            for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
            {
                int colour = sourceColours[pixelIndex];
                int quantisedIndex =
                    ((colour & 0xf80000) >> 9) +
                    ((colour & 0xf800) >> 6) +
                    ((colour & 0xf8) >> 3);
                int nearestPaletteEntry = colourFrequencies[quantisedIndex];

                if (nearestPaletteEntry == UnmappedPaletteEntry)
                {
                    int minimumDistance = LargeDistanceInitial;
                    int blueChannel = colour >> 16 & 0xff;
                    int greenChannel = colour >> 8 & 0xff;
                    int redChannel = colour & 0xff;

                    for (int paletteIndex = 0; paletteIndex < MaxPaletteSize; paletteIndex += 1)
                    {
                        int paletteColour = topPalette[paletteIndex];
                        int paletteBlue = paletteColour >> 16 & 0xff;
                        int paletteGreen = paletteColour >> 8 & 0xff;
                        int paletteRed = paletteColour & 0xff;
                        int distanceSquared =
                            (blueChannel - paletteBlue) * (blueChannel - paletteBlue) +
                            (greenChannel - paletteGreen) * (greenChannel - paletteGreen) +
                            (redChannel - paletteRed) * (redChannel - paletteRed);

                        if (distanceSquared < minimumDistance)
                        {
                            minimumDistance = distanceSquared;
                            nearestPaletteEntry = paletteIndex;
                        }
                    }

                    colourFrequencies[quantisedIndex] = nearestPaletteEntry;
                }

                colourIndexBuffer[pixelIndex] = (sbyte)nearestPaletteEntry;
            }

            gameImage.PictureColourIndexes[pictureIndex] = colourIndexBuffer;
            gameImage.PictureColour[pictureIndex] = topPalette;
            gameImage.PictureColours[pictureIndex] = null;
        }

        internal void LoadImage(int pictureIndex)
        {
            if (gameImage.PictureColourIndexes[pictureIndex] is null)
            {
                return;
            }

            int pixelCount = gameImage.PictureWidth[pictureIndex] * gameImage.PictureHeight[pictureIndex];
            sbyte[] colourIndexBuffer = gameImage.PictureColourIndexes[pictureIndex];
            int[] palette = gameImage.PictureColour[pictureIndex];
            int[] resolvedColours = new int[pixelCount];

            for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
            {
                int colour = palette[colourIndexBuffer[pixelIndex] & 0xff];

                if (colour == 0)
                {
                    colour = 1;
                }
                else if (colour == TransparentColour)
                {
                    colour = 0;
                }

                resolvedColours[pixelIndex] = colour;
            }

            gameImage.PictureColours[pictureIndex] = resolvedColours;
            gameImage.PictureColourIndexes[pictureIndex] = null;
            gameImage.PictureColour[pictureIndex] = null;
        }

        internal void FillPicture(int pictureIndex, int x, int y, int width, int height)
        {
            gameImage.PictureWidth[pictureIndex] = width;
            gameImage.PictureHeight[pictureIndex] = height;
            gameImage.HasTransparentBackground[pictureIndex] = false;
            gameImage.PictureOffsetX[pictureIndex] = 0;
            gameImage.PictureOffsetY[pictureIndex] = 0;
            gameImage.PictureAssumedWidth[pictureIndex] = width;
            gameImage.PictureAssumedHeight[pictureIndex] = height;
            int pixelIndex = 0;
            gameImage.PictureColours[pictureIndex] = new int[width * height];

            for (int column = x; column < x + width; column += 1)
            {
                for (int row = y; row < y + height; row += 1)
                {
                    gameImage.PictureColours[pictureIndex][pixelIndex] = gameImage.Pixels[column + row * gameImage.GameWidth];
                    pixelIndex += 1;
                }
            }
        }

        internal void DrawImage(int pictureIndex, int x, int y, int width, int height)
        {
            gameImage.PictureWidth[pictureIndex] = width;
            gameImage.PictureHeight[pictureIndex] = height;
            gameImage.HasTransparentBackground[pictureIndex] = false;
            gameImage.PictureOffsetX[pictureIndex] = 0;
            gameImage.PictureOffsetY[pictureIndex] = 0;
            gameImage.PictureAssumedWidth[pictureIndex] = width;
            gameImage.PictureAssumedHeight[pictureIndex] = height;
            int pixelIndex = 0;
            gameImage.PictureColours[pictureIndex] = new int[width * height];

            for (int row = y; row < y + height; row += 1)
            {
                for (int column = x; column < x + width; column += 1)
                {
                    gameImage.PictureColours[pictureIndex][pixelIndex] = gameImage.Pixels[column + row * gameImage.GameWidth];
                    pixelIndex += 1;
                }
            }
        }
    }
}
