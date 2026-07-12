using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Texture mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class GameTextureMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="textureEntity">Texture entity.</param>
        internal static GameTexture ToDomainModel(this GameTextureEntity textureEntity) => new()
        {
            Name = textureEntity.Name,
            SubName = textureEntity.SubName
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="texture">Texture.</param>
        internal static GameTextureEntity ToDataObject(this GameTexture texture) => new()
        {
            Name = texture.Name,
            SubName = texture.SubName
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="textureEntities">Texture entities.</param>
        internal static IEnumerable<GameTexture> ToDomainModels(
            this IEnumerable<GameTextureEntity> textureEntities)
            => textureEntities.Select(textureEntity => textureEntity.ToDomainModel());

        internal static IEnumerable<GameTextureEntity> ToDataObjects(
            this IEnumerable<GameTexture> textures)
            => textures.Select(texture => texture.ToDataObject());
    }
}
