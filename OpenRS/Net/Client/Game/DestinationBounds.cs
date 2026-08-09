namespace OpenRS.Net.Client.Game
{
    internal sealed class DestinationBounds
    {
        internal int BottomX { get; }

        internal int BottomY { get; }

        internal int UpperX { get; }

        internal int UpperY { get; }

        internal DestinationBounds(int bottomX, int bottomY, int upperX, int upperY)
        {
            BottomX = bottomX;
            BottomY = bottomY;
            UpperX = upperX;
            UpperY = upperY;
        }
    }
}
