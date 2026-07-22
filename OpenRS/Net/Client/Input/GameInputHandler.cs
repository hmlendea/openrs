using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Input
{
    internal sealed class GameInputHandler(GameClient client)
    {
        private readonly CameraInputHandler cameraInputHandler = new(client);
        private readonly ChatAndSleepInputHandler chatAndSleepInputHandler = new(client);
        private readonly MobMovementInputHandler mobMovementInputHandler = new(client);

        private static int CombatTimeoutMaximum => 500;

        private static int CombatTimeoutSpriteLeft => 8;

        private static int CombatTimeoutSpriteRight => 9;

        private static int SleepWordDelayTimerMaximum => 20;

        private static int SystemUpdateMinimum => 1;

        internal void CheckGameInputs()
        {
            if (TryHandleMissingPlayer())
            {
                return;
            }

            UpdateGameTimers();

            if (TryHandleAppearanceWindow())
            {
                return;
            }

            mobMovementInputHandler.UpdatePlayers();
            mobMovementInputHandler.UpdateNpcs();
            UpdateSleepWordDelayTimer();
            mobMovementInputHandler.UpdateProjectileDistances();
            cameraInputHandler.UpdateCameraTracking();
            ResetSleepWordDelayWhenExpired();

            if (chatAndSleepInputHandler.HandleSleepInput())
            {
                return;
            }

            chatAndSleepInputHandler.HandleChatAndMouseInput();
            cameraInputHandler.HandleCameraInput();
            cameraInputHandler.UpdateSceneEffects();
        }

        private void ResetSleepWordDelayWhenExpired()
        {
            if (client.sleepWordDelayTimer <= SleepWordDelayTimerMaximum)
            {
                return;
            }

            client.sleepWordDelay = false;
            client.sleepWordDelayTimer = 0;
        }

        private bool TryHandleAppearanceWindow()
        {
            if (!client.showAppearanceWindow)
            {
                return false;
            }

            client.UpdateAppearanceWindow();
            return true;
        }

        private bool TryHandleMissingPlayer()
        {
            if (client.ourPlayer is not null)
            {
                return false;
            }

            client.SendPingPacketAsync();
            return true;
        }

        private void UpdateCombatTimeout()
        {
            if (client.ourPlayer.CurrentSprite == CombatTimeoutSpriteLeft ||
                client.ourPlayer.CurrentSprite == CombatTimeoutSpriteRight)
            {
                client.combatTimeout = CombatTimeoutMaximum;
            }

            if (client.combatTimeout > 0)
            {
                client.combatTimeout -= 1;
            }
        }

        private void UpdateGameTimers()
        {
            if (client.systemUpdate > SystemUpdateMinimum)
            {
                client.systemUpdate -= 1;
            }

            client.SendPingPacketAsync();

            if (client.logoutTimer > 0)
            {
                client.logoutTimer -= 1;
            }

            UpdateCombatTimeout();
        }

        private void UpdateSleepWordDelayTimer()
        {
            if (client.drawMenuTab == 2)
            {
                return;
            }

            if (GameImage.SpiralDrawCount > 0)
            {
                client.sleepWordDelayTimer += 1;
            }

            if (GameImage.CharacterDrawCount > 0)
            {
                client.sleepWordDelayTimer = 0;
            }

            GameImage.SpiralDrawCount = 0;
            GameImage.CharacterDrawCount = 0;
        }
    }
}