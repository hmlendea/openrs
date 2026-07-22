using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;

using OpenRS.Gui.Helpers;
using OpenRS.Localisation;
using OpenRS.Settings;

namespace OpenRS.Gui
{
    public sealed class FpsIndicator
    {
        private static string FontContentFile => "Fonts/FrameCounterFont";
        private static string FpsTextPrefix => $"{LocalisationManager.GetString("ui.fps")}: ";
        private static Vector2 DrawPosition => new(1, 1);

        private GameTime currentGameTime;
        private SpriteFont framesPerSecondFont;
        private string framesPerSecondText;

        public void LoadContent() =>
            framesPerSecondFont = NuciContentManager.Instance.LoadSpriteFont(FontContentFile);

        public void UnloadContent() { }

        public void Update(GameTime gameTime)
        {
            currentGameTime = gameTime;

            framesPerSecondText =
                $"{FpsTextPrefix}{Math.Round(FramerateCounter.Instance.AverageFramesPerSecond)}";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float deltaTime = (float)currentGameTime.ElapsedGameTime.TotalSeconds;
            FramerateCounter.Instance.Update(deltaTime);

            if (SettingsManager.Instance.DebugMode)
            {
                spriteBatch.DrawString(
                    framesPerSecondFont,
                    framesPerSecondText,
                    DrawPosition,
                    Color.Lime);
            }
        }
    }
}
