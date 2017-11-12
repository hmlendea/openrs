using Microsoft.Xna.Framework;

using RuneScapeSolo.Graphics.Enumerations;
using RuneScapeSolo.Graphics.Primitives;

namespace RuneScapeSolo.Gui.GuiElements
{
    public class GuiItemCard : GuiElement
    {
        GuiImage icon;
        GuiText quantity;

        public int ItemPictureId { get; set; }

        public int Quantity { get; set; }

        public GuiItemCard()
        {
            Size = new Size2D(40, 40);
        }

        public override void LoadContent()
        {
            icon = new GuiImage
            {
                Size = new Size2D(32, 32),
                ContentFile = "Interface/items"
            };
            quantity = new GuiText
            {
                Size = new Size2D(Size.Width, 10),
                FontName = "ItemCardFont",
                HorizontalAlignment = HorizontalAlignment.Top,
                VerticalAlignment = VerticalAlignment.Left
            };

            Children.Add(icon);
            Children.Add(quantity);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Quantity == 1)
            {
                quantity.Visible = false;
            }
            else
            {
                quantity.Visible = true;
            }

            base.Update(gameTime);
        }

        protected override void SetChildrenProperties()
        {
            base.SetChildrenProperties();

            icon.SourceRectangle = CalculateIconSourceRectangle(ItemPictureId);
            icon.Location = new Point2D(
                Location.X + (Size.Width - icon.Size.Width) / 2,
                Location.Y + (Size.Height - icon.Size.Height) / 2);

            quantity.Location = Location;
            quantity.Text = Quantity.ToString();
        }

        Rectangle2D CalculateIconSourceRectangle(int id)
        {
            return new Rectangle2D(id / 32 * icon.Size.Width,
                                   id % 32 * icon.Size.Height,
                                   icon.Size.Width,
                                   icon.Size.Height);
        }
    }
}
