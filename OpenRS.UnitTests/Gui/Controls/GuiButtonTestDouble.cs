using NuciXNA.Primitives;

using OpenRS.Gui.Controls;

namespace OpenRS.UnitTests.Gui.Controls
{
    internal sealed class GuiButtonTestDouble : GuiButton
    {
        internal Rectangle2D CalculateRectangle(int sectionIndex)
            => CalculateSourceRectangle(sectionIndex);
    }
}