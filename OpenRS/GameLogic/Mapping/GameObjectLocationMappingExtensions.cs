using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class GameObjectLocationMappingExtensions
    {
        internal static GameObjectLocation ToServiceModel(
            this GameObjectLocationEntity gameObjectLocationEntity) => new()
        {
            Location = new(
                gameObjectLocationEntity.XCoordinate,
                gameObjectLocationEntity.YCoordinate),
            Direction = gameObjectLocationEntity.Direction,
            Type = (GameObjectType)gameObjectLocationEntity.Type
        };

        internal static GameObjectLocationEntity ToDataObject(
            this GameObjectLocation gameObjectLocation) => new()
        {
            XCoordinate = gameObjectLocation.Location.X,
            YCoordinate = gameObjectLocation.Location.Y,
            Direction = gameObjectLocation.Direction,
            Type = (int)gameObjectLocation.Type
        };

        internal static IEnumerable<GameObjectLocation> ToServiceModels(
            this IEnumerable<GameObjectLocationEntity> gameObjectLocationEntities)
            => gameObjectLocationEntities.Select(
                gameObjectLocationEntity => gameObjectLocationEntity.ToServiceModel());

        internal static IEnumerable<GameObjectLocationEntity> ToDataObjects(
            this IEnumerable<GameObjectLocation> gameObjectLocations)
            => gameObjectLocations.Select(
                gameObjectLocation => gameObjectLocation.ToDataObject());
    }
}
