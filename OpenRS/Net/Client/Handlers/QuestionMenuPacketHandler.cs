using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class QuestionMenuPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.OpenQuestionMenu)
            {
                HandleOpenQuestionMenu(packetData);
                return true;
            }

            if (command == ServerCommand.CloseQuestionMenu)
            {
                Client.showQuestionMenu = false;
                return true;
            }

            return false;
        }

        private void HandleOpenQuestionMenu(sbyte[] packetData)
        {
            Client.showQuestionMenu = true;
            int count = BinaryDataReader.GetByte(packetData[1]);
            Client.questionMenuCount = count;
            int offset = 2;

            for (int optionIndex = 0; optionIndex < count; optionIndex += 1)
            {
                int optionLength = BinaryDataReader.GetByte(packetData[offset]);
                offset += 1;
                Client.questionMenuAnswer[optionIndex] = DecodePacketString(
                    packetData,
                    offset,
                    optionLength);
                offset += optionLength;
            }
        }
    }
}