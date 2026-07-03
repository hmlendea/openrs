using System.Collections.Generic;
using System.Linq;

using NuciXNA.Primitives;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// ItemLocation mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class ItemLocationMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="itemLocationEntity">ItemLocation entity.</param>
        internal static ItemLocation ToDomainModel(this ItemLocationEntity itemLocationEntity) => new()
        {
            Coordinates = new Point2D(itemLocationEntity.X, itemLocationEntity.Y),
            Amount = itemLocationEntity.Amount,
            RespawnTime = itemLocationEntity.RespawnTime
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="itemLocation">ItemLocation.</param>
        internal static ItemLocationEntity ToDataObject(this ItemLocation itemLocation) => new()
        {
            X = itemLocation.Coordinates.X,
            Y = itemLocation.Coordinates.Y,
            Amount = itemLocation.Amount,
            RespawnTime = itemLocation.RespawnTime
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="itemLocationEntities">ItemLocation entities.</param>
        internal static IEnumerable<ItemLocation> ToDomainModels(
            this IEnumerable<ItemLocationEntity> itemLocationEntities)
            => itemLocationEntities.Select(itemLocationEntity => itemLocationEntity.ToDomainModel());

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="itemLocations">ItemLocations.</param>
        internal static IEnumerable<ItemLocationEntity> ToDataObjects(
            this IEnumerable<ItemLocation> itemLocations)
            => itemLocations.Select(itemLocation => itemLocation.ToDataObject());
    }
}
