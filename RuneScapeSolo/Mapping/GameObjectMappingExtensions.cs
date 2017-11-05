using System.Collections.Generic;
using System.Linq;

using RuneScapeSolo.DataAccess.DataObjects;
using RuneScapeSolo.Models;

namespace RuneScapeSolo.Mapping
{
    /// <summary>
    /// GameObject mapping extensions for converting between entities and domain models.
    /// </summary>
    static class GameObjectMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="gameObjectEntity">GameObject entity.</param>
        internal static GameObject ToDomainModel(this GameObjectEntity gameObjectEntity)
        {
            GameObject gameObject = new GameObject
            {
                Name = gameObjectEntity.Name,
                Description = gameObjectEntity.Description,
                Command1 = gameObjectEntity.Command1,
                Command2 = gameObjectEntity.Command2,
                Type = gameObjectEntity.Type,
                Width = gameObjectEntity.Width,
                Height = gameObjectEntity.Height,
                GroundItemVar = gameObjectEntity.GroundItemVar,
                ObjectModel = gameObjectEntity.ObjectModel,
                ModelId = gameObjectEntity.ModelId
            };

            return gameObject;
        }

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="gameObject">GameObject.</param>
        internal static GameObjectEntity ToEntity(this GameObject gameObject)
        {
            GameObjectEntity gameObjectEntity = new GameObjectEntity
            {
                Name = gameObject.Name,
                Description = gameObject.Description,
                Command1 = gameObject.Command1,
                Command2 = gameObject.Command2,
                Type = gameObject.Type,
                Width = gameObject.Width,
                Height = gameObject.Height,
                GroundItemVar = gameObject.GroundItemVar,
                ObjectModel = gameObject.ObjectModel,
                ModelId = gameObject.ModelId
            };

            return gameObjectEntity;
        }

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="gameObjectEntities">GameObject entities.</param>
        internal static IEnumerable<GameObject> ToDomainModels(this IEnumerable<GameObjectEntity> gameObjectEntities)
        {
            IEnumerable<GameObject> gameObjects = gameObjectEntities.Select(gameObjectEntity => gameObjectEntity.ToDomainModel());

            return gameObjects;
        }

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="gameObjects">GameObjects.</param>
        internal static IEnumerable<GameObjectEntity> ToEntities(this IEnumerable<GameObject> gameObjects)
        {
            IEnumerable<GameObjectEntity> gameObjectEntities = gameObjects.Select(gameObject => gameObject.ToEntity());

            return gameObjectEntities;
        }
    }
}
