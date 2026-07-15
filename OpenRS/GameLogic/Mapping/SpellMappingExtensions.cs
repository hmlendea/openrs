using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class SpellMappingExtensions
    {
        internal static Spell ToServiceModel(this SpellEntity spellEntity) => new()
        {
            Id = spellEntity.Id,
            V1Id = spellEntity.V1Id,
            Name = LocalisationManager.GetString(spellEntity.Name),
            Description = LocalisationManager.GetString(spellEntity.Description),
            RequiredLevel = spellEntity.RequiredLevel,
            Type = spellEntity.Type,
            RequiredRunes = Enumerable
                .Range(0, spellEntity.RuneCount)
                .ToDictionary(
                    runeIndex => spellEntity.RequiredRunesIds[runeIndex],
                    runeIndex => spellEntity.RequiredRunesCounts[runeIndex]),
            ExperienceGain = spellEntity.ExperienceGain
        };

        internal static SpellEntity ToDataObject(this Spell spell) => new()
        {
            Id = spell.Id,
            V1Id = spell.V1Id,
            Name = spell.Name,
            Description = spell.Description,
            RequiredLevel = spell.RequiredLevel,
            Type = spell.Type,
            RuneCount = spell.RequiredRunes.Count,
            RequiredRunesIds = [.. spell.RequiredRunes.Keys],
            RequiredRunesCounts = [.. spell.RequiredRunes.Values],
            ExperienceGain = spell.ExperienceGain
        };

        internal static IEnumerable<Spell> ToServiceModels(
            this IEnumerable<SpellEntity> spellEntities)
            => spellEntities.Select(spellEntity => spellEntity.ToServiceModel());

        internal static IEnumerable<SpellEntity> ToDataObjects(
            this IEnumerable<Spell> spells)
            => spells.Select(spell => spell.ToDataObject());
    }
}
