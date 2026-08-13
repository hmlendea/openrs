using NUnit.Framework;

using Microsoft.Xna.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    public sealed class FaceTests
    {
        [Test]
        public void GivenOnlyPoints_WhenConstructingAFace_ThenItUsesTheDefaultColourAndNoImage()
        {
            int[] expectedPoints = [4, 8, 16];

            Face face = new(expectedPoints);

            Assert.That(face.Points, Is.SameAs(expectedPoints));
            Assert.That(face.FaceColour, Is.EqualTo(Color.Red));
            Assert.That(face.Image, Is.EqualTo(-1));
        }

        [Test]
        public void GivenAColour_WhenConstructingAFace_ThenItUsesThatColourAndNoImage()
        {
            Color expectedColour = new(42, 64, 96, 128);
            int[] expectedPoints = [4, 8, 16];

            Face face = new(expectedColour, expectedPoints);

            Assert.That(face.Points, Is.SameAs(expectedPoints));
            Assert.That(face.FaceColour, Is.EqualTo(expectedColour));
            Assert.That(face.Image, Is.EqualTo(-1));
        }

        [TestCase(0)]
        [TestCase(42)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void GivenAnImage_WhenConstructingAFace_ThenItUsesThatImageAndTheDefaultColour(int image)
        {
            int[] expectedPoints = [4, 8, 16];

            Face face = new(image, expectedPoints);

            Assert.That(face.Points, Is.SameAs(expectedPoints));
            Assert.That(face.FaceColour, Is.EqualTo(default(Color)));
            Assert.That(face.Image, Is.EqualTo(image));
        }

        [Test]
        public void GivenAFace_WhenReplacingItsColour_ThenTheNewColourIsRetained()
        {
            Face face = new([4, 8, 16]);
            Color expectedColour = new(42, 64, 96, 128);

            face.FaceColour = expectedColour;

            Assert.That(face.FaceColour, Is.EqualTo(expectedColour));
        }

        [Test]
        public void GivenNullPoints_WhenConstructingAFace_ThenTheNullReferenceIsRetained()
        {
            Face face = new((int[])null!);

            Assert.That(face.Points, Is.Null);
        }
    }
}