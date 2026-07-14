using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ItemMappingExtensions
    {
        internal static Item ToDomainModel(this ItemEntity itemEntity) => new()
        {
            Id = itemEntity.Id,
            Name = LocalisationManager.GetString(itemEntity.Name),
            Description = LocalisationManager.GetString(itemEntity.Description),
            Command = LocalisationManager.GetString(itemEntity.Command),
            BasePrice = itemEntity.BasePrice,
            SpriteId = itemEntity.SpriteId,
            InventoryPicture = itemEntity.InventoryPicture,
            SpriteName = itemEntity.SpriteName,
            PictureMask = itemEntity.PictureMask,
            IsEquipable = itemEntity.IsEquipable,
            IsPremium = itemEntity.IsPremium,
            IsSpecial = itemEntity.IsSpecial,
            IsStackable = itemEntity.IsStackable,
            IsUnused = itemEntity.IsUnused
        };

        internal static ItemEntity ToDataObject(this Item item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Command = item.Command,
            BasePrice = item.BasePrice,
            SpriteId = item.SpriteId,
            InventoryPicture = item.InventoryPicture,
            SpriteName = item.SpriteName,
            PictureMask = item.PictureMask,
            IsEquipable = item.IsEquipable,
            IsPremium = item.IsPremium,
            IsSpecial = item.IsSpecial,
            IsStackable = item.IsStackable,
            IsUnused = item.IsUnused
        };

        internal static IEnumerable<Item> ToDomainModels(
            this IEnumerable<ItemEntity> itemEntities)
            => itemEntities.Select(itemEntity => itemEntity.ToDomainModel());

        internal static IEnumerable<ItemEntity> ToDataObjects(
            this IEnumerable<Item> items)
            => items.Select(item => item.ToDataObject());
    }
}
