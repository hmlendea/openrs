using System.IO;

namespace OpenRS.Net.Client.Game
{
    public sealed class SectorTile
    {
        public byte groundElevation = 0;
        public byte groundTexture = 0;
        public byte roofTexture = 0;
        public byte horizontalWall = 0;
        public byte verticalWall = 0;
        public int diagonalWalls = 0;
        public byte groundOverlay = 0;
		public SectorTile() {}
		public SectorTile(RscSector sector)
		{
			this.Sector = sector;
		}
        public static SectorTile unpack(MemoryStream indata)
        {
            if (indata.Remaining() < 10)
            {
                throw new IOException("Provided buffer too short");
            }
            SectorTile tile = new();
            BinaryReader binReader = new(indata);
            tile.groundElevation = binReader.ReadByte();
            tile.groundTexture = binReader.ReadByte();
            tile.groundOverlay = binReader.ReadByte();
            tile.roofTexture = binReader.ReadByte();
            tile.horizontalWall = binReader.ReadByte();
            tile.verticalWall = binReader.ReadByte();
            tile.diagonalWalls = binReader.ReadInt32();
            binReader.Close();

            return tile;
        }

		public RscSector Sector { get; set; }
	}
}
