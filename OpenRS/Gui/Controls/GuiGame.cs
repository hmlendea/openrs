using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciLog.Core;

using NuciXNA.Graphics;
using NuciXNA.Gui.Controls;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiGame(GameClient client) : GuiControl
    {
        private static string ItemSpritesDirectory
            => Path.Combine(ApplicationPaths.ApplicationDirectory, "Content", "sprites", "items");

        private static string PngFileExtension => ".png";
        private static int RedChannelByteIndex => 2;
        private static int GreenChannelByteIndex => 1;
        private static int BlueChannelByteIndex => 0;
        private static byte OpaqueAlphaValue => 255;
        private static int SectionLoadedDelayMilliseconds => 200;
        private static int ContentLoadedDelayMilliseconds => 300;

        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<GuiGame>();

        private SpriteBatch gameSpriteBatch;

        private Texture2D lastGameImageTexture;
        private readonly Dictionary<string, Texture2D> itemTextureCache = [];

        private bool isSectionLoading;
        private bool isContentLoading;

        protected override void DoLoadContent()
        {
            gameSpriteBatch = new SpriteBatch(GraphicsManager.Instance.Graphics.GraphicsDevice);
            RegisterEvents();
        }

        protected override void DoUnloadContent()
        {
            foreach (Texture2D texture in itemTextureCache.Values)
            {
                texture?.Dispose();
            }

            itemTextureCache.Clear();
            client.Dispose();
            UnregisterEvents();
        }

        protected override void DoUpdate(GameTime gameTime)
        {
            client?.Update(gameTime);
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            if (!isContentLoading)
            {
                DrawGame();
            }
        }

        private void RegisterEvents()
        {
            client.OnContentLoadedCompleted += OnClientContentLoadedCompleted;
            client.OnContentLoaded += OnClientContentLoaded;
            client.OnLoadingSection += OnClientLoadingSection;
            client.OnLoadingSectionCompleted += OnClientLoadingSectionCompleted;
        }

        private void UnregisterEvents()
        {
            client.OnContentLoadedCompleted -= OnClientContentLoadedCompleted;
            client.OnContentLoaded -= OnClientContentLoaded;
            client.OnLoadingSection -= OnClientLoadingSection;
            client.OnLoadingSectionCompleted -= OnClientLoadingSectionCompleted;
        }

        private void DrawGame()
        {
            if (client is null || client.gameGraphics is null)
            {
                return;
            }

            try
            {
                if (!isSectionLoading)
                {
                    if (!DrawGameClient(client))
                    {
                        return;
                    }

                    int pixelCount = client.gameGraphics.pixels.Length;
                    uint[] pixelColours = new uint[pixelCount];

                    for (int pixelIndex = 0; pixelIndex < pixelCount; pixelIndex += 1)
                    {
                        int pixelValue = client.gameGraphics.pixels[pixelIndex];
                        byte[] pixelBytes = BitConverter.GetBytes(pixelValue);
                        byte redChannel = pixelBytes[RedChannelByteIndex];
                        byte greenChannel = pixelBytes[GreenChannelByteIndex];
                        byte blueChannel = pixelBytes[BlueChannelByteIndex];

                        pixelColours[pixelIndex] = GraphicsEngine.RgbaToUInt(
                            redChannel,
                            greenChannel,
                            blueChannel,
                            OpaqueAlphaValue);
                    }

                    Rectangle sourceRectangle = CalculateGameSourceRectangle(client);
                    Rectangle destinationRectangle = CalculateGameDestinationRectangle(client);

                    client.GameDisplayOffsetX = destinationRectangle.X;
                    client.GameDisplayOffsetY = destinationRectangle.Y;
                    client.GameDisplayScaleX =
                        (float)destinationRectangle.Width / sourceRectangle.Width;
                    client.GameDisplayScaleY =
                        (float)destinationRectangle.Height / sourceRectangle.Height;

                    if (client.gameGraphics.pixels.Any(pixel => pixel != 0) &&
                        client.DrawIsNecessary)
                    {
                        Texture2D imageTexture = new(
                            GraphicsManager.Instance.Graphics.GraphicsDevice,
                            sourceRectangle.Width,
                            sourceRectangle.Height,
                            false,
                            SurfaceFormat.Color);

                        imageTexture.SetData(pixelColours.ToArray());

                        DrawTextureOpaque(imageTexture, destinationRectangle, sourceRectangle);

                        lastGameImageTexture = imageTexture;

                        client.DrawIsNecessary = false;
                    }
                    else if (lastGameImageTexture is not null)
                    {
                        DrawTextureOpaque(
                            lastGameImageTexture,
                            destinationRectangle,
                            sourceRectangle);
                    }

                    DrawItemSprites(client);
                }
                else if (lastGameImageTexture is not null)
                {
                    Rectangle sourceRectangle = CalculateGameSourceRectangle(client);
                    Rectangle destinationRectangle = CalculateGameDestinationRectangle(client);

                    DrawTextureOpaque(
                        lastGameImageTexture,
                        destinationRectangle,
                        sourceRectangle);
                }
            }
            catch (Exception ex)
            {
                logger.Error("An error has occurred while drawing the game.", ex);
            }
        }

        private void DrawItemSprites(GameClient gameClient)
        {
            IReadOnlyList<ItemSpriteDrawCall> drawCalls = gameClient.PendingItemSpriteDrawCalls;

            if (drawCalls.Count == 0)
            {
                return;
            }

            gameSpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.NonPremultiplied,
                SamplerState.PointClamp);

            foreach (ItemSpriteDrawCall drawCall in drawCalls)
            {
                Texture2D texture = LoadItemTexture(drawCall.SpriteName);

                if (texture is null)
                {
                    continue;
                }

                Rectangle destination = new(
                    (int)(gameClient.GameDisplayOffsetX +
                        drawCall.PixelX * gameClient.GameDisplayScaleX),
                    (int)(gameClient.GameDisplayOffsetY +
                        drawCall.PixelY * gameClient.GameDisplayScaleY),
                    (int)(drawCall.PixelWidth * gameClient.GameDisplayScaleX),
                    (int)(drawCall.PixelHeight * gameClient.GameDisplayScaleY));

                gameSpriteBatch.Draw(texture, destination, Color.White);
            }

            gameSpriteBatch.End();
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
                logger.Warn(
                    "Item texture file not found.",
                    new LogInfo(GameLogInfoKey.FilePath, pngPath));
                itemTextureCache[spriteName] = null;

                return null;
            }

            try
            {
                using Stream stream = File.OpenRead(pngPath);
                Texture2D texture = Texture2D.FromStream(
                    GraphicsManager.Instance.Graphics.GraphicsDevice,
                    stream);
                itemTextureCache[spriteName] = texture;

                return texture;
            }
            catch (Exception loadException)
            {
                logger.Error(
                    "Failed to load item texture.",
                    loadException,
                    new LogInfo(GameLogInfoKey.FilePath, pngPath));
                itemTextureCache[spriteName] = null;

                return null;
            }
        }

        private Rectangle CalculateGameSourceRectangle(GameClient gameClient)
        {
            int bufferWidth = gameClient.gameGraphics.GameSize.Width;
            int bufferHeight = gameClient.gameGraphics.GameSize.Height;

            return new Rectangle(0, 0, bufferWidth, bufferHeight);
        }

        private Rectangle CalculateGameDestinationRectangle(GameClient gameClient)
        {
            int bufferWidth = gameClient.gameGraphics.GameSize.Width;
            int bufferHeight = gameClient.gameGraphics.GameSize.Height;
            float scale = Math.Min(
                (float)Size.Width / bufferWidth,
                (float)Size.Height / bufferHeight);
            int drawWidth = (int)(bufferWidth * scale);
            int drawHeight = (int)(bufferHeight * scale);
            int drawPositionX = (Size.Width - drawWidth) / 2;
            int drawPositionY = (Size.Height - drawHeight) / 2;

            return new Rectangle(drawPositionX, drawPositionY, drawWidth, drawHeight);
        }

        private void DrawTextureOpaque(
            Texture2D texture,
            Rectangle destination,
            Rectangle source)
        {
            gameSpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.PointClamp);
            gameSpriteBatch.Draw(texture, destination, source, Color.White);
            gameSpriteBatch.End();
        }

        private static bool DrawGameClient(GameClient gameClient)
        {
            gameClient.Paint();

            try
            {
                if (!gameClient.loggedIn)
                {
                    gameClient.gameGraphics.IsLoggedIn = false;
                    gameClient.DrawLoginScreens();
                }
                else
                {
                    gameClient.gameGraphics.IsLoggedIn = true;
                    gameClient.DrawGame();

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("An error has occurred while drawing the game.", ex);

                gameClient.UnloadContent();
                gameClient.memoryError = true;

                return false;
            }

            return true;
        }

        private void OnClientLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(SectionLoadedDelayMilliseconds);

            isSectionLoading = false;
        }

        private void OnClientLoadingSection(object sender, EventArgs e) =>
            isSectionLoading = true;

        private void OnClientContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(ContentLoadedDelayMilliseconds);

            isContentLoading = false;
        }

        private void OnClientContentLoaded(object sender, ContentLoadedEventArgs e) =>
            isContentLoading = true;
    }
}
