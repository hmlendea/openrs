using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using RuneScapeSolo.Input;
using RuneScapeSolo.Net.Client;
using RuneScapeSolo.Net.Client.Game;
using RuneScapeSolo.Net.Client.Game.Cameras;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiMinimap : GuiElement
    {
        GameClient client;
        
        public bool IsClickable { get; set; }

        public GuiMinimap()
        {
            IsClickable = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawMinimapMenu();

            base.Draw(spriteBatch);
        }

        public void AssociateGameClient(ref GameClient client)
        {
            this.client = client;
        }

        public void DrawMinimapMenu()
        {
            if (client.gameGraphics == null || !client.loggedIn)
            {
                return; // TODO: Remove this ugly fix
            }

            int l = client.gameGraphics.gameWidth - 199;
            int c1 = 156;//'æ';//(char)234;//'\u234';
            int c3 = 152;// '~';//(char)230;//'\u230';
            client.gameGraphics.drawPicture(l - 49, 3, client.baseInventoryPic + 2);
            l += 40;
            client.gameGraphics.drawBox(l, 36, c1, c3, 0);
            client.gameGraphics.setDimensions(l, 36, l + c1, 36 + c3);
            int j1 = 192 + client.minimapRandomRotationY;
            int l1 = client.cameraRotation + client.minimapRandomRotationX & 0xff;
            int j2 = ((client.CurrentPlayer.currentX - 6040) * 3 * j1) / 2048;
            int l3 = ((client.CurrentPlayer.currentY - 6040) * 3 * j1) / 2048;
            int j5 = Camera.bbk[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.bbk[(1024 - l1 * 4 & 0x3ff) + 1024];
            int j6 = l3 * j5 + j2 * l5 >> 18;
            l3 = l3 * l5 - j2 * j5 >> 18;
            j2 = j6;
            client.gameGraphics.drawMinimapPic((l + c1 / 2) - j2, 36 + c3 / 2 + l3, client.baseInventoryPic - 1, l1 + 64 & 0xff, j1);

            for (int i = 0; i < client.ObjectCount; i++)
            {
                int objectX = (((client.ObjectX[i] * client.GridSize + 64) - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int objectY = (((client.ObjectY[i] * client.GridSize + 64) - client.CurrentPlayer.currentY) * 3 * j1) / 2048;
                int k6 = objectY * j5 + objectX * l5 >> 18;
                objectY = objectY * l5 - objectX * j5 >> 18;
                objectX = k6;
                DrawMinimapObject(l + c1 / 2 + objectX, (36 + c3 / 2) - objectY, 65535);
            }

            for (int i = 0; i < client.GroundItemCount; i++)
            {
                int groundItemX = (((client.GroundItemX[i] * client.GridSize + 64) - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int groundItemY = (((client.GroundItemY[i] * client.GridSize + 64) - client.CurrentPlayer.currentY) * 3 * j1) / 2048;
                int l6 = groundItemY * j5 + groundItemX * l5 >> 18;
                groundItemY = groundItemY * l5 - groundItemX * j5 >> 18;
                groundItemX = l6;
                DrawMinimapObject(l + c1 / 2 + groundItemX, (36 + c3 / 2) - groundItemY, 0xff0000);
            }

            for (int i = 0; i < client.NpcCount; i++)
            {
                Mob mob = client.Npcs[i];

                int mobPosX = ((mob.currentX - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int mobPosY = ((mob.currentY - client.CurrentPlayer.currentY) * 3 * j1) / 2048;

                int i7 = mobPosY * j5 + mobPosX * l5 >> 18;
                mobPosY = mobPosY * l5 - mobPosX * j5 >> 18;
                mobPosX = i7;

                DrawMinimapObject(l + c1 / 2 + mobPosX, (36 + c3 / 2) - mobPosY, 0xffff00);
            }

            for (int i = 0; i < client.PlayerCount; i++)
            {
                Mob player = client.Players[i];
                int playerPosX = ((player.currentX - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int playerPosY = ((player.currentY - client.CurrentPlayer.currentY) * 3 * j1) / 2048;

                int j7 = playerPosY * j5 + playerPosX * l5 >> 18;
                playerPosY = playerPosY * l5 - playerPosX * j5 >> 18;
                playerPosX = j7;

                int i9 = 0xffffff;
                for (int j = 0; j < client.friendsCount; j++)
                {
                    if (player.NameHash != client.friendsList[j] || client.friendsWorld[j] != 99)
                    {
                        continue;
                    }

                    i9 = 65280;
                    break;
                }

                DrawMinimapObject(l + c1 / 2 + playerPosX, (36 + c3 / 2) - playerPosY, i9);
            }

            // compass
            client.gameGraphics.drawCircle(l + c1 / 2, 36 + c3 / 2, 2, 0xffffff, 255);
            client.gameGraphics.drawMinimapPic(l + 19, 55, client.baseInventoryPic + 24, client.cameraRotation + 128 & 0xff, 128);
            client.gameGraphics.setDimensions(0, 0, client.windowWidth, client.windowHeight + 12);

            if (!IsClickable)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (client.gameGraphics.gameWidth - 199);
            int l8 = InputManager.Instance.MouseLocation.Y - 36;

            if (l >= 40 && l8 >= 0 && l < 196 && l8 < 152)
            {
                int c2 = 156;//'\u234';
                int c4 = 152;//'\u230';
                int k1 = 192 + client.minimapRandomRotationY;
                int i2 = client.cameraRotation + client.minimapRandomRotationX & 0xff;
                int i1 = client.gameGraphics.gameWidth - 199;
                i1 += 40;
                int k3 = ((InputManager.Instance.MouseLocation.X - (i1 + c2 / 2)) * 16384) / (3 * k1);
                int i5 = ((InputManager.Instance.MouseLocation.Y - (36 + c4 / 2)) * 16384) / (3 * k1);
                int k5 = Camera.bbk[1024 - i2 * 4 & 0x3ff];
                int i6 = Camera.bbk[(1024 - i2 * 4 & 0x3ff) + 1024];
                int k7 = i5 * k5 + k3 * i6 >> 15;
                i5 = i5 * i6 - k3 * k5 >> 15;
                k3 = k7;
                k3 += client.CurrentPlayer.currentX;
                i5 = client.CurrentPlayer.currentY - i5;

                if (client.mouseButtonClick == 1)
                {
                    client.walkTo1Tile(client.SectionX, client.SectionY, k3 / 128, i5 / 128, false);
                }

                client.mouseButtonClick = 0;
            }
        }

        void DrawMinimapObject(int x, int y, int color)
        {
            client.gameGraphics.drawMinimapPixel(x, y, color);
            client.gameGraphics.drawMinimapPixel(x - 1, y, color);
            client.gameGraphics.drawMinimapPixel(x + 1, y, color);
            client.gameGraphics.drawMinimapPixel(x, y - 1, color);
            client.gameGraphics.drawMinimapPixel(x, y + 1, color);
        }
    }
}
