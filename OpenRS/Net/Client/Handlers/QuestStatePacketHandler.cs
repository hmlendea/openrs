using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class QuestStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.WontImplement182)
            {
                HandleLegacyQuestStatus(packetData);
                return true;
            }

            if (command == ServerCommand.WontImplement233)
            {
                HandleCompactQuestStatus(packetData);
                return true;
            }

            return false;
        }

        private void HandleLegacyQuestStatus(sbyte[] packetData)
        {
            Client.questPoints = BinaryDataReader.GetShort(packetData, 1);

            for (int questIndex = 0; questIndex < Client.questName.Length; questIndex += 1)
            {
                Client.questStage[questIndex] = packetData[questIndex + 1];
            }
        }

        private void HandleCompactQuestStatus(sbyte[] packetData)
        {
            Client.questPoints = BinaryDataReader.GetByte(packetData[1]);
            int count = BinaryDataReader.GetByte(packetData[2]);
            int offset = 3;
            string[] usedQuestNames = new string[count];
            int[] questStages = new int[count];

            for (int questIndex = 0; questIndex < count; questIndex += 1)
            {
                usedQuestNames[questIndex] = Client.questName[
                    BinaryDataReader.GetByte(packetData[offset++])];
                questStages[questIndex] = BinaryDataReader.GetByte(packetData[offset++]);
            }

            Client.usedQuestName = usedQuestNames;
            Client.questStage = questStages;
        }
    }
}