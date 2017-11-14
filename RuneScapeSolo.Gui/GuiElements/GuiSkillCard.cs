using System;

using RuneScapeSolo.Graphics.Enumerations;
using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Graphics.Primitives.Mapping;
using RuneScapeSolo.Input;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiSkillCard : GuiElement
    {
        GuiImage background;
        GuiImage skillIcon;

        GuiText currentLevelText;
        GuiText baseLevelText;

        GuiTooltip tooltip;

        public string SkillIcon { get; set; }

        public int CurrentLevel { get; set; }

        public int BaseLevel { get; set; }

        public int Experience { get; set; }

        public GuiSkillCard()
        {
            Size = new Size2D(60, 32);
        }

        public override void LoadContent()
        {
            background = new GuiImage
            {
                Size = Size,
                ContentFile = "Interface/skillcard"
            };
            skillIcon = new GuiImage
            {
                ContentFile = SkillIcon
            };

            currentLevelText = new GuiText
            {
                FontName = "SkillCardFont",
                FontOutline = FontOutline.BottomRight,
                Size = new Size2D(12, 10),
                ForegroundColour = Colour.Yellow
            };
            baseLevelText = new GuiText
            {
                FontName = "SkillCardFont",
                FontOutline = FontOutline.BottomRight,
                Size = new Size2D(12, 10),
                ForegroundColour = Colour.Yellow
            };

            tooltip = new GuiTooltip
            {
                FontName = "SkillCardFont",
                Size = new Size2D(100, 26),
                BackgroundColour = Colour.Black,
                ForegroundColour = Colour.Yellow
            };

            Children.Add(background);
            Children.Add(skillIcon);
            Children.Add(currentLevelText);
            Children.Add(baseLevelText);
            Children.Add(tooltip);

            LinkEvents();

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            UnlinkEvents();

            base.UnloadContent();
        }

        protected override void SetChildrenProperties()
        {
            background.Location = Location;
            background.Size = Size;

            skillIcon.Location = new Point2D(Location.X + 5, Location.Y + 5);
            skillIcon.ContentFile = SkillIcon;

            currentLevelText.Text = CurrentLevel.ToString();
            currentLevelText.Location = new Point2D(Location.X + 32, Location.Y + 4);

            baseLevelText.Text = BaseLevel.ToString();
            baseLevelText.Location = new Point2D(Location.X + 44, Location.Y + 16);

            if (tooltip.Visible)
            {
                tooltip.Location = InputManager.Instance.MouseLocation.ToPoint2D();
                tooltip.Text = $"XP: {Experience}{Environment.NewLine}Next Level at: ";
            }

            base.SetChildrenProperties();
        }

        void LinkEvents()
        {
            MouseEntered += GuiSkillCard_MouseEntered;
            MouseLeft += GuiSkillCard_MouseLeft;
        }

        void UnlinkEvents()
        {
            MouseEntered -= GuiSkillCard_MouseEntered;
            MouseLeft -= GuiSkillCard_MouseLeft;
        }

        void GuiSkillCard_MouseEntered(object sender, Input.Events.MouseEventArgs e)
        {
            tooltip.Show();
        }

        void GuiSkillCard_MouseLeft(object sender, Input.Events.MouseEventArgs e)
        {
            tooltip.Hide();
        }
    }
}
