using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class AnimationMappingExtensions
    {
        internal static Animation ToDomainModel(this AnimationEntity animationEntity) => new()
        {
            Name = animationEntity.Name,
            CharacterColour = animationEntity.CharacterColour,
            GenderModel = animationEntity.GenderModel,
            HasA = animationEntity.HasA,
            HasF = animationEntity.HasF,
            Number = animationEntity.Number
        };

        internal static AnimationEntity ToDataObject(this Animation animation) => new()
        {
            Name = animation.Name,
            CharacterColour = animation.CharacterColour,
            GenderModel = animation.GenderModel,
            HasA = animation.HasA,
            HasF = animation.HasF,
            Number = animation.Number
        };

        internal static IEnumerable<Animation> ToDomainModels(
            this IEnumerable<AnimationEntity> animationEntities)
            => animationEntities.Select(animationEntity => animationEntity.ToDomainModel());

        internal static IEnumerable<AnimationEntity> ToDataObjects(
            this IEnumerable<Animation> animations)
            => animations.Select(animation => animation.ToDataObject());
    }
}
