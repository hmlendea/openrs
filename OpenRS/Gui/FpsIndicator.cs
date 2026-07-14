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
        private GameTime currentGameTime;
        private SpriteFont framesPerSecondFont;
        private string framesPerSecondText;

        public Vector2 Location { get; set; }

        public FpsIndicator()
        {
            Location = Vector2.Zero;
        }

        public void LoadContent() => framesPerSecondFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/FrameCounterFont");

        public void UnloadContent() { }

        public void Update(GameTime gameTime)
        {
            currentGameTime = gameTime;

            framesPerSecondText = $"FPS: {Math.Round(FramerateCounter.Instance.AverageFramesPerSecond)}";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float deltaTime = (float)currentGameTime.ElapsedGameTime.TotalSeconds;
            FramerateCounter.Instance.Update(deltaTime);

            if (SettingsManager.Instance.DebugMode)
            {
                spriteBatch.DrawString(framesPerSecondFont, framesPerSecondText, new Vector2(1, 1), Color.Lime);
            }
        }
    }
}
