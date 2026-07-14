using OpenRS.Models;

namespace OpenRS.GameLogic.GameManagers
{
    public sealed class InventoryManager(EntityManager entityManager)
    {
        public static int MaximumInventorySize => 30;

        public static int MaximumBankSize => 48;

        public int InventoryItemsCount { get; set; }

        public int BankItemsCount { get; set; }

        public int ServerBankItemsCount { get; set; }

        private InventoryItem[] inventoryItems;
        private InventoryItem[] bankItems;
        private InventoryItem[] serverBankItems;

        public void LoadContent()
        {
            inventoryItems = new InventoryItem[35];
            bankItems = new InventoryItem[256];
            serverBankItems = new InventoryItem[256];

            for (int slotIndex = 0; slotIndex < inventoryItems.Length; slotIndex += 1)
            {
                inventoryItems[slotIndex] = new();
            }

            for (int slotIndex = 0; slotIndex < bankItems.Length; slotIndex += 1)
            {
                bankItems[slotIndex] = new();
                serverBankItems[slotIndex] = new();
            }
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
                ServerBankItemsCount -= 1;

                for (int slotIndex = itemSlot; slotIndex < ServerBankItemsCount; slotIndex += 1)
                {
                    serverBankItems[slotIndex].Index = serverBankItems[slotIndex + 1].Index;
                    serverBankItems[slotIndex].Quantity = serverBankItems[slotIndex + 1].Quantity;
                }
            }
            else
            {
                serverBankItems[itemSlot].Index = itemId;
                serverBankItems[itemSlot].Quantity = quantity;

                if (itemSlot >= ServerBankItemsCount)
                {
                    ServerBankItemsCount = itemSlot + 1;
                }
            }

            UpdateBankItems();
        }

        public InventoryItem GetItem(int slot) => inventoryItems[slot];

        public InventoryItem GetBankItem(int slot) => bankItems[slot];

        public InventoryItem GetServerBankItem(int slot) => serverBankItems[slot];

        public void SetItem(int itemSlot, int numericalId) => inventoryItems[itemSlot].Index = numericalId;

        public void SetItemCount(int itemSlot, int quantity) => inventoryItems[itemSlot].Quantity = quantity;

        public void SetItemEquippedStatus(int itemSlot, bool isEquipped) => inventoryItems[itemSlot].IsEquipped = isEquipped;

        public void RemoveItem(int itemSlot)
        {
            InventoryItemsCount -= 1;

            for (int slotIndex = itemSlot; slotIndex < InventoryItemsCount; slotIndex += 1)
            {
                inventoryItems[slotIndex].Index = inventoryItems[slotIndex + 1].Index;
                inventoryItems[slotIndex].Quantity = inventoryItems[slotIndex + 1].Quantity;
                inventoryItems[slotIndex].IsEquipped = inventoryItems[slotIndex + 1].IsEquipped;
            }
        }

        public void UpdateBankItems()
        {
            BankItemsCount = ServerBankItemsCount;

            for (int serverSlotIndex = 0; serverSlotIndex < ServerBankItemsCount; serverSlotIndex += 1)
            {
                bankItems[serverSlotIndex].Index = serverBankItems[serverSlotIndex].Index;
                bankItems[serverSlotIndex].Quantity = serverBankItems[serverSlotIndex].Quantity;
            }

            for (int itemSlot = 0; itemSlot < InventoryItemsCount; itemSlot += 1)
            {
                if (BankItemsCount >= MaximumBankSize)
                {
                    break;
                }

                int itemIndex = inventoryItems[itemSlot].Index;
                bool isAlreadyInBank = false;

                for (int bankSlot = 0; bankSlot < BankItemsCount; bankSlot += 1)
                {
                    if (bankItems[bankSlot].Index == itemIndex)
                    {
                        isAlreadyInBank = true;
                        break;
                    }
                }

                if (!isAlreadyInBank)
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

                if (entityManager.GetItem(itemIndex).IsStackable == 1)
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
    }
}
