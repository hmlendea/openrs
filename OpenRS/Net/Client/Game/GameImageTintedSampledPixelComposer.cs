namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageTintedSampledPixelComposer :
        IGameImageSampledPixelComposer
    {
        private readonly GameImageColourTint tint;

        internal GameImageTintedSampledPixelComposer(int colour)
        {
            tint = new GameImageColourTint(colour);
        }

        public int Compose(
            int sourceColour,
            int[] pixels,
            int destinationOffset)
            => tint.ApplyPrimary(sourceColour);
    }
}