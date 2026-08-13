using NUnit.Framework;

using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    public sealed class CameraVariableTests
    {
        [TestCase(0, 0, 0, 0)]
        [TestCase(4, 8, 16, 32)]
        [TestCase(-42, -64, -96, -128)]
        [TestCase(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue)]
        public void GivenScanlineValues_WhenSettingThem_ThenEachValueRemainsIndependent(
            int leftPositionX,
            int rightPositionX,
            int leftShade,
            int rightShade)
        {
            CameraVariable variable = new()
            {
                LeftX = leftPositionX,
                RightX = rightPositionX,
                LeftShade = leftShade,
                RightShade = rightShade,
            };

            Assert.That(variable.LeftX, Is.EqualTo(leftPositionX));
            Assert.That(variable.RightX, Is.EqualTo(rightPositionX));
            Assert.That(variable.LeftShade, Is.EqualTo(leftShade));
            Assert.That(variable.RightShade, Is.EqualTo(rightShade));
        }
    }
}