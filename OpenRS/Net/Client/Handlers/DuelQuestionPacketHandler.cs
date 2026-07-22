using OpenRS.Net;
namespace OpenRS.Net.Client.Handlers
{
    internal sealed class DuelQuestionPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private readonly QuestionMenuPacketHandler questionMenuPacketHandler = new(client);
        private readonly AppearancePacketHandler appearancePacketHandler = new(client);
        private readonly DuelStatePacketHandler duelStatePacketHandler = new(client);

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (questionMenuPacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            if (appearancePacketHandler.TryHandlePacket(command))
            {
                return true;
            }

            return duelStatePacketHandler.TryHandlePacket(command, packetData);
        }
    }
}
