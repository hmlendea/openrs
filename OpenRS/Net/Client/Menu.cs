using System;
using OpenRS.Net.Client.Game;
using Microsoft.Xna.Framework.Input;

namespace OpenRS.Net.Client
{
    public sealed class Menu
    {


        public Menu(GameImage j1, int i)
        {
            selectedComponent = -1;
            gdg = true;
            gameImage = j1;
            gal = i;
            componentAcceptsInput = new bool[i];
            gan = new bool[i];
            componentIsPasswordField = new bool[i];
            componentSkip = new bool[i];
            componentWhiteText = new bool[i];
            listShownEntries = new int[i];
            listLength = new int[i];
            gbe = new int[i];
            gbf = new int[i];
            componentX = new int[i];
            componentY = new int[i];
            componentType = new int[i];
            componentWidth = new int[i];
            componentHeight = new int[i];
            copmonentInputMaxLength = new int[i];
            componentTextSize = new int[i];
            componentText = new String[i];
            componentTextList = new String[i][];
            scrollBarGradientColorTop = rgbToIntMod(114, 114, 176);
            scrollBarGradientColorBottom = rgbToIntMod(14, 14, 62);
            scrollBarDraggingBarLine1Color = rgbToIntMod(200, 208, 232);
            scrollBarDraggingBarColor = rgbToIntMod(96, 129, 184);
            scrollBarDraggingBarLine2Color = rgbToIntMod(53, 95, 115);
            gcn = rgbToIntMod(117, 142, 171);
            gda = rgbToIntMod(98, 122, 158);
            gdb = rgbToIntMod(86, 100, 136);
            gdc = rgbToIntMod(135, 146, 179);
            gdd = rgbToIntMod(97, 112, 151);
            gde = rgbToIntMod(88, 102, 136);
            gdf = rgbToIntMod(84, 93, 120);
        }

        public int rgbToIntMod(int i, int k, int l)
        {
            return GameImage.rgbToInt((redMod * i) / 114, (greenMod * k) / 114, (blueMod * l) / 176);
        }

        public void mouseClick(int mouseX, int mouseY, int lastMouseButton, int mouseButton)
        {
            this.mouseX = mouseX;
            this.mouseY = mouseY;
            this.mouseButton = mouseButton;
            if (lastMouseButton != 0)
            {
                this.lastMouseButton = lastMouseButton;
            }

            if (lastMouseButton == 1)
            {
                for (int i = 0; i < menuItemsCount; i++)
                {
                    if (componentAcceptsInput[i] && componentType[i] == 10 && this.mouseX >= componentX[i] && this.mouseY >= componentY[i] && this.mouseX <= componentX[i] + componentWidth[i] && this.mouseY <= componentY[i] + componentHeight[i])
                    {
                        componentSkip[i] = true;
                    }

                    if (componentAcceptsInput[i] && componentType[i] == 14 && this.mouseX >= componentX[i] && this.mouseY >= componentY[i] && this.mouseX <= componentX[i] + componentWidth[i] && this.mouseY <= componentY[i] + componentHeight[i])
                    {
                        gbe[i] = 1 - gbe[i];
                    }
                }

            }
            if (mouseButton == 1)
            {
                gch += 1;
            }
            else
            {
                gch = 0;
            }

            if (lastMouseButton == 1 || gch > 20)
            {
                for (int k = 0; k < menuItemsCount; k++)
                {
                    if (componentAcceptsInput[k] && componentType[k] == 15 && this.mouseX >= componentX[k] && this.mouseY >= componentY[k] && this.mouseX <= componentX[k] + componentWidth[k] && this.mouseY <= componentY[k] + componentHeight[k])
                    {
                        componentSkip[k] = true;
                    }
                }

                gch -= 5;
            }
        }

        public bool isClicked(int i)
        {
            if (componentAcceptsInput[i] && componentSkip[i])
            {
                componentSkip[i] = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void keyPress(Keys key, char c)
        {
            if (key == 0)
            {
                return;
            }

            if (selectedComponent != -1 && componentText[selectedComponent] is not null && componentAcceptsInput[selectedComponent])
            {
                int i = componentText[selectedComponent].Length;
                if (key == Keys.Back && i > 0)
                {
                    componentText[selectedComponent] = componentText[selectedComponent].Substring(0, i - 1);
                }

                if ((key == Keys.Enter) && i > 0)
                {
                    componentSkip[selectedComponent] = true;
                }

                String s = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö0123456789!\"" + (char)243 + "$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
                if (i < copmonentInputMaxLength[selectedComponent])
                {
                    for (int k = 0; k < s.Length; k++)
                    {
                        if (c == s[k])
                        {
                            componentText[selectedComponent] += c;
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

        public void drawMenu()
        {
            for (int i = 0; i < menuItemsCount; i++)
            {
                if (componentAcceptsInput[i])
                {
                    if (componentType[i] == 0)
                    {
                        gef(i, componentX[i], componentY[i], componentText[i], componentTextSize[i]);
                    }
                    else if (componentType[i] == 1)
                    {
                        gef(i, componentX[i] - gameImage.textWidth(componentText[i], componentTextSize[i]) / 2, componentY[i], componentText[i], componentTextSize[i]);
                    }
                    else if (componentType[i] == 2)
                    {
                        gei(componentX[i], componentY[i], componentWidth[i], componentHeight[i]);
                    }
                    else if (componentType[i] == 3)
                    {
                        drawLineX(componentX[i], componentY[i], componentWidth[i]);
                    }
                    else if (componentType[i] == 4)
                    {
                        gem(i, componentX[i], componentY[i], componentWidth[i], componentHeight[i], componentTextSize[i], componentTextList[i], listLength[i], listShownEntries[i]);
                    }
                    else if (componentType[i] == 5 || componentType[i] == 6)
                    {
                        drawInputBox(i, componentX[i], componentY[i], componentWidth[i], componentHeight[i], componentText[i], componentTextSize[i]);
                    }
                    else if (componentType[i] == 7)
                    {
                        gfa(i, componentX[i], componentY[i], componentTextSize[i], componentTextList[i]);
                    }
                    else if (componentType[i] == 8)
                    {
                        gfb(i, componentX[i], componentY[i], componentTextSize[i], componentTextList[i]);
                    }
                    else if (componentType[i] == 9)
                    {
                        drawList(i, componentX[i], componentY[i], componentWidth[i], componentHeight[i], componentTextSize[i], componentTextList[i], listLength[i], listShownEntries[i]);
                    }
                    else if (componentType[i] == 11)
                    {
                        gej(componentX[i], componentY[i], componentWidth[i], componentHeight[i]);
                    }
                    else if (componentType[i] == 12)
                    {
                        drawPicture(componentX[i], componentY[i], componentTextSize[i]);
                    }
                    else if (componentType[i] == 14)
                    {
                        gee(i, componentX[i], componentY[i], componentWidth[i], componentHeight[i]);
                    }
                }
            }

            lastMouseButton = 0;
        }

        protected void gee(int componentIndex, int x, int y, int w, int h)
        {
            gameImage.drawBox(x, y, w, h, 0xffffff);
            gameImage.drawLineX(x, y, w, gdc);
            gameImage.drawLineY(x, y, h, gdc);
			gameImage.drawLineX(x, (y + h) - 1, w, gdf);
			gameImage.drawLineY((x + w) - 1, y, h, gdf);
            if (gbe[componentIndex].Equals(1))
            {
                for (int i = 0; i < h; i++)
                {
                    gameImage.drawLineX(x + i, y + i, 1, 0);
                    gameImage.drawLineX((x + w) - 1 - i, y + i, 1, 0);
                }

            }
        }

        protected void gef(int i, int k, int l, String s, int i1)
        {
            int j1 = l + gameImage.textHeightNumber(i1) / 3;
            geg(i, k, j1, s, i1);
        }

        protected void geg(int componentIndex, int xPosition, int yPosition, String text, int fontIndex)
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

            gameImage.drawString(text, xPosition, yPosition, fontIndex, textColour);
        }

        protected void drawInputBox(int componentIndex, int xPosition, int yPosition, int width, int height, String text, int fontIndex)
        {
            if (componentIsPasswordField[componentIndex])
            {
                int maskedLength = text.Length;
                text = "";
                for (int l = 0; l < maskedLength; l++)
                {
                    text = text + "X";
                }
            }

            if (componentType[componentIndex].Equals(5))
            {
                if (lastMouseButton.Equals(1) && mouseX >= xPosition && mouseY >= yPosition - height / 2 && mouseX <= xPosition + width && mouseY <= yPosition + height / 2)
                {
                    selectedComponent = componentIndex;
                }
            }
            else
            {
                if (componentType[componentIndex].Equals(6))
                {
                    if (lastMouseButton.Equals(1) && mouseX >= xPosition - width / 2 && mouseY >= yPosition - height / 2 && mouseX <= xPosition + width / 2 && mouseY <= yPosition + height / 2)
                    {
                        selectedComponent = componentIndex;
                    }

                    xPosition -= gameImage.textWidth(text, fontIndex) / 2;
                }
            }

            if (selectedComponent.Equals(componentIndex))
            {
                text = text + "*";
            }

            int textY = yPosition + gameImage.textHeightNumber(fontIndex) / 3;
            geg(componentIndex, xPosition, textY, text, fontIndex);
        }

        public void gei(int xPosition, int yPosition, int width, int height)
        {
            gameImage.setDimensions(xPosition, yPosition, xPosition + width, yPosition + height);
            gameImage.drawGradientBox(xPosition, yPosition, width, height, gdf, gdc);
            if (gdh)
            {
                for (int i = xPosition - (yPosition & 0x3f); i < xPosition + width; i += 128)
                {
                    for (int k = yPosition - (yPosition & 0x1f); k < yPosition + height; k += 128)
                    {
                        gameImage.drawPicture(i, k, 6 + baseScrollPic, 128);
                    }
                }

            }
            gameImage.drawLineX(xPosition, yPosition, width, gdc);
            gameImage.drawLineX(xPosition + 1, yPosition + 1, width - 2, gdc);
            gameImage.drawLineX(xPosition + 2, yPosition + 2, width - 4, gdd);
            gameImage.drawLineY(xPosition, yPosition, height, gdc);
            gameImage.drawLineY(xPosition + 1, yPosition + 1, height - 2, gdc);
            gameImage.drawLineY(xPosition + 2, yPosition + 2, height - 4, gdd);
            gameImage.drawLineX(xPosition, (yPosition + height) - 1, width, gdf);
            gameImage.drawLineX(xPosition + 1, (yPosition + height) - 2, width - 2, gdf);
            gameImage.drawLineX(xPosition + 2, (yPosition + height) - 3, width - 4, gde);
            gameImage.drawLineY((xPosition + width) - 1, yPosition, height, gdf);
            gameImage.drawLineY((xPosition + width) - 2, yPosition + 1, height - 2, gdf);
            gameImage.drawLineY((xPosition + width) - 3, yPosition + 2, height - 4, gde);
            gameImage.resetDimensions();
        }

        public void gej(int i, int k, int l, int i1)
        {
            gameImage.drawBox(i, k, l, i1, 0);
            gameImage.drawBoxEdge(i, k, l, i1, gcn);
            gameImage.drawBoxEdge(i + 1, k + 1, l - 2, i1 - 2, gda);
            gameImage.drawBoxEdge(i + 2, k + 2, l - 4, i1 - 4, gdb);
            gameImage.drawPicture(i, k, 2 + baseScrollPic);
            gameImage.drawPicture((i + l) - 7, k, 3 + baseScrollPic);
            gameImage.drawPicture(i, (k + i1) - 7, 4 + baseScrollPic);
            gameImage.drawPicture((i + l) - 7, (k + i1) - 7, 5 + baseScrollPic);
        }

        protected void drawPicture(int i, int k, int l)
        {
            gameImage.drawPicture(i, k, l);
        }

        protected void drawLineX(int i, int k, int l)
        {
            gameImage.drawLineX(i, k, l, 0);
        }

        protected void gem(int componentIndex, int xPosition, int yPosition, int width, int height, int fontIndex, String[] textList,
                int listLength, int shownEntries)
        {
            int visibleEntries = height / gameImage.textHeightNumber(fontIndex);

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
                int scrollbarX = (xPosition + width) - 12;
                int scrollbarThumbSize = ((height - 27) * visibleEntries) / listLength;

                if (scrollbarThumbSize < 6)
                {
                    scrollbarThumbSize = 6;
                }

                int scrollbarThumbOffset = ((height - 27 - scrollbarThumbSize) * shownEntries) / (listLength - visibleEntries);

                if (mouseButton.Equals(1) && mouseX >= scrollbarX && mouseX <= scrollbarX + 12)
                {
                    if (mouseY > yPosition && mouseY < yPosition + 12 && shownEntries > 0)
                    {
                        shownEntries -= 1;
                    }

                    if (mouseY > (yPosition + height) - 12 && mouseY < yPosition + height && shownEntries < listLength - visibleEntries)
                    {
                        shownEntries += 1;
                    }

                    listShownEntries[componentIndex] = shownEntries;
                }

                if (mouseButton.Equals(1) && (mouseX >= scrollbarX && mouseX <= scrollbarX + 12 || mouseX >= scrollbarX - 12 && mouseX <= scrollbarX + 24 && gan[componentIndex]))
                {
                    if (mouseY > yPosition + 12 && mouseY < (yPosition + height) - 12)
                    {
                        gan[componentIndex] = true;
                        int dragOffset = mouseY - yPosition - 12 - scrollbarThumbSize / 2;
                        shownEntries = (dragOffset * listLength) / (height - 24);

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
                    gan[componentIndex] = false;
                }

                scrollbarThumbOffset = ((height - 27 - scrollbarThumbSize) * shownEntries) / (listLength - visibleEntries);
                drawScrollbar(xPosition, yPosition, width, height, scrollbarThumbOffset, scrollbarThumbSize);
            }

            int remainingSpace = height - visibleEntries * gameImage.textHeightNumber(fontIndex);
            int textY = yPosition + (gameImage.textHeightNumber(fontIndex) * 5) / 6 + remainingSpace / 2;

            for (int entryIndex = shownEntries; entryIndex < listLength; entryIndex++)
            {
                geg(componentIndex, xPosition + 2, textY, textList[entryIndex], fontIndex);
                textY += gameImage.textHeightNumber(fontIndex) - chatMenuTextHeightMod;

                if (textY >= yPosition + height)
                {
                    return;
                }
            }

        }

        protected void drawScrollbar(int i, int k, int l, int i1, int j1, int k1)
        {
            int l1 = (i + l) - 12;
            gameImage.drawBoxEdge(l1, k, 12, i1, 0);// border
            gameImage.drawPicture(l1 + 1, k + 1, baseScrollPic);// up arrow
            gameImage.drawPicture(l1 + 1, (k + i1) - 12, 1 + baseScrollPic);// down arrow
            gameImage.drawLineX(l1, k + 13, 12, 0);// up arrow border
            gameImage.drawLineX(l1, (k + i1) - 13, 12, 0);// down arrow border
            gameImage.drawGradientBox(l1 + 1, k + 14, 11, i1 - 27, scrollBarGradientColorTop, scrollBarGradientColorBottom);// background gradient
            gameImage.drawBox(l1 + 3, j1 + k + 14, 7, k1, scrollBarDraggingBarColor);// dragging bar
            gameImage.drawLineY(l1 + 2, j1 + k + 14, k1, scrollBarDraggingBarLine1Color);// dragging bar
            gameImage.drawLineY(l1 + 2 + 8, j1 + k + 14, k1, scrollBarDraggingBarLine2Color);// drawgging bar
        }

        protected void gfa(int componentIndex, int xPosition, int yPosition, int fontIndex, String[] options)
        {
            int totalWidth = 0;
            int optionCount = options.Length;

            for (int l = 0; l < optionCount; l++)
            {
                totalWidth += gameImage.textWidth(options[l], fontIndex);

                if (l < optionCount - 1)
                {
                    totalWidth += gameImage.textWidth("  ", fontIndex);
                }
            }

            int currentX = xPosition - totalWidth / 2;
            int textY = yPosition + gameImage.textHeightNumber(fontIndex) / 3;

            for (int optionIndex = 0; optionIndex < optionCount; optionIndex++)
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

                if (mouseX >= currentX && mouseX <= currentX + gameImage.textWidth(options[optionIndex], fontIndex) && mouseY <= textY && mouseY > textY - gameImage.textHeightNumber(fontIndex))
                {
                    if (componentWhiteText[componentIndex])
                    {
                        textColour = 0x808080;
                    }
                    else
                    {
                        textColour = 0xffffff;
                    }

                    if (lastMouseButton.Equals(1))
                    {
                        gbe[componentIndex] = optionIndex;
                        componentSkip[componentIndex] = true;
                    }
                }

                if (gbe[componentIndex].Equals(optionIndex))
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

                gameImage.drawString(options[optionIndex], currentX, textY, fontIndex, textColour);
                currentX += gameImage.textWidth(options[optionIndex] + "  ", fontIndex);
            }

        }

        protected void gfb(int componentIndex, int xPosition, int yPosition, int fontIndex, String[] options)
        {
            int optionCount = options.Length;
            int currentY = yPosition - (gameImage.textHeightNumber(fontIndex) * (optionCount - 1)) / 2;

            for (int optionIndex = 0; optionIndex < optionCount; optionIndex++)
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

                int optionWidth = gameImage.textWidth(options[optionIndex], fontIndex);

                if (mouseX >= xPosition - optionWidth / 2 && mouseX <= xPosition + optionWidth / 2 && mouseY - 2 <= currentY && mouseY - 2 > currentY - gameImage.textHeightNumber(fontIndex))
                {
                    if (componentWhiteText[componentIndex])
                    {
                        textColour = 0x808080;
                    }
                    else
                    {
                        textColour = 0xffffff;
                    }

                    if (lastMouseButton.Equals(1))
                    {
                        gbe[componentIndex] = optionIndex;
                        componentSkip[componentIndex] = true;
                    }
                }

                if (gbe[componentIndex].Equals(optionIndex))
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

                gameImage.drawString(options[optionIndex], xPosition - optionWidth / 2, currentY, fontIndex, textColour);
                currentY += gameImage.textHeightNumber(fontIndex);
            }

        }
        // drawList(x, componentX[x], componentY[x], componentWidth[x], componentHeight[x], componentTextSize[x], componentTextList[x], listLength[x], gbc[x]);
        protected void drawList(int listIndex, int listX, int listY, int listWidth, int listHeight, int listTextSize, String[] listText,
                int listLength, int shownEntries)
        {
            int entryCount = listHeight / gameImage.textHeightNumber(listTextSize);
            if (entryCount < listLength)
            {
                int k = (listX + listWidth) - 12;
                int i1 = ((listHeight - 27) * entryCount) / listLength;
                if (i1 < 6)
                {
                    i1 = 6;
                }

                int k1 = ((listHeight - 27 - i1) * shownEntries) / (listLength - entryCount);
                if (mouseButton == 1 && mouseX >= k && mouseX <= k + 12)
                {
                    if (mouseY > listY && mouseY < listY + 12 && shownEntries > 0)
                    {
                        shownEntries--;
                    }

                    if (mouseY > (listY + listHeight) - 12 && mouseY < listY + listHeight && shownEntries < listLength - entryCount)
                    {
                        shownEntries += 1;
                    }

                    listShownEntries[listIndex] = shownEntries;
                }
                if (mouseButton == 1 && (mouseX >= k && mouseX <= k + 12 || mouseX >= k - 12 && mouseX <= k + 24 && gan[listIndex]))
                {
                    if (mouseY > listY + 12 && mouseY < (listY + listHeight) - 12)
                    {
                        gan[listIndex] = true;
                        int i2 = mouseY - listY - 12 - i1 / 2;
                        shownEntries = (i2 * listLength) / (listHeight - 24);
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
                    gan[listIndex] = false;
                }
                k1 = ((listHeight - 27 - i1) * shownEntries) / (listLength - entryCount);
                drawScrollbar(listX, listY, listWidth, listHeight, k1, i1);
            }
            else
            {
                shownEntries = 0;
                listShownEntries[listIndex] = 0;
            }
            gbf[listIndex] = -1;
            int l = listHeight - entryCount * gameImage.textHeightNumber(listTextSize);
            int j1 = listY + (gameImage.textHeightNumber(listTextSize) * 5) / 6 + l / 2;
            for (int l1 = shownEntries; l1 < listLength; l1++)
            {
                int j2;
                if (componentWhiteText[listIndex])
                {
                    j2 = 0xffffff;
                }
                else
                {
                    j2 = 0;
                }

                if (mouseX >= listX + 2 && mouseX <= listX + 2 + gameImage.textWidth(listText[l1], listTextSize) && mouseY - 2 <= j1 && mouseY - 2 > j1 - gameImage.textHeightNumber(listTextSize))
                {
                    if (componentWhiteText[listIndex])
                    {
                        j2 = 0x808080;
                    }
                    else
                    {
                        j2 = 0xffffff;
                    }

                    gbf[listIndex] = l1;
                    if (lastMouseButton == 1)
                    {
                        gbe[listIndex] = l1;
                        componentSkip[listIndex] = true;
                    }
                }
                if (gbe[listIndex] == l1 && gdg)
                {
                    j2 = 0xff0000;
                }

                gameImage.drawString(listText[l1], listX + 2, j1, listTextSize, j2);
                j1 += gameImage.textHeightNumber(listTextSize);
                if (j1 >= listY + listHeight)
                {
                    return;
                }
            }

        }

        public int drawText(int i, int k, String s, int l, bool flag)
        {
            componentType[menuItemsCount] = 1;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = l;
            componentWhiteText[menuItemsCount] = flag;
            componentX[menuItemsCount] = i;
            componentY[menuItemsCount] = k;
            componentText[menuItemsCount] = s;
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int drawButton(int i, int k, int l, int i1)
        {
            componentType[menuItemsCount] = 2;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = i - l / 2;
            componentY[menuItemsCount] = k - i1 / 2;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int drawCurvedBox(int i, int k, int l, int i1)
        {
            componentType[menuItemsCount] = 11;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = i - l / 2;
            componentY[menuItemsCount] = k - i1 / 2;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int drawArrow(int i, int k, int l)
        {
            int i1 = gameImage.pictureWidth[l];
            int j1 = gameImage.pictureHeight[l];
            componentType[menuItemsCount] = 12;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = i - i1 / 2;
            componentY[menuItemsCount] = k - j1 / 2;
            componentWidth[menuItemsCount] = i1;
            componentHeight[menuItemsCount] = j1;
            componentTextSize[menuItemsCount] = l;
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int gfh(int i, int k, int l, int i1, int j1, int k1, bool flag)
        {
            componentType[menuItemsCount] = 4;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = i;
            componentY[menuItemsCount] = k;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            componentWhiteText[menuItemsCount] = flag;
            componentTextSize[menuItemsCount] = j1;
            copmonentInputMaxLength[menuItemsCount] = k1;
            listLength[menuItemsCount] = 0;
            listShownEntries[menuItemsCount] = 0;
            componentTextList[menuItemsCount] = new String[k1];
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int gfi(int i, int k, int l, int i1, int j1, int k1, bool flag,
                bool flag1)
        {
            componentType[menuItemsCount] = 5;
            componentAcceptsInput[menuItemsCount] = true;
            componentIsPasswordField[menuItemsCount] = flag;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = j1;
            componentWhiteText[menuItemsCount] = flag1;
            componentX[menuItemsCount] = i;
            componentY[menuItemsCount] = k;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            copmonentInputMaxLength[menuItemsCount] = k1;
            componentText[menuItemsCount] = "";
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int createInput(int i, int k, int l, int i1, int j1, int k1, bool flag,
                bool flag1)
        {
            componentType[menuItemsCount] = 6;
            componentAcceptsInput[menuItemsCount] = true;
            componentIsPasswordField[menuItemsCount] = flag;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = j1;
            componentWhiteText[menuItemsCount] = flag1;
            componentX[menuItemsCount] = i;
            componentY[menuItemsCount] = k;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            copmonentInputMaxLength[menuItemsCount] = k1;
            componentText[menuItemsCount] = "";
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int createList(int i, int k, int l, int i1, int j1, int k1, bool flag)
        {
            componentType[menuItemsCount] = 9;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = j1;
            componentWhiteText[menuItemsCount] = flag;
            componentX[menuItemsCount] = i;
            componentY[menuItemsCount] = k;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            copmonentInputMaxLength[menuItemsCount] = k1;
            componentTextList[menuItemsCount] = new String[k1];
            listLength[menuItemsCount] = 0;
            listShownEntries[menuItemsCount] = 0;
            gbe[menuItemsCount] = -1;
            gbf[menuItemsCount] = -1;
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public int createButton(int i, int k, int l, int i1)
        {
            componentType[menuItemsCount] = 10;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentX[menuItemsCount] = i - l / 2;
            componentY[menuItemsCount] = k - i1 / 2;
            componentWidth[menuItemsCount] = l;
            componentHeight[menuItemsCount] = i1;
            menuItemsCount += 1;
            return menuItemsCount - 1;
        }

        public void clearList(int i)
        {
            listLength[i] = 0;
        }

        public void switchList(int i)
        {
            listShownEntries[i] = 0;
            gbf[i] = -1;
        }

        public void addListItem(int i, int k, String s)
        {
            componentTextList[i][k] = s;
            if (k + 1 > listLength[i])
            {
                listLength[i] = k + 1;
            }
        }

        public void addMessage(int componentIndex, String messageText, bool isScrollToBottom)
        {
            int messageIndex = listLength[componentIndex] += 1;
            if (messageIndex >= copmonentInputMaxLength[componentIndex])
            {
                messageIndex--;
                listLength[componentIndex]--;
                for (int k = 0; k < messageIndex; k++)
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

        public void updateText(int i, String s)
        {
            componentText[i] = s;
        }

        public String getText(int i)
        {
            if (componentText[i] is null)
            {
                return "null";
            }
            else
            {
                return componentText[i];
            }
        }

        public void enableInput(int i)
        {
            componentAcceptsInput[i] = true;
        }

        public void disableInput(int i)
        {
            componentAcceptsInput[i] = false;
        }

        public void setFocus(int i)
        {
            selectedComponent = i;
        }

        public int getEntryHighlighted(int i)
        {
            int k = gbf[i];
            return k;
        }


        protected GameImage gameImage;
        private int menuItemsCount;
        private int gal;
        public bool[] componentAcceptsInput;
        public bool[] gan;
        public bool[] componentIsPasswordField;
        public bool[] componentSkip;
        public int[] listShownEntries;
        public int[] listLength;
        public int[] gbe;
        public int[] gbf;
        private bool[] componentWhiteText;
        private int[] componentX;
        private int[] componentY;
        private int[] componentType;
        private int[] componentWidth;
        private int[] componentHeight;
        private int[] copmonentInputMaxLength;
        private int[] componentTextSize;
        private String[] componentText;
        private String[][] componentTextList;
        private int mouseX;
        private int mouseY;
        private int lastMouseButton;
        private int mouseButton;
        private int selectedComponent;
        private int gch;
        private int scrollBarGradientColorTop;
        private int scrollBarGradientColorBottom;
        private int scrollBarDraggingBarLine1Color;
        private int scrollBarDraggingBarColor;
        private int scrollBarDraggingBarLine2Color;
        private int gcn;
        private int gda;
        private int gdb;
        private int gdc;
        private int gdd;
        private int gde;
        private int gdf;
        public bool gdg;
        public static bool gdh = true;
        public static int baseScrollPic;
        public static int redMod = 114;
        public static int greenMod = 114;
        public static int blueMod = 176;
        public static int chatMenuTextHeightMod;
    }
}
