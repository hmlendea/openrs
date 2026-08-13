namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectComposer
    {
        internal static void BuildComposite(
            GameObject target,
            GameObject[] childObjects,
            int childCount,
            bool applyLighting)
        {
            int totalFaceCount = 0;
            int totalVertexCount = 0;

            for (int childIndex = 0; childIndex < childCount; childIndex += 1)
            {
                totalFaceCount += childObjects[childIndex].FaceCount;
                totalVertexCount += childObjects[childIndex].VertexCount;
            }

            target.InitialiseArrays(totalVertexCount, totalFaceCount);

            if (applyLighting)
            {
                target.polygonGroupMapping = new int[totalFaceCount][];
            }

            for (int childIndex = 0; childIndex < childCount; childIndex += 1)
            {
                CopyChildObject(target, childObjects, childIndex, childCount, applyLighting);
            }

            target.ObjectState = 1;
        }

        private static void CopyChildObject(
            GameObject target,
            GameObject[] childObjects,
            int childIndex,
            int totalChildCount,
            bool applyLighting)
        {
            GameObject childObject = childObjects[childIndex];
            childObject.ResetWorldTransform();
            target.BaseShadeLevel = childObject.BaseShadeLevel;
            target.AmbientLightLevel = childObject.AmbientLightLevel;
            target.LightDirectionX = childObject.LightDirectionX;
            target.LightDirectionY = childObject.LightDirectionY;
            target.LightDirectionZ = childObject.LightDirectionZ;
            target.LightMagnitude = childObject.LightMagnitude;

            for (int faceIndex = 0; faceIndex < childObject.FaceCount; faceIndex += 1)
            {
                CopyChildFace(
                    target,
                    childObject,
                    childIndex,
                    faceIndex,
                    totalChildCount,
                    applyLighting);
            }
        }

        private static void CopyChildFace(
            GameObject target,
            GameObject childObject,
            int childIndex,
            int faceIndex,
            int totalChildCount,
            bool applyLighting)
        {
            int vertexCountForFace = childObject.FaceVertexCounts[faceIndex];
            int[] sourceVertexIndices = childObject.FaceVertexIndices[faceIndex];
            int[] mappedVertexIndices = new int[vertexCountForFace];

            for (int vertexPosition = 0; vertexPosition < vertexCountForFace; vertexPosition += 1)
            {
                int sourceVertex = sourceVertexIndices[vertexPosition];
                mappedVertexIndices[vertexPosition] = target.GetVertexIndex(
                    childObject.VertexCoordinatesX[sourceVertex],
                    childObject.VertexCoordinatesY[sourceVertex],
                    childObject.VertexCoordinatesZ[sourceVertex]);
            }

            int newFaceIndex = target.AddFaceVertices(
                vertexCountForFace,
                mappedVertexIndices,
                childObject.TextureBack[faceIndex],
                childObject.TextureFront[faceIndex]);

            target.GouraudShade[newFaceIndex] = childObject.GouraudShade[faceIndex];
            target.FaceRenderFlag[newFaceIndex] = childObject.FaceRenderFlag[faceIndex];
            target.FaceVisibility[newFaceIndex] = childObject.FaceVisibility[faceIndex];

            if (applyLighting)
            {
                AssignPolygonGroupMapping(
                    target,
                    childObject,
                    newFaceIndex,
                    faceIndex,
                    childIndex,
                    totalChildCount);
            }
        }

        private static void AssignPolygonGroupMapping(
            GameObject target,
            GameObject childObject,
            int targetFaceIndex,
            int sourceFaceIndex,
            int childIndex,
            int totalChildCount)
        {
            int sourceGroupLength = childObject.polygonGroupMapping[sourceFaceIndex].Length;

            if (totalChildCount > 1)
            {
                target.polygonGroupMapping[targetFaceIndex] = new int[sourceGroupLength + 1];
                target.polygonGroupMapping[targetFaceIndex][0] = childIndex;

                for (int groupIndex = 0; groupIndex < sourceGroupLength; groupIndex += 1)
                {
                    target.polygonGroupMapping[targetFaceIndex][groupIndex + 1] =
                        childObject.polygonGroupMapping[sourceFaceIndex][groupIndex];
                }
            }
            else
            {
                target.polygonGroupMapping[targetFaceIndex] = new int[sourceGroupLength];

                for (int groupIndex = 0; groupIndex < sourceGroupLength; groupIndex += 1)
                {
                    target.polygonGroupMapping[targetFaceIndex][groupIndex] =
                        childObject.polygonGroupMapping[sourceFaceIndex][groupIndex];
                }
            }
        }

    }
}
