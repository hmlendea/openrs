using System.Collections.Generic;
using System.Linq;

using NuciXNA.Primitives;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ItemLocationMappingExtensions
    {
        internal static ItemLocation ToDomainModel(this ItemLocationEntity itemLocationEntity) => new()
        {
            Coordinates = new Point2D(itemLocationEntity.X, itemLocationEntity.Y),
            Amount = itemLocationEntity.Amount,
            RespawnTime = itemLocationEntity.RespawnTime
        };
        internal static ItemLocationEntity ToDataObject(this ItemLocation itemLocation) => new()
        {
            X = itemLocation.Coordinates.X,
            Y = itemLocation.Coordinates.Y,
            Amount = itemLocation.Amount,
            RespawnTime = itemLocation.RespawnTime
        };
        internal static IEnumerable<ItemLocation> ToDomainModels(
            this IEnumerable<ItemLocationEntity> itemLocationEntities)
            => itemLocationEntities.Select(itemLocationEntity => itemLocationEntity.ToDomainModel());
        internal static IEnumerable<ItemLocationEntity> ToDataObjects(
            this IEnumerable<ItemLocation> itemLocations)
            => itemLocations.Select(itemLocation => itemLocation.ToDataObject());
    }
}
