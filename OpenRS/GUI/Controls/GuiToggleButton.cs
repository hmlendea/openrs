using Microsoft.Xna.Framework;

using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiToggleButton : GuiButton
    {
        public bool IsToggled { get; set; }

        public Colour ToggleColour { get; set; }

        public GuiToggleButton()
        {
            ToggleColour = Colour.DarkRed;
        }
        protected override void DoLoadContent()
        {
            base.DoLoadContent();

            SetChildrenProperties();
        }
        protected override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            SetChildrenProperties();
        }

        private void SetChildrenProperties()
        {
            for (int i = 0; i < images.Count; i += 1)
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
                return new Rectangle2D(
                    rect.X + 4 * ButtonTileSize.Width,
                    rect.Y,
                    rect.Width,
                    rect.Height);
            }
            else
            {
                return rect;
            }
        }
    }
}
