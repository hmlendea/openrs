using OpenRS.Models;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class InventoryManager(EntityManager entityManager)
    {
        public static int MaximumInventorySize => 30;

        public static int MaximumBankSize => 48;

        private static int InventorySlotCount => 35;

        private static int BankSlotCount => 256;

        public int InventoryItemsCount { get; set; }

        public int BankItemsCount { get; set; }

        public int ServerBankItemsCount { get; set; }

        private InventoryItem[] inventoryItems;
        private InventoryItem[] bankItems;
        private InventoryItem[] serverBankItems;

        public void LoadContent()
        {
            InventoryItemsCount = 0;
            BankItemsCount = 0;
            ServerBankItemsCount = 0;

            inventoryItems = CreateItemSlots(InventorySlotCount);
            bankItems = CreateItemSlots(BankSlotCount);
            serverBankItems = CreateItemSlots(BankSlotCount);
        }

        public bool IsItemEquipped(int itemIndex)
        {
            for (int slotIndex = 0; slotIndex < InventoryItemsCount; slotIndex += 1)
            {
                if (inventoryItems[slotIndex].Index == itemIndex &&
                    inventoryItems[slotIndex].IsEquipped)
                {
                    return true;
                }
            }

            return false;
        }

        public void BankItem(int itemId, int itemSlot, int quantity)
        {
            if (quantity == 0)
            {
                RemoveServerBankItem(itemSlot);
            }
            else
            {
                SetServerBankItem(itemId, itemSlot, quantity);
            }

            UpdateBankItems();
        }

        public InventoryItem GetItem(int slot) => inventoryItems[slot];

        public InventoryItem GetBankItem(int slot) => bankItems[slot];

        public InventoryItem GetServerBankItem(int slot) => serverBankItems[slot];

        public void SetItem(int itemSlot, int numericalId)
            => inventoryItems[itemSlot].Index = numericalId;

        public void SetItemCount(int itemSlot, int quantity)
            => inventoryItems[itemSlot].Quantity = quantity;

        public void SetItemEquippedStatus(int itemSlot, bool isEquipped)
            => inventoryItems[itemSlot].IsEquipped = isEquipped;

        public void RemoveItem(int itemSlot)
        {
            InventoryItemsCount -= 1;

            for (int slotIndex = itemSlot; slotIndex < InventoryItemsCount; slotIndex += 1)
            {
                CopyItemValues(inventoryItems[slotIndex + 1], inventoryItems[slotIndex]);
                inventoryItems[slotIndex].IsEquipped = inventoryItems[slotIndex + 1].IsEquipped;
            }
        }

        public void UpdateBankItems()
        {
            BankItemsCount = ServerBankItemsCount;
            CopyServerBankItems();
            AppendMissingInventoryItems();
        }

        private void CopyServerBankItems()
        {
            for (int serverSlotIndex = 0; serverSlotIndex < ServerBankItemsCount; serverSlotIndex += 1)
            {
                CopyItemValues(serverBankItems[serverSlotIndex], bankItems[serverSlotIndex]);
            }
        }

        private void AppendMissingInventoryItems()
        {
            for (int itemSlot = 0; itemSlot < InventoryItemsCount; itemSlot += 1)
            {
                if (BankItemsCount >= MaximumBankSize)
                {
                    break;
                }

                int itemIndex = inventoryItems[itemSlot].Index;

                if (!IsItemInBank(itemIndex))
                {
                    bankItems[BankItemsCount].Index = itemIndex;
                    bankItems[BankItemsCount].Quantity = 0;
                    bankItems[BankItemsCount].IsEquipped = false;

                    BankItemsCount += 1;
                }
            }
        }

        public int GetItemTotalCount(int itemIndex)
        {
            int quantity = 0;

            for (int slotIndex = 0; slotIndex < InventoryItemsCount; slotIndex += 1)
            {
                if (inventoryItems[slotIndex].Index != itemIndex)
                {
                    continue;
                }

                if (entityManager.GetItem(itemIndex).IsStackable)
                {
                    quantity += 1;
                }
                else
                {
                    quantity += inventoryItems[slotIndex].Quantity;
                }
            }

            return quantity;
        }

        private bool IsItemInBank(int itemIndex)
        {
            for (int bankSlot = 0; bankSlot < BankItemsCount; bankSlot += 1)
            {
                if (bankItems[bankSlot].Index == itemIndex)
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveServerBankItem(int itemSlot)
        {
            ServerBankItemsCount -= 1;

            for (int slotIndex = itemSlot; slotIndex < ServerBankItemsCount; slotIndex += 1)
            {
                CopyItemValues(serverBankItems[slotIndex + 1], serverBankItems[slotIndex]);
            }
        }

        private void SetServerBankItem(int itemId, int itemSlot, int quantity)
        {
            serverBankItems[itemSlot].Index = itemId;
            serverBankItems[itemSlot].Quantity = quantity;

            if (itemSlot >= ServerBankItemsCount)
            {
                ServerBankItemsCount = itemSlot + 1;
            }
        }

        private static InventoryItem[] CreateItemSlots(int slotCount)
        {
            InventoryItem[] items = new InventoryItem[slotCount];

            for (int slotIndex = 0; slotIndex < items.Length; slotIndex += 1)
            {
                items[slotIndex] = new();
            }

            return items;
        }

        private static void CopyItemValues(InventoryItem source, InventoryItem destination)
        {
            destination.Index = source.Index;
            destination.Quantity = source.Quantity;
        }
    }
}
