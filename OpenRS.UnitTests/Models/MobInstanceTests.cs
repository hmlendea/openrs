using System;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.UnitTests.Models
{
    [TestFixture]
    public sealed class MobInstanceTests
    {
        private MobInstanceTestDouble mobInstance = null!;

        [SetUp]
        public void SetUp()
        {
            mobInstance = new MobInstanceTestDouble();
        }

        [Test]
        public void GivenANewMob_WhenReadingItsState_ThenItsDefaultsRemainCompatible()
        {
            Assert.That(mobInstance.CombatLevel, Is.EqualTo(3));
            Assert.That(mobInstance.MobSprite, Is.EqualTo(1));
            Assert.That(mobInstance.AppearanceId, Is.Zero);
            Assert.That(mobInstance.CombatTime, Is.Zero);
            Assert.That(mobInstance.LastCombatState, Is.EqualTo(CombatState.Waiting));
            Assert.That(mobInstance.HasAppearanceChanged);
            Assert.That(mobInstance.HasSpriteChanged, Is.False);
            Assert.That(mobInstance.HasMoved, Is.False);
            Assert.That(mobInstance.IsInCombat, Is.False);
            Assert.That(mobInstance.FinishedPath());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(13)]
        public void GivenAValidPrayerIndex_WhenTogglingIt_ThenItsActivationStateIsUpdated(int prayerIndex)
        {
            Assert.That(mobInstance.IsPrayerActivated(prayerIndex), Is.False);

            mobInstance.TogglePrayer(prayerIndex, true);

            Assert.That(mobInstance.IsPrayerActivated(prayerIndex));

            mobInstance.TogglePrayer(prayerIndex, false);

            Assert.That(mobInstance.IsPrayerActivated(prayerIndex), Is.False);
        }

        [TestCase(-1)]
        [TestCase(14)]
        [TestCase(42)]
        public void GivenAnInvalidPrayerIndex_WhenReadingIt_ThenAnIndexExceptionIsThrown(int prayerIndex)
            => Assert.That(
                () => mobInstance.IsPrayerActivated(prayerIndex),
                Throws.TypeOf<IndexOutOfRangeException>());

        [TestCase(-1)]
        [TestCase(14)]
        [TestCase(42)]
        public void GivenAnInvalidPrayerIndex_WhenTogglingIt_ThenAnIndexExceptionIsThrown(int prayerIndex)
            => Assert.That(
                () => mobInstance.TogglePrayer(prayerIndex, true),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenEveryPrayer_WhenActivatingThem_ThenEachPrayerRetainsItsState()
        {
            for (int prayerIndex = 0; prayerIndex < Prayer.MaximumCount; prayerIndex += 1)
            {
                mobInstance.TogglePrayer(prayerIndex, true);
            }

            for (int prayerIndex = 0; prayerIndex < Prayer.MaximumCount; prayerIndex += 1)
            {
                Assert.That(mobInstance.IsPrayerActivated(prayerIndex));
            }
        }

        [Test]
        public void GivenTwoActivePrayers_WhenDeactivatingOne_ThenTheOtherRemainsActive()
        {
            mobInstance.TogglePrayer(0, true);
            mobInstance.TogglePrayer(Prayer.MaximumCount - 1, true);

            mobInstance.TogglePrayer(0, false);

            Assert.That(mobInstance.IsPrayerActivated(0), Is.False);
            Assert.That(mobInstance.IsPrayerActivated(Prayer.MaximumCount - 1));
        }

        [Test]
        public void GivenAnAppearanceChange_WhenUpdatingItsIdentifier_ThenEachCallIncrementsIt()
        {
            mobInstance.UpdateAppearanceId();
            mobInstance.UpdateAppearanceId();

            Assert.That(mobInstance.AppearanceId, Is.EqualTo(2));
        }

        [TestCase(8)]
        [TestCase(9)]
        [TestCase(42)]
        public void GivenANewSprite_WhenSettingIt_ThenTheSpriteChangeIsRecorded(int spriteIndex)
        {
            mobInstance.MobSprite = spriteIndex;

            Assert.That(mobInstance.MobSprite, Is.EqualTo(spriteIndex));
            Assert.That(mobInstance.HasSpriteChanged);
            Assert.That(mobInstance.IsInCombat, Is.False);
        }

        [TestCase(-1, -1, 7)]
        [TestCase(0, -1, 0)]
        [TestCase(1, -1, 1)]
        [TestCase(-1, 0, 6)]
        [TestCase(0, 0, -1)]
        [TestCase(1, 0, 2)]
        [TestCase(-1, 1, 5)]
        [TestCase(0, 1, 4)]
        [TestCase(1, 1, 3)]
        public void GivenAnAdjacentLocation_WhenMovingWithoutTeleporting_ThenTheDirectionalSpriteIsSelected(
            int destinationX,
            int destinationY,
            int expectedSpriteIndex)
        {
            Point2D destination = new(destinationX, destinationY);

            mobInstance.SetLocation(destination, false);

            Assert.That(mobInstance.Location, Is.EqualTo(destination));
            Assert.That(mobInstance.MobSprite, Is.EqualTo(expectedSpriteIndex));
            Assert.That(mobInstance.HasMoved);
            Assert.That(mobInstance.HasSpriteChanged);
        }

        [Test]
        public void GivenADistantLocation_WhenTeleporting_ThenMovementAndSpriteStateRemainUnchanged()
        {
            Point2D destination = new(42, 64);

            mobInstance.SetLocation(destination, true);

            Assert.That(mobInstance.Location, Is.EqualTo(destination));
            Assert.That(mobInstance.MobSprite, Is.EqualTo(1));
            Assert.That(mobInstance.HasMoved, Is.False);
            Assert.That(mobInstance.HasSpriteChanged, Is.False);
        }

        [Test]
        public void GivenADistantLocation_WhenMovingWithoutTeleporting_ThenLocationChangesAndSpriteRemains()
        {
            Point2D destination = new(42, 64);

            mobInstance.SetLocation(destination, false);

            Assert.That(mobInstance.Location, Is.EqualTo(destination));
            Assert.That(mobInstance.MobSprite, Is.EqualTo(1));
            Assert.That(mobInstance.HasMoved);
            Assert.That(mobInstance.HasSpriteChanged, Is.False);
        }

        [Test]
        public void GivenAMovementWarning_WhenSettingALocation_ThenTheWarningIsCleared()
        {
            mobInstance.WarnedToMove = true;

            mobInstance.SetLocation(new Point2D(4, 8), true);

            Assert.That(mobInstance.WarnedToMove, Is.False);
        }

        [Test]
        public void GivenANewMob_WhenUpdatingCombatTime_ThenAUnixTimestampIsRecorded()
        {
            mobInstance.UpdateCombatTime();

            Assert.That(mobInstance.CombatTime, Is.GreaterThan(0));
        }

        [Test]
        public void GivenAMob_WhenRemovingIt_ThenTheConcreteRemovalMethodIsInvoked()
        {
            mobInstance.Remove();

            Assert.That(mobInstance.IsRemoveInvoked);
        }

        [Test]
        public void GivenAMob_WhenResettingCombat_ThenANotImplementedExceptionIsThrown()
            => Assert.That(
                () => mobInstance.ResetCombat(),
                Throws.TypeOf<NotImplementedException>());

        [Test]
        public void GivenAMob_WhenCheckingWhetherItIsAtAnObject_ThenANotImplementedExceptionIsThrown()
            => Assert.That(
                () => mobInstance.IsAtObject(),
                Throws.TypeOf<NotImplementedException>());
    }
}