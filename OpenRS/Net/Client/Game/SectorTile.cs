using System;
using System.IO;
using System.Text;

namespace OpenRS.Net.Client.Game
{
    public sealed class SectorTile
    {
        private static int TileDataSize => sizeof(byte) * 6 + sizeof(int);

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
        {
            ValidateInputStream(inputStream);

            using BinaryReader binaryReader = new(inputStream, Encoding.UTF8, true);

            return ReadTile(binaryReader);
        }

        private static SectorTile ReadTile(BinaryReader binaryReader) => new()
        {
            GroundElevation = binaryReader.ReadByte(),
            GroundTexture = binaryReader.ReadByte(),
            GroundOverlay = binaryReader.ReadByte(),
            RoofTexture = binaryReader.ReadByte(),
            HorizontalWall = binaryReader.ReadByte(),
            VerticalWall = binaryReader.ReadByte(),
            DiagonalWalls = binaryReader.ReadInt32(),
        };

        private static void ValidateInputStream(MemoryStream inputStream)
        {
            if (inputStream is null)
            {
                throw new ArgumentNullException(
                    nameof(inputStream),
                    "The input stream cannot be null.");
            }

            if (inputStream.Remaining() < TileDataSize)
            {
                throw new IOException(
                    $"The provided buffer is too short to unpack a sector tile. " +
                    $"At least {TileDataSize} bytes are required.");
            }
        }
    }
}
