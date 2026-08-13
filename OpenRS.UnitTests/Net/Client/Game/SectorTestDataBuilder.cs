using System.IO;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    internal static class SectorTestDataBuilder
    {
        internal static int TileByteCount => 10;

        internal static int SectorTileCount => RscSector.Width * RscSector.Height;

        internal static int SectorByteCount => TileByteCount * SectorTileCount;

        internal static int DiagonalWallsValue => 0x12345678;

        internal static byte[] BuildTileData()
        {
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);
            writer.Write((byte)4);
            writer.Write((byte)8);
            writer.Write((byte)16);
            writer.Write((byte)32);
            writer.Write((byte)42);
            writer.Write((byte)48);
            writer.Write(DiagonalWallsValue);

            return stream.ToArray();
        }

        internal static byte[] BuildSectorData()
        {
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);

            for (int tileIndex = 0; tileIndex < SectorTileCount; tileIndex += 1)
            {
                writer.Write((byte)tileIndex);
                writer.Write((byte)(tileIndex + 1));
                writer.Write((byte)(tileIndex + 2));
                writer.Write((byte)(tileIndex + 3));
                writer.Write((byte)(tileIndex + 4));
                writer.Write((byte)(tileIndex + 5));
                writer.Write(tileIndex);
            }

            return stream.ToArray();
        }
    }
}