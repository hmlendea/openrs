
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.GuiElements;
using NuciXNA.Gui.Screens;

using OpenRS.Settings;

namespace OpenRS.Gui.Screens
{
    /// <summary>
    /// Settings screen.
    /// </summary>
    public class SettingsScreen : MenuScreen
    {
        GuiMenuToggle debugModeToggle;
        GuiMenuToggle fowToggle;
        GuiMenuToggle roofsToggle;
        GuiMenuLink backLink;

        /// <summary>
        /// Loads the content.
        /// </summary>
        public override void LoadContent()
        {
            debugModeToggle = new GuiMenuToggle
            {
                Id = "debugToggle",
                Text = "Toggle debug mode",
                Property = "DebugMode"
            };
            fowToggle = new GuiMenuToggle
            {
                Id = "fowToggle",
                Text = "Toggle fog",
                Property = "FogOfWar"
            };
            roofsToggle = new GuiMenuToggle
            {
                Id = "roofsToggle",
                Text = "Toggle roofs",
                Property = "ShowRoofs"
            };
            backLink = new GuiMenuLink
            {
                Id = "back",
                Text = "Back",
                TargetScreen = typeof(TitleScreen)
            };
            
            Items.Add(debugModeToggle);
            Items.Add(fowToggle);
            Items.Add(roofsToggle);
            Items.Add(backLink);
            
            debugModeToggle.ToggleState = SettingsManager.Instance.DebugMode;
            fowToggle.ToggleState = SettingsManager.Instance.GraphicsSettings.FogOfWar;
            roofsToggle.ToggleState = SettingsManager.Instance.GraphicsSettings.ShowRoofs;

            base.LoadContent();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        public override void UnloadContent()
        {
            SettingsManager.Instance.SaveContent();
            base.UnloadContent();
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SettingsManager.Instance.DebugMode = debugModeToggle.ToggleState;
            SettingsManager.Instance.GraphicsSettings.FogOfWar = fowToggle.ToggleState;
            SettingsManager.Instance.GraphicsSettings.ShowRoofs = roofsToggle.ToggleState;
        }

        /// <summary>
        /// Draws the content on the specified spriteBatch.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
