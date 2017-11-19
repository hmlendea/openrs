namespace RuneScapeSolo.GameLogic.GameManagers
{
    public class InventoryManager
    {
        public int maxInventoryItems;
        public int maxBankItems;

        public int[] serverBankItems;
        public int[] serverBankItemCount;
        public int serverBankItemsCount;
        public int bankItemsCount;
        public int[] bankItems;
        public int[] bankItemCount;

        public int InventoryItemsCount { get; set; }
        public int[] InventoryItems { get; set; }
        public int[] InventoryItemCount { get; set; }
        public int[] InventoryItemEquipped { get; set; }

        public InventoryManager()
        {
            bankItems = new int[256];
            bankItemCount = new int[256];
            serverBankItems = new int[256];
            serverBankItemCount = new int[256];

            InventoryItems = new int[35];
            InventoryItemCount = new int[35];
            InventoryItemEquipped = new int[35];

            maxInventoryItems = 30;
            maxBankItems = 48;
        }

        public bool IsItemEquipped(int itemIndex)
        {
            for (int i = 0; i < InventoryItemsCount; i++)
            {
                if (InventoryItems[i] == itemIndex &&
                    InventoryItemEquipped[i] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void BankItem(int itemId, int itemSlot, int itemCount)
        {
            if (itemCount == 0)
            {
                serverBankItemsCount -= 1;

                for (int i = itemSlot; i < serverBankItemsCount; i++)
                {
                    serverBankItems[i] = serverBankItems[i + 1];
                    serverBankItemCount[i] = serverBankItemCount[i + 1];
                }
            }
            else
            {
                serverBankItems[itemSlot] = itemId;
                serverBankItemCount[itemSlot] = itemCount;

                if (itemSlot >= serverBankItemsCount)
                {
                    serverBankItemsCount = itemSlot + 1;
                }
            }

            UpdateBankItems();
        }

        public void RemoveItem(int itemSlot)
        {
            InventoryItemsCount--;

            for (int i = itemSlot; i < InventoryItemsCount; i++)
            {
                InventoryItems[i] = InventoryItems[i + 1];
                InventoryItemCount[i] = InventoryItemCount[i + 1];
                InventoryItemEquipped[i] = InventoryItemEquipped[i + 1];
            }
        }

        public void UpdateBankItems()
        {
            bankItemsCount = serverBankItemsCount;
            for (int l = 0; l < serverBankItemsCount; l++)
            {
                bankItems[l] = serverBankItems[l];
                bankItemCount[l] = serverBankItemCount[l];
            }

            for (int i1 = 0; i1 < InventoryItemsCount; i1++)
            {
                if (bankItemsCount >= maxBankItems)
                {
                    break;
                }

                int j1 = InventoryItems[i1];
                bool flag = false;
                for (int k1 = 0; k1 < bankItemsCount; k1++)
                {
                    if (bankItems[k1] != j1)
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    bankItems[bankItemsCount] = j1;
                    bankItemCount[bankItemsCount] = 0;
                    bankItemsCount++;
                }
            }

        }

        public int GetItemTotalCount(int itemId)
        {
            int count = 0;

            for (int i = 0; i < InventoryItemsCount; i++)
            {
                if (InventoryItems[i] != itemId)
                {
                    continue;
                }

                if (EntityManager.GetItem(itemId).IsStackable == 1)
                {
                    count += 1;
                }
                else
                {
                    count += InventoryItemCount[i];
                }
            }

            return count;
        }
    }
}
