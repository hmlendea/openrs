using NUnit.Framework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using OpenRS.Primitives;

namespace OpenRS.UnitTests.Primitives
{
    [TestFixture]
    public sealed class VertexPositionNormalTests
    {
        [Test]
        public void GivenPositionAndNormalVectors_WhenConstructingAVertex_ThenBothVectorsAreRetained()
        {
            Vector3 expectedPosition = new(4, 8, 16);
            Vector3 expectedNormal = new(32, 42, 48);

            VertexPositionNormal vertex = new(expectedPosition, expectedNormal);

            Assert.That(vertex.Position, Is.EqualTo(expectedPosition));
            Assert.That(vertex.Normal, Is.EqualTo(expectedNormal));
        }

        [Test]
        public void GivenTheVertexContract_WhenReadingItsDeclaration_ThenPositionAndNormalLayoutRemainCompatible()
        {
            VertexPositionNormal vertex = new(Vector3.Zero, Vector3.Zero);
            VertexElement[] elements = VertexPositionNormal.VertexDeclaration.GetVertexElements();

            Assert.That(VertexPositionNormal.VertexDeclaration.VertexStride, Is.EqualTo(24));
            Assert.That(elements, Has.Length.EqualTo(2));
            AssertElement(
                elements[0],
                0,
                VertexElementFormat.Vector3,
                VertexElementUsage.Position);
            AssertElement(
                elements[1],
                12,
                VertexElementFormat.Vector3,
                VertexElementUsage.Normal);
            Assert.That(
                ((IVertexType)vertex).VertexDeclaration,
                Is.SameAs(VertexPositionNormal.VertexDeclaration));
        }

        private static void AssertElement(
            VertexElement element,
            int expectedOffset,
            VertexElementFormat expectedFormat,
            VertexElementUsage expectedUsage)
        {
            Assert.That(element.Offset, Is.EqualTo(expectedOffset));
            Assert.That(element.VertexElementFormat, Is.EqualTo(expectedFormat));
            Assert.That(element.VertexElementUsage, Is.EqualTo(expectedUsage));
            Assert.That(element.UsageIndex, Is.Zero);
        }
    }
}