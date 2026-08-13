using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class ClientMobTests
    {
        [Test]
        public void GivenANewClientMob_WhenReadingItsState_ThenProtocolArraysAndDefaultsAreInitialised()
        {
            ClientMob mob = new();

            Assert.That(mob.Appearance, Is.Not.Null);
            Assert.That(mob.Location, Is.EqualTo(new Point2D(0, 0)));
            Assert.That(mob.LocationX, Is.Zero);
            Assert.That(mob.LocationY, Is.Zero);
            Assert.That(mob.WaypointXPositions, Has.Length.EqualTo(10));
            Assert.That(mob.WaypointYPositions, Has.Length.EqualTo(10));
            Assert.That(mob.AppearanceItems, Has.Length.EqualTo(12));
            Assert.That(mob.WaypointXPositions, Has.All.Zero);
            Assert.That(mob.WaypointYPositions, Has.All.Zero);
            Assert.That(mob.AppearanceItems, Has.All.Zero);
            Assert.That(mob.CombatLevel, Is.EqualTo(-1));
        }

        [Test]
        public void GivenTwoClientMobs_WhenReadingTheirArraysAndAppearance_ThenTheirInstancesAreIndependent()
        {
            ClientMob firstMob = new();
            ClientMob secondMob = new();

            firstMob.WaypointXPositions[0] = 42;
            firstMob.WaypointYPositions[0] = 64;
            firstMob.AppearanceItems[0] = 96;

            Assert.That(firstMob.Appearance, Is.Not.SameAs(secondMob.Appearance));
            Assert.That(firstMob.WaypointXPositions, Is.Not.SameAs(secondMob.WaypointXPositions));
            Assert.That(firstMob.WaypointYPositions, Is.Not.SameAs(secondMob.WaypointYPositions));
            Assert.That(firstMob.AppearanceItems, Is.Not.SameAs(secondMob.AppearanceItems));
            Assert.That(secondMob.WaypointXPositions[0], Is.Zero);
            Assert.That(secondMob.WaypointYPositions[0], Is.Zero);
            Assert.That(secondMob.AppearanceItems[0], Is.Zero);
        }

        [TestCase(int.MinValue)]
        [TestCase(-42)]
        [TestCase(0)]
        [TestCase(42)]
        [TestCase(int.MaxValue)]
        public void GivenAnXCoordinate_WhenSettingIt_ThenTheYCoordinateIsPreserved(int positionX)
        {
            ClientMob mob = new()
            {
                Location = new Point2D(4, 8),
            };

            mob.LocationX = positionX;

            Assert.That(mob.Location, Is.EqualTo(new Point2D(positionX, 8)));
            Assert.That(mob.LocationX, Is.EqualTo(positionX));
            Assert.That(mob.LocationY, Is.EqualTo(8));
        }

        [TestCase(int.MinValue)]
        [TestCase(-42)]
        [TestCase(0)]
        [TestCase(42)]
        [TestCase(int.MaxValue)]
        public void GivenAYCoordinate_WhenSettingIt_ThenTheXCoordinateIsPreserved(int positionY)
        {
            ClientMob mob = new()
            {
                Location = new Point2D(4, 8),
            };

            mob.LocationY = positionY;

            Assert.That(mob.Location, Is.EqualTo(new Point2D(4, positionY)));
            Assert.That(mob.LocationX, Is.EqualTo(4));
            Assert.That(mob.LocationY, Is.EqualTo(positionY));
        }
    }
}