using System;
using System.IO;

namespace OpenRS.Net.Client.Game
{
    public sealed class RscSector
    {
        private readonly SectorTile[] tiles;

        public static short Width => 48;

        public static short Height => 48;

        public RscSector()
        {
            tiles = new SectorTile[Width * Height];

            for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex += 1)
            {
                tiles[tileIndex] = new SectorTile(this);
            }
        }

        public void SetTile(int index, SectorTile tile)
        {
            if (tile is null)
            {
                throw new ArgumentNullException(nameof(tile), "The tile cannot be null.");
            }

            tile.Sector = this;
            tiles[index] = tile;
        }

        public void SetTile(int x, int y, SectorTile tile)
            => SetTile(GetTileIndex(x, y), tile);

        public SectorTile GetTile(int index)
            => tiles[index];

        public SectorTile GetTile(int x, int y)
            => GetTile(GetTileIndex(x, y));

        public static RscSector Unpack(MemoryStream inputStream)
            => RscSectorDecoder.Decode(inputStream);

        private static int GetTileIndex(int positionX, int positionY)
            => positionX * Width + positionY;
    }
}
