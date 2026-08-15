using System;

using NUnit.Framework;

using OpenRS.GameLogic.GameManagers;

namespace OpenRS.UnitTests.GameLogic.GameManagers
{
    [TestFixture]
    [NonParallelizable]
    public sealed class EntityManagerModelTests
    {
        [Test]
        public void GivenTheNotAvailableModel_WhenResolvingIt_ThenZeroIsReturnedWithoutIncreasingCount()
        {
            EntityManager manager = new();
            int initialCount = manager.ObjectModelCount;

            int modelIndex = manager.GetModelIndex("na");

            Assert.That(modelIndex, Is.Zero);
            Assert.That(manager.ObjectModelCount, Is.EqualTo(initialCount));
        }

        [Test]
        public void GivenANewModelName_WhenResolvingIt_ThenItIsAppendedAndRetrievable()
        {
            EntityManager manager = new();
            string modelName = $"coverage_model_{manager.ObjectModelCount}";

            int modelIndex = manager.GetModelIndex(modelName);

            Assert.That(manager.GetObjectModelName(modelIndex), Is.EqualTo(modelName));
            Assert.That(manager.ObjectModelCount, Is.EqualTo(modelIndex + 1));
        }

        [Test]
        public void GivenAnExistingModelNameWithDifferentCase_WhenResolvingIt_ThenTheSameIndexIsReturned()
        {
            EntityManager manager = new();
            string modelName = $"coverage_case_{manager.ObjectModelCount}";
            int expectedIndex = manager.GetModelIndex(modelName);

            int modelIndex = manager.GetModelIndex(modelName.ToUpperInvariant());

            Assert.That(modelIndex, Is.EqualTo(expectedIndex));
        }

        [Test]
        public void GivenUnloadedEntityCollections_WhenReadingTheirCounts_ThenNullStateIsPreserved()
        {
            EntityManager manager = new();

            Assert.That(() => _ = manager.AnimationCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.ElevationCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.ItemCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.NpcCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.PrayerCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.SpellCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.TextureCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.TileCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.WallObjectCount, Throws.TypeOf<NullReferenceException>());
            Assert.That(() => _ = manager.WorldObjectCount, Throws.TypeOf<NullReferenceException>());
        }
    }
}