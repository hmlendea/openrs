using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;

using OpenRS.Gui.Helpers;
using OpenRS.Settings;

namespace OpenRS.Gui
{
    public sealed class FpsIndicator
    {
        private GameTime gameTime;
        private SpriteFont fpsFont;
        private string fpsString;
        public Vector2 Location { get; set; }
        public FpsIndicator()
        {
            Location = Vector2.Zero;
        }
        public void LoadContent() => fpsFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/FrameCounterFont");
        public void UnloadContent()
        {

        }
        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;

            fpsString = $"FPS: {Math.Round(FramerateCounter.Instance.AverageFramesPerSecond)}";
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FramerateCounter.Instance.Update(deltaTime);

            if (SettingsManager.Instance.DebugMode)
            {
                spriteBatch.DrawString(fpsFont, fpsString, new Vector2(1, 1), Color.Lime);
            }
        }
    }
}
