using System;

namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectNormalCalculator
    {
        private static int AmbientShadeShiftAmount => 8;

        private static int CrossProductShiftThreshold => 8192;

        private static double NormalisationScale => 256D;

        private static int NormalComponentScale => 0x10000;

        private static int NormalZComponentScale => 65535;

        private static int UnrenderedFaceFlag => -1;

        internal static void Recalculate(GameObject gameObject)
        {
            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            int ambientLightProduct = gameObject.AmbientLightLevel * gameObject.LightMagnitude;
            int ambientShade = ambientLightProduct >> AmbientShadeShiftAmount;
            UpdateFlatShadedFaces(gameObject, ambientShade);
            int[] accumulatedNormalsX = new int[gameObject.VertexCount];
            int[] accumulatedNormalsY = new int[gameObject.VertexCount];
            int[] accumulatedNormalsZ = new int[gameObject.VertexCount];
            int[] accumulatedNormalCount = new int[gameObject.VertexCount];

            AccumulateGouraudNormals(
                gameObject,
                accumulatedNormalsX,
                accumulatedNormalsY,
                accumulatedNormalsZ,
                accumulatedNormalCount);
            ApplyAccumulatedNormals(
                gameObject,
                ambientShade,
                accumulatedNormalsX,
                accumulatedNormalsY,
                accumulatedNormalsZ,
                accumulatedNormalCount);
        }

        internal static void CalculatePolygonNormals(GameObject gameObject)
        {
            if (gameObject.DoesNotReceiveShadows && gameObject.HasNoCollider)
            {
                return;
            }

            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                ComputeAndStoreFaceNormal(gameObject, faceIndex);
            }

            Recalculate(gameObject);
        }

        private static void UpdateFlatShadedFaces(GameObject gameObject, int ambientShade)
        {
            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                if (gameObject.GouraudShade[faceIndex] != GameObject.DefaultShadeValue)
                {
                    gameObject.GouraudShade[faceIndex] =
                        (gameObject.NormalX[faceIndex] * gameObject.LightDirectionX +
                        gameObject.NormalY[faceIndex] * gameObject.LightDirectionY +
                        gameObject.NormalZ[faceIndex] * gameObject.LightDirectionZ) /
                        ambientShade;
                }
            }
        }

        private static void AccumulateGouraudNormals(
            GameObject gameObject,
            int[] accumulatedNormalsX,
            int[] accumulatedNormalsY,
            int[] accumulatedNormalsZ,
            int[] accumulatedNormalCount)
        {
            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                if (gameObject.GouraudShade[faceIndex] == GameObject.DefaultShadeValue)
                {
                    AccumulateFaceNormals(
                        gameObject,
                        faceIndex,
                        accumulatedNormalsX,
                        accumulatedNormalsY,
                        accumulatedNormalsZ,
                        accumulatedNormalCount);
                }
            }
        }

        private static void AccumulateFaceNormals(
            GameObject gameObject,
            int faceIndex,
            int[] accumulatedNormalsX,
            int[] accumulatedNormalsY,
            int[] accumulatedNormalsZ,
            int[] accumulatedNormalCount)
        {
            int faceVertexCount = gameObject.FaceVertexCounts[faceIndex];
            int[] faceVertices = gameObject.FaceVertexIndices[faceIndex];

            for (int vertexPosition = 0; vertexPosition < faceVertexCount; vertexPosition += 1)
            {
                int vertexIndex = faceVertices[vertexPosition];
                accumulatedNormalsX[vertexIndex] += gameObject.NormalX[faceIndex];
                accumulatedNormalsY[vertexIndex] += gameObject.NormalY[faceIndex];
                accumulatedNormalsZ[vertexIndex] += gameObject.NormalZ[faceIndex];
                accumulatedNormalCount[vertexIndex] += 1;
            }
        }

        private static void ApplyAccumulatedNormals(
            GameObject gameObject,
            int ambientShade,
            int[] accumulatedNormalsX,
            int[] accumulatedNormalsY,
            int[] accumulatedNormalsZ,
            int[] accumulatedNormalCount)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                if (accumulatedNormalCount[vertexIndex] > 0)
                {
                    gameObject.FaceNormalComponent[vertexIndex] =
                        (accumulatedNormalsX[vertexIndex] * gameObject.LightDirectionX +
                        accumulatedNormalsY[vertexIndex] * gameObject.LightDirectionY +
                        accumulatedNormalsZ[vertexIndex] * gameObject.LightDirectionZ) /
                        (ambientShade * accumulatedNormalCount[vertexIndex]);
                }
            }
        }

        private static void ComputeAndStoreFaceNormal(GameObject gameObject, int faceIndex)
        {
            int[] faceVertices = gameObject.FaceVertexIndices[faceIndex];
            int firstVertexX = gameObject.WorldVertX[faceVertices[0]];
            int firstVertexY = gameObject.WorldVertY[faceVertices[0]];
            int firstVertexZ = gameObject.WorldVertZ[faceVertices[0]];
            int edge1X = gameObject.WorldVertX[faceVertices[1]] - firstVertexX;
            int edge1Y = gameObject.WorldVertY[faceVertices[1]] - firstVertexY;
            int edge1Z = gameObject.WorldVertZ[faceVertices[1]] - firstVertexZ;
            int edge2X = gameObject.WorldVertX[faceVertices[2]] - firstVertexX;
            int edge2Y = gameObject.WorldVertY[faceVertices[2]] - firstVertexY;
            int edge2Z = gameObject.WorldVertZ[faceVertices[2]] - firstVertexZ;
            int crossX = edge1Y * edge2Z - edge2Y * edge1Z;
            int crossY = edge1Z * edge2X - edge2Z * edge1X;
            int crossZ = edge1X * edge2Y - edge2X * edge1Y;

            NormaliseCrossProduct(gameObject, faceIndex, crossX, crossY, crossZ);
        }

        private static bool CrossProductExceedsThreshold(int crossX, int crossY, int crossZ) =>
            crossX > CrossProductShiftThreshold ||
            crossY > CrossProductShiftThreshold ||
            crossZ > CrossProductShiftThreshold ||
            crossX < -CrossProductShiftThreshold ||
            crossY < -CrossProductShiftThreshold ||
            crossZ < -CrossProductShiftThreshold;

        private static void NormaliseCrossProduct(
            GameObject gameObject,
            int faceIndex,
            int crossX,
            int crossY,
            int crossZ)
        {
            while (CrossProductExceedsThreshold(crossX, crossY, crossZ))
            {
                crossX >>= 1;
                crossY >>= 1;
                crossZ >>= 1;
            }

            double squaredMagnitude = crossX * crossX + crossY * crossY + crossZ * crossZ;
            int normalisedMagnitude = (int)(NormalisationScale * Math.Sqrt(squaredMagnitude));

            if (normalisedMagnitude <= 0)
            {
                normalisedMagnitude = 1;
            }

            gameObject.NormalX[faceIndex] = crossX * NormalComponentScale / normalisedMagnitude;
            gameObject.NormalY[faceIndex] = crossY * NormalComponentScale / normalisedMagnitude;
            gameObject.NormalZ[faceIndex] =
                crossZ * NormalZComponentScale /
                normalisedMagnitude;
            gameObject.FaceRenderFlag[faceIndex] = UnrenderedFaceFlag;
        }
    }
}