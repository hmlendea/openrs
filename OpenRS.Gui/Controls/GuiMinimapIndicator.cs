using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public class GuiMinimapIndicator : GuiControl
    {
        GuiImage indicator;
        GuiImage icon;

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

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            indicator = new GuiImage { ContentFile = "Interface/Minimap/indicator_bg" };
            icon = new GuiImage { ContentFile = Icon };
            
            RegisterChildren(indicator, icon);
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
        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();
        }

        /// <summary>
        /// Draw the content on the specified <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }

        void SetChildrenProperties()
        {
            indicator.Location = Location;
            indicator.TintColour = BackgroundColour;
            indicator.Size = new Size2D(Size.Width, (int)(Size.Height * FillLevel));
            indicator.SourceRectangle = new Rectangle2D(0, Size.Height - indicator.Size.Height, Size.Width, indicator.Size.Height);
            indicator.Location = new Point2D(Location.X, Location.Y + Size.Height - indicator.Size.Height);

            icon.Location = Location;
            icon.Rotation = IconRotation;
        }
    }
}
