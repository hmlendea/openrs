using Microsoft.Xna.Framework;

using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiToggleButton : GuiButton
    {
        private static int ToggledSectionOffset => 4;

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
            for (int imageIndex = 0; imageIndex < images.Count; imageIndex += 1)
            {
                images[imageIndex].TintColour = Colour.White;

                if (IsToggled)
                {
                    images[imageIndex].TintColour = ToggleColour;
                }
            }
        }

        protected override Rectangle2D CalculateSourceRectangle(int sectionIndex)
        {
            Rectangle2D rect = base.CalculateSourceRectangle(sectionIndex);

            if (IsToggled && !IsHovered)
            {
                return new Rectangle2D(
                    rect.X + ToggledSectionOffset * ButtonTileSize.Width,
                    rect.Y,
                    rect.Width,
                    rect.Height);
            }

            return rect;
        }
    }
}
