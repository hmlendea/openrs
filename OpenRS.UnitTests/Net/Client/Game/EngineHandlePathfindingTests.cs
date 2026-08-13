using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class EngineHandlePathfindingTests
    {
        private static int GridSize => 96;

        private static int PathCapacity => 10000;

        private static int FullyBlockedTile => 0x7f;

        private EngineHandle engineHandle = null!;
        private int[] pathPositionX = null!;
        private int[] pathPositionY = null!;

        [SetUp]
        public void SetUp()
        {
            engineHandle = new EngineHandle(null!, null!, null!);
            pathPositionX = new int[PathCapacity];
            pathPositionY = new int[PathCapacity];
        }

        [TestCase(0, 0)]
        [TestCase(48, 48)]
        [TestCase(95, 95)]
        public void GivenAStartInsideTheDestination_WhenGeneratingAPath_ThenTheStartIsReturned(
            int positionX,
            int positionY)
        {
            int pathLength = GeneratePath(
                positionX,
                positionY,
                positionX,
                positionY,
                positionX,
                positionY,
                false);

            Assert.That(pathLength, Is.EqualTo(1));
            AssertPathPoint(0, positionX, positionY);
        }

        [TestCase(47, 48)]
        [TestCase(49, 48)]
        [TestCase(48, 47)]
        [TestCase(48, 49)]
        [TestCase(47, 47)]
        [TestCase(49, 47)]
        [TestCase(47, 49)]
        [TestCase(49, 49)]
        public void GivenAnAdjacentDestination_WhenGeneratingAPath_ThenOneDirectSegmentIsReturned(
            int destinationX,
            int destinationY)
        {
            int pathLength = GeneratePath(48, 48, destinationX, destinationY, destinationX, destinationY, false);

            Assert.That(pathLength, Is.EqualTo(1));
            AssertPathPoint(0, destinationX, destinationY);
        }

        [Test]
        public void GivenACornerToCornerDestination_WhenGeneratingAPath_ThenOneDiagonalSegmentIsReturned()
        {
            int pathLength = GeneratePath(0, 0, 95, 95, 95, 95, false);

            Assert.That(pathLength, Is.EqualTo(1));
            AssertPathPoint(0, 95, 95);
        }

        [Test]
        public void GivenARectangularDestination_WhenGeneratingAPath_ThenTheNearestIncludedTileIsReturned()
        {
            int pathLength = GeneratePath(4, 8, 16, 32, 18, 34, false);

            Assert.That(pathLength, Is.GreaterThanOrEqualTo(1));
            Assert.That(pathPositionX[0], Is.InRange(16, 18));
            Assert.That(pathPositionY[0], Is.InRange(32, 34));
        }

        [TestCase(9, 10, 10, 10)]
        [TestCase(11, 10, 10, 10)]
        [TestCase(10, 9, 10, 10)]
        [TestCase(10, 11, 10, 10)]
        public void GivenAnOpenTileAdjacentToAnObject_WhenCheckingObjects_ThenTheCurrentTileIsReturned(
            int startX,
            int startY,
            int objectX,
            int objectY)
        {
            int pathLength = GeneratePath(startX, startY, objectX, objectY, objectX, objectY, true);

            Assert.That(pathLength, Is.EqualTo(1));
            AssertPathPoint(0, startX, startY);
        }

        [TestCase(9, 10, 10, 10)]
        [TestCase(11, 10, 10, 10)]
        [TestCase(10, 9, 10, 10)]
        [TestCase(10, 11, 10, 10)]
        public void GivenAnOpenTileAdjacentToADestination_WhenNotCheckingObjects_ThenTheDestinationIsReturned(
            int startX,
            int startY,
            int destinationX,
            int destinationY)
        {
            int pathLength = GeneratePath(
                startX,
                startY,
                destinationX,
                destinationY,
                destinationX,
                destinationY,
                false);

            Assert.That(pathLength, Is.EqualTo(1));
            AssertPathPoint(0, destinationX, destinationY);
        }

        [Test]
        public void GivenAFullyBlockedObjectTile_WhenCheckingObjects_ThenNoPathIsReturned()
        {
            engineHandle.Tiles[10][10] = FullyBlockedTile;

            int pathLength = GeneratePath(9, 10, 10, 10, 10, 10, true);

            Assert.That(pathLength, Is.EqualTo(-1));
            Assert.That(engineHandle.Steps[10][10], Is.Zero);
        }

        [Test]
        public void GivenAFullyBlockedDestination_WhenGeneratingAPath_ThenNoPathIsReturned()
        {
            engineHandle.Tiles[16][32] = FullyBlockedTile;

            int pathLength = GeneratePath(4, 8, 16, 32, 16, 32, false);

            Assert.That(pathLength, Is.EqualTo(-1));
            Assert.That(engineHandle.Steps[16][32], Is.Zero);
        }

        [Test]
        public void GivenAnEnclosedStart_WhenGeneratingAPath_ThenNoPathIsReturned()
        {
            BlockAllNeighbours(48, 48);

            int pathLength = GeneratePath(48, 48, 64, 64, 64, 64, false);

            Assert.That(pathLength, Is.EqualTo(-1));
        }

        [Test]
        public void GivenAnObstacleOnTheDirectRoute_WhenGeneratingAPath_ThenDirectionChangesAreReturned()
        {
            engineHandle.Tiles[12][10] = FullyBlockedTile;

            int pathLength = GeneratePath(10, 10, 14, 10, 14, 10, false);

            Assert.That(pathLength, Is.GreaterThan(1));
            AssertPathPoint(0, 14, 10);
            Assert.That(engineHandle.Steps[12][10], Is.Zero);
        }

        [Test]
        public void GivenAPreviousExhaustiveSearch_WhenGeneratingAnotherPath_ThenTheStepsGridIsCleared()
        {
            engineHandle.Tiles[16][32] = FullyBlockedTile;
            GeneratePath(4, 8, 16, 32, 16, 32, false);
            Assert.That(engineHandle.Steps[95][95], Is.Not.Zero);
            engineHandle.Tiles[16][32] = 0;

            int pathLength = GeneratePath(4, 8, 5, 8, 5, 8, false);

            Assert.That(pathLength, Is.EqualTo(1));
            Assert.That(engineHandle.Steps[95][95], Is.Zero);
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(96, 0)]
        [TestCase(0, 96)]
        public void GivenAnInvalidStartCoordinate_WhenGeneratingAPath_ThenAnIndexExceptionIsThrown(
            int startX,
            int startY)
            => Assert.That(
                () => GeneratePath(startX, startY, 4, 8, 4, 8, false),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenANullXBuffer_WhenGeneratingAPath_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => engineHandle.GeneratePath(4, 8, 16, 32, 16, 32, null!, pathPositionY, false),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenANullYBuffer_WhenGeneratingAPath_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => engineHandle.GeneratePath(4, 8, 16, 32, 16, 32, pathPositionX, null!, false),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenEmptyPathBuffers_WhenGeneratingAPath_ThenAnIndexExceptionIsThrown()
            => Assert.That(
                () => engineHandle.GeneratePath(4, 8, 16, 32, 16, 32, [], [], false),
                Throws.TypeOf<IndexOutOfRangeException>());

        [Test]
        public void GivenSingleSlotBuffersAndAnAlreadyReachedDestination_WhenGeneratingAPath_ThenTheStartIsReturned()
        {
            int[] singlePositionX = new int[1];
            int[] singlePositionY = new int[1];

            int pathLength = engineHandle.GeneratePath(
                4,
                8,
                4,
                8,
                4,
                8,
                singlePositionX,
                singlePositionY,
                false);

            Assert.That(pathLength, Is.EqualTo(1));
            Assert.That(singlePositionX[0], Is.EqualTo(4));
            Assert.That(singlePositionY[0], Is.EqualTo(8));
        }

        [Test]
        public void GivenADestinationOutsideTheGrid_WhenGeneratingAPath_ThenNoPathIsReturned()
            => Assert.That(
                GeneratePath(4, 8, GridSize, GridSize, GridSize, GridSize, false),
                Is.EqualTo(-1));

        [Test]
        public void GivenReversedDestinationBounds_WhenGeneratingAPath_ThenNoPathIsReturned()
            => Assert.That(
                GeneratePath(4, 8, 16, 32, 8, 16, false),
                Is.EqualTo(-1));

        private void AssertPathPoint(int pathIndex, int expectedPositionX, int expectedPositionY)
        {
            Assert.That(pathPositionX[pathIndex], Is.EqualTo(expectedPositionX));
            Assert.That(pathPositionY[pathIndex], Is.EqualTo(expectedPositionY));
        }

        private void BlockAllNeighbours(int positionX, int positionY)
        {
            for (int offsetX = -1; offsetX <= 1; offsetX += 1)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY += 1)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    engineHandle.Tiles[positionX + offsetX][positionY + offsetY] = FullyBlockedTile;
                }
            }
        }

        private int GeneratePath(
            int startX,
            int startY,
            int bottomDestinationX,
            int bottomDestinationY,
            int upperDestinationX,
            int upperDestinationY,
            bool checkForObjects)
            => engineHandle.GeneratePath(
                startX,
                startY,
                bottomDestinationX,
                bottomDestinationY,
                upperDestinationX,
                upperDestinationY,
                pathPositionX,
                pathPositionY,
                checkForObjects);
    }
}