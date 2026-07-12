using System.IO;

namespace OpenRS.Net.Client.Game
{
    public sealed class RscSector
    {
        public static short WIDTH = 48;
        public static short HEIGHT = 48;
        private SectorTile[] tiles;


        public RscSector()
        {
            tiles = new SectorTile[WIDTH * HEIGHT];
            for (int i = 0; i < tiles.Length; i += 1)
            {
                tiles[i] = new SectorTile(this);

            }
        }

        public void SetTile(int x, int y, SectorTile t)
        {
            SetTile(x * WIDTH + y, t);
        }

        public void SetTile(int i, SectorTile t)
        {
            tiles[i] = t;
        }

        public SectorTile GetTile(int x, int y)
        {
            return GetTile(x * WIDTH + y);
        }

        public SectorTile GetTile(int i)
        {
            return tiles[i];
        }

        public static RscSector unpack(MemoryStream indata)
        {
            int length = WIDTH * HEIGHT;
            if (indata.Remaining() < (10 * length))
            {
                throw new IOException("Provided buffer too short");
            }
            RscSector sector = new();

            for (int i = 0; i < length; i += 1)
            {
                sector.SetTile(i, SectorTile.unpack(indata));
            }
            return sector;
        }
    }
}
