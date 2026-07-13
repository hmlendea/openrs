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
        private static int MessageHeight => 24;

        private GuiImage background;

        private List<GuiText> messageRows;

        public GuiChatPanel()
        {
            BackgroundColour = Colour.Black;
            ForegroundColour = Colour.Yellow;
        }
        protected override void DoLoadContent()
        {
            messageRows = [];

            background = new GuiImage
            {
                ContentFile = "ScreenManager/FillImage",
                TextureLayout = TextureLayout.Tile
            };

            RegisterChild(background);
            SetChildrenProperties();
        }
        protected override void DoUnloadContent()
        {

        }
        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }

        public void AddMessage(string message)
        {
            for (int i = 0; i < messageRows.Count - 1; i += 1)
            {
                messageRows[i].Text = messageRows[i + 1].Text;
            }

            messageRows[messageRows.Count - 1].Text = message;
        }

        private void SetChildrenProperties()
        {
            background.Size = Size;
            background.Location = new Point2D(0, 0);
            background.TintColour = BackgroundColour;

            // Add additional rows if there is enough room (the chat panel was expanded)
            while (Size.Height - (messageRows.Count * MessageHeight) >= MessageHeight)
            {
                GuiText newRow = new()
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
            int y = Size.Height - MessageHeight;
            for (int i = messageRows.Count - 1; i >= 0; i -= 1)
            {
                GuiText message = messageRows[i];

                message.Size = new Size2D(Size.Width, MessageHeight);
                message.ForegroundColour = ForegroundColour;
                message.Location = new Point2D(0, y);

                y -= message.Size.Height;
            }
        }
    }
}
