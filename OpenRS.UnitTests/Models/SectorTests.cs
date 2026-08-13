using System;

using NUnit.Framework;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class SectorTests
    {
        private Sector sector = null!;

        [SetUp]
        public void SetUp()
        {
            sector = new Sector();
        }

        [Test]
        public void GivenTheSectorContract_WhenReadingItsSize_ThenItRemainsFortyEightByFortyEight()
        {
            Assert.That(Sector.Size.Width, Is.EqualTo(48));
            Assert.That(Sector.Size.Height, Is.EqualTo(48));
            Assert.That(Sector.Size.Area, Is.EqualTo(2304));
        }

        [TestCase(0, 0)]
        [TestCase(47, 47)]
        [TestCase(4, 8)]
        public void GivenAValidCoordinate_WhenSettingATile_ThenTheSameTileIsReturned(int positionX, int positionY)
        {
            WorldTile expectedTile = new();

            sector.SetTile(positionX, positionY, expectedTile);

            Assert.That(
                sector.GetTile(positionX, positionY),
                Is.SameAs(expectedTile));
        }

        [Test]
        public void GivenACoordinate_WhenSettingATile_ThenTheCompatibleLinearIndexIsUpdated()
        {
            WorldTile expectedTile = new();

            sector.SetTile(4, 8, expectedTile);

            Assert.That(
                sector.GetTile(200),
                Is.SameAs(expectedTile));
        }

        [Test]
        public void GivenAYCoordinateEqualToTheSectorHeight_WhenRetrievingATile_ThenTheNextLinearRowIsUsed()
            => Assert.That(
                sector.GetTile(0, 48),
                Is.SameAs(sector.GetTile(1, 0)));

        [TestCase(0)]
        [TestCase(2303)]
        [TestCase(512)]
        public void GivenAValidIndex_WhenSettingATile_ThenTheSameTileIsReturned(int tileIndex)
        {
            WorldTile expectedTile = new();

            sector.SetTile(tileIndex, expectedTile);

            Assert.That(
                sector.GetTile(tileIndex),
                Is.SameAs(expectedTile));
        }

        [Test]
        public void GivenANewSector_WhenRetrievingDifferentTiles_ThenTheirInstancesAreDistinct()
            => Assert.That(
                sector.GetTile(0),
                Is.Not.SameAs(sector.GetTile(1)));

        [TestCase(-1)]
        [TestCase(2304)]
        public void GivenAnInvalidIndex_WhenRetrievingATile_ThenAnIndexExceptionIsThrown(int tileIndex)
            => Assert.That(
                () => sector.GetTile(tileIndex),
                Throws.TypeOf<IndexOutOfRangeException>());

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(48, 0)]
        public void GivenAnInvalidCoordinate_WhenRetrievingATile_ThenAnIndexExceptionIsThrown(
            int positionX,
            int positionY)
            => Assert.That(
                () => sector.GetTile(positionX, positionY),
                Throws.TypeOf<IndexOutOfRangeException>());
    }
}