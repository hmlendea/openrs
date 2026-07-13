using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class WallObjectMappingExtensions
    {
        internal static WallObject ToDomainModel(this WallObjectEntity wallObjectEntity) => new()
        {
            Name = wallObjectEntity.Name,
            Description = wallObjectEntity.Description,
            Command1 = wallObjectEntity.Command1,
            Command2 = wallObjectEntity.Command2,
            Type = wallObjectEntity.Type,
            Unknown = wallObjectEntity.Unknown,
            ModelHeight = wallObjectEntity.ModelHeight,
            ModelFaceBack = wallObjectEntity.ModelFaceBack,
            ModelFaceFront = wallObjectEntity.ModelFaceFront
        };
        internal static WallObjectEntity ToDataObject(this WallObject wallObject) => new()
        {
            Name = wallObject.Name,
            Description = wallObject.Description,
            Command1 = wallObject.Command1,
            Command2 = wallObject.Command2,
            Type = wallObject.Type,
            Unknown = wallObject.Unknown,
            ModelHeight = wallObject.ModelHeight,
            ModelFaceBack = wallObject.ModelFaceBack,
            ModelFaceFront = wallObject.ModelFaceFront
        };
        internal static IEnumerable<WallObject> ToDomainModels(
            this IEnumerable<WallObjectEntity> wallObjectEntities)
            => wallObjectEntities.Select(wallObjectEntity => wallObjectEntity.ToDomainModel());
        internal static IEnumerable<WallObjectEntity> ToDataObjects(
            this IEnumerable<WallObject> wallObjects)
            => wallObjects.Select(wallObject => wallObject.ToDataObject());
    }
}
