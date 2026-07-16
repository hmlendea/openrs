using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiItemCard : GuiControl
    {
        private static string ItemSpritePathPrefix => "sprites/items/";

        private static string QuantityFontName => "ItemCardFont";
        private static int QuantityTextHeight => 10;
        private static int DefaultCardSize => 36;
        private static int IconRenderSize => 32;

        private GuiImage icon;
        private GuiText quantity;

        public string SpriteName { get; set; }

        public int Quantity { get; set; }

        public GuiItemCard()
        {
            Size = new Size2D(DefaultCardSize, DefaultCardSize);
        }

        protected override void DoLoadContent()
        {
            icon = new GuiImage
            {
                Size = new Size2D(IconRenderSize, IconRenderSize),
                ContentFile = NuciContentManager.MissingTexturePlaceholder
            };
            quantity = new GuiText
            {
                Size = new Size2D(Size.Width, QuantityTextHeight),
                FontName = QuantityFontName,
                FontOutline = FontOutline.BottomRight,
                VerticalAlignment = Alignment.Beginning,
                HorizontalAlignment = Alignment.Beginning
            };

            RegisterChildren(icon, quantity);
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
            UpdateIconProperties();
            UpdateQuantityProperties();
        }

        private void UpdateIconProperties()
        {
            if (Quantity > 0 && !string.IsNullOrEmpty(SpriteName))
            {
                string contentFile = ItemSpritePathPrefix + SpriteName;

                if (!string.Equals(icon.ContentFile, contentFile, StringComparison.Ordinal))
                {
                    icon.ContentFile = contentFile;
                }

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
        }

        private void UpdateQuantityProperties()
        {
            if (Quantity > 1)
            {
                quantity.Location = Point2D.Empty;
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
    }
}
