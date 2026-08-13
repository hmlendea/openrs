using Microsoft.Xna.Framework.Input;

using OpenRS.Net.Client.Game;

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
            componentType = new MenuComponentType[capacity];
            componentWidth = new int[capacity];
            componentHeight = new int[capacity];
            copmonentInputMaxLength = new int[capacity];
            componentTextSize = new int[capacity];
            componentText = new string[capacity];
            componentTextList = new string[capacity][];
            panelRenderer = new MenuPanelRenderer(this, gameImage);
            textRenderer = new MenuTextRenderer(gameImage);
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
                    if (componentAcceptsInput[componentIndex] &&
                        componentType[componentIndex] == MenuComponentType.Button &&
                        IsMouseWithinComponent(componentIndex))
                    {
                        componentSkip[componentIndex] = true;
                    }

                    if (componentAcceptsInput[componentIndex] &&
                        componentType[componentIndex] == MenuComponentType.Toggle &&
                        IsMouseWithinComponent(componentIndex))
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
                    if (componentAcceptsInput[componentIndex] &&
                        componentType[componentIndex] == MenuComponentType.RepeatingButton &&
                        IsMouseWithinComponent(componentIndex))
                    {
                        componentSkip[componentIndex] = true;
                    }
                }

                mouseClickHoldCounter -= 5;
            }
        }

        private bool IsMouseWithinComponent(int componentIndex) =>
            mouseX >= componentX[componentIndex] &&
            mouseY >= componentY[componentIndex] &&
            mouseX <= componentX[componentIndex] + componentWidth[componentIndex] &&
            mouseY <= componentY[componentIndex] + componentHeight[componentIndex];

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

            if (selectedComponent == -1 ||
                componentText[selectedComponent] is null ||
                !componentAcceptsInput[selectedComponent])
            {
                return;
            }

            int currentLength = componentText[selectedComponent].Length;
            componentText[selectedComponent] = MenuTextInputEditor.RemoveLastCharacter(
                key,
                componentText[selectedComponent],
                currentLength);

            if (MenuTextInputEditor.IsSubmitted(key, currentLength))
            {
                componentSkip[selectedComponent] = true;
            }

            componentText[selectedComponent] = MenuTextInputEditor.AppendCharacter(
                character,
                componentText[selectedComponent],
                currentLength,
                copmonentInputMaxLength[selectedComponent]);

            if (key == Keys.Tab)
            {
                SelectNextInputComponent();
            }
        }

        private void SelectNextInputComponent()
        {
            do
            {
                selectedComponent = (selectedComponent + 1) % menuItemsCount;
            }
            while (componentType[selectedComponent] != MenuComponentType.LeftAlignedTextInput &&
                componentType[selectedComponent] != MenuComponentType.CentredTextInput);
        }

        public void DrawMenu()
        {
            for (int componentIndex = 0; componentIndex < menuItemsCount; componentIndex += 1)
            {
                if (componentAcceptsInput[componentIndex])
                {
                    if (componentType[componentIndex] == MenuComponentType.LeftAlignedText)
                    {
                        textRenderer.DrawAligned(
                            componentX[componentIndex],
                            componentY[componentIndex],
                            componentText[componentIndex],
                            componentTextSize[componentIndex],
                            componentWhiteText[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.CentredText)
                    {
                        textRenderer.DrawAligned(
                            componentX[componentIndex] - gameImage.TextWidth(
                                componentText[componentIndex],
                                componentTextSize[componentIndex]) / 2,
                            componentY[componentIndex],
                            componentText[componentIndex],
                            componentTextSize[componentIndex],
                            componentWhiteText[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.BackgroundPanel)
                    {
                        DrawBackgroundPanel(componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.HorizontalLine)
                    {
                        DrawLineX(componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.ScrollableTextBox)
                    {
                        DrawScrollableList(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex], listLength[componentIndex], listShownEntries[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.LeftAlignedTextInput ||
                        componentType[componentIndex] == MenuComponentType.CentredTextInput)
                    {
                        DrawInputBox(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex], componentText[componentIndex], componentTextSize[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.HorizontalOptions)
                    {
                        DrawHorizontalOptions(componentIndex, componentX[componentIndex], componentY[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.VerticalOptions)
                    {
                        DrawVerticalOptions(componentIndex, componentX[componentIndex], componentY[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.SelectableList)
                    {
                        DrawList(componentIndex, componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex], componentTextSize[componentIndex], componentTextList[componentIndex], listLength[componentIndex], listShownEntries[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.CurvedPanel)
                    {
                        DrawScrollCornerPanel(componentX[componentIndex], componentY[componentIndex], componentWidth[componentIndex], componentHeight[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.Picture)
                    {
                        DrawPicture(componentX[componentIndex], componentY[componentIndex], componentTextSize[componentIndex]);
                    }
                    else if (componentType[componentIndex] == MenuComponentType.Toggle)
                    {
                        panelRenderer.DrawBorderBox(
                            componentX[componentIndex],
                            componentY[componentIndex],
                            componentWidth[componentIndex],
                            componentHeight[componentIndex],
                            componentSelectedIndex[componentIndex] == 1);
                    }
                }
            }

            lastMouseButton = 0;
        }

        private void DrawInputBox(int componentIndex, int xPosition, int yPosition, int width, int height, string text, int fontIndex)
        {
            text = textRenderer.MaskPassword(
                text,
                componentIsPasswordField[componentIndex]);

            if (textRenderer.IsInputSelected(
                componentType[componentIndex],
                xPosition,
                yPosition,
                width,
                height,
                lastMouseButton,
                mouseX,
                mouseY))
            {
                selectedComponent = componentIndex;
            }

            textRenderer.DrawInput(
                componentType[componentIndex],
                xPosition,
                yPosition,
                text,
                fontIndex,
                componentWhiteText[componentIndex],
                selectedComponent == componentIndex);
        }

        public void DrawBackgroundPanel(int xPosition, int yPosition, int width, int height)
            => panelRenderer.DrawBackgroundPanel(
                xPosition,
                yPosition,
                width,
                height,
                isBackgroundPatternEnabled,
                baseScrollPic);

        public void DrawScrollCornerPanel(int xPosition, int yPosition, int width, int height)
            => panelRenderer.DrawScrollCornerPanel(
                xPosition,
                yPosition,
                width,
                height,
                baseScrollPic);

        private void DrawPicture(int xPosition, int yPosition, int pictureIndex)
        {
            gameImage.DrawPicture(xPosition, yPosition, pictureIndex);
        }

        private void DrawLineX(int xPosition, int yPosition, int width)
        {
            gameImage.DrawLineX(xPosition, yPosition, width, 0);
        }

        private void DrawScrollableList(int componentIndex, int xPosition, int yPosition, int width, int height, int fontIndex, string[] textList,
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
                MenuScrollbarState scrollbarState = MenuScrollbarStateCalculator.Calculate(
                    xPosition,
                    yPosition,
                    width,
                    height,
                    visibleEntries,
                    listLength,
                    shownEntries,
                    isScrollDragging[componentIndex],
                    mouseX,
                    mouseY,
                    mouseButton);
                shownEntries = scrollbarState.ShownEntries;
                listShownEntries[componentIndex] = shownEntries;
                isScrollDragging[componentIndex] = scrollbarState.IsDragging;
                panelRenderer.DrawScrollbar(
                    xPosition,
                    yPosition,
                    width,
                    height,
                    scrollbarState.ThumbOffset,
                    scrollbarState.ThumbSize,
                    baseScrollPic);
            }

            int remainingSpace = height - visibleEntries * gameImage.TextHeightNumber(fontIndex);
            int textY = yPosition + gameImage.TextHeightNumber(fontIndex) * 5 / 6 + remainingSpace / 2;

            for (int entryIndex = shownEntries; entryIndex < listLength; entryIndex += 1)
            {
                textRenderer.Draw(
                    xPosition + 2,
                    textY,
                    textList[entryIndex],
                    fontIndex,
                    componentWhiteText[componentIndex]);
                textY += gameImage.TextHeightNumber(fontIndex) - chatMenuTextHeightMod;

                if (textY >= yPosition + height)
                {
                    return;
                }
            }

        }

        private void DrawHorizontalOptions(int componentIndex, int xPosition, int yPosition, int fontIndex, string[] options)
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

        private void DrawVerticalOptions(int componentIndex, int xPosition, int yPosition, int fontIndex, string[] options)
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
        private void DrawList(int listIndex, int listX, int listY, int listWidth, int listHeight, int listTextSize, string[] listText,
                int listLength, int shownEntries)
        {
            int entryCount = listHeight / gameImage.TextHeightNumber(listTextSize);

            if (entryCount < listLength)
            {
                MenuScrollbarState scrollbarState = MenuScrollbarStateCalculator.Calculate(
                    listX,
                    listY,
                    listWidth,
                    listHeight,
                    entryCount,
                    listLength,
                    shownEntries,
                    isScrollDragging[listIndex],
                    mouseX,
                    mouseY,
                    mouseButton);
                shownEntries = scrollbarState.ShownEntries;
                listShownEntries[listIndex] = shownEntries;
                isScrollDragging[listIndex] = scrollbarState.IsDragging;
                panelRenderer.DrawScrollbar(
                    listX,
                    listY,
                    listWidth,
                    listHeight,
                    scrollbarState.ThumbOffset,
                    scrollbarState.ThumbSize,
                    baseScrollPic);
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
            componentType[menuItemsCount] = MenuComponentType.CentredText;
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
            => CreateCentredRectangleComponent(
                MenuComponentType.BackgroundPanel,
                xPosition,
                yPosition,
                width,
                height);

        public int DrawCurvedBox(int xPosition, int yPosition, int width, int height)
            => CreateCentredRectangleComponent(
                MenuComponentType.CurvedPanel,
                xPosition,
                yPosition,
                width,
                height);

        public int DrawArrow(int xPosition, int yPosition, int pictureIndex)
        {
            int pictureWidth = gameImage.PictureWidth[pictureIndex];
            int pictureHeight = gameImage.PictureHeight[pictureIndex];
            componentType[menuItemsCount] = MenuComponentType.Picture;
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
            => CreateListComponent(
                MenuComponentType.ScrollableTextBox,
                xPosition,
                yPosition,
                width,
                height,
                fontIndex,
                maxItems,
                isWhiteText);

        public int CreateTextInput(int xPosition, int yPosition, int width, int height, int fontIndex, int maxLength, bool isPasswordField,
                bool isWhiteText)
            => CreateTextInputComponent(
                MenuComponentType.LeftAlignedTextInput,
                xPosition,
                yPosition,
                width,
                height,
                fontIndex,
                maxLength,
                isPasswordField,
                isWhiteText);

        public int CreateInput(int xPosition, int yPosition, int width, int height, int fontIndex, int maxLength, bool isPasswordField,
                bool isWhiteText)
            => CreateTextInputComponent(
                MenuComponentType.CentredTextInput,
                xPosition,
                yPosition,
                width,
                height,
                fontIndex,
                maxLength,
                isPasswordField,
                isWhiteText);

        private int CreateTextInputComponent(
            MenuComponentType inputComponentType,
            int xPosition,
            int yPosition,
            int width,
            int height,
            int fontIndex,
            int maximumLength,
            bool isPasswordField,
            bool isWhiteText)
        {
            componentType[menuItemsCount] = inputComponentType;
            componentAcceptsInput[menuItemsCount] = true;
            componentIsPasswordField[menuItemsCount] = isPasswordField;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = fontIndex;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            copmonentInputMaxLength[menuItemsCount] = maximumLength;
            componentText[menuItemsCount] = string.Empty;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateList(int xPosition, int yPosition, int width, int height, int fontIndex, int maxItems, bool isWhiteText)
        {
            int componentIndex = CreateListComponent(
                MenuComponentType.SelectableList,
                xPosition,
                yPosition,
                width,
                height,
                fontIndex,
                maxItems,
                isWhiteText);
            componentSelectedIndex[componentIndex] = -1;
            componentHighlightedIndex[componentIndex] = -1;

            return componentIndex;
        }

        private int CreateListComponent(
            MenuComponentType listComponentType,
            int xPosition,
            int yPosition,
            int width,
            int height,
            int fontIndex,
            int maximumItems,
            bool isWhiteText)
        {
            componentType[menuItemsCount] = listComponentType;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = fontIndex;
            componentWhiteText[menuItemsCount] = isWhiteText;
            componentX[menuItemsCount] = xPosition;
            componentY[menuItemsCount] = yPosition;
            componentWidth[menuItemsCount] = width;
            componentHeight[menuItemsCount] = height;
            copmonentInputMaxLength[menuItemsCount] = maximumItems;
            componentTextList[menuItemsCount] = new string[maximumItems];
            listLength[menuItemsCount] = 0;
            listShownEntries[menuItemsCount] = 0;
            menuItemsCount += 1;

            return menuItemsCount - 1;
        }

        public int CreateButton(int xPosition, int yPosition, int width, int height)
            => CreateCentredRectangleComponent(
                MenuComponentType.Button,
                xPosition,
                yPosition,
                width,
                height);

        private int CreateCentredRectangleComponent(
            MenuComponentType rectangleComponentType,
            int xPosition,
            int yPosition,
            int width,
            int height)
        {
            componentType[menuItemsCount] = rectangleComponentType;
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

        private readonly GameImage gameImage;
        private readonly MenuPanelRenderer panelRenderer;
        private readonly MenuTextRenderer textRenderer;
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
        private readonly MenuComponentType[] componentType;
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
        public bool isListSelectionHighlighted;
        public static bool isBackgroundPatternEnabled = true;
        public static int baseScrollPic;
        public static int redMod = 114;
        public static int greenMod = 114;
        public static int blueMod = 176;
        public static int chatMenuTextHeightMod;
    }
}
