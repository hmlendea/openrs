using OpenRS.Models;

namespace OpenRS.GameLogic.GameManagers
{
    internal sealed class InventoryBank
    {
        private InventoryItem[] items;
        private InventoryItem[] serverItems;

        internal void Reset(InventoryItem[] items, InventoryItem[] serverItems)
        {
            this.items = items;
            this.serverItems = serverItems;
        }

        internal InventoryItem GetItem(int slot) => items[slot];

        internal InventoryItem GetServerItem(int slot) => serverItems[slot];

        internal void SetItem(
            InventoryManager inventoryManager,
            int itemId,
            int itemSlot,
            int quantity,
            InventoryItem[] inventoryItems,
            int inventoryItemsCount,
            int maximumVisibleItems)
        {
            if (quantity == 0)
            {
                RemoveServerItem(inventoryManager, itemSlot);
            }
            else
            {
                SetServerItem(inventoryManager, itemId, itemSlot, quantity);
            }

            Synchronise(
                inventoryManager,
                inventoryItems,
                inventoryItemsCount,
                maximumVisibleItems);
        }

        internal void Synchronise(
            InventoryManager inventoryManager,
            InventoryItem[] inventoryItems,
            int inventoryItemsCount,
            int maximumVisibleItems)
        {
            inventoryManager.BankItemsCount = inventoryManager.ServerBankItemsCount;
            CopyServerItems(inventoryManager.ServerBankItemsCount);
            AppendMissingInventoryItems(
                inventoryManager,
                inventoryItems,
                inventoryItemsCount,
                maximumVisibleItems);
        }

        private void CopyServerItems(int serverItemsCount)
        {
            for (int serverSlotIndex = 0;
                serverSlotIndex < serverItemsCount;
                serverSlotIndex += 1)
            {
                CopyItemValues(serverItems[serverSlotIndex], items[serverSlotIndex]);
            }
        }

        private void AppendMissingInventoryItems(
            InventoryManager inventoryManager,
            InventoryItem[] inventoryItems,
            int inventoryItemsCount,
            int maximumVisibleItems)
        {
            for (int itemSlot = 0; itemSlot < inventoryItemsCount; itemSlot += 1)
            {
                if (inventoryManager.BankItemsCount >= maximumVisibleItems)
                {
                    break;
                }

                int itemIndex = inventoryItems[itemSlot].Index;

                if (!ContainsItem(itemIndex, inventoryManager.BankItemsCount))
                {
                    items[inventoryManager.BankItemsCount].Index = itemIndex;
                    items[inventoryManager.BankItemsCount].Quantity = 0;
                    items[inventoryManager.BankItemsCount].IsEquipped = false;

                    inventoryManager.BankItemsCount += 1;
                }
            }
        }

        private bool ContainsItem(int itemIndex, int itemsCount)
        {
            for (int itemSlot = 0; itemSlot < itemsCount; itemSlot += 1)
            {
                if (items[itemSlot].Index == itemIndex)
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveServerItem(
            InventoryManager inventoryManager,
            int itemSlot)
        {
            inventoryManager.ServerBankItemsCount -= 1;

            for (int slotIndex = itemSlot;
                slotIndex < inventoryManager.ServerBankItemsCount;
                slotIndex += 1)
            {
                CopyItemValues(serverItems[slotIndex + 1], serverItems[slotIndex]);
            }
        }

        private void SetServerItem(
            InventoryManager inventoryManager,
            int itemId,
            int itemSlot,
            int quantity)
        {
            serverItems[itemSlot].Index = itemId;
            serverItems[itemSlot].Quantity = quantity;

            if (itemSlot >= inventoryManager.ServerBankItemsCount)
            {
                inventoryManager.ServerBankItemsCount = itemSlot + 1;
            }
        }

        private static void CopyItemValues(
            InventoryItem source,
            InventoryItem destination)
        {
            destination.Index = source.Index;
            destination.Quantity = source.Quantity;
        }
    }
}