using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    public sealed class ClientMob
    {
        public ClientMob()
        {
            Appearance = new();
            Location = new();
            Waypoints = new Point2D[10];

            for (int i = 0; i < Waypoints.Length; i += 1)
            {
                Waypoints[i] = new();
            }

            AppearanceItems = new int[12];
            CombatLevel = -1;
        }

        public Appearance Appearance { get; set; }
        public Point2D Location { get; set; }
        public Point2D[] Waypoints { get; set; }
        public int Admin { get; set; }
        public int AttackingPlayerIndex { get; set; }
        public int AttackingNpcIndex { get; set; }
        public int BaseHitpoints { get; set; }
        public int CombatLevel { get; set; }
        public int CurrentHitpoints;
        public int ItemAboveHeadId { get; set; }
        public int LastDamageCount { get; set; }
        public int PlayerSkullTimeout { get; set; }
        public int PlayerSkulled { get; set; }
        public int ProjectileDistance { get; set; }
        public int ProjectileType { get; set; }
        public int ServerId { get; set; }
        public int ServerIndex { get; set; }
        public int WaypointsEndSprite { get; set; }
        public int WaypointCurrent { get; set; }
        public int[] AppearanceItems { get; set; }
        public long NameHash { get; set; }
        public string Flag { get; set; }
        public string Name { get; set; }

        public string Username;
        public int NpcIdentifier;
        public int StepCount;
        public int CurrentSprite;
        public int NextSprite;
        public string LastMessage;
        public int LastMessageTimeout;
        public int CombatTimer;

        // Legacy camelCase field aliases for GameClient compatibility
        public int[] appearanceItems { get => AppearanceItems; set => AppearanceItems = value; }
        public int attackingNpcIndex { get => AttackingNpcIndex; set => AttackingNpcIndex = value; }
        public int attackingPlayerIndex { get => AttackingPlayerIndex; set => AttackingPlayerIndex = value; }
        public int baseHits { get => BaseHitpoints; set => BaseHitpoints = value; }
        public int combatTimer { get => CombatTimer; set => CombatTimer = value; }
        public int currentHits { get => CurrentHitpoints; set => CurrentHitpoints = value; }
        public int currentSprite { get => CurrentSprite; set => CurrentSprite = value; }
        public int itemAboveHeadID { get => ItemAboveHeadId; set => ItemAboveHeadId = value; }
        public int lastDamageCount { get => LastDamageCount; set => LastDamageCount = value; }
        public string lastMessage { get => LastMessage; set => LastMessage = value; }
        public int lastMessageTimeout { get => LastMessageTimeout; set => LastMessageTimeout = value; }
        public int level { get => CombatLevel; set => CombatLevel = value; }
        public long nameHash { get => NameHash; set => NameHash = value; }
        public int nextSprite { get => NextSprite; set => NextSprite = value; }
        public int npcId { get => NpcIdentifier; set => NpcIdentifier = value; }
        public int playerSkulled { get => PlayerSkulled; set => PlayerSkulled = value; }
        public int playerSkullTimeout { get => PlayerSkullTimeout; set => PlayerSkullTimeout = value; }
        public int projectileDistance { get => ProjectileDistance; set => ProjectileDistance = value; }
        public int projectileType { get => ProjectileType; set => ProjectileType = value; }
        public int serverID { get => ServerId; set => ServerId = value; }
        public int serverIndex { get => ServerIndex; set => ServerIndex = value; }
        public int stepCount { get => StepCount; set => StepCount = value; }
        public string username { get => Username; set => Username = value; }
        public int waypointCurrent { get => WaypointCurrent; set => WaypointCurrent = value; }
        public int waypointsEndSprite { get => WaypointsEndSprite; set => WaypointsEndSprite = value; }

        // Appearance colour aliases
        public int hairColour { get => Appearance.HairColour; set => Appearance.HairColour = value; }
        public int topColour { get => Appearance.TopColour; set => Appearance.TopColour = value; }
        public int bottomColour { get => Appearance.TrousersColour; set => Appearance.TrousersColour = value; }
        public int skinColour { get => Appearance.SkinColour; set => Appearance.SkinColour = value; }

        // Location aliases
        public int currentX { get => Location.X; set => Location = new(value, Location.Y); }
        public int currentY { get => Location.Y; set => Location = new(Location.X, value); }

        // Waypoint int arrays
        public int[] waypointsX = new int[10];
        public int[] waypointsY = new int[10];
    }
}
