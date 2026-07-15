namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofBuilder
    {
        private readonly EngineHandle engineHandle;

        internal RoofBuilder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void BuildRoof(int height)
        {
            AssignRoofTiles();
            CalculateRoofHeights();
            BuildRoofGeometry(height);
            FinaliseRoofSection(height);
        }

        private void AssignRoofTiles()
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    int wallType = engineHandle.GetHorizontalWall(tileX, tileY);

                    if (wallType > 0)
                    {
                        engineHandle.SetRoofTile(wallType - 1, tileX, tileY, tileX + 1, tileY);
                    }

                    wallType = engineHandle.GetVerticalWall(tileX, tileY);

                    if (wallType > 0)
                    {
                        engineHandle.SetRoofTile(wallType - 1, tileX, tileY, tileX, tileY + 1);
                    }

                    wallType = engineHandle.GetDiagonalWall(tileX, tileY);

                    if (wallType > 0 && wallType < EngineHandle.DiagonalWallBackOffset)
                    {
                        engineHandle.SetRoofTile(wallType - 1, tileX, tileY, tileX + 1, tileY + 1);
                    }

                    if (wallType > EngineHandle.DiagonalWallBackOffset && wallType < 24000)
                    {
                        engineHandle.SetRoofTile(wallType - EngineHandle.DiagonalWallBackOffset - 1, tileX + 1, tileY, tileX, tileY + 1);
                    }
                }
            }
        }

        private void CalculateRoofHeights()
        {
            for (int tileX = 1; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 1; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    int roofType = engineHandle.GetTileRoofType(tileX, tileY);

                    if (roofType <= 0)
                    {
                        continue;
                    }

                    int cornerTopLeftX = tileX;
                    int cornerTopLeftY = tileY;
                    int cornerTopRightX = tileX + 1;
                    int cornerTopRightY = tileY;
                    int cornerBottomRightX = tileX + 1;
                    int cornerBottomRightY = tileY + 1;
                    int cornerBottomLeftX = tileX;
                    int cornerBottomLeftY = tileY + 1;
                    int maxRoofHeight = 0;
                    int cornerHeightTopLeft = engineHandle.roofTiles[cornerTopLeftX][cornerTopLeftY];
                    int cornerHeightTopRight = engineHandle.roofTiles[cornerTopRightX][cornerTopRightY];
                    int cornerHeightBottomRight = engineHandle.roofTiles[cornerBottomRightX][cornerBottomRightY];
                    int cornerHeightBottomLeft = engineHandle.roofTiles[cornerBottomLeftX][cornerBottomLeftY];

                    if (cornerHeightTopLeft > EngineHandle.RoofElevationFlag)
                    {
                        cornerHeightTopLeft -= EngineHandle.RoofElevationFlag;
                    }

                    if (cornerHeightTopRight > EngineHandle.RoofElevationFlag)
                    {
                        cornerHeightTopRight -= EngineHandle.RoofElevationFlag;
                    }

                    if (cornerHeightBottomRight > EngineHandle.RoofElevationFlag)
                    {
                        cornerHeightBottomRight -= EngineHandle.RoofElevationFlag;
                    }

                    if (cornerHeightBottomLeft > EngineHandle.RoofElevationFlag)
                    {
                        cornerHeightBottomLeft -= EngineHandle.RoofElevationFlag;
                    }

                    if (cornerHeightTopLeft > maxRoofHeight)
                    {
                        maxRoofHeight = cornerHeightTopLeft;
                    }

                    if (cornerHeightTopRight > maxRoofHeight)
                    {
                        maxRoofHeight = cornerHeightTopRight;
                    }

                    if (cornerHeightBottomRight > maxRoofHeight)
                    {
                        maxRoofHeight = cornerHeightBottomRight;
                    }

                    if (cornerHeightBottomLeft > maxRoofHeight)
                    {
                        maxRoofHeight = cornerHeightBottomLeft;
                    }

                    if (maxRoofHeight >= EngineHandle.RoofElevationFlag)
                    {
                        maxRoofHeight -= EngineHandle.RoofElevationFlag;
                    }

                    if (engineHandle.roofTiles[cornerTopLeftX][cornerTopLeftY] < EngineHandle.RoofElevationFlag)
                    {
                        engineHandle.roofTiles[cornerTopLeftX][cornerTopLeftY] = maxRoofHeight;
                    }
                    else
                    {
                        engineHandle.roofTiles[cornerTopLeftX][cornerTopLeftY] -= EngineHandle.RoofElevationFlag;
                    }

                    if (engineHandle.roofTiles[cornerTopRightX][cornerTopRightY] < EngineHandle.RoofElevationFlag)
                    {
                        engineHandle.roofTiles[cornerTopRightX][cornerTopRightY] = maxRoofHeight;
                    }
                    else
                    {
                        engineHandle.roofTiles[cornerTopRightX][cornerTopRightY] -= EngineHandle.RoofElevationFlag;
                    }

                    if (engineHandle.roofTiles[cornerBottomRightX][cornerBottomRightY] < EngineHandle.RoofElevationFlag)
                    {
                        engineHandle.roofTiles[cornerBottomRightX][cornerBottomRightY] = maxRoofHeight;
                    }
                    else
                    {
                        engineHandle.roofTiles[cornerBottomRightX][cornerBottomRightY] -= EngineHandle.RoofElevationFlag;
                    }

                    if (engineHandle.roofTiles[cornerBottomLeftX][cornerBottomLeftY] < EngineHandle.RoofElevationFlag)
                    {
                        engineHandle.roofTiles[cornerBottomLeftX][cornerBottomLeftY] = maxRoofHeight;
                    }
                    else
                    {
                        engineHandle.roofTiles[cornerBottomLeftX][cornerBottomLeftY] -= EngineHandle.RoofElevationFlag;
                    }
                }
            }
        }

        private void BuildRoofGeometry(int height)
        {
            engineHandle.currentSectionObject.ResetObjectIndexes();

            for (int tileX = 1; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 1; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    int roofType = engineHandle.GetTileRoofType(tileX, tileY);

                    if (roofType <= 0)
                    {
                        continue;
                    }

                    int cornerTopLeftX = tileX;
                    int cornerTopLeftY = tileY;
                    int cornerTopRightX = tileX + 1;
                    int cornerTopRightY = tileY;
                    int cornerBottomRightX = tileX + 1;
                    int cornerBottomRightY = tileY + 1;
                    int cornerBottomLeftX = tileX;
                    int cornerBottomLeftY = tileY + 1;
                    int worldTopLeftX = tileX * EngineHandle.TileWorldSize;
                    int worldTopLeftY = tileY * EngineHandle.TileWorldSize;
                    int worldTopRightX = worldTopLeftX + EngineHandle.TileWorldSize;
                    int worldTopRightY = worldTopLeftY;
                    int worldBottomRightX = worldTopRightX;
                    int worldBottomRightY = worldTopLeftY + EngineHandle.TileWorldSize;
                    int worldBottomLeftX = worldTopLeftX;
                    int worldBottomLeftY = worldBottomRightY;
                    int elevTopLeft = engineHandle.roofTiles[cornerTopLeftX][cornerTopLeftY];
                    int elevTopRight = engineHandle.roofTiles[cornerTopRightX][cornerTopRightY];
                    int elevBottomRight = engineHandle.roofTiles[cornerBottomRightX][cornerBottomRightY];
                    int elevBottomLeft = engineHandle.roofTiles[cornerBottomLeftX][cornerBottomLeftY];
                    int roofRaiseAmount = engineHandle.entityManager.GetElevation(roofType - 1).Roof;

                    if (engineHandle.IsRoofTile(cornerTopLeftX, cornerTopLeftY) && elevTopLeft < EngineHandle.RoofElevationFlag)
                    {
                        elevTopLeft += roofRaiseAmount + EngineHandle.RoofElevationFlag;
                        engineHandle.roofTiles[cornerTopLeftX][cornerTopLeftY] = elevTopLeft;
                    }

                    if (engineHandle.IsRoofTile(cornerTopRightX, cornerTopRightY) && elevTopRight < EngineHandle.RoofElevationFlag)
                    {
                        elevTopRight += roofRaiseAmount + EngineHandle.RoofElevationFlag;
                        engineHandle.roofTiles[cornerTopRightX][cornerTopRightY] = elevTopRight;
                    }

                    if (engineHandle.IsRoofTile(cornerBottomRightX, cornerBottomRightY) && elevBottomRight < EngineHandle.RoofElevationFlag)
                    {
                        elevBottomRight += roofRaiseAmount + EngineHandle.RoofElevationFlag;
                        engineHandle.roofTiles[cornerBottomRightX][cornerBottomRightY] = elevBottomRight;
                    }

                    if (engineHandle.IsRoofTile(cornerBottomLeftX, cornerBottomLeftY) && elevBottomLeft < EngineHandle.RoofElevationFlag)
                    {
                        elevBottomLeft += roofRaiseAmount + EngineHandle.RoofElevationFlag;
                        engineHandle.roofTiles[cornerBottomLeftX][cornerBottomLeftY] = elevBottomLeft;
                    }

                    if (elevTopLeft >= EngineHandle.RoofElevationFlag)
                    {
                        elevTopLeft -= EngineHandle.RoofElevationFlag;
                    }

                    if (elevTopRight >= EngineHandle.RoofElevationFlag)
                    {
                        elevTopRight -= EngineHandle.RoofElevationFlag;
                    }

                    if (elevBottomRight >= EngineHandle.RoofElevationFlag)
                    {
                        elevBottomRight -= EngineHandle.RoofElevationFlag;
                    }

                    if (elevBottomLeft >= EngineHandle.RoofElevationFlag)
                    {
                        elevBottomLeft -= EngineHandle.RoofElevationFlag;
                    }

                    int roofEdgeInset = 16;

                    if (!engineHandle.HasRoofTiles(cornerTopLeftX - 1, cornerTopLeftY))
                    {
                        worldTopLeftX -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopLeftX + 1, cornerTopLeftY))
                    {
                        worldTopLeftX += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopLeftX, cornerTopLeftY - 1))
                    {
                        worldTopLeftY -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopLeftX, cornerTopLeftY + 1))
                    {
                        worldTopLeftY += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopRightX - 1, cornerTopRightY))
                    {
                        worldTopRightX -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopRightX + 1, cornerTopRightY))
                    {
                        worldTopRightX += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopRightX, cornerTopRightY - 1))
                    {
                        worldTopRightY -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerTopRightX, cornerTopRightY + 1))
                    {
                        worldTopRightY += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomRightX - 1, cornerBottomRightY))
                    {
                        worldBottomRightX -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomRightX + 1, cornerBottomRightY))
                    {
                        worldBottomRightX += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomRightX, cornerBottomRightY - 1))
                    {
                        worldBottomRightY -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomRightX, cornerBottomRightY + 1))
                    {
                        worldBottomRightY += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomLeftX - 1, cornerBottomLeftY))
                    {
                        worldBottomLeftX -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomLeftX + 1, cornerBottomLeftY))
                    {
                        worldBottomLeftX += roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomLeftX, cornerBottomLeftY - 1))
                    {
                        worldBottomLeftY -= roofEdgeInset;
                    }

                    if (!engineHandle.HasRoofTiles(cornerBottomLeftX, cornerBottomLeftY + 1))
                    {
                        worldBottomLeftY += roofEdgeInset;
                    }

                    int roofColour = engineHandle.entityManager.GetElevation(roofType - 1).Colour;
                    elevTopLeft = -elevTopLeft;
                    elevTopRight = -elevTopRight;
                    elevBottomRight = -elevBottomRight;
                    elevBottomLeft = -elevBottomLeft;
                    AddRoofFaces(
                        tileX,
                        tileY,
                        roofColour,
                        worldTopLeftX,
                        worldTopLeftY,
                        worldTopRightX,
                        worldTopRightY,
                        worldBottomRightX,
                        worldBottomRightY,
                        worldBottomLeftX,
                        worldBottomLeftY,
                        elevTopLeft,
                        elevTopRight,
                        elevBottomRight,
                        elevBottomLeft);
                }
            }
        }

        private void AddRoofFaces(
            int tileX,
            int tileY,
            int roofColour,
            int worldTopLeftX,
            int worldTopLeftY,
            int worldTopRightX,
            int worldTopRightY,
            int worldBottomRightX,
            int worldBottomRightY,
            int worldBottomLeftX,
            int worldBottomLeftY,
            int elevTopLeft,
            int elevTopRight,
            int elevBottomRight,
            int elevBottomLeft)
        {
            int diagonalWall = engineHandle.GetDiagonalWall(tileX, tileY);

            if (diagonalWall > EngineHandle.DiagonalWallBackOffset &&
                diagonalWall < 24000 &&
                engineHandle.GetTileRoofType(tileX - 1, tileY - 1) == 0)
            {
                int[] faceVerts =
                [
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
            ];
                engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts, roofColour, EngineHandle.EmptyTileColour);
            }
            else if (diagonalWall > EngineHandle.DiagonalWallBackOffset &&
                     diagonalWall < 24000 &&
                     engineHandle.GetTileRoofType(tileX + 1, tileY + 1) == 0)
            {
                int[] faceVerts =
                [
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
            ];
                engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts, roofColour, EngineHandle.EmptyTileColour);
            }
            else if (diagonalWall > 0 &&
                     diagonalWall < EngineHandle.DiagonalWallBackOffset &&
                     engineHandle.GetTileRoofType(tileX + 1, tileY - 1) == 0)
            {
                int[] faceVerts =
                [
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
            ];
                engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts, roofColour, EngineHandle.EmptyTileColour);
            }
            else if (diagonalWall > 0 &&
                     diagonalWall < EngineHandle.DiagonalWallBackOffset &&
                     engineHandle.GetTileRoofType(tileX - 1, tileY + 1) == 0)
            {
                int[] faceVerts =
                [
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
            ];
                engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts, roofColour, EngineHandle.EmptyTileColour);
            }
            else if (elevTopLeft == elevTopRight && elevBottomRight == elevBottomLeft)
            {
                int[] faceVerts =
                [
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
            ];
                engineHandle.currentSectionObject.AddFaceVertices(4, faceVerts, roofColour, EngineHandle.EmptyTileColour);
            }
            else if (elevTopLeft == elevBottomLeft && elevTopRight == elevBottomRight)
            {
                int[] faceVerts =
                [
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
            ];
                engineHandle.currentSectionObject.AddFaceVertices(4, faceVerts, roofColour, EngineHandle.EmptyTileColour);
            }
            else
            {
                bool hasNoRoof = !(engineHandle.GetTileRoofType(tileX - 1, tileY - 1) > 0);

                if (engineHandle.GetTileRoofType(tileX + 1, tileY + 1) > 0)
                {
                    hasNoRoof = false;
                }

                if (!hasNoRoof)
                {
                    int[] faceVerts1 =
                    [
                        engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                ];
                    engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts1, roofColour, EngineHandle.EmptyTileColour);

                    int[] faceVerts2 =
                    [
                        engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
                ];
                    engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts2, roofColour, EngineHandle.EmptyTileColour);
                }
                else
                {
                    if (engineHandle.currentSectionObject is null)
                    {
                        return;
                    }

                    int[] faceVerts1 =
                    [
                        engineHandle.currentSectionObject.GetVertexIndex(worldTopLeftX, elevTopLeft, worldTopLeftY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
                ];
                    engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts1, roofColour, EngineHandle.EmptyTileColour);

                    int[] faceVerts2 =
                    [
                        engineHandle.currentSectionObject.GetVertexIndex(worldBottomRightX, elevBottomRight, worldBottomRightY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldBottomLeftX, elevBottomLeft, worldBottomLeftY),
                    engineHandle.currentSectionObject.GetVertexIndex(worldTopRightX, elevTopRight, worldTopRightY),
                ];
                    engineHandle.currentSectionObject.AddFaceVertices(3, faceVerts2, roofColour, EngineHandle.EmptyTileColour);
                }
            }
        }

        private void FinaliseRoofSection(int height)
        {
            engineHandle.currentSectionObject.UpdateShading(true, 50, 50, -50, -10, -50);
            engineHandle.roofObject[height] = engineHandle.currentSectionObject.GetObjectsWithinArea(0, 0, 1536, 1536, 8, EngineHandle.ChunkCount, 169, true);

            for (int chunkIndex = 0; chunkIndex < EngineHandle.ChunkCount; chunkIndex += 1)
            {
                engineHandle.camera.AddModel(engineHandle.roofObject[height][chunkIndex]);
            }

            if (engineHandle.roofObject[height][0] is null)
            {
                throw new System.Exception("null roof!");
            }

            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    if (engineHandle.roofTiles[tileX][tileY] >= EngineHandle.RoofElevationFlag)
                    {
                        engineHandle.roofTiles[tileX][tileY] -= EngineHandle.RoofElevationFlag;
                    }
                }
            }
        }
    }
}
