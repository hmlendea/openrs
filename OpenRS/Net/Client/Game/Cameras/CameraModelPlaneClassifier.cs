namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraModelPlaneClassifier
    {
        internal static bool HasAnyVertexOutside(
            CameraModel sourceModel,
            CameraModel referenceModel,
            bool isForwardCheck)
        {
            GameObject sourceObject = sourceModel.SourceObject;
            int sourceFaceIndex = sourceModel.FaceIndex;
            int[] sourceFaceVertices =
                sourceObject.FaceVertexIndices[sourceFaceIndex];
            int sourceVertexCount =
                sourceObject.FaceVertexCounts[sourceFaceIndex];

            GameObject referenceObject = referenceModel.SourceObject;
            int referenceFaceIndex = referenceModel.FaceIndex;
            int referenceFirstVertex =
                referenceObject.FaceVertexIndices[referenceFaceIndex][0];
            int referenceX = referenceObject.ProjectedX[referenceFirstVertex];
            int referenceY = referenceObject.ProjectedY[referenceFirstVertex];
            int referenceDepth =
                referenceObject.ProjectedDepth[referenceFirstVertex];
            int planeNormalX = referenceModel.NormalX;
            int planeNormalY = referenceModel.NormalY;
            int planeNormalZ = referenceModel.NormalZ;
            int visibilityRange =
                referenceObject.FaceVisibility[referenceFaceIndex];
            int facingDot = isForwardCheck
                ? referenceModel.VisibilityDot
                : -referenceModel.VisibilityDot;

            for (int vertexIndex = 0;
                vertexIndex < sourceVertexCount;
                vertexIndex += 1)
            {
                int vertex = sourceFaceVertices[vertexIndex];
                int dotProduct =
                    (referenceX - sourceObject.ProjectedX[vertex]) * planeNormalX +
                    (referenceY - sourceObject.ProjectedY[vertex]) * planeNormalY +
                    (referenceDepth - sourceObject.ProjectedDepth[vertex]) * planeNormalZ;

                if ((dotProduct >= -visibilityRange || facingDot >= 0) &&
                    (dotProduct <= visibilityRange || facingDot <= 0))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}