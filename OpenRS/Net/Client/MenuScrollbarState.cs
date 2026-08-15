namespace OpenRS.Net.Client
{
    internal readonly struct MenuScrollbarState
    {
        internal int ShownEntries { get; }

        internal bool IsDragging { get; }

        internal int ThumbOffset { get; }

        internal int ThumbSize { get; }

        internal MenuScrollbarState(
            int shownEntries,
            bool isDragging,
            int thumbOffset,
            int thumbSize)
        {
            ShownEntries = shownEntries;
            IsDragging = isDragging;
            ThumbOffset = thumbOffset;
            ThumbSize = thumbSize;
        }
    }
}