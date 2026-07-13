using System;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Primitives;
using OpenRS.Gui.Controls;
using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;

namespace OpenRS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public sealed class GameWindow : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameClient rscMudclient;
        private Thread gameThread;

        private Texture2D lastGameImageTexture;

        private Texture2D gameLogo;

        private SpriteFont diagnosticFont;
        private SpriteFont diagnosticFont2;

        private bool isSectionLoading;
        private bool isContentLoading;
        private string contentLoadingStatusText = "";
        private decimal contentLoadingStatusProgress = 0m;

        private Texture2D loadingBackgroundImage;

        private GuiInventoryPanel inventoryPanel;
        private SpriteBatch guiSpriteBatch;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 480
            };

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            Window.Title = "RuneScape Classic";
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to Run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            diagnosticFont = Content.Load<SpriteFont>("fonts/gameFont12");
            diagnosticFont2 = Content.Load<SpriteFont>("fonts/gameFont16");

            loadingBackgroundImage = Content.Load<Texture2D>("sprites/pattern_40");

            gameLogo = Content.Load<Texture2D>("sprites/yuno4");

            rscMudclient = GameClient.CreateMudclient(Window.Title, 768, 480);
            rscMudclient.DoNotDrawLogo = true;

            rscMudclient.OnContentLoadedCompleted += new EventHandler(rscMudclient_OnContentLoadedCompleted);
            rscMudclient.OnContentLoaded += new EventHandler<ContentLoadedEventArgs>(rscMudclient_OnContentLoaded);
            rscMudclient.OnLoadingSection += new EventHandler(rscMudclient_OnLoadingSection);
            rscMudclient.OnLoadingSectionCompleted += new EventHandler(rscMudclient_OnLoadingSectionCompleted);

            GameClient.GameWindow = Window;

            rscMudclient.gameMinThreadSleepTime = 10;
            rscMudclient.Start();

            gameThread = new Thread(rscMudclient.Run);
            gameThread.Start();

            // Initialise NuciXNA content and graphics managers (needed by GuiControl)
            guiSpriteBatch = new SpriteBatch(GraphicsDevice);
            GraphicsManager.Instance.Graphics = graphics;
            GraphicsManager.Instance.SpriteBatch = guiSpriteBatch;
            NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
            NuciContentManager.MissingTexturePlaceholder = "sprites/pattern_40";

            inventoryPanel = new GuiInventoryPanel(rscMudclient)
            {
                Location = new Point2D(768, 0),
                Size = new Size2D(256, 480)
            };
            inventoryPanel.LoadContent();
        }

        private void rscMudclient_OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(200);
            isSectionLoading = false;
        }

        private void rscMudclient_OnLoadingSection(object sender, EventArgs e)
        {
            isSectionLoading = true;
        }

        private void rscMudclient_OnContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(300);
            isContentLoading = false;
        }

        private void rscMudclient_OnContentLoaded(object sender, ContentLoadedEventArgs e)
        {
            isContentLoading = true;
            contentLoadingStatusProgress = e.Progress;
            contentLoadingStatusText = e.StatusText;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            try
            {
                rscMudclient.Destroy();
            }
            catch { }
        }

        /// <summary>
        /// Allows the game to Run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            if (rscMudclient is not null)
            {
                rscMudclient.Update(gameTime);

                try
                {
                    if (rscMudclient.IsTradeWindowVisible(TradeAndDuelState.Initial))
                    {
                    }
                }
                catch { }
            }

            if (inventoryPanel is not null && rscMudclient?.loggedIn == true)
            {
                inventoryPanel.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (!isContentLoading)
            {
                DrawGame(rscMudclient);

                if (!rscMudclient.loggedIn && rscMudclient.DoNotDrawLogo)
                {
                    DrawLogo();
                }
            }

            if (isContentLoading)
            {
                DrawContentLoading(contentLoadingStatusText, contentLoadingStatusProgress);
            }

            if (inventoryPanel is not null && rscMudclient?.loggedIn == true)
            {
                guiSpriteBatch.Begin();
                inventoryPanel.Draw(guiSpriteBatch);
                guiSpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawLogo()
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
                int logoWidth = gameLogo.Width;
                int logoHeight = gameLogo.Height;
                float logoAspectRatio = (float)logoHeight / (float)logoWidth;

                int halfWindowWidth = rscMudclient.windowWidth / 2;
                float scaledLogoHeight = halfWindowWidth * logoAspectRatio;

                spriteBatch.Draw(gameLogo, new Rectangle((rscMudclient.windowWidth / 2) - (halfWindowWidth / 2), 0, halfWindowWidth, (int)scaledLogoHeight), Color.White);

                spriteBatch.End();
            }
            catch { }
        }

        private void DrawGame(GameClient rscMudclient)
        {
            if (rscMudclient is not null)
            {
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

                                colors[pixelIndex] = Net.Client.Game.GameImage.RgbaToUInt(redChannel, greenChannel, blueChannel, 255);
                            }

                            if (rscMudclient.DrawIsNecessary)
                            {
                                Texture2D imageTexture = new(GraphicsDevice, rscMudclient.gameGraphics.gameWidth, rscMudclient.gameGraphics.gameHeight, false, SurfaceFormat.Color);
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
                            DrawSectionLoading();
                        }
                    }

                    spriteBatch.End();
                }
                catch { }
            }
        }

        private static bool DrawMudclient(GameClient rscMudclient)
        {
            rscMudclient.Paint(GameClient.graphics);

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

        private void DrawSectionLoading()
        {
            string message = "Loading... Please wait";
            Vector2 textSize = diagnosticFont2.MeasureString(message);
            Vector2 textPosition = new(graphics.PreferredBackBufferWidth / 2 - (textSize.X / 2),
                                               graphics.PreferredBackBufferHeight / 2 - (textSize.Y / 2));

            spriteBatch.DrawString(diagnosticFont2, message, textPosition, Color.White);
        }

        private void DrawContentLoading(string contentLoadingStatusText, decimal contentLoadingStatusProgress)
        {
            GraphicsDevice.Clear(Color.Black);

            string statusMessage = contentLoadingStatusText + " - " + contentLoadingStatusProgress + "%";
            Vector2 statusTextSize = diagnosticFont.MeasureString(statusMessage);
            Vector2 statusTextPosition = new(graphics.PreferredBackBufferWidth / 2 - (statusTextSize.X / 2),
                                                     graphics.PreferredBackBufferHeight / 2 - (statusTextSize.Y / 2));
            spriteBatch.Begin();

            if (loadingBackgroundImage is not null)
            {
                if (loadingBackgroundImage.Width < graphics.PreferredBackBufferWidth)
                {
                    int tilesHorizontal = (graphics.PreferredBackBufferWidth / loadingBackgroundImage.Width) + 1;
                    int tilesVertical = (graphics.PreferredBackBufferHeight / loadingBackgroundImage.Height) + 1;

                    for (int tileRow = 0; tileRow < tilesVertical; tileRow += 1)
                    {
                        for (int tileColumn = 0; tileColumn < tilesHorizontal; tileColumn += 1)
                        {
                            spriteBatch.Draw(loadingBackgroundImage, new Vector2(tileColumn * loadingBackgroundImage.Width,
                                tileRow * loadingBackgroundImage.Height), Color.White);
                        }
                    }

                    spriteBatch.DrawGradient(20, 20, 20, 90, Color.FromNonPremultiplied(255, 255, 255, 255),
                                             Color.FromNonPremultiplied(255, 255, 255, 100));
                }
                else
                {
                    spriteBatch.Draw(loadingBackgroundImage, Vector2.Zero, Color.White);
                }
            }

            spriteBatch.FillRect((graphics.PreferredBackBufferWidth / 4) - 12, (int)statusTextPosition.Y - 12, (graphics.PreferredBackBufferWidth / 2) + 24, (int)statusTextSize.Y + 24, Color.FromNonPremultiplied(0, 0, 0, 150));
            spriteBatch.DrawRect((graphics.PreferredBackBufferWidth / 4) - 12, (int)statusTextPosition.Y - 12, (graphics.PreferredBackBufferWidth / 2) + 24, (int)statusTextSize.Y + 24, Color.DarkGray);
            spriteBatch.FillRect((graphics.PreferredBackBufferWidth / 4) - 10, (int)statusTextPosition.Y - 10, (int)(((float)contentLoadingStatusProgress / 100f) * ((graphics.PreferredBackBufferWidth / 2) + 20)), (int)statusTextSize.Y + 21, Color.DarkGray);
            spriteBatch.DrawString(diagnosticFont, statusMessage, statusTextPosition, Color.White);
            spriteBatch.End();
        }
    }
}
