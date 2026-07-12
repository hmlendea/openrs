using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Tile mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class TileMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="tileEntity">Tile entity.</param>
        internal static Tile ToDomainModel(this TileEntity tileEntity) => new()
        {
            Colour = tileEntity.Colour,
            Unknown = tileEntity.Unknown,
            Type = tileEntity.Type
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="tile">Tile.</param>
        internal static TileEntity ToDataObject(this Tile tile) => new()
        {
            Colour = tile.Colour,
            Unknown = tile.Unknown,
            Type = tile.Type
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="tileEntities">Tile entities.</param>
        internal static IEnumerable<Tile> ToDomainModels(
            this IEnumerable<TileEntity> tileEntities)
            => tileEntities.Select(tileEntity => tileEntity.ToDomainModel());

        internal static IEnumerable<TileEntity> ToDataObjects(
            this IEnumerable<Tile> tiles)
            => tiles.Select(tile => tile.ToDataObject());
    }
}
