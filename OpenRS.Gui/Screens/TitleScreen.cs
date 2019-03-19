using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.GuiElements;
using NuciXNA.Gui.Screens;

namespace OpenRS.Gui.Screens
{
    /// <summary>
    /// Title screen.
    /// </summary>
    public class TitleScreen : MenuScreen
    {
        GuiMenuLink newGameLink;
        GuiMenuLink settingsLink;
        GuiMenuAction extiAction;

        /// <summary>
        /// Loads the content.
        /// </summary>
        public override void LoadContent()
        {
            newGameLink = new GuiMenuLink
            {
                Id = "newGame",
                Text = "Login as 'test'",
                TargetScreen = typeof(GameplayScreen),
                Parameters = new object[] { "test", "test" }
            };

            settingsLink = new GuiMenuLink
            {
                Id = "settings",
                Text = "Settings",
                TargetScreen = typeof(SettingsScreen)
            };

            extiAction = new GuiMenuAction
            {
                Id = "exit",
                Text = "Exit",
                ActionId = "Exit"
            };

            Items.Add(newGameLink);
            Items.Add(settingsLink);
            Items.Add(extiAction);

            base.LoadContent();
        }
    }
}
