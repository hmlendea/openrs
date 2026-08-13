using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    [NonParallelizable]
    public sealed class CameraRenderingTests
    {
        private GameImage image = null!;
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
            image = new GameImage(128, 96, 1);
            camera = new Camera(image, 4, 16, 4);
            camera.SetCameraSize(64, 48, 64, 48, 128, 8);
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
        public void GivenAVisibleColouredTriangle_WhenFinishingTheCamera_ThenItIsCollectedAndPainted()
        {
            GameObject model = BuildTriangle(128, Camera.GetTextureColour(255, 0, 0));
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenNoModels_WhenFinishingTheCamera_ThenProjectionBoundsArePreparedWithoutPainting()
        {
            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.Zero);
            Assert.That(image.Pixels, Has.All.Zero);
            Assert.That(Camera.NearX, Is.EqualTo(-250));
            Assert.That(Camera.FarX, Is.EqualTo(250));
            Assert.That(Camera.NearY, Is.EqualTo(-187));
            Assert.That(Camera.FarY, Is.EqualTo(187));
            Assert.That(Camera.NearZ, Is.Zero);
            Assert.That(Camera.FarZ, Is.EqualTo(1000));
        }

        [TestCase(5)]
        [TestCase(1000)]
        public void GivenATriangleAtAnExclusiveDepthBoundary_WhenFinishingTheCamera_ThenItIsNotCollected(
            int depth)
        {
            camera.AddModel(BuildTriangle(depth, Camera.GetTextureColour(255, 0, 0)));

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.Zero);
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [TestCase(64, 0)]
        [TestCase(0, 48)]
        public void GivenATriangleOutsideOneScreenAxis_WhenFinishingTheCamera_ThenItIsNotCollected(
            int offsetX,
            int offsetY)
        {
            GameObject model = BuildTriangle(128, Camera.GetTextureColour(255, 0, 0));

            for (int vertexIndex = 0; vertexIndex < model.VertexCount; vertexIndex += 1)
            {
                model.VertexCoordinatesX[vertexIndex] += offsetX;
                model.VertexCoordinatesY[vertexIndex] += offsetY;
            }

            camera.AddModel(model);
            camera.FinishCamera();

            Assert.That(model.IsVisible);
            Assert.That(camera.SavedModelIndex, Is.Zero);
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenTheNotSetFaceTexture_WhenFinishingTheCamera_ThenTheFaceIsNotCollected()
        {
            camera.AddModel(BuildTriangle(128, 0xbc614e));

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.Zero);
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenTheHiddenRenderTexture_WhenFinishingTheCamera_ThenTheFaceIsCollectedButNotPainted()
        {
            camera.AddModel(BuildTriangle(128, -2));

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenATriangleCrossingTheNearPlane_WhenFinishingTheCamera_ThenItsVisiblePortionIsPainted()
        {
            int texture = Camera.GetTextureColour(255, 0, 0);
            GameObject model = new(3, 1);
            model.AddVertex(-16, -16, 4);
            model.AddVertex(16, -16, 128);
            model.AddVertex(0, 16, 128);
            model.AddFaceVertices(3, [0, 1, 2], texture, texture);
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void GivenAVisiblePolygon_WhenFinishingTheCamera_ThenItsVertexCountPathPaintsPixels(
            int vertexCount)
        {
            GameObject model = BuildRegularPolygon(
                vertexCount,
                128,
                Camera.GetTextureColour(255, 0, 0));
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenACameraTransform_WhenFinishingTheCamera_ThenModelCoordinatesUseItsOrigin()
        {
            GameObject model = new(1, 0)
            {
                ObjectState = 2,
            };
            model.AddVertex(42, 64, 96);
            camera.AddModel(model);
            camera.SetCameraTransform(42, 64, 96, 0, 0, 0, 32);

            camera.FinishCamera();

            Assert.That(model.ProjectedX[0], Is.Zero);
            Assert.That(model.ProjectedY[0], Is.Zero);
            Assert.That(model.ProjectedDepth[0], Is.EqualTo(32));
        }

        [Test]
        public void GivenRotationsSeparatedByAFullTurn_WhenFinishingTheCamera_ThenProjectionIsEquivalent()
        {
            GameObject firstModel = new(1, 0)
            {
                ObjectState = 2,
            };
            firstModel.AddVertex(4, 8, 16);
            camera.AddModel(firstModel);
            camera.SetCameraTransform(0, 0, 0, 0, 0, 0, 0);
            camera.FinishCamera();
            int expectedX = firstModel.ProjectedX[0];
            int expectedY = firstModel.ProjectedY[0];
            int expectedDepth = firstModel.ProjectedDepth[0];

            Camera equivalentCamera = new(image, 2, 4, 1);
            equivalentCamera.SetCameraSize(64, 48, 64, 48, 128, 8);
            GameObject equivalentModel = new(1, 0)
            {
                ObjectState = 2,
            };
            equivalentModel.AddVertex(4, 8, 16);
            equivalentCamera.AddModel(equivalentModel);
            equivalentCamera.SetCameraTransform(0, 0, 0, 1024, 1024, 1024, 0);
            equivalentCamera.FinishCamera();

            Assert.That(equivalentModel.ProjectedX[0], Is.EqualTo(expectedX));
            Assert.That(equivalentModel.ProjectedY[0], Is.EqualTo(expectedY));
            Assert.That(equivalentModel.ProjectedDepth[0], Is.EqualTo(expectedDepth));
        }

        [TestCase(0, 4096)]
        [TestCase(1, 16384)]
        public void GivenAnOpaqueTexture_WhenFinishingTheCamera_ThenTheTexturedTriangleIsPainted(
            int frameType,
            int pixelCount)
        {
            camera.CreateTexture(1, 1, 1);
            camera.SetTexture(0, new sbyte[pixelCount], [0xf80000], frameType);
            camera.AddModel(BuildTriangle(128, 0));

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenATransparentTexture_WhenFinishingTheCamera_ThenTheFaceIsCollectedWithoutPainting()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[4096], [0xf800ff], 0);
            camera.AddModel(BuildTriangle(128, 0));

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenAPerspectiveTexturedTriangle_WhenFinishingTheCamera_ThenPerspectivePixelsArePainted()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[4096], [0x00f800], 0);
            GameObject model = BuildTriangle(128, 0);
            model.IsPerspectiveTextured = true;
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenA128PerspectiveTexture_WhenFinishingTheCamera_ThenPerspectivePixelsArePainted()
        {
            camera.CreateTexture(1, 0, 1);
            camera.SetTexture(0, new sbyte[16384], [0x00f800], 1);
            GameObject model = BuildTriangle(128, 0);
            model.IsPerspectiveTextured = true;
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenA128TransparentTexture_WhenFinishingTheCamera_ThenTheFaceIsCollectedWithoutPainting()
        {
            camera.CreateTexture(1, 0, 1);
            camera.SetTexture(0, new sbyte[16384], [0xf800ff], 1);
            camera.AddModel(BuildTriangle(128, 0));

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [TestCase(0, 4096)]
        [TestCase(1, 16384)]
        public void GivenAnInterlacedTexture_WhenFinishingTheCamera_ThenAlternatingRowsArePainted(
            int frameType,
            int pixelCount)
        {
            image.IsInterlaced = true;
            camera.CreateTexture(1, 1, 1);
            camera.SetTexture(0, new sbyte[pixelCount], [0xf80000], frameType);
            camera.AddModel(BuildTriangle(128, 0));

            camera.FinishCamera();

            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));

            for (int positionY = 1; positionY < image.GameHeight; positionY += 2)
            {
                Assert.That(
                    image.Pixels.Skip(positionY * image.GameWidth).Take(image.GameWidth),
                    Has.All.Zero);
            }
        }

        [Test]
        public void GivenAGiantCrystal_WhenFinishingTheCamera_ThenTheShiftedColourPathPaintsPixels()
        {
            GameObject model = BuildTriangle(128, Camera.GetTextureColour(255, 0, 0));
            model.IsGiantCrystal = true;
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenAWideColouredPolygon_WhenFinishingTheCamera_ThenItsVisibleSpanIsClippedAndPainted()
        {
            GameObject model = new(4, 1);
            model.AddVertex(-128, -16, 128);
            model.AddVertex(128, -16, 128);
            model.AddVertex(128, 16, 128);
            model.AddVertex(-128, 16, 128);
            int texture = Camera.GetTextureColour(255, 0, 0);
            model.AddFaceVertices(4, [0, 1, 2, 3], texture, texture);
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [TestCase(0, 4096)]
        [TestCase(1, 16384)]
        public void GivenAWideTexturedPolygon_WhenFinishingTheCamera_ThenItsVisibleSpanIsClippedAndPainted(
            int frameType,
            int pixelCount)
        {
            camera.CreateTexture(1, 1, 1);
            camera.SetTexture(0, new sbyte[pixelCount], [0x00f800], frameType);
            GameObject model = new(4, 1);
            model.AddVertex(-128, -16, 128);
            model.AddVertex(128, -16, 128);
            model.AddVertex(128, 16, 128);
            model.AddVertex(-128, 16, 128);
            model.AddFaceVertices(4, [0, 1, 2, 3], 0, 0);
            camera.AddModel(model);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenATextureIndexBeyondTheLoadedCount_WhenFinishingTheCamera_ThenAnIndexExceptionIsThrown()
        {
            camera.CreateTexture(1, 1, 0);
            camera.SetTexture(0, new sbyte[4096], [0xf80000], 0);
            camera.AddModel(BuildTriangle(128, 42));

            Assert.That(
                () => camera.FinishCamera(),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenAnInterlacedFrame_WhenFinishingTheCamera_ThenOnlyAlternatingRowsArePainted()
        {
            image.IsInterlaced = true;
            camera.IsInterlaced = true;
            camera.AddModel(BuildTriangle(128, Camera.GetTextureColour(255, 0, 0)));

            camera.FinishCamera();

            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));

            for (int positionY = 1; positionY < image.GameHeight; positionY += 2)
            {
                Assert.That(
                    image.Pixels.Skip(positionY * image.GameWidth).Take(image.GameWidth),
                    Has.All.Zero);
            }
        }

        [Test]
        public void GivenAVisibleSceneSprite_WhenFinishingTheCamera_ThenItIsCollectedAndPainted()
        {
            SetSpritePicture(42);
            camera.AddSpriteToScene(0, 0, 0, 128, 32, 32, 64);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.EqualTo(1));
            Assert.That(image.Pixels.Count(pixel => pixel != 0), Is.GreaterThan(0));
        }

        [Test]
        public void GivenAMouseOverAVisibleSceneSprite_WhenFinishingTheCamera_ThenAHitIsRecorded()
        {
            SetSpritePicture(42);
            camera.AddSpriteToScene(0, 0, 0, 128, 32, 32, 64);
            camera.SetMousePosition(64, 0);

            camera.FinishCamera();

            Assert.That(camera.GetOptionCount(), Is.EqualTo(1));
            Assert.That(camera.GetHighlightedObjects()[0], Is.SameAs(camera.HighlightedObject));
            Assert.That(camera.GetHighlightedPlayers()[0], Is.Zero);
        }

        [TestCase(5)]
        [TestCase(1000)]
        public void GivenASceneSpriteAtAnExclusiveDepthBoundary_WhenFinishingTheCamera_ThenItIsNotCollected(
            int depth)
        {
            SetSpritePicture(42);
            camera.AddSpriteToScene(0, 0, 0, depth, 32, 32, 64);

            camera.FinishCamera();

            Assert.That(camera.SavedModelIndex, Is.Zero);
            Assert.That(image.Pixels, Has.All.Zero);
        }

        [Test]
        public void GivenAVisibleGeometryMatrix_WhenFinishingEachCamera_ThenEveryConfigurationCompletes()
        {
            int renderedConfigurationCount = 0;

            for (int vertexCount = 3; vertexCount <= 8; vertexCount += 1)
            {
                for (int rotationStep = 0; rotationStep < 12; rotationStep += 1)
                {
                    for (int centreX = -32; centreX <= 32; centreX += 32)
                    {
                        for (int centreY = -24; centreY <= 24; centreY += 24)
                        {
                            GameImage targetImage = new(128, 96, 1);
                            Camera targetCamera = new(targetImage, 4, 16, 4);
                            targetCamera.SetCameraSize(64, 48, 64, 48, 128, 8);
                            targetCamera.IsInterlaced = rotationStep % 3 == 0;
                            targetImage.IsInterlaced = rotationStep % 4 == 0;
                            GameObject model = BuildProjectedPolygon(
                                vertexCount,
                                centreX,
                                centreY,
                                128,
                                16 + rotationStep % 3 * 8,
                                rotationStep * Math.PI / 6D,
                                Camera.GetTextureColour(255, 0, 0));

                            if ((rotationStep & 1) != 0)
                            {
                                Array.Reverse(model.FaceVertexIndices[0]);
                            }

                            model.IsGiantCrystal = rotationStep % 5 == 0;
                            targetCamera.AddModel(model);

                            Assert.That(() => targetCamera.FinishCamera(), Throws.Nothing);

                            if (targetCamera.SavedModelIndex > 0)
                            {
                                renderedConfigurationCount += 1;
                            }
                        }
                    }
                }
            }

            Assert.That(renderedConfigurationCount, Is.GreaterThan(0));
        }

        [Test]
        public void GivenNearPlaneGeometryPermutations_WhenFinishingEachCamera_ThenClippedPolygonsComplete()
        {
            for (int nearVertexIndex = 0; nearVertexIndex < 4; nearVertexIndex += 1)
            {
                GameImage targetImage = new(128, 96, 1);
                Camera targetCamera = new(targetImage, 4, 16, 4);
                targetCamera.SetCameraSize(64, 48, 64, 48, 128, 8);
                GameObject model = new(4, 1);
                int[] positionsX = [-24, 24, 24, -24];
                int[] positionsY = [-24, -24, 24, 24];

                for (int vertexIndex = 0; vertexIndex < 4; vertexIndex += 1)
                {
                    int depth = vertexIndex == nearVertexIndex ? 4 : 128;
                    model.AddVertex(positionsX[vertexIndex], positionsY[vertexIndex], depth);
                }

                int texture = Camera.GetTextureColour(0, 255, 0);
                model.AddFaceVertices(4, [0, 1, 2, 3], texture, texture);
                targetCamera.AddModel(model);

                Assert.That(() => targetCamera.FinishCamera(), Throws.Nothing);
                Assert.That(targetCamera.SavedModelIndex, Is.EqualTo(1));
            }
        }

        private static GameObject BuildTriangle(int depth, int texture)
        {
            GameObject model = new(3, 1);
            model.AddVertex(-16, -16, depth);
            model.AddVertex(16, -16, depth);
            model.AddVertex(0, 16, depth);
            model.AddFaceVertices(3, [0, 1, 2], texture, texture);

            return model;
        }

        private static GameObject BuildRegularPolygon(int vertexCount, int depth, int texture)
        {
            GameObject model = new(vertexCount, 1);

            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex += 1)
            {
                double angle = Math.PI * 2D * vertexIndex / vertexCount;
                model.AddVertex(
                    (int)(Math.Cos(angle) * 16D),
                    (int)(Math.Sin(angle) * 16D),
                    depth);
            }

            model.AddFaceVertices(
                vertexCount,
                Enumerable.Range(0, vertexCount).ToArray(),
                texture,
                texture);

            return model;
        }

        private static GameObject BuildProjectedPolygon(
            int vertexCount,
            int centreX,
            int centreY,
            int depth,
            int radius,
            double rotation,
            int texture)
        {
            GameObject model = new(vertexCount, 1);

            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex += 1)
            {
                double angle = rotation + Math.PI * 2D * vertexIndex / vertexCount;
                model.AddVertex(
                    centreX + (int)Math.Round(Math.Cos(angle) * radius),
                    centreY + (int)Math.Round(Math.Sin(angle) * radius),
                    depth);
            }

            model.AddFaceVertices(
                vertexCount,
                Enumerable.Range(0, vertexCount).ToArray(),
                texture,
                texture);

            return model;
        }

        private void SetSpritePicture(int colour)
        {
            image.PictureWidth[0] = 1;
            image.PictureHeight[0] = 1;
            image.PictureAssumedWidth[0] = 1;
            image.PictureAssumedHeight[0] = 1;
            image.PictureColours[0] = [colour];
        }
    }
}