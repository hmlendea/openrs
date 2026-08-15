using System;

using NUnit.Framework;

using NuciXNA.Primitives;

using OpenRS.Gui.Controls;

namespace OpenRS.UnitTests.Gui.Controls
{
    [TestFixture]
    public sealed class GuiInventoryGridLayoutTests
    {
        [Test]
        public void GivenTheInventoryGrid_WhenReadingItsDimensions_ThenThirtySlotsUseFourColumns()
        {
            Assert.Multiple(() =>
            {
                Assert.That(GuiInventoryGridLayout.SlotCount, Is.EqualTo(30));
                Assert.That(GuiInventoryGridLayout.ColumnCount, Is.EqualTo(4));
                Assert.That(GuiInventoryGridLayout.RowCount, Is.EqualTo(8));
            });
        }

        [TestCase(190, 262, 47, 32)]
        [TestCase(192, 256, 48, 32)]
        [TestCase(512, 512, 128, 64)]
        public void GivenPanelDimensions_WhenCalculatingItemCardSize_ThenTheGridCellSizeIsReturned(
            int panelWidth,
            int panelHeight,
            int expectedWidth,
            int expectedHeight)
            => Assert.That(
                GuiInventoryGridLayout.CalculateItemCardSize(new Size2D(panelWidth, panelHeight)),
                Is.EqualTo(new Size2D(expectedWidth, expectedHeight)));

        [TestCase(190, 262, 0, 1, 3)]
        [TestCase(190, 262, 3, 142, 3)]
        [TestCase(190, 262, 4, 1, 35)]
        [TestCase(190, 262, 29, 48, 227)]
        [TestCase(192, 256, 29, 48, 224)]
        public void GivenAValidSlot_WhenCalculatingItsLocation_ThenFourColumnCoordinatesAreReturned(
            int panelWidth,
            int panelHeight,
            int slotIndex,
            int expectedX,
            int expectedY)
            => Assert.That(
                GuiInventoryGridLayout.CalculateItemCardLocation(
                    new Size2D(panelWidth, panelHeight),
                    slotIndex),
                Is.EqualTo(new Point2D(expectedX, expectedY)));

        [TestCase(-1)]
        [TestCase(30)]
        [TestCase(42)]
        public void GivenAnInvalidSlot_WhenCalculatingItsLocation_ThenAnExceptionIsThrown(int slotIndex)
            => Assert.That(
                () => GuiInventoryGridLayout.CalculateItemCardLocation(new Size2D(190, 262), slotIndex),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property(nameof(ArgumentOutOfRangeException.ParamName))
                    .EqualTo(nameof(slotIndex)));
    }
}