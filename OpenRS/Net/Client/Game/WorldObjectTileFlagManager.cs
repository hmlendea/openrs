using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    internal sealed class WorldObjectTileFlagManager
    {
        private static readonly int MinimumSceneCoordinate = 1;
        private static readonly int SolidWallType = 1;
        private static readonly int SolidWorldObjectType = 1;
        private static readonly int DirectionalWorldObjectType = 2;
        private static readonly int RotationZeroDegrees = 0;
        private static readonly int RotationNinetyDegrees = 2;
        private static readonly int RotationOneHundredEightyDegrees = 4;
        private static readonly int RotationTwoHundredSeventyDegrees = 6;
        private static readonly int WallRotationZeroDegrees = 0;
        private static readonly int WallRotationNinetyDegrees = 1;
        private static readonly int WallRotationOneHundredEightyDegrees = 2;
        private static readonly int WallRotationTwoHundredSeventyDegrees = 3;
        private static readonly int TileFlagNorthBoundary = 1;
        private static readonly int TileFlagWestBoundary = 2;
        private static readonly int TileFlagSouthBoundary = 4;
        private static readonly int TileFlagEastBoundary = 8;
        private static readonly int TileFlagDiagonalSouthEastBoundary = 0x10;
        private static readonly int TileFlagDiagonalSouthWestBoundary = 0x20;
        private static readonly int TileFlagSolidObject = 0x40;
        private static readonly int SceneMaskCurrentTile = 0x63;
        private static readonly int SceneMaskWestTile = 0x59;
        private static readonly int SceneMaskNorthTile = 0x56;
        private static readonly int SceneMaskNorthWestTile = 0x6c;
        private static readonly int OccupiedSceneFlag = 35;
        private static readonly int EmptySceneFlag = 0;

        private readonly EngineHandle engineHandle;

        internal WorldObjectTileFlagManager(EngineHandle engineHandle)
            => this.engineHandle = engineHandle;

        internal void AddObjectToScene(int tileX, int tileY, int objectWidth, int objectHeight)
        {
            if (!IsSceneAreaWithinBounds(tileX, tileY, objectWidth, objectHeight))
            {
                return;
            }

            for (int occupiedTileX = tileX;
                occupiedTileX <= tileX + objectWidth;
                occupiedTileX += 1)
            {
                for (int occupiedTileY = tileY;
                    occupiedTileY <= tileY + objectHeight;
                    occupiedTileY += 1)
                {
                    UpdateSceneTile(occupiedTileX, occupiedTileY);
                }
            }
        }

        internal bool IsObjectAreaWithinBounds(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0)
            {
                return false;
            }

            if (tileX >= EngineHandle.GridSize - 1)
            {
                return false;
            }

            return tileY < EngineHandle.GridSize - 1;
        }

        internal bool IsGridCoordinateWithinBounds(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0)
            {
                return false;
            }

            if (tileX >= EngineHandle.GridSize)
            {
                return false;
            }

            return tileY < EngineHandle.GridSize;
        }

        internal bool IsSupportedWallObject(WallObject wallObject)
            => wallObject.Type == SolidWallType;

        internal bool IsSupportedWorldObject(WorldObject worldObject)
        {
            if (worldObject.Type == SolidWorldObjectType)
            {
                return true;
            }

            return worldObject.Type == DirectionalWorldObjectType;
        }

        internal int GetObjectWidth(WorldObject worldObject, int rotation)
        {
            if (UsesDefaultFootprint(rotation))
            {
                return worldObject.Width;
            }

            return worldObject.Height;
        }

        internal int GetObjectHeight(WorldObject worldObject, int rotation)
        {
            if (UsesDefaultFootprint(rotation))
            {
                return worldObject.Height;
            }

            return worldObject.Width;
        }

        internal void RemoveWallTile(int tileX, int tileY, int wallRotation)
        {
            if (wallRotation == WallRotationZeroDegrees)
            {
                RemoveZeroDegreeWall(tileX, tileY);
                return;
            }

            if (wallRotation == WallRotationNinetyDegrees)
            {
                RemoveNinetyDegreeWall(tileX, tileY);
                return;
            }

            if (wallRotation == WallRotationOneHundredEightyDegrees)
            {
                ClearTileFlag(tileX, tileY, TileFlagDiagonalSouthEastBoundary);
                return;
            }

            if (wallRotation == WallRotationTwoHundredSeventyDegrees)
            {
                ClearTileFlag(tileX, tileY, TileFlagDiagonalSouthWestBoundary);
            }
        }

        internal void CreateWallTile(int tileX, int tileY, int wallRotation)
        {
            if (wallRotation == WallRotationZeroDegrees)
            {
                CreateZeroDegreeWall(tileX, tileY);
                return;
            }

            if (wallRotation == WallRotationNinetyDegrees)
            {
                CreateNinetyDegreeWall(tileX, tileY);
                return;
            }

            if (wallRotation == WallRotationOneHundredEightyDegrees)
            {
                SetTileFlag(tileX, tileY, TileFlagDiagonalSouthEastBoundary);
                return;
            }

            if (wallRotation == WallRotationTwoHundredSeventyDegrees)
            {
                SetTileFlag(tileX, tileY, TileFlagDiagonalSouthWestBoundary);
            }
        }

        internal void ClearObjectTiles(
            int tileX,
            int tileY,
            int objectWidth,
            int objectHeight,
            int worldObjectType,
            int rotation)
        {
            for (int occupiedTileX = tileX;
                occupiedTileX < tileX + objectWidth;
                occupiedTileX += 1)
            {
                for (int occupiedTileY = tileY;
                    occupiedTileY < tileY + objectHeight;
                    occupiedTileY += 1)
                {
                    ClearObjectTile(occupiedTileX, occupiedTileY, worldObjectType, rotation);
                }
            }
        }

        internal void SetObjectTiles(
            int tileX,
            int tileY,
            int objectWidth,
            int objectHeight,
            int worldObjectType,
            int rotation)
        {
            for (int occupiedTileX = tileX;
                occupiedTileX < tileX + objectWidth;
                occupiedTileX += 1)
            {
                for (int occupiedTileY = tileY;
                    occupiedTileY < tileY + objectHeight;
                    occupiedTileY += 1)
                {
                    SetObjectTile(occupiedTileX, occupiedTileY, worldObjectType, rotation);
                }
            }
        }

        private void UpdateSceneTile(int tileX, int tileY)
        {
            if (HasBlockingSceneNeighbour(tileX, tileY))
            {
                engineHandle.SetTileFlags(tileX, tileY, OccupiedSceneFlag);
                return;
            }

            engineHandle.SetTileFlags(tileX, tileY, EmptySceneFlag);
        }

        private bool HasBlockingSceneNeighbour(int tileX, int tileY)
        {
            if ((engineHandle.GetTile(tileX, tileY) & SceneMaskCurrentTile) != 0)
            {
                return true;
            }

            if ((engineHandle.GetTile(tileX - 1, tileY) & SceneMaskWestTile) != 0)
            {
                return true;
            }

            if ((engineHandle.GetTile(tileX, tileY - 1) & SceneMaskNorthTile) != 0)
            {
                return true;
            }

            return (engineHandle.GetTile(tileX - 1, tileY - 1) & SceneMaskNorthWestTile) != 0;
        }

        private static bool IsSceneAreaWithinBounds(
            int tileX,
            int tileY,
            int objectWidth,
            int objectHeight)
        {
            if (tileX < MinimumSceneCoordinate || tileY < MinimumSceneCoordinate)
            {
                return false;
            }

            if (tileX + objectWidth >= EngineHandle.GridSize)
            {
                return false;
            }

            return tileY + objectHeight < EngineHandle.GridSize;
        }

        private static bool UsesDefaultFootprint(int rotation)
        {
            if (rotation == RotationZeroDegrees)
            {
                return true;
            }

            return rotation == RotationOneHundredEightyDegrees;
        }

        private void RemoveZeroDegreeWall(int tileX, int tileY)
        {
            ClearTileFlag(tileX, tileY, TileFlagNorthBoundary);

            if (tileY > 0)
            {
                engineHandle.DrawObjectSprite(tileX, tileY - 1, TileFlagSouthBoundary);
            }
        }

        private void RemoveNinetyDegreeWall(int tileX, int tileY)
        {
            ClearTileFlag(tileX, tileY, TileFlagWestBoundary);

            if (tileX > 0)
            {
                engineHandle.DrawObjectSprite(tileX - 1, tileY, TileFlagEastBoundary);
            }
        }

        private void CreateZeroDegreeWall(int tileX, int tileY)
        {
            SetTileFlag(tileX, tileY, TileFlagNorthBoundary);

            if (tileY > 0)
            {
                engineHandle.SetTileFlags(tileX, tileY - 1, TileFlagSouthBoundary);
            }
        }

        private void CreateNinetyDegreeWall(int tileX, int tileY)
        {
            SetTileFlag(tileX, tileY, TileFlagWestBoundary);

            if (tileX > 0)
            {
                engineHandle.SetTileFlags(tileX - 1, tileY, TileFlagEastBoundary);
            }
        }

        private void ClearObjectTile(
            int tileX,
            int tileY,
            int worldObjectType,
            int rotation)
        {
            if (worldObjectType == SolidWorldObjectType)
            {
                ClearTileFlag(tileX, tileY, TileFlagSolidObject);
                return;
            }

            ClearDirectionalObjectTile(tileX, tileY, rotation);
        }

        private void SetObjectTile(int tileX, int tileY, int worldObjectType, int rotation)
        {
            if (worldObjectType == SolidWorldObjectType)
            {
                SetTileFlag(tileX, tileY, TileFlagSolidObject);
                return;
            }

            SetDirectionalObjectTile(tileX, tileY, rotation);
        }

        private void ClearDirectionalObjectTile(int tileX, int tileY, int rotation)
        {
            if (rotation == RotationZeroDegrees)
            {
                ClearZeroDegreeObjectTile(tileX, tileY);
                return;
            }

            if (rotation == RotationNinetyDegrees)
            {
                ClearNinetyDegreeObjectTile(tileX, tileY);
                return;
            }

            if (rotation == RotationOneHundredEightyDegrees)
            {
                ClearOneHundredEightyDegreeObjectTile(tileX, tileY);
                return;
            }

            if (rotation == RotationTwoHundredSeventyDegrees)
            {
                ClearTwoHundredSeventyDegreeObjectTile(tileX, tileY);
            }
        }

        private void SetDirectionalObjectTile(int tileX, int tileY, int rotation)
        {
            if (rotation == RotationZeroDegrees)
            {
                SetZeroDegreeObjectTile(tileX, tileY);
                return;
            }

            if (rotation == RotationNinetyDegrees)
            {
                SetNinetyDegreeObjectTile(tileX, tileY);
                return;
            }

            if (rotation == RotationOneHundredEightyDegrees)
            {
                SetOneHundredEightyDegreeObjectTile(tileX, tileY);
                return;
            }

            if (rotation == RotationTwoHundredSeventyDegrees)
            {
                SetTwoHundredSeventyDegreeObjectTile(tileX, tileY);
            }
        }

        private void ClearZeroDegreeObjectTile(int tileX, int tileY)
        {
            ClearTileFlag(tileX, tileY, TileFlagWestBoundary);

            if (tileX > 0)
            {
                engineHandle.DrawObjectSprite(tileX - 1, tileY, TileFlagEastBoundary);
            }
        }

        private void ClearNinetyDegreeObjectTile(int tileX, int tileY)
        {
            ClearTileFlag(tileX, tileY, TileFlagSouthBoundary);

            if (tileY < EngineHandle.GridSize - 1)
            {
                engineHandle.DrawObjectSprite(tileX, tileY + 1, TileFlagNorthBoundary);
            }
        }

        private void ClearOneHundredEightyDegreeObjectTile(int tileX, int tileY)
        {
            ClearTileFlag(tileX, tileY, TileFlagEastBoundary);

            if (tileX < EngineHandle.GridSize - 1)
            {
                engineHandle.DrawObjectSprite(tileX + 1, tileY, TileFlagWestBoundary);
            }
        }

        private void ClearTwoHundredSeventyDegreeObjectTile(int tileX, int tileY)
        {
            ClearTileFlag(tileX, tileY, TileFlagNorthBoundary);

            if (tileY > 0)
            {
                engineHandle.DrawObjectSprite(tileX, tileY - 1, TileFlagSouthBoundary);
            }
        }

        private void SetZeroDegreeObjectTile(int tileX, int tileY)
        {
            SetTileFlag(tileX, tileY, TileFlagWestBoundary);

            if (tileX > 0)
            {
                engineHandle.SetTileFlags(tileX - 1, tileY, TileFlagEastBoundary);
            }
        }

        private void SetNinetyDegreeObjectTile(int tileX, int tileY)
        {
            SetTileFlag(tileX, tileY, TileFlagSouthBoundary);

            if (tileY < EngineHandle.GridSize - 1)
            {
                engineHandle.SetTileFlags(tileX, tileY + 1, TileFlagNorthBoundary);
            }
        }

        private void SetOneHundredEightyDegreeObjectTile(int tileX, int tileY)
        {
            SetTileFlag(tileX, tileY, TileFlagEastBoundary);

            if (tileX < EngineHandle.GridSize - 1)
            {
                engineHandle.SetTileFlags(tileX + 1, tileY, TileFlagWestBoundary);
            }
        }

        private void SetTwoHundredSeventyDegreeObjectTile(int tileX, int tileY)
        {
            SetTileFlag(tileX, tileY, TileFlagNorthBoundary);

            if (tileY > 0)
            {
                engineHandle.SetTileFlags(tileX, tileY - 1, TileFlagSouthBoundary);
            }
        }

        private void SetTileFlag(int tileX, int tileY, int tileFlag)
            => engineHandle.Tiles[tileX][tileY] |= tileFlag;

        private void ClearTileFlag(int tileX, int tileY, int tileFlag)
            => engineHandle.Tiles[tileX][tileY] &= ~tileFlag;
    }
}
