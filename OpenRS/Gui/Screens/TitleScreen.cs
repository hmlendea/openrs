using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;

namespace OpenRS.Gui.Screens
{
    public sealed class TitleScreen : MenuScreen
    {
        private static string TestUsername => "test";

        private static string TestPassword => "test";

        private GuiMenuLink newGameLink;
        private GuiMenuLink settingsLink;
        private GuiMenuItem exitAction;

        protected override void DoLoadContent()
        {
            newGameLink = new GuiMenuLink
            {
                Id = nameof(newGameLink),
                Text = $"Login as '{TestUsername}'",
                TargetScreen = typeof(GameplayScreen),
                Parameters = [TestUsername, TestPassword]
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
