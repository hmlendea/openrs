using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Net.Client;
using OpenRS.Settings;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSideBar(GameClient client) : GuiControl
    {

        private GuiImage background;
        private GuiMinimap minimap;

        private GuiSideBarPanel panel;
        private GuiCombatPanel combatPanel;
        private GuiSkillsPanel skillsPanel;
        private GuiInventoryPanel inventoryPanel;

        private GuiToggleButton combatButton;
        private GuiToggleButton skillsButton;
        private GuiToggleButton questsButton;
        private GuiToggleButton tasksButton;
        private GuiToggleButton inventoryButton;
        private GuiToggleButton equipmentButton;
        private GuiToggleButton prayerButton;
        private GuiToggleButton spellsButton;
        private GuiToggleButton exitButton;
        protected override void DoLoadContent()
        {
            background = new GuiImage
            {
                ContentFile = "Interface/Backgrounds/sidebar",
                TextureLayout = TextureLayout.Tile
            };
            minimap = new GuiMinimap(client)
            {
                Size = new Size2D(224, 176)
            };
            panel = new GuiSideBarPanel
            {
                Size = new Size2D(240, 262)
            };
            combatPanel = new GuiCombatPanel(client)
            {
                Size = new Size2D(190, 262)
            };
            skillsPanel = new GuiSkillsPanel(client) {
                Size = new Size2D(190, 262)
            };
            inventoryPanel = new GuiInventoryPanel(client) {
                Size = new Size2D(190, 262)
            };

            combatButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/combat_button_icon",
                Size = new Size2D(30, 36)
            };
            skillsButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/skills_button_icon",
                Size = new Size2D(30, 36)
            };
            questsButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/quests_button_icon",
                Size = new Size2D(30, 36)
            };
            tasksButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/tasks_button_icon",
                Size = new Size2D(30, 36)
            };
            inventoryButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/inventory_button_icon",
                Size = new Size2D(30, 36)
            };
            equipmentButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/equipment_button_icon",
                Size = new Size2D(30, 36)
            };
            prayerButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/prayer_button_icon",
                Size = new Size2D(30, 36)
            };
            spellsButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/spells_button_icon",
                Size = new Size2D(30, 36)
            };
            exitButton = new GuiToggleButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/exit_button_icon",
                Size = new Size2D(240, 36)
            };

            RegisterChildren(background, minimap);
            RegisterChildren(panel, combatPanel, skillsPanel, inventoryPanel);
            RegisterChildren(
                combatButton,
                skillsButton,
                questsButton,
                tasksButton,
                inventoryButton,
                equipmentButton,
                prayerButton,
                spellsButton,
                exitButton);

            RegisterEvents();
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() => UnregisterEvents();
        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }
        private void RegisterEvents()
        {
            ContentLoaded += OnContentLoaded;

            combatButton.Clicked += OnCombatButtonClicked;
            skillsButton.Clicked += OnSkillsButtonClicked;
            questsButton.Clicked += OnQuestsButtonClicked;
            tasksButton.Clicked += OnTasksButtonClicked;
            inventoryButton.Clicked += OnInventoryButtonClicked;
            equipmentButton.Clicked += OnEquipmentButtonClicked;
            prayerButton.Clicked += OnPrayerButtonClicked;
            spellsButton.Clicked += OnSpellsButtonClicked;
            exitButton.Clicked += OnExitButtonClicked;
        }
        private void UnregisterEvents()
        {
            ContentLoaded -= OnContentLoaded;

            combatButton.Clicked -= OnCombatButtonClicked;
            skillsButton.Clicked -= OnSkillsButtonClicked;
            questsButton.Clicked -= OnQuestsButtonClicked;
            tasksButton.Clicked -= OnTasksButtonClicked;
            inventoryButton.Clicked -= OnInventoryButtonClicked;
            equipmentButton.Clicked -= OnEquipmentButtonClicked;
            prayerButton.Clicked -= OnPrayerButtonClicked;
            spellsButton.Clicked -= OnSpellsButtonClicked;
            exitButton.Clicked -= OnExitButtonClicked;
        }

        private void SetChildrenProperties()
        {
            minimap.Location = new Point2D(
                (Size.Width - minimap.Size.Width) / 2,
                (Size.Width - minimap.Size.Width) / 2);

            exitButton.Location = new Point2D(
                (Size.Width - exitButton.Size.Width) / 2,
                Size.Height - GameDefines.GuiTileSize);

            panel.Location = new Point2D(
                (Size.Width - panel.Size.Width) / 2,
                exitButton.Location.Y - panel.Size.Height);
            combatPanel.Location = new Point2D(
                panel.Location.X + 25,
                panel.Location.Y);
            skillsPanel.Location = combatPanel.Location;
            inventoryPanel.Location = skillsPanel.Location;

            combatButton.Location = new Point2D(
                panel.Location.X,
                panel.Location.Y - combatButton.Size.Height);
            skillsButton.Location = new Point2D(
                combatButton.ClientRectangle.Right,
                combatButton.Location.Y);
            questsButton.Location = new Point2D(
                skillsButton.ClientRectangle.Right,
                skillsButton.Location.Y);
            tasksButton.Location = new Point2D(
                questsButton.ClientRectangle.Right,
                questsButton.Location.Y);
            inventoryButton.Location = new Point2D(
                tasksButton.ClientRectangle.Right,
                tasksButton.Location.Y);
            equipmentButton.Location = new Point2D(
                inventoryButton.ClientRectangle.Right,
                inventoryButton.Location.Y);
            prayerButton.Location = new Point2D(
                equipmentButton.ClientRectangle.Right,
                equipmentButton.Location.Y);
            spellsButton.Location = new Point2D(
                prayerButton.ClientRectangle.Right,
                prayerButton.Location.Y);
        }

        private void OnContentLoaded(object sender, EventArgs e)
        {
            UnselectEverything();
            inventoryButton.IsToggled = true;
            inventoryPanel.Show();
        }

        private void OnCombatButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            combatButton.IsToggled = true;
            combatPanel.Show();
        }

        private void OnSkillsButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            skillsButton.IsToggled = true;
            skillsPanel.Show();
        }

        private void OnQuestsButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            questsButton.IsToggled = true;
        }

        private void OnTasksButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            tasksButton.IsToggled = true;
        }

        private void OnInventoryButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            inventoryButton.IsToggled = true;
            inventoryPanel.Show();
        }

        private void OnEquipmentButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            equipmentButton.IsToggled = true;
        }

        private void OnPrayerButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            prayerButton.IsToggled = true;
        }

        private void OnSpellsButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            spellsButton.IsToggled = true;
        }

        private void OnExitButtonClicked(object sender, MouseButtonEventArgs e)
        {
            UnselectEverything();

            exitButton.IsToggled = true;
        }

        private void UnselectEverything()
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            combatPanel.Hide();
            skillsPanel.Hide();
            inventoryPanel.Hide();
        }
    }
}
