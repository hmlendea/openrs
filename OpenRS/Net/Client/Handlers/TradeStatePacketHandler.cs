using OpenRS.Net;
namespace OpenRS.Net.Client.Handlers
{
    internal sealed class TradeStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.OpenTradeWindow)
            {
                HandleOpenTradeWindow(packetData);
                return true;
            }

            if (command == ServerCommand.CloseTradeWindow)
            {
                HandleCloseTradeWindow();
                return true;
            }

            if (command == ServerCommand.TradeItems)
            {
                HandleTradeItems(packetData);
                return true;
            }

            if (command == ServerCommand.TradeAcceptedByOther)
            {
                Client.tradeOtherAccepted = packetData[1] == 1;
                return true;
            }

            if (command == ServerCommand.TradeAcceptedBySelf)
            {
                Client.tradeWeAccepted = packetData[1] == 1;
                return true;
            }

            if (command == ServerCommand.TradeConfirmation)
            {
                HandleTradeConfirmation(packetData);
                return true;
            }

            return false;
        }

        private void HandleOpenTradeWindow(sbyte[] packetData)
        {
            PacketReadCursor cursor = new(1);
            int otherPlayerIndex = PacketCursorDataReader.ReadShort(packetData, cursor);

            if (Client.playerBufferArray[otherPlayerIndex] is not null)
            {
                Client.tradeOtherName = Client.playerBufferArray[otherPlayerIndex].Username;
            }

            Client.showTradeBox = true;
            Client.tradeOtherAccepted = false;
            Client.tradeWeAccepted = false;
            Client.tradeItemsOurCount = 0;
            Client.tradeItemsOtherCount = 0;
        }

        private void HandleCloseTradeWindow()
        {
            Client.showTradeBox = false;
            Client.showTradeConfirmBox = false;
        }

        private void HandleTradeItems(sbyte[] packetData)
        {
            PacketReadCursor cursor = new(1);
            Client.tradeItemsOtherCount = PacketItemRecordReader.ReadCountPrefixedItemAndCountRecords(
                packetData,
                cursor,
                Client.tradeItemsOther,
                Client.tradeItemOtherCount);

            Client.tradeOtherAccepted = false;
            Client.tradeWeAccepted = false;
        }

        private void HandleTradeConfirmation(sbyte[] packetData)
        {
            Client.showTradeConfirmBox = true;
            Client.tradeConfirmAccepted = false;
            Client.showTradeBox = false;
            PacketReadCursor cursor = new(1);
            Client.tradeConfirmOtherNameLong = PacketCursorDataReader.ReadLong(packetData, cursor);
            ReadTradeConfirmationOtherItems(packetData, cursor);
            ReadTradeConfirmationOurItems(packetData, cursor);
        }

        private void ReadTradeConfirmationOtherItems(
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            Client.tradeConfirmOtherItemCount =
                PacketItemRecordReader.ReadCountPrefixedItemAndCountRecords(
                    packetData,
                    cursor,
                    Client.tradeConfirmOtherItems,
                    Client.tradeConfirmOtherItemsCount);
        }

        private void ReadTradeConfirmationOurItems(
            sbyte[] packetData,
            PacketReadCursor cursor)
        {
            Client.tradeConfigItemCount =
                PacketItemRecordReader.ReadCountPrefixedItemAndCountRecords(
                    packetData,
                    cursor,
                    Client.tradeConfirmItems,
                    Client.tradeConfigItemsCount);
        }
    }
}