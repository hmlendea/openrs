using System.Threading;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class Camera
    {
        private static sbyte[] lookupTable;

        private readonly int lightingFactor;
        private readonly CameraTextureManager textureManager;
        private readonly int[] modelPriorities;
        private readonly CameraModel[] visibleModels;
        private readonly CameraSceneObjectTracker sceneObjectTracker;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<Camera>();
        private readonly CameraDepthSorter depthSorter = new();
        private readonly CameraPolygonRasteriser polygonRasteriser;
        private readonly CameraModelRenderer modelRenderer;
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
        private int currentObjectCount;
        private int totalModelCount;
        private GameObject[] objectCache;
        private GameImage gameImage;
        private int[] clippedPolygonScreenX;
        private int[] clippedPolygonScreenY;
        private int[] clippedPolygonShadeLevels;
        private int[] polygonVertexScreenX;
        private int[] polygonVertexScreenY;
        private int[] polygonVertexDepth;

        public static int[] TrigonometryTable => CameraTrigonometryTable.Table;

        public static int NearX { get; set; }

        public static int FarX { get; set; }

        public static int NearY { get; set; }

        public static int FarY { get; set; }

        public static int NearZ { get; set; }

        public static int FarZ { get; set; }

        public int SavedModelIndex { get; set; }

        public int NearPlane { get; set; }

        public int FarClipDistance { get; set; }

        public int SpriteFarClipDistance { get; set; }

        public int FogGradientStep { get; set; }

        public int FogStartDistance { get; set; }

        public bool IsInterlaced
        {
            get => modelRenderer.IsInterlaced;
            set => modelRenderer.IsInterlaced = value;
        }

        public double ScaleFactor { get; set; }

        public int DepthSortStride { get; set; }

        public GameObject HighlightedObject => sceneObjectTracker.HighlightedObject;

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
        private static int DepthSortLookAheadCount => 100;

        private static double DefaultScaleFactor => 1.1D;

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

            NearPlane = DefaultNearPlane;
            FarClipDistance = DefaultZoom;
            SpriteFarClipDistance = DefaultZoom;
            FogGradientStep = DefaultFogGradientStep;
            FogStartDistance = DefaultFogGradientThreshold;
            ScaleFactor = DefaultScaleFactor;
            DepthSortStride = 1;
            defaultScreenHalfWidth = DefaultScreenHalfWidth;
            screenCentreX = gameImageSource.GameWidth / 2;
            screenCentreY = gameImageSource.GameHeight / 2;
            screenMouseOffsetX = DefaultMouseOffsetX;
            scanlineBufferCentre = DefaultScanlineBufferCentre;
            screenProjectionShift = DefaultScreenProjectionShift;
            lightingFactor = DefaultLightingFactor;
            clippedPolygonScreenX = new int[ClipPolygonCapacity];
            clippedPolygonScreenY = new int[ClipPolygonCapacity];
            clippedPolygonShadeLevels = new int[ClipPolygonCapacity];
            polygonVertexScreenX = new int[ClipPolygonCapacity];
            polygonVertexScreenY = new int[ClipPolygonCapacity];
            polygonVertexDepth = new int[ClipPolygonCapacity];
            gameImage = gameImageSource;
            totalModelCount = maxObjects;
            objectCache = new GameObject[totalModelCount];
            modelPriorities = new int[totalModelCount];
            visibleModels = new CameraModel[maxVisibleObjects];

            for (int visibleModelIndex = 0; visibleModelIndex < maxVisibleObjects; visibleModelIndex += 1)
            {
                visibleModels[visibleModelIndex] = new CameraModel();
            }

            sceneObjectTracker = new CameraSceneObjectTracker(maxSceneObjects, MaxHighlightedObjectCount);
            polygonRasteriser = new CameraPolygonRasteriser(sceneObjectTracker);
            modelRenderer = new CameraModelRenderer(polygonRasteriser, textureManager, gameImageSource.Pixels);
            lookupTable ??= new sbyte[LookupTableSize];
        }

        public void AddModel(GameObject gameObject)
        {
            if (gameObject is null)
            {
                logger.Warn(GameOperation.AddSceneObject, "Attempted to add a null object.");
                return;
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
            => sceneObjectTracker.InitializeScene();

        public void RemoveLastUpdates(int count)
            => sceneObjectTracker.RemoveLastUpdates(count);

        public int AddSpriteToScene(
            int objectId,
            int x,
            int y,
            int z,
            int width,
            int height,
            int entityType)
            => sceneObjectTracker.AddSpriteToScene(objectId, x, y, z, width, height, entityType);

        public void RemoveSprite(int spriteIndex)
            => sceneObjectTracker.RemoveSprite(spriteIndex);

        public void UpdateSpritePosition(int spriteIndex, int frameIndex)
            => sceneObjectTracker.UpdateSpritePosition(spriteIndex, frameIndex);

        public void SetMousePosition(int mouseX, int mouseY)
            => sceneObjectTracker.SetMousePosition(mouseX - screenMouseOffsetX, mouseY);

        public int GetOptionCount()
            => sceneObjectTracker.GetOptionCount();

        public int[] GetHighlightedPlayers()
            => sceneObjectTracker.GetHighlightedPlayers();

        public GameObject[] GetHighlightedObjects()
            => sceneObjectTracker.GetHighlightedObjects();

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
            polygonRasteriser.Initialise(
                centreScrY + scanlineCentre,
                scanlineCentre,
                centreScrY);
            modelRenderer.Initialise(
                centreScrX,
                mouseOffsetX,
                screenHalfWidth,
                scanlineCentre,
                projectionShift);
        }

        public void SetViewAngle(int viewX, int viewY, int viewZ)
        {
            int rollAngle = -cameraOffsetX + CameraTrigonometryTable.TrigTableHalfSize & CameraTrigonometryTable.TrigTableMask;
            int pitchAngle = -cameraOffsetY + CameraTrigonometryTable.TrigTableHalfSize & CameraTrigonometryTable.TrigTableMask;
            int yawAngle = -cameraOffsetZ + CameraTrigonometryTable.TrigTableHalfSize & CameraTrigonometryTable.TrigTableMask;

            if (yawAngle != 0)
            {
                int yawSin = CameraTrigonometryTable.Table[yawAngle];
                int yawCos = CameraTrigonometryTable.Table[yawAngle + CameraTrigonometryTable.TrigTableHalfSize];
                int rotatedX = viewY * yawSin + viewX * yawCos >> 15;
                viewY = viewY * yawCos - viewX * yawSin >> 15;
                viewX = rotatedX;
            }

            if (rollAngle != 0)
            {
                int rollSin = CameraTrigonometryTable.Table[rollAngle];
                int rollCos = CameraTrigonometryTable.Table[rollAngle + CameraTrigonometryTable.TrigTableHalfSize];
                int rotatedY = viewY * rollCos - viewZ * rollSin >> 15;
                viewZ = viewY * rollSin + viewZ * rollCos >> 15;
                viewY = rotatedY;
            }

            if (pitchAngle != 0)
            {
                int pitchSin = CameraTrigonometryTable.Table[pitchAngle];
                int pitchCos = CameraTrigonometryTable.Table[pitchAngle + CameraTrigonometryTable.TrigTableHalfSize];
                int rotatedZ = viewZ * pitchSin + viewX * pitchCos >> 15;
                viewZ = viewZ * pitchCos - viewX * pitchSin >> 15;
                viewX = rotatedZ;
            }

            if (viewX < NearX)
            {
                NearX = viewX;
            }

            if (viewX > FarX)
            {
                FarX = viewX;
            }

            if (viewY < NearY)
            {
                NearY = viewY;
            }

            if (viewY > FarY)
            {
                FarY = viewY;
            }

            if (viewZ < NearZ)
            {
                NearZ = viewZ;
            }

            if (viewZ > FarZ)
            {
                FarZ = viewZ;
            }
        }

        public void FinishCamera()
        {
            modelRenderer.IsRenderingInterlaced = gameImage.IsInterlaced;
            SetupProjectionBounds();

            if (!ProjectAllObjects())
            {
                return;
            }

            CollectVisiblePolygons();
            CollectVisibleSprites();

            if (currentModelIndex == 0)
            {
                return;
            }

            SavedModelIndex = currentModelIndex;
            depthSorter.SortByDepth(visibleModels, 0, currentModelIndex - 1);
            depthSorter.ResolveRenderOrder(DepthSortLookAheadCount, visibleModels, currentModelIndex);
            RenderSortedModels();
        }

        private void SetupProjectionBounds()
        {
            int halfProjectionWidth = screenCentreX * FarClipDistance >> screenProjectionShift;
            int halfProjectionHeight = screenCentreY * FarClipDistance >> screenProjectionShift;
            NearX = 0;
            FarX = 0;
            NearY = 0;
            FarY = 0;
            NearZ = 0;
            FarZ = 0;
            SetViewAngle(-halfProjectionWidth, -halfProjectionHeight, FarClipDistance);
            SetViewAngle(-halfProjectionWidth, halfProjectionHeight, FarClipDistance);
            SetViewAngle(halfProjectionWidth, -halfProjectionHeight, FarClipDistance);
            SetViewAngle(halfProjectionWidth, halfProjectionHeight, FarClipDistance);
            SetViewAngle(-screenCentreX, -screenCentreY, 0);
            SetViewAngle(-screenCentreX, screenCentreY, 0);
            SetViewAngle(screenCentreX, -screenCentreY, 0);
            SetViewAngle(screenCentreX, screenCentreY, 0);
            NearX += viewX;
            FarX += viewX;
            NearY += viewY;
            FarY += viewY;
            NearZ += viewZ;
            FarZ += viewZ;
        }

        private bool ProjectAllObjects()
        {
            objectCache[currentObjectCount] = sceneObjectTracker.HighlightedObject;
            sceneObjectTracker.HighlightedObject.ObjectState = 2;

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
                    NearPlane);
            }

            int msSlept = 0;

            while (objectCache[currentObjectCount] is null)
            {
                Thread.Sleep(10);
                msSlept += 10;

                if (msSlept > 1000)
                {
                    return false;
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
                NearPlane);
            currentModelIndex = 0;

            return true;
        }

        private void CollectVisiblePolygons()
        {
            for (int objectIndex = 0; objectIndex < currentObjectCount; objectIndex += 1)
            {
                GameObject currentObject = objectCache[objectIndex];

                if (currentObject is null)
                {
                    continue;
                }

                if (!currentObject.IsVisible)
                {
                    continue;
                }

                for (int faceIndex = 0; faceIndex < currentObject.FaceCount; faceIndex += 1)
                {
                    CollectVisibleFace(currentObject, faceIndex);
                }
            }
        }

        private void CollectVisibleFace(GameObject currentObject, int faceIndex)
        {
            int faceVertCount = currentObject.FaceVertexCounts[faceIndex];
            int[] faceVertIndices = currentObject.FaceVertexIndices[faceIndex];
            bool hasVisibleVertex = false;

            for (int vertIndex = 0; vertIndex < faceVertCount; vertIndex += 1)
            {
                int vertDepth = currentObject.ProjectedDepth[faceVertIndices[vertIndex]];

                if (vertDepth <= NearPlane || vertDepth >= FarClipDistance)
                {
                    continue;
                }

                hasVisibleVertex = true;
                break;
            }

            if (!hasVisibleVertex)
            {
                return;
            }

            int horizontalVisibilityFlags = 0;

            for (int horzCheckIndex = 0; horzCheckIndex < faceVertCount; horzCheckIndex += 1)
            {
                int projectedU = currentObject.ProjectedU[faceVertIndices[horzCheckIndex]];

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

            if (horizontalVisibilityFlags != 3)
            {
                return;
            }

            int verticalVisibilityFlags = 0;

            for (int vertCheckIndex = 0; vertCheckIndex < faceVertCount; vertCheckIndex += 1)
            {
                int projectedV = currentObject.ProjectedV[faceVertIndices[vertCheckIndex]];

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

            if (verticalVisibilityFlags != 3)
            {
                return;
            }

            CameraModel cameraModel = visibleModels[currentModelIndex];
            cameraModel.SourceObject = currentObject;
            cameraModel.FaceIndex = faceIndex;
            CameraModelBoundsCalculator.ComputePolygonBounds(cameraModel, lightingFactor);
            int textureIndex;

            if (cameraModel.VisibilityDot < 0)
            {
                textureIndex = currentObject.TextureBack[faceIndex];
            }
            else
            {
                textureIndex = currentObject.TextureFront[faceIndex];
            }

            if (textureIndex == NotSetSentinel)
            {
                return;
            }

            int totalDepth = 0;

            for (int depthAccumIndex = 0; depthAccumIndex < faceVertCount; depthAccumIndex += 1)
            {
                totalDepth += currentObject.ProjectedDepth[faceVertIndices[depthAccumIndex]];
            }

            cameraModel.Scale = totalDepth / faceVertCount + currentObject.ScaleBias;
            cameraModel.CurrentTextureIndex = textureIndex;
            currentModelIndex += 1;
        }

        private void CollectVisibleSprites()
        {
            GameObject spriteScene = sceneObjectTracker.HighlightedObject;

            if (!spriteScene.IsVisible)
            {
                return;
            }

            for (int spriteIndex = 0; spriteIndex < spriteScene.FaceCount; spriteIndex += 1)
            {
                int[] spriteVertices = spriteScene.FaceVertexIndices[spriteIndex];
                int firstVertexIndex = spriteVertices[0];
                int spriteProjU = spriteScene.ProjectedU[firstVertexIndex];
                int spriteProjV = spriteScene.ProjectedV[firstVertexIndex];
                int spriteDepth = spriteScene.ProjectedDepth[firstVertexIndex];

                if (spriteDepth <= NearPlane || spriteDepth >= SpriteFarClipDistance)
                {
                    continue;
                }

                int spriteScreenWidth = (sceneObjectTracker.SceneObjectWidths[spriteIndex] << screenProjectionShift) / spriteDepth;
                int spriteScreenHeight = (sceneObjectTracker.SceneObjectHeights[spriteIndex] << screenProjectionShift) / spriteDepth;

                if (spriteProjU - spriteScreenWidth / 2 <= screenCentreX &&
                    spriteProjU + spriteScreenWidth / 2 >= -screenCentreX &&
                    spriteProjV - spriteScreenHeight <= screenCentreY &&
                    spriteProjV >= -screenCentreY)
                {
                    CameraModel spriteCameraModel = visibleModels[currentModelIndex];
                    spriteCameraModel.SourceObject = spriteScene;
                    spriteCameraModel.FaceIndex = spriteIndex;
                    CameraModelBoundsCalculator.ComputeSpriteBounds(spriteCameraModel);
                    spriteCameraModel.Scale = (spriteDepth + spriteScene.ProjectedDepth[spriteVertices[1]]) / 2;
                    currentModelIndex += 1;
                }
            }
        }

        private void RenderSortedModels()
        {
            for (int sortedModelIndex = 0; sortedModelIndex < currentModelIndex; sortedModelIndex += 1)
            {
                CameraModel sortedModel = visibleModels[sortedModelIndex];
                GameObject model = sortedModel.SourceObject;
                int faceIndex = sortedModel.FaceIndex;

                if (model == sceneObjectTracker.HighlightedObject)
                {
                    RenderSpriteModel(model, faceIndex);
                }
                else
                {
                    RenderPolygonModel(sortedModel, model, faceIndex);
                }
            }

            sceneObjectTracker.FinaliseFrame();
        }

        private void RenderSpriteModel(GameObject model, int faceIndex)
        {
            int[] spriteFaceVerts = model.FaceVertexIndices[faceIndex];
            int spriteFirstVertex = spriteFaceVerts[0];
            int spriteProjU = model.ProjectedU[spriteFirstVertex];
            int spriteProjV = model.ProjectedV[spriteFirstVertex];
            int spriteDepth = model.ProjectedDepth[spriteFirstVertex];
            int spriteWidth = (sceneObjectTracker.SceneObjectWidths[faceIndex] << screenProjectionShift) / spriteDepth;
            int spriteHeight = (sceneObjectTracker.SceneObjectHeights[faceIndex] << screenProjectionShift) / spriteDepth;
            int xOffset = model.ProjectedU[spriteFaceVerts[1]] - spriteProjU;
            int screenX = spriteProjU - spriteWidth / 2;
            int screenY = scanlineBufferCentre + spriteProjV - spriteHeight;
            gameImage.DrawVisibleEntity(
                screenX + screenMouseOffsetX,
                screenY,
                spriteWidth,
                spriteHeight,
                sceneObjectTracker.SceneObjectIds[faceIndex],
                xOffset,
                (256 << screenProjectionShift) / spriteDepth);

            if (!sceneObjectTracker.IsHitCandidate)
            {
                return;
            }

            int hitScreenX = screenX + (sceneObjectTracker.SceneObjectFrames[faceIndex] << screenProjectionShift) / spriteDepth;

            if (sceneObjectTracker.MouseAdjustedY >= screenY &&
                sceneObjectTracker.MouseAdjustedY <= screenY + spriteHeight &&
                sceneObjectTracker.MouseAdjustedX >= hitScreenX &&
                sceneObjectTracker.MouseAdjustedX <= hitScreenX + spriteWidth &&
                !model.DoesShareEntityArrays &&
                model.PolygonTypeData[faceIndex] == 0)
            {
                sceneObjectTracker.RecordHit(model, faceIndex);
            }
        }

        private void RenderPolygonModel(CameraModel sortedModel, GameObject model, int faceIndex)
        {
            int clippedVertCount = 0;
            int shadeLevel = 0;
            int vertCount = model.FaceVertexCounts[faceIndex];
            int[] modelFaceVerts = model.FaceVertexIndices[faceIndex];

            if (model.GouraudShade[faceIndex] != NotSetSentinel)
            {
                if (sortedModel.VisibilityDot < 0)
                {
                    shadeLevel = model.BaseShadeLevel - model.GouraudShade[faceIndex];
                }
                else
                {
                    shadeLevel = model.BaseShadeLevel + model.GouraudShade[faceIndex];
                }
            }

            for (int faceVertLoopIndex = 0; faceVertLoopIndex < vertCount; faceVertLoopIndex += 1)
            {
                int vertIndex = modelFaceVerts[faceVertLoopIndex];
                polygonVertexScreenX[faceVertLoopIndex] = model.ProjectedX[vertIndex];
                polygonVertexScreenY[faceVertLoopIndex] = model.ProjectedY[vertIndex];
                polygonVertexDepth[faceVertLoopIndex] = model.ProjectedDepth[vertIndex];

                if (model.GouraudShade[faceIndex] == NotSetSentinel)
                {
                    if (sortedModel.VisibilityDot < 0)
                    {
                        shadeLevel = model.BaseShadeLevel - model.FaceNormalComponent[vertIndex] + model.VertexColour[vertIndex];
                    }
                    else
                    {
                        shadeLevel = model.BaseShadeLevel + model.FaceNormalComponent[vertIndex] + model.VertexColour[vertIndex];
                    }
                }

                if (model.ProjectedDepth[vertIndex] >= NearPlane)
                {
                    clippedPolygonScreenX[clippedVertCount] = model.ProjectedU[vertIndex];
                    clippedPolygonScreenY[clippedVertCount] = model.ProjectedV[vertIndex];
                    clippedPolygonShadeLevels[clippedVertCount] = shadeLevel;

                    if (model.ProjectedDepth[vertIndex] > FogStartDistance)
                    {
                        clippedPolygonShadeLevels[clippedVertCount] += (model.ProjectedDepth[vertIndex] - FogStartDistance) / FogGradientStep;
                    }

                    clippedVertCount += 1;
                }
                else
                {
                    clippedVertCount = ClipVertexAgainstNearPlane(model, modelFaceVerts, faceVertLoopIndex, vertCount, shadeLevel, clippedVertCount);
                }
            }

            ApplyShadeClamping(sortedModel, vertCount);

            polygonRasteriser.Rasterise(
                clippedVertCount,
                clippedPolygonScreenX,
                clippedPolygonScreenY,
                clippedPolygonShadeLevels,
                model,
                faceIndex);

            if (polygonRasteriser.MaxVisibleScanline > polygonRasteriser.MinVisibleScanline)
            {
                modelRenderer.Render(
                    vertCount,
                    polygonVertexScreenX,
                    polygonVertexScreenY,
                    polygonVertexDepth,
                    sortedModel.CurrentTextureIndex,
                    model);
            }
        }

        private int ClipVertexAgainstNearPlane(
            GameObject model,
            int[] modelFaceVerts,
            int faceVertLoopIndex,
            int vertCount,
            int shadeLevel,
            int clippedVertCount)
        {
            int vertIndex = modelFaceVerts[faceVertLoopIndex];
            int prevAdjacentVertIndex;

            if (faceVertLoopIndex == 0)
            {
                prevAdjacentVertIndex = modelFaceVerts[vertCount - 1];
            }
            else
            {
                prevAdjacentVertIndex = modelFaceVerts[faceVertLoopIndex - 1];
            }

            if (model.ProjectedDepth[prevAdjacentVertIndex] >= NearPlane)
            {
                int prevDepthDiff = model.ProjectedDepth[vertIndex] - model.ProjectedDepth[prevAdjacentVertIndex];
                int clippedX = model.ProjectedX[vertIndex] - (model.ProjectedX[vertIndex] - model.ProjectedX[prevAdjacentVertIndex]) * (model.ProjectedDepth[vertIndex] - NearPlane) / prevDepthDiff;
                int clippedY = model.ProjectedY[vertIndex] - (model.ProjectedY[vertIndex] - model.ProjectedY[prevAdjacentVertIndex]) * (model.ProjectedDepth[vertIndex] - NearPlane) / prevDepthDiff;
                clippedPolygonScreenX[clippedVertCount] = (clippedX << screenProjectionShift) / NearPlane;
                clippedPolygonScreenY[clippedVertCount] = (clippedY << screenProjectionShift) / NearPlane;
                clippedPolygonShadeLevels[clippedVertCount] = shadeLevel;
                clippedVertCount += 1;
            }

            int nextAdjacentVertIndex;

            if (faceVertLoopIndex == vertCount - 1)
            {
                nextAdjacentVertIndex = modelFaceVerts[0];
            }
            else
            {
                nextAdjacentVertIndex = modelFaceVerts[faceVertLoopIndex + 1];
            }

            if (model.ProjectedDepth[nextAdjacentVertIndex] >= NearPlane)
            {
                int nextDepthDiff = model.ProjectedDepth[vertIndex] - model.ProjectedDepth[nextAdjacentVertIndex];
                int nextClippedX = model.ProjectedX[vertIndex] - (model.ProjectedX[vertIndex] - model.ProjectedX[nextAdjacentVertIndex]) * (model.ProjectedDepth[vertIndex] - NearPlane) / nextDepthDiff;
                int nextClippedY = model.ProjectedY[vertIndex] - (model.ProjectedY[vertIndex] - model.ProjectedY[nextAdjacentVertIndex]) * (model.ProjectedDepth[vertIndex] - NearPlane) / nextDepthDiff;
                clippedPolygonScreenX[clippedVertCount] = (nextClippedX << screenProjectionShift) / NearPlane;
                clippedPolygonScreenY[clippedVertCount] = (nextClippedY << screenProjectionShift) / NearPlane;
                clippedPolygonShadeLevels[clippedVertCount] = shadeLevel;
                clippedVertCount += 1;
            }

            return clippedVertCount;
        }

        private void ApplyShadeClamping(CameraModel sortedModel, int vertCount)
        {
            for (int clampLoopIndex = 0; clampLoopIndex < vertCount; clampLoopIndex += 1)
            {
                if (clippedPolygonShadeLevels[clampLoopIndex] < 0)
                {
                    clippedPolygonShadeLevels[clampLoopIndex] = 0;
                }
                else if (clippedPolygonShadeLevels[clampLoopIndex] > 255)
                {
                    clippedPolygonShadeLevels[clampLoopIndex] = 255;
                }

                if (sortedModel.CurrentTextureIndex >= 0)
                {
                    if (textureManager.textureLastAccessFrame[sortedModel.CurrentTextureIndex] == 1)
                    {
                        clippedPolygonShadeLevels[clampLoopIndex] <<= 9;
                    }
                    else
                    {
                        clippedPolygonShadeLevels[clampLoopIndex] <<= 6;
                    }
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
            rotationX &= CameraTrigonometryTable.TrigTableMask;
            rotationY &= CameraTrigonometryTable.TrigTableMask;
            rotationZ &= CameraTrigonometryTable.TrigTableMask;
            cameraOffsetX = CameraTrigonometryTable.TrigTableHalfSize - rotationX & CameraTrigonometryTable.TrigTableMask;
            cameraOffsetY = CameraTrigonometryTable.TrigTableHalfSize - rotationY & CameraTrigonometryTable.TrigTableMask;
            cameraOffsetZ = CameraTrigonometryTable.TrigTableHalfSize - rotationZ & CameraTrigonometryTable.TrigTableMask;
            int xOffset = 0;
            int yOffset = 0;
            int zOffset = distance;

            if (rotationX != 0)
            {
                int rotXSin = CameraTrigonometryTable.Table[rotationX];
                int rotXCos = CameraTrigonometryTable.Table[rotationX + CameraTrigonometryTable.TrigTableHalfSize];
                int rotatedY = yOffset * rotXCos - zOffset * rotXSin >> 15;
                zOffset = yOffset * rotXSin + zOffset * rotXCos >> 15;
                yOffset = rotatedY;
            }

            if (rotationY != 0)
            {
                int rotYSin = CameraTrigonometryTable.Table[rotationY];
                int rotYCos = CameraTrigonometryTable.Table[rotationY + CameraTrigonometryTable.TrigTableHalfSize];
                int rotatedX = zOffset * rotYSin + xOffset * rotYCos >> 15;
                zOffset = zOffset * rotYCos - xOffset * rotYSin >> 15;
                xOffset = rotatedX;
            }

            if (rotationZ != 0)
            {
                int rotZSin = CameraTrigonometryTable.Table[rotationZ];
                int rotZCos = CameraTrigonometryTable.Table[rotationZ + CameraTrigonometryTable.TrigTableHalfSize];
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
                objectCache[modelIndex].OffsetModelColours(redOffset, greenOffset, blueOffset);
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
                objectCache[modelIndex].SetModelColours(fromColour, toColour, x, y, z);
            }
        }

        public static int GetTextureColour(int r, int g, int b)
            => CameraTextureManager.GetTextureColour(r, g, b);
    }
}
