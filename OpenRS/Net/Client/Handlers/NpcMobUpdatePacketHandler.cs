using OpenRS.Localisation;
using OpenRS.Net;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class NpcMobUpdatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int NpcMessageType => 5;

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command != ServerCommand.NpcUpdates)
            {
                return false;
            }

            HandleNpcUpdates(packetData);
            return true;
        }

        private void HandleNpcUpdates(sbyte[] packetData)
        {
            int mobCount = BinaryDataReader.GetShort(packetData, 1);
            PacketReadCursor cursor = new(3);

            for (int mobIndex = 0; mobIndex < mobCount; mobIndex += 1)
            {
                int npcIndex = PacketCursorDataReader.ReadShort(packetData, cursor);
                ClientMob mob = Client.npcAttackingArray[npcIndex];
                int updateType = PacketCursorDataReader.ReadByte(packetData, cursor);
                HandleNpcUpdateType(mob, updateType, packetData, cursor);
            }
        }

        private void HandleNpcUpdateType(
            ClientMob mob,
            int updateType,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            if (updateType == 1)
            {
                HandleNpcSpeechUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 2)
            {
                HandleNpcHitUpdate(mob, packetData, cursor);
            }
        }

        private void HandleNpcSpeechUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            int playerIndex = PacketCursorDataReader.ReadShort(packetData, cursor);
            sbyte messageLength = packetData[cursor.Index++];

            if (mob is not null)
            {
                string message = ChatMessage.BytesToString(packetData, cursor.Index, messageLength);
                mob.LastMessageTimeout = PacketHandlerConstants.MessageTimeout;
                mob.LastMessage = message;

                if (playerIndex == Client.ourPlayer.ServerIndex)
                {
                    Client.DisplayMessage(
                        string.Format(
                            LocalisationManager.GetString("social.npc_message"),
                            Client.entityManager.GetNpc(mob.NpcIdentifier).Name,
                            mob.LastMessage),
                        NpcMessageType);
                }
            }

            cursor.Index += messageLength;
        }

        private void HandleNpcHitUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            int lastDamageCount = PacketCursorDataReader.ReadByte(packetData, cursor);
            int currentHits = PacketCursorDataReader.ReadByte(packetData, cursor);
            int baseHits = PacketCursorDataReader.ReadByte(packetData, cursor);

            if (mob is null)
            {
                return;
            }

            mob.LastDamageCount = lastDamageCount;
            mob.CurrentHitpoints = currentHits;
            mob.BaseHitpoints = baseHits;
            mob.CombatTimer = PacketHandlerConstants.CombatTimer;
        }
    }
}