using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class EngineHandleTileTests
    {
        private static int GridSize => 96;

        private static int SectorSize => 48;

        private static int TileWorldSize => 128;

        private EngineHandle engineHandle = null!;

        [SetUp]
        public void SetUp()
        {
            engineHandle = new EngineHandle(null!, null!, null!);
        }

        [Test]
        public void GivenANewEngineHandle_WhenReadingItsGridState_ThenArraysAndDefaultsAreInitialised()
        {
            Assert.That(engineHandle.RoofTiles, Has.Length.EqualTo(GridSize));
            Assert.That(engineHandle.Tiles, Has.Length.EqualTo(GridSize));
            Assert.That(engineHandle.Steps, Has.Length.EqualTo(GridSize));
            Assert.That(engineHandle.RoofTiles[0], Has.Length.EqualTo(GridSize));
            Assert.That(engineHandle.Tiles[95], Has.Length.EqualTo(GridSize));
            Assert.That(engineHandle.Steps[48], Has.Length.EqualTo(GridSize));
            Assert.That(engineHandle.TileGroundTexture, Has.Length.EqualTo(4));
            Assert.That(engineHandle.TileGroundTexture[0], Has.Length.EqualTo(2304));
            Assert.That(engineHandle.TileGroundElevation, Has.Length.EqualTo(4));
            Assert.That(engineHandle.TileChunks, Has.Length.EqualTo(64));
            Assert.That(engineHandle.SelectedX, Has.Length.EqualTo(18432));
            Assert.That(engineHandle.SelectedY, Has.Length.EqualTo(18432));
            Assert.That(engineHandle.GroundTexture, Has.Length.EqualTo(256));
            Assert.That(engineHandle.TileLightingX, Is.EqualTo(GridSize));
            Assert.That(engineHandle.TileLightingY, Is.EqualTo(GridSize));
            Assert.That(engineHandle.BaseInventoryPic, Is.EqualTo(750));
            Assert.That(engineHandle.DefaultLightingIntensity, Is.EqualTo(128));
            Assert.That(engineHandle.IsCameraInitialised);
            Assert.That(engineHandle.PlayerIsAlive, Is.False);
            Assert.That(engineHandle.ShowAllWalls, Is.False);
        }

        [TestCase(0, 0)]
        [TestCase(47, 47)]
        [TestCase(48, 0)]
        [TestCase(95, 47)]
        [TestCase(0, 48)]
        [TestCase(47, 95)]
        [TestCase(48, 48)]
        [TestCase(95, 95)]
        public void GivenACoordinateInAnySector_WhenReadingTileData_ThenTheMappedValuesAreReturned(
            int positionX,
            int positionY)
        {
            SetSectorValue(engineHandle.TileGroundTexture, positionX, positionY, 0x1234);
            SetSectorValue(engineHandle.TileGroundElevation, positionX, positionY, unchecked((sbyte)0xff));
            SetSectorValue(engineHandle.TileRoofType, positionX, positionY, 42);
            SetSectorValue(engineHandle.TileObjectRotation, positionX, positionY, 64);
            SetSectorValue(engineHandle.TileGroundOverlay, positionX, positionY, 128);
            SetSectorValue(engineHandle.TileHorizontalWall, positionX, positionY, 200);
            SetSectorValue(engineHandle.TileVerticalWall, positionX, positionY, 255);
            SetSectorValue(engineHandle.TileDiagonalWall, positionX, positionY, int.MaxValue);

            Assert.That(engineHandle.GetTileGroundTextureIndex(positionX, positionY), Is.EqualTo(0x34));
            Assert.That(engineHandle.GetTileElevation(positionX, positionY), Is.EqualTo(765));
            Assert.That(engineHandle.GetTileRoofType(positionX, positionY), Is.EqualTo(42));
            Assert.That(engineHandle.GetTileRotation(positionX, positionY), Is.EqualTo(64));
            Assert.That(engineHandle.GetTileGroundOverlayIndex(positionX, positionY, 0), Is.EqualTo(128));
            Assert.That(engineHandle.GetTileGroundOverlayIndex(positionX, positionY, 42), Is.EqualTo(128));
            Assert.That(engineHandle.GetHorizontalWall(positionX, positionY), Is.EqualTo(200));
            Assert.That(engineHandle.GetVerticalWall(positionX, positionY), Is.EqualTo(255));
            Assert.That(engineHandle.GetDiagonalWall(positionX, positionY), Is.EqualTo(int.MaxValue));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(96, 0)]
        [TestCase(0, 96)]
        [TestCase(int.MinValue, int.MaxValue)]
        public void GivenAnOutOfGridCoordinate_WhenReadingTileData_ThenZeroIsReturned(
            int positionX,
            int positionY)
        {
            Assert.That(engineHandle.GetTileGroundTextureIndex(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetTileElevation(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetTileRoofType(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetTileRotation(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetTileGroundOverlayIndex(positionX, positionY, 42), Is.Zero);
            Assert.That(engineHandle.GetHorizontalWall(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetVerticalWall(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetDiagonalWall(positionX, positionY), Is.Zero);
            Assert.That(engineHandle.GetTile(positionX, positionY), Is.Zero);
        }

        [TestCase(0, 0, 0)]
        [TestCase(47, 47, 42)]
        [TestCase(48, 0, 128)]
        [TestCase(95, 95, 255)]
        public void GivenAValidCoordinate_WhenSettingGroundOverlayHeight_ThenTheValueIsStored(
            int positionX,
            int positionY,
            int overlayHeight)
        {
            engineHandle.SetTileGroundOverlayHeight(positionX, positionY, overlayHeight);

            Assert.That(
                engineHandle.GetTileGroundOverlayIndex(positionX, positionY, 0),
                Is.EqualTo(overlayHeight & 0xff));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(96, 0)]
        [TestCase(0, 96)]
        public void GivenAnInvalidCoordinate_WhenSettingGroundOverlayHeight_ThenNoExceptionIsThrown(
            int positionX,
            int positionY)
            => Assert.That(
                () => engineHandle.SetTileGroundOverlayHeight(positionX, positionY, 42),
                Throws.Nothing);

        [Test]
        public void GivenNoGroundOverlay_WhenResolvingItsTexture_ThenTheProvidedDefaultIsReturned()
            => Assert.That(
                engineHandle.GetTileGroundOverlayTextureOrDefault(4, 8, 0, 42),
                Is.EqualTo(42));

        [Test]
        public void GivenNoGroundOverlay_WhenResolvingElevationMinimum_ThenNegativeOneIsReturned()
            => Assert.That(engineHandle.GetElevationMinimum(4, 8, 0), Is.EqualTo(-1));

        [TestCase(0, 0, 30)]
        [TestCase(64, 32, 60)]
        [TestCase(100, 100, 101)]
        [TestCase(127, 127, 120)]
        public void GivenFourTileElevations_WhenInterpolatingWithinTheTile_ThenTheExpectedElevationIsReturned(
            int fractionalX,
            int fractionalY,
            int expectedElevation)
        {
            SetSectorValue(engineHandle.TileGroundElevation, 10, 10, 10);
            SetSectorValue(engineHandle.TileGroundElevation, 11, 10, 20);
            SetSectorValue(engineHandle.TileGroundElevation, 10, 11, 30);
            SetSectorValue(engineHandle.TileGroundElevation, 11, 11, 40);
            int worldX = 10 * TileWorldSize + fractionalX;
            int worldY = 10 * TileWorldSize + fractionalY;

            int elevation = engineHandle.GetAveragedElevation(worldX, worldY);

            Assert.That(elevation, Is.EqualTo(expectedElevation));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(95 * 128, 0)]
        [TestCase(0, 95 * 128)]
        public void GivenAWorldCoordinateOutsideInterpolableTiles_WhenReadingElevation_ThenZeroIsReturned(
            int worldX,
            int worldY)
            => Assert.That(engineHandle.GetAveragedElevation(worldX, worldY), Is.Zero);

        [Test]
        public void GivenTileFlags_WhenAddingAndRemovingThem_ThenOnlyRequestedBitsChange()
        {
            engineHandle.SetTileFlags(4, 8, 0x02);
            engineHandle.SetTileFlags(4, 8, 0x08);

            Assert.That(engineHandle.GetTile(4, 8), Is.EqualTo(0x0a));

            engineHandle.DrawObjectSprite(4, 8, 0x02);

            Assert.That(engineHandle.GetTile(4, 8), Is.EqualTo(0x08));
        }

        [Test]
        public void GivenNoRoofTiles_WhenCheckingTheArea_ThenBothRoofPredicatesAreFalse()
        {
            Assert.That(engineHandle.HasRoofTiles(10, 10), Is.False);
            Assert.That(engineHandle.IsRoofTile(10, 10), Is.False);
        }

        [Test]
        public void GivenOneRoofTile_WhenCheckingTheArea_ThenOnlyAnyRoofPredicateIsTrue()
        {
            SetSectorValue(engineHandle.TileRoofType, 10, 10, 1);

            Assert.That(engineHandle.HasRoofTiles(10, 10));
            Assert.That(engineHandle.IsRoofTile(10, 10), Is.False);
        }

        [Test]
        public void GivenAllFourRoofTiles_WhenCheckingTheArea_ThenBothRoofPredicatesAreTrue()
        {
            SetSectorValue(engineHandle.TileRoofType, 10, 10, 1);
            SetSectorValue(engineHandle.TileRoofType, 9, 10, 1);
            SetSectorValue(engineHandle.TileRoofType, 9, 9, 1);
            SetSectorValue(engineHandle.TileRoofType, 10, 9, 1);

            Assert.That(engineHandle.HasRoofTiles(10, 10));
            Assert.That(engineHandle.IsRoofTile(10, 10));
        }

        [Test]
        public void GivenOverlaySeams_WhenStitchingColours_ThenBoundaryAndInteriorValuesAreResolved()
        {
            SetSectorValue(engineHandle.TileGroundOverlay, 4, 8, 250);
            SetSectorValue(engineHandle.TileGroundOverlay, 47, 8, 250);
            SetSectorValue(engineHandle.TileGroundOverlay, 8, 47, 250);
            SetSectorValue(engineHandle.TileGroundOverlay, 47, 47, 250);
            SetSectorValue(engineHandle.TileGroundOverlay, 48, 47, 2);
            SetSectorValue(engineHandle.TileGroundOverlay, 47, 48, 2);

            engineHandle.StitchAreaTileColours();

            Assert.That(engineHandle.GetTileGroundOverlayIndex(4, 8, 0), Is.EqualTo(2));
            Assert.That(engineHandle.GetTileGroundOverlayIndex(47, 8, 0), Is.EqualTo(9));
            Assert.That(engineHandle.GetTileGroundOverlayIndex(8, 47, 0), Is.EqualTo(9));
            Assert.That(engineHandle.GetTileGroundOverlayIndex(47, 47, 0), Is.EqualTo(2));
        }

        [Test]
        public void GivenANullTileChunk_WhenUpdatingItsTileColour_ThenNoExceptionIsThrown()
            => Assert.That(
                () => engineHandle.UpdateTileChunk(0, 0, 4, 8, 42),
                Throws.Nothing);

        [Test]
        public void GivenATileChunkWithAMatchingVertex_WhenUpdatingItsColour_ThenOnlyThatVertexChanges()
        {
            GameObject tileChunk = new(3, 0);
            tileChunk.AddVertex(4 * TileWorldSize, 0, 8 * TileWorldSize);
            tileChunk.AddVertex(16 * TileWorldSize, 0, 32 * TileWorldSize);
            tileChunk.AddVertex(4 * TileWorldSize, 42, 8 * TileWorldSize);
            engineHandle.TileChunks[0] = tileChunk;

            engineHandle.UpdateTileChunk(0, 0, 4, 8, 64);

            Assert.That(tileChunk.VertexColour, Is.EqualTo(new[] { 64, 0, 0 }));
        }

        [Test]
        public void GivenATileChunkWithoutAMatchingVertex_WhenUpdatingItsColour_ThenNoVertexChanges()
        {
            GameObject tileChunk = new(2, 0);
            tileChunk.AddVertex(4 * TileWorldSize, 0, 8 * TileWorldSize);
            tileChunk.AddVertex(16 * TileWorldSize, 0, 32 * TileWorldSize);
            engineHandle.TileChunks[0] = tileChunk;

            engineHandle.UpdateTileChunk(0, 0, 42, 64, 96);

            Assert.That(tileChunk.VertexColour, Has.All.Zero);
        }

        [Test]
        public void GivenATileOnTwoChunkBoundaries_WhenSettingItsData_ThenAllFourChunksAreUpdated()
        {
            int tileX = 12;
            int tileY = 12;
            int[] affectedChunkIndices = [0, 1, 8, 9];

            foreach (int chunkIndex in affectedChunkIndices)
            {
                GameObject tileChunk = new(1, 0);
                tileChunk.AddVertex(tileX * TileWorldSize, 0, tileY * TileWorldSize);
                engineHandle.TileChunks[chunkIndex] = tileChunk;
            }

            engineHandle.SetTileData(tileX, tileY, 42);

            foreach (int chunkIndex in affectedChunkIndices)
            {
                Assert.That(engineHandle.TileChunks[chunkIndex].VertexColour[0], Is.EqualTo(42));
            }
        }

        [Test]
        public void GivenATileInsideAChunk_WhenSettingItsData_ThenOnlyItsCurrentChunkIsUpdated()
        {
            int tileX = 13;
            int tileY = 13;
            GameObject currentChunk = new(1, 0);
            currentChunk.AddVertex(tileX * TileWorldSize, 0, tileY * TileWorldSize);
            GameObject adjacentChunk = new(1, 0);
            adjacentChunk.AddVertex(tileX * TileWorldSize, 0, tileY * TileWorldSize);
            engineHandle.TileChunks[9] = currentChunk;
            engineHandle.TileChunks[8] = adjacentChunk;

            engineHandle.SetTileData(tileX, tileY, 42);

            Assert.That(currentChunk.VertexColour[0], Is.EqualTo(42));
            Assert.That(adjacentChunk.VertexColour[0], Is.Zero);
        }

        [TestCase(0, 0, 1)]
        [TestCase(-1, 0, 1)]
        [TestCase(0, -1, 2)]
        [TestCase(-1, -1, 4)]
        public void GivenABlockingSceneNeighbour_WhenRefreshingATile_ThenTheOccupiedFlagIsApplied(
            int neighbourOffsetX,
            int neighbourOffsetY,
            int blockingFlag)
        {
            int tileX = 16;
            int tileY = 32;
            engineHandle.SetTileFlags(
                tileX + neighbourOffsetX,
                tileY + neighbourOffsetY,
                blockingFlag);

            engineHandle.AddObjectToScene(tileX, tileY, 0, 0);

            Assert.That(engineHandle.GetTile(tileX, tileY), Is.EqualTo(35));
        }

        [Test]
        public void GivenAnUnblockedSceneArea_WhenRefreshingIt_ThenNoTileFlagIsAdded()
        {
            engineHandle.AddObjectToScene(16, 32, 0, 0);

            Assert.That(engineHandle.GetTile(16, 32), Is.Zero);
        }

        [Test]
        public void GivenABlockingTileAndAnExtendedSceneArea_WhenRefreshingIt_ThenTheInclusiveAreaIsOccupied()
        {
            engineHandle.SetTileFlags(16, 32, 1);

            engineHandle.AddObjectToScene(16, 32, 1, 1);

            Assert.That(engineHandle.GetTile(16, 32), Is.EqualTo(35));
            Assert.That(engineHandle.GetTile(16, 33), Is.EqualTo(35));
            Assert.That(engineHandle.GetTile(17, 32), Is.EqualTo(35));
            Assert.That(engineHandle.GetTile(17, 33), Is.EqualTo(35));
        }

        [TestCase(0, 16, 0, 0)]
        [TestCase(16, 0, 0, 0)]
        [TestCase(95, 16, 1, 0)]
        [TestCase(16, 95, 0, 1)]
        public void GivenASceneAreaOutsideTheGrid_WhenRefreshingIt_ThenNoTileChanges(
            int tileX,
            int tileY,
            int objectWidth,
            int objectHeight)
        {
            engineHandle.AddObjectToScene(tileX, tileY, objectWidth, objectHeight);

            Assert.That(engineHandle.Tiles.SelectMany(row => row), Has.All.Zero);
        }

        [TestCase(0, new[]
        {
            0x7c0000, 0x7c0000, 0x7c0000,
            0x7c0000, 0x7c0000, 0x007c00,
            0x7c0000, 0x007c00, 0x007c00,
        })]
        [TestCase(1, new[]
        {
            0x007c00, 0x007c00, 0x007c00,
            0x7c0000, 0x007c00, 0x007c00,
            0x7c0000, 0x7c0000, 0x007c00,
        })]
        public void GivenTwoMinimapTextures_WhenDrawingAValidOrder_ThenTheTrianglePatternIsDrawn(
            int drawOrder,
            int[] expectedPixels)
        {
            GameImage minimapImage = new(3, 3, 0);
            Camera camera = new(minimapImage, 1, 1, 1);
            EngineHandle minimapEngine = new(camera, minimapImage, null!);
            int redTexture = Camera.GetTextureColour(255, 0, 0);
            int greenTexture = Camera.GetTextureColour(0, 255, 0);

            minimapEngine.DrawMinimapPixel(0, 0, drawOrder, redTexture, greenTexture);

            Assert.That(minimapImage.Pixels, Is.EqualTo(expectedPixels));
        }

        [TestCase(-1)]
        [TestCase(2)]
        [TestCase(42)]
        public void GivenTwoMinimapTextures_WhenDrawingAnUnsupportedOrder_ThenNoPixelChanges(
            int drawOrder)
        {
            GameImage minimapImage = new(3, 3, 0);
            Camera camera = new(minimapImage, 1, 1, 1);
            EngineHandle minimapEngine = new(camera, minimapImage, null!);
            int redTexture = Camera.GetTextureColour(255, 0, 0);
            int greenTexture = Camera.GetTextureColour(0, 255, 0);

            minimapEngine.DrawMinimapPixel(0, 0, drawOrder, redTexture, greenTexture);

            Assert.That(minimapImage.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenWorldObjects_WhenCleaningUpWithoutAnInitialisedCamera_ThenAllChunkReferencesAreCleared()
        {
            engineHandle.IsCameraInitialised = false;
            engineHandle.TileChunks[0] = new GameObject(0, 0);
            engineHandle.WallObject[0][0] = new GameObject(0, 0);
            engineHandle.RoofObject[0][0] = new GameObject(0, 0);

            engineHandle.CleanUpWorld();

            Assert.That(engineHandle.TileChunks, Has.All.Null);

            for (int sectorIndex = 0; sectorIndex < 4; sectorIndex += 1)
            {
                Assert.That(engineHandle.WallObject[sectorIndex], Has.All.Null);
                Assert.That(engineHandle.RoofObject[sectorIndex], Has.All.Null);
            }
        }

        private static void SetSectorValue(
            int[][] sectorValues,
            int positionX,
            int positionY,
            int value)
        {
            int sectorIndex = GetSectorIndex(positionX, positionY);
            int localIndex = GetLocalIndex(positionX, positionY);
            sectorValues[sectorIndex][localIndex] = value;
        }

        private static void SetSectorValue(
            sbyte[][] sectorValues,
            int positionX,
            int positionY,
            sbyte value)
        {
            int sectorIndex = GetSectorIndex(positionX, positionY);
            int localIndex = GetLocalIndex(positionX, positionY);
            sectorValues[sectorIndex][localIndex] = value;
        }

        private static int GetSectorIndex(int positionX, int positionY)
        {
            int sectorIndex = 0;

            if (positionX >= SectorSize)
            {
                sectorIndex += 1;
            }

            if (positionY >= SectorSize)
            {
                sectorIndex += 2;
            }

            return sectorIndex;
        }

        private static int GetLocalIndex(int positionX, int positionY)
            => positionX % SectorSize * SectorSize + positionY % SectorSize;
    }
}