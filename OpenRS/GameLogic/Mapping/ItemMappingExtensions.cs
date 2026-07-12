using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Item mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class ItemMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="itemEntity">Item entity.</param>
        internal static Item ToDomainModel(this ItemEntity itemEntity) => new()
        {
            Id = itemEntity.Id,
            Name = itemEntity.Name,
            Description = itemEntity.Description,
            Command = itemEntity.Command,
            BasePrice = itemEntity.BasePrice,
            SpriteId = itemEntity.SpriteId,
            InventoryPicture = itemEntity.InventoryPicture,
            PictureMask = itemEntity.PictureMask,
            IsEquipable = itemEntity.IsEquipable,
            IsPremium = itemEntity.IsPremium,
            IsSpecial = itemEntity.IsSpecial,
            IsStackable = itemEntity.IsStackable,
            IsUnused = itemEntity.IsUnused
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="item">Item.</param>
        internal static ItemEntity ToDataObject(this Item item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Command = item.Command,
            BasePrice = item.BasePrice,
            SpriteId = item.SpriteId,
            InventoryPicture = item.InventoryPicture,
            PictureMask = item.PictureMask,
            IsEquipable = item.IsEquipable,
            IsPremium = item.IsPremium,
            IsSpecial = item.IsSpecial,
            IsStackable = item.IsStackable,
            IsUnused = item.IsUnused
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="itemEntities">Item entities.</param>
        internal static IEnumerable<Item> ToDomainModels(
            this IEnumerable<ItemEntity> itemEntities)
            => itemEntities.Select(itemEntity => itemEntity.ToDomainModel());

        internal static IEnumerable<ItemEntity> ToDataObjects(
            this IEnumerable<Item> items)
            => items.Select(item => item.ToDataObject());
    }
}
