using System;
using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class AnimationMappingExtensions
    {
        internal static Animation ToServiceModel(this AnimationEntity animationEntity) => new()
        {
            Name = animationEntity.Name,
            CharacterColour = animationEntity.CharacterColour,
            GenderModel = animationEntity.GenderModel,
            HasA = animationEntity.HasA != 0,
            HasF = animationEntity.HasF != 0,
            Number = animationEntity.Number
        };

        internal static AnimationEntity ToDataObject(this Animation animation) => new()
        {
            Name = animation.Name,
            CharacterColour = animation.CharacterColour,
            GenderModel = animation.GenderModel,
            HasA = Convert.ToInt32(animation.HasA),
            HasF = Convert.ToInt32(animation.HasF),
            Number = animation.Number
        };

        internal static IEnumerable<Animation> ToServiceModels(
            this IEnumerable<AnimationEntity> animationEntities)
            => animationEntities.Select(animationEntity => animationEntity.ToServiceModel());

        internal static IEnumerable<AnimationEntity> ToDataObjects(
            this IEnumerable<Animation> animations)
            => animations.Select(animation => animation.ToDataObject());
    }
}
