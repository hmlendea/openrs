namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageOpaqueSampledPixelComposer :
        IGameImageSampledPixelComposer
    {
        public int Compose(
            int sourceColour,
            int[] pixels,
            int destinationOffset)
            => sourceColour;
    }
}