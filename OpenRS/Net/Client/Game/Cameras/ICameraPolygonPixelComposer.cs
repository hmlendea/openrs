namespace OpenRS.Net.Client.Game.Cameras
{
    internal interface ICameraPolygonPixelComposer
    {
        public static abstract int Compose(
            int sourceColour,
            int[] pixels,
            int pixelBufferOffset);
    }
}