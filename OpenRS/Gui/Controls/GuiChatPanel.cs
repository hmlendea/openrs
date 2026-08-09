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

        private static string BackgroundContentFile => "ScreenManager/FillImage";

        private static string MessageFontName => "ChatFont";

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
                ContentFile = BackgroundContentFile,
                TextureLayout = TextureLayout.Tile
            };

            RegisterChild(background);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        public void AddMessage(string message)
        {
            for (int messageRowIndex = 0; messageRowIndex < messageRows.Count - 1; messageRowIndex += 1)
            {
                messageRows[messageRowIndex].Text = messageRows[messageRowIndex + 1].Text;
            }

            messageRows[messageRows.Count - 1].Text = message;
        }

        private void SetChildrenProperties()
        {
            UpdateBackgroundProperties();
            EnsureCorrectRowCount();
            UpdateRowProperties();
        }

        private void UpdateBackgroundProperties()
        {
            background.Size = Size;
            background.Location = Point2D.Empty;
            background.TintColour = BackgroundColour;
        }

        private void EnsureCorrectRowCount()
        {
            // Add additional rows if there is enough room (the chat panel was expanded).
            while (Size.Height - (messageRows.Count * MessageHeight) >= MessageHeight)
            {
                GuiText newRow = new()
                {
                    FontName = MessageFontName,
                    HorizontalAlignment = Alignment.Beginning
                };

                RegisterChild(newRow);

                messageRows.Insert(0, newRow);
            }

            // Remove extra rows if there is not enough room (the chat panel was shrunk).
            while (Size.Height < MessageHeight * messageRows.Count)
            {
                messageRows.RemoveAt(0);
            }
        }

        private void UpdateRowProperties()
        {
            int verticalOffset = Size.Height - MessageHeight;

            for (int messageRowIndex = messageRows.Count - 1; messageRowIndex >= 0; messageRowIndex -= 1)
            {
                GuiText messageRow = messageRows[messageRowIndex];

                messageRow.Size = new(Size.Width, MessageHeight);
                messageRow.ForegroundColour = ForegroundColour;
                messageRow.Location = new(0, verticalOffset);

                verticalOffset -= messageRow.Size.Height;
            }
        }
    }
}

