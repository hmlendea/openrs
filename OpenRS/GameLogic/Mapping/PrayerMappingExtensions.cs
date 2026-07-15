using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class PrayerMappingExtensions
    {
        internal static Prayer ToServiceModel(this PrayerEntity prayerEntity) => new()
        {
            Id = prayerEntity.Id,
            V1Id = prayerEntity.V1Id,
            Name = LocalisationManager.GetString(prayerEntity.Name),
            Description = LocalisationManager.GetString(prayerEntity.Description),
            RequiredLevel = prayerEntity.RequiredLevel,
            DrainRate = prayerEntity.DrainRate
        };

        internal static PrayerEntity ToDataObject(this Prayer prayer) => new()
        {
            Id = prayer.Id,
            V1Id = prayer.V1Id,
            Name = prayer.Name,
            Description = prayer.Description,
            RequiredLevel = prayer.RequiredLevel,
            DrainRate = prayer.DrainRate
        };

        internal static IEnumerable<Prayer> ToServiceModels(
            this IEnumerable<PrayerEntity> prayerEntities)
            => prayerEntities.Select(prayerEntity => prayerEntity.ToServiceModel());

        internal static IEnumerable<PrayerEntity> ToDataObjects(
            this IEnumerable<Prayer> prayers)
            => prayers.Select(prayer => prayer.ToDataObject());
    }
}
