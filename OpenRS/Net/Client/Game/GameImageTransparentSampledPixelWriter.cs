namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageTransparentSampledPixelWriter :
        IGameImageSampledPixelWriter
    {
        public void Write(
            int sourceColour,
            int[] pixels,
            int destinationOffset)
        {
            if (sourceColour != 0)
            {
                pixels[destinationOffset] = sourceColour;
            }
        }
    }
}