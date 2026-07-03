using Microsoft.Xna.Framework;

using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public class GuiToggleButton : GuiButton
    {
        public bool IsToggled { get; set; }

        public Colour ToggleColour { get; set; }

        public GuiToggleButton()
        {
            ToggleColour = Colour.DarkRed;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            base.DoLoadContent();

            SetChildrenProperties();
        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            SetChildrenProperties();
        }

        void SetChildrenProperties()
        {
            for (int i = 0; i < images.Count; i++)
            {
                if (IsToggled)
                {
                    images[i].TintColour = ToggleColour;
                }
                else
                {
                    images[i].TintColour = Colour.White;
                }
            }
        }

        protected override Rectangle2D CalculateSourceRectangle(int x)
        {
            Rectangle2D rect = base.CalculateSourceRectangle(x);

            if (IsToggled && !IsHovered)
            {
                return new Rectangle2D(rect.X + 4 * ButtonTileSize.Width, rect.Y, rect.Width, rect.Height);
            }
            else
            {
                return rect;
            }
        }
    }
}
