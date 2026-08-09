using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiMinimapIndicator : GuiControl
    {
        private static string IndicatorBackgroundContentFile => "Interface/Minimap/indicator_bg";
        private static int DefaultSize => 22;
        private static float FullFillLevel => 1.0f;

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
                float fillLevel = FullFillLevel;

                if (CurrentValue != BaseValue)
                {
                    fillLevel = (float)CurrentValue / BaseValue;
                }

                return fillLevel;
            }
        }

        public GuiMinimapIndicator()
        {
            Size = new Size2D(DefaultSize, DefaultSize);
            BackgroundColour = Colour.White;
        }

        protected override void DoLoadContent()
        {
            indicator = new GuiImage { ContentFile = IndicatorBackgroundContentFile };
            icon = new GuiImage { ContentFile = Icon };

            RegisterChildren(indicator, icon);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent()
        {
        }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void SetChildrenProperties()
        {
            UpdateIndicatorProperties();
            UpdateIconProperties();
        }

        private void UpdateIndicatorProperties()
        {
            indicator.TintColour = BackgroundColour;
            indicator.Location = new Point2D(0, Size.Height - indicator.Size.Height);
            indicator.Size = new Size2D(Size.Width, (int)(Size.Height * FillLevel));
            indicator.SourceRectangle = new Rectangle2D(
                0,
                Size.Height - indicator.Size.Height,
                Size.Width,
                indicator.Size.Height);
        }

        private void UpdateIconProperties()
        {
            icon.Rotation = IconRotation;
            icon.Location = new Point2D(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - icon.Size.Height) / 2);
        }
    }
}
