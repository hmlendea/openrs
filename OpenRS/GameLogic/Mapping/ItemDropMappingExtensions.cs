using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ItemDropMappingExtensions
    {
        internal static ItemDrop ToServiceModel(this ItemDropEntity itemDropEntity) => new()
        {
            ItemId = itemDropEntity.ItemId,
            Amount = itemDropEntity.Amount,
            Weight = itemDropEntity.Weight
        };

        internal static ItemDropEntity ToDataObject(this ItemDrop itemDrop) => new()
        {
            ItemId = itemDrop.ItemId,
            Amount = itemDrop.Amount,
            Weight = itemDrop.Weight
        };

        internal static IEnumerable<ItemDrop> ToServiceModels(
            this IEnumerable<ItemDropEntity> itemDropEntities)
            => itemDropEntities?.Select(itemDropEntity => itemDropEntity.ToServiceModel());

        internal static IEnumerable<ItemDropEntity> ToDataObjects(
            this IEnumerable<ItemDrop> itemDrops)
            => itemDrops?.Select(itemDrop => itemDrop.ToDataObject());
    }
}
