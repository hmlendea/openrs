using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client
{
    internal sealed class MenuTextRenderer
    {
        private readonly GameImage gameImage;

        internal MenuTextRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void DrawAligned(
            int xPosition,
            int yPosition,
            string text,
            int fontIndex,
            bool isWhiteText)
        {
            int textYPosition =
                yPosition + gameImage.TextHeightNumber(fontIndex) / 3;
            Draw(xPosition, textYPosition, text, fontIndex, isWhiteText);
        }

        internal void Draw(
            int xPosition,
            int yPosition,
            string text,
            int fontIndex,
            bool isWhiteText)
        {
            int textColour = 0;

            if (isWhiteText)
            {
                textColour = 0xffffff;
            }

            gameImage.DrawString(text, xPosition, yPosition, fontIndex, textColour);
        }

        internal string MaskPassword(string text, bool isPasswordField)
        {
            if (!isPasswordField)
            {
                return text;
            }

            int maskedLength = text.Length;
            string maskedText = string.Empty;

            for (int maskIndex = 0; maskIndex < maskedLength; maskIndex += 1)
            {
                maskedText += "X";
            }

            return maskedText;
        }

        internal bool IsInputSelected(
            MenuComponentType componentType,
            int xPosition,
            int yPosition,
            int width,
            int height,
            int lastMouseButton,
            int mouseX,
            int mouseY)
        {
            if (lastMouseButton != 1)
            {
                return false;
            }

            if (componentType == MenuComponentType.LeftAlignedTextInput)
            {
                return mouseX >= xPosition &&
                    mouseY >= yPosition - height / 2 &&
                    mouseX <= xPosition + width &&
                    mouseY <= yPosition + height / 2;
            }

            if (componentType == MenuComponentType.CentredTextInput)
            {
                return mouseX >= xPosition - width / 2 &&
                    mouseY >= yPosition - height / 2 &&
                    mouseX <= xPosition + width / 2 &&
                    mouseY <= yPosition + height / 2;
            }

            return false;
        }

        internal void DrawInput(
            MenuComponentType componentType,
            int xPosition,
            int yPosition,
            string text,
            int fontIndex,
            bool isWhiteText,
            bool isSelected)
        {
            if (componentType == MenuComponentType.CentredTextInput)
            {
                xPosition -= gameImage.TextWidth(text, fontIndex) / 2;
            }

            if (isSelected)
            {
                text += "*";
            }

            int textY = yPosition + gameImage.TextHeightNumber(fontIndex) / 3;
            Draw(xPosition, textY, text, fontIndex, isWhiteText);
        }
    }
}