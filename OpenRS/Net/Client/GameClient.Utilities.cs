using System;
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


namespace OpenRS.Net.Client
{
    public sealed partial class GameClient
    {
        public int GetInventoryItemTotalCount(int itemId)
        {
            int l = 0;
            for (int i1 = 0; i1 < inventoryItemsCount; i1 += 1)
            {
                if (inventoryItems[i1] == itemId)
                {
                    if (GameData.itemStackable[itemId] == 1)
                    {
                        l += 1;
                    }
                    else
                    {
                        l += inventoryItemCount[i1];
                    }
                }
            }

            return l;
        }
        public void SendLogout()
        {
            if (!loggedIn)
            {
                return;
            }

            if (combatTimeout > 450)
            {
                DisplayMessage("@cya@You can't logout during combat!", 3);
                return;
            }
            if (combatTimeout > 0)
            {
                DisplayMessage("@cya@You can't logout for 10 seconds after combat", 3);
                return;
            }
            else
            {
                streamClass.CreatePacket(129);
                streamClass.FormatPacket();
                logoutTimer = 1000;

                streamClass.CloseStream();
                return;
            }
        }
        public bool IsItemEquipped(int itemId)
        {
            for (int l = 0; l < inventoryItemsCount; l += 1)
            {
                if (inventoryItems[l] == itemId && inventoryItemEquipped[l] == 1)
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
                    SendPingPacket();
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
        public override void DisplayMessage(string s1)
        {
            if (s1.StartsWith("@bor@"))
            {
                DisplayMessage(s1, 4);
                return;
            }
            if (s1.StartsWith("@que@"))
            {
                DisplayMessage("@whi@" + s1, 5);
                return;
            }
            if (s1.StartsWith("@pri@"))
            {
                DisplayMessage(s1, 6);
                return;
            }
            else
            {
                DisplayMessage(s1, 3);
                return;
            }
        }
        public override void CantLogout()
        {
            logoutTimer = 0;
            DisplayMessage("@cya@Sorry, you can't logout at the moment", 3);
        }
        public GraphicsDevice GetGraphics()
        {
            //if(GameApplet.gameFrame is not null)
            //    return GameApplet.gameFrame.GetGraphics();
            //if(Link.gameApplet is not null)
            //    return Link.gameApplet.GetGraphics();
            //else
            //    return base.GetGraphics();
            return graphics;
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
            if (l == 31 && (IsItemEquipped(197) || IsItemEquipped(615) || IsItemEquipped(682)))
            {
                return true;
            }

            if (l == 32 && (IsItemEquipped(102) || IsItemEquipped(616) || IsItemEquipped(683)))
            {
                return true;
            }

            if (l == 33 && (IsItemEquipped(101) || IsItemEquipped(617) || IsItemEquipped(684)))
            {
                return true;
            }

            if (l == 34 && (IsItemEquipped(103) || IsItemEquipped(618) || IsItemEquipped(685)))
            {
                return true;
            }

            return GetInventoryItemTotalCount(l) >= i1;
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
                    for (int j1 = 0; j1 < ignoresCount; j1 += 1)
                    {
                        if (ignoresList[j1] == l1)
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

            if (messagesTab != 0)
            {
                if (type == 4 || type == 3)
                {
                    chatTabAllMsgFlash = 200;
                }

                if (type == 2 && messagesTab != 1)
                {
                    chatTabHistoryFlash = 200;
                }

                if (type == 5 && messagesTab != 2)
                {
                    chatTabQuestFlash = 200;
                }

                if (type == 6 && messagesTab != 3)
                {
                    chatTabPrivateFlash = 200;
                }

                if (type == 3 && messagesTab != 0)
                {
                    messagesTab = 0;
                }

                if (type == 6 && messagesTab != 3 && messagesTab != 0)
                {
                    messagesTab = 0;
                }
            }
            for (int i1 = 4; i1 > 0; i1 -= 1)
            {
                messagesArray[i1] = messagesArray[i1 - 1];
                messagesTimeout[i1] = messagesTimeout[i1 - 1];
            }

            messagesArray[0] = message;
            messagesTimeout[0] = 300;
            if (type == 2)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType2] == chatInputMenu.listLength[messagesHandleType2] - 4)
                {
                    chatInputMenu.AddMessage(messagesHandleType2, message, true);
                }
                else
                {
                    chatInputMenu.AddMessage(messagesHandleType2, message, false);
                }
            }

            if (type == 5)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType5] == chatInputMenu.listLength[messagesHandleType5] - 4)
                {
                    chatInputMenu.AddMessage(messagesHandleType5, message, true);
                }
                else
                {
                    chatInputMenu.AddMessage(messagesHandleType5, message, false);
                }
            }

            if (type == 6)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType6] == chatInputMenu.listLength[messagesHandleType6] - 4)
                {
                    chatInputMenu.AddMessage(messagesHandleType6, message, true);
                    return;
                }
                chatInputMenu.AddMessage(messagesHandleType6, message, false);
            }
        }
        public void PlaySound(string s1)
        {
            if (audioPlayer is null || !Config.MembersFeatures)
            {
                return;
            }

            if (!configSoundOff)
            {
                int off = (int)DataOperations.GetObjectOffset(s1 + ".pcm", soundData);
                int len = DataOperations.GetSoundLength(s1 + ".pcm", soundData);
                audioPlayer.Play(soundData, off, len);
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
            return JoinString(hay, glue, 0);
        }
    }

}
