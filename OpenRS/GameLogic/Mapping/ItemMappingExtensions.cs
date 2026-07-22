using System;
using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ItemMappingExtensions
    {
        internal static Item ToServiceModel(this ItemEntity itemEntity) => new()
        {
            Id = itemEntity.Id,
            V1Id = itemEntity.V1Id,
            Name = LocalisationManager.GetString(itemEntity.Name),
            Description = LocalisationManager.GetString(itemEntity.Description),
            Command = LocalisationManager.GetString(itemEntity.Command),
            BasePrice = itemEntity.BasePrice,
            SpriteId = itemEntity.SpriteId,
            InventoryPicture = itemEntity.InventoryPicture,
            SpriteName = itemEntity.SpriteName,
            PictureMask = itemEntity.PictureMask,
            IsEquipable = itemEntity.IsEquipable,
            IsPremium = itemEntity.IsPremium != 0,
            IsSpecial = itemEntity.IsSpecial != 0,
            IsStackable = itemEntity.IsStackable != 0,
            IsUnused = itemEntity.IsUnused != 0
        };

        internal static ItemEntity ToDataObject(this Item item) => new()
        {
            Id = item.Id,
            V1Id = item.V1Id,
            Name = item.Name,
            Description = item.Description,
            Command = item.Command,
            BasePrice = item.BasePrice,
            SpriteId = item.SpriteId,
            InventoryPicture = item.InventoryPicture,
            SpriteName = item.SpriteName,
            PictureMask = item.PictureMask,
            IsEquipable = item.IsEquipable,
            IsPremium = Convert.ToInt32(item.IsPremium),
            IsSpecial = Convert.ToInt32(item.IsSpecial),
            IsStackable = Convert.ToInt32(item.IsStackable),
            IsUnused = Convert.ToInt32(item.IsUnused)
        };

        internal static IEnumerable<Item> ToServiceModels(
            this IEnumerable<ItemEntity> itemEntities)
            => itemEntities.Select(itemEntity => itemEntity.ToServiceModel());

        internal static IEnumerable<ItemEntity> ToDataObjects(
            this IEnumerable<Item> items)
            => items.Select(item => item.ToDataObject());
    }
}
