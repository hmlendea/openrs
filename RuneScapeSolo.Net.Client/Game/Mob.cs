namespace RuneScapeSolo.Net.Client.Game
{
    public class Mob
    {
        public Mob()
        {
            WaypointsX = new int[10];
            WaypointsY = new int[10];
            AppearanceItems = new int[12];
            CombatLevel = -1;
        }

        public int Admin { get; set; }
        public int AttackingPlayerIndex { get; set; }
        public int AttackingNpcIndex { get; set; }
        public int BaseHitpoints { get; set; }
        public int BottomColour { get; set; }
        public int CombatLevel { get; set; }
        public int CurrentHitpoints;
        public int HairColour { get; set; }
        public int ItemAboveHeadId { get; set; }
        public int LastDamageCount { get; set; }
        public int PlayerSkullTimeout { get; set; }
        public int PlayerSkulled { get; set; }
        public int ProjectileDistance { get; set; }
        public int ProjectileType { get; set; }
        public int ServerId { get; set; }
        public int ServerIndex { get; set; }
        public int SkinColour { get; set; }
        public int TopColour { get; set; }
        public int WaypointsEndSprite { get; set; }
        public int WaypointCurrent { get; set; }
        public int[] AppearanceItems { get; set; }
        public int[] WaypointsX { get; set; }
        public int[] WaypointsY { get; set; }
        public long NameHash { get; set; }
        public string Flag { get; set; }
        public string Name { get; set; }

        public string username;
        public int currentX;
        public int currentY;
        public int npcId;
        public int stepCount;
        public int currentSprite;
        public int nextSprite;
        public string lastMessage;
        public int lastMessageTimeout;
        public int combatTimer;
    }
}
