using OpenRS.Net;
namespace OpenRS.Net.Client.Handlers
{
    internal sealed class BankStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.OpenBankWindow)
            {
                HandleOpenBankWindow(packetData);
                return true;
            }

            if (command == ServerCommand.CloseBankWindow)
            {
                Client.showBankBox = false;
                return true;
            }

            return false;
        }

        private void HandleOpenBankWindow(sbyte[] packetData)
        {
            Client.showBankBox = true;
            PacketReadCursor cursor = new(1);
            Client.serverBankItemsCount = PacketCursorDataReader.ReadByte(packetData, cursor);
            Client.maxBankItems = PacketCursorDataReader.ReadByte(packetData, cursor);
            PacketItemRecordReader.ReadItemAndCountRecords(
                packetData,
                cursor,
                Client.serverBankItemsCount,
                Client.serverBankItems,
                Client.serverBankItemCount);

            Client.UpdateBankItems();
        }
    }
}