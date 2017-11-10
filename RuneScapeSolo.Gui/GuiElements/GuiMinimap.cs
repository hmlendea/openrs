using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RuneScapeSolo.DataAccess.Resources;
using RuneScapeSolo.GameLogic.GameManagers;
using RuneScapeSolo.Graphics;
using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Graphics.Primitives.Mapping;
using RuneScapeSolo.Net.Client;
using RuneScapeSolo.Net.Client.Game;
using RuneScapeSolo.Net.Client.Game.Cameras;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiMinimap : GuiElement
    {
        GameClient client;

        GuiMinimapIndicator compassIndicator;
        GuiMinimapIndicator healthIndicator;
        GuiMinimapIndicator staminaIndicator;
        GuiMinimapIndicator prayerIndicator;

        byte[,] alphaMask;

        Sprite mobDot;
        Sprite pixel;
        Sprite frame;

        public bool IsClickable { get; set; }

        public GuiMinimap()
        {
            IsClickable = true;
        }

        public override void LoadContent()
        {
            mobDot = new Sprite { ContentFile = "Interface/Minimap/entity_dot" };
            pixel = new Sprite { ContentFile = "ScreenManager/FillImage" };
            frame = new Sprite { ContentFile = "Interface/Minimap/frame" };

            compassIndicator = new GuiMinimapIndicator { BackgroundColour = Colour.Bisque };
            healthIndicator = new GuiMinimapIndicator { BackgroundColour = Colour.PersianRed };
            staminaIndicator = new GuiMinimapIndicator { BackgroundColour = Colour.OliveDrab };
            prayerIndicator = new GuiMinimapIndicator { BackgroundColour = Colour.CornflowerBlue };

            Texture2D maskTexture = ResourceManager.Instance.LoadTexture2D("Interface/Minimap/mask");
            Color[] maskBits = new Color[maskTexture.Width * maskTexture.Height];
            maskTexture.GetData(maskBits, 0, maskBits.Length);

            alphaMask = new byte[Size.Width, Size.Height];

            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    int i = x + y * Size.Width;

                    alphaMask[x, y] = maskBits[i].R;
                }
            }

            mobDot.LoadContent();
            pixel.LoadContent();
            frame.LoadContent();

            Children.Add(compassIndicator);
            Children.Add(healthIndicator);
            Children.Add(staminaIndicator);
            Children.Add(prayerIndicator);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            mobDot.Update(gameTime);
            pixel.Update(gameTime);
            frame.Update(gameTime);

            if (client.loggedIn)
            {
                UpdateIndicators();
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawMinimapMenu(spriteBatch);
            frame.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        public void AssociateGameClient(ref GameClient client)
        {
            this.client = client;
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            frame.Location = Location;

            compassIndicator.Location = new Point2D(Location.X + 40, Location.Y + 9);
            healthIndicator.Location = new Point2D(Location.X + 17, Location.Y + 36);
            staminaIndicator.Location = new Point2D(Location.X + 162, Location.Y + 146);
            prayerIndicator.Location = new Point2D(Location.X + 10, Location.Y + 72);
        }

        void UpdateIndicators()
        {
            healthIndicator.BaseValue = client.PlayerHealthBase;
            healthIndicator.CurrentValue = client.PlayerHealthCurrent;

            staminaIndicator.BaseValue = 100;
            staminaIndicator.CurrentValue = staminaIndicator.BaseValue - client.PlayerFatigue;

            prayerIndicator.BaseValue = client.PlayerPrayerBase;
            prayerIndicator.CurrentValue = client.PlayerPrayerCurrent;
        }

        void DrawMinimapMenu(SpriteBatch spriteBatch)
        {
            if (client.gameGraphics == null || !client.loggedIn)
            {
                return; // TODO: Remove this ugly fix
            }

            int c1 = 156;//'æ';//(char)234;//'\u234';
            int c3 = 152;// '~';//(char)230;//'\u230';

            int j1 = 192 + client.minimapRandomRotationY;
            int l1 = client.cameraRotation + client.minimapRandomRotationX & 0xff;
            int j5 = Camera.bbk[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.bbk[(1024 - l1 * 4 & 0x3ff) + 1024];

            DrawMinimapTiles(spriteBatch);

            for (int i = 0; i < client.GroundItemCount; i++)
            {
                int groundItemX = (((client.GroundItemX[i] * client.GridSize + 64) - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int groundItemY = (((client.GroundItemY[i] * client.GridSize + 64) - client.CurrentPlayer.currentY) * 3 * j1) / 2048;
                int l6 = groundItemY * j5 + groundItemX * l5 >> 18;
                groundItemY = groundItemY * l5 - groundItemX * j5 >> 18;
                groundItemX = l6;

                int groundItemMapX = Location.X + c1 / 2 + groundItemX;
                int groundItemMapY = Location.Y + (36 + c3 / 2) - groundItemY;

                DrawMinimapObject(spriteBatch, groundItemMapX, groundItemMapY, Colour.Red);
            }


            foreach (Mob npc in client.Npcs.Where(x => x != null))
            {
                int npcPosX = ((npc.currentX - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int npcPosY = ((npc.currentY - client.CurrentPlayer.currentY) * 3 * j1) / 2048;

                int i7 = npcPosY * j5 + npcPosX * l5 >> 18;
                npcPosY = npcPosY * l5 - npcPosX * j5 >> 18;
                npcPosX = i7;

                int dotX = Location.X + c1 / 2 + npcPosX;
                int dotY = Location.Y + (36 + c3 / 2) - npcPosY;

                DrawMinimapObject(spriteBatch, dotX, dotY, Colour.Yellow);
            }

            foreach (Mob player in client.Players.Where(x => x != null))
            {
                int playerPosX = ((player.currentX - client.CurrentPlayer.currentX) * 3 * j1) / 2048;
                int playerPosY = ((player.currentY - client.CurrentPlayer.currentY) * 3 * j1) / 2048;

                int j7 = playerPosY * j5 + playerPosX * l5 >> 18;
                playerPosY = playerPosY * l5 - playerPosX * j5 >> 18;
                playerPosX = j7;

                Colour dotColour = Colour.White;
                for (int j = 0; j < client.friendsCount; j++)
                {
                    if (player.NameHash != client.friendsList[j] || client.friendsWorld[j] != 99)
                    {
                        continue;
                    }

                    dotColour = Colour.Red;
                    break;
                }

                int dotX = Location.X + c1 / 2 + playerPosX;
                int dotY = Location.Y + (36 + c3 / 2) - playerPosY;

                DrawMinimapObject(spriteBatch, dotX, dotY, dotColour);
            }
        }

        void DrawMinimapTiles(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    Colour tileColour = Colour.Black;
                    int alpha = tileColour.A - 255 + alphaMask[x, y];

                    pixel.Location = new Point2D(Location.X + x, Location.Y + y);
                    pixel.Tint = Color.FromNonPremultiplied(tileColour.R, tileColour.G, tileColour.B, alpha).ToColour();

                    pixel.Draw(spriteBatch);
                }
            }
        }

        void DrawMinimapObject(SpriteBatch spriteBatch, int x, int y, Colour colour)
        {
            if (x < ClientRectangle.Left || x >= ClientRectangle.Right ||
                y < ClientRectangle.Top || y >= ClientRectangle.Bottom)
            {
                return;
            }

            mobDot.Tint = colour;
            mobDot.Opacity = alphaMask[x - Location.X, y - Location.Y];
            mobDot.Location = new Point2D(
                x - mobDot.SpriteSize.Width / 2,
                y - mobDot.SpriteSize.Height / 2);

            mobDot.Draw(spriteBatch);
        }
    }
}
