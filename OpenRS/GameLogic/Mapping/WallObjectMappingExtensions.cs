using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class WallObjectMappingExtensions
    {
        internal static WallObject ToServiceModel(
            this WallObjectEntity wallObjectEntity) => new()
        {
            Name = LocalisationManager.GetString(wallObjectEntity.Name),
            Description = LocalisationManager.GetString(wallObjectEntity.Description),
            Command1 = LocalisationManager.GetString(wallObjectEntity.Command1),
            Command2 = LocalisationManager.GetString(wallObjectEntity.Command2),
            Type = wallObjectEntity.Type,
            FaceRenderMode = wallObjectEntity.FaceRenderMode,
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
            FaceRenderMode = wallObject.FaceRenderMode,
            ModelHeight = wallObject.ModelHeight,
            ModelFaceBack = wallObject.ModelFaceBack,
            ModelFaceFront = wallObject.ModelFaceFront
        };

        internal static IEnumerable<WallObject> ToServiceModels(
            this IEnumerable<WallObjectEntity> wallObjectEntities)
            => wallObjectEntities.Select(wallObjectEntity => wallObjectEntity.ToServiceModel());

        internal static IEnumerable<WallObjectEntity> ToDataObjects(
            this IEnumerable<WallObject> wallObjects)
            => wallObjects.Select(wallObject => wallObject.ToDataObject());
    }
}
