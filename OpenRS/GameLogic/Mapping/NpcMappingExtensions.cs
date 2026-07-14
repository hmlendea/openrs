using System.Collections.Generic;
using System.Linq;

using OpenRS.DataAccess.DataObjects;
using OpenRS.Localisation;
using OpenRS.Models;

namespace OpenRS.GameLogic.Mapping
{
    internal static class NpcMappingExtensions
    {
        internal static Npc ToDomainModel(this NpcEntity npcEntity) => new()
        {
            Id = npcEntity.Id,
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
            Camera1 = npcEntity.Camera1,
            Camera2 = npcEntity.Camera2,
            WalkModel = npcEntity.WalkModel,
            CombatModel = npcEntity.CombatModel,
            CombatSprite = npcEntity.CombatSprite,
            HealthLevel = npcEntity.HealthLevel,
            AttackLevel = npcEntity.AttackLevel,
            DefenceLevel = npcEntity.DefenceLevel,
            StrengthLevel = npcEntity.StrengthLevel,
            RespawnTime = npcEntity.RespawnTime,
            IsAttackable = npcEntity.IsAttackable,
            IsAggressive = npcEntity.IsAggressive,
            Drops = npcEntity.Drops?.ToDomainModels().ToArray()
        };

        internal static NpcEntity ToDataObject(this Npc npc) => new()
        {
            Id = npc.Id,
            Name = npc.Name,
            Description = npc.Description,
            Command = npc.Command,
            Sprites = npc.Sprites,
            HairColour = npc.Appearance.HairColour,
            TopColour = npc.Appearance.TopColour,
            TrousersColour = npc.Appearance.TrousersColour,
            SkinColour = npc.Appearance.SkinColour,
            Camera1 = npc.Camera1,
            Camera2 = npc.Camera2,
            WalkModel = npc.WalkModel,
            CombatModel = npc.CombatModel,
            CombatSprite = npc.CombatSprite,
            HealthLevel = npc.HealthLevel,
            AttackLevel = npc.AttackLevel,
            DefenceLevel = npc.DefenceLevel,
            StrengthLevel = npc.StrengthLevel,
            RespawnTime = npc.RespawnTime,
            IsAttackable = npc.IsAttackable,
            IsAggressive = npc.IsAggressive,
            Drops = npc.Drops?.ToDataObjects().ToArray()
        };

        internal static IEnumerable<Npc> ToDomainModels(
            this IEnumerable<NpcEntity> npcEntities)
            => npcEntities.Select(npcEntity => npcEntity.ToDomainModel());

        internal static IEnumerable<NpcEntity> ToDataObjects(
            this IEnumerable<Npc> npcs)
            => npcs.Select(npc => npc.ToDataObject());
    }
}
