using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ElevationMappingExtensions
    {
        internal static Elevation ToDomainModel(this ElevationEntity elevationEntity) => new()
        {
            Roof = elevationEntity.Roof,
            Unknown = elevationEntity.Unknown
        };
        internal static ElevationEntity ToDataObject(this Elevation elevation) => new()
        {
            Roof = elevation.Roof,
            Unknown = elevation.Unknown
        };
        internal static IEnumerable<Elevation> ToDomainModels(this IEnumerable<ElevationEntity> elevationEntities)
            => elevationEntities.Select(elevationEntity => elevationEntity.ToDomainModel());
        internal static IEnumerable<ElevationEntity> ToDataObjects(
            this IEnumerable<Elevation> elevations)
            => elevations.Select(elevation => elevation.ToDataObject());
    }
}
