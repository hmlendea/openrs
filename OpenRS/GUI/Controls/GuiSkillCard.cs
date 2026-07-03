using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public class GuiSkillCard : GuiControl
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

        /// <summary>
        /// Loads the content.
        /// </summary>
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

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent() => UnregisterEvents();

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        /// <summary>
        /// Draw the content on the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Registers the events.
        /// </summary>
        void RegisterEvents()
        {
            MouseEntered += OnMouseEntered;
            MouseLeft += OnMouseLeft;
        }

        /// <summary>
        /// Unregisters the events.
        /// </summary>
        void UnregisterEvents()
        {
            MouseEntered -= OnMouseEntered;
            MouseLeft -= OnMouseLeft;
        }

        void SetChildrenProperties()
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

        void OnContentLoaded(object sender, MouseEventArgs e)
        {
            detailsBackground.Hide();
            detailsText.Hide();
        }

        void OnMouseEntered(object sender, MouseEventArgs e)
        {
            regularBackground.Hide();
            skillIcon.Hide();
            currentLevelText.Hide();
            baseLevelText.Hide();

            detailsBackground.Show();
            detailsText.Show();
        }

        void OnMouseLeft(object sender, MouseEventArgs e)
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
