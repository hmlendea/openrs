using System;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class GameEntityInstanceTests
    {
        [TestCase(0, 0, 0, 0, 0, true)]
        [TestCase(0, 0, 1, 0, 0, false)]
        [TestCase(0, 0, 0, 1, 0, false)]
        [TestCase(0, 0, 4, 8, 8, true)]
        [TestCase(0, 0, 8, 8, 8, true)]
        [TestCase(0, 0, 9, 8, 8, false)]
        [TestCase(0, 0, 8, 9, 8, false)]
        [TestCase(4, 8, -4, -8, 16, true)]
        [TestCase(4, 8, -4, -9, 16, false)]
        [TestCase(-42, -64, -42, -64, -1, false)]
        public void GivenTwoLocations_WhenCheckingTheirRange_ThenChebyshevDistanceIsApplied(
            int entityPositionX,
            int entityPositionY,
            int otherPositionX,
            int otherPositionY,
            int radius,
            bool expectedIsWithinRange)
        {
            GameEntityInstance entity = BuildEntity(entityPositionX, entityPositionY);
            Point2D otherLocation = new(otherPositionX, otherPositionY);

            bool isWithinRange = entity.IsWithinRange(otherLocation, radius);

            Assert.That(isWithinRange, Is.EqualTo(expectedIsWithinRange));
        }

        [TestCase(0)]
        [TestCase(4)]
        [TestCase(42)]
        public void GivenAnotherEntity_WhenCheckingItsRange_ThenItsLocationIsUsed(int radius)
        {
            GameEntityInstance entity = BuildEntity(4, 8);
            GameEntityInstance other = BuildEntity(4 + radius, 8 + radius);

            Assert.That(entity.IsWithinRange(other, radius));
        }

        [Test]
        public void GivenANullEntity_WhenCheckingItsRange_ThenANullReferenceExceptionIsThrown()
        {
            GameEntityInstance entity = new();

            Assert.That(
                () => entity.IsWithinRange((GameEntityInstance)null!, 8),
                Throws.TypeOf<NullReferenceException>());
        }

        [TestCase(int.MinValue, 0)]
        [TestCase(0, int.MinValue)]
        public void GivenAnOverflowingCoordinateDifference_WhenCheckingItsRange_ThenAnOverflowExceptionIsThrown(
            int otherPositionX,
            int otherPositionY)
        {
            GameEntityInstance entity = new();

            Assert.That(
                () => entity.IsWithinRange(new Point2D(otherPositionX, otherPositionY), int.MaxValue),
                Throws.TypeOf<OverflowException>());
        }

        [Test]
        public void GivenANewLocation_WhenSettingIt_ThenTheEntityLocationIsReplaced()
        {
            GameEntityInstance entity = BuildEntity(4, 8);
            Point2D expectedLocation = new(42, 64);

            entity.SetLocation(expectedLocation);

            Assert.That(entity.Location, Is.EqualTo(expectedLocation));
        }

        [Test]
        public void GivenAnEntityAtTheDestination_WhenCalculatingItsNextStep_ThenItsLocationIsReturned()
        {
            Point2D location = new(42, 64);
            GameEntityInstance entity = BuildEntity(location.X, location.Y);
            GameEntityInstance other = BuildEntity(location.X, location.Y);

            Point2D? nextStep = entity.NextStep(location, other);

            Assert.That(nextStep, Is.EqualTo(location));
        }

        private static GameEntityInstance BuildEntity(int positionX, int positionY)
        {
            GameEntityInstance entity = new();
            entity.SetLocation(new Point2D(positionX, positionY));

            return entity;
        }
    }
}