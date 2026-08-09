using System;
using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class NpcMappingExtensions
    {
        internal static Npc ToServiceModel(this NpcEntity npcEntity) => new()
        {
            Id = npcEntity.Id,
            V1Id = npcEntity.V1Id,
            Name = LocalisationManager.GetString(npcEntity.Name),
            Description = LocalisationManager.GetString(npcEntity.Description),
            Command = LocalisationManager.GetString(npcEntity.Command),
            Sprites = npcEntity.Sprites,
            Appearance = new()
            {
                HairColour = npcEntity.HairColour,
                TopColour = npcEntity.TopColour,
                TrousersColour = npcEntity.TrousersColour,
                SkinColour = npcEntity.SkinColour
            },
            SpriteWidth = npcEntity.SpriteWidth,
            SpriteHeight = npcEntity.SpriteHeight,
            WalkAnimationSpeed = npcEntity.WalkAnimationSpeed,
            CombatAnimationSpeed = npcEntity.CombatAnimationSpeed,
            CombatSwingOffset = npcEntity.CombatSwingOffset,
            HealthLevel = npcEntity.HealthLevel,
            AttackLevel = npcEntity.AttackLevel,
            DefenceLevel = npcEntity.DefenceLevel,
            StrengthLevel = npcEntity.StrengthLevel,
            RespawnTime = npcEntity.RespawnTime,
            IsAttackable = npcEntity.IsAttackable != 0,
            IsAggressive = npcEntity.IsAggressive != 0,
            Drops = npcEntity.Drops?.ToServiceModels().ToArray()
        };

        internal static NpcEntity ToDataObject(this Npc npc) => new()
        {
            Id = npc.Id,
            V1Id = npc.V1Id,
            Name = npc.Name,
            Description = npc.Description,
            Command = npc.Command,
            Sprites = npc.Sprites,
            HairColour = npc.Appearance.HairColour,
            TopColour = npc.Appearance.TopColour,
            TrousersColour = npc.Appearance.TrousersColour,
            SkinColour = npc.Appearance.SkinColour,
            SpriteWidth = npc.SpriteWidth,
            SpriteHeight = npc.SpriteHeight,
            WalkAnimationSpeed = npc.WalkAnimationSpeed,
            CombatAnimationSpeed = npc.CombatAnimationSpeed,
            CombatSwingOffset = npc.CombatSwingOffset,
            HealthLevel = npc.HealthLevel,
            AttackLevel = npc.AttackLevel,
            DefenceLevel = npc.DefenceLevel,
            StrengthLevel = npc.StrengthLevel,
            RespawnTime = npc.RespawnTime,
            IsAttackable = Convert.ToInt32(npc.IsAttackable),
            IsAggressive = Convert.ToInt32(npc.IsAggressive),
            Drops = npc.Drops?.ToDataObjects().ToArray()
        };

        internal static IEnumerable<Npc> ToServiceModels(
            this IEnumerable<NpcEntity> npcEntities)
            => npcEntities.Select(npcEntity => npcEntity.ToServiceModel());

        internal static IEnumerable<NpcEntity> ToDataObjects(
            this IEnumerable<Npc> npcs)
            => npcs.Select(npc => npc.ToDataObject());
    }
}
