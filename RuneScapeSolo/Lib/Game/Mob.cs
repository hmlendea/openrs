using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSCXNALib.Game
{
    public class Mob
    {
        public Mob()
        {
            waypointsX = new int[10];
            waypointsY = new int[10];
            appearanceItems = new int[12];
            level = -1;
        }

        public long nameHash;
        public string username;
        public int serverIndex;
        public int serverID;
        public int currentX;
        public int currentY;
        public int npcId;
        public int stepCount;
        public int currentSprite;
        public int nextSprite;
        public int waypointsEndSprite;
        public int waypointCurrent;
        public int[] waypointsX;
        public int[] waypointsY;
        public int[] appearanceItems;
        public string lastMessage;
        public int lastMessageTimeout;
        public int itemAboveHeadID;
        public int playerSkullTimeout;
        public int lastDamageCount;
        public int currentHits;
        public int baseHits;
        public int combatTimer;
        public int level;
        public int hairColour;
        public int topColour;
        public int bottomColour;
        public int skinColour;
        public int projectileType;
        public int attackingPlayerIndex;
        public int attackingNpcIndex;
        public int projectileDistance;
        public int playerSkulled;
    }

}
