namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageBlendedSampledPixelComposer :
        IGameImageSampledPixelComposer
    {
        private readonly int blendComplement;
        private readonly int blendFactor;

        internal GameImageBlendedSampledPixelComposer(
            int blendFactor,
            int blendComplement)
        {
            this.blendFactor = blendFactor;
            this.blendComplement = blendComplement;
        }

        public int Compose(
            int sourceColour,
            int[] pixels,
            int destinationOffset)
            => GameImageColourBlender.Blend(
                sourceColour,
                pixels[destinationOffset],
                blendFactor,
                blendComplement);
    }
}