namespace OpenRS.Net.Client.Game
{
    internal sealed class PathDestinationMatcher(EngineHandle engineHandle)
    {
        private static int TileDirectionNorth => 1;

        private static int TileDirectionEast => 2;

        private static int TileDirectionSouth => 4;

        private static int TileDirectionWest => 8;

        internal static bool IsAtDestination(
            int currentX,
            int currentY,
            DestinationBounds destination)
            => currentX >= destination.BottomX &&
                currentX <= destination.UpperX &&
                currentY >= destination.BottomY &&
                currentY <= destination.UpperY;

        internal bool IsAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination)
            => IsWestTileAdjacentToDestination(currentX, currentY, destination) ||
                IsEastTileAdjacentToDestination(currentX, currentY, destination) ||
                IsSouthTileAdjacentToDestination(currentX, currentY, destination) ||
                IsNorthTileAdjacentToDestination(currentX, currentY, destination);

        private bool IsWestTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination)
            => currentX > 0 &&
                currentX - 1 >= destination.BottomX &&
                currentX - 1 <= destination.UpperX &&
                currentY >= destination.BottomY &&
                currentY <= destination.UpperY &&
                (engineHandle.Tiles[currentX - 1][currentY] &
                    TileDirectionWest) == 0;

        private bool IsEastTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination)
            => currentX < EngineHandle.GridSize - 1 &&
                currentX + 1 >= destination.BottomX &&
                currentX + 1 <= destination.UpperX &&
                currentY >= destination.BottomY &&
                currentY <= destination.UpperY &&
                (engineHandle.Tiles[currentX + 1][currentY] &
                    TileDirectionEast) == 0;

        private bool IsSouthTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination)
            => currentY > 0 &&
                currentX >= destination.BottomX &&
                currentX <= destination.UpperX &&
                currentY - 1 >= destination.BottomY &&
                currentY - 1 <= destination.UpperY &&
                (engineHandle.Tiles[currentX][currentY - 1] &
                    TileDirectionSouth) == 0;

        private bool IsNorthTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination)
            => currentY < EngineHandle.GridSize - 1 &&
                currentX >= destination.BottomX &&
                currentX <= destination.UpperX &&
                currentY + 1 >= destination.BottomY &&
                currentY + 1 <= destination.UpperY &&
                (engineHandle.Tiles[currentX][currentY + 1] &
                    TileDirectionNorth) == 0;
    }
}