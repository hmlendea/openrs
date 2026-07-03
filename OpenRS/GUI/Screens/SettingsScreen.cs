
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
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

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {
            SettingsManager.Instance.SaveContent();

            UnregisterEvents();

            base.DoUnloadContent();
        }

        /// <summary>
        /// Registers the events.
        /// </summary>
        void RegisterEvents()
        {
            debugModeToggle.Triggered += OnDebugModeToggleTriggered;
            fowToggle.Triggered += OnFowToggleTriggered;
            roofsToggle.Triggered += OnRoofsToggleTriggered;
        }

        /// <summary>
        /// Unregisters the events.
        /// </summary>
        void UnregisterEvents()
        {
            debugModeToggle.Triggered -= OnDebugModeToggleTriggered;
            fowToggle.Triggered -= OnFowToggleTriggered;
            roofsToggle.Triggered -= OnRoofsToggleTriggered;
        }

        void SetChildrenProperties()
        {
            debugModeToggle.SetState(SettingsManager.Instance.DebugMode);
            fowToggle.SetState(SettingsManager.Instance.GraphicsSettings.FogOfWar);
            roofsToggle.SetState(SettingsManager.Instance.GraphicsSettings.ShowRoofs);
        }

        void OnDebugModeToggleTriggered(object sender, EventArgs e)
        {
            SettingsManager.Instance.DebugMode = debugModeToggle.IsOn;
        }

        void OnFowToggleTriggered(object sender, EventArgs e)
        {
            SettingsManager.Instance.GraphicsSettings.FogOfWar = fowToggle.IsOn;
        }

        void OnRoofsToggleTriggered(object sender, EventArgs e)
        {
            SettingsManager.Instance.GraphicsSettings.ShowRoofs = roofsToggle.IsOn;
        }
    }
}
