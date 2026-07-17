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

    public int tileLightingX = 96;
    public int tileLightingY = 96;
    public int[][] tileHorizontalWall;
    public int defaultTileColour = EmptyTileColour;
    public int defaultLightingIntensity = 128;
    public int[][] tileDiagonalWall;
    public int[][] tileGroundOverlay;
    public int[][] tileObjectRotation;
    public bool showAllWalls;
    public GameImage gameGraphics;
    public Camera camera;
    public int[] selectedY;
    public int[][] tileGroundTexture;
    public int[] groundTexture;
    public GameObject[] TileChunks;
    public GameObject currentSectionObject;
    public int[][] roofTiles;
    public int[][] tileVerticalWall;
    public int[][] steps;
    public sbyte[][] tileGroundElevation;
    public GameObject[][] roofObject;
    public bool playerIsAlive;
    public int[][] tiles;
    public GameObject[][] wallObject;
    public int[] selectedX;
    public int[][] tileRoofType;
    public bool isCameraInitialised;
    public int baseInventoryPic;

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

    public EngineHandle(Camera worldCamera, GameImage graphicsHandler, EntityManager entityManager)
    {
        this.entityManager = entityManager;

        tileHorizontalWall = new int[SectorCount][];
        tileDiagonalWall = new int[SectorCount][];
        tileGroundOverlay = new int[SectorCount][];
        tileObjectRotation = new int[SectorCount][];
        tileGroundTexture = new int[SectorCount][];
        tileVerticalWall = new int[SectorCount][];
        tileGroundElevation = new sbyte[SectorCount][];
        tileRoofType = new int[SectorCount][];
        wallObject = new GameObject[SectorCount][];
        roofObject = new GameObject[SectorCount][];

        for (int sectorIndex = 0; sectorIndex < SectorCount; sectorIndex += 1)
        {
            tileHorizontalWall[sectorIndex] = new int[TilesPerSector];
            tileDiagonalWall[sectorIndex] = new int[TilesPerSector];
            tileGroundOverlay[sectorIndex] = new int[TilesPerSector];
            tileObjectRotation[sectorIndex] = new int[TilesPerSector];
            tileGroundTexture[sectorIndex] = new int[TilesPerSector];
            tileVerticalWall[sectorIndex] = new int[TilesPerSector];
            tileGroundElevation[sectorIndex] = new sbyte[TilesPerSector];
            tileRoofType[sectorIndex] = new int[TilesPerSector];
            wallObject[sectorIndex] = new GameObject[ChunkCount];
            roofObject[sectorIndex] = new GameObject[ChunkCount];
        }

        roofTiles = new int[GridSize][];
        tiles = new int[GridSize][];
        steps = new int[GridSize][];
        objectDirs = new int[GridSize][];

        for (int gridIndex = 0; gridIndex < GridSize; gridIndex += 1)
        {
            roofTiles[gridIndex] = new int[GridSize];
            tiles[gridIndex] = new int[GridSize];
            steps[gridIndex] = new int[GridSize];
            objectDirs[gridIndex] = new int[GridSize];
        }

        showAllWalls = false;
        selectedY = new int[18432];
        groundTexture = new int[256];
        TileChunks = new GameObject[ChunkCount];
        playerIsAlive = false;
        selectedX = new int[18432];
        isCameraInitialised = true;
        baseInventoryPic = 750;
        camera = worldCamera;
        gameGraphics = graphicsHandler;

        for (int textureIndex = 0; textureIndex < ChunkCount; textureIndex += 1)
        {
            groundTexture[textureIndex] = Camera.GetTextureColour(
                255 - textureIndex * 4,
                255 - (int)(textureIndex * 1.75D),
                255 - textureIndex * 4);
        }

        for (int textureIndex = 0; textureIndex < ChunkCount; textureIndex += 1)
        {
            groundTexture[textureIndex + ChunkCount] = Camera.GetTextureColour(textureIndex * 3, 144, 0);
        }

        for (int textureIndex = 0; textureIndex < ChunkCount; textureIndex += 1)
        {
            groundTexture[textureIndex + ChunkCount * 2] = Camera.GetTextureColour(
                192 - (int)(textureIndex * 1.5D),
                144 - (int)(textureIndex * 1.5D),
                0);
        }

        for (int textureIndex = 0; textureIndex < ChunkCount; textureIndex += 1)
        {
            groundTexture[textureIndex + ChunkCount * 3] = Camera.GetTextureColour(
                96 - (int)(textureIndex * 1.5D),
                48 + (int)(textureIndex * 1.5D),
                0);
        }

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
        StitchAreaTileColors();
        currentSectionObject ??= new GameObject(SectionVertexCapacity, SectionVertexCapacity, true, true, false, false, true);
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
            StitchAreaTileColors();
        }
    }

    public void StitchAreaTileColors()
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
        if (isCameraInitialised)
        {
            camera.CleanUp();
        }

        for (int chunkIndex = 0; chunkIndex < ChunkCount; chunkIndex += 1)
        {
            TileChunks[chunkIndex] = null;

            for (int sectorIndex = 0; sectorIndex < SectorCount; sectorIndex += 1)
            {
                wallObject[sectorIndex][chunkIndex] = null;
                roofObject[sectorIndex][chunkIndex] = null;
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

        return tileGroundTexture[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
    }

    public int GetTileElevation(int tileX, int tileY)
    {
        if (tileX < 0 || tileX >= GridSize || tileY < 0 || tileY >= GridSize)
        {
            return 0;
        }

        SectorCoordinates coords = SectorCoordinates.From(tileX, tileY);

        return (tileGroundElevation[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff) * 3;
    }

    public int GetTileRoofType(int tileX, int tileY)
    {
        if (tileX < 0 || tileX >= GridSize || tileY < 0 || tileY >= GridSize)
        {
            return 0;
        }

        SectorCoordinates coords = SectorCoordinates.From(tileX, tileY);

        return tileRoofType[coords.Layer][coords.X * SectorSize + coords.Y];
    }

    public int GetTileRotation(int tileX, int tileY)
    {
        if (tileX < 0 || tileX >= GridSize || tileY < 0 || tileY >= GridSize)
        {
            return 0;
        }

        SectorCoordinates coords = SectorCoordinates.From(tileX, tileY);

        return tileObjectRotation[coords.Layer][coords.X * SectorSize + coords.Y];
    }

    public int GetTileGroundOverlayIndex(int x, int y, int height)
    {
        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
        {
            return 0;
        }

        SectorCoordinates coords = SectorCoordinates.From(x, y);

        return tileGroundOverlay[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
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

        return tileHorizontalWall[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
    }

    public int GetVerticalWall(int x, int y)
    {
        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
        {
            return 0;
        }

        SectorCoordinates coords = SectorCoordinates.From(x, y);

        return tileVerticalWall[coords.Layer][coords.X * SectorSize + coords.Y] & 0xff;
    }

    public int GetDiagonalWall(int x, int y)
    {
        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
        {
            return 0;
        }

        SectorCoordinates coords = SectorCoordinates.From(x, y);

        return tileDiagonalWall[coords.Layer][coords.X * SectorSize + coords.Y];
    }

    public void SetTileGroundOverlayHeight(int x, int y, int height)
    {
        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
        {
            return;
        }

        SectorCoordinates coords = SectorCoordinates.From(x, y);
        tileGroundOverlay[coords.Layer][coords.X * SectorSize + coords.Y] = height;
    }

    public int GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= GridSize || y >= GridSize)
        {
            return 0;
        }

        return tiles[x][y];
    }

    public void SetTileFlags(int x, int y, int flags) => tiles[x][y] |= flags;

    public void DrawObjectSprite(int tileX, int tileY, int flagMask) => tiles[tileX][tileY] &= 65535 - flagMask;

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

        if (roofTiles[srcX][srcY] < RoofElevationFlag)
        {
            roofTiles[srcX][srcY] += RoofElevationFlag + wallHeight;
        }

        if (roofTiles[destX][destY] < RoofElevationFlag)
        {
            roofTiles[destX][destY] += RoofElevationFlag + wallHeight;
        }
    }

    public void DrawMinimapPixel(int x, int y, int drawOrder, int textureIndex1, int textureIndex2)
    {
        int destX = x * 3;
        int destY = y * 3;
        int smoothedTexture1 = camera.ApplyTextureSmoothing(textureIndex1);
        int smoothedTexture2 = camera.ApplyTextureSmoothing(textureIndex2);
        smoothedTexture1 = smoothedTexture1 >> 1 & 0x7f7f7f;
        smoothedTexture2 = smoothedTexture2 >> 1 & 0x7f7f7f;

        if (drawOrder == 0)
        {
            gameGraphics.DrawLineX(destX, destY, 3, smoothedTexture1);
            gameGraphics.DrawLineX(destX, destY + 1, 2, smoothedTexture1);
            gameGraphics.DrawLineX(destX, destY + 2, 1, smoothedTexture1);
            gameGraphics.DrawLineX(destX + 2, destY + 1, 1, smoothedTexture2);
            gameGraphics.DrawLineX(destX + 1, destY + 2, 2, smoothedTexture2);

            return;
        }

        if (drawOrder == 1)
        {
            gameGraphics.DrawLineX(destX, destY, 3, smoothedTexture2);
            gameGraphics.DrawLineX(destX + 1, destY + 1, 2, smoothedTexture2);
            gameGraphics.DrawLineX(destX + 2, destY + 2, 1, smoothedTexture2);
            gameGraphics.DrawLineX(destX, destY + 1, 1, smoothedTexture1);
            gameGraphics.DrawLineX(destX, destY + 2, 2, smoothedTexture1);
        }
    }

    public void UpdateTileChunk(int chunkX, int chunkY, int tileX, int tileY, int colourValue)
    {
        GameObject tileChunk = TileChunks[chunkX + chunkY * 8];

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
        SetTileFlags(startX, startY, 40);
        SetTileFlags(endX, endY, 40);

        int wallHeight = entityManager.GetWallObject(wallTypeIndex).ModelHeight;
        int backColour = entityManager.GetWallObject(wallTypeIndex).ModelFaceBack;
        int frontColour = entityManager.GetWallObject(wallTypeIndex).ModelFaceFront;
        int startWorldX = startX * TileWorldSize;
        int startWorldZ = startY * TileWorldSize;
        int endWorldX = endX * TileWorldSize;
        int endWorldZ = endY * TileWorldSize;
        int vertBottomStart = wallContainer.GetVertexIndex(startWorldX, -roofTiles[startX][startY], startWorldZ);
        int vertTopStart = wallContainer.GetVertexIndex(startWorldX, -roofTiles[startX][startY] - wallHeight, startWorldZ);
        int vertTopEnd = wallContainer.GetVertexIndex(endWorldX, -roofTiles[endX][endY] - wallHeight, endWorldZ);
        int vertBottomEnd = wallContainer.GetVertexIndex(endWorldX, -roofTiles[endX][endY], endWorldZ);
        int[] faceVerts = [vertBottomStart, vertTopStart, vertTopEnd, vertBottomEnd];
        int faceIndex = wallContainer.AddFaceVertices(4, faceVerts, backColour, frontColour);

        if (entityManager.GetWallObject(wallTypeIndex).FaceRenderMode == 5)
        {
            wallContainer.EntityType[faceIndex] = 30000 + wallTypeIndex;
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
}

}
