using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Prayer mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class PrayerMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="prayerEntity">Prayer entity.</param>
        internal static Prayer ToDomainModel(this PrayerEntity prayerEntity) => new()
        {
            Id = prayerEntity.Id,
            Name = prayerEntity.Name,
            Description = prayerEntity.Description,
            RequiredLevel = prayerEntity.RequiredLevel,
            DrainRate = prayerEntity.DrainRate
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="prayer">Prayer.</param>
        internal static PrayerEntity ToDataObject(this Prayer prayer) => new()
        {
            Id = prayer.Id,
            Name = prayer.Name,
            Description = prayer.Description,
            RequiredLevel = prayer.RequiredLevel,
            DrainRate = prayer.DrainRate
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="prayerEntities">Prayer entities.</param>
        internal static IEnumerable<Prayer> ToDomainModels(
            this IEnumerable<PrayerEntity> prayerEntities)
            => prayerEntities.Select(prayerEntity => prayerEntity.ToDomainModel());

        internal static IEnumerable<PrayerEntity> ToDataObjects(
            this IEnumerable<Prayer> prayers)
            => prayers.Select(prayer => prayer.ToDataObject());
    }
}
