using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

using OpenRS.Settings;

namespace OpenRS.Gui.Controls
{
    /// <summary>
    /// Button GUI element.
    /// </summary>
    public class GuiButton : GuiControl
    {
        public Size2D ButtonTileSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the button.
        /// </summary>
        /// <value>The size of the button.</value>
        public Size2D ButtonSize => new Size2D(
            Size.Width / ButtonTileSize.Width,
            Size.Height / ButtonTileSize.Height);

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        public string Icon { get; set; }

        public string Texture { get; set; }

        protected List<GuiImage> images;
        GuiImage icon;
        GuiText text;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiButton"/> class.
        /// </summary>
        public GuiButton()
        {
            Texture = "Interface/button";
            FontName = "ButtonFont";
            ButtonTileSize = new Size2D(GameDefines.GUI_TILE_SIZE, GameDefines.GUI_TILE_SIZE);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            icon = new GuiImage();
            images = new List<GuiImage>();
            text = new GuiText();

            for (int x = 0; x < ButtonSize.Width; x++)
            {
                GuiImage image = new GuiImage { SourceRectangle = CalculateSourceRectangle(x) };

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

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {
            UnregisterEvents();
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

        /// <summary>
        /// Registers the events.
        /// </summary>
        void RegisterEvents()
        {
            Clicked += OnClicked;
            MouseEntered += OnMouseEntered;
        }

        /// <summary>
        /// Unregisters the events.
        /// </summary>
        void UnregisterEvents()
        {
            Clicked -= OnClicked;
            MouseEntered -= OnMouseEntered;
        }

        void SetChildrenProperties()
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].ContentFile = Texture;
                images[i].Location = new Point2D(Location.X + i * ButtonTileSize.Width, Location.Y);
                images[i].SourceRectangle = CalculateSourceRectangle(i);
            }

            text.Text = Text;
            text.ForegroundColour = ForegroundColour;
            text.FontName = FontName;
            text.Location = Location;
            text.Size = Size;

            icon.ContentFile = Icon;
            icon.Location = new Point2D(
                Location.X + (Size.Width - icon.Size.Width) / 2,
                Location.Y + (Size.Height - icon.Size.Height) / 2);
        }

        /// <summary>
        /// Fired by the Clicked event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.Button != MouseButton.Left)
            {
                return;
            }

            //AudioManager.Instance.PlaySound("Interface/click");
        }

        /// <summary>
        /// Fired by the MouseMoved event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnMouseEntered(object sender, MouseEventArgs e)
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
