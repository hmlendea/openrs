using System;
using System.IO;

using NUnit.Framework;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class SectorTileTests
    {
        [Test]
        public void GivenANewTile_WhenReadingItsState_ThenItsValuesAreZeroAndItsSectorIsNull()
        {
            SectorTile tile = new();

            Assert.That(tile.GroundElevation, Is.Zero);
            Assert.That(tile.GroundTexture, Is.Zero);
            Assert.That(tile.GroundOverlay, Is.Zero);
            Assert.That(tile.RoofTexture, Is.Zero);
            Assert.That(tile.HorizontalWall, Is.Zero);
            Assert.That(tile.VerticalWall, Is.Zero);
            Assert.That(tile.DiagonalWalls, Is.Zero);
            Assert.That(tile.Sector, Is.Null);
        }

        [Test]
        public void GivenASector_WhenConstructingATile_ThenTheSectorReferenceIsPreserved()
        {
            RscSector expectedSector = new();

            SectorTile tile = new(expectedSector);

            Assert.That(tile.Sector, Is.SameAs(expectedSector));
        }

        [Test]
        public void GivenACompleteTileBuffer_WhenUnpackingIt_ThenEveryFieldIsReadInWireOrder()
        {
            using MemoryStream stream = new(SectorTestDataBuilder.BuildTileData());

            SectorTile tile = SectorTile.Unpack(stream);

            Assert.That(tile.GroundElevation, Is.EqualTo(4));
            Assert.That(tile.GroundTexture, Is.EqualTo(8));
            Assert.That(tile.GroundOverlay, Is.EqualTo(16));
            Assert.That(tile.RoofTexture, Is.EqualTo(32));
            Assert.That(tile.HorizontalWall, Is.EqualTo(42));
            Assert.That(tile.VerticalWall, Is.EqualTo(48));
            Assert.That(tile.DiagonalWalls, Is.EqualTo(SectorTestDataBuilder.DiagonalWallsValue));
            Assert.That(tile.Sector, Is.Null);
            Assert.That(stream.Position, Is.EqualTo(SectorTestDataBuilder.TileByteCount));
        }

        [Test]
        public void GivenAnOffsetTileBuffer_WhenUnpackingIt_ThenOnlyTheTileBytesAreConsumed()
        {
            byte[] prefix = new byte[4];
            byte[] suffix = new byte[8];
            byte[] buffer = [.. prefix, .. SectorTestDataBuilder.BuildTileData(), .. suffix];
            using MemoryStream stream = new(buffer)
            {
                Position = prefix.Length,
            };

            SectorTile tile = SectorTile.Unpack(stream);

            Assert.That(tile.GroundElevation, Is.EqualTo(4));
            Assert.That(
                stream.Position,
                Is.EqualTo(prefix.Length + SectorTestDataBuilder.TileByteCount));
            Assert.That(stream.Remaining(), Is.EqualTo(suffix.Length));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(8)]
        [TestCase(9)]
        public void GivenATruncatedTileBuffer_WhenUnpackingIt_ThenAnIOExceptionIsThrown(int byteCount)
        {
            using MemoryStream stream = new(new byte[byteCount]);

            Assert.That(
                () => SectorTile.Unpack(stream),
                Throws.TypeOf<IOException>());
        }

        [Test]
        public void GivenInsufficientBytesAfterThePosition_WhenUnpackingATile_ThenAnIOExceptionIsThrown()
        {
            using MemoryStream stream = new(new byte[SectorTestDataBuilder.TileByteCount + 8])
            {
                Position = 9,
            };

            Assert.That(
                () => SectorTile.Unpack(stream),
                Throws.TypeOf<IOException>());
        }

        [Test]
        public void GivenANullStream_WhenUnpackingATile_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => SectorTile.Unpack(null!),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("inputStream"));

        [Test]
        public void GivenADisposedStream_WhenUnpackingATile_ThenAnObjectDisposedExceptionIsThrown()
        {
            MemoryStream stream = new(SectorTestDataBuilder.BuildTileData());
            stream.Dispose();

            Assert.That(
                () => SectorTile.Unpack(stream),
                Throws.TypeOf<ObjectDisposedException>());
        }
    }
}