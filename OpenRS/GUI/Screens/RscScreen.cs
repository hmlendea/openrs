using System;
using System.Linq;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Gui.Screens;
using NuciXNA.Primitives;

using OpenRS.Gui.Controls;
using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;

namespace OpenRS.Gui.Screens
{
    public sealed class RscScreen : Screen
    {
        private GameClient rscMudclient;
        private Thread gameThread;

        private Texture2D lastGameImageTexture;
        private SpriteBatch guiSpriteBatch;

        private SpriteFont diagnosticFont;
        private SpriteFont diagnosticFont2;

        private bool isSectionLoading;
        private bool isContentLoading;
        private string contentLoadingStatusText = "";
        private decimal contentLoadingStatusProgress = 0m;

        private GuiInventoryPanel inventoryPanel;

        protected override void DoLoadContent()
        {
            guiSpriteBatch = new SpriteBatch(GraphicsManager.Instance.Graphics.GraphicsDevice);

            diagnosticFont = NuciContentManager.Instance.LoadSpriteFont("fonts/gameFont12");
            diagnosticFont2 = NuciContentManager.Instance.LoadSpriteFont("fonts/gameFont16");

            rscMudclient = GameClient.CreateMudclient("RuneScape Classic", 768, 480);
            rscMudclient.DoNotDrawLogo = true;

            rscMudclient.OnContentLoadedCompleted += OnContentLoadedCompleted;
            rscMudclient.OnContentLoaded += OnContentLoaded;
            rscMudclient.OnLoadingSection += OnLoadingSection;
            rscMudclient.OnLoadingSectionCompleted += OnLoadingSectionCompleted;

            rscMudclient.gameMinThreadSleepTime = 10;
            rscMudclient.Start();

            gameThread = new Thread(rscMudclient.Run);
            gameThread.Start();

            inventoryPanel = new GuiInventoryPanel(rscMudclient)
            {
                Location = new Point2D(768, 0),
                Size = new Size2D(256, 480)
            };
            inventoryPanel.LoadContent();
        }

        protected override void DoUnloadContent()
        {
            try
            {
                rscMudclient.Destroy();
            }
            catch { }
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            rscMudclient?.Update(gameTime);

            if (inventoryPanel is not null && rscMudclient?.loggedIn == true)
            {
                inventoryPanel.Update(gameTime);
            }
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            if (!isContentLoading)
            {
                DrawGame(spriteBatch);

                if (!rscMudclient.loggedIn && rscMudclient.DoNotDrawLogo)
                {
                    DrawLogo(spriteBatch);
                }
            }

            if (isContentLoading)
            {
                DrawContentLoading(spriteBatch, contentLoadingStatusText, contentLoadingStatusProgress);
            }

            if (inventoryPanel is not null && rscMudclient?.loggedIn == true)
            {
                guiSpriteBatch.Begin();
                inventoryPanel.Draw(guiSpriteBatch);
                guiSpriteBatch.End();
            }
        }

        private void OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(200);
            isSectionLoading = false;
        }

        private void OnLoadingSection(object sender, EventArgs e) => isSectionLoading = true;

        private void OnContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(300);
            isContentLoading = false;
        }

        private void OnContentLoaded(object sender, ContentLoadedEventArgs e)
        {
            isContentLoading = true;
            contentLoadingStatusProgress = e.Progress;
            contentLoadingStatusText = e.StatusText;
        }

        private void DrawGame(SpriteBatch spriteBatch)
        {
            if (rscMudclient is null)
            {
                return;
            }

            try
            {
                try
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                }
                catch
                {
                    try
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                    catch { }
                }

                if (rscMudclient.gameGraphics is not null)
                {
                    if (!isSectionLoading)
                    {
                        if (!DrawMudclient(rscMudclient))
                        {
                            return;
                        }

                        uint[] colors = new uint[rscMudclient.gameGraphics.pixels.Length];

                        for (int pixelIndex = 0; pixelIndex < rscMudclient.gameGraphics.pixels.Length; pixelIndex += 1)
                        {
                            byte[] pixelBytes = BitConverter.GetBytes(rscMudclient.gameGraphics.pixels[pixelIndex]);
                            byte redChannel = pixelBytes[2];
                            byte greenChannel = pixelBytes[1];
                            byte blueChannel = pixelBytes[0];

                            colors[pixelIndex] = GameImage.RgbaToUInt(redChannel, greenChannel, blueChannel, 255);
                        }

                        if (rscMudclient.DrawIsNecessary)
                        {
                            Texture2D imageTexture = new(GraphicsManager.Instance.Graphics.GraphicsDevice, rscMudclient.gameGraphics.gameWidth, rscMudclient.gameGraphics.gameHeight, false, SurfaceFormat.Color);
                            imageTexture.SetData(colors.ToArray());

                            spriteBatch.Draw(imageTexture, Vector2.Zero, Color.White);

                            lastGameImageTexture = imageTexture;

                            rscMudclient.DrawIsNecessary = false;
                        }
                        else if (lastGameImageTexture is not null)
                        {
                            spriteBatch.Draw(lastGameImageTexture, Vector2.Zero, Color.White);
                        }
                    }
                    else if (lastGameImageTexture is not null)
                    {
                        DrawSectionLoading(spriteBatch);
                    }
                }

                spriteBatch.End();
            }
            catch { }
        }

        private void DrawLogo(SpriteBatch spriteBatch)
        {
            try
            {
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            }
            catch
            {
                try
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
                }
                catch { }
            }

            try
            {
                spriteBatch.End();
            }
            catch { }
        }

        private void DrawSectionLoading(SpriteBatch spriteBatch)
        {
            string message = "Loading... Please wait";
            Vector2 textSize = diagnosticFont2.MeasureString(message);
            Vector2 textPosition = new(
                ScreenManager.Instance.Size.Width / 2 - (textSize.X / 2),
                ScreenManager.Instance.Size.Height / 2 - (textSize.Y / 2));

            spriteBatch.DrawString(diagnosticFont2, message, textPosition, Color.White);
        }

        private void DrawContentLoading(SpriteBatch spriteBatch, string statusText, decimal statusProgress)
        {
            GraphicsManager.Instance.Graphics.GraphicsDevice.Clear(Color.Black);

            string statusMessage = statusText + " - " + statusProgress + "%";
            Vector2 statusTextSize = diagnosticFont.MeasureString(statusMessage);
            Vector2 statusTextPosition = new(
                ScreenManager.Instance.Size.Width / 2 - (statusTextSize.X / 2),
                ScreenManager.Instance.Size.Height / 2 - (statusTextSize.Y / 2));

            spriteBatch.Begin();

            spriteBatch.FillRect(
                (ScreenManager.Instance.Size.Width / 4) - 12, (int)statusTextPosition.Y - 12,
                (ScreenManager.Instance.Size.Width / 2) + 24, (int)statusTextSize.Y + 24,
                Color.FromNonPremultiplied(0, 0, 0, 150));
            spriteBatch.DrawRect(
                (ScreenManager.Instance.Size.Width / 4) - 12, (int)statusTextPosition.Y - 12,
                (ScreenManager.Instance.Size.Width / 2) + 24, (int)statusTextSize.Y + 24,
                Color.DarkGray);
            spriteBatch.FillRect(
                (ScreenManager.Instance.Size.Width / 4) - 10, (int)statusTextPosition.Y - 10,
                (int)((float)statusProgress / 100f * ((ScreenManager.Instance.Size.Width / 2) + 20)),
                (int)statusTextSize.Y + 21,
                Color.DarkGray);
            spriteBatch.DrawString(diagnosticFont, statusMessage, statusTextPosition, Color.White);
            spriteBatch.End();
        }

        private static bool DrawMudclient(GameClient rscMudclient)
        {
            rscMudclient.Paint(GameClient.graphics);

            if (rscMudclient.memoryError || rscMudclient.gameGraphics is null)
            {
                return false;
            }

            try
            {
                if (!rscMudclient.loggedIn)
                {
                    rscMudclient.gameGraphics.loggedIn = false;
                    rscMudclient.DrawLoginScreens();
                }

                if (rscMudclient.loggedIn)
                {
                    rscMudclient.gameGraphics.loggedIn = true;
                    rscMudclient.DrawGame();

                    return true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[DrawMudclient EXCEPTION] {exception.GetType().Name}: {exception.Message}");
                Console.WriteLine(exception.StackTrace);
                rscMudclient.CleanUp();
                rscMudclient.memoryError = true;

                return false;
            }

            return true;
        }
    }
}
