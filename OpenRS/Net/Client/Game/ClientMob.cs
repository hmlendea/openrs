using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    public sealed class ClientMob
    {
        public int[] WaypointXPositions = new int[10];

        public int[] WaypointYPositions = new int[10];

        public Appearance Appearance { get; set; }

        public int Admin { get; set; }

        public int AttackingNpcIndex { get; set; }

        public int AttackingPlayerIndex { get; set; }

        public int[] AppearanceItems { get; set; }

        public int BaseHitpoints { get; set; }

        public int CombatLevel { get; set; }

        public int CombatTimer { get; set; }

        public int CurrentHitpoints { get; set; }

        public int CurrentSprite { get; set; }

        public int ItemAboveHeadId { get; set; }

        public int LastDamageCount { get; set; }

        public int LastMessageTimeout { get; set; }

        public Point2D Location { get; set; }

        public int LocationX
        {
            get => Location.X;
            set => Location = new(value, Location.Y);
        }

        public int LocationY
        {
            get => Location.Y;
            set => Location = new(Location.X, value);
        }

        public long NameHash { get; set; }

        public int NextSprite { get; set; }

        public int NpcIdentifier { get; set; }

        public int PlayerSkullTimeout { get; set; }

        public int PlayerSkulled { get; set; }

        public int ProjectileDistance { get; set; }

        public int ProjectileType { get; set; }

        public int ServerId { get; set; }

        public int ServerIndex { get; set; }

        public int StepCount { get; set; }

        public int WaypointCurrent { get; set; }

        public int WaypointsEndSprite { get; set; }

        public string Flag { get; set; }

        public string LastMessage { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public ClientMob()
        {
            Appearance = new();
            Location = new();
            AppearanceItems = new int[12];
            CombatLevel = -1;
        }
    }
}
