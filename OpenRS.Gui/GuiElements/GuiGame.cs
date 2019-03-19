﻿using System;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics;
using NuciXNA.Gui.GuiElements;

using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;
using OpenRS.Net.Client.Game;

namespace OpenRS.Gui.GuiElements
{
    public class GuiGame : GuiElement
    {
        GameClient gameClient;

        SpriteBatch spriteBatch;

        Texture2D _lastGameImageTexture;

        bool isSectionLoading;
        bool isContentLoading;

        public override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = GraphicsManager.Instance.SpriteBatch;
        }

        public override void UnloadContent()
        {
            gameClient.Dispose();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (gameClient != null)
            {
                gameClient.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!isContentLoading)
            {
                DrawGame(gameClient);
            }
        }

        public void AssociateGameClient(ref GameClient client)
        {
            this.gameClient = client;
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            gameClient.OnContentLoadedCompleted += client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded += client_OnContentLoaded;
            gameClient.OnLoadingSection += client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted += client_OnLoadingSectionCompleted;
        }

        protected override void UnregisterEvents()
        {
            base.UnregisterEvents();

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
