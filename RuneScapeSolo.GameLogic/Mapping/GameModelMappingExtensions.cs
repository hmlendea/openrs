using System.Collections.Generic;
using System.Linq;

using RuneScapeSolo.DataAccess.DataObjects;
using RuneScapeSolo.Models;

namespace RuneScapeSolo.GameLogic.Mapping
{
    /// <summary>
    /// Model mapping extensions for converting between entities and domain models.
    /// </summary>
    static class GameModelMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="modelEntity">Model entity.</param>
        internal static GameModel ToDomainModel(this GameModelEntity modelEntity)
        {
            GameModel model = new GameModel
            {
                Id = modelEntity.Id,
                Name = modelEntity.Name,
                Description = modelEntity.Description,
                Command1 = modelEntity.Command1,
                Command2 = modelEntity.Command2,
                Type = modelEntity.Type,
                Width = modelEntity.Width,
                Height = modelEntity.Height,
                GroundItemVar = modelEntity.GroundItemVar,
                ObjectModel = modelEntity.ObjectModel,
                ModelId = modelEntity.ModelId
            };

            return model;
        }

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="model">Model.</param>
        internal static GameModelEntity ToEntity(this GameModel model)
        {
            GameModelEntity modelEntity = new GameModelEntity
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Command1 = model.Command1,
                Command2 = model.Command2,
                Type = model.Type,
                Width = model.Width,
                Height = model.Height,
                GroundItemVar = model.GroundItemVar,
                ObjectModel = model.ObjectModel,
                ModelId = model.ModelId
            };

            return modelEntity;
        }

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="modelEntities">Model entities.</param>
        internal static IEnumerable<GameModel> ToDomainModels(this IEnumerable<GameModelEntity> modelEntities)
        {
            IEnumerable<GameModel> models = modelEntities.Select(modelEntity => modelEntity.ToDomainModel());

            return models;
        }

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="models">Models.</param>
        internal static IEnumerable<GameModelEntity> ToEntities(this IEnumerable<GameModel> models)
        {
            IEnumerable<GameModelEntity> modelEntities = models.Select(model => model.ToEntity());

            return modelEntities;
        }
    }
}
