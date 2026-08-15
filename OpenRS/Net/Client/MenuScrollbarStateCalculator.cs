namespace OpenRS.Net.Client
{
    internal static class MenuScrollbarStateCalculator
    {
        private static int ScrollbarWidth => 12;

        private static int MinimumThumbSize => 6;

        private static int TrackHeightAdjustment => 27;

        private static int DragHeightAdjustment => 24;

        internal static MenuScrollbarState Calculate(
            int xPosition,
            int yPosition,
            int width,
            int height,
            int visibleEntries,
            int listLength,
            int shownEntries,
            bool isDragging,
            int mouseX,
            int mouseY,
            int mouseButton)
        {
            int scrollbarX = xPosition + width - ScrollbarWidth;
            int thumbSize =
                (height - TrackHeightAdjustment) * visibleEntries / listLength;

            if (thumbSize < MinimumThumbSize)
            {
                thumbSize = MinimumThumbSize;
            }

            if (mouseButton == 1 &&
                mouseX >= scrollbarX &&
                mouseX <= scrollbarX + ScrollbarWidth)
            {
                if (mouseY > yPosition &&
                    mouseY < yPosition + ScrollbarWidth &&
                    shownEntries > 0)
                {
                    shownEntries -= 1;
                }

                if (mouseY > yPosition + height - ScrollbarWidth &&
                    mouseY < yPosition + height &&
                    shownEntries < listLength - visibleEntries)
                {
                    shownEntries += 1;
                }
            }

            if (mouseButton == 1 &&
                (mouseX >= scrollbarX && mouseX <= scrollbarX + ScrollbarWidth ||
                    mouseX >= scrollbarX - ScrollbarWidth &&
                    mouseX <= scrollbarX + ScrollbarWidth * 2 &&
                    isDragging))
            {
                if (mouseY > yPosition + ScrollbarWidth &&
                    mouseY < yPosition + height - ScrollbarWidth)
                {
                    isDragging = true;
                    int dragOffset =
                        mouseY - yPosition - ScrollbarWidth - thumbSize / 2;
                    shownEntries =
                        dragOffset * listLength / (height - DragHeightAdjustment);

                    if (shownEntries > listLength - visibleEntries)
                    {
                        shownEntries = listLength - visibleEntries;
                    }

                    if (shownEntries < 0)
                    {
                        shownEntries = 0;
                    }
                }
            }
            else
            {
                isDragging = false;
            }

            int thumbOffset =
                (height - TrackHeightAdjustment - thumbSize) * shownEntries /
                (listLength - visibleEntries);

            return new MenuScrollbarState(
                shownEntries,
                isDragging,
                thumbOffset,
                thumbSize);
        }
    }
}