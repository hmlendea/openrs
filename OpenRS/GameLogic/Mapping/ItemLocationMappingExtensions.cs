using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ItemLocationMappingExtensions
    {
        internal static ItemLocation ToServiceModel(
            this ItemLocationEntity itemLocationEntity) => new()
        {
            Coordinates = new(itemLocationEntity.XCoordinate, itemLocationEntity.YCoordinate),
            Amount = itemLocationEntity.Amount,
            RespawnTime = itemLocationEntity.RespawnTime
        };

        internal static ItemLocationEntity ToDataObject(this ItemLocation itemLocation) => new()
        {
            XCoordinate = itemLocation.Coordinates.X,
            YCoordinate = itemLocation.Coordinates.Y,
            Amount = itemLocation.Amount,
            RespawnTime = itemLocation.RespawnTime
        };

        internal static IEnumerable<ItemLocation> ToServiceModels(
            this IEnumerable<ItemLocationEntity> itemLocationEntities)
            => itemLocationEntities.Select(
                itemLocationEntity => itemLocationEntity.ToServiceModel());

        internal static IEnumerable<ItemLocationEntity> ToDataObjects(
            this IEnumerable<ItemLocation> itemLocations)
            => itemLocations.Select(itemLocation => itemLocation.ToDataObject());
    }
}
