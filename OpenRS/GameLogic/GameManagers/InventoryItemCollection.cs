using OpenRS.Models;

namespace OpenRS.GameLogic.GameManagers
{
    internal sealed class InventoryItemCollection(EntityManager entityManager)
    {
        private InventoryItem[] items;

        internal InventoryItem[] Items => items;

        internal void Reset(InventoryItem[] items)
        {
            this.items = items;
        }

        internal InventoryItem GetItem(int slot) => items[slot];

        internal void SetItem(int itemSlot, int numericalId)
            => items[itemSlot].Index = numericalId;

        internal void SetItemCount(int itemSlot, int quantity)
            => items[itemSlot].Quantity = quantity;

        internal void SetItemEquippedStatus(int itemSlot, bool isEquipped)
            => items[itemSlot].IsEquipped = isEquipped;

        internal bool IsItemEquipped(int itemIndex, int itemsCount)
        {
            for (int slotIndex = 0; slotIndex < itemsCount; slotIndex += 1)
            {
                if (items[slotIndex].Index == itemIndex &&
                    items[slotIndex].IsEquipped)
                {
                    return true;
                }
            }

            return false;
        }

        internal void RemoveItem(InventoryManager inventoryManager, int itemSlot)
        {
            inventoryManager.InventoryItemsCount -= 1;

            for (int slotIndex = itemSlot;
                slotIndex < inventoryManager.InventoryItemsCount;
                slotIndex += 1)
            {
                CopyItemValues(items[slotIndex + 1], items[slotIndex]);
                items[slotIndex].IsEquipped = items[slotIndex + 1].IsEquipped;
            }
        }

        internal int GetItemTotalCount(int itemIndex, int itemsCount)
        {
            int quantity = 0;

            for (int slotIndex = 0; slotIndex < itemsCount; slotIndex += 1)
            {
                if (items[slotIndex].Index != itemIndex)
                {
                    continue;
                }

                if (entityManager.GetItem(itemIndex).IsStackable)
                {
                    quantity += 1;
                }
                else
                {
                    quantity += items[slotIndex].Quantity;
                }
            }

            return quantity;
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