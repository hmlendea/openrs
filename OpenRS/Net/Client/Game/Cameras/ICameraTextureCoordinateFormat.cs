namespace OpenRS.Net.Client.Game.Cameras
{
    internal interface ICameraTextureCoordinateFormat
    {
        public static abstract int CoordinateShift { get; }

        public static abstract int CoordinateWrapMask { get; }

        public static abstract int RowMask { get; }

        public static abstract int ShadingBandMask { get; }

        public static abstract int ShadingLightShift { get; }
    }
}