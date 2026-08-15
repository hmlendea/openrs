namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageEntityClip
    {
        internal int DestinationOffset { get; }

        internal int DestinationRowStride { get; }

        internal int Height { get; }

        internal byte RowStep { get; }

        internal int ScaleX { get; }

        internal int ScaleY { get; }

        internal int SourceWidth { get; }

        internal int SourceX { get; }

        internal int SourceY { get; }

        internal int Width { get; }

        private GameImageEntityClip(
            int destinationOffset,
            int destinationRowStride,
            int height,
            byte rowStep,
            int scaleX,
            int scaleY,
            int sourceWidth,
            int sourceX,
            int sourceY,
            int width)
        {
            DestinationOffset = destinationOffset;
            DestinationRowStride = destinationRowStride;
            Height = height;
            RowStep = rowStep;
            ScaleX = scaleX;
            ScaleY = scaleY;
            SourceWidth = sourceWidth;
            SourceX = sourceX;
            SourceY = sourceY;
            Width = width;
        }

        internal static GameImageEntityClip Calculate(
            GameImage gameImage,
            int positionX,
            int positionY,
            int width,
            int height,
            int pictureIndex)
        {
            int gameWidth = gameImage.GameWidth;
            int sourceWidth = gameImage.PictureWidth[pictureIndex];
            int sourceHeight = gameImage.PictureHeight[pictureIndex];
            int sourceX = 0;
            int sourceY = 0;
            int scaleX = (sourceWidth << 16) / width;
            int scaleY = (sourceHeight << 16) / height;

            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                int assumedWidth = gameImage.PictureAssumedWidth[pictureIndex];
                int assumedHeight = gameImage.PictureAssumedHeight[pictureIndex];
                scaleX = (assumedWidth << 16) / width;
                scaleY = (assumedHeight << 16) / height;
                positionX +=
                    (gameImage.PictureOffsetX[pictureIndex] * width + assumedWidth - 1) /
                    assumedWidth;
                positionY +=
                    (gameImage.PictureOffsetY[pictureIndex] * height + assumedHeight - 1) /
                    assumedHeight;

                if (gameImage.PictureOffsetX[pictureIndex] * width % assumedWidth != 0)
                {
                    sourceX =
                        (assumedWidth -
                        gameImage.PictureOffsetX[pictureIndex] * width % assumedWidth << 16) /
                        width;
                }

                if (gameImage.PictureOffsetY[pictureIndex] * height % assumedHeight != 0)
                {
                    sourceY =
                        (assumedHeight -
                        gameImage.PictureOffsetY[pictureIndex] * height % assumedHeight << 16) /
                        height;
                }

                width = width * (gameImage.PictureWidth[pictureIndex] - (sourceX >> 16)) /
                    assumedWidth;
                height = height * (gameImage.PictureHeight[pictureIndex] - (sourceY >> 16)) /
                    assumedHeight;
            }

            int destinationOffset = positionX + positionY * gameWidth;
            int destinationRowStride = gameWidth - width;

            if (positionY < gameImage.ImageY)
            {
                int topClip = gameImage.ImageY - positionY;
                height -= topClip;
                positionY = 0;
                destinationOffset += topClip * gameWidth;
                sourceY += scaleY * topClip;
            }

            if (positionY + height >= gameImage.ImageHeight)
            {
                height -= positionY + height - gameImage.ImageHeight + 1;
            }

            if (positionX < gameImage.ImageX)
            {
                int leftClip = gameImage.ImageX - positionX;
                width -= leftClip;
                positionX = 0;
                destinationOffset += leftClip;
                sourceX += scaleX * leftClip;
                destinationRowStride += leftClip;
            }

            if (positionX + width >= gameImage.ImageWidth)
            {
                int rightClip = positionX + width - gameImage.ImageWidth + 1;
                width -= rightClip;
                destinationRowStride += rightClip;
            }

            byte rowStep = 1;

            if (gameImage.IsInterlaced)
            {
                rowStep = 2;
                destinationRowStride += gameWidth;
                scaleY += scaleY;

                if ((positionY & 1) != 0)
                {
                    destinationOffset += gameWidth;
                    height -= 1;
                }
            }

            return new GameImageEntityClip(
                destinationOffset,
                destinationRowStride,
                height,
                rowStep,
                scaleX,
                scaleY,
                sourceWidth,
                sourceX,
                sourceY,
                width);
        }
    }
}