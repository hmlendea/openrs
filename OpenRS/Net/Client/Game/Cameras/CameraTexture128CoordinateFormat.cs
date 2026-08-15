namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraTexture128CoordinateFormat :
        ICameraTextureCoordinateFormat
    {
        public static int CoordinateShift => 7;

        public static int CoordinateWrapMask => 0x3fff;

        public static int RowMask => 0x3f80;

        public static int ShadingBandMask => 0x600000;

        public static int ShadingLightShift => 23;
    }
}