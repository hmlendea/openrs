namespace OpenRS.Models
{
    public sealed class Npc
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
        public int[] Sprites { get; set; }

        public Appearance Appearance { get; set; }

        public int Camera1 { get; set; }

        public int Camera2 { get; set; }
        public int WalkModel { get; set; }
        public int CombatModel { get; set; }
        public int CombatSprite { get; set; }
        public int HealthLevel { get; set; }
        public int AttackLevel { get; set; }
        public int DefenceLevel { get; set; }
        public int StrengthLevel { get; set; }
        public int RespawnTime { get; set; }

        // TODO: Convert to bool.
        public int IsAttackable { get; set; }

        // TODO: Convert to bool.
        public int IsAggressive { get; set; }
        public ItemDrop[] Drops { get; set; }
        public Npc()
        {
            Appearance = new Appearance();
            Sprites = new int[12];
        }
    }
}
