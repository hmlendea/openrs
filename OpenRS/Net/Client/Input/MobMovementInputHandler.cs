using OpenRS.Localisation;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Input
{
    internal sealed class MobMovementInputHandler(GameClient client)
    {
        private static int BaseMovementSpeed => 4;

        private static int DirectionNone => -1;

        private static int MaximumWaypointDistanceBeforeSnap => 8;

        private static int NpcStepAnimationIdentifier => 43;

        private static int RespawnMessageType => 3;

        private static int SnapDistanceMultiplier => 3;

        private static int SpriteEast => 2;

        private static int SpriteNorth => 0;

        private static int SpriteNorthEast => 1;

        private static int SpriteNorthWest => 7;

        private static int SpriteSouth => 4;

        private static int SpriteSouthEast => 3;

        private static int SpriteSouthWest => 5;

        private static int SpriteWest => 6;

        private static int WaypointCount => 10;

        private static int WaypointSpeedThreshold => 2;

        internal void UpdateNpcs()
        {
            for (int npcIndex = 0; npcIndex < client.npcCount; npcIndex += 1)
            {
                UpdateNpc(client.npcArray[npcIndex]);
            }
        }

        internal void UpdatePlayers()
        {
            for (int playerIndex = 0; playerIndex < client.playerCount; playerIndex += 1)
            {
                ClientMob player = client.playerArray[playerIndex];

                if (player is null)
                {
                    continue;
                }

                UpdatePlayer(player);
            }
        }

        internal void UpdateProjectileDistances()
        {
            for (int playerIndex = 0; playerIndex < client.playerCount; playerIndex += 1)
            {
                ClientMob player = client.playerArray[playerIndex];

                if (player.ProjectileDistance > 0)
                {
                    player.ProjectileDistance -= 1;
                }
            }
        }

        private void AdvanceWaypointIfReached(ClientMob mob, int targetSprite)
        {
            if (mob.LocationX != mob.WaypointXPositions[targetSprite])
            {
                return;
            }

            if (mob.LocationY != mob.WaypointYPositions[targetSprite])
            {
                return;
            }

            mob.WaypointsEndSprite = (targetSprite + 1) % WaypointCount;
        }

        private int ApplyHorizontalMovement(
            ClientMob mob,
            int targetSprite,
            int movementSpeed,
            int direction)
        {
            if (mob.LocationX < mob.WaypointXPositions[targetSprite])
            {
                mob.LocationX += movementSpeed;
                mob.StepCount += 1;
                return SpriteEast;
            }

            if (mob.LocationX > mob.WaypointXPositions[targetSprite])
            {
                mob.LocationX -= movementSpeed;
                mob.StepCount += 1;
                return SpriteWest;
            }

            return direction;
        }

        private int ApplyVerticalMovement(
            ClientMob mob,
            int targetSprite,
            int movementSpeed,
            int direction)
        {
            if (mob.LocationY < mob.WaypointYPositions[targetSprite])
            {
                mob.LocationY += movementSpeed;
                mob.StepCount += 1;
                return GetSouthboundSprite(direction);
            }

            if (mob.LocationY > mob.WaypointYPositions[targetSprite])
            {
                mob.LocationY -= movementSpeed;
                mob.StepCount += 1;
                return GetNorthboundSprite(direction);
            }

            return direction;
        }

        private static int GetMovementSpeed(int waypointDistance)
        {
            if (waypointDistance > WaypointSpeedThreshold)
            {
                return (waypointDistance - 1) * BaseMovementSpeed;
            }

            return BaseMovementSpeed;
        }

        private static int GetNorthboundSprite(int direction)
        {
            if (direction == DirectionNone)
            {
                return SpriteNorth;
            }

            if (direction == SpriteEast)
            {
                return SpriteNorthEast;
            }

            return SpriteNorthWest;
        }

        private static int GetSouthboundSprite(int direction)
        {
            if (direction == DirectionNone)
            {
                return SpriteSouth;
            }

            if (direction == SpriteEast)
            {
                return SpriteSouthEast;
            }

            return SpriteSouthWest;
        }

        private static int GetWaypointDistance(int nextWaypointIndex, int targetSprite)
        {
            if (targetSprite < nextWaypointIndex)
            {
                return nextWaypointIndex - targetSprite;
            }

            return WaypointCount + nextWaypointIndex - targetSprite;
        }

        private int MoveMobTowardsWaypoint(
            ClientMob mob,
            int targetSprite,
            int movementSpeed)
        {
            int direction = DirectionNone;
            direction = ApplyHorizontalMovement(mob, targetSprite, movementSpeed, direction);
            SnapHorizontalPosition(mob, targetSprite, movementSpeed);
            direction = ApplyVerticalMovement(mob, targetSprite, movementSpeed, direction);
            SnapVerticalPosition(mob, targetSprite, movementSpeed);
            return direction;
        }

        private bool ShouldSnapMobToWaypoint(
            ClientMob mob,
            int targetSprite,
            int waypointDistance)
        {
            int maximumSnapDistance = client.gridSize * SnapDistanceMultiplier;

            if (mob.WaypointXPositions[targetSprite] - mob.LocationX > maximumSnapDistance)
            {
                return true;
            }

            if (mob.WaypointYPositions[targetSprite] - mob.LocationY > maximumSnapDistance)
            {
                return true;
            }

            if (mob.WaypointXPositions[targetSprite] - mob.LocationX < -maximumSnapDistance)
            {
                return true;
            }

            if (mob.WaypointYPositions[targetSprite] - mob.LocationY < -maximumSnapDistance)
            {
                return true;
            }

            return waypointDistance > MaximumWaypointDistanceBeforeSnap;
        }

        private void SnapHorizontalPosition(
            ClientMob mob,
            int targetSprite,
            int movementSpeed)
        {
            int distanceToTarget = mob.LocationX - mob.WaypointXPositions[targetSprite];

            if (distanceToTarget < movementSpeed && distanceToTarget > -movementSpeed)
            {
                mob.LocationX = mob.WaypointXPositions[targetSprite];
            }
        }

        private void SnapMobToWaypoint(ClientMob mob, int targetSprite)
        {
            mob.LocationX = mob.WaypointXPositions[targetSprite];
            mob.LocationY = mob.WaypointYPositions[targetSprite];
        }

        private void SnapVerticalPosition(
            ClientMob mob,
            int targetSprite,
            int movementSpeed)
        {
            int distanceToTarget = mob.LocationY - mob.WaypointYPositions[targetSprite];

            if (distanceToTarget < movementSpeed && distanceToTarget > -movementSpeed)
            {
                mob.LocationY = mob.WaypointYPositions[targetSprite];
            }
        }

        private void UpdateMobMovement(ClientMob mob)
        {
            int nextWaypointIndex = (mob.WaypointCurrent + 1) % WaypointCount;

            if (mob.WaypointsEndSprite == nextWaypointIndex)
            {
                return;
            }

            int targetSprite = mob.WaypointsEndSprite;
            int waypointDistance = GetWaypointDistance(nextWaypointIndex, targetSprite);

            if (ShouldSnapMobToWaypoint(mob, targetSprite, waypointDistance))
            {
                SnapMobToWaypoint(mob, targetSprite);
            }
            else
            {
                int movementSpeed = GetMovementSpeed(waypointDistance);
                int direction = MoveMobTowardsWaypoint(mob, targetSprite, movementSpeed);

                if (direction != DirectionNone)
                {
                    mob.CurrentSprite = direction;
                }
            }

            AdvanceWaypointIfReached(mob, targetSprite);
        }

        private void UpdateMobTimeouts(ClientMob mob)
        {
            if (mob.LastMessageTimeout > 0)
            {
                mob.LastMessageTimeout -= 1;
            }

            if (mob.PlayerSkullTimeout > 0)
            {
                mob.PlayerSkullTimeout -= 1;
            }

            if (mob.CombatTimer > 0)
            {
                mob.CombatTimer -= 1;
            }
        }

        private void UpdateNpc(ClientMob npcMob)
        {
            int nextWaypointIndex = (npcMob.WaypointCurrent + 1) % WaypointCount;

            if (npcMob.WaypointsEndSprite != nextWaypointIndex)
            {
                UpdateMobMovement(npcMob);
            }
            else
            {
                npcMob.CurrentSprite = npcMob.NextSprite;

                if (npcMob.NpcIdentifier == NpcStepAnimationIdentifier)
                {
                    npcMob.StepCount += 1;
                }
            }

            UpdateMobTimeouts(npcMob);
        }

        private void UpdatePlayer(ClientMob player)
        {
            int nextWaypointIndex = (player.WaypointCurrent + 1) % WaypointCount;

            if (player.WaypointsEndSprite != nextWaypointIndex)
            {
                UpdateMobMovement(player);
            }
            else
            {
                player.CurrentSprite = player.NextSprite;
            }

            UpdateMobTimeouts(player);
            UpdatePlayerAliveTimeout();
        }

        private void UpdatePlayerAliveTimeout()
        {
            if (client.playerAliveTimeout <= 0)
            {
                return;
            }

            client.playerAliveTimeout -= 1;

            if (client.playerAliveTimeout == 0)
            {
                client.DisplayMessage(
                    LocalisationManager.GetString("social.respawn_granted"),
                    RespawnMessageType);
            }

            if (client.playerAliveTimeout == 0)
            {
                client.DisplayMessage(
                    LocalisationManager.GetString("social.respawn_retain_skills"),
                    RespawnMessageType);
            }
        }
    }
}