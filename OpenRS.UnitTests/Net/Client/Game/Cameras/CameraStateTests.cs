using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    [NonParallelizable]
    public sealed class CameraStateTests
    {
        private static int ModelCapacity => 2;

        private static int VisibleModelCapacity => 8;

        private static int SceneObjectCapacity => 4;

        private Camera camera = null!;

        [SetUp]
        public void SetUp()
        {
            Camera.NearX = 0;
            Camera.FarX = 0;
            Camera.NearY = 0;
            Camera.FarY = 0;
            Camera.NearZ = 0;
            Camera.FarZ = 0;
            camera = new Camera(
                new GameImage(128, 96, 0),
                ModelCapacity,
                VisibleModelCapacity,
                SceneObjectCapacity);
        }

        [Test]
        public void GivenANewCamera_WhenReadingItsState_ThenCompatibleDefaultsAreInitialised()
        {
            Assert.That(camera.NearPlane, Is.EqualTo(5));
            Assert.That(camera.FarClipDistance, Is.EqualTo(1000));
            Assert.That(camera.SpriteFarClipDistance, Is.EqualTo(1000));
            Assert.That(camera.FogGradientStep, Is.EqualTo(20));
            Assert.That(camera.FogStartDistance, Is.EqualTo(10));
            Assert.That(camera.ScaleFactor, Is.EqualTo(1.1D));
            Assert.That(camera.DepthSortStride, Is.EqualTo(1));
            Assert.That(camera.SavedModelIndex, Is.Zero);
            Assert.That(camera.IsInterlaced, Is.False);
            Assert.That(camera.HighlightedObject, Is.Not.Null);
            Assert.That(camera.HighlightedObject.TotalVertexCapacity, Is.EqualTo(8));
            Assert.That(camera.GetHighlightedPlayers(), Has.Length.EqualTo(100));
            Assert.That(camera.GetHighlightedObjects(), Has.Length.EqualTo(100));
            Assert.That(camera.GetOptionCount(), Is.Zero);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GivenAnInterlacingState_WhenSettingIt_ThenTheRendererStateIsUpdated(bool isInterlaced)
        {
            camera.IsInterlaced = isInterlaced;

            Assert.That(camera.IsInterlaced, Is.EqualTo(isInterlaced));
        }

        [Test]
        public void GivenModelsWithinCapacity_WhenOffsettingColours_ThenEveryAddedModelIsUpdated()
        {
            GameObject firstModel = new(0, 0);
            GameObject secondModel = new(0, 0);
            camera.AddModel(firstModel);
            camera.AddModel(secondModel);

            camera.OffsetAllModelColours(4, 8, 16);

            AssertLightDirection(firstModel, 4, 8, 16);
            AssertLightDirection(secondModel, 4, 8, 16);
        }

        [Test]
        public void GivenAModelBeyondCapacity_WhenOffsettingColours_ThenTheExtraModelIsIgnored()
        {
            GameObject firstModel = new(0, 0);
            GameObject secondModel = new(0, 0);
            GameObject ignoredModel = new(0, 0);
            camera.AddModel(firstModel);
            camera.AddModel(secondModel);
            camera.AddModel(ignoredModel);

            camera.OffsetAllModelColours(4, 8, 16);

            AssertLightDirection(firstModel, 4, 8, 16);
            AssertLightDirection(secondModel, 4, 8, 16);
            AssertLightDirection(ignoredModel, 180, 155, 95);
        }

        [Test]
        public void GivenANullModel_WhenAddingAndOffsettingColours_ThenNoExceptionIsThrown()
        {
            camera.AddModel(null!);

            Assert.That(
                () => camera.OffsetAllModelColours(4, 8, 16),
                Throws.Nothing);
        }

        [Test]
        public void GivenAnAddedModel_WhenRemovingAndOffsettingColours_ThenItIsNotUpdated()
        {
            GameObject removedModel = new(0, 0);
            GameObject retainedModel = new(0, 0);
            camera.AddModel(removedModel);
            camera.AddModel(retainedModel);

            camera.RemoveModel(removedModel);
            camera.OffsetAllModelColours(4, 8, 16);

            AssertLightDirection(removedModel, 180, 155, 95);
            AssertLightDirection(retainedModel, 4, 8, 16);
        }

        [Test]
        public void GivenAnUnregisteredModel_WhenRemovingIt_ThenRegisteredModelsRemainActive()
        {
            GameObject registeredModel = new(0, 0);
            camera.AddModel(registeredModel);

            camera.RemoveModel(new GameObject(0, 0));
            camera.OffsetAllModelColours(4, 8, 16);

            AssertLightDirection(registeredModel, 4, 8, 16);
        }

        [Test]
        public void GivenAddedModels_WhenCleaningUpAndOffsettingColours_ThenNoModelIsUpdated()
        {
            GameObject firstModel = new(0, 0);
            GameObject secondModel = new(0, 0);
            camera.AddModel(firstModel);
            camera.AddModel(secondModel);

            camera.CleanUp();
            camera.OffsetAllModelColours(4, 8, 16);

            AssertLightDirection(firstModel, 180, 155, 95);
            AssertLightDirection(secondModel, 180, 155, 95);
        }

        [Test]
        public void GivenZeroColourOffsets_WhenOffsettingModels_ThenTheCompatibleXFallbackIsApplied()
        {
            GameObject model = new(0, 0);
            camera.AddModel(model);

            camera.OffsetAllModelColours(0, 0, 0);

            AssertLightDirection(model, 32, 0, 0);
            Assert.That(model.LightMagnitude, Is.EqualTo(32));
        }

        [Test]
        public void GivenLightingValues_WhenSettingAllModelColours_ThenShadeAndDirectionStateAreUpdated()
        {
            GameObject model = new(0, 0);
            camera.AddModel(model);

            camera.SetAllModelColours(42, 48, 4, 8, 16);

            Assert.That(model.BaseShadeLevel, Is.EqualTo(88));
            Assert.That(model.AmbientLightLevel, Is.EqualTo(384));
            AssertLightDirection(model, 4, 8, 16);
            Assert.That(model.LightMagnitude, Is.EqualTo(18));
        }

        [Test]
        public void GivenZeroLightingDirection_WhenSettingAllModelColours_ThenTheCompatibleXFallbackIsApplied()
        {
            GameObject model = new(0, 0);
            camera.AddModel(model);

            camera.SetAllModelColours(42, 48, 0, 0, 0);

            AssertLightDirection(model, 32, 0, 0);
        }

        [Test]
        public void GivenViewPoints_WhenExpandingBounds_ThenEveryMinimumAndMaximumIsUpdated()
        {
            camera.SetViewAngle(-42, 64, -96);
            camera.SetViewAngle(128, -8, 256);

            Assert.That(Camera.NearX, Is.EqualTo(-42));
            Assert.That(Camera.FarX, Is.EqualTo(128));
            Assert.That(Camera.NearY, Is.EqualTo(-8));
            Assert.That(Camera.FarY, Is.EqualTo(64));
            Assert.That(Camera.NearZ, Is.EqualTo(-96));
            Assert.That(Camera.FarZ, Is.EqualTo(256));
        }

        [Test]
        public void GivenViewPointsWithinExistingBounds_WhenExpandingBounds_ThenExistingExtremesRemain()
        {
            Camera.NearX = -128;
            Camera.FarX = 128;
            Camera.NearY = -128;
            Camera.FarY = 128;
            Camera.NearZ = -128;
            Camera.FarZ = 128;

            camera.SetViewAngle(4, 8, 16);

            Assert.That(Camera.NearX, Is.EqualTo(-128));
            Assert.That(Camera.FarX, Is.EqualTo(128));
            Assert.That(Camera.NearY, Is.EqualTo(-128));
            Assert.That(Camera.FarY, Is.EqualTo(128));
            Assert.That(Camera.NearZ, Is.EqualTo(-128));
            Assert.That(Camera.FarZ, Is.EqualTo(128));
        }

        [Test]
        public void GivenSceneSpriteData_WhenAddingAndUpdatingIt_ThenFacadeStateIsUpdated()
        {
            int spriteIndex = camera.AddSpriteToScene(42, 4, 8, 16, 32, 48, 64);

            camera.UpdateSpritePosition(spriteIndex, 96);

            Assert.That(spriteIndex, Is.Zero);
            Assert.That(camera.HighlightedObject.VertexCount, Is.EqualTo(2));
            Assert.That(camera.HighlightedObject.FaceCount, Is.EqualTo(1));
            Assert.That(camera.HighlightedObject.EntityType[0], Is.EqualTo(64));
        }

        [Test]
        public void GivenASceneSprite_WhenRemovingIt_ThenItsPolygonIsMarkedAsRemoved()
        {
            int spriteIndex = camera.AddSpriteToScene(42, 4, 8, 16, 32, 48, 64);

            camera.RemoveSprite(spriteIndex);

            Assert.That(camera.HighlightedObject.PolygonTypeData[spriteIndex], Is.EqualTo(1));
        }

        [Test]
        public void GivenSceneSprites_WhenInitialisingTheScene_ThenGeometryIndicesReset()
        {
            camera.AddSpriteToScene(42, 4, 8, 16, 32, 48, 64);

            camera.InitializeScene();

            Assert.That(camera.HighlightedObject.VertexCount, Is.Zero);
            Assert.That(camera.HighlightedObject.FaceCount, Is.Zero);
        }

        [Test]
        public void GivenSceneSprites_WhenRemovingLastUpdates_ThenGeometryCountsDecrease()
        {
            camera.AddSpriteToScene(42, 4, 8, 16, 32, 48, 64);
            camera.AddSpriteToScene(96, 4, 8, 16, 32, 48, 128);

            camera.RemoveLastUpdates(1);

            Assert.That(camera.HighlightedObject.VertexCount, Is.EqualTo(2));
            Assert.That(camera.HighlightedObject.FaceCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenCameraSizeValues_WhenInitialisingRasterState_ThenNoGraphicsDeviceIsRequired()
            => Assert.That(
                () => camera.SetCameraSize(42, 64, 96, 128, 256, 8),
                Throws.Nothing);

        [Test]
        public void GivenExpandedStaticBounds_WhenConstructingAnotherCamera_ThenTheBoundsRemainShared()
        {
            camera.SetViewAngle(-42, 64, -96);

            _ = new Camera(new GameImage(128, 96, 0), 1, 1, 1);

            Assert.That(Camera.NearX, Is.EqualTo(-42));
            Assert.That(Camera.FarX, Is.Zero);
            Assert.That(Camera.NearY, Is.Zero);
            Assert.That(Camera.FarY, Is.EqualTo(64));
            Assert.That(Camera.NearZ, Is.EqualTo(-96));
            Assert.That(Camera.FarZ, Is.Zero);
        }

        [Test]
        public void GivenExtremeViewCoordinates_WhenExpandingBounds_ThenTheExtremeValuesAreRetained()
        {
            camera.SetViewAngle(int.MinValue, int.MaxValue, int.MinValue);

            Assert.That(Camera.NearX, Is.EqualTo(int.MinValue));
            Assert.That(Camera.FarX, Is.Zero);
            Assert.That(Camera.NearY, Is.Zero);
            Assert.That(Camera.FarY, Is.EqualTo(int.MaxValue));
            Assert.That(Camera.NearZ, Is.EqualTo(int.MinValue));
            Assert.That(Camera.FarZ, Is.Zero);
        }

        private static void AssertLightDirection(
            GameObject model,
            int expectedDirectionX,
            int expectedDirectionY,
            int expectedDirectionZ)
        {
            Assert.That(model.LightDirectionX, Is.EqualTo(expectedDirectionX));
            Assert.That(model.LightDirectionY, Is.EqualTo(expectedDirectionY));
            Assert.That(model.LightDirectionZ, Is.EqualTo(expectedDirectionZ));
        }
    }
}