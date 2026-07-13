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

        private Texture2D lastGameImageTexture;

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
            client?.Update(gameTime);
        }

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            if (!isContentLoading)
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

                    uint[] pixelColours = new uint[client.gameGraphics.pixels.Length];

                    for (int pixelIndex = 0; pixelIndex < client.gameGraphics.pixels.Length; pixelIndex += 1)
                    {
                        byte[] pixelBytes = BitConverter.GetBytes(client.gameGraphics.pixels[pixelIndex]);
                        byte redChannel = pixelBytes[2];
                        byte greenChannel = pixelBytes[1];
                        byte blueChannel = pixelBytes[0];

                        pixelColours[pixelIndex] = GraphicsEngine.RgbaToUInt(redChannel, greenChannel, blueChannel, 255);
                    }

                    Rectangle sourceRectangle = CalculateGameSourceRectangle(client);
                    Rectangle destinationRectangle = CalculateGameDestinationRectangle(client);

                    client.GameDisplayOffsetX = destinationRectangle.X;
                    client.GameDisplayOffsetY = destinationRectangle.Y;
                    client.GameDisplayScaleX = (float)destinationRectangle.Width / sourceRectangle.Width;
                    client.GameDisplayScaleY = (float)destinationRectangle.Height / sourceRectangle.Height;

                    if (client.gameGraphics.pixels.Any(pixel => pixel != 0) && client.DrawIsNecessary)
                    {
                        Texture2D imageTexture = new(
                            GraphicsManager.Instance.Graphics.GraphicsDevice,
                            sourceRectangle.Width,
                            sourceRectangle.Height,
                            false,
                            SurfaceFormat.Color);

                        imageTexture.SetData(pixelColours.ToArray());

                        gameSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                        gameSpriteBatch.Draw(imageTexture, destinationRectangle, sourceRectangle, Color.White);
                        gameSpriteBatch.End();

                        lastGameImageTexture = imageTexture;

                        client.DrawIsNecessary = false;
                    }
                    else if (lastGameImageTexture is not null)
                    {
                        gameSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                        gameSpriteBatch.Draw(lastGameImageTexture, destinationRectangle, sourceRectangle, Color.White);
                        gameSpriteBatch.End();
                    }
                }
                else if (lastGameImageTexture is not null)
                {
                    Rectangle sourceRectangle = CalculateGameSourceRectangle(client);
                    Rectangle destinationRectangle = CalculateGameDestinationRectangle(client);

                    gameSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
                    gameSpriteBatch.Draw(lastGameImageTexture, destinationRectangle, sourceRectangle, Color.White);
                    gameSpriteBatch.End();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                Console.WriteLine(ex);
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
            float scale = Math.Min((float)Size.Width / bufferWidth, (float)Size.Height / bufferHeight);
            int drawWidth = (int)(bufferWidth * scale);
            int drawHeight = (int)(bufferHeight * scale);
            int drawPositionX = (Size.Width - drawWidth) / 2;
            int drawPositionY = (Size.Height - drawHeight) / 2;

            return new Rectangle(drawPositionX, drawPositionY, drawWidth, drawHeight);
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
