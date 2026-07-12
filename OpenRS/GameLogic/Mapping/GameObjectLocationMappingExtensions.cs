using System.Collections.Generic;
using System.Linq;

using NuciXNA.Primitives;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;
using OpenRS.Models.Enumerations;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Model mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class GameObjectLocationMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="gameObjectLocationEntity">Model entity.</param>
        internal static GameObjectLocation ToDomainModel(this GameObjectLocationEntity gameObjectLocationEntity) => new()
        {
            Location = new Point2D(gameObjectLocationEntity.X, gameObjectLocationEntity.Y),
            Direction = gameObjectLocationEntity.Direction,
            Type = (GameObjectType)gameObjectLocationEntity.Type
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="gameObjectLocation">Model.</param>
        internal static GameObjectLocationEntity ToDataObject(this GameObjectLocation gameObjectLocation) => new()
        {
            X = gameObjectLocation.Location.X,
            Y = gameObjectLocation.Location.Y,
            Direction = gameObjectLocation.Direction,
            Type = (int)gameObjectLocation.Type
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="gameObjectLocationEntities">Model entities.</param>
        internal static IEnumerable<GameObjectLocation> ToDomainModels(
            this IEnumerable<GameObjectLocationEntity> gameObjectLocationEntities)
            => gameObjectLocationEntities.Select(gameObjectLocationEntity => gameObjectLocationEntity.ToDomainModel());

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="gameObjectLocations">Models.</param>
        internal static IEnumerable<GameObjectLocationEntity> ToDataObjects(
            this IEnumerable<GameObjectLocation> gameObjectLocations)
            => gameObjectLocations.Select(gameObjectLocation => gameObjectLocation.ToDataObject());
    }
}
