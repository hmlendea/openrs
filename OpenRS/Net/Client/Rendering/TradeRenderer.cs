using System;

using OpenRS.Models;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Utilities;
using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class TradeRenderer(GameClient client, Action<int, int, int, int, Item> recordItemSprite)
    {

        public void DrawDuelConfirmBox()
        {
            int boxOffsetX = 22;
            int boxOffsetY = 36;
            client.gameGraphics.DrawBox(boxOffsetX, boxOffsetY, 468, 16, 192);
            int backgroundColour = 0x989898;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 16, 468, 246, backgroundColour, 160);
            client.gameGraphics.DrawText(
                LocalisationManager.GetString("trade.duel_confirm_prompt") + PlayerNameEncoder.HashToName(client.duelOpponentHash),
                boxOffsetX + 234,
                boxOffsetY + 12,
                1,
                0xffffff);
            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_your_stake"), boxOffsetX + 117, boxOffsetY + 30, 1, 0xffff00);

            for (int stakeIndex = 0; stakeIndex < client.duelOurStakeCount; stakeIndex += 1)
            {
                Item stakeItem = client.entityManager.GetItem(client.duelOurStakeItem[stakeIndex]);
                string stakeItemName = stakeItem.Name;

                if (!stakeItem.IsStackable)
                {
                    stakeItemName = stakeItemName + " x " + GameClientUtilities.FormatItemCount(client.duelOurStakeItemCount[stakeIndex]);
                }

                client.gameGraphics.DrawText(stakeItemName, boxOffsetX + 117, boxOffsetY + 42 + stakeIndex * 12, 1, 0xffffff);
            }

            if (client.duelOurStakeCount == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_nothing"), boxOffsetX + 117, boxOffsetY + 42, 1, 0xffffff);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_opponent_stake"), boxOffsetX + 351, boxOffsetY + 30, 1, 0xffff00);

            for (int opponentStakeIndex = 0; opponentStakeIndex < client.duelOpponentStakeCount; opponentStakeIndex += 1)
            {
                Item opponentStakeItem = client.entityManager.GetItem(client.duelOpponentStakeItem[opponentStakeIndex]);
                string opponentStakeItemName = opponentStakeItem.Name;

                if (!opponentStakeItem.IsStackable)
                {
                    opponentStakeItemName = opponentStakeItemName + " x " + GameClientUtilities.FormatItemCount(client.duelOutStakeItemCount[opponentStakeIndex]);
                }

                client.gameGraphics.DrawText(opponentStakeItemName, boxOffsetX + 351, boxOffsetY + 42 + opponentStakeIndex * 12, 1, 0xffffff);
            }

            if (client.duelOpponentStakeCount == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_nothing"), boxOffsetX + 351, boxOffsetY + 42, 1, 0xffffff);
            }

            if (client.duelRetreat == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_can_retreat"), boxOffsetX + 234, boxOffsetY + 180, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_no_retreat"), boxOffsetX + 234, boxOffsetY + 180, 1, 0xff0000);
            }

            if (client.duelMagic == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_magic_allowed"), boxOffsetX + 234, boxOffsetY + 192, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_magic_forbidden"), boxOffsetX + 234, boxOffsetY + 192, 1, 0xff0000);
            }

            if (client.duelPrayer == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_prayer_allowed"), boxOffsetX + 234, boxOffsetY + 204, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_prayer_forbidden"), boxOffsetX + 234, boxOffsetY + 204, 1, 0xff0000);
            }

            if (client.duelWeapons == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_weapons_allowed"), boxOffsetX + 234, boxOffsetY + 216, 1, 65280);
            }
            else
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_weapons_forbidden"), boxOffsetX + 234, boxOffsetY + 216, 1, 0xff0000);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_accept_prompt"), boxOffsetX + 234, boxOffsetY + 230, 1, 0xffffff);

            if (!client.duelConfirmOurAccepted)
            {
                client.gameGraphics.DrawPicture(boxOffsetX + 118 - 35, boxOffsetY + 238, client.baseInventoryPic + 25);
                client.gameGraphics.DrawPicture(boxOffsetX + 352 - 35, boxOffsetY + 238, client.baseInventoryPic + 26);
            }
            else
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_waiting"), boxOffsetX + 234, boxOffsetY + 250, 1, 0xffff00);
            }

            if (client.mouseButtonClick == 1)
            {
                if (client.mouseX < boxOffsetX ||
                    client.mouseY < boxOffsetY ||
                    client.mouseX > boxOffsetX + 468 ||
                    client.mouseY > boxOffsetY + 262)
                {
                    client.showDuelConfirmBox = false;
                    client.streamClass.CreatePacket(35);
                    client.streamClass.FormatPacket();
                }

                if (client.mouseX >= boxOffsetX + 118 - 35 &&
                    client.mouseX <= boxOffsetX + 118 + 70 &&
                    client.mouseY >= boxOffsetY + 238 &&
                    client.mouseY <= boxOffsetY + 238 + 21)
                {
                    client.duelConfirmOurAccepted = true;
                    client.streamClass.CreatePacket(87);
                    client.streamClass.FormatPacket();
                }

                if (client.mouseX >= boxOffsetX + 352 - 35 &&
                    client.mouseX <= boxOffsetX + 353 + 70 &&
                    client.mouseY >= boxOffsetY + 238 &&
                    client.mouseY <= boxOffsetY + 238 + 21)
                {
                    client.showDuelConfirmBox = false;
                    client.streamClass.CreatePacket(35);
                    client.streamClass.FormatPacket();
                }

                client.mouseButtonClick = 0;
            }
        }


        public void DrawTradeBox()
        {
            if (client.mouseButtonClick != 0)
            {
                int mx = client.mouseX - 22;
                int my = client.mouseY - 36;
                if (mx >= 0 && my >= 30 && mx < 462 && my < 262)
                {
                    if (mx > 216 && my > 30 && mx < 462 && my < 235)
                    {
                        int curItem = (mx - 217) / 49 + (my - 31) / 34 * 5;
                        if (curItem >= 0 && curItem < client.inventoryItemsCount)
                        {
                            int item = client.inventoryItems[curItem];
                            client.mouseClickedHeldInTradeDuelBox = 1;
                            bool ourTradeItemsChanged = false;
                            int someInt = 0;
                            for (int tradeItem = 0; tradeItem < client.tradeItemsOurCount; tradeItem += 1)
                            {
                                if (client.tradeItemsOur[tradeItem] == item)
                                {
                                    if (!client.entityManager.GetItem(item).IsStackable)
                                    {
                                        for (int i = 0; i < client.mouseClickedHeldInTradeDuelBox; i += 1)
                                        {
                                            if (client.tradeItemOurCount[tradeItem] < client.inventoryItemCount[curItem])
                                            {
                                                client.tradeItemOurCount[tradeItem] += 1;
                                            }

                                            ourTradeItemsChanged = true;
                                        }
                                    }
                                    else
                                    {
                                        someInt += 1;
                                    }
                                }
                            }

                            if (client.GetInventoryItemTotalCount(item) <= someInt)
                            {
                                ourTradeItemsChanged = true;
                            }

                            if (client.entityManager.GetItem(item).IsSpecial)
                            {
                                client.DisplayMessage(LocalisationManager.GetString("trade.trade_not_tradeable"), 3);
                                ourTradeItemsChanged = true;
                            }
                            if (!ourTradeItemsChanged && client.tradeItemsOurCount < 12)
                            {
                                client.tradeItemsOur[client.tradeItemsOurCount] = item;
                                client.tradeItemOurCount[client.tradeItemsOurCount] = 1;
                                client.tradeItemsOurCount += 1;
                                ourTradeItemsChanged = true;
                            }
                            if (ourTradeItemsChanged)
                            {
                                client.streamClass.CreatePacket(70);
                                client.streamClass.AddByte(client.tradeItemsOurCount);
                                for (int i = 0; i < client.tradeItemsOurCount; i += 1)
                                {
                                    client.streamClass.AddShort(client.tradeItemsOur[i]);
                                    client.streamClass.AddInt(client.tradeItemOurCount[i]);
                                }
                                client.streamClass.FormatPacket();
                                client.tradeOtherAccepted = false;
                                client.tradeWeAccepted = false;
                            }
                        }
                    }
                    else if (mx > 8 && my > 30 && mx < 205 && my < 133)
                    {
                        int curItem = (mx - 9) / 49 + (my - 31) / 34 * 4;
                        if (curItem >= 0 && curItem < client.tradeItemsOurCount)
                        {
                            int item = client.tradeItemsOur[curItem];
                            for (int i = 0; i < client.mouseClickedHeldInTradeDuelBox; i += 1)
                            {
                                if (!client.entityManager.GetItem(item).IsStackable && client.tradeItemOurCount[curItem] > 1)
                                {
                                    client.tradeItemOurCount[curItem] -= 1;
                                    continue;
                                }
                                client.tradeItemsOurCount -= 1;
                                client.mouseButtonHeldTime = 0;
                                for (int j = curItem; j < client.tradeItemsOurCount; j += 1)
                                {
                                    client.tradeItemsOur[j] = client.tradeItemsOur[j + 1];
                                    client.tradeItemOurCount[j] = client.tradeItemOurCount[j + 1];
                                }
                                break;
                            }
                            client.streamClass.CreatePacket(70);
                            client.streamClass.AddByte(client.tradeItemsOurCount);
                            for (int i = 0; i < client.tradeItemsOurCount; i += 1)
                            {
                                client.streamClass.AddShort(client.tradeItemsOur[i]);
                                client.streamClass.AddInt(client.tradeItemOurCount[i]);
                            }
                            client.streamClass.FormatPacket();
                            client.tradeOtherAccepted = false;
                            client.tradeWeAccepted = false;
                        }
                    }
                    if (mx >= 217 && my >= 238 && mx <= 286 && my <= 259)
                    {
                        client.tradeWeAccepted = true;
                        client.streamClass.CreatePacket(211);
                        client.streamClass.FormatPacket();
                    }
                    if (mx >= 394 && my >= 238 && mx < 463 && my < 259)
                    {
                        client.showTradeBox = false;
                        client.streamClass.CreatePacket(216);
                        client.streamClass.FormatPacket();
                    }
                }
                client.mouseButtonClick = 0;
                client.mouseClickedHeldInTradeDuelBox = 0;
            }

            if (!client.showTradeBox)
            {
                return;
            }

            int boxOffsetX = 22;
            int boxOffsetY = 36;
            client.gameGraphics.DrawBox(boxOffsetX, boxOffsetY, 468, 12, 192);
            int backgroundColour = 0x989898;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 12, 468, 18, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 30, 8, 248, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 205, boxOffsetY + 30, 11, 248, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 462, boxOffsetY + 30, 6, 248, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 133, 197, 22, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 258, 197, 20, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 216, boxOffsetY + 235, 246, 43, backgroundColour, 160);
            int lightColour = 0xd0d0d0;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 30, 197, 103, lightColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 155, 197, 103, lightColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 216, boxOffsetY + 30, 246, 205, lightColour, 160);

            for (int ourOfferRowIndex = 0; ourOfferRowIndex < 4; ourOfferRowIndex += 1)
            {
                client.gameGraphics.DrawLineX(boxOffsetX + 8, boxOffsetY + 30 + ourOfferRowIndex * 34, 197, 0);
            }

            for (int theirOfferRowIndex = 0; theirOfferRowIndex < 4; theirOfferRowIndex += 1)
            {
                client.gameGraphics.DrawLineX(boxOffsetX + 8, boxOffsetY + 155 + theirOfferRowIndex * 34, 197, 0);
            }

            for (int inventoryRowIndex = 0; inventoryRowIndex < 7; inventoryRowIndex += 1)
            {
                client.gameGraphics.DrawLineX(boxOffsetX + 216, boxOffsetY + 30 + inventoryRowIndex * 34, 246, 0);
            }

            for (int columnIndex = 0; columnIndex < 6; columnIndex += 1)
            {
                if (columnIndex < 5)
                {
                    client.gameGraphics.DrawLineY(boxOffsetX + 8 + columnIndex * 49, boxOffsetY + 30, 103, 0);
                }

                if (columnIndex < 5)
                {
                    client.gameGraphics.DrawLineY(boxOffsetX + 8 + columnIndex * 49, boxOffsetY + 155, 103, 0);
                }

                client.gameGraphics.DrawLineY(boxOffsetX + 216 + columnIndex * 49, boxOffsetY + 30, 205, 0);
            }

            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.trade_with_prefix") + client.tradeOtherName, boxOffsetX + 1, boxOffsetY + 10, 1, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.trade_your_offer"), boxOffsetX + 9, boxOffsetY + 27, 4, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.trade_opponent_offer"), boxOffsetX + 9, boxOffsetY + 152, 4, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.trade_your_inventory"), boxOffsetX + 216, boxOffsetY + 27, 4, 0xffffff);

            if (!client.tradeWeAccepted)
            {
                client.gameGraphics.DrawPicture(boxOffsetX + 217, boxOffsetY + 238, client.baseInventoryPic + 25);
            }

            client.gameGraphics.DrawPicture(boxOffsetX + 394, boxOffsetY + 238, client.baseInventoryPic + 26);

            if (client.tradeOtherAccepted)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_other_accepted"), boxOffsetX + 341, boxOffsetY + 246, 1, 0xffffff);
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_has_accepted"), boxOffsetX + 341, boxOffsetY + 256, 1, 0xffffff);
            }

            if (client.tradeWeAccepted)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_waiting_for"), boxOffsetX + 217 + 35, boxOffsetY + 246, 1, 0xffffff);
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_other_player"), boxOffsetX + 217 + 35, boxOffsetY + 256, 1, 0xffffff);
            }

            for (int inventoryIndex = 0; inventoryIndex < client.inventoryItemsCount; inventoryIndex += 1)
            {
                int inventoryCellX = 217 + boxOffsetX + inventoryIndex % 5 * 49;
                int inventoryCellY = 31 + boxOffsetY + inventoryIndex / 5 * 34;
                client.gameGraphics.DrawImage(inventoryCellX, inventoryCellY, 48, 32, client.baseItemPicture + client.entityManager.GetItem(client.inventoryItems[inventoryIndex]).InventoryPicture, client.entityManager.GetItem(client.inventoryItems[inventoryIndex]).PictureMask, 0, 0, false);

                if (!client.entityManager.GetItem(client.inventoryItems[inventoryIndex]).IsStackable)
                {
                    client.gameGraphics.DrawString(client.inventoryItemCount[inventoryIndex].ToString(), inventoryCellX + 1, inventoryCellY + 10, 1, 0xffff00);
                }
            }

            for (int ourStakeIndex = 0; ourStakeIndex < client.tradeItemsOurCount; ourStakeIndex += 1)
            {
                int ourStakeCellX = 9 + boxOffsetX + ourStakeIndex % 4 * 49;
                int ourStakeCellY = 31 + boxOffsetY + ourStakeIndex / 4 * 34;
                recordItemSprite(ourStakeCellX, ourStakeCellY, 48, 32, client.entityManager.GetItem(client.tradeItemsOur[ourStakeIndex]));

                if (!client.entityManager.GetItem(client.tradeItemsOur[ourStakeIndex]).IsStackable)
                {
                    client.gameGraphics.DrawString(client.tradeItemOurCount[ourStakeIndex].ToString(), ourStakeCellX + 1, ourStakeCellY + 10, 1, 0xffff00);
                }

                if (client.mouseX > ourStakeCellX &&
                    client.mouseX < ourStakeCellX + 48 &&
                    client.mouseY > ourStakeCellY &&
                    client.mouseY < ourStakeCellY + 32)
                {
                    Item tradeOurItem = client.entityManager.GetItem(client.tradeItemsOur[ourStakeIndex]);
                    client.gameGraphics.DrawString(tradeOurItem.Name + ": @whi@" + tradeOurItem.Description, boxOffsetX + 8, boxOffsetY + 273, 1, 0xffff00);
                }
            }

            for (int theirStakeIndex = 0; theirStakeIndex < client.tradeItemsOtherCount; theirStakeIndex += 1)
            {
                int theirStakeCellX = 9 + boxOffsetX + theirStakeIndex % 4 * 49;
                int theirStakeCellY = 156 + boxOffsetY + theirStakeIndex / 4 * 34;
                recordItemSprite(theirStakeCellX, theirStakeCellY, 48, 32, client.entityManager.GetItem(client.tradeItemsOther[theirStakeIndex]));

                if (!client.entityManager.GetItem(client.tradeItemsOther[theirStakeIndex]).IsStackable)
                {
                    client.gameGraphics.DrawString(client.tradeItemOtherCount[theirStakeIndex].ToString(), theirStakeCellX + 1, theirStakeCellY + 10, 1, 0xffff00);
                }

                if (client.mouseX > theirStakeCellX &&
                    client.mouseX < theirStakeCellX + 48 &&
                    client.mouseY > theirStakeCellY &&
                    client.mouseY < theirStakeCellY + 32)
                {
                    Item tradeOtherItem = client.entityManager.GetItem(client.tradeItemsOther[theirStakeIndex]);
                    client.gameGraphics.DrawString(tradeOtherItem.Name + ": @whi@" + tradeOtherItem.Description, boxOffsetX + 8, boxOffsetY + 273, 1, 0xffff00);
                }
            }
        }


        public void DrawTradeConfirmBox()
        {
            int boxOffsetX = 22;
            int boxOffsetY = 36;
            client.gameGraphics.DrawBox(boxOffsetX, boxOffsetY, 468, 16, 192);
            int backgroundColour = 0x989898;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 16, 468, 246, backgroundColour, 160);
            client.gameGraphics.DrawText(
                LocalisationManager.GetString("trade.confirm_prompt") + PlayerNameEncoder.HashToName(client.tradeConfirmOtherNameLong),
                boxOffsetX + 234,
                boxOffsetY + 12,
                1,
                0xffffff);
            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.confirm_giving"), boxOffsetX + 117, boxOffsetY + 30, 1, 0xffff00);

            for (int ourStakeIndex = 0; ourStakeIndex < client.tradeConfigItemCount; ourStakeIndex += 1)
            {
                Item tradeConfirmItem = client.entityManager.GetItem(client.tradeConfirmItems[ourStakeIndex]);
                string ourItemName = tradeConfirmItem.Name;

                if (!tradeConfirmItem.IsStackable)
                {
                    ourItemName = ourItemName + " x " + GameClientUtilities.FormatItemCount(client.tradeConfigItemsCount[ourStakeIndex]);
                }

                client.gameGraphics.DrawText(ourItemName, boxOffsetX + 117, boxOffsetY + 42 + ourStakeIndex * 12, 1, 0xffffff);
            }

            if (client.tradeConfigItemCount == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_nothing"), boxOffsetX + 117, boxOffsetY + 42, 1, 0xffffff);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.confirm_receiving"), boxOffsetX + 351, boxOffsetY + 30, 1, 0xffff00);

            for (int theirStakeIndex = 0; theirStakeIndex < client.tradeConfirmOtherItemCount; theirStakeIndex += 1)
            {
                Item tradeConfirmOtherItem = client.entityManager.GetItem(client.tradeConfirmOtherItems[theirStakeIndex]);
                string theirItemName = tradeConfirmOtherItem.Name;

                if (!tradeConfirmOtherItem.IsStackable)
                {
                    theirItemName = theirItemName + " x " + GameClientUtilities.FormatItemCount(client.tradeConfirmOtherItemsCount[theirStakeIndex]);
                }

                client.gameGraphics.DrawText(theirItemName, boxOffsetX + 351, boxOffsetY + 42 + theirStakeIndex * 12, 1, 0xffffff);
            }

            if (client.tradeConfirmOtherItemCount == 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_nothing"), boxOffsetX + 351, boxOffsetY + 42, 1, 0xffffff);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.confirm_warning1"), boxOffsetX + 234, boxOffsetY + 200, 4, 65535);
            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.confirm_warning2"), boxOffsetX + 234, boxOffsetY + 215, 1, 0xffffff);
            client.gameGraphics.DrawText(LocalisationManager.GetString("trade.confirm_warning3"), boxOffsetX + 234, boxOffsetY + 230, 1, 0xffffff);

            if (!client.tradeConfirmAccepted)
            {
                client.gameGraphics.DrawPicture(boxOffsetX + 118 - 35, boxOffsetY + 238, client.baseInventoryPic + 25);
                client.gameGraphics.DrawPicture(boxOffsetX + 352 - 35, boxOffsetY + 238, client.baseInventoryPic + 26);
            }
            else
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.duel_waiting"), boxOffsetX + 234, boxOffsetY + 250, 1, 0xffff00);
            }

            if (client.mouseButtonClick == 1)
            {
                if (client.mouseX >= boxOffsetX + 118 - 35 &&
                    client.mouseX <= boxOffsetX + 118 + 70 &&
                    client.mouseY >= boxOffsetY + 238 &&
                    client.mouseY <= boxOffsetY + 238 + 21)
                {
                    client.tradeConfirmAccepted = true;
                    client.streamClass.CreatePacket(53);
                    client.streamClass.FormatPacket();
                }

                if (client.mouseX >= boxOffsetX + 352 - 35 &&
                    client.mouseX <= boxOffsetX + 353 + 70 &&
                    client.mouseY >= boxOffsetY + 238 &&
                    client.mouseY <= boxOffsetY + 238 + 21)
                {
                    client.showTradeConfirmBox = false;
                    client.streamClass.CreatePacket(216);
                    client.streamClass.FormatPacket();
                }

                client.mouseButtonClick = 0;
            }
        }


        public void DrawDuelBox()
        {
            if (client.mouseButtonClick != 0 && client.mouseClickedHeldInTradeDuelBox == 0)
            {
                client.mouseClickedHeldInTradeDuelBox = 1;
            }

            if (client.mouseClickedHeldInTradeDuelBox > 0)
            {
                int clickOffsetX = client.mouseX - 22;
                int clickOffsetY = client.mouseY - 36;

                if (clickOffsetX >= 0 && clickOffsetY >= 0 && clickOffsetX < 468 && clickOffsetY < 262)
                {
                    if (clickOffsetX > 216 && clickOffsetY > 30 && clickOffsetX < 462 && clickOffsetY < 235)
                    {
                        int inventoryClickIndex = (clickOffsetX - 217) / 49 + (clickOffsetY - 31) / 34 * 5;

                        if (inventoryClickIndex >= 0 && inventoryClickIndex < client.inventoryItemsCount)
                        {
                            bool isItemAdded = false;
                            int existingStackCount = 0;
                            int clickedItemId = client.inventoryItems[inventoryClickIndex];

                            for (int duelItemIndex = 0; duelItemIndex < client.duelMyItemCount; duelItemIndex += 1)
                            {
                                if (client.duelMyItems[duelItemIndex] == clickedItemId)
                                {
                                    if (!client.entityManager.GetItem(clickedItemId).IsStackable)
                                    {
                                        for (int clickCount = 0; clickCount < client.mouseClickedHeldInTradeDuelBox; clickCount += 1)
                                        {
                                            if (client.duelMyItemsCount[duelItemIndex] < client.inventoryItemCount[inventoryClickIndex])
                                            {
                                                client.duelMyItemsCount[duelItemIndex] += 1;
                                            }

                                            isItemAdded = true;
                                        }
                                    }
                                    else
                                    {
                                        existingStackCount += 1;
                                    }
                                }
                            }

                            if (client.GetInventoryItemTotalCount(clickedItemId) <= existingStackCount)
                            {
                                isItemAdded = true;
                            }

                            if (client.entityManager.GetItem(clickedItemId).IsSpecial)
                            {
                                client.DisplayMessage(LocalisationManager.GetString("trade.duel_not_addable"), 3);
                                isItemAdded = true;
                            }

                            if (!isItemAdded && client.duelMyItemCount < 8)
                            {
                                client.duelMyItems[client.duelMyItemCount] = clickedItemId;
                                client.duelMyItemsCount[client.duelMyItemCount] = 1;
                                client.duelMyItemCount += 1;
                                isItemAdded = true;
                            }

                            if (isItemAdded)
                            {
                                client.streamClass.CreatePacket(123);
                                client.streamClass.AddByte(client.duelMyItemCount);

                                for (int streamItemIndex = 0; streamItemIndex < client.duelMyItemCount; streamItemIndex += 1)
                                {
                                    client.streamClass.AddShort(client.duelMyItems[streamItemIndex]);
                                    client.streamClass.AddInt(client.duelMyItemsCount[streamItemIndex]);
                                }

                                client.streamClass.FormatPacket();
                                client.duelOpponentAccepted = false;
                                client.duelMyAccepted = false;
                            }
                        }
                    }

                    if (clickOffsetX > 8 && clickOffsetY > 30 && clickOffsetX < 205 && clickOffsetY < 129)
                    {
                        int ourStakeClickIndex = (clickOffsetX - 9) / 49 + (clickOffsetY - 31) / 34 * 4;

                        if (ourStakeClickIndex >= 0 && ourStakeClickIndex < client.duelMyItemCount)
                        {
                            int ourStakeItemId = client.duelMyItems[ourStakeClickIndex];

                            for (int clickCountIndex = 0; clickCountIndex < client.mouseClickedHeldInTradeDuelBox; clickCountIndex += 1)
                            {
                                if (!client.entityManager.GetItem(ourStakeItemId).IsStackable &&
                                    client.duelMyItemsCount[ourStakeClickIndex] > 1)
                                {
                                    client.duelMyItemsCount[ourStakeClickIndex] -= 1;
                                    continue;
                                }

                                client.duelMyItemCount -= 1;
                                client.mouseButtonHeldTime = 0;

                                for (int shiftIndex = ourStakeClickIndex; shiftIndex < client.duelMyItemCount; shiftIndex += 1)
                                {
                                    client.duelMyItems[shiftIndex] = client.duelMyItems[shiftIndex + 1];
                                    client.duelMyItemsCount[shiftIndex] = client.duelMyItemsCount[shiftIndex + 1];
                                }

                                break;
                            }

                            client.streamClass.CreatePacket(123);
                            client.streamClass.AddByte(client.duelMyItemCount);

                            for (int streamItemIndex = 0; streamItemIndex < client.duelMyItemCount; streamItemIndex += 1)
                            {
                                client.streamClass.AddShort(client.duelMyItems[streamItemIndex]);
                                client.streamClass.AddInt(client.duelMyItemsCount[streamItemIndex]);
                            }

                            client.streamClass.FormatPacket();
                            client.duelOpponentAccepted = false;
                            client.duelMyAccepted = false;
                        }
                    }

                    bool isDuelOptionChanged = false;

                    if (clickOffsetX >= 93 && clickOffsetY >= 221 && clickOffsetX <= 104 && clickOffsetY <= 232)
                    {
                        client.duelNoRetreating = !client.duelNoRetreating;
                        isDuelOptionChanged = true;
                    }

                    if (clickOffsetX >= 93 && clickOffsetY >= 240 && clickOffsetX <= 104 && clickOffsetY <= 251)
                    {
                        client.duelNoMagic = !client.duelNoMagic;
                        isDuelOptionChanged = true;
                    }

                    if (clickOffsetX >= 191 && clickOffsetY >= 221 && clickOffsetX <= 202 && clickOffsetY <= 232)
                    {
                        client.duelNoPrayer = !client.duelNoPrayer;
                        isDuelOptionChanged = true;
                    }

                    if (clickOffsetX >= 191 && clickOffsetY >= 240 && clickOffsetX <= 202 && clickOffsetY <= 251)
                    {
                        client.duelNoWeapons = !client.duelNoWeapons;
                        isDuelOptionChanged = true;
                    }

                    if (isDuelOptionChanged)
                    {
                        client.streamClass.CreatePacket(225);
                        int duelNoRetreatingByte = 0;
                        if (client.duelNoRetreating)
                        {
                            duelNoRetreatingByte = 1;
                        }
                        client.streamClass.AddByte(duelNoRetreatingByte);
                        int duelNoMagicByte = 0;
                        if (client.duelNoMagic)
                        {
                            duelNoMagicByte = 1;
                        }
                        client.streamClass.AddByte(duelNoMagicByte);
                        int duelNoPrayerByte = 0;
                        if (client.duelNoPrayer)
                        {
                            duelNoPrayerByte = 1;
                        }
                        client.streamClass.AddByte(duelNoPrayerByte);
                        int duelNoWeaponsByte = 0;
                        if (client.duelNoWeapons)
                        {
                            duelNoWeaponsByte = 1;
                        }
                        client.streamClass.AddByte(duelNoWeaponsByte);
                        client.streamClass.FormatPacket();
                        client.duelOpponentAccepted = false;
                        client.duelMyAccepted = false;
                    }
                    if (clickOffsetX >= 217 && clickOffsetY >= 238 && clickOffsetX <= 286 && clickOffsetY <= 259)
                    {
                        client.duelMyAccepted = true;
                        client.streamClass.CreatePacket(252);
                        client.streamClass.FormatPacket();
                    }

                    if (clickOffsetX >= 394 && clickOffsetY >= 238 && clickOffsetX < 463 && clickOffsetY < 259)
                    {
                        client.showDuelBox = false;
                        client.streamClass.CreatePacket(35);
                        client.streamClass.FormatPacket();
                    }
                }
                else if (client.mouseButtonClick != 0)
                {
                    client.showDuelBox = false;
                    client.streamClass.CreatePacket(35);
                    client.streamClass.FormatPacket();
                }

                client.mouseButtonClick = 0;
                client.mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!client.showDuelBox)
            {
                return;
            }

            int boxOffsetX = 22;
            int boxOffsetY = 36;
            client.gameGraphics.DrawBox(boxOffsetX, boxOffsetY, 468, 12, 0xc90b1d);
            int backgroundColour = 0x989898;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 12, 468, 18, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + 30, 8, 248, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 205, boxOffsetY + 30, 11, 248, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 462, boxOffsetY + 30, 6, 248, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 99, 197, 24, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 192, 197, 23, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 258, 197, 20, backgroundColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 216, boxOffsetY + 235, 246, 43, backgroundColour, 160);
            int cellColour = 0xd0d0d0;
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 30, 197, 69, cellColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 123, 197, 69, cellColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 8, boxOffsetY + 215, 197, 43, cellColour, 160);
            client.gameGraphics.DrawBoxAlpha(boxOffsetX + 216, boxOffsetY + 30, 246, 205, cellColour, 160);

            for (int ourRowIndex = 0; ourRowIndex < 3; ourRowIndex += 1)
            {
                client.gameGraphics.DrawLineX(boxOffsetX + 8, boxOffsetY + 30 + ourRowIndex * 34, 197, 0);
            }

            for (int theirRowIndex = 0; theirRowIndex < 3; theirRowIndex += 1)
            {
                client.gameGraphics.DrawLineX(boxOffsetX + 8, boxOffsetY + 123 + theirRowIndex * 34, 197, 0);
            }

            for (int inventoryRowIndex = 0; inventoryRowIndex < 7; inventoryRowIndex += 1)
            {
                client.gameGraphics.DrawLineX(boxOffsetX + 216, boxOffsetY + 30 + inventoryRowIndex * 34, 246, 0);
            }

            for (int columnIndex = 0; columnIndex < 6; columnIndex += 1)
            {
                if (columnIndex < 5)
                {
                    client.gameGraphics.DrawLineY(boxOffsetX + 8 + columnIndex * 49, boxOffsetY + 30, 69, 0);
                }

                if (columnIndex < 5)
                {
                    client.gameGraphics.DrawLineY(boxOffsetX + 8 + columnIndex * 49, boxOffsetY + 123, 69, 0);
                }

                client.gameGraphics.DrawLineY(boxOffsetX + 216 + columnIndex * 49, boxOffsetY + 30, 205, 0);
            }

            client.gameGraphics.DrawLineX(boxOffsetX + 8, boxOffsetY + 215, 197, 0);
            client.gameGraphics.DrawLineX(boxOffsetX + 8, boxOffsetY + 257, 197, 0);
            client.gameGraphics.DrawLineY(boxOffsetX + 8, boxOffsetY + 215, 43, 0);
            client.gameGraphics.DrawLineY(boxOffsetX + 204, boxOffsetY + 215, 43, 0);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_with_prefix") + client.duelOpponent, boxOffsetX + 1, boxOffsetY + 10, 1, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_your_stake_tab"), boxOffsetX + 9, boxOffsetY + 27, 4, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_opponent_stake_tab"), boxOffsetX + 9, boxOffsetY + 120, 4, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_options"), boxOffsetX + 9, boxOffsetY + 212, 4, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.trade_your_inventory"), boxOffsetX + 216, boxOffsetY + 27, 4, 0xffffff);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_option_no_retreat"), boxOffsetX + 8 + 1, boxOffsetY + 215 + 16, 3, 0xffff00);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_option_no_magic"), boxOffsetX + 8 + 1, boxOffsetY + 215 + 35, 3, 0xffff00);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_option_no_prayer"), boxOffsetX + 8 + 102, boxOffsetY + 215 + 16, 3, 0xffff00);
            client.gameGraphics.DrawString(LocalisationManager.GetString("trade.duel_option_no_weapons"), boxOffsetX + 8 + 102, boxOffsetY + 215 + 35, 3, 0xffff00);
            client.gameGraphics.DrawBoxEdge(boxOffsetX + 93, boxOffsetY + 215 + 6, 11, 11, 0xffff00);

            if (client.duelNoRetreating)
            {
                client.gameGraphics.DrawBox(boxOffsetX + 95, boxOffsetY + 215 + 8, 7, 7, 0xffff00);
            }

            client.gameGraphics.DrawBoxEdge(boxOffsetX + 93, boxOffsetY + 215 + 25, 11, 11, 0xffff00);

            if (client.duelNoMagic)
            {
                client.gameGraphics.DrawBox(boxOffsetX + 95, boxOffsetY + 215 + 27, 7, 7, 0xffff00);
            }

            client.gameGraphics.DrawBoxEdge(boxOffsetX + 191, boxOffsetY + 215 + 6, 11, 11, 0xffff00);

            if (client.duelNoPrayer)
            {
                client.gameGraphics.DrawBox(boxOffsetX + 193, boxOffsetY + 215 + 8, 7, 7, 0xffff00);
            }

            client.gameGraphics.DrawBoxEdge(boxOffsetX + 191, boxOffsetY + 215 + 25, 11, 11, 0xffff00);

            if (client.duelNoWeapons)
            {
                client.gameGraphics.DrawBox(boxOffsetX + 193, boxOffsetY + 215 + 27, 7, 7, 0xffff00);
            }

            if (!client.duelMyAccepted)
            {
                client.gameGraphics.DrawPicture(boxOffsetX + 217, boxOffsetY + 238, client.baseInventoryPic + 25);
            }

            client.gameGraphics.DrawPicture(boxOffsetX + 394, boxOffsetY + 238, client.baseInventoryPic + 26);

            if (client.duelOpponentAccepted)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_other_accepted"), boxOffsetX + 341, boxOffsetY + 246, 1, 0xffffff);
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_has_accepted"), boxOffsetX + 341, boxOffsetY + 256, 1, 0xffffff);
            }

            if (client.duelMyAccepted)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_waiting_for"), boxOffsetX + 217 + 35, boxOffsetY + 246, 1, 0xffffff);
                client.gameGraphics.DrawText(LocalisationManager.GetString("trade.trade_other_player"), boxOffsetX + 217 + 35, boxOffsetY + 256, 1, 0xffffff);
            }

            for (int inventoryIndex = 0; inventoryIndex < client.inventoryItemsCount; inventoryIndex += 1)
            {
                int inventoryCellX = 217 + boxOffsetX + inventoryIndex % 5 * 49;
                int inventoryCellY = 31 + boxOffsetY + inventoryIndex / 5 * 34;
                recordItemSprite(inventoryCellX, inventoryCellY, 48, 32, client.entityManager.GetItem(client.inventoryItems[inventoryIndex]));

                if (!client.entityManager.GetItem(client.inventoryItems[inventoryIndex]).IsStackable)
                {
                    client.gameGraphics.DrawString(client.inventoryItemCount[inventoryIndex].ToString(), inventoryCellX + 1, inventoryCellY + 10, 1, 0xffff00);
                }
            }

            for (int ourStakeIndex = 0; ourStakeIndex < client.duelMyItemCount; ourStakeIndex += 1)
            {
                int ourStakeCellX = 9 + boxOffsetX + ourStakeIndex % 4 * 49;
                int ourStakeCellY = 31 + boxOffsetY + ourStakeIndex / 4 * 34;
                recordItemSprite(ourStakeCellX, ourStakeCellY, 48, 32, client.entityManager.GetItem(client.duelMyItems[ourStakeIndex]));

                if (!client.entityManager.GetItem(client.duelMyItems[ourStakeIndex]).IsStackable)
                {
                    client.gameGraphics.DrawString(client.duelMyItemsCount[ourStakeIndex].ToString(), ourStakeCellX + 1, ourStakeCellY + 10, 1, 0xffff00);
                }

                if (client.mouseX > ourStakeCellX &&
                    client.mouseX < ourStakeCellX + 48 &&
                    client.mouseY > ourStakeCellY &&
                    client.mouseY < ourStakeCellY + 32)
                {
                    Item duelMyItem = client.entityManager.GetItem(client.duelMyItems[ourStakeIndex]);
                    client.gameGraphics.DrawString(duelMyItem.Name + ": @whi@" + duelMyItem.Description, boxOffsetX + 8, boxOffsetY + 273, 1, 0xffff00);
                }
            }

            for (int opponentStakeIndex = 0; opponentStakeIndex < client.duelOpponentItemCount; opponentStakeIndex += 1)
            {
                int opponentStakeCellX = 9 + boxOffsetX + opponentStakeIndex % 4 * 49;
                int opponentStakeCellY = 124 + boxOffsetY + opponentStakeIndex / 4 * 34;
                recordItemSprite(opponentStakeCellX, opponentStakeCellY, 48, 32, client.entityManager.GetItem(client.duelOpponentItems[opponentStakeIndex]));

                if (!client.entityManager.GetItem(client.duelOpponentItems[opponentStakeIndex]).IsStackable)
                {
                    client.gameGraphics.DrawString(client.duelOpponentItemsCount[opponentStakeIndex].ToString(), opponentStakeCellX + 1, opponentStakeCellY + 10, 1, 0xffff00);
                }

                if (client.mouseX > opponentStakeCellX &&
                    client.mouseX < opponentStakeCellX + 48 &&
                    client.mouseY > opponentStakeCellY &&
                    client.mouseY < opponentStakeCellY + 32)
                {
                    Item duelOpponentItem = client.entityManager.GetItem(client.duelOpponentItems[opponentStakeIndex]);
                    client.gameGraphics.DrawString(duelOpponentItem.Name + ": @whi@" + duelOpponentItem.Description, boxOffsetX + 8, boxOffsetY + 273, 1, 0xffff00);
                }
            }
        }

   
    }
}
