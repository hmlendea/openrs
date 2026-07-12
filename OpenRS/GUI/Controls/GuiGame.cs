using System;
using System.Linq;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics;
using NuciXNA.Gui.Controls;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;

namespace OpenRS.Gui.Controls
{
    public class GuiGame(GameClient client) : GuiControl
    {
        private readonly GameClient gameClient = client;

        private SpriteBatch spriteBatch;
        private SpriteBatch gameSpriteBatch;

        private Texture2D _lastGameImageTexture;

        private bool isSectionLoading;
        private bool isContentLoading;

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            spriteBatch = GraphicsManager.Instance.SpriteBatch;
            gameSpriteBatch = new SpriteBatch(GraphicsManager.Instance.Graphics.GraphicsDevice);
            RegisterEvents();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {
            gameClient.Dispose();
            UnregisterEvents();
        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime)
        {
            if (gameClient is not null) // TODO: Ugly null check
            {
                gameClient.Update(gameTime);
            }
        }

        /// <summary>
        /// Draw the content on the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            if (!isContentLoading) // TODO: Ugly check here
            {
                DrawGame(gameClient);
            }
        }

        /// <summary>
        /// Registers the events.
        /// </summary>
        private void RegisterEvents()
        {
            gameClient.OnContentLoadedCompleted += client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded += client_OnContentLoaded;
            gameClient.OnLoadingSection += client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted += client_OnLoadingSectionCompleted;
        }

        /// <summary>
        /// Unregisters the events.
        /// </summary>
        private void UnregisterEvents()
        {
            gameClient.OnContentLoadedCompleted -= client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded -= client_OnContentLoaded;
            gameClient.OnLoadingSection -= client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted -= client_OnLoadingSectionCompleted;
        }

        private void DrawGame(GameClient client)
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

                    uint[] colors = new uint[client.gameGraphics.pixels.Length];

                    for (int j = 0; j < client.gameGraphics.pixels.Length; j++)
                    {
                        var bytes = BitConverter.GetBytes(client.gameGraphics.pixels[j]);
                        var r = bytes[2];
                        var g = bytes[1];
                        var b = bytes[0];

                        colors[j] = GraphicsEngine.RgbaToUInt(r, g, b, 255);
                    }

                    int bufW = client.gameGraphics.GameSize.Width;
                    int bufH = client.gameGraphics.GameSize.Height;

                    float scale = Math.Min((float)Size.Width / bufW, (float)Size.Height / bufH);
                    int drawW = (int)(bufW * scale);
                    int drawH = (int)(bufH * scale);
                    int drawX = (Size.Width - drawW) / 2;
                    int drawY = (Size.Height - drawH) / 2;

                    client.GameDisplayOffsetX = drawX;
                    client.GameDisplayOffsetY = drawY;
                    client.GameDisplayScaleX = scale;
                    client.GameDisplayScaleY = scale;

                    Rectangle srcRect = new Rectangle(0, 0, bufW, bufH);
                    Rectangle destRect = new Rectangle(drawX, drawY, drawW, drawH);

                    if (client.gameGraphics.pixels.Any(p => p != 0) && client.DrawIsNecessary)
                    {
                        Texture2D imageTexture = new(
                            GraphicsManager.Instance.Graphics.GraphicsDevice,
                            bufW,
                            bufH,
                            false,
                            SurfaceFormat.Color);

                        imageTexture.SetData(colors.ToArray());

                        gameSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                        gameSpriteBatch.Draw(imageTexture, destRect, srcRect, Color.White);
                        gameSpriteBatch.End();

                        _lastGameImageTexture = imageTexture;

                        client.DrawIsNecessary = false;

                    }
                    else if (_lastGameImageTexture is not null)
                    {
                        gameSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                        gameSpriteBatch.Draw(_lastGameImageTexture, destRect, srcRect, Color.White);
                        gameSpriteBatch.End();
                    }
                }
                else if (_lastGameImageTexture is not null)
                {
                    int bufW = client.gameGraphics.GameSize.Width;
                    int bufH = client.gameGraphics.GameSize.Height;
                    float scale = Math.Min((float)Size.Width / bufW, (float)Size.Height / bufH);
                    int drawW = (int)(bufW * scale);
                    int drawH = (int)(bufH * scale);
                    int drawX = (Size.Width - drawW) / 2;
                    int drawY = (Size.Height - drawH) / 2;
                    Rectangle srcRect = new Rectangle(0, 0, bufW, bufH);
                    Rectangle destRect = new Rectangle(drawX, drawY, drawW, drawH);
                    gameSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                    gameSpriteBatch.Draw(_lastGameImageTexture, destRect, srcRect, Color.White);
                    gameSpriteBatch.End();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                Console.WriteLine(ex);
            }
        }

        private static bool DrawGameClient(GameClient client)
        {
            client.paint();

            try
            {
                if (!client.loggedIn)
                {
                    client.gameGraphics.IsLoggedIn = false;
                    client.drawLoginScreens();
                }
                else
                {
                    client.gameGraphics.IsLoggedIn = true;
                    client.drawGame();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                Console.WriteLine(ex);

                client.UnloadContent();
                client.memoryError = true;

                return false;
            }

            return true;
        }

        private void client_OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(200);

            isSectionLoading = false;
        }

        private void client_OnLoadingSection(object sender, EventArgs e) => isSectionLoading = true;

        private void client_OnContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(300);

            isContentLoading = false;
        }

        private void client_OnContentLoaded(object sender, ContentLoadedEventArgs e) => isContentLoading = true;
    }
}
