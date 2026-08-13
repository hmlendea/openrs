using System;
using System.IO;

namespace OpenRS.Net.Client.Game
{
    internal static class RscSectorDecoder
    {
        internal static RscSector Decode(MemoryStream inputStream)
        {
            if (inputStream is null)
            {
                throw new ArgumentNullException(
                    nameof(inputStream),
                    "The input stream cannot be null.");
            }

            int tileCount = RscSector.Width * RscSector.Height;
            int requiredByteCount = SectorTile.SerialisedByteCount * tileCount;

            if (inputStream.Remaining() < requiredByteCount)
            {
                throw new IOException(
                    $"The provided buffer is too short to unpack a sector. " +
                    $"At least {requiredByteCount} bytes are required.");
            }

            RscSector sector = new();

            for (int tileIndex = 0; tileIndex < tileCount; tileIndex += 1)
            {
                sector.SetTile(tileIndex, SectorTileDecoder.Decode(inputStream));
            }

            return sector;
        }
    }
}