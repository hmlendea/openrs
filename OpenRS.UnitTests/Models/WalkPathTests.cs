using System;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class WalkPathTests
    {
        [Test]
        public void GivenADestinationOnly_WhenConstructingThePath_ThenItContainsNoWaypoints()
        {
            Point2D destination = new(42, 64);
            WalkPath path = new(destination);

            Assert.That(path.StartLocation, Is.EqualTo(destination));
            Assert.That(path.Length, Is.Zero);
        }

        [Test]
        public void GivenWaypointOffsets_WhenConstructingThePath_ThenItsLengthMatchesTheOffsets()
        {
            Point2D[] waypointOffsets = [new(4, 8), new(16, 32), new(42, 64)];
            WalkPath path = new(new Point2D(128, 256), waypointOffsets);

            Assert.That(path.Length, Is.EqualTo(3));
        }

        [TestCase(0, 132, 264)]
        [TestCase(1, 144, 288)]
        [TestCase(2, 170, 320)]
        public void GivenWaypointOffsets_WhenRetrievingAWaypoint_ThenItIsRelativeToTheStart(
            int waypointIndex,
            int expectedPositionX,
            int expectedPositionY)
        {
            Point2D[] waypointOffsets = [new(4, 8), new(16, 32), new(42, 64)];
            WalkPath path = new(new Point2D(128, 256), waypointOffsets);

            Assert.That(
                path.GetWaypoint(waypointIndex),
                Is.EqualTo(new Point2D(expectedPositionX, expectedPositionY)));
        }

        [TestCase(-1)]
        [TestCase(3)]
        public void GivenAnInvalidWaypointIndex_WhenRetrievingAWaypoint_ThenAnIndexExceptionIsThrown(
            int waypointIndex)
        {
            WalkPath path = new(new Point2D(128, 256), [new Point2D(4, 8)]);

            Assert.That(
                () => path.GetWaypoint(waypointIndex),
                Throws.TypeOf<IndexOutOfRangeException>());
        }
    }
}