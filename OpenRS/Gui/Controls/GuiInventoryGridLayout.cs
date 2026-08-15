using System;

using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public static class GuiInventoryGridLayout
    {
        public static int RowCount => 8;

        public static int ColumnCount => 4;

        public static int SlotCount => 30;

        public static Size2D CalculateItemCardSize(Size2D panelSize)
            => new(panelSize.Width / ColumnCount, panelSize.Height / RowCount);

        public static Point2D CalculateItemCardLocation(Size2D panelSize, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(slotIndex),
                    slotIndex,
                    $"The inventory slot index must be between 0 and {SlotCount - 1}.");
            }

            Size2D itemCardSize = CalculateItemCardSize(panelSize);
            int horizontalOffset = (panelSize.Width - ColumnCount * itemCardSize.Width) / 2;
            int verticalOffset = (panelSize.Height - RowCount * itemCardSize.Height) / 2;
            int columnIndex = slotIndex % ColumnCount;
            int rowIndex = slotIndex / ColumnCount;

            return new Point2D(
                horizontalOffset + itemCardSize.Width * columnIndex,
                verticalOffset + itemCardSize.Height * rowIndex);
        }
    }
}