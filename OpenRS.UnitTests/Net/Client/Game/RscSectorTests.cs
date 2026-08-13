using System;
using System.IO;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class RscSectorTests
    {
        [Test]
        public void GivenTheSectorContract_WhenReadingItsDimensions_ThenItRemainsFortyEightByFortyEight()
        {
            Assert.That(RscSector.Width, Is.EqualTo(48));
            Assert.That(RscSector.Height, Is.EqualTo(48));
            Assert.That(SectorTestDataBuilder.SectorTileCount, Is.EqualTo(2304));
        }

        [Test]
        public void GivenANewSector_WhenReadingItsTiles_ThenEachTileHasAnIndependentOwnedInstance()
        {
            RscSector sector = new();
            SectorTile firstTile = sector.GetTile(0);
            SectorTile secondTile = sector.GetTile(1);

            Assert.That(firstTile, Is.Not.SameAs(secondTile));
            Assert.That(firstTile.Sector, Is.SameAs(sector));
            Assert.That(secondTile.Sector, Is.SameAs(sector));
        }

        [TestCase(0)]
        [TestCase(512)]
        [TestCase(2303)]
        public void GivenAValidIndex_WhenReplacingATile_ThenTheTileAndItsOwnerAreUpdated(int tileIndex)
        {
            RscSector sector = new();
            SectorTile expectedTile = new();

            sector.SetTile(tileIndex, expectedTile);

            Assert.That(sector.GetTile(tileIndex), Is.SameAs(expectedTile));
            Assert.That(expectedTile.Sector, Is.SameAs(sector));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 47, 47)]
        [TestCase(1, 0, 48)]
        [TestCase(4, 8, 200)]
        [TestCase(47, 47, 2303)]
        public void GivenAValidCoordinate_WhenReplacingATile_ThenTheCompatibleLinearIndexIsUsed(
            int positionX,
            int positionY,
            int expectedTileIndex)
        {
            RscSector sector = new();
            SectorTile expectedTile = new();

            sector.SetTile(positionX, positionY, expectedTile);

            Assert.That(sector.GetTile(expectedTileIndex), Is.SameAs(expectedTile));
            Assert.That(sector.GetTile(positionX, positionY), Is.SameAs(expectedTile));
        }

        [Test]
        public void GivenAYCoordinateEqualToTheSectorHeight_WhenReadingATile_ThenTheNextLinearRowIsUsed()
        {
            RscSector sector = new();

            Assert.That(sector.GetTile(0, 48), Is.SameAs(sector.GetTile(1, 0)));
        }

        [Test]
        public void GivenANullTile_WhenReplacingIt_ThenAnArgumentNullExceptionIsThrown()
        {
            RscSector sector = new();

            Assert.That(
                () => sector.SetTile(0, null!),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("tile"));
        }

        [TestCase(-1)]
        [TestCase(2304)]
        public void GivenAnInvalidIndex_WhenReadingATile_ThenAnIndexExceptionIsThrown(int tileIndex)
        {
            RscSector sector = new();

            Assert.That(
                () => sector.GetTile(tileIndex),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(-1)]
        [TestCase(2304)]
        public void GivenAnInvalidIndex_WhenReplacingATile_ThenAnIndexExceptionIsThrown(int tileIndex)
        {
            RscSector sector = new();

            Assert.That(
                () => sector.SetTile(tileIndex, new SectorTile()),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(48, 0)]
        public void GivenAnInvalidCoordinate_WhenReadingATile_ThenAnIndexExceptionIsThrown(
            int positionX,
            int positionY)
        {
            RscSector sector = new();

            Assert.That(
                () => sector.GetTile(positionX, positionY),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenACompleteSectorBuffer_WhenUnpackingIt_ThenEveryTileIsDecodedAndOwned()
        {
            using MemoryStream stream = new(SectorTestDataBuilder.BuildSectorData());

            RscSector sector = RscSector.Unpack(stream);

            AssertTile(sector, 0);
            AssertTile(sector, 1);
            AssertTile(sector, 255);
            AssertTile(sector, 256);
            AssertTile(sector, 2303);
            Assert.That(stream.Position, Is.EqualTo(SectorTestDataBuilder.SectorByteCount));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(23039)]
        public void GivenATruncatedSectorBuffer_WhenUnpackingIt_ThenAnIOExceptionIsThrown(int byteCount)
        {
            using MemoryStream stream = new(new byte[byteCount]);

            Assert.That(
                () => RscSector.Unpack(stream),
                Throws.TypeOf<IOException>());
        }

        [Test]
        public void GivenANullStream_WhenUnpackingASector_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => RscSector.Unpack(null!),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("inputStream"));

        [Test]
        public void GivenADisposedStream_WhenUnpackingASector_ThenAnObjectDisposedExceptionIsThrown()
        {
            MemoryStream stream = new(SectorTestDataBuilder.BuildSectorData());
            stream.Dispose();

            Assert.That(
                () => RscSector.Unpack(stream),
                Throws.TypeOf<ObjectDisposedException>());
        }

        private static void AssertTile(RscSector sector, int tileIndex)
        {
            SectorTile tile = sector.GetTile(tileIndex);

            Assert.That(tile.GroundElevation, Is.EqualTo((byte)tileIndex));
            Assert.That(tile.GroundTexture, Is.EqualTo((byte)(tileIndex + 1)));
            Assert.That(tile.GroundOverlay, Is.EqualTo((byte)(tileIndex + 2)));
            Assert.That(tile.RoofTexture, Is.EqualTo((byte)(tileIndex + 3)));
            Assert.That(tile.HorizontalWall, Is.EqualTo((byte)(tileIndex + 4)));
            Assert.That(tile.VerticalWall, Is.EqualTo((byte)(tileIndex + 5)));
            Assert.That(tile.DiagonalWalls, Is.EqualTo(tileIndex));
            Assert.That(tile.Sector, Is.SameAs(sector));
        }
    }
}