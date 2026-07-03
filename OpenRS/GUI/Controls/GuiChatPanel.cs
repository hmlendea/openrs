using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace OpenRS.Gui.Controls
{
    public sealed class GuiChatPanel : GuiControl
    {
        const int MessageHeight = 24;

        GuiImage background;

        List<GuiText> messageRows;

        public GuiChatPanel()
        {
            BackgroundColour = Colour.Black;
            ForegroundColour = Colour.Yellow;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            messageRows = new List<GuiText>();

            background = new GuiImage
            {
                ContentFile = "ScreenManager/FillImage",
                TextureLayout = TextureLayout.Tile
            };
            
            RegisterChild(background);
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

        public void AddMessage(string message)
        {
            for (int i = 0; i < messageRows.Count - 1; i++)
            {
                messageRows[i].Text = messageRows[i + 1].Text;
            }

            messageRows[messageRows.Count - 1].Text = message;
        }

        void SetChildrenProperties()
        {
            background.Size = Size;
            background.Location = Location;
            background.TintColour = BackgroundColour;

            // Add additional rows if there is enough room (the chat panel was expanded)
            while (Size.Height - (messageRows.Count * MessageHeight) >= MessageHeight)
            {
                GuiText newRow = new GuiText
                {
                    FontName = "ChatFont",
                    HorizontalAlignment = Alignment.Beginning
                };

                RegisterChild(newRow);

                messageRows.Insert(0, newRow);
            }

            // Remove extra rows if there is not enough room (the chat panel was shrunk)
            while (Size.Height < MessageHeight * messageRows.Count)
            {
                messageRows.RemoveAt(0);
            }

            // Update the properties of 
            int y = ClientRectangle.Bottom - MessageHeight;
            for (int i = messageRows.Count - 1; i >= 0; i--)
            {
                GuiText message = messageRows[i];

                message.Size = new Size2D(Size.Width, MessageHeight);
                message.ForegroundColour = ForegroundColour;
                message.Location = new Point2D(Location.X, y);

                y -= message.Size.Height;
            }
        }
    }
}
