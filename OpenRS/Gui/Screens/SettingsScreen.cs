using System;

using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;

using OpenRS.Settings;

namespace OpenRS.Gui.Screens
{
    public sealed class SettingsScreen : MenuScreen
    {
        private GuiMenuToggle debugModeToggle;
        private GuiMenuToggle fowToggle;
        private GuiMenuToggle roofsToggle;
        private GuiMenuLink backLink;
        protected override void DoLoadContent()
        {
            debugModeToggle = new GuiMenuToggle
            {
                Id = nameof(debugModeToggle),
                Text = "Toggle debug mode"
            };
            fowToggle = new GuiMenuToggle
            {
                Id = nameof(fowToggle),
                Text = "Toggle fog"
            };
            roofsToggle = new GuiMenuToggle
            {
                Id = nameof(roofsToggle),
                Text = "Toggle roofs"
            };
            backLink = new GuiMenuLink
            {
                Id = nameof(backLink),
                Text = "Back",
                TargetScreen = typeof(TitleScreen)
            };

            Items.Add(debugModeToggle);
            Items.Add(fowToggle);
            Items.Add(roofsToggle);
            Items.Add(backLink);

            RegisterEvents();
            SetChildrenProperties();

            base.DoLoadContent();
        }

        protected override void DoUnloadContent()
        {
            SettingsManager.Instance.SaveContent();

            UnregisterEvents();

            base.DoUnloadContent();
        }

        private void RegisterEvents()
        {
            debugModeToggle.Triggered += OnDebugModeToggleTriggered;
            fowToggle.Triggered += OnFowToggleTriggered;
            roofsToggle.Triggered += OnRoofsToggleTriggered;
        }

        private void UnregisterEvents()
        {
            debugModeToggle.Triggered -= OnDebugModeToggleTriggered;
            fowToggle.Triggered -= OnFowToggleTriggered;
            roofsToggle.Triggered -= OnRoofsToggleTriggered;
        }

        private void SetChildrenProperties()
        {
            debugModeToggle.SetState(SettingsManager.Instance.DebugMode);
            fowToggle.SetState(SettingsManager.Instance.GraphicsSettings.FogOfWar);
            roofsToggle.SetState(SettingsManager.Instance.GraphicsSettings.ShowRoofs);
        }

        private void OnDebugModeToggleTriggered(object sender, EventArgs e) => SettingsManager.Instance.DebugMode = debugModeToggle.IsOn;

        private void OnFowToggleTriggered(object sender, EventArgs e) => SettingsManager.Instance.GraphicsSettings.FogOfWar = fowToggle.IsOn;

        private void OnRoofsToggleTriggered(object sender, EventArgs e) => SettingsManager.Instance.GraphicsSettings.ShowRoofs = roofsToggle.IsOn;
    }
}
