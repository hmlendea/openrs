using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class InventoryStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int StackMask => 0x7fff;
        private static int EquippedMaskDivisor => 32768;

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.Inventory)
            {
                HandleInventory(packetData);
                return true;
            }

            if (command == ServerCommand.UpdateItem)
            {
                HandleUpdateItem(packetData);
                return true;
            }

            if (command == ServerCommand.RemoveItem)
            {
                HandleRemoveItem(packetData);
                return true;
            }

            if (command == ServerCommand.BankItem)
            {
                HandleBankItem(packetData);
                return true;
            }

            return false;
        }

        private void HandleInventory(sbyte[] packetData)
        {
            PacketReadCursor cursor = new(1);
            Client.inventoryItemsCount = PacketCursorDataReader.ReadByte(packetData, cursor);

            for (int itemIndex = 0; itemIndex < Client.inventoryItemsCount; itemIndex += 1)
            {
                int data = PacketCursorDataReader.ReadShort(packetData, cursor);
                int itemId = data & StackMask;
                Client.inventoryItems[itemIndex] = itemId;
                Client.inventoryItemEquipped[itemIndex] = data / EquippedMaskDivisor;
                Client.inventoryItemCount[itemIndex] =
                    ReadInventoryItemCount(packetData, itemId, cursor);
            }
        }

        private int ReadInventoryItemCount(
            sbyte[] packetData,
            int itemId,
            PacketReadCursor cursor)
        {
            if (Client.entityManager.GetItem(itemId).IsStackable)
            {
                return 1;
            }

            return PacketCursorDataReader.ReadInt(packetData, cursor);
        }

        private void HandleUpdateItem(sbyte[] packetData)
        {
            int offset = 1;
            int inventorySlot = packetData[offset++] & 0xff;
            int itemCount = 1;
            int data = BinaryDataReader.GetShort(packetData, offset);
            offset += 2;
            int itemId = data & StackMask;

            if (!Client.entityManager.GetItem(itemId).IsStackable)
            {
                itemCount = BinaryDataReader.GetInt(packetData, offset);
            }

            Client.inventoryItems[inventorySlot] = itemId;
            Client.inventoryItemEquipped[inventorySlot] = data / EquippedMaskDivisor;
            Client.inventoryItemCount[inventorySlot] = itemCount;

            if (inventorySlot >= Client.inventoryItemsCount)
            {
                Client.inventoryItemsCount = inventorySlot + 1;
            }
        }

        private void HandleRemoveItem(sbyte[] packetData)
        {
            int inventorySlot = packetData[1] & 0xff;
            Client.inventoryItemsCount -= 1;

            for (int itemIndex = inventorySlot;
                itemIndex < Client.inventoryItemsCount;
                itemIndex += 1)
            {
                Client.inventoryItems[itemIndex] = Client.inventoryItems[itemIndex + 1];
                Client.inventoryItemCount[itemIndex] = Client.inventoryItemCount[itemIndex + 1];
                Client.inventoryItemEquipped[itemIndex] =
                    Client.inventoryItemEquipped[itemIndex + 1];
            }
        }

        private void HandleBankItem(sbyte[] packetData)
        {
            int offset = 1;
            int itemSlot = packetData[offset++] & 0xff;
            int itemId = BinaryDataReader.GetShort(packetData, offset);
            offset += 2;
            int itemCount = BinaryDataReader.GetInt(packetData, offset);

            if (itemCount == 0)
            {
                RemoveBankItem(itemSlot);
            }
            else
            {
                UpdateBankItem(itemSlot, itemId, itemCount);
            }

            Client.UpdateBankItems();
        }

        private void RemoveBankItem(int itemSlot)
        {
            Client.serverBankItemsCount -= 1;

            for (int bankIndex = itemSlot;
                bankIndex < Client.serverBankItemsCount;
                bankIndex += 1)
            {
                Client.serverBankItems[bankIndex] = Client.serverBankItems[bankIndex + 1];
                Client.serverBankItemCount[bankIndex] =
                    Client.serverBankItemCount[bankIndex + 1];
            }
        }

        private void UpdateBankItem(int itemSlot, int itemId, int itemCount)
        {
            Client.serverBankItems[itemSlot] = itemId;
            Client.serverBankItemCount[itemSlot] = itemCount;

            if (itemSlot >= Client.serverBankItemsCount)
            {
                Client.serverBankItemsCount = itemSlot + 1;
            }
        }
    }
}