using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// ItemDrop mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class ItemDropMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="itemDropEntity">ItemDrop entity.</param>
        internal static ItemDrop ToDomainModel(this ItemDropEntity itemDropEntity) => new()
        {
            ItemId = itemDropEntity.ItemId,
            Amount = itemDropEntity.Amount,
            Weight = itemDropEntity.Weight
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="itemDrop">ItemDrop.</param>
        internal static ItemDropEntity ToDataObject(this ItemDrop itemDrop) => new()
        {
            ItemId = itemDrop.ItemId,
            Amount = itemDrop.Amount,
            Weight = itemDrop.Weight
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="itemDropEntities">ItemDrop entities.</param>
        internal static IEnumerable<ItemDrop> ToDomainModels(this IEnumerable<ItemDropEntity> itemDropEntities)
            => itemDropEntities?.Select(itemDropEntity => itemDropEntity.ToDomainModel());

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="itemDrops">ItemDrops.</param>
        internal static IEnumerable<ItemDropEntity> ToDataObjects(this IEnumerable<ItemDrop> itemDrops)
            => itemDrops?.Select(itemDrop => itemDrop.ToDataObject());
    }
}
