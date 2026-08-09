using System;
using System.IO;

using Microsoft.Xna.Framework;

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

        internal static int ReadBinaryData(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int readVertexCount,
            int readFaceCount)
        {
            offset = ReadVertexCoordinates(gameObject, data, offset, readVertexCount);
            gameObject.VertexCount = readVertexCount;
            offset = ReadFaceVertexCounts(gameObject, data, offset, readFaceCount);
            offset = ReadTextureData(gameObject, data, offset, readFaceCount);
            offset = ReadGouraudShadeData(gameObject, data, offset, readFaceCount);
            ReadFaceVertexIndices(gameObject, data, offset, readFaceCount, readVertexCount);

            return offset;
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

        private static int ReadVertexCoordinates(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int readVertexCount)
        {
            for (int vertexIndex = 0; vertexIndex < readVertexCount; vertexIndex += 1)
            {
                gameObject.polygonGroupMapping[vertexIndex] = new int[1];
                gameObject.VertexCoordinatesX[vertexIndex] =
                    BinaryDataReader.GetSignedShort(data, offset);
                gameObject.VertexVectors[vertexIndex] = new Vector3(
                    gameObject.VertexCoordinatesX[vertexIndex],
                    gameObject.VertexVectors[vertexIndex].Y,
                    gameObject.VertexVectors[vertexIndex].Z);
                offset += 2;
            }

            for (int vertexIndex = 0; vertexIndex < readVertexCount; vertexIndex += 1)
            {
                gameObject.VertexCoordinatesY[vertexIndex] =
                    BinaryDataReader.GetSignedShort(data, offset);
                gameObject.VertexVectors[vertexIndex] = new Vector3(
                    gameObject.VertexVectors[vertexIndex].X,
                    gameObject.VertexCoordinatesY[vertexIndex],
                    gameObject.VertexVectors[vertexIndex].Z);
                offset += 2;
            }

            for (int vertexIndex = 0; vertexIndex < readVertexCount; vertexIndex += 1)
            {
                gameObject.VertexCoordinatesZ[vertexIndex] =
                    BinaryDataReader.GetSignedShort(data, offset);
                gameObject.VertexVectors[vertexIndex] = new Vector3(
                    gameObject.VertexVectors[vertexIndex].X,
                    gameObject.VertexVectors[vertexIndex].Y,
                    gameObject.VertexCoordinatesZ[vertexIndex]);
                offset += 2;
            }

            return offset;
        }

        private static int ReadFaceVertexCounts(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int readFaceCount)
        {
            for (int faceIndex = 0; faceIndex < readFaceCount; faceIndex += 1)
            {
                gameObject.FaceVertexCounts[faceIndex] = data[offset++] & 0xff;
            }

            return offset;
        }

        private static int ReadTextureData(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int readFaceCount)
        {
            for (int faceIndex = 0; faceIndex < readFaceCount; faceIndex += 1)
            {
                gameObject.TextureBack[faceIndex] =
                    BinaryDataReader.GetSignedShort(data, offset);
                offset += 2;

                if (gameObject.TextureBack[faceIndex] == GameObject.TextureShadeMaxValue)
                {
                    gameObject.TextureBack[faceIndex] = GameObject.DefaultShadeValue;
                }
            }

            for (int faceIndex = 0; faceIndex < readFaceCount; faceIndex += 1)
            {
                gameObject.TextureFront[faceIndex] =
                    BinaryDataReader.GetSignedShort(data, offset);
                offset += 2;

                if (gameObject.TextureFront[faceIndex] == GameObject.TextureShadeMaxValue)
                {
                    gameObject.TextureFront[faceIndex] = GameObject.DefaultShadeValue;
                }
            }

            return offset;
        }

        private static int ReadGouraudShadeData(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int readFaceCount)
        {
            for (int faceIndex = 0; faceIndex < readFaceCount; faceIndex += 1)
            {
                int shadeFlag = data[offset++] & 0xff;
                gameObject.GouraudShade[faceIndex] = 0;

                if (shadeFlag != 0)
                {
                    gameObject.GouraudShade[faceIndex] = GameObject.DefaultShadeValue;
                }
            }

            return offset;
        }

        private static void ReadFaceVertexIndices(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int readFaceCount,
            int readVertexCount)
        {
            for (int faceIndex = 0; faceIndex < readFaceCount; faceIndex += 1)
            {
                gameObject.FaceVertexIndices[faceIndex] =
                    new int[gameObject.FaceVertexCounts[faceIndex]];
                offset = ReadSingleFaceVertexIndices(
                    gameObject,
                    data,
                    offset,
                    faceIndex,
                    readVertexCount);
            }
        }

        private static int ReadSingleFaceVertexIndices(
            GameObject gameObject,
            sbyte[] data,
            int offset,
            int faceIndex,
            int readVertexCount)
        {
            int faceVertexCount = gameObject.FaceVertexCounts[faceIndex];

            for (int vertexPosition = 0; vertexPosition < faceVertexCount; vertexPosition += 1)
            {
                if (readVertexCount < 256)
                {
                    gameObject.FaceVertexIndices[faceIndex][vertexPosition] =
                        data[offset++] & 0xff;
                }
                else
                {
                    gameObject.FaceVertexIndices[faceIndex][vertexPosition] =
                        BinaryDataReader.GetShort(data, offset);
                    offset += 2;
                }
            }

            return offset;
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
