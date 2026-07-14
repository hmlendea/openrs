using NuciXNA.Primitives;

namespace OpenRS.Models
{
    public sealed class Sector
    {
        public static readonly Size2D Size = new(48, 48);

        private readonly WorldTile[] tiles;

        public Sector()
        {
            tiles = new WorldTile[Size.Width * Size.Height];

            for (int tileIndex = 0; tileIndex < Size.Area; tileIndex += 1)
            {
                tiles[tileIndex] = new();
            }
        }

        public void SetTile(int x, int y, WorldTile tile) => SetTile(x * Size.Width + y, tile);

        public void SetTile(int index, WorldTile tile) => tiles[index] = tile;

        public WorldTile GetTile(int x, int y) => GetTile(x * Size.Width + y);

        public WorldTile GetTile(int index) => tiles[index];
    }
}
