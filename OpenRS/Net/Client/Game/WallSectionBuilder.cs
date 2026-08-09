namespace OpenRS.Net.Client.Game
{
    internal sealed class WallSectionBuilder
    {
        private readonly EngineHandle engineHandle;

        private static int DiagonalWallMaxIndex => 24000;

        private static int WallTypeNone => 0;

        private static int HorizontalWallTileFlag => 1;

        private static int VerticalWallTileFlag => 2;

        private static int DiagonalForwardWallTileFlag => 0x20;

        private static int DiagonalBackWallTileFlag => 0x10;

        private static int NorthNeighbourWallTileFlag => 4;

        private static int WestNeighbourWallTileFlag => 8;

        private static int WallMinimapColour => 0x606060;

        private static int MinimapPixelsPerTile => 3;

        private static int InventoryPicSize => 285;

        private static int SectionAreaSize => 1536;

        private static int WallChunkId => 338;

        private static int AreaChunkCellCount => 8;

        private static int WallShadingAmbient => 60;

        private static int WallShadingDiffuse => 24;

        private static int LightDirectionX => -50;

        private static int LightDirectionY => -10;

        private static int LightDirectionZ => -50;

        internal WallSectionBuilder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void BuildWalls(int height, bool freshLoad)
        {
            engineHandle.CurrentSectionObject.ResetObjectIndexes();

            for (int tileX = 0; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    BuildWallTile(tileX, tileY, freshLoad);
                }
            }

            if (freshLoad)
            {
                engineHandle.GameGraphics.FillPicture(
                    engineHandle.BaseInventoryPic - 1,
                    0,
                    0,
                    InventoryPicSize,
                    InventoryPicSize);
            }

            FinaliseWallSection(height);
        }

        private void BuildWallTile(int tileX, int tileY, bool freshLoad)
        {
            BuildHorizontalWall(tileX, tileY, freshLoad);
            BuildVerticalWall(tileX, tileY, freshLoad);
            BuildDiagonalWalls(tileX, tileY, freshLoad);
        }

        private void BuildHorizontalWall(int tileX, int tileY, bool freshLoad)
        {
            int wallTypeIndex = engineHandle.GetHorizontalWall(tileX, tileY);

            if (wallTypeIndex <= 0 || !IsWallObjectVisible(wallTypeIndex - 1))
            {
                return;
            }

            engineHandle.MakeWall(
                engineHandle.CurrentSectionObject, wallTypeIndex - 1, tileX, tileY, tileX + 1, tileY);

            if (freshLoad)
            {
                ApplyHorizontalWallTileFlags(tileX, tileY, wallTypeIndex);
                engineHandle.GameGraphics.DrawLineX(
                    tileX * MinimapPixelsPerTile,
                    tileY * MinimapPixelsPerTile,
                    MinimapPixelsPerTile,
                    WallMinimapColour);
            }
        }

        private void ApplyHorizontalWallTileFlags(int tileX, int tileY, int wallTypeIndex)
        {
            if (engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).Type == WallTypeNone)
            {
                return;
            }

            engineHandle.Tiles[tileX][tileY] |= HorizontalWallTileFlag;

            if (tileY > 0)
            {
                engineHandle.SetTileFlags(tileX, tileY - 1, NorthNeighbourWallTileFlag);
            }
        }

        private void BuildVerticalWall(int tileX, int tileY, bool freshLoad)
        {
            int wallTypeIndex = engineHandle.GetVerticalWall(tileX, tileY);

            if (wallTypeIndex <= 0 || !IsWallObjectVisible(wallTypeIndex - 1))
            {
                return;
            }

            engineHandle.MakeWall(
                engineHandle.CurrentSectionObject, wallTypeIndex - 1, tileX, tileY, tileX, tileY + 1);

            if (freshLoad)
            {
                ApplyVerticalWallTileFlags(tileX, tileY, wallTypeIndex);
                engineHandle.GameGraphics.DrawLineY(
                    tileX * MinimapPixelsPerTile,
                    tileY * MinimapPixelsPerTile,
                    MinimapPixelsPerTile,
                    WallMinimapColour);
            }
        }

        private void ApplyVerticalWallTileFlags(int tileX, int tileY, int wallTypeIndex)
        {
            if (engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).Type == WallTypeNone)
            {
                return;
            }

            engineHandle.Tiles[tileX][tileY] |= VerticalWallTileFlag;

            if (tileX > 0)
            {
                engineHandle.SetTileFlags(tileX - 1, tileY, WestNeighbourWallTileFlag);
            }
        }

        private void BuildDiagonalWalls(int tileX, int tileY, bool freshLoad)
        {
            int wallTypeIndex = engineHandle.GetDiagonalWall(tileX, tileY);
            BuildForwardDiagonalWall(tileX, tileY, wallTypeIndex, freshLoad);
            BuildBackDiagonalWall(tileX, tileY, wallTypeIndex, freshLoad);
        }

        private void BuildForwardDiagonalWall(int tileX, int tileY, int wallTypeIndex, bool freshLoad)
        {
            if (wallTypeIndex <= 0 ||
                wallTypeIndex >= EngineHandle.DiagonalWallBackOffset ||
                !IsWallObjectVisible(wallTypeIndex - 1))
            {
                return;
            }

            engineHandle.MakeWall(
                engineHandle.CurrentSectionObject, wallTypeIndex - 1, tileX, tileY, tileX + 1, tileY + 1);

            if (freshLoad)
            {
                ApplyForwardDiagonalWallTileFlags(tileX, tileY, wallTypeIndex);
                DrawForwardDiagonalWallMinimap(tileX, tileY);
            }
        }

        private void ApplyForwardDiagonalWallTileFlags(int tileX, int tileY, int wallTypeIndex)
        {
            if (engineHandle.entityManager.GetWallObject(wallTypeIndex - 1).Type != WallTypeNone)
            {
                engineHandle.Tiles[tileX][tileY] |= DiagonalForwardWallTileFlag;
            }
        }

        private void DrawForwardDiagonalWallMinimap(int tileX, int tileY)
        {
            for (int pixel = 0; pixel < MinimapPixelsPerTile; pixel += 1)
            {
                engineHandle.GameGraphics.DrawMinimapPixel(
                    tileX * MinimapPixelsPerTile + pixel,
                    tileY * MinimapPixelsPerTile + pixel,
                    WallMinimapColour);
            }
        }

        private void BuildBackDiagonalWall(int tileX, int tileY, int wallTypeIndex, bool freshLoad)
        {
            if (wallTypeIndex <= EngineHandle.DiagonalWallBackOffset ||
                wallTypeIndex >= DiagonalWallMaxIndex ||
                !IsWallObjectVisible(wallTypeIndex - EngineHandle.DiagonalWallBackOffset - 1))
            {
                return;
            }

            int adjustedIndex = wallTypeIndex - EngineHandle.DiagonalWallBackOffset - 1;
            engineHandle.MakeWall(
                engineHandle.CurrentSectionObject, adjustedIndex, tileX + 1, tileY, tileX, tileY + 1);

            if (freshLoad)
            {
                ApplyBackDiagonalWallTileFlags(tileX, tileY, adjustedIndex);
                DrawBackDiagonalWallMinimap(tileX, tileY);
            }
        }

        private void ApplyBackDiagonalWallTileFlags(int tileX, int tileY, int adjustedIndex)
        {
            if (engineHandle.entityManager.GetWallObject(adjustedIndex).Type != WallTypeNone)
            {
                engineHandle.Tiles[tileX][tileY] |= DiagonalBackWallTileFlag;
            }
        }

        private void DrawBackDiagonalWallMinimap(int tileX, int tileY)
        {
            for (int pixel = 0; pixel < MinimapPixelsPerTile; pixel += 1)
            {
                engineHandle.GameGraphics.DrawMinimapPixel(
                    tileX * MinimapPixelsPerTile + (MinimapPixelsPerTile - 1 - pixel),
                    tileY * MinimapPixelsPerTile + pixel,
                    WallMinimapColour);
            }
        }

        private bool IsWallObjectVisible(int wallObjectIndex) =>
            engineHandle.entityManager.GetWallObject(wallObjectIndex).FaceRenderMode == 0 ||
            engineHandle.ShowAllWalls;

        private void FinaliseWallSection(int height)
        {
            engineHandle.CurrentSectionObject.UpdateShading(
                false,
                WallShadingAmbient,
                WallShadingDiffuse,
                LightDirectionX,
                LightDirectionY,
                LightDirectionZ);

            engineHandle.WallObject[height] = engineHandle.CurrentSectionObject.GetObjectsWithinArea(
                0,
                0,
                SectionAreaSize,
                SectionAreaSize,
                AreaChunkCellCount,
                EngineHandle.ChunkCount,
                WallChunkId,
                true);

            for (int chunkIndex = 0; chunkIndex < EngineHandle.ChunkCount; chunkIndex += 1)
            {
                engineHandle.WorldCamera.AddModel(engineHandle.WallObject[height][chunkIndex]);
            }
        }
    }
}
