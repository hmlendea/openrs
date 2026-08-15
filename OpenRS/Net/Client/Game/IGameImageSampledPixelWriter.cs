namespace OpenRS.Net.Client.Game
{
    internal interface IGameImageSampledPixelWriter
    {
        public void Write(
            int sourceColour,
            int[] pixels,
            int destinationOffset);
    }
}