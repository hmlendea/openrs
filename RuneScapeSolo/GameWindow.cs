using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RuneScapeSolo.Events;
using RuneScapeSolo.Input;
using RuneScapeSolo.Lib;

namespace RuneScapeSolo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameWindow : Game
    {
        readonly GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameClient gameClient;
        System.Threading.Thread _gameThread;

        Texture2D _lastGameImageTexture;

        Texture2D _gameLogo;

        SpriteFont _diagnosticFont;
        SpriteFont _diagnosticFont2;

        bool _isSectionLoading;
        bool _isContentLoading;
        string _contentLoadingStatusText = "";
        decimal _contentLoadingStatusProgress;

        Texture2D _loadingBackgroundImage;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            Window.Title = "RuneScape Classic";
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            _diagnosticFont = Content.Load<SpriteFont>("fonts/gameFont12");
            _diagnosticFont2 = Content.Load<SpriteFont>("fonts/gameFont16");

            _loadingBackgroundImage = Content.Load<Texture2D>("sprites/pattern_40");

            _gameLogo = Content.Load<Texture2D>("sprites/yuno4");

            gameClient = GameClient.CreateGameClient(Window.Title, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            gameClient.DoNotDrawLogo = true;

            gameClient.OnContentLoadedCompleted += client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded += client_OnContentLoaded;
            gameClient.OnLoadingSection += client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted += client_OnLoadingSectionCompleted;
            //_rscMudclient.LoadContent();

            gameClient.gameMinThreadSleepTime = 10;
            gameClient.Start();

            _gameThread = new System.Threading.Thread(gameClient.run);
            _gameThread.Start();

            //_rscMudclient.

            // TODO: use this.Content to load your game content here
        }

        void client_OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(200);
            _isSectionLoading = false;
        }

        void client_OnLoadingSection(object sender, EventArgs e)
        {
            _isSectionLoading = true;
        }

        void client_OnContentLoadedCompleted(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(300);
            _isContentLoading = false;
        }

        void client_OnContentLoaded(object sender, ContentLoadedEventArgs e)
        {
            _isContentLoading = true;
            _contentLoadingStatusProgress = e.Progress;
            _contentLoadingStatusText = e.StatusText;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            //gameThread.Abort();

            gameClient.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            if (gameClient != null)
            {
                gameClient.Update(gameTime);
                try
                {
                    if (gameClient != null && gameClient.engineHandle != null && gameClient.engineHandle._camera != null && gameClient.engineHandle._camera.objectCache != null)
                    {
                        //var obj = _rscMudclient.engineHandle._camera.objectCache.FirstOrDefault();
                        //if (obj != null)
                        //{
                        //  if (test == null)//|| test.)
                        //  {
                        //      test = new OB3Model(this.GraphicsDevice, obj);
                        //  }
                        //  //var faceCount = obj.face_count;
                        //  //var vertices = obj._vertices;
                        //  //var faceBack = obj.texture_front;
                        //  //var faceFront = obj.texture_back;
                        //  //var faces = obj.face_verts;
                        //  //var face_vert_count = obj.face_vert_count;


                        //}
                    }
                    /*if (_rscMudclient.IsTradeWindowVisible(TradeAndDuelState.Initial))
                    {

                    }*/ // this /* ... */ was not commented in the original code
                    //  _rscMudclient.checkInputs();
                }
                catch { }
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Black);

            //rscMudclient.Draw(gameTime);

            //    if (rscMudclient.gameGraphics != null)
            //        rscMudclient.gameGraphics.drawImage(spriteBatch, 0, 0);
            //if (GameTexture != null)
            //{
            //    spriteBatch.Begin();
            //    spriteBatch.Draw(GameTexture, Vector2.Zero, Color.White);
            //    spriteBatch.End();
            //}
            GraphicsDevice.Clear(Color.Black);

            if (!_isContentLoading)
            {
                DrawGame(gameClient);

                if (!gameClient.loggedIn && gameClient.DoNotDrawLogo) // at loginscreen.
                {
                    DrawLogo();
                }
            }

            if (_isContentLoading)
            {
                DrawContentLoading(_contentLoadingStatusText, _contentLoadingStatusProgress);
            }

            // TODO: Add your drawing code here


            //if (test != null)
            //{            // Create camera matrices, making the object spin.
            //  float time = (float)gameTime.TotalGameTime.TotalSeconds;
            //  float yaw = time * 0.4f;
            //  float pitch = time * 0.7f;
            //  float roll = time * 1.1f;

            //  Vector3 cameraPosition = new Vector3(0, 0, 2.5f);

            //  float aspect = GraphicsDevice.Viewport.AspectRatio;

            //  Matrix world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            //  Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            //  Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 10);

            //  test.Draw(world, view, projection, Color.Red);
            //}

            base.Draw(gameTime);



            //GC.Collect();
        }

        void DrawLogo()
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
                catch
                {
                }
            }

            try
            {
                var w = _gameLogo.Width;
                var h = _gameLogo.Height;
                var aspect = h / (float)w;

                var newWidth = gameClient.windowWidth / 2;
                var newHeight = newWidth * aspect;

                spriteBatch.Draw(_gameLogo, new Rectangle((gameClient.windowWidth / 2) - (newWidth / 2), 0, newWidth, (int)newHeight), Color.White);

                spriteBatch.End();
            }
            catch { }
        }

        void DrawGame(GameClient client)
        {
            if (client != null)
            {
                try
                {
                    try
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                    catch
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    }
                    //rscMudclient.Draw(gameTime);

                    if (client.gameGraphics != null)
                    {
                        // rscMudclient.gameGraphics.UpdateGameImage();
                        if (!_isSectionLoading)
                        {
                            if (!DrawGameClient(client))
                            {
                                return;
                            }

                            // _rscMudclient.drawWindow();

                            // var colors = new List<Color>();
                            uint[] colors = new uint[client.gameGraphics.pixels.Length];
                            for (int j = 0; j < client.gameGraphics.pixels.Length; j++)
                            {
                                var bytes = BitConverter.GetBytes(client.gameGraphics.pixels[j]);
                                var r = bytes[2];
                                var g = bytes[1];
                                var b = bytes[0];

                                colors[j] = RuneScapeSolo.Lib.Game.GameImage.rgbaToUInt(r, g, b, 255);//new Color(r, g, b, 255).PackedValue;                            
                                //colors.Add();
                            }

                            if (client.gameGraphics.pixels.Any(p => p != 0) && client.DrawIsNecessary)
                            {

                                var imageTexture = new Texture2D(GraphicsDevice, client.gameGraphics.gameWidth, client.gameGraphics.gameHeight, false, SurfaceFormat.Color);
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
                            DrawSectionLoading();
                        }
                    }

                    spriteBatch.End();
                }
                catch { }
            }
        }

        static bool DrawGameClient(GameClient client)
        {
            client.paint(GameClient.graphics);

            try
            {
                if (!client.loggedIn)
                {
                    client.gameGraphics.loggedIn = false;
                    client.drawLoginScreens();
                }
                else
                {
                    client.gameGraphics.loggedIn = true;
                    client.drawGame();

                    return true;
                }
            }
            catch (Exception ex)
            {
                client.cleanUp();
                client.memoryError = true;

                return false;
            }
            return true;
        }

        void DrawSectionLoading()
        {
            var s1 = "Loading... Please wait";
            var sSize = _diagnosticFont2.MeasureString(s1);
            var sPos = new Vector2(graphics.PreferredBackBufferWidth / 2 - (sSize.X / 2),
                                   graphics.PreferredBackBufferHeight / 2 - (sSize.Y / 2));

            spriteBatch.DrawString(_diagnosticFont2, s1, sPos, Color.White);
        }

        void DrawContentLoading(string contentLoadingStatusText, decimal contentLoadingStatusProgress)
        {
            GraphicsDevice.Clear(Color.Black);

            var s1 = contentLoadingStatusText + " - " + contentLoadingStatusProgress + "%";
            var sSize = _diagnosticFont.MeasureString(s1);
            var sPos = new Vector2(graphics.PreferredBackBufferWidth / 2 - (sSize.X / 2),
                                   graphics.PreferredBackBufferHeight / 2 - (sSize.Y / 2));
            spriteBatch.Begin();


            if (_loadingBackgroundImage != null)
            {
                if (_loadingBackgroundImage.Width < graphics.PreferredBackBufferWidth)
                {
                    var xs = (graphics.PreferredBackBufferWidth / _loadingBackgroundImage.Width) + 1;
                    var ys = (graphics.PreferredBackBufferHeight / _loadingBackgroundImage.Height) + 1;
                    for (int y = 0; y < ys; y++)
                    {
                        for (int x = 0; x < xs; x++)
                        {
                            spriteBatch.Draw(_loadingBackgroundImage, new Vector2(x * _loadingBackgroundImage.Width,
                                y * _loadingBackgroundImage.Height), Color.White);
                        }
                    }

                    spriteBatch.drawGradient(20, 20, 20, 90, Color.FromNonPremultiplied(255, 255, 255, 255),
                                             Color.FromNonPremultiplied(255, 255, 255, 100));
                }
                else
                {
                    spriteBatch.Draw(_loadingBackgroundImage, Vector2.Zero, Color.White);
                }

            }

            /* Draw Background Image if any. */


            spriteBatch.fillRect((graphics.PreferredBackBufferWidth / 4) - 12, (int)sPos.Y - 12, (graphics.PreferredBackBufferWidth / 2) + 24, (int)sSize.Y + 24, Color.FromNonPremultiplied(0, 0, 0, 150));
            spriteBatch.drawRect((graphics.PreferredBackBufferWidth / 4) - 12, (int)sPos.Y - 12, (graphics.PreferredBackBufferWidth / 2) + 24, (int)sSize.Y + 24, Color.DarkGray);
            spriteBatch.fillRect((graphics.PreferredBackBufferWidth / 4) - 10, (int)sPos.Y - 10, (int)(((float)contentLoadingStatusProgress / 100f) * ((graphics.PreferredBackBufferWidth / 2) + 20)), (int)sSize.Y + 21, Color.DarkGray);
            spriteBatch.DrawString(_diagnosticFont, s1, sPos, Color.White);
            spriteBatch.End();

            //  System.Threading.Thread.Sleep(1000);
        }
    }
}