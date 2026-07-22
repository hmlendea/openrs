using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class AppearancePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        internal bool TryHandlePacket(ServerCommand command)
        {
            if (command != ServerCommand.ShowAppearanceWindow)
            {
                return false;
            }

            Client.showAppearanceWindow = true;
            return true;
        }
    }
}