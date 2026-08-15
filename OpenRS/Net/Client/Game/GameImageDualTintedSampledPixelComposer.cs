namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageDualTintedSampledPixelComposer :
        IGameImageSampledPixelComposer
    {
        private readonly GameImageColourTint primaryTint;
        private readonly GameImageColourTint secondaryTint;

        internal GameImageDualTintedSampledPixelComposer(
            int primaryColour,
            int secondaryColour)
        {
            primaryTint = new GameImageColourTint(primaryColour);
            secondaryTint = new GameImageColourTint(secondaryColour);
        }

        public int Compose(
            int sourceColour,
            int[] pixels,
            int destinationOffset)
            => primaryTint.ApplyPrimaryAndSecondary(
                sourceColour,
                secondaryTint);
    }
}