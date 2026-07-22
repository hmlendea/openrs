namespace OpenRS.Net.Client.Game
{
    internal sealed class VertexProjectionContext
    {
        public int OriginX { get; set; }

        public int OriginY { get; set; }

        public int OriginZ { get; set; }

        public int RotationZAngle { get; set; }

        public int RotationYAngle { get; set; }

        public int RotationXAngle { get; set; }

        public int SinZ { get; set; }

        public int CosZ { get; set; }

        public int SinY { get; set; }

        public int CosY { get; set; }

        public int SinX { get; set; }

        public int CosX { get; set; }

        public int ProjectionScale { get; set; }

        public int NearPlane { get; set; }
    }
}
