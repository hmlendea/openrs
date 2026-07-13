using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class GameTextureMappingExtensions
    {
        internal static GameTexture ToDomainModel(this GameTextureEntity textureEntity) => new()
        {
            Name = textureEntity.Name,
            SubName = textureEntity.SubName
        };
        internal static GameTextureEntity ToDataObject(this GameTexture texture) => new()
        {
            Name = texture.Name,
            SubName = texture.SubName
        };
        internal static IEnumerable<GameTexture> ToDomainModels(
            this IEnumerable<GameTextureEntity> textureEntities)
            => textureEntities.Select(textureEntity => textureEntity.ToDomainModel());

        internal static IEnumerable<GameTextureEntity> ToDataObjects(
            this IEnumerable<GameTexture> textures)
            => textures.Select(texture => texture.ToDataObject());
    }
}
