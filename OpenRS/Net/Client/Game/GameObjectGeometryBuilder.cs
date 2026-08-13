namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectGeometryBuilder
    {
        internal static void ResetProjectionArrays(GameObject gameObject)
        {
            gameObject.ProjectedX = new int[gameObject.VertexCount];
            gameObject.ProjectedY = new int[gameObject.VertexCount];
            gameObject.ProjectedDepth = new int[gameObject.VertexCount];
            gameObject.ProjectedU = new int[gameObject.VertexCount];
            gameObject.ProjectedV = new int[gameObject.VertexCount];
        }

        internal static void ResetObjectIndexes(GameObject gameObject)
        {
            gameObject.FaceCount = 0;
            gameObject.VertexCount = 0;
        }

        internal static void RemoveLastGroup(
            GameObject gameObject,
            int faceDecrement,
            int vertexDecrement)
        {
            gameObject.FaceCount -= faceDecrement;

            if (gameObject.FaceCount < 0)
            {
                gameObject.FaceCount = 0;
            }

            gameObject.VertexCount -= vertexDecrement;

            if (gameObject.VertexCount < 0)
            {
                gameObject.VertexCount = 0;
            }
        }

        internal static int GetOrAddVertex(GameObject gameObject, int x, int y, int z)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                if (gameObject.VertexCoordinatesX[vertexIndex] == x &&
                    gameObject.VertexCoordinatesY[vertexIndex] == y &&
                    gameObject.VertexCoordinatesZ[vertexIndex] == z)
                {
                    return vertexIndex;
                }
            }

            return AddVertex(gameObject, x, y, z);
        }

        internal static int AddVertex(GameObject gameObject, int x, int y, int z)
        {
            if (gameObject.VertexCount >= gameObject.TotalVertexCapacity)
            {
                return -1;
            }

            gameObject.VertexCoordinatesX[gameObject.VertexCount] = x;
            gameObject.VertexCoordinatesY[gameObject.VertexCount] = y;
            gameObject.VertexCoordinatesZ[gameObject.VertexCount] = z;
            gameObject.VertexCount += 1;

            return gameObject.VertexCount - 1;
        }

        internal static int AddFace(
            GameObject gameObject,
            int vertexCountForFace,
            int[] faceVertices,
            int faceBack,
            int faceFront)
        {
            if (gameObject.FaceCount >= gameObject.TotalFaceCapacity)
            {
                return -1;
            }

            gameObject.FaceVertexCounts[gameObject.FaceCount] = vertexCountForFace;
            gameObject.FaceVertexIndices[gameObject.FaceCount] = faceVertices;
            gameObject.TextureBack[gameObject.FaceCount] = faceBack;
            gameObject.TextureFront[gameObject.FaceCount] = faceFront;
            gameObject.ObjectState = GameObject.ObjectStateRequiresTransform;
            gameObject.FaceCount += 1;

            return gameObject.FaceCount - 1;
        }

        internal static void SetVertexColour(GameObject gameObject, int vertexIndex, int value)
            => gameObject.VertexColour[vertexIndex] = value;
    }
}