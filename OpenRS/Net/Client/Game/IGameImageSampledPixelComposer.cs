namespace OpenRS.Net.Client.Game
{
    internal interface IGameImageSampledPixelComposer
    {
        public int Compose(
            int sourceColour,
            int[] pixels,
            int destinationOffset);
    }
}