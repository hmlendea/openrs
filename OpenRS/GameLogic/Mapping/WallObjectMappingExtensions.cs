using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// WallObject mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class WallObjectMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="wallObjectEntity">WallObject entity.</param>
        internal static WallObject ToDomainModel(this WallObjectEntity wallObjectEntity) => new()
        {
            Name = wallObjectEntity.Name,
            Description = wallObjectEntity.Description,
            Command1 = wallObjectEntity.Command1,
            Command2 = wallObjectEntity.Command2,
            Type = wallObjectEntity.Type,
            Unknown = wallObjectEntity.Unknown,
            ModelHeight = wallObjectEntity.ModelHeight,
            ModelFaceBack = wallObjectEntity.ModelFaceBack,
            ModelFaceFront = wallObjectEntity.ModelFaceFront
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="wallObject">WallObject.</param>
        internal static WallObjectEntity ToDataObject(this WallObject wallObject) => new()
        {
            Name = wallObject.Name,
            Description = wallObject.Description,
            Command1 = wallObject.Command1,
            Command2 = wallObject.Command2,
            Type = wallObject.Type,
            Unknown = wallObject.Unknown,
            ModelHeight = wallObject.ModelHeight,
            ModelFaceBack = wallObject.ModelFaceBack,
            ModelFaceFront = wallObject.ModelFaceFront
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="wallObjectEntities">WallObject entities.</param>
        internal static IEnumerable<WallObject> ToDomainModels(
            this IEnumerable<WallObjectEntity> wallObjectEntities)
            => wallObjectEntities.Select(wallObjectEntity => wallObjectEntity.ToDomainModel());

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="wallObjects">WallObjects.</param>
        internal static IEnumerable<WallObjectEntity> ToDataObjects(
            this IEnumerable<WallObject> wallObjects)
            => wallObjects.Select(wallObject => wallObject.ToDataObject());
    }
}
