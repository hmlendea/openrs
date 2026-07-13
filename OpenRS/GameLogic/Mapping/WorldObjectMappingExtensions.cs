using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class WorldObjectMappingExtensions
    {
        internal static WorldObject ToDomainModel(this WorldObjectEntity worldObjectEntity) => new()
        {
            Id = worldObjectEntity.Id,
            Name = worldObjectEntity.Name,
            Description = worldObjectEntity.Description,
            Command1 = worldObjectEntity.Command1,
            Command2 = worldObjectEntity.Command2,
            Type = worldObjectEntity.Type,
            Width = worldObjectEntity.Width,
            Height = worldObjectEntity.Height,
            GroundItemVar = worldObjectEntity.GroundItemVar,
            ObjectModel = worldObjectEntity.ObjectModel,
            ModelId = worldObjectEntity.ModelId
        };
        internal static WorldObjectEntity ToDataObject(this WorldObject worldObject) => new()
        {
            Id = worldObject.Id,
            Name = worldObject.Name,
            Description = worldObject.Description,
            Command1 = worldObject.Command1,
            Command2 = worldObject.Command2,
            Type = worldObject.Type,
            Width = worldObject.Width,
            Height = worldObject.Height,
            GroundItemVar = worldObject.GroundItemVar,
            ObjectModel = worldObject.ObjectModel,
            ModelId = worldObject.ModelId
        };
        internal static IEnumerable<WorldObject> ToDomainModels(
            this IEnumerable<WorldObjectEntity> worldObjectEntities)
            => worldObjectEntities.Select(worldObjectEntity => worldObjectEntity.ToDomainModel());

        internal static IEnumerable<WorldObjectEntity> ToDataObjects(
            this IEnumerable<WorldObject> worldObjects)
            => worldObjects.Select(worldObject => worldObject.ToDataObject());
    }
}
