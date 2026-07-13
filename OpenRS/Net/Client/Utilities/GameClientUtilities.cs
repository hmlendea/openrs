using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using OpenRS.Net.Client.Data;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Utilities
{
    public sealed class GameClientUtilities(GameClient client)
    {
        private readonly Lock sync = new();
        private static bool isSendingPing = false;

        public int GetInventoryItemTotalCount(int itemId)
        {
            int totalCount = 0;

            for (int itemIndex = 0; itemIndex < client.inventoryItemsCount; itemIndex += 1)
            {
                if (client.inventoryItems[itemIndex] == itemId)
                {
                    if (GameData.itemStackable[itemId] == 1)
                    {
                        totalCount += 1;
                    }
                    else
                    {
                        totalCount += client.inventoryItemCount[itemIndex];
                    }
                }
            }

            return totalCount;
        }
        public void SendLogout()
        {
            if (!client.loggedIn)
            {
                return;
            }

            if (client.combatTimeout > 450)
            {
                client.DisplayMessage("@cya@You can't logout during combat!", 3);
                return;
            }

            if (client.combatTimeout > 0)
            {
                client.DisplayMessage("@cya@You can't logout for 10 seconds after combat", 3);
                return;
            }

            client.streamClass.CreatePacket(129);
            client.streamClass.FormatPacket();
            client.logoutTimer = 1000;
            client.streamClass.CloseStream();
        }
        public bool IsItemEquipped(int itemId)
        {
            for (int inventorySlotIndex = 0; inventorySlotIndex < client.inventoryItemsCount; inventorySlotIndex += 1)
            {
                if (client.inventoryItems[inventorySlotIndex] == itemId && client.inventoryItemEquipped[inventorySlotIndex] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void SendPingPacketAsync()
        {
            lock (sync)
            {
                if (isSendingPing)
                {
                    return;
                }

                isSendingPing = true;
            }

            Task.Run(() =>
            {
                try
                {
                    client.CallSendPingPacket();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"[SendPingPacket EXCEPTION] {exception.GetType().Name}: {exception.Message}");
                    Console.WriteLine(exception.StackTrace);
                }
                finally
                {
                    lock (sync)
                    {
                        isSendingPing = false;
                    }

                    OnMyTaskCompleted(new AsyncCompletedEventArgs(null, false, null));
                }
            });
        }

        public event AsyncCompletedEventHandler MyTaskCompleted;

        protected void OnMyTaskCompleted(AsyncCompletedEventArgs eventArgs)
        {
            if (MyTaskCompleted is not null)
            {
                MyTaskCompleted(this, eventArgs);
            }
        }
        public void DisplayMessage(string message)
        {
            if (message.StartsWith("@bor@"))
            {
                client.DisplayMessage(message, 4);
                return;
            }

            if (message.StartsWith("@que@"))
            {
                client.DisplayMessage("@whi@" + message, 5);
                return;
            }

            if (message.StartsWith("@pri@"))
            {
                client.DisplayMessage(message, 6);
                return;
            }

            client.DisplayMessage(message, 3);
        }
        public void CantLogout()
        {
            client.logoutTimer = 0;
            client.DisplayMessage("@cya@Sorry, you can't logout at the moment", 3);
        }

        public GraphicsDevice GetGraphics() => GameClient.graphics;
        public static string FormatItemCount(int itemCount)
        {
            string formattedCount = itemCount.ToString();

            for (int separatorIndex = formattedCount.Length - 3; separatorIndex > 0; separatorIndex -= 3)
            {
                formattedCount = formattedCount.Substring(0, separatorIndex) + "," + formattedCount.Substring(separatorIndex);
            }

            if (formattedCount.Length > 8)
            {
                formattedCount = "@gre@" + formattedCount.Substring(0, formattedCount.Length - 8) + " million @whi@(" + formattedCount + ")";
            }
            else if (formattedCount.Length > 4)
            {
                formattedCount = "@cya@" + formattedCount.Substring(0, formattedCount.Length - 4) + "K @whi@(" + formattedCount + ")";
            }

            return formattedCount;
        }
        public bool HasRequiredRunes(int runeId, int requiredAmount)
        {
            if (runeId == 31 && (client.IsItemEquipped(197) || client.IsItemEquipped(615) || client.IsItemEquipped(682)))
            {
                return true;
            }

            if (runeId == 32 && (client.IsItemEquipped(102) || client.IsItemEquipped(616) || client.IsItemEquipped(683)))
            {
                return true;
            }

            if (runeId == 33 && (client.IsItemEquipped(101) || client.IsItemEquipped(617) || client.IsItemEquipped(684)))
            {
                return true;
            }

            if (runeId == 34 && (client.IsItemEquipped(103) || client.IsItemEquipped(618) || client.IsItemEquipped(685)))
            {
                return true;
            }

            return client.GetInventoryItemTotalCount(runeId) >= requiredAmount;
        }
        public void DisplayMessage(string message, int messageType)
        {
            if (messageType == 2 || messageType == 4 || messageType == 6)
            {
                while (message.Length > 5 && message[0] == '@' && message[4] == '@')
                {
                    message = message.Substring(5);
                }

                int colonIndex = message.IndexOf(":");

                if (colonIndex != -1)
                {
                    string senderName = message.Substring(0, colonIndex);
                    long senderNameHash = DataOperations.NameToHash(senderName);

                    for (int ignoreIndex = 0; ignoreIndex < client.ignoresCount; ignoreIndex += 1)
                    {
                        if (client.ignoresList[ignoreIndex] == senderNameHash)
                        {
                            return;
                        }
                    }
                }
            }

            if (messageType == 2)
            {
                message = "@yel@" + message;
            }

            if (messageType == 3 || messageType == 4)
            {
                message = "@whi@" + message;
            }

            if (messageType == 6)
            {
                message = "@cya@" + message;
            }

            if (client.messagesTab != 0)
            {
                if (messageType == 4 || messageType == 3)
                {
                    client.chatTabAllMsgFlash = 200;
                }

                if (messageType == 2 && client.messagesTab != 1)
                {
                    client.chatTabHistoryFlash = 200;
                }

                if (messageType == 5 && client.messagesTab != 2)
                {
                    client.chatTabQuestFlash = 200;
                }

                if (messageType == 6 && client.messagesTab != 3)
                {
                    client.chatTabPrivateFlash = 200;
                }

                if (messageType == 3 && client.messagesTab != 0)
                {
                    client.messagesTab = 0;
                }

                if (messageType == 6 && client.messagesTab != 3 && client.messagesTab != 0)
                {
                    client.messagesTab = 0;
                }
            }

            for (int messageIndex = 4; messageIndex > 0; messageIndex -= 1)
            {
                client.messagesArray[messageIndex] = client.messagesArray[messageIndex - 1];
                client.messagesTimeout[messageIndex] = client.messagesTimeout[messageIndex - 1];
            }

            client.messagesArray[0] = message;
            client.messagesTimeout[0] = 300;

            if (messageType == 2)
            {
                if (client.chatInputMenu.listShownEntries[client.messagesHandleType2] == client.chatInputMenu.listLength[client.messagesHandleType2] - 4)
                {
                    client.chatInputMenu.AddMessage(client.messagesHandleType2, message, true);
                }
                else
                {
                    client.chatInputMenu.AddMessage(client.messagesHandleType2, message, false);
                }
            }

            if (messageType == 5)
            {
                if (client.chatInputMenu.listShownEntries[client.messagesHandleType5] == client.chatInputMenu.listLength[client.messagesHandleType5] - 4)
                {
                    client.chatInputMenu.AddMessage(client.messagesHandleType5, message, true);
                }
                else
                {
                    client.chatInputMenu.AddMessage(client.messagesHandleType5, message, false);
                }
            }

            if (messageType == 6)
            {
                if (client.chatInputMenu.listShownEntries[client.messagesHandleType6] == client.chatInputMenu.listLength[client.messagesHandleType6] - 4)
                {
                    client.chatInputMenu.AddMessage(client.messagesHandleType6, message, true);
                    return;
                }

                client.chatInputMenu.AddMessage(client.messagesHandleType6, message, false);
            }
        }
        public void PlaySound(string soundName)
        {
            if (client.audioPlayer is null || !Config.MembersFeatures)
            {
                return;
            }

            if (!client.configSoundOff)
            {
                int soundOffset = (int)DataOperations.GetObjectOffset(soundName + ".pcm", client.soundData);
                int soundLength = DataOperations.GetSoundLength(soundName + ".pcm", client.soundData);
                client.audioPlayer.Play(client.soundData, soundOffset, soundLength);
            }
        }

        protected int GetUID() => Link.userId;

        public bool TakeScreenshot(bool isVerbose) => true;

        public string JoinString(string[] parts, string separator, int startIndex)
        {
            string result = "";

            for (int partIndex = startIndex; partIndex < parts.Length; partIndex += 1)
            {
                result += parts[partIndex];

                if (partIndex != parts.Length - 1)
                {
                    result += separator;
                }
            }

            return result;
        }

        public string JoinString(string[] parts, string separator) => client.JoinString(parts, separator, 0);

    }

}
