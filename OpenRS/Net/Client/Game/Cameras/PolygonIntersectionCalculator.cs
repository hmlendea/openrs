namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class PolygonIntersectionCalculator
    {
        internal static bool AreBoundsDisjoint(CameraModel modelA, CameraModel modelB)
        {
            if (modelA.boundsMinX >= modelB.boundsMaxX)
            {
                return true;
            }

            if (modelB.boundsMinX >= modelA.boundsMaxX)
            {
                return true;
            }

            if (modelA.boundsMinY >= modelB.boundsMaxY)
            {
                return true;
            }

            if (modelB.boundsMinY >= modelA.boundsMaxY)
            {
                return true;
            }

            if (modelA.boundsMinZ >= modelB.boundsMaxZ)
            {
                return true;
            }

            if (modelB.boundsMinZ > modelA.boundsMaxZ)
            {
                return false;
            }

            GameObject objectA = modelA.Object;
            GameObject objectB = modelB.Object;
            int faceIndexA = modelA.faceVertCountIndex1;
            int faceIndexB = modelB.faceVertCountIndex1;
            int[] faceVerticesA = objectA.face_vertices[faceIndexA];
            int[] faceVerticesB = objectB.face_vertices[faceIndexB];
            int vertCountA = objectA.face_vertices_count[faceIndexA];
            int vertCountB = objectB.face_vertices_count[faceIndexB];
            int refX = objectB.projectedX[faceVerticesB[0]];
            int refY = objectB.projectedY[faceVerticesB[0]];
            int refDepth = objectB.projectedDepth[faceVerticesB[0]];
            int planeNormalX = modelB.normalX;
            int planeNormalY = modelB.normalY;
            int planeNormalZ = modelB.normalZ;
            int visibilityRange = objectB.faceVisibility[faceIndexB];
            int facingDot = modelB.visibilityDot;
            bool isOutsidePlane = false;
            for (int vertLoopA = 0; vertLoopA < vertCountA; vertLoopA += 1)
            {
                int vertexA = faceVerticesA[vertLoopA];
                int dotProductA = (refX - objectA.projectedX[vertexA]) * planeNormalX + (refY - objectA.projectedY[vertexA]) * planeNormalY + (refDepth - objectA.projectedDepth[vertexA]) * planeNormalZ;
                if ((dotProductA >= -visibilityRange || facingDot >= 0) && (dotProductA <= visibilityRange || facingDot <= 0))
                {
                    continue;
                }

                isOutsidePlane = true;
                break;
            }

            if (!isOutsidePlane)
            {
                return true;
            }

            refX = objectA.projectedX[faceVerticesA[0]];
            refY = objectA.projectedY[faceVerticesA[0]];
            refDepth = objectA.projectedDepth[faceVerticesA[0]];
            planeNormalX = modelA.normalX;
            planeNormalY = modelA.normalY;
            planeNormalZ = modelA.normalZ;
            visibilityRange = objectA.faceVisibility[faceIndexA];
            facingDot = modelA.visibilityDot;
            isOutsidePlane = false;
            for (int vertLoopB = 0; vertLoopB < vertCountB; vertLoopB += 1)
            {
                int vertexB = faceVerticesB[vertLoopB];
                int dotProductB = (refX - objectB.projectedX[vertexB]) * planeNormalX + (refY - objectB.projectedY[vertexB]) * planeNormalY + (refDepth - objectB.projectedDepth[vertexB]) * planeNormalZ;
                if ((dotProductB >= -visibilityRange || facingDot <= 0) && (dotProductB <= visibilityRange || facingDot >= 0))
                {
                    continue;
                }

                isOutsidePlane = true;
                break;
            }

            if (!isOutsidePlane)
            {
                return true;
            }

            int[] polygonAX;
            int[] polygonAY;
            if (vertCountA == 2)
            {
                polygonAX = new int[4];
                polygonAY = new int[4];
                int vert0A = faceVerticesA[0];
                int vert1A = faceVerticesA[1];
                polygonAX[0] = objectA.projectedU[vert0A] - 20;
                polygonAX[1] = objectA.projectedU[vert1A] - 20;
                polygonAX[2] = objectA.projectedU[vert1A] + 20;
                polygonAX[3] = objectA.projectedU[vert0A] + 20;
                polygonAY[0] = polygonAY[3] = objectA.projectedV[vert0A];
                polygonAY[1] = polygonAY[2] = objectA.projectedV[vert1A];
            }
            else
            {
                polygonAX = new int[vertCountA];
                polygonAY = new int[vertCountA];
                for (int vertLoopAInner = 0; vertLoopAInner < vertCountA; vertLoopAInner += 1)
                {
                    int vertA = faceVerticesA[vertLoopAInner];
                    polygonAX[vertLoopAInner] = objectA.projectedU[vertA];
                    polygonAY[vertLoopAInner] = objectA.projectedV[vertA];
                }

            }
            int[] polygonBX;
            int[] polygonBY;
            if (vertCountB == 2)
            {
                polygonBX = new int[4];
                polygonBY = new int[4];
                int vert0B = faceVerticesB[0];
                int vert1B = faceVerticesB[1];
                polygonBX[0] = objectB.projectedU[vert0B] - 20;
                polygonBX[1] = objectB.projectedU[vert1B] - 20;
                polygonBX[2] = objectB.projectedU[vert1B] + 20;
                polygonBX[3] = objectB.projectedU[vert0B] + 20;
                polygonBY[0] = polygonBY[3] = objectB.projectedV[vert0B];
                polygonBY[1] = polygonBY[2] = objectB.projectedV[vert1B];
            }
            else
            {
                polygonBX = new int[vertCountB];
                polygonBY = new int[vertCountB];
                for (int vertLoopBInner = 0; vertLoopBInner < vertCountB; vertLoopBInner += 1)
                {
                    int vertB = faceVerticesB[vertLoopBInner];
                    polygonBX[vertLoopBInner] = objectB.projectedU[vertB];
                    polygonBY[vertLoopBInner] = objectB.projectedV[vertB];
                }

            }
            return !PolygonsIntersect(polygonAX, polygonAY, polygonBX, polygonBY);
        }

        internal static bool IsModelBehind(CameraModel frontModel, CameraModel behindModel)
        {
            GameObject frontObject = frontModel.Object;
            GameObject behindObject = behindModel.Object;
            int frontFaceIndex = frontModel.faceVertCountIndex1;
            int behindFaceIndex = behindModel.faceVertCountIndex1;
            int[] frontFaceVertices = frontObject.face_vertices[frontFaceIndex];
            int[] behindFaceVertices = behindObject.face_vertices[behindFaceIndex];
            int frontVertCount = frontObject.face_vertices_count[frontFaceIndex];
            int behindVertCount = behindObject.face_vertices_count[behindFaceIndex];
            int refX = behindObject.projectedX[behindFaceVertices[0]];
            int refY = behindObject.projectedY[behindFaceVertices[0]];
            int refDepth = behindObject.projectedDepth[behindFaceVertices[0]];
            int planeNormalX = behindModel.normalX;
            int planeNormalY = behindModel.normalY;
            int planeNormalZ = behindModel.normalZ;
            int visibilityRange = behindObject.faceVisibility[behindFaceIndex];
            int facingDot = behindModel.visibilityDot;
            bool isOutsidePlane = false;
            for (int vertLoopFront = 0; vertLoopFront < frontVertCount; vertLoopFront += 1)
            {
                int frontVertex = frontFaceVertices[vertLoopFront];
                int frontDotProduct = (refX - frontObject.projectedX[frontVertex]) * planeNormalX + (refY - frontObject.projectedY[frontVertex]) * planeNormalY + (refDepth - frontObject.projectedDepth[frontVertex]) * planeNormalZ;
                if ((frontDotProduct >= -visibilityRange || facingDot >= 0) && (frontDotProduct <= visibilityRange || facingDot <= 0))
                {
                    continue;
                }

                isOutsidePlane = true;
                break;
            }

            if (!isOutsidePlane)
            {
                return true;
            }

            refX = frontObject.projectedX[frontFaceVertices[0]];
            refY = frontObject.projectedY[frontFaceVertices[0]];
            refDepth = frontObject.projectedDepth[frontFaceVertices[0]];
            planeNormalX = frontModel.normalX;
            planeNormalY = frontModel.normalY;
            planeNormalZ = frontModel.normalZ;
            visibilityRange = frontObject.faceVisibility[frontFaceIndex];
            facingDot = frontModel.visibilityDot;
            isOutsidePlane = false;
            for (int vertLoopBehind = 0; vertLoopBehind < behindVertCount; vertLoopBehind += 1)
            {
                int behindVertex = behindFaceVertices[vertLoopBehind];
                int behindDotProduct = (refX - behindObject.projectedX[behindVertex]) * planeNormalX + (refY - behindObject.projectedY[behindVertex]) * planeNormalY + (refDepth - behindObject.projectedDepth[behindVertex]) * planeNormalZ;
                if ((behindDotProduct >= -visibilityRange || facingDot <= 0) && (behindDotProduct <= visibilityRange || facingDot >= 0))
                {
                    continue;
                }

                isOutsidePlane = true;
                break;
            }

            return !isOutsidePlane;
        }

        internal static int LinearInterpolate(int valueA, int rangeStartY, int valueB, int rangeEndY, int targetY)
        {
            if (rangeEndY == rangeStartY)
            {
                return valueA;
            }

            return valueA + (valueB - valueA) * (targetY - rangeStartY) / (rangeEndY - rangeStartY);
        }

        internal static bool ComparePolygonRanges(int valueA, int valueB, int rangeMin, int rangeMax, bool isAscending)
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

        internal static bool ComparePolygonRange(int valueA, int valueB, int rangeLimit, bool isAscending)
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

        internal static bool PolygonsIntersect(int[] polygonAX, int[] polygonAY, int[] polygonBX, int[] polygonBY)
        {
            int polygonAVertCount = polygonAX.Length;
            int polygonBVertCount = polygonBX.Length;
            byte bothStarted = 0;
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
                for (rightIndexA = minYIndexA; polygonAY[rightIndexA] < polygonBY[minYIndexB]; rightIndexA = (rightIndexA + 1) % polygonAVertCount)
                {
                    ;
                }

                for (; polygonAY[minYIndexA] < polygonBY[minYIndexB]; minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount)
                {
                    ;
                }

                int leftXA = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[minYIndexB]);
                int rightXA = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[minYIndexB]);
                int leftXB = polygonBX[minYIndexB];
                isIntersecting = (leftXA < leftXB) | (rightXA < leftXB);
                if (ComparePolygonRange(leftXA, rightXA, leftXB, isIntersecting))
                {
                    return true;
                }

                rightIndexB = (minYIndexB + 1) % polygonBVertCount;
                minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;
                if (minYIndexA == rightIndexA)
                {
                    bothStarted = 1;
                }
            }
            else
            {
                for (rightIndexB = minYIndexB; polygonBY[rightIndexB] < polygonAY[minYIndexA]; rightIndexB = (rightIndexB + 1) % polygonBVertCount)
                {
                    ;
                }

                for (; polygonBY[minYIndexB] < polygonAY[minYIndexA]; minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount)
                {
                    ;
                }

                int targetY = polygonAX[minYIndexA];
                int l11 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[minYIndexA]);
                int k16 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[minYIndexA]);
                isIntersecting = (targetY < l11) | (targetY < k16);
                if (ComparePolygonRange(l11, k16, targetY, !isIntersecting))
                {
                    return true;
                }

                rightIndexA = (minYIndexA + 1) % polygonAVertCount;
                minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;
                if (minYIndexB == rightIndexB)
                {
                    bothStarted = 2;
                }
            }
            while (bothStarted == 0)
            {
                if (polygonAY[minYIndexA] < polygonAY[rightIndexA])
                {
                    if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
                    {
                        if (polygonAY[minYIndexA] < polygonBY[rightIndexB])
                        {
                            int leftXInterp = polygonAX[minYIndexA];
                            int leftXALower = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonAY[minYIndexA]);
                            int i12 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[minYIndexA]);
                            int l16 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[minYIndexA]);
                            if (ComparePolygonRanges(leftXInterp, leftXALower, i12, l16, isIntersecting))
                            {
                                return true;
                            }

                            minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;
                            if (minYIndexA == rightIndexA)
                            {
                                bothStarted = 1;
                            }
                        }
                        else
                        {
                            int rightXInterp = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[rightIndexB]);
                            int rightXALower = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[rightIndexB]);
                            int loopCount = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonBY[rightIndexB]);
                            int i17 = polygonBX[rightIndexB];
                            if (ComparePolygonRanges(rightXInterp, rightXALower, loopCount, i17, isIntersecting))
                            {
                                return true;
                            }

                            rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                            if (minYIndexB == rightIndexB)
                            {
                                bothStarted = 2;
                            }
                        }
                    }
                    else if (polygonBY[minYIndexB] < polygonBY[rightIndexB])
                    {
                        int bLeftXInterp = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[minYIndexB]);
                        int i8 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[minYIndexB]);
                        int leftXBLower = polygonBX[minYIndexB];
                        int j17 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonBY[minYIndexB]);
                        if (ComparePolygonRanges(bLeftXInterp, i8, leftXBLower, j17, isIntersecting))
                        {
                            return true;
                        }

                        minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;
                        if (minYIndexB == rightIndexB)
                        {
                            bothStarted = 2;
                        }
                    }
                    else
                    {
                        int targetXB = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[rightIndexB]);
                        int j8 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[rightIndexB]);
                        int leftXBStep = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonBY[rightIndexB]);
                        int k17 = polygonBX[rightIndexB];
                        if (ComparePolygonRanges(targetXB, j8, leftXBStep, k17, isIntersecting))
                        {
                            return true;
                        }

                        rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                        if (minYIndexB == rightIndexB)
                        {
                            bothStarted = 2;
                        }
                    }
                }
                else if (polygonAY[rightIndexA] < polygonBY[minYIndexB])
                {
                    if (polygonAY[rightIndexA] < polygonBY[rightIndexB])
                    {
                        int leftXC = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonAY[rightIndexA]);
                        int k8 = polygonAX[rightIndexA];
                        int i13 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[rightIndexA]);
                        int l17 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[rightIndexA]);
                        if (ComparePolygonRanges(leftXC, k8, i13, l17, isIntersecting))
                        {
                            return true;
                        }

                        rightIndexA = (rightIndexA + 1) % polygonAVertCount;
                        if (minYIndexA == rightIndexA)
                        {
                            bothStarted = 1;
                        }
                    }
                    else
                    {
                        int rightXBLower = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[rightIndexB]);
                        int l8 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[rightIndexB]);
                        int j13 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonBY[rightIndexB]);
                        int i18 = polygonBX[rightIndexB];
                        if (ComparePolygonRanges(rightXBLower, l8, j13, i18, isIntersecting))
                        {
                            return true;
                        }

                        rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                        if (minYIndexB == rightIndexB)
                        {
                            bothStarted = 2;
                        }
                    }
                }
                else if (polygonBY[minYIndexB] < polygonBY[rightIndexB])
                {
                    int rightXC = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[minYIndexB]);
                    int i9 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[minYIndexB]);
                    int k13 = polygonBX[minYIndexB];
                    int j18 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonBY[minYIndexB]);
                    if (ComparePolygonRanges(rightXC, i9, k13, j18, isIntersecting))
                    {
                        return true;
                    }

                    minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;
                    if (minYIndexB == rightIndexB)
                    {
                        bothStarted = 2;
                    }
                }
                else
                {
                    int leftXCB = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[rightIndexB]);
                    int j9 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[rightIndexB]);
                    int l13 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonBY[rightIndexB]);
                    int k18 = polygonBX[rightIndexB];
                    if (ComparePolygonRanges(leftXCB, j9, l13, k18, isIntersecting))
                    {
                        return true;
                    }

                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                    if (minYIndexB == rightIndexB)
                    {
                        bothStarted = 2;
                    }
                }
            }

            while (bothStarted == 1)
            {
                if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
                {
                    if (polygonAY[minYIndexA] < polygonBY[rightIndexB])
                    {
                        int rightXBStep = polygonAX[minYIndexA];
                        int i14 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[minYIndexA]);
                        int l18 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[minYIndexA]);
                        return ComparePolygonRange(i14, l18, rightXBStep, !isIntersecting);
                    }
                    int vertIterA = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[rightIndexB]);
                    int k9 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[rightIndexB]);
                    int j14 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonBY[rightIndexB]);
                    int i19 = polygonBX[rightIndexB];
                    if (ComparePolygonRanges(vertIterA, k9, j14, i19, isIntersecting))
                    {
                        return true;
                    }

                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                    if (minYIndexB == rightIndexB)
                    {
                        bothStarted = 0;
                    }
                }
                else if (polygonBY[minYIndexB] < polygonBY[rightIndexB])
                {
                    int vertIterB = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[minYIndexB]);
                    int l9 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[minYIndexB]);
                    int k14 = polygonBX[minYIndexB];
                    int j19 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonBY[minYIndexB]);
                    if (ComparePolygonRanges(vertIterB, l9, k14, j19, isIntersecting))
                    {
                        return true;
                    }

                    minYIndexB = (minYIndexB - 1 + polygonBVertCount) % polygonBVertCount;
                    if (minYIndexB == rightIndexB)
                    {
                        bothStarted = 0;
                    }
                }
                else
                {
                    int k5 = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[rightIndexB]);
                    int i10 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[rightIndexB]);
                    int l14 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonBY[rightIndexB]);
                    int k19 = polygonBX[rightIndexB];
                    if (ComparePolygonRanges(k5, i10, l14, k19, isIntersecting))
                    {
                        return true;
                    }

                    rightIndexB = (rightIndexB + 1) % polygonBVertCount;
                    if (minYIndexB == rightIndexB)
                    {
                        bothStarted = 0;
                    }
                }
            }

            while (bothStarted == 2)
            {
                if (polygonBY[minYIndexB] < polygonAY[minYIndexA])
                {
                    if (polygonBY[minYIndexB] < polygonAY[rightIndexA])
                    {
                        int l5 = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[minYIndexB]);
                        int j10 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[minYIndexB]);
                        int i15 = polygonBX[minYIndexB];
                        return ComparePolygonRange(l5, j10, i15, isIntersecting);
                    }
                    int i6 = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonAY[rightIndexA]);
                    int k10 = polygonAX[rightIndexA];
                    int j15 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[rightIndexA]);
                    int l19 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[rightIndexA]);
                    if (ComparePolygonRanges(i6, k10, j15, l19, isIntersecting))
                    {
                        return true;
                    }

                    rightIndexA = (rightIndexA + 1) % polygonAVertCount;
                    if (minYIndexA == rightIndexA)
                    {
                        bothStarted = 0;
                    }
                }
                else if (polygonAY[minYIndexA] < polygonAY[rightIndexA])
                {
                    int j6 = polygonAX[minYIndexA];
                    int l10 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonAY[minYIndexA]);
                    int k15 = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[minYIndexA]);
                    int i20 = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[minYIndexA]);
                    if (ComparePolygonRanges(j6, l10, k15, i20, isIntersecting))
                    {
                        return true;
                    }

                    minYIndexA = (minYIndexA - 1 + polygonAVertCount) % polygonAVertCount;
                    if (minYIndexA == rightIndexA)
                    {
                        bothStarted = 0;
                    }
                }
                else
                {
                    int polygonAXLeft = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonAY[rightIndexA]);
                    int polygonARightX = polygonAX[rightIndexA];
                    int polygonBXLeft = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[rightIndexA]);
                    int polygonBXRight = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[rightIndexA]);
                    if (ComparePolygonRanges(polygonAXLeft, polygonARightX, polygonBXLeft, polygonBXRight, isIntersecting))
                    {
                        return true;
                    }

                    rightIndexA = (rightIndexA + 1) % polygonAVertCount;
                    if (minYIndexA == rightIndexA)
                    {
                        bothStarted = 0;
                    }
                }
            }

            if (polygonAY[minYIndexA] < polygonBY[minYIndexB])
            {
                int polygonAXAtMin = polygonAX[minYIndexA];
                int polygonBXLeftInterp = LinearInterpolate(polygonBX[(minYIndexB + 1) % polygonBVertCount], polygonBY[(minYIndexB + 1) % polygonBVertCount], polygonBX[minYIndexB], polygonBY[minYIndexB], polygonAY[minYIndexA]);
                int polygonBXRightInterp = LinearInterpolate(polygonBX[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBY[(rightIndexB - 1 + polygonBVertCount) % polygonBVertCount], polygonBX[rightIndexB], polygonBY[rightIndexB], polygonAY[minYIndexA]);

                return ComparePolygonRange(polygonBXLeftInterp, polygonBXRightInterp, polygonAXAtMin, !isIntersecting);
            }

            int polygonAXLeft2 = LinearInterpolate(polygonAX[(minYIndexA + 1) % polygonAVertCount], polygonAY[(minYIndexA + 1) % polygonAVertCount], polygonAX[minYIndexA], polygonAY[minYIndexA], polygonBY[minYIndexB]);
            int polygonAXRight2 = LinearInterpolate(polygonAX[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAY[(rightIndexA - 1 + polygonAVertCount) % polygonAVertCount], polygonAX[rightIndexA], polygonAY[rightIndexA], polygonBY[minYIndexB]);
            int polygonBXAtMin2 = polygonBX[minYIndexB];

            return ComparePolygonRange(polygonAXLeft2, polygonAXRight2, polygonBXAtMin2, isIntersecting);
        }

    }

}
