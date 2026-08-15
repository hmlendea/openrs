namespace OpenRS.Net.Client.Game
{
    internal sealed class PathRouteReconstructor(EngineHandle engineHandle)
    {
        private static int TileDirectionNorth => 1;

        private static int TileDirectionEast => 2;

        private static int TileDirectionSouth => 4;

        private static int TileDirectionWest => 8;

        internal int Reconstruct(
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