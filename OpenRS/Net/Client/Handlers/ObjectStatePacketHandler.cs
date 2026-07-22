using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class ObjectStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private readonly GroundItemStatePacketHandler groundItemStatePacketHandler = new(client);
        private readonly GameObjectStatePacketHandler gameObjectStatePacketHandler = new(client);
        private readonly WallObjectStatePacketHandler wallObjectStatePacketHandler = new(client);

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (groundItemStatePacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            if (gameObjectStatePacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            return wallObjectStatePacketHandler.TryHandlePacket(
                command,
                packetLength,
                packetData);
        }
    }
}
