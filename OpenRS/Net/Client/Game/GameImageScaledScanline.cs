namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageScaledScanline
    {
        internal int EndX { get; }

        internal int SourceX { get; }

        internal int StartX { get; }

        private GameImageScaledScanline(int endX, int sourceX, int startX)
        {
            EndX = endX;
            SourceX = sourceX;
            StartX = startX;
        }

        internal static GameImageScaledScanline Calculate(
            int sourceX,
            int xPosition,
            int width,
            int xStep,
            int imageX,
            int imageWidth)
        {
            int startX = xPosition >> 16;
            int clippedWidth = width;

            if (startX < imageX)
            {
                int leftClip = imageX - startX;
                clippedWidth -= leftClip;
                startX = imageX;
                sourceX += xStep * leftClip;
            }

            if (startX + clippedWidth >= imageWidth)
            {
                int rightClip = startX + clippedWidth - imageWidth;
                clippedWidth -= rightClip;
            }

            return new GameImageScaledScanline(
                startX + clippedWidth,
                sourceX,
                startX);
        }
    }
}