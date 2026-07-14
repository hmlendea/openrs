using System.Collections.Generic;
using System.Linq;

using NuciXNA.Primitives;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class GameObjectLocationMappingExtensions
    {
        internal static GameObjectLocation ToDomainModel(this GameObjectLocationEntity gameObjectLocationEntity) => new()
        {
            Location = new(gameObjectLocationEntity.X, gameObjectLocationEntity.Y),
            Direction = gameObjectLocationEntity.Direction,
            Type = (GameObjectType)gameObjectLocationEntity.Type
        };

        internal static GameObjectLocationEntity ToDataObject(this GameObjectLocation gameObjectLocation) => new()
        {
            X = gameObjectLocation.Location.X,
            Y = gameObjectLocation.Location.Y,
            Direction = gameObjectLocation.Direction,
            Type = (int)gameObjectLocation.Type
        };

        internal static IEnumerable<GameObjectLocation> ToDomainModels(
            this IEnumerable<GameObjectLocationEntity> gameObjectLocationEntities)
            => gameObjectLocationEntities.Select(gameObjectLocationEntity => gameObjectLocationEntity.ToDomainModel());

        internal static IEnumerable<GameObjectLocationEntity> ToDataObjects(
            this IEnumerable<GameObjectLocation> gameObjectLocations)
            => gameObjectLocations.Select(gameObjectLocation => gameObjectLocation.ToDataObject());
    }
}
