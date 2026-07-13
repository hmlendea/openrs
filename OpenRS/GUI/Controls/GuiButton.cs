using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Settings;

namespace OpenRS.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public Size2D ButtonTileSize { get; set; }
        public Size2D ButtonSize => new(
            Size.Width / ButtonTileSize.Width,
            Size.Height / ButtonTileSize.Height);
        public string Text { get; set; }

        public string Icon { get; set; }

        public string Texture { get; set; }

        protected List<GuiImage> images;
        private GuiImage icon;
        private GuiText text;
        public GuiButton()
        {
            Texture = "Interface/button";
            FontName = "ButtonFont";
            ButtonTileSize = new Size2D(GameDefines.GuiTileSize, GameDefines.GuiTileSize);
        }
        protected override void DoLoadContent()
        {
            icon = new GuiImage();
            images = [];
            text = new GuiText();

            for (int x = 0; x < ButtonSize.Width; x += 1)
            {
                GuiImage image = new() { SourceRectangle = CalculateSourceRectangle(x) };

                images.Add(image);
            }

            RegisterChildren(images);
            RegisterChild(text);

            if (!string.IsNullOrWhiteSpace(Icon))
            {
                RegisterChild(icon);
            }

            RegisterEvents();
            SetChildrenProperties();
        }
        protected override void DoUnloadContent() => UnregisterEvents();
        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }
        private void RegisterEvents()
        {
            Clicked += OnClicked;
            MouseEntered += OnMouseEntered;
        }
        private void UnregisterEvents()
        {
            Clicked -= OnClicked;
            MouseEntered -= OnMouseEntered;
        }

        private void SetChildrenProperties()
        {
            for (int i = 0; i < images.Count; i += 1)
            {
                images[i].ContentFile = Texture;
                images[i].Location = new Point2D(i * ButtonTileSize.Width, 0);
                images[i].SourceRectangle = CalculateSourceRectangle(i);
            }

            text.Text = Text;
            text.ForegroundColour = ForegroundColour;
            text.FontName = FontName;
            text.Location = new Point2D(0, 0);
            text.Size = Size;

            icon.ContentFile = Icon;
            icon.Location = new Point2D(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - icon.Size.Height) / 2);
        }
        private void OnClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.Button != MouseButton.Left)
            {
                return;
            }

            //AudioManager.Instance.PlaySound("Interface/click");
        }
        private void OnMouseEntered(object sender, MouseEventArgs e)
        {
            //AudioManager.Instance.PlaySound("Interface/select");
        }

        protected virtual Rectangle2D CalculateSourceRectangle(int x)
        {
            int sx = 1;

            if (ButtonSize.Width == 1)
            {
                sx = 3;
            }
            else if (x == 0)
            {
                sx = 0;
            }
            else if (x == ButtonSize.Width - 1)
            {
                sx = 2;
            }

            if (IsHovered)
            {
                sx += 4;
            }

            return new Rectangle2D(
                sx * ButtonTileSize.Width, 0,
                ButtonTileSize.Width, ButtonTileSize.Height);
        }
    }
}
