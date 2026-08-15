namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageIndexedPalette
    {
        internal sbyte[] ColourIndexes { get; }

        internal int[] Colours { get; }

        internal GameImageIndexedPalette(
            sbyte[] colourIndexes,
            int[] colours)
        {
            ColourIndexes = colourIndexes;
            Colours = colours;
        }
    }
}