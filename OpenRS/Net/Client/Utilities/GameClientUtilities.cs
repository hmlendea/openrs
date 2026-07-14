using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using NuciLog.Core;

using OpenRS.Audio;
using OpenRS.Models;
using OpenRS.Net.Client.Data;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Utilities
{
    public sealed class GameClientUtilities(GameClient client)
    {
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameClientUtilities>();

        private readonly Lock sync = new();
        private static bool isSendingPing;

        public int GetInventoryItemTotalCount(int itemId)
        {
            int totalCount = 0;

            for (int itemIndex = 0; itemIndex < client.inventoryItemsCount; itemIndex += 1)
            {
                if (client.inventoryItems[itemIndex] == itemId)
                {
                    if (client.entityManager.GetItem(itemId).IsStackable == 1)
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
                client.DisplayMessage("@cya@You can't logout during combat!", (int)MessageType.Game);
                return;
            }

            if (client.combatTimeout > 0)
            {
                client.DisplayMessage("@cya@You can't logout for 10 seconds after combat", (int)MessageType.Game);
                return;
            }

            client.streamClass.CreatePacket((int)ClientPacket.SendLogout);
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
                    logger.Error("The SendPingPacket call has failed.", exception);
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

        private void OnMyTaskCompleted(AsyncCompletedEventArgs eventArgs)
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
                client.DisplayMessage(message, (int)MessageType.GameLocal);
                return;
            }

            if (message.StartsWith("@que@"))
            {
                client.DisplayMessage("@whi@" + message, (int)MessageType.Quest);
                return;
            }

            if (message.StartsWith("@pri@"))
            {
                client.DisplayMessage(message, (int)MessageType.PrivateMessage);
                return;
            }

            client.DisplayMessage(message, (int)MessageType.Game);
        }

        public void CantLogout()
        {
            client.logoutTimer = 0;
            client.DisplayMessage("@cya@Sorry, you can't logout at the moment", (int)MessageType.Game);
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
            if (runeId == (int)RuneElement.Air && (client.IsItemEquipped((int)ElementalStaff.Air) || client.IsItemEquipped((int)ElementalBattlestaff.Air) || client.IsItemEquipped((int)ElementalMysticStaff.Air)))
            {
                return true;
            }

            if (runeId == (int)RuneElement.Water && (client.IsItemEquipped((int)ElementalStaff.Water) || client.IsItemEquipped((int)ElementalBattlestaff.Water) || client.IsItemEquipped((int)ElementalMysticStaff.Water)))
            {
                return true;
            }

            if (runeId == (int)RuneElement.Earth && (client.IsItemEquipped((int)ElementalStaff.Earth) || client.IsItemEquipped((int)ElementalBattlestaff.Earth) || client.IsItemEquipped((int)ElementalMysticStaff.Earth)))
            {
                return true;
            }

            if (runeId == (int)RuneElement.Fire && (client.IsItemEquipped((int)ElementalStaff.Fire) || client.IsItemEquipped((int)ElementalBattlestaff.Fire) || client.IsItemEquipped((int)ElementalMysticStaff.Fire)))
            {
                return true;
            }

            return client.GetInventoryItemTotalCount(runeId) >= requiredAmount;
        }

        public void DisplayMessage(string message, int messageType)
        {
            if (messageType == (int)MessageType.Chat || messageType == (int)MessageType.GameLocal || messageType == (int)MessageType.PrivateMessage)
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

            if (messageType == (int)MessageType.Chat)
            {
                message = "@yel@" + message;
            }

            if (messageType == (int)MessageType.Game || messageType == (int)MessageType.GameLocal)
            {
                message = "@whi@" + message;
            }

            if (messageType == (int)MessageType.PrivateMessage)
            {
                message = "@cya@" + message;
            }

            if (client.messagesTab != 0)
            {
                if (messageType == (int)MessageType.GameLocal || messageType == (int)MessageType.Game)
                {
                    client.chatTabAllMsgFlash = 200;
                }

                if (messageType == (int)MessageType.Chat && client.messagesTab != 1)
                {
                    client.chatTabHistoryFlash = 200;
                }

                if (messageType == (int)MessageType.Quest && client.messagesTab != 2)
                {
                    client.chatTabQuestFlash = 200;
                }

                if (messageType == (int)MessageType.PrivateMessage && client.messagesTab != 3)
                {
                    client.chatTabPrivateFlash = 200;
                }

                if (messageType == (int)MessageType.Game && client.messagesTab != 0)
                {
                    client.messagesTab = 0;
                }

                if (messageType == (int)MessageType.PrivateMessage && client.messagesTab != 3 && client.messagesTab != 0)
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

            if (messageType == (int)MessageType.Chat)
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

            if (messageType == (int)MessageType.Quest)
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

            if (messageType == (int)MessageType.PrivateMessage)
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
            if (!Config.MembersFeatures || client.configSoundOff)
            {
                return;
            }

            AudioManager.Instance.PlaySound(soundName);
        }

        private int GetUID() => Link.userId;

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
