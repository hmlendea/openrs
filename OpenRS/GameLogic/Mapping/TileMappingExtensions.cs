using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class TileMappingExtensions
    {
        internal static Tile ToDomainModel(this TileEntity tileEntity) => new()
        {
            Colour = tileEntity.Colour,
            Unknown = tileEntity.Unknown,
            Type = tileEntity.Type
        };

        internal static TileEntity ToDataObject(this Tile tile) => new()
        {
            Colour = tile.Colour,
            Unknown = tile.Unknown,
            Type = tile.Type
        };
        internal static IEnumerable<Tile> ToDomainModels(
            this IEnumerable<TileEntity> tileEntities)
            => tileEntities.Select(tileEntity => tileEntity.ToDomainModel());

        internal static IEnumerable<TileEntity> ToDataObjects(
            this IEnumerable<Tile> tiles)
            => tiles.Select(tile => tile.ToDataObject());
    }
}
