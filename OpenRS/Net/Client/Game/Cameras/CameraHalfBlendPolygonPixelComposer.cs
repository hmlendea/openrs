namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraHalfBlendPolygonPixelComposer :
        ICameraPolygonPixelComposer
    {
        private static int HalfBlendChannelMask => 0x7f7f7f;

        public static int Compose(
            int sourceColour,
            int[] pixels,
            int pixelBufferOffset)
            => sourceColour +
                (pixels[pixelBufferOffset + 1] >> 1 & HalfBlendChannelMask);
    }
}