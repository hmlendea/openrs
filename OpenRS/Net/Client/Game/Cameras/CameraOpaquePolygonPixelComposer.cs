namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraOpaquePolygonPixelComposer :
        ICameraPolygonPixelComposer
    {
        public static int Compose(
            int sourceColour,
            int[] pixels,
            int pixelBufferOffset)
            => sourceColour;
    }
}