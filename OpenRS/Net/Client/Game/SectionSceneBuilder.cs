namespace OpenRS.Net.Client.Game
{
    internal sealed class SectionSceneBuilder
    {
        private readonly EngineHandle engineHandle;

        internal SectionSceneBuilder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void BuildSection(int height, bool freshLoad)
        {
            if (freshLoad)
            {
                InitialiseGroundVertices(height);
                BuildGroundTileGeometry(height);
                BuildWaterBorderGeometry(height);
                FinaliseGroundSection();
            }

            BuildWalls(height, freshLoad);
        }

        private void InitialiseGroundVertices(int height)
        {
            engineHandle.gameGraphics.ClearScreen();

            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    engineHandle.tiles[tileX][tileY] = 0;
                }
            }

            GameObject sectionObj = engineHandle.currentSectionObject;
            sectionObj.ResetObjectIndexes();

            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    int elevation = -engineHandle.GetTileElevation(tileX, tileY);

                    if (engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) > 0 &&
                        engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) - 1).Type == 4)
                    {
                        elevation = 0;
                    }

                    if (engineHandle.GetTileGroundOverlayIndex(tileX - 1, tileY, height) > 0 &&
                        engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX - 1, tileY, height) - 1).Type == 4)
                    {
                        elevation = 0;
                    }

                    if (engineHandle.GetTileGroundOverlayIndex(tileX, tileY - 1, height) > 0 &&
                        engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX, tileY - 1, height) - 1).Type == 4)
                    {
                        elevation = 0;
                    }

                    if (engineHandle.GetTileGroundOverlayIndex(tileX - 1, tileY - 1, height) > 0 &&
                        engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX - 1, tileY - 1, height) - 1).Type == 4)
                    {
                        elevation = 0;
                    }

                    int vertexIndex = sectionObj.GetVertexIndex(tileX * EngineHandle.TileWorldSize, elevation, tileY * EngineHandle.TileWorldSize);
                    int vertexColour = (int)(Helper.Random.NextDouble() * 10D) - 5;
                    sectionObj.SetVertexColour(vertexIndex, vertexColour);
                }
            }
        }

        private void BuildGroundTileGeometry(int height)
        {
            GameObject sectionObj = engineHandle.currentSectionObject;

            for (int tileX = 0; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    int textureIndex = engineHandle.GetTileGroundTextureIndex(tileX, tileY);
                    int primaryTexture = engineHandle.groundTexture[textureIndex];
                    int secondaryTexture = primaryTexture;
                    int defaultTexture = primaryTexture;
                    int triangleOrientation = 0;

                    if (height == 1 || height == 2)
                    {
                        primaryTexture = EngineHandle.EmptyTileColour;
                        secondaryTexture = EngineHandle.EmptyTileColour;
                        defaultTexture = EngineHandle.EmptyTileColour;
                    }

                    if (engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) > 0)
                    {
                        int tileIndex = engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height);
                        int tileType = engineHandle.entityManager.GetTile(tileIndex - 1).Type;
                        int elevationMinimum = engineHandle.GetElevationMinimum(tileX, tileY, height);
                        primaryTexture = secondaryTexture = engineHandle.entityManager.GetTile(tileIndex - 1).Colour;

                        if (tileType == 4)
                        {
                            primaryTexture = 1;
                            secondaryTexture = 1;

                            if (tileIndex == 12)
                            {
                                primaryTexture = 31;
                                secondaryTexture = 31;
                            }
                        }

                        if (tileType == 5)
                        {
                            if (engineHandle.GetDiagonalWall(tileX, tileY) > 0 &&
                                engineHandle.GetDiagonalWall(tileX, tileY) < 24000)
                            {
                                if (engineHandle.GetTileGroundOverlayTextureOrDefault(tileX - 1, tileY, height, defaultTexture) != EngineHandle.EmptyTileColour &&
                                    engineHandle.GetTileGroundOverlayTextureOrDefault(tileX, tileY - 1, height, defaultTexture) != EngineHandle.EmptyTileColour)
                                {
                                    primaryTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(tileX - 1, tileY, height, defaultTexture);
                                    triangleOrientation = 0;
                                }
                                else if (engineHandle.GetTileGroundOverlayTextureOrDefault(tileX + 1, tileY, height, defaultTexture) != EngineHandle.EmptyTileColour &&
                                         engineHandle.GetTileGroundOverlayTextureOrDefault(tileX, tileY + 1, height, defaultTexture) != EngineHandle.EmptyTileColour)
                                {
                                    secondaryTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(tileX + 1, tileY, height, defaultTexture);
                                    triangleOrientation = 0;
                                }
                                else if (engineHandle.GetTileGroundOverlayTextureOrDefault(tileX + 1, tileY, height, defaultTexture) != EngineHandle.EmptyTileColour &&
                                         engineHandle.GetTileGroundOverlayTextureOrDefault(tileX, tileY - 1, height, defaultTexture) != EngineHandle.EmptyTileColour)
                                {
                                    secondaryTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(tileX + 1, tileY, height, defaultTexture);
                                    triangleOrientation = 1;
                                }
                                else if (engineHandle.GetTileGroundOverlayTextureOrDefault(tileX - 1, tileY, height, defaultTexture) != EngineHandle.EmptyTileColour &&
                                         engineHandle.GetTileGroundOverlayTextureOrDefault(tileX, tileY + 1, height, defaultTexture) != EngineHandle.EmptyTileColour)
                                {
                                    primaryTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(tileX - 1, tileY, height, defaultTexture);
                                    triangleOrientation = 1;
                                }
                            }
                        }
                        else if (tileType != 2 ||
                                 engineHandle.GetDiagonalWall(tileX, tileY) > 0 &&
                                 engineHandle.GetDiagonalWall(tileX, tileY) < 24000)
                        {
                            if (engineHandle.GetElevationMinimum(tileX - 1, tileY, height) != elevationMinimum &&
                                engineHandle.GetElevationMinimum(tileX, tileY - 1, height) != elevationMinimum)
                            {
                                primaryTexture = defaultTexture;
                                triangleOrientation = 0;
                            }
                            else if (engineHandle.GetElevationMinimum(tileX + 1, tileY, height) != elevationMinimum &&
                                     engineHandle.GetElevationMinimum(tileX, tileY + 1, height) != elevationMinimum)
                            {
                                secondaryTexture = defaultTexture;
                                triangleOrientation = 0;
                            }
                            else if (engineHandle.GetElevationMinimum(tileX + 1, tileY, height) != elevationMinimum &&
                                     engineHandle.GetElevationMinimum(tileX, tileY - 1, height) != elevationMinimum)
                            {
                                secondaryTexture = defaultTexture;
                                triangleOrientation = 1;
                            }
                            else if (engineHandle.GetElevationMinimum(tileX - 1, tileY, height) != elevationMinimum &&
                                     engineHandle.GetElevationMinimum(tileX, tileY + 1, height) != elevationMinimum)
                            {
                                primaryTexture = defaultTexture;
                                triangleOrientation = 1;
                            }
                        }

                        if (engineHandle.entityManager.GetTile(tileIndex - 1).Unknown != 0)
                        {
                            engineHandle.tiles[tileX][tileY] |= 0x40;
                        }

                        if (engineHandle.entityManager.GetTile(tileIndex - 1).Type == 2)
                        {
                            engineHandle.tiles[tileX][tileY] |= 0x80;
                        }
                    }

                    engineHandle.DrawMinimapPixel(tileX, tileY, triangleOrientation, primaryTexture, secondaryTexture);

                    int elevationVariance =
                        engineHandle.GetTileElevation(tileX + 1, tileY + 1) -
                        engineHandle.GetTileElevation(tileX + 1, tileY) +
                        engineHandle.GetTileElevation(tileX, tileY + 1) -
                        engineHandle.GetTileElevation(tileX, tileY);

                    if (primaryTexture != secondaryTexture || elevationVariance != 0)
                    {
                        int[] triangleCoords1 = new int[3];
                        int[] triangleCoords2 = new int[3];

                        if (triangleOrientation == 0)
                        {
                            if (primaryTexture != EngineHandle.EmptyTileColour)
                            {
                                triangleCoords1[0] = tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize;
                                triangleCoords1[1] = tileY + tileX * EngineHandle.GridSize;
                                triangleCoords1[2] = tileY + tileX * EngineHandle.GridSize + 1;
                                int faceIndex = sectionObj.AddFaceVertices(3, triangleCoords1, EngineHandle.EmptyTileColour, primaryTexture);
                                engineHandle.selectedX[faceIndex] = tileX;
                                engineHandle.selectedY[faceIndex] = tileY;
                                sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
                            }

                            if (secondaryTexture != EngineHandle.EmptyTileColour)
                            {
                                triangleCoords2[0] = tileY + tileX * EngineHandle.GridSize + 1;
                                triangleCoords2[1] = tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1;
                                triangleCoords2[2] = tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize;
                                int faceIndex = sectionObj.AddFaceVertices(3, triangleCoords2, EngineHandle.EmptyTileColour, secondaryTexture);
                                engineHandle.selectedX[faceIndex] = tileX;
                                engineHandle.selectedY[faceIndex] = tileY;
                                sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
                            }
                        }
                        else
                        {
                            if (primaryTexture != EngineHandle.EmptyTileColour)
                            {
                                triangleCoords1[0] = tileY + tileX * EngineHandle.GridSize + 1;
                                triangleCoords1[1] = tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1;
                                triangleCoords1[2] = tileY + tileX * EngineHandle.GridSize;
                                int faceIndex = sectionObj.AddFaceVertices(3, triangleCoords1, EngineHandle.EmptyTileColour, primaryTexture);
                                engineHandle.selectedX[faceIndex] = tileX;
                                engineHandle.selectedY[faceIndex] = tileY;
                                sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
                            }

                            if (secondaryTexture != EngineHandle.EmptyTileColour)
                            {
                                triangleCoords2[0] = tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize;
                                triangleCoords2[1] = tileY + tileX * EngineHandle.GridSize;
                                triangleCoords2[2] = tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1;
                                int faceIndex = sectionObj.AddFaceVertices(3, triangleCoords2, EngineHandle.EmptyTileColour, secondaryTexture);
                                engineHandle.selectedX[faceIndex] = tileX;
                                engineHandle.selectedY[faceIndex] = tileY;
                                sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
                            }
                        }
                    }
                    else if (primaryTexture != EngineHandle.EmptyTileColour)
                    {
                        int[] quadCoords =
                        [
                            tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize,
                        tileY + tileX * EngineHandle.GridSize,
                        tileY + tileX * EngineHandle.GridSize + 1,
                        tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1,
                    ];
                        int faceIndex = sectionObj.AddFaceVertices(4, quadCoords, EngineHandle.EmptyTileColour, primaryTexture);
                        engineHandle.selectedX[faceIndex] = tileX;
                        engineHandle.selectedY[faceIndex] = tileY;
                        sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
                    }
                }
            }
        }

        private void BuildWaterBorderGeometry(int height)
        {
            GameObject sectionObj = engineHandle.currentSectionObject;

            for (int tileX = 1; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 1; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    if (engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) > 0 &&
                        engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) - 1).Type == 4)
                    {
                        int tileColour = engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) - 1).Colour;
                        int[] faceVerts =
                        [
                            sectionObj.GetVertexIndex(tileX * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX, tileY), tileY * EngineHandle.TileWorldSize),
                        sectionObj.GetVertexIndex((tileX + 1) * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX + 1, tileY), tileY * EngineHandle.TileWorldSize),
                        sectionObj.GetVertexIndex((tileX + 1) * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX + 1, tileY + 1), (tileY + 1) * EngineHandle.TileWorldSize),
                        sectionObj.GetVertexIndex(tileX * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX, tileY + 1), (tileY + 1) * EngineHandle.TileWorldSize),
                    ];
                        int faceIndex = sectionObj.AddFaceVertices(4, faceVerts, tileColour, EngineHandle.EmptyTileColour);
                        engineHandle.selectedX[faceIndex] = tileX;
                        engineHandle.selectedY[faceIndex] = tileY;
                        sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
                        engineHandle.DrawMinimapPixel(tileX, tileY, 0, tileColour, tileColour);
                    }
                    else if (engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) == 0 ||
                             engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height) - 1).Type != 3)
                    {
                        AddAdjacentWaterBorderFace(height, tileX, tileY, tileX, tileY + 1, sectionObj);
                        AddAdjacentWaterBorderFace(height, tileX, tileY, tileX, tileY - 1, sectionObj);
                        AddAdjacentWaterBorderFace(height, tileX, tileY, tileX + 1, tileY, sectionObj);
                        AddAdjacentWaterBorderFace(height, tileX, tileY, tileX - 1, tileY, sectionObj);
                    }
                }
            }
        }

        private void AddAdjacentWaterBorderFace(int height, int tileX, int tileY, int adjacentX, int adjacentY, GameObject sectionObj)
        {
            if (engineHandle.GetTileGroundOverlayIndex(adjacentX, adjacentY, height) <= 0 ||
                engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(adjacentX, adjacentY, height) - 1).Type != 4)
            {
                return;
            }

            int tileColour = engineHandle.entityManager.GetTile(engineHandle.GetTileGroundOverlayIndex(adjacentX, adjacentY, height) - 1).Colour;
            int[] faceVerts =
            [
                sectionObj.GetVertexIndex(tileX * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX, tileY), tileY * EngineHandle.TileWorldSize),
            sectionObj.GetVertexIndex((tileX + 1) * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX + 1, tileY), tileY * EngineHandle.TileWorldSize),
            sectionObj.GetVertexIndex((tileX + 1) * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX + 1, tileY + 1), (tileY + 1) * EngineHandle.TileWorldSize),
            sectionObj.GetVertexIndex(tileX * EngineHandle.TileWorldSize, -engineHandle.GetTileElevation(tileX, tileY + 1), (tileY + 1) * EngineHandle.TileWorldSize),
        ];
            int faceIndex = sectionObj.AddFaceVertices(4, faceVerts, tileColour, EngineHandle.EmptyTileColour);
            engineHandle.selectedX[faceIndex] = tileX;
            engineHandle.selectedY[faceIndex] = tileY;
            sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
            engineHandle.DrawMinimapPixel(tileX, tileY, 0, tileColour, tileColour);
        }

        private void FinaliseGroundSection()
        {
            engineHandle.currentSectionObject.UpdateShading(true, 40, 48, -50, -10, -50);
            engineHandle.TileChunks = engineHandle.currentSectionObject.GetObjectsWithinArea(0, 0, 1536, 1536, 8, EngineHandle.ChunkCount, 233, false);

            for (int chunkIndex = 0; chunkIndex < EngineHandle.ChunkCount; chunkIndex += 1)
            {
                engineHandle.camera.AddModel(engineHandle.TileChunks[chunkIndex]);
            }

            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    engineHandle.roofTiles[tileX][tileY] = engineHandle.GetTileElevation(tileX, tileY);
                }
            }
        }

        private void BuildWalls(int height, bool freshLoad)
        {
            engineHandle.currentSectionObject.ResetObjectIndexes();
            int wallMinimapColour = 0x606060;

            for (int tileX = 0; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    int wallTypeIndex = engineHandle.GetHorizontalWall(tileX, tileY);

                    if (wallTypeIndex > 0 && (engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).FaceRenderMode == 0 || engineHandle.showAllWalls))
                    {
                        engineHandle.MakeWall(engineHandle.currentSectionObject, wallTypeIndex - 1, tileX, tileY, tileX + 1, tileY);

                        if (freshLoad && engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).Type != 0)
                        {
                            engineHandle.tiles[tileX][tileY] |= 1;

                            if (tileY > 0)
                            {
                                engineHandle.SetTileFlags(tileX, tileY - 1, 4);
                            }
                        }

                        if (freshLoad)
                        {
                            engineHandle.gameGraphics.DrawLineX(tileX * 3, tileY * 3, 3, wallMinimapColour);
                        }
                    }

                    wallTypeIndex = engineHandle.GetVerticalWall(tileX, tileY);

                    if (wallTypeIndex > 0 && (engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).FaceRenderMode == 0 || engineHandle.showAllWalls))
                    {
                        engineHandle.MakeWall(engineHandle.currentSectionObject, wallTypeIndex - 1, tileX, tileY, tileX, tileY + 1);

                        if (freshLoad && engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).Type != 0)
                        {
                            engineHandle.tiles[tileX][tileY] |= 2;

                            if (tileX > 0)
                            {
                                engineHandle.SetTileFlags(tileX - 1, tileY, 8);
                            }
                        }

                        if (freshLoad)
                        {
                            engineHandle.gameGraphics.DrawLineY(tileX * 3, tileY * 3, 3, wallMinimapColour);
                        }
                    }

                    wallTypeIndex = engineHandle.GetDiagonalWall(tileX, tileY);

                    if (wallTypeIndex > 0 &&
                        wallTypeIndex < EngineHandle.DiagonalWallBackOffset &&
                        (engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).FaceRenderMode == 0 || engineHandle.showAllWalls))
                    {
                        engineHandle.MakeWall(engineHandle.currentSectionObject, wallTypeIndex - 1, tileX, tileY, tileX + 1, tileY + 1);

                        if (freshLoad && engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).Type != 0)
                        {
                            engineHandle.tiles[tileX][tileY] |= 0x20;
                        }

                        if (freshLoad)
                        {
                            engineHandle.gameGraphics.DrawMinimapPixel(tileX * 3, tileY * 3, wallMinimapColour);
                            engineHandle.gameGraphics.DrawMinimapPixel(tileX * 3 + 1, tileY * 3 + 1, wallMinimapColour);
                            engineHandle.gameGraphics.DrawMinimapPixel(tileX * 3 + 2, tileY * 3 + 2, wallMinimapColour);
                        }
                    }

                    if (wallTypeIndex > EngineHandle.DiagonalWallBackOffset &&
                        wallTypeIndex < 24000 &&
                        (engineHandle.entityManager.GetWallObject(wallTypeIndex - EngineHandle.DiagonalWallBackOffset - 1).FaceRenderMode == 0 || engineHandle.showAllWalls))
                    {
                        engineHandle.MakeWall(engineHandle.currentSectionObject, wallTypeIndex - EngineHandle.DiagonalWallBackOffset - 1, tileX + 1, tileY, tileX, tileY + 1);

                        if (freshLoad && engineHandle.entityManager.GetWallObject(wallTypeIndex - EngineHandle.DiagonalWallBackOffset - 1).Type != 0)
                        {
                            engineHandle.tiles[tileX][tileY] |= 0x10;
                        }

                        if (freshLoad)
                        {
                            engineHandle.gameGraphics.DrawMinimapPixel(tileX * 3 + 2, tileY * 3, wallMinimapColour);
                            engineHandle.gameGraphics.DrawMinimapPixel(tileX * 3 + 1, tileY * 3 + 1, wallMinimapColour);
                            engineHandle.gameGraphics.DrawMinimapPixel(tileX * 3, tileY * 3 + 2, wallMinimapColour);
                        }
                    }
                }
            }

            if (freshLoad)
            {
                engineHandle.gameGraphics.FillPicture(engineHandle.baseInventoryPic - 1, 0, 0, 285, 285);
            }

            engineHandle.currentSectionObject.UpdateShading(false, 60, 24, -50, -10, -50);
            engineHandle.wallObject[height] = engineHandle.currentSectionObject.GetObjectsWithinArea(0, 0, 1536, 1536, 8, EngineHandle.ChunkCount, 338, true);

            for (int chunkIndex = 0; chunkIndex < EngineHandle.ChunkCount; chunkIndex += 1)
            {
                engineHandle.camera.AddModel(engineHandle.wallObject[height][chunkIndex]);
            }
        }
    }
}
