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
        private static string EntityDotContentFile => "Interface/Minimap/entity_dot";
        private static string FillImageContentFile => "ScreenManager/FillImage";
        private static string FrameContentFile => "Interface/Minimap/frame";
        private static string MaskContentFile => "Interface/Minimap/mask";
        private static string CompassIconContentFile => "Interface/Minimap/icon_compass";
        private static string HealthIconContentFile => "Interface/Minimap/icon_health";
        private static string StaminaIconContentFile => "Interface/Minimap/icon_stamina";
        private static string PrayerIconContentFile => "Interface/Minimap/icon_prayer";

        private static int NorthCameraRotation => 128;
        private static float CameraRotationToDegreesFactor => 1.4025f;
        private static int CameraRotationDegreeOffset => 180;

        private static int BaseZoomScale => 192;
        private static int ZoomCoordinateMultiplier => 3;
        private static int ZoomCoordinateDivisor => 2048;
        private static int GroundItemCentreOffset => 64;
        private static int RotationBitShift => 18;
        private static int RotationTableStep => 4;
        private static int TrigonometryTableSineOffset => 1024;
        private static int TrigonometryRotationMask => 0x3ff;
        private static int FullRotationMask => 0xff;

        private static int HealthSkillIndex => 3;
        private static int PrayerSkillIndex => 5;
        private static int MaximumStamina => 100;

        private static int MinimapInnerWidth => 156;
        private static int MinimapInnerHeight => 152;
        private static int MinimapInnerTopOffset => 36;

        private static int CompassIndicatorLocationX => 40;
        private static int CompassIndicatorLocationY => 9;
        private static int HealthIndicatorLocationX => 17;
        private static int HealthIndicatorLocationY => 36;
        private static int StaminaIndicatorLocationX => 162;
        private static int StaminaIndicatorLocationY => 146;
        private static int PrayerIndicatorLocationX => 10;
        private static int PrayerIndicatorLocationY => 72;

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
                ContentFile = EntityDotContentFile
            };
            pixel = new TextureSprite
            {
                ContentFile = FillImageContentFile
            };
            frame = new TextureSprite
            {
                ContentFile = FrameContentFile
            };

            compassIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.Bisque,
                Location = new Point2D(CompassIndicatorLocationX, CompassIndicatorLocationY),
                Icon = CompassIconContentFile
            };
            healthIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.PersianRed,
                Location = new Point2D(HealthIndicatorLocationX, HealthIndicatorLocationY),
                Icon = HealthIconContentFile
            };
            staminaIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.OliveDrab,
                Location = new Point2D(StaminaIndicatorLocationX, StaminaIndicatorLocationY),
                Icon = StaminaIconContentFile
            };
            prayerIndicator = new GuiMinimapIndicator
            {
                BackgroundColour = Colour.CornflowerBlue,
                Location = new Point2D(PrayerIndicatorLocationX, PrayerIndicatorLocationY),
                Icon = PrayerIconContentFile
            };

            Texture2D maskTexture = NuciContentManager.Instance.LoadTexture2D(MaskContentFile);
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

            RegisterChildren(
                compassIndicator,
                healthIndicator,
                staminaIndicator,
                prayerIndicator);
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

            UpdateIndicatorValues();
        }

        private void UpdateIndicatorValues()
        {
            // The cameraRotation uses a non-standard scale: 64 = -90 degrees, 32 = -45 degrees, etc.
            // It is converted to degrees by multiplying by CameraRotationToDegreesFactor, then offset
            // by CameraRotationDegreeOffset (flip), and then converted to radians.
            compassIndicator.IconRotation =
                (float)(Math.PI / 180) *
                (client.cameraRotation * CameraRotationToDegreesFactor +
                    CameraRotationDegreeOffset);

            healthIndicator.BaseValue = client.Skills[HealthSkillIndex].BaseLevel;
            healthIndicator.CurrentValue = client.Skills[HealthSkillIndex].CurrentLevel;

            staminaIndicator.BaseValue = MaximumStamina;
            staminaIndicator.CurrentValue = MaximumStamina - client.PlayerFatigue;

            prayerIndicator.BaseValue = client.Skills[PrayerSkillIndex].BaseLevel;
            prayerIndicator.CurrentValue = client.Skills[PrayerSkillIndex].CurrentLevel;
        }

        private void DrawMinimapMenu(SpriteBatch spriteBatch)
        {
            if (client.gameGraphics is null || !client.loggedIn || client.CurrentPlayer is null)
            {
                return;
            }

            int zoomScale = BaseZoomScale + client.minimapRandomRotationY;
            int rotationAngle =
                client.cameraRotation + client.minimapRandomRotationX & FullRotationMask;
            int trigIndex =
                TrigonometryTableSineOffset - rotationAngle * RotationTableStep &
                TrigonometryRotationMask;
            int rotationCosine = Camera.TrigonometryTable[trigIndex];
            int rotationSine = Camera.TrigonometryTable[trigIndex + TrigonometryTableSineOffset];

            DrawMinimapTiles(spriteBatch);

            DrawGroundItemDots(spriteBatch, zoomScale, rotationCosine, rotationSine);
            DrawNpcDots(spriteBatch, zoomScale, rotationCosine, rotationSine);
            DrawPlayerDots(spriteBatch, zoomScale, rotationCosine, rotationSine);
        }

        private void DrawGroundItemDots(
            SpriteBatch spriteBatch,
            int zoomScale,
            int rotationCosine,
            int rotationSine)
        {
            int playerLocationX = client.CurrentPlayer.Location.X;
            int playerLocationY = client.CurrentPlayer.Location.Y;
            int groundItemCount = client.GroundItemCount;

            for (int groundItemIndex = 0; groundItemIndex < groundItemCount; groundItemIndex += 1)
            {
                int groundX = client.GroundItemLocations[groundItemIndex].X;
                int groundY = client.GroundItemLocations[groundItemIndex].Y;

                int scaledX = (groundX * client.GridSize + GroundItemCentreOffset -
                    playerLocationX) * ZoomCoordinateMultiplier * zoomScale / ZoomCoordinateDivisor;
                int scaledY = (groundY * client.GridSize + GroundItemCentreOffset -
                    playerLocationY) * ZoomCoordinateMultiplier * zoomScale / ZoomCoordinateDivisor;

                Point2D groundItemLocation = new(scaledX, scaledY);

                int rotatedCoordinateX =
                    groundItemLocation.Y * rotationCosine +
                    groundItemLocation.X * rotationSine >> RotationBitShift;
                groundItemLocation.Y =
                    groundItemLocation.Y * rotationSine -
                    groundItemLocation.X * rotationCosine >> RotationBitShift;
                groundItemLocation.X = rotatedCoordinateX;

                DrawMinimapDot(
                    spriteBatch,
                    new Point2D(groundItemLocation.X, -groundItemLocation.Y),
                    Colour.Yellow);
            }
        }

        private void DrawNpcDots(
            SpriteBatch spriteBatch,
            int zoomScale,
            int rotationCosine,
            int rotationSine)
        {
            int playerLocationX = client.CurrentPlayer.Location.X;
            int playerLocationY = client.CurrentPlayer.Location.Y;

            foreach (ClientMob npc in client.Npcs.Where(npcEntry => npcEntry is not null))
            {
                int scaledX = (npc.Location.X - playerLocationX) *
                    ZoomCoordinateMultiplier * zoomScale / ZoomCoordinateDivisor;
                int scaledY = (npc.Location.Y - playerLocationY) *
                    ZoomCoordinateMultiplier * zoomScale / ZoomCoordinateDivisor;

                Point2D npcLocation = new(scaledX, scaledY);

                int rotatedCoordinateX =
                    npcLocation.Y * rotationCosine +
                    npcLocation.X * rotationSine >> RotationBitShift;
                npcLocation.Y =
                    npcLocation.Y * rotationSine -
                    npcLocation.X * rotationCosine >> RotationBitShift;
                npcLocation.X = rotatedCoordinateX;

                DrawMinimapDot(
                    spriteBatch,
                    new Point2D(npcLocation.X, -npcLocation.Y),
                    Colour.ChromeYellow);
            }
        }

        private void DrawPlayerDots(
            SpriteBatch spriteBatch,
            int zoomScale,
            int rotationCosine,
            int rotationSine)
        {
            int currentPlayerX = client.CurrentPlayer.Location.X;
            int currentPlayerY = client.CurrentPlayer.Location.Y;

            foreach (ClientMob player in client.Players.Where(playerEntry => playerEntry is not null))
            {
                int scaledX = (player.Location.X - currentPlayerX) *
                    ZoomCoordinateMultiplier * zoomScale / ZoomCoordinateDivisor;
                int scaledY = (player.Location.Y - currentPlayerY) *
                    ZoomCoordinateMultiplier * zoomScale / ZoomCoordinateDivisor;

                Point2D playerLocation = new(scaledX, scaledY);

                int rotatedCoordinateX =
                    playerLocation.Y * rotationCosine +
                    playerLocation.X * rotationSine >> RotationBitShift;
                playerLocation.Y =
                    playerLocation.Y * rotationSine -
                    playerLocation.X * rotationCosine >> RotationBitShift;
                playerLocation.X = rotatedCoordinateX;

                DrawMinimapDot(
                    spriteBatch,
                    new Point2D(playerLocation.X, -playerLocation.Y),
                    Colour.White);
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

                    pixel.Location = new Point2D(
                        ScreenLocation.X + tileColumn,
                        ScreenLocation.Y + tileRow);

                    Color rawColour = Color.FromNonPremultiplied(
                        tileColour.R, tileColour.G, tileColour.B, alpha);
                    pixel.Tint = rawColour.ToColour();

                    pixel.Draw(spriteBatch);
                }
            }
        }

        private void DrawMinimapDot(SpriteBatch spriteBatch, Point2D location, Colour colour)
        {
            Point2D dotOffset = new(
                MinimapInnerWidth / 2,
                MinimapInnerTopOffset + MinimapInnerHeight / 2);
            Point2D minimapLocation = location + dotOffset;
            Point2D screenLocation =
                new Point2D(dotSprite.SpriteSize / 2) +
                ScreenLocation +
                minimapLocation;

            if (!DisplayRectangle.Contains(screenLocation))
            {
                return;
            }

            if (minimapLocation.X < 0 || minimapLocation.X >= Size.Width ||
                minimapLocation.Y < 0 || minimapLocation.Y >= Size.Height)
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

        private void OnCompassIndicatorClicked(object sender, MouseButtonEventArgs e) =>
            client.cameraRotation = NorthCameraRotation;
    }
}
