using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Elevation mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class ElevationMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="elevationEntity">Elevation entity.</param>
        internal static Elevation ToDomainModel(this ElevationEntity elevationEntity) => new()
        {
            Roof = elevationEntity.Roof,
            Unknown = elevationEntity.Unknown
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="elevation">Elevation.</param>
        internal static ElevationEntity ToDataObject(this Elevation elevation) => new()
        {
            Roof = elevation.Roof,
            Unknown = elevation.Unknown
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="elevationEntities">Elevation entities.</param>
        internal static IEnumerable<Elevation> ToDomainModels(this IEnumerable<ElevationEntity> elevationEntities)
            => elevationEntities.Select(elevationEntity => elevationEntity.ToDomainModel());

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="elevations">Elevations.</param>
        internal static IEnumerable<ElevationEntity> ToDataObjects(
            this IEnumerable<Elevation> elevations)
            => elevations.Select(elevation => elevation.ToDataObject());
    }
}
