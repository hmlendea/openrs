using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class ClientStatusPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int SystemUpdateMultiplier => 32;
        private static string IncorrectSleepStatusText => "Incorrect - Please wait...";

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.FatigueChange)
            {
                Client.fatigue = BinaryDataReader.GetShort(packetData, 1);
                return true;
            }

            if (command == ServerCommand.KillingSpree)
            {
                Client.killingSpree = BinaryDataReader.GetShort(packetData, 1);
                return true;
            }

            if (command == ServerCommand.Awake)
            {
                Client.isSleeping = false;
                return true;
            }

            if (command == ServerCommand.WontImplement225)
            {
                Client.sleepingStatusText = IncorrectSleepStatusText;
                return true;
            }

            if (command == ServerCommand.SystemUpdateTimer)
            {
                Client.systemUpdate = BinaryDataReader.GetShort(packetData, 1) *
                    SystemUpdateMultiplier;
                return true;
            }

            if (command == ServerCommand.TakeScreenshot)
            {
                if (Client.autoScreenshot)
                {
                    Client.TakeScreenshot(false);
                }

                return true;
            }

            return false;
        }
    }
}