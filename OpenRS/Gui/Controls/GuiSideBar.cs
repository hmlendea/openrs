using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Net.Client;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSideBar(GameClient client) : GuiControl
    {
        private static string BackgroundContentFile => "Interface/Backgrounds/sidebar";
        private static string ButtonTextureContentFile => "Interface/SideBar/button";
        private static string CombatButtonIconContentFile => "Interface/SideBar/combat_button_icon";
        private static string SkillsButtonIconContentFile => "Interface/SideBar/skills_button_icon";
        private static string QuestsButtonIconContentFile => "Interface/SideBar/quests_button_icon";
        private static string TasksButtonIconContentFile => "Interface/SideBar/tasks_button_icon";
        private static string InventoryButtonIconContentFile => "Interface/SideBar/inventory_button_icon";
        private static string EquipmentButtonIconContentFile => "Interface/SideBar/equipment_button_icon";
        private static string PrayerButtonIconContentFile => "Interface/SideBar/prayer_button_icon";
        private static string SpellsButtonIconContentFile => "Interface/SideBar/spells_button_icon";
        private static string ExitButtonIconContentFile => "Interface/SideBar/exit_button_icon";

        private static int MinimapWidth => 224;
        private static int MinimapHeight => 176;
        private static int PanelWidth => 240;
        private static int PanelHeight => 262;
        private static int SubPanelWidth => 190;
        private static int ButtonTileWidth => 30;
        private static int ButtonTileHeight => 36;
        private static int ExitButtonWidth => 240;
        private static int SubPanelHorizontalOffset => 25;

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
                ContentFile = BackgroundContentFile,
                TextureLayout = TextureLayout.Stretch
            };
            minimap = new GuiMinimap(client)
            {
                Size = new Size2D(MinimapWidth, MinimapHeight)
            };
            panel = new GuiSideBarPanel
            {
                Size = new Size2D(PanelWidth, PanelHeight)
            };
            combatPanel = new GuiCombatPanel(client)
            {
                Size = new Size2D(SubPanelWidth, PanelHeight)
            };
            skillsPanel = new GuiSkillsPanel(client)
            {
                Size = new Size2D(SubPanelWidth, PanelHeight)
            };
            inventoryPanel = new GuiInventoryPanel(client)
            {
                Size = new Size2D(SubPanelWidth, PanelHeight)
            };

            combatButton = CreateButton(CombatButtonIconContentFile);
            skillsButton = CreateButton(SkillsButtonIconContentFile);
            questsButton = CreateButton(QuestsButtonIconContentFile);
            tasksButton = CreateButton(TasksButtonIconContentFile);
            inventoryButton = CreateButton(InventoryButtonIconContentFile);
            equipmentButton = CreateButton(EquipmentButtonIconContentFile);
            prayerButton = CreateButton(PrayerButtonIconContentFile);
            spellsButton = CreateButton(SpellsButtonIconContentFile);
            exitButton = new GuiToggleButton
            {
                Texture = ButtonTextureContentFile,
                ButtonTileSize = new Size2D(ButtonTileWidth, ButtonTileHeight),
                Icon = ExitButtonIconContentFile,
                Size = new Size2D(ExitButtonWidth, ButtonTileHeight),
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

        private static GuiToggleButton CreateButton(string iconContentFile)
        {
            return new GuiToggleButton
            {
                Texture = ButtonTextureContentFile,
                ButtonTileSize = new Size2D(ButtonTileWidth, ButtonTileHeight),
                Icon = iconContentFile,
                Size = new Size2D(ButtonTileWidth, ButtonTileHeight)
            };
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
            UpdateMinimapLayout();
            UpdatePanelLayouts();
            UpdateButtonLayout();
        }

        private void UpdateMinimapLayout()
        {
            minimap.Location = new Point2D(
                (Size.Width - minimap.Size.Width) / 2,
                (Size.Width - minimap.Size.Width) / 2);
        }

        private void UpdatePanelLayouts()
        {
            exitButton.Location = new Point2D(
                (Size.Width - exitButton.Size.Width) / 2,
                Size.Height - exitButton.Size.Height);

            panel.Location = new Point2D(
                (Size.Width - panel.Size.Width) / 2,
                exitButton.Location.Y - panel.Size.Height);

            Point2D subPanelLocation = new(
                panel.Location.X + SubPanelHorizontalOffset,
                panel.Location.Y);

            combatPanel.Location = subPanelLocation;
            skillsPanel.Location = subPanelLocation;
            inventoryPanel.Location = subPanelLocation;
        }

        private void UpdateButtonLayout()
        {
            int buttonRowY = panel.Location.Y - combatButton.Size.Height;

            combatButton.Location = new Point2D(panel.Location.X, buttonRowY);
            skillsButton.Location = new Point2D(combatButton.ClientRectangle.Right, buttonRowY);
            questsButton.Location = new Point2D(skillsButton.ClientRectangle.Right, buttonRowY);
            tasksButton.Location = new Point2D(questsButton.ClientRectangle.Right, buttonRowY);
            inventoryButton.Location = new Point2D(tasksButton.ClientRectangle.Right, buttonRowY);
            equipmentButton.Location = new Point2D(inventoryButton.ClientRectangle.Right, buttonRowY);
            prayerButton.Location = new Point2D(equipmentButton.ClientRectangle.Right, buttonRowY);
            spellsButton.Location = new Point2D(prayerButton.ClientRectangle.Right, buttonRowY);
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
