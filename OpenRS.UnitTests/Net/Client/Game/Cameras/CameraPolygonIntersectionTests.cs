using System;
using System.Linq;

using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    public sealed class CameraPolygonIntersectionTests
    {
        private CameraDepthSorter depthSorter = null!;

        [SetUp]
        public void SetUp()
        {
            depthSorter = new CameraDepthSorter();
        }

        [Test]
        public void GivenVariedConvexPolygons_WhenResolvingTheirOrder_ThenEveryModelIsRetainedAndSorted()
        {
            int caseCount = 0;

            for (int firstVertexCount = 2; firstVertexCount <= 7; firstVertexCount += 1)
            {
                for (int secondVertexCount = 2; secondVertexCount <= 7; secondVertexCount += 1)
                {
                    for (int rotationStep = 0; rotationStep < 8; rotationStep += 1)
                    {
                        for (int offsetX = -32; offsetX <= 32; offsetX += 16)
                        {
                            CameraModel firstModel = BuildModel(
                                firstVertexCount,
                                0,
                                0,
                                0,
                                32,
                                0D);
                            CameraModel secondModel = BuildModel(
                                secondVertexCount,
                                offsetX,
                                rotationStep % 3 * 8 - 8,
                                10,
                                16 + rotationStep % 4 * 8,
                                rotationStep * Math.PI / 8D);
                            CameraModel[] models = [firstModel, secondModel];

                            depthSorter.ResolveRenderOrder(8, models, models.Length);

                            Assert.That(models, Does.Contain(firstModel));
                            Assert.That(models, Does.Contain(secondModel));
                            Assert.That(models, Has.All.Property(nameof(CameraModel.IsSorted)).True);
                            caseCount += 1;
                        }
                    }
                }
            }

            Assert.That(caseCount, Is.EqualTo(1440));
        }

        [Test]
        public void GivenPolygonsWithReversedWinding_WhenResolvingTheirOrder_ThenBothWindingsComplete()
        {
            for (int vertexCount = 3; vertexCount <= 8; vertexCount += 1)
            {
                CameraModel clockwiseModel = BuildModel(vertexCount, 0, 0, 0, 32, 0D);
                CameraModel anticlockwiseModel = BuildModel(vertexCount, 4, 4, 10, 32, Math.PI / 4D);
                Array.Reverse(anticlockwiseModel.SourceObject.FaceVertexIndices[0]);
                CameraModel[] models = [clockwiseModel, anticlockwiseModel];

                depthSorter.ResolveRenderOrder(8, models, models.Length);

                Assert.That(models, Has.All.Property(nameof(CameraModel.IsSorted)).True);
            }
        }

        [Test]
        public void GivenProjectedPolygonsWithSharedEdges_WhenResolvingTheirOrder_ThenNoModelIsLost()
        {
            CameraModel firstModel = BuildRectangle(-32, -16, 0, 32, 32);
            CameraModel secondModel = BuildRectangle(0, -16, 10, 32, 32);
            CameraModel[] models = [firstModel, secondModel];

            depthSorter.ResolveRenderOrder(8, models, models.Length);

            Assert.That(models.Select(model => model.SourceObject), Is.Unique);
            Assert.That(models, Has.All.Property(nameof(CameraModel.IsSorted)).True);
        }

        private static CameraModel BuildModel(
            int vertexCount,
            int centreX,
            int centreY,
            int depth,
            int radius,
            double rotation)
        {
            GameObject sourceObject = new(vertexCount, 1);

            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex += 1)
            {
                double angle = rotation + Math.PI * 2D * vertexIndex / vertexCount;
                int positionX = centreX + (int)Math.Round(Math.Cos(angle) * radius);
                int positionY = centreY + (int)Math.Round(Math.Sin(angle) * radius);
                sourceObject.AddVertex(positionX, positionY, depth);
                sourceObject.ProjectedX[vertexIndex] = positionX;
                sourceObject.ProjectedY[vertexIndex] = positionY;
                sourceObject.ProjectedDepth[vertexIndex] = depth;
                sourceObject.ProjectedU[vertexIndex] = positionX;
                sourceObject.ProjectedV[vertexIndex] = positionY;
            }

            sourceObject.AddFaceVertices(
                vertexCount,
                Enumerable.Range(0, vertexCount).ToArray(),
                42,
                48);

            return BuildCameraModel(sourceObject);
        }

        private static CameraModel BuildRectangle(
            int positionX,
            int positionY,
            int depth,
            int width,
            int height)
        {
            int[] positionsX = [positionX, positionX + width, positionX + width, positionX];
            int[] positionsY = [positionY, positionY, positionY + height, positionY + height];
            GameObject sourceObject = new(4, 1);

            for (int vertexIndex = 0; vertexIndex < 4; vertexIndex += 1)
            {
                sourceObject.AddVertex(positionsX[vertexIndex], positionsY[vertexIndex], depth);
                sourceObject.ProjectedX[vertexIndex] = positionsX[vertexIndex];
                sourceObject.ProjectedY[vertexIndex] = positionsY[vertexIndex];
                sourceObject.ProjectedDepth[vertexIndex] = depth;
                sourceObject.ProjectedU[vertexIndex] = positionsX[vertexIndex];
                sourceObject.ProjectedV[vertexIndex] = positionsY[vertexIndex];
            }

            sourceObject.AddFaceVertices(4, [0, 1, 2, 3], 42, 48);

            return BuildCameraModel(sourceObject);
        }

        private static CameraModel BuildCameraModel(GameObject sourceObject) => new()
        {
            BoundsMinX = -128,
            BoundsMaxX = 128,
            BoundsMinY = -128,
            BoundsMaxY = 128,
            BoundsMinZ = -32,
            BoundsMaxZ = 32,
            SourceObject = sourceObject,
            FaceIndex = 0,
            NormalZ = 1,
            VisibilityDot = 1,
        };
    }
}