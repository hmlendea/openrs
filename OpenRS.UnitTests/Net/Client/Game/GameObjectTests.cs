using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class GameObjectTests
    {
        private GameObject gameObject = null!;

        private static int VertexCapacity => 4;

        private static int FaceCapacity => 8;

        private static int DefaultShadeValue => 0xbc614e;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject(VertexCapacity, FaceCapacity);
        }

        [Test]
        public void GivenCapacities_WhenConstructingAnObject_ThenItsRequiredArraysAndDefaultsAreInitialised()
        {
            Assert.That(gameObject.VertexCount, Is.Zero);
            Assert.That(gameObject.FaceCount, Is.Zero);
            Assert.That(gameObject.TotalVertexCapacity, Is.EqualTo(VertexCapacity));
            Assert.That(gameObject.VertexCoordinatesX, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.VertexCoordinatesY, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.VertexCoordinatesZ, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.VertexVectors, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.ProjectedX, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.ProjectedY, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.ProjectedDepth, Has.Length.EqualTo(VertexCapacity));
            Assert.That(gameObject.FaceVertexCounts, Has.Length.EqualTo(FaceCapacity));
            Assert.That(gameObject.FaceVertexIndices, Has.Length.EqualTo(FaceCapacity));
            Assert.That(gameObject.TextureBack, Has.Length.EqualTo(FaceCapacity));
            Assert.That(gameObject.TextureFront, Has.Length.EqualTo(FaceCapacity));
            Assert.That(gameObject.EntityType, Has.Length.EqualTo(FaceCapacity));
            Assert.That(gameObject.PolygonTypeData, Has.Length.EqualTo(FaceCapacity));
            Assert.That(gameObject.WorldVertX, Is.Not.SameAs(gameObject.VertexCoordinatesX));
            Assert.That(gameObject.IsVisible);
            Assert.That(gameObject.IsTranslucent);
            Assert.That(gameObject.IsPerspectiveTextured, Is.False);
            Assert.That(gameObject.Index, Is.EqualTo(-1));
            Assert.That(gameObject.MaximumFaceSpan, Is.EqualTo(DefaultShadeValue));
            Assert.That(gameObject.LightDirectionX, Is.EqualTo(180));
            Assert.That(gameObject.LightDirectionY, Is.EqualTo(155));
            Assert.That(gameObject.LightDirectionZ, Is.EqualTo(95));
            Assert.That(gameObject.LightMagnitude, Is.EqualTo(256));
            Assert.That(gameObject.AmbientLightLevel, Is.EqualTo(512));
            Assert.That(gameObject.BaseShadeLevel, Is.EqualTo(32));
        }

        [Test]
        public void GivenAllSharingFlags_WhenConstructingAnObject_ThenOptionalArraysFollowThoseFlags()
        {
            GameObject sharedObject = new(4, 8, true, true, true, true, true);

            Assert.That(sharedObject.DoesShareWorldVertices);
            Assert.That(sharedObject.HasNoCollider);
            Assert.That(sharedObject.DoesNotReceiveShadows);
            Assert.That(sharedObject.DoesShareEntityArrays);
            Assert.That(sharedObject.DoesShareVertexArrays);
            Assert.That(sharedObject.WorldVertX, Is.SameAs(sharedObject.VertexCoordinatesX));
            Assert.That(sharedObject.WorldVertY, Is.SameAs(sharedObject.VertexCoordinatesY));
            Assert.That(sharedObject.WorldVertZ, Is.SameAs(sharedObject.VertexCoordinatesZ));
            Assert.That(sharedObject.ProjectedX, Is.Null);
            Assert.That(sharedObject.ProjectedY, Is.Null);
            Assert.That(sharedObject.ProjectedDepth, Is.Null);
            Assert.That(sharedObject.EntityType, Is.Null);
            Assert.That(sharedObject.PolygonTypeData, Is.Null);
            Assert.That(sharedObject.NormalX, Is.Null);
            Assert.That(sharedObject.NormalY, Is.Null);
            Assert.That(sharedObject.NormalZ, Is.Null);
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void GivenEitherShadowsOrCollision_WhenConstructingAnObject_ThenNormalArraysAreAllocated(
            bool doesNotReceiveShadows,
            bool isCollisionless)
        {
            GameObject model = new(4, 8, false, isCollisionless, doesNotReceiveShadows, false, false);

            Assert.That(model.NormalX, Has.Length.EqualTo(8));
            Assert.That(model.NormalY, Has.Length.EqualTo(8));
            Assert.That(model.NormalZ, Has.Length.EqualTo(8));
        }

        [Test]
        public void GivenExistingData_WhenReinitialisingArrays_ThenCountsAndCapacitiesAreReset()
        {
            gameObject.AddVertex(4, 8, 16);
            gameObject.AddFaceVertices(1, [0], 32, 42);

            gameObject.InitialiseArrays(16, 32);

            Assert.That(gameObject.VertexCount, Is.Zero);
            Assert.That(gameObject.FaceCount, Is.Zero);
            Assert.That(gameObject.TotalVertexCapacity, Is.EqualTo(16));
            Assert.That(gameObject.VertexCoordinatesX, Has.Length.EqualTo(16));
            Assert.That(gameObject.FaceVertexIndices, Has.Length.EqualTo(32));
        }

        [Test]
        public void GivenAvailableCapacity_WhenAddingVertices_ThenCoordinatesAndIndicesAreStoredInOrder()
        {
            Assert.That(gameObject.AddVertex(4, 8, 16), Is.Zero);
            Assert.That(gameObject.AddVertex(32, 42, 48), Is.EqualTo(1));
            Assert.That(gameObject.AddVertex(-64, -96, -128), Is.EqualTo(2));
            Assert.That(gameObject.VertexCount, Is.EqualTo(3));
            Assert.That(gameObject.VertexCoordinatesX[..3], Is.EqualTo(new[] { 4, 32, -64 }));
            Assert.That(gameObject.VertexCoordinatesY[..3], Is.EqualTo(new[] { 8, 42, -96 }));
            Assert.That(gameObject.VertexCoordinatesZ[..3], Is.EqualTo(new[] { 16, 48, -128 }));
        }

        [Test]
        public void GivenFullVertexCapacity_WhenAddingAnotherVertex_ThenItIsRejectedWithoutMutation()
        {
            FillVertexCapacity(gameObject);

            int vertexIndex = gameObject.AddVertex(512, 613, 873);

            Assert.That(vertexIndex, Is.EqualTo(-1));
            Assert.That(gameObject.VertexCount, Is.EqualTo(VertexCapacity));
        }

        [Test]
        public void GivenAnExistingVertex_WhenRetrievingItsIndex_ThenTheVertexIsNotDuplicated()
        {
            int firstIndex = gameObject.GetVertexIndex(4, 8, 16);
            int duplicateIndex = gameObject.GetVertexIndex(4, 8, 16);

            Assert.That(firstIndex, Is.Zero);
            Assert.That(duplicateIndex, Is.Zero);
            Assert.That(gameObject.VertexCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenFullCapacity_WhenRetrievingAnExistingOrNewVertexIndex_ThenOnlyTheExistingOneResolves()
        {
            FillVertexCapacity(gameObject);

            Assert.That(gameObject.GetVertexIndex(2, 4, 6), Is.EqualTo(2));
            Assert.That(gameObject.GetVertexIndex(512, 613, 873), Is.EqualTo(-1));
            Assert.That(gameObject.VertexCount, Is.EqualTo(VertexCapacity));
        }

        [Test]
        public void GivenAvailableFaceCapacity_WhenAddingAFace_ThenItsDataAndIdentityAreStored()
        {
            int[] expectedVertices = [0, 1, 2];

            int faceIndex = gameObject.AddFaceVertices(3, expectedVertices, 42, 64);

            Assert.That(faceIndex, Is.Zero);
            Assert.That(gameObject.FaceCount, Is.EqualTo(1));
            Assert.That(gameObject.FaceVertexCounts[0], Is.EqualTo(3));
            Assert.That(gameObject.FaceVertexIndices[0], Is.SameAs(expectedVertices));
            Assert.That(gameObject.TextureBack[0], Is.EqualTo(42));
            Assert.That(gameObject.TextureFront[0], Is.EqualTo(64));
            Assert.That(gameObject.ObjectState, Is.EqualTo(1));
        }

        [Test]
        public void GivenFullFaceCapacity_WhenAddingAnotherFace_ThenItIsRejectedWithoutMutation()
        {
            for (int faceIndex = 0; faceIndex < FaceCapacity; faceIndex += 1)
            {
                gameObject.AddFaceVertices(1, [0], faceIndex, faceIndex);
            }

            int rejectedFaceIndex = gameObject.AddFaceVertices(1, [0], 42, 64);

            Assert.That(rejectedFaceIndex, Is.EqualTo(-1));
            Assert.That(gameObject.FaceCount, Is.EqualTo(FaceCapacity));
        }

        [Test]
        public void GivenVerticesAndFaces_WhenResettingObjectIndices_ThenBothCountsReturnToZero()
        {
            gameObject.AddVertex(4, 8, 16);
            gameObject.AddFaceVertices(1, [0], 32, 42);

            gameObject.ResetObjectIndexes();

            Assert.That(gameObject.VertexCount, Is.Zero);
            Assert.That(gameObject.FaceCount, Is.Zero);
        }

        [TestCase(1, 2, 2, 2)]
        [TestCase(3, 6, 0, 0)]
        [TestCase(8, 16, 0, 0)]
        public void GivenObjectData_WhenRemovingTheLastGroup_ThenCountsDecreaseWithoutBecomingNegative(
            int faceDecrement,
            int vertexDecrement,
            int expectedFaceCount,
            int expectedVertexCount)
        {
            for (int vertexIndex = 0; vertexIndex < VertexCapacity; vertexIndex += 1)
            {
                gameObject.AddVertex(vertexIndex, vertexIndex, vertexIndex);
            }

            for (int faceIndex = 0; faceIndex < 3; faceIndex += 1)
            {
                gameObject.AddFaceVertices(1, [faceIndex], faceIndex, faceIndex);
            }

            gameObject.AddPolygonToGroup(faceDecrement, vertexDecrement);

            Assert.That(gameObject.FaceCount, Is.EqualTo(expectedFaceCount));
            Assert.That(gameObject.VertexCount, Is.EqualTo(expectedVertexCount));
        }

        [TestCase(0, 42)]
        [TestCase(3, -42)]
        public void GivenAValidVertexIndex_WhenSettingItsColour_ThenTheValueIsStored(
            int vertexIndex,
            int colour)
        {
            gameObject.SetVertexColour(vertexIndex, colour);

            Assert.That(gameObject.VertexColour[vertexIndex], Is.EqualTo(colour));
        }

        [TestCase(-1)]
        [TestCase(4)]
        public void GivenAnInvalidVertexIndex_WhenSettingItsColour_ThenAnIndexExceptionIsThrown(int vertexIndex)
            => Assert.That(
                () => gameObject.SetVertexColour(vertexIndex, 42),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenVertices_WhenResettingProjectionArrays_ThenTheirLengthsMatchTheActiveVertexCount()
        {
            gameObject.AddVertex(4, 8, 16);
            gameObject.AddVertex(32, 42, 48);

            gameObject.ResetVertexNormals();

            Assert.That(gameObject.ProjectedX, Has.Length.EqualTo(2));
            Assert.That(gameObject.ProjectedY, Has.Length.EqualTo(2));
            Assert.That(gameObject.ProjectedDepth, Has.Length.EqualTo(2));
            Assert.That(gameObject.ProjectedU, Has.Length.EqualTo(2));
            Assert.That(gameObject.ProjectedV, Has.Length.EqualTo(2));
        }

        [Test]
        public void GivenATranslation_WhenUpdatingWorldCoordinates_ThenEachAxisIsOffset()
        {
            GameObject model = BuildSingleVertexObject(4, 8, 16);
            model.SetPosition(32, 42, 48);

            model.UpdateWorldTransformation();

            AssertWorldVertex(model, 36, 50, 64);
            Assert.That(model.ObjectState, Is.Zero);
        }

        [Test]
        public void GivenMultipleTranslationOffsets_WhenUpdatingWorldCoordinates_ThenTheyAccumulate()
        {
            GameObject model = BuildSingleVertexObject(4, 8, 16);
            model.OffsetPosition(32, 42, 48);
            model.OffsetPosition(-4, -8, -16);

            model.UpdateWorldTransformation();

            AssertWorldVertex(model, 32, 42, 48);
        }

        [Test]
        public void GivenASourceTranslation_WhenCopyingIt_ThenTheDestinationUsesTheSameTransform()
        {
            GameObject source = BuildSingleVertexObject(0, 0, 0);
            source.SetPosition(32, 42, 48);
            GameObject destination = BuildSingleVertexObject(4, 8, 16);

            destination.CopyTranslation(source);
            destination.UpdateWorldTransformation();

            AssertWorldVertex(destination, 36, 50, 64);
        }

        [TestCase(64)]
        [TestCase(320)]
        [TestCase(-192)]
        public void GivenAQuarterTurnAroundZ_WhenUpdatingWorldCoordinates_ThenRotationIsMaskedAndApplied(
            int rotationZ)
        {
            GameObject model = BuildSingleVertexObject(128, 0, 0);
            model.SetRotation(0, 0, rotationZ);

            model.UpdateWorldTransformation();

            AssertWorldVertex(model, 0, -128, 0);
        }

        [Test]
        public void GivenATransformedObject_WhenResettingWorldTransform_ThenTheTransformIsBakedIntoLocalCoordinates()
        {
            GameObject model = BuildSingleVertexObject(4, 8, 16);
            model.SetPosition(32, 42, 48);

            model.ResetWorldTransform();
            model.UpdateWorldTransformation();

            Assert.That(model.VertexCoordinatesX[0], Is.EqualTo(36));
            Assert.That(model.VertexCoordinatesY[0], Is.EqualTo(50));
            Assert.That(model.VertexCoordinatesZ[0], Is.EqualTo(64));
            AssertWorldVertex(model, 36, 50, 64);
        }

        [Test]
        public void GivenANullTransformState_WhenUpdatingWorldCoordinates_ThenLocalCoordinatesAndOpenBoundsAreUsed()
        {
            GameObject model = BuildSingleVertexObject(4, 8, 16);
            model.ObjectState = 2;

            model.UpdateWorldTransformation();

            AssertWorldVertex(model, 4, 8, 16);
            Assert.That(model.BoundsMinX, Is.EqualTo(-9999999));
            Assert.That(model.BoundsMinY, Is.EqualTo(-9999999));
            Assert.That(model.BoundsMinZ, Is.EqualTo(-9999999));
            Assert.That(model.BoundsMaxX, Is.EqualTo(9999999));
            Assert.That(model.BoundsMaxY, Is.EqualTo(9999999));
            Assert.That(model.BoundsMaxZ, Is.EqualTo(9999999));
        }

        [Test]
        public void GivenEncodedShadeValues_WhenDecodingThem_ThenTheCompatibleSignedValuesAreReturned()
        {
            Assert.That(new GameObject(0, 0).GetShadeValue(ToSignedBytes("000")), Is.EqualTo(-131072));
            Assert.That(new GameObject(0, 0).GetShadeValue(ToSignedBytes("W00")), Is.Zero);
            Assert.That(new GameObject(0, 0).GetShadeValue(ToSignedBytes("W0g")), Is.EqualTo(42));
            Assert.That(new GameObject(0, 0).GetShadeValue(ToSignedBytes("$$$")), Is.EqualTo(131071));
        }

        [Test]
        public void GivenTheSpecialShadeEncoding_WhenDecodingIt_ThenTheDefaultShadeValueIsReturned()
        {
            sbyte[] buffer = [unchecked((sbyte)163), (sbyte)'9', (sbyte)'0'];

            Assert.That(
                new GameObject(0, 0).GetShadeValue(buffer),
                Is.EqualTo(DefaultShadeValue));
        }

        [Test]
        public void GivenLineBreaksAndConsecutiveValues_WhenDecodingThem_ThenBreaksAreSkippedAndPositionAdvances()
        {
            sbyte[] buffer = [(sbyte)'\n', (sbyte)'\r', .. ToSignedBytes("W0gW00")];
            GameObject model = new(0, 0);

            int firstValue = model.GetShadeValue(buffer);
            int secondValue = model.GetShadeValue(buffer);

            Assert.That(firstValue, Is.EqualTo(42));
            Assert.That(secondValue, Is.Zero);
        }

        [Test]
        public void GivenATruncatedShadeBuffer_WhenDecodingIt_ThenAnIndexExceptionIsThrown()
            => Assert.That(
                () => new GameObject(0, 0).GetShadeValue(ToSignedBytes("W0")),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenANullShadeBuffer_WhenDecodingIt_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => new GameObject(0, 0).GetShadeValue(null!),
                Throws.TypeOf<NullReferenceException>());

        private static void AssertWorldVertex(
            GameObject model,
            int expectedPositionX,
            int expectedPositionY,
            int expectedPositionZ)
        {
            Assert.That(model.WorldVertX[0], Is.EqualTo(expectedPositionX));
            Assert.That(model.WorldVertY[0], Is.EqualTo(expectedPositionY));
            Assert.That(model.WorldVertZ[0], Is.EqualTo(expectedPositionZ));
        }

        private static GameObject BuildSingleVertexObject(int positionX, int positionY, int positionZ)
        {
            GameObject model = new(1, 0);
            model.AddVertex(positionX, positionY, positionZ);

            return model;
        }

        private static void FillVertexCapacity(GameObject model)
        {
            for (int vertexIndex = 0; vertexIndex < model.TotalVertexCapacity; vertexIndex += 1)
            {
                model.AddVertex(vertexIndex, vertexIndex * 2, vertexIndex * 3);
            }
        }

        private static sbyte[] ToSignedBytes(string text)
        {
            sbyte[] bytes = new sbyte[text.Length];

            for (int characterIndex = 0; characterIndex < text.Length; characterIndex += 1)
            {
                bytes[characterIndex] = (sbyte)text[characterIndex];
            }

            return bytes;
        }
    }
}