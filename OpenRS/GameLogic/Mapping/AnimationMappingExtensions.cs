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
            HasAttackFrames = animationEntity.HasAttackFrames != 0,
            HasFemaleFrames = animationEntity.HasFemaleFrames != 0,
            SpriteIndex = animationEntity.SpriteIndex
        };

        internal static AnimationEntity ToDataObject(this Animation animation) => new()
        {
            Name = animation.Name,
            CharacterColour = animation.CharacterColour,
            GenderModel = animation.GenderModel,
            HasAttackFrames = Convert.ToInt32(animation.HasAttackFrames),
            HasFemaleFrames = Convert.ToInt32(animation.HasFemaleFrames),
            SpriteIndex = animation.SpriteIndex
        };

        internal static IEnumerable<Animation> ToServiceModels(
            this IEnumerable<AnimationEntity> animationEntities)
            => animationEntities.Select(animationEntity => animationEntity.ToServiceModel());

        internal static IEnumerable<AnimationEntity> ToDataObjects(
            this IEnumerable<Animation> animations)
            => animations.Select(animation => animation.ToDataObject());
    }
}
