using OpenRS.Net;
namespace OpenRS.Net.Client.Handlers
{
    internal sealed class SystemPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private readonly AlertPacketHandler alertPacketHandler = new(client);
        private readonly ClientStatusPacketHandler clientStatusPacketHandler = new(client);
        private readonly SilentProgressPacketHandler silentProgressPacketHandler = new();

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (alertPacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            if (clientStatusPacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            return silentProgressPacketHandler.TryHandlePacket(command);
        }
    }
}
