using System;

namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectShaderCalculator
    {
        private static int BoundsExtremePositive => 0xf423f;

        private static int AmbientShadeShiftAmount => 8;

        private static int MaximumShadeLevel => 256;

        private static int LightIntensityMultiplier => 4;

        private static int AmbientLightOffsetRange => 64;

        private static int AmbientLightScale => 16;

        private static int AmbientLightBase => 128;

        private static int CrossProductShiftThreshold => 8192;

        private static double NormalisationScale => 256D;

        private static int NormalComponentScale => 0x10000;

        private static int NormalZComponentScale => 65535;

        private static int UnrenderedFaceFlag => -1;

        internal static void CalculateBounds(GameObject gameObject)
        {
            ResetBoundsToExtremes(gameObject);

            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                FaceBoundsData faceBounds = ComputeFaceBounds(gameObject, faceIndex);
                UpdateColliderBounds(gameObject, faceIndex, faceBounds);
                UpdateGlobalBounds(gameObject, faceBounds);
            }
        }

        internal static void RecalculateNormals(GameObject gameObject)
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

            RecalculateNormals(gameObject);
        }

        internal static void ApplyShading(
            GameObject gameObject,
            bool applyShadeValue,
            int lightIntensity,
            int ambientLight,
            int directionX,
            int directionY,
            int directionZ)
        {
            SetShadeLevels(gameObject, lightIntensity, ambientLight);

            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            InitialiseGouraudShades(gameObject, applyShadeValue);
            ApplyLightDirection(gameObject, directionX, directionY, directionZ);
        }

        internal static void SetLightingColours(
            GameObject gameObject,
            int lightIntensity,
            int ambientLight,
            int directionX,
            int directionY,
            int directionZ)
        {
            SetShadeLevels(gameObject, lightIntensity, ambientLight);

            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            ApplyLightDirection(gameObject, directionX, directionY, directionZ);
        }

        internal static void OffsetLightingColours(
            GameObject gameObject,
            int directionX,
            int directionY,
            int directionZ)
        {
            if (gameObject.DoesNotReceiveShadows)
            {
                return;
            }

            ApplyLightDirection(gameObject, directionX, directionY, directionZ);
        }

        private static void SetShadeLevels(
            GameObject gameObject,
            int lightIntensity,
            int ambientLight)
        {
            gameObject.BaseShadeLevel =
                MaximumShadeLevel - lightIntensity * LightIntensityMultiplier;
            gameObject.AmbientLightLevel =
                (AmbientLightOffsetRange - ambientLight) * AmbientLightScale +
                AmbientLightBase;
        }

        private static void ApplyLightDirection(
            GameObject gameObject,
            int directionX,
            int directionY,
            int directionZ)
        {
            gameObject.LightDirectionX = directionX;
            gameObject.LightDirectionY = directionY;
            gameObject.LightDirectionZ = directionZ;
            gameObject.LightMagnitude = (int)Math.Sqrt(
                directionX * directionX + directionY * directionY + directionZ * directionZ);
            RecalculateNormals(gameObject);
        }

        private static void InitialiseGouraudShades(GameObject gameObject, bool applyShadeValue)
        {
            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                gameObject.GouraudShade[faceIndex] = 0;

                if (applyShadeValue)
                {
                    gameObject.GouraudShade[faceIndex] = GameObject.DefaultShadeValue;
                }
            }
        }

        private static void ResetBoundsToExtremes(GameObject gameObject)
        {
            int extremeNegative = -BoundsExtremePositive;
            gameObject.BoundsMinX = BoundsExtremePositive;
            gameObject.BoundsMinY = BoundsExtremePositive;
            gameObject.BoundsMinZ = BoundsExtremePositive;
            gameObject.BoundsMaxX = extremeNegative;
            gameObject.BoundsMaxY = extremeNegative;
            gameObject.BoundsMaxZ = extremeNegative;
            gameObject.MaximumFaceSpan = extremeNegative;
        }

        private static FaceBoundsData ComputeFaceBounds(GameObject gameObject, int faceIndex)
        {
            int[] faceVerts = gameObject.FaceVertexIndices[faceIndex];
            int firstVertex = faceVerts[0];
            int vertCount = gameObject.FaceVertexCounts[faceIndex];

            FaceBoundsData bounds = new()
            {
                MinimumX = gameObject.WorldVertX[firstVertex],
                MaximumX = gameObject.WorldVertX[firstVertex],
                MinimumY = gameObject.WorldVertY[firstVertex],
                MaximumY = gameObject.WorldVertY[firstVertex],
                MinimumZ = gameObject.WorldVertZ[firstVertex],
                MaximumZ = gameObject.WorldVertZ[firstVertex]
            };

            for (int vertexIndex = 0; vertexIndex < vertCount; vertexIndex += 1)
            {
                UpdateAxisBounds(gameObject, faceVerts[vertexIndex], bounds);
            }

            return bounds;
        }

        private static void UpdateAxisBounds(
            GameObject gameObject,
            int vertexIndex,
            FaceBoundsData bounds)
        {
            if (gameObject.WorldVertX[vertexIndex] < bounds.MinimumX)
            {
                bounds.MinimumX = gameObject.WorldVertX[vertexIndex];
            }
            else if (gameObject.WorldVertX[vertexIndex] > bounds.MaximumX)
            {
                bounds.MaximumX = gameObject.WorldVertX[vertexIndex];
            }

            if (gameObject.WorldVertY[vertexIndex] < bounds.MinimumY)
            {
                bounds.MinimumY = gameObject.WorldVertY[vertexIndex];
            }
            else if (gameObject.WorldVertY[vertexIndex] > bounds.MaximumY)
            {
                bounds.MaximumY = gameObject.WorldVertY[vertexIndex];
            }

            if (gameObject.WorldVertZ[vertexIndex] < bounds.MinimumZ)
            {
                bounds.MinimumZ = gameObject.WorldVertZ[vertexIndex];
            }
            else if (gameObject.WorldVertZ[vertexIndex] > bounds.MaximumZ)
            {
                bounds.MaximumZ = gameObject.WorldVertZ[vertexIndex];
            }
        }

        private static void UpdateColliderBounds(
            GameObject gameObject,
            int faceIndex,
            FaceBoundsData faceBounds)
        {
            if (gameObject.HasNoCollider)
            {
                return;
            }

            gameObject.faceBoundsMinX[faceIndex] = faceBounds.MinimumX;
            gameObject.faceBoundsMaxX[faceIndex] = faceBounds.MaximumX;
            gameObject.faceBoundsMinY[faceIndex] = faceBounds.MinimumY;
            gameObject.faceBoundsMaxY[faceIndex] = faceBounds.MaximumY;
            gameObject.faceBoundsMinZ[faceIndex] = faceBounds.MinimumZ;
            gameObject.faceBoundsMaxZ[faceIndex] = faceBounds.MaximumZ;
        }

        private static void UpdateGlobalBounds(GameObject gameObject, FaceBoundsData faceBounds)
        {
            if (faceBounds.MaximumX - faceBounds.MinimumX > gameObject.MaximumFaceSpan)
            {
                gameObject.MaximumFaceSpan = faceBounds.MaximumX - faceBounds.MinimumX;
            }

            if (faceBounds.MaximumY - faceBounds.MinimumY > gameObject.MaximumFaceSpan)
            {
                gameObject.MaximumFaceSpan = faceBounds.MaximumY - faceBounds.MinimumY;
            }

            if (faceBounds.MaximumZ - faceBounds.MinimumZ > gameObject.MaximumFaceSpan)
            {
                gameObject.MaximumFaceSpan = faceBounds.MaximumZ - faceBounds.MinimumZ;
            }

            if (faceBounds.MinimumX < gameObject.BoundsMinX)
            {
                gameObject.BoundsMinX = faceBounds.MinimumX;
            }

            if (faceBounds.MaximumX > gameObject.BoundsMaxX)
            {
                gameObject.BoundsMaxX = faceBounds.MaximumX;
            }

            if (faceBounds.MinimumY < gameObject.BoundsMinY)
            {
                gameObject.BoundsMinY = faceBounds.MinimumY;
            }

            if (faceBounds.MaximumY > gameObject.BoundsMaxY)
            {
                gameObject.BoundsMaxY = faceBounds.MaximumY;
            }

            if (faceBounds.MinimumZ < gameObject.BoundsMinZ)
            {
                gameObject.BoundsMinZ = faceBounds.MinimumZ;
            }

            if (faceBounds.MaximumZ > gameObject.BoundsMaxZ)
            {
                gameObject.BoundsMaxZ = faceBounds.MaximumZ;
            }
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
            int[] faceVerts = gameObject.FaceVertexIndices[faceIndex];

            for (int vertexPosition = 0; vertexPosition < faceVertexCount; vertexPosition += 1)
            {
                int vertexIndex = faceVerts[vertexPosition];
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
            int[] faceVerts = gameObject.FaceVertexIndices[faceIndex];
            int firstVertX = gameObject.WorldVertX[faceVerts[0]];
            int firstVertY = gameObject.WorldVertY[faceVerts[0]];
            int firstVertZ = gameObject.WorldVertZ[faceVerts[0]];
            int edge1X = gameObject.WorldVertX[faceVerts[1]] - firstVertX;
            int edge1Y = gameObject.WorldVertY[faceVerts[1]] - firstVertY;
            int edge1Z = gameObject.WorldVertZ[faceVerts[1]] - firstVertZ;
            int edge2X = gameObject.WorldVertX[faceVerts[2]] - firstVertX;
            int edge2Y = gameObject.WorldVertY[faceVerts[2]] - firstVertY;
            int edge2Z = gameObject.WorldVertZ[faceVerts[2]] - firstVertZ;

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
