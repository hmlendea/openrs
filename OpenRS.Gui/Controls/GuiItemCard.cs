using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public class GuiItemCard : GuiControl
    {
        const int SpriteRows = 32;
        const int SpriteColumns = 32;

        GuiImage icon;
        GuiText quantity;

        public int ItemPictureId { get; set; }

        public int Quantity { get; set; }

        public GuiItemCard()
        {
            Size = new Size2D(36, 36);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
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
            if (Quantity > 0)
            {
                icon.SourceRectangle = CalculateIconSourceRectangle(ItemPictureId);
                icon.Location = new Point2D(
                    Location.X + (Size.Width - icon.Size.Width) / 2,
                    Location.Y + (Size.Height - icon.Size.Height) / 2);

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
                quantity.Location = Location;
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

        Rectangle2D CalculateIconSourceRectangle(int id)
        {
            int x = id % SpriteColumns;
            int y = id / SpriteRows;

            return new Rectangle2D(x * icon.Size.Width,
                                   y * icon.Size.Height,
                                   icon.Size.Width,
                                   icon.Size.Height);
        }
    }
}
