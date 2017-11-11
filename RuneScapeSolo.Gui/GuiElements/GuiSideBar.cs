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

        GuiButton combatButton;
        GuiButton skillsButton;
        GuiButton questsButton;
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

            combatButton = new GuiButton
            {
                Icon = "Interface/SideBar/combat_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE)
            };
            skillsButton = new GuiButton
            {
                Icon = "Interface/SideBar/skills_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE)
            };
            questsButton = new GuiButton
            {
                Icon = "Interface/SideBar/quests_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE)
            };
            inventoryButton = new GuiButton
            {
                Icon = "Interface/SideBar/inventory_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE),
                IsToggled = true
            };
            equipmentButton = new GuiButton
            {
                Icon = "Interface/SideBar/equipment_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE)
            };
            prayerButton = new GuiButton
            {
                Icon = "Interface/SideBar/prayer_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE)
            };
            spellsButton = new GuiButton
            {
                Icon = "Interface/SideBar/spells_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE)
            };
            exitButton = new GuiButton
            {
                Icon = "Interface/SideBar/exit_button_icon",
                Size = new Size2D(GameDefines.GUI_TILE_SIZE * 7, GameDefines.GUI_TILE_SIZE)
            };

            Children.Add(background);
            Children.Add(minimap);
            Children.Add(combatButton);
            Children.Add(skillsButton);
            Children.Add(questsButton);
            Children.Add(inventoryButton);
            Children.Add(equipmentButton);
            Children.Add(prayerButton);
            Children.Add(spellsButton);
            Children.Add(exitButton);

            LinkEvents();

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
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            background.Location = Location;
            background.Size = Size;

            minimap.Location = new Point2D(
                Location.X + (Size.Width - minimap.Size.Width) / 2,
                Location.Y + (Size.Width - minimap.Size.Width) / 2);

            combatButton.Location = new Point2D(
                Location.X + (Size.Width - GameDefines.GUI_TILE_SIZE * 7) / 2,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            skillsButton.Location = new Point2D(
                combatButton.ClientRectangle.Right,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            questsButton.Location = new Point2D(
                skillsButton.ClientRectangle.Right,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            inventoryButton.Location = new Point2D(
                questsButton.ClientRectangle.Right,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            equipmentButton.Location = new Point2D(
                inventoryButton.ClientRectangle.Right,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            prayerButton.Location = new Point2D(
                equipmentButton.ClientRectangle.Right,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            spellsButton.Location = new Point2D(
                prayerButton.ClientRectangle.Right,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE * 8);
            exitButton.Location = new Point2D(
                Location.X + (Size.Width - exitButton.Size.Width) / 2,
                ClientRectangle.Bottom - GameDefines.GUI_TILE_SIZE);
        }

        void LinkEvents()
        {
            combatButton.Clicked += CombatButton_Clicked;
            skillsButton.Clicked += SkillsButton_Clicked;
            questsButton.Clicked += QuestsButton_Clicked;
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
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;
        }

        void SkillsButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = true;
            questsButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;
        }

        void QuestsButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = true;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;
        }

        void InventoryButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            inventoryButton.IsToggled = true;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;
        }

        void EquipmentButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = true;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;
        }

        void PrayerButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = true;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = false;
        }

        void SpellsButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = true;
            exitButton.IsToggled = false;
        }

        void ExitButton_Clicked(object sender, Input.Events.MouseButtonEventArgs e)
        {
            combatButton.IsToggled = false;
            skillsButton.IsToggled = false;
            questsButton.IsToggled = false;
            inventoryButton.IsToggled = false;
            equipmentButton.IsToggled = false;
            prayerButton.IsToggled = false;
            spellsButton.IsToggled = false;
            exitButton.IsToggled = true;
        }
    }
}
