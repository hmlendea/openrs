using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class PathHandlerTests
    {
        private MobInstanceTestDouble mobInstance = null!;
        private PathHandler pathHandler = null!;

        [SetUp]
        public void SetUp()
        {
            mobInstance = new MobInstanceTestDouble();
            pathHandler = new PathHandler(mobInstance);
        }

        [Test]
        public void GivenANewPathHandler_WhenCheckingItsPath_ThenItIsFinishedWithoutAPath()
        {
            Assert.That(pathHandler.Path, Is.Null);
            Assert.That(pathHandler.FinishedPath());
        }

        [Test]
        public void GivenADestinationAtTheMobLocation_WhenSettingThePath_ThenItIsFinished()
        {
            WalkPath path = new(mobInstance.Location);

            pathHandler.SetPath(path);

            Assert.That(pathHandler.Path, Is.SameAs(path));
            Assert.That(pathHandler.FinishedPath());
        }

        [Test]
        public void GivenADestinationAwayFromTheMob_WhenSettingThePath_ThenItIsNotFinished()
        {
            pathHandler.SetPath(new WalkPath(new Point2D(42, 64)));

            Assert.That(pathHandler.FinishedPath(), Is.False);
        }

        [Test]
        public void GivenTheMobAtTheFinalWaypoint_WhenCheckingThePath_ThenItIsFinished()
        {
            WalkPath path = new(new Point2D(4, 8), [new Point2D(12, 24), new Point2D(26, 56)]);
            mobInstance.SetLocation(new Point2D(30, 64), true);
            pathHandler.SetPath(path);

            Assert.That(pathHandler.FinishedPath());
        }

        [Test]
        public void GivenTheMobBeforeTheFinalWaypoint_WhenCheckingThePath_ThenItIsNotFinished()
        {
            WalkPath path = new(new Point2D(4, 8), [new Point2D(12, 24), new Point2D(26, 56)]);
            mobInstance.SetLocation(new Point2D(16, 32), true);
            pathHandler.SetPath(path);

            Assert.That(pathHandler.FinishedPath(), Is.False);
        }

        [Test]
        public void GivenAFinishedPath_WhenUpdatingLocation_ThenTheMobIsNotMoved()
        {
            Point2D expectedLocation = new(42, 64);
            mobInstance.SetLocation(expectedLocation, true);
            pathHandler.SetPath(new WalkPath(expectedLocation));

            pathHandler.UpdateLocation();

            Assert.That(mobInstance.Location, Is.EqualTo(expectedLocation));
            Assert.That(mobInstance.HasMoved, Is.False);
        }

        [Test]
        public void GivenAPath_WhenResettingIt_ThenTheHandlerReturnsToFinishedState()
        {
            pathHandler.SetPath(new WalkPath(new Point2D(42, 64)));

            pathHandler.ResetPath();

            Assert.That(pathHandler.Path, Is.Null);
            Assert.That(pathHandler.FinishedPath());
        }
    }
}