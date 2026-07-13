using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class Camera
    {
        public Camera(GameImage gameImageSource, int maxObjects, int maxVisibleObjects, int maxSceneObjects)
        {
            maxTextureCount = 50;
            textureClipIds = new int[maxTextureCount];
            textureClipData = new int[maxTextureCount][];

            for (int j = 0; j < maxTextureCount; j += 1)
            {
                textureClipData[j] = new int[256];
            }

            nearPlane = 5;
            zoom1 = 1000;
            zoom2 = 1000;
            zoom3 = 20;
            zoom4 = 10;
            isInterlaced = false;
            scaleFactor = 1.1000000000000001D;
            depthSortStride = 1;
            isMousePositionUpdated = false;
            maxHighlightedObjects = 100;
            _highlightedObjects = new GameObject[maxHighlightedObjects];
            highlightedPlayerIds = new int[maxHighlightedObjects];
            defaultScreenHalfWidth = 512;
            screenCentreX = 256;
            screenCentreY = 192;
            screenMouseOffsetX = 256;
            scanlineBufferCentre = 256;
            screenProjectionShift = 8;
            lightingFactor = 4;
            clipPolygonX = new int[40];
            clipPolygonY = new int[40];
            clipPolygonValues = new int[40];
            vertX = new int[40];
            vertY = new int[40];
            vertZ = new int[40];
            isRenderingInterlaced = false;
            gameImage = gameImageSource;
            screenCentreX = gameImageSource.gameWidth / 2;
            screenCentreY = gameImageSource.gameHeight / 2;
            screenPixels = gameImageSource.pixels;
            currentObjectCount = 0;
            totalModelCount = maxObjects;
            objectCache = new GameObject[totalModelCount];
            modelPriorities = new int[totalModelCount];
            currentModelIndex = 0;
            visibleModels = new CameraModel[maxVisibleObjects];
            for (int k = 0; k < maxVisibleObjects; k += 1)
            {
                visibleModels[k] = new CameraModel();
            }

            sceneObjectCount = 0;
            highlightedObject = new GameObject(maxSceneObjects * 2, maxSceneObjects);
            sceneObjectId = new int[maxSceneObjects];
            sceneObjectWidths = new int[maxSceneObjects];
            sceneObjectHeights = new int[maxSceneObjects];
            sceneObjectX = new int[maxSceneObjects];
            sceneObjectY = new int[maxSceneObjects];
            sceneObjectZ = new int[maxSceneObjects];
            sceneObjectFrames = new int[maxSceneObjects];
            if (lookupTable is null)
            {
                lookupTable = new sbyte[17691];
            }

            viewX = 0;
            ViewY = 0;
            ViewZ = 0;
            cameraOffsetX = 0;
            cameraOffsetY = 0;
            cameraOffsetZ = 0;
            for (int i1 = 0; i1 < 256; i1 += 1)
            {
                int sineValue = (int)(Math.Sin(i1 * 0.02454369D) * 32768D);
                sinCosTable[i1] = (int)(Math.Sin(i1 * 0.02454369D) * 32768D);
                sinCosTable[i1 + 256] = (int)(Math.Cos(i1 * 0.02454369D) * 32768D);
            }

            for (int j1 = 0; j1 < 1024; j1 += 1)
            {
                int sineValue = (int)(Math.Sin(j1 * 0.00613592315D) * 32768D);
                trigonometryTable[j1] = (int)(Math.Sin(j1 * 0.00613592315D) * 32768D);
                trigonometryTable[j1 + 1024] = (int)(Math.Cos(j1 * 0.00613592315D) * 32768D);
            }

        }

        public void AddModel(GameObject gameObject)
        {
            if (gameObject is null)
            {
                Console.WriteLine("Warning tried to add null object!");
            }

            if (currentObjectCount < totalModelCount)
            {
                modelPriorities[currentObjectCount] = 0;
                objectCache[currentObjectCount++] = gameObject;
            }
        }

        public void RemoveModel(GameObject gameObject)
        {
            for (int k = 0; k < currentObjectCount; k += 1)
            {
                if (objectCache[k] == gameObject)
                {
                    currentObjectCount -= 1;
                    for (int i1 = k; i1 < currentObjectCount; i1 += 1)
                    {
                        objectCache[i1] = objectCache[i1 + 1];
                        modelPriorities[i1] = modelPriorities[i1 + 1];
                    }

                }
            }
        }

        public void CleanUp()
        {
            InitializeScene();
            for (int k = 0; k < currentObjectCount; k += 1)
            {
                objectCache[k] = null;
            }

            currentObjectCount = 0;
        }

        public void InitializeScene()
        {
            sceneObjectCount = 0;
            highlightedObject.ResetObjectIndexes();
        }

        public void RemoveLastUpdates(int count)
        {
            sceneObjectCount -= count;
            highlightedObject.AddPolygonToGroup(count, count * 2);
            if (sceneObjectCount < 0)
            {
                sceneObjectCount = 0;
            }
        }

        public int AddSpriteToScene(int objectId, int x, int y, int z, int width, int height, int entityType)
        {
            sceneObjectId[sceneObjectCount] = objectId;
            sceneObjectX[sceneObjectCount] = x;
            sceneObjectY[sceneObjectCount] = y;
            sceneObjectZ[sceneObjectCount] = z;
            sceneObjectWidths[sceneObjectCount] = width;
            sceneObjectHeights[sceneObjectCount] = height;
            sceneObjectFrames[sceneObjectCount] = 0;
            int k2 = highlightedObject.AddVertex(x, y, z);
            int l2 = highlightedObject.AddVertex(x, y - height, z);
            int[] ai = [k2, l2];
            highlightedObject.AddFaceVertices(2, ai, 0, 0);
            highlightedObject.entityType[sceneObjectCount] = entityType;
            highlightedObject.polygonTypeData[sceneObjectCount++] = 0;
            return sceneObjectCount - 1;
        }

        public void RemoveSprite(int spriteIndex)
        {
            highlightedObject.polygonTypeData[spriteIndex] = 1;
        }

        public void UpdateSpritePosition(int spriteIndex, int frameIndex)
        {
            sceneObjectFrames[spriteIndex] = frameIndex;
        }

        public void SetMousePosition(int mouseX, int mouseY)
        {
            mouseAdjustedX = mouseX - screenMouseOffsetX;
            mouseAdjustedY = mouseY;
            optionCount = 0;
            isMousePositionUpdated = true;
        }

        public int GetOptionCount()
        {
            return optionCount;
        }

        public int[] GetHighlightedPlayers()
        {
            return highlightedPlayerIds;
        }

        public GameObject[] GetHighlightedObjects()
        {
            return _highlightedObjects;
        }

        public void SetCameraSize(int mouseOffsetX, int scanlineCentre, int centreScrX, int centreScrY, int screenHalfWidth, int projectionShift)
        {
            screenCentreX = centreScrX;
            screenCentreY = centreScrY;
            screenMouseOffsetX = mouseOffsetX;
            scanlineBufferCentre = scanlineCentre;
            defaultScreenHalfWidth = screenHalfWidth;
            screenProjectionShift = projectionShift;
            scanlineVariables = new CameraVariable[centreScrY + scanlineCentre];
            for (int k = 0; k < centreScrY + scanlineCentre; k += 1)
            {
                scanlineVariables[k] = new CameraVariable();
            }
        }

        private void SortAndRenderModels(CameraModel[] models, int startIndex, int endIndex)
        {
            if (startIndex < endIndex)
            {
                int k = startIndex - 1;
                int i1 = endIndex + 1;
                int j1 = (startIndex + endIndex) / 2;
                CameraModel l1 = models[j1];
                models[j1] = models[startIndex];
                models[startIndex] = l1;
                int k1 = l1.Scale;
                while (k < i1)
                {
                    do
                    {
                        i1 -= 1;
                    }
                    while (models[i1].Scale < k1);
                    do
                    {
                        k += 1;
                    }
                    while (models[k].Scale > k1);
                    if (k < i1)
                    {
                        CameraModel l2 = models[k];
                        models[k] = models[i1];
                        models[i1] = l2;
                    }
                }
                SortAndRenderModels(models, startIndex, i1);
                SortAndRenderModels(models, i1 + 1, endIndex);
            }
        }

        public void RenderCameraModel(int priority, CameraModel[] models, int index)
        {
            for (int k = 0; k <= index; k += 1)
            {
                models[k].isSorted = false;
                models[k].sortIndex = k;
                models[k].dependencyIndex = -1;
            }

            int i1 = 0;
            do
            {
                while (models[i1].isSorted)
                {
                    i1 += 1;
                }

                if (i1 == index)
                {
                    return;
                }

                CameraModel l1 = models[i1];
                l1.isSorted = true;
                int j1 = i1;
                int k1 = i1 + priority;
                if (k1 >= index)
                {
                    k1 = index - 1;
                }

                for (int i2 = k1; i2 >= j1 + 1; i2 -= 1)
                {
                    CameraModel l2 = models[i2];
                    if (l1.boundsMinX < l2.boundsMaxX && l2.boundsMinX < l1.boundsMaxX && l1.boundsMinY < l2.boundsMaxY && l2.boundsMinY < l1.boundsMaxY && l1.sortIndex != l2.dependencyIndex && !AreBoundsDisjoint(l1, l2) && IsModelBehind(l2, l1))
                    {
                        HasVisiblePolygons(models, j1, i2);
                        if (models[i2] != l2)
                        {
                            i2 += 1;
                        }

                        j1 = sortRangeStart;
                        l2.dependencyIndex = l1.sortIndex;
                    }
                }

            } while (true);
        }

        public bool HasVisiblePolygons(CameraModel[] models, int start, int stop)
        {
            do
            {
                CameraModel l1 = models[start];
                for (int k = start + 1; k <= stop; k += 1)
                {
                    CameraModel l2 = models[k];
                    if (!AreBoundsDisjoint(l2, l1))
                    {
                        break;
                    }

                    models[start] = l2;
                    models[k] = l1;
                    start = k;
                    if (start == stop)
                    {
                        sortRangeStart = start;
                        sortRangeEnd = start - 1;
                        return true;
                    }
                }

                CameraModel l3 = models[stop];
                for (int i1 = stop - 1; i1 >= start; i1 -= 1)
                {
                    CameraModel l4 = models[i1];
                    if (!AreBoundsDisjoint(l3, l4))
                    {
                        break;
                    }

                    models[stop] = l4;
                    models[i1] = l3;
                    stop = i1;
                    if (start == stop)
                    {
                        sortRangeStart = stop + 1;
                        sortRangeEnd = stop;
                        return true;
                    }
                }

                if (start + 1 >= stop)
                {
                    sortRangeStart = start;
                    sortRangeEnd = stop;
                    return false;
                }
                if (!HasVisiblePolygons(models, start + 1, stop))
                {
                    sortRangeStart = start;
                    return false;
                }
                stop = sortRangeEnd;
            } while (true);
        }

        public void SetViewAngle(int viewX, int viewY, int viewZ)
        {
            int k1 = -cameraOffsetX + 1024 & 0x3ff;
            int l1 = -cameraOffsetY + 1024 & 0x3ff;
            int i2 = -cameraOffsetZ + 1024 & 0x3ff;
            if (i2 != 0)
            {
                int j2 = trigonometryTable[i2];
                int i3 = trigonometryTable[i2 + 1024];
                int l3 = viewY * j2 + viewX * i3 >> 15;
                viewY = viewY * i3 - viewX * j2 >> 15;
                viewX = l3;
            }
            if (k1 != 0)
            {
                int k2 = trigonometryTable[k1];
                int j3 = trigonometryTable[k1 + 1024];
                int i4 = viewY * j3 - viewZ * k2 >> 15;
                viewZ = viewY * k2 + viewZ * j3 >> 15;
                viewY = i4;
            }
            if (l1 != 0)
            {
                int l2 = trigonometryTable[l1];
                int k3 = trigonometryTable[l1 + 1024];
                int j4 = viewZ * l2 + viewX * k3 >> 15;
                viewZ = viewZ * k3 - viewX * l2 >> 15;
                viewX = j4;
            }
            if (viewX < nearX)
            {
                nearX = viewX;
            }

            if (viewX > farX)
            {
                farX = viewX;
            }

            if (viewY < nearY)
            {
                nearY = viewY;
            }

            if (viewY > farY)
            {
                farY = viewY;
            }

            if (viewZ < nearZ)
            {
                nearZ = viewZ;
            }

            if (viewZ > farZ)
            {
                farZ = viewZ;
            }
        }

        public void FinishCamera()
        {
            isRenderingInterlaced = gameImage.interlace;
            int k4 = screenCentreX * zoom1 >> screenProjectionShift;
            int l4 = screenCentreY * zoom1 >> screenProjectionShift;
            nearX = 0;
            farX = 0;
            nearY = 0;
            farY = 0;
            nearZ = 0;
            farZ = 0;
            SetViewAngle(-k4, -l4, zoom1);
            SetViewAngle(-k4, l4, zoom1);
            SetViewAngle(k4, -l4, zoom1);
            SetViewAngle(k4, l4, zoom1);
            SetViewAngle(-screenCentreX, -screenCentreY, 0);
            SetViewAngle(-screenCentreX, screenCentreY, 0);
            SetViewAngle(screenCentreX, -screenCentreY, 0);
            SetViewAngle(screenCentreX, screenCentreY, 0);
            nearX += viewX;
            farX += viewX;
            nearY += ViewY;
            farY += ViewY;
            nearZ += ViewZ;
            farZ += ViewZ;
            objectCache[currentObjectCount] = highlightedObject;
            highlightedObject.objectState = 2;
            for (int k1 = 0; k1 < currentObjectCount; k1 += 1)
            {
                objectCache[k1]?.ProjectWithRotation(viewX, ViewY, ViewZ, cameraOffsetX, cameraOffsetY, cameraOffsetZ, screenProjectionShift, nearPlane);
            }

            int msSlept = 0;
            while (objectCache[currentObjectCount] is null)
            {
                System.Threading.Thread.Sleep(10);
                msSlept += 10;
                if (msSlept > 1000)
                {
                    return;
                }
            }

            objectCache[currentObjectCount].ProjectWithRotation(viewX, ViewY, ViewZ, cameraOffsetX, cameraOffsetY, cameraOffsetZ, screenProjectionShift, nearPlane);
            currentModelIndex = 0;
            for (int i5 = 0; i5 < currentObjectCount; i5 += 1)
            {
                GameObject k = objectCache[i5];
                if (k is null)
                {
                    continue;
                }

                if (k.visible)
                {
                    for (int l1 = 0; l1 < k.face_count; l1 += 1)
                    {
                        int j5 = k.face_vertices_count[l1];
                        int[] ai1 = k.face_vertices[l1];
                        bool flag = false;
                        for (int i6 = 0; i6 < j5; i6 += 1)
                        {
                            int k2 = k.projectedDepth[ai1[i6]];
                            if (k2 <= nearPlane || k2 >= zoom1)
                            {
                                continue;
                            }

                            flag = true;
                            break;
                        }

                        if (flag)
                        {
                            int j3 = 0;
                            for (int j7 = 0; j7 < j5; j7 += 1)
                            {
                                int l2 = k.projectedU[ai1[j7]];
                                if (l2 > -screenCentreX)
                                {
                                    j3 |= 1;
                                }

                                if (l2 < screenCentreX)
                                {
                                    j3 |= 2;
                                }

                                if (j3 == 3)
                                {
                                    break;
                                }
                            }

                            if (j3 == 3)
                            {
                                int k3 = 0;
                                for (int k8 = 0; k8 < j5; k8 += 1)
                                {
                                    int i3 = k.projectedV[ai1[k8]];
                                    if (i3 > -screenCentreY)
                                    {
                                        k3 |= 1;
                                    }

                                    if (i3 < screenCentreY)
                                    {
                                        k3 |= 2;
                                    }

                                    if (k3 == 3)
                                    {
                                        break;
                                    }
                                }

                                if (k3 == 3)
                                {
                                    CameraModel l9 = visibleModels[currentModelIndex];
                                    l9.Object = k;
                                    l9.faceVertCountIndex1 = l1;
                                    UpdateModelAtIndex(currentModelIndex);
                                    int textureIndex;
                                    if (l9.visibilityDot < 0)
                                    {
                                        textureIndex = k.texture_back[l1];
                                    }
                                    else
                                    {
                                        textureIndex = k.texture_front[l1];
                                    }

                                    if (textureIndex != 0xbc614e)
                                    {
                                        int l3 = 0;
                                        for (int l11 = 0; l11 < j5; l11 += 1)
                                        {
                                            l3 += k.projectedDepth[ai1[l11]];
                                        }

                                        l9.Scale = l3 / j5 + k.scaleBias;
                                        l9.currentTextureIndex = textureIndex;
                                        currentModelIndex += 1;
                                    }
                                }
                            }
                        }
                    }

                }
            }

            GameObject i1 = highlightedObject;
            if (i1.visible)
            {
                for (int i2 = 0; i2 < i1.face_count; i2 += 1)
                {
                    int[] ai = i1.face_vertices[i2];
                    int l5 = ai[0];
                    int j6 = i1.projectedU[l5];
                    int k7 = i1.projectedV[l5];
                    int l8 = i1.projectedDepth[l5];
                    if (l8 > nearPlane && l8 < zoom2)
                    {
                        int i10 = (sceneObjectWidths[i2] << screenProjectionShift) / l8;
                        int i11 = (sceneObjectHeights[i2] << screenProjectionShift) / l8;
                        if (j6 - i10 / 2 <= screenCentreX && j6 + i10 / 2 >= -screenCentreX && k7 - i11 <= screenCentreY && k7 >= -screenCentreY)
                        {
                            CameraModel l12 = visibleModels[currentModelIndex];
                            l12.Object = i1;
                            l12.faceVertCountIndex1 = i2;
                            RemoveModelAtIndex(currentModelIndex);
                            l12.Scale = (l8 + i1.projectedDepth[ai[1]]) / 2;
                            currentModelIndex += 1;
                        }
                    }
                }

            }
            if (currentModelIndex == 0)
            {
                return;
            }

            savedModelIndex = currentModelIndex;
            SortAndRenderModels(visibleModels, 0, currentModelIndex - 1);
            RenderCameraModel(100, visibleModels, currentModelIndex);
            for (int k5 = 0; k5 < currentModelIndex; k5 += 1)
            {
                CameraModel l6 = visibleModels[k5];
                GameObject model = l6.Object;
                int j2 = l6.faceVertCountIndex1;
                if (model == highlightedObject)
                {
                    int[] ai2 = model.face_vertices[j2];
                    int l7 = ai2[0];
                    int i9 = model.projectedU[l7];
                    int j10 = model.projectedV[l7];
                    int j11 = model.projectedDepth[l7];
                    int i12 = (sceneObjectWidths[j2] << screenProjectionShift) / j11;
                    int k12 = (sceneObjectHeights[j2] << screenProjectionShift) / j11;
                    int j13 = j10 - model.projectedV[ai2[1]];
                    int k13 = (model.projectedU[ai2[1]] - i9) * j13 / k12;
                    k13 = model.projectedU[ai2[1]] - i9;
                    int i14 = i9 - i12 / 2;
                    int k14 = scanlineBufferCentre + j10 - k12;
                    gameImage.DrawVisibleEntity(i14 + screenMouseOffsetX, k14, i12, k12, sceneObjectId[j2], k13, (256 << screenProjectionShift) / j11);
                    if (isMousePositionUpdated && optionCount < maxHighlightedObjects)
                    {
                        i14 += (sceneObjectFrames[j2] << screenProjectionShift) / j11;
                        if (mouseAdjustedY >= k14 && mouseAdjustedY <= k14 + k12 && mouseAdjustedX >= i14 && mouseAdjustedX <= i14 + i12 && !model.shareEntityArrays && model.polygonTypeData[j2] == 0)
                        {
                            _highlightedObjects[optionCount] = model;
                            highlightedPlayerIds[optionCount] = j2;
                            optionCount += 1;
                        }
                    }
                }
                else
                {
                    int k10 = 0;
                    int j12 = 0;
                    int vertCount = model.face_vertices_count[j2];
                    int[] ai3 = model.face_vertices[j2];
                    if (model.gouraud_shade[j2] != 0xbc614e)
                    {
                        if (l6.visibilityDot < 0)
                        {
                            j12 = model.baseShadeLevel - model.gouraud_shade[j2];
                        }
                        else
                        {
                            j12 = model.baseShadeLevel + model.gouraud_shade[j2];
                        }
                    }

                    for (int l13 = 0; l13 < vertCount; l13 += 1)
                    {
                        int i4 = ai3[l13];
                        vertX[l13] = model.projectedX[i4];
                        vertY[l13] = model.projectedY[i4];
                        vertZ[l13] = model.projectedDepth[i4];
                        if (model.gouraud_shade[j2] == 0xbc614e)
                        {
                            if (l6.visibilityDot < 0)
                            {
                                j12 = model.baseShadeLevel - model.faceNormalComponent[i4] + model.vertexColor[i4];
                            }
                            else
                            {
                                j12 = model.baseShadeLevel + model.faceNormalComponent[i4] + model.vertexColor[i4];
                            }
                        }

                        if (model.projectedDepth[i4] >= nearPlane)
                        {
                            clipPolygonX[k10] = model.projectedU[i4];
                            clipPolygonY[k10] = model.projectedV[i4];
                            clipPolygonValues[k10] = j12;
                            if (model.projectedDepth[i4] > zoom4)
                            {
                                clipPolygonValues[k10] += (model.projectedDepth[i4] - zoom4) / zoom3;
                            }

                            k10 += 1;
                        }
                        else
                        {
                            int k11;
                            if (l13 == 0)
                            {
                                k11 = ai3[vertCount - 1];
                            }
                            else
                            {
                                k11 = ai3[l13 - 1];
                            }

                            if (model.projectedDepth[k11] >= nearPlane)
                            {
                                int j9 = model.projectedDepth[i4] - model.projectedDepth[k11];
                                int k6 = model.projectedX[i4] - (model.projectedX[i4] - model.projectedX[k11]) * (model.projectedDepth[i4] - nearPlane) / j9;
                                int i8 = model.projectedY[i4] - (model.projectedY[i4] - model.projectedY[k11]) * (model.projectedDepth[i4] - nearPlane) / j9;
                                clipPolygonX[k10] = (k6 << screenProjectionShift) / nearPlane;
                                clipPolygonY[k10] = (i8 << screenProjectionShift) / nearPlane;
                                clipPolygonValues[k10] = j12;
                                k10 += 1;
                            }
                            if (l13 == vertCount - 1)
                            {
                                k11 = ai3[0];
                            }
                            else
                            {
                                k11 = ai3[l13 + 1];
                            }

                            if (model.projectedDepth[k11] >= nearPlane)
                            {
                                int k9 = model.projectedDepth[i4] - model.projectedDepth[k11];
                                int i7 = model.projectedX[i4] - (model.projectedX[i4] - model.projectedX[k11]) * (model.projectedDepth[i4] - nearPlane) / k9;
                                int j8 = model.projectedY[i4] - (model.projectedY[i4] - model.projectedY[k11]) * (model.projectedDepth[i4] - nearPlane) / k9;
                                clipPolygonX[k10] = (i7 << screenProjectionShift) / nearPlane;
                                clipPolygonY[k10] = (j8 << screenProjectionShift) / nearPlane;
                                clipPolygonValues[k10] = j12;
                                k10 += 1;
                            }
                        }
                    }

                    for (int j14 = 0; j14 < vertCount; j14 += 1)
                    {
                        if (clipPolygonValues[j14] < 0)
                        {
                            clipPolygonValues[j14] = 0;
                        }
                        else
                            if (clipPolygonValues[j14] > 255)
                        {
                            clipPolygonValues[j14] = 255;
                        }

                        if (l6.currentTextureIndex >= 0)
                        {
                            if (textureLastAccessFrame[l6.currentTextureIndex] == 1)
                            {
                                clipPolygonValues[j14] <<= 9;
                            }
                            else
                            {
                                clipPolygonValues[j14] <<= 6;
                            }
                        }
                    }

                    RenderPolygon(0, 0, 0, 0, k10, clipPolygonX, clipPolygonY, clipPolygonValues, model, j2);
                    if (maxVisibleScanline > minVisibleScanline)
                    {
                        RenderModel(0, 0, vertCount, vertX, vertY, vertZ, l6.currentTextureIndex, model);
                    }
                }
            }

            isMousePositionUpdated = false;
        }

        private void RenderPolygon(int polygonFlags, int shadingMode, int textureFlags, int colourShift, int polygonIndex, int[] polygonX, int[] polygonY,
                int[] polygonZ, GameObject arg8, int arg9)
        {
            if (polygonIndex == 3)
            {
                int k = polygonY[0] + scanlineBufferCentre;
                int l1 = polygonY[1] + scanlineBufferCentre;
                int l2 = polygonY[2] + scanlineBufferCentre;
                int l3 = polygonX[0];
                int i5 = polygonX[1];
                int k6 = polygonX[2];
                int i8 = polygonZ[0];
                int k9 = polygonZ[1];
                int k10 = polygonZ[2];
                int k11 = scanlineBufferCentre + screenCentreY - 1;
                int i12 = 0;
                int k12 = 0;
                int i13 = 0;
                int k13 = 0;
                int i14 = 0xbc614e;
                int k14 = -i14;//0xff439eb2;
                if (l2 != k)
                {
                    k12 = (k6 - l3 << 8) / (l2 - k);
                    k13 = (k10 - i8 << 8) / (l2 - k);
                    if (k < l2)
                    {
                        i12 = l3 << 8;
                        i13 = i8 << 8;
                        i14 = k;
                        k14 = l2;
                    }
                    else
                    {
                        i12 = k6 << 8;
                        i13 = k10 << 8;
                        i14 = l2;
                        k14 = k;
                    }
                    if (i14 < 0)
                    {
                        i12 -= k12 * i14;
                        i13 -= k13 * i14;
                        i14 = 0;
                    }
                    if (k14 > k11)
                    {
                        k14 = k11;
                    }
                }
                int i15 = 0;
                int k15 = 0;
                int i16 = 0;
                int k16 = 0;
                int i17 = 0xbc614e;
                int k17 = -i17;//0xff439eb2;
                if (l1 != k)
                {
                    k15 = (i5 - l3 << 8) / (l1 - k);
                    k16 = (k9 - i8 << 8) / (l1 - k);
                    if (k < l1)
                    {
                        i15 = l3 << 8;
                        i16 = i8 << 8;
                        i17 = k;
                        k17 = l1;
                    }
                    else
                    {
                        i15 = i5 << 8;
                        i16 = k9 << 8;
                        i17 = l1;
                        k17 = k;
                    }
                    if (i17 < 0)
                    {
                        i15 -= k15 * i17;
                        i16 -= k16 * i17;
                        i17 = 0;
                    }
                    if (k17 > k11)
                    {
                        k17 = k11;
                    }
                }
                int i18 = 0;
                int k18 = 0;
                int i19 = 0;
                int k19 = 0;
                int i20 = 0xbc614e;
                int k20 = -i20;//0xff439eb2;
                if (l2 != l1)
                {
                    k18 = (k6 - i5 << 8) / (l2 - l1);
                    k19 = (k10 - k9 << 8) / (l2 - l1);
                    if (l1 < l2)
                    {
                        i18 = i5 << 8;
                        i19 = k9 << 8;
                        i20 = l1;
                        k20 = l2;
                    }
                    else
                    {
                        i18 = k6 << 8;
                        i19 = k10 << 8;
                        i20 = l2;
                        k20 = l1;
                    }
                    if (i20 < 0)
                    {
                        i18 -= k18 * i20;
                        i19 -= k19 * i20;
                        i20 = 0;
                    }
                    if (k20 > k11)
                    {
                        k20 = k11;
                    }
                }
                minVisibleScanline = i14;
                if (i17 < minVisibleScanline)
                {
                    minVisibleScanline = i17;
                }

                if (i20 < minVisibleScanline)
                {
                    minVisibleScanline = i20;
                }

                maxVisibleScanline = k14;
                if (k17 > maxVisibleScanline)
                {
                    maxVisibleScanline = k17;
                }

                if (k20 > maxVisibleScanline)
                {
                    maxVisibleScanline = k20;
                }

                int i21 = 0;
                for (textureFlags = minVisibleScanline; textureFlags < maxVisibleScanline; textureFlags += 1)
                {
                    if (textureFlags >= i14 && textureFlags < k14)
                    {
                        polygonFlags = shadingMode = i12;
                        colourShift = i21 = i13;
                        i12 += k12;
                        i13 += k13;
                    }
                    else
                    {
                        polygonFlags = 0xa0000;
                        shadingMode = unchecked((int)0xfff60000);
                    }
                    if (textureFlags >= i17 && textureFlags < k17)
                    {
                        if (i15 < polygonFlags)
                        {
                            polygonFlags = i15;
                            colourShift = i16;
                        }
                        if (i15 > shadingMode)
                        {
                            shadingMode = i15;
                            i21 = i16;
                        }
                        i15 += k15;
                        i16 += k16;
                    }
                    if (textureFlags >= i20 && textureFlags < k20)
                    {
                        if (i18 < polygonFlags)
                        {
                            polygonFlags = i18;
                            colourShift = i19;
                        }
                        if (i18 > shadingMode)
                        {
                            shadingMode = i18;
                            i21 = i19;
                        }
                        i18 += k18;
                        i19 += k19;
                    }
                    CameraVariable m7 = scanlineVariables[textureFlags];
                    m7.scanlineMinX = polygonFlags;
                    m7.scanlineMaxX = shadingMode;
                    m7.scanlineMinValue = colourShift;
                    m7.scanlineMaxValue = i21;
                }

                if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }
            }
            else
                if (polygonIndex == 4)
                {
                    int i1 = polygonY[0] + scanlineBufferCentre;
                    int i2 = polygonY[1] + scanlineBufferCentre;
                    int i3 = polygonY[2] + scanlineBufferCentre;
                    int i4 = polygonY[3] + scanlineBufferCentre;
                    int j5 = polygonX[0];
                    int l6 = polygonX[1];
                    int j8 = polygonX[2];
                    int l9 = polygonX[3];
                    int l10 = polygonZ[0];
                    int l11 = polygonZ[1];
                    int j12 = polygonZ[2];
                    int l12 = polygonZ[3];
                    int j13 = scanlineBufferCentre + screenCentreY - 1;
                    int l13 = 0;
                    int j14 = 0;
                    int l14 = 0;
                    int j15 = 0;
                    int l15 = 0xbc614e;
                    int j16 = -l15;// 0xff439eb2;
                    if (i4 != i1)
                    {
                        j14 = (l9 - j5 << 8) / (i4 - i1);
                        j15 = (l12 - l10 << 8) / (i4 - i1);
                        if (i1 < i4)
                        {
                            l13 = j5 << 8;
                            l14 = l10 << 8;
                            l15 = i1;
                            j16 = i4;
                        }
                        else
                        {
                            l13 = l9 << 8;
                            l14 = l12 << 8;
                            l15 = i4;
                            j16 = i1;
                        }
                        if (l15 < 0)
                        {
                            l13 -= j14 * l15;
                            l14 -= j15 * l15;
                            l15 = 0;
                        }
                        if (j16 > j13)
                    {
                        j16 = j13;
                    }
                }
                    int l16 = 0;
                    int j17 = 0;
                    int l17 = 0;
                    int j18 = 0;
                    int l18 = 0xbc614e;
                    int j19 = -l18;//0xff439eb2;
                    if (i2 != i1)
                    {
                        j17 = (l6 - j5 << 8) / (i2 - i1);
                        j18 = (l11 - l10 << 8) / (i2 - i1);
                        if (i1 < i2)
                        {
                            l16 = j5 << 8;
                            l17 = l10 << 8;
                            l18 = i1;
                            j19 = i2;
                        }
                        else
                        {
                            l16 = l6 << 8;
                            l17 = l11 << 8;
                            l18 = i2;
                            j19 = i1;
                        }
                        if (l18 < 0)
                        {
                            l16 -= j17 * l18;
                            l17 -= j18 * l18;
                            l18 = 0;
                        }
                        if (j19 > j13)
                    {
                        j19 = j13;
                    }
                }
                    int l19 = 0;
                    int j20 = 0;
                    int l20 = 0;
                    int j21 = 0;
                    int k21 = 0xbc614e;
                    int l21 = -k21;//0xff439eb2;
                    if (i3 != i2)
                    {
                        j20 = (j8 - l6 << 8) / (i3 - i2);
                        j21 = (j12 - l11 << 8) / (i3 - i2);
                        if (i2 < i3)
                        {
                            l19 = l6 << 8;
                            l20 = l11 << 8;
                            k21 = i2;
                            l21 = i3;
                        }
                        else
                        {
                            l19 = j8 << 8;
                            l20 = j12 << 8;
                            k21 = i3;
                            l21 = i2;
                        }
                        if (k21 < 0)
                        {
                            l19 -= j20 * k21;
                            l20 -= j21 * k21;
                            k21 = 0;
                        }
                        if (l21 > j13)
                    {
                        l21 = j13;
                    }
                }
                    int i22 = 0;
                    int j22 = 0;
                    int k22 = 0;
                    int l22 = 0;
                    int i23 = 0xbc614e;
                    int j23 = -i23;//0xff439eb2;
                    if (i4 != i3)
                    {
                        j22 = (l9 - j8 << 8) / (i4 - i3);
                        l22 = (l12 - j12 << 8) / (i4 - i3);
                        if (i3 < i4)
                        {
                            i22 = j8 << 8;
                            k22 = j12 << 8;
                            i23 = i3;
                            j23 = i4;
                        }
                        else
                        {
                            i22 = l9 << 8;
                            k22 = l12 << 8;
                            i23 = i4;
                            j23 = i3;
                        }
                        if (i23 < 0)
                        {
                            i22 -= j22 * i23;
                            k22 -= l22 * i23;
                            i23 = 0;
                        }
                        if (j23 > j13)
                    {
                        j23 = j13;
                    }
                }
                    minVisibleScanline = l15;
                    if (l18 < minVisibleScanline)
                {
                    minVisibleScanline = l18;
                }

                if (k21 < minVisibleScanline)
                {
                    minVisibleScanline = k21;
                }

                if (i23 < minVisibleScanline)
                {
                    minVisibleScanline = i23;
                }

                maxVisibleScanline = j16;
                    if (j19 > maxVisibleScanline)
                {
                    maxVisibleScanline = j19;
                }

                if (l21 > maxVisibleScanline)
                {
                    maxVisibleScanline = l21;
                }

                if (j23 > maxVisibleScanline)
                {
                    maxVisibleScanline = j23;
                }

                int k23 = 0;
                    for (textureFlags = minVisibleScanline; textureFlags < maxVisibleScanline; textureFlags += 1)
                    {
                        if (textureFlags >= l15 && textureFlags < j16)
                        {
                            polygonFlags = shadingMode = l13;
                            colourShift = k23 = l14;
                            l13 += j14;
                            l14 += j15;
                        }
                        else
                        {
                            polygonFlags = 0xa0000;
                            shadingMode = unchecked((int)0xfff60000);//0xfff60000;
                        }
                        if (textureFlags >= l18 && textureFlags < j19)
                        {
                            if (l16 < polygonFlags)
                            {
                                polygonFlags = l16;
                                colourShift = l17;
                            }
                            if (l16 > shadingMode)
                            {
                                shadingMode = l16;
                                k23 = l17;
                            }
                            l16 += j17;
                            l17 += j18;
                        }
                        if (textureFlags >= k21 && textureFlags < l21)
                        {
                            if (l19 < polygonFlags)
                            {
                                polygonFlags = l19;
                                colourShift = l20;
                            }
                            if (l19 > shadingMode)
                            {
                                shadingMode = l19;
                                k23 = l20;
                            }
                            l19 += j20;
                            l20 += j21;
                        }
                        if (textureFlags >= i23 && textureFlags < j23)
                        {
                            if (i22 < polygonFlags)
                            {
                                polygonFlags = i22;
                                colourShift = k22;
                            }
                            if (i22 > shadingMode)
                            {
                                shadingMode = i22;
                                k23 = k22;
                            }
                            i22 += j22;
                            k22 += l22;
                        }
                        CameraVariable m8 = scanlineVariables[textureFlags];
                        m8.scanlineMinX = polygonFlags;
                        m8.scanlineMaxX = shadingMode;
                        m8.scanlineMinValue = colourShift;
                        m8.scanlineMaxValue = k23;
                    }

                    if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }
            }
                else
                {
                    maxVisibleScanline = minVisibleScanline = polygonY[0] += scanlineBufferCentre;
                    for (textureFlags = 1; textureFlags < polygonIndex; textureFlags += 1)
                    {
                        int j1;
                        if ((j1 = polygonY[textureFlags] += scanlineBufferCentre) < minVisibleScanline)
                    {
                        minVisibleScanline = j1;
                    }
                    else
                            if (j1 > maxVisibleScanline)
                    {
                        maxVisibleScanline = j1;
                    }
                }

                    if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }

                if (maxVisibleScanline >= scanlineBufferCentre + screenCentreY)
                {
                    maxVisibleScanline = scanlineBufferCentre + screenCentreY - 1;
                }

                if (minVisibleScanline >= maxVisibleScanline)
                {
                    return;
                }

                for (textureFlags = minVisibleScanline; textureFlags < maxVisibleScanline; textureFlags += 1)
                    {
                        CameraVariable m1 = scanlineVariables[textureFlags];
                        m1.scanlineMinX = 0xa0000;
                        m1.scanlineMaxX = unchecked((int)0xfff60000);//0xfff60000;
                    }

                    int k1 = polygonIndex - 1;
                    int j2 = polygonY[0];
                    int j3 = polygonY[k1];
                    if (j2 < j3)
                    {
                        int j4 = polygonX[0] << 8;
                        int k5 = ((polygonX[k1] - polygonX[0]) << 8) / (j3 - j2);
                        int i7 = polygonZ[0] << 8;
                        int k8 = ((polygonZ[k1] - polygonZ[0]) << 8) / (j3 - j2);
                        if (j2 < 0)
                        {
                            j4 -= k5 * j2;
                            i7 -= k8 * j2;
                            j2 = 0;
                        }
                        if (j3 > maxVisibleScanline)
                    {
                        j3 = maxVisibleScanline;
                    }

                    for (textureFlags = j2; textureFlags <= j3; textureFlags += 1)
                        {
                            CameraVariable m3 = scanlineVariables[textureFlags];
                            m3.scanlineMinX = m3.scanlineMaxX = j4;
                            m3.scanlineMinValue = m3.scanlineMaxValue = i7;
                            j4 += k5;
                            i7 += k8;
                        }

                    }
                    else
                        if (j2 > j3)
                        {
                            int k4 = polygonX[k1] << 8;
                            int l5 = (polygonX[0] - polygonX[k1] << 8) / (j2 - j3);
                            int j7 = polygonZ[k1] << 8;
                            int l8 = (polygonZ[0] - polygonZ[k1] << 8) / (j2 - j3);
                            if (j3 < 0)
                            {
                                k4 -= l5 * j3;
                                j7 -= l8 * j3;
                                j3 = 0;
                            }
                            if (j2 > maxVisibleScanline)
                    {
                        j2 = maxVisibleScanline;
                    }

                    for (textureFlags = j3; textureFlags <= j2; textureFlags += 1)
                            {
                                CameraVariable m4 = scanlineVariables[textureFlags];
                                m4.scanlineMinX = m4.scanlineMaxX = k4;
                                m4.scanlineMinValue = m4.scanlineMaxValue = j7;
                                k4 += l5;
                                j7 += l8;
                            }

                        }
                    for (textureFlags = 0; textureFlags < k1; textureFlags += 1)
                    {
                        int l4 = textureFlags + 1;
                        int k2 = polygonY[textureFlags];
                        int k3 = polygonY[l4];
                        if (k2 < k3)
                        {
                            int i6 = polygonX[textureFlags] << 8;
                            int k7 = (polygonX[l4] - polygonX[textureFlags] << 8) / (k3 - k2);
                            int i9 = polygonZ[textureFlags] << 8;
                            int i10 = (polygonZ[l4] - polygonZ[textureFlags] << 8) / (k3 - k2);
                            if (k2 < 0)
                            {
                                i6 -= k7 * k2;
                                i9 -= i10 * k2;
                                k2 = 0;
                            }
                            if (k3 > maxVisibleScanline)
                        {
                            k3 = maxVisibleScanline;
                        }

                        for (int i11 = k2; i11 <= k3; i11 += 1)
                            {
                                CameraVariable m5 = scanlineVariables[i11];
                                if (i6 < m5.scanlineMinX)
                                {
                                    m5.scanlineMinX = i6;
                                    m5.scanlineMinValue = i9;
                                }
                                if (i6 > m5.scanlineMaxX)
                                {
                                    m5.scanlineMaxX = i6;
                                    m5.scanlineMaxValue = i9;
                                }
                                i6 += k7;
                                i9 += i10;
                            }

                        }
                        else
                            if (k2 > k3)
                            {
                                int j6 = polygonX[l4] << 8;
                                int l7 = (polygonX[textureFlags] - polygonX[l4] << 8) / (k2 - k3);
                                int j9 = polygonZ[l4] << 8;
                                int j10 = (polygonZ[textureFlags] - polygonZ[l4] << 8) / (k2 - k3);
                                if (k3 < 0)
                                {
                                    j6 -= l7 * k3;
                                    j9 -= j10 * k3;
                                    k3 = 0;
                                }
                                if (k2 > maxVisibleScanline)
                        {
                            k2 = maxVisibleScanline;
                        }

                        for (int j11 = k3; j11 <= k2; j11 += 1)
                                {
                                    CameraVariable m6 = scanlineVariables[j11];
                                    if (j6 < m6.scanlineMinX)
                                    {
                                        m6.scanlineMinX = j6;
                                        m6.scanlineMinValue = j9;
                                    }
                                    if (j6 > m6.scanlineMaxX)
                                    {
                                        m6.scanlineMaxX = j6;
                                        m6.scanlineMaxValue = j9;
                                    }
                                    j6 += l7;
                                    j9 += j10;
                                }

                            }
                    }

                    if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }
            }
            if (isMousePositionUpdated && optionCount < maxHighlightedObjects && mouseAdjustedY >= minVisibleScanline && mouseAdjustedY < maxVisibleScanline)
            {
                CameraVariable m2 = scanlineVariables[mouseAdjustedY];
                bool hitX = mouseAdjustedX >= m2.scanlineMinX >> 8 && mouseAdjustedX <= m2.scanlineMaxX >> 8;
                bool hitScanline = m2.scanlineMinX <= m2.scanlineMaxX;
                bool hitShare = !arg8.shareEntityArrays;
                bool hitPoly = hitShare && arg8.polygonTypeData[arg9] == 0;
                if (hitX && hitScanline && hitShare && hitPoly)
                {
                    _highlightedObjects[optionCount] = arg8;
                    highlightedPlayerIds[optionCount] = arg9;
                    optionCount += 1;
                }
            }
        }

        private void RenderModel(int colourMode, int textureMode, int vertCount, int[] vertX, int[] vertY, int[] vertZ, int textureIndex,
                GameObject arg7)
        {
            if (textureIndex == -2)
            {
                return;
            }

            if (textureIndex >= 0)
            {
                if (textureIndex >= textureCount)
                {
                    textureIndex = 0;
                }

                UpdateTextureSmoothing(textureIndex);
                int k = vertX[0];
                int j1 = vertY[0];
                int i2 = vertZ[0];
                int l2 = k - vertX[1];
                int j3 = j1 - vertY[1];
                int l3 = i2 - vertZ[1];
                vertCount -= 1;
                int l5 = vertX[vertCount] - k;
                int i7 = vertY[vertCount] - j1;
                int j8 = vertZ[vertCount] - i2;
                if (textureLastAccessFrame[textureIndex] == 1)
                {
                    int k9 = l5 * j1 - i7 * k << 12;
                    int j10 = i7 * i2 - j8 * j1 << 5 - screenProjectionShift + 7 + 4;
                    int l10 = j8 * k - l5 * i2 << 5 - screenProjectionShift + 7;
                    int j11 = l2 * j1 - j3 * k << 12;
                    int l11 = j3 * i2 - l3 * j1 << 5 - screenProjectionShift + 7 + 4;
                    int j12 = l3 * k - l2 * i2 << 5 - screenProjectionShift + 7;
                    int l12 = j3 * l5 - l2 * i7 << 5;
                    int j13 = l3 * i7 - j3 * j8 << 5 - screenProjectionShift + 4;
                    int l13 = l2 * j8 - l3 * l5 >> screenProjectionShift - 5;
                    int j14 = j10 >> 4;
                    int l14 = l11 >> 4;
                    int j15 = j13 >> 4;
                    int l15 = minVisibleScanline - scanlineBufferCentre;
                    int j16 = defaultScreenHalfWidth;
                    int l16 = screenMouseOffsetX + minVisibleScanline * j16;
                    byte byte1 = 1;
                    k9 += l10 * l15;
                    j11 += j12 * l15;
                    l12 += l13 * l15;
                    if (isRenderingInterlaced)
                    {
                        if ((minVisibleScanline & 1) == 1)
                        {
                            minVisibleScanline += 1;
                            k9 += l10;
                            j11 += j12;
                            l12 += l13;
                            l16 += j16;
                        }
                        l10 <<= 1;
                        j12 <<= 1;
                        l13 <<= 1;
                        j16 <<= 1;
                        byte1 = 2;
                    }
                    if (arg7.isPerspectiveTextured)
                    {
                        for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte1)
                        {
                            CameraVariable m4 = scanlineVariables[colourMode];
                            textureMode = m4.scanlineMinX >> 8;
                            int j17 = m4.scanlineMaxX >> 8;
                            int j20 = j17 - textureMode;
                            if (j20 <= 0)
                            {
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                            else
                            {
                                int l21 = m4.scanlineMinValue;
                                int j23 = (m4.scanlineMaxValue - l21) / j20;
                                if (textureMode < -screenCentreX)
                                {
                                    l21 += (-screenCentreX - textureMode) * j23;
                                    textureMode = -screenCentreX;
                                    j20 = j17 - textureMode;
                                }
                                if (j17 > screenCentreX)
                                {
                                    int k17 = screenCentreX;
                                    j20 = k17 - textureMode;
                                }
                                DrawShadedPolygon(screenPixels, objectTexturePixels[textureIndex], 0, 0, k9 + j14 * textureMode, j11 + l14 * textureMode, l12 + j15 * textureMode, j10, l11, j13, j20, l16 + textureMode, l21, j23 << 2);
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                        }

                        return;
                    }
                    if (!textureIsTransparent[textureIndex])
                    {
                        for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte1)
                        {
                            CameraVariable m5 = scanlineVariables[colourMode];
                            textureMode = m5.scanlineMinX >> 8;
                            int l17 = m5.scanlineMaxX >> 8;
                            int k20 = l17 - textureMode;
                            if (k20 <= 0)
                            {
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                            else
                            {
                                int i22 = m5.scanlineMinValue;
                                int k23 = (m5.scanlineMaxValue - i22) / k20;
                                if (textureMode < -screenCentreX)
                                {
                                    i22 += (-screenCentreX - textureMode) * k23;
                                    textureMode = -screenCentreX;
                                    k20 = l17 - textureMode;
                                }
                                if (l17 > screenCentreX)
                                {
                                    int i18 = screenCentreX;
                                    k20 = i18 - textureMode;
                                }
                                DrawFlatPolygon(screenPixels, objectTexturePixels[textureIndex], 0, 0, k9 + j14 * textureMode, j11 + l14 * textureMode, l12 + j15 * textureMode, j10, l11, j13, k20, l16 + textureMode, i22, k23 << 2);
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                        }

                        return;
                    }
                    for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte1)
                    {
                        CameraVariable m6 = scanlineVariables[colourMode];
                        textureMode = m6.scanlineMinX >> 8;
                        int j18 = m6.scanlineMaxX >> 8;
                        int l20 = j18 - textureMode;
                        if (l20 <= 0)
                        {
                            k9 += l10;
                            j11 += j12;
                            l12 += l13;
                            l16 += j16;
                        }
                        else
                        {
                            int j22 = m6.scanlineMinValue;
                            int l23 = (m6.scanlineMaxValue - j22) / l20;
                            if (textureMode < -screenCentreX)
                            {
                                j22 += (-screenCentreX - textureMode) * l23;
                                textureMode = -screenCentreX;
                                l20 = j18 - textureMode;
                            }
                            if (j18 > screenCentreX)
                            {
                                int k18 = screenCentreX;
                                l20 = k18 - textureMode;
                            }
                            DrawTexturedPolygon(screenPixels, 0, 0, 0, objectTexturePixels[textureIndex], k9 + j14 * textureMode, j11 + l14 * textureMode, l12 + j15 * textureMode, j10, l11, j13, l20, l16 + textureMode, j22, l23);
                            k9 += l10;
                            j11 += j12;
                            l12 += l13;
                            l16 += j16;
                        }
                    }

                    return;
                }
                int l9 = (l5 * j1 - i7 * k) << 11;
                int k10 = (i7 * i2 - j8 * j1) << 5 - screenProjectionShift + 6 + 4;
                int i11 = (j8 * k - l5 * i2) << 5 - screenProjectionShift + 6;
                int k11 = (l2 * j1 - j3 * k) << 11;
                int i12 = (j3 * i2 - l3 * j1) << 5 - screenProjectionShift + 6 + 4;
                int k12 = (l3 * k - l2 * i2) << 5 - screenProjectionShift + 6;
                int i13 = (j3 * l5 - l2 * i7) << 5;
                int k13 = (l3 * i7 - j3 * j8) << 5 - screenProjectionShift + 4;
                int i14 = (l2 * j8 - l3 * l5) >> (screenProjectionShift - 5);
                int k14 = k10 >> 4;
                int i15 = i12 >> 4;
                int k15 = k13 >> 4;
                int i16 = minVisibleScanline - scanlineBufferCentre;
                int k16 = defaultScreenHalfWidth;
                int i17 = screenMouseOffsetX + minVisibleScanline * k16;
                byte byte2 = 1;
                l9 += i11 * i16;
                k11 += k12 * i16;
                i13 += i14 * i16;
                if (isRenderingInterlaced)
                {
                    if ((minVisibleScanline & 1) == 1)
                    {
                        minVisibleScanline += 1;
                        l9 += i11;
                        k11 += k12;
                        i13 += i14;
                        i17 += k16;
                    }
                    i11 <<= 1;
                    k12 <<= 1;
                    i14 <<= 1;
                    k16 <<= 1;
                    byte2 = 2;
                }
                if (arg7.isPerspectiveTextured)
                {
                    for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte2)
                    {
                        CameraVariable m7 = scanlineVariables[colourMode];
                        textureMode = m7.scanlineMinX >> 8;
                        int l18 = m7.scanlineMaxX >> 8;
                        int i21 = l18 - textureMode;
                        if (i21 <= 0)
                        {
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                        else
                        {
                            int k22 = m7.scanlineMinValue;
                            int i24 = (m7.scanlineMaxValue - k22) / i21;
                            if (textureMode < -screenCentreX)
                            {
                                k22 += (-screenCentreX - textureMode) * i24;
                                textureMode = -screenCentreX;
                                i21 = l18 - textureMode;
                            }
                            if (l18 > screenCentreX)
                            {
                                int i19 = screenCentreX;
                                i21 = i19 - textureMode;
                            }
                            DrawMaskedPolygon(screenPixels, objectTexturePixels[textureIndex], 0, 0, l9 + k14 * textureMode, k11 + i15 * textureMode, i13 + k15 * textureMode, k10, i12, k13, i21, i17 + textureMode, k22, i24);
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                    }

                    return;
                }
                if (!textureIsTransparent[textureIndex])
                {
                    for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte2)
                    {
                        CameraVariable m8 = scanlineVariables[colourMode];
                        textureMode = m8.scanlineMinX >> 8;
                        int j19 = m8.scanlineMaxX >> 8;
                        int j21 = j19 - textureMode;
                        if (j21 <= 0)
                        {
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                        else
                        {
                            int l22 = m8.scanlineMinValue;
                            int j24 = (m8.scanlineMaxValue - l22) / j21;
                            if (textureMode < -screenCentreX)
                            {
                                l22 += (-screenCentreX - textureMode) * j24;
                                textureMode = -screenCentreX;
                                j21 = j19 - textureMode;
                            }
                            if (j19 > screenCentreX)
                            {
                                int k19 = screenCentreX;
                                j21 = k19 - textureMode;
                            }
                            DrawTransparentPolygon(screenPixels, objectTexturePixels[textureIndex], 0, 0, l9 + k14 * textureMode, k11 + i15 * textureMode, i13 + k15 * textureMode, k10, i12, k13, j21, i17 + textureMode, l22, j24);
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                    }

                    return;
                }
                for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte2)
                {
                    CameraVariable m9 = scanlineVariables[colourMode];
                    textureMode = m9.scanlineMinX >> 8;
                    int l19 = m9.scanlineMaxX >> 8;
                    int k21 = l19 - textureMode;
                    if (k21 <= 0)
                    {
                        l9 += i11;
                        k11 += k12;
                        i13 += i14;
                        i17 += k16;
                    }
                    else
                    {
                        int i23 = m9.scanlineMinValue;
                        int k24 = (m9.scanlineMaxValue - i23) / k21;
                        if (textureMode < -screenCentreX)
                        {
                            i23 += (-screenCentreX - textureMode) * k24;
                            textureMode = -screenCentreX;
                            k21 = l19 - textureMode;
                        }
                        if (l19 > screenCentreX)
                        {
                            int i20 = screenCentreX;
                            k21 = i20 - textureMode;
                        }
                        DrawFlatTexturedPolygon(screenPixels, 0, 0, 0, objectTexturePixels[textureIndex], l9 + k14 * textureMode, k11 + i15 * textureMode, i13 + k15 * textureMode, k10, i12, k13, k21, i17 + textureMode, i23, k24);
                        l9 += i11;
                        k11 += k12;
                        i13 += i14;
                        i17 += k16;
                    }
                }

                return;
            }
            for (int i1 = 0; i1 < maxTextureCount; i1 += 1)
            {
                if (textureClipIds[i1] == textureIndex)
                {
                    textureClipSizes = textureClipData[i1];
                    break;
                }
                if (i1 == maxTextureCount - 1)
                {
                    double randomValue = Helper.Random.NextDouble();
                    int k1 = (int)(randomValue /*Math.random()*/ * maxTextureCount);
                    if (k1 >= textureClipIds.Length)
                    {
                        k1 -= 1;
                    }

                    textureClipIds[k1] = textureIndex;
                    textureIndex = -1 - textureIndex;
                    int j2 = (textureIndex >> 10 & 0x1f) * 8;
                    int i3 = (textureIndex >> 5 & 0x1f) * 8;
                    int k3 = (textureIndex & 0x1f) * 8;
                    for (int i4 = 0; i4 < 256; i4 += 1)
                    {
                        int i6 = i4 * i4;
                        int j7 = j2 * i6 / 0x10000;
                        int k8 = i3 * i6 / 0x10000;
                        int i10 = k3 * i6 / 0x10000;
                        textureClipData[k1][255 - i4] = (j7 << 16) + (k8 << 8) + i10;
                    }

                    textureClipSizes = textureClipData[k1];
                }
            }

            int l1 = defaultScreenHalfWidth;
            int k2 = screenMouseOffsetX + minVisibleScanline * l1;
            byte byte0 = 1;
            if (isRenderingInterlaced)
            {
                if ((minVisibleScanline & 1) == 1)
                {
                    minVisibleScanline += 1;
                    k2 += l1;
                }
                l1 <<= 1;
                byte0 = 2;
            }
            if (arg7.isGiantCrystal)
            {
                for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte0)
                {
                    CameraVariable m1 = scanlineVariables[colourMode];
                    textureMode = m1.scanlineMinX >> 8;
                    int j4 = m1.scanlineMaxX >> 8;
                    int j6 = j4 - textureMode;
                    if (j6 <= 0)
                    {
                        k2 += l1;
                    }
                    else
                    {
                        int k7 = m1.scanlineMinValue;
                        int l8 = (m1.scanlineMaxValue - k7) / j6;
                        if (textureMode < -screenCentreX)
                        {
                            k7 += (-screenCentreX - textureMode) * l8;
                            textureMode = -screenCentreX;
                            j6 = j4 - textureMode;
                        }
                        if (j4 > screenCentreX)
                        {
                            int k4 = screenCentreX;
                            j6 = k4 - textureMode;
                        }
                        DrawShiftColorPolygon(screenPixels, -j6, k2 + textureMode, 0, textureClipSizes, k7, l8);
                        k2 += l1;
                    }
                }

                return;
            }
            if (isInterlaced)
            {
                for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte0)
                {
                    CameraVariable m2 = scanlineVariables[colourMode];
                    textureMode = m2.scanlineMinX >> 8;
                    int l4 = m2.scanlineMaxX >> 8;
                    int k6 = l4 - textureMode;
                    if (k6 <= 0)
                    {
                        k2 += l1;
                    }
                    else
                    {
                        int l7 = m2.scanlineMinValue;
                        int i9 = (m2.scanlineMaxValue - l7) / k6;
                        if (textureMode < -screenCentreX)
                        {
                            l7 += (-screenCentreX - textureMode) * i9;
                            textureMode = -screenCentreX;
                            k6 = l4 - textureMode;
                        }
                        if (l4 > screenCentreX)
                        {
                            int i5 = screenCentreX;
                            k6 = i5 - textureMode;
                        }
                        DrawVertexColorPolygon(screenPixels, -k6, k2 + textureMode, 0, textureClipSizes, l7, i9);
                        k2 += l1;
                    }
                }

                return;
            }
            for (colourMode = minVisibleScanline; colourMode < maxVisibleScanline; colourMode += byte0)
            {
                CameraVariable m3 = scanlineVariables[colourMode];
                textureMode = m3.scanlineMinX >> 8;
                int j5 = m3.scanlineMaxX >> 8;
                int l6 = j5 - textureMode;
                if (l6 <= 0)
                {
                    k2 += l1;
                }
                else
                {
                    int i8 = m3.scanlineMinValue;
                    int j9 = (m3.scanlineMaxValue - i8) / l6;
                    if (textureMode < -screenCentreX)
                    {
                        i8 += (-screenCentreX - textureMode) * j9;
                        textureMode = -screenCentreX;
                        l6 = j5 - textureMode;
                    }
                    if (j5 > screenCentreX)
                    {
                        int k5 = screenCentreX;
                        l6 = k5 - textureMode;
                    }
                    DrawGradientPolygon(screenPixels, -l6, k2 + textureMode, 0, textureClipSizes, i8, j9);
                    k2 += l1;
                }
            }

        }

        private static void DrawFlatPolygon(int[] pixels, int[] scanlines, int colour, int startScanline, int endScanline, int startPixelX, int destOffset, int scanlineCount,
                int scanlineStep, int interlaceMode, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            int l1 = 0;
            if (destOffset != 0)
            {
                colour = endScanline / destOffset << 7;
                startScanline = startPixelX / destOffset << 7;
            }
            if (colour < 0)
            {
                colour = 0;
            }
            else
                if (colour > 16256)
            {
                colour = 16256;
            }

            endScanline += scanlineCount;
            startPixelX += scanlineStep;
            destOffset += interlaceMode;
            if (destOffset != 0)
            {
                k = endScanline / destOffset << 7;
                i1 = startPixelX / destOffset << 7;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 16256)
            {
                k = 16256;
            }

            int j1 = k - colour >> 4;
            int k1 = i1 - startScanline >> 4;
            for (int i2 = arg10 >> 4; i2 > 0; i2 -= 1)
            {
                colour += arg12 & 0x600000;
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                colour = (colour & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                colour = (colour & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                colour = (colour & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour = k;
                startScanline = i1;
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;
                if (destOffset != 0)
                {
                    k = endScanline / destOffset << 7;
                    i1 = startPixelX / destOffset << 7;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 16256)
                {
                    k = 16256;
                }

                j1 = k - colour >> 4;
                k1 = i1 - startScanline >> 4;
            }

            for (int j2 = 0; j2 < (arg10 & 0xf); j2 += 1)
            {
                if ((j2 & 3) == 0)
                {
                    colour = (colour & 0x3fff) + (arg12 & 0x600000);
                    l1 = arg12 >> 23;
                    arg12 += arg13;
                }
                pixels[arg11++] = scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1;
                colour += j1;
                startScanline += k1;
            }

        }

        private static void DrawShadedPolygon(int[] pixels, int[] scanlines, int colour, int startScanline, int endScanline, int startPixelX, int destOffset, int scanlineCount,
                int scanlineStep, int interlaceMode, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            int l1 = 0;
            if (destOffset != 0)
            {
                colour = endScanline / destOffset << 7;
                startScanline = startPixelX / destOffset << 7;
            }
            if (colour < 0)
            {
                colour = 0;
            }
            else
                if (colour > 16256)
            {
                colour = 16256;
            }

            endScanline += scanlineCount;
            startPixelX += scanlineStep;
            destOffset += interlaceMode;
            if (destOffset != 0)
            {
                k = endScanline / destOffset << 7;
                i1 = startPixelX / destOffset << 7;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 16256)
            {
                k = 16256;
            }

            int j1 = k - colour >> 4;
            int k1 = i1 - startScanline >> 4;
            for (int i2 = arg10 >> 4; i2 > 0; i2 -= 1)
            {
                colour += arg12 & 0x600000;
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                colour = (colour & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                colour = (colour & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                colour = (colour & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour = k;
                startScanline = i1;
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;
                if (destOffset != 0)
                {
                    k = endScanline / destOffset << 7;
                    i1 = startPixelX / destOffset << 7;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 16256)
                {
                    k = 16256;
                }

                j1 = k - colour >> 4;
                k1 = i1 - startScanline >> 4;
            }

            for (int j2 = 0; j2 < (arg10 & 0xf); j2 += 1)
            {
                if ((j2 & 3) == 0)
                {
                    colour = (colour & 0x3fff) + (arg12 & 0x600000);
                    l1 = arg12 >> 23;
                    arg12 += arg13;
                }
                pixels[arg11++] = (scanlines[(startScanline & 0x3f80) + (colour >> 7)] >> l1) + (pixels[arg11] >> 1 & 0x7f7f7f);
                colour += j1;
                startScanline += k1;
            }

        }

        private static void DrawTexturedPolygon(int[] pixels, int texturePixels, int startU, int startV, int[] textureData, int startScanline, int destOffset, int scanlineCount,
                int scanlineStep, int uStep, int vStep, int interlaceMode, int arg12, int arg13, int arg14)
        {
            if (interlaceMode <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg14 <<= 2;
            if (scanlineCount != 0)
            {
                k = startScanline / scanlineCount << 7;
                i1 = destOffset / scanlineCount << 7;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 16256)
            {
                k = 16256;
            }

            for (int l1 = interlaceMode; l1 > 0; l1 -= 16)
            {
                startScanline += scanlineStep;
                destOffset += uStep;
                scanlineCount += vStep;
                startU = k;
                startV = i1;
                if (scanlineCount != 0)
                {
                    k = startScanline / scanlineCount << 7;
                    i1 = destOffset / scanlineCount << 7;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 16256)
                {
                    k = 16256;
                }

                int j1 = k - startU >> 4;
                int k1 = i1 - startV >> 4;
                int i2 = arg13 >> 23;
                startU += arg13 & 0x600000;
                arg13 += arg14;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2 += 1)
                    {
                        if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                        {
                            pixels[arg12] = texturePixels;
                        }

                        arg12 += 1;
                        startU += j1;
                        startV += k1;
                        if ((j2 & 3) == 3)
                        {
                            startU = (startU & 0x3fff) + (arg13 & 0x600000);
                            i2 = arg13 >> 23;
                            arg13 += arg14;
                        }
                    }

                }
                else
                {
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    startU = (startU & 0x3fff) + (arg13 & 0x600000);
                    i2 = arg13 >> 23;
                    arg13 += arg14;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    startU = (startU & 0x3fff) + (arg13 & 0x600000);
                    i2 = arg13 >> 23;
                    arg13 += arg14;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    startU = (startU & 0x3fff) + (arg13 & 0x600000);
                    i2 = arg13 >> 23;
                    arg13 += arg14;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0x3f80) + (startU >> 7)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                }
            }

        }

        private static void DrawTransparentPolygon(int[] pixels, int[] scanlines, int colour, int startScanline, int endScanline, int startPixelX, int destOffset, int scanlineCount,
                int scanlineStep, int interlaceMode, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg13 <<= 2;
            if (destOffset != 0)
            {
                k = endScanline / destOffset << 6;
                i1 = startPixelX / destOffset << 6;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 4032)
            {
                k = 4032;
            }

            for (int l1 = arg10; l1 > 0; l1 -= 16)
            {
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;
                colour = k;
                startScanline = i1;
                if (destOffset != 0)
                {
                    k = endScanline / destOffset << 6;
                    i1 = startPixelX / destOffset << 6;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 4032)
                {
                    k = 4032;
                }

                int j1 = k - colour >> 4;
                int k1 = i1 - startScanline >> 4;
                int i2 = arg12 >> 20;
                colour += arg12 & 0xc0000;
                arg12 += arg13;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2 += 1)
                    {
                        pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                        colour += j1;
                        startScanline += k1;
                        if ((j2 & 3) == 3)
                        {
                            colour = (colour & 0xfff) + (arg12 & 0xc0000);
                            i2 = arg12 >> 20;
                            arg12 += arg13;
                        }
                    }

                }
                else
                {
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    colour = (colour & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    colour = (colour & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    colour = (colour & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2;
                }
            }

        }

        private static void DrawMaskedPolygon(int[] pixels, int[] scanlines, int colour, int startScanline, int endScanline, int startPixelX, int destOffset, int scanlineCount,
                int scanlineStep, int interlaceMode, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg13 <<= 2;
            if (destOffset != 0)
            {
                k = (endScanline / destOffset) << 6;
                i1 = (startPixelX / destOffset) << 6;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 4032)
            {
                k = 4032;
            }

            for (int l1 = arg10; l1 > 0; l1 -= 16)
            {
                endScanline += scanlineCount;
                startPixelX += scanlineStep;
                destOffset += interlaceMode;
                colour = k;
                startScanline = i1;
                if (destOffset != 0)
                {
                    k = endScanline / destOffset << 6;
                    i1 = startPixelX / destOffset << 6;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 4032)
                {
                    k = 4032;
                }

                int j1 = k - colour >> 4;
                int k1 = i1 - startScanline >> 4;
                int i2 = arg12 >> 20;
                colour += arg12 & 0xc0000;
                arg12 += arg13;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2 += 1)
                    {
                        pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                        colour += j1;
                        startScanline += k1;
                        if ((j2 & 3) == 3)
                        {
                            colour = (colour & 0xfff) + (arg12 & 0xc0000);
                            i2 = arg12 >> 20;
                            arg12 += arg13;
                        }
                    }

                }
                else
                {
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    colour = (colour & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    colour = (colour & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    colour = (colour & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                    colour += j1;
                    startScanline += k1;
                    pixels[arg11++] = (scanlines[(startScanline & 0xfc0) + (colour >> 6)] >> i2) + (pixels[arg11] >> 1 & 0x7f7f7f);
                }
            }

        }

        private static void DrawFlatTexturedPolygon(int[] pixels, int texturePixels, int startU, int startV, int[] textureData, int startScanline, int destOffset, int scanlineCount,
                int scanlineStep, int uStep, int vStep, int interlaceMode, int arg12, int arg13, int arg14)
        {
            if (interlaceMode <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg14 <<= 2;
            if (scanlineCount != 0)
            {
                k = startScanline / scanlineCount << 6;
                i1 = destOffset / scanlineCount << 6;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 4032)
            {
                k = 4032;
            }

            for (int l1 = interlaceMode; l1 > 0; l1 -= 16)
            {
                startScanline += scanlineStep;
                destOffset += uStep;
                scanlineCount += vStep;
                startU = k;
                startV = i1;
                if (scanlineCount != 0)
                {
                    k = startScanline / scanlineCount << 6;
                    i1 = destOffset / scanlineCount << 6;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 4032)
                {
                    k = 4032;
                }

                int j1 = k - startU >> 4;
                int k1 = i1 - startV >> 4;
                int i2 = arg13 >> 20;
                startU += arg13 & 0xc0000;
                arg13 += arg14;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2 += 1)
                    {
                        if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                        {
                            pixels[arg12] = texturePixels;
                        }

                        arg12 += 1;
                        startU += j1;
                        startV += k1;
                        if ((j2 & 3) == 3)
                        {
                            startU = (startU & 0xfff) + (arg13 & 0xc0000);
                            i2 = arg13 >> 20;
                            arg13 += arg14;
                        }
                    }

                }
                else
                {
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    startU = (startU & 0xfff) + (arg13 & 0xc0000);
                    i2 = arg13 >> 20;
                    arg13 += arg14;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    startU = (startU & 0xfff) + (arg13 & 0xc0000);
                    i2 = arg13 >> 20;
                    arg13 += arg14;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    startU = (startU & 0xfff) + (arg13 & 0xc0000);
                    i2 = arg13 >> 20;
                    arg13 += arg14;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                    startU += j1;
                    startV += k1;
                    if ((texturePixels = textureData[(startV & 0xfc0) + (startU >> 6)] >> i2) != 0)
                    {
                        pixels[arg12] = texturePixels;
                    }

                    arg12 += 1;
                }
            }

        }

        private static void DrawVertexColorPolygon(int[] pixels, int startColour, int startU, int startV, int[] shadeData, int startScanline, int destOffset)
        {
            if (startColour >= 0)
            {
                return;
            }

            destOffset <<= 1;
            startV = shadeData[startScanline >> 8 & 0xff];
            startScanline += destOffset;
            int k = startColour / 8;
            for (int i1 = k; i1 < 0; i1 += 1)
            {
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
            }

            k = -(startColour % 8);
            for (int j1 = 0; j1 < k; j1 += 1)
            {
                pixels[startU++] = startV;
                if ((j1 & 1) == 1)
                {
                    startV = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }

        }

        private static void DrawShiftColorPolygon(int[] pixels, int startColour, int startU, int startV, int[] shadeData, int startScanline, int destOffset)
        {
            if (startColour >= 0)
            {
                return;
            }

            destOffset <<= 2;
            startV = shadeData[startScanline >> 8 & 0xff];
            startScanline += destOffset;
            int k = startColour / 16;
            for (int i1 = k; i1 < 0; i1 += 1)
            {
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
            }

            k = -(startColour % 16);
            for (int j1 = 0; j1 < k; j1 += 1)
            {
                pixels[startU++] = startV + (pixels[startU] >> 1 & 0x7f7f7f);
                if ((j1 & 3) == 3)
                {
                    startV = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                    startScanline += destOffset;
                }
            }

        }

        private static void DrawGradientPolygon(int[] pixels, int startColour, int startU, int startV, int[] shadeData, int startScanline, int destOffset)
        {
            if (startColour >= 0)
            {
                return;
            }

            destOffset <<= 2;
            startV = shadeData[startScanline >> 8 & 0xff];
            startScanline += destOffset;
            int k = startColour / 16;
            for (int i1 = k; i1 < 0; i1 += 1)
            {
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                pixels[startU++] = startV;
                startV = shadeData[startScanline >> 8 & 0xff];
                startScanline += destOffset;
            }

            k = -(startColour % 16);
            for (int j1 = 0; j1 < k; j1 += 1)
            {
                pixels[startU++] = startV;
                if ((j1 & 3) == 3)
                {
                    startV = shadeData[startScanline >> 8 & 0xff];
                    startScanline += destOffset;
                }
            }

        }

        public void SetCameraTransform(int x, int y, int z, int rotationX, int rotationY, int rotationZ, int distance)
        {
            rotationX &= 0x3ff;
            rotationY &= 0x3ff;
            rotationZ &= 0x3ff;
            cameraOffsetX = 1024 - rotationX & 0x3ff;
            cameraOffsetY = 1024 - rotationY & 0x3ff;
            cameraOffsetZ = 1024 - rotationZ & 0x3ff;
            int xOffset = 0;
            int yOffset = 0;
            int zOffset = distance;
            if (rotationX != 0)
            {
                int j3 = trigonometryTable[rotationX];
                int i4 = trigonometryTable[rotationX + 1024];
                int l4 = yOffset * i4 - zOffset * j3 >> 15;
                zOffset = yOffset * j3 + zOffset * i4 >> 15;
                yOffset = l4;
            }
            if (rotationY != 0)
            {
                int k3 = trigonometryTable[rotationY];
                int j4 = trigonometryTable[rotationY + 1024];
                int i5 = zOffset * k3 + xOffset * j4 >> 15;
                zOffset = zOffset * j4 - xOffset * k3 >> 15;
                xOffset = i5;
            }
            if (rotationZ != 0)
            {
                int l3 = trigonometryTable[rotationZ];
                int k4 = trigonometryTable[rotationZ + 1024];
                int j5 = yOffset * l3 + xOffset * k4 >> 15;
                yOffset = yOffset * k4 - xOffset * l3 >> 15;
                xOffset = j5;
            }
            viewX = x - xOffset;
            ViewY = y - yOffset;
            ViewZ = z - zOffset;
        }

        private void UpdateModelAtIndex(int modelIndex)
        {
            CameraModel l1 = visibleModels[modelIndex];
            GameObject k = l1.Object;
            int i1 = l1.faceVertCountIndex1;
            int[] ai = k.face_vertices[i1];
            int j1 = k.face_vertices_count[i1];
            int k1 = k.faceRenderFlag[i1];
            int j2 = k.projectedX[ai[0]];
            int k2 = k.projectedY[ai[0]];
            int l2 = k.projectedDepth[ai[0]];
            int i3 = k.projectedX[ai[1]] - j2;
            int j3 = k.projectedY[ai[1]] - k2;
            int k3 = k.projectedDepth[ai[1]] - l2;
            int l3 = k.projectedX[ai[2]] - j2;
            int i4 = k.projectedY[ai[2]] - k2;
            int j4 = k.projectedDepth[ai[2]] - l2;
            int xDistance = j3 * j4 - i4 * k3;
            int yDistance = k3 * l3 - j4 * i3;
            int zDistance = i3 * i4 - l3 * j3;
            if (k1 == -1)
            {
                k1 = 0;
                for (; xDistance > 25000 || yDistance > 25000 || zDistance > 25000 || xDistance < -25000 || yDistance < -25000 || zDistance < -25000; zDistance >>= 1)
                {
                    k1 += 1;
                    xDistance >>= 1;
                    yDistance >>= 1;
                }

                k.faceRenderFlag[i1] = k1;
                k.faceVisibility[i1] = (int)(lightingFactor * Math.Sqrt(xDistance * xDistance + yDistance * yDistance + zDistance * zDistance));
            }
            else
            {
                xDistance >>= k1;
                yDistance >>= k1;
                zDistance >>= k1;
            }
            l1.visibilityDot = j2 * xDistance + k2 * yDistance + l2 * zDistance;
            l1.normalX = xDistance;
            l1.normalY = yDistance;
            l1.normalZ = zDistance;
            int j5 = k.projectedDepth[ai[0]];
            int k5 = j5;
            int l5 = k.projectedU[ai[0]];
            int i6 = l5;
            int j6 = k.projectedV[ai[0]];
            int k6 = j6;
            for (int l6 = 1; l6 < j1; l6 += 1)
            {
                int i2 = k.projectedDepth[ai[l6]];
                if (i2 > k5)
                {
                    k5 = i2;
                }
                else
                    if (i2 < j5)
                {
                    j5 = i2;
                }

                i2 = k.projectedU[ai[l6]];
                if (i2 > i6)
                {
                    i6 = i2;
                }
                else
                    if (i2 < l5)
                {
                    l5 = i2;
                }

                i2 = k.projectedV[ai[l6]];
                if (i2 > k6)
                {
                    k6 = i2;
                }
                else
                    if (i2 < j6)
                {
                    j6 = i2;
                }
            }

            l1.boundsMinZ = j5;
            l1.boundsMaxZ = k5;
            l1.boundsMinX = l5;
            l1.boundsMaxX = i6;
            l1.boundsMinY = j6;
            l1.boundsMaxY = k6;
        }

        private void RemoveModelAtIndex(int modelIndex)
        {
            CameraModel l1 = visibleModels[modelIndex];
            GameObject k = l1.Object;
            int i1 = l1.faceVertCountIndex1;
            int[] faceVertices = k.face_vertices[i1];
            int k1 = 0;
            int i2 = 0;
            int j2 = 1;
            int k2 = k.projectedX[faceVertices[0]];
            int l2 = k.projectedY[faceVertices[0]];
            int i3 = k.projectedDepth[faceVertices[0]];
            k.faceVisibility[i1] = 1;
            k.faceRenderFlag[i1] = 0;
            l1.visibilityDot = k2 * k1 + l2 * i2 + i3 * j2;
            l1.normalX = k1;
            l1.normalY = i2;
            l1.normalZ = j2;
            int j3 = k.projectedDepth[faceVertices[0]];
            int k3 = j3;
            int l3 = k.projectedU[faceVertices[0]];
            int i4 = l3;
            if (k.projectedU[faceVertices[1]] < l3)
            {
                l3 = k.projectedU[faceVertices[1]];
            }
            else
            {
                i4 = k.projectedU[faceVertices[1]];
            }

            int j4 = k.projectedV[faceVertices[1]];
            int k4 = k.projectedV[faceVertices[0]];
            int j1 = k.projectedDepth[faceVertices[1]];
            if (j1 > k3)
            {
                k3 = j1;
            }
            else
                if (j1 < j3)
            {
                j3 = j1;
            }

            j1 = k.projectedU[faceVertices[1]];
            if (j1 > i4)
            {
                i4 = j1;
            }
            else
                if (j1 < l3)
            {
                l3 = j1;
            }

            j1 = k.projectedV[faceVertices[1]];
            if (j1 > k4)
            {
                k4 = j1;
            }
            else
                if (j1 < j4)
            {
                j4 = j1;
            }

            l1.boundsMinZ = j3;
            l1.boundsMaxZ = k3;
            l1.boundsMinX = l3 - 20;
            l1.boundsMaxX = i4 + 20;
            l1.boundsMinY = j4;
            l1.boundsMaxY = k4;
        }

        private bool AreBoundsDisjoint(CameraModel modelA, CameraModel modelB)
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

            GameObject k = modelA.Object;
            GameObject i1 = modelB.Object;
            int j1 = modelA.faceVertCountIndex1;
            int k1 = modelB.faceVertCountIndex1;
            int[] ai = k.face_vertices[j1];
            int[] ai1 = i1.face_vertices[k1];
            int l1 = k.face_vertices_count[j1];
            int i2 = i1.face_vertices_count[k1];
            int l3 = i1.projectedX[ai1[0]];
            int i4 = i1.projectedY[ai1[0]];
            int j4 = i1.projectedDepth[ai1[0]];
            int k4 = modelB.normalX;
            int l4 = modelB.normalY;
            int i5 = modelB.normalZ;
            int j5 = i1.faceVisibility[k1];
            int k5 = modelB.visibilityDot;
            bool flag = false;
            for (int l5 = 0; l5 < l1; l5 += 1)
            {
                int j2 = ai[l5];
                int j3 = (l3 - k.projectedX[j2]) * k4 + (i4 - k.projectedY[j2]) * l4 + (j4 - k.projectedDepth[j2]) * i5;
                if ((j3 >= -j5 || k5 >= 0) && (j3 <= j5 || k5 <= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (!flag)
            {
                return true;
            }

            l3 = k.projectedX[ai[0]];
            i4 = k.projectedY[ai[0]];
            j4 = k.projectedDepth[ai[0]];
            k4 = modelA.normalX;
            l4 = modelA.normalY;
            i5 = modelA.normalZ;
            j5 = k.faceVisibility[j1];
            k5 = modelA.visibilityDot;
            flag = false;
            for (int i6 = 0; i6 < i2; i6 += 1)
            {
                int k2 = ai1[i6];
                int k3 = (l3 - i1.projectedX[k2]) * k4 + (i4 - i1.projectedY[k2]) * l4 + (j4 - i1.projectedDepth[k2]) * i5;
                if ((k3 >= -j5 || k5 <= 0) && (k3 <= j5 || k5 >= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (!flag)
            {
                return true;
            }

            int[] ai2;
            int[] ai3;
            if (l1 == 2)
            {
                ai2 = new int[4];
                ai3 = new int[4];
                int j6 = ai[0];
                int l2 = ai[1];
                ai2[0] = k.projectedU[j6] - 20;
                ai2[1] = k.projectedU[l2] - 20;
                ai2[2] = k.projectedU[l2] + 20;
                ai2[3] = k.projectedU[j6] + 20;
                ai3[0] = ai3[3] = k.projectedV[j6];
                ai3[1] = ai3[2] = k.projectedV[l2];
            }
            else
            {
                ai2 = new int[l1];
                ai3 = new int[l1];
                for (int k6 = 0; k6 < l1; k6 += 1)
                {
                    int j7 = ai[k6];
                    ai2[k6] = k.projectedU[j7];
                    ai3[k6] = k.projectedV[j7];
                }

            }
            int[] ai4;
            int[] ai5;
            if (i2 == 2)
            {
                ai4 = new int[4];
                ai5 = new int[4];
                int l6 = ai1[0];
                int i3 = ai1[1];
                ai4[0] = i1.projectedU[l6] - 20;
                ai4[1] = i1.projectedU[i3] - 20;
                ai4[2] = i1.projectedU[i3] + 20;
                ai4[3] = i1.projectedU[l6] + 20;
                ai5[0] = ai5[3] = i1.projectedV[l6];
                ai5[1] = ai5[2] = i1.projectedV[i3];
            }
            else
            {
                ai4 = new int[i2];
                ai5 = new int[i2];
                for (int i7 = 0; i7 < i2; i7 += 1)
                {
                    int k7 = ai1[i7];
                    ai4[i7] = i1.projectedU[k7];
                    ai5[i7] = i1.projectedV[k7];
                }

            }
            return !PolygonsIntersect(ai2, ai3, ai4, ai5);
        }

        private bool IsModelBehind(CameraModel frontModel, CameraModel behindModel)
        {
            GameObject k = frontModel.Object;
            GameObject i1 = behindModel.Object;
            int j1 = frontModel.faceVertCountIndex1;
            int k1 = behindModel.faceVertCountIndex1;
            int[] ai = k.face_vertices[j1];
            int[] ai1 = i1.face_vertices[k1];
            int l1 = k.face_vertices_count[j1];
            int i2 = i1.face_vertices_count[k1];
            int j3 = i1.projectedX[ai1[0]];
            int k3 = i1.projectedY[ai1[0]];
            int l3 = i1.projectedDepth[ai1[0]];
            int i4 = behindModel.normalX;
            int j4 = behindModel.normalY;
            int k4 = behindModel.normalZ;
            int l4 = i1.faceVisibility[k1];
            int i5 = behindModel.visibilityDot;
            bool flag = false;
            for (int j5 = 0; j5 < l1; j5 += 1)
            {
                int j2 = ai[j5];
                int l2 = (j3 - k.projectedX[j2]) * i4 + (k3 - k.projectedY[j2]) * j4 + (l3 - k.projectedDepth[j2]) * k4;
                if ((l2 >= -l4 || i5 >= 0) && (l2 <= l4 || i5 <= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (!flag)
            {
                return true;
            }

            j3 = k.projectedX[ai[0]];
            k3 = k.projectedY[ai[0]];
            l3 = k.projectedDepth[ai[0]];
            i4 = frontModel.normalX;
            j4 = frontModel.normalY;
            k4 = frontModel.normalZ;
            l4 = k.faceVisibility[j1];
            i5 = frontModel.visibilityDot;
            flag = false;
            for (int k5 = 0; k5 < i2; k5 += 1)
            {
                int k2 = ai1[k5];
                int i3 = (j3 - i1.projectedX[k2]) * i4 + (k3 - i1.projectedY[k2]) * j4 + (l3 - i1.projectedDepth[k2]) * k4;
                if ((i3 >= -l4 || i5 <= 0) && (i3 <= l4 || i5 >= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            return !flag;
        }

        public void CreateTexture(int totalCount, int pixelBufferCount, int colourMapCount)
        {
            textureCount = totalCount;
            texturePictureColorIndex = new sbyte[totalCount][];
            texturePictureColorArray = new int[totalCount][];
            textureLastAccessFrame = new int[totalCount];
            textureLastAccessTimes = new long[totalCount];
            textureIsTransparent = new bool[totalCount];
            objectTexturePixels = new int[totalCount][];
            textureLastUsedTime = 0L;
            texturePixels = new int[pixelBufferCount][];
            textureColourMaps = new int[colourMapCount][];
        }

        public void SetTexture(int textureIndex, sbyte[] colourIndices, int[] colourArray, int frameType)
        {
            texturePictureColorIndex[textureIndex] = colourIndices;
            texturePictureColorArray[textureIndex] = colourArray;
            textureLastAccessFrame[textureIndex] = frameType;
            textureLastAccessTimes[textureIndex] = 0L;
            textureIsTransparent[textureIndex] = false;
            objectTexturePixels[textureIndex] = null;
            UpdateTextureSmoothing(textureIndex);
        }

        public void UpdateTextureSmoothing(int textureIndex)
        {
            if (textureIndex < 0)
            {
                return;
            }

            textureLastAccessTimes[textureIndex] = textureLastUsedTime += 1;
            if (objectTexturePixels[textureIndex] is not null)
            {
                return;
            }

            if (textureLastAccessFrame[textureIndex] == 0)
            {
                for (int k = 0; k < texturePixels.Length; k += 1)
                {
                    if (texturePixels[k] is null)
                    {
                        texturePixels[k] = new int[16384];
                        objectTexturePixels[textureIndex] = texturePixels[k];
                        ApplyTexture(textureIndex);
                        return;
                    }
                }

                long l1 = 1L << 30;
                int j1 = 0;
                for (int i2 = 0; i2 < textureCount; i2 += 1)
                {
                    if (i2 != textureIndex && textureLastAccessFrame[i2] == 0 && objectTexturePixels[i2] is not null && textureLastAccessTimes[i2] < l1)
                    {
                        l1 = textureLastAccessTimes[i2];
                        j1 = i2;
                    }
                }

                objectTexturePixels[textureIndex] = objectTexturePixels[j1];
                objectTexturePixels[j1] = null;
                ApplyTexture(textureIndex);
                return;
            }
            for (int i1 = 0; i1 < textureColourMaps.Length; i1 += 1)
            {
                if (textureColourMaps[i1] is null)
                {
                    textureColourMaps[i1] = new int[0x10000];
                    objectTexturePixels[textureIndex] = textureColourMaps[i1];
                    ApplyTexture(textureIndex);
                    return;
                }
            }

            long l2 = 1L << 30;
            int k1 = 0;
            for (int j2 = 0; j2 < textureCount; j2 += 1)
            {
                if (j2 != textureIndex && textureLastAccessFrame[j2] == 1 && objectTexturePixels[j2] is not null && textureLastAccessTimes[j2] < l2)
                {
                    l2 = textureLastAccessTimes[j2];
                    k1 = j2;
                }
            }

            objectTexturePixels[textureIndex] = objectTexturePixels[k1];
            objectTexturePixels[k1] = null;
            ApplyTexture(textureIndex);
        }

        private void ApplyTexture(int textureIndex)
        {
            int textureSize;
            if (textureLastAccessFrame[textureIndex] == 0)
            {
                textureSize = 64;
            }
            else
            {
                textureSize = 128;//'\200';
            }

            int[] texture = objectTexturePixels[textureIndex];
            int pixelCount = 0;
            for (int x = 0; x < textureSize; x += 1)
            {
                for (int y = 0; y < textureSize; y += 1)
                {
                    int pixel = texturePictureColorArray[textureIndex][texturePictureColorIndex[textureIndex][y + x * textureSize] & 0xff];
                    pixel &= 0xf8f8ff;
                    if (pixel == 0)
                    {
                        pixel = 1;
                    }
                    else if (pixel == 0xf800ff)
                    {
                        pixel = 0;
                        textureIsTransparent[textureIndex] = true;
                    }
                    texture[pixelCount++] = pixel;
                }

            }
            // blend objects with correct lightning?
            for (int pixel = 0; pixel < pixelCount; pixel += 1)
            {
                int i2 = texture[pixel];
                texture[pixelCount + pixel] = i2 - (i2 >> 3) & 0xf8f8ff;
                texture[pixelCount * 2 + pixel] = i2 - (i2 >> 2) & 0xf8f8ff;
                texture[pixelCount * 3 + pixel] = i2 - (i2 >> 2) - (i2 >> 3) & 0xf8f8ff;
            }

        }
        public void UpdateLighting(int textureIndex)
        {
            if (objectTexturePixels[textureIndex] is null)
            {
                return;
            }

            int[] objLight = objectTexturePixels[textureIndex];
            for (int k = 0; k < 64; k += 1)
            {
                int i1 = k + 4032;
                int j1 = objLight[i1];
                for (int l1 = 0; l1 < 63; l1 += 1)
                {
                    objLight[i1] = objLight[i1 - 64];
                    i1 -= 64;
                }

                objectTexturePixels[textureIndex][i1] = j1;
            }

            int c = 4096;
            for (int k1 = 0; k1 < c; k1 += 1)
            {
                int i2 = objLight[k1];
                objLight[c + k1] = i2 - (i2 >> 3) & 0xf8f8ff;
                objLight[c * 2 + k1] = i2 - (i2 >> 2) & 0xf8f8ff;
                objLight[c * 3 + k1] = i2 - (i2 >> 2) - (i2 >> 3) & 0xf8f8ff;
            }

        }

        public int ApplyTextureSmoothing(int index)
        {
            if (index == 0xbc614e)
            {
                return 0;
            }

            UpdateTextureSmoothing(index);
            if (index >= 0)
            {
                return objectTexturePixels[index][0];
            }

            if (index < 0)
            {
                index = -(index + 1);
                int i1 = index >> 10 & 0x1f;
                int j1 = index >> 5 & 0x1f;
                int k1 = index & 0x1f;
                return (i1 << 19) + (j1 << 11) + (k1 << 3);
            }
            else
            {
                return 0;
            }
        }

        public void OffsetAllModelColours(int redOffset, int greenOffset, int blueOffset)
        {
            if (redOffset == 0 && greenOffset == 0 && blueOffset == 0)
            {
                redOffset = 32;
            }

            for (int k = 0; k < currentObjectCount; k += 1)
            {
                objectCache[k].OffsetModelColors(redOffset, greenOffset, blueOffset);
            }
        }

        public void SetAllModelColours(int fromColour, int toColour, int x, int y, int z)
        {
            if (x == 0 && y == 0 && z == 0)
            {
                x = 32;
            }

            for (int k = 0; k < currentObjectCount; k += 1)
            {
                objectCache[k].SetModelColors(fromColour, toColour, x, y, z);
            }
        }

        public static int GetTextureColour(int r, int g, int b)
        {
            return -1 - r / 8 * 1024 - g / 8 * 32 - b / 8;
        }

        public int LinearInterpolate(int valueA, int rangeStartY, int valueB, int rangeEndY, int targetY)
        {
            if (rangeEndY == rangeStartY)
            {
                return valueA;
            }
            else
            {
                return valueA + (valueB - valueA) * (targetY - rangeStartY) / (rangeEndY - rangeStartY);
            }
        }

        public bool ComparePolygonRanges(int valueA, int valueB, int rangeMin, int rangeMax, bool isAscending)
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

        public bool ComparePolygonRange(int valueA, int valueB, int rangeLimit, bool isAscending)
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
            else
            {
                return isAscending;
            }
        }

        public bool PolygonsIntersect(int[] polygonAX, int[] polygonAY, int[] polygonBX, int[] polygonBY)
        {
            int k = polygonAX.Length;
            int i1 = polygonBX.Length; ;
            byte byte0 = 0;
            int l20;
            int j21 = l20 = polygonAY[0];
            int j1 = 0;
            int i21;
            int k21 = i21 = polygonBY[0];
            int l1 = 0;
            for (int l21 = 1; l21 < k; l21 += 1)
            {
                if (polygonAY[l21] < l20)
                {
                    l20 = polygonAY[l21];
                    j1 = l21;
                }
                else
                    if (polygonAY[l21] > j21)
                {
                    j21 = polygonAY[l21];
                }
            }

            for (int i22 = 1; i22 < i1; i22 += 1)
            {
                if (polygonBY[i22] < i21)
                {
                    i21 = polygonBY[i22];
                    l1 = i22;
                }
                else
                    if (polygonBY[i22] > k21)
                {
                    k21 = polygonBY[i22];
                }
            }

            if (i21 >= j21)
            {
                return false;
            }

            if (l20 >= k21)
            {
                return false;
            }

            int k1;
            int i2;
            bool flag;
            if (polygonAY[j1] < polygonBY[l1])
            {
                for (k1 = j1; polygonAY[k1] < polygonBY[l1]; k1 = (k1 + 1) % k)
                {
                    ;
                }

                for (; polygonAY[j1] < polygonBY[l1]; j1 = (j1 - 1 + k) % k)
                {
                    ;
                }

                int j2 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[l1]);
                int j7 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[l1]);
                int k11 = polygonBX[l1];
                flag = (j2 < k11) | (j7 < k11);
                if (ComparePolygonRange(j2, j7, k11, flag))
                {
                    return true;
                }

                i2 = (l1 + 1) % i1;
                l1 = (l1 - 1 + i1) % i1;
                if (j1 == k1)
                {
                    byte0 = 1;
                }
            }
            else
            {
                for (i2 = l1; polygonBY[i2] < polygonAY[j1]; i2 = (i2 + 1) % i1)
                {
                    ;
                }

                for (; polygonBY[l1] < polygonAY[j1]; l1 = (l1 - 1 + i1) % i1)
                {
                    ;
                }

                int k2 = polygonAX[j1];
                int l11 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[j1]);
                int k16 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[j1]);
                flag = (k2 < l11) | (k2 < k16);
                if (ComparePolygonRange(l11, k16, k2, !flag))
                {
                    return true;
                }

                k1 = (j1 + 1) % k;
                j1 = (j1 - 1 + k) % k;
                if (l1 == i2)
                {
                    byte0 = 2;
                }
            }
            while (byte0 == 0)
            {
                if (polygonAY[j1] < polygonAY[k1])
                {
                    if (polygonAY[j1] < polygonBY[l1])
                    {
                        if (polygonAY[j1] < polygonBY[i2])
                        {
                            int l2 = polygonAX[j1];
                            int k7 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonAY[j1]);
                            int i12 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[j1]);
                            int l16 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[j1]);
                            if (ComparePolygonRanges(l2, k7, i12, l16, flag))
                            {
                                return true;
                            }

                            j1 = (j1 - 1 + k) % k;
                            if (j1 == k1)
                            {
                                byte0 = 1;
                            }
                        }
                        else
                        {
                            int i3 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[i2]);
                            int l7 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[i2]);
                            int j12 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonBY[i2]);
                            int i17 = polygonBX[i2];
                            if (ComparePolygonRanges(i3, l7, j12, i17, flag))
                            {
                                return true;
                            }

                            i2 = (i2 + 1) % i1;
                            if (l1 == i2)
                            {
                                byte0 = 2;
                            }
                        }
                    }
                    else
                        if (polygonBY[l1] < polygonBY[i2])
                        {
                            int j3 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[l1]);
                            int i8 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[l1]);
                            int k12 = polygonBX[l1];
                            int j17 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonBY[l1]);
                            if (ComparePolygonRanges(j3, i8, k12, j17, flag))
                        {
                            return true;
                        }

                        l1 = (l1 - 1 + i1) % i1;
                            if (l1 == i2)
                        {
                            byte0 = 2;
                        }
                    }
                        else
                        {
                            int k3 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[i2]);
                            int j8 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[i2]);
                            int l12 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonBY[i2]);
                            int k17 = polygonBX[i2];
                            if (ComparePolygonRanges(k3, j8, l12, k17, flag))
                        {
                            return true;
                        }

                        i2 = (i2 + 1) % i1;
                            if (l1 == i2)
                        {
                            byte0 = 2;
                        }
                    }
                }
                else
                    if (polygonAY[k1] < polygonBY[l1])
                    {
                        if (polygonAY[k1] < polygonBY[i2])
                        {
                            int l3 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonAY[k1]);
                            int k8 = polygonAX[k1];
                            int i13 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[k1]);
                            int l17 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[k1]);
                            if (ComparePolygonRanges(l3, k8, i13, l17, flag))
                        {
                            return true;
                        }

                        k1 = (k1 + 1) % k;
                            if (j1 == k1)
                        {
                            byte0 = 1;
                        }
                    }
                        else
                        {
                            int i4 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[i2]);
                            int l8 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[i2]);
                            int j13 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonBY[i2]);
                            int i18 = polygonBX[i2];
                            if (ComparePolygonRanges(i4, l8, j13, i18, flag))
                        {
                            return true;
                        }

                        i2 = (i2 + 1) % i1;
                            if (l1 == i2)
                        {
                            byte0 = 2;
                        }
                    }
                    }
                    else
                        if (polygonBY[l1] < polygonBY[i2])
                        {
                            int j4 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[l1]);
                            int i9 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[l1]);
                            int k13 = polygonBX[l1];
                            int j18 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonBY[l1]);
                            if (ComparePolygonRanges(j4, i9, k13, j18, flag))
                    {
                        return true;
                    }

                    l1 = (l1 - 1 + i1) % i1;
                            if (l1 == i2)
                    {
                        byte0 = 2;
                    }
                }
                        else
                        {
                            int k4 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[i2]);
                            int j9 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[i2]);
                            int l13 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonBY[i2]);
                            int k18 = polygonBX[i2];
                            if (ComparePolygonRanges(k4, j9, l13, k18, flag))
                    {
                        return true;
                    }

                    i2 = (i2 + 1) % i1;
                            if (l1 == i2)
                    {
                        byte0 = 2;
                    }
                }
            }

            while (byte0 == 1)
            {
                if (polygonAY[j1] < polygonBY[l1])
                {
                    if (polygonAY[j1] < polygonBY[i2])
                    {
                        int l4 = polygonAX[j1];
                        int i14 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[j1]);
                        int l18 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[j1]);
                        return ComparePolygonRange(i14, l18, l4, !flag);
                    }
                    int i5 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[i2]);
                    int k9 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[i2]);
                    int j14 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonBY[i2]);
                    int i19 = polygonBX[i2];
                    if (ComparePolygonRanges(i5, k9, j14, i19, flag))
                    {
                        return true;
                    }

                    i2 = (i2 + 1) % i1;
                    if (l1 == i2)
                    {
                        byte0 = 0;
                    }
                }
                else
                    if (polygonBY[l1] < polygonBY[i2])
                    {
                        int j5 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[l1]);
                        int l9 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[l1]);
                        int k14 = polygonBX[l1];
                        int j19 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonBY[l1]);
                        if (ComparePolygonRanges(j5, l9, k14, j19, flag))
                    {
                        return true;
                    }

                    l1 = (l1 - 1 + i1) % i1;
                        if (l1 == i2)
                    {
                        byte0 = 0;
                    }
                }
                    else
                    {
                        int k5 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[i2]);
                        int i10 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[i2]);
                        int l14 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonBY[i2]);
                        int k19 = polygonBX[i2];
                        if (ComparePolygonRanges(k5, i10, l14, k19, flag))
                    {
                        return true;
                    }

                    i2 = (i2 + 1) % i1;
                        if (l1 == i2)
                    {
                        byte0 = 0;
                    }
                }
            }

            while (byte0 == 2)
            {
                if (polygonBY[l1] < polygonAY[j1])
                {
                    if (polygonBY[l1] < polygonAY[k1])
                    {
                        int l5 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[l1]);
                        int j10 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[l1]);
                        int i15 = polygonBX[l1];
                        return ComparePolygonRange(l5, j10, i15, flag);
                    }
                    int i6 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonAY[k1]);
                    int k10 = polygonAX[k1];
                    int j15 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[k1]);
                    int l19 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[k1]);
                    if (ComparePolygonRanges(i6, k10, j15, l19, flag))
                    {
                        return true;
                    }

                    k1 = (k1 + 1) % k;
                    if (j1 == k1)
                    {
                        byte0 = 0;
                    }
                }
                else
                    if (polygonAY[j1] < polygonAY[k1])
                    {
                        int j6 = polygonAX[j1];
                        int l10 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonAY[j1]);
                        int k15 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[j1]);
                        int i20 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[j1]);
                        if (ComparePolygonRanges(j6, l10, k15, i20, flag))
                    {
                        return true;
                    }

                    j1 = (j1 - 1 + k) % k;
                        if (j1 == k1)
                    {
                        byte0 = 0;
                    }
                }
                    else
                    {
                        int k6 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonAY[k1]);
                        int i11 = polygonAX[k1];
                        int l15 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[k1]);
                        int j20 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[k1]);
                        if (ComparePolygonRanges(k6, i11, l15, j20, flag))
                    {
                        return true;
                    }

                    k1 = (k1 + 1) % k;
                        if (j1 == k1)
                    {
                        byte0 = 0;
                    }
                }
            }

            if (polygonAY[j1] < polygonBY[l1])
            {
                int l6 = polygonAX[j1];
                int i16 = LinearInterpolate(polygonBX[(l1 + 1) % i1], polygonBY[(l1 + 1) % i1], polygonBX[l1], polygonBY[l1], polygonAY[j1]);
                int k20 = LinearInterpolate(polygonBX[(i2 - 1 + i1) % i1], polygonBY[(i2 - 1 + i1) % i1], polygonBX[i2], polygonBY[i2], polygonAY[j1]);
                return ComparePolygonRange(i16, k20, l6, !flag);
            }
            int i7 = LinearInterpolate(polygonAX[(j1 + 1) % k], polygonAY[(j1 + 1) % k], polygonAX[j1], polygonAY[j1], polygonBY[l1]);
            int j11 = LinearInterpolate(polygonAX[(k1 - 1 + k) % k], polygonAY[(k1 - 1 + k) % k], polygonAX[k1], polygonAY[k1], polygonBY[l1]);
            int j16 = polygonBX[l1];
            return ComparePolygonRange(i7, j11, j16, flag);
        }

        public int maxTextureCount;
        public int[] textureClipIds;
        public int[][] textureClipData;
        public int[] textureClipSizes;
        public int savedModelIndex;
        public int nearPlane;
        public int zoom1;
        public int zoom2;
        public int zoom3;
        public int zoom4;
        public static int[] trigonometryTable = new int[2048];
        private static readonly int[] sinCosTable = new int[512];
        public bool isInterlaced;
        public double scaleFactor;
        public int depthSortStride;
        private bool isMousePositionUpdated;
        private int mouseAdjustedX;
        private int mouseAdjustedY;
        private int optionCount;
        private readonly int maxHighlightedObjects;
        private readonly GameObject[] _highlightedObjects;
        private readonly int[] highlightedPlayerIds;
        private int defaultScreenHalfWidth;
        private int screenCentreX;
        private int screenCentreY;
        private int screenMouseOffsetX;
        private int scanlineBufferCentre;
        private int screenProjectionShift;
        private readonly int lightingFactor;
        private int viewX;
        private int ViewY;
        private int ViewZ;
        private int cameraOffsetX;
        private int cameraOffsetY;
        private int cameraOffsetZ;
        public int currentObjectCount;
        public int totalModelCount;
        public GameObject[] objectCache;
        private readonly int[] modelPriorities;
        private int currentModelIndex;
        private readonly CameraModel[] visibleModels;
        private int sceneObjectCount;
        private readonly int[] sceneObjectId;
        private readonly int[] sceneObjectX;
        private readonly int[] sceneObjectY;
        private readonly int[] sceneObjectZ;
        private readonly int[] sceneObjectWidths;
        private readonly int[] sceneObjectHeights;
        private readonly int[] sceneObjectFrames;
        public GameObject highlightedObject;
        public int textureCount;
        public sbyte[][] texturePictureColorIndex;
        public int[][] texturePictureColorArray;
        public int[] textureLastAccessFrame;
        public long[] textureLastAccessTimes;
        public int[][] objectTexturePixels;
        public bool[] textureIsTransparent;
        private static long textureLastUsedTime;
        public int[][] texturePixels;
        public int[][] textureColourMaps;
        private static sbyte[] lookupTable;
        public GameImage gameImage;
        public int[] screenPixels;
        public CameraVariable[] scanlineVariables;
        public int minVisibleScanline;
        public int maxVisibleScanline;
        public int[] clipPolygonX;
        public int[] clipPolygonY;
        public int[] clipPolygonValues;
        public int[] vertX;
        public int[] vertY;
        public int[] vertZ;
        public bool isRenderingInterlaced;
        public static int nearX;
        public static int farX;
        public static int nearY;
        public static int farY;
        public static int nearZ;
        public static int farZ;
        public int sortRangeStart;
        public int sortRangeEnd;

    }

}
