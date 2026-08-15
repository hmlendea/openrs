namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageSpriteClip
    {
        internal int DestinationOffset { get; }

        internal int DestinationRowStride { get; }

        internal int Height { get; }

        internal bool IsVisible { get; }

        internal byte RowStep { get; }

        internal int SourceOffset { get; }

        internal int SourceRowStride { get; }

        internal int Width { get; }

        private GameImageSpriteClip(
            int destinationOffset,
            int destinationRowStride,
            int height,
            byte rowStep,
            int sourceOffset,
            int sourceRowStride,
            int width)
        {
            DestinationOffset = destinationOffset;
            DestinationRowStride = destinationRowStride;
            Height = height;
            IsVisible = true;
            RowStep = rowStep;
            SourceOffset = sourceOffset;
            SourceRowStride = sourceRowStride;
            Width = width;
        }

        internal static GameImageSpriteClip Calculate(
            GameImage gameImage,
            int positionX,
            int positionY,
            int pictureIndex)
        {
            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                positionX += gameImage.PictureOffsetX[pictureIndex];
                positionY += gameImage.PictureOffsetY[pictureIndex];
            }

            int gameWidth = gameImage.GameWidth;
            int destinationOffset = positionX + positionY * gameWidth;
            int sourceOffset = 0;
            int height = gameImage.PictureHeight[pictureIndex];
            int width = gameImage.PictureWidth[pictureIndex];
            int destinationRowStride = gameWidth - width;
            int sourceRowStride = 0;

            if (positionY < gameImage.ImageY)
            {
                int topClip = gameImage.ImageY - positionY;
                height -= topClip;
                positionY = gameImage.ImageY;
                sourceOffset += topClip * width;
                destinationOffset += topClip * gameWidth;
            }

            if (positionY + height >= gameImage.ImageHeight)
            {
                height -= positionY + height - gameImage.ImageHeight + 1;
            }

            if (positionX < gameImage.ImageX)
            {
                int leftClip = gameImage.ImageX - positionX;
                width -= leftClip;
                positionX = gameImage.ImageX;
                sourceOffset += leftClip;
                destinationOffset += leftClip;
                sourceRowStride += leftClip;
                destinationRowStride += leftClip;
            }

            if (positionX + width >= gameImage.ImageWidth)
            {
                int rightClip = positionX + width - gameImage.ImageWidth + 1;
                width -= rightClip;
                sourceRowStride += rightClip;
                destinationRowStride += rightClip;
            }

            if (width <= 0 || height <= 0)
            {
                return default;
            }

            byte rowStep = 1;

            if (gameImage.IsInterlaced)
            {
                rowStep = 2;
                destinationRowStride += gameWidth;
                sourceRowStride += gameImage.PictureWidth[pictureIndex];

                if ((positionY & 1) != 0)
                {
                    destinationOffset += gameWidth;
                    height -= 1;
                }
            }

            return new GameImageSpriteClip(
                destinationOffset,
                destinationRowStride,
                height,
                rowStep,
                sourceOffset,
                sourceRowStride,
                width);
        }
    }
}