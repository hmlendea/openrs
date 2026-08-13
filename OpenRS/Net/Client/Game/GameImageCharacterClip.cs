namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageCharacterClip
    {
        internal int DestinationOffset { get; }

        internal int Height { get; }

        internal int ScaleX { get; }

        internal int ScaleY { get; }

        internal int ScanlineMode { get; }

        internal int SourceWidth { get; }

        internal int SourceX { get; }

        internal int SourceY { get; }

        internal int Width { get; }

        internal int XPosition { get; }

        internal int XStep { get; }

        private GameImageCharacterClip(
            int destinationOffset,
            int height,
            int scaleX,
            int scaleY,
            int scanlineMode,
            int sourceWidth,
            int sourceX,
            int sourceY,
            int width,
            int xPosition,
            int xStep)
        {
            DestinationOffset = destinationOffset;
            Height = height;
            ScaleX = scaleX;
            ScaleY = scaleY;
            ScanlineMode = scanlineMode;
            SourceWidth = sourceWidth;
            SourceX = sourceX;
            SourceY = sourceY;
            Width = width;
            XPosition = xPosition;
            XStep = xStep;
        }

        internal static GameImageCharacterClip Calculate(
            GameImage gameImage,
            int positionX,
            int positionY,
            int width,
            int height,
            int pictureIndex,
            int shearFactor,
            bool isFlipped)
        {
            int gameWidth = gameImage.GameWidth;
            int pictureWidth = gameImage.PictureWidth[pictureIndex];
            int pictureHeight = gameImage.PictureHeight[pictureIndex];
            int sourceX = 0;
            int sourceY = 0;
            int xPosition = shearFactor << 16;
            int scaleX = (pictureWidth << 16) / width;
            int scaleY = (pictureHeight << 16) / height;
            int xStep = -(shearFactor << 16) / height;

            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                int assumedWidth = gameImage.PictureAssumedWidth[pictureIndex];
                int assumedHeight = gameImage.PictureAssumedHeight[pictureIndex];
                scaleX = (assumedWidth << 16) / width;
                scaleY = (assumedHeight << 16) / height;
                int pictureOffsetX = gameImage.PictureOffsetX[pictureIndex];
                int pictureOffsetY = gameImage.PictureOffsetY[pictureIndex];

                if (isFlipped)
                {
                    pictureOffsetX = assumedWidth - pictureWidth - pictureOffsetX;
                }

                positionX += (pictureOffsetX * width + assumedWidth - 1) / assumedWidth;
                int adjustedVerticalOffset =
                    (pictureOffsetY * height + assumedHeight - 1) /
                    assumedHeight;
                positionY += adjustedVerticalOffset;
                xPosition += adjustedVerticalOffset * xStep;

                if (pictureOffsetX * width % assumedWidth != 0)
                {
                    sourceX =
                        (assumedWidth - pictureOffsetX * width % assumedWidth << 16) /
                        width;
                }

                if (pictureOffsetY * height % assumedHeight != 0)
                {
                    sourceY =
                        (assumedHeight - pictureOffsetY * height % assumedHeight << 16) /
                        height;
                }

                width = ((pictureWidth << 16) - sourceX + scaleX - 1) / scaleX;
                height = ((pictureHeight << 16) - sourceY + scaleY - 1) / scaleY;
            }

            int destinationOffset = positionY * gameWidth;
            xPosition += positionX << 16;

            if (positionY < gameImage.ImageY)
            {
                int topClip = gameImage.ImageY - positionY;
                height -= topClip;
                positionY = gameImage.ImageY;
                destinationOffset += topClip * gameWidth;
                sourceY += scaleY * topClip;
                xPosition += xStep * topClip;
            }

            if (positionY + height >= gameImage.ImageHeight)
            {
                height -= positionY + height - gameImage.ImageHeight + 1;
            }

            int scanlineMode = 2;

            if (gameImage.IsInterlaced)
            {
                scanlineMode = destinationOffset / gameWidth & 1;
            }

            if (isFlipped)
            {
                sourceX = (pictureWidth << 16) - sourceX - 1;
                scaleX = -scaleX;
            }

            return new GameImageCharacterClip(
                destinationOffset,
                height,
                scaleX,
                scaleY,
                scanlineMode,
                pictureWidth,
                sourceX,
                sourceY,
                width,
                xPosition,
                xStep);
        }
    }
}