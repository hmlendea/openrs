namespace OpenRS.Net.Client.Game
{
    internal sealed class PathFinder
    {
        private readonly EngineHandle engineHandle;

        private static int StartingStepMarker => 99;

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

        private static int DiagonalStepNorthEast => TileDirectionEast | TileDirectionNorth;

        private static int DiagonalStepNorthWest => TileDirectionWest | TileDirectionNorth;

        private static int DiagonalStepSouthEast => TileDirectionEast | TileDirectionSouth;

        private static int DiagonalStepSouthWest => TileDirectionWest | TileDirectionSouth;

        internal PathFinder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal int GeneratePath(
            int curX,
            int curY,
            int bottomDestX,
            int bottomDestY,
            int upperDestX,
            int upperDestY,
            int[] pathX,
            int[] pathY,
            bool checkForObjects)
        {
            InitialiseStepsGrid();

            PathSearchQueue searchQueue = SetupSearch(curX, curY, pathX, pathY);
            DestinationBounds destination = new(bottomDestX, bottomDestY, upperDestX, upperDestY);
            PathBfsResult bfsResult = RunBreadthFirstSearch(searchQueue, destination, checkForObjects);

            if (!bfsResult.FoundPath)
            {
                return -1;
            }

            return ReconstructPath(curX, curY, bfsResult.CurrentX, bfsResult.CurrentY, pathX, pathY);
        }

        private void InitialiseStepsGrid()
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    engineHandle.Steps[tileX][tileY] = 0;
                }
            }
        }

        private PathSearchQueue SetupSearch(int curX, int curY, int[] pathX, int[] pathY)
        {
            engineHandle.Steps[curX][curY] = StartingStepMarker;

            PathSearchQueue searchQueue = new(pathX, pathY);
            searchQueue.Enqueue(curX, curY);

            return searchQueue;
        }

        private PathBfsResult RunBreadthFirstSearch(
            PathSearchQueue searchQueue,
            DestinationBounds destination,
            bool checkForObjects)
        {
            int currentX = 0;
            int currentY = 0;

            while (!searchQueue.IsEmpty)
            {
                currentX = searchQueue.CurrentX;
                currentY = searchQueue.CurrentY;
                searchQueue.Advance();

                if (IsAtDestination(currentX, currentY, destination))
                {
                    return new PathBfsResult(true, currentX, currentY);
                }

                if (checkForObjects &&
                    IsAdjacentToDestination(currentX, currentY, destination))
                {
                    return new PathBfsResult(true, currentX, currentY);
                }

                ExpandNeighbours(currentX, currentY, searchQueue);
            }

            return new PathBfsResult(false, currentX, currentY);
        }

        private static bool IsAtDestination(
            int currentX,
            int currentY,
            DestinationBounds destination) =>
            currentX >= destination.BottomX &&
            currentX <= destination.UpperX &&
            currentY >= destination.BottomY &&
            currentY <= destination.UpperY;

        private bool IsAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination) =>
            IsWestTileAdjacentToDestination(currentX, currentY, destination) ||
            IsEastTileAdjacentToDestination(currentX, currentY, destination) ||
            IsSouthTileAdjacentToDestination(currentX, currentY, destination) ||
            IsNorthTileAdjacentToDestination(currentX, currentY, destination);

        private bool IsWestTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination) =>
            currentX > 0 &&
            currentX - 1 >= destination.BottomX &&
            currentX - 1 <= destination.UpperX &&
            currentY >= destination.BottomY &&
            currentY <= destination.UpperY &&
            (engineHandle.Tiles[currentX - 1][currentY] & TileDirectionWest) == 0;

        private bool IsEastTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination) =>
            currentX < EngineHandle.GridSize - 1 &&
            currentX + 1 >= destination.BottomX &&
            currentX + 1 <= destination.UpperX &&
            currentY >= destination.BottomY &&
            currentY <= destination.UpperY &&
            (engineHandle.Tiles[currentX + 1][currentY] & TileDirectionEast) == 0;

        private bool IsSouthTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination) =>
            currentY > 0 &&
            currentX >= destination.BottomX &&
            currentX <= destination.UpperX &&
            currentY - 1 >= destination.BottomY &&
            currentY - 1 <= destination.UpperY &&
            (engineHandle.Tiles[currentX][currentY - 1] & TileDirectionSouth) == 0;

        private bool IsNorthTileAdjacentToDestination(
            int currentX,
            int currentY,
            DestinationBounds destination) =>
            currentY < EngineHandle.GridSize - 1 &&
            currentX >= destination.BottomX &&
            currentX <= destination.UpperX &&
            currentY + 1 >= destination.BottomY &&
            currentY + 1 <= destination.UpperY &&
            (engineHandle.Tiles[currentX][currentY + 1] & TileDirectionNorth) == 0;

        private void ExpandNeighbours(int currentX, int currentY, PathSearchQueue searchQueue)
        {
            ExpandCardinalNeighbours(currentX, currentY, searchQueue);
            ExpandDiagonalNeighbours(currentX, currentY, searchQueue);
        }

        private void ExpandCardinalNeighbours(
            int currentX,
            int currentY,
            PathSearchQueue searchQueue)
        {
            int gridMaxIndex = EngineHandle.GridSize - 1;

            if (currentX > 0 &&
                engineHandle.Steps[currentX - 1][currentY] == 0 &&
                (engineHandle.Tiles[currentX - 1][currentY] & WestMovementMask) == 0)
            {
                searchQueue.Enqueue(currentX - 1, currentY);
                engineHandle.Steps[currentX - 1][currentY] = TileDirectionEast;
            }

            if (currentX < gridMaxIndex &&
                engineHandle.Steps[currentX + 1][currentY] == 0 &&
                (engineHandle.Tiles[currentX + 1][currentY] & EastMovementMask) == 0)
            {
                searchQueue.Enqueue(currentX + 1, currentY);
                engineHandle.Steps[currentX + 1][currentY] = TileDirectionWest;
            }

            if (currentY > 0 &&
                engineHandle.Steps[currentX][currentY - 1] == 0 &&
                (engineHandle.Tiles[currentX][currentY - 1] & SouthMovementMask) == 0)
            {
                searchQueue.Enqueue(currentX, currentY - 1);
                engineHandle.Steps[currentX][currentY - 1] = TileDirectionNorth;
            }

            if (currentY < gridMaxIndex &&
                engineHandle.Steps[currentX][currentY + 1] == 0 &&
                (engineHandle.Tiles[currentX][currentY + 1] & NorthMovementMask) == 0)
            {
                searchQueue.Enqueue(currentX, currentY + 1);
                engineHandle.Steps[currentX][currentY + 1] = TileDirectionSouth;
            }
        }

        private void ExpandDiagonalNeighbours(
            int currentX,
            int currentY,
            PathSearchQueue searchQueue)
        {
            int gridMaxIndex = EngineHandle.GridSize - 1;

            if (currentX > 0 &&
                currentY > 0 &&
                (engineHandle.Tiles[currentX][currentY - 1] & SouthMovementMask) == 0 &&
                (engineHandle.Tiles[currentX - 1][currentY] & WestMovementMask) == 0 &&
                (engineHandle.Tiles[currentX - 1][currentY - 1] & SouthWestMovementMask) == 0 &&
                engineHandle.Steps[currentX - 1][currentY - 1] == 0)
            {
                searchQueue.Enqueue(currentX - 1, currentY - 1);
                engineHandle.Steps[currentX - 1][currentY - 1] = DiagonalStepNorthEast;
            }

            if (currentX < gridMaxIndex &&
                currentY > 0 &&
                (engineHandle.Tiles[currentX][currentY - 1] & SouthMovementMask) == 0 &&
                (engineHandle.Tiles[currentX + 1][currentY] & EastMovementMask) == 0 &&
                (engineHandle.Tiles[currentX + 1][currentY - 1] & SouthEastMovementMask) == 0 &&
                engineHandle.Steps[currentX + 1][currentY - 1] == 0)
            {
                searchQueue.Enqueue(currentX + 1, currentY - 1);
                engineHandle.Steps[currentX + 1][currentY - 1] = DiagonalStepNorthWest;
            }

            if (currentX > 0 &&
                currentY < gridMaxIndex &&
                (engineHandle.Tiles[currentX][currentY + 1] & NorthMovementMask) == 0 &&
                (engineHandle.Tiles[currentX - 1][currentY] & WestMovementMask) == 0 &&
                (engineHandle.Tiles[currentX - 1][currentY + 1] & NorthWestMovementMask) == 0 &&
                engineHandle.Steps[currentX - 1][currentY + 1] == 0)
            {
                searchQueue.Enqueue(currentX - 1, currentY + 1);
                engineHandle.Steps[currentX - 1][currentY + 1] = DiagonalStepSouthEast;
            }

            if (currentX < gridMaxIndex &&
                currentY < gridMaxIndex &&
                (engineHandle.Tiles[currentX][currentY + 1] & NorthMovementMask) == 0 &&
                (engineHandle.Tiles[currentX + 1][currentY] & EastMovementMask) == 0 &&
                (engineHandle.Tiles[currentX + 1][currentY + 1] & NorthEastMovementMask) == 0 &&
                engineHandle.Steps[currentX + 1][currentY + 1] == 0)
            {
                searchQueue.Enqueue(currentX + 1, currentY + 1);
                engineHandle.Steps[currentX + 1][currentY + 1] = DiagonalStepSouthWest;
            }
        }

        private int ReconstructPath(
            int startX,
            int startY,
            int currentX,
            int currentY,
            int[] pathX,
            int[] pathY)
        {
            int outputIndex = 0;
            pathX[outputIndex] = currentX;
            pathY[outputIndex] = currentY;
            outputIndex += 1;
            int previousDirection = engineHandle.Steps[currentX][currentY];
            int currentDirection = previousDirection;

            while (currentX != startX || currentY != startY)
            {
                if (currentDirection != previousDirection)
                {
                    previousDirection = currentDirection;
                    pathX[outputIndex] = currentX;
                    pathY[outputIndex] = currentY;
                    outputIndex += 1;
                }

                currentX = StepInX(currentX, currentDirection);
                currentY = StepInY(currentY, currentDirection);
                currentDirection = engineHandle.Steps[currentX][currentY];
            }

            return outputIndex;
        }

        private static int StepInX(int currentX, int direction)
        {
            if ((direction & TileDirectionEast) != 0)
            {
                return currentX + 1;
            }

            if ((direction & TileDirectionWest) != 0)
            {
                return currentX - 1;
            }

            return currentX;
        }

        private static int StepInY(int currentY, int direction)
        {
            if ((direction & TileDirectionNorth) != 0)
            {
                return currentY + 1;
            }

            if ((direction & TileDirectionSouth) != 0)
            {
                return currentY - 1;
            }

            return currentY;
        }
    }
}
