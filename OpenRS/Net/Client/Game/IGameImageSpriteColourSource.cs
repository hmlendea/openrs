namespace OpenRS.Net.Client.Game
{
    internal interface IGameImageSpriteColourSource
    {
        public GameImageSpriteSample Read(int sourceOffset);
    }
}