using NuciXNA.Primitives;

using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    public class ClientMob
    {
        public ClientMob()
        {
            Appearance = new Appearance();
            Location = new Point2D();
            Waypoints = new Point2D[10];

            for (int i = 0; i < Waypoints.Length; i++)
            {
                Waypoints[i] = new Point2D();
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

        public string username;
        public int npcId;
        public int stepCount;
        public int currentSprite;
        public int nextSprite;
        public string lastMessage;
        public int lastMessageTimeout;
        public int combatTimer;
    }
}
