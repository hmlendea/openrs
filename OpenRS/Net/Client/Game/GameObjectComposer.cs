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

        internal static void CopyPolygonData(
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
                destination.VertexColour[destinationVertexIndex] = source.VertexColour[sourceVertexIndex];
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

        internal static GameObject[] SplitByArea(
            GameObject source,
            int x,
            int y,
            int width,
            int height,
            int chunkSize,
            int chunkCount,
            int maxVertexCount,
            bool applyLighting)
        {
            source.ResetWorldTransform();

            int[] chunkVertexTotals = new int[chunkCount];
            int[] chunkFaceTotals = new int[chunkCount];
            CountFacesPerChunk(source, width, height, chunkSize, chunkVertexTotals, chunkFaceTotals);

            GameObject[] chunkObjects = BuildChunkObjects(
                source,
                chunkCount,
                maxVertexCount,
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
                int chunkIndex = ComputeFaceChunkIndex(source, faceIndex, width, height, chunkSize);
                chunkVertexTotals[chunkIndex] += source.FaceVertexCounts[faceIndex];
                chunkFaceTotals[chunkIndex] += 1;
            }
        }

        private static GameObject[] BuildChunkObjects(
            GameObject source,
            int chunkCount,
            int maxVertexCount,
            int[] chunkVertexTotals,
            int[] chunkFaceTotals,
            bool applyLighting)
        {
            GameObject[] chunkObjects = new GameObject[chunkCount];

            for (int chunkIndex = 0; chunkIndex < chunkCount; chunkIndex += 1)
            {
                if (chunkVertexTotals[chunkIndex] > maxVertexCount)
                {
                    chunkVertexTotals[chunkIndex] = maxVertexCount;
                }

                chunkObjects[chunkIndex] = new GameObject(
                    chunkVertexTotals[chunkIndex],
                    chunkFaceTotals[chunkIndex],
                    true, true, true, applyLighting, true)
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
                int chunkIndex = ComputeFaceChunkIndex(source, faceIndex, width, height, chunkSize);
                int[] vertexIndices = source.FaceVertexIndices[faceIndex];
                int vertexCount = source.FaceVertexCounts[faceIndex];
                CopyPolygonData(
                    chunkObjects[chunkIndex],
                    source,
                    vertexIndices,
                    vertexCount,
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

            return sumX / (vertexCount * width) + sumZ / (vertexCount * height) * chunkSize;
        }
    }
}
