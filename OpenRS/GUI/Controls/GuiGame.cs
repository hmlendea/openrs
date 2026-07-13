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
    public sealed class GuiGame(GameClient client) : GuiControl
    {

        private SpriteBatch spriteBatch;
        private SpriteBatch gameSpriteBatch;

        private Texture2D _lastGameImageTexture;

        private bool isSectionLoading;
        private bool isContentLoading;
        protected override void DoLoadContent()
        {
            spriteBatch = GraphicsManager.Instance.SpriteBatch;
            gameSpriteBatch = new SpriteBatch(GraphicsManager.Instance.Graphics.GraphicsDevice);
            RegisterEvents();
        }
        protected override void DoUnloadContent()
        {
            client.Dispose();
            UnregisterEvents();
        }
        protected override void DoUpdate(GameTime gameTime)
        {
            if (client is not null) // TODO: Ugly null check.
            {
                client.Update(gameTime);
            }
        }
        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            if (!isContentLoading) // TODO: Ugly check here.
            {
                DrawGame(client);
            }
        }
        private void RegisterEvents()
        {
            client.OnContentLoadedCompleted += client_OnContentLoadedCompleted;
            client.OnContentLoaded += client_OnContentLoaded;
            client.OnLoadingSection += client_OnLoadingSection;
            client.OnLoadingSectionCompleted += client_OnLoadingSectionCompleted;
        }
        private void UnregisterEvents()
        {
            client.OnContentLoadedCompleted -= client_OnContentLoadedCompleted;
            client.OnContentLoaded -= client_OnContentLoaded;
            client.OnLoadingSection -= client_OnLoadingSection;
            client.OnLoadingSectionCompleted -= client_OnLoadingSectionCompleted;
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

                    for (int pixelIndex = 0; pixelIndex < client.gameGraphics.pixels.Length; pixelIndex += 1)
                    {
                        byte[] pixelBytes = BitConverter.GetBytes(client.gameGraphics.pixels[pixelIndex]);
                        byte redChannel = pixelBytes[2];
                        byte greenChannel = pixelBytes[1];
                        byte blueChannel = pixelBytes[0];

                        colors[pixelIndex] = GraphicsEngine.RgbaToUInt(redChannel, greenChannel, blueChannel, 255);
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

                    Rectangle srcRect = new(0, 0, bufW, bufH);
                    Rectangle destRect = new(drawX, drawY, drawW, drawH);

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
                    Rectangle srcRect = new(0, 0, bufW, bufH);
                    Rectangle destRect = new(drawX, drawY, drawW, drawH);
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
            client.Paint();

            try
            {
                if (!client.loggedIn)
                {
                    client.gameGraphics.IsLoggedIn = false;
                    client.DrawLoginScreens();
                }
                else
                {
                    client.gameGraphics.IsLoggedIn = true;
                    client.DrawGame();

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
