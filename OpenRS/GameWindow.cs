using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;

using OpenRS.Gui.Screens;
using OpenRS.Net.Client;
using OpenRS.Settings;

using XnaButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace OpenRS
{
    public sealed class GameWindow : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public GameWindow()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = GameDefines.WindowWidth,
                PreferredBackBufferHeight = GameDefines.WindowHeight
            };

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            Window.Title = "RuneScape Classic";
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GraphicsManager.Instance.Graphics = graphics;
            GraphicsManager.Instance.SpriteBatch = spriteBatch;

            NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
            NuciContentManager.MissingTexturePlaceholder = "ScreenManager/missing-texture";

            GameClient.GameWindow = Window;

            ScreenManager.Instance.SpriteBatch = spriteBatch;
            ScreenManager.Instance.StartingScreenType = typeof(SplashScreen);
            ScreenManager.Instance.LoadContent();
        }

        protected override void UnloadContent() => ScreenManager.Instance.UnloadContent();

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == XnaButtonState.Pressed)
            {
                Exit();
            }

            InputManager.Instance.Update(Window);
            ScreenManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            ScreenManager.Instance.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
