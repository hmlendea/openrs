using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Animation mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class AnimationMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="animationEntity">Animation entity.</param>
        internal static Animation ToDomainModel(this AnimationEntity animationEntity) => new()
        {
            Name = animationEntity.Name,
            CharacterColour = animationEntity.CharacterColour,
            GenderModel = animationEntity.GenderModel,
            HasA = animationEntity.HasA,
            HasF = animationEntity.HasF,
            Number = animationEntity.Number
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="animation">Animation.</param>
        internal static AnimationEntity ToDataObject(this Animation animation) => new()
        {
            Name = animation.Name,
            CharacterColour = animation.CharacterColour,
            GenderModel = animation.GenderModel,
            HasA = animation.HasA,
            HasF = animation.HasF,
            Number = animation.Number
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="animationEntities">Animation entities.</param>
        internal static IEnumerable<Animation> ToDomainModels(
            this IEnumerable<AnimationEntity> animationEntities)
            => animationEntities.Select(animationEntity => animationEntity.ToDomainModel());

        /// <summary>
        /// Converts the domain models into entities.
        /// </summary>
        /// <returns>The entities.</returns>
        /// <param name="animations">Animations.</param>
        internal static IEnumerable<AnimationEntity> ToDataObjects(
            this IEnumerable<Animation> animations)
            => animations.Select(animation => animation.ToDataObject());
    }
}
