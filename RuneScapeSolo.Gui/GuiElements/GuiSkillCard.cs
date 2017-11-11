﻿using RuneScapeSolo.Graphics.Primitives;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiSkillCard : GuiElement
    {
        GuiImage background;
        GuiImage skillIcon;

        GuiText currentLevelText;
        GuiText baseLevelText;

        public string SkillIcon { get; set; }

        public int CurrentLevel { get; set; }

        public int BaseLevel { get; set; }

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
                Size = new Size2D(12, 10),
                ForegroundColour = Colour.Yellow
            };
            baseLevelText = new GuiText
            {
                FontName = "SkillCardFont",
                Size = new Size2D(12, 10),
                ForegroundColour = Colour.Yellow
            };

            Children.Add(background);
            Children.Add(skillIcon);
            Children.Add(currentLevelText);
            Children.Add(baseLevelText);

            base.LoadContent();
        }

        protected override void SetChildrenProperties()
        {
            background.Location = Location;
            background.Size = Size;

            skillIcon.Location = new Point2D(Location.X + 5, Location.Y + 5);
            skillIcon.ContentFile = SkillIcon;

            currentLevelText.Text = CurrentLevel.ToString();
            currentLevelText.Location = new Point2D(Location.X + 36, Location.Y + 4);

            baseLevelText.Text = BaseLevel.ToString();
            baseLevelText.Location = new Point2D(Location.X + 48, Location.Y + 16);

            base.SetChildrenProperties();
        }
    }
}