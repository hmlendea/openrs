using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;

namespace OpenRS.Gui.Screens
{
    public sealed class TitleScreen : MenuScreen
    {
        private GuiMenuLink newGameLink;
        private GuiMenuLink settingsLink;
        private GuiMenuItem exitAction;

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

            exitAction = new GuiMenuItem
            {
                Id = nameof(exitAction),
                Text = "Exit"
            };

            Items.Add(newGameLink);
            Items.Add(settingsLink);
            Items.Add(exitAction);

            base.DoLoadContent();
        }
    }
}
