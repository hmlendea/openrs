using System;

using OpenRS.Models;
using OpenRS.Net.Client.Game;
using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class InventoryRenderer(GameClient client, Action<int, int, int, int, Item> recordItemSprite)
    {

        public void DrawInventoryMenu(bool canRightClick)
        {
            int inventoryStartX = client.gameGraphics.gameWidth - 248;
            client.gameGraphics.DrawPicture(inventoryStartX, 3, client.baseInventoryPic + 1);

            for (int itemIndex = 0; itemIndex < client.maxInventoryItems; itemIndex += 1)
            {
                int cellX = inventoryStartX + itemIndex % 5 * 49;
                int cellY = 36 + itemIndex / 5 * 34;
                int cellColour = GameImage.RgbToInt(181, 181, 181);

                if (itemIndex < client.inventoryItemsCount && client.inventoryItemEquipped[itemIndex] == 1)
                {
                    cellColour = 0xff0000;
                }

                client.gameGraphics.DrawBoxAlpha(cellX, cellY, 49, 34, cellColour, 128);

                if (itemIndex < client.inventoryItemsCount)
                {
                    Item inventoryItem = client.entityManager.GetItem(client.inventoryItems[itemIndex]);
                    recordItemSprite(cellX, cellY, 48, 32, inventoryItem);

                    if (inventoryItem.IsStackable == 0)
                    {
                        client.gameGraphics.DrawString(client.inventoryItemCount[itemIndex].ToString(), cellX + 1, cellY + 10, 1, 0xffff00);
                    }
                }
            }

            for (int columnIndex = 1; columnIndex <= 4; columnIndex += 1)
            {
                client.gameGraphics.DrawLineY(inventoryStartX + columnIndex * 49, 36, client.maxInventoryItems / 5 * 34, 0);
            }

            for (int rowIndex = 1; rowIndex <= client.maxInventoryItems / 5 - 1; rowIndex += 1)
            {
                client.gameGraphics.DrawLineX(inventoryStartX, 36 + rowIndex * 34, 245, 0);
            }

            if (!canRightClick)
            {
                return;
            }

            int relativeMouseX = client.mouseX - (client.gameGraphics.gameWidth - 248);
            int relativeMouseY = client.mouseY - 36;

            if (relativeMouseX >= 0 &&
                relativeMouseY >= 0 &&
                relativeMouseX < 248 &&
                relativeMouseY < client.maxInventoryItems / 5 * 34)
            {
                int hoveredItemIndex = relativeMouseX / 49 + relativeMouseY / 34 * 5;

                if (hoveredItemIndex < client.inventoryItemsCount)
                {
                    int itemId = client.inventoryItems[hoveredItemIndex];
                    Item inventoryItem = client.entityManager.GetItem(itemId);

                    if (client.selectedSpell >= 0)
                    {
                        if (client.entityManager.GetSpell(client.selectedSpell).Type == 3)
                        {
                            client.menuText1[client.menuOptionsCount] = LocalisationManager.GetString("inventory.action_cast_prefix") + client.entityManager.GetSpell(client.selectedSpell).Name + " on";
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 600;
                            client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                            client.menuActionVar1[client.menuOptionsCount] = client.selectedSpell;
                            client.menuOptionsCount += 1;

                            return;
                        }
                    }
                    else
                    {
                        if (client.selectedItem >= 0)
                        {
                            client.menuText1[client.menuOptionsCount] = LocalisationManager.GetString("inventory.action_use_prefix") + client.selectedItemName + " with";
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 610;
                            client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                            client.menuActionVar1[client.menuOptionsCount] = client.selectedItem;
                            client.menuOptionsCount += 1;

                            return;
                        }

                        if (client.inventoryItemEquipped[hoveredItemIndex] == 1)
                        {
                            client.menuText1[client.menuOptionsCount] = LocalisationManager.GetString("inventory.action_remove");
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 620;
                            client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                            client.menuOptionsCount += 1;
                        }
                        else if (inventoryItem.IsEquipable != 0)
                        {
                            string wieldLabel = LocalisationManager.GetString("inventory.action_wear");

                            if ((inventoryItem.IsEquipable & 0x18) != 0)
                            {
                                wieldLabel = LocalisationManager.GetString("inventory.action_wield");
                            }

                            client.menuText1[client.menuOptionsCount] = wieldLabel;
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 630;
                            client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                            client.menuOptionsCount += 1;
                        }

                        if (inventoryItem.Command != "")
                        {
                            client.menuText1[client.menuOptionsCount] = inventoryItem.Command;
                            client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                            client.menuActionID[client.menuOptionsCount] = 640;
                            client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                            client.menuOptionsCount += 1;
                        }

                        client.menuText1[client.menuOptionsCount] = "Use";
                        client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                        client.menuActionID[client.menuOptionsCount] = 650;
                        client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                        client.menuOptionsCount += 1;
                        client.menuText1[client.menuOptionsCount] = LocalisationManager.GetString("inventory.action_drop");
                        client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                        client.menuActionID[client.menuOptionsCount] = 660;
                        client.menuActionType[client.menuOptionsCount] = hoveredItemIndex;
                        client.menuOptionsCount += 1;
                        client.menuText1[client.menuOptionsCount] = LocalisationManager.GetString("inventory.action_examine");
                        client.menuText2[client.menuOptionsCount] = "@lre@" + inventoryItem.Name;
                        client.menuActionID[client.menuOptionsCount] = 3600;
                        client.menuActionType[client.menuOptionsCount] = itemId;
                        client.menuOptionsCount += 1;
                    }
                }
            }
        }


        public void DrawItem(int x, int y, int width, int height, int itemID, int xOffset, int yOffset)
        {
            int picture = client.entityManager.GetItem(itemID).InventoryPicture + client.baseItemPicture;
            int mask = client.entityManager.GetItem(itemID).PictureMask;
            client.gameGraphics.DrawImage(x, y, width, height, picture, mask, 0, 0, false);
        }


        public void DrawShopBox()
        {
            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;
                int clickOffsetX = client.mouseX - 52;
                int clickOffsetY = client.mouseY - 44;

                if (clickOffsetX >= 0 && clickOffsetY >= 12 && clickOffsetX < 408 && clickOffsetY < 246)
                {
                    int shopItemIndex = 0;

                    for (int rowIndex = 0; rowIndex < 5; rowIndex += 1)
                    {
                        for (int columnIndex = 0; columnIndex < 8; columnIndex += 1)
                        {
                            int cellX = 7 + columnIndex * 49;
                            int cellY = 28 + rowIndex * 34;

                            if (clickOffsetX > cellX &&
                                clickOffsetX < cellX + 49 &&
                                clickOffsetY > cellY &&
                                clickOffsetY < cellY + 34 &&
                                client.shopItems[shopItemIndex] != -1)
                            {
                                client.selectedShopItemIndex = shopItemIndex;
                                client.selectedShopItemType = client.shopItems[shopItemIndex];
                            }

                            shopItemIndex += 1;
                        }
                    }

                    if (client.selectedShopItemIndex >= 0)
                    {
                        int clickedItemId = client.shopItems[client.selectedShopItemIndex];

                        if (clickedItemId != -1)
                        {
                            if (client.shopItemCount[client.selectedShopItemIndex] > 0 &&
                                clickOffsetX > 298 &&
                                clickOffsetY >= 204 &&
                                clickOffsetX < 408 &&
                                clickOffsetY <= 215)
                            {
                                client.streamClass.CreatePacket(128);
                                client.streamClass.AddShort(client.shopItems[client.selectedShopItemIndex]);
                                client.streamClass.AddInt(client.shopItemBuyPrice[client.selectedShopItemIndex]);
                                client.streamClass.FormatPacket();
                            }

                            if (client.GetInventoryItemTotalCount(clickedItemId) > 0 &&
                                clickOffsetX > 2 &&
                                clickOffsetY >= 229 &&
                                clickOffsetX < 112 &&
                                clickOffsetY <= 240)
                            {
                                client.streamClass.CreatePacket(255);
                                client.streamClass.AddShort(client.shopItems[client.selectedShopItemIndex]);
                                client.streamClass.AddInt(client.shopItemSellPrice[client.selectedShopItemIndex]);
                                client.streamClass.FormatPacket();
                            }
                        }
                    }
                }
                else
                {
                    client.streamClass.CreatePacket(253);
                    client.streamClass.FormatPacket();
                    client.showShopBox = false;

                    return;
                }
            }

            int boxOffsetX = 52;
            int boxOffsetY = 44;
            client.gameGraphics.DrawBox(boxOffsetX, boxOffsetY, 408, 12, 192);
            int backgroundColour = 0x989898;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 12, 408, 17, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 29, 8, 170, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 399, boxOffsetY + 29, 9, 170, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 199, 408, 47, backgroundColour, 160);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.shop_title"), boxOffsetX + 1, boxOffsetY + 10, 1, 0xffffff);
            int closeLabelColour = 0xffffff;

            if (client.mouseX > boxOffsetX + 320 &&
                client.mouseY >= boxOffsetY &&
                client.mouseX < boxOffsetX + 408 &&
                client.mouseY < boxOffsetY + 12)
            {
                closeLabelColour = 0xff0000;
            }

            client.gameGraphics.DrawLabel(LocalisationManager.GetString("inventory.shop_close"), boxOffsetX + 406, boxOffsetY + 10, 1, closeLabelColour);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.shop_stock_hint"), boxOffsetX + 2, boxOffsetY + 24, 1, 65280);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.shop_owned_hint"), boxOffsetX + 135, boxOffsetY + 24, 1, 65535);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.shop_money_prefix") + client.GetInventoryItemTotalCount(10) + "gp", boxOffsetX + 280, boxOffsetY + 24, 1, 0xffff00);
            int cellColour = 0xd0d0d0;
            int shopDisplayIndex = 0;

            for (int rowIndex = 0; rowIndex < 5; rowIndex += 1)
            {
                for (int columnIndex = 0; columnIndex < 8; columnIndex += 1)
                {
                    int itemCellX = boxOffsetX + 7 + columnIndex * 49;
                    int itemCellY = boxOffsetY + 28 + rowIndex * 34;

                    if (client.selectedShopItemIndex == shopDisplayIndex)
                    {
                        client.gameGraphics.DrawBoxAlpha(itemCellX, itemCellY, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        client.gameGraphics.DrawBoxAlpha(itemCellX, itemCellY, 49, 34, cellColour, 160);
                    }

                    client.gameGraphics.DrawBoxEdge(itemCellX, itemCellY, 50, 35, 0);

                    if (client.shopItems[shopDisplayIndex] != -1)
                    {
                        recordItemSprite(itemCellX, itemCellY, 48, 32, client.entityManager.GetItem(client.shopItems[shopDisplayIndex]));
                        client.gameGraphics.DrawString(client.shopItemCount[shopDisplayIndex].ToString(), itemCellX + 1, itemCellY + 10, 1, 65280);
                        client.gameGraphics.DrawLabel(client.GetInventoryItemTotalCount(client.shopItems[shopDisplayIndex]).ToString(), itemCellX + 47, itemCellY + 10, 1, 65535);
                    }

                    shopDisplayIndex += 1;
                }
            }

            client.gameGraphics.DrawLineX(boxOffsetX + 5, boxOffsetY + 222, 398, 0);

            if (client.selectedShopItemIndex == -1)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("inventory.shop_select_hint"), boxOffsetX + 204, boxOffsetY + 214, 3, 0xffff00);

                return;
            }

            int selectedItemId = client.shopItems[client.selectedShopItemIndex];

            if (selectedItemId != -1)
            {
                if (client.shopItemCount[client.selectedShopItemIndex] > 0)
                {
                    int buyPriceModifier = client.shopItemBuyPriceModifier + client.shopItemBasePriceModifier[client.selectedShopItemIndex];

                    if (buyPriceModifier < 10)
                    {
                        buyPriceModifier = 10;
                    }

                    int buyPrice = buyPriceModifier * client.entityManager.GetItem(selectedItemId).BasePrice / 100;
                    client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.shop_buy_prefix") + client.entityManager.GetItem(selectedItemId).Name + " for " + buyPrice + "gp", boxOffsetX + 2, boxOffsetY + 214, 1, 0xffff00);
                    int buyButtonColour = 0xffffff;

                    if (client.mouseX > boxOffsetX + 298 &&
                        client.mouseY >= boxOffsetY + 204 &&
                        client.mouseX < boxOffsetX + 408 &&
                        client.mouseY <= boxOffsetY + 215)
                    {
                        buyButtonColour = 0xff0000;
                    }

                    client.gameGraphics.DrawLabel(LocalisationManager.GetString("inventory.shop_buy_action"), boxOffsetX + 405, boxOffsetY + 214, 3, buyButtonColour);
                }
                else
                {
                    client.gameGraphics.DrawText(LocalisationManager.GetString("inventory.shop_buy_unavailable"), boxOffsetX + 204, boxOffsetY + 214, 3, 0xffff00);
                }

                if (client.GetInventoryItemTotalCount(selectedItemId) > 0)
                {
                    int sellPriceModifier = client.shopItemSellPriceModifier + client.shopItemBasePriceModifier[client.selectedShopItemIndex];

                    if (sellPriceModifier < 10)
                    {
                        sellPriceModifier = 10;
                    }

                    int sellPrice = sellPriceModifier * client.entityManager.GetItem(selectedItemId).BasePrice / 100;
                    client.gameGraphics.DrawLabel(LocalisationManager.GetString("inventory.shop_sell_prefix") + client.entityManager.GetItem(selectedItemId).Name + " for " + sellPrice + "gp", boxOffsetX + 405, boxOffsetY + 239, 1, 0xffff00);
                    int sellButtonColour = 0xffffff;

                    if (client.mouseX > boxOffsetX + 2 &&
                        client.mouseY >= boxOffsetY + 229 &&
                        client.mouseX < boxOffsetX + 112 &&
                        client.mouseY <= boxOffsetY + 240)
                    {
                        sellButtonColour = 0xff0000;
                    }

                    client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.shop_sell_action"), boxOffsetX + 2, boxOffsetY + 239, 3, sellButtonColour);

                    return;
                }

                client.gameGraphics.DrawText(LocalisationManager.GetString("inventory.shop_sell_none"), boxOffsetX + 204, boxOffsetY + 239, 3, 0xffff00);
            }
        }


        public void DrawBankBox()
        {
            int bankWidth = 408;
            int bankHeight = 334;

            if (client.bankPage > 0 && client.bankItemsCount <= 48)
            {
                client.bankPage = 0;
            }

            if (client.bankPage > 1 && client.bankItemsCount <= 96)
            {
                client.bankPage = 1;
            }

            if (client.bankPage > 2 && client.bankItemsCount <= 144)
            {
                client.bankPage = 2;
            }

            if (client.selectedBankItem >= client.bankItemsCount || client.selectedBankItem < 0)
            {
                client.selectedBankItem = -1;
            }

            if (client.selectedBankItem != -1 && client.bankItems[client.selectedBankItem] != client.selectedBankItemType)
            {
                client.selectedBankItem = -1;
                client.selectedBankItemType = -2;
            }

            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;
                int clickOffsetX = client.mouseX - (256 - bankWidth / 2);
                int clickOffsetY = client.mouseY - (170 - bankHeight / 2);

                if (clickOffsetX >= 0 && clickOffsetY >= 12 && clickOffsetX < bankWidth && clickOffsetY < 280)
                {
                    int bankPageStart = client.bankPage * 48;

                    for (int gridRowIndex = 0; gridRowIndex < 6; gridRowIndex += 1)
                    {
                        for (int gridColumnIndex = 0; gridColumnIndex < 8; gridColumnIndex += 1)
                        {
                            int cellX = 7 + gridColumnIndex * 49;
                            int cellY = 28 + gridRowIndex * 34;

                            if (clickOffsetX > cellX && clickOffsetX < cellX + 49 && clickOffsetY > cellY && clickOffsetY < cellY + 34 && bankPageStart < client.bankItemsCount && client.bankItems[bankPageStart] != -1)
                            {
                                client.selectedBankItemType = client.bankItems[bankPageStart];
                                client.selectedBankItem = bankPageStart;
                            }

                            bankPageStart += 1;
                        }
                    }

                    int clickBoxLeft = 256 - bankWidth / 2;
                    int clickBoxTop = 170 - bankHeight / 2;
                    int id;

                    if (client.selectedBankItem < 0)
                    {
                        id = -1;
                    }
                    else
                    {
                        id = client.bankItems[client.selectedBankItem];
                    }

                    if (id != -1)
                    {
                        int count = client.bankItemCount[client.selectedBankItem];

                        if (client.entityManager.GetItem(id).IsStackable == 1 && count > 1)
                        {
                            count = 1;
                        }

                        if (count >= 1 && client.mouseX >= clickBoxLeft + 220 && client.mouseY >= clickBoxTop + 238 && client.mouseX < clickBoxLeft + 250 && client.mouseY <= clickBoxTop + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(1);
                            client.streamClass.FormatPacket();
                        }

                        if (count >= 5 && client.mouseX >= clickBoxLeft + 250 && client.mouseY >= clickBoxTop + 238 && client.mouseX < clickBoxLeft + 280 && client.mouseY <= clickBoxTop + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(5);
                            client.streamClass.FormatPacket();
                        }

                        if (count >= 25 && client.mouseX >= clickBoxLeft + 280 && client.mouseY >= clickBoxTop + 238 && client.mouseX < clickBoxLeft + 305 && client.mouseY <= clickBoxTop + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(25);
                            client.streamClass.FormatPacket();
                        }

                        if (count >= 100 && client.mouseX >= clickBoxLeft + 305 && client.mouseY >= clickBoxTop + 238 && client.mouseX < clickBoxLeft + 335 && client.mouseY <= clickBoxTop + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(100);
                            client.streamClass.FormatPacket();
                        }

                        if (count >= 500 && client.mouseX >= clickBoxLeft + 335 && client.mouseY >= clickBoxTop + 238 && client.mouseX < clickBoxLeft + 368 && client.mouseY <= clickBoxTop + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(500);
                            client.streamClass.FormatPacket();
                        }

                        if (count >= 2500 && client.mouseX >= clickBoxLeft + 370 && client.mouseY >= clickBoxTop + 238 && client.mouseX < clickBoxLeft + 400 && client.mouseY <= clickBoxTop + 249)
                        {
                            client.streamClass.CreatePacket(183);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(2500);
                            client.streamClass.FormatPacket();
                        }

                        if (client.GetInventoryItemTotalCount(id) >= 1 && client.mouseX >= clickBoxLeft + 220 && client.mouseY >= clickBoxTop + 263 && client.mouseX < clickBoxLeft + 250 && client.mouseY <= clickBoxTop + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(1);
                            client.streamClass.FormatPacket();
                        }

                        if (client.GetInventoryItemTotalCount(id) >= 5 && client.mouseX >= clickBoxLeft + 250 && client.mouseY >= clickBoxTop + 263 && client.mouseX < clickBoxLeft + 280 && client.mouseY <= clickBoxTop + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(5);
                            client.streamClass.FormatPacket();
                        }

                        if (client.GetInventoryItemTotalCount(id) >= 25 && client.mouseX >= clickBoxLeft + 280 && client.mouseY >= clickBoxTop + 263 && client.mouseX < clickBoxLeft + 305 && client.mouseY <= clickBoxTop + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(25);
                            client.streamClass.FormatPacket();
                        }

                        if (client.GetInventoryItemTotalCount(id) >= 100 && client.mouseX >= clickBoxLeft + 305 && client.mouseY >= clickBoxTop + 263 && client.mouseX < clickBoxLeft + 335 && client.mouseY <= clickBoxTop + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(100);
                            client.streamClass.FormatPacket();
                        }

                        if (client.GetInventoryItemTotalCount(id) >= 500 && client.mouseX >= clickBoxLeft + 335 && client.mouseY >= clickBoxTop + 263 && client.mouseX < clickBoxLeft + 368 && client.mouseY <= clickBoxTop + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(500);
                            client.streamClass.FormatPacket();
                        }

                        if (client.GetInventoryItemTotalCount(id) >= 2500 && client.mouseX >= clickBoxLeft + 370 && client.mouseY >= clickBoxTop + 263 && client.mouseX < clickBoxLeft + 400 && client.mouseY <= clickBoxTop + 274)
                        {
                            client.streamClass.CreatePacket(198);
                            client.streamClass.AddShort(id);
                            client.streamClass.AddInt(2500);
                            client.streamClass.FormatPacket();
                        }
                    }
                }
                else if (client.bankItemsCount > 48 && clickOffsetX >= 50 && clickOffsetX <= 115 && clickOffsetY <= 12)
                {
                    client.bankPage = 0;
                }
                else if (client.bankItemsCount > 48 && clickOffsetX >= 115 && clickOffsetX <= 180 && clickOffsetY <= 12)
                {
                    client.bankPage = 1;
                }
                else if (client.bankItemsCount > 96 && clickOffsetX >= 180 && clickOffsetX <= 245 && clickOffsetY <= 12)
                {
                    client.bankPage = 2;
                }
                else if (client.bankItemsCount > 144 && clickOffsetX >= 245 && clickOffsetX <= 310 && clickOffsetY <= 12)
                {
                    client.bankPage = 3;
                }
                else
                {
                    client.streamClass.CreatePacket(48);
                    client.streamClass.FormatPacket();
                    client.showBankBox = false;
                    return;
                }
            }

            int boxLeft = 256 - bankWidth / 2;
            int boxTop = 170 - bankHeight / 2;
            client.gameGraphics.DrawBox(boxLeft, boxTop, bankWidth, 12, 192);
            int panelColour = 0x989898;
            client.gameGraphics.DrawBoxAlpha(boxLeft, boxTop + 12, bankWidth, 17, panelColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxLeft, boxTop + 29, 8, 204, panelColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxLeft + 399, boxTop + 29, 9, 204, panelColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxLeft, boxTop + 233, bankWidth, 47, panelColour, 160);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.bank_title"), boxLeft + 1, boxTop + 10, 1, 0xffffff);
            int pageButtonX = 50;

            if (client.bankItemsCount > 48)
            {
                int page1LabelColour = 0xffffff;

                if (client.bankPage == 0)
                {
                    page1LabelColour = 0xff0000;
                }
                else if (client.mouseX > boxLeft + pageButtonX && client.mouseY >= boxTop && client.mouseX < boxLeft + pageButtonX + 65 && client.mouseY < boxTop + 12)
                {
                    page1LabelColour = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 1>", boxLeft + pageButtonX, boxTop + 10, 1, page1LabelColour);
                pageButtonX += 65;
                int page2LabelColour = 0xffffff;

                if (client.bankPage == 1)
                {
                    page2LabelColour = 0xff0000;
                }
                else if (client.mouseX > boxLeft + pageButtonX && client.mouseY >= boxTop && client.mouseX < boxLeft + pageButtonX + 65 && client.mouseY < boxTop + 12)
                {
                    page2LabelColour = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 2>", boxLeft + pageButtonX, boxTop + 10, 1, page2LabelColour);
                pageButtonX += 65;
            }

            if (client.bankItemsCount > 96)
            {
                int page3LabelColour = 0xffffff;

                if (client.bankPage == 2)
                {
                    page3LabelColour = 0xff0000;
                }
                else if (client.mouseX > boxLeft + pageButtonX && client.mouseY >= boxTop && client.mouseX < boxLeft + pageButtonX + 65 && client.mouseY < boxTop + 12)
                {
                    page3LabelColour = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 3>", boxLeft + pageButtonX, boxTop + 10, 1, page3LabelColour);
                pageButtonX += 65;
            }

            if (client.bankItemsCount > 144)
            {
                int page4LabelColour = 0xffffff;

                if (client.bankPage == 3)
                {
                    page4LabelColour = 0xff0000;
                }
                else if (client.mouseX > boxLeft + pageButtonX && client.mouseY >= boxTop && client.mouseX < boxLeft + pageButtonX + 65 && client.mouseY < boxTop + 12)
                {
                    page4LabelColour = 0xffff00;
                }

                client.gameGraphics.DrawString("<page 4>", boxLeft + pageButtonX, boxTop + 10, 1, page4LabelColour);
            }

            int closeLabelColour = 0xffffff;

            if (client.mouseX > boxLeft + 320 && client.mouseY >= boxTop && client.mouseX < boxLeft + bankWidth && client.mouseY < boxTop + 12)
            {
                closeLabelColour = 0xff0000;
            }

            client.gameGraphics.DrawLabel(LocalisationManager.GetString("inventory.shop_close"), boxLeft + 406, boxTop + 10, 1, closeLabelColour);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.bank_stock_hint"), boxLeft + 7, boxTop + 24, 1, 65280);
            client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.bank_held_hint"), boxLeft + 289, boxTop + 24, 1, 65535);
            int cellColour = 0xd0d0d0;
            int pageStartIndex = client.bankPage * 48;

            for (int gridRowIndex = 0; gridRowIndex < 6; gridRowIndex += 1)
            {
                for (int gridColumnIndex = 0; gridColumnIndex < 8; gridColumnIndex += 1)
                {
                    int cellX = boxLeft + 7 + gridColumnIndex * 49;
                    int cellY = boxTop + 28 + gridRowIndex * 34;

                    if (client.selectedBankItem == pageStartIndex)
                    {
                        client.gameGraphics.DrawBoxAlpha(cellX, cellY, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        client.gameGraphics.DrawBoxAlpha(cellX, cellY, 49, 34, cellColour, 160);
                    }

                    client.gameGraphics.DrawBoxEdge(cellX, cellY, 50, 35, 0);

                    if (pageStartIndex < client.bankItemsCount && client.bankItems[pageStartIndex] != -1)
                    {
                        recordItemSprite(cellX, cellY, 48, 32, client.entityManager.GetItem(client.bankItems[pageStartIndex]));
                        client.gameGraphics.DrawString(client.bankItemCount[pageStartIndex].ToString(), cellX + 1, cellY + 10, 1, 65280);
                        client.gameGraphics.DrawLabel(client.GetInventoryItemTotalCount(client.bankItems[pageStartIndex]).ToString(), cellX + 47, cellY + 29, 1, 65535);
                    }

                    pageStartIndex += 1;
                }
            }

            client.gameGraphics.DrawLineX(boxLeft + 5, boxTop + 256, 398, 0);

            if (client.selectedBankItem == -1)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("inventory.bank_select_hint"), boxLeft + 204, boxTop + 248, 3, 0xffff00);
                return;
            }

            int selectedItemId;

            if (client.selectedBankItem < 0)
            {
                selectedItemId = -1;
            }
            else
            {
                selectedItemId = client.bankItems[client.selectedBankItem];
            }

            if (selectedItemId != -1)
            {
                int itemCount = client.bankItemCount[client.selectedBankItem];

                if (client.entityManager.GetItem(selectedItemId).IsStackable == 1 && itemCount > 1)
                {
                    itemCount = 1;
                }

                if (itemCount > 0)
                {
                    client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.bank_withdraw_prefix") + client.entityManager.GetItem(selectedItemId).Name, boxLeft + 2, boxTop + 248, 1, 0xffffff);
                    int withdrawOneLabelColour = 0xffffff;

                    if (client.mouseX >= boxLeft + 220 && client.mouseY >= boxTop + 238 && client.mouseX < boxLeft + 250 && client.mouseY <= boxTop + 249)
                    {
                        withdrawOneLabelColour = 0xff0000;
                    }

                    client.gameGraphics.DrawString("One", boxLeft + 222, boxTop + 248, 1, withdrawOneLabelColour);

                    if (itemCount >= 5)
                    {
                        int withdrawFiveLabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 250 && client.mouseY >= boxTop + 238 && client.mouseX < boxLeft + 280 && client.mouseY <= boxTop + 249)
                        {
                            withdrawFiveLabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("Five", boxLeft + 252, boxTop + 248, 1, withdrawFiveLabelColour);
                    }

                    if (itemCount >= 25)
                    {
                        int withdraw25LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 280 && client.mouseY >= boxTop + 238 && client.mouseX < boxLeft + 305 && client.mouseY <= boxTop + 249)
                        {
                            withdraw25LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("25", boxLeft + 282, boxTop + 248, 1, withdraw25LabelColour);
                    }

                    if (itemCount >= 100)
                    {
                        int withdraw100LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 305 && client.mouseY >= boxTop + 238 && client.mouseX < boxLeft + 335 && client.mouseY <= boxTop + 249)
                        {
                            withdraw100LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("100", boxLeft + 307, boxTop + 248, 1, withdraw100LabelColour);
                    }

                    if (itemCount >= 500)
                    {
                        int withdraw500LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 335 && client.mouseY >= boxTop + 238 && client.mouseX < boxLeft + 368 && client.mouseY <= boxTop + 249)
                        {
                            withdraw500LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("500", boxLeft + 337, boxTop + 248, 1, withdraw500LabelColour);
                    }

                    if (itemCount >= 2500)
                    {
                        int withdraw2500LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 370 && client.mouseY >= boxTop + 238 && client.mouseX < boxLeft + 400 && client.mouseY <= boxTop + 249)
                        {
                            withdraw2500LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("2500", boxLeft + 370, boxTop + 248, 1, withdraw2500LabelColour);
                    }
                }

                if (client.GetInventoryItemTotalCount(selectedItemId) > 0)
                {
                    client.gameGraphics.DrawString(LocalisationManager.GetString("inventory.bank_deposit_prefix") + client.entityManager.GetItem(selectedItemId).Name, boxLeft + 2, boxTop + 273, 1, 0xffffff);
                    int depositOneLabelColour = 0xffffff;

                    if (client.mouseX >= boxLeft + 220 && client.mouseY >= boxTop + 263 && client.mouseX < boxLeft + 250 && client.mouseY <= boxTop + 274)
                    {
                        depositOneLabelColour = 0xff0000;
                    }

                    client.gameGraphics.DrawString("One", boxLeft + 222, boxTop + 273, 1, depositOneLabelColour);

                    if (client.GetInventoryItemTotalCount(selectedItemId) >= 5)
                    {
                        int depositFiveLabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 250 && client.mouseY >= boxTop + 263 && client.mouseX < boxLeft + 280 && client.mouseY <= boxTop + 274)
                        {
                            depositFiveLabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("Five", boxLeft + 252, boxTop + 273, 1, depositFiveLabelColour);
                    }

                    if (client.GetInventoryItemTotalCount(selectedItemId) >= 25)
                    {
                        int deposit25LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 280 && client.mouseY >= boxTop + 263 && client.mouseX < boxLeft + 305 && client.mouseY <= boxTop + 274)
                        {
                            deposit25LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("25", boxLeft + 282, boxTop + 273, 1, deposit25LabelColour);
                    }

                    if (client.GetInventoryItemTotalCount(selectedItemId) >= 100)
                    {
                        int deposit100LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 305 && client.mouseY >= boxTop + 263 && client.mouseX < boxLeft + 335 && client.mouseY <= boxTop + 274)
                        {
                            deposit100LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("100", boxLeft + 307, boxTop + 273, 1, deposit100LabelColour);
                    }

                    if (client.GetInventoryItemTotalCount(selectedItemId) >= 500)
                    {
                        int deposit500LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 335 && client.mouseY >= boxTop + 263 && client.mouseX < boxLeft + 368 && client.mouseY <= boxTop + 274)
                        {
                            deposit500LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("500", boxLeft + 337, boxTop + 273, 1, deposit500LabelColour);
                    }

                    if (client.GetInventoryItemTotalCount(selectedItemId) >= 2500)
                    {
                        int deposit2500LabelColour = 0xffffff;

                        if (client.mouseX >= boxLeft + 370 && client.mouseY >= boxTop + 263 && client.mouseX < boxLeft + 400 && client.mouseY <= boxTop + 274)
                        {
                            deposit2500LabelColour = 0xff0000;
                        }

                        client.gameGraphics.DrawString("2500", boxLeft + 370, boxTop + 273, 1, deposit2500LabelColour);
                    }
                }
            }
        }

   
    }
}
