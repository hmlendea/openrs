using System;

using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Input
{
    internal sealed class ChatAndSleepInputHandler(GameClient client)
    {
        private static string ChatCommandPrefix => "::";

        private static string CloseConnectionCommand => "::closecon";

        private static int HeldMouseIncrementFast => 100000;

        private static int HeldMouseIncrementLarge => 10000;

        private static int HeldMouseIncrementMedium => 1000;

        private static int HeldMouseIncrementSmall => 100;

        private static int HeldMouseIncrementTiny => 10;

        private static int HeldMouseInitialRepeatThreshold => 20;

        private static int HeldMouseMask => 5;

        private static int HeldMouseThresholdFast => 500;

        private static int HeldMouseThresholdLarge => 350;

        private static int HeldMouseThresholdMedium => 250;

        private static int HeldMouseThresholdRepeat => 50;

        private static int HeldMouseThresholdSmall => 150;

        private static int HeldMouseThresholdTiny => 100;

        private static string LostConnectionCommand => "::lostcon";

        private static int MessageAbuseTabMaximumX => 497;

        private static int MessageAbuseTabMinimumX => 417;

        private static int MessageChatTabAllMaximumX => 96;

        private static int MessageChatTabAllMinimumX => 15;

        private static int MessageChatTabHistoryMaximumX => 194;

        private static int MessageChatTabHistoryMinimumX => 110;

        private static int MessageChatTabPrivateMaximumX => 395;

        private static int MessageChatTabPrivateMinimumX => 315;

        private static int MessageChatTabQuestMaximumX => 295;

        private static int MessageChatTabQuestMinimumX => 215;

        private static int MessageInputProtectionMinimumX => 494;

        private static int MessageInputProtectionMinimumYOffset => 66;

        private static int MessageTabAbuse => 1;

        private static int MessageTabBottomMargin => 4;

        private static int MessageTabHistory => 1;

        private static int MessageTabNone => 0;

        private static int MessageTabPrivate => 3;

        private static int MessageTabQuest => 2;

        private static int OutgoingChatMessageType => 2;

        private static int OutgoingChatMessageTimeout => 150;

        private static int ResetListEntryValue => 0xf423f;

        private static string SleepKeepAliveText => "-null-";

        private static int SleepMouseMaximumX => 456;

        private static int SleepMouseMaximumY => 310;

        private static int SleepMouseMinimumX => 56;

        private static int SleepMouseMinimumY => 275;

        private static int SleepPacketIdentifier => 200;

        private static string SleepStatusText => "Please wait...";

        private static int TradeOrDuelHoldReset => 0;

        private static int VisibleMessageCount => 5;

        internal void HandleChatAndMouseInput()
        {
            HandleChatTabClicks();
            client.chatInputMenu.MouseClick(
                client.mouseX,
                client.mouseY,
                client.lastMouseButton,
                client.mouseButton);
            PreventMessageListClickThrough();
            HandleChatInputSubmission();
            UpdateVisibleMessageTimeouts();
            ClearLastMouseButtonWhenRespawning();
            UpdateHeldTradeAndDuelMouseState();
            UpdateMouseButtonClick();
            client.gameCamera.SetMousePosition(client.mouseX, client.mouseY);
            client.lastMouseButton = 0;
        }

        internal bool HandleSleepInput()
        {
            if (!client.isSleeping)
            {
                return false;
            }

            HandleSleepTextInput();
            HandleSleepMouseInput();
            client.lastMouseButton = 0;
            return true;
        }

        private void ClearInputFields()
        {
            client.inputText = string.Empty;
            client.enteredInputText = string.Empty;
        }

        private void ClearLastMouseButtonWhenRespawning()
        {
            if (client.playerAliveTimeout != 0)
            {
                client.lastMouseButton = 0;
            }
        }

        private void HandleChatCommand(string input)
        {
            string command = input[2..];

            if (!client.HandleCommand(command))
            {
                client.CallSendCommand(command);
            }
        }

        private void HandleChatInputSubmission()
        {
            if (!client.chatInputMenu.IsClicked(client.chatInputBox))
            {
                return;
            }

            string input = client.chatInputMenu.GetText(client.chatInputBox);
            client.chatInputMenu.UpdateText(client.chatInputBox, string.Empty);

            if (input.StartsWith(ChatCommandPrefix, StringComparison.Ordinal))
            {
                HandleChatCommand(input);
                return;
            }

            SendPublicChatMessage(input);
        }

        private void HandleChatTabClicks()
        {
            if (client.mouseY <= client.windowHeight - MessageTabBottomMargin)
            {
                return;
            }

            if (client.lastMouseButton == 1)
            {
                UpdateClickedChatTab();
            }

            client.lastMouseButton = 0;
            client.mouseButton = 0;
        }

        private void HandleSleepMouseInput()
        {
            if (client.lastMouseButton != 1)
            {
                return;
            }

            if (client.mouseY <= SleepMouseMinimumY || client.mouseY >= SleepMouseMaximumY)
            {
                return;
            }

            if (client.mouseX <= SleepMouseMinimumX || client.mouseX >= SleepMouseMaximumX)
            {
                return;
            }

            SendSleepText(SleepKeepAliveText);
        }

        private void HandleSleepTextInput()
        {
            if (client.enteredInputText.Length <= 0)
            {
                return;
            }

            string lowerInput = client.enteredInputText.ToLowerInvariant();

            if (string.Equals(lowerInput, LostConnectionCommand, StringComparison.Ordinal))
            {
                client.streamClass.CloseStream();
                return;
            }

            if (string.Equals(lowerInput, CloseConnectionCommand, StringComparison.Ordinal))
            {
                client.CallRequestLogout();
                return;
            }

            SendSleepText(client.enteredInputText);
        }

        private void IncreaseHeldTradeAndDuelCounter()
        {
            if (client.mouseButtonHeldTime > HeldMouseThresholdFast)
            {
                client.mouseClickedHeldInTradeDuelBox += HeldMouseIncrementFast;
                return;
            }

            if (client.mouseButtonHeldTime > HeldMouseThresholdLarge)
            {
                client.mouseClickedHeldInTradeDuelBox += HeldMouseIncrementLarge;
                return;
            }

            if (client.mouseButtonHeldTime > HeldMouseThresholdMedium)
            {
                client.mouseClickedHeldInTradeDuelBox += HeldMouseIncrementMedium;
                return;
            }

            if (client.mouseButtonHeldTime > HeldMouseThresholdSmall)
            {
                client.mouseClickedHeldInTradeDuelBox += HeldMouseIncrementSmall;
                return;
            }

            if (client.mouseButtonHeldTime > HeldMouseThresholdTiny)
            {
                client.mouseClickedHeldInTradeDuelBox += HeldMouseIncrementTiny;
                return;
            }

            if (client.mouseButtonHeldTime > HeldMouseThresholdRepeat)
            {
                client.mouseClickedHeldInTradeDuelBox += 1;
                return;
            }

            if (client.mouseButtonHeldTime > HeldMouseInitialRepeatThreshold &&
                (client.mouseButtonHeldTime & HeldMouseMask) == 0)
            {
                client.mouseClickedHeldInTradeDuelBox += 1;
            }
        }

        private void OpenAbuseBox()
        {
            client.showAbuseBox = MessageTabAbuse;
            client.reportAbuseOptionSelected = 0;
            ClearInputFields();
        }

        private void PreventMessageListClickThrough()
        {
            if (client.messagesTab <= 0)
            {
                return;
            }

            if (client.mouseX < MessageInputProtectionMinimumX)
            {
                return;
            }

            if (client.mouseY < client.windowHeight - MessageInputProtectionMinimumYOffset)
            {
                return;
            }

            client.lastMouseButton = 0;
        }

        private void ResetSleepStateAfterSubmit()
        {
            ClearInputFields();
            client.sleepingStatusText = SleepStatusText;
        }

        private void SendPublicChatMessage(string input)
        {
            int chatMessageLength = ChatMessage.StringToBytes(input);
            client.CallSendChatMessage(ChatMessage.LastChat, chatMessageLength);
            input = ChatMessage.BytesToString(ChatMessage.LastChat, 0, chatMessageLength);
            client.ourPlayer.LastMessageTimeout = OutgoingChatMessageTimeout;
            client.ourPlayer.LastMessage = input;
            client.DisplayMessage(
                client.ourPlayer.Username + ": " + input,
                OutgoingChatMessageType);
        }

        private void SendSleepText(string text)
        {
            client.streamClass.CreatePacket(SleepPacketIdentifier);
            client.streamClass.AddString(text);

            if (!client.sleepWordDelay)
            {
                client.streamClass.AddByte(0);
                client.sleepWordDelay = true;
            }

            client.streamClass.FormatPacket();
            ResetSleepStateAfterSubmit();
        }

        private void UpdateClickedChatTab()
        {
            if (client.mouseX > MessageChatTabAllMinimumX &&
                client.mouseX < MessageChatTabAllMaximumX)
            {
                client.messagesTab = MessageTabNone;
            }

            if (client.mouseX > MessageChatTabHistoryMinimumX &&
                client.mouseX < MessageChatTabHistoryMaximumX)
            {
                client.messagesTab = MessageTabHistory;
                client.chatInputMenu.listShownEntries[client.messagesHandleType2] =
                    ResetListEntryValue;
            }

            if (client.mouseX > MessageChatTabQuestMinimumX &&
                client.mouseX < MessageChatTabQuestMaximumX)
            {
                client.messagesTab = MessageTabQuest;
                client.chatInputMenu.listShownEntries[client.messagesHandleType5] =
                    ResetListEntryValue;
            }

            if (client.mouseX > MessageChatTabPrivateMinimumX &&
                client.mouseX < MessageChatTabPrivateMaximumX)
            {
                client.messagesTab = MessageTabPrivate;
                client.chatInputMenu.listShownEntries[client.messagesHandleType6] =
                    ResetListEntryValue;
            }

            if (client.mouseX > MessageAbuseTabMinimumX &&
                client.mouseX < MessageAbuseTabMaximumX)
            {
                OpenAbuseBox();
            }
        }

        private void UpdateHeldTradeAndDuelMouseState()
        {
            if (!client.showTradeBox && !client.showDuelBox)
            {
                client.mouseButtonHeldTime = TradeOrDuelHoldReset;
                client.mouseClickedHeldInTradeDuelBox = TradeOrDuelHoldReset;
                return;
            }

            if (client.mouseButton != 0)
            {
                client.mouseButtonHeldTime += 1;
            }
            else
            {
                client.mouseButtonHeldTime = TradeOrDuelHoldReset;
            }

            IncreaseHeldTradeAndDuelCounter();
        }

        private void UpdateMouseButtonClick()
        {
            if (client.lastMouseButton == 1)
            {
                client.mouseButtonClick = 1;
            }
            else if (client.lastMouseButton == 2)
            {
                client.mouseButtonClick = 2;
            }
        }

        private void UpdateVisibleMessageTimeouts()
        {
            if (client.messagesTab != MessageTabNone)
            {
                return;
            }

            for (int messageTabIndex = 0;
                messageTabIndex < VisibleMessageCount;
                messageTabIndex += 1)
            {
                if (client.messagesTimeout[messageTabIndex] > 0)
                {
                    client.messagesTimeout[messageTabIndex] -= 1;
                }
            }
        }
    }
}