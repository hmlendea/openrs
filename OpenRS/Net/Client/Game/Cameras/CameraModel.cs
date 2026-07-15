namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class CameraModel
    {
        public int BoundsMinX { get; set; }

        public int BoundsMinY { get; set; }

        public int BoundsMaxX { get; set; }

        public int BoundsMaxY { get; set; }

        public int BoundsMinZ { get; set; }

        public int BoundsMaxZ { get; set; }

        public GameObject SourceObject { get; set; }

        public int FaceIndex { get; set; }

        public int Scale { get; set; }

        public int NormalX { get; set; }

        public int NormalY { get; set; }

        public int NormalZ { get; set; }

        public int VisibilityDot { get; set; }

        public int CurrentTextureIndex { get; set; }

        public bool IsSorted { get; set; }

        public int SortIndex { get; set; }

        public int DependencyIndex { get; set; } = -1;
    }
}
