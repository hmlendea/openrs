using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciLog.Core;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Gui;
using NuciXNA.Gui.Screens;

using OpenRS.Gui.Controls;
using OpenRS.Localisation;
using OpenRS.Logging;
using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;

namespace OpenRS.Gui.Screens
{
    public sealed class GameplayScreen : Screen
    {
        private static string ItemSpritesDirectory
            => Path.Combine(ApplicationPaths.ApplicationDirectory, "Content", "sprites", "items");

        private static string PngFileExtension => ".png";
        private static string SectionLoadingMessage
            => LocalisationManager.GetString("ui.section_loading_message");

        private static int RedChannelByteIndex => 2;
        private static int GreenChannelByteIndex => 1;
        private static int BlueChannelByteIndex => 0;
        private static int OpaqueAlphaValue => 255;
        private static int SectionLoadedDelayMilliseconds => 200;
        private static int ContentLoadedDelayMilliseconds => 300;
        private static int LoadingBarOuterPadding => 12;
        private static int LoadingBarInnerPadding => 10;
        private static int LoadingBarProgressHeightAdjustment => 21;
        private static int LoadingBarBackgroundAlpha => 150;
        private static float FullProgressPercent => 100f;

        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameplayScreen>();

        private GameClient gameClient;
        private Thread gameThread;

        private Texture2D lastGameImageTexture;
        private SpriteBatch itemSpriteBatch;

        private SpriteFont contentLoadingFont;
        private SpriteFont sectionLoadingFont;

        private readonly Dictionary<string, Texture2D> itemTextureCache = [];

        private bool isSectionLoading;
        private bool isContentLoading;
        private string loadingStatusText = string.Empty;
        private decimal loadingStatusProgress;

        private GuiSideBar sideBar;

        protected override void DoLoadContent()
        {
            itemSpriteBatch = new SpriteBatch(GraphicsManager.Instance.Graphics.GraphicsDevice);

            contentLoadingFont = NuciContentManager.Instance.LoadSpriteFont("fonts/gameFont12");
            sectionLoadingFont = NuciContentManager.Instance.LoadSpriteFont("fonts/gameFont16");

            gameClient = GameClient.CreateMudclient("RuneScape Classic", GameDefines.GameViewportWidth, GameDefines.WindowHeight);
            gameClient.DoNotDrawLogo = true;

            gameClient.OnContentLoadedCompleted += OnContentLoadedCompleted;
            gameClient.OnContentLoaded += OnContentLoaded;
            gameClient.OnLoadingSection += OnLoadingSection;
            gameClient.OnLoadingSectionCompleted += OnLoadingSectionCompleted;

            gameClient.gameMinThreadSleepTime = 10;
            gameClient.Start();

            gameThread = new Thread(gameClient.Run);
            gameThread.Start();

            sideBar = new GuiSideBar(gameClient)
            {
                Location = new(GameDefines.GameViewportWidth, 0),
                Size = new(GameDefines.SidePanelWidth, GameDefines.WindowHeight)
            };

            GuiManager.Instance.RegisterControls(sideBar);
            sideBar.Hide();
        }

        protected override void DoUnloadContent()
        {
            foreach (Texture2D texture in itemTextureCache.Values)
            {
                texture?.Dispose();
            }

            itemTextureCache.Clear();

            try
            {
                gameClient.Destroy();
            }
            catch { }
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            gameClient?.Update(gameTime);

            if (gameClient?.loggedIn == true)
            {
                sideBar.Show();
            }
            else
            {
                sideBar.Hide();
            }
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            // Ensure the batch is closed before our custom drawing begins,
            // regardless of the state left by the previous frame's GuiManager pass.
            try
            {
                spriteBatch.End();
            }
            catch { }

            if (!isContentLoading)
            {
                DrawGame(spriteBatch);

                if (gameClient?.loggedIn == true)
                {
                    DrawItemSprites();
                }
            }
            else
            {
                DrawContentLoading(spriteBatch, loadingStatusText, loadingStatusProgress);
            }

            spriteBatch.Begin();
        }

        private void OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(SectionLoadedDelayMilliseconds);
            isSectionLoading = false;
        }

        private void OnLoadingSection(object sender, EventArgs e) => isSectionLoading = true;

        private void OnContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(ContentLoadedDelayMilliseconds);
            isContentLoading = false;
        }

        private void OnContentLoaded(object sender, ContentLoadedEventArgs e)
        {
            isContentLoading = true;
            loadingStatusProgress = e.Progress;
            loadingStatusText = e.StatusText;
        }

        private void DrawGame(SpriteBatch spriteBatch)
        {
            if (gameClient is null)
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

                if (gameClient.gameGraphics is not null)
                {
                    if (!isSectionLoading)
                    {
                        if (!DrawMudclient(gameClient))
                        {
                            return;
                        }

                        uint[] pixelColours = new uint[gameClient.gameGraphics.Pixels.Length];

                        for (int pixelIndex = 0; pixelIndex < gameClient.gameGraphics.Pixels.Length; pixelIndex += 1)
                        {
                            byte[] pixelBytes = BitConverter.GetBytes(gameClient.gameGraphics.Pixels[pixelIndex]);
                            byte redChannel = pixelBytes[RedChannelByteIndex];
                            byte greenChannel = pixelBytes[GreenChannelByteIndex];
                            byte blueChannel = pixelBytes[BlueChannelByteIndex];

                            pixelColours[pixelIndex] = GameImage.RgbaToUInt(
                                redChannel, greenChannel, blueChannel, OpaqueAlphaValue);
                        }

                        if (gameClient.DrawIsNecessary)
                        {
                            Texture2D imageTexture = new(
                                GraphicsManager.Instance.Graphics.GraphicsDevice,
                                gameClient.gameGraphics.GameWidth,
                                gameClient.gameGraphics.GameHeight,
                                false,
                                SurfaceFormat.Color);
                            imageTexture.SetData(pixelColours);

                            spriteBatch.Draw(imageTexture, Vector2.Zero, Color.White);

                            lastGameImageTexture = imageTexture;
                            gameClient.DrawIsNecessary = false;
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

        private void DrawItemSprites()
        {
            IReadOnlyList<ItemSpriteDrawCall> drawCalls = gameClient.PendingItemSpriteDrawCalls;

            if (drawCalls.Count == 0)
            {
                return;
            }

            itemSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);

            foreach (ItemSpriteDrawCall drawCall in drawCalls)
            {
                Texture2D texture = LoadItemTexture(drawCall.SpriteName);

                if (texture is null)
                {
                    continue;
                }

                Rectangle destination = new(
                    (int)(gameClient.GameDisplayOffsetX + drawCall.PixelX * gameClient.GameDisplayScaleX),
                    (int)(gameClient.GameDisplayOffsetY + drawCall.PixelY * gameClient.GameDisplayScaleY),
                    (int)(drawCall.PixelWidth * gameClient.GameDisplayScaleX),
                    (int)(drawCall.PixelHeight * gameClient.GameDisplayScaleY));

                itemSpriteBatch.Draw(texture, destination, Color.White);
            }

            itemSpriteBatch.End();
        }

        private Texture2D LoadItemTexture(string spriteName)
        {
            if (itemTextureCache.TryGetValue(spriteName, out Texture2D cached))
            {
                return cached;
            }

            string pngPath = Path.Combine(ItemSpritesDirectory, spriteName + PngFileExtension);

            if (!File.Exists(pngPath))
            {
                itemTextureCache[spriteName] = null;
                return null;
            }

            try
            {
                using Stream stream = File.OpenRead(pngPath);
                Texture2D texture = Texture2D.FromStream(GraphicsManager.Instance.Graphics.GraphicsDevice, stream);
                itemTextureCache[spriteName] = texture;
                return texture;
            }
            catch (Exception ex)
            {
                logger.Error(
                    GameOperation.LoadItemTexture,
                    "Failed to load item texture.",
                    ex,
                    new LogInfo(GameLogInfoKey.FilePath, pngPath));
                itemTextureCache[spriteName] = null;
                return null;
            }
        }

        private void DrawSectionLoading(SpriteBatch spriteBatch)
        {
            Vector2 textSize = sectionLoadingFont.MeasureString(SectionLoadingMessage);
            Vector2 textPosition = new(
                ScreenManager.Instance.Size.Width / 2 - (textSize.X / 2),
                ScreenManager.Instance.Size.Height / 2 - (textSize.Y / 2));

            spriteBatch.DrawString(sectionLoadingFont, SectionLoadingMessage, textPosition, Color.White);
        }

        private void DrawContentLoading(SpriteBatch spriteBatch, string statusText, decimal statusProgress)
        {
            GraphicsManager.Instance.Graphics.GraphicsDevice.Clear(Color.Black);

            string statusMessage = statusText + " - " + statusProgress + "%";
            Vector2 statusTextSize = contentLoadingFont.MeasureString(statusMessage);
            Vector2 statusTextPosition = new(
                ScreenManager.Instance.Size.Width / 2 - (statusTextSize.X / 2),
                ScreenManager.Instance.Size.Height / 2 - (statusTextSize.Y / 2));

            spriteBatch.Begin();

            spriteBatch.FillRect(
                (ScreenManager.Instance.Size.Width / 4) - LoadingBarOuterPadding,
                (int)statusTextPosition.Y - LoadingBarOuterPadding,
                (ScreenManager.Instance.Size.Width / 2) + (LoadingBarOuterPadding * 2),
                (int)statusTextSize.Y + (LoadingBarOuterPadding * 2),
                Color.FromNonPremultiplied(0, 0, 0, LoadingBarBackgroundAlpha));
            spriteBatch.DrawRect(
                (ScreenManager.Instance.Size.Width / 4) - LoadingBarOuterPadding,
                (int)statusTextPosition.Y - LoadingBarOuterPadding,
                (ScreenManager.Instance.Size.Width / 2) + (LoadingBarOuterPadding * 2),
                (int)statusTextSize.Y + (LoadingBarOuterPadding * 2),
                Color.DarkGray);
            spriteBatch.FillRect(
                (ScreenManager.Instance.Size.Width / 4) - LoadingBarInnerPadding,
                (int)statusTextPosition.Y - LoadingBarInnerPadding,
                (int)((float)statusProgress / FullProgressPercent *
                    ((ScreenManager.Instance.Size.Width / 2) + (LoadingBarInnerPadding * 2))),
                (int)statusTextSize.Y + LoadingBarProgressHeightAdjustment,
                Color.DarkGray);
            spriteBatch.DrawString(contentLoadingFont, statusMessage, statusTextPosition, Color.White);
            spriteBatch.End();
        }

        private static bool DrawMudclient(GameClient gameClient)
        {
            gameClient.Paint(GameClient.graphics);

            if (gameClient.memoryError || gameClient.gameGraphics is null)
            {
                return false;
            }

            try
            {
                if (!gameClient.loggedIn)
                {
                    gameClient.gameGraphics.IsLoggedIn = false;
                    gameClient.DrawLoginScreens();
                }

                if (gameClient.loggedIn)
                {
                    gameClient.gameGraphics.IsLoggedIn = true;
                    gameClient.DrawGame();

                    return true;
                }
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderGame,
                    exception);

                gameClient.CleanUp();
                gameClient.memoryError = true;

                return false;
            }

            return true;
        }
    }
}
