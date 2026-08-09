namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofFaceContext
    {
        public int TileX { get; set; }

        public int TileY { get; set; }

        public int RoofColour { get; set; }

        public int DiagonalWall { get; set; }

        public int TopLeftElevation { get; set; }

        public int TopRightElevation { get; set; }

        public int BottomRightElevation { get; set; }

        public int BottomLeftElevation { get; set; }

        public int TopLeftVertexIndex { get; set; }

        public int TopRightVertexIndex { get; set; }

        public int BottomRightVertexIndex { get; set; }

        public int BottomLeftVertexIndex { get; set; }
    }
}