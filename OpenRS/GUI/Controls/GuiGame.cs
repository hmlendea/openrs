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

        private Texture2D _lastGameImageTexture;

        private bool isSectionLoading;
        private bool isContentLoading;

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            spriteBatch = GraphicsManager.Instance.SpriteBatch;
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

                    int gameViewWidth = client.gameGraphics.GameSize.Width - 248;
                    int gameViewHeight = client.WindowSize.Height;
                    Rectangle srcRect = new Rectangle(0, 0, gameViewWidth, gameViewHeight);

                    if (client.gameGraphics.pixels.Any(p => p != 0) && client.DrawIsNecessary)
                    {
                        Texture2D imageTexture = new(
                            GraphicsManager.Instance.Graphics.GraphicsDevice,
                            client.gameGraphics.GameSize.Width,
                            client.gameGraphics.GameSize.Height,
                            false,
                            SurfaceFormat.Color);

                        imageTexture.SetData(colors.ToArray());

                        spriteBatch.Draw(imageTexture, new Rectangle(0, 0, Size.Width, Size.Height), srcRect, Color.White);

                        _lastGameImageTexture = imageTexture;

                        client.DrawIsNecessary = false;

                    }
                    else if (_lastGameImageTexture is not null)
                    {
                        spriteBatch.Draw(_lastGameImageTexture, new Rectangle(0, 0, Size.Width, Size.Height), srcRect, Color.White);
                    }
                }
                else if (_lastGameImageTexture is not null)
                {
                    int gameViewWidth = client.gameGraphics.GameSize.Width - 248;
                    int gameViewHeight = client.WindowSize.Height;
                    Rectangle srcRect = new Rectangle(0, 0, gameViewWidth, gameViewHeight);
                    spriteBatch.Draw(_lastGameImageTexture, new Rectangle(0, 0, Size.Width, Size.Height), srcRect, Color.White);
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
