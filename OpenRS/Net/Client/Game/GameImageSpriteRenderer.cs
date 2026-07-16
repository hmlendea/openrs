using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageSpriteRenderer
    {
        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageSpriteRenderer>();

        internal GameImageSpriteRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void DrawPicture(int x, int y, int pictureIndex)
        {
            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                x += gameImage.PictureOffsetX[pictureIndex];
                y += gameImage.PictureOffsetY[pictureIndex];
            }

            int[] pixels = gameImage.Pixels;
            int gameWidth = gameImage.GameWidth;
            int imageX = gameImage.ImageX;
            int imageY = gameImage.ImageY;
            int imageWidth = gameImage.ImageWidth;
            int imageHeight = gameImage.ImageHeight;

            int i1 = x + y * gameWidth;
            int j1 = 0;
            int k1 = gameImage.PictureHeight[pictureIndex];
            int l1 = gameImage.PictureWidth[pictureIndex];
            int i2 = gameWidth - l1;
            int j2 = 0;

            if (y < imageY)
            {
                int k2 = imageY - y;
                k1 -= k2;
                y = imageY;
                j1 += k2 * l1;
                i1 += k2 * gameWidth;
            }

            if (y + k1 >= imageHeight)
            {
                k1 -= y + k1 - imageHeight + 1;
            }

            if (x < imageX)
            {
                int l2 = imageX - x;
                l1 -= l2;
                x = imageX;
                j1 += l2;
                i1 += l2;
                j2 += l2;
                i2 += l2;
            }

            if (x + l1 >= imageWidth)
            {
                int i3 = x + l1 - imageWidth + 1;
                l1 -= i3;
                j2 += i3;
                i2 += i3;
            }

            if (l1 <= 0 || k1 <= 0)
            {
                return;
            }

            byte byte0 = 1;

            if (gameImage.IsInterlaced)
            {
                byte0 = 2;
                i2 += gameWidth;
                j2 += gameImage.PictureWidth[pictureIndex];

                if ((y & 1) != 0)
                {
                    i1 += gameWidth;
                    k1 -= 1;
                }
            }

            if (gameImage.PictureColours[pictureIndex] is null)
            {
                GameImageSpriteBlitter.DrawSpriteTextured(pixels, gameImage.PictureColourIndexes[pictureIndex], gameImage.PictureColour[pictureIndex], j1, i1, l1, k1, i2, j2, byte0);
            }
            else
            {
                GameImageSpriteBlitter.DrawSpriteOpaque(pixels, gameImage.PictureColours[pictureIndex], j1, i1, l1, k1, i2, j2, byte0);
            }
        }

        internal void DrawPicture(int x, int y, int pictureIndex, int blendFactor)
        {
            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                x += gameImage.PictureOffsetX[pictureIndex];
                y += gameImage.PictureOffsetY[pictureIndex];
            }

            int[] pixels = gameImage.Pixels;
            int gameWidth = gameImage.GameWidth;
            int imageX = gameImage.ImageX;
            int imageY = gameImage.ImageY;
            int imageWidth = gameImage.ImageWidth;
            int imageHeight = gameImage.ImageHeight;

            int j1 = x + y * gameWidth;
            int k1 = 0;
            int l1 = gameImage.PictureHeight[pictureIndex];
            int i2 = gameImage.PictureWidth[pictureIndex];
            int j2 = gameWidth - i2;
            int k2 = 0;

            if (y < imageY)
            {
                int l2 = imageY - y;
                l1 -= l2;
                y = imageY;
                k1 += l2 * i2;
                j1 += l2 * gameWidth;
            }

            if (y + l1 >= imageHeight)
            {
                l1 -= y + l1 - imageHeight + 1;
            }

            if (x < imageX)
            {
                int i3 = imageX - x;
                i2 -= i3;
                x = imageX;
                k1 += i3;
                j1 += i3;
                k2 += i3;
                j2 += i3;
            }

            if (x + i2 >= imageWidth)
            {
                int j3 = x + i2 - imageWidth + 1;
                i2 -= j3;
                k2 += j3;
                j2 += j3;
            }

            if (i2 <= 0 || l1 <= 0)
            {
                return;
            }

            byte byte0 = 1;

            if (gameImage.IsInterlaced)
            {
                byte0 = 2;
                j2 += gameWidth;
                k2 += gameImage.PictureWidth[pictureIndex];

                if ((y & 1) != 0)
                {
                    j1 += gameWidth;
                    l1 -= 1;
                }
            }

            if (gameImage.PictureColours[pictureIndex] is null)
            {
                GameImageSpriteBlitter.DrawSpriteColorShiftedTextured(pixels, gameImage.PictureColourIndexes[pictureIndex], gameImage.PictureColour[pictureIndex], k1, j1, i2, l1, j2, k2, byte0, blendFactor);
            }
            else
            {
                GameImageSpriteBlitter.DrawSpriteColorShifted(pixels, gameImage.PictureColours[pictureIndex], k1, j1, i2, l1, j2, k2, byte0, blendFactor);
            }
        }

        internal void DrawEntity(int x, int y, int width, int height, int index)
        {
            try
            {
                int[] pixels = gameImage.Pixels;
                int gameWidth = gameImage.GameWidth;
                int imageX = gameImage.ImageX;
                int imageY = gameImage.ImageY;
                int imageWidth = gameImage.ImageWidth;
                int imageHeight = gameImage.ImageHeight;

                int k1 = gameImage.PictureWidth[index];
                int l1 = gameImage.PictureHeight[index];
                int i2 = 0;
                int j2 = 0;
                int k2 = (k1 << 16) / width;
                int l2 = (l1 << 16) / height;

                if (gameImage.HasTransparentBackground[index])
                {
                    int i3 = gameImage.PictureAssumedWidth[index];
                    int k3 = gameImage.PictureAssumedHeight[index];
                    k2 = (i3 << 16) / width;
                    l2 = (k3 << 16) / height;
                    x += (gameImage.PictureOffsetX[index] * width + i3 - 1) / i3;
                    y += (gameImage.PictureOffsetY[index] * height + k3 - 1) / k3;

                    if (gameImage.PictureOffsetX[index] * width % i3 != 0)
                    {
                        i2 = (i3 - gameImage.PictureOffsetX[index] * width % i3 << 16) / width;
                    }

                    if (gameImage.PictureOffsetY[index] * height % k3 != 0)
                    {
                        j2 = (k3 - gameImage.PictureOffsetY[index] * height % k3 << 16) / height;
                    }

                    width = width * (gameImage.PictureWidth[index] - (i2 >> 16)) / i3;
                    height = height * (gameImage.PictureHeight[index] - (j2 >> 16)) / k3;
                }

                int j3 = x + y * gameWidth;
                int l3 = gameWidth - width;

                if (y < imageY)
                {
                    int i4 = imageY - y;
                    height -= i4;
                    y = 0;
                    j3 += i4 * gameWidth;
                    j2 += l2 * i4;
                }

                if (y + height >= imageHeight)
                {
                    height -= y + height - imageHeight + 1;
                }

                if (x < imageX)
                {
                    int j4 = imageX - x;
                    width -= j4;
                    x = 0;
                    j3 += j4;
                    i2 += k2 * j4;
                    l3 += j4;
                }

                if (x + width >= imageWidth)
                {
                    int k4 = x + width - imageWidth + 1;
                    width -= k4;
                    l3 += k4;
                }

                byte byte0 = 1;

                if (gameImage.IsInterlaced)
                {
                    byte0 = 2;
                    l3 += gameWidth;
                    l2 += l2;

                    if ((y & 1) != 0)
                    {
                        j3 += gameWidth;
                        height -= 1;
                    }
                }

                GameImageSpriteBlitter.DrawSpriteTransparent(pixels, gameImage.PictureColours[index], i2, j2, j3, l3, width, height, k2, l2, k1, byte0);
            }
            catch (System.Exception)
            {
                logger.Error(GameOperation.RenderEntity, "Error in sprite clipping routine.");
            }
        }
    }
}
