namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class ProjectedPolygonBuilder
    {
        private static int LineSegmentHalfWidth => 20;

        internal static ProjectedPolygon Build(
            GameObject gameObject,
            int[] faceVertices,
            int vertexCount)
        {
            if (vertexCount == 2)
            {
                int firstVertex = faceVertices[0];
                int secondVertex = faceVertices[1];
                int[] x = new int[4];
                int[] y = new int[4];
                x[0] = gameObject.ProjectedU[firstVertex] - LineSegmentHalfWidth;
                x[1] = gameObject.ProjectedU[secondVertex] - LineSegmentHalfWidth;
                x[2] = gameObject.ProjectedU[secondVertex] + LineSegmentHalfWidth;
                x[3] = gameObject.ProjectedU[firstVertex] + LineSegmentHalfWidth;
                y[0] = y[3] = gameObject.ProjectedV[firstVertex];
                y[1] = y[2] = gameObject.ProjectedV[secondVertex];

                return new ProjectedPolygon(x, y);
            }

            int[] polygonX = new int[vertexCount];
            int[] polygonY = new int[vertexCount];

            for (int vertexIndex = 0;
                vertexIndex < vertexCount;
                vertexIndex += 1)
            {
                int vertex = faceVertices[vertexIndex];
                polygonX[vertexIndex] = gameObject.ProjectedU[vertex];
                polygonY[vertexIndex] = gameObject.ProjectedV[vertex];
            }

            return new ProjectedPolygon(polygonX, polygonY);
        }
    }
}