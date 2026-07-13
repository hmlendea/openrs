using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using OpenRS.Net.Client.Events;
using OpenRS.Settings;
using System.Threading;
using System;

namespace OpenRS.Net.Client
{
    public sealed class GameClientUtilities(GameClient client)
    {

        public int GetInventoryItemTotalCount(int itemId)
        {
            int l = 0;
            for (int i1 = 0; i1 < client.inventoryItemsCount; i1 += 1)
            {
                if (client.inventoryItems[i1] == itemId)
                {
                    if (GameData.itemStackable[itemId] == 1)
                    {
                        l += 1;
                    }
                    else
                    {
                        l += client.inventoryItemCount[i1];
                    }
                }
            }

            return l;
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
            else
            {
                client.streamClass.CreatePacket(129);
                client.streamClass.FormatPacket();
                client.logoutTimer = 1000;

                client.streamClass.CloseStream();
                return;
            }
        }
        public bool IsItemEquipped(int itemId)
        {
            for (int l = 0; l < client.inventoryItemsCount; l += 1)
            {
                if (client.inventoryItems[l] == itemId && client.inventoryItemEquipped[l] == 1)
                {
                    return true;
                }
            }

            return false;
        }
        private readonly Lock _sync = new();
        public static bool sendingPing = false;
        public void SendPingPacketAsync()
        {
            lock (_sync)
            {
                if (sendingPing)
                {
                    return;
                }

                sendingPing = true;
            }

            Task.Run(() =>
            {
                try
                {
                    client.CallSendPingPacket();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SendPingPacket EXCEPTION] {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    lock (_sync)
                    {
                        sendingPing = false;
                    }
                    OnMyTaskCompleted(new AsyncCompletedEventArgs(null, false, null));
                }
            });
        }
        public event AsyncCompletedEventHandler MyTaskCompleted;
        protected void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted is not null)
            {
                MyTaskCompleted(this, e);
            }
        }
        public void DisplayMessage(string s1)
        {
            if (s1.StartsWith("@bor@"))
            {
                client.DisplayMessage(s1, 4);
                return;
            }
            if (s1.StartsWith("@que@"))
            {
                client.DisplayMessage("@whi@" + s1, 5);
                return;
            }
            if (s1.StartsWith("@pri@"))
            {
                client.DisplayMessage(s1, 6);
                return;
            }
            else
            {
                client.DisplayMessage(s1, 3);
                return;
            }
        }
        public void CantLogout()
        {
            client.logoutTimer = 0;
            client.DisplayMessage("@cya@Sorry, you can't logout at the moment", 3);
        }
        public GraphicsDevice GetGraphics()
        {
            //if(GameApplet.gameFrame is not null)
            //    return GameApplet.gameFrame.GetGraphics();
            //if(Link.gameApplet is not null)
            //    return Link.gameApplet.GetGraphics();
            //else
            //    return base.GetGraphics();
            return GameClient.graphics;
        }
        public static string formatItemCount(int itemCount)
        {
            string s1 = itemCount.ToString();
            for (int l = s1.Length - 3; l > 0; l -= 3)
            {
                s1 = s1.Substring(0, l) + "," + s1.Substring(l);
            }

            if (s1.Length > 8)
            {
                s1 = "@gre@" + s1.Substring(0, s1.Length - 8) + " million @whi@(" + s1 + ")";
            }
            else
                if (s1.Length > 4)
            {
                s1 = "@cya@" + s1.Substring(0, s1.Length - 4) + "K @whi@(" + s1 + ")";
            }

            return s1;
        }
        public bool HasRequiredRunes(int l, int i1)
        {
            if (l == 31 && (client.IsItemEquipped(197) || client.IsItemEquipped(615) || client.IsItemEquipped(682)))
            {
                return true;
            }

            if (l == 32 && (client.IsItemEquipped(102) || client.IsItemEquipped(616) || client.IsItemEquipped(683)))
            {
                return true;
            }

            if (l == 33 && (client.IsItemEquipped(101) || client.IsItemEquipped(617) || client.IsItemEquipped(684)))
            {
                return true;
            }

            if (l == 34 && (client.IsItemEquipped(103) || client.IsItemEquipped(618) || client.IsItemEquipped(685)))
            {
                return true;
            }

            return client.GetInventoryItemTotalCount(l) >= i1;
        }
        public void DisplayMessage(string message, int type)
        {
            if (type == 2 || type == 4 || type == 6)
            {
                for (; message.Length > 5 && message[0] == '@' && message[4] == '@'; message = message.Substring(5))
                {
                    ;
                }

                int l = message.IndexOf(":");
                if (l != -1)
                {
                    string s1 = message.Substring(0, l);
                    long l1 = DataOperations.NameToHash(s1);
                    for (int j1 = 0; j1 < client.ignoresCount; j1 += 1)
                    {
                        if (client.ignoresList[j1] == l1)
                        {
                            return;
                        }
                    }
                }
            }
            if (type == 2)
            {
                message = "@yel@" + message;
            }

            if (type == 3 || type == 4)
            {
                message = "@whi@" + message;
            }

            if (type == 6)
            {
                message = "@cya@" + message;
            }

            if (client.messagesTab != 0)
            {
                if (type == 4 || type == 3)
                {
                    client.chatTabAllMsgFlash = 200;
                }

                if (type == 2 && client.messagesTab != 1)
                {
                    client.chatTabHistoryFlash = 200;
                }

                if (type == 5 && client.messagesTab != 2)
                {
                    client.chatTabQuestFlash = 200;
                }

                if (type == 6 && client.messagesTab != 3)
                {
                    client.chatTabPrivateFlash = 200;
                }

                if (type == 3 && client.messagesTab != 0)
                {
                    client.messagesTab = 0;
                }

                if (type == 6 && client.messagesTab != 3 && client.messagesTab != 0)
                {
                    client.messagesTab = 0;
                }
            }
            for (int i1 = 4; i1 > 0; i1 -= 1)
            {
                client.messagesArray[i1] = client.messagesArray[i1 - 1];
                client.messagesTimeout[i1] = client.messagesTimeout[i1 - 1];
            }

            client.messagesArray[0] = message;
            client.messagesTimeout[0] = 300;
            if (type == 2)
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

            if (type == 5)
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

            if (type == 6)
            {
                if (client.chatInputMenu.listShownEntries[client.messagesHandleType6] == client.chatInputMenu.listLength[client.messagesHandleType6] - 4)
                {
                    client.chatInputMenu.AddMessage(client.messagesHandleType6, message, true);
                    return;
                }
                client.chatInputMenu.AddMessage(client.messagesHandleType6, message, false);
            }
        }
        public void PlaySound(string s1)
        {
            if (client.audioPlayer is null || !Config.MembersFeatures)
            {
                return;
            }

            if (!client.configSoundOff)
            {
                int off = (int)DataOperations.GetObjectOffset(s1 + ".pcm", client.soundData);
                int len = DataOperations.GetSoundLength(s1 + ".pcm", client.soundData);
                client.audioPlayer.Play(client.soundData, off, len);
            }
        }
        protected int GetUID()
        {
            return Link.userId;
        }
        public bool TakeScreenshot(bool verb)
        {
            //try
            //{
            //    string charName = DataOperations.hashToName(DataOperations.nameToHash(username));
            //    File dir = new File(Config.MEDIA_DIR + "/" + charName);
            //    if (!dir.exists() || !dir.isDirectory())
            //        dir.mkdir();
            //    string folder = dir.getPath() + "/";
            //    File file = null;
            //    for (int count = 0; file is null || file.exists(); count += 1)
            //        file = new File(folder + "screenshot" + count + ".png");
            //    BufferedImage bi = new BufferedImage(windowWidth, windowHeight + 11, BufferedImage.TYPE_INT_RGB);
            //    Graphics2D g2d = bi.createGraphics();
            //    g2d.DrawImage(gameGraphics.image, 0, 0, this);
            //    g2d.dispose();
            //    ImageIO.write(bi, "png", file);
            //    if (verb)
            //        DisplayMessage("Screenshot saved as " + file.getName());
            //    return true;
            //}
            //catch (IOException ioe)
            //{
            //    if (verb)
            //        DisplayMessage("Error saving screenshot");
            //    return false;
            //}
            return true;
        }
        public string JoinString(string[] hay, string glue, int start)
        {
            string ret = "";

            for (int i = start; i < hay.Length; i += 1)
            {
                ret += hay[i];

                if (i != hay.Length - 1)
                {
                    ret += glue;
                }
            }

            return ret;
        }
        public string JoinString(string[] hay, string glue)
        {
            return client.JoinString(hay, glue, 0);
        }

    }

}
