using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal static class SectionPacketCoordinates
    {
        internal static int GetMapTileCoordinate(int sectionCoordinate, int gridSize)
            => (sectionCoordinate * gridSize) + PacketHandlerConstants.TileOffset;

        internal static int GetSectionCoordinateFromByte(
            int sectionBase,
            sbyte[] packetData,
            int offset)
            => sectionBase + (packetData[offset] >> PacketHandlerConstants.SectionShiftSize);

        internal static int GetSectionCoordinateFromSignedShort(
            int sectionBase,
            sbyte[] packetData,
            int offset)
            =>
                sectionBase +
                (BinaryDataReader.GetSignedShort(packetData, offset) >>
                PacketHandlerConstants.SectionShiftSize);

        internal static int GetWorldTileCoordinate(
            int sectionCoordinate,
            int areaOffset,
            int gridSize)
            => ((sectionCoordinate + areaOffset) * gridSize) + PacketHandlerConstants.TileOffset;

        internal static bool IsOutsideSection(int tileX, int tileY, int sectionX, int sectionY)
            =>
                (tileX >> PacketHandlerConstants.SectionShiftSize) != sectionX ||
                (tileY >> PacketHandlerConstants.SectionShiftSize) != sectionY;
    }
}