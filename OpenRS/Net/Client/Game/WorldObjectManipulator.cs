using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    internal sealed class WorldObjectManipulator
    {
        private static readonly int SingleTileSize = 1;

        private readonly EngineHandle engineHandle;
        private readonly WorldObjectSceneAdder worldObjectSceneAdder;
        private readonly WorldObjectTileFlagManager worldObjectTileFlagManager;

        internal WorldObjectManipulator(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
            worldObjectTileFlagManager = new WorldObjectTileFlagManager(engineHandle);
            worldObjectSceneAdder = new WorldObjectSceneAdder(
                engineHandle,
                worldObjectTileFlagManager);
        }

        internal void AddObjectToScene(
            int tileX,
            int tileY,
            int objectWidth,
            int objectHeight)
            => worldObjectTileFlagManager.AddObjectToScene(
                tileX,
                tileY,
                objectWidth,
                objectHeight);

        internal void RemoveWallObject(
            int tileX,
            int tileY,
            int wallRotation,
            int wallObjectIndex)
        {
            if (!worldObjectTileFlagManager.IsObjectAreaWithinBounds(tileX, tileY))
            {
                return;
            }

            WallObject wallObject = engineHandle.entityManager.GetWallObject(wallObjectIndex);

            if (!worldObjectTileFlagManager.IsSupportedWallObject(wallObject))
            {
                return;
            }

            worldObjectTileFlagManager.RemoveWallTile(tileX, tileY, wallRotation);
            AddObjectToScene(tileX, tileY, SingleTileSize, SingleTileSize);
        }

        internal void CreateWall(
            int tileX,
            int tileY,
            int wallRotation,
            int wallObjectIndex)
        {
            if (!worldObjectTileFlagManager.IsObjectAreaWithinBounds(tileX, tileY))
            {
                return;
            }

            WallObject wallObject = engineHandle.entityManager.GetWallObject(wallObjectIndex);

            if (!worldObjectTileFlagManager.IsSupportedWallObject(wallObject))
            {
                return;
            }

            worldObjectTileFlagManager.CreateWallTile(tileX, tileY, wallRotation);
            AddObjectToScene(tileX, tileY, SingleTileSize, SingleTileSize);
        }

        internal void RegisterObjectDir(int tileX, int tileY, int direction)
        {
            if (!worldObjectTileFlagManager.IsGridCoordinateWithinBounds(tileX, tileY))
            {
                return;
            }

            engineHandle.objectDirs[tileX][tileY] = direction;
        }

        internal void RemoveObject(
            int tileX,
            int tileY,
            int worldObjectIndex,
            int rotation)
        {
            if (!worldObjectTileFlagManager.IsObjectAreaWithinBounds(tileX, tileY))
            {
                return;
            }

            WorldObject worldObject = engineHandle.entityManager.GetWorldObject(worldObjectIndex);

            if (!worldObjectTileFlagManager.IsSupportedWorldObject(worldObject))
            {
                return;
            }

            int objectWidth = worldObjectTileFlagManager.GetObjectWidth(worldObject, rotation);
            int objectHeight = worldObjectTileFlagManager.GetObjectHeight(worldObject, rotation);

            worldObjectTileFlagManager.ClearObjectTiles(
                tileX,
                tileY,
                objectWidth,
                objectHeight,
                worldObject.Type,
                rotation);

            AddObjectToScene(tileX, tileY, objectWidth, objectHeight);
        }

        internal void CreateObject(int tileX, int tileY, int worldObjectIndex, int rotation)
        {
            if (!worldObjectTileFlagManager.IsObjectAreaWithinBounds(tileX, tileY))
            {
                return;
            }

            WorldObject worldObject = engineHandle.entityManager.GetWorldObject(worldObjectIndex);

            if (!worldObjectTileFlagManager.IsSupportedWorldObject(worldObject))
            {
                return;
            }

            int objectWidth = worldObjectTileFlagManager.GetObjectWidth(worldObject, rotation);
            int objectHeight = worldObjectTileFlagManager.GetObjectHeight(worldObject, rotation);

            worldObjectTileFlagManager.SetObjectTiles(
                tileX,
                tileY,
                objectWidth,
                objectHeight,
                worldObject.Type,
                rotation);

            AddObjectToScene(tileX, tileY, objectWidth, objectHeight);
        }

        internal void AddObjects(GameObject[] tileModels)
            => worldObjectSceneAdder.AddObjects(tileModels);
    }
}
