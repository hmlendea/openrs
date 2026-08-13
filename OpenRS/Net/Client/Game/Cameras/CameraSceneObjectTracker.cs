namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class CameraSceneObjectTracker(int maxSceneObjects, int maxHighlightedObjects)
    {
        private static int VerticesPerSceneObject => 2;

        private readonly int[] sceneObjectId = new int[maxSceneObjects];
        private readonly int[] sceneObjectX = new int[maxSceneObjects];
        private readonly int[] sceneObjectY = new int[maxSceneObjects];
        private readonly int[] sceneObjectZ = new int[maxSceneObjects];
        private readonly int[] sceneObjectWidths = new int[maxSceneObjects];
        private readonly int[] sceneObjectHeights = new int[maxSceneObjects];
        private readonly int[] sceneObjectFrames = new int[maxSceneObjects];
        private readonly GameObject[] highlightedObjects = new GameObject[maxHighlightedObjects];
        private readonly int[] highlightedPlayerIds = new int[maxHighlightedObjects];

        private int sceneObjectCount;
        private bool isMousePositionUpdated;
        private int mouseAdjustedX;
        private int mouseAdjustedY;
        private int optionCount;

        public GameObject HighlightedObject { get; } = new GameObject(
            maxSceneObjects * VerticesPerSceneObject,
            maxSceneObjects);

        public int[] SceneObjectIds => sceneObjectId;
        public int[] SceneObjectWidths => sceneObjectWidths;
        public int[] SceneObjectHeights => sceneObjectHeights;
        public int[] SceneObjectFrames => sceneObjectFrames;

        public bool IsHitCandidate => isMousePositionUpdated && optionCount < maxHighlightedObjects;
        public int MouseAdjustedX => mouseAdjustedX;
        public int MouseAdjustedY => mouseAdjustedY;

        public int AddSpriteToScene(
            int objectId,
            int x,
            int y,
            int z,
            int width,
            int height,
            int entityType)
        {
            StoreSceneObject(objectId, x, y, z, width, height);
            AddSceneObjectGeometry(x, y, z, height, entityType);
            sceneObjectCount += 1;

            return sceneObjectCount - 1;
        }

        private void StoreSceneObject(
            int objectId,
            int positionX,
            int positionY,
            int positionZ,
            int width,
            int height)
        {
            sceneObjectId[sceneObjectCount] = objectId;
            sceneObjectX[sceneObjectCount] = positionX;
            sceneObjectY[sceneObjectCount] = positionY;
            sceneObjectZ[sceneObjectCount] = positionZ;
            sceneObjectWidths[sceneObjectCount] = width;
            sceneObjectHeights[sceneObjectCount] = height;
            sceneObjectFrames[sceneObjectCount] = 0;
        }

        private void AddSceneObjectGeometry(
            int positionX,
            int positionY,
            int positionZ,
            int height,
            int entityType)
        {
            int topVertexIndex = HighlightedObject.AddVertex(
                positionX,
                positionY,
                positionZ);
            int bottomVertexIndex = HighlightedObject.AddVertex(
                positionX,
                positionY - height,
                positionZ);
            int[] spriteVertexIndices = [topVertexIndex, bottomVertexIndex];
            HighlightedObject.AddFaceVertices(
                VerticesPerSceneObject,
                spriteVertexIndices,
                0,
                0);
            HighlightedObject.EntityType[sceneObjectCount] = entityType;
            HighlightedObject.PolygonTypeData[sceneObjectCount] = 0;
        }

        public void RemoveSprite(int spriteIndex)
            => HighlightedObject.PolygonTypeData[spriteIndex] = 1;

        public void UpdateSpritePosition(int spriteIndex, int frameIndex)
            => sceneObjectFrames[spriteIndex] = frameIndex;

        public void InitializeScene()
        {
            sceneObjectCount = 0;
            HighlightedObject.ResetObjectIndexes();
        }

        public void RemoveLastUpdates(int count)
        {
            sceneObjectCount -= count;
            HighlightedObject.AddPolygonToGroup(
                count,
                count * VerticesPerSceneObject);

            if (sceneObjectCount < 0)
            {
                sceneObjectCount = 0;
            }
        }

        public void SetMousePosition(int adjustedMouseX, int mouseY)
        {
            mouseAdjustedX = adjustedMouseX;
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

        public void RecordHit(GameObject gameObject, int faceIndex)
        {
            highlightedObjects[optionCount] = gameObject;
            highlightedPlayerIds[optionCount] = faceIndex;
            optionCount += 1;
        }

        public void FinaliseFrame()
            => isMousePositionUpdated = false;
    }
}
