using OpenRS.Net.Client.Game;
using Microsoft.Xna.Framework.Input;

namespace OpenRS.Net.Client
{
    public sealed class Menu
    {


        public Menu(GameImage gameImageInstance, int capacity)
        {
            selectedComponent = -1;
            isListSelectionHighlighted = true;
            gameImage = gameImageInstance;
            componentCapacity = capacity;
            componentAcceptsInput = new bool[capacity];
            isScrollDragging = new bool[capacity];
            componentIsPasswordField = new bool[capacity];
            componentSkip = new bool[capacity];
            componentWhiteText = new bool[capacity];
            listShownEntries = new int[capacity];
            listLength = new int[capacity];
            componentSelectedIndex = new int[capacity];
            componentHighlightedIndex = new int[capacity];
            componentX = new int[capacity];
            componentY = new int[capacity];
            componentType = new int[capacity];
            componentWidth = new int[capacity];
            componentHeight = new int[capacity];
            copmonentInputMaxLength = new int[capacity];
            componentTextSize = new int[capacity];
            componentText = new string[capacity];
            componentTextList = new string[capacity][];
            scrollBarGradientColorTop = RgbToInt(114, 114, 176);
            scrollBarGradientColorBottom = RgbToInt(14, 14, 62);
            scrollBarDraggingBarLine1Color = RgbToInt(200, 208, 232);
            scrollBarDraggingBarColor = RgbToInt(96, 129, 184);
            scrollBarDraggingBarLine2Color = RgbToInt(53, 95, 115);
            borderColourOutside = RgbToInt(117, 142, 171);
            borderColourMiddle = RgbToInt(98, 122, 158);
            borderColourInner = RgbToInt(86, 100, 136);
            panelTopLeftColour = RgbToInt(135, 146, 179);
            panelTopLeftAltColour = RgbToInt(97, 112, 151);
            panelBottomRightAltColour = RgbToInt(88, 102, 136);
            panelBottomRightColour = RgbToInt(84, 93, 120);
        }

        public int RgbToInt(int redValue, int greenValue, int blueValue)
        {
            return GameImage.RgbToInt(redMod * redValue / 114, greenMod * greenValue / 114, blueMod * blueValue / 176);
        }

        public void MouseClick(int mouseXPosition, int mouseYPosition, int lastMouseButtonState, int mouseButtonState)
        {
            this.mouseX = mouseXPosition;
            this.mouseY = mouseYPosition;
            this.mouseButton = mouseButtonState;

            if (lastMouseButtonState != 0)
            {
                this.lastMouseButton = lastMouseButtonState;
            }

            if (lastMouseButtonState == 1)
            {
                for (int componentIndex = 0; componentIndex < menuItemsCount; componentIndex += 1)
                {
                    if (componentAcceptsInput[componentIndex] && componentType[componentIndex] == 10 && this.mouseX >= componentX[componentIndex] && this.mouseY >= componentY[componentIndex] && this.mouseX <= componentX[componentIndex] + componentWidth[componentIndex] && this.mouseY <= componentY[componentIndex] + componentHeight[componentIndex])
                    {
                        componentSkip[componentIndex] = true;
                    }

                    if (componentAcceptsInput[componentIndex] && componentType[componentIndex] == 14 && this.mouseX >= componentX[componentIndex] && this.mouseY >= componentY[componentIndex] && this.mouseX <= componentX[componentIndex] + componentWidth[componentIndex] && this.mouseY <= componentY[componentIndex] + componentHeight[componentIndex])
                    {
                        componentSelectedIndex[componentIndex] = 1 - componentSelectedIndex[componentIndex];
                    }
                }
            }

            if (mouseButtonState == 1)
            {
                mouseClickHoldCounter += 1;
            }
            else
            {
                mouseClickHoldCounter = 0;
            }

            if (lastMouseButtonState == 1 || mouseClickHoldCounter > 20)
            {
                for (int componentIndex = 0; componentIndex < menuItemsCount; componentIndex += 1)
                {
                    if (componentAcceptsInput[componentIndex] && componentType[componentIndex] == 15 && this.mouseX >= componentX[componentIndex] && this.mouseY >= componentY[componentIndex] && this.mouseX <= componentX[componentIndex] + componentWidth[componentIndex] && this.mouseY <= componentY[componentIndex] + componentHeight[componentIndex])
                    {
                        componentSkip[componentIndex] = true;
                    }
                }

                mouseClickHoldCounter -= 5;
            }
        }

        public bool IsClicked(int componentIndex)
        {
            if (componentAcceptsInput[componentIndex] && componentSkip[componentIndex])
            {
                componentSkip[componentIndex] = false;

                return true;
            }

            return false;
        }

        public void KeyPress(Keys key, char character)
        {
            if (key == 0)
            {
                return;
            }

            if (selectedComponent != -1 && componentText[selectedComponent] is not null && componentAcceptsInput[selectedComponent])
            {
                int currentLength = componentText[selectedComponent].Length;

                if (key == Keys.Back && currentLength > 0)
                {
                    componentText[selectedComponent] = componentText[selectedComponent].Substring(0, currentLength - 1);
                }

                if (key == Keys.Enter && currentLength > 0)
                {
                    componentSkip[selectedComponent] = true;
                }

                string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö0123456789!\"" + (char)243 + "$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";

                if (currentLength < copmonentInputMaxLength[selectedComponent])
                {
                    for (int charIndex = 0; charIndex < allowedChars.Length; charIndex += 1)
                    {
                        if (character == allowedChars[charIndex])
                        {
                            componentText[selectedComponent] += character;
                        }
                    }
                }

                if (key == Keys.Tab)
                {
                    do
                    {
                        selectedComponent = (selectedComponent + 1) % menuItemsCount;
                    }
                    while (componentType[selectedComponent] != 5 && componentType[selectedComponent] != 6);

                    return;
                }
            }
        }

        public void DrawMenu()
        {
            for (int componentIndex = 0; componentIndex < menuItemsCount; componentIndex += 1)
            {
                if (componentAcceptsInput[componentIndex])
                {
                    if (componentType[componentIndex] == 0)
                    {
                        DrawComponentTextAligned(componentIndex, componentX[componentIndex], componentY[componentIndex], componentText[componentIndex], componentTextSize[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 1)
                    {
                        DrawComponentTextAligned(componentIndex, componentX[componentIndex] - gameImage.TextWidth(componentText[componentIndex], componentTextSize[componentIndex]) / 2, componentY[componentIndex], componentText[componentIndex], componentTextSize[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 2)
                    {
                        DrawBackgroundPanel(componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 3)
                    {
                        DrawLineX(componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 4)
                    {
                        DrawScrollableList(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex], listLength[componentIndex], listShownEntries[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 5 || componentType[componentIndex] == 6)
                    {
                        DrawInputBox(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex], componentText[componentIndex], componentTextSize[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 7)
                    {
                        DrawHorizontalOptions(componentIndex, componentX[componentIndex], componentY[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 8)
                    {
                        DrawVerticalOptions(componentIndex, componentX[componentIndex], componentY[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 9)
                    {
                        DrawList(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex], listLength[componentIndex], listShownEntries[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 11)
                    {
                        DrawScrollCornerPanel(componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 12)
                    {
                        DrawPicture(componentX[componentIndex], componentY[componentIndex], componentTextSize[componentIndex]);
                    }
                    else if (componentType[componentIndex] == 14)
                    {
                        DrawBorderBox(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex]);
                    }
                }
            }

            lastMouseButton = 0;
        }

        protected void DrawBorderBox(int componentIndex, int x, int y, int w, int h)
        {
            gameImage.DrawBox(x, y, w, h, 0xffffff);
            gameImage.DrawLineX(x, y, w, panelTopLeftColour);
            gameImage.DrawLineY(x, y, h, panelTopLeftColour);
            gameImage.DrawLineX(x, y + h - 1, w, panelBottomRightColour);
            gameImage.DrawLineY(x + w - 1, y, h, panelBottomRightColour);

            if (componentSelectedIndex[componentIndex] == 1)
            {
                for (int drawIndex = 0; drawIndex < h; drawIndex += 1)
                {
                    gameImage.DrawLineX(x + drawIndex, y + drawIndex, 1, 0);
                    gameImage.DrawLineX(x + w - 1 - drawIndex, y + drawIndex, 1, 0);
                }
            }
        }

        protected void DrawComponentTextAligned(int componentIndex, int xPosition, int yPosition, string text, int fontIndex)
        {
            int textYPosition = yPosition + gameImage.TextHeightNumber(fontIndex) / 3;
            DrawComponentTextColored(componentIndex, xPosition, textYPosition, text, fontIndex);
        }

        protected void DrawComponentTextColored(int componentIndex, int xPosition, int yPosition, string text, int fontIndex)
        {
            int textColour;
            if (componentWhiteText[componentIndex])
            {
                textColour = 0xffffff;
            }
            else
            {
                textColour = 0;
            }

            gameImage.DrawString(text, xPosition, yPosition, fontIndex, textColour);
        }

        protected void DrawInputBox(int componentIndex, int xPosition, int yPosition, int width, int height, string text, int fontIndex)
        {
            if (componentIsPasswordField[componentIndex])
            {
                int maskedLength = text.Length;
                text = "";

                for (int maskIndex = 0; maskIndex < maskedLength; maskIndex += 1)
                {
                    text += "X";
                }
            }

            if (componentType[componentIndex] == 5)
            {
                if (lastMouseButton == 1 && mouseX >= xPosition && mouseY >= yPosition - height / 2 && mouseX <= xPosition + width && mouseY <= yPosition + height / 2)
                {
                    selectedComponent = componentIndex;
                }
            }
            else
            {
                if (componentType[componentIndex] == 6)
                {
                    if (lastMouseButton == 1 && mouseX >= xPosition - width / 2 && mouseY >= yPosition - height / 2 && mouseX <= xPosition + width / 2 && mouseY <= yPosition + height / 2)
                    {
                        selectedComponent = componentIndex;
                    }

                    xPosition -= gameImage.TextWidth(text, fontIndex) / 2;
                }
            }

            if (selectedComponent == componentIndex)
            {
                text += "*";
            }

            int textY = yPosition + gameImage.TextHeightNumber(fontIndex) / 3;
            DrawComponentTextColored(componentIndex, xPosition, textY, text, fontIndex);
        }

        public void DrawBackgroundPanel(int xPosition, int yPosition, int width, int height)
        {
            gameImage.SetDimensions(xPosition, yPosition, xPosition + width, yPosition + height);
            gameImage.DrawGradientBox(xPosition, yPosition, width, height, panelBottomRightColour, panelTopLeftColour);
            if (isBackgroundPatternEnabled)
            {
                for (int i = xPosition - (yPosition & 0x3f); i < xPosition + width; i += 128)
                {
                    for (int k = yPosition - (yPosition & 0x1f); k < yPosition + height; k += 128)
                    {
                        gameImage.DrawPicture(i, k, 6 + baseScrollPic, 128);
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

        public void DrawScrollCornerPanel(int xPosition, int yPosition, int width, int height)
        {
            gameImage.DrawBox(xPosition, yPosition, width, height, 0);
            gameImage.DrawBoxEdge(xPosition, yPosition, width, height, borderColourOutside);
            gameImage.DrawBoxEdge(xPosition + 1, yPosition + 1, width - 2, height - 2, borderColourMiddle);
            gameImage.DrawBoxEdge(xPosition + 2, yPosition + 2, width - 4, height - 4, borderColourInner);
            gameImage.DrawPicture(xPosition, yPosition, 2 + baseScrollPic);
            gameImage.DrawPicture(xPosition + width - 7, yPosition, 3 + baseScrollPic);
            gameImage.DrawPicture(xPosition, yPosition + height - 7, 4 + baseScrollPic);
            gameImage.DrawPicture(xPosition + width - 7, yPosition + height - 7, 5 + baseScrollPic);
        }

        protected void DrawPicture(int xPosition, int yPosition, int pictureIndex)
        {
            gameImage.DrawPicture(xPosition, yPosition, pictureIndex);
        }

        protected void DrawLineX(int xPosition, int yPosition, int width)
        {
            gameImage.DrawLineX(xPosition, yPosition, width, 0);
        }

        protected void DrawScrollableList(int componentIndex, int xPosition, int yPosition, int width, int height, int fontIndex, string[] textList,
                int listLength, int shownEntries)
        {
            int visibleEntries = height / gameImage.TextHeightNumber(fontIndex);

            if (shownEntries > listLength - visibleEntries)
            {
                shownEntries = listLength - visibleEntries;
            }

            if (shownEntries < 0)
            {
                shownEntries = 0;
            }

            listShownEntries[componentIndex] = shownEntries;

            if (visibleEntries < listLength)
            {
                int scrollbarX = xPosition + width - 12;
                int scrollbarThumbSize = (height - 27) * visibleEntries / listLength;

                if (scrollbarThumbSize < 6)
                {
                    scrollbarThumbSize = 6;
                }

                int scrollbarThumbOffset = (height - 27 - scrollbarThumbSize) * shownEntries / (listLength - visibleEntries);

                if (mouseButton == 1 && mouseX >= scrollbarX && mouseX <= scrollbarX + 12)
                {
                    if (mouseY > yPosition && mouseY < yPosition + 12 && shownEntries > 0)
                    {
                        shownEntries -= 1;
                    }

                    if (mouseY > yPosition + height - 12 && mouseY < yPosition + height && shownEntries < listLength - visibleEntries)
                    {
                        shownEntries += 1;
                    }

                    listShownEntries[componentIndex] = shownEntries;
                }

                if (mouseButton == 1 && (mouseX >= scrollbarX && mouseX <= scrollbarX + 12 || mouseX >= scrollbarX - 12 && mouseX <= scrollbarX + 24 && isScrollDragging[componentIndex]))
                {
                    if (mouseY > yPosition + 12 && mouseY < yPosition + height - 12)
                    {
                        isScrollDragging[componentIndex] = true;
                        int dragOffset = mouseY - yPosition - 12 - scrollbarThumbSize / 2;
                        shownEntries = dragOffset * listLength / (height - 24);

                        if (shownEntries > listLength - visibleEntries)
                        {
                            shownEntries = listLength - visibleEntries;
                        }

                        if (shownEntries < 0)
                        {
                            shownEntries = 0;
                        }

                        listShownEntries[componentIndex] = shownEntries;
                    }
                }
                else
                {
                    isScrollDragging[componentIndex] = false;
                }

                scrollbarThumbOffset = (height - 27 - scrollbarThumbSize) * shownEntries / (listLength - visibleEntries);
                DrawScrollbar(xPosition, yPosition, width, height, scrollbarThumbOffset, scrollbarThumbSize);
            }

            int remainingSpace = height - visibleEntries * gameImage.TextHeightNumber(fontIndex);
            int textY = yPosition + gameImage.TextHeightNumber(fontIndex) * 5 / 6 + remainingSpace / 2;

            for (int entryIndex = shownEntries; entryIndex < listLength; entryIndex += 1)
            {
                DrawComponentTextColored(componentIndex, xPosition + 2, textY, textList[entryIndex], fontIndex);
                textY += gameImage.TextHeightNumber(fontIndex) - chatMenuTextHeightMod;

                if (textY >= yPosition + height)
                {
                    return;
                }
            }

        }

        protected void DrawScrollbar(int xPosition, int yPosition, int width, int height, int thumbOffset, int thumbSize)
        {
            int scrollbarXOffset = xPosition + width - 12;
            gameImage.DrawBoxEdge(scrollbarXOffset, yPosition, 12, height, 0); // Border.
            gameImage.DrawPicture(scrollbarXOffset + 1, yPosition + 1, baseScrollPic); // Up arrow.
            gameImage.DrawPicture(scrollbarXOffset + 1, yPosition + height - 12, 1 + baseScrollPic); // Down arrow.
            gameImage.DrawLineX(scrollbarXOffset, yPosition + 13, 12, 0); // Up arrow border.
            gameImage.DrawLineX(scrollbarXOffset, yPosition + height - 13, 12, 0); // Down arrow border.
            gameImage.DrawGradientBox(scrollbarXOffset + 1, yPosition + 14, 11, height - 27, scrollBarGradientColorTop, scrollBarGradientColorBottom); // Background gradient.
            gameImage.DrawBox(scrollbarXOffset + 3, thumbOffset + yPosition + 14, 7, thumbSize, scrollBarDraggingBarColor); // Dragging bar.
            gameImage.DrawLineY(scrollbarXOffset + 2, thumbOffset + yPosition + 14, thumbSize, scrollBarDraggingBarLine1Color); // Dragging bar.
            gameImage.DrawLineY(scrollbarXOffset + 2 + 8, thumbOffset + yPosition + 14, thumbSize, scrollBarDraggingBarLine2Color); // Dragging bar.
        }

        protected void DrawHorizontalOptions(int componentIndex, int xPosition, int yPosition, int fontIndex, string[] options)
        {
            int totalWidth = 0;
            int optionCount = options.Length;

            for (int optionIndex = 0; optionIndex < optionCount; optionIndex += 1)
            {
                totalWidth += gameImage.TextWidth(options[optionIndex], fontIndex);

                if (optionIndex < optionCount - 1)
                {
                    totalWidth += gameImage.TextWidth("  ", fontIndex);
                }
            }

            int currentX = xPosition - totalWidth / 2;
            int textY = yPosition + gameImage.TextHeightNumber(fontIndex) / 3;

            for (int optionIndex = 0; optionIndex < optionCount; optionIndex += 1)
            {
                int textColour;

                if (componentWhiteText[componentIndex])
                {
                    textColour = 0xffffff;
                }
                else
                {
                    textColour = 0;
                }

                if (mouseX >= currentX && mouseX <= currentX + gameImage.TextWidth(options[optionIndex], fontIndex) && mouseY <= textY && mouseY > textY - gameImage.TextHeightNumber(fontIndex))
                {
                    if (componentWhiteText[componentIndex])
                    {
                        textColour = 0x808080;
                    }
                    else
                    {
                        textColour = 0xffffff;
                    }

                    if (lastMouseButton == 1)
                    {
                        componentSelectedIndex[componentIndex] = optionIndex;
                        componentSkip[componentIndex] = true;
                    }
                }

                if (componentSelectedIndex[componentIndex] == optionIndex)
                {
                    if (componentWhiteText[componentIndex])
                    {
                        textColour = 0xff0000;
                    }
                    else
                    {
                        textColour = 0xc00000;
                    }
                }

                gameImage.DrawString(options[optionIndex], currentX, textY, fontIndex, textColour);
                currentX += gameImage.TextWidth(options[optionIndex] + "  ", fontIndex);
            }

        }

        protected void DrawVerticalOptions(int componentIndex, int xPosition, int yPosition, int fontIndex, string[] options)
        {
            int optionCount = options.Length;
            int currentY = yPosition - gameImage.TextHeightNumber(fontIndex) * (optionCount - 1) / 2;

            for (int optionIndex = 0; optionIndex < optionCount; optionIndex += 1)
            {
                int textColour;

                if (componentWhiteText[componentIndex])
                {
                    textColour = 0xffffff;
                }
                else
                {
                    textColour = 0;
                }

                int optionWidth = gameImage.TextWidth(options[optionIndex], fontIndex);

                if (mouseX >= xPosition - optionWidth / 2 && mouseX <= xPosition + optionWidth / 2 && mouseY - 2 <= currentY && mouseY - 2 > currentY - gameImage.TextHeightNumber(fontIndex))
                {
                    if (componentWhiteText[componentIndex])
                    {
                        textColour = 0x808080;
                    }
                    else
                    {
                        textColour = 0xffffff;
                    }

                    if (lastMouseButton == 1)
                    {
                        componentSelectedIndex[componentIndex] = optionIndex;
                        componentSkip[componentIndex] = true;
                    }
                }

                if (componentSelectedIndex[componentIndex] == optionIndex)
                {
                    if (componentWhiteText[componentIndex])
                    {
                        textColour = 0xff0000;
                    }
                    else
                    {
                        textColour = 0xc00000;
                    }
                }

                gameImage.DrawString(options[optionIndex], xPosition - optionWidth / 2, currentY, fontIndex, textColour);
                currentY += gameImage.TextHeightNumber(fontIndex);
            }

        }
        // DrawList(x, componentX[x], componentY[x], componentWidth[x], componentHeight[x], componentTextSize[x], componentTextList[x], listLength[x], gbc[x]);
        protected void DrawList(int listIndex, int listX, int listY, int listWidth, int listHeight, int listTextSize, string[] listText,
                int listLength, int shownEntries)
        {
            int entryCount = listHeight / gameImage.TextHeightNumber(listTextSize);

            if (entryCount < listLength)
            {
                int scrollbarX = listX + listWidth - 12;
                int thumbSize = (listHeight - 27) * entryCount / listLength;

                if (thumbSize < 6)
                {
                    thumbSize = 6;
                }

                int thumbOffset = (listHeight - 27 - thumbSize) * shownEntries / (listLength - entryCount);

                if (mouseButton == 1 && mouseX >= scrollbarX && mouseX <= scrollbarX + 12)
                {
                    if (mouseY > listY && mouseY < listY + 12 && shownEntries > 0)
                    {
                        shownEntries -= 1;
                    }

                    if (mouseY > listY + listHeight - 12 && mouseY < listY + listHeight && shownEntries < listLength - entryCount)
                    {
                        shownEntries += 1;
                    }

                    listShownEntries[listIndex] = shownEntries;
                }

                if (mouseButton == 1 && (mouseX >= scrollbarX && mouseX <= scrollbarX + 12 || mouseX >= scrollbarX - 12 && mouseX <= scrollbarX + 24 && isScrollDragging[listIndex]))
                {
                    if (mouseY > listY + 12 && mouseY < listY + listHeight - 12)
                    {
                        isScrollDragging[listIndex] = true;
                        int dragOffset = mouseY - listY - 12 - thumbSize / 2;
                        shownEntries = dragOffset * listLength / (listHeight - 24);

                        if (shownEntries < 0)
                        {
                            shownEntries = 0;
                        }

                        if (shownEntries > listLength - entryCount)
                        {
                            shownEntries = listLength - entryCount;
                        }

                        listShownEntries[listIndex] = shownEntries;
                    }
                }
                else
                {
                    isScrollDragging[listIndex] = false;
                }

                thumbOffset = (listHeight - 27 - thumbSize) * shownEntries / (listLength - entryCount);
                DrawScrollbar(listX, listY, listWidth, listHeight, thumbOffset, thumbSize);
            }
            else
            {
                shownEntries = 0;
                listShownEntries[listIndex] = 0;
            }

            componentHighlightedIndex[listIndex] = -1;
            int remainingSpace = listHeight - entryCount * gameImage.TextHeightNumber(listTextSize);
            int textY = listY + gameImage.TextHeightNumber(listTextSize) * 5 / 6 + remainingSpace / 2;

            for (int entryIndex = shownEntries; entryIndex < listLength; entryIndex += 1)
            {
                int textColour;

                if (componentWhiteText[listIndex])
                {
                    textColour = 0xffffff;
                }
                else
                {
                    textColour = 0;
                }

                if (mouseX >= listX + 2 && mouseX <= listX + 2 + gameImage.TextWidth(listText[entryIndex], listTextSize) && mouseY - 2 <= textY && mouseY - 2 > textY - gameImage.TextHeightNumber(listTextSize))
                {
                    if (componentWhiteText[listIndex])
                    {
                        textColour = 0x808080;
                    }
                    else
                    {
                        textColour = 0xffffff;
                    }

                    componentHighlightedIndex[listIndex] = entryIndex;

                    if (lastMouseButton == 1)
                    {
                        componentSelectedIndex[listIndex] = entryIndex;
                        componentSkip[listIndex] = true;
                    }
                }

                if (componentSelectedIndex[listIndex] == entryIndex && isListSelectionHighlighted)
                {
                    textColour = 0xff0000;
                }

                gameImage.DrawString(listText[entryIndex], listX + 2, textY, listTextSize, textColour);
                textY += gameImage.TextHeightNumber(listTextSize);

                if (textY >= listY + listHeight)
                {
                    return;
                }
            }
        }

        public int DrawText(int xPosition, int yPosition, string text, int fontIndex, bool isWhiteText)
        {
            componentType[menuItemsCount] = 1;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = fontIndex;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentText[menuItemsCount] = text;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int DrawButton(int xPosition, int yPosition, int width, int height)
        {
            componentType[menuItemsCount] = 2;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = xPosition - width / 2;
            componentY[menuItemsCount] = yPosition - height / 2;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int DrawCurvedBox(int xPosition, int yPosition, int width, int height)
        {
            componentType[menuItemsCount] = 11;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = xPosition - width / 2;
            componentY[menuItemsCount] = yPosition - height / 2;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int DrawArrow(int xPosition, int yPosition, int pictureIndex)
        {
            int pictureWidth = gameImage.pictureWidth[pictureIndex];
            int pictureHeight = gameImage.pictureHeight[pictureIndex];
            componentType[menuItemsCount] = 12;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = xPosition - pictureWidth / 2;
            componentY[menuItemsCount] = yPosition - pictureHeight / 2;
            componentWidth[menuItemsCount] = pictureWidth;
            componentHeight[menuItemsCount] = pictureHeight;
            componentTextSize[menuItemsCount] = pictureIndex;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateScrollableTextBox(int xPosition, int yPosition, int width, int height, int fontIndex, int maxItems, bool isWhiteText)
        {
            componentType[menuItemsCount] = 4;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentTextSize[menuItemsCount] = fontIndex;
            copmonentInputMaxLength[menuItemsCount] = maxItems;
            listLength[menuItemsCount] = 0;
            listShownEntries[menuItemsCount] = 0;
            componentTextList[menuItemsCount] = new string[maxItems];
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateTextInput(int xPosition, int yPosition, int width, int height, int fontIndex, int maxLength, bool isPasswordField,
                bool isWhiteText)
        {
            componentType[menuItemsCount] = 5;
            componentAcceptsInput[menuItemsCount] = true;
            componentIsPasswordField[menuItemsCount] = isPasswordField;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = fontIndex;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            copmonentInputMaxLength[menuItemsCount] = maxLength;
            componentText[menuItemsCount] = "";
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateInput(int xPosition, int yPosition, int width, int height, int fontIndex, int maxLength, bool isPasswordField,
                bool isWhiteText)
        {
            componentType[menuItemsCount] = 6;
            componentAcceptsInput[menuItemsCount] = true;
            componentIsPasswordField[menuItemsCount] = isPasswordField;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = fontIndex;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            copmonentInputMaxLength[menuItemsCount] = maxLength;
            componentText[menuItemsCount] = "";
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateList(int xPosition, int yPosition, int width, int height, int fontIndex, int maxItems, bool isWhiteText)
        {
            componentType[menuItemsCount] = 9;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = fontIndex;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            copmonentInputMaxLength[menuItemsCount] = maxItems;
            componentTextList[menuItemsCount] = new string[maxItems];
            listLength[menuItemsCount] = 0;
            listShownEntries[menuItemsCount] = 0;
            componentSelectedIndex[menuItemsCount] = -1;
            componentHighlightedIndex[menuItemsCount] = -1;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateButton(int xPosition, int yPosition, int width, int height)
        {
            componentType[menuItemsCount] = 10;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = xPosition - width / 2;
            componentY[menuItemsCount] = yPosition - height / 2;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public void ClearList(int componentIndex)
        {
            listLength[componentIndex] = 0;
        }

        public void SwitchList(int componentIndex)
        {
            listShownEntries[componentIndex] = 0;
            componentHighlightedIndex[componentIndex] = -1;
        }

        public void AddListItem(int componentIndex, int itemIndex, string text)
        {
            componentTextList[componentIndex][itemIndex] = text;

            if (itemIndex + 1 > listLength[componentIndex])
            {
                listLength[componentIndex] = itemIndex + 1;
            }
        }

        public void AddMessage(int componentIndex, string messageText, bool isScrollToBottom)
        {
            int messageIndex = listLength[componentIndex] += 1;
            if (messageIndex >= copmonentInputMaxLength[componentIndex])
            {
                messageIndex -= 1;
                listLength[componentIndex] -= 1;
                for (int k = 0; k < messageIndex; k += 1)
                {
                    componentTextList[componentIndex][k] = componentTextList[componentIndex][k + 1];
                }
            }
            componentTextList[componentIndex][messageIndex] = messageText;
            if (isScrollToBottom)
            {
                listShownEntries[componentIndex] = 0xf423f;
            }
        }

        public void UpdateText(int componentIndex, string text)
        {
            componentText[componentIndex] = text;
        }

        public string GetText(int componentIndex)
        {
            if (componentText[componentIndex] is null)
            {
                return "null";
            }
            else
            {
                return componentText[componentIndex];
            }
        }

        public void EnableInput(int componentIndex)
        {
            componentAcceptsInput[componentIndex] = true;
        }

        public void DisableInput(int componentIndex)
        {
            componentAcceptsInput[componentIndex] = false;
        }

        public void SetFocus(int componentIndex)
        {
            selectedComponent = componentIndex;
        }

        public int GetEntryHighlighted(int componentIndex)
        {
            int highlightedIndex = componentHighlightedIndex[componentIndex];

            return highlightedIndex;
        }


        protected GameImage gameImage;
        private int menuItemsCount;
        private readonly int componentCapacity;
        public bool[] componentAcceptsInput;
        public bool[] isScrollDragging;
        public bool[] componentIsPasswordField;
        public bool[] componentSkip;
        public int[] listShownEntries;
        public int[] listLength;
        public int[] componentSelectedIndex;
        public int[] componentHighlightedIndex;
        private readonly bool[] componentWhiteText;
        private readonly int[] componentX;
        private readonly int[] componentY;
        private readonly int[] componentType;
        private readonly int[] componentWidth;
        private readonly int[] componentHeight;
        private readonly int[] copmonentInputMaxLength;
        private readonly int[] componentTextSize;
        private readonly string[] componentText;
        private readonly string[][] componentTextList;
        private int mouseX;
        private int mouseY;
        private int lastMouseButton;
        private int mouseButton;
        private int selectedComponent;
        private int mouseClickHoldCounter;
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
        public bool isListSelectionHighlighted;
        public static bool isBackgroundPatternEnabled = true;
        public static int baseScrollPic;
        public static int redMod = 114;
        public static int greenMod = 114;
        public static int blueMod = 176;
        public static int chatMenuTextHeightMod;
    }
}
