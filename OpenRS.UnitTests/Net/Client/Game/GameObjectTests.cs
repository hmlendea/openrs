using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    [NonParallelizable]
    public sealed class GameObjectTests
    {
        private GameObject gameObject = null!;

        private static int VertexCapacity => 4;

        private static int FaceCapacity => 8;

        private static int DefaultShadeValue => 0xbc614e;

        private static int CameraNearBound => -1024;

        private static int CameraFarBound => 1024;

        [SetUp]
        public void SetUp()
        {
            Camera.NearX = CameraNearBound;
            Camera.FarX = CameraFarBound;
            Camera.NearY = CameraNearBound;
            Camera.FarY = CameraFarBound;
            Camera.NearZ = CameraNearBound;
            Camera.FarZ = CameraFarBound;
            gameObject = new GameObject(VertexCapacity, FaceCapacity);
        }

        [TearDown]
        public void TearDown()
        {
            Camera.NearX = 0;
            Camera.FarX = 0;
            Camera.NearY = 0;
            Camera.FarY = 0;
            Camera.NearZ = 0;
            Camera.FarZ = 0;
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
        public void GivenLightingValues_WhenUpdatingShading_ThenShadeLevelsAndDirectionAreUpdated()
        {
            gameObject.UpdateShading(false, 42, 48, 32, 42, 48);

            Assert.That(gameObject.BaseShadeLevel, Is.EqualTo(88));
            Assert.That(gameObject.AmbientLightLevel, Is.EqualTo(384));
            Assert.That(gameObject.LightDirectionX, Is.EqualTo(32));
            Assert.That(gameObject.LightDirectionY, Is.EqualTo(42));
            Assert.That(gameObject.LightDirectionZ, Is.EqualTo(48));
            Assert.That(gameObject.LightMagnitude, Is.EqualTo(71));
        }

        [TestCase(false, 0)]
        [TestCase(true, 0xbc614e)]
        public void GivenAFace_WhenUpdatingShading_ThenItsGouraudShadeMatchesTheRequestedMode(
            bool applyShadeValue,
            int expectedShade)
        {
            GameObject model = BuildTriangleObject();

            model.UpdateShading(applyShadeValue, 42, 48, 32, 42, 48);

            Assert.That(model.GouraudShade[0], Is.EqualTo(expectedShade));
        }

        [Test]
        public void GivenAShadowlessObject_WhenUpdatingShading_ThenOnlyShadeLevelsAreUpdated()
        {
            GameObject model = new(0, 0, false, false, true, false, false);

            model.UpdateShading(true, 42, 48, 32, 42, 48);

            Assert.That(model.BaseShadeLevel, Is.EqualTo(88));
            Assert.That(model.AmbientLightLevel, Is.EqualTo(384));
            Assert.That(model.LightDirectionX, Is.EqualTo(180));
            Assert.That(model.LightDirectionY, Is.EqualTo(155));
            Assert.That(model.LightDirectionZ, Is.EqualTo(95));
            Assert.That(model.LightMagnitude, Is.EqualTo(256));
        }

        [Test]
        public void GivenLightingValues_WhenSettingModelColours_ThenShadeLevelsAndDirectionAreUpdated()
        {
            gameObject.SetModelColours(42, 48, 32, 42, 48);

            Assert.That(gameObject.BaseShadeLevel, Is.EqualTo(88));
            Assert.That(gameObject.AmbientLightLevel, Is.EqualTo(384));
            Assert.That(gameObject.LightDirectionX, Is.EqualTo(32));
            Assert.That(gameObject.LightDirectionY, Is.EqualTo(42));
            Assert.That(gameObject.LightDirectionZ, Is.EqualTo(48));
            Assert.That(gameObject.LightMagnitude, Is.EqualTo(71));
        }

        [Test]
        public void GivenDirectionValues_WhenOffsettingModelColours_ThenOnlyDirectionAndMagnitudeAreUpdated()
        {
            int expectedBaseShadeLevel = gameObject.BaseShadeLevel;
            int expectedAmbientLightLevel = gameObject.AmbientLightLevel;

            gameObject.OffsetModelColours(32, 42, 48);

            Assert.That(gameObject.BaseShadeLevel, Is.EqualTo(expectedBaseShadeLevel));
            Assert.That(gameObject.AmbientLightLevel, Is.EqualTo(expectedAmbientLightLevel));
            Assert.That(gameObject.LightDirectionX, Is.EqualTo(32));
            Assert.That(gameObject.LightDirectionY, Is.EqualTo(42));
            Assert.That(gameObject.LightDirectionZ, Is.EqualTo(48));
            Assert.That(gameObject.LightMagnitude, Is.EqualTo(71));
        }

        [Test]
        public void GivenPolygonData_WhenCopyingItToAnotherObject_ThenGeometryAndMetadataAreCopied()
        {
            GameObject source = BuildTriangleObject();
            source.FaceNormalComponent[0] = 4;
            source.FaceNormalComponent[1] = 8;
            source.FaceNormalComponent[2] = 16;
            source.VertexColour[0] = 32;
            source.VertexColour[1] = 42;
            source.VertexColour[2] = 48;
            source.EntityType[0] = 64;
            source.GouraudShade[0] = 96;
            source.FaceRenderFlag[0] = 128;
            source.FaceVisibility[0] = 256;
            GameObject destination = new(3, 1);

            source.CopyModelData(destination, [0, 1, 2], 3, 0);

            Assert.That(destination.VertexCount, Is.EqualTo(3));
            Assert.That(destination.FaceCount, Is.EqualTo(1));
            Assert.That(destination.VertexCoordinatesX[..3], Is.EqualTo(new[] { 0, 32, 0 }));
            Assert.That(destination.VertexCoordinatesY[..3], Is.EqualTo(new[] { 0, 0, 32 }));
            Assert.That(destination.VertexCoordinatesZ[..3], Is.EqualTo(new[] { 0, 0, 0 }));
            Assert.That(destination.FaceNormalComponent[..3], Is.EqualTo(new[] { 4, 8, 16 }));
            Assert.That(destination.VertexColour[..3], Is.EqualTo(new[] { 32, 42, 48 }));
            Assert.That(destination.FaceVertexIndices[0], Is.EqualTo(new[] { 0, 1, 2 }));
            Assert.That(destination.TextureBack[0], Is.EqualTo(42));
            Assert.That(destination.TextureFront[0], Is.EqualTo(48));
            Assert.That(destination.EntityType[0], Is.EqualTo(64));
            Assert.That(destination.GouraudShade[0], Is.EqualTo(96));
            Assert.That(destination.FaceRenderFlag[0], Is.EqualTo(128));
            Assert.That(destination.FaceVisibility[0], Is.EqualTo(256));
        }

        [Test]
        public void GivenDuplicatePolygonVertices_WhenCopyingThem_ThenDestinationVerticesAreReused()
        {
            GameObject source = BuildTriangleObject();
            GameObject destination = new(3, 2);

            source.CopyModelData(destination, [0, 1, 2], 3, 0);
            source.CopyModelData(destination, [2, 1, 0], 3, 0);

            Assert.That(destination.VertexCount, Is.EqualTo(3));
            Assert.That(destination.FaceCount, Is.EqualTo(2));
            Assert.That(destination.FaceVertexIndices[0], Is.EqualTo(new[] { 0, 1, 2 }));
            Assert.That(destination.FaceVertexIndices[1], Is.EqualTo(new[] { 2, 1, 0 }));
        }

        [Test]
        public void GivenChildObjects_WhenBuildingAComposite_ThenGeometryAndLightingAreCombined()
        {
            GameObject firstChild = BuildTriangleObject();
            firstChild.UpdateWorldTransformation();
            firstChild.SetModelColours(42, 48, 32, 42, 48);
            firstChild.GouraudShade[0] = 64;
            firstChild.FaceRenderFlag[0] = 96;
            firstChild.FaceVisibility[0] = 128;
            GameObject secondChild = BuildTriangleObject();
            secondChild.SetPosition(32, 0, 0);

            GameObject composite = new([firstChild, secondChild], 2);

            Assert.That(composite.VertexCount, Is.EqualTo(5));
            Assert.That(composite.FaceCount, Is.EqualTo(2));
            Assert.That(composite.GouraudShade[0], Is.EqualTo(64));
            Assert.That(composite.FaceRenderFlag[0], Is.EqualTo(96));
            Assert.That(composite.FaceVisibility[0], Is.EqualTo(128));
            Assert.That(composite.BaseShadeLevel, Is.EqualTo(secondChild.BaseShadeLevel));
            Assert.That(composite.AmbientLightLevel, Is.EqualTo(secondChild.AmbientLightLevel));
            Assert.That(composite.ObjectState, Is.EqualTo(1));
        }

        [Test]
        public void GivenMoreChildrenThanTheRequestedCount_WhenBuildingAComposite_ThenOnlyThePrefixIsUsed()
        {
            GameObject firstChild = BuildTriangleObject();
            GameObject ignoredChild = BuildTriangleObject();

            GameObject composite = new([firstChild, ignoredChild], 1);

            Assert.That(composite.VertexCount, Is.EqualTo(3));
            Assert.That(composite.FaceCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenExistingTargetData_WhenBuildingAComposite_ThenTheTargetIsReinitialised()
        {
            GameObject target = BuildSingleVertexObject(4, 8, 16);
            GameObject child = BuildTriangleObject();

            target.BuildGameObject([child], 1, true);

            Assert.That(target.VertexCount, Is.EqualTo(3));
            Assert.That(target.FaceCount, Is.EqualTo(1));
            Assert.That(target.TotalVertexCapacity, Is.EqualTo(3));
            Assert.That(target.VertexCoordinatesX[..3], Is.EqualTo(new[] { 0, 32, 0 }));
        }

        [Test]
        public void GivenAnObject_WhenCreatingAParent_ThenGeometryAndDisplayStateAreCopied()
        {
            GameObject child = BuildTriangleObject();
            child.ScaleBias = 42;
            child.IsGiantCrystal = true;

            GameObject parent = child.CreateParent();

            Assert.That(parent.VertexCount, Is.EqualTo(3));
            Assert.That(parent.FaceCount, Is.EqualTo(1));
            Assert.That(parent.ScaleBias, Is.EqualTo(42));
            Assert.That(parent.IsGiantCrystal);
            Assert.That(parent.FaceVertexIndices[0], Is.EqualTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void GivenSharingOptions_WhenCreatingAParent_ThenTheRequestedStorageContractIsUsed()
        {
            GameObject child = BuildTriangleObject();
            child.ScaleBias = 42;

            GameObject parent = child.CreateParent(true, true, true, true);

            Assert.That(parent.ScaleBias, Is.EqualTo(42));
            Assert.That(parent.DoesShareWorldVertices);
            Assert.That(parent.HasNoCollider);
            Assert.That(parent.DoesNotReceiveShadows);
            Assert.That(parent.DoesShareEntityArrays);
            Assert.That(parent.WorldVertX, Is.SameAs(parent.VertexCoordinatesX));
            Assert.That(parent.EntityType, Is.Null);
            Assert.That(parent.PolygonTypeData, Is.Null);
            Assert.That(parent.NormalX, Is.Null);
            Assert.That(parent.NormalY, Is.Null);
            Assert.That(parent.NormalZ, Is.Null);
        }

        [Test]
        public void GivenFacesInDifferentAreas_WhenSplittingTheObject_ThenEachFaceUsesItsCalculatedChunk()
        {
            GameObject source = BuildTwoAreaObject();

            GameObject[] chunks = source.GetObjectsWithinArea(4, 8, 32, 32, 2, 2, 8, false);

            Assert.That(chunks, Has.Length.EqualTo(2));
            Assert.That(chunks[0].VertexCount, Is.EqualTo(3));
            Assert.That(chunks[0].FaceCount, Is.EqualTo(1));
            Assert.That(chunks[0].VertexCoordinatesX[..3], Is.EqualTo(new[] { 0, 16, 0 }));
            Assert.That(chunks[1].VertexCount, Is.EqualTo(3));
            Assert.That(chunks[1].FaceCount, Is.EqualTo(1));
            Assert.That(chunks[1].VertexCoordinatesX[..3], Is.EqualTo(new[] { 32, 48, 32 }));
        }

        [Test]
        public void GivenEntityMetadata_WhenSplittingWithoutLighting_ThenEntityMetadataIsCopied()
        {
            GameObject source = BuildTwoAreaObject();
            source.EntityType[0] = 42;
            source.EntityType[1] = 64;

            GameObject[] chunks = source.GetObjectsWithinArea(4, 8, 32, 32, 2, 2, 8, false);

            Assert.That(chunks[0].DoesShareEntityArrays, Is.False);
            Assert.That(chunks[1].DoesShareEntityArrays, Is.False);
            Assert.That(chunks[0].EntityType[0], Is.EqualTo(42));
            Assert.That(chunks[1].EntityType[0], Is.EqualTo(64));
        }

        [Test]
        public void GivenEntityMetadata_WhenSplittingWithLighting_ThenEntityArraysAreNotAllocated()
        {
            GameObject source = BuildTwoAreaObject();

            GameObject[] chunks = source.GetObjectsWithinArea(4, 8, 32, 32, 2, 2, 8, true);

            Assert.That(chunks[0].DoesShareEntityArrays);
            Assert.That(chunks[1].DoesShareEntityArrays);
            Assert.That(chunks[0].EntityType, Is.Null);
            Assert.That(chunks[1].EntityType, Is.Null);
            Assert.That(chunks[0].PolygonTypeData, Is.Null);
            Assert.That(chunks[1].PolygonTypeData, Is.Null);
        }

        [Test]
        public void GivenATranslatedObject_WhenSplittingIt_ThenTheTransformIsBakedIntoChunkVertices()
        {
            GameObject source = BuildTriangleObject();
            source.SetPosition(32, 42, 0);

            GameObject[] chunks = source.GetObjectsWithinArea(4, 8, 64, 64, 1, 1, 8, false);

            Assert.That(chunks[0].VertexCoordinatesX[..3], Is.EqualTo(new[] { 32, 64, 32 }));
            Assert.That(chunks[0].VertexCoordinatesY[..3], Is.EqualTo(new[] { 42, 42, 74 }));
            Assert.That(source.VertexCoordinatesX[..3], Is.EqualTo(new[] { 32, 64, 32 }));
            Assert.That(source.VertexCoordinatesY[..3], Is.EqualTo(new[] { 42, 42, 74 }));
        }

        [Test]
        public void GivenNoFaces_WhenSplittingTheObject_ThenEmptyChunksAreReturned()
        {
            GameObject source = new(0, 0);

            GameObject[] chunks = source.GetObjectsWithinArea(4, 8, 32, 32, 2, 4, 8, false);

            Assert.That(chunks, Has.Length.EqualTo(4));
            Assert.That(chunks.Select(chunk => chunk.VertexCount), Has.All.Zero);
            Assert.That(chunks.Select(chunk => chunk.FaceCount), Has.All.Zero);
        }

        [Test]
        public void GivenRepeatedFaceVerticesBeyondTheMaximum_WhenSplittingTheObject_ThenCapacityIsClamped()
        {
            GameObject source = new(1, 1);
            source.AddVertex(4, 8, 16);
            source.AddFaceVertices(3, [0, 0, 0], 42, 48);

            GameObject[] chunks = source.GetObjectsWithinArea(4, 8, 32, 32, 1, 1, 1, false);

            Assert.That(chunks[0].TotalVertexCapacity, Is.EqualTo(1));
            Assert.That(chunks[0].VertexCount, Is.EqualTo(1));
            Assert.That(chunks[0].FaceCount, Is.EqualTo(1));
            Assert.That(chunks[0].FaceVertexIndices[0], Is.EqualTo(new[] { 0, 0, 0 }));
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
        public void GivenATransformedTriangle_WhenUpdatingWorldCoordinates_ThenBoundsAndSpanAreCalculated()
        {
            GameObject model = BuildTriangleObject();
            model.SetPosition(-4, 8, 16);

            model.UpdateWorldTransformation();

            Assert.That(model.BoundsMinX, Is.EqualTo(-4));
            Assert.That(model.BoundsMaxX, Is.EqualTo(28));
            Assert.That(model.BoundsMinY, Is.EqualTo(8));
            Assert.That(model.BoundsMaxY, Is.EqualTo(40));
            Assert.That(model.BoundsMinZ, Is.EqualTo(16));
            Assert.That(model.BoundsMaxZ, Is.EqualTo(16));
            Assert.That(model.MaximumFaceSpan, Is.EqualTo(32));
        }

        [Test]
        public void GivenATriangle_WhenCalculatingNormals_ThenItsNormalAndRenderStateAreUpdated()
        {
            GameObject model = BuildTriangleObject();
            model.UpdateWorldTransformation();

            model.CalculateNormals();

            Assert.That(model.NormalX[0], Is.Zero);
            Assert.That(model.NormalY[0], Is.Zero);
            Assert.That(model.NormalZ[0], Is.EqualTo(255));
            Assert.That(model.FaceRenderFlag[0], Is.EqualTo(-1));
            Assert.That(model.GouraudShade[0], Is.EqualTo(47));
        }

        [Test]
        public void GivenADegenerateFace_WhenCalculatingNormals_ThenItsNormalComponentsAreZero()
        {
            GameObject model = new(3, 1);
            model.AddVertex(4, 8, 16);
            model.AddVertex(4, 8, 16);
            model.AddVertex(4, 8, 16);
            model.AddFaceVertices(3, [0, 1, 2], 42, 48);
            model.UpdateWorldTransformation();

            model.CalculateNormals();

            Assert.That(model.NormalX[0], Is.Zero);
            Assert.That(model.NormalY[0], Is.Zero);
            Assert.That(model.NormalZ[0], Is.Zero);
            Assert.That(model.FaceRenderFlag[0], Is.EqualTo(-1));
        }

        [Test]
        public void GivenAShadowlessCollisionlessObject_WhenCalculatingNormals_ThenNoNormalStorageIsRequired()
        {
            GameObject model = new(3, 1, false, true, true, false, false);

            Assert.That(() => model.CalculateNormals(), Throws.Nothing);
            Assert.That(model.NormalX, Is.Null);
            Assert.That(model.NormalY, Is.Null);
            Assert.That(model.NormalZ, Is.Null);
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
        public void GivenAVisibleTriangle_WhenProjectingIt_ThenPerspectiveCoordinatesAreCalculated()
        {
            GameObject model = BuildTriangleObject(128);

            model.ProjectWithRotation(0, 0, 0, 0, 0, 0, 8, 5);

            Assert.That(model.IsVisible);
            Assert.That(model.ProjectedX[..3], Is.EqualTo(new[] { 0, 32, 0 }));
            Assert.That(model.ProjectedY[..3], Is.EqualTo(new[] { 0, 0, 32 }));
            Assert.That(model.ProjectedDepth[..3], Is.EqualTo(new[] { 128, 128, 128 }));
            Assert.That(model.ProjectedU[..3], Is.EqualTo(new[] { 0, 64, 0 }));
            Assert.That(model.ProjectedV[..3], Is.EqualTo(new[] { 0, 0, 64 }));
        }

        [Test]
        public void GivenVerticesBeforeTheNearPlane_WhenProjectingThem_ThenUnscaledDepthDivisionIsSkipped()
        {
            GameObject model = BuildTriangleObject(4);

            model.ProjectWithRotation(0, 0, 0, 0, 0, 0, 8, 5);

            Assert.That(model.ProjectedDepth[..3], Is.EqualTo(new[] { 4, 4, 4 }));
            Assert.That(model.ProjectedU[..3], Is.EqualTo(new[] { 0, 8192, 0 }));
            Assert.That(model.ProjectedV[..3], Is.EqualTo(new[] { 0, 0, 8192 }));
        }

        [Test]
        public void GivenAProjectionOrigin_WhenProjectingVertices_ThenTheOriginIsSubtracted()
        {
            GameObject model = BuildTriangleObject(48);

            model.ProjectWithRotation(4, 8, 16, 0, 0, 0, 8, 5);

            Assert.That(model.ProjectedX[..3], Is.EqualTo(new[] { -4, 28, -4 }));
            Assert.That(model.ProjectedY[..3], Is.EqualTo(new[] { -8, -8, 24 }));
            Assert.That(model.ProjectedDepth[..3], Is.EqualTo(new[] { 32, 32, 32 }));
        }

        [TestCase(0, 0, 256, 0, -32, 128)]
        [TestCase(0, 256, 0, 128, 0, -32)]
        [TestCase(256, 0, 0, 32, -128, 0)]
        public void GivenAQuarterCameraRotation_WhenProjectingAVertex_ThenTheSelectedAxisIsApplied(
            int rotationX,
            int rotationY,
            int rotationZ,
            int expectedPositionX,
            int expectedPositionY,
            int expectedDepth)
        {
            GameObject model = BuildTriangleObject(128);

            model.ProjectWithRotation(0, 0, 0, rotationX, rotationY, rotationZ, 8, 5);

            Assert.That(model.ProjectedX[1], Is.EqualTo(expectedPositionX));
            Assert.That(model.ProjectedY[1], Is.EqualTo(expectedPositionY));
            Assert.That(model.ProjectedDepth[1], Is.EqualTo(expectedDepth));
        }

        [TestCase(-1, 0, 0)]
        [TestCase(0, -1, 0)]
        [TestCase(0, 0, -1)]
        [TestCase(1024, 0, 0)]
        [TestCase(0, 1024, 0)]
        [TestCase(0, 0, 1024)]
        public void GivenAnInvalidCameraRotation_WhenProjectingVertices_ThenAnIndexExceptionIsThrown(
            int rotationX,
            int rotationY,
            int rotationZ)
        {
            GameObject model = BuildTriangleObject(128);

            Assert.That(
                () => model.ProjectWithRotation(0, 0, 0, rotationX, rotationY, rotationZ, 8, 5),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(2048, 4096, -32, 32, -32, 32)]
        [TestCase(-4096, -2048, -32, 32, -32, 32)]
        [TestCase(-32, 32, 2048, 4096, -32, 32)]
        [TestCase(-32, 32, -4096, -2048, -32, 32)]
        [TestCase(-32, 32, -32, 32, 2048, 4096)]
        [TestCase(-32, 32, -32, 32, -4096, -2048)]
        public void GivenBoundsOutsideOneFrustumPlane_WhenProjectingTheObject_ThenItIsHidden(
            int minimumX,
            int maximumX,
            int minimumY,
            int maximumY,
            int minimumZ,
            int maximumZ)
        {
            GameObject model = new(0, 0)
            {
                ObjectState = 0,
                BoundsMinX = minimumX,
                BoundsMaxX = maximumX,
                BoundsMinY = minimumY,
                BoundsMaxY = maximumY,
                BoundsMinZ = minimumZ,
                BoundsMaxZ = maximumZ,
            };

            model.ProjectWithRotation(0, 0, 0, 0, 0, 0, 8, 5);

            Assert.That(model.IsVisible, Is.False);
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

        private static GameObject BuildTriangleObject() => BuildTriangleObject(0);

        private static GameObject BuildTriangleObject(int depth)
        {
            GameObject model = new(3, 1);
            model.AddVertex(0, 0, depth);
            model.AddVertex(32, 0, depth);
            model.AddVertex(0, 32, depth);
            model.AddFaceVertices(3, [0, 1, 2], 42, 48);

            return model;
        }

        private static GameObject BuildTwoAreaObject()
        {
            GameObject model = new(6, 2);
            model.AddVertex(0, 0, 0);
            model.AddVertex(16, 0, 0);
            model.AddVertex(0, 16, 0);
            model.AddVertex(32, 0, 0);
            model.AddVertex(48, 0, 0);
            model.AddVertex(32, 16, 0);
            model.AddFaceVertices(3, [0, 1, 2], 42, 48);
            model.AddFaceVertices(3, [3, 4, 5], 64, 96);

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