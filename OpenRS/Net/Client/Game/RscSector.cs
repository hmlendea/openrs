using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenRS.Net.Client.Game
{
    public class RscSector
    {
        public static short WIDTH = 48;
        public static short HEIGHT = 48;
        private SectorTile[] tiles;


        public RscSector()
        {
            tiles = new SectorTile[RscSector.WIDTH * RscSector.HEIGHT];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new SectorTile(this);

            }
        }

        public void setTile(int x, int y, SectorTile t)
        {
            setTile(x * RscSector.WIDTH + y, t);
        }

        public void setTile(int i, SectorTile t)
        {
            tiles[i] = t;
        }

        public SectorTile getTile(int x, int y)
        {
            return getTile(x * RscSector.WIDTH + y);
        }

        public SectorTile getTile(int i)
        {
            return tiles[i];
        }

        public static RscSector unpack(MemoryStream indata)
        {
            int length = RscSector.WIDTH * RscSector.HEIGHT;
            if (indata.Remaining() < (10 * length))
            {
                throw new IOException("Provided buffer too short");
            }
            RscSector sector = new RscSector();

            for (int i = 0; i < length; i++)
            {
                sector.setTile(i, SectorTile.unpack(indata));
            }
            return sector;
        }
    }
}
