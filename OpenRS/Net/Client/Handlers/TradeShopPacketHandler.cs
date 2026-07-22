using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class TradeShopPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private readonly TradeStatePacketHandler tradeStatePacketHandler = new(client);
        private readonly ShopStatePacketHandler shopStatePacketHandler = new(client);
        private readonly BankStatePacketHandler bankStatePacketHandler = new(client);

        internal bool TryHandlePacket(ServerCommand command, int packetLength, sbyte[] packetData)
        {
            if (tradeStatePacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            if (shopStatePacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            return bankStatePacketHandler.TryHandlePacket(command, packetData);
        }
    }
}
