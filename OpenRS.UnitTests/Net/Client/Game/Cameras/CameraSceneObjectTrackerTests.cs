using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    public sealed class CameraSceneObjectTrackerTests
    {
        private CameraSceneObjectTracker tracker = null!;

        [SetUp]
        public void SetUp()
        {
            tracker = new CameraSceneObjectTracker(4, 8);
        }

        [Test]
        public void GivenANewTracker_WhenReadingItsState_ThenSceneAndHitCollectionsAreInitialised()
        {
            Assert.That(tracker.SceneObjectIds, Has.Length.EqualTo(4));
            Assert.That(tracker.SceneObjectWidths, Has.Length.EqualTo(4));
            Assert.That(tracker.SceneObjectHeights, Has.Length.EqualTo(4));
            Assert.That(tracker.SceneObjectFrames, Has.Length.EqualTo(4));
            Assert.That(tracker.GetHighlightedPlayers(), Has.Length.EqualTo(8));
            Assert.That(tracker.GetHighlightedObjects(), Has.Length.EqualTo(8));
            Assert.That(tracker.GetOptionCount(), Is.Zero);
            Assert.That(tracker.IsHitCandidate, Is.False);
            Assert.That(tracker.HighlightedObject.VertexCount, Is.Zero);
            Assert.That(tracker.HighlightedObject.FaceCount, Is.Zero);
        }

        [Test]
        public void GivenSpriteData_WhenAddingIt_ThenItsSceneAndGeometryStateAreStored()
        {
            int spriteIndex = tracker.AddSpriteToScene(42, 64, 96, 128, 16, 32, 8);

            Assert.That(spriteIndex, Is.Zero);
            Assert.That(tracker.SceneObjectIds[0], Is.EqualTo(42));
            Assert.That(tracker.SceneObjectWidths[0], Is.EqualTo(16));
            Assert.That(tracker.SceneObjectHeights[0], Is.EqualTo(32));
            Assert.That(tracker.SceneObjectFrames[0], Is.Zero);
            Assert.That(tracker.HighlightedObject.VertexCount, Is.EqualTo(2));
            Assert.That(tracker.HighlightedObject.FaceCount, Is.EqualTo(1));
            Assert.That(tracker.HighlightedObject.EntityType[0], Is.EqualTo(8));
            Assert.That(tracker.HighlightedObject.PolygonTypeData[0], Is.Zero);
        }

        [Test]
        public void GivenMultipleSprites_WhenAddingThem_ThenSequentialIndicesAreReturned()
        {
            Assert.That(AddSprite(42), Is.Zero);
            Assert.That(AddSprite(64), Is.EqualTo(1));
            Assert.That(AddSprite(96), Is.EqualTo(2));
            Assert.That(AddSprite(128), Is.EqualTo(3));
        }

        [Test]
        public void GivenFullSceneCapacity_WhenAddingAnotherSprite_ThenAnIndexExceptionIsThrown()
        {
            AddSprite(42);
            AddSprite(64);
            AddSprite(96);
            AddSprite(128);

            Assert.That(
                () => AddSprite(256),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(0, 4)]
        [TestCase(1, 8)]
        [TestCase(3, 42)]
        public void GivenASprite_WhenUpdatingItsFrame_ThenTheFrameIsStored(int spriteIndex, int frameIndex)
        {
            tracker.UpdateSpritePosition(spriteIndex, frameIndex);

            Assert.That(tracker.SceneObjectFrames[spriteIndex], Is.EqualTo(frameIndex));
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void GivenAnInvalidSpriteIndex_WhenUpdatingItsFrame_ThenAnIndexExceptionIsThrown(int spriteIndex)
            => Assert.That(
                () => tracker.UpdateSpritePosition(spriteIndex, 42),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenASprite_WhenRemovingIt_ThenItsPolygonIsMarkedAsRemoved()
        {
            int spriteIndex = AddSprite(42);

            tracker.RemoveSprite(spriteIndex);

            Assert.That(tracker.HighlightedObject.PolygonTypeData[spriteIndex], Is.EqualTo(1));
        }

        [Test]
        public void GivenSceneData_WhenInitialisingTheScene_ThenIndicesAndGeometryRestartAtZero()
        {
            AddSprite(42);
            AddSprite(64);

            tracker.InitializeScene();

            Assert.That(tracker.HighlightedObject.VertexCount, Is.Zero);
            Assert.That(tracker.HighlightedObject.FaceCount, Is.Zero);
            Assert.That(AddSprite(96), Is.Zero);
        }

        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(3, 0)]
        [TestCase(8, 0)]
        public void GivenThreeSprites_WhenRemovingLastUpdates_ThenTheNextIndexReflectsTheRemainingCount(
            int removalCount,
            int expectedNextIndex)
        {
            AddSprite(42);
            AddSprite(64);
            AddSprite(96);

            tracker.RemoveLastUpdates(removalCount);

            Assert.That(AddSprite(128), Is.EqualTo(expectedNextIndex));
            Assert.That(tracker.HighlightedObject.FaceCount, Is.EqualTo(expectedNextIndex + 1));
            Assert.That(tracker.HighlightedObject.VertexCount, Is.EqualTo((expectedNextIndex + 1) * 2));
        }

        [Test]
        public void GivenAMousePosition_WhenSettingIt_ThenHitCollectionBecomesAvailable()
        {
            tracker.SetMousePosition(42, 64);

            Assert.That(tracker.MouseAdjustedX, Is.EqualTo(42));
            Assert.That(tracker.MouseAdjustedY, Is.EqualTo(64));
            Assert.That(tracker.GetOptionCount(), Is.Zero);
            Assert.That(tracker.IsHitCandidate);
        }

        [Test]
        public void GivenRecordedHits_WhenReadingThem_ThenObjectsAndFaceIndicesRemainInOrder()
        {
            GameObject firstObject = new(0, 0);
            GameObject secondObject = new(0, 0);
            tracker.SetMousePosition(42, 64);

            tracker.RecordHit(firstObject, 4);
            tracker.RecordHit(secondObject, 8);

            Assert.That(tracker.GetOptionCount(), Is.EqualTo(2));
            Assert.That(tracker.GetHighlightedObjects()[0], Is.SameAs(firstObject));
            Assert.That(tracker.GetHighlightedObjects()[1], Is.SameAs(secondObject));
            Assert.That(tracker.GetHighlightedPlayers()[..2], Is.EqualTo(new[] { 4, 8 }));
        }

        [Test]
        public void GivenMaximumRecordedHits_WhenCheckingCandidateState_ThenNoFurtherCandidateIsAvailable()
        {
            tracker.SetMousePosition(42, 64);

            for (int hitIndex = 0; hitIndex < 8; hitIndex += 1)
            {
                tracker.RecordHit(new GameObject(0, 0), hitIndex);
            }

            Assert.That(tracker.IsHitCandidate, Is.False);
            Assert.That(
                () => tracker.RecordHit(new GameObject(0, 0), 8),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenRecordedHits_WhenSettingAnotherMousePosition_ThenTheOptionCountRestarts()
        {
            tracker.SetMousePosition(42, 64);
            tracker.RecordHit(new GameObject(0, 0), 4);

            tracker.SetMousePosition(96, 128);

            Assert.That(tracker.GetOptionCount(), Is.Zero);
            Assert.That(tracker.IsHitCandidate);
        }

        [Test]
        public void GivenAnActiveHitFrame_WhenFinalisingIt_ThenHitCollectionIsDeactivated()
        {
            tracker.SetMousePosition(42, 64);

            tracker.FinaliseFrame();

            Assert.That(tracker.IsHitCandidate, Is.False);
        }

        [Test]
        public void GivenAnUnusedValidSpriteIndex_WhenRemovingIt_ThenThePolygonSlotIsMarkedAsRemoved()
        {
            tracker.RemoveSprite(3);

            Assert.That(tracker.HighlightedObject.PolygonTypeData[3], Is.EqualTo(1));
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void GivenAnInvalidSpriteIndex_WhenRemovingIt_ThenAnIndexExceptionIsThrown(int spriteIndex)
            => Assert.That(
                () => tracker.RemoveSprite(spriteIndex),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenSceneData_WhenInitialisingTheScene_ThenStoredMetadataRemainsInItsSlots()
        {
            AddSprite(42);

            tracker.InitializeScene();

            Assert.That(tracker.SceneObjectIds[0], Is.EqualTo(42));
            Assert.That(tracker.SceneObjectWidths[0], Is.EqualTo(32));
            Assert.That(tracker.SceneObjectHeights[0], Is.EqualTo(42));
            Assert.That(tracker.SceneObjectFrames[0], Is.Zero);
        }

        [Test]
        public void GivenARecordedHitWithoutAMouseFrame_WhenReadingIt_ThenTheHitIsStoredButNotActive()
        {
            GameObject expectedObject = new(0, 0);

            tracker.RecordHit(expectedObject, 42);

            Assert.That(tracker.GetOptionCount(), Is.EqualTo(1));
            Assert.That(tracker.GetHighlightedObjects()[0], Is.SameAs(expectedObject));
            Assert.That(tracker.GetHighlightedPlayers()[0], Is.EqualTo(42));
            Assert.That(tracker.IsHitCandidate, Is.False);
        }

        private int AddSprite(int objectIdentifier)
            => tracker.AddSpriteToScene(objectIdentifier, 4, 8, 16, 32, 42, 64);
    }
}