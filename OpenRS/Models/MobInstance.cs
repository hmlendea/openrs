using System;
using System.Collections.Generic;

using NuciLog.Core;

using NuciXNA.Primitives;

using OpenRS.Settings;

namespace OpenRS.Models
{
    public abstract class MobInstance : GameEntityInstance
    {
        private int combatLevel;
        private int mobSprite;
        private readonly int[][] mobSprites;
        private readonly bool[] activatedPrayers;
        private readonly PathHandler pathHandler;
        // viewArea

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<MobInstance>();

        protected Dictionary<long, int> totalDamageTable;
        protected Dictionary<long, int> meleeDamageTable;
        protected Dictionary<long, int> rangeDamageTable;

        private MobInstance CombatOpponent { get; set; }

        public CombatState LastCombatState { get; private set; }

        public int CombatLevel
        {
            get
            {
                return combatLevel;
            }
            set
            {
                combatLevel = value;
                HasAppearanceChanged = true;
            }
        }

        public int AppearanceId { get; private set; }

        public long CombatTime { get; private set; }

        public int HitsMade { get; set; }

        public int LastDamage { get; set; }

        public int MobSprite
        {
            get
            {
                return mobSprite;
            }
            set
            {
                mobSprite = value;
                HasSpriteChanged = true;
            }
        }

        public long LastMovementTime { get; private set; }

        public bool HasAppearanceChanged { get; private set; }

        public bool HasMoved { get; set; }

        public bool HasSpriteChanged { get; set; }

        public bool IsBusy { get; set; }

        public bool IsInCombat
        {
            get
            {
                return (mobSprite == 8 || mobSprite == 9) && CombatOpponent is not null;
            }
        }

        public bool IsRemoved { get; protected set; }

        public bool WarnedToMove { get; set; }

        public MobInstance()
        {
            mobSprites = [
                [3, 2, 1],
                [4, -1, 0],
                [5, 6, 7]
            ];
            mobSprite = 1;
            combatLevel = 3;
            HasAppearanceChanged = true;
            AppearanceId = 0;
            LastMovementTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            activatedPrayers = new bool[Prayer.MaximumCount];
            LastCombatState = CombatState.Waiting;

            pathHandler = new PathHandler(this);

            totalDamageTable = [];
            meleeDamageTable = [];
            rangeDamageTable = [];
        }

        public void UpdateKillStealing(long index, int damage, AttackType attackType)
        {
            if (totalDamageTable.ContainsKey(index))
            {
                totalDamageTable[index] += damage;
            }
            else
            {
                totalDamageTable.Add(index, damage);
            }

            switch (attackType)
            {
                case AttackType.Melee:
                    if (meleeDamageTable.ContainsKey(index))
                    {
                        meleeDamageTable[index] += damage;
                        return;
                    }

                    meleeDamageTable.Add(index, damage);
                    break;

                case AttackType.Ranged:
                    if (rangeDamageTable.ContainsKey(index))
                    {
                        rangeDamageTable[index] += damage;
                        return;
                    }

                    rangeDamageTable.Add(index, damage);
                    break;
            }
        }

        public abstract void Remove();

        public void ResetCombat() => throw new NotImplementedException();

        public void ResetPath() => pathHandler.ResetPath();

        public bool IsAtObject() => throw new NotImplementedException();

        public bool IsPrayerActivated(int prayerIndex) => activatedPrayers[prayerIndex];

        public void TogglePrayer(int prayerIndex, bool toggleStatus) => activatedPrayers[prayerIndex] = toggleStatus;

        public void UpdateAppearanceId()
        {
            if (HasAppearanceChanged)
            {
                AppearanceId += 1;
            }
        }

        public void UpdateCombatTime() => CombatTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public void UpdateLocation() => pathHandler.UpdateLocation();

        public void UpdateMovementTime() => LastMovementTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public override void SetLocation(Point2D location) => SetLocation(location, false);

        public void SetLocation(Point2D location, bool teleported)
        {
            if (!teleported)
            {
                UpdateSprite(location);
                HasMoved = true;
            }

            UpdateMovementTime();
            WarnedToMove = false;
            base.SetLocation(location);
        }

        public bool FinishedPath() => pathHandler.FinishedPath();

        public void SetPath(WalkPath path) => pathHandler.SetPath(path);

        protected void UpdateSprite(Point2D newLocation)
        {
            try
            {
                Point2D index = new(
                    Location.X - newLocation.X + 1,
                    Location.Y - newLocation.Y + 1);

                MobSprite = mobSprites[index.X][index.Y];
            }
            catch (Exception ex)
            {
                logger.Error("Failed to update the mob sprite.", ex);
            }
        }
    }
}
