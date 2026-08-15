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

        private readonly InventoryBank inventoryBank = new();
        private readonly InventoryItemCollection inventoryItems = new(entityManager);

        public void LoadContent()
        {
            InventoryItemsCount = 0;
            BankItemsCount = 0;
            ServerBankItemsCount = 0;

            inventoryItems.Reset(CreateItemSlots(InventorySlotCount));
            inventoryBank.Reset(
                CreateItemSlots(BankSlotCount),
                CreateItemSlots(BankSlotCount));
        }

        public bool IsItemEquipped(int itemIndex)
            => inventoryItems.IsItemEquipped(itemIndex, InventoryItemsCount);

        public void BankItem(int itemId, int itemSlot, int quantity)
            => inventoryBank.SetItem(
                this,
                itemId,
                itemSlot,
                quantity,
                inventoryItems.Items,
                InventoryItemsCount,
                MaximumBankSize);

        public InventoryItem GetItem(int slot) => inventoryItems.GetItem(slot);

        public InventoryItem GetBankItem(int slot) => inventoryBank.GetItem(slot);

        public InventoryItem GetServerBankItem(int slot)
            => inventoryBank.GetServerItem(slot);

        public void SetItem(int itemSlot, int numericalId)
            => inventoryItems.SetItem(itemSlot, numericalId);

        public void SetItemCount(int itemSlot, int quantity)
            => inventoryItems.SetItemCount(itemSlot, quantity);

        public void SetItemEquippedStatus(int itemSlot, bool isEquipped)
            => inventoryItems.SetItemEquippedStatus(itemSlot, isEquipped);

        public void RemoveItem(int itemSlot)
            => inventoryItems.RemoveItem(this, itemSlot);

        public void UpdateBankItems()
            => inventoryBank.Synchronise(
                this,
                inventoryItems.Items,
                InventoryItemsCount,
                MaximumBankSize);

        public int GetItemTotalCount(int itemIndex)
            => inventoryItems.GetItemTotalCount(itemIndex, InventoryItemsCount);

        private static InventoryItem[] CreateItemSlots(int slotCount)
        {
            InventoryItem[] items = new InventoryItem[slotCount];

            for (int slotIndex = 0; slotIndex < items.Length; slotIndex += 1)
            {
                items[slotIndex] = new();
            }

            return items;
        }
    }
}
