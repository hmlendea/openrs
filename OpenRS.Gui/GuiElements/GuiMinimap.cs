using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Resources;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.GuiElements;
using NuciXNA.Input;
using NuciXNA.Primitives;
using NuciXNA.Primitives.Mapping;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Gui.GuiElements
{
    public class GuiMinimap : GuiElement
    {
        GameClient client;

        GuiMinimapIndicator compassIndicator;
        GuiMinimapIndicator healthIndicator;
        GuiMinimapIndicator staminaIndicator;
        GuiMinimapIndicator prayerIndicator;

        byte[,] alphaMask;

        TextureSprite dot;
        TextureSprite pixel;
        TextureSprite frame;

        public bool IsClickable { get; set; }

        public int ZoomLevel { get; set; }

        public GuiMinimap()
        {
            IsClickable = true;
            ZoomLevel = 2;
        }

        public override void LoadContent()
        {
            dot = new TextureSprite
            {
                ContentFile = "Interface/Minimap/entity_dot"
            };
            pixel = new TextureSprite
            {
                ContentFile = "ScreenManager/FillImage"
            };
            frame = new TextureSprite {
                ContentFile = "Interface/Minimap/frame"
            };

            compassIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.Bisque,
                Icon = "Interface/Minimap/icon_compass"
            };
            healthIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.PersianRed,
                Icon = "Interface/Minimap/icon_health"
            };
            staminaIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.OliveDrab,
                Icon = "Interface/Minimap/icon_stamina"
            };
            prayerIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.CornflowerBlue,
                Icon = "Interface/Minimap/icon_prayer"
            };

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

            dot.LoadContent();
            pixel.LoadContent();
            frame.LoadContent();

            AddChild(compassIndicator);
            AddChild(healthIndicator);
            AddChild(staminaIndicator);
            AddChild(prayerIndicator);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            dot.Update(gameTime);
            pixel.Update(gameTime);
            frame.Update(gameTime);

            compassIndicator.IconRotation = 1;

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

            if (client == null || !client.loggedIn)
            {
                return; // TODO: Ugly fix
            }

            // The cameraRotation is expressed in a non-standard manner. 64 = -90 degrees, 32 = -45 degrees, etc...
            // so we have to convert it to degrees by multiplying it with 1.4025, add 180 degrees (flip),
            // and then convert that to radians in order to use them to rotate the image
            compassIndicator.IconRotation = (float)(Math.PI / 180) * (client.cameraRotation * 1.4025f + 180);

            healthIndicator.BaseValue = client.Skills[3].BaseLevel;
            healthIndicator.CurrentValue = client.Skills[3].CurrentLevel;

            staminaIndicator.BaseValue = 100;
            staminaIndicator.CurrentValue = staminaIndicator.BaseValue - client.PlayerFatigue;

            prayerIndicator.BaseValue = client.Skills[5].BaseLevel;
            prayerIndicator.CurrentValue = client.Skills[5].CurrentLevel;
        }

        protected override void RegisterEvents()
        {
            compassIndicator.Clicked += OnCompassIndicatorClicked;
        }

        protected override void UnregisterEvents()
        {
            compassIndicator.Clicked -= OnCompassIndicatorClicked;
        }

        void DrawMinimapMenu(SpriteBatch spriteBatch)
        {
            if (client.gameGraphics == null || !client.loggedIn)
            {
                return; // TODO: Remove this ugly fix
            }

            int j1 = 192 + client.minimapRandomRotationY;
            int l1 = client.cameraRotation + client.minimapRandomRotationX & 0xff;
            int j5 = Camera.bbk[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.bbk[(1024 - l1 * 4 & 0x3ff) + 1024];

            DrawMinimapTiles(spriteBatch);

            DrawGroundItemDots(spriteBatch, j1, l1, j5, l5);
            DrawNpcDots(spriteBatch, j1, l1, j5, l5);
            DrawPlayerDots(spriteBatch, j1, l1, j5, l5);
        }
        
        void DrawGroundItemDots(SpriteBatch spriteBatch, int j1, int l1, int j5, int l5)
        {
            for (int groundItemIndex = 0; groundItemIndex < client.GroundItemCount; groundItemIndex++)
            {
                Point2D groundItemLocation = new Point2D(
                    (((client.GroundItemLocations[groundItemIndex].X * client.GridSize + 64) - client.CurrentPlayer.Location.X) * 3 * j1) / 2048,
                    (((client.GroundItemLocations[groundItemIndex].Y * client.GridSize + 64) - client.CurrentPlayer.Location.Y) * 3 * j1) / 2048);

                int l6 = groundItemLocation.Y * j5 + groundItemLocation.X * l5 >> 18;
                groundItemLocation.Y = groundItemLocation.Y * l5 - groundItemLocation.X * j5 >> 18;
                groundItemLocation.X = l6;

                Point2D dotLocation = new Point2D(groundItemLocation.X, -groundItemLocation.Y);

                DrawMinimapDot(spriteBatch, dotLocation, Colour.Yellow);
            }
        }
        
        void DrawNpcDots(SpriteBatch spriteBatch, int j1, int l1, int j5, int l5)
        {
            foreach (ClientMob npc in client.Npcs.Where(x => x != null))
            {
                Point2D npcLocaiton = new Point2D(
                    ((npc.Location.X - client.CurrentPlayer.Location.X) * 3 * j1) / 2048,
                    ((npc.Location.Y - client.CurrentPlayer.Location.Y) * 3 * j1) / 2048);

                int i7 = npcLocaiton.Y * j5 + npcLocaiton.X * l5 >> 18;
                npcLocaiton.Y = npcLocaiton.Y * l5 - npcLocaiton.X * j5 >> 18;
                npcLocaiton.X = i7;

                Point2D dotLocation = new Point2D(npcLocaiton.X, -npcLocaiton.Y);

                DrawMinimapDot(spriteBatch, dotLocation, Colour.ChromeYellow);
            }
        }
        
        void DrawPlayerDots(SpriteBatch spriteBatch, int j1, int l1, int j5, int l5)
        {
            foreach (ClientMob player in client.Players.Where(x => x != null))
            {
                Point2D playerLocaiton = new Point2D(
                    ((player.Location.X - client.CurrentPlayer.Location.X) * 3 * j1) / 2048,
                    ((player.Location.Y - client.CurrentPlayer.Location.Y) * 3 * j1) / 2048);

                int j7 = playerLocaiton.Y * j5 + playerLocaiton.X * l5 >> 18;
                playerLocaiton.Y = playerLocaiton.Y * l5 - playerLocaiton.X * j5 >> 18;
                playerLocaiton.X = j7;

                Point2D dotLocation = new Point2D(playerLocaiton.X, -playerLocaiton.Y);

                DrawMinimapDot(spriteBatch, dotLocation, Colour.White);
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

        void DrawMinimapDot(SpriteBatch spriteBatch, Point2D location, Colour colour)
        {
            Point2D dotOffset = new Point2D(156 / 2, 36 + 152 / 2);
            Point2D minimapLocation = location + dotOffset;
            Point2D screenLocation = new Point2D(dot.SpriteSize / 2) + Location + minimapLocation;

            if (!ClientRectangle.Contains(screenLocation))
            {
                return;
            }

            int alpha = alphaMask[minimapLocation.X, minimapLocation.Y];

            if (alpha == 0)
            {
                return;
            }

            dot.Tint = colour;
            dot.Location = screenLocation;

            dot.Draw(spriteBatch);
        }

        void OnCompassIndicatorClicked(object sender, MouseButtonEventArgs e)
        {
            client.cameraRotation = 128;
        }
    }
}
