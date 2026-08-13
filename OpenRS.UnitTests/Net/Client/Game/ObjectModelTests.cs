using System;
using System.Collections.Generic;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class ObjectModelTests
    {
        [Test]
        public void GivenANewModel_WhenReadingItsTransform_ThenItUsesIdentityScaleAndZeroOffsets()
        {
            ObjectModel model = new();

            Assert.That(model.Vertices, Is.Empty);
            Assert.That(model.Faces, Is.Empty);
            Assert.That(model.XRotation, Is.Zero);
            Assert.That(model.YRotation, Is.Zero);
            Assert.That(model.ZRotation, Is.Zero);
            Assert.That(model.XScale, Is.EqualTo(1.0F));
            Assert.That(model.YScale, Is.EqualTo(1.0F));
            Assert.That(model.ZScale, Is.EqualTo(1.0F));
            Assert.That(model.XTranslation, Is.Zero);
            Assert.That(model.YTranslation, Is.Zero);
            Assert.That(model.ZTranslation, Is.Zero);
            Assert.That(model.TextureCount, Is.Zero);
        }

        [Test]
        public void GivenSourceCollections_WhenConstructingAModel_ThenTheCollectionsAreCopied()
        {
            Vertex expectedVertex = new(4, 8, 16);
            Face expectedFace = new([0, 1, 2]);
            List<Vertex> vertices = [expectedVertex];
            List<Face> faces = [expectedFace];
            ObjectModel model = new(vertices, faces);

            vertices.Clear();
            faces.Clear();

            Assert.That(model.GetVertex(0), Is.SameAs(expectedVertex));
            Assert.That(model.GetFace(0), Is.SameAs(expectedFace));
        }

        [Test]
        public void GivenReplacementCollections_WhenSettingThem_ThenTheCollectionsAreCopied()
        {
            Vertex expectedVertex = new(4, 8, 16);
            Face expectedFace = new([0, 1, 2]);
            List<Vertex> vertices = [expectedVertex];
            List<Face> faces = [expectedFace];
            ObjectModel model = new()
            {
                Vertices = vertices,
                Faces = faces,
            };

            vertices.Clear();
            faces.Clear();

            Assert.That(model.GetVertex(0), Is.SameAs(expectedVertex));
            Assert.That(model.GetFace(0), Is.SameAs(expectedFace));
        }

        [Test]
        public void GivenNullVertices_WhenConstructingAModel_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => new ObjectModel(null!, []),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("vertices"));

        [Test]
        public void GivenNullFaces_WhenConstructingAModel_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => new ObjectModel([], null!),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("faces"));

        [Test]
        public void GivenNullVertices_WhenReplacingThem_ThenAnArgumentNullExceptionIsThrown()
        {
            ObjectModel model = new();

            Assert.That(
                () => model.Vertices = null!,
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GivenNullFaces_WhenReplacingThem_ThenAnArgumentNullExceptionIsThrown()
        {
            ObjectModel model = new();

            Assert.That(
                () => model.Faces = null!,
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GivenVertices_WhenAddingAndRemovingThem_ThenOrderAndIdentityArePreserved()
        {
            Vertex firstVertex = new(4, 8, 16);
            Vertex secondVertex = new(32, 42, 48);
            ObjectModel model = new();
            model.AddVertex(firstVertex);
            model.AddVertex(secondVertex);

            Vertex removedVertex = model.RemoveVertex(0);

            Assert.That(removedVertex, Is.SameAs(firstVertex));
            Assert.That(model.GetVertex(0), Is.SameAs(secondVertex));
        }

        [Test]
        public void GivenFaces_WhenAddingAndRemovingThem_ThenOrderAndIdentityArePreserved()
        {
            Face firstFace = new([0, 1, 2]);
            Face secondFace = new([2, 3, 0]);
            ObjectModel model = new();
            model.AddFace(firstFace);
            model.AddFace(secondFace);

            Face removedFace = model.RemoveFace(0);

            Assert.That(removedFace, Is.SameAs(firstFace));
            Assert.That(model.GetFace(0), Is.SameAs(secondFace));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(42)]
        public void GivenAnInvalidVertexIndex_WhenReadingIt_ThenAnArgumentExceptionIsThrown(int vertexIndex)
        {
            ObjectModel model = new();

            Assert.That(
                () => model.GetVertex(vertexIndex),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(42)]
        public void GivenAnInvalidFaceIndex_WhenReadingIt_ThenAnArgumentExceptionIsThrown(int faceIndex)
        {
            ObjectModel model = new();

            Assert.That(
                () => model.GetFace(faceIndex),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(-721.0F, -361.0F)]
        [TestCase(-720.0F, -360.0F)]
        [TestCase(-361.0F, -1.0F)]
        [TestCase(-360.0F, -360.0F)]
        [TestCase(-1.0F, -1.0F)]
        [TestCase(0.0F, 0.0F)]
        [TestCase(1.0F, 1.0F)]
        [TestCase(360.0F, 360.0F)]
        [TestCase(361.0F, 1.0F)]
        [TestCase(720.0F, 360.0F)]
        [TestCase(721.0F, 361.0F)]
        public void GivenARotation_WhenSettingEachAxis_ThenOneExcessRevolutionIsRemoved(
            float rotation,
            float expectedRotation)
        {
            ObjectModel model = new()
            {
                XRotation = rotation,
                YRotation = rotation,
                ZRotation = rotation,
            };

            Assert.That(model.XRotation, Is.EqualTo(expectedRotation));
            Assert.That(model.YRotation, Is.EqualTo(expectedRotation));
            Assert.That(model.ZRotation, Is.EqualTo(expectedRotation));
        }

        [TestCase(float.NegativeInfinity)]
        [TestCase(-42.0F)]
        [TestCase(0.0F)]
        [TestCase(3.14F)]
        [TestCase(42.0F)]
        [TestCase(float.PositiveInfinity)]
        public void GivenAScale_WhenSettingIt_ThenAllAxesUseTheSameValue(float scale)
        {
            ObjectModel model = new();

            model.SetScale(scale);

            Assert.That(model.XScale, Is.EqualTo(scale));
            Assert.That(model.YScale, Is.EqualTo(scale));
            Assert.That(model.ZScale, Is.EqualTo(scale));
        }

        [Test]
        public void GivenANotANumberScale_WhenSettingIt_ThenAllAxesRemainNotANumber()
        {
            ObjectModel model = new();

            model.SetScale(float.NaN);

            Assert.That(model.XScale, Is.NaN);
            Assert.That(model.YScale, Is.NaN);
            Assert.That(model.ZScale, Is.NaN);
        }
    }
}