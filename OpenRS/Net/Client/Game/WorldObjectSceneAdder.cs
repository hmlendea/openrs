using System;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    internal sealed class WorldObjectSceneAdder
    {
        private static readonly int MaximumTrackedDiagonalWallValue = 60000;
        private static readonly int TileCentreDivisor = 2;
        private static readonly int RotationUnit = 32;
        private static readonly int ModelColourRed = 48;
        private static readonly int ModelColourGreen = 48;
        private static readonly int ModelColourBlue = -50;
        private static readonly int ModelColourShade = -10;
        private static readonly int ModelColourDiffuse = -50;

        private readonly EngineHandle engineHandle;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<WorldObjectSceneAdder>();
        private readonly WorldObjectTileFlagManager worldObjectTileFlagManager;

        internal WorldObjectSceneAdder(
            EngineHandle engineHandle,
            WorldObjectTileFlagManager worldObjectTileFlagManager)
        {
            this.engineHandle = engineHandle;
            this.worldObjectTileFlagManager = worldObjectTileFlagManager;
        }

        internal void AddObjects(GameObject[] tileModels)
        {
            for (int tileX = 0; tileX < EngineHandle.GridSize - 2; tileX += 1)
            {
                for (int tileY = 0; tileY < EngineHandle.GridSize - 2; tileY += 1)
                {
                    AddObjectAtTile(tileX, tileY, tileModels);
                }
            }
        }

        private void AddObjectAtTile(int tileX, int tileY, GameObject[] tileModels)
        {
            int diagonalWall = engineHandle.GetDiagonalWall(tileX, tileY);

            if (!IsTrackedWorldObject(diagonalWall))
            {
                return;
            }

            try
            {
                AddTrackedObject(tileX, tileY, diagonalWall, tileModels);
            }
            catch (Exception exception)
            {
                LogObjectAddFailure(tileX, tileY, diagonalWall, exception);
            }
        }

        private static bool IsTrackedWorldObject(int diagonalWall)
        {
            if (diagonalWall <= EngineHandle.LocationEntityBase)
            {
                return false;
            }

            return diagonalWall < MaximumTrackedDiagonalWallValue;
        }

        private void AddTrackedObject(
            int tileX,
            int tileY,
            int diagonalWall,
            GameObject[] tileModels)
        {
            int objectIndex = diagonalWall - EngineHandle.LocationEntityBase - 1;
            int objectRotation = engineHandle.objectDirs[tileX][tileY];
            WorldObject worldObject = engineHandle.entityManager.GetWorldObject(objectIndex);

            if (!worldObjectTileFlagManager.IsSupportedWorldObject(worldObject))
            {
                return;
            }

            int objectWidth = worldObjectTileFlagManager.GetObjectWidth(worldObject, objectRotation);
            int objectHeight = worldObjectTileFlagManager.GetObjectHeight(worldObject, objectRotation);

            worldObjectTileFlagManager.SetObjectTiles(
                tileX,
                tileY,
                objectWidth,
                objectHeight,
                worldObject.Type,
                objectRotation);
            worldObjectTileFlagManager.AddObjectToScene(tileX, tileY, objectWidth, objectHeight);
            AddObjectModel(
                tileX,
                tileY,
                objectRotation,
                objectWidth,
                objectHeight,
                worldObject,
                tileModels);

            if (objectWidth <= 1 && objectHeight <= 1)
            {
                return;
            }

            ClearCoveredObjectTiles(tileX, tileY, objectWidth, objectHeight, objectIndex);
        }

        private void AddObjectModel(
            int tileX,
            int tileY,
            int objectRotation,
            int objectWidth,
            int objectHeight,
            WorldObject worldObject,
            GameObject[] tileModels)
        {
            GameObject objectModel = CreateObjectModel(worldObject, tileModels);
            int worldCentreX = CalculateWorldCentre(tileX, objectWidth);
            int worldCentreZ = CalculateWorldCentre(tileY, objectHeight);
            int worldCentreY = -engineHandle.GetAveragedElevation(worldCentreX, worldCentreZ);

            objectModel.OffsetPosition(worldCentreX, worldCentreY, worldCentreZ);
            objectModel.SetRotation(0, engineHandle.GetTileRotation(tileX, tileY) * RotationUnit, 0);
            objectModel.SetRotation(0, objectRotation * RotationUnit, 0);
            engineHandle.WorldCamera.AddModel(objectModel);
            objectModel.SetModelColours(
                ModelColourRed,
                ModelColourGreen,
                ModelColourBlue,
                ModelColourShade,
                ModelColourDiffuse);
        }

        private static GameObject CreateObjectModel(WorldObject worldObject, GameObject[] tileModels)
            => tileModels[worldObject.ModelIndex].CreateParent(false, true, false, false);

        private static int CalculateWorldCentre(int startTile, int objectSize)
            => (startTile + startTile + objectSize) * EngineHandle.TileWorldSize / TileCentreDivisor;

        private void ClearCoveredObjectTiles(
            int originTileX,
            int originTileY,
            int objectWidth,
            int objectHeight,
            int objectIndex)
        {
            for (int occupiedTileX = originTileX;
                occupiedTileX < originTileX + objectWidth;
                occupiedTileX += 1)
            {
                for (int occupiedTileY = originTileY;
                    occupiedTileY < originTileY + objectHeight;
                    occupiedTileY += 1)
                {
                    ClearCoveredObjectTile(
                        objectIndex,
                        originTileX,
                        originTileY,
                        occupiedTileX,
                        occupiedTileY);
                }
            }
        }

        private void ClearCoveredObjectTile(
            int objectIndex,
            int originTileX,
            int originTileY,
            int occupiedTileX,
            int occupiedTileY)
        {
            if (IsOriginTile(originTileX, originTileY, occupiedTileX, occupiedTileY))
            {
                return;
            }

            if (!HasMatchingTrackedObject(occupiedTileX, occupiedTileY, objectIndex))
            {
                return;
            }

            SectorCoordinates sectorCoordinates = SectorCoordinates.From(occupiedTileX, occupiedTileY);
            int sectorTileIndex = sectorCoordinates.X * EngineHandle.SectorSize + sectorCoordinates.Y;
            engineHandle.TileDiagonalWall[sectorCoordinates.Layer][sectorTileIndex] = 0;
        }

        private static bool IsOriginTile(
            int originTileX,
            int originTileY,
            int occupiedTileX,
            int occupiedTileY)
            => occupiedTileX == originTileX && occupiedTileY == originTileY;

        private bool HasMatchingTrackedObject(int tileX, int tileY, int objectIndex)
            => engineHandle.GetDiagonalWall(tileX, tileY) - EngineHandle.LocationEntityBase - 1 ==
                objectIndex;

        private void LogObjectAddFailure(
            int tileX,
            int tileY,
            int diagonalWall,
            Exception exception)
        {
            int objectIndex = diagonalWall - EngineHandle.LocationEntityBase - 1;

            logger.Error(
                GameOperation.AddSceneObject,
                "Failed to add a world object to the scene.",
                exception,
                new LogInfo(GameLogInfoKey.ObjectIndex, objectIndex),
                new LogInfo(GameLogInfoKey.CoordinateX, tileX),
                new LogInfo(GameLogInfoKey.CoordinateY, tileY));
        }
    }
}
