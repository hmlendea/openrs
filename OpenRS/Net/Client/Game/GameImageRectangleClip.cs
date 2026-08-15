namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageRectangleClip
    {
        internal int DestinationOffset { get; }

        internal int Height { get; }

        internal int RowStep { get; }

        internal int RowStride { get; }

        internal int Width { get; }

        private GameImageRectangleClip(
            int destinationOffset,
            int height,
            int rowStep,
            int rowStride,
            int width)
        {
            DestinationOffset = destinationOffset;
            Height = height;
            RowStep = rowStep;
            RowStride = rowStride;
            Width = width;
        }

        internal static GameImageRectangleClip Calculate(
            GameImage image,
            int x,
            int y,
            int width,
            int height)
        {
            if (x < image.ImageX)
            {
                width -= image.ImageX - x;
                x = image.ImageX;
            }

            if (y < image.ImageY)
            {
                height -= image.ImageY - y;
                y = image.ImageY;
            }

            if (x + width > image.ImageWidth)
            {
                width = image.ImageWidth - x;
            }

            if (y + height > image.ImageHeight)
            {
                height = image.ImageHeight - y;
            }

            int rowStride = image.GameWidth - width;
            int rowStep = 1;

            if (image.IsInterlaced)
            {
                rowStep = 2;
                rowStride += image.GameWidth;

                if ((y & 1) != 0)
                {
                    y += 1;
                    height -= 1;
                }
            }

            return new GameImageRectangleClip(
                x + y * image.GameWidth,
                height,
                rowStep,
                rowStride,
                width);
        }
    }
}