namespace OpenRS.Net.Client.Game
{
    internal sealed class GroundSectionBuilder
    {
        private readonly EngineHandle engineHandle;

        private readonly GroundTileTextureResolver groundTileTextureResolver;


        private static int GroundVertexColourRange => 10;

        private static int GroundVertexColourOffset => 5;

        private static int SectionAreaSize => 1536;

        private static int GroundChunkId => 233;

        private static int AreaChunkCellCount => 8;

        private static int DefaultTriangleOrientation => 0;

        private static int TriangleVertexCount => 3;

        private static int QuadVertexCount => 4;

        private static int GroundShadingAmbient => 40;

        private static int GroundShadingDiffuse => 48;

        private static int LightDirectionX => -50;

        private static int LightDirectionY => -10;

        private static int LightDirectionZ => -50;

        internal GroundSectionBuilder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
            groundTileTextureResolver = new GroundTileTextureResolver(engineHandle);
        }

        internal void InitialiseGroundVertices(int height)
        {
            engineHandle.GameGraphics.ClearScreen();
            ResetTileGrid();

            GameObject sectionObj = engineHandle.CurrentSectionObject;
            sectionObj.ResetObjectIndexes();

            SetGroundVertexColours(height, sectionObj);
        }

        internal void BuildGroundTileGeometry(int height)
        {
            GameObject sectionObj = engineHandle.CurrentSectionObject;

            for (int tileX = 0; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    BuildGroundTile(tileX, tileY, height, sectionObj);
                }
            }
        }

        internal void BuildWaterBorderGeometry(int height)
        {
            GameObject sectionObj = engineHandle.CurrentSectionObject;

            for (int tileX = 1; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 1; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    BuildWaterBorderTile(tileX, tileY, height, sectionObj);
                }
            }
        }

        internal void FinaliseGroundSection()
        {
            engineHandle.CurrentSectionObject.UpdateShading(
                true,
                GroundShadingAmbient,
                GroundShadingDiffuse,
                LightDirectionX,
                LightDirectionY,
                LightDirectionZ);

            engineHandle.TileChunks = engineHandle.CurrentSectionObject.GetObjectsWithinArea(
                0,
                0,
                SectionAreaSize,
                SectionAreaSize,
                AreaChunkCellCount,
                EngineHandle.ChunkCount,
                GroundChunkId,
                false);

            for (int chunkIndex = 0; chunkIndex < EngineHandle.ChunkCount; chunkIndex += 1)
            {
                engineHandle.WorldCamera.AddModel(engineHandle.TileChunks[chunkIndex]);
            }

            PopulateRoofTileElevations();
        }

        private void ResetTileGrid()
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    engineHandle.Tiles[tileX][tileY] = 0;
                }
            }
        }

        private void SetGroundVertexColours(int height, GameObject sectionObj)
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    int elevation = CalculateVertexElevation(tileX, tileY, height);
                    int vertexIndex = sectionObj.GetVertexIndex(
                        tileX * EngineHandle.TileWorldSize,
                        elevation,
                        tileY * EngineHandle.TileWorldSize);
                    int vertexColour =
                        (int)(Helper.Random.NextDouble() * GroundVertexColourRange) -
                        GroundVertexColourOffset;
                    sectionObj.SetVertexColour(vertexIndex, vertexColour);
                }
            }
        }

        private int CalculateVertexElevation(int tileX, int tileY, int height)
        {
            int elevation = -engineHandle.GetTileElevation(tileX, tileY);

            if (groundTileTextureResolver.IsWaterOverlay(tileX, tileY, height) ||
                groundTileTextureResolver.IsWaterOverlay(tileX - 1, tileY, height) ||
                groundTileTextureResolver.IsWaterOverlay(tileX, tileY - 1, height) ||
                groundTileTextureResolver.IsWaterOverlay(tileX - 1, tileY - 1, height))
            {
                elevation = 0;
            }

            return elevation;
        }

        private void BuildGroundTile(int tileX, int tileY, int height, GameObject sectionObj)
        {
            TileTextureResult groundTextureResult =
                groundTileTextureResolver.InitialiseGroundTextureResult(
                    tileX,
                    tileY,
                    height);
            int defaultTexture = groundTextureResult.PrimaryTexture;
            int primaryTexture = groundTextureResult.PrimaryTexture;
            int secondaryTexture = groundTextureResult.SecondaryTexture;
            int triangleOrientation = groundTextureResult.TriangleOrientation;

            int overlayIndex = engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height);

            if (overlayIndex > 0)
            {
                TileTextureResult overlayResult =
                    groundTileTextureResolver.ResolveOverlayTextures(
                        tileX,
                        tileY,
                        height,
                        defaultTexture);
                primaryTexture = overlayResult.PrimaryTexture;
                secondaryTexture = overlayResult.SecondaryTexture;
                triangleOrientation = overlayResult.TriangleOrientation;
                groundTileTextureResolver.ApplyOverlayTileFlags(tileX, tileY, overlayIndex);
            }

            engineHandle.DrawMinimapPixel(
                tileX,
                tileY,
                triangleOrientation,
                primaryTexture,
                secondaryTexture);

            int elevationVariance = CalculateElevationVariance(tileX, tileY);
            AddTileFaceGeometry(
                tileX,
                tileY,
                primaryTexture,
                secondaryTexture,
                triangleOrientation,
                elevationVariance,
                sectionObj);
        }

        private int CalculateElevationVariance(int tileX, int tileY) =>
            engineHandle.GetTileElevation(tileX + 1, tileY + 1) -
            engineHandle.GetTileElevation(tileX + 1, tileY) +
            engineHandle.GetTileElevation(tileX, tileY + 1) -
            engineHandle.GetTileElevation(tileX, tileY);

        private void AddTileFaceGeometry(
            int tileX,
            int tileY,
            int primaryTexture,
            int secondaryTexture,
            int triangleOrientation,
            int elevationVariance,
            GameObject sectionObj)
        {
            if (primaryTexture != secondaryTexture || elevationVariance != 0)
            {
                AddTriangleFaceGeometry(
                    tileX, tileY, primaryTexture, secondaryTexture, triangleOrientation, sectionObj);

                return;
            }

            if (primaryTexture != EngineHandle.EmptyTileColour)
            {
                AddQuadFaceGeometry(tileX, tileY, primaryTexture, sectionObj);
            }
        }

        private void AddTriangleFaceGeometry(
            int tileX,
            int tileY,
            int primaryTexture,
            int secondaryTexture,
            int triangleOrientation,
            GameObject sectionObj)
        {
            if (triangleOrientation == DefaultTriangleOrientation)
            {
                AddOrientationZeroTriangles(
                    tileX,
                    tileY,
                    primaryTexture,
                    secondaryTexture,
                    sectionObj);

                return;
            }

            AddOrientationOneTriangles(tileX, tileY, primaryTexture, secondaryTexture, sectionObj);
        }

        private void AddOrientationZeroTriangles(
            int tileX,
            int tileY,
            int primaryTexture,
            int secondaryTexture,
            GameObject sectionObj)
        {
            if (primaryTexture != EngineHandle.EmptyTileColour)
            {
                int[] coords =
                [
                    tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize,
                    tileY + tileX * EngineHandle.GridSize,
                    tileY + tileX * EngineHandle.GridSize + 1,
                ];
                int faceIndex = sectionObj.AddFaceVertices(
                    TriangleVertexCount,
                    coords,
                    EngineHandle.EmptyTileColour,
                    primaryTexture);
                RegisterTileFace(tileX, tileY, faceIndex, sectionObj);
            }

            if (secondaryTexture != EngineHandle.EmptyTileColour)
            {
                int[] coords =
                [
                    tileY + tileX * EngineHandle.GridSize + 1,
                    tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1,
                    tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize,
                ];
                int faceIndex = sectionObj.AddFaceVertices(
                    TriangleVertexCount,
                    coords,
                    EngineHandle.EmptyTileColour,
                    secondaryTexture);
                RegisterTileFace(tileX, tileY, faceIndex, sectionObj);
            }
        }

        private void AddOrientationOneTriangles(
            int tileX,
            int tileY,
            int primaryTexture,
            int secondaryTexture,
            GameObject sectionObj)
        {
            if (primaryTexture != EngineHandle.EmptyTileColour)
            {
                int[] coords =
                [
                    tileY + tileX * EngineHandle.GridSize + 1,
                    tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1,
                    tileY + tileX * EngineHandle.GridSize,
                ];
                int faceIndex = sectionObj.AddFaceVertices(
                    TriangleVertexCount,
                    coords,
                    EngineHandle.EmptyTileColour,
                    primaryTexture);
                RegisterTileFace(tileX, tileY, faceIndex, sectionObj);
            }

            if (secondaryTexture != EngineHandle.EmptyTileColour)
            {
                int[] coords =
                [
                    tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize,
                    tileY + tileX * EngineHandle.GridSize,
                    tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1,
                ];
                int faceIndex = sectionObj.AddFaceVertices(
                    TriangleVertexCount,
                    coords,
                    EngineHandle.EmptyTileColour,
                    secondaryTexture);
                RegisterTileFace(tileX, tileY, faceIndex, sectionObj);
            }
        }

        private void AddQuadFaceGeometry(int tileX, int tileY, int texture, GameObject sectionObj)
        {
            int[] coords =
            [
                tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize,
                tileY + tileX * EngineHandle.GridSize,
                tileY + tileX * EngineHandle.GridSize + 1,
                tileY + tileX * EngineHandle.GridSize + EngineHandle.GridSize + 1,
            ];
            int faceIndex = sectionObj.AddFaceVertices(
                QuadVertexCount,
                coords,
                EngineHandle.EmptyTileColour,
                texture);
            RegisterTileFace(tileX, tileY, faceIndex, sectionObj);
        }

        private void RegisterTileFace(int tileX, int tileY, int faceIndex, GameObject sectionObj)
        {
            engineHandle.SelectedX[faceIndex] = tileX;
            engineHandle.SelectedY[faceIndex] = tileY;
            sectionObj.EntityType[faceIndex] = EngineHandle.TileEntityTypeBase + faceIndex;
        }

        private void BuildWaterBorderTile(int tileX, int tileY, int height, GameObject sectionObj)
        {
            int overlayIndex = engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height);

            if (overlayIndex > 0 &&
                groundTileTextureResolver.IsWaterOverlay(overlayIndex))
            {
                int tileColour = groundTileTextureResolver.GetOverlayTileColour(overlayIndex);
                AddWaterFace(tileX, tileY, tileColour, sectionObj);
                engineHandle.DrawMinimapPixel(
                    tileX,
                    tileY,
                    DefaultTriangleOrientation,
                    tileColour,
                    tileColour);

                return;
            }

            bool isRoofTile =
                overlayIndex > 0 &&
                groundTileTextureResolver.IsRoofOverlay(overlayIndex);

            if (!isRoofTile)
            {
                AddAdjacentWaterBorderFace(height, tileX, tileY, tileX, tileY + 1, sectionObj);
                AddAdjacentWaterBorderFace(height, tileX, tileY, tileX, tileY - 1, sectionObj);
                AddAdjacentWaterBorderFace(height, tileX, tileY, tileX + 1, tileY, sectionObj);
                AddAdjacentWaterBorderFace(height, tileX, tileY, tileX - 1, tileY, sectionObj);
            }
        }

        private void AddWaterFace(int tileX, int tileY, int tileColour, GameObject sectionObj)
        {
            int[] faceVerts =
            [
                sectionObj.GetVertexIndex(
                    tileX * EngineHandle.TileWorldSize,
                    -engineHandle.GetTileElevation(tileX, tileY),
                    tileY * EngineHandle.TileWorldSize),
                sectionObj.GetVertexIndex(
                    (tileX + 1) * EngineHandle.TileWorldSize,
                    -engineHandle.GetTileElevation(tileX + 1, tileY),
                    tileY * EngineHandle.TileWorldSize),
                sectionObj.GetVertexIndex(
                    (tileX + 1) * EngineHandle.TileWorldSize,
                    -engineHandle.GetTileElevation(tileX + 1, tileY + 1),
                    (tileY + 1) * EngineHandle.TileWorldSize),
                sectionObj.GetVertexIndex(
                    tileX * EngineHandle.TileWorldSize,
                    -engineHandle.GetTileElevation(tileX, tileY + 1),
                    (tileY + 1) * EngineHandle.TileWorldSize),
            ];
            int faceIndex = sectionObj.AddFaceVertices(
                QuadVertexCount,
                faceVerts,
                tileColour,
                EngineHandle.EmptyTileColour);
            RegisterTileFace(tileX, tileY, faceIndex, sectionObj);
        }

        private void AddAdjacentWaterBorderFace(
            int height,
            int tileX,
            int tileY,
            int adjacentX,
            int adjacentY,
            GameObject sectionObj)
        {
            int adjacentOverlayIndex = engineHandle.GetTileGroundOverlayIndex(
                adjacentX,
                adjacentY,
                height);

            if (adjacentOverlayIndex <= 0 ||
                !groundTileTextureResolver.IsWaterOverlay(adjacentOverlayIndex))
            {
                return;
            }

            int tileColour = groundTileTextureResolver.GetOverlayTileColour(
                adjacentOverlayIndex);
            AddWaterFace(tileX, tileY, tileColour, sectionObj);
            engineHandle.DrawMinimapPixel(
                tileX,
                tileY,
                DefaultTriangleOrientation,
                tileColour,
                tileColour);
        }

        private void PopulateRoofTileElevations()
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    engineHandle.RoofTiles[tileX][tileY] = engineHandle.GetTileElevation(tileX, tileY);
                }
            }
        }
    }
}
