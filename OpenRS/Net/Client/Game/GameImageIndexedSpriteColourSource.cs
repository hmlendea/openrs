namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageIndexedSpriteColourSource(
        sbyte[] colourIndexes,
        int[] colourLookup) : IGameImageSpriteColourSource
    {
        public GameImageSpriteSample Read(int sourceOffset)
        {
            sbyte colourIndex = colourIndexes[sourceOffset];

            if (colourIndex == 0)
            {
                return new GameImageSpriteSample(0, false);
            }

            return new GameImageSpriteSample(
                colourLookup[colourIndex & 0xff],
                true);
        }
    }
}