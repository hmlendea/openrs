namespace OpenRS.Models
{
    public sealed class Npc
    {
        public static int SpriteCount => 12;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Command { get; set; }

        public int[] Sprites { get; set; }

        public Appearance Appearance { get; set; }

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

        public bool IsAttackable { get; set; }

        public bool IsAggressive { get; set; }

        public ItemDrop[] Drops { get; set; }

        public Npc()
        {
            Appearance = new();
            Sprites = new int[SpriteCount];
        }
    }
}
