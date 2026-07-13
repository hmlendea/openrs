using System.Collections.Generic;
using System.Linq;

using NuciXNA.Primitives;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class NpcLocationMappingExtensions
    {
        internal static NpcLocation ToDomainModel(this NpcLocationEntity npcLocationEntity) => new()
        {
            InitialCoordinates = new Point2D(npcLocationEntity.InitialX, npcLocationEntity.InitialY),
            MinimumCoordinates = new Point2D(npcLocationEntity.MinX, npcLocationEntity.MinY),
            MaximumCoordinates = new Point2D(npcLocationEntity.MaxX, npcLocationEntity.MaxY)
        };
        internal static NpcLocationEntity ToDataObject(this NpcLocation npcLocation) => new()
        {
            InitialX = npcLocation.InitialCoordinates.X,
            InitialY = npcLocation.InitialCoordinates.Y,
            MinX = npcLocation.MinimumCoordinates.X,
            MinY = npcLocation.MinimumCoordinates.Y,
            MaxX = npcLocation.MaximumCoordinates.X,
            MaxY = npcLocation.MaximumCoordinates.Y
        };
        internal static IEnumerable<NpcLocation> ToDomainModels(
            this IEnumerable<NpcLocationEntity> npcLocationEntities)
            => npcLocationEntities.Select(npcLocationEntity => npcLocationEntity.ToDomainModel());

        internal static IEnumerable<NpcLocationEntity> ToDataObjects(
            this IEnumerable<NpcLocation> npcLocations)
            => npcLocations.Select(npcLocation => npcLocation.ToDataObject());
    }
}
