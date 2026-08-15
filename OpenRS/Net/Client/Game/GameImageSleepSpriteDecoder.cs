using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageSleepSpriteDecoder(GameImage gameImage, ILogger logger)
    {
        private static int WhiteColour => 0xffffff;

        private static int SpriteWidth => 255;

        private static int SpriteHeight => 40;

        private static int SpritePixelCount => SpriteWidth * SpriteHeight;

        internal void Decode(int pictureIndex, sbyte[] spriteData)
        {
            int[] colours = gameImage.PictureColours[pictureIndex] = new int[SpritePixelCount];
            gameImage.PictureWidth[pictureIndex] = SpriteWidth;
            gameImage.PictureHeight[pictureIndex] = SpriteHeight;
            gameImage.PictureOffsetX[pictureIndex] = 0;
            gameImage.PictureOffsetY[pictureIndex] = 0;
            gameImage.PictureAssumedWidth[pictureIndex] = SpriteWidth;
            gameImage.PictureAssumedHeight[pictureIndex] = SpriteHeight;
            gameImage.HasTransparentBackground[pictureIndex] = false;
            int currentColour = 0;
            int dataOffset = 1;
            int pixelIndex = 0;

            try
            {
                for (; pixelIndex < SpriteWidth;)
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

                for (int row = 1; row < SpriteHeight; row += 1)
                {
                    for (int columnIndex = 0; columnIndex < SpriteWidth;)
                    {
                        int runLength = spriteData[dataOffset] & 0xff;
                        dataOffset += 1;

                        for (int runPosition = 0; runPosition < runLength; runPosition += 1)
                        {
                            colours[pixelIndex] = colours[pixelIndex - SpriteWidth];
                            pixelIndex += 1;
                            columnIndex += 1;
                        }

                        if (columnIndex < SpriteWidth)
                        {
                            colours[pixelIndex] = WhiteColour - colours[pixelIndex - SpriteWidth];
                            pixelIndex += 1;
                            columnIndex += 1;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "An error has occurred while applying the image.",
                    exception);
            }
        }
    }
}