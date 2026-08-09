using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class ElevationMappingExtensions
    {
        internal static Elevation ToServiceModel(this ElevationEntity elevationEntity) => new()
        {
            V1Id = elevationEntity.V1Id,
            Roof = elevationEntity.Roof,
            Colour = elevationEntity.Colour
        };

        internal static ElevationEntity ToDataObject(this Elevation elevation) => new()
        {
            V1Id = elevation.V1Id,
            Roof = elevation.Roof,
            Colour = elevation.Colour
        };

        internal static IEnumerable<Elevation> ToServiceModels(
            this IEnumerable<ElevationEntity> elevationEntities)
            => elevationEntities.Select(elevationEntity => elevationEntity.ToServiceModel());

        internal static IEnumerable<ElevationEntity> ToDataObjects(
            this IEnumerable<Elevation> elevations)
            => elevations.Select(elevation => elevation.ToDataObject());
    }
}
