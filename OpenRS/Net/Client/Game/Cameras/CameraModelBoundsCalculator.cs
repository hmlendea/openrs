using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraModelBoundsCalculator
    {
        private static int FaceNormalNormaliseThreshold => 25000;

        private static int UncomputedFaceNormal => -1;

        private static int SpriteBoundsHorizontalPadding => 20;

        private static int SpriteFrontFacingNormal => 1;

        internal static void UpdateModelAtIndex(
            CameraModel[] visibleModels,
            int modelIndex,
            int lightingFactor)
        {
            CameraModel cameraModel = visibleModels[modelIndex];
            GameObject gameObject = cameraModel.Object;

            int faceIndex = cameraModel.faceVertCountIndex1;
            int[] faceVertices = gameObject.face_vertices[faceIndex];
            int faceVertCount = gameObject.face_vertices_count[faceIndex];
            int normaliseScale = gameObject.faceRenderFlag[faceIndex];
            int vert0X = gameObject.projectedX[faceVertices[0]];
            int vert0Y = gameObject.projectedY[faceVertices[0]];
            int vert0Depth = gameObject.projectedDepth[faceVertices[0]];
            int edge1X = gameObject.projectedX[faceVertices[1]] - vert0X;
            int edge1Y = gameObject.projectedY[faceVertices[1]] - vert0Y;
            int edge1Depth = gameObject.projectedDepth[faceVertices[1]] - vert0Depth;
            int edge2X = gameObject.projectedX[faceVertices[2]] - vert0X;
            int edge2Y = gameObject.projectedY[faceVertices[2]] - vert0Y;
            int edge2Depth = gameObject.projectedDepth[faceVertices[2]] - vert0Depth;
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

                gameObject.faceRenderFlag[faceIndex] = normaliseScale;

                double normalMagnitude = Math.Sqrt(
                    normalX * normalX +
                    normalY * normalY +
                    normalZ * normalZ);
                gameObject.faceVisibility[faceIndex] = (int)(lightingFactor * normalMagnitude);
            }
            else
            {
                normalX >>= normaliseScale;
                normalY >>= normaliseScale;
                normalZ >>= normaliseScale;
            }

            cameraModel.visibilityDot =
                vert0X * normalX +
                vert0Y * normalY +
                vert0Depth * normalZ;
            cameraModel.normalX = normalX;
            cameraModel.normalY = normalY;
            cameraModel.normalZ = normalZ;

            ApplyPolygonBounds(cameraModel, gameObject, faceVertices, faceVertCount);
        }

        internal static void RemoveModelAtIndex(CameraModel[] visibleModels, int modelIndex)
        {
            CameraModel cameraModel = visibleModels[modelIndex];
            GameObject gameObject = cameraModel.Object;
            int faceIndex = cameraModel.faceVertCountIndex1;
            int[] faceVertices = gameObject.face_vertices[faceIndex];
            int vert0Depth = gameObject.projectedDepth[faceVertices[0]];
            gameObject.faceVisibility[faceIndex] = SpriteFrontFacingNormal;
            gameObject.faceRenderFlag[faceIndex] = 0;
            cameraModel.visibilityDot = vert0Depth;
            cameraModel.normalX = 0;
            cameraModel.normalY = 0;
            cameraModel.normalZ = SpriteFrontFacingNormal;

            int vert1Depth = gameObject.projectedDepth[faceVertices[1]];
            int depthMin = Math.Min(vert0Depth, vert1Depth);
            int depthMax = Math.Max(vert0Depth, vert1Depth);
            int vert0U = gameObject.projectedU[faceVertices[0]];
            int vert1U = gameObject.projectedU[faceVertices[1]];
            int projUMin = Math.Min(vert0U, vert1U);
            int projUMax = Math.Max(vert0U, vert1U);
            int projVMin = gameObject.projectedV[faceVertices[1]];
            int projVMax = gameObject.projectedV[faceVertices[0]];

            if (projVMin > projVMax)
            {
                projVMax = projVMin;
            }

            cameraModel.boundsMinZ = depthMin;
            cameraModel.boundsMaxZ = depthMax;
            cameraModel.boundsMinX = projUMin - SpriteBoundsHorizontalPadding;
            cameraModel.boundsMaxX = projUMax + SpriteBoundsHorizontalPadding;
            cameraModel.boundsMinY = projVMin;
            cameraModel.boundsMaxY = projVMax;
        }

        private static void ApplyPolygonBounds(
            CameraModel cameraModel,
            GameObject gameObject,
            int[] faceVertices,
            int faceVertCount)
        {
            int depthMin = gameObject.projectedDepth[faceVertices[0]];
            int depthMax = depthMin;
            int projUMin = gameObject.projectedU[faceVertices[0]];
            int projUMax = projUMin;
            int projVMin = gameObject.projectedV[faceVertices[0]];
            int projVMax = projVMin;

            for (int vertLoopIndex = 1; vertLoopIndex < faceVertCount; vertLoopIndex += 1)
            {
                int vertexIndex = faceVertices[vertLoopIndex];
                depthMin = Math.Min(depthMin, gameObject.projectedDepth[vertexIndex]);
                depthMax = Math.Max(depthMax, gameObject.projectedDepth[vertexIndex]);
                projUMin = Math.Min(projUMin, gameObject.projectedU[vertexIndex]);
                projUMax = Math.Max(projUMax, gameObject.projectedU[vertexIndex]);
                projVMin = Math.Min(projVMin, gameObject.projectedV[vertexIndex]);
                projVMax = Math.Max(projVMax, gameObject.projectedV[vertexIndex]);
            }

            cameraModel.boundsMinZ = depthMin;
            cameraModel.boundsMaxZ = depthMax;
            cameraModel.boundsMinX = projUMin;
            cameraModel.boundsMaxX = projUMax;
            cameraModel.boundsMinY = projVMin;
            cameraModel.boundsMaxY = projVMax;
        }
    }
}
