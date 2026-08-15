namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageOpaqueSampledPixelWriter :
        IGameImageSampledPixelWriter
    {
        public void Write(
            int sourceColour,
            int[] pixels,
            int destinationOffset)
            => pixels[destinationOffset] = sourceColour;
    }
}