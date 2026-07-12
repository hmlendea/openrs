using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;

namespace OpenRS.Gui.Screens
{
    /// <summary>
    /// Title screen.
    /// </summary>
    public sealed class TitleScreen : MenuScreen
    {
        private GuiMenuLink newGameLink;
        private GuiMenuLink settingsLink;
        private GuiMenuItem extiAction;

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            newGameLink = new GuiMenuLink
            {
                Id = nameof(newGameLink),
                Text = "Login as 'test'",
                TargetScreen = typeof(GameplayScreen),
                Parameters = ["test", "test"]
            };

            settingsLink = new GuiMenuLink
            {
                Id = nameof(settingsLink),
                Text = "Settings",
                TargetScreen = typeof(SettingsScreen)
            };

            extiAction = new GuiMenuItem
            {
                Id = nameof(extiAction),
                Text = "Exit"
            };

            Items.Add(newGameLink);
            Items.Add(settingsLink);
            Items.Add(extiAction);

            base.DoLoadContent();
        }
    }
}
