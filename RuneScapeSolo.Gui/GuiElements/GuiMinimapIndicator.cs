using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Gui.GuiElements.Enumerations;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiMinimapIndicator : GuiElement
    {
        GuiImage indicator;

        public MinimapIndicator Type { get; set; }

        public int CurrentValue { get; set; }

        public int BaseValue { get; set; }

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

        public override void LoadContent()
        {
            indicator = new GuiImage { ContentFile = "Interface/Minimap/indicator_bg" };

            indicator.LoadContent();

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            indicator.UnloadContent();

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            indicator.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            indicator.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            indicator.Location = Location;
            indicator.TintColour = BackgroundColour;
            indicator.Size = new Size2D(Size.Width, (int)(Size.Height * FillLevel));
            indicator.SourceRectangle = new Rectangle2D(0, Size.Height - indicator.Size.Height, Size.Width, indicator.Size.Height);
            indicator.Location = new Point2D(Location.X, Location.Y + Size.Height - indicator.Size.Height);
        }
    }
}
