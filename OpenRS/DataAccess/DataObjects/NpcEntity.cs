using NuciDAL.DataObjects;

using OpenRS.Models;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class NpcEntity : EntityBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Command { get; set; }

        public int[] Sprites { get; set; }

        public int HairColour { get; set; }

        public int TopColour { get; set; }

        public int TrousersColour { get; set; }

        public int SkinColour { get; set; }

        public int SpriteWidth { get; set; }

        public int SpriteHeight { get; set; }

        public int WalkAnimationSpeed { get; set; }

        public int CombatAnimationSpeed { get; set; }

        public int CombatSwingOffset { get; set; }

        public int HealthLevel { get; set; }

        public int AttackLevel { get; set; }

        public int DefenceLevel { get; set; }

        public int StrengthLevel { get; set; }

        public int RespawnTime { get; set; }

        // RSC game data: 0 = not attackable, non-zero values encode combat level/type.
        public int IsAttackable { get; set; }

        public int IsAggressive { get; set; }

        public ItemDropEntity[] Drops { get; set; }

        public NpcEntity()
        {
            Sprites = new int[Npc.SpriteCount];
        }
    }
}
