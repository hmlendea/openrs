namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectPolygonCopier
    {
        internal static void Copy(
            GameObject destination,
            GameObject source,
            int[] vertexIndices,
            int vertexCount,
            int polygonIndex)
        {
            int[] mappedVertexIndices = new int[vertexCount];

            for (int vertexPosition = 0; vertexPosition < vertexCount; vertexPosition += 1)
            {
                int sourceVertexIndex = vertexIndices[vertexPosition];
                int destinationVertexIndex = destination.GetVertexIndex(
                    source.VertexCoordinatesX[sourceVertexIndex],
                    source.VertexCoordinatesY[sourceVertexIndex],
                    source.VertexCoordinatesZ[sourceVertexIndex]);
                mappedVertexIndices[vertexPosition] = destinationVertexIndex;
                destination.FaceNormalComponent[destinationVertexIndex] =
                    source.FaceNormalComponent[sourceVertexIndex];
                destination.VertexColour[destinationVertexIndex] =
                    source.VertexColour[sourceVertexIndex];
            }

            int newFaceIndex = destination.AddFaceVertices(
                vertexCount,
                mappedVertexIndices,
                source.TextureBack[polygonIndex],
                source.TextureFront[polygonIndex]);

            if (!destination.DoesShareEntityArrays && !source.DoesShareEntityArrays)
            {
                destination.EntityType[newFaceIndex] = source.EntityType[polygonIndex];
            }

            destination.GouraudShade[newFaceIndex] = source.GouraudShade[polygonIndex];
            destination.FaceRenderFlag[newFaceIndex] = source.FaceRenderFlag[polygonIndex];
            destination.FaceVisibility[newFaceIndex] = source.FaceVisibility[polygonIndex];
        }
    }
}