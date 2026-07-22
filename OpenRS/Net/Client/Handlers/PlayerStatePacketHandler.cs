using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class PlayerStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private readonly InventoryStatePacketHandler inventoryStatePacketHandler = new(client);
        private readonly PlayerStatsPacketHandler playerStatsPacketHandler = new(client);
        private readonly QuestStatePacketHandler questStatePacketHandler = new(client);

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (inventoryStatePacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            if (playerStatsPacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            return questStatePacketHandler.TryHandlePacket(command, packetData);
        }
    }
}
