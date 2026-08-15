namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraHitTracker(int maximumHitCount)
    {
        private readonly GameObject[] highlightedObjects =
            new GameObject[maximumHitCount];
        private readonly int[] highlightedPlayerIds =
            new int[maximumHitCount];

        private bool isMousePositionUpdated;
        private int mouseAdjustedX;
        private int mouseAdjustedY;
        private int optionCount;

        internal bool IsHitCandidate
            => isMousePositionUpdated && optionCount < maximumHitCount;

        internal int MouseAdjustedX => mouseAdjustedX;

        internal int MouseAdjustedY => mouseAdjustedY;

        internal void SetMousePosition(int adjustedMouseX, int mouseY)
        {
            mouseAdjustedX = adjustedMouseX;
            mouseAdjustedY = mouseY;
            optionCount = 0;
            isMousePositionUpdated = true;
        }

        internal int GetOptionCount() => optionCount;

        internal int[] GetHighlightedPlayers() => highlightedPlayerIds;

        internal GameObject[] GetHighlightedObjects() => highlightedObjects;

        internal void RecordHit(GameObject gameObject, int faceIndex)
        {
            highlightedObjects[optionCount] = gameObject;
            highlightedPlayerIds[optionCount] = faceIndex;
            optionCount += 1;
        }

        internal void FinaliseFrame() => isMousePositionUpdated = false;
    }
}