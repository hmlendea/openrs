using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Localisation;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiSkillCard : GuiControl
    {
        private static string RegularBackgroundContentFile => "Interface/skillcard";
        private static string DetailsBackgroundContentFile => "Interface/skillcard_details";
        private static string SkillCardFontName => "SkillCardFont";

        private static int DefaultWidth => 60;
        private static int DefaultHeight => 32;
        private static int SkillIconSize => 26;
        private static int LevelTextWidth => 12;
        private static int LevelTextHeight => 10;
        private static int IconAreaWidth => 30;
        private static int CurrentLevelTextLocationX => 32;
        private static int CurrentLevelTextLocationY => 4;
        private static int BaseLevelTextLocationX => 44;
        private static int BaseLevelTextLocationY => 16;

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

        public GuiSkillCard()
        {
            Size = new Size2D(DefaultWidth, DefaultHeight);
        }

        protected override void DoLoadContent()
        {
            regularBackground = new GuiImage { ContentFile = RegularBackgroundContentFile };
            detailsBackground = new GuiImage { ContentFile = DetailsBackgroundContentFile };
            skillIcon = new GuiImage
            {
                ContentFile = SkillIcon,
                Size = new Size2D(SkillIconSize, SkillIconSize)
            };

            currentLevelText = CreateLevelText();
            baseLevelText = CreateLevelText();
            detailsText = new GuiText
            {
                FontName = SkillCardFontName,
                Size = new Size2D(LevelTextWidth, LevelTextHeight),
                ForegroundColour = Colour.Yellow
            };

            RegisterChildren(regularBackground, detailsBackground, skillIcon);
            RegisterChildren(currentLevelText, baseLevelText, detailsText);
            RegisterEvents();
            SetChildrenProperties();

            detailsBackground.Hide();
            detailsText.Hide();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private static GuiText CreateLevelText()
        {
            return new GuiText
            {
                FontName = SkillCardFontName,
                FontOutline = FontOutline.BottomRight,
                Size = new Size2D(LevelTextWidth, LevelTextHeight),
                ForegroundColour = Colour.Yellow
            };
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
            UpdateBackgroundProperties();
            UpdateIconProperties();
            UpdateLevelTextProperties();
            UpdateDetailsProperties();
        }

        private void UpdateBackgroundProperties()
        {
            regularBackground.Location = Point2D.Empty;
            regularBackground.Size = Size;

            detailsBackground.Location = Point2D.Empty;
            detailsBackground.Size = Size;
        }

        private void UpdateIconProperties()
        {
            skillIcon.Location = new Point2D(
                (IconAreaWidth - skillIcon.Size.Width) / 2,
                (Size.Height - skillIcon.Size.Height) / 2);
            skillIcon.ContentFile = SkillIcon;
        }

        private void UpdateLevelTextProperties()
        {
            currentLevelText.Text = CurrentLevel.ToString();
            currentLevelText.Location = new Point2D(
                CurrentLevelTextLocationX,
                CurrentLevelTextLocationY);

            baseLevelText.Text = BaseLevel.ToString();
            baseLevelText.Location = new Point2D(
                BaseLevelTextLocationX,
                BaseLevelTextLocationY);
        }

        private void UpdateDetailsProperties()
        {
            detailsText.Location = Point2D.Empty;
            detailsText.Size = Size;
            detailsText.Text = $"{LocalisationManager.GetString("player.skill_xp_label")}:{Environment.NewLine}{Experience}";
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
