using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiItemCard : GuiControl
    {
        private static string ItemSpritePathPrefix => "sprites/items/";

        private static Size2D IconSize => new(48, 32);

        private GuiImage slotBackground;
        private GuiImage icon;
        private GuiText quantity;

        public string SpriteName { get; set; }

        public int Quantity { get; set; }

        public GuiItemCard()
        {
            Size = new(36, 36);
        }

        protected override void DoLoadContent()
        {
            slotBackground = new GuiImage
            {
                Size = Size,
                ContentFile = "ScreenManager/FillImage",
                TintColour = new Colour(30, 20, 10)
            };
            icon = new GuiImage
            {
                Size = new(32, 32),
                ContentFile = "ScreenManager/missing-texture"
            };
            quantity = new GuiText
            {
                Size = new(Size.Width, 10),
                FontName = "ItemCardFont",
                FontOutline = FontOutline.BottomRight,
                VerticalAlignment = Alignment.Beginning,
                HorizontalAlignment = Alignment.Beginning
            };

            RegisterChildren(slotBackground, icon, quantity);
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
            if (Quantity > 0 && !string.IsNullOrEmpty(SpriteName))
            {
                string contentFile = ItemSpritePathPrefix + SpriteName;

                if (!string.Equals(icon.ContentFile, contentFile, System.StringComparison.Ordinal))
                {
                    icon.ContentFile = contentFile;
                }

                icon.Location = new(
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
                quantity.Location = new(0, 0);
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
