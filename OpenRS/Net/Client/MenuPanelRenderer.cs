using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client
{
    internal sealed class MenuPanelRenderer
    {
        private readonly GameImage gameImage;
        private readonly int scrollBarGradientColorTop;
        private readonly int scrollBarGradientColorBottom;
        private readonly int scrollBarDraggingBarLine1Color;
        private readonly int scrollBarDraggingBarColor;
        private readonly int scrollBarDraggingBarLine2Color;
        private readonly int borderColourOutside;
        private readonly int borderColourMiddle;
        private readonly int borderColourInner;
        private readonly int panelTopLeftColour;
        private readonly int panelTopLeftAltColour;
        private readonly int panelBottomRightAltColour;
        private readonly int panelBottomRightColour;

        internal MenuPanelRenderer(Menu menu, GameImage gameImage)
        {
            this.gameImage = gameImage;
            scrollBarGradientColorTop = menu.RgbToInt(114, 114, 176);
            scrollBarGradientColorBottom = menu.RgbToInt(14, 14, 62);
            scrollBarDraggingBarLine1Color = menu.RgbToInt(200, 208, 232);
            scrollBarDraggingBarColor = menu.RgbToInt(96, 129, 184);
            scrollBarDraggingBarLine2Color = menu.RgbToInt(53, 95, 115);
            borderColourOutside = menu.RgbToInt(117, 142, 171);
            borderColourMiddle = menu.RgbToInt(98, 122, 158);
            borderColourInner = menu.RgbToInt(86, 100, 136);
            panelTopLeftColour = menu.RgbToInt(135, 146, 179);
            panelTopLeftAltColour = menu.RgbToInt(97, 112, 151);
            panelBottomRightAltColour = menu.RgbToInt(88, 102, 136);
            panelBottomRightColour = menu.RgbToInt(84, 93, 120);
        }

        internal void DrawBorderBox(int x, int y, int width, int height, bool isSelected)
        {
            gameImage.DrawBox(x, y, width, height, 0xffffff);
            gameImage.DrawLineX(x, y, width, panelTopLeftColour);
            gameImage.DrawLineY(x, y, height, panelTopLeftColour);
            gameImage.DrawLineX(x, y + height - 1, width, panelBottomRightColour);
            gameImage.DrawLineY(x + width - 1, y, height, panelBottomRightColour);

            if (!isSelected)
            {
                return;
            }

            for (int drawIndex = 0; drawIndex < height; drawIndex += 1)
            {
                gameImage.DrawLineX(x + drawIndex, y + drawIndex, 1, 0);
                gameImage.DrawLineX(x + width - 1 - drawIndex, y + drawIndex, 1, 0);
            }
        }

        internal void DrawBackgroundPanel(
            int xPosition,
            int yPosition,
            int width,
            int height,
            bool isPatternEnabled,
            int basePictureIndex)
        {
            gameImage.SetDimensions(xPosition, yPosition, xPosition + width, yPosition + height);
            gameImage.DrawGradientBox(
                xPosition,
                yPosition,
                width,
                height,
                panelBottomRightColour,
                panelTopLeftColour);

            if (isPatternEnabled)
            {
                for (int patternX = xPosition - (yPosition & 0x3f);
                    patternX < xPosition + width;
                    patternX += 128)
                {
                    for (int patternY = yPosition - (yPosition & 0x1f);
                        patternY < yPosition + height;
                        patternY += 128)
                    {
                        gameImage.DrawPicture(
                            patternX,
                            patternY,
                            6 + basePictureIndex,
                            128);
                    }
                }
            }

            gameImage.DrawLineX(xPosition, yPosition, width, panelTopLeftColour);
            gameImage.DrawLineX(xPosition + 1, yPosition + 1, width - 2, panelTopLeftColour);
            gameImage.DrawLineX(xPosition + 2, yPosition + 2, width - 4, panelTopLeftAltColour);
            gameImage.DrawLineY(xPosition, yPosition, height, panelTopLeftColour);
            gameImage.DrawLineY(xPosition + 1, yPosition + 1, height - 2, panelTopLeftColour);
            gameImage.DrawLineY(xPosition + 2, yPosition + 2, height - 4, panelTopLeftAltColour);
            gameImage.DrawLineX(xPosition, yPosition + height - 1, width, panelBottomRightColour);
            gameImage.DrawLineX(xPosition + 1, yPosition + height - 2, width - 2, panelBottomRightColour);
            gameImage.DrawLineX(xPosition + 2, yPosition + height - 3, width - 4, panelBottomRightAltColour);
            gameImage.DrawLineY(xPosition + width - 1, yPosition, height, panelBottomRightColour);
            gameImage.DrawLineY(xPosition + width - 2, yPosition + 1, height - 2, panelBottomRightColour);
            gameImage.DrawLineY(xPosition + width - 3, yPosition + 2, height - 4, panelBottomRightAltColour);
            gameImage.ResetDimensions();
        }

        internal void DrawScrollCornerPanel(
            int xPosition,
            int yPosition,
            int width,
            int height,
            int basePictureIndex)
        {
            gameImage.DrawBox(xPosition, yPosition, width, height, 0);
            gameImage.DrawBoxEdge(xPosition, yPosition, width, height, borderColourOutside);
            gameImage.DrawBoxEdge(xPosition + 1, yPosition + 1, width - 2, height - 2, borderColourMiddle);
            gameImage.DrawBoxEdge(xPosition + 2, yPosition + 2, width - 4, height - 4, borderColourInner);
            gameImage.DrawPicture(xPosition, yPosition, 2 + basePictureIndex);
            gameImage.DrawPicture(xPosition + width - 7, yPosition, 3 + basePictureIndex);
            gameImage.DrawPicture(xPosition, yPosition + height - 7, 4 + basePictureIndex);
            gameImage.DrawPicture(xPosition + width - 7, yPosition + height - 7, 5 + basePictureIndex);
        }

        internal void DrawScrollbar(
            int xPosition,
            int yPosition,
            int width,
            int height,
            int thumbOffset,
            int thumbSize,
            int basePictureIndex)
        {
            int scrollbarXOffset = xPosition + width - 12;
            gameImage.DrawBoxEdge(scrollbarXOffset, yPosition, 12, height, 0);
            gameImage.DrawPicture(scrollbarXOffset + 1, yPosition + 1, basePictureIndex);
            gameImage.DrawPicture(scrollbarXOffset + 1, yPosition + height - 12, 1 + basePictureIndex);
            gameImage.DrawLineX(scrollbarXOffset, yPosition + 13, 12, 0);
            gameImage.DrawLineX(scrollbarXOffset, yPosition + height - 13, 12, 0);
            gameImage.DrawGradientBox(
                scrollbarXOffset + 1,
                yPosition + 14,
                11,
                height - 27,
                scrollBarGradientColorTop,
                scrollBarGradientColorBottom);
            gameImage.DrawBox(
                scrollbarXOffset + 3,
                thumbOffset + yPosition + 14,
                7,
                thumbSize,
                scrollBarDraggingBarColor);
            gameImage.DrawLineY(
                scrollbarXOffset + 2,
                thumbOffset + yPosition + 14,
                thumbSize,
                scrollBarDraggingBarLine1Color);
            gameImage.DrawLineY(
                scrollbarXOffset + 10,
                thumbOffset + yPosition + 14,
                thumbSize,
                scrollBarDraggingBarLine2Color);
        }
    }
}