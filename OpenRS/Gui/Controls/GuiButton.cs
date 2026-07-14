using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

using OpenRS.Settings;

namespace OpenRS.Gui.Controls
{
    public class GuiButton : GuiControl
    {
        public Size2D ButtonTileSize { get; set; }

        public Size2D ButtonTileCount => new(
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
            ButtonTileSize = new(GameDefines.GuiTileSize, GameDefines.GuiTileSize);
        }

        protected override void DoLoadContent()
        {
            icon = new GuiImage();
            images = [];
            text = new GuiText();

            for (int sectionIndex = 0; sectionIndex < ButtonTileCount.Width; sectionIndex += 1)
            {
                GuiImage image = new() { SourceRectangle = CalculateSourceRectangle(sectionIndex) };

                images.Add(image);
            }

            RegisterChildren(images);
            RegisterChild(text);

            if (!string.IsNullOrWhiteSpace(Icon))
            {
                RegisterChild(icon);
            }

            SetChildrenProperties();
        }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoUnloadContent() { }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        protected virtual Rectangle2D CalculateSourceRectangle(int sectionIndex)
        {
            int spriteStateOffset = 1;

            if (ButtonTileCount.Width == 1)
            {
                spriteStateOffset = 3;
            }
            else if (sectionIndex == 0)
            {
                spriteStateOffset = 0;
            }
            else if (sectionIndex == ButtonTileCount.Width - 1)
            {
                spriteStateOffset = 2;
            }

            if (IsHovered)
            {
                spriteStateOffset += 4;
            }

            return new Rectangle2D(
                spriteStateOffset * ButtonTileSize.Width, 0,
                ButtonTileSize.Width, ButtonTileSize.Height);
        }

        private void SetChildrenProperties()
        {
            for (int imageIndex = 0; imageIndex < images.Count; imageIndex += 1)
            {
                GuiImage image = images[imageIndex];
                image.ContentFile = Texture;
                image.Location = new(imageIndex * ButtonTileSize.Width, 0);
                image.SourceRectangle = CalculateSourceRectangle(imageIndex);
            }

            text.Text = Text;
            text.ForegroundColour = ForegroundColour;
            text.FontName = FontName;
            text.Location = Point2D.Empty;
            text.Size = Size;

            icon.ContentFile = Icon;
            icon.Location = new(
                (Size.Width - icon.Size.Width) / 2,
                (Size.Height - icon.Size.Height) / 2);
        }
    }
}

