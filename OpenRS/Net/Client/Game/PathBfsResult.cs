namespace OpenRS.Net.Client.Game
{
    internal sealed class PathBfsResult
    {
        internal bool FoundPath { get; }

        internal int CurrentX { get; }

        internal int CurrentY { get; }

        internal PathBfsResult(bool foundPath, int currentX, int currentY)
        {
            FoundPath = foundPath;
            CurrentX = currentX;
            CurrentY = currentY;
        }
    }
}
