namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectBoundsCalculator
    {
        private static int BoundsExtremePositive => 0xf423f;

        internal static void Calculate(GameObject gameObject)
        {
            ResetBoundsToExtremes(gameObject);

            for (int faceIndex = 0; faceIndex < gameObject.FaceCount; faceIndex += 1)
            {
                FaceBoundsData faceBounds = ComputeFaceBounds(gameObject, faceIndex);
                UpdateColliderBounds(gameObject, faceIndex, faceBounds);
                UpdateGlobalBounds(gameObject, faceBounds);
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
            int[] faceVertices = gameObject.FaceVertexIndices[faceIndex];
            int firstVertex = faceVertices[0];
            int vertexCount = gameObject.FaceVertexCounts[faceIndex];
            FaceBoundsData bounds = new()
            {
                MinimumX = gameObject.WorldVertX[firstVertex],
                MaximumX = gameObject.WorldVertX[firstVertex],
                MinimumY = gameObject.WorldVertY[firstVertex],
                MaximumY = gameObject.WorldVertY[firstVertex],
                MinimumZ = gameObject.WorldVertZ[firstVertex],
                MaximumZ = gameObject.WorldVertZ[firstVertex]
            };

            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex += 1)
            {
                UpdateAxisBounds(gameObject, faceVertices[vertexIndex], bounds);
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
    }
}