using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class NpcLocationMappingExtensions
    {
        internal static NpcLocation ToServiceModel(
            this NpcLocationEntity npcLocationEntity) => new()
        {
            InitialCoordinates = new(
                npcLocationEntity.InitialXCoordinate,
                npcLocationEntity.InitialYCoordinate),
            MinimumCoordinates = new(
                npcLocationEntity.MinimumXCoordinate,
                npcLocationEntity.MinimumYCoordinate),
            MaximumCoordinates = new(
                npcLocationEntity.MaximumXCoordinate,
                npcLocationEntity.MaximumYCoordinate)
        };

        internal static NpcLocationEntity ToDataObject(this NpcLocation npcLocation) => new()
        {
            InitialXCoordinate = npcLocation.InitialCoordinates.X,
            InitialYCoordinate = npcLocation.InitialCoordinates.Y,
            MinimumXCoordinate = npcLocation.MinimumCoordinates.X,
            MinimumYCoordinate = npcLocation.MinimumCoordinates.Y,
            MaximumXCoordinate = npcLocation.MaximumCoordinates.X,
            MaximumYCoordinate = npcLocation.MaximumCoordinates.Y
        };

        internal static IEnumerable<NpcLocation> ToServiceModels(
            this IEnumerable<NpcLocationEntity> npcLocationEntities)
            => npcLocationEntities.Select(
                npcLocationEntity => npcLocationEntity.ToServiceModel());

        internal static IEnumerable<NpcLocationEntity> ToDataObjects(
            this IEnumerable<NpcLocation> npcLocations)
            => npcLocations.Select(npcLocation => npcLocation.ToDataObject());
    }
}
