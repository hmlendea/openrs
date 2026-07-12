using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    /// <summary>
    /// Spell mapping extensions for converting between entities and domain models.
    /// </summary>
    internal static class SpellMappingExtensions
    {
        /// <summary>
        /// Converts the entity into a domain model.
        /// </summary>
        /// <returns>The domain model.</returns>
        /// <param name="spellEntity">Spell entity.</param>
        internal static Spell ToDomainModel(this SpellEntity spellEntity) => new()
        {
            Id = spellEntity.Id,
            Name = spellEntity.Name,
            Description = spellEntity.Description,
            RequiredLevel = spellEntity.RequiredLevel,
            Type = spellEntity.Type,
            RuneCount = spellEntity.RuneCount,
            RequiredRunesIds = spellEntity.RequiredRunesIds,
            RequiredRunesCounts = spellEntity.RequiredRunesCounts,
            ExperienceGain = spellEntity.ExperienceGain
        };

        /// <summary>
        /// Converts the domain model into an entity.
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="spell">Spell.</param>
        internal static SpellEntity ToDataObject(this Spell spell) => new()
        {
            Id = spell.Id,
            Name = spell.Name,
            Description = spell.Description,
            RequiredLevel = spell.RequiredLevel,
            Type = spell.Type,
            RuneCount = spell.RuneCount,
            RequiredRunesIds = spell.RequiredRunesIds,
            RequiredRunesCounts = spell.RequiredRunesCounts,
            ExperienceGain = spell.ExperienceGain
        };

        /// <summary>
        /// Converts the entities into domain models.
        /// </summary>
        /// <returns>The domain models.</returns>
        /// <param name="spellEntities">Spell entities.</param>
        internal static IEnumerable<Spell> ToDomainModels(
            this IEnumerable<SpellEntity> spellEntities)
            => spellEntities.Select(spellEntity => spellEntity.ToDomainModel());

        internal static IEnumerable<SpellEntity> ToDataObjects(
            this IEnumerable<Spell> spells)
            => spells.Select(spell => spell.ToDataObject());
    }
}
