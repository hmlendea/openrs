using System.Collections.Generic;
using System.Linq;

using NuciXNA.Primitives;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// NpcLocation mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class NpcLocationMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="npcLocationEntity">NpcLocation entity.</param>
        internal static NpcLocation ToDomainModel(this NpcLocationEntity npcLocationEntity) => new()
        {
            InitialCoordinates = new Point2D(npcLocationEntity.InitialX, npcLocationEntity.InitialY),
            MinimumCoordinates = new Point2D(npcLocationEntity.MinX, npcLocationEntity.MinY),
            MaximumCoordinates = new Point2D(npcLocationEntity.MaxX, npcLocationEntity.MaxY)
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="npcLocation">NpcLocation.</param>
        internal static NpcLocationEntity ToDataObject(this NpcLocation npcLocation) => new()
        {
            InitialX = npcLocation.InitialCoordinates.X,
            InitialY = npcLocation.InitialCoordinates.Y,
            MinX = npcLocation.MinimumCoordinates.X,
            MinY = npcLocation.MinimumCoordinates.Y,
            MaxX = npcLocation.MaximumCoordinates.X,
            MaxY = npcLocation.MaximumCoordinates.Y
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="npcLocationEntities">NpcLocation entities.</param>
        internal static IEnumerable<NpcLocation> ToDomainModels(
            this IEnumerable<NpcLocationEntity> npcLocationEntities)
            => npcLocationEntities.Select(npcLocationEntity => npcLocationEntity.ToDomainModel());

        internal static IEnumerable<NpcLocationEntity> ToDataObjects(
            this IEnumerable<NpcLocation> npcLocations)
            => npcLocations.Select(npcLocation => npcLocation.ToDataObject());
    }
}
