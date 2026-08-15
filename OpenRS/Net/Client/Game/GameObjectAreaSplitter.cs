namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectAreaSplitter
    {
        internal static GameObject[] Split(
            GameObject source,
            int width,
            int height,
            int chunkSize,
            int chunkCount,
            int maximumVertexCount,
            bool applyLighting)
        {
            source.ResetWorldTransform();
            int[] chunkVertexTotals = new int[chunkCount];
            int[] chunkFaceTotals = new int[chunkCount];
            CountFacesPerChunk(
                source,
                width,
                height,
                chunkSize,
                chunkVertexTotals,
                chunkFaceTotals);
            GameObject[] chunkObjects = BuildChunkObjects(
                source,
                chunkCount,
                maximumVertexCount,
                chunkVertexTotals,
                chunkFaceTotals,
                applyLighting);
            AssignFacesToChunks(source, chunkObjects, width, height, chunkSize);

            for (int chunkIndex = 0; chunkIndex < chunkCount; chunkIndex += 1)
            {
                chunkObjects[chunkIndex].ResetVertexNormals();
            }

            return chunkObjects;
        }

        private static void CountFacesPerChunk(
            GameObject source,
            int width,
            int height,
            int chunkSize,
            int[] chunkVertexTotals,
            int[] chunkFaceTotals)
        {
            for (int faceIndex = 0; faceIndex < source.FaceCount; faceIndex += 1)
            {
                int chunkIndex = ComputeFaceChunkIndex(
                    source,
                    faceIndex,
                    width,
                    height,
                    chunkSize);
                chunkVertexTotals[chunkIndex] += source.FaceVertexCounts[faceIndex];
                chunkFaceTotals[chunkIndex] += 1;
            }
        }

        private static GameObject[] BuildChunkObjects(
            GameObject source,
            int chunkCount,
            int maximumVertexCount,
            int[] chunkVertexTotals,
            int[] chunkFaceTotals,
            bool applyLighting)
        {
            GameObject[] chunkObjects = new GameObject[chunkCount];

            for (int chunkIndex = 0; chunkIndex < chunkCount; chunkIndex += 1)
            {
                if (chunkVertexTotals[chunkIndex] > maximumVertexCount)
                {
                    chunkVertexTotals[chunkIndex] = maximumVertexCount;
                }

                chunkObjects[chunkIndex] = new GameObject(
                    chunkVertexTotals[chunkIndex],
                    chunkFaceTotals[chunkIndex],
                    true,
                    true,
                    true,
                    applyLighting,
                    true)
                {
                    AmbientLightLevel = source.AmbientLightLevel,
                    BaseShadeLevel = source.BaseShadeLevel
                };
            }

            return chunkObjects;
        }

        private static void AssignFacesToChunks(
            GameObject source,
            GameObject[] chunkObjects,
            int width,
            int height,
            int chunkSize)
        {
            for (int faceIndex = 0; faceIndex < source.FaceCount; faceIndex += 1)
            {
                int chunkIndex = ComputeFaceChunkIndex(
                    source,
                    faceIndex,
                    width,
                    height,
                    chunkSize);
                GameObjectPolygonCopier.Copy(
                    chunkObjects[chunkIndex],
                    source,
                    source.FaceVertexIndices[faceIndex],
                    source.FaceVertexCounts[faceIndex],
                    faceIndex);
            }
        }

        private static int ComputeFaceChunkIndex(
            GameObject source,
            int faceIndex,
            int width,
            int height,
            int chunkSize)
        {
            int vertexCount = source.FaceVertexCounts[faceIndex];
            int[] vertexIndices = source.FaceVertexIndices[faceIndex];
            int sumX = 0;
            int sumZ = 0;

            for (int vertexPosition = 0; vertexPosition < vertexCount; vertexPosition += 1)
            {
                sumX += source.VertexCoordinatesX[vertexIndices[vertexPosition]];
                sumZ += source.VertexCoordinatesZ[vertexIndices[vertexPosition]];
            }

            return
                sumX / (vertexCount * width) +
                sumZ / (vertexCount * height) * chunkSize;
        }
    }
}