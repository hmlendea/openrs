using System;
using System.Threading;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class Camera
    {
        private static readonly int[] sinCosTable = new int[SinCosTableHalfSize * 2];

        public static int[] trigonometryTable = new int[TrigTableHalfSize * 2];
        public static int nearX;
        public static int farX;
        public static int nearY;
        public static int farY;
        public static int nearZ;
        public static int farZ;
        private static sbyte[] lookupTable;

        private readonly int maxHighlightedObjects;
        private readonly GameObject[] highlightedObjects;
        private readonly int[] highlightedPlayerIds;
        private readonly int lightingFactor;
        private readonly CameraTextureManager textureManager;
        private readonly int[] modelPriorities;
        private readonly CameraModel[] visibleModels;
        private readonly int[] sceneObjectId;
        private readonly int[] sceneObjectX;
        private readonly int[] sceneObjectY;
        private readonly int[] sceneObjectZ;
        private readonly int[] sceneObjectWidths;
        private readonly int[] sceneObjectHeights;
        private readonly int[] sceneObjectFrames;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<Camera>();

        public int savedModelIndex;
        public int nearPlane;
        public int zoom1;
        public int zoom2;
        public int zoom3;
        public int zoom4;
        public bool isInterlaced;
        public double scaleFactor;
        public int depthSortStride;
        public int currentObjectCount;
        public int totalModelCount;
        public GameObject[] objectCache;
        public GameObject highlightedObject;
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
        public int sortRangeStart;
        public int sortRangeEnd;
        private bool isMousePositionUpdated;
        private int mouseAdjustedX;
        private int mouseAdjustedY;
        private int optionCount;
        private int defaultScreenHalfWidth;
        private int screenCentreX;
        private int screenCentreY;
        private int screenMouseOffsetX;
        private int scanlineBufferCentre;
        private int screenProjectionShift;
        private int viewX;
        private int viewY;
        private int viewZ;
        private int cameraOffsetX;
        private int cameraOffsetY;
        private int cameraOffsetZ;
        private int currentModelIndex;
        private int sceneObjectCount;

        private static int DefaultTextureClipCount => 50;
        private static int TextureClipDataSize => 256;
        private static int DefaultNearPlane => 5;
        private static int DefaultZoom => 1000;
        private static int DefaultFogGradientStep => 20;
        private static int DefaultFogGradientThreshold => 10;
        private static int MaxHighlightedObjectCount => 100;
        private static int DefaultScreenHalfWidth => 512;
        private static int DefaultMouseOffsetX => 256;
        private static int DefaultScanlineBufferCentre => 256;
        private static int DefaultScreenProjectionShift => 8;
        private static int DefaultLightingFactor => 4;
        private static int ClipPolygonCapacity => 40;
        private static int LookupTableSize => 17691;
        private static int NotSetSentinel => 0xbc614e;

        // Trigonometry table constants.
        private static int TrigTableHalfSize => 1024;
        private static int TrigTableMask => 0x3ff;
        private static int TrigFixedPointScale => 32768;
        private static int SinCosTableHalfSize => 256;
        private static double DefaultScaleFactor => 1.1D;
        private static double SinCosTableAngleStep => 0.02454369D;
        private static double TrigTableAngleStep => 0.00613592315D;

        // Scanline X boundary sentinels.
        private static int ScanlineLeftXSentinel => 0xa0000;
        private static int ScanlineRightXSentinel => unchecked((int)0xfff60000);

        // Intensity scaling constant.
        private static int SquaredIntensityDivisor => 0x10000;

        public Camera(
            GameImage gameImageSource,
            int maxObjects,
            int maxVisibleObjects,
            int maxSceneObjects)
        {
            textureManager = new CameraTextureManager(DefaultTextureClipCount, TextureClipDataSize);

            nearPlane = DefaultNearPlane;
            zoom1 = DefaultZoom;
            zoom2 = DefaultZoom;
            zoom3 = DefaultFogGradientStep;
            zoom4 = DefaultFogGradientThreshold;
            scaleFactor = DefaultScaleFactor;
            depthSortStride = 1;
            maxHighlightedObjects = MaxHighlightedObjectCount;
            highlightedObjects = new GameObject[maxHighlightedObjects];
            highlightedPlayerIds = new int[maxHighlightedObjects];
            defaultScreenHalfWidth = DefaultScreenHalfWidth;
            screenCentreX = gameImageSource.gameWidth / 2;
            screenCentreY = gameImageSource.gameHeight / 2;
            screenMouseOffsetX = DefaultMouseOffsetX;
            scanlineBufferCentre = DefaultScanlineBufferCentre;
            screenProjectionShift = DefaultScreenProjectionShift;
            lightingFactor = DefaultLightingFactor;
            clipPolygonX = new int[ClipPolygonCapacity];
            clipPolygonY = new int[ClipPolygonCapacity];
            clipPolygonValues = new int[ClipPolygonCapacity];
            vertX = new int[ClipPolygonCapacity];
            vertY = new int[ClipPolygonCapacity];
            vertZ = new int[ClipPolygonCapacity];
            gameImage = gameImageSource;
            screenPixels = gameImageSource.pixels;
            totalModelCount = maxObjects;
            objectCache = new GameObject[totalModelCount];
            modelPriorities = new int[totalModelCount];
            visibleModels = new CameraModel[maxVisibleObjects];

            for (int visibleModelIndex = 0; visibleModelIndex < maxVisibleObjects; visibleModelIndex += 1)
            {
                visibleModels[visibleModelIndex] = new CameraModel();
            }

            highlightedObject = new GameObject(maxSceneObjects * 2, maxSceneObjects);
            sceneObjectId = new int[maxSceneObjects];
            sceneObjectWidths = new int[maxSceneObjects];
            sceneObjectHeights = new int[maxSceneObjects];
            sceneObjectX = new int[maxSceneObjects];
            sceneObjectY = new int[maxSceneObjects];
            sceneObjectZ = new int[maxSceneObjects];
            sceneObjectFrames = new int[maxSceneObjects];
            lookupTable ??= new sbyte[LookupTableSize];

            for (int sinCosIndex = 0; sinCosIndex < SinCosTableHalfSize; sinCosIndex += 1)
            {
                sinCosTable[sinCosIndex] = (int)(Math.Sin(sinCosIndex * SinCosTableAngleStep) * TrigFixedPointScale);
                sinCosTable[sinCosIndex + SinCosTableHalfSize] = (int)(Math.Cos(sinCosIndex * SinCosTableAngleStep) * TrigFixedPointScale);
            }

            for (int trigIndex = 0; trigIndex < TrigTableHalfSize; trigIndex += 1)
            {
                trigonometryTable[trigIndex] = (int)(Math.Sin(trigIndex * TrigTableAngleStep) * TrigFixedPointScale);
                trigonometryTable[trigIndex + TrigTableHalfSize] = (int)(Math.Cos(trigIndex * TrigTableAngleStep) * TrigFixedPointScale);
            }
        }

        public void AddModel(GameObject gameObject)
        {
            if (gameObject is null)
            {
                logger.Warn(GameOperation.AddSceneObject, "Attempted to add a null object.");
            }

            if (currentObjectCount < totalModelCount)
            {
                modelPriorities[currentObjectCount] = 0;
                objectCache[currentObjectCount] = gameObject;
                currentObjectCount += 1;
            }
        }

        public void RemoveModel(GameObject gameObject)
        {
            for (int modelIndex = 0; modelIndex < currentObjectCount; modelIndex += 1)
            {
                if (objectCache[modelIndex] == gameObject)
                {
                    currentObjectCount -= 1;

                    for (int shiftIndex = modelIndex; shiftIndex < currentObjectCount; shiftIndex += 1)
                    {
                        objectCache[shiftIndex] = objectCache[shiftIndex + 1];
                        modelPriorities[shiftIndex] = modelPriorities[shiftIndex + 1];
                    }
                }
            }
        }

        public void CleanUp()
        {
            InitializeScene();

            for (int modelIndex = 0; modelIndex < currentObjectCount; modelIndex += 1)
            {
                objectCache[modelIndex] = null;
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

        public int AddSpriteToScene(
            int objectId,
            int x,
            int y,
            int z,
            int width,
            int height,
            int entityType)
        {
            sceneObjectId[sceneObjectCount] = objectId;
            sceneObjectX[sceneObjectCount] = x;
            sceneObjectY[sceneObjectCount] = y;
            sceneObjectZ[sceneObjectCount] = z;
            sceneObjectWidths[sceneObjectCount] = width;
            sceneObjectHeights[sceneObjectCount] = height;
            sceneObjectFrames[sceneObjectCount] = 0;
            int topVertexIndex = highlightedObject.AddVertex(x, y, z);
            int bottomVertexIndex = highlightedObject.AddVertex(x, y - height, z);
            int[] spriteVertexIndices = [topVertexIndex, bottomVertexIndex];
            highlightedObject.AddFaceVertices(2, spriteVertexIndices, 0, 0);
            highlightedObject.entityType[sceneObjectCount] = entityType;
            highlightedObject.polygonTypeData[sceneObjectCount] = 0;
            sceneObjectCount += 1;

            return sceneObjectCount - 1;
        }

        public void RemoveSprite(int spriteIndex)
            => highlightedObject.polygonTypeData[spriteIndex] = 1;

        public void UpdateSpritePosition(int spriteIndex, int frameIndex)
            => sceneObjectFrames[spriteIndex] = frameIndex;

        public void SetMousePosition(int mouseX, int mouseY)
        {
            mouseAdjustedX = mouseX - screenMouseOffsetX;
            mouseAdjustedY = mouseY;
            optionCount = 0;
            isMousePositionUpdated = true;
        }

        public int GetOptionCount()
            => optionCount;

        public int[] GetHighlightedPlayers()
            => highlightedPlayerIds;

        public GameObject[] GetHighlightedObjects()
            => highlightedObjects;

        public void SetCameraSize(
            int mouseOffsetX,
            int scanlineCentre,
            int centreScrX,
            int centreScrY,
            int screenHalfWidth,
            int projectionShift)
        {
            screenCentreX = centreScrX;
            screenCentreY = centreScrY;
            screenMouseOffsetX = mouseOffsetX;
            scanlineBufferCentre = scanlineCentre;
            defaultScreenHalfWidth = screenHalfWidth;
            screenProjectionShift = projectionShift;
            scanlineVariables = new CameraVariable[centreScrY + scanlineCentre];

            for (int scanlineIndex = 0; scanlineIndex < centreScrY + scanlineCentre; scanlineIndex += 1)
            {
                scanlineVariables[scanlineIndex] = new CameraVariable();
            }
        }

        private void SortAndRenderModels(CameraModel[] models, int startIndex, int endIndex)
        {
            if (startIndex < endIndex)
            {
                int leftPartition = startIndex - 1;
                int rightPartition = endIndex + 1;
                int midIndex = (startIndex + endIndex) / 2;
                CameraModel pivotModel = models[midIndex];
                models[midIndex] = models[startIndex];
                models[startIndex] = pivotModel;
                int pivotScale = pivotModel.Scale;

                while (leftPartition < rightPartition)
                {
                    do
                    {
                        rightPartition -= 1;
                    }
                    while (models[rightPartition].Scale < pivotScale);

                    do
                    {
                        leftPartition += 1;
                    }
                    while (models[leftPartition].Scale > pivotScale);

                    if (leftPartition < rightPartition)
                    {
                        CameraModel swapModel = models[leftPartition];
                        models[leftPartition] = models[rightPartition];
                        models[rightPartition] = swapModel;
                    }
                }

                SortAndRenderModels(models, startIndex, rightPartition);
                SortAndRenderModels(models, rightPartition + 1, endIndex);
            }
        }

        public void RenderCameraModel(int priority, CameraModel[] models, int index)
        {
            for (int modelIndex = 0; modelIndex <= index; modelIndex += 1)
            {
                models[modelIndex].isSorted = false;
                models[modelIndex].sortIndex = modelIndex;
                models[modelIndex].dependencyIndex = -1;
            }

            int searchIndex = 0;

            do
            {
                while (models[searchIndex].isSorted)
                {
                    searchIndex += 1;
                }

                if (searchIndex == index)
                {
                    return;
                }

                CameraModel currentModel = models[searchIndex];
                currentModel.isSorted = true;
                int rangeStart = searchIndex;
                int rangeEnd = searchIndex + priority;

                if (rangeEnd >= index)
                {
                    rangeEnd = index - 1;
                }

                for (int compareIndex = rangeEnd; compareIndex >= rangeStart + 1; compareIndex -= 1)
                {
                    CameraModel compareModel = models[compareIndex];

                    if (currentModel.boundsMinX < compareModel.boundsMaxX &&
                        compareModel.boundsMinX < currentModel.boundsMaxX &&
                        currentModel.boundsMinY < compareModel.boundsMaxY &&
                        compareModel.boundsMinY < currentModel.boundsMaxY &&
                        currentModel.sortIndex != compareModel.dependencyIndex &&
                        !PolygonIntersectionCalculator.AreBoundsDisjoint(currentModel, compareModel) &&
                        PolygonIntersectionCalculator.IsModelBehind(compareModel, currentModel))
                    {
                        HasVisiblePolygons(models, rangeStart, compareIndex);

                        if (models[compareIndex] != compareModel)
                        {
                            compareIndex += 1;
                        }

                        rangeStart = sortRangeStart;
                        compareModel.dependencyIndex = currentModel.sortIndex;
                    }
                }

            } while (true);
        }

        public bool HasVisiblePolygons(CameraModel[] models, int start, int stop)
        {
            do
            {
                CameraModel frontModel = models[start];

                for (int forwardIndex = start + 1; forwardIndex <= stop; forwardIndex += 1)
                {
                    CameraModel forwardCandidate = models[forwardIndex];

                    if (!PolygonIntersectionCalculator.AreBoundsDisjoint(forwardCandidate, frontModel))
                    {
                        break;
                    }

                    models[start] = forwardCandidate;
                    models[forwardIndex] = frontModel;
                    start = forwardIndex;

                    if (start == stop)
                    {
                        sortRangeStart = start;
                        sortRangeEnd = start - 1;
                        return true;
                    }
                }

                CameraModel backModel = models[stop];

                for (int backwardIndex = stop - 1; backwardIndex >= start; backwardIndex -= 1)
                {
                    CameraModel backwardCandidate = models[backwardIndex];

                    if (!PolygonIntersectionCalculator.AreBoundsDisjoint(backModel, backwardCandidate))
                    {
                        break;
                    }

                    models[stop] = backwardCandidate;
                    models[backwardIndex] = backModel;
                    stop = backwardIndex;

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
            int rollAngle = -cameraOffsetX + TrigTableHalfSize & TrigTableMask;
            int pitchAngle = -cameraOffsetY + TrigTableHalfSize & TrigTableMask;
            int yawAngle = -cameraOffsetZ + TrigTableHalfSize & TrigTableMask;

            if (yawAngle != 0)
            {
                int yawSin = trigonometryTable[yawAngle];
                int yawCos = trigonometryTable[yawAngle + TrigTableHalfSize];
                int rotatedX = viewY * yawSin + viewX * yawCos >> 15;
                viewY = viewY * yawCos - viewX * yawSin >> 15;
                viewX = rotatedX;
            }

            if (rollAngle != 0)
            {
                int rollSin = trigonometryTable[rollAngle];
                int rollCos = trigonometryTable[rollAngle + TrigTableHalfSize];
                int rotatedY = viewY * rollCos - viewZ * rollSin >> 15;
                viewZ = viewY * rollSin + viewZ * rollCos >> 15;
                viewY = rotatedY;
            }

            if (pitchAngle != 0)
            {
                int pitchSin = trigonometryTable[pitchAngle];
                int pitchCos = trigonometryTable[pitchAngle + TrigTableHalfSize];
                int rotatedZ = viewZ * pitchSin + viewX * pitchCos >> 15;
                viewZ = viewZ * pitchCos - viewX * pitchSin >> 15;
                viewX = rotatedZ;
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
            int halfProjectionWidth = screenCentreX * zoom1 >> screenProjectionShift;
            int halfProjectionHeight = screenCentreY * zoom1 >> screenProjectionShift;
            nearX = 0;
            farX = 0;
            nearY = 0;
            farY = 0;
            nearZ = 0;
            farZ = 0;
            SetViewAngle(-halfProjectionWidth, -halfProjectionHeight, zoom1);
            SetViewAngle(-halfProjectionWidth, halfProjectionHeight, zoom1);
            SetViewAngle(halfProjectionWidth, -halfProjectionHeight, zoom1);
            SetViewAngle(halfProjectionWidth, halfProjectionHeight, zoom1);
            SetViewAngle(-screenCentreX, -screenCentreY, 0);
            SetViewAngle(-screenCentreX, screenCentreY, 0);
            SetViewAngle(screenCentreX, -screenCentreY, 0);
            SetViewAngle(screenCentreX, screenCentreY, 0);
            nearX += viewX;
            farX += viewX;
            nearY += viewY;
            farY += viewY;
            nearZ += viewZ;
            farZ += viewZ;
            objectCache[currentObjectCount] = highlightedObject;
            highlightedObject.objectState = 2;

            for (int projectionIndex = 0; projectionIndex < currentObjectCount; projectionIndex += 1)
            {
                objectCache[projectionIndex]?.ProjectWithRotation(
                    viewX,
                    viewY,
                    viewZ,
                    cameraOffsetX,
                    cameraOffsetY,
                    cameraOffsetZ,
                    screenProjectionShift,
                    nearPlane);
            }

            int msSlept = 0;

            while (objectCache[currentObjectCount] is null)
            {
                Thread.Sleep(10);
                msSlept += 10;

                if (msSlept > 1000)
                {
                    return;
                }
            }

            objectCache[currentObjectCount].ProjectWithRotation(
                viewX,
                viewY,
                viewZ,
                cameraOffsetX,
                cameraOffsetY,
                cameraOffsetZ,
                screenProjectionShift,
                nearPlane);
            currentModelIndex = 0;

            for (int objectIndex = 0; objectIndex < currentObjectCount; objectIndex += 1)
            {
                GameObject currentObject = objectCache[objectIndex];

                if (currentObject is null)
                {
                    continue;
                }

                if (currentObject.visible)
                {
                    for (int faceIndex = 0; faceIndex < currentObject.face_count; faceIndex += 1)
                    {
                        int faceVertCount = currentObject.face_vertices_count[faceIndex];
                        int[] faceVertIndices = currentObject.face_vertices[faceIndex];
                        bool hasVisibleVertex = false;

                        for (int vertIndex = 0; vertIndex < faceVertCount; vertIndex += 1)
                        {
                            int vertDepth = currentObject.projectedDepth[faceVertIndices[vertIndex]];

                            if (vertDepth <= nearPlane || vertDepth >= zoom1)
                            {
                                continue;
                            }

                            hasVisibleVertex = true;
                            break;
                        }

                        if (hasVisibleVertex)
                        {
                            int horizontalVisibilityFlags = 0;

                            for (int horzCheckIndex = 0; horzCheckIndex < faceVertCount; horzCheckIndex += 1)
                            {
                                int projectedU = currentObject.projectedU[faceVertIndices[horzCheckIndex]];

                                if (projectedU > -screenCentreX)
                                {
                                    horizontalVisibilityFlags |= 1;
                                }

                                if (projectedU < screenCentreX)
                                {
                                    horizontalVisibilityFlags |= 2;
                                }

                                if (horizontalVisibilityFlags == 3)
                                {
                                    break;
                                }
                            }

                            if (horizontalVisibilityFlags == 3)
                            {
                                int verticalVisibilityFlags = 0;

                                for (int vertCheckIndex = 0; vertCheckIndex < faceVertCount; vertCheckIndex += 1)
                                {
                                    int projectedV = currentObject.projectedV[faceVertIndices[vertCheckIndex]];

                                    if (projectedV > -screenCentreY)
                                    {
                                        verticalVisibilityFlags |= 1;
                                    }

                                    if (projectedV < screenCentreY)
                                    {
                                        verticalVisibilityFlags |= 2;
                                    }

                                    if (verticalVisibilityFlags == 3)
                                    {
                                        break;
                                    }
                                }

                                if (verticalVisibilityFlags == 3)
                                {
                                    CameraModel cameraModel = visibleModels[currentModelIndex];
                                    cameraModel.Object = currentObject;
                                    cameraModel.faceVertCountIndex1 = faceIndex;
                                    CameraModelBoundsCalculator.UpdateModelAtIndex(visibleModels, currentModelIndex, lightingFactor);
                                    int textureIndex;

                                    if (cameraModel.visibilityDot < 0)
                                    {
                                        textureIndex = currentObject.texture_back[faceIndex];
                                    }
                                    else
                                    {
                                        textureIndex = currentObject.texture_front[faceIndex];
                                    }

                                    if (textureIndex != NotSetSentinel)
                                    {
                                        int totalDepth = 0;

                                        for (int depthAccumIndex = 0; depthAccumIndex < faceVertCount; depthAccumIndex += 1)
                                        {
                                            totalDepth += currentObject.projectedDepth[faceVertIndices[depthAccumIndex]];
                                        }

                                        cameraModel.Scale = totalDepth / faceVertCount + currentObject.scaleBias;
                                        cameraModel.currentTextureIndex = textureIndex;
                                        currentModelIndex += 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            GameObject spriteScene = highlightedObject;

            if (spriteScene.visible)
            {
                for (int spriteIndex = 0; spriteIndex < spriteScene.face_count; spriteIndex += 1)
                {
                    int[] spriteVertices = spriteScene.face_vertices[spriteIndex];
                    int firstVertexIndex = spriteVertices[0];
                    int spriteProjU = spriteScene.projectedU[firstVertexIndex];
                    int spriteProjV = spriteScene.projectedV[firstVertexIndex];
                    int spriteDepth = spriteScene.projectedDepth[firstVertexIndex];

                    if (spriteDepth > nearPlane && spriteDepth < zoom2)
                    {
                        int spriteScreenWidth = (sceneObjectWidths[spriteIndex] << screenProjectionShift) / spriteDepth;
                        int spriteScreenHeight = (sceneObjectHeights[spriteIndex] << screenProjectionShift) / spriteDepth;

                        if (spriteProjU - spriteScreenWidth / 2 <= screenCentreX &&
                            spriteProjU + spriteScreenWidth / 2 >= -screenCentreX &&
                            spriteProjV - spriteScreenHeight <= screenCentreY &&
                            spriteProjV >= -screenCentreY)
                        {
                            CameraModel spriteCameraModel = visibleModels[currentModelIndex];
                            spriteCameraModel.Object = spriteScene;
                            spriteCameraModel.faceVertCountIndex1 = spriteIndex;
                            CameraModelBoundsCalculator.RemoveModelAtIndex(visibleModels, currentModelIndex);
                            spriteCameraModel.Scale = (spriteDepth + spriteScene.projectedDepth[spriteVertices[1]]) / 2;
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

            for (int sortedModelIndex = 0; sortedModelIndex < currentModelIndex; sortedModelIndex += 1)
            {
                CameraModel sortedModel = visibleModels[sortedModelIndex];
                GameObject model = sortedModel.Object;
                int faceIndex = sortedModel.faceVertCountIndex1;

                if (model == highlightedObject)
                {
                    int[] spriteFaceVerts = model.face_vertices[faceIndex];
                    int spriteFirstVertex = spriteFaceVerts[0];
                    int spriteProjU = model.projectedU[spriteFirstVertex];
                    int spriteProjV = model.projectedV[spriteFirstVertex];
                    int spriteDepth = model.projectedDepth[spriteFirstVertex];
                    int spriteWidth = (sceneObjectWidths[faceIndex] << screenProjectionShift) / spriteDepth;
                    int spriteHeight = (sceneObjectHeights[faceIndex] << screenProjectionShift) / spriteDepth;
                    int heightSpan = spriteProjV - model.projectedV[spriteFaceVerts[1]];
                    int xOffset = (model.projectedU[spriteFaceVerts[1]] - spriteProjU) * heightSpan / spriteHeight;
                    xOffset = model.projectedU[spriteFaceVerts[1]] - spriteProjU;
                    int screenX = spriteProjU - spriteWidth / 2;
                    int screenY = scanlineBufferCentre + spriteProjV - spriteHeight;
                    gameImage.DrawVisibleEntity(
                        screenX + screenMouseOffsetX,
                        screenY,
                        spriteWidth,
                        spriteHeight,
                        sceneObjectId[faceIndex],
                        xOffset,
                        (256 << screenProjectionShift) / spriteDepth);

                    if (isMousePositionUpdated && optionCount < maxHighlightedObjects)
                    {
                        screenX += (sceneObjectFrames[faceIndex] << screenProjectionShift) / spriteDepth;

                        if (mouseAdjustedY >= screenY &&
                            mouseAdjustedY <= screenY + spriteHeight &&
                            mouseAdjustedX >= screenX &&
                            mouseAdjustedX <= screenX + spriteWidth &&
                            !model.shareEntityArrays &&
                            model.polygonTypeData[faceIndex] == 0)
                        {
                            highlightedObjects[optionCount] = model;
                            highlightedPlayerIds[optionCount] = faceIndex;
                            optionCount += 1;
                        }
                    }
                }
                else
                {
                    int clippedVertCount = 0;
                    int shadeLevel = 0;
                    int vertCount = model.face_vertices_count[faceIndex];
                    int[] modelFaceVerts = model.face_vertices[faceIndex];

                    if (model.gouraud_shade[faceIndex] != NotSetSentinel)
                    {
                        if (sortedModel.visibilityDot < 0)
                        {
                            shadeLevel = model.baseShadeLevel - model.gouraud_shade[faceIndex];
                        }
                        else
                        {
                            shadeLevel = model.baseShadeLevel + model.gouraud_shade[faceIndex];
                        }
                    }

                    for (int faceVertLoopIndex = 0; faceVertLoopIndex < vertCount; faceVertLoopIndex += 1)
                    {
                        int vertIndex = modelFaceVerts[faceVertLoopIndex];
                        vertX[faceVertLoopIndex] = model.projectedX[vertIndex];
                        vertY[faceVertLoopIndex] = model.projectedY[vertIndex];
                        vertZ[faceVertLoopIndex] = model.projectedDepth[vertIndex];

                        if (model.gouraud_shade[faceIndex] == NotSetSentinel)
                        {
                            if (sortedModel.visibilityDot < 0)
                            {
                                shadeLevel = model.baseShadeLevel - model.faceNormalComponent[vertIndex] + model.vertexColor[vertIndex];
                            }
                            else
                            {
                                shadeLevel = model.baseShadeLevel + model.faceNormalComponent[vertIndex] + model.vertexColor[vertIndex];
                            }
                        }

                        if (model.projectedDepth[vertIndex] >= nearPlane)
                        {
                            clipPolygonX[clippedVertCount] = model.projectedU[vertIndex];
                            clipPolygonY[clippedVertCount] = model.projectedV[vertIndex];
                            clipPolygonValues[clippedVertCount] = shadeLevel;

                            if (model.projectedDepth[vertIndex] > zoom4)
                            {
                                clipPolygonValues[clippedVertCount] += (model.projectedDepth[vertIndex] - zoom4) / zoom3;
                            }

                            clippedVertCount += 1;
                        }
                        else
                        {
                            int adjacentVertIndex;

                            if (faceVertLoopIndex == 0)
                            {
                                adjacentVertIndex = modelFaceVerts[vertCount - 1];
                            }
                            else
                            {
                                adjacentVertIndex = modelFaceVerts[faceVertLoopIndex - 1];
                            }

                            if (model.projectedDepth[adjacentVertIndex] >= nearPlane)
                            {
                                int prevDepthDiff = model.projectedDepth[vertIndex] - model.projectedDepth[adjacentVertIndex];
                                int clippedX = model.projectedX[vertIndex] - (model.projectedX[vertIndex] - model.projectedX[adjacentVertIndex]) * (model.projectedDepth[vertIndex] - nearPlane) / prevDepthDiff;
                                int clippedY = model.projectedY[vertIndex] - (model.projectedY[vertIndex] - model.projectedY[adjacentVertIndex]) * (model.projectedDepth[vertIndex] - nearPlane) / prevDepthDiff;
                                clipPolygonX[clippedVertCount] = (clippedX << screenProjectionShift) / nearPlane;
                                clipPolygonY[clippedVertCount] = (clippedY << screenProjectionShift) / nearPlane;
                                clipPolygonValues[clippedVertCount] = shadeLevel;
                                clippedVertCount += 1;
                            }

                            if (faceVertLoopIndex == vertCount - 1)
                            {
                                adjacentVertIndex = modelFaceVerts[0];
                            }
                            else
                            {
                                adjacentVertIndex = modelFaceVerts[faceVertLoopIndex + 1];
                            }

                            if (model.projectedDepth[adjacentVertIndex] >= nearPlane)
                            {
                                int nextDepthDiff = model.projectedDepth[vertIndex] - model.projectedDepth[adjacentVertIndex];
                                int nextClippedX = model.projectedX[vertIndex] - (model.projectedX[vertIndex] - model.projectedX[adjacentVertIndex]) * (model.projectedDepth[vertIndex] - nearPlane) / nextDepthDiff;
                                int nextClippedY = model.projectedY[vertIndex] - (model.projectedY[vertIndex] - model.projectedY[adjacentVertIndex]) * (model.projectedDepth[vertIndex] - nearPlane) / nextDepthDiff;
                                clipPolygonX[clippedVertCount] = (nextClippedX << screenProjectionShift) / nearPlane;
                                clipPolygonY[clippedVertCount] = (nextClippedY << screenProjectionShift) / nearPlane;
                                clipPolygonValues[clippedVertCount] = shadeLevel;
                                clippedVertCount += 1;
                            }
                        }
                    }

                    for (int clampLoopIndex = 0; clampLoopIndex < vertCount; clampLoopIndex += 1)
                    {
                        if (clipPolygonValues[clampLoopIndex] < 0)
                        {
                            clipPolygonValues[clampLoopIndex] = 0;
                        }
                        else if (clipPolygonValues[clampLoopIndex] > 255)
                        {
                            clipPolygonValues[clampLoopIndex] = 255;
                        }

                        if (sortedModel.currentTextureIndex >= 0)
                        {
                            if (textureManager.textureLastAccessFrame[sortedModel.currentTextureIndex] == 1)
                            {
                                clipPolygonValues[clampLoopIndex] <<= 9;
                            }
                            else
                            {
                                clipPolygonValues[clampLoopIndex] <<= 6;
                            }
                        }
                    }

                    RenderPolygon(
                        0,
                        0,
                        0,
                        0,
                        clippedVertCount,
                        clipPolygonX,
                        clipPolygonY,
                        clipPolygonValues,
                        model,
                        faceIndex);

                    if (maxVisibleScanline > minVisibleScanline)
                    {
                        RenderModel(
                            0,
                            0,
                            vertCount,
                            vertX,
                            vertY,
                            vertZ,
                            sortedModel.currentTextureIndex,
                            model);
                    }
                }
            }

            isMousePositionUpdated = false;
        }

        private void RenderPolygon(
            int scanlineLeftX,
            int scanlineRightX,
            int scanlineY,
            int scanlineLeftShade,
            int vertexCount,
            int[] polygonX,
            int[] polygonY,
            int[] polygonZ,
            GameObject gameObject,
            int faceVertexIndex)
        {
            if (vertexCount == 3)
            {
                int scanlineY0 = polygonY[0] + scanlineBufferCentre;
                int scanlineY1 = polygonY[1] + scanlineBufferCentre;
                int scanlineY2 = polygonY[2] + scanlineBufferCentre;
                int vertX0 = polygonX[0];
                int vertX1 = polygonX[1];
                int vertX2 = polygonX[2];
                int vertShade0 = polygonZ[0];
                int vertShade1 = polygonZ[1];
                int vertShade2 = polygonZ[2];
                int scanlineLimit = scanlineBufferCentre + screenCentreY - 1;
                int edgeAX = 0;
                int edgeAXSlope = 0;
                int edgeAShade = 0;
                int edgeAShadeSlope = 0;
                int edgeAMinY = NotSetSentinel;
                int edgeAMaxY = -NotSetSentinel;

                if (scanlineY2 != scanlineY0)
                {
                    edgeAXSlope = (vertX2 - vertX0 << 8) / (scanlineY2 - scanlineY0);
                    edgeAShadeSlope = (vertShade2 - vertShade0 << 8) / (scanlineY2 - scanlineY0);

                    if (scanlineY0 < scanlineY2)
                    {
                        edgeAX = vertX0 << 8;
                        edgeAShade = vertShade0 << 8;
                        edgeAMinY = scanlineY0;
                        edgeAMaxY = scanlineY2;
                    }
                    else
                    {
                        edgeAX = vertX2 << 8;
                        edgeAShade = vertShade2 << 8;
                        edgeAMinY = scanlineY2;
                        edgeAMaxY = scanlineY0;
                    }

                    if (edgeAMinY < 0)
                    {
                        edgeAX -= edgeAXSlope * edgeAMinY;
                        edgeAShade -= edgeAShadeSlope * edgeAMinY;
                        edgeAMinY = 0;
                    }

                    if (edgeAMaxY > scanlineLimit)
                    {
                        edgeAMaxY = scanlineLimit;
                    }
                }

                int edgeBX = 0;
                int edgeBXSlope = 0;
                int edgeBShade = 0;
                int edgeBShadeSlope = 0;
                int edgeBMinY = NotSetSentinel;
                int edgeBMaxY = -NotSetSentinel;

                if (scanlineY1 != scanlineY0)
                {
                    edgeBXSlope = (vertX1 - vertX0 << 8) / (scanlineY1 - scanlineY0);
                    edgeBShadeSlope = (vertShade1 - vertShade0 << 8) / (scanlineY1 - scanlineY0);

                    if (scanlineY0 < scanlineY1)
                    {
                        edgeBX = vertX0 << 8;
                        edgeBShade = vertShade0 << 8;
                        edgeBMinY = scanlineY0;
                        edgeBMaxY = scanlineY1;
                    }
                    else
                    {
                        edgeBX = vertX1 << 8;
                        edgeBShade = vertShade1 << 8;
                        edgeBMinY = scanlineY1;
                        edgeBMaxY = scanlineY0;
                    }

                    if (edgeBMinY < 0)
                    {
                        edgeBX -= edgeBXSlope * edgeBMinY;
                        edgeBShade -= edgeBShadeSlope * edgeBMinY;
                        edgeBMinY = 0;
                    }

                    if (edgeBMaxY > scanlineLimit)
                    {
                        edgeBMaxY = scanlineLimit;
                    }
                }

                int edgeCX = 0;
                int edgeCXSlope = 0;
                int edgeCShade = 0;
                int edgeCShadeSlope = 0;
                int edgeCMinY = NotSetSentinel;
                int edgeCMaxY = -NotSetSentinel;

                if (scanlineY2 != scanlineY1)
                {
                    edgeCXSlope = (vertX2 - vertX1 << 8) / (scanlineY2 - scanlineY1);
                    edgeCShadeSlope = (vertShade2 - vertShade1 << 8) / (scanlineY2 - scanlineY1);

                    if (scanlineY1 < scanlineY2)
                    {
                        edgeCX = vertX1 << 8;
                        edgeCShade = vertShade1 << 8;
                        edgeCMinY = scanlineY1;
                        edgeCMaxY = scanlineY2;
                    }
                    else
                    {
                        edgeCX = vertX2 << 8;
                        edgeCShade = vertShade2 << 8;
                        edgeCMinY = scanlineY2;
                        edgeCMaxY = scanlineY1;
                    }

                    if (edgeCMinY < 0)
                    {
                        edgeCX -= edgeCXSlope * edgeCMinY;
                        edgeCShade -= edgeCShadeSlope * edgeCMinY;
                        edgeCMinY = 0;
                    }

                    if (edgeCMaxY > scanlineLimit)
                    {
                        edgeCMaxY = scanlineLimit;
                    }
                }

                minVisibleScanline = edgeAMinY;

                if (edgeBMinY < minVisibleScanline)
                {
                    minVisibleScanline = edgeBMinY;
                }

                if (edgeCMinY < minVisibleScanline)
                {
                    minVisibleScanline = edgeCMinY;
                }

                maxVisibleScanline = edgeAMaxY;

                if (edgeBMaxY > maxVisibleScanline)
                {
                    maxVisibleScanline = edgeBMaxY;
                }

                if (edgeCMaxY > maxVisibleScanline)
                {
                    maxVisibleScanline = edgeCMaxY;
                }

                int scanlineRightShade = 0;

                for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += 1)
                {
                    if (scanlineY >= edgeAMinY && scanlineY < edgeAMaxY)
                    {
                        scanlineLeftX = scanlineRightX = edgeAX;
                        scanlineLeftShade = scanlineRightShade = edgeAShade;
                        edgeAX += edgeAXSlope;
                        edgeAShade += edgeAShadeSlope;
                    }
                    else
                    {
                        scanlineLeftX = ScanlineLeftXSentinel;
                        scanlineRightX = ScanlineRightXSentinel;
                    }

                    if (scanlineY >= edgeBMinY && scanlineY < edgeBMaxY)
                    {
                        if (edgeBX < scanlineLeftX)
                        {
                            scanlineLeftX = edgeBX;
                            scanlineLeftShade = edgeBShade;
                        }

                        if (edgeBX > scanlineRightX)
                        {
                            scanlineRightX = edgeBX;
                            scanlineRightShade = edgeBShade;
                        }

                        edgeBX += edgeBXSlope;
                        edgeBShade += edgeBShadeSlope;
                    }

                    if (scanlineY >= edgeCMinY && scanlineY < edgeCMaxY)
                    {
                        if (edgeCX < scanlineLeftX)
                        {
                            scanlineLeftX = edgeCX;
                            scanlineLeftShade = edgeCShade;
                        }

                        if (edgeCX > scanlineRightX)
                        {
                            scanlineRightX = edgeCX;
                            scanlineRightShade = edgeCShade;
                        }

                        edgeCX += edgeCXSlope;
                        edgeCShade += edgeCShadeSlope;
                    }

                    CameraVariable scanline = scanlineVariables[scanlineY];
                    scanline.scanlineMinX = scanlineLeftX;
                    scanline.scanlineMaxX = scanlineRightX;
                    scanline.scanlineMinValue = scanlineLeftShade;
                    scanline.scanlineMaxValue = scanlineRightShade;
                }

                if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }
            }
            else if (vertexCount == 4)
            {
                int scanlineY0 = polygonY[0] + scanlineBufferCentre;
                int scanlineY1 = polygonY[1] + scanlineBufferCentre;
                int scanlineY2 = polygonY[2] + scanlineBufferCentre;
                int scanlineY3 = polygonY[3] + scanlineBufferCentre;
                int vertX0 = polygonX[0];
                int vertX1 = polygonX[1];
                int vertX2 = polygonX[2];
                int vertX3 = polygonX[3];
                int vertShade0 = polygonZ[0];
                int vertShade1 = polygonZ[1];
                int vertShade2 = polygonZ[2];
                int vertShade3 = polygonZ[3];
                int scanlineLimit = scanlineBufferCentre + screenCentreY - 1;
                int edgeAX = 0;
                int edgeAXSlope = 0;
                int edgeAShade = 0;
                int edgeAShadeSlope = 0;
                int edgeAMinY = NotSetSentinel;
                int edgeAMaxY = -NotSetSentinel;

                if (scanlineY3 != scanlineY0)
                {
                    edgeAXSlope = (vertX3 - vertX0 << 8) / (scanlineY3 - scanlineY0);
                    edgeAShadeSlope = (vertShade3 - vertShade0 << 8) / (scanlineY3 - scanlineY0);
                    if (scanlineY0 < scanlineY3)
                    {
                        edgeAX = vertX0 << 8;
                        edgeAShade = vertShade0 << 8;
                        edgeAMinY = scanlineY0;
                        edgeAMaxY = scanlineY3;
                    }
                    else
                    {
                        edgeAX = vertX3 << 8;
                        edgeAShade = vertShade3 << 8;
                        edgeAMinY = scanlineY3;
                        edgeAMaxY = scanlineY0;
                    }
                    if (edgeAMinY < 0)
                    {
                        edgeAX -= edgeAXSlope * edgeAMinY;
                        edgeAShade -= edgeAShadeSlope * edgeAMinY;
                        edgeAMinY = 0;
                    }
                    if (edgeAMaxY > scanlineLimit)
                    {
                        edgeAMaxY = scanlineLimit;
                    }
                }

                int edgeBX = 0;
                int edgeBXSlope = 0;
                int edgeBShade = 0;
                int edgeBShadeSlope = 0;
                int edgeBMinY = NotSetSentinel;
                int edgeBMaxY = -NotSetSentinel;

                if (scanlineY1 != scanlineY0)
                {
                    edgeBXSlope = (vertX1 - vertX0 << 8) / (scanlineY1 - scanlineY0);
                    edgeBShadeSlope = (vertShade1 - vertShade0 << 8) / (scanlineY1 - scanlineY0);
                    if (scanlineY0 < scanlineY1)
                    {
                        edgeBX = vertX0 << 8;
                        edgeBShade = vertShade0 << 8;
                        edgeBMinY = scanlineY0;
                        edgeBMaxY = scanlineY1;
                    }
                    else
                    {
                        edgeBX = vertX1 << 8;
                        edgeBShade = vertShade1 << 8;
                        edgeBMinY = scanlineY1;
                        edgeBMaxY = scanlineY0;
                    }
                    if (edgeBMinY < 0)
                    {
                        edgeBX -= edgeBXSlope * edgeBMinY;
                        edgeBShade -= edgeBShadeSlope * edgeBMinY;
                        edgeBMinY = 0;
                    }
                    if (edgeBMaxY > scanlineLimit)
                    {
                        edgeBMaxY = scanlineLimit;
                    }
                }

                int edgeCX = 0;
                int edgeCXSlope = 0;
                int edgeCShade = 0;
                int edgeCShadeSlope = 0;
                int edgeCMinY = NotSetSentinel;
                int edgeCMaxY = -NotSetSentinel;

                if (scanlineY2 != scanlineY1)
                {
                    edgeCXSlope = (vertX2 - vertX1 << 8) / (scanlineY2 - scanlineY1);
                    edgeCShadeSlope = (vertShade2 - vertShade1 << 8) / (scanlineY2 - scanlineY1);
                    if (scanlineY1 < scanlineY2)
                    {
                        edgeCX = vertX1 << 8;
                        edgeCShade = vertShade1 << 8;
                        edgeCMinY = scanlineY1;
                        edgeCMaxY = scanlineY2;
                    }
                    else
                    {
                        edgeCX = vertX2 << 8;
                        edgeCShade = vertShade2 << 8;
                        edgeCMinY = scanlineY2;
                        edgeCMaxY = scanlineY1;
                    }
                    if (edgeCMinY < 0)
                    {
                        edgeCX -= edgeCXSlope * edgeCMinY;
                        edgeCShade -= edgeCShadeSlope * edgeCMinY;
                        edgeCMinY = 0;
                    }
                    if (edgeCMaxY > scanlineLimit)
                    {
                        edgeCMaxY = scanlineLimit;
                    }
                }

                int edgeDX = 0;
                int edgeDXSlope = 0;
                int edgeDShade = 0;
                int edgeDShadeSlope = 0;
                int edgeDMinY = NotSetSentinel;
                int edgeDMaxY = -NotSetSentinel;

                if (scanlineY3 != scanlineY2)
                {
                    edgeDXSlope = (vertX3 - vertX2 << 8) / (scanlineY3 - scanlineY2);
                    edgeDShadeSlope = (vertShade3 - vertShade2 << 8) / (scanlineY3 - scanlineY2);
                    if (scanlineY2 < scanlineY3)
                    {
                        edgeDX = vertX2 << 8;
                        edgeDShade = vertShade2 << 8;
                        edgeDMinY = scanlineY2;
                        edgeDMaxY = scanlineY3;
                    }
                    else
                    {
                        edgeDX = vertX3 << 8;
                        edgeDShade = vertShade3 << 8;
                        edgeDMinY = scanlineY3;
                        edgeDMaxY = scanlineY2;
                    }
                    if (edgeDMinY < 0)
                    {
                        edgeDX -= edgeDXSlope * edgeDMinY;
                        edgeDShade -= edgeDShadeSlope * edgeDMinY;
                        edgeDMinY = 0;
                    }
                    if (edgeDMaxY > scanlineLimit)
                    {
                        edgeDMaxY = scanlineLimit;
                    }
                }

                minVisibleScanline = edgeAMinY;

                if (edgeBMinY < minVisibleScanline)
                {
                    minVisibleScanline = edgeBMinY;
                }

                if (edgeCMinY < minVisibleScanline)
                {
                    minVisibleScanline = edgeCMinY;
                }

                if (edgeDMinY < minVisibleScanline)
                {
                    minVisibleScanline = edgeDMinY;
                }

                maxVisibleScanline = edgeAMaxY;

                if (edgeBMaxY > maxVisibleScanline)
                {
                    maxVisibleScanline = edgeBMaxY;
                }

                if (edgeCMaxY > maxVisibleScanline)
                {
                    maxVisibleScanline = edgeCMaxY;
                }

                if (edgeDMaxY > maxVisibleScanline)
                {
                    maxVisibleScanline = edgeDMaxY;
                }

                int scanlineRightShade = 0;

                for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += 1)
                {
                    if (scanlineY >= edgeAMinY && scanlineY < edgeAMaxY)
                    {
                        scanlineLeftX = scanlineRightX = edgeAX;
                        scanlineLeftShade = scanlineRightShade = edgeAShade;
                        edgeAX += edgeAXSlope;
                        edgeAShade += edgeAShadeSlope;
                    }
                    else
                    {
                        scanlineLeftX = ScanlineLeftXSentinel;
                        scanlineRightX = ScanlineRightXSentinel;
                    }

                    if (scanlineY >= edgeBMinY && scanlineY < edgeBMaxY)
                    {
                        if (edgeBX < scanlineLeftX)
                        {
                            scanlineLeftX = edgeBX;
                            scanlineLeftShade = edgeBShade;
                        }
                        if (edgeBX > scanlineRightX)
                        {
                            scanlineRightX = edgeBX;
                            scanlineRightShade = edgeBShade;
                        }
                        edgeBX += edgeBXSlope;
                        edgeBShade += edgeBShadeSlope;
                    }

                    if (scanlineY >= edgeCMinY && scanlineY < edgeCMaxY)
                    {
                        if (edgeCX < scanlineLeftX)
                        {
                            scanlineLeftX = edgeCX;
                            scanlineLeftShade = edgeCShade;
                        }
                        if (edgeCX > scanlineRightX)
                        {
                            scanlineRightX = edgeCX;
                            scanlineRightShade = edgeCShade;
                        }
                        edgeCX += edgeCXSlope;
                        edgeCShade += edgeCShadeSlope;
                    }

                    if (scanlineY >= edgeDMinY && scanlineY < edgeDMaxY)
                    {
                        if (edgeDX < scanlineLeftX)
                        {
                            scanlineLeftX = edgeDX;
                            scanlineLeftShade = edgeDShade;
                        }
                        if (edgeDX > scanlineRightX)
                        {
                            scanlineRightX = edgeDX;
                            scanlineRightShade = edgeDShade;
                        }
                        edgeDX += edgeDXSlope;
                        edgeDShade += edgeDShadeSlope;
                    }
                    CameraVariable scanline = scanlineVariables[scanlineY];
                    scanline.scanlineMinX = scanlineLeftX;
                    scanline.scanlineMaxX = scanlineRightX;
                    scanline.scanlineMinValue = scanlineLeftShade;
                    scanline.scanlineMaxValue = scanlineRightShade;
                }

                if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }
            }
            else
            {
                maxVisibleScanline = minVisibleScanline = polygonY[0] += scanlineBufferCentre;
                for (scanlineY = 1; scanlineY < vertexCount; scanlineY += 1)
                {
                    int scanlineOffset;
                    if ((scanlineOffset = polygonY[scanlineY] += scanlineBufferCentre) < minVisibleScanline)
                    {
                        minVisibleScanline = scanlineOffset;
                    }
                    else if (scanlineOffset > maxVisibleScanline)
                    {
                        maxVisibleScanline = scanlineOffset;
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

                for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += 1)
                {
                    CameraVariable scanline = scanlineVariables[scanlineY];
                    scanline.scanlineMinX = ScanlineLeftXSentinel;
                    scanline.scanlineMaxX = ScanlineRightXSentinel;
                }

                int lastVertIndex = vertexCount - 1;
                int firstScanlineY = polygonY[0];
                int lastScanlineY = polygonY[lastVertIndex];
                if (firstScanlineY < lastScanlineY)
                {
                    int edgeXStart = polygonX[0] << 8;
                    int edgeXSlope = ((polygonX[lastVertIndex] - polygonX[0]) << 8) / (lastScanlineY - firstScanlineY);
                    int edgeShadeStart = polygonZ[0] << 8;
                    int edgeShadeSlope = ((polygonZ[lastVertIndex] - polygonZ[0]) << 8) / (lastScanlineY - firstScanlineY);
                    if (firstScanlineY < 0)
                    {
                        edgeXStart -= edgeXSlope * firstScanlineY;
                        edgeShadeStart -= edgeShadeSlope * firstScanlineY;
                        firstScanlineY = 0;
                    }
                    if (lastScanlineY > maxVisibleScanline)
                    {
                        lastScanlineY = maxVisibleScanline;
                    }

                    for (scanlineY = firstScanlineY; scanlineY <= lastScanlineY; scanlineY += 1)
                    {
                        CameraVariable scanline = scanlineVariables[scanlineY];
                        scanline.scanlineMinX = scanline.scanlineMaxX = edgeXStart;
                        scanline.scanlineMinValue = scanline.scanlineMaxValue = edgeShadeStart;
                        edgeXStart += edgeXSlope;
                        edgeShadeStart += edgeShadeSlope;
                    }

                }
                else if (firstScanlineY > lastScanlineY)
                {
                    int revEdgeXStart = polygonX[lastVertIndex] << 8;
                    int revEdgeXSlope = (polygonX[0] - polygonX[lastVertIndex] << 8) / (firstScanlineY - lastScanlineY);
                    int revEdgeShadeStart = polygonZ[lastVertIndex] << 8;
                    int revEdgeShadeSlope = (polygonZ[0] - polygonZ[lastVertIndex] << 8) / (firstScanlineY - lastScanlineY);
                    if (lastScanlineY < 0)
                    {
                        revEdgeXStart -= revEdgeXSlope * lastScanlineY;
                        revEdgeShadeStart -= revEdgeShadeSlope * lastScanlineY;
                        lastScanlineY = 0;
                    }
                    if (firstScanlineY > maxVisibleScanline)
                    {
                        firstScanlineY = maxVisibleScanline;
                    }

                    for (scanlineY = lastScanlineY; scanlineY <= firstScanlineY; scanlineY += 1)
                    {
                        CameraVariable scanline = scanlineVariables[scanlineY];
                        scanline.scanlineMinX = scanline.scanlineMaxX = revEdgeXStart;
                        scanline.scanlineMinValue = scanline.scanlineMaxValue = revEdgeShadeStart;
                        revEdgeXStart += revEdgeXSlope;
                        revEdgeShadeStart += revEdgeShadeSlope;
                    }

                }
                for (scanlineY = 0; scanlineY < lastVertIndex; scanlineY += 1)
                {
                    int nextEdgeVertIndex = scanlineY + 1;
                    int edgeY0 = polygonY[scanlineY];
                    int edgeY1 = polygonY[nextEdgeVertIndex];
                    if (edgeY0 < edgeY1)
                    {
                        int edgeX = polygonX[scanlineY] << 8;
                        int edgeXSlopeLocal = (polygonX[nextEdgeVertIndex] - polygonX[scanlineY] << 8) / (edgeY1 - edgeY0);
                        int edgeShade = polygonZ[scanlineY] << 8;
                        int edgeShadeSlopeLocal = (polygonZ[nextEdgeVertIndex] - polygonZ[scanlineY] << 8) / (edgeY1 - edgeY0);
                        if (edgeY0 < 0)
                        {
                            edgeX -= edgeXSlopeLocal * edgeY0;
                            edgeShade -= edgeShadeSlopeLocal * edgeY0;
                            edgeY0 = 0;
                        }
                        if (edgeY1 > maxVisibleScanline)
                        {
                            edgeY1 = maxVisibleScanline;
                        }

                        for (int scanlineYForward = edgeY0; scanlineYForward <= edgeY1; scanlineYForward += 1)
                        {
                            CameraVariable scanline = scanlineVariables[scanlineYForward];
                            if (edgeX < scanline.scanlineMinX)
                            {
                                scanline.scanlineMinX = edgeX;
                                scanline.scanlineMinValue = edgeShade;
                            }
                            if (edgeX > scanline.scanlineMaxX)
                            {
                                scanline.scanlineMaxX = edgeX;
                                scanline.scanlineMaxValue = edgeShade;
                            }
                            edgeX += edgeXSlopeLocal;
                            edgeShade += edgeShadeSlopeLocal;
                        }

                    }
                    else if (edgeY0 > edgeY1)
                    {
                        int revEdgeX = polygonX[nextEdgeVertIndex] << 8;
                        int revEdgeXSlopeLocal = (polygonX[scanlineY] - polygonX[nextEdgeVertIndex] << 8) / (edgeY0 - edgeY1);
                        int revEdgeShade = polygonZ[nextEdgeVertIndex] << 8;
                        int revEdgeShadeSlopeLocal = (polygonZ[scanlineY] - polygonZ[nextEdgeVertIndex] << 8) / (edgeY0 - edgeY1);
                        if (edgeY1 < 0)
                        {
                            revEdgeX -= revEdgeXSlopeLocal * edgeY1;
                            revEdgeShade -= revEdgeShadeSlopeLocal * edgeY1;
                            edgeY1 = 0;
                        }
                        if (edgeY0 > maxVisibleScanline)
                        {
                            edgeY0 = maxVisibleScanline;
                        }

                        for (int scanlineYBackward = edgeY1; scanlineYBackward <= edgeY0; scanlineYBackward += 1)
                        {
                            CameraVariable scanline = scanlineVariables[scanlineYBackward];
                            if (revEdgeX < scanline.scanlineMinX)
                            {
                                scanline.scanlineMinX = revEdgeX;
                                scanline.scanlineMinValue = revEdgeShade;
                            }
                            if (revEdgeX > scanline.scanlineMaxX)
                            {
                                scanline.scanlineMaxX = revEdgeX;
                                scanline.scanlineMaxValue = revEdgeShade;
                            }
                            revEdgeX += revEdgeXSlopeLocal;
                            revEdgeShade += revEdgeShadeSlopeLocal;
                        }

                    }
                }

                if (minVisibleScanline < scanlineBufferCentre - screenCentreY)
                {
                    minVisibleScanline = scanlineBufferCentre - screenCentreY;
                }
            }
            if (isMousePositionUpdated &&
                optionCount < maxHighlightedObjects &&
                mouseAdjustedY >= minVisibleScanline &&
                mouseAdjustedY < maxVisibleScanline)
            {
                CameraVariable m2 = scanlineVariables[mouseAdjustedY];
                bool hitX = mouseAdjustedX >= m2.scanlineMinX >> 8 && mouseAdjustedX <= m2.scanlineMaxX >> 8;
                bool hitScanline = m2.scanlineMinX <= m2.scanlineMaxX;
                bool hitShare = !gameObject.shareEntityArrays;
                bool hitPoly = hitShare && gameObject.polygonTypeData[faceVertexIndex] == 0;
                if (hitX && hitScanline && hitShare && hitPoly)
                {
                    highlightedObjects[optionCount] = gameObject;
                    highlightedPlayerIds[optionCount] = faceVertexIndex;
                    optionCount += 1;
                }
            }
        }

        private void RenderModel(
            int scanlineY,
            int scanlineX,
            int vertCount,
            int[] vertX,
            int[] vertY,
            int[] vertZ,
            int textureIndex,
            GameObject gameObject)
        {
            if (textureIndex == -2)
            {
                return;
            }

            if (textureIndex >= 0)
            {
                if (textureIndex >= textureManager.textureCount)
                {
                    textureIndex = 0;
                }

                textureManager.UpdateTextureSmoothing(textureIndex);
                int vert0X = vertX[0];
                int vert0Y = vertY[0];
                int vert0Z = vertZ[0];
                int edge1X = vert0X - vertX[1];
                int edge1Y = vert0Y - vertY[1];
                int edge1Z = vert0Z - vertZ[1];
                vertCount -= 1;
                int edge2X = vertX[vertCount] - vert0X;
                int edge2Y = vertY[vertCount] - vert0Y;
                int edge2Z = vertZ[vertCount] - vert0Z;
                if (textureManager.textureLastAccessFrame[textureIndex] == 1)
                {
                    int texUOrigin = edge2X * vert0Y - edge2Y * vert0X << 12;
                    int texURowStep = edge2Y * vert0Z - edge2Z * vert0Y << 5 - screenProjectionShift + 7 + 4;
                    int texUColStep = edge2Z * vert0X - edge2X * vert0Z << 5 - screenProjectionShift + 7;
                    int texVOrigin = edge1X * vert0Y - edge1Y * vert0X << 12;
                    int texVRowStep = edge1Y * vert0Z - edge1Z * vert0Y << 5 - screenProjectionShift + 7 + 4;
                    int texVColStep = edge1Z * vert0X - edge1X * vert0Z << 5 - screenProjectionShift + 7;
                    int texDenomOrigin = edge1Y * edge2X - edge1X * edge2Y << 5;
                    int texDenomRowStep = edge1Z * edge2Y - edge1Y * edge2Z << 5 - screenProjectionShift + 4;
                    int texDenomColStep = edge1X * edge2Z - edge1Z * edge2X >> screenProjectionShift - 5;
                    int texURowStepScaled = texURowStep >> 4;
                    int texVRowStepScaled = texVRowStep >> 4;
                    int texDenomRowStepScaled = texDenomRowStep >> 4;
                    int scanlineOffset = minVisibleScanline - scanlineBufferCentre;
                    int rowStride = defaultScreenHalfWidth;
                    int pixelOffset = screenMouseOffsetX + minVisibleScanline * rowStride;
                    byte scanlineStep128 = 1;
                    texUOrigin += texUColStep * scanlineOffset;
                    texVOrigin += texVColStep * scanlineOffset;
                    texDenomOrigin += texDenomColStep * scanlineOffset;
                    if (isRenderingInterlaced)
                    {
                        if ((minVisibleScanline & 1) == 1)
                        {
                            minVisibleScanline += 1;
                            texUOrigin += texUColStep;
                            texVOrigin += texVColStep;
                            texDenomOrigin += texDenomColStep;
                            pixelOffset += rowStride;
                        }
                        texUColStep <<= 1;
                        texVColStep <<= 1;
                        texDenomColStep <<= 1;
                        rowStride <<= 1;
                        scanlineStep128 = 2;
                    }
                    if (gameObject.isPerspectiveTextured)
                    {
                        for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStep128)
                        {
                            CameraVariable scanline = scanlineVariables[scanlineY];
                            scanlineX = scanline.scanlineMinX >> 8;
                            int scanlineMaxX = scanline.scanlineMaxX >> 8;
                            int spanWidth = scanlineMaxX - scanlineX;
                            if (spanWidth <= 0)
                            {
                                texUOrigin += texUColStep;
                                texVOrigin += texVColStep;
                                texDenomOrigin += texDenomColStep;
                                pixelOffset += rowStride;
                            }
                            else
                            {
                                int shadeAccum = scanline.scanlineMinValue;
                                int shadeStep = (scanline.scanlineMaxValue - shadeAccum) / spanWidth;
                                if (scanlineX < -screenCentreX)
                                {
                                    shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                                    scanlineX = -screenCentreX;
                                    spanWidth = scanlineMaxX - scanlineX;
                                }
                                if (scanlineMaxX > screenCentreX)
                                {
                                    int clampedX = screenCentreX;
                                    spanWidth = clampedX - scanlineX;
                                }
                                CameraPolygonDrawer.DrawShadedPolygon(
                                    screenPixels,
                                    textureManager.objectTexturePixels[textureIndex],
                                    0,
                                    0,
                                    texUOrigin + texURowStepScaled * scanlineX,
                                    texVOrigin + texVRowStepScaled * scanlineX,
                                    texDenomOrigin + texDenomRowStepScaled * scanlineX,
                                    texURowStep,
                                    texVRowStep,
                                    texDenomRowStep,
                                    spanWidth,
                                    pixelOffset + scanlineX,
                                    shadeAccum,
                                    shadeStep << 2);
                                texUOrigin += texUColStep;
                                texVOrigin += texVColStep;
                                texDenomOrigin += texDenomColStep;
                                pixelOffset += rowStride;
                            }
                        }

                        return;
                    }
                    if (!textureManager.textureIsTransparent[textureIndex])
                    {
                        for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStep128)
                        {
                            CameraVariable scanline = scanlineVariables[scanlineY];
                            scanlineX = scanline.scanlineMinX >> 8;
                            int scanlineMaxX = scanline.scanlineMaxX >> 8;
                            int spanWidth = scanlineMaxX - scanlineX;
                            if (spanWidth <= 0)
                            {
                                texUOrigin += texUColStep;
                                texVOrigin += texVColStep;
                                texDenomOrigin += texDenomColStep;
                                pixelOffset += rowStride;
                            }
                            else
                            {
                                int shadeAccum64 = scanline.scanlineMinValue;
                                int shadeStep64 = (scanline.scanlineMaxValue - shadeAccum64) / spanWidth;
                                if (scanlineX < -screenCentreX)
                                {
                                    shadeAccum64 += (-screenCentreX - scanlineX) * shadeStep64;
                                    scanlineX = -screenCentreX;
                                    spanWidth = scanlineMaxX - scanlineX;
                                }
                                if (scanlineMaxX > screenCentreX)
                                {
                                    int clampedX = screenCentreX;
                                    spanWidth = clampedX - scanlineX;
                                }
                                CameraPolygonDrawer.DrawFlatPolygon(
                                    screenPixels,
                                    textureManager.objectTexturePixels[textureIndex],
                                    0,
                                    0,
                                    texUOrigin + texURowStepScaled * scanlineX,
                                    texVOrigin + texVRowStepScaled * scanlineX,
                                    texDenomOrigin + texDenomRowStepScaled * scanlineX,
                                    texURowStep,
                                    texVRowStep,
                                    texDenomRowStep,
                                    spanWidth,
                                    pixelOffset + scanlineX,
                                    shadeAccum64,
                                    shadeStep64 << 2);
                                texUOrigin += texUColStep;
                                texVOrigin += texVColStep;
                                texDenomOrigin += texDenomColStep;
                                pixelOffset += rowStride;
                            }
                        }

                        return;
                    }
                    for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStep128)
                    {
                        CameraVariable scanline = scanlineVariables[scanlineY];
                        scanlineX = scanline.scanlineMinX >> 8;
                        int scanlineMaxX = scanline.scanlineMaxX >> 8;
                        int spanWidth = scanlineMaxX - scanlineX;
                        if (spanWidth <= 0)
                        {
                            texUOrigin += texUColStep;
                            texVOrigin += texVColStep;
                            texDenomOrigin += texDenomColStep;
                            pixelOffset += rowStride;
                        }
                        else
                        {
                            int shadeAccum = scanline.scanlineMinValue;
                            int shadeStep = (scanline.scanlineMaxValue - shadeAccum) / spanWidth;
                            if (scanlineX < -screenCentreX)
                            {
                                shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                                scanlineX = -screenCentreX;
                                spanWidth = scanlineMaxX - scanlineX;
                            }
                            if (scanlineMaxX > screenCentreX)
                            {
                                int clampedX = screenCentreX;
                                spanWidth = clampedX - scanlineX;
                            }
                            CameraPolygonDrawer.DrawTexturedPolygon(
                                screenPixels,
                                0,
                                0,
                                0,
                                textureManager.objectTexturePixels[textureIndex],
                                texUOrigin + texURowStepScaled * scanlineX,
                                texVOrigin + texVRowStepScaled * scanlineX,
                                texDenomOrigin + texDenomRowStepScaled * scanlineX,
                                texURowStep,
                                texVRowStep,
                                texDenomRowStep,
                                spanWidth,
                                pixelOffset + scanlineX,
                                shadeAccum,
                                shadeStep);
                            texUOrigin += texUColStep;
                            texVOrigin += texVColStep;
                            texDenomOrigin += texDenomColStep;
                            pixelOffset += rowStride;
                        }
                    }

                    return;
                }
                int texUOrigin64 = (edge2X * vert0Y - edge2Y * vert0X) << 11;
                int texURowStep64 = (edge2Y * vert0Z - edge2Z * vert0Y) << 5 - screenProjectionShift + 6 + 4;
                int texUColStep64 = (edge2Z * vert0X - edge2X * vert0Z) << 5 - screenProjectionShift + 6;
                int texVOrigin64 = (edge1X * vert0Y - edge1Y * vert0X) << 11;
                int texVRowStep64 = (edge1Y * vert0Z - edge1Z * vert0Y) << 5 - screenProjectionShift + 6 + 4;
                int texVColStep64 = (edge1Z * vert0X - edge1X * vert0Z) << 5 - screenProjectionShift + 6;
                int texDenomOrigin64 = (edge1Y * edge2X - edge1X * edge2Y) << 5;
                int texDenomRowStep64 = (edge1Z * edge2Y - edge1Y * edge2Z) << 5 - screenProjectionShift + 4;
                int texDenomColStep64 = (edge1X * edge2Z - edge1Z * edge2X) >> (screenProjectionShift - 5);
                int texURowStepScaled64 = texURowStep64 >> 4;
                int texVRowStepScaled64 = texVRowStep64 >> 4;
                int texDenomRowStepScaled64 = texDenomRowStep64 >> 4;
                int scanlineOffset64 = minVisibleScanline - scanlineBufferCentre;
                int rowStride64 = defaultScreenHalfWidth;
                int pixelOffset64 = screenMouseOffsetX + minVisibleScanline * rowStride64;
                byte scanlineStep64 = 1;
                texUOrigin64 += texUColStep64 * scanlineOffset64;
                texVOrigin64 += texVColStep64 * scanlineOffset64;
                texDenomOrigin64 += texDenomColStep64 * scanlineOffset64;
                if (isRenderingInterlaced)
                {
                    if ((minVisibleScanline & 1) == 1)
                    {
                        minVisibleScanline += 1;
                        texUOrigin64 += texUColStep64;
                        texVOrigin64 += texVColStep64;
                        texDenomOrigin64 += texDenomColStep64;
                        pixelOffset64 += rowStride64;
                    }
                    texUColStep64 <<= 1;
                    texVColStep64 <<= 1;
                    texDenomColStep64 <<= 1;
                    rowStride64 <<= 1;
                    scanlineStep64 = 2;
                }
                if (gameObject.isPerspectiveTextured)
                {
                    for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStep64)
                    {
                        CameraVariable scanline = scanlineVariables[scanlineY];
                        scanlineX = scanline.scanlineMinX >> 8;
                        int clampedX64 = scanline.scanlineMaxX >> 8;
                        int spanWidth = clampedX64 - scanlineX;
                        if (spanWidth <= 0)
                        {
                            texUOrigin64 += texUColStep64;
                            texVOrigin64 += texVColStep64;
                            texDenomOrigin64 += texDenomColStep64;
                            pixelOffset64 += rowStride64;
                        }
                        else
                        {
                            int shadeAccum = scanline.scanlineMinValue;
                            int shadeStep = (scanline.scanlineMaxValue - shadeAccum) / spanWidth;
                            if (scanlineX < -screenCentreX)
                            {
                                shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                                scanlineX = -screenCentreX;
                                spanWidth = clampedX64 - scanlineX;
                            }
                            if (clampedX64 > screenCentreX)
                            {
                                int clampedX = screenCentreX;
                                spanWidth = clampedX - scanlineX;
                            }
                            CameraPolygonDrawer.DrawMaskedPolygon(
                                screenPixels,
                                textureManager.objectTexturePixels[textureIndex],
                                0,
                                0,
                                texUOrigin64 + texURowStepScaled64 * scanlineX,
                                texVOrigin64 + texVRowStepScaled64 * scanlineX,
                                texDenomOrigin64 + texDenomRowStepScaled64 * scanlineX,
                                texURowStep64,
                                texVRowStep64,
                                texDenomRowStep64,
                                spanWidth,
                                pixelOffset64 + scanlineX,
                                shadeAccum,
                                shadeStep);
                            texUOrigin64 += texUColStep64;
                            texVOrigin64 += texVColStep64;
                            texDenomOrigin64 += texDenomColStep64;
                            pixelOffset64 += rowStride64;
                        }
                    }

                    return;
                }
                if (!textureManager.textureIsTransparent[textureIndex])
                {
                    for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStep64)
                    {
                        CameraVariable scanline = scanlineVariables[scanlineY];
                        scanlineX = scanline.scanlineMinX >> 8;
                        int scanlineMaxX = scanline.scanlineMaxX >> 8;
                        int spanWidth = scanlineMaxX - scanlineX;
                        if (spanWidth <= 0)
                        {
                            texUOrigin64 += texUColStep64;
                            texVOrigin64 += texVColStep64;
                            texDenomOrigin64 += texDenomColStep64;
                            pixelOffset64 += rowStride64;
                        }
                        else
                        {
                            int shadeAccum = scanline.scanlineMinValue;
                            int shadeStep = (scanline.scanlineMaxValue - shadeAccum) / spanWidth;
                            if (scanlineX < -screenCentreX)
                            {
                                shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                                scanlineX = -screenCentreX;
                                spanWidth = scanlineMaxX - scanlineX;
                            }
                            if (scanlineMaxX > screenCentreX)
                            {
                                int clampedX = screenCentreX;
                                spanWidth = clampedX - scanlineX;
                            }
                            CameraPolygonDrawer.DrawTransparentPolygon(
                                screenPixels,
                                textureManager.objectTexturePixels[textureIndex],
                                0,
                                0,
                                texUOrigin64 + texURowStepScaled64 * scanlineX,
                                texVOrigin64 + texVRowStepScaled64 * scanlineX,
                                texDenomOrigin64 + texDenomRowStepScaled64 * scanlineX,
                                texURowStep64,
                                texVRowStep64,
                                texDenomRowStep64,
                                spanWidth,
                                pixelOffset64 + scanlineX,
                                shadeAccum,
                                shadeStep);
                            texUOrigin64 += texUColStep64;
                            texVOrigin64 += texVColStep64;
                            texDenomOrigin64 += texDenomColStep64;
                            pixelOffset64 += rowStride64;
                        }
                    }

                    return;
                }
                for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStep64)
                {
                    CameraVariable scanline = scanlineVariables[scanlineY];
                    scanlineX = scanline.scanlineMinX >> 8;
                    int scanlineMaxX = scanline.scanlineMaxX >> 8;
                    int spanWidth = scanlineMaxX - scanlineX;
                    if (spanWidth <= 0)
                    {
                        texUOrigin64 += texUColStep64;
                        texVOrigin64 += texVColStep64;
                        texDenomOrigin64 += texDenomColStep64;
                        pixelOffset64 += rowStride64;
                    }
                    else
                    {
                        int shadeAccum = scanline.scanlineMinValue;
                        int shadeStep = (scanline.scanlineMaxValue - shadeAccum) / spanWidth;
                        if (scanlineX < -screenCentreX)
                        {
                            shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                            scanlineX = -screenCentreX;
                            spanWidth = scanlineMaxX - scanlineX;
                        }
                        if (scanlineMaxX > screenCentreX)
                        {
                            int spanWidth64 = screenCentreX;
                            spanWidth = spanWidth64 - scanlineX;
                        }
                        CameraPolygonDrawer.DrawFlatTexturedPolygon(
                            screenPixels,
                            0,
                            0,
                            0,
                            textureManager.objectTexturePixels[textureIndex],
                            texUOrigin64 + texURowStepScaled64 * scanlineX,
                            texVOrigin64 + texVRowStepScaled64 * scanlineX,
                            texDenomOrigin64 + texDenomRowStepScaled64 * scanlineX,
                            texURowStep64,
                            texVRowStep64,
                            texDenomRowStep64,
                            spanWidth,
                            pixelOffset64 + scanlineX,
                            shadeAccum,
                            shadeStep);
                        texUOrigin64 += texUColStep64;
                        texVOrigin64 += texVColStep64;
                        texDenomOrigin64 += texDenomColStep64;
                        pixelOffset64 += rowStride64;
                    }
                }

                return;
            }
            for (int clipIndex = 0; clipIndex < textureManager.maxTextureCount; clipIndex += 1)
            {
                if (textureManager.textureClipIds[clipIndex] == textureIndex)
                {
                    textureManager.textureClipSizes = textureManager.textureClipData[clipIndex];
                    break;
                }
                if (clipIndex == textureManager.maxTextureCount - 1)
                {
                    double randomValue = Helper.Random.NextDouble();
                    int randomSlot = (int)(randomValue /*Math.random()*/ * textureManager.maxTextureCount);
                    if (randomSlot >= textureManager.textureClipIds.Length)
                    {
                        randomSlot -= 1;
                    }

                    textureManager.textureClipIds[randomSlot] = textureIndex;
                    textureIndex = -1 - textureIndex;
                    int redChannel = (textureIndex >> 10 & 0x1f) * 8;
                    int greenChannel = (textureIndex >> 5 & 0x1f) * 8;
                    int blueChannel = (textureIndex & 0x1f) * 8;
                    for (int colourTableIndex = 0; colourTableIndex < 256; colourTableIndex += 1)
                    {
                        int squaredIntensity = colourTableIndex * colourTableIndex;
                        int redScaled = redChannel * squaredIntensity / SquaredIntensityDivisor;
                        int greenScaled = greenChannel * squaredIntensity / SquaredIntensityDivisor;
                        int blueScaled = blueChannel * squaredIntensity / SquaredIntensityDivisor;
                        textureManager.textureClipData[randomSlot][255 - colourTableIndex] = (redScaled << 16) + (greenScaled << 8) + blueScaled;
                    }

                    textureManager.textureClipSizes = textureManager.textureClipData[randomSlot];
                }
            }

            int rowStrideColour = defaultScreenHalfWidth;
            int pixelOffsetColour = screenMouseOffsetX + minVisibleScanline * rowStrideColour;
            byte scanlineStepColour = 1;
            if (isRenderingInterlaced)
            {
                if ((minVisibleScanline & 1) == 1)
                {
                    minVisibleScanline += 1;
                    pixelOffsetColour += rowStrideColour;
                }
                rowStrideColour <<= 1;
                scanlineStepColour = 2;
            }
            if (gameObject.isGiantCrystal)
            {
                for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStepColour)
                {
                    CameraVariable scanline = scanlineVariables[scanlineY];
                    scanlineX = scanline.scanlineMinX >> 8;
                    int scanlineMaxXColour = scanline.scanlineMaxX >> 8;
                    int spanWidthColour = scanlineMaxXColour - scanlineX;
                    if (spanWidthColour <= 0)
                    {
                        pixelOffsetColour += rowStrideColour;
                    }
                    else
                    {
                        int shadeAccumColour = scanline.scanlineMinValue;
                        int shadeStepColour = (scanline.scanlineMaxValue - shadeAccumColour) / spanWidthColour;
                        if (scanlineX < -screenCentreX)
                        {
                            shadeAccumColour += (-screenCentreX - scanlineX) * shadeStepColour;
                            scanlineX = -screenCentreX;
                            spanWidthColour = scanlineMaxXColour - scanlineX;
                        }
                        if (scanlineMaxXColour > screenCentreX)
                        {
                            int clampedXColour = screenCentreX;
                            spanWidthColour = clampedXColour - scanlineX;
                        }
                        CameraPolygonDrawer.DrawShiftColorPolygon(
                            screenPixels,
                            -spanWidthColour,
                            pixelOffsetColour + scanlineX,
                            0,
                            textureManager.textureClipSizes,
                            shadeAccumColour,
                            shadeStepColour);
                        pixelOffsetColour += rowStrideColour;
                    }
                }

                return;
            }
            if (isInterlaced)
            {
                for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStepColour)
                {
                    CameraVariable scanline = scanlineVariables[scanlineY];
                    scanlineX = scanline.scanlineMinX >> 8;
                    int scanlineMaxXInterlaced = scanline.scanlineMaxX >> 8;
                    int spanWidthInterlaced = scanlineMaxXInterlaced - scanlineX;
                    if (spanWidthInterlaced <= 0)
                    {
                        pixelOffsetColour += rowStrideColour;
                    }
                    else
                    {
                        int shadeAccumInterlaced = scanline.scanlineMinValue;
                        int shadeStepInterlaced = (scanline.scanlineMaxValue - shadeAccumInterlaced) / spanWidthInterlaced;
                        if (scanlineX < -screenCentreX)
                        {
                            shadeAccumInterlaced += (-screenCentreX - scanlineX) * shadeStepInterlaced;
                            scanlineX = -screenCentreX;
                            spanWidthInterlaced = scanlineMaxXInterlaced - scanlineX;
                        }
                        if (scanlineMaxXInterlaced > screenCentreX)
                        {
                            int clampedXInterlaced = screenCentreX;
                            spanWidthInterlaced = clampedXInterlaced - scanlineX;
                        }
                        CameraPolygonDrawer.DrawVertexColorPolygon(
                            screenPixels,
                            -spanWidthInterlaced,
                            pixelOffsetColour + scanlineX,
                            0,
                            textureManager.textureClipSizes,
                            shadeAccumInterlaced,
                            shadeStepInterlaced);
                        pixelOffsetColour += rowStrideColour;
                    }
                }

                return;
            }
            for (scanlineY = minVisibleScanline; scanlineY < maxVisibleScanline; scanlineY += scanlineStepColour)
            {
                CameraVariable scanline = scanlineVariables[scanlineY];
                scanlineX = scanline.scanlineMinX >> 8;
                int scanlineMaxXDefault = scanline.scanlineMaxX >> 8;
                int spanWidthDefault = scanlineMaxXDefault - scanlineX;
                if (spanWidthDefault <= 0)
                {
                    pixelOffsetColour += rowStrideColour;
                }
                else
                {
                    int shadeAccumDefault = scanline.scanlineMinValue;
                    int shadeStepDefault = (scanline.scanlineMaxValue - shadeAccumDefault) / spanWidthDefault;
                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccumDefault += (-screenCentreX - scanlineX) * shadeStepDefault;
                        scanlineX = -screenCentreX;
                        spanWidthDefault = scanlineMaxXDefault - scanlineX;
                    }
                    if (scanlineMaxXDefault > screenCentreX)
                    {
                        int clampedXDefault = screenCentreX;
                        spanWidthDefault = clampedXDefault - scanlineX;
                    }
                    CameraPolygonDrawer.DrawGradientPolygon(
                        screenPixels,
                        -spanWidthDefault,
                        pixelOffsetColour + scanlineX,
                        0,
                        textureManager.textureClipSizes,
                        shadeAccumDefault,
                        shadeStepDefault);
                    pixelOffsetColour += rowStrideColour;
                }
            }

        }

        public void SetCameraTransform(
            int x,
            int y,
            int z,
            int rotationX,
            int rotationY,
            int rotationZ,
            int distance)
        {
            rotationX &= 0x3ff;
            rotationY &= 0x3ff;
            rotationZ &= 0x3ff;
            cameraOffsetX = TrigTableHalfSize - rotationX & TrigTableMask;
            cameraOffsetY = TrigTableHalfSize - rotationY & TrigTableMask;
            cameraOffsetZ = TrigTableHalfSize - rotationZ & TrigTableMask;
            int xOffset = 0;
            int yOffset = 0;
            int zOffset = distance;

            if (rotationX != 0)
            {
                int rotXSin = trigonometryTable[rotationX];
                int rotXCos = trigonometryTable[rotationX + TrigTableHalfSize];
                int rotatedY = yOffset * rotXCos - zOffset * rotXSin >> 15;
                zOffset = yOffset * rotXSin + zOffset * rotXCos >> 15;
                yOffset = rotatedY;
            }

            if (rotationY != 0)
            {
                int rotYSin = trigonometryTable[rotationY];
                int rotYCos = trigonometryTable[rotationY + TrigTableHalfSize];
                int rotatedX = zOffset * rotYSin + xOffset * rotYCos >> 15;
                zOffset = zOffset * rotYCos - xOffset * rotYSin >> 15;
                xOffset = rotatedX;
            }

            if (rotationZ != 0)
            {
                int rotZSin = trigonometryTable[rotationZ];
                int rotZCos = trigonometryTable[rotationZ + TrigTableHalfSize];
                int rotatedY = yOffset * rotZSin + xOffset * rotZCos >> 15;
                yOffset = yOffset * rotZCos - xOffset * rotZSin >> 15;
                xOffset = rotatedY;
            }

            viewX = x - xOffset;
            viewY = y - yOffset;
            viewZ = z - zOffset;
        }


        public void CreateTexture(int totalCount, int pixelBufferCount, int colourMapCount)
            => textureManager.CreateTexture(totalCount, pixelBufferCount, colourMapCount);

        public void SetTexture(int textureIndex, sbyte[] colourIndices, int[] colourArray, int frameType)
            => textureManager.SetTexture(textureIndex, colourIndices, colourArray, frameType);

        public void UpdateTextureSmoothing(int textureIndex)
            => textureManager.UpdateTextureSmoothing(textureIndex);

        public void UpdateLighting(int textureIndex)
            => textureManager.UpdateLighting(textureIndex);

        public int ApplyTextureSmoothing(int index)
            => textureManager.ApplyTextureSmoothing(index, NotSetSentinel);

        public void OffsetAllModelColours(int redOffset, int greenOffset, int blueOffset)
        {
            if (redOffset == 0 && greenOffset == 0 && blueOffset == 0)
            {
                redOffset = 32;
            }

            for (int modelIndex = 0; modelIndex < currentObjectCount; modelIndex += 1)
            {
                objectCache[modelIndex].OffsetModelColors(redOffset, greenOffset, blueOffset);
            }
        }

        public void SetAllModelColours(int fromColour, int toColour, int x, int y, int z)
        {
            if (x == 0 && y == 0 && z == 0)
            {
                x = 32;
            }

            for (int modelIndex = 0; modelIndex < currentObjectCount; modelIndex += 1)
            {
                objectCache[modelIndex].SetModelColors(fromColour, toColour, x, y, z);
            }
        }

        public static int GetTextureColour(int r, int g, int b)
            => CameraTextureManager.GetTextureColour(r, g, b);
    }
}
