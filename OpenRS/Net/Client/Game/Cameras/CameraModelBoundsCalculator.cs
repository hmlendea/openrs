using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraModelBoundsCalculator
    {
        private static int FaceNormalNormaliseThreshold => 25000;

        private static int UncomputedFaceNormal => -1;

        private static int SpriteBoundsHorizontalPadding => 20;

        private static int SpriteFrontFacingNormal => 1;

        internal static void ComputePolygonBounds(CameraModel cameraModel, int lightingFactor)
        {
            GameObject gameObject = cameraModel.SourceObject;
            int faceIndex = cameraModel.FaceIndex;
            int[] faceVertices = gameObject.FaceVertexIndices[faceIndex];
            int faceVertCount = gameObject.FaceVertexCounts[faceIndex];
            int normaliseScale = gameObject.FaceRenderFlag[faceIndex];
            int vert0X = gameObject.ProjectedX[faceVertices[0]];
            int vert0Y = gameObject.ProjectedY[faceVertices[0]];
            int vert0Depth = gameObject.ProjectedDepth[faceVertices[0]];
            int edge1X = gameObject.ProjectedX[faceVertices[1]] - vert0X;
            int edge1Y = gameObject.ProjectedY[faceVertices[1]] - vert0Y;
            int edge1Depth = gameObject.ProjectedDepth[faceVertices[1]] - vert0Depth;
            int edge2X = gameObject.ProjectedX[faceVertices[2]] - vert0X;
            int edge2Y = gameObject.ProjectedY[faceVertices[2]] - vert0Y;
            int edge2Depth = gameObject.ProjectedDepth[faceVertices[2]] - vert0Depth;
            int normalX = edge1Y * edge2Depth - edge2Y * edge1Depth;
            int normalY = edge1Depth * edge2X - edge2Depth * edge1X;
            int normalZ = edge1X * edge2Y - edge2X * edge1Y;

            if (normaliseScale == UncomputedFaceNormal)
            {
                normaliseScale = 0;

                while (normalX > FaceNormalNormaliseThreshold ||
                    normalY > FaceNormalNormaliseThreshold ||
                    normalZ > FaceNormalNormaliseThreshold ||
                    normalX < -FaceNormalNormaliseThreshold ||
                    normalY < -FaceNormalNormaliseThreshold ||
                    normalZ < -FaceNormalNormaliseThreshold)
                {
                    normaliseScale += 1;
                    normalX >>= 1;
                    normalY >>= 1;
                    normalZ >>= 1;
                }

                gameObject.FaceRenderFlag[faceIndex] = normaliseScale;

                double normalMagnitude = Math.Sqrt(
                    normalX * normalX +
                    normalY * normalY +
                    normalZ * normalZ);
                gameObject.FaceVisibility[faceIndex] = (int)(lightingFactor * normalMagnitude);
            }
            else
            {
                normalX >>= normaliseScale;
                normalY >>= normaliseScale;
                normalZ >>= normaliseScale;
            }

            cameraModel.VisibilityDot =
                vert0X * normalX +
                vert0Y * normalY +
                vert0Depth * normalZ;
            cameraModel.NormalX = normalX;
            cameraModel.NormalY = normalY;
            cameraModel.NormalZ = normalZ;

            ApplyPolygonBounds(cameraModel, gameObject, faceVertices, faceVertCount);
        }

        internal static void ComputeSpriteBounds(CameraModel cameraModel)
        {
            GameObject gameObject = cameraModel.SourceObject;
            int faceIndex = cameraModel.FaceIndex;
            int[] faceVertices = gameObject.FaceVertexIndices[faceIndex];
            int vert0Depth = gameObject.ProjectedDepth[faceVertices[0]];
            gameObject.FaceVisibility[faceIndex] = SpriteFrontFacingNormal;
            gameObject.FaceRenderFlag[faceIndex] = 0;
            cameraModel.VisibilityDot = vert0Depth;
            cameraModel.NormalX = 0;
            cameraModel.NormalY = 0;
            cameraModel.NormalZ = SpriteFrontFacingNormal;

            int vert1Depth = gameObject.ProjectedDepth[faceVertices[1]];
            int depthMin = Math.Min(vert0Depth, vert1Depth);
            int depthMax = Math.Max(vert0Depth, vert1Depth);
            int vert0U = gameObject.ProjectedU[faceVertices[0]];
            int vert1U = gameObject.ProjectedU[faceVertices[1]];
            int projUMin = Math.Min(vert0U, vert1U);
            int projUMax = Math.Max(vert0U, vert1U);
            int projVMin = gameObject.ProjectedV[faceVertices[1]];
            int projVMax = gameObject.ProjectedV[faceVertices[0]];

            if (projVMin > projVMax)
            {
                projVMax = projVMin;
            }

            cameraModel.BoundsMinZ = depthMin;
            cameraModel.BoundsMaxZ = depthMax;
            cameraModel.BoundsMinX = projUMin - SpriteBoundsHorizontalPadding;
            cameraModel.BoundsMaxX = projUMax + SpriteBoundsHorizontalPadding;
            cameraModel.BoundsMinY = projVMin;
            cameraModel.BoundsMaxY = projVMax;
        }

        private static void ApplyPolygonBounds(
            CameraModel cameraModel,
            GameObject gameObject,
            int[] faceVertices,
            int faceVertCount)
        {
            int depthMin = gameObject.ProjectedDepth[faceVertices[0]];
            int depthMax = depthMin;
            int projUMin = gameObject.ProjectedU[faceVertices[0]];
            int projUMax = projUMin;
            int projVMin = gameObject.ProjectedV[faceVertices[0]];
            int projVMax = projVMin;

            for (int vertLoopIndex = 1; vertLoopIndex < faceVertCount; vertLoopIndex += 1)
            {
                int vertexIndex = faceVertices[vertLoopIndex];
                depthMin = Math.Min(depthMin, gameObject.ProjectedDepth[vertexIndex]);
                depthMax = Math.Max(depthMax, gameObject.ProjectedDepth[vertexIndex]);
                projUMin = Math.Min(projUMin, gameObject.ProjectedU[vertexIndex]);
                projUMax = Math.Max(projUMax, gameObject.ProjectedU[vertexIndex]);
                projVMin = Math.Min(projVMin, gameObject.ProjectedV[vertexIndex]);
                projVMax = Math.Max(projVMax, gameObject.ProjectedV[vertexIndex]);
            }

            cameraModel.BoundsMinZ = depthMin;
            cameraModel.BoundsMaxZ = depthMax;
            cameraModel.BoundsMinX = projUMin;
            cameraModel.BoundsMaxX = projUMax;
            cameraModel.BoundsMinY = projVMin;
            cameraModel.BoundsMaxY = projVMax;
        }
    }
}
