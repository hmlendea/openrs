using System.Collections.Generic;
using System.Linq;

using RuneScapeSolo.DataAccess.DataObjects;
using RuneScapeSolo.Models;

namespace RuneScapeSolo.GameLogic.Mapping
{
    /// <summary>
    /// Texture mapping extensions for converting between entities and domain models.
    /// </summary>
    static class TextureMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="textureEntity">Texture entity.</param>
        internal static Texture ToDomainModel(this TextureEntity textureEntity)
        {
            Texture texture = new Texture
            {
                Name = textureEntity.Name,
                SubName = textureEntity.SubName
            };

            return texture;
        }

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="texture">Texture.</param>
        internal static TextureEntity ToEntity(this Texture texture)
        {
            TextureEntity textureEntity = new TextureEntity
            {
                Name = texture.Name,
                SubName = texture.SubName
            };

            return textureEntity;
        }

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="textureEntities">Texture entities.</param>
        internal static IEnumerable<Texture> ToDomainModels(this IEnumerable<TextureEntity> textureEntities)
        {
            IEnumerable<Texture> textures = textureEntities.Select(textureEntity => textureEntity.ToDomainModel());

            return textures;
        }

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="textures">Textures.</param>
        internal static IEnumerable<TextureEntity> ToEntities(this IEnumerable<Texture> textures)
        {
            IEnumerable<TextureEntity> textureEntities = textures.Select(texture => texture.ToEntity());

            return textureEntities;
        }
    }
}
