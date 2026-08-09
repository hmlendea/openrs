using System;

namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofBuilder
    {
        private readonly EngineHandle engineHandle;

        private readonly RoofGeometryBuilder roofGeometryBuilder;

        private readonly RoofTileContextFactory roofTileContextFactory;

        private static int DiagonalWallMaxIndex => 24000;

        private static int SectionAreaSize => 1536;

        private static int AreaChunkCellCount => 8;

        private static int RoofChunkId => 169;

        private static int RoofShadingAmbient => 50;

        private static int RoofShadingDiffuse => 50;

        private static int LightDirectionX => -50;

        private static int LightDirectionY => -10;

        private static int LightDirectionZ => -50;

        private static int TriangleVertexCount => 3;

        private static int QuadVertexCount => 4;

        internal RoofBuilder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
            roofTileContextFactory = new RoofTileContextFactory(engineHandle);
            roofGeometryBuilder = new RoofGeometryBuilder(engineHandle, roofTileContextFactory);
        }

        internal void BuildRoof(int height)
        {
            AssignRoofTiles();
            CalculateRoofHeights();
            roofGeometryBuilder.BuildRoofGeometry();
            FinaliseRoofSection(height);
        }

        private void AssignRoofTiles()
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    AssignRoofTile(tileX, tileY);
                }
            }
        }

        private void AssignRoofTile(int tileX, int tileY)
        {
            AssignHorizontalRoofTile(tileX, tileY);
            AssignVerticalRoofTile(tileX, tileY);
            AssignDiagonalRoofTile(tileX, tileY);
        }

        private void AssignHorizontalRoofTile(int tileX, int tileY)
        {
            int wallType = engineHandle.GetHorizontalWall(tileX, tileY);

            if (wallType <= 0)
            {
                return;
            }

            engineHandle.SetRoofTile(wallType - 1, tileX, tileY, tileX + 1, tileY);
        }

        private void AssignVerticalRoofTile(int tileX, int tileY)
        {
            int wallType = engineHandle.GetVerticalWall(tileX, tileY);

            if (wallType <= 0)
            {
                return;
            }

            engineHandle.SetRoofTile(wallType - 1, tileX, tileY, tileX, tileY + 1);
        }

        private void AssignDiagonalRoofTile(int tileX, int tileY)
        {
            int wallType = engineHandle.GetDiagonalWall(tileX, tileY);
            AssignForwardDiagonalRoofTile(tileX, tileY, wallType);
            AssignBackDiagonalRoofTile(tileX, tileY, wallType);
        }

        private void AssignForwardDiagonalRoofTile(int tileX, int tileY, int wallType)
        {
            if (wallType <= 0 || wallType >= EngineHandle.DiagonalWallBackOffset)
            {
                return;
            }

            engineHandle.SetRoofTile(wallType - 1, tileX, tileY, tileX + 1, tileY + 1);
        }

        private void AssignBackDiagonalRoofTile(int tileX, int tileY, int wallType)
        {
            if (wallType <= EngineHandle.DiagonalWallBackOffset ||
                wallType >= DiagonalWallMaxIndex)
            {
                return;
            }

            engineHandle.SetRoofTile(
                wallType - EngineHandle.DiagonalWallBackOffset - 1,
                tileX + 1,
                tileY,
                tileX,
                tileY + 1);
        }

        private void CalculateRoofHeights()
        {
            for (int tileX = 1; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 1; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    CalculateRoofTileHeight(tileX, tileY);
                }
            }
        }

        private void CalculateRoofTileHeight(int tileX, int tileY)
        {
            if (engineHandle.GetTileRoofType(tileX, tileY) <= 0)
            {
                return;
            }

            RoofCornerCoordinates cornerCoordinates =
                roofTileContextFactory.CreateCornerCoordinates(tileX, tileY);
            int maximumRoofHeight = GetMaximumRoofHeight(cornerCoordinates);
            ApplyRoofHeight(cornerCoordinates, maximumRoofHeight);
        }

        private int GetMaximumRoofHeight(RoofCornerCoordinates cornerCoordinates)
        {
            RoofCornerElevations cornerElevations =
                roofTileContextFactory.LoadCornerElevations(cornerCoordinates);
            int maximumRoofHeight =
                ReduceRoofHeightCalculationElevation(cornerElevations.TopLeft);
            maximumRoofHeight = Math.Max(
                maximumRoofHeight,
                ReduceRoofHeightCalculationElevation(cornerElevations.TopRight));
            maximumRoofHeight = Math.Max(
                maximumRoofHeight,
                ReduceRoofHeightCalculationElevation(cornerElevations.BottomRight));
            maximumRoofHeight = Math.Max(
                maximumRoofHeight,
                ReduceRoofHeightCalculationElevation(cornerElevations.BottomLeft));

            if (maximumRoofHeight >= EngineHandle.RoofElevationFlag)
            {
                maximumRoofHeight -= EngineHandle.RoofElevationFlag;
            }

            return maximumRoofHeight;
        }

        private int ReduceRoofHeightCalculationElevation(int elevation)
        {
            if (elevation > EngineHandle.RoofElevationFlag)
            {
                return elevation - EngineHandle.RoofElevationFlag;
            }

            return elevation;
        }

        private void ApplyRoofHeight(
            RoofCornerCoordinates cornerCoordinates,
            int maximumRoofHeight)
        {
            ApplyRoofHeightToCorner(
                cornerCoordinates.TopLeftX,
                cornerCoordinates.TopLeftY,
                maximumRoofHeight);
            ApplyRoofHeightToCorner(
                cornerCoordinates.TopRightX,
                cornerCoordinates.TopRightY,
                maximumRoofHeight);
            ApplyRoofHeightToCorner(
                cornerCoordinates.BottomRightX,
                cornerCoordinates.BottomRightY,
                maximumRoofHeight);
            ApplyRoofHeightToCorner(
                cornerCoordinates.BottomLeftX,
                cornerCoordinates.BottomLeftY,
                maximumRoofHeight);
        }

        private void ApplyRoofHeightToCorner(int cornerX, int cornerY, int maximumRoofHeight)
        {
            if (engineHandle.RoofTiles[cornerX][cornerY] < EngineHandle.RoofElevationFlag)
            {
                engineHandle.RoofTiles[cornerX][cornerY] = maximumRoofHeight;

                return;
            }

            engineHandle.RoofTiles[cornerX][cornerY] -= EngineHandle.RoofElevationFlag;
        }

        private void FinaliseRoofSection(int height)
        {
            UpdateRoofShading();
            BuildRoofChunks(height);
            AddRoofChunksToWorld(height);
            EnsureRoofChunkWasInitialised(height);
            ClearRoofElevationFlags();
        }

        private void UpdateRoofShading()
        {
            engineHandle.CurrentSectionObject.UpdateShading(
                true,
                RoofShadingAmbient,
                RoofShadingDiffuse,
                LightDirectionX,
                LightDirectionY,
                LightDirectionZ);
        }

        private void BuildRoofChunks(int height)
        {
            engineHandle.RoofObject[height] = engineHandle.CurrentSectionObject.GetObjectsWithinArea(
                0,
                0,
                SectionAreaSize,
                SectionAreaSize,
                AreaChunkCellCount,
                EngineHandle.ChunkCount,
                RoofChunkId,
                true);
        }

        private void AddRoofChunksToWorld(int height)
        {
            for (int chunkIndex = 0; chunkIndex < EngineHandle.ChunkCount; chunkIndex += 1)
            {
                engineHandle.WorldCamera.AddModel(engineHandle.RoofObject[height][chunkIndex]);
            }
        }

        private void EnsureRoofChunkWasInitialised(int height)
        {
            if (engineHandle.RoofObject[height][0] is not null)
            {
                return;
            }

            throw new InvalidOperationException(
                $"Roof chunk 0 for height {height} was not initialised.");
        }

        private void ClearRoofElevationFlags()
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize; tileY += 1)
                {
                    ClearRoofElevationFlag(tileX, tileY);
                }
            }
        }

        private void ClearRoofElevationFlag(int tileX, int tileY)
        {
            if (engineHandle.RoofTiles[tileX][tileY] < EngineHandle.RoofElevationFlag)
            {
                return;
            }

            engineHandle.RoofTiles[tileX][tileY] -= EngineHandle.RoofElevationFlag;
        }
    }
}
