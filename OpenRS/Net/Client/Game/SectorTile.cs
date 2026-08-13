using System.IO;

namespace OpenRS.Net.Client.Game
{
    public sealed class SectorTile
    {
        internal static int SerialisedByteCount => sizeof(byte) * 6 + sizeof(int);

        public byte GroundElevation { get; set; }

        public byte GroundTexture { get; set; }

        public byte RoofTexture { get; set; }

        public byte HorizontalWall { get; set; }

        public byte VerticalWall { get; set; }

        public int DiagonalWalls { get; set; }

        public byte GroundOverlay { get; set; }

        public RscSector Sector { get; set; }

        public SectorTile()
        {
        }

        public SectorTile(RscSector sector)
        {
            Sector = sector;
        }

        public static SectorTile Unpack(MemoryStream inputStream)
            => SectorTileDecoder.Decode(inputStream);
    }
}
