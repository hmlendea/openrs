using OpenRS.Net;
namespace OpenRS.Net.Client.Handlers
{
    internal sealed class ShopStatePacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int InventoryExclusionItem => 10;
        private static int MaximumShopItemCount => 40;
        private static int ItemIdMask => 0x7fff;
        private static int InvalidShopItemIndex => -1;
        private static int InvalidShopItemType => -2;

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.OpenShopWindow)
            {
                HandleOpenShopWindow(packetData);
                return true;
            }

            if (command == ServerCommand.CloseShopWindow)
            {
                Client.showShopBox = false;
                return true;
            }

            return false;
        }

        private void HandleOpenShopWindow(sbyte[] packetData)
        {
            Client.showShopBox = true;
            PacketReadCursor cursor = new(1);
            int newShopItemCount = PacketCursorDataReader.ReadByte(packetData, cursor);
            bool isGeneralShop = PacketCursorDataReader.ReadByte(packetData, cursor) == 1;
            Client.shopItemSellPriceModifier = PacketCursorDataReader.ReadByte(packetData, cursor);
            Client.shopItemBuyPriceModifier = PacketCursorDataReader.ReadByte(packetData, cursor);
            ClearShopItems();
            ReadShopItems(packetData, newShopItemCount, cursor);

            if (isGeneralShop)
            {
                AppendGeneralShopInventory(newShopItemCount);
            }

            ResetSelectedShopItemIfInvalid();
        }

        private void ClearShopItems()
        {
            for (int itemIndex = 0; itemIndex < MaximumShopItemCount; itemIndex += 1)
            {
                Client.shopItems[itemIndex] = InvalidShopItemIndex;
            }
        }

        private void ReadShopItems(
            sbyte[] packetData,
            int newShopItemCount,
            PacketReadCursor cursor)
        {
            PacketItemRecordReader.ReadShopItemRecords(
                packetData,
                cursor,
                newShopItemCount,
                Client.shopItems,
                Client.shopItemCount,
                Client.shopItemBuyPrice,
                Client.shopItemSellPrice);
        }

        private void AppendGeneralShopInventory(int newShopItemCount)
        {
            int shopIndex = MaximumShopItemCount - 1;

            for (int inventoryIndex = 0;
                inventoryIndex < Client.inventoryItemsCount;
                inventoryIndex += 1)
            {
                if (shopIndex < newShopItemCount)
                {
                    return;
                }

                int inventoryItem = Client.inventoryItems[inventoryIndex];

                if (ShouldSkipGeneralShopItem(inventoryItem))
                {
                    continue;
                }

                Client.shopItems[shopIndex] = inventoryItem & ItemIdMask;
                Client.shopItemCount[shopIndex] = 0;
                Client.shopItemSellPrice[shopIndex] = CalculateGeneralShopSellPrice(shopIndex);
                shopIndex -= 1;
            }
        }

        private bool ShouldSkipGeneralShopItem(int inventoryItem)
        {
            if (inventoryItem == InventoryExclusionItem)
            {
                return true;
            }

            for (int shopIndex = 0; shopIndex < MaximumShopItemCount; shopIndex += 1)
            {
                if (Client.shopItems[shopIndex] == inventoryItem)
                {
                    return true;
                }
            }

            return false;
        }

        private int CalculateGeneralShopSellPrice(int shopIndex)
        {
            int itemId = Client.shopItems[shopIndex];
            int basePrice = Client.entityManager.GetItem(itemId).BasePrice;
            int sellPrice = basePrice - (int)(basePrice / 2.5);
            sellPrice -= (int)(sellPrice * 0.10);
            return sellPrice;
        }

        private void ResetSelectedShopItemIfInvalid()
        {
            if (Client.selectedShopItemIndex < 0 ||
                Client.selectedShopItemIndex >= MaximumShopItemCount)
            {
                return;
            }

            if (Client.shopItems[Client.selectedShopItemIndex] == Client.selectedShopItemType)
            {
                return;
            }

            Client.selectedShopItemIndex = InvalidShopItemIndex;
            Client.selectedShopItemType = InvalidShopItemType;
        }
    }
}