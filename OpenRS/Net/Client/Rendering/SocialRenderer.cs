using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class SocialRenderer(GameClient client)
    {

        public void DrawFriendsMenu(bool canClick)
        {
            int menuX = client.gameGraphics.gameWidth - 199;
            int menuY = 36;
            client.gameGraphics.DrawPicture(menuX - 49, 3, client.baseInventoryPic + 5);
            int menuWidth = 196;
            int menuHeight = 182;
            int friendsTabColour = GameImage.RgbToInt(160, 160, 160);
            int ignoreTabColour = GameImage.RgbToInt(160, 160, 160);

            if (client.friendsIgnoreMenuSelected == 0)
            {
                friendsTabColour = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                ignoreTabColour = GameImage.RgbToInt(220, 220, 220);
            }

            client.gameGraphics.DrawBoxAlpha(menuX, menuY, menuWidth / 2, 24, friendsTabColour, 128);
            client.gameGraphics.DrawBoxAlpha(menuX + menuWidth / 2, menuY, menuWidth / 2, 24, ignoreTabColour, 128);
            client.gameGraphics.DrawBoxAlpha(menuX, menuY + 24, menuWidth, menuHeight - 24, GameImage.RgbToInt(220, 220, 220), 128);
            client.gameGraphics.DrawLineX(menuX, menuY + 24, menuWidth, 0);
            client.gameGraphics.DrawLineY(menuX + menuWidth / 2, menuY, 24, 0);
            client.gameGraphics.DrawLineX(menuX, menuY + menuHeight - 16, menuWidth, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("social.tab_friends"), menuX + menuWidth / 4, menuY + 16, 4, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("social.tab_ignore"), menuX + menuWidth / 4 + menuWidth / 2, menuY + 16, 4, 0);
            client.friendsMenu.ClearList(client.friendsMenuHandle);

            if (client.friendsIgnoreMenuSelected == 0)
            {
                for (int friendsListIndex = 0; friendsListIndex < client.friendsCount; friendsListIndex += 1)
                {
                    string friendColourPrefix = "@red@";

                    if (client.friendsWorld[friendsListIndex] == 99)
                    {
                        friendColourPrefix = "@gre@";
                    }
                    else if (client.friendsWorld[friendsListIndex] > 0)
                    {
                        friendColourPrefix = "@yel@";
                    }

                    client.friendsMenu.AddListItem(client.friendsMenuHandle, friendsListIndex, friendColourPrefix + DataOperations.HashToName(client.friendsList[friendsListIndex]) + "~439~@whi@Remove         WWWWWWWWWW");
                }
            }

            if (client.friendsIgnoreMenuSelected == 1)
            {
                for (int ignoreListIndex = 0; ignoreListIndex < client.ignoresCount; ignoreListIndex += 1)
                {
                    client.friendsMenu.AddListItem(client.friendsMenuHandle, ignoreListIndex, "@yel@" + DataOperations.HashToName(client.ignoresList[ignoreListIndex]) + "~439~@whi@Remove         WWWWWWWWWW");
                }
            }

            client.friendsMenu.DrawMenu();

            if (client.friendsIgnoreMenuSelected == 0)
            {
                int highlightedFriendIndex = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);

                if (highlightedFriendIndex >= 0 && client.mouseX < 489)
                {
                    if (client.mouseX > 429)
                    {
                        client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_remove_prefix") + DataOperations.HashToName(client.friendsList[highlightedFriendIndex]), menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                    }
                    else if (client.friendsWorld[highlightedFriendIndex] == 99)
                    {
                        client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_message_prefix") + DataOperations.HashToName(client.friendsList[highlightedFriendIndex]), menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                    }
                    else if (client.friendsWorld[highlightedFriendIndex] > 0)
                    {
                        client.gameGraphics.DrawText(DataOperations.HashToName(client.friendsList[highlightedFriendIndex]) + " is on world " + client.friendsWorld[highlightedFriendIndex], menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                    }
                    else
                    {
                        client.gameGraphics.DrawText(DataOperations.HashToName(client.friendsList[highlightedFriendIndex]) + " is offline", menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_send_hint"), menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                }

                int addFriendButtonColour = 0xffffff;

                if (client.mouseX > menuX &&
                    client.mouseX < menuX + menuWidth &&
                    client.mouseY > menuY + menuHeight - 16 &&
                    client.mouseY < menuY + menuHeight)
                {
                    addFriendButtonColour = 0xffff00;
                }

                client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_add"), menuX + menuWidth / 2, menuY + menuHeight - 3, 1, addFriendButtonColour);
            }

            if (client.friendsIgnoreMenuSelected == 1)
            {
                int highlightedIgnoreIndex = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);

                if (highlightedIgnoreIndex >= 0 && client.mouseX < 489 && client.mouseX > 429)
                {
                    client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_remove_prefix") + DataOperations.HashToName(client.ignoresList[highlightedIgnoreIndex]), menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawText(LocalisationManager.GetString("social.ignore_blocking_prefix"), menuX + menuWidth / 2, menuY + 35, 1, 0xffffff);
                }

                int addIgnoreButtonColour = 0xffffff;

                if (client.mouseX > menuX &&
                    client.mouseX < menuX + menuWidth &&
                    client.mouseY > menuY + menuHeight - 16 &&
                    client.mouseY < menuY + menuHeight)
                {
                    addIgnoreButtonColour = 0xffff00;
                }

                client.gameGraphics.DrawText(LocalisationManager.GetString("social.ignore_add"), menuX + menuWidth / 2, menuY + menuHeight - 3, 1, addIgnoreButtonColour);
            }

            if (!canClick)
            {
                return;
            }

            int clickOffsetX = client.mouseX - (client.gameGraphics.gameWidth - 199);
            int clickOffsetY = client.mouseY - 36;

            if (clickOffsetX >= 0 && clickOffsetY >= 0 && clickOffsetX < 196 && clickOffsetY < 182)
            {
                client.friendsMenu.MouseClick(clickOffsetX + (client.gameGraphics.gameWidth - 199), clickOffsetY + 36, client.lastMouseButton, client.mouseButton);

                if (clickOffsetY <= 24 && client.mouseButtonClick == 1)
                {
                    if (clickOffsetX < 98 && client.friendsIgnoreMenuSelected == 1)
                    {
                        client.friendsIgnoreMenuSelected = 0;
                        client.friendsMenu.SwitchList(client.friendsMenuHandle);
                    }
                    else if (clickOffsetX > 98 && client.friendsIgnoreMenuSelected == 0)
                    {
                        client.friendsIgnoreMenuSelected = 1;
                        client.friendsMenu.SwitchList(client.friendsMenuHandle);
                    }
                }

                if (client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 0)
                {
                    int clickedFriendIndex = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);

                    if (clickedFriendIndex >= 0 && client.mouseX < 489)
                    {
                        if (client.mouseX > 429)
                        {
                            client.CallRemoveFriend(client.friendsList[clickedFriendIndex]);
                        }
                        else if (client.friendsWorld[clickedFriendIndex] != 0)
                        {
                            client.showFriendsBox = 2;
                            client.pmTarget = client.friendsList[clickedFriendIndex];
                            client.pmText = "";
                            client.enteredPMText = "";
                        }
                    }
                }

                if (client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 1)
                {
                    int clickedIgnoreIndex = client.friendsMenu.GetEntryHighlighted(client.friendsMenuHandle);

                    if (clickedIgnoreIndex >= 0 && client.mouseX < 489 && client.mouseX > 429)
                    {
                        client.CallRemoveIgnore(client.ignoresList[clickedIgnoreIndex]);
                    }
                }

                if (clickOffsetY > 166 && client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 0)
                {
                    client.showFriendsBox = 1;
                    client.inputText = "";
                    client.enteredInputText = "";
                }

                if (clickOffsetY > 166 && client.mouseButtonClick == 1 && client.friendsIgnoreMenuSelected == 1)
                {
                    client.showFriendsBox = 3;
                    client.inputText = "";
                    client.enteredInputText = "";
                }

                client.mouseButtonClick = 0;
            }
        }


        public void DrawChatMessageTabs()
        {
            client.gameGraphics.DrawPicture(0, client.windowHeight - 4, client.baseInventoryPic + 23);
            int tabColour = GameImage.RgbToInt(200, 200, 255);

            if (client.messagesTab == 0)
            {
                tabColour = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabAllMsgFlash % 30 > 15)
            {
                tabColour = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("social.chat_all_messages"), 54, client.windowHeight + 6, 0, tabColour);
            tabColour = GameImage.RgbToInt(200, 200, 255);

            if (client.messagesTab == 1)
            {
                tabColour = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabHistoryFlash % 30 > 15)
            {
                tabColour = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("social.chat_chat_history"), 155, client.windowHeight + 6, 0, tabColour);
            tabColour = GameImage.RgbToInt(200, 200, 255);

            if (client.messagesTab == 2)
            {
                tabColour = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabQuestFlash % 30 > 15)
            {
                tabColour = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("social.chat_quest_history"), 255, client.windowHeight + 6, 0, tabColour);
            tabColour = GameImage.RgbToInt(200, 200, 255);

            if (client.messagesTab == 3)
            {
                tabColour = GameImage.RgbToInt(255, 200, 50);
            }

            if (client.chatTabPrivateFlash % 30 > 15)
            {
                tabColour = GameImage.RgbToInt(255, 50, 50);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("social.chat_private_history"), 355, client.windowHeight + 6, 0, tabColour);
            client.gameGraphics.DrawText(LocalisationManager.GetString("social.chat_report_abuse"), 457, client.windowHeight + 6, 0, 0xffffff);
        }


        public void DrawServerMessageBox()
        {
            int boxWidth = 400;
            int boxHeight = 100;

            if (client.serverMessageBoxTop)
            {
                boxHeight = 300;
            }

            client.gameGraphics.DrawBox(256 - boxWidth / 2, 167 - boxHeight / 2, boxWidth, boxHeight, 0);
            client.gameGraphics.DrawBoxEdge(256 - boxWidth / 2, 167 - boxHeight / 2, boxWidth, boxHeight, 0xffffff);
            client.gameGraphics.DrawFloatingText(client.serverMessage, 256, 167 - boxHeight / 2 + 20, 1, 0xffffff, boxWidth - 40);
            int closeLinkY = 157 + boxHeight / 2;
            int closeLinkColour = 0xffffff;

            if (client.mouseY > closeLinkY - 12 &&
                client.mouseY <= closeLinkY &&
                client.mouseX > 106 &&
                client.mouseX < 406)
            {
                closeLinkColour = 0xff0000;
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.close_window"), 256, closeLinkY, 1, closeLinkColour);

            if (client.mouseButtonClick == 1)
            {
                if (closeLinkColour == 0xff0000)
                {
                    client.showServerMessageBox = false;
                }

                if ((client.mouseX < 256 - boxWidth / 2 || client.mouseX > 256 + boxWidth / 2) &&
                    (client.mouseY < 167 - boxHeight / 2 || client.mouseY > 167 + boxHeight / 2))
                {
                    client.showServerMessageBox = false;
                }
            }

            client.mouseButtonClick = 0;
        }


        public void DrawFriendsBox()
        {
            int boxY = 145;

            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;

                if (client.showFriendsBox == 1 && (client.mouseX < 106 || client.mouseY < boxY || client.mouseX > 406 || client.mouseY > 215))
                {
                    client.showFriendsBox = 0;
                    return;
                }

                if (client.showFriendsBox == 2 && (client.mouseX < 6 || client.mouseY < boxY || client.mouseX > 506 || client.mouseY > 215))
                {
                    client.showFriendsBox = 0;
                    return;
                }

                if (client.showFriendsBox == 3 && (client.mouseX < 106 || client.mouseY < boxY || client.mouseX > 406 || client.mouseY > 215))
                {
                    client.showFriendsBox = 0;
                    return;
                }

                if (client.mouseX > 236 && client.mouseX < 276 && client.mouseY > 193 && client.mouseY < 213)
                {
                    client.showFriendsBox = 0;
                    return;
                }
            }

            if (client.showFriendsBox == 1)
            {
                client.gameGraphics.DrawBox(106, boxY, 300, 70, 0);
                client.gameGraphics.DrawBoxEdge(106, boxY, 300, 70, 0xffffff);
                boxY += 20;
                client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_add_prompt"), 256, boxY, 4, 0xffffff);
                boxY += 20;
                client.gameGraphics.DrawText(client.inputText + "*", 256, boxY, 4, 0xffffff);

                if (client.enteredInputText.Length > 0)
                {
                    string trimmedName = client.enteredInputText.Trim();
                    client.inputText = "";
                    client.enteredInputText = "";
                    client.showFriendsBox = 0;

                    if (trimmedName.Length > 0 && DataOperations.NameToHash(trimmedName) != client.ourPlayer.nameHash)
                    {
                        client.CallAddFriend(trimmedName);
                    }
                }
            }

            if (client.showFriendsBox == 2)
            {
                client.gameGraphics.DrawBox(6, boxY, 500, 70, 0);
                client.gameGraphics.DrawBoxEdge(6, boxY, 500, 70, 0xffffff);
                boxY += 20;
                client.gameGraphics.DrawText(LocalisationManager.GetString("social.friends_message_prompt") + DataOperations.HashToName(client.pmTarget), 256, boxY, 4, 0xffffff);
                boxY += 20;
                client.gameGraphics.DrawText(client.pmText + "*", 256, boxY, 4, 0xffffff);

                if (client.enteredPMText.Length > 0)
                {
                    string messageText = client.enteredPMText;
                    client.pmText = "";
                    client.enteredPMText = "";
                    client.showFriendsBox = 0;
                    int byteCount = ChatMessage.StringToBytes(messageText);
                    client.CallSendPrivateMessage(client.pmTarget, ChatMessage.lastChat, byteCount);
                    messageText = ChatMessage.BytesToString(ChatMessage.lastChat, 0, byteCount);
                    client.DisplayMessage("@pri@You tell " + DataOperations.HashToName(client.pmTarget) + ": " + messageText);
                }
            }

            if (client.showFriendsBox == 3)
            {
                client.gameGraphics.DrawBox(106, boxY, 300, 70, 0);
                client.gameGraphics.DrawBoxEdge(106, boxY, 300, 70, 0xffffff);
                boxY += 20;
                client.gameGraphics.DrawText(LocalisationManager.GetString("social.ignore_add_prompt"), 256, boxY, 4, 0xffffff);
                boxY += 20;
                client.gameGraphics.DrawText(client.inputText + "*", 256, boxY, 4, 0xffffff);

                if (client.enteredInputText.Length > 0)
                {
                    string trimmedIgnoreName = client.enteredInputText.Trim();
                    client.inputText = "";
                    client.enteredInputText = "";
                    client.showFriendsBox = 0;

                    if (trimmedIgnoreName.Length > 0 && DataOperations.NameToHash(trimmedIgnoreName) != client.ourPlayer.nameHash)
                    {
                        client.CallAddIgnore(trimmedIgnoreName);
                    }
                }
            }

            int cancelLabelColour = 0xffffff;

            if (client.mouseX > 236 && client.mouseX < 276 && client.mouseY > 193 && client.mouseY < 213)
            {
                cancelLabelColour = 0xffff00;
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("social.action_cancel"), 256, 208, 1, cancelLabelColour);
        }


    }
}
