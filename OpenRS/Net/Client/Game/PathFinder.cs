namespace OpenRS.Net.Client.Game
{
    internal sealed class PathFinder
    {
        private readonly EngineHandle engineHandle;
        private readonly PathDestinationMatcher destinationMatcher;
        private readonly PathNeighbourExpander neighbourExpander;
        private readonly PathRouteReconstructor routeReconstructor;

        private static int StartingStepMarker => 99;

        internal PathFinder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
            destinationMatcher = new PathDestinationMatcher(engineHandle);
            neighbourExpander = new PathNeighbourExpander(engineHandle);
            routeReconstructor = new PathRouteReconstructor(engineHandle);
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

            return routeReconstructor.Reconstruct(
                curX,
                curY,
                bfsResult.CurrentX,
                bfsResult.CurrentY,
                pathX,
                pathY);
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

                if (PathDestinationMatcher.IsAtDestination(
                    currentX,
                    currentY,
                    destination))
                {
                    return new PathBfsResult(true, currentX, currentY);
                }

                if (checkForObjects &&
                    destinationMatcher.IsAdjacentToDestination(
                        currentX,
                        currentY,
                        destination))
                {
                    return new PathBfsResult(true, currentX, currentY);
                }

                neighbourExpander.Expand(currentX, currentY, searchQueue);
            }

            return new PathBfsResult(false, currentX, currentY);
        }

    }
}
