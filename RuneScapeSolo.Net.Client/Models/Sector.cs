using System.IO;
using RuneScapeSolo.Net.Client.Extensions;

namespace RuneScapeSolo.Net.Client.Models
{
    public class Sector
    {
        public static short WIDTH = 48;
        public static short HEIGHT = 48;
        Tile[] tiles;


        public Sector()
        {
            tiles = new Tile[Sector.WIDTH * Sector.HEIGHT];
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = new Tile(this);

            }
        }

        public void setTile(int x, int y, Tile t)
        {
            setTile(x * Sector.WIDTH + y, t);
        }

        public void setTile(int i, Tile t)
        {
            tiles[i] = t;
        }

        public Tile getTile(int x, int y)
        {
            return getTile(x * Sector.WIDTH + y);
        }

        public Tile getTile(int i)
        {
            return tiles[i];
        }

        public static Sector unpack(MemoryStream indata)
        {
            int length = Sector.WIDTH * Sector.HEIGHT;
            if (indata.Remaining() < (10 * length))
            {
                throw new IOException("Provided buffer too short");
            }
            Sector sector = new Sector();

            for (int i = 0; i < length; i++)
            {
                sector.setTile(i, Tile.unpack(indata));
            }
            return sector;
        }
    }
}
