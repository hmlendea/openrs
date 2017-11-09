using System;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RuneScapeSolo.DataAccess.Resources;
using RuneScapeSolo.Graphics;
using RuneScapeSolo.Gui.Screens;
using RuneScapeSolo.Net.Client;
using RuneScapeSolo.Net.Client.Events;
using RuneScapeSolo.Net.Client.Extensions;
using RuneScapeSolo.Net.Client.Game;
using RuneScapeSolo.Settings;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiGame : GuiElement
    {
        [XmlIgnore]
        public GameClient gameClient; // TODO: Remove the public modifier and the XmlIgnore decoration
        Thread _gameThread;

        SpriteBatch spriteBatch;

        SpriteFont _diagnosticFont;
        SpriteFont _diagnosticFont2;

        Texture2D _lastGameImageTexture;
        Texture2D _gameLogo;
        Texture2D _loadingBackgroundImage;

        bool _isSectionLoading;
        bool _isContentLoading;
        string _contentLoadingStatusText = "";
        decimal _contentLoadingStatusProgress;

        public override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = GraphicsManager.Instance.SpriteBatch;

            _diagnosticFont = ResourceManager.Instance.LoadSpriteFont("Fonts/gameFont12");
            _diagnosticFont2 = ResourceManager.Instance.LoadSpriteFont("Fonts/gameFont16");

            _loadingBackgroundImage = ResourceManager.Instance.LoadTexture2D("sprites/pattern_40");
            _gameLogo = ResourceManager.Instance.LoadTexture2D("sprites/yuno4");

            gameClient = GameClient.CreateGameClient(GameDefines.ApplicationName, Size.Width, Size.Height);
            gameClient.DoNotDrawLogo = true;

            gameClient.OnContentLoadedCompleted += client_OnContentLoadedCompleted;
            gameClient.OnContentLoaded += client_OnContentLoaded;
            gameClient.OnLoadingSection += client_OnLoadingSection;
            gameClient.OnLoadingSectionCompleted += client_OnLoadingSectionCompleted;
            //_rscMudclient.LoadContent();

            gameClient.gameMinThreadSleepTime = 10;
            gameClient.Start();

            _gameThread = new Thread(gameClient.run);
            _gameThread.Start();
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
                catch
                {
                    Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

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

        }

        void DrawLogo()
        {
            /*
            try
            {
                //spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
            }
            catch
            {
                Console.WriteLine($"An error has occured in {nameof(GuiGame)}.cs");

                try
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
                }
                catch
                {
                    Console.WriteLine($"An error has occured in {nameof(GuiGame)}.cs");
                }
            }
            */

            try
            {
                var w = _gameLogo.Width;
                var h = _gameLogo.Height;
                var aspect = h / (float)w;

                var newWidth = gameClient.windowWidth / 2;
                var newHeight = newWidth * aspect;

                spriteBatch.Draw(_gameLogo, new Rectangle((gameClient.windowWidth / 2) - (newWidth / 2), 0, newWidth, (int)newHeight), Color.White);

                //spriteBatch.End();
            }
            catch
            {
                Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
            }
        }

        void DrawGame(GameClient client)
        {
            if (client != null)
            {
                try
                {
                    /*
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
                    */

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

                                colors[j] = GameImage.rgbaToUInt(r, g, b, 255);//new Color(r, g, b, 255).PackedValue;                            
                                //colors.Add();
                            }

                            if (client.gameGraphics.pixels.Any(p => p != 0) && client.DrawIsNecessary)
                            {
                                Texture2D imageTexture = new Texture2D(
                                    GraphicsManager.Instance.Graphics.GraphicsDevice,
                                    client.gameGraphics.gameWidth,
                                    client.gameGraphics.gameHeight,
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
                            DrawSectionLoading();
                        }
                    }

                    //spriteBatch.End();
                }
                catch
                {
                    Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                }
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
                Console.WriteLine($"An error has occured in {nameof(GameWindow)}.cs");
                Console.WriteLine(ex.Message);

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
            var sPos = new Vector2(Size.Width / 2 - (sSize.X / 2), Size.Height / 2 - (sSize.Y / 2));

            spriteBatch.DrawString(_diagnosticFont2, s1, sPos, Color.White);
        }

        void DrawContentLoading(string contentLoadingStatusText, decimal contentLoadingStatusProgress)
        {
            GraphicsManager.Instance.Graphics.GraphicsDevice.Clear(Color.Black);

            var s1 = contentLoadingStatusText + " - " + contentLoadingStatusProgress + "%";
            var sSize = _diagnosticFont.MeasureString(s1);
            var sPos = new Vector2(Size.Width / 2 - (sSize.X / 2), Size.Height / 2 - (sSize.Y / 2));
            //spriteBatch.Begin();


            if (_loadingBackgroundImage != null)
            {
                if (_loadingBackgroundImage.Width < Size.Width)
                {
                    var xs = (Size.Width / _loadingBackgroundImage.Width) + 1;
                    var ys = (Size.Height / _loadingBackgroundImage.Height) + 1;

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


            spriteBatch.fillRect((Size.Width / 4) - 12, (int)sPos.Y - 12, (Size.Width / 2) + 24, (int)sSize.Y + 24, Color.FromNonPremultiplied(0, 0, 0, 150));
            spriteBatch.drawRect((Size.Width / 4) - 12, (int)sPos.Y - 12, (Size.Width / 2) + 24, (int)sSize.Y + 24, Color.DarkGray);
            spriteBatch.fillRect((Size.Width / 4) - 10, (int)sPos.Y - 10, (int)(((float)contentLoadingStatusProgress / 100f) * ((Size.Width / 2) + 20)), (int)sSize.Y + 21, Color.DarkGray);
            spriteBatch.DrawString(_diagnosticFont, s1, sPos, Color.White);
            //spriteBatch.End();

            // Thread.Sleep(1000);
        }
        
        void client_OnLoadingSectionCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(200);

            _isSectionLoading = false;
        }

        void client_OnLoadingSection(object sender, EventArgs e)
        {
            _isSectionLoading = true;
        }

        void client_OnContentLoadedCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(300);

            _isContentLoading = false;
        }

        void client_OnContentLoaded(object sender, ContentLoadedEventArgs e)
        {
            _isContentLoading = true;
            _contentLoadingStatusProgress = e.Progress;
            _contentLoadingStatusText = e.StatusText;
        }
    }
}
