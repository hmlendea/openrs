using OpenRS.Net;
namespace OpenRS.Net.Client.Handlers
{
    internal sealed class DuelStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int EnabledFlag => 1;

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.OpenDuelWindow)
            {
                HandleOpenDuelWindow(packetData);
                return true;
            }

            if (command == ServerCommand.CloseDuelWindow)
            {
                HandleCloseDuelWindow();
                return true;
            }

            if (command == ServerCommand.DuelItems)
            {
                HandleDuelItems(packetData);
                return true;
            }

            if (command == ServerCommand.DuelSettings)
            {
                HandleDuelSettings(packetData);
                return true;
            }

            if (command == ServerCommand.DuelAcceptedByOther)
            {
                Client.duelOpponentAccepted = packetData[1] == EnabledFlag;
                return true;
            }

            if (command == ServerCommand.DuelAcceptedBySelf)
            {
                Client.duelMyAccepted = packetData[1] == EnabledFlag;
                return true;
            }

            if (command == ServerCommand.DuelConfirmation)
            {
                HandleDuelConfirmation(packetData);
                return true;
            }

            return false;
        }

        private void HandleOpenDuelWindow(sbyte[] packetData)
        {
            PacketReadCursor cursor = new(1);
            int opponentIndex = PacketCursorDataReader.ReadShort(packetData, cursor);

            if (Client.playerBufferArray[opponentIndex] is not null)
            {
                Client.duelOpponent = Client.playerBufferArray[opponentIndex].Username;
            }

            Client.showDuelBox = true;
            Client.duelMyItemCount = 0;
            Client.duelOpponentItemCount = 0;
            Client.duelOpponentAccepted = false;
            Client.duelMyAccepted = false;
            Client.duelNoRetreating = false;
            Client.duelNoMagic = false;
            Client.duelNoPrayer = false;
            Client.duelNoWeapons = false;
        }

        private void HandleCloseDuelWindow()
        {
            Client.showDuelBox = false;
            Client.showDuelConfirmBox = false;
        }

        private void HandleDuelItems(sbyte[] packetData)
        {
            PacketReadCursor cursor = new(1);
            Client.duelOpponentItemCount = PacketItemRecordReader.ReadCountPrefixedItemAndCountRecords(
                packetData,
                cursor,
                Client.duelOpponentItems,
                Client.duelOpponentItemsCount);

            Client.duelOpponentAccepted = false;
            Client.duelMyAccepted = false;
        }

        private void HandleDuelSettings(sbyte[] packetData)
        {
            Client.duelNoRetreating = packetData[1] == EnabledFlag;
            Client.duelNoMagic = packetData[2] == EnabledFlag;
            Client.duelNoPrayer = packetData[3] == EnabledFlag;
            Client.duelNoWeapons = packetData[4] == EnabledFlag;
            Client.duelOpponentAccepted = false;
            Client.duelMyAccepted = false;
        }

        private void HandleDuelConfirmation(sbyte[] packetData)
        {
            Client.showDuelConfirmBox = true;
            Client.duelConfirmOurAccepted = false;
            Client.showDuelBox = false;
            PacketReadCursor cursor = new(1);
            Client.duelOpponentHash = PacketCursorDataReader.ReadLong(packetData, cursor);
            ReadDuelOpponentStake(packetData, cursor);
            ReadDuelOurStake(packetData, cursor);
            Client.duelRetreat = PacketCursorDataReader.ReadByte(packetData, cursor);
            Client.duelMagic = PacketCursorDataReader.ReadByte(packetData, cursor);
            Client.duelPrayer = PacketCursorDataReader.ReadByte(packetData, cursor);
            Client.duelWeapons = PacketCursorDataReader.ReadByte(packetData, cursor);
        }

        private void ReadDuelOpponentStake(
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            Client.duelOpponentStakeCount =
                PacketItemRecordReader.ReadCountPrefixedItemAndCountRecords(
                    packetData,
                    cursor,
                    Client.duelOpponentStakeItem,
                    Client.duelOutStakeItemCount);
        }

        private void ReadDuelOurStake(
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            Client.duelOurStakeCount =
                PacketItemRecordReader.ReadCountPrefixedItemAndCountRecords(
                    packetData,
                    cursor,
                    Client.duelOurStakeItem,
                    Client.duelOurStakeItemCount);
        }
    }
}