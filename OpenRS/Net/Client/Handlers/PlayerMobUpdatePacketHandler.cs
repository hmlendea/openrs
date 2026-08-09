using OpenRS.Net;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class PlayerMobUpdatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int HealthStatIndex => 3;
        private static int PlayerMessageType => 2;
        private static int EchoMessageType => 5;
        private static int AppearanceItemsCount => 12;
        private static int NoAttackingPlayer => -1;
        private static int NoAttackingNpc => -1;

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command != ServerCommand.PlayerUpdates)
            {
                return false;
            }

            HandlePlayerUpdates(packetData);
            return true;
        }

        private void HandlePlayerUpdates(sbyte[] packetData)
        {
            int mobCount = BinaryDataReader.GetShort(packetData, 1);
            PacketReadCursor cursor = new(3);

            for (int mobIndex = 0; mobIndex < mobCount; mobIndex += 1)
            {
                if (!TryReadUpdatedPlayer(packetData, cursor, out ClientMob mob))
                {
                    return;
                }

                sbyte updateType = packetData[cursor.Index++];
                HandlePlayerUpdateType(mob, updateType, packetData, cursor);
            }
        }

        private bool TryReadUpdatedPlayer(
            sbyte[] packetData,
            PacketReadCursor cursor,
            out ClientMob mob)
        {
            int playerIndex = PacketCursorDataReader.ReadShort(packetData, cursor);

            if (playerIndex < 0 || playerIndex > Client.playerBufferArray.Length)
            {
                mob = null;
                return false;
            }

            mob = Client.playerBufferArray[playerIndex];
            return mob is not null;
        }

        private void HandlePlayerUpdateType(
            ClientMob mob,
            sbyte updateType,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            if (updateType == 0)
            {
                HandleItemAboveHeadUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 1)
            {
                HandlePublicChatUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 2)
            {
                HandleHitUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 3)
            {
                HandleNpcProjectileUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 4)
            {
                HandlePlayerProjectileUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 5)
            {
                HandleAppearanceUpdate(mob, packetData, cursor);
                return;
            }

            if (updateType == 6)
            {
                HandlePrivateMessageEchoUpdate(mob, packetData, cursor);
            }
        }

        private void HandleItemAboveHeadUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            mob.PlayerSkullTimeout = PacketHandlerConstants.MessageTimeout;
            mob.ItemAboveHeadId = PacketCursorDataReader.ReadShort(packetData, cursor);
        }

        private void HandlePublicChatUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            sbyte messageLength = packetData[cursor.Index++];
            string message = ChatMessage.BytesToString(packetData, cursor.Index, messageLength);

            if (Client.useChatFilter && Client.textCensor is not null)
            {
                message = Client.textCensor.Censor(message);
            }

            if (!ShouldIgnoreMob(mob))
            {
                mob.LastMessageTimeout = PacketHandlerConstants.MessageTimeout;
                mob.LastMessage = message;
                Client.DisplayMessage(mob.Username + ": " + mob.LastMessage, PlayerMessageType);
            }

            cursor.Index += messageLength;
        }

        private bool ShouldIgnoreMob(ClientMob mob)
        {
            for (int ignoreIndex = 0; ignoreIndex < Client.ignoresCount; ignoreIndex += 1)
            {
                if (Client.ignoresList[ignoreIndex] == mob.NameHash)
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleHitUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            mob.LastDamageCount = PacketCursorDataReader.ReadByte(packetData, cursor);
            mob.CurrentHitpoints = PacketCursorDataReader.ReadByte(packetData, cursor);
            mob.BaseHitpoints = PacketCursorDataReader.ReadByte(packetData, cursor);
            mob.CombatTimer = PacketHandlerConstants.CombatTimer;

            if (mob == Client.ourPlayer)
            {
                Client.playerStatCurrent[HealthStatIndex] = mob.CurrentHitpoints;
                Client.playerStatBase[HealthStatIndex] = mob.BaseHitpoints;
                Client.showWelcomeBox = false;
                Client.showServerMessageBox = false;
            }
        }

        private void HandleNpcProjectileUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            mob.ProjectileType = PacketCursorDataReader.ReadShort(packetData, cursor);
            mob.AttackingNpcIndex = PacketCursorDataReader.ReadShort(packetData, cursor);
            mob.AttackingPlayerIndex = NoAttackingPlayer;
            mob.ProjectileDistance = Client.projectileRange;
        }

        private void HandlePlayerProjectileUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            mob.ProjectileType = PacketCursorDataReader.ReadShort(packetData, cursor);
            mob.AttackingPlayerIndex = PacketCursorDataReader.ReadShort(packetData, cursor);
            mob.AttackingNpcIndex = NoAttackingNpc;
            mob.ProjectileDistance = Client.projectileRange;
        }

        private void HandleAppearanceUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            mob.ServerId = PacketCursorDataReader.ReadShort(packetData, cursor);
            mob.NameHash = PacketCursorDataReader.ReadLong(packetData, cursor);
            mob.Username = PlayerNameEncoder.HashToName(mob.NameHash);
            int appearanceCount = PacketCursorDataReader.ReadByte(packetData, cursor);
            LoadAppearanceItems(mob, packetData, cursor, appearanceCount);
            ClearRemainingAppearanceItems(mob, appearanceCount);
            mob.Appearance.HairColour = packetData[cursor.Index++] & 0xff;
            mob.Appearance.TopColour = packetData[cursor.Index++] & 0xff;
            mob.Appearance.TrousersColour = packetData[cursor.Index++] & 0xff;
            mob.Appearance.SkinColour = packetData[cursor.Index++] & 0xff;
            mob.CombatLevel = packetData[cursor.Index++] & 0xff;
            mob.PlayerSkulled = packetData[cursor.Index++] & 0xff;
            mob.Admin = packetData[cursor.Index++] & 0xff;
        }

        private void LoadAppearanceItems(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor,
            int appearanceCount)
        {
            for (int appearanceIndex = 0;
                appearanceIndex < appearanceCount;
                appearanceIndex += 1)
            {
                mob.AppearanceItems[appearanceIndex] =
                    PacketCursorDataReader.ReadByte(packetData, cursor);
            }
        }

        private void ClearRemainingAppearanceItems(ClientMob mob, int appearanceCount)
        {
            for (int appearanceIndex = appearanceCount;
                appearanceIndex < AppearanceItemsCount;
                appearanceIndex += 1)
            {
                mob.AppearanceItems[appearanceIndex] = 0;
            }
        }

        private void HandlePrivateMessageEchoUpdate(
            ClientMob mob,
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            sbyte messageLength = packetData[cursor.Index++];
            string message = ChatMessage.BytesToString(packetData, cursor.Index, messageLength);
            mob.LastMessageTimeout = PacketHandlerConstants.MessageTimeout;
            mob.LastMessage = message;

            if (mob == Client.ourPlayer)
            {
                Client.DisplayMessage(mob.Username + ": " + mob.LastMessage, EchoMessageType);
            }

            cursor.Index += messageLength;
        }
    }
}