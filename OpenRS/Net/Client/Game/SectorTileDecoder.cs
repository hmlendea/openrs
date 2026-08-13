using System;
using System.IO;
using System.Text;

namespace OpenRS.Net.Client.Game
{
    internal static class SectorTileDecoder
    {
        internal static SectorTile Decode(MemoryStream inputStream)
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

            if (inputStream.Remaining() < SectorTile.SerialisedByteCount)
            {
                throw new IOException(
                    $"The provided buffer is too short to unpack a sector tile. " +
                    $"At least {SectorTile.SerialisedByteCount} bytes are required.");
            }
        }
    }
}