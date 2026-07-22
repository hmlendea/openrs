using System;

using OpenRS.GameLogic.GameManagers;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Net.Client.Game
{
    public sealed class EngineHandle
    {
        internal readonly EntityManager entityManager;
        internal readonly int[][] objectDirs;

        private readonly SectorLoader sectorLoader;
        private readonly PathFinder pathFinder;
        private readonly SectionSceneBuilder sceneBuilder;
        private readonly RoofBuilder roofBuilder;
        private readonly WorldObjectManipulator worldObjectManipulator;

        public int TileLightingX = GridSize;
        public int TileLightingY = GridSize;
        public int[][] TileHorizontalWall;
        public int DefaultTileColour = EmptyTileColour;
        public int DefaultLightingIntensity = DefaultLightingIntensityInitial;
        public int[][] TileDiagonalWall;
        public int[][] TileGroundOverlay;
        public int[][] TileObjectRotation;
        public bool ShowAllWalls;
        public GameImage GameGraphics;
        public Camera WorldCamera;
        public int[] SelectedY;
        public int[][] TileGroundTexture;
        public int[] GroundTexture;
        public GameObject[] TileChunks;
        public GameObject CurrentSectionObject;
        public int[][] RoofTiles;
        public int[][] TileVerticalWall;
        public int[][] Steps;
        public sbyte[][] TileGroundElevation;
        public GameObject[][] RoofObject;
        public bool PlayerIsAlive;
        public int[][] Tiles;
        public GameObject[][] WallObject;
        public int[] SelectedX;
        public int[][] TileRoofType;
        public bool IsCameraInitialised;
        public int BaseInventoryPic;

        internal static int TilesPerSector => 2304;
        internal static int SectorSize => 48;
        internal static int GridSize => 96;
        internal static int ChunkCount => 64;
        internal static int TileWorldSize => 128;
        internal static int EmptyTileColour => 0xbc614e;
        internal static int DiagonalWallBackOffset => 12000;
        internal static int LocationEntityBase => 48000;
        internal static int RoofElevationFlag => 0x13880;
        internal static int TileEntityTypeBase => 0x30d40;
        internal static int RunLengthThreshold => 128;

        private static int SectorCount => 4;
        private static int SectionVertexCapacity => 18688;
        private static int SelectedArraySize => 18432;
        private static int GroundTextureArraySize => 256;
        private static int DefaultBaseInventoryPicIndex => 750;
        private static int DefaultLightingIntensityInitial => 128;
        private static int ChunkColumnCount => 8;
        private static int MinimapPixelSize => 3;
        private static int TileDimMask => 0x7f7f7f;
        private static int AllTileFlagsMask => 0xFFFF;
        private static int WallTileFlag => 40;
        private static int WallFaceVertexCount => 4;
        private static int WallEntityTypeBase => 30000;
        private static int TexturedWallRenderMode => 5;

        public EngineHandle(Camera worldCamera, GameImage graphicsHandler, EntityManager entityManager)
        {
            this.entityManager = entityManager;
            InitialiseSectorArrays();

            RoofTiles = new int[GridSize][];
            Tiles = new int[GridSize][];
            Steps = new int[GridSize][];
            objectDirs = new int[GridSize][];

            for (int gridIndex = 0; gridIndex < GridSize; gridIndex += 1)
            {
                RoofTiles[gridIndex] = new int[GridSize];
                Tiles[gridIndex] = new int[GridSize];
                Steps[gridIndex] = new int[GridSize];
                objectDirs[gridIndex] = new int[GridSize];
            }

            InitialiseScalarFields(worldCamera, graphicsHandler);
            GroundTexture = GroundTexturePalette.Create();

            sectorLoader = new SectorLoader(this);
            pathFinder = new PathFinder(this);
            sceneBuilder = new SectionSceneBuilder(this);
            roofBuilder = new RoofBuilder(this);
            worldObjectManipulator = new WorldObjectManipulator(this);
        }

        public void LoadSection(int x, int y, int height, bool freshLoad)
        {
            int sectionX = (x + 24) / SectorSize;
            int sectionY = (y + 24) / SectorSize;
            sectorLoader.LoadSector(sectionX - 1, sectionY - 1, height, 0);
            sectorLoader.LoadSector(sectionX, sectionY - 1, height, 1);
            sectorLoader.LoadSector(sectionX - 1, sectionY, height, 2);
            sectorLoader.LoadSector(sectionX, sectionY, height, 3);
            StitchAreaTileColours();
            CurrentSectionObject ??= new GameObject(SectionVertexCapacity, SectionVertexCapacity, true, true, false, false, true);
            sceneBuilder.BuildSection(height, freshLoad);
            roofBuilder.BuildRoof(height);
        }

        public void LoadSection(int x, int y, int height)
        {
            CleanUpWorld();

            int sectionX = (x + 24) / SectorSize;
            int sectionY = (y + 24) / SectorSize;
            LoadSection(x, y, height, true);

            if (height == 0)
            {
                LoadSection(x, y, 1, false);
                LoadSection(x, y, 2, false);
                sectorLoader.LoadSector(sectionX - 1, sectionY - 1, height, 0);
                sectorLoader.LoadSector(sectionX, sectionY - 1, height, 1);
                sectorLoader.LoadSector(sectionX - 1, sectionY, height, 2);
                sectorLoader.LoadSector(sectionX, sectionY, height, 3);
                StitchAreaTileColours();
            }
        }

        public void StitchAreaTileColours()
        {
            for (int tileX = 0; tileX < GridSize; tileX += 1)
            {
                for (int tileY = 0; tileY < GridSize; tileY += 1)
                {
                    if (GetTileGroundOverlayIndex(tileX, tileY, 0) != 250)
                    {
                        continue;
                    }

                    if (tileX == SectorSize - 1 &&
                        GetTileGroundOverlayIndex(tileX + 1, tileY, 0) != 250 &&
                        GetTileGroundOverlayIndex(tileX + 1, tileY, 0) != 2)
                    {
                        SetTileGroundOverlayHeight(tileX, tileY, 9);
                    }
                    else if (tileY == SectorSize - 1 &&
                             GetTileGroundOverlayIndex(tileX, tileY + 1, 0) != 250 &&
                             GetTileGroundOverlayIndex(tileX, tileY + 1, 0) != 2)
                    {
                        SetTileGroundOverlayHeight(tileX, tileY, 9);
                    }
                    else
                    {
                        SetTileGroundOverlayHeight(tileX, tileY, 2);
                    }
                }
            }
        }

        public void CleanUpWorld()
        {
            if (IsCameraInitialised)
            {
                WorldCamera.CleanUp();
            }

            for (int chunkIndex = 0; chunkIndex < ChunkCount; chunkIndex += 1)
            {
                TileChunks[chunkIndex] = null;

                for (int sectorIndex = 0; sectorIndex < SectorCount; sectorIndex += 1)
                {
                    WallObject[sectorIndex][chunkIndex] = null;
                    RoofObject[sectorIndex][chunkIndex] = null;
                }
            }

            GC.Collect();
        }

        public int GetAveragedElevation(int worldX, int worldZ)
        {
            int tileX = worldX >> 7;
            int tileZ = worldZ >> 7;
            int fractX = worldX & 0x7f;
            int fractZ = worldZ & 0x7f;

            if (tileX < 0 || tileZ < 0 || tileX >= GridSize - 1 || tileZ >= GridSize - 1)
            {
                return 0;
            }

            int baseElevation;
            int elevationDeltaX;
            int elevationDeltaZ;

            if (fractX <= 128 - fractZ)
            {
                baseElevation = GetTileElevation(tileX, tileZ);
                elevationDeltaX = GetTileElevation(tileX + 1, tileZ) - baseElevation;
                elevationDeltaZ = GetTileElevation(tileX, tileZ + 1) - baseElevation;
            }
            else
            {
                baseElevation = GetTileElevation(tileX + 1, tileZ + 1);
                elevationDeltaX = GetTileElevation(tileX, tileZ + 1) - baseElevation;
                elevationDeltaZ = GetTileElevation(tileX + 1, tileZ) - baseElevation;
                fractX = 128 - fractX;
                fractZ = 128 - fractZ;
            }

            return baseElevation + elevationDeltaX * fractX / 128 + elevationDeltaZ * fractZ / 128;
        }

        public void SetTileData(int x, int y, int colourValue)
        {
            int chunkX = x / 12;
            int chunkY = y / 12;
            int prevChunkX = (x - 1) / 12;
            int prevChunkY = (y - 1) / 12;
            UpdateTileChunk(chunkX, chunkY, x, y, colourValue);

            if (chunkX != prevChunkX)
            {
                UpdateTileChunk(prevChunkX, chunkY, x, y, colourValue);
            }

            if (chunkY != prevChunkY)
            {
                UpdateTileChunk(chunkX, prevChunkY, x, y, colourValue);
            }

            if (chunkX != prevChunkX && chunkY != prevChunkY)
            {
                UpdateTileChunk(prevChunkX, prevChunkY, x, y, colourValue);
            }
        }

        public int GetTileGroundTextureIndex(int x, int y)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(x, y);

            return TileGroundTexture[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
        }

        public int GetTileElevation(int tileX, int tileY)
        {
            if (tileX < 0 || tileX >= GridSize || tileY < 0 || tileY >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(tileX, tileY);

            return (TileGroundElevation[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff) * 3;
        }

        public int GetTileRoofType(int tileX, int tileY)
        {
            if (tileX < 0 || tileX >= GridSize || tileY < 0 || tileY >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(tileX, tileY);

            return TileRoofType[coords.Layer][coords.X * SectorSize + coords.Y];
        }

        public int GetTileRotation(int tileX, int tileY)
        {
            if (tileX < 0 || tileX >= GridSize || tileY < 0 || tileY >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(tileX, tileY);

            return TileObjectRotation[coords.Layer][coords.X * SectorSize + coords.Y];
        }

        public int GetTileGroundOverlayIndex(int x, int y, int height)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(x, y);

            return TileGroundOverlay[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
        }

        public int GetTileGroundOverlayTextureOrDefault(int x, int y, int height, int defaultTexture)
        {
            int overlayIndex = GetTileGroundOverlayIndex(x, y, height);

            if (overlayIndex == 0)
            {
                return defaultTexture;
            }

            return entityManager.GetTile(overlayIndex - 1).Colour;
        }

        public int GetElevationMinimum(int x, int y, int height)
        {
            int overlayIndex = GetTileGroundOverlayIndex(x, y, height);

            if (overlayIndex == 0)
            {
                return -1;
            }

            if (entityManager.GetTile(overlayIndex - 1).Type != 2)
            {
                return 0;
            }

            return 1;
        }

        public int GetHorizontalWall(int x, int y)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(x, y);

            return TileHorizontalWall[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
        }

        public int GetVerticalWall(int x, int y)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(x, y);

            return TileVerticalWall[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
        }

        public int GetDiagonalWall(int x, int y)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            {
                return 0;
            }

            SectorCoordinates coords = SectorCoordinates.From(x, y);

            return TileDiagonalWall[coords.Layer][coords.X * SectorSize + coords.Y];
        }

        public void SetTileGroundOverlayHeight(int x, int y, int height)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            {
                return;
            }

            SectorCoordinates coords = SectorCoordinates.From(x, y);
            TileGroundOverlay[coords.Layer][coords.X * SectorSize + coords.Y] = height;
        }

        public int GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= GridSize || y >= GridSize)
            {
                return 0;
            }

            return Tiles[x][y];
        }

        public void SetTileFlags(int x, int y, int flags) => Tiles[x][y] |= flags;

        public void DrawObjectSprite(int tileX, int tileY, int flagMask)
            => Tiles[tileX][tileY] &= AllTileFlagsMask - flagMask;

        public bool HasRoofTiles(int x, int y) =>
            GetTileRoofType(x, y) > 0 ||
            GetTileRoofType(x - 1, y) > 0 ||
            GetTileRoofType(x - 1, y - 1) > 0 ||
            GetTileRoofType(x, y - 1) > 0;

        public bool IsRoofTile(int x, int y) =>
            GetTileRoofType(x, y) > 0 &&
            GetTileRoofType(x - 1, y) > 0 &&
            GetTileRoofType(x - 1, y - 1) > 0 &&
            GetTileRoofType(x, y - 1) > 0;

        public void SetRoofTile(int wallObjectIndex, int srcX, int srcY, int destX, int destY)
        {
            int wallHeight = entityManager.GetWallObject(wallObjectIndex).ModelHeight;

            if (RoofTiles[srcX][srcY] < RoofElevationFlag)
            {
                RoofTiles[srcX][srcY] += RoofElevationFlag + wallHeight;
            }

            if (RoofTiles[destX][destY] < RoofElevationFlag)
            {
                RoofTiles[destX][destY] += RoofElevationFlag + wallHeight;
            }
        }

        public void DrawMinimapPixel(int x, int y, int drawOrder, int textureIndex1, int textureIndex2)
        {
            int destX = x * MinimapPixelSize;
            int destY = y * MinimapPixelSize;
            int smoothedTexture1 = WorldCamera.ApplyTextureSmoothing(textureIndex1) >> 1 & TileDimMask;
            int smoothedTexture2 = WorldCamera.ApplyTextureSmoothing(textureIndex2) >> 1 & TileDimMask;

            if (drawOrder == 0)
            {
                GameGraphics.DrawLineX(destX, destY, MinimapPixelSize, smoothedTexture1);
                GameGraphics.DrawLineX(destX, destY + 1, MinimapPixelSize - 1, smoothedTexture1);
                GameGraphics.DrawLineX(destX, destY + 2, MinimapPixelSize - 2, smoothedTexture1);
                GameGraphics.DrawLineX(destX + 2, destY + 1, MinimapPixelSize - 2, smoothedTexture2);
                GameGraphics.DrawLineX(destX + 1, destY + 2, MinimapPixelSize - 1, smoothedTexture2);

                return;
            }

            if (drawOrder == 1)
            {
                GameGraphics.DrawLineX(destX, destY, MinimapPixelSize, smoothedTexture2);
                GameGraphics.DrawLineX(destX + 1, destY + 1, MinimapPixelSize - 1, smoothedTexture2);
                GameGraphics.DrawLineX(destX + 2, destY + 2, MinimapPixelSize - 2, smoothedTexture2);
                GameGraphics.DrawLineX(destX, destY + 1, MinimapPixelSize - 2, smoothedTexture1);
                GameGraphics.DrawLineX(destX, destY + 2, MinimapPixelSize - 1, smoothedTexture1);
            }
        }

        public void UpdateTileChunk(int chunkX, int chunkY, int tileX, int tileY, int colourValue)
        {
            GameObject tileChunk = TileChunks[chunkX + chunkY * ChunkColumnCount];

            if (tileChunk is null)
            {
                return;
            }

            for (int vertIndex = 0; vertIndex < tileChunk.VertexCount; vertIndex += 1)
            {
                if (tileChunk.VertexCoordinatesX[vertIndex] == tileX * TileWorldSize &&
                    tileChunk.VertexCoordinatesZ[vertIndex] == tileY * TileWorldSize)
                {
                    tileChunk.SetVertexColour(vertIndex, colourValue);

                    return;
                }
            }
        }

        public void MakeWall(
            GameObject wallContainer,
            int wallTypeIndex,
            int startX,
            int startY,
            int endX,
            int endY)
        {
            SetTileFlags(startX, startY, WallTileFlag);
            SetTileFlags(endX, endY, WallTileFlag);

            int wallHeight = entityManager.GetWallObject(wallTypeIndex).ModelHeight;
            int backColour = entityManager.GetWallObject(wallTypeIndex).ModelFaceBack;
            int frontColour = entityManager.GetWallObject(wallTypeIndex).ModelFaceFront;
            int faceIndex = BuildWallFace(
                wallContainer,
                wallTypeIndex,
                startX,
                startY,
                endX,
                endY,
                wallHeight,
                backColour,
                frontColour);

            if (entityManager.GetWallObject(wallTypeIndex).FaceRenderMode == TexturedWallRenderMode)
            {
                wallContainer.EntityType[faceIndex] = WallEntityTypeBase + wallTypeIndex;
            }
            else
            {
                wallContainer.EntityType[faceIndex] = 0;
            }
        }

        public int GeneratePath(
            int curX,
            int curY,
            int bottomDestX,
            int bottomDestY,
            int upperDestX,
            int upperDestY,
            int[] pathX,
            int[] pathY,
            bool checkForObjects)
            => pathFinder.GeneratePath(
                curX,
                curY,
                bottomDestX,
                bottomDestY,
                upperDestX,
                upperDestY,
                pathX,
                pathY,
                checkForObjects);

        public void AddObjectToScene(int x, int y, int objWidth, int objHeight)
            => worldObjectManipulator.AddObjectToScene(x, y, objWidth, objHeight);

        public void RemoveWallObject(int x, int y, int wallDirection, int index)
            => worldObjectManipulator.RemoveWallObject(x, y, wallDirection, index);

        public void CreateWall(int x, int y, int wallDirection, int index)
            => worldObjectManipulator.CreateWall(x, y, wallDirection, index);

        public void RegisterObjectDir(int x, int y, int dir)
            => worldObjectManipulator.RegisterObjectDir(x, y, dir);

        public void RemoveObject(int x, int y, int objType, int objDir)
            => worldObjectManipulator.RemoveObject(x, y, objType, objDir);

        public void CreateObject(int x, int y, int index, int direction)
            => worldObjectManipulator.CreateObject(x, y, index, direction);

        public void AddObjects(GameObject[] tileModels)
            => worldObjectManipulator.AddObjects(tileModels);

        private void InitialiseSectorArrays()
        {
            TileHorizontalWall = new int[SectorCount][];
            TileDiagonalWall = new int[SectorCount][];
            TileGroundOverlay = new int[SectorCount][];
            TileObjectRotation = new int[SectorCount][];
            TileGroundTexture = new int[SectorCount][];
            TileVerticalWall = new int[SectorCount][];
            TileGroundElevation = new sbyte[SectorCount][];
            TileRoofType = new int[SectorCount][];
            WallObject = new GameObject[SectorCount][];
            RoofObject = new GameObject[SectorCount][];

            for (int sectorIndex = 0; sectorIndex < SectorCount; sectorIndex += 1)
            {
                TileHorizontalWall[sectorIndex] = new int[TilesPerSector];
                TileDiagonalWall[sectorIndex] = new int[TilesPerSector];
                TileGroundOverlay[sectorIndex] = new int[TilesPerSector];
                TileObjectRotation[sectorIndex] = new int[TilesPerSector];
                TileGroundTexture[sectorIndex] = new int[TilesPerSector];
                TileVerticalWall[sectorIndex] = new int[TilesPerSector];
                TileGroundElevation[sectorIndex] = new sbyte[TilesPerSector];
                TileRoofType[sectorIndex] = new int[TilesPerSector];
                WallObject[sectorIndex] = new GameObject[ChunkCount];
                RoofObject[sectorIndex] = new GameObject[ChunkCount];
            }
        }

        private void InitialiseScalarFields(Camera worldCamera, GameImage graphicsHandler)
        {
            ShowAllWalls = false;
            SelectedY = new int[SelectedArraySize];
            GroundTexture = new int[GroundTextureArraySize];
            TileChunks = new GameObject[ChunkCount];
            PlayerIsAlive = false;
            SelectedX = new int[SelectedArraySize];
            IsCameraInitialised = true;
            BaseInventoryPic = DefaultBaseInventoryPicIndex;
            WorldCamera = worldCamera;
            GameGraphics = graphicsHandler;
        }


        private int BuildWallFace(
            GameObject wallContainer,
            int wallTypeIndex,
            int startX,
            int startY,
            int endX,
            int endY,
            int wallHeight,
            int backColour,
            int frontColour)
        {
            int startWorldX = startX * TileWorldSize;
            int startWorldZ = startY * TileWorldSize;
            int endWorldX = endX * TileWorldSize;
            int endWorldZ = endY * TileWorldSize;
            int vertBottomStart = wallContainer.GetVertexIndex(
                startWorldX,
                -RoofTiles[startX][startY],
                startWorldZ);
            int vertTopStart = wallContainer.GetVertexIndex(
                startWorldX,
                -RoofTiles[startX][startY] - wallHeight,
                startWorldZ);
            int vertTopEnd = wallContainer.GetVertexIndex(
                endWorldX,
                -RoofTiles[endX][endY] - wallHeight,
                endWorldZ);
            int vertBottomEnd = wallContainer.GetVertexIndex(
                endWorldX,
                -RoofTiles[endX][endY],
                endWorldZ);
            int[] faceVerts = [vertBottomStart, vertTopStart, vertTopEnd, vertBottomEnd];

            return wallContainer.AddFaceVertices(WallFaceVertexCount, faceVerts, backColour, frontColour);
        }
    }
}
