using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class GameTextureMappingExtensions
    {
        internal static GameTexture ToServiceModel(this GameTextureEntity textureEntity) => new()
        {
            V1Id = textureEntity.V1Id,
            Name = textureEntity.Name,
            SubName = textureEntity.SubName
        };

        internal static GameTextureEntity ToDataObject(this GameTexture texture) => new()
        {
            V1Id = texture.V1Id,
            Name = texture.Name,
            SubName = texture.SubName
        };

        internal static IEnumerable<GameTexture> ToServiceModels(
            this IEnumerable<GameTextureEntity> textureEntities)
            => textureEntities.Select(textureEntity => textureEntity.ToServiceModel());

        internal static IEnumerable<GameTextureEntity> ToDataObjects(
            this IEnumerable<GameTexture> textures)
            => textures.Select(texture => texture.ToDataObject());
    }
}
