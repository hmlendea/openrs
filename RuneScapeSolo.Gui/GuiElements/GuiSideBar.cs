using RuneScapeSolo.Graphics.Enumerations;
using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Net.Client;
using RuneScapeSolo.Settings;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiSideBar : GuiElement
    {
        GameClient client;

        GuiImage background;
        GuiMinimap minimap;
        GuiSideBarPanel panel;

        GuiSkillsPanel skillsPanel;

        GuiButton combatButton;
        GuiButton skillsButton;
        GuiButton questsButton;
        GuiButton tasksButton;
        GuiButton inventoryButton;
        GuiButton equipmentButton;
        GuiButton prayerButton;
        GuiButton spellsButton;
        GuiButton exitButton;

        public override void LoadContent()
        {
            background = new GuiImage
            {
                ContentFile = "Interface/Backgrounds/sidebar",
                TextureLayout = TextureLayout.Tile
            };
            minimap = new GuiMinimap { Size = new Size2D(224, 176) };
            panel = new GuiSideBarPanel { Size = new Size2D(240, 262) };
            skillsPanel = new GuiSkillsPanel { Size = new Size2D(180, 252) };

            combatButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/combat_button_icon",
                Size = new Size2D(30, 36)
            };
            skillsButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/skills_button_icon",
                Size = new Size2D(30, 36)
            };
            questsButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/quests_button_icon",
                Size = new Size2D(30, 36)
            };
            tasksButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/tasks_button_icon",
                Size = new Size2D(30, 36)
            };
            inventoryButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/inventory_button_icon",
                Size = new Size2D(30, 36)
            };
            equipmentButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/equipment_button_icon",
                Size = new Size2D(30, 36)
            };
            prayerButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/prayer_button_icon",
                Size = new Size2D(30, 36)
            };
            spellsButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/spells_button_icon",
                Size = new Size2D(30, 36)
            };
            exitButton = new GuiButton
            {
                Texture = "Interface/SideBar/button",
                ButtonTileSize = new Size2D(30, 36),
                Icon = "Interface/SideBar/exit_button_icon",
                Size = new Size2D(240, 36)
            };

            Children.Add(background);
            Children.Add(minimap);

            Children.Add(panel);
            Children.Add(skillsPanel);

            Children.Add(combatButton);
            Children.Add(skillsButton);
            Children.Add(questsButton);
            Children.Add(tasksButton);
            Children.Add(inventoryButton);
            Children.Add(equipmentButton);
            Children.Add(prayerButton);
            Children.Add(spellsButton);
            Children.Add(exitButton);

            LinkEvents();

            InventoryButton_Clicked(this, null);

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            UnlinkEvents();

            base.UnloadContent();
        }

        public void AssociateGameClient(ref GameClient client)
        {
            this.client = client;

            minimap.AssociateGameClient(ref client);
            skillsPanel.AssociateGameClient(ref client);
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            background.Location = Location;
            background.Size = Size;

            minimap.Location = new Point2D(
                Location.X + (Size.Width - minimap.Size.Width) / 2,
                Location.Y + (Size.Width - minimap.Size.Width) / 2);

            exitButton.Location = new Point2D(
                Location.X + (Size.Width - exitButton.Size.Width) / 2,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE);

            panel.Location = new Point2D(
                Location.X + (Size.Width - panel.Size.Width) / 2,
                exitButton.Location.Y - panel.Size.Height);
            skillsPanel.Location = new Point2D(
                panel.Location.X + 30,
                panel.Location.Y + 5);

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

        void LinkEvents()
        {
            combatButton.Clicked += CombatButton_Clicked;
            skillsButton.Clicked += SkillsButton_Clicked;
            questsButton.Clicked += QuestsButton_Clicked;
            tasksButton.Clicked += TasksButton_Clicked;
            inventoryButton.Clicked += InventoryButton_Clicked;
            equipmentButton.Clicked += EquipmentButton_Clicked;
            prayerButton.Clicked += PrayerButton_Clicked;
            spellsButton.Clicked += SpellsButton_Clicked;
            exitButton.Clicked += ExitButton_Clicked;
        }

        void UnlinkEvents()
        {
            combatButton.Clicked -= CombatButton_Clicked;
            skillsButton.Clicked -= SkillsButton_Clicked;
            questsButton.Clicked -= QuestsButton_Clicked;
            tasksButton.Clicked -= TasksButton_Clicked;
            inventoryButton.Clicked -= InventoryButton_Clicked;
            equipmentButton.Clicked -= EquipmentButton_Clicked;
            prayerButton.Clicked -= PrayerButton_Clicked;
            spellsButton.Clicked -= SpellsButton_Clicked;
            exitButton.Clicked -= ExitButton_Clicked;
        }

        void CombatButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = true;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void SkillsButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = true;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = true;
        }

        void QuestsButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = true;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void TasksButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = true;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void InventoryButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = true;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void EquipmentButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = true;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void PrayerButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = true;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void SpellsButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = true;
            exitButton.IsToggled = false;

            skillsPanel.Visible = false;
        }

        void ExitButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            tasksButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = true;

            skillsPanel.Visible = false;
        }
    }
}
