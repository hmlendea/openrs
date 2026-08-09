using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class TileMappingExtensions
    {
        internal static Tile ToServiceModel(this TileEntity tileEntity) => new()
        {
            V1Id = tileEntity.V1Id,
            Colour = tileEntity.Colour,
            Unknown = tileEntity.Unknown,
            Type = tileEntity.Type
        };

        internal static TileEntity ToDataObject(this Tile tile) => new()
        {
            V1Id = tile.V1Id,
            Colour = tile.Colour,
            Unknown = tile.Unknown,
            Type = tile.Type
        };

        internal static IEnumerable<Tile> ToServiceModels(
            this IEnumerable<TileEntity> tileEntities)
            => tileEntities.Select(tileEntity => tileEntity.ToServiceModel());

        internal static IEnumerable<TileEntity> ToDataObjects(
            this IEnumerable<Tile> tiles)
            => tiles.Select(tile => tile.ToDataObject());
    }
}
