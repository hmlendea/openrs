using System;

using RuneScapeSolo.Graphics.Enumerations;
using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Graphics.Primitives.Mapping;
using RuneScapeSolo.Input;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiSkillCard : GuiElement
    {
        GuiImage regularBackground;
        GuiImage detailsBackground;
        GuiImage skillIcon;

        GuiText currentLevelText;
        GuiText baseLevelText;
        GuiText detailsText;

        public string SkillIcon { get; set; }

        public int CurrentLevel { get; set; }

        public int BaseLevel { get; set; }

        public int Experience { get; set; }

        public string SkillDetails { get; set; }

        public GuiSkillCard()
        {
            Size = new Size2D(60, 32);
        }

        public override void LoadContent()
        {
            regularBackground = new GuiImage { ContentFile = "Interface/skillcard" };
            detailsBackground = new GuiImage { ContentFile = "Interface/skillcard_details" };
            skillIcon = new GuiImage { ContentFile = SkillIcon };

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
            detailsText = new GuiText
            {
                FontName = "SkillCardFont",
                Size = new Size2D(12, 10),
                ForegroundColour = Colour.Yellow
            };

            Children.Add(regularBackground);
            Children.Add(detailsBackground);
            Children.Add(skillIcon);

            Children.Add(currentLevelText);
            Children.Add(baseLevelText);
            Children.Add(detailsText);

            LinkEvents();

            detailsBackground.Visible = false;
            detailsText.Visible = false;

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            UnlinkEvents();

            base.UnloadContent();
        }

        protected override void SetChildrenProperties()
        {
            regularBackground.Location = Location;
            regularBackground.Size = Size;

            detailsBackground.Location = Location;
            detailsBackground.Size = Size;

            skillIcon.Location = new Point2D(Location.X + 5, Location.Y + 5);
            skillIcon.ContentFile = SkillIcon;

            currentLevelText.Text = CurrentLevel.ToString();
            currentLevelText.Location = new Point2D(Location.X + 32, Location.Y + 4);

            baseLevelText.Text = BaseLevel.ToString();
            baseLevelText.Location = new Point2D(Location.X + 44, Location.Y + 16);

            detailsText.Location = Location;
            detailsText.Size = Size;
            detailsText.Text = $"Xp:{Environment.NewLine}{Experience}";

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
            regularBackground.Hide();
            skillIcon.Hide();
            currentLevelText.Hide();
            baseLevelText.Hide();

            detailsBackground.Show();
            detailsText.Show();
        }

        void GuiSkillCard_MouseLeft(object sender, Input.Events.MouseEventArgs e)
        {
            regularBackground.Show();
            skillIcon.Show();
            currentLevelText.Show();
            baseLevelText.Show();

            detailsBackground.Hide();
            detailsText.Hide();
        }
    }
}
