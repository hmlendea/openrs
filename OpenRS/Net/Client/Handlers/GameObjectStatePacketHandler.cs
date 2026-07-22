using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class GameObjectStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int RotationHalfTurn => 4;
        private static int ObjectShadingRed => 48;
        private static int ObjectShadingGreen => 48;
        private static int ObjectShadingBlue => -50;
        private static int ObjectShadingAmbient => -10;
        private static int ObjectShadingDiffuse => -50;
        private static int SpecialObjectType => 74;
        private static int SpecialObjectYOffset => -480;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameObjectStatePacketHandler>();

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (command != ServerCommand.GameObjectPositions)
            {
                return false;
            }

            HandleGameObjectPositions(packetLength, packetData);
            return true;
        }

        private void HandleGameObjectPositions(int packetLength, sbyte[] packetData)
        {
            int offset = 1;

            while (offset < packetLength)
            {
                if (BinaryDataReader.GetByte(packetData[offset]) ==
                    PacketHandlerConstants.SectionResetMarker)
                {
                    offset = ResetObjectSection(packetData, offset);
                    continue;
                }

                offset = UpdateGameObject(packetData, offset);
            }
        }

        private int ResetObjectSection(sbyte[] packetData, int offset)
        {
            int sectionX = SectionPacketCoordinates.GetSectionCoordinateFromByte(
                Client.sectionX,
                packetData,
                offset + 1);
            int sectionY = SectionPacketCoordinates.GetSectionCoordinateFromByte(
                Client.sectionY,
                packetData,
                offset + 2);
            int nextOffset = offset + 3;
            int retainedCount = 0;

            for (int objectIndex = 0; objectIndex < Client.objectCount; objectIndex += 1)
            {
                if (IsOutsideSection(objectIndex, sectionX, sectionY))
                {
                    CopyObject(objectIndex, retainedCount);
                    retainedCount += 1;
                    continue;
                }

                RemoveGameObjectAt(objectIndex);
            }

            Client.objectCount = retainedCount;
            return nextOffset;
        }

        private int UpdateGameObject(sbyte[] packetData, int offset)
        {
            int objectId = BinaryDataReader.GetShort(packetData, offset);
            int nextOffset = offset + 2;
            int objectX = Client.sectionX + packetData[nextOffset++];
            int objectY = Client.sectionY + packetData[nextOffset++];
            int rotation = packetData[nextOffset++];
            RemoveMatchingGameObjects(objectX, objectY, rotation);

            if (objectId != PacketHandlerConstants.ObjectRemovalIdentifier)
            {
                AddGameObject(objectId, objectX, objectY, rotation);
            }

            return nextOffset;
        }

        private void RemoveMatchingGameObjects(int objectX, int objectY, int rotation)
        {
            int retainedCount = 0;

            for (int objectIndex = 0; objectIndex < Client.objectCount; objectIndex += 1)
            {
                if (ShouldRetainGameObject(objectIndex, objectX, objectY, rotation))
                {
                    CopyObject(objectIndex, retainedCount);
                    retainedCount += 1;
                    continue;
                }

                RemoveGameObjectAt(objectIndex);
            }

            Client.objectCount = retainedCount;
        }

        private bool ShouldRetainGameObject(
            int objectIndex,
            int objectX,
            int objectY,
            int rotation)
        {
            if (Client.objectX[objectIndex] != objectX || Client.objectY[objectIndex] != objectY)
            {
                return true;
            }

            return Client.objectRotation[objectIndex] != rotation;
        }

        private bool IsOutsideSection(int objectIndex, int sectionX, int sectionY)
            => SectionPacketCoordinates.IsOutsideSection(
                Client.objectX[objectIndex],
                Client.objectY[objectIndex],
                sectionX,
                sectionY);

        private void RemoveGameObjectAt(int objectIndex)
        {
            Client.gameCamera.RemoveModel(Client.objectArray[objectIndex]);
            Client.engineHandle.RemoveObject(
                Client.objectX[objectIndex],
                Client.objectY[objectIndex],
                Client.objectType[objectIndex],
                Client.objectRotation[objectIndex]);
        }

        private void AddGameObject(int objectId, int objectX, int objectY, int rotation)
        {
            if (objectId >= Client.entityManager.WorldObjectCount)
            {
                LogUnknownObject(objectId);
                return;
            }

            Client.engineHandle.RegisterObjectDir(objectX, objectY, rotation);
            GameObject gameObject = CreateGameObject(objectId, objectX, objectY, rotation);
            ApplySpecialObjectOffset(gameObject, objectId);
            StoreGameObject(gameObject, objectId, objectX, objectY, rotation);
        }

        private GameObject CreateGameObject(int objectId, int objectX, int objectY, int rotation)
        {
            int objectWidth = GetObjectWidth(objectId, rotation);
            int objectHeight = GetObjectHeight(objectId, rotation);
            int worldX = ((objectX + objectX + objectWidth) * Client.gridSize) / 2;
            int worldY = ((objectY + objectY + objectHeight) * Client.gridSize) / 2;
            int modelIndex = Client.entityManager.GetWorldObject(objectId).ModelIndex;
            GameObject gameObject = Client.gameDataObjects[modelIndex].CreateParent();
            Client.gameCamera.AddModel(gameObject);
            gameObject.Index = Client.objectCount;
            gameObject.OffsetMiniPosition(0, rotation * 32, 0);
            gameObject.OffsetPosition(
                worldX,
                -Client.engineHandle.GetAveragedElevation(worldX, worldY),
                worldY);
            gameObject.UpdateShading(
                true,
                ObjectShadingRed,
                ObjectShadingGreen,
                ObjectShadingBlue,
                ObjectShadingAmbient,
                ObjectShadingDiffuse);
            Client.engineHandle.CreateObject(objectX, objectY, objectId, rotation);
            return gameObject;
        }

        private static void ApplySpecialObjectOffset(GameObject gameObject, int objectId)
        {
            if (objectId == SpecialObjectType)
            {
                gameObject.OffsetPosition(0, SpecialObjectYOffset, 0);
            }
        }

        private void StoreGameObject(
            GameObject gameObject,
            int objectId,
            int objectX,
            int objectY,
            int rotation)
        {
            Client.objectX[Client.objectCount] = objectX;
            Client.objectY[Client.objectCount] = objectY;
            Client.objectType[Client.objectCount] = objectId;
            Client.objectRotation[Client.objectCount] = rotation;
            Client.objectArray[Client.objectCount] = gameObject;
            Client.objectCount += 1;
        }

        private void LogUnknownObject(int objectId)
            => logger.Warn(
                GameOperation.HandlePacket,
                "Skipping unknown object.",
                new LogInfo(GameLogInfoKey.ObjectIndex, objectId),
                new LogInfo(
                    GameLogInfoKey.ObjectCount,
                    Client.entityManager.WorldObjectCount));

        private int GetObjectWidth(int objectId, int rotation)
        {
            if (rotation == 0 || rotation == RotationHalfTurn)
            {
                return Client.entityManager.GetWorldObject(objectId).Width;
            }

            return Client.entityManager.GetWorldObject(objectId).Height;
        }

        private int GetObjectHeight(int objectId, int rotation)
        {
            if (rotation == 0 || rotation == RotationHalfTurn)
            {
                return Client.entityManager.GetWorldObject(objectId).Height;
            }

            return Client.entityManager.GetWorldObject(objectId).Width;
        }

        private void CopyObject(int sourceIndex, int destinationIndex)
        {
            if (sourceIndex == destinationIndex)
            {
                return;
            }

            Client.objectArray[destinationIndex] = Client.objectArray[sourceIndex];
            Client.objectArray[destinationIndex].Index = destinationIndex;
            Client.objectX[destinationIndex] = Client.objectX[sourceIndex];
            Client.objectY[destinationIndex] = Client.objectY[sourceIndex];
            Client.objectType[destinationIndex] = Client.objectType[sourceIndex];
            Client.objectRotation[destinationIndex] = Client.objectRotation[sourceIndex];
        }
    }
}