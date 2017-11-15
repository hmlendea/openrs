using System.Collections.Generic;
using System.Linq;

using RuneScapeSolo.Graphics.Enumerations;
using RuneScapeSolo.Graphics.Primitives;

namespace RuneScapeSolo.Gui.GuiElements
{
    public sealed class GuiChatPanel : GuiElement
    {
        const int MessageHeight = 24;

        GuiImage background;

        List<GuiText> messages;

        public GuiChatPanel()
        {
            BackgroundColour = Colour.Black;
            ForegroundColour = Colour.Yellow;
        }

        public override void LoadContent()
        {
            messages = new List<GuiText>();

            background = new GuiImage
            {
                ContentFile = "ScreenManager/FillImage",
                TextureLayout = TextureLayout.Tile
            };

            Children.Add(background);

            base.LoadContent();
        }

        public void AddMessage(string message)
        {
            GuiText messageText = new GuiText {
                FontName = "ChatFont",
                Text = message,
                ForegroundColour = ForegroundColour,
                Size = new Size2D(Size.Width, MessageHeight),
                VerticalAlignment = VerticalAlignment.Left
            };

            messageText.LoadContent();

            Children.Add(messageText);
            messages.Add(messageText);

            if (Size.Height < messages.Sum(x => x.Size.Height))
            {
                GuiText messageToRemove = messages[0];

                Children.Remove(messageToRemove);
                messages.Remove(messageToRemove);
            }
        }

        protected override void SetChildrenProperties()
        {
            background.Size = Size;
            background.Location = Location;
            background.TintColour = BackgroundColour;

            int y = ClientRectangle.Bottom - MessageHeight;
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                GuiText message = messages[i];

                message.Size = new Size2D(Size.Width, message.Size.Height);
                message.Location = new Point2D(Location.X, y);

                y -= MessageHeight;
            }

            base.SetChildrenProperties();
        }
    }
}
