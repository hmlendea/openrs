using NUnit.Framework;

using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    public sealed class CameraDataContractTests
    {
        [Test]
        public void GivenCameraModelValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            GameObject sourceObject = new(0, 0);
            CameraModel model = new()
            {
                BoundsMinX = 4,
                BoundsMinY = 8,
                BoundsMaxX = 16,
                BoundsMaxY = 32,
                BoundsMinZ = 42,
                BoundsMaxZ = 48,
                SourceObject = sourceObject,
                FaceIndex = 64,
                Scale = 96,
                NormalX = 128,
                NormalY = 256,
                NormalZ = 512,
                VisibilityDot = 613,
                CurrentTextureIndex = 873,
                IsSorted = true,
                SortIndex = 1024,
                DependencyIndex = 2048,
            };

            Assert.That(model.BoundsMinX, Is.EqualTo(4));
            Assert.That(model.BoundsMinY, Is.EqualTo(8));
            Assert.That(model.BoundsMaxX, Is.EqualTo(16));
            Assert.That(model.BoundsMaxY, Is.EqualTo(32));
            Assert.That(model.BoundsMinZ, Is.EqualTo(42));
            Assert.That(model.BoundsMaxZ, Is.EqualTo(48));
            Assert.That(model.SourceObject, Is.SameAs(sourceObject));
            Assert.That(model.FaceIndex, Is.EqualTo(64));
            Assert.That(model.Scale, Is.EqualTo(96));
            Assert.That(model.NormalX, Is.EqualTo(128));
            Assert.That(model.NormalY, Is.EqualTo(256));
            Assert.That(model.NormalZ, Is.EqualTo(512));
            Assert.That(model.VisibilityDot, Is.EqualTo(613));
            Assert.That(model.CurrentTextureIndex, Is.EqualTo(873));
            Assert.That(model.IsSorted);
            Assert.That(model.SortIndex, Is.EqualTo(1024));
            Assert.That(model.DependencyIndex, Is.EqualTo(2048));
        }

        [Test]
        public void GivenANewCameraModel_WhenReadingDependency_ThenItIsUnset()
            => Assert.That(new CameraModel().DependencyIndex, Is.EqualTo(-1));

        [Test]
        public void GivenCameraVariableValues_WhenAssigningThem_ThenEveryValueIsRetained()
        {
            CameraVariable variable = new()
            {
                LeftX = 4,
                RightX = 8,
                LeftShade = 16,
                RightShade = 32,
            };

            Assert.That(variable.LeftX, Is.EqualTo(4));
            Assert.That(variable.RightX, Is.EqualTo(8));
            Assert.That(variable.LeftShade, Is.EqualTo(16));
            Assert.That(variable.RightShade, Is.EqualTo(32));
        }
    }
}