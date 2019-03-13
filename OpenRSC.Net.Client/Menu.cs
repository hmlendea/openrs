using Microsoft.Xna.Framework.Input;

using OpenRSC.Net.Client.Game;
using OpenRSC.Primitives.Mapping;

namespace OpenRSC.Net.Client
{
    public class Menu
    {
        public Menu(GraphicsEngine j1, int i)
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
            componentText = new string[i];
            componentTextList = new string[i][];
            scrollBarColour = rgbToIntMod(114, 114, 176);
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

        int rgbToIntMod(int r, int g, int b)
        {
            return ColourTranslator.ToArgb((redMod * r) / 114, (greenMod * g) / 114, (blueMod * b) / 176);
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
                gch++;
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

            return false;
        }

        public void keyPress(Keys key, char c)
        {
            if (key == 0)
            {
                return;
            }

            if (selectedComponent != -1 && componentText[selectedComponent] != null && componentAcceptsInput[selectedComponent])
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

                string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö0123456789!\"" + (char)243 + "$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
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

        protected void gee(int arg0, int x, int y, int width, int height)
        {
            gameImage.DrawBox(x, y, width, height, 0xffffff);
            gameImage.DrawHorizontalLine(x, y, width, gdc);
            gameImage.DrawVerticalLine(x, y, height, gdc);
            gameImage.DrawHorizontalLine(x, (y + height) - 1, width, gdf);
            gameImage.DrawVerticalLine((x + width) - 1, y, height, gdf);

            if (gbe[arg0] == 1)
            {
                for (int i = 0; i < height; i++)
                {
                    gameImage.DrawHorizontalLine(x + i, y + i, 1, 0);
                    gameImage.DrawHorizontalLine((x + width) - 1 - i, y + i, 1, 0);
                }
            }
        }

        protected void gef(int i, int x, int y, string text, int i1)
        {
            int j1 = y + gameImage.textHeightNumber(i1) / 3;
            geg(i, x, j1, text, i1);
        }

        protected void geg(int arg0, int x, int y, string text, int arg4)
        {
            int i;

            if (componentWhiteText[arg0])
            {
                i = 0xffffff;
            }
            else
            {
                i = 0;
            }

            gameImage.DrawString(text, x, y, arg4, i);
        }

        protected void drawInputBox(int arg0, int x, int y, int width, int height, string text, int fontIndex)
        {
            if (componentIsPasswordField[arg0])
            {
                int i = text.Length;
                text = "";
                for (int l = 0; l < i; l++)
                {
                    text = text + "X";
                }
            }

            if (componentType[arg0] == 5)
            {
                if (lastMouseButton == 1 && mouseX >= x && mouseY >= y - height / 2 && mouseX <= x + width && mouseY <= y + height / 2)
                {
                    selectedComponent = arg0;
                }
            }
            else if (componentType[arg0] == 6)
            {
                if (lastMouseButton == 1 && mouseX >= x - width / 2 && mouseY >= y - height / 2 && mouseX <= x + width / 2 && mouseY <= y + height / 2)
                {
                    selectedComponent = arg0;
                }

                x -= gameImage.textWidth(text, fontIndex) / 2;
            }

            if (selectedComponent == arg0)
            {
                text = text + "*";
            }

            int k = y + gameImage.textHeightNumber(fontIndex) / 3;
            geg(arg0, x, k, text, fontIndex);
        }

        public void gei(int x, int y, int width, int height)
        {
            gameImage.SetDimensions(x, y, x + width, y + height);
            gameImage.DrawBox(x, y, width, height, gdf);

            if (gdh)
            {
                for (int i = x - (y & 0x3f); i < x + width; i += 128)
                {
                    for (int k = y - (y & 0x1f); k < y + height; k += 128)
                    {
                        gameImage.DrawPicture(i, k, 6 + baseScrollPic, 128);
                    }
                }

            }

            gameImage.DrawHorizontalLine(x, y, width, gdc);
            gameImage.DrawHorizontalLine(x + 1, y + 1, width - 2, gdc);
            gameImage.DrawHorizontalLine(x + 2, y + 2, width - 4, gdd);
            gameImage.DrawVerticalLine(x, y, height, gdc);
            gameImage.DrawVerticalLine(x + 1, y + 1, height - 2, gdc);
            gameImage.DrawVerticalLine(x + 2, y + 2, height - 4, gdd);
            gameImage.DrawHorizontalLine(x, (y + height) - 1, width, gdf);
            gameImage.DrawHorizontalLine(x + 1, (y + height) - 2, width - 2, gdf);
            gameImage.DrawHorizontalLine(x + 2, (y + height) - 3, width - 4, gde);
            gameImage.DrawVerticalLine((x + width) - 1, y, height, gdf);
            gameImage.DrawVerticalLine((x + width) - 2, y + 1, height - 2, gdf);
            gameImage.DrawVerticalLine((x + width) - 3, y + 2, height - 4, gde);
            gameImage.ResetDimensions();
        }

        public void gej(int x, int y, int width, int height)
        {
            gameImage.DrawBox(x, y, width, height, 0);
            gameImage.DrawBoxEdge(x, y, width, height, gcn);
            gameImage.DrawBoxEdge(x + 1, y + 1, width - 2, height - 2, gda);
            gameImage.DrawBoxEdge(x + 2, y + 2, width - 4, height - 4, gdb);
            gameImage.DrawPicture(x, y, 2 + baseScrollPic);
            gameImage.DrawPicture((x + width) - 7, y, 3 + baseScrollPic);
            gameImage.DrawPicture(x, (y + height) - 7, 4 + baseScrollPic);
            gameImage.DrawPicture((x + width) - 7, (y + height) - 7, 5 + baseScrollPic);
        }

        protected void drawPicture(int x, int y, int size)
        {
            gameImage.DrawPicture(x, y, size);
        }

        protected void drawLineX(int x, int y, int width)
        {
            gameImage.DrawHorizontalLine(x, y, width, 0);
        }

        protected void gem(int arg0, int x, int y, int width, int height, int textSize, string[] texts,
                int arg7, int arg8)
        {
            int i = height / gameImage.textHeightNumber(textSize);
            if (arg8 > arg7 - i)
            {
                arg8 = arg7 - i;
            }

            if (arg8 < 0)
            {
                arg8 = 0;
            }

            listShownEntries[arg0] = arg8;
            if (i < arg7)
            {
                int k = (x + width) - 12;
                int i1 = ((height - 27) * i) / arg7;
                if (i1 < 6)
                {
                    i1 = 6;
                }

                int k1 = ((height - 27 - i1) * arg8) / (arg7 - i);
                if (mouseButton == 1 && mouseX >= k && mouseX <= k + 12)
                {
                    if (mouseY > y && mouseY < y + 12 && arg8 > 0)
                    {
                        arg8--;
                    }

                    if (mouseY > (y + height) - 12 && mouseY < y + height && arg8 < arg7 - i)
                    {
                        arg8++;
                    }

                    listShownEntries[arg0] = arg8;
                }
                if (mouseButton == 1 && (mouseX >= k && mouseX <= k + 12 || mouseX >= k - 12 && mouseX <= k + 24 && gan[arg0]))
                {
                    if (mouseY > y + 12 && mouseY < (y + height) - 12)
                    {
                        gan[arg0] = true;
                        int i2 = mouseY - y - 12 - i1 / 2;
                        arg8 = (i2 * arg7) / (height - 24);
                        if (arg8 > arg7 - i)
                        {
                            arg8 = arg7 - i;
                        }

                        if (arg8 < 0)
                        {
                            arg8 = 0;
                        }

                        listShownEntries[arg0] = arg8;
                    }
                }
                else
                {
                    gan[arg0] = false;
                }
                k1 = ((height - 27 - i1) * arg8) / (arg7 - i);
                drawScrollbar(x, y, width, height, k1, i1);
            }
            int l = height - i * gameImage.textHeightNumber(textSize);
            int j1 = y + (gameImage.textHeightNumber(textSize) * 5) / 6 + l / 2;
            for (int l1 = arg8; l1 < arg7; l1++)
            {
                geg(arg0, x + 2, j1, texts[l1], textSize);
                j1 += gameImage.textHeightNumber(textSize);
                if (j1 >= y + height)
                {
                    return;
                }
            }

        }

        protected void drawScrollbar(int x, int y, int width, int height, int j1, int k1)
        {
            int l1 = (x + width) - 12;
            gameImage.DrawBoxEdge(l1, y, 12, height, 0);// border
            gameImage.DrawPicture(l1 + 1, y + 1, baseScrollPic);// up arrow
            gameImage.DrawPicture(l1 + 1, (y + height) - 12, 1 + baseScrollPic);// down arrow
            gameImage.DrawHorizontalLine(l1, y + 13, 12, 0);// up arrow border
            gameImage.DrawHorizontalLine(l1, (y + height) - 13, 12, 0);// down arrow border
            gameImage.DrawBox(l1 + 1, y + 14, 11, height - 27, scrollBarColour);
            gameImage.DrawBox(l1 + 3, j1 + y + 14, 7, k1, scrollBarDraggingBarColor);// dragging bar
            gameImage.DrawVerticalLine(l1 + 2, j1 + y + 14, k1, scrollBarDraggingBarLine1Color);// dragging bar
            gameImage.DrawVerticalLine(l1 + 2 + 8, j1 + y + 14, k1, scrollBarDraggingBarLine2Color);// drawgging bar
        }

        protected void gfa(int arg0, int x, int y, int fontIndex, string[] texts)
        {
            int i = 0;
            int k = texts.Length;
            for (int l = 0; l < k; l++)
            {
                i += gameImage.textWidth(texts[l], fontIndex);
                if (l < k - 1)
                {
                    i += gameImage.textWidth("  ", fontIndex);
                }
            }

            int i1 = x - i / 2;
            int j1 = y + gameImage.textHeightNumber(fontIndex) / 3;
            for (int k1 = 0; k1 < k; k1++)
            {
                int l1;
                if (componentWhiteText[arg0])
                {
                    l1 = 0xffffff;
                }
                else
                {
                    l1 = 0;
                }

                if (mouseX >= i1 && mouseX <= i1 + gameImage.textWidth(texts[k1], fontIndex) && mouseY <= j1 && mouseY > j1 - gameImage.textHeightNumber(fontIndex))
                {
                    if (componentWhiteText[arg0])
                    {
                        l1 = 0x808080;
                    }
                    else
                    {
                        l1 = 0xffffff;
                    }

                    if (lastMouseButton == 1)
                    {
                        gbe[arg0] = k1;
                        componentSkip[arg0] = true;
                    }
                }
                if (gbe[arg0] == k1)
                {
                    if (componentWhiteText[arg0])
                    {
                        l1 = 0xff0000;
                    }
                    else
                    {
                        l1 = 0xc00000;
                    }
                }

                gameImage.DrawString(texts[k1], i1, j1, fontIndex, l1);
                i1 += gameImage.textWidth(texts[k1] + "  ", fontIndex);
            }

        }

        protected void gfb(int arg0, int x, int y, int fontIndex, string[] texts)
        {
            int k = y - (gameImage.textHeightNumber(fontIndex) * (texts.Length - 1)) / 2;

            for (int l = 0; l < texts.Length; l++)
            {
                int i1;

                if (componentWhiteText[arg0])
                {
                    i1 = 0xffffff;
                }
                else
                {
                    i1 = 0;
                }

                int j1 = gameImage.textWidth(texts[l], fontIndex);

                if (mouseX >= x - j1 / 2 && mouseX <= x + j1 / 2 && mouseY - 2 <= k && mouseY - 2 > k - gameImage.textHeightNumber(fontIndex))
                {
                    if (componentWhiteText[arg0])
                    {
                        i1 = 0x808080;
                    }
                    else
                    {
                        i1 = 0xffffff;
                    }

                    if (lastMouseButton == 1)
                    {
                        gbe[arg0] = l;
                        componentSkip[arg0] = true;
                    }
                }

                if (gbe[arg0] == l)
                {
                    if (componentWhiteText[arg0])
                    {
                        i1 = 0xff0000;
                    }
                    else
                    {
                        i1 = 0xc00000;
                    }
                }

                gameImage.DrawString(texts[l], x - j1 / 2, k, fontIndex, i1);
                k += gameImage.textHeightNumber(fontIndex);
            }
        }
        // drawList(x, componentX[x], componentY[x], componentWidth[x], componentHeight[x], componentTextSize[x], componentTextList[x], listLength[x], gbc[x]);
        protected void drawList(int listIndex, int listX, int listY, int listWidth, int listHeight, int listTextSize, string[] listText,
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
                        shownEntries++;
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

                gameImage.DrawString(listText[l1], listX + 2, j1, listTextSize, j2);
                j1 += gameImage.textHeightNumber(listTextSize);
                if (j1 >= listY + listHeight)
                {
                    return;
                }
            }

        }

        public int drawText(int i, int k, string s, int l, bool flag)
        {
            componentType[menuItemsCount] = 1;
            componentAcceptsInput[menuItemsCount] = true;
            componentSkip[menuItemsCount] = false;
            componentTextSize[menuItemsCount] = l;
            componentWhiteText[menuItemsCount] = flag;
            componentX[menuItemsCount] = i;
            componentY[menuItemsCount] = k;
            componentText[menuItemsCount] = s;
            return menuItemsCount++;
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
            return menuItemsCount++;
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
            return menuItemsCount++;
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
            return menuItemsCount++;
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
            componentTextList[menuItemsCount] = new string[k1];
            return menuItemsCount++;
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
            return menuItemsCount++;
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
            return menuItemsCount++;
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
            componentTextList[menuItemsCount] = new string[k1];
            listLength[menuItemsCount] = 0;
            listShownEntries[menuItemsCount] = 0;
            gbe[menuItemsCount] = -1;
            gbf[menuItemsCount] = -1;
            return menuItemsCount++;
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
            return menuItemsCount++;
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

        public void addListItem(int i, int k, string s)
        {
            componentTextList[i][k] = s;
            if (k + 1 > listLength[i])
            {
                listLength[i] = k + 1;
            }
        }

        public void addMessage(int arg0, string arg1, bool arg2)
        {
            int i = listLength[arg0]++;
            if (i >= copmonentInputMaxLength[arg0])
            {
                i--;
                listLength[arg0]--;
                for (int k = 0; k < i; k++)
                {
                    componentTextList[arg0][k] = componentTextList[arg0][k + 1];
                }
            }
            componentTextList[arg0][i] = arg1;
            if (arg2)
            {
                listShownEntries[arg0] = 0xf423f;
            }
        }

        public void updateText(int i, string s)
        {
            componentText[i] = s;
        }

        public string getText(int i)
        {
            if (componentText[i] == null)
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


        protected GraphicsEngine gameImage;
        int menuItemsCount;
        int gal;
        public bool[] componentAcceptsInput;
        public bool[] gan;
        public bool[] componentIsPasswordField;
        public bool[] componentSkip;
        public int[] listShownEntries;
        public int[] listLength;
        public int[] gbe;
        public int[] gbf;
        bool[] componentWhiteText;
        int[] componentX;
        int[] componentY;
        int[] componentType;
        int[] componentWidth;
        int[] componentHeight;
        int[] copmonentInputMaxLength;
        int[] componentTextSize;
        string[] componentText;
        string[][] componentTextList;
        int mouseX;
        int mouseY;
        int lastMouseButton;
        int mouseButton;
        int selectedComponent;
        int gch;
        int scrollBarColour;
        int scrollBarDraggingBarLine1Color;
        int scrollBarDraggingBarColor;
        int scrollBarDraggingBarLine2Color;
        int gcn;
        int gda;
        int gdb;
        int gdc;
        int gdd;
        int gde;
        int gdf;
        public bool gdg;
        public static bool gdh = true;
        public static int baseScrollPic;
        public static int redMod = 114;
        public static int greenMod = 114;
        public static int blueMod = 176;
    }
}
