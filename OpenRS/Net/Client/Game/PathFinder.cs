namespace OpenRS.Net.Client.Game
{
    internal sealed class PathFinder
    {
        private readonly EngineHandle engineHandle;

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
            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    engineHandle.steps[tileX][tileY] = 0;
                }
            }

            int writeIndex = 0;
            int readIndex = 0;
            int currentX = curX;
            int currentY = curY;
            engineHandle.steps[curX][curY] = 99;
            pathX[writeIndex] = curX;
            pathY[writeIndex++] = curY;
            int pathCapacity = pathX.Length;
            bool foundPath = false;

            while (readIndex != writeIndex)
            {
                currentX = pathX[readIndex];
                currentY = pathY[readIndex];
                readIndex = (readIndex + 1) % pathCapacity;

                if (currentX >= bottomDestX &&
                    currentX <= upperDestX &&
                    currentY >= bottomDestY &&
                    currentY <= upperDestY)
                {
                    foundPath = true;
                    break;
                }

                if (checkForObjects)
                {
                    if (currentX > 0 &&
                        currentX - 1 >= bottomDestX &&
                        currentX - 1 <= upperDestX &&
                        currentY >= bottomDestY &&
                        currentY <= upperDestY &&
                        (engineHandle.tiles[currentX - 1][currentY] & 8) == 0)
                    {
                        foundPath = true;
                        break;
                    }

                    if (currentX < 95 &&
                        currentX + 1 >= bottomDestX &&
                        currentX + 1 <= upperDestX &&
                        currentY >= bottomDestY &&
                        currentY <= upperDestY &&
                        (engineHandle.tiles[currentX + 1][currentY] & 2) == 0)
                    {
                        foundPath = true;
                        break;
                    }

                    if (currentY > 0 &&
                        currentX >= bottomDestX &&
                        currentX <= upperDestX &&
                        currentY - 1 >= bottomDestY &&
                        currentY - 1 <= upperDestY &&
                        (engineHandle.tiles[currentX][currentY - 1] & 4) == 0)
                    {
                        foundPath = true;
                        break;
                    }

                    if (currentY < 95 &&
                        currentX >= bottomDestX &&
                        currentX <= upperDestX &&
                        currentY + 1 >= bottomDestY &&
                        currentY + 1 <= upperDestY &&
                        (engineHandle.tiles[currentX][currentY + 1] & 1) == 0)
                    {
                        foundPath = true;
                        break;
                    }
                }

                if (currentX > 0 && engineHandle.steps[currentX - 1][currentY] == 0 && (engineHandle.tiles[currentX - 1][currentY] & 0x78) == 0)
                {
                    pathX[writeIndex] = currentX - 1;
                    pathY[writeIndex] = currentY;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX - 1][currentY] = 2;
                }

                if (currentX < 95 && engineHandle.steps[currentX + 1][currentY] == 0 && (engineHandle.tiles[currentX + 1][currentY] & 0x72) == 0)
                {
                    pathX[writeIndex] = currentX + 1;
                    pathY[writeIndex] = currentY;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX + 1][currentY] = 8;
                }

                if (currentY > 0 && engineHandle.steps[currentX][currentY - 1] == 0 && (engineHandle.tiles[currentX][currentY - 1] & 0x74) == 0)
                {
                    pathX[writeIndex] = currentX;
                    pathY[writeIndex] = currentY - 1;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX][currentY - 1] = 1;
                }

                if (currentY < 95 && engineHandle.steps[currentX][currentY + 1] == 0 && (engineHandle.tiles[currentX][currentY + 1] & 0x71) == 0)
                {
                    pathX[writeIndex] = currentX;
                    pathY[writeIndex] = currentY + 1;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX][currentY + 1] = 4;
                }

                if (currentX > 0 &&
                    currentY > 0 &&
                    (engineHandle.tiles[currentX][currentY - 1] & 0x74) == 0 &&
                    (engineHandle.tiles[currentX - 1][currentY] & 0x78) == 0 &&
                    (engineHandle.tiles[currentX - 1][currentY - 1] & 0x7c) == 0 &&
                    engineHandle.steps[currentX - 1][currentY - 1] == 0)
                {
                    pathX[writeIndex] = currentX - 1;
                    pathY[writeIndex] = currentY - 1;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX - 1][currentY - 1] = 3;
                }

                if (currentX < 95 &&
                    currentY > 0 &&
                    (engineHandle.tiles[currentX][currentY - 1] & 0x74) == 0 &&
                    (engineHandle.tiles[currentX + 1][currentY] & 0x72) == 0 &&
                    (engineHandle.tiles[currentX + 1][currentY - 1] & 0x76) == 0 &&
                    engineHandle.steps[currentX + 1][currentY - 1] == 0)
                {
                    pathX[writeIndex] = currentX + 1;
                    pathY[writeIndex] = currentY - 1;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX + 1][currentY - 1] = 9;
                }

                if (currentX > 0 &&
                    currentY < 95 &&
                    (engineHandle.tiles[currentX][currentY + 1] & 0x71) == 0 &&
                    (engineHandle.tiles[currentX - 1][currentY] & 0x78) == 0 &&
                    (engineHandle.tiles[currentX - 1][currentY + 1] & 0x79) == 0 &&
                    engineHandle.steps[currentX - 1][currentY + 1] == 0)
                {
                    pathX[writeIndex] = currentX - 1;
                    pathY[writeIndex] = currentY + 1;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX - 1][currentY + 1] = 6;
                }

                if (currentX < 95 &&
                    currentY < 95 &&
                    (engineHandle.tiles[currentX][currentY + 1] & 0x71) == 0 &&
                    (engineHandle.tiles[currentX + 1][currentY] & 0x72) == 0 &&
                    (engineHandle.tiles[currentX + 1][currentY + 1] & 0x73) == 0 &&
                    engineHandle.steps[currentX + 1][currentY + 1] == 0)
                {
                    pathX[writeIndex] = currentX + 1;
                    pathY[writeIndex] = currentY + 1;
                    writeIndex = (writeIndex + 1) % pathCapacity;
                    engineHandle.steps[currentX + 1][currentY + 1] = 12;
                }
            }

            if (!foundPath)
            {
                return -1;
            }

            int outputIndex = 0;
            pathX[outputIndex] = currentX;
            pathY[outputIndex++] = currentY;
            int previousDirection = engineHandle.steps[currentX][currentY];
            int currentDirection = previousDirection;

            for (; currentX != curX || currentY != curY; currentDirection = engineHandle.steps[currentX][currentY])
            {
                if (currentDirection != previousDirection)
                {
                    previousDirection = currentDirection;
                    pathX[outputIndex] = currentX;
                    pathY[outputIndex++] = currentY;
                }

                if ((currentDirection & 2) != 0)
                {
                    currentX += 1;
                }
                else if ((currentDirection & 8) != 0)
                {
                    currentX -= 1;
                }

                if ((currentDirection & 1) != 0)
                {
                    currentY += 1;
                }
                else if ((currentDirection & 4) != 0)
                {
                    currentY -= 1;
                }
            }

            return outputIndex;
        }
    }
}
