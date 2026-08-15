namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraTexture64CoordinateFormat :
        ICameraTextureCoordinateFormat
    {
        public static int CoordinateShift => 6;

        public static int CoordinateWrapMask => 0xfff;

        public static int RowMask => CameraTexture64SpanDrawer.TextureRowMask;

        public static int ShadingBandMask => 0xc0000;

        public static int ShadingLightShift => 20;
    }
}