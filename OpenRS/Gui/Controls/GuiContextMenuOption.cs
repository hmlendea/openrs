using System;

namespace OpenRS.Gui.Controls
{
    internal sealed class GuiContextMenuOption
    {
        internal string Text { get; set; }

        internal Action SelectedAction { get; set; }
    }
}