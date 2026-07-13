using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSkillCard : GuiControl
    {
        private GuiImage regularBackground;
        private GuiImage detailsBackground;
        private GuiImage skillIcon;

        private GuiText currentLevelText;
        private GuiText baseLevelText;
        private GuiText detailsText;

        public string SkillIcon { get; set; }

        public int CurrentLevel { get; set; }

        public int BaseLevel { get; set; }

        public int Experience { get; set; }

        public string SkillDetails { get; set; }

        public GuiSkillCard()
        {
            Size = new Size2D(60, 32);
        }
        protected override void DoLoadContent()
        {
            regularBackground = new GuiImage { ContentFile = "Interface/skillcard" };
            detailsBackground = new GuiImage { ContentFile = "Interface/skillcard_details" };
            skillIcon = new GuiImage
            {
                ContentFile = SkillIcon,
                Size = new Size2D(26, 26)
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
            detailsText = new GuiText
            {
                FontName = "SkillCardFont",
                Size = new Size2D(12, 10),
                ForegroundColour = Colour.Yellow
            };

            RegisterChildren(regularBackground, detailsBackground, skillIcon);
            RegisterChildren(currentLevelText, baseLevelText, detailsText);
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
            MouseEntered += OnMouseEntered;
            MouseLeft += OnMouseLeft;
        }
        private void UnregisterEvents()
        {
            MouseEntered -= OnMouseEntered;
            MouseLeft -= OnMouseLeft;
        }

        private void SetChildrenProperties()
        {
            regularBackground.Location = new Point2D(0, 0);
            regularBackground.Size = Size;

            detailsBackground.Location = new Point2D(0, 0);
            detailsBackground.Size = Size;

            skillIcon.Location = new Point2D(
                (30 - skillIcon.Size.Width) / 2,
                (Size.Height - skillIcon.Size.Height) / 2);
            skillIcon.ContentFile = SkillIcon;

            currentLevelText.Text = CurrentLevel.ToString();
            currentLevelText.Location = new Point2D(32, 4);

            baseLevelText.Text = BaseLevel.ToString();
            baseLevelText.Location = new Point2D(44, 16);

            detailsText.Location = new Point2D(0, 0);
            detailsText.Size = Size;
            detailsText.Text = $"Xp:{Environment.NewLine}{Experience}";
        }

        private void OnMouseEntered(object sender, MouseEventArgs e)
        {
            regularBackground.Hide();
            skillIcon.Hide();
            currentLevelText.Hide();
            baseLevelText.Hide();

            detailsBackground.Show();
            detailsText.Show();
        }

        private void OnMouseLeft(object sender, MouseEventArgs e)
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
