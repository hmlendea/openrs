namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class PolygonIntersectionCalculator
    {
        private static int LineSegmentHalfWidth => 20;

        internal static bool AreBoundsDisjoint(CameraModel modelA, CameraModel modelB)
        {
            if (modelA.BoundsMinX >= modelB.BoundsMaxX)
            {
                return true;
            }

            if (modelB.BoundsMinX >= modelA.BoundsMaxX)
            {
                return true;
            }

            if (modelA.BoundsMinY >= modelB.BoundsMaxY)
            {
                return true;
            }

            if (modelB.BoundsMinY >= modelA.BoundsMaxY)
            {
                return true;
            }

            if (modelA.BoundsMinZ >= modelB.BoundsMaxZ)
            {
                return true;
            }

            if (modelB.BoundsMinZ > modelA.BoundsMaxZ)
            {
                return false;
            }

            if (!IsAnyVertexOutsidePlane(modelA, modelB, isForwardCheck: true))
            {
                return true;
            }

            if (!IsAnyVertexOutsidePlane(modelB, modelA, isForwardCheck: false))
            {
                return true;
            }

            GameObject objectA = modelA.SourceObject;
            int faceIndexA = modelA.FaceIndex;
            int[] faceVerticesA = objectA.FaceVertexIndices[faceIndexA];
            int vertCountA = objectA.FaceVertexCounts[faceIndexA];

            GameObject objectB = modelB.SourceObject;
            int faceIndexB = modelB.FaceIndex;
            int[] faceVerticesB = objectB.FaceVertexIndices[faceIndexB];
            int vertCountB = objectB.FaceVertexCounts[faceIndexB];

            ProjectedPolygon polygonA = BuildProjectedPolygon(objectA, faceVerticesA, vertCountA);
            ProjectedPolygon polygonB = BuildProjectedPolygon(objectB, faceVerticesB, vertCountB);

            return !PolygonsIntersect(polygonA.X, polygonA.Y, polygonB.X, polygonB.Y);
        }

        internal static bool IsModelBehind(CameraModel frontModel, CameraModel behindModel) =>
            !IsAnyVertexOutsidePlane(frontModel, behindModel, isForwardCheck: true) ||
            !IsAnyVertexOutsidePlane(behindModel, frontModel, isForwardCheck: false);

        private static int LinearInterpolate(int valueA, int rangeStartY, int valueB, int rangeEndY, int targetY)
        {
            if (rangeEndY == rangeStartY)
            {
                return valueA;
            }

            return valueA + (valueB - valueA) * (targetY - rangeStartY) / (rangeEndY - rangeStartY);
        }

        private static bool ComparePolygonRanges(int valueA, int valueB, int rangeMin, int rangeMax, bool isAscending)
        {
            if (isAscending && valueA <= rangeMin || valueA < rangeMin)
            {
                if (valueA > rangeMax)
                {
                    return true;
                }

                if (valueB > rangeMin)
                {
                    return true;
                }

                if (valueB > rangeMax)
                {
                    return true;
                }

                return !isAscending;
            }

            if (valueA < rangeMax)
            {
                return true;
            }

            if (valueB < rangeMin)
            {
                return true;
            }

            if (valueB < rangeMax)
            {
                return true;
            }
            else
            {
                return isAscending;
            }
        }

        private static bool ComparePolygonRange(int valueA, int valueB, int rangeLimit, bool isAscending)
        {
            if (isAscending && valueA <= rangeLimit || valueA < rangeLimit)
            {
                if (valueB > rangeLimit)
                {
                    return true;
                }

                return !isAscending;
            }

            if (valueB < rangeLimit)
            {
                return true;
            }

            return isAscending;
        }

        private static bool PolygonsIntersect(
            int[] polygonAX,
            int[] polygonAY,
            int[] polygonBX,
            int[] polygonBY)
        {
            int polygonAVertCount = polygonAX.Length;
            int polygonBVertCount = polygonBX.Length;
            PolygonSweepState sweepState = PolygonSweepState.BothChainsActive;
            int minY;
            int maxY = minY = polygonAY[0];
            int minYIndexA = 0;
            int minYB;
            int maxYB = minYB = polygonBY[0];
            int minYIndexB = 0;

            for (int loopA = 1; loopA < polygonAVertCount; loopA += 1)
            {
                if (polygonAY[loopA] < minY)
                {
                    minY = polygonAY[loopA];
                    minYIndexA = loopA;
                }
                else if (polygonAY[loopA] > maxY)
                {
                    maxY = polygonAY[loopA];
                }
            }

            for (int loopB = 1; loopB < polygonBVertCount; loopB += 1)
            {
                if (polygonBY[loopB] < minYB)
                {
                    minYB = polygonBY[loopB];
                    minYIndexB = loopB;
                }
                else if (polygonBY[loopB] > maxYB)
                {
                    maxYB = polygonBY[loopB];
                }
            }

            if (minYB >= maxY)
            {
                return false;
            }

            if (minY >= maxYB)
            {
                return false;
            }

            int rightIndexA;
            int rightIndexB;
            bool isIntersecting;

            if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
            {
                rightIndexA = minYIndexA;

                while (polygonAY[rightIndexA] < polygonBY[minYIndexB])
                {
                    rightIndexA = (rightIndexA + 1) % polygonAVertCount;
                }

                while (polygonAY[minYIndexA] < polygonBY[minYIndexB])
                {
                    minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;
                }

                int leftEdgeAX = LinearInterpolate(
                    polygonAX[(minYIndexA + 1) % polygonAVertCount],
                    polygonAY[(minYIndexA + 1) % polygonAVertCount],
                    polygonAX[minYIndexA],
                    polygonAY[minYIndexA],
                    polygonBY[minYIndexB]);
                int rightEdgeAX = LinearInterpolate(
                    polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                    polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                    polygonAX[rightIndexA],
                    polygonAY[rightIndexA],
                    polygonBY[minYIndexB]);
                int leftVertexBX = polygonBX[minYIndexB];
                isIntersecting = (leftEdgeAX < leftVertexBX) | (rightEdgeAX < leftVertexBX);

                if (ComparePolygonRange(leftEdgeAX, rightEdgeAX, leftVertexBX, isIntersecting))
                {
                    return true;
                }

                rightIndexB = (minYIndexB + 1) % polygonBVertCount;
                minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;

                if (minYIndexA == rightIndexA)
                {
                    sweepState = PolygonSweepState.PolygonAChainsMerged;
                }
            }
            else
            {
                rightIndexB = minYIndexB;

                while (polygonBY[rightIndexB] < polygonAY[minYIndexA])
                {
                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                }

                while (polygonBY[minYIndexB] < polygonAY[minYIndexA])
                {
                    minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;
                }

                int startVertexAX = polygonAX[minYIndexA];
                int leftEdgeBX = LinearInterpolate(
                    polygonBX[(minYIndexB + 1) % polygonBVertCount],
                    polygonBY[(minYIndexB + 1) % polygonBVertCount],
                    polygonBX[minYIndexB],
                    polygonBY[minYIndexB],
                    polygonAY[minYIndexA]);
                int rightEdgeBX = LinearInterpolate(
                    polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                    polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                    polygonBX[rightIndexB],
                    polygonBY[rightIndexB],
                    polygonAY[minYIndexA]);
                isIntersecting = (startVertexAX < leftEdgeBX) | (startVertexAX < rightEdgeBX);

                if (ComparePolygonRange(leftEdgeBX, rightEdgeBX, startVertexAX, !isIntersecting))
                {
                    return true;
                }

                rightIndexA = (minYIndexA + 1) % polygonAVertCount;
                minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;

                if (minYIndexB == rightIndexB)
                {
                    sweepState = PolygonSweepState.PolygonBChainsMerged;
                }
            }

            while (sweepState == PolygonSweepState.BothChainsActive)
            {
                if (polygonAY[minYIndexA] < polygonAY[rightIndexA])
                {
                    if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
                    {
                        if (polygonAY[minYIndexA] < polygonBY[rightIndexB])
                        {
                            int leftChainAVertexX = polygonAX[minYIndexA];
                            int rightEdgeAX = LinearInterpolate(
                                polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                                polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                                polygonAX[rightIndexA],
                                polygonAY[rightIndexA],
                                polygonAY[minYIndexA]);
                            int leftEdgeBX = LinearInterpolate(
                                polygonBX[(minYIndexB + 1) % polygonBVertCount],
                                polygonBY[(minYIndexB + 1) % polygonBVertCount],
                                polygonBX[minYIndexB],
                                polygonBY[minYIndexB],
                                polygonAY[minYIndexA]);
                            int rightEdgeBX = LinearInterpolate(
                                polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                                polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                                polygonBX[rightIndexB],
                                polygonBY[rightIndexB],
                                polygonAY[minYIndexA]);

                            if (ComparePolygonRanges(
                                leftChainAVertexX,
                                rightEdgeAX,
                                leftEdgeBX,
                                rightEdgeBX,
                                isIntersecting))
                            {
                                return true;
                            }

                            minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;

                            if (minYIndexA == rightIndexA)
                            {
                                sweepState = PolygonSweepState.PolygonAChainsMerged;
                            }
                        }
                        else
                        {
                            int leftEdgeAX = LinearInterpolate(
                                polygonAX[(minYIndexA + 1) % polygonAVertCount],
                                polygonAY[(minYIndexA + 1) % polygonAVertCount],
                                polygonAX[minYIndexA],
                                polygonAY[minYIndexA],
                                polygonBY[rightIndexB]);
                            int rightEdgeAX = LinearInterpolate(
                                polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                                polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                                polygonAX[rightIndexA],
                                polygonAY[rightIndexA],
                                polygonBY[rightIndexB]);
                            int leftEdgeBX = LinearInterpolate(
                                polygonBX[(minYIndexB + 1) % polygonBVertCount],
                                polygonBY[(minYIndexB + 1) % polygonBVertCount],
                                polygonBX[minYIndexB],
                                polygonBY[minYIndexB],
                                polygonBY[rightIndexB]);
                            int rightChainBVertexX = polygonBX[rightIndexB];

                            if (ComparePolygonRanges(
                                leftEdgeAX,
                                rightEdgeAX,
                                leftEdgeBX,
                                rightChainBVertexX,
                                isIntersecting))
                            {
                                return true;
                            }

                            rightIndexB = (rightIndexB + 1) % polygonBVertCount;

                            if (minYIndexB == rightIndexB)
                            {
                                sweepState = PolygonSweepState.PolygonBChainsMerged;
                            }
                        }
                    }
                    else if (polygonBY[minYIndexB] < polygonBY[rightIndexB])
                    {
                        int leftEdgeAX = LinearInterpolate(
                            polygonAX[(minYIndexA + 1) % polygonAVertCount],
                            polygonAY[(minYIndexA + 1) % polygonAVertCount],
                            polygonAX[minYIndexA],
                            polygonAY[minYIndexA],
                            polygonBY[minYIndexB]);
                        int rightEdgeAX = LinearInterpolate(
                            polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAX[rightIndexA],
                            polygonAY[rightIndexA],
                            polygonBY[minYIndexB]);
                        int leftChainBVertexX = polygonBX[minYIndexB];
                        int rightEdgeBX = LinearInterpolate(
                            polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                            polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                            polygonBX[rightIndexB],
                            polygonBY[rightIndexB],
                            polygonBY[minYIndexB]);

                        if (ComparePolygonRanges(
                            leftEdgeAX,
                            rightEdgeAX,
                            leftChainBVertexX,
                            rightEdgeBX,
                            isIntersecting))
                        {
                            return true;
                        }

                        minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;

                        if (minYIndexB == rightIndexB)
                        {
                            sweepState = PolygonSweepState.PolygonBChainsMerged;
                        }
                    }
                    else
                    {
                        int leftEdgeAX = LinearInterpolate(
                            polygonAX[(minYIndexA + 1) % polygonAVertCount],
                            polygonAY[(minYIndexA + 1) % polygonAVertCount],
                            polygonAX[minYIndexA],
                            polygonAY[minYIndexA],
                            polygonBY[rightIndexB]);
                        int rightEdgeAX = LinearInterpolate(
                            polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAX[rightIndexA],
                            polygonAY[rightIndexA],
                            polygonBY[rightIndexB]);
                        int leftEdgeBX = LinearInterpolate(
                            polygonBX[(minYIndexB + 1) % polygonBVertCount],
                            polygonBY[(minYIndexB + 1) % polygonBVertCount],
                            polygonBX[minYIndexB],
                            polygonBY[minYIndexB],
                            polygonBY[rightIndexB]);
                        int rightChainBVertexX = polygonBX[rightIndexB];

                        if (ComparePolygonRanges(
                            leftEdgeAX,
                            rightEdgeAX,
                            leftEdgeBX,
                            rightChainBVertexX,
                            isIntersecting))
                        {
                            return true;
                        }

                        rightIndexB = (rightIndexB + 1) % polygonBVertCount;

                        if (minYIndexB == rightIndexB)
                        {
                            sweepState = PolygonSweepState.PolygonBChainsMerged;
                        }
                    }
                }
                else if (polygonAY[rightIndexA] < polygonBY[minYIndexB])
                {
                    if (polygonAY[rightIndexA] < polygonBY[rightIndexB])
                    {
                        int leftEdgeAX = LinearInterpolate(
                            polygonAX[(minYIndexA + 1) % polygonAVertCount],
                            polygonAY[(minYIndexA + 1) % polygonAVertCount],
                            polygonAX[minYIndexA],
                            polygonAY[minYIndexA],
                            polygonAY[rightIndexA]);
                        int rightChainAVertexX = polygonAX[rightIndexA];
                        int leftEdgeBX = LinearInterpolate(
                            polygonBX[(minYIndexB + 1) % polygonBVertCount],
                            polygonBY[(minYIndexB + 1) % polygonBVertCount],
                            polygonBX[minYIndexB],
                            polygonBY[minYIndexB],
                            polygonAY[rightIndexA]);
                        int rightEdgeBX = LinearInterpolate(
                            polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                            polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                            polygonBX[rightIndexB],
                            polygonBY[rightIndexB],
                            polygonAY[rightIndexA]);

                        if (ComparePolygonRanges(
                            leftEdgeAX,
                            rightChainAVertexX,
                            leftEdgeBX,
                            rightEdgeBX,
                            isIntersecting))
                        {
                            return true;
                        }

                        rightIndexA = (rightIndexA + 1) % polygonAVertCount;

                        if (minYIndexA == rightIndexA)
                        {
                            sweepState = PolygonSweepState.PolygonAChainsMerged;
                        }
                    }
                    else
                    {
                        int leftEdgeAX = LinearInterpolate(
                            polygonAX[(minYIndexA + 1) % polygonAVertCount],
                            polygonAY[(minYIndexA + 1) % polygonAVertCount],
                            polygonAX[minYIndexA],
                            polygonAY[minYIndexA],
                            polygonBY[rightIndexB]);
                        int rightEdgeAX = LinearInterpolate(
                            polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAX[rightIndexA],
                            polygonAY[rightIndexA],
                            polygonBY[rightIndexB]);
                        int leftEdgeBX = LinearInterpolate(
                            polygonBX[(minYIndexB + 1) % polygonBVertCount],
                            polygonBY[(minYIndexB + 1) % polygonBVertCount],
                            polygonBX[minYIndexB],
                            polygonBY[minYIndexB],
                            polygonBY[rightIndexB]);
                        int rightChainBVertexX = polygonBX[rightIndexB];

                        if (ComparePolygonRanges(
                            leftEdgeAX,
                            rightEdgeAX,
                            leftEdgeBX,
                            rightChainBVertexX,
                            isIntersecting))
                        {
                            return true;
                        }

                        rightIndexB = (rightIndexB + 1) % polygonBVertCount;

                        if (minYIndexB == rightIndexB)
                        {
                            sweepState = PolygonSweepState.PolygonBChainsMerged;
                        }
                    }
                }
                else if (polygonBY[minYIndexB] < polygonBY[rightIndexB])
                {
                    int leftEdgeAX = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonBY[minYIndexB]);
                    int rightEdgeAX = LinearInterpolate(
                        polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAX[rightIndexA],
                        polygonAY[rightIndexA],
                        polygonBY[minYIndexB]);
                    int leftChainBVertexX = polygonBX[minYIndexB];
                    int rightEdgeBX = LinearInterpolate(
                        polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBX[rightIndexB],
                        polygonBY[rightIndexB],
                        polygonBY[minYIndexB]);

                    if (ComparePolygonRanges(
                        leftEdgeAX,
                        rightEdgeAX,
                        leftChainBVertexX,
                        rightEdgeBX,
                        isIntersecting))
                    {
                        return true;
                    }

                    minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;

                    if (minYIndexB == rightIndexB)
                    {
                        sweepState = PolygonSweepState.PolygonBChainsMerged;
                    }
                }
                else
                {
                    int leftEdgeAX = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonBY[rightIndexB]);
                    int rightEdgeAX = LinearInterpolate(
                        polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAX[rightIndexA],
                        polygonAY[rightIndexA],
                        polygonBY[rightIndexB]);
                    int leftEdgeBX = LinearInterpolate(
                        polygonBX[(minYIndexB + 1) % polygonBVertCount],
                        polygonBY[(minYIndexB + 1) % polygonBVertCount],
                        polygonBX[minYIndexB],
                        polygonBY[minYIndexB],
                        polygonBY[rightIndexB]);
                    int rightChainBVertexX = polygonBX[rightIndexB];

                    if (ComparePolygonRanges(
                        leftEdgeAX,
                        rightEdgeAX,
                        leftEdgeBX,
                        rightChainBVertexX,
                        isIntersecting))
                    {
                        return true;
                    }

                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;

                    if (minYIndexB == rightIndexB)
                    {
                        sweepState = PolygonSweepState.PolygonBChainsMerged;
                    }
                }
            }

            while (sweepState == PolygonSweepState.PolygonAChainsMerged)
            {
                if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
                {
                    if (polygonAY[minYIndexA] < polygonBY[rightIndexB])
                    {
                        int singleVertexAX = polygonAX[minYIndexA];
                        int leftEdgeBX = LinearInterpolate(
                            polygonBX[(minYIndexB + 1) % polygonBVertCount],
                            polygonBY[(minYIndexB + 1) % polygonBVertCount],
                            polygonBX[minYIndexB],
                            polygonBY[minYIndexB],
                            polygonAY[minYIndexA]);
                        int rightEdgeBX = LinearInterpolate(
                            polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                            polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                            polygonBX[rightIndexB],
                            polygonBY[rightIndexB],
                            polygonAY[minYIndexA]);

                        return ComparePolygonRange(leftEdgeBX, rightEdgeBX, singleVertexAX, !isIntersecting);
                    }

                    int leftEdgeAX = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonBY[rightIndexB]);
                    int rightEdgeAX = LinearInterpolate(
                        polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAX[rightIndexA],
                        polygonAY[rightIndexA],
                        polygonBY[rightIndexB]);
                    int leftEdgeBAtRightChainX = LinearInterpolate(
                        polygonBX[(minYIndexB + 1) % polygonBVertCount],
                        polygonBY[(minYIndexB + 1) % polygonBVertCount],
                        polygonBX[minYIndexB],
                        polygonBY[minYIndexB],
                        polygonBY[rightIndexB]);
                    int rightChainBVertexX = polygonBX[rightIndexB];

                    if (ComparePolygonRanges(
                        leftEdgeAX,
                        rightEdgeAX,
                        leftEdgeBAtRightChainX,
                        rightChainBVertexX,
                        isIntersecting))
                    {
                        return true;
                    }

                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;

                    if (minYIndexB == rightIndexB)
                    {
                        sweepState = PolygonSweepState.BothChainsActive;
                    }
                }
                else if (polygonBY[minYIndexB] < polygonBY[rightIndexB])
                {
                    int leftEdgeAX = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonBY[minYIndexB]);
                    int rightEdgeAX = LinearInterpolate(
                        polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAX[rightIndexA],
                        polygonAY[rightIndexA],
                        polygonBY[minYIndexB]);
                    int leftChainBVertexX = polygonBX[minYIndexB];
                    int rightEdgeBX = LinearInterpolate(
                        polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBX[rightIndexB],
                        polygonBY[rightIndexB],
                        polygonBY[minYIndexB]);

                    if (ComparePolygonRanges(
                        leftEdgeAX,
                        rightEdgeAX,
                        leftChainBVertexX,
                        rightEdgeBX,
                        isIntersecting))
                    {
                        return true;
                    }

                    minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;

                    if (minYIndexB == rightIndexB)
                    {
                        sweepState = PolygonSweepState.BothChainsActive;
                    }
                }
                else
                {
                    int leftEdgeAX = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonBY[rightIndexB]);
                    int rightEdgeAX = LinearInterpolate(
                        polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAX[rightIndexA],
                        polygonAY[rightIndexA],
                        polygonBY[rightIndexB]);
                    int leftEdgeBX = LinearInterpolate(
                        polygonBX[(minYIndexB + 1) % polygonBVertCount],
                        polygonBY[(minYIndexB + 1) % polygonBVertCount],
                        polygonBX[minYIndexB],
                        polygonBY[minYIndexB],
                        polygonBY[rightIndexB]);
                    int rightChainBVertexX = polygonBX[rightIndexB];

                    if (ComparePolygonRanges(
                        leftEdgeAX,
                        rightEdgeAX,
                        leftEdgeBX,
                        rightChainBVertexX,
                        isIntersecting))
                    {
                        return true;
                    }

                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;

                    if (minYIndexB == rightIndexB)
                    {
                        sweepState = PolygonSweepState.BothChainsActive;
                    }
                }
            }

            while (sweepState == PolygonSweepState.PolygonBChainsMerged)
            {
                if (polygonBY[minYIndexB] < polygonAY[minYIndexA])
                {
                    if (polygonBY[minYIndexB] < polygonAY[rightIndexA])
                    {
                        int leftEdgeAX = LinearInterpolate(
                            polygonAX[(minYIndexA + 1) % polygonAVertCount],
                            polygonAY[(minYIndexA + 1) % polygonAVertCount],
                            polygonAX[minYIndexA],
                            polygonAY[minYIndexA],
                            polygonBY[minYIndexB]);
                        int rightEdgeAX = LinearInterpolate(
                            polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                            polygonAX[rightIndexA],
                            polygonAY[rightIndexA],
                            polygonBY[minYIndexB]);
                        int singleVertexBX = polygonBX[minYIndexB];

                        return ComparePolygonRange(leftEdgeAX, rightEdgeAX, singleVertexBX, isIntersecting);
                    }

                    int leftEdgeAXStep = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonAY[rightIndexA]);
                    int rightChainAVertexX = polygonAX[rightIndexA];
                    int leftEdgeBXStep = LinearInterpolate(
                        polygonBX[(minYIndexB + 1) % polygonBVertCount],
                        polygonBY[(minYIndexB + 1) % polygonBVertCount],
                        polygonBX[minYIndexB],
                        polygonBY[minYIndexB],
                        polygonAY[rightIndexA]);
                    int rightEdgeBXStep = LinearInterpolate(
                        polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBX[rightIndexB],
                        polygonBY[rightIndexB],
                        polygonAY[rightIndexA]);

                    if (ComparePolygonRanges(
                        leftEdgeAXStep,
                        rightChainAVertexX,
                        leftEdgeBXStep,
                        rightEdgeBXStep,
                        isIntersecting))
                    {
                        return true;
                    }

                    rightIndexA = (rightIndexA + 1) % polygonAVertCount;

                    if (minYIndexA == rightIndexA)
                    {
                        sweepState = PolygonSweepState.BothChainsActive;
                    }
                }
                else if (polygonAY[minYIndexA] < polygonAY[rightIndexA])
                {
                    int leftChainAVertexX = polygonAX[minYIndexA];
                    int rightEdgeAX = LinearInterpolate(
                        polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                        polygonAX[rightIndexA],
                        polygonAY[rightIndexA],
                        polygonAY[minYIndexA]);
                    int leftEdgeBX = LinearInterpolate(
                        polygonBX[(minYIndexB + 1) % polygonBVertCount],
                        polygonBY[(minYIndexB + 1) % polygonBVertCount],
                        polygonBX[minYIndexB],
                        polygonBY[minYIndexB],
                        polygonAY[minYIndexA]);
                    int rightEdgeBX = LinearInterpolate(
                        polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBX[rightIndexB],
                        polygonBY[rightIndexB],
                        polygonAY[minYIndexA]);

                    if (ComparePolygonRanges(
                        leftChainAVertexX,
                        rightEdgeAX,
                        leftEdgeBX,
                        rightEdgeBX,
                        isIntersecting))
                    {
                        return true;
                    }

                    minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;

                    if (minYIndexA == rightIndexA)
                    {
                        sweepState = PolygonSweepState.BothChainsActive;
                    }
                }
                else
                {
                    int leftEdgeAX = LinearInterpolate(
                        polygonAX[(minYIndexA + 1) % polygonAVertCount],
                        polygonAY[(minYIndexA + 1) % polygonAVertCount],
                        polygonAX[minYIndexA],
                        polygonAY[minYIndexA],
                        polygonAY[rightIndexA]);
                    int rightChainAVertexX = polygonAX[rightIndexA];
                    int leftEdgeBX = LinearInterpolate(
                        polygonBX[(minYIndexB + 1) % polygonBVertCount],
                        polygonBY[(minYIndexB + 1) % polygonBVertCount],
                        polygonBX[minYIndexB],
                        polygonBY[minYIndexB],
                        polygonAY[rightIndexA]);
                    int rightEdgeBX = LinearInterpolate(
                        polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                        polygonBX[rightIndexB],
                        polygonBY[rightIndexB],
                        polygonAY[rightIndexA]);

                    if (ComparePolygonRanges(
                        leftEdgeAX,
                        rightChainAVertexX,
                        leftEdgeBX,
                        rightEdgeBX,
                        isIntersecting))
                    {
                        return true;
                    }

                    rightIndexA = (rightIndexA + 1) % polygonAVertCount;

                    if (minYIndexA == rightIndexA)
                    {
                        sweepState = PolygonSweepState.BothChainsActive;
                    }
                }
            }

            if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
            {
                int singleVertexAX = polygonAX[minYIndexA];
                int leftEdgeBX = LinearInterpolate(
                    polygonBX[(minYIndexB + 1) % polygonBVertCount],
                    polygonBY[(minYIndexB + 1) % polygonBVertCount],
                    polygonBX[minYIndexB],
                    polygonBY[minYIndexB],
                    polygonAY[minYIndexA]);
                int rightEdgeBX = LinearInterpolate(
                    polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                    polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount],
                    polygonBX[rightIndexB],
                    polygonBY[rightIndexB],
                    polygonAY[minYIndexA]);

                return ComparePolygonRange(leftEdgeBX, rightEdgeBX, singleVertexAX, !isIntersecting);
            }

            int finalLeftEdgeAX = LinearInterpolate(
                polygonAX[(minYIndexA + 1) % polygonAVertCount],
                polygonAY[(minYIndexA + 1) % polygonAVertCount],
                polygonAX[minYIndexA],
                polygonAY[minYIndexA],
                polygonBY[minYIndexB]);
            int finalRightEdgeAX = LinearInterpolate(
                polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount],
                polygonAX[rightIndexA],
                polygonAY[rightIndexA],
                polygonBY[minYIndexB]);
            int finalSingleVertexBX = polygonBX[minYIndexB];

            return ComparePolygonRange(finalLeftEdgeAX, finalRightEdgeAX, finalSingleVertexBX, isIntersecting);
        }

        private static bool IsAnyVertexOutsidePlane(
            CameraModel sourceModel,
            CameraModel referenceModel,
            bool isForwardCheck)
        {
            GameObject sourceObject = sourceModel.SourceObject;
            int sourceFaceIndex = sourceModel.FaceIndex;
            int[] sourceFaceVertices = sourceObject.FaceVertexIndices[sourceFaceIndex];
            int sourceVertCount = sourceObject.FaceVertexCounts[sourceFaceIndex];

            GameObject referenceObject = referenceModel.SourceObject;
            int referenceFaceIndex = referenceModel.FaceIndex;
            int referenceFirstVertex = referenceObject.FaceVertexIndices[referenceFaceIndex][0];
            int refX = referenceObject.ProjectedX[referenceFirstVertex];
            int refY = referenceObject.ProjectedY[referenceFirstVertex];
            int refDepth = referenceObject.ProjectedDepth[referenceFirstVertex];
            int planeNormalX = referenceModel.NormalX;
            int planeNormalY = referenceModel.NormalY;
            int planeNormalZ = referenceModel.NormalZ;
            int visibilityRange = referenceObject.FaceVisibility[referenceFaceIndex];
            int facingDot = isForwardCheck ? referenceModel.VisibilityDot : -referenceModel.VisibilityDot;

            for (int vertexIndex = 0; vertexIndex < sourceVertCount; vertexIndex += 1)
            {
                int vertex = sourceFaceVertices[vertexIndex];
                int dotProduct =
                    (refX - sourceObject.ProjectedX[vertex]) * planeNormalX +
                    (refY - sourceObject.ProjectedY[vertex]) * planeNormalY +
                    (refDepth - sourceObject.ProjectedDepth[vertex]) * planeNormalZ;

                if ((dotProduct >= -visibilityRange || facingDot >= 0) &&
                    (dotProduct <= visibilityRange || facingDot <= 0))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static ProjectedPolygon BuildProjectedPolygon(
            GameObject gameObject,
            int[] faceVertices,
            int vertCount)
        {
            if (vertCount == 2)
            {
                int vertex0 = faceVertices[0];
                int vertex1 = faceVertices[1];
                int[] x = new int[4];
                int[] y = new int[4];
                x[0] = gameObject.ProjectedU[vertex0] - LineSegmentHalfWidth;
                x[1] = gameObject.ProjectedU[vertex1] - LineSegmentHalfWidth;
                x[2] = gameObject.ProjectedU[vertex1] + LineSegmentHalfWidth;
                x[3] = gameObject.ProjectedU[vertex0] + LineSegmentHalfWidth;
                y[0] = y[3] = gameObject.ProjectedV[vertex0];
                y[1] = y[2] = gameObject.ProjectedV[vertex1];

                return new ProjectedPolygon(x, y);
            }

            int[] polygonX = new int[vertCount];
            int[] polygonY = new int[vertCount];

            for (int vertexIndex = 0; vertexIndex < vertCount; vertexIndex += 1)
            {
                int vertex = faceVertices[vertexIndex];
                polygonX[vertexIndex] = gameObject.ProjectedU[vertex];
                polygonY[vertexIndex] = gameObject.ProjectedV[vertex];
            }

            return new ProjectedPolygon(polygonX, polygonY);
        }
    }

}
