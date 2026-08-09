using System;
using System.IO;

namespace OpenRS.Net.Client.Game
{
    public sealed class RscSector
    {
        private readonly SectorTile[] tiles;

        public static short Width => 48;

        public static short Height => 48;

        private static int SectorTileByteSize => sizeof(byte) * 6 + sizeof(int);

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
            => SetTile(x * Width + y, tile);

        public SectorTile GetTile(int index)
            => tiles[index];

        public SectorTile GetTile(int x, int y)
            => GetTile(x * Width + y);

        public static RscSector Unpack(MemoryStream inputStream)
        {
            if (inputStream is null)
            {
                throw new ArgumentNullException(
                    nameof(inputStream),
                    "The input stream cannot be null.");
            }

            int tileCount = Width * Height;
            int requiredByteCount = SectorTileByteSize * tileCount;

            if (inputStream.Remaining() < requiredByteCount)
            {
                throw new IOException(
                    $"The provided buffer is too short to unpack a sector. " +
                    $"At least {requiredByteCount} bytes are required.");
            }

            RscSector sector = new();

            for (int tileIndex = 0; tileIndex < tileCount; tileIndex += 1)
            {
                sector.SetTile(tileIndex, SectorTile.Unpack(inputStream));
            }

            return sector;
        }
    }
}
