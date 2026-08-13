using NUnit.Framework;

using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.UnitTests.Net.Client.Game.Cameras
{
    [TestFixture]
    public sealed class CameraColourAndTrigonometryTests
    {
        [TestCase(0, 0, 0, -1)]
        [TestCase(7, 7, 7, -1)]
        [TestCase(8, 0, 0, -1025)]
        [TestCase(0, 8, 0, -33)]
        [TestCase(0, 0, 8, -2)]
        [TestCase(128, 64, 32, -16645)]
        [TestCase(255, 255, 255, -32768)]
        public void GivenRgbChannels_WhenRetrievingATextureColour_ThenTheCompatibleIndexIsReturned(
            int red,
            int green,
            int blue,
            int expectedColour)
            => Assert.That(
                Camera.GetTextureColour(red, green, blue),
                Is.EqualTo(expectedColour));

        [Test]
        public void GivenTheTrigonometryContract_WhenReadingItsDimensions_ThenTheyRemainCompatible()
        {
            Assert.That(CameraTrigonometryTable.TrigTableHalfSize, Is.EqualTo(1024));
            Assert.That(CameraTrigonometryTable.TrigTableMask, Is.EqualTo(1023));
            Assert.That(CameraTrigonometryTable.Table, Has.Length.EqualTo(2048));
        }

        [TestCase(0, 0, 32768)]
        [TestCase(1, 201, 32767)]
        [TestCase(256, 32768, 0)]
        [TestCase(512, 0, -32768)]
        [TestCase(768, -32768, 0)]
        [TestCase(1023, -201, 32767)]
        public void GivenATrigonometryIndex_WhenReadingTheTable_ThenTheFixedPointValuesRemainCompatible(
            int tableIndex,
            int expectedSine,
            int expectedCosine)
        {
            Assert.That(
                CameraTrigonometryTable.Table[tableIndex],
                Is.EqualTo(expectedSine));
            Assert.That(
                CameraTrigonometryTable.Table[tableIndex + CameraTrigonometryTable.TrigTableHalfSize],
                Is.EqualTo(expectedCosine));
        }
    }
}