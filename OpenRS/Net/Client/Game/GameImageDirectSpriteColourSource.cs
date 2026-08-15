namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageDirectSpriteColourSource(int[] colours) :
        IGameImageSpriteColourSource
    {
        public GameImageSpriteSample Read(int sourceOffset)
        {
            int colour = colours[sourceOffset];

            return new GameImageSpriteSample(colour, colour != 0);
        }
    }
}