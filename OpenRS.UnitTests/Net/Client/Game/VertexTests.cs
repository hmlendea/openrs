using NUnit.Framework;

using Microsoft.Xna.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class VertexTests
    {
        [TestCase(0.0, 0.0, 0.0)]
        [TestCase(3.14, 6.13, 8.73)]
        [TestCase(-42.0, 64.0, -128.0)]
        [TestCase(double.MaxValue, double.MinValue, 0.0)]
        [TestCase(double.PositiveInfinity, double.NegativeInfinity, 0.0)]
        public void GivenDoubleCoordinates_WhenConstructingAVertex_ThenTheyAreConvertedToSinglePrecision(
            double pointX,
            double pointY,
            double pointZ)
        {
            Vertex vertex = new(pointX, pointY, pointZ);

            Assert.That(
                vertex.GetLocalPoint(),
                Is.EqualTo(new Vector3((float)pointX, (float)pointY, (float)pointZ)));
        }

            [Test]
            public void GivenANotANumberCoordinate_WhenConstructingAVertex_ThenItRemainsNotANumber()
            {
                Vertex vertex = new(double.NaN, double.NaN, double.NaN);

                Assert.That(vertex.GetLocalPoint().X, Is.NaN);
                Assert.That(vertex.GetLocalPoint().Y, Is.NaN);
                Assert.That(vertex.GetLocalPoint().Z, Is.NaN);
            }

        [Test]
        public void GivenALocalPoint_WhenConstructingAVertex_ThenOnlyLocalStateIsInitialised()
        {
            Vector3 expectedLocalPoint = new(4, 8, 16);
            Vertex vertex = new(expectedLocalPoint);

            Assert.That(vertex.GetLocalPoint(), Is.EqualTo(expectedLocalPoint));
            Assert.That(vertex.GetWorldPoint(), Is.EqualTo(Vector3.Zero));
            Assert.That(vertex.GetAlignedPoint(), Is.EqualTo(Vector3.Zero));
        }

        [Test]
        public void GivenThreeCoordinateStates_WhenSettingThem_ThenEachStateRemainsIndependent()
        {
            Vector3 expectedLocalPoint = new(4, 8, 16);
            Vector3 expectedWorldPoint = new(32, 42, 48);
            Vector3 expectedAlignedPoint = new(64, 96, 128);
            Vertex vertex = new(Vector3.Zero);

            vertex.SetLocalPoint(expectedLocalPoint);
            vertex.SetWorldPoint(expectedWorldPoint);
            vertex.SetAlignedPoint(expectedAlignedPoint);

            Assert.That(vertex.GetLocalPoint(), Is.EqualTo(expectedLocalPoint));
            Assert.That(vertex.GetWorldPoint(), Is.EqualTo(expectedWorldPoint));
            Assert.That(vertex.GetAlignedPoint(), Is.EqualTo(expectedAlignedPoint));
        }
    }
}