using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiItemCard : GuiControl
    {
        private static int SpriteRows => 32;

        private static int SpriteColumns => 32;

        private GuiImage icon;
        private GuiText quantity;

        public int ItemPictureId { get; set; }

        public int Quantity { get; set; }

        public GuiItemCard()
        {
            Size = new Size2D(36, 36);
        }
        protected override void DoLoadContent()
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
                FontOutline = FontOutline.BottomRight,
                VerticalAlignment = Alignment.Beginning,
                HorizontalAlignment = Alignment.Beginning
            };

            RegisterChildren(icon, quantity);
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            if (Quantity > 0)
            {
                icon.SourceRectangle = CalculateIconSourceRectangle(ItemPictureId);
                icon.Location = new Point2D(
                    (Size.Width - icon.Size.Width) / 2,
                    (Size.Height - icon.Size.Height) / 2);

                if (!icon.IsVisible)
                {
                    icon.Show();
                }
            }
            else
            {
                icon.Hide();
            }

            if (Quantity > 1)
            {
                quantity.Location = new Point2D(0, 0);
                quantity.Text = Quantity.ToString();

                if (!quantity.IsVisible)
                {
                    quantity.Show();
                }
            }
            else
            {
                quantity.Hide();
            }
        }

        private Rectangle2D CalculateIconSourceRectangle(int id)
        {
            int columnIndex = id % SpriteColumns;
            int rowIndex = id / SpriteRows;

            return new Rectangle2D(
                columnIndex * icon.Size.Width,
                rowIndex * icon.Size.Height,
                icon.Size.Width,
                icon.Size.Height);
        }
    }
}
