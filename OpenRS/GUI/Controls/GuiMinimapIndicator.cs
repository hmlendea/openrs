using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiMinimapIndicator : GuiControl
    {
        private GuiImage indicator;
        private GuiImage icon;

        public int CurrentValue { get; set; }

        public int BaseValue { get; set; }

        public string Icon { get; set; }

        public float IconRotation { get; set; }

        public float FillLevel
        {
            get
            {
                if (CurrentValue == BaseValue)
                {
                    return 1.0f;
                }

                return (float)CurrentValue / BaseValue;
            }
        }

        public GuiMinimapIndicator()
        {
            Size = new Size2D(22, 22);
            BackgroundColour = Colour.White;
        }

        protected override void DoLoadContent()
        {
            indicator = new GuiImage { ContentFile = "Interface/Minimap/indicator_bg" };
            icon = new GuiImage { ContentFile = Icon };

            RegisterChildren(indicator, icon);
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            indicator.Location = new Point2D(0, 0);
            indicator.TintColour = BackgroundColour;
            indicator.Size = new Size2D(Size.Width, (int)(Size.Height * FillLevel));
            indicator.SourceRectangle = new Rectangle2D(0, Size.Height - indicator.Size.Height, Size.Width, indicator.Size.Height);
            indicator.Location = new Point2D(0, Size.Height - indicator.Size.Height);

            icon.Location = new Point2D(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - icon.Size.Height) / 2);
            icon.Rotation = IconRotation;
        }
    }
}
