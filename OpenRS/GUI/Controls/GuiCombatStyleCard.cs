using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Primitives;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;

namespace OpenRS.Gui.Controls
{
    public class GuiCombatStyleCard : GuiControl
    {
        private GuiImage background;
        private GuiImage icon;
        private GuiText nameText;

        public bool IsToggled { get; set; }

        public string Icon { get; set; }

        public string CombatStyleName { get; set; }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            background = new GuiImage { ContentFile = "Interface/combatcard" };
            icon = new GuiImage();
            nameText = new GuiText
            {
                FontName = "SkillCardFont",
                FontOutline = FontOutline.BottomRight,
                VerticalAlignment = Alignment.Beginning
            };

            RegisterChildren(background, icon, nameText);
            SetChildrenProperties();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {

        }

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

        private void SetChildrenProperties()
        {
            background.Size = Size;
            background.Location = new Point2D(0, 0);

            if (IsToggled)
            {
                background.TintColour = Colour.Red;
            }
            else
            {
                background.TintColour = Colour.White;
            }

            icon.ContentFile = Icon;
            icon.Location = new Point2D(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - nameText.Size.Height - icon.Size.Height) / 2);

            nameText.Size = new Size2D(Size.Width, 14);
            nameText.Location = new Point2D(0, Size.Height - nameText.Size.Height);
            nameText.Text = CombatStyleName;
            nameText.ForegroundColour = ForegroundColour;
        }
    }
}
