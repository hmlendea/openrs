using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;
using NuciXNA.Primitives.Mapping;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiMinimap(GameClient client) : GuiControl
    {
        private readonly GameClient client = client;

        private GuiMinimapIndicator compassIndicator;
        private GuiMinimapIndicator healthIndicator;
        private GuiMinimapIndicator staminaIndicator;
        private GuiMinimapIndicator prayerIndicator;

        private byte[,] alphaMask;

        private TextureSprite dotSprite;
        private TextureSprite pixel;
        private TextureSprite frame;

        public bool IsClickable { get; set; } = true;

        public int ZoomLevel { get; set; } = 2;
        protected override void DoLoadContent()
        {
            dotSprite = new TextureSprite
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
                Location = new Point2D(40, 9),
                Icon = "Interface/Minimap/icon_compass"
            };
            healthIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.PersianRed,
                Location = new Point2D(17, 36),
                Icon = "Interface/Minimap/icon_health"
            };
            staminaIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.OliveDrab,
                Location = new Point2D(162, 146),
                Icon = "Interface/Minimap/icon_stamina"
            };
            prayerIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.CornflowerBlue,
                Location = new Point2D(10, 72),
                Icon = "Interface/Minimap/icon_prayer"
            };

            Texture2D maskTexture = NuciContentManager.Instance.LoadTexture2D("Interface/Minimap/mask");
            Color[] maskBits = new Color[maskTexture.Width * maskTexture.Height];
            maskTexture.GetData(maskBits, 0, maskBits.Length);

            alphaMask = new byte[Size.Width, Size.Height];

            for (int tileRow = 0; tileRow < Size.Height; tileRow += 1)
            {
                for (int tileColumn = 0; tileColumn < Size.Width; tileColumn += 1)
                {
                    int pixelIndex = tileColumn + tileRow * Size.Width;

                    alphaMask[tileColumn, tileRow] = maskBits[pixelIndex].R;
                }
            }

            dotSprite.LoadContent();
            pixel.LoadContent();
            frame.LoadContent();

            RegisterChildren(compassIndicator, healthIndicator, staminaIndicator, prayerIndicator);
            RegisterEvents();
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() => UnregisterEvents();
        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();

            dotSprite.Update(gameTime);
            pixel.Update(gameTime);
            frame.Update(gameTime);

            compassIndicator.IconRotation = 1;
        }
        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            DrawMinimapMenu(spriteBatch);
            frame.Draw(spriteBatch);
        }

        private void RegisterEvents() => compassIndicator.Clicked += OnCompassIndicatorClicked;

        private void UnregisterEvents() => compassIndicator.Clicked -= OnCompassIndicatorClicked;

        private void SetChildrenProperties()
        {
            frame.Location = ScreenLocation;

            if (client is null || !client.loggedIn)
            {
                return;
            }

            // The cameraRotation is expressed in a non-standard manner. 64 = -90 degrees, 32 = -45 degrees, etc...
            // so we have to convert it to degrees by multiplying it with 1.4025, add 180 degrees (flip),
            // and then convert that to radians in order to use them to Rotate the image
            compassIndicator.IconRotation = (float)(Math.PI / 180) * (client.cameraRotation * 1.4025f + 180);

            healthIndicator.BaseValue = client.Skills[3].BaseLevel;
            healthIndicator.CurrentValue = client.Skills[3].CurrentLevel;

            staminaIndicator.BaseValue = 100;
            staminaIndicator.CurrentValue = staminaIndicator.BaseValue - client.PlayerFatigue;

            prayerIndicator.BaseValue = client.Skills[5].BaseLevel;
            prayerIndicator.CurrentValue = client.Skills[5].CurrentLevel;
        }

        private void DrawMinimapMenu(SpriteBatch spriteBatch)
        {
            if (client.gameGraphics is null || !client.loggedIn)
            {
                return;
            }

            int zoomScale = 192 + client.minimapRandomRotationY;
            int rotationAngle = client.cameraRotation + client.minimapRandomRotationX & 0xff;
            int rotationCosine = Camera.trigonometryTable[1024 - rotationAngle * 4 & 0x3ff];
            int rotationSine = Camera.trigonometryTable[(1024 - rotationAngle * 4 & 0x3ff) + 1024];

            DrawMinimapTiles(spriteBatch);

            DrawGroundItemDots(spriteBatch, zoomScale, rotationAngle, rotationCosine, rotationSine);
            DrawNpcDots(spriteBatch, zoomScale, rotationAngle, rotationCosine, rotationSine);
            DrawPlayerDots(spriteBatch, zoomScale, rotationAngle, rotationCosine, rotationSine);
        }

        private void DrawGroundItemDots(SpriteBatch spriteBatch, int zoomScale, int rotationAngle, int rotationCosine, int rotationSine)
        {
            for (int groundItemIndex = 0; groundItemIndex < client.GroundItemCount; groundItemIndex += 1)
            {
                Point2D groundItemLocation = new(
                    (client.GroundItemLocations[groundItemIndex].X * client.GridSize + 64 - client.CurrentPlayer.Location.X) * 3 * zoomScale / 2048,
                    (client.GroundItemLocations[groundItemIndex].Y * client.GridSize + 64 - client.CurrentPlayer.Location.Y) * 3 * zoomScale / 2048);

                int rotatedCoordinateX = groundItemLocation.Y * rotationCosine + groundItemLocation.X * rotationSine >> 18;
                groundItemLocation.Y = groundItemLocation.Y * rotationSine - groundItemLocation.X * rotationCosine >> 18;
                groundItemLocation.X = rotatedCoordinateX;

                Point2D dotLocation = new(groundItemLocation.X, -groundItemLocation.Y);

                DrawMinimapDot(spriteBatch, dotLocation, Colour.Yellow);
            }
        }

        private void DrawNpcDots(SpriteBatch spriteBatch, int zoomScale, int rotationAngle, int rotationCosine, int rotationSine)
        {
            foreach (ClientMob npc in client.Npcs.Where(npcEntry => npcEntry is not null))
            {
                Point2D npcLocation = new(
                    (npc.Location.X - client.CurrentPlayer.Location.X) * 3 * zoomScale / 2048,
                    (npc.Location.Y - client.CurrentPlayer.Location.Y) * 3 * zoomScale / 2048);

                int rotatedCoordinateX = npcLocation.Y * rotationCosine + npcLocation.X * rotationSine >> 18;
                npcLocation.Y = npcLocation.Y * rotationSine - npcLocation.X * rotationCosine >> 18;
                npcLocation.X = rotatedCoordinateX;

                Point2D dotLocation = new(npcLocation.X, -npcLocation.Y);

                DrawMinimapDot(spriteBatch, dotLocation, Colour.ChromeYellow);
            }
        }

        private void DrawPlayerDots(SpriteBatch spriteBatch, int zoomScale, int rotationAngle, int rotationCosine, int rotationSine)
        {
            foreach (ClientMob player in client.Players.Where(playerEntry => playerEntry is not null))
            {
                Point2D playerLocation = new(
                    (player.Location.X - client.CurrentPlayer.Location.X) * 3 * zoomScale / 2048,
                    (player.Location.Y - client.CurrentPlayer.Location.Y) * 3 * zoomScale / 2048);

                int rotatedCoordinateX = playerLocation.Y * rotationCosine + playerLocation.X * rotationSine >> 18;
                playerLocation.Y = playerLocation.Y * rotationSine - playerLocation.X * rotationCosine >> 18;
                playerLocation.X = rotatedCoordinateX;

                Point2D dotLocation = new(playerLocation.X, -playerLocation.Y);

                DrawMinimapDot(spriteBatch, dotLocation, Colour.White);
            }
        }

        private void DrawMinimapTiles(SpriteBatch spriteBatch)
        {
            for (int tileRow = 0; tileRow < Size.Height; tileRow += 1)
            {
                for (int tileColumn = 0; tileColumn < Size.Width; tileColumn += 1)
                {
                    Colour tileColour = Colour.Black;
                    int alpha = tileColour.A - 255 + alphaMask[tileColumn, tileRow];

                    pixel.Location = new Point2D(ScreenLocation.X + tileColumn, ScreenLocation.Y + tileRow);
                    pixel.Tint = Color.FromNonPremultiplied(tileColour.R, tileColour.G, tileColour.B, alpha).ToColour();

                    pixel.Draw(spriteBatch);
                }
            }
        }

        private void DrawMinimapDot(SpriteBatch spriteBatch, Point2D location, Colour colour)
        {
            Point2D dotOffset = new(156 / 2, 36 + 152 / 2);
            Point2D minimapLocation = location + dotOffset;
            Point2D screenLocation = new Point2D(dotSprite.SpriteSize / 2) + ScreenLocation + minimapLocation;

            if (!DisplayRectangle.Contains(screenLocation))
            {
                return;
            }

            int alpha = alphaMask[minimapLocation.X, minimapLocation.Y];

            if (alpha == 0)
            {
                return;
            }

            dotSprite.Tint = colour;
            dotSprite.Location = screenLocation;

            dotSprite.Draw(spriteBatch);
        }

        private void OnCompassIndicatorClicked(object sender, MouseButtonEventArgs e) => client.cameraRotation = 128;
    }
}
