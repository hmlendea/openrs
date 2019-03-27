using System;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics;
using NuciXNA.Gui.Controls;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;

namespace OpenRS.Gui.Controls
{
    public class GuiGame : GuiControl
    {
        readonly GameClient gameClient;

        SpriteBatch spriteBatch;

        Texture2D _lastGameImageTexture;

        bool isSectionLoading;
        bool isContentLoading;

        public GuiGame(GameClient client)
        {
            this.gameClient = client;
        }

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
            if (gameClient != null) // TODO: Ugly null check
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
        void RegisterEvents()
        {
            gameClient.OnContentLoadedCompleted += client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded += client_OnContentLoaded;
            gameClient.OnLoadingSection += client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted += client_OnLoadingSectionCompleted;
        }

        /// <summary>
        /// Unregisters the events.
        /// </summary>
        void UnregisterEvents()
        {
            gameClient.OnContentLoadedCompleted -= client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded -= client_OnContentLoaded;
            gameClient.OnLoadingSection -= client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted -= client_OnLoadingSectionCompleted; 
        }

        void DrawGame(GameClient client)
        {
            if (client == null || client.gameGraphics == null)
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

                        colors[j] = GraphicsEngine.rgbaToUInt(r, g, b, 255);
                    }

                    if (client.gameGraphics.pixels.Any(p => p != 0) && client.DrawIsNecessary)
                    {
                        Texture2D imageTexture = new Texture2D(
                            GraphicsManager.Instance.Graphics.GraphicsDevice,
                            client.gameGraphics.GameSize.Width,
                            client.gameGraphics.GameSize.Height,
                            false,
                            SurfaceFormat.Color);

                        imageTexture.SetData(colors.ToArray());

                        spriteBatch.Draw(imageTexture, Vector2.Zero, Color.White);

                        _lastGameImageTexture = imageTexture;

                        client.DrawIsNecessary = false;

                    }
                    else if (_lastGameImageTexture != null)
                    {
                        spriteBatch.Draw(_lastGameImageTexture, Vector2.Zero, Color.White);
                    }
                }
                else if (_lastGameImageTexture != null)
                {
                    spriteBatch.Draw(_lastGameImageTexture, Vector2.Zero, Color.White);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                Console.WriteLine(ex);
            }
        }

        static bool DrawGameClient(GameClient client)
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

        void client_OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(200);

            isSectionLoading = false;
        }

        void client_OnLoadingSection(object sender, EventArgs e)
        {
            isSectionLoading = true;
        }

        void client_OnContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(300);

            isContentLoading = false;
        }

        void client_OnContentLoaded(object sender, ContentLoadedEventArgs e)
        {
            isContentLoading = true;
        }
    }
}
