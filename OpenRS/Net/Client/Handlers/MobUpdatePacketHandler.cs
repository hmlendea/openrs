using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class MobUpdatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private readonly PlayerMobUpdatePacketHandler playerMobUpdatePacketHandler = new(client);
        private readonly NpcMobUpdatePacketHandler npcMobUpdatePacketHandler = new(client);

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (playerMobUpdatePacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            return npcMobUpdatePacketHandler.TryHandlePacket(command, packetData);
        }
    }
}
