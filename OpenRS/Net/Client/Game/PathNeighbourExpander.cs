namespace OpenRS.Net.Client.Game
{
    internal sealed class PathNeighbourExpander(EngineHandle engineHandle)
    {
        private static int TileDirectionNorth => 1;

        private static int TileDirectionEast => 2;

        private static int TileDirectionSouth => 4;

        private static int TileDirectionWest => 8;

        private static int WestMovementMask => 0x78;

        private static int EastMovementMask => 0x72;

        private static int SouthMovementMask => 0x74;

        private static int NorthMovementMask => 0x71;

        private static int SouthWestMovementMask => 0x7c;

        private static int SouthEastMovementMask => 0x76;

        private static int NorthWestMovementMask => 0x79;

        private static int NorthEastMovementMask => 0x73;

        private static int DiagonalStepNorthEast =>
            TileDirectionEast | TileDirectionNorth;

        private static int DiagonalStepNorthWest =>
            TileDirectionWest | TileDirectionNorth;

        private static int DiagonalStepSouthEast =>
            TileDirectionEast | TileDirectionSouth;

        private static int DiagonalStepSouthWest =>
            TileDirectionWest | TileDirectionSouth;

        internal void Expand(
            int currentX,
            int currentY,
            PathSearchQueue searchQueue)
        {
            ExpandCardinalNeighbours(currentX, currentY, searchQueue);
            ExpandDiagonalNeighbours(currentX, currentY, searchQueue);
        }

        private void ExpandCardinalNeighbours(
            int currentX,
            int currentY,
            PathSearchQueue searchQueue)
        {
            TryExpandCardinalNeighbour(
                currentX - 1,
                currentY,
                WestMovementMask,
                TileDirectionEast,
                searchQueue);
            TryExpandCardinalNeighbour(
                currentX + 1,
                currentY,
                EastMovementMask,
                TileDirectionWest,
                searchQueue);
            TryExpandCardinalNeighbour(
                currentX,
                currentY - 1,
                SouthMovementMask,
                TileDirectionNorth,
                searchQueue);
            TryExpandCardinalNeighbour(
                currentX,
                currentY + 1,
                NorthMovementMask,
                TileDirectionSouth,
                searchQueue);
        }

        private void TryExpandCardinalNeighbour(
            int targetX,
            int targetY,
            int movementMask,
            int returnDirection,
            PathSearchQueue searchQueue)
        {
            if (targetX < 0 ||
                targetX >= EngineHandle.GridSize ||
                targetY < 0 ||
                targetY >= EngineHandle.GridSize ||
                engineHandle.Steps[targetX][targetY] != 0 ||
                (engineHandle.Tiles[targetX][targetY] & movementMask) != 0)
            {
                return;
            }

            searchQueue.Enqueue(targetX, targetY);
            engineHandle.Steps[targetX][targetY] = returnDirection;
        }

        private void ExpandDiagonalNeighbours(
            int currentX,
            int currentY,
            PathSearchQueue searchQueue)
        {
            TryExpandDiagonalNeighbour(
                currentX,
                currentY,
                -1,
                -1,
                WestMovementMask,
                SouthMovementMask,
                SouthWestMovementMask,
                DiagonalStepNorthEast,
                searchQueue);
            TryExpandDiagonalNeighbour(
                currentX,
                currentY,
                1,
                -1,
                EastMovementMask,
                SouthMovementMask,
                SouthEastMovementMask,
                DiagonalStepNorthWest,
                searchQueue);
            TryExpandDiagonalNeighbour(
                currentX,
                currentY,
                -1,
                1,
                WestMovementMask,
                NorthMovementMask,
                NorthWestMovementMask,
                DiagonalStepSouthEast,
                searchQueue);
            TryExpandDiagonalNeighbour(
                currentX,
                currentY,
                1,
                1,
                EastMovementMask,
                NorthMovementMask,
                NorthEastMovementMask,
                DiagonalStepSouthWest,
                searchQueue);
        }

        private void TryExpandDiagonalNeighbour(
            int currentX,
            int currentY,
            int offsetX,
            int offsetY,
            int horizontalMovementMask,
            int verticalMovementMask,
            int diagonalMovementMask,
            int returnDirection,
            PathSearchQueue searchQueue)
        {
            int targetX = currentX + offsetX;
            int targetY = currentY + offsetY;

            if (targetX < 0 ||
                targetX >= EngineHandle.GridSize ||
                targetY < 0 ||
                targetY >= EngineHandle.GridSize ||
                (engineHandle.Tiles[targetX][currentY] & horizontalMovementMask) != 0 ||
                (engineHandle.Tiles[currentX][targetY] & verticalMovementMask) != 0 ||
                (engineHandle.Tiles[targetX][targetY] & diagonalMovementMask) != 0 ||
                engineHandle.Steps[targetX][targetY] != 0)
            {
                return;
            }

            searchQueue.Enqueue(targetX, targetY);
            engineHandle.Steps[targetX][targetY] = returnDirection;
        }
    }
}