using System;
using System.IO;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectDataLoader
    {
        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger(typeof(GameObjectDataLoader));

        internal static void Initialise(GameObject gameObject)
        {
            gameObject.ObjectState = 1;
            gameObject.IsVisible = true;
            gameObject.IsTranslucent = true;
            gameObject.IsPerspectiveTextured = false;
            gameObject.IsGiantCrystal = false;
            gameObject.Index = -1;
            gameObject.DoesShareWorldVertices = false;
            gameObject.HasNoCollider = false;
            gameObject.DoesNotReceiveShadows = false;
            gameObject.DoesShareEntityArrays = false;
            gameObject.DoesShareVertexArrays = false;
            gameObject.MaximumFaceSpan = GameObject.DefaultShadeValue;
            gameObject.LightDirectionX = GameObject.DefaultLightDirectionX;
            gameObject.LightDirectionY = GameObject.DefaultLightDirectionY;
            gameObject.LightDirectionZ = GameObject.DefaultLightDirectionZ;
            gameObject.LightMagnitude = GameObject.DefaultLightMagnitude;
            gameObject.AmbientLightLevel = GameObject.DefaultAmbientLightLevel;
            gameObject.BaseShadeLevel = GameObject.DefaultBaseShadeLevel;
        }

        internal static void AllocateOptionalArrays(
            GameObject gameObject,
            int vertexCapacity,
            int faceCapacity)
        {
            if (!gameObject.DoesShareVertexArrays)
            {
                gameObject.ProjectedX = new int[vertexCapacity];
                gameObject.ProjectedY = new int[vertexCapacity];
                gameObject.ProjectedDepth = new int[vertexCapacity];
                gameObject.ProjectedU = new int[vertexCapacity];
                gameObject.ProjectedV = new int[vertexCapacity];
            }

            if (!gameObject.DoesShareEntityArrays)
            {
                gameObject.PolygonTypeData = new int[faceCapacity];
                gameObject.EntityType = new int[faceCapacity];
            }

            if (gameObject.DoesShareWorldVertices)
            {
                gameObject.WorldVertX = gameObject.VertexCoordinatesX;
                gameObject.WorldVertY = gameObject.VertexCoordinatesY;
                gameObject.WorldVertZ = gameObject.VertexCoordinatesZ;
            }
            else
            {
                gameObject.WorldVertX = new int[vertexCapacity];
                gameObject.WorldVertY = new int[vertexCapacity];
                gameObject.WorldVertZ = new int[vertexCapacity];
            }

            if (!gameObject.DoesNotReceiveShadows || !gameObject.HasNoCollider)
            {
                gameObject.NormalX = new int[faceCapacity];
                gameObject.NormalY = new int[faceCapacity];
                gameObject.NormalZ = new int[faceCapacity];
            }

            if (!gameObject.HasNoCollider)
            {
                gameObject.faceBoundsMinX = new int[faceCapacity];
                gameObject.faceBoundsMaxX = new int[faceCapacity];
                gameObject.faceBoundsMinY = new int[faceCapacity];
                gameObject.faceBoundsMaxY = new int[faceCapacity];
                gameObject.faceBoundsMinZ = new int[faceCapacity];
                gameObject.faceBoundsMaxZ = new int[faceCapacity];
            }
        }

        internal static sbyte[] LoadFromFile(GameObject gameObject, string fileName)
        {
            byte[] fileBuffer;

            try
            {
                MemoryStream inputStream = GameResourceLoader.OpenInputStream(fileName);
                byte[] headerBuffer = new byte[3];
                gameObject.ShadeBufferIndex = 0;
                int headerBytesRead = 0;

                while (headerBytesRead < 3)
                {
                    headerBytesRead += inputStream.Read(
                        headerBuffer,
                        headerBytesRead,
                        3 - headerBytesRead);
                }

                int dataLength = gameObject.GetShadeValue((sbyte[])(Array)headerBuffer);
                fileBuffer = new byte[dataLength];
                gameObject.ShadeBufferIndex = 0;
                int dataBytesRead = 0;

                while (dataBytesRead < dataLength)
                {
                    dataBytesRead += inputStream.Read(
                        fileBuffer,
                        dataBytesRead,
                        dataLength - dataBytesRead);
                }

                inputStream.Close();
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.LoadGameObject,
                    "Failed to load the game object from file.",
                    exception,
                    new LogInfo(GameLogInfoKey.FileName, fileName));
                gameObject.VertexCount = 0;
                gameObject.FaceCount = 0;

                return null;
            }

            return (sbyte[])(Array)fileBuffer;
        }

        internal static void ReadVerticesFromShadeBuffer(
            GameObject gameObject,
            sbyte[] fileData,
            int readVertexCount)
        {
            for (int vertexIndex = 0; vertexIndex < readVertexCount; vertexIndex += 1)
            {
                int vertexX = gameObject.GetShadeValue(fileData);
                int vertexY = gameObject.GetShadeValue(fileData);
                int vertexZ = gameObject.GetShadeValue(fileData);
                gameObject.GetVertexIndex(vertexX, vertexY, vertexZ);
            }
        }

        internal static void ReadFacesFromShadeBuffer(
            GameObject gameObject,
            sbyte[] fileData,
            int readFaceCount)
        {
            for (int faceIndex = 0; faceIndex < readFaceCount; faceIndex += 1)
            {
                ReadSingleFaceFromShadeBuffer(gameObject, fileData, faceIndex);
            }
        }

        private static void ReadSingleFaceFromShadeBuffer(
            GameObject gameObject,
            sbyte[] fileData,
            int faceIndex)
        {
            int primaryVertexCount = gameObject.GetShadeValue(fileData);
            int textureBackValue = gameObject.GetShadeValue(fileData);
            int textureFrontValue = gameObject.GetShadeValue(fileData);
            int groupMappingCount = gameObject.GetShadeValue(fileData);
            gameObject.AmbientLightLevel = gameObject.GetShadeValue(fileData);
            gameObject.BaseShadeLevel = gameObject.GetShadeValue(fileData);
            int isGouraud = gameObject.GetShadeValue(fileData);

            int[] primaryVertexIndices = new int[primaryVertexCount];

            for (int index = 0; index < primaryVertexCount; index += 1)
            {
                primaryVertexIndices[index] = gameObject.GetShadeValue(fileData);
            }

            int[] groupMappingIndices = new int[groupMappingCount];

            for (int index = 0; index < groupMappingCount; index += 1)
            {
                groupMappingIndices[index] = gameObject.GetShadeValue(fileData);
            }

            int addedFaceIndex = gameObject.AddFaceVertices(
                primaryVertexCount,
                primaryVertexIndices,
                textureBackValue,
                textureFrontValue);
            gameObject.polygonGroupMapping[faceIndex] = groupMappingIndices;
            gameObject.GouraudShade[addedFaceIndex] = 0;

            if (isGouraud != 0)
            {
                gameObject.GouraudShade[addedFaceIndex] = GameObject.DefaultShadeValue;
            }
        }
    }
}
