namespace OpenRS.Net.Client.Input
{
    internal sealed class ContextMenuInputHandler(GameClient client)
    {
        private static string CancelMenuText => "Cancel";

        private static int CancelMenuActionIdentifier => 4000;

        private static string ChooseOptionTitle => "Choose option";

        private static string ChooseTargetText => "Choose a target";

        private static int MenuDisplayColour => 0xffff00;

        private static int MenuDisplayFontIndex => 1;

        private static int MenuDisplayX => 6;

        private static int MenuDisplayY => 14;

        private static int MenuHeightLimit => 315;

        private static int MenuHeightOffset => 7;

        private static int MenuItemHeight => 15;

        private static int MenuPadding => 5;

        private static int MenuWidthLimit => 510;

        private static int MaximumDisplayedMenuOptions => 20;

        internal void CheckMouseStatus()
        {
            AddCancelOptionWhenRequired();
            PopulateMenuIndexes();
            SortMenuIndexes();
            TrimMenuOptions();

            if (client.menuOptionsCount <= 0)
            {
                return;
            }

            DrawMenuStatusText();

            if (TryExecutePrimaryMenuAction())
            {
                return;
            }

            TryOpenMenu();
        }

        private void AddCancelOptionWhenRequired()
        {
            if (client.selectedSpell < 0 && client.selectedItem < 0)
            {
                return;
            }

            client.menuText1[client.menuOptionsCount] = CancelMenuText;
            client.menuText2[client.menuOptionsCount] = string.Empty;
            client.menuActionID[client.menuOptionsCount] = CancelMenuActionIdentifier;
            client.menuOptionsCount += 1;
        }

        private string BuildStatusText()
        {
            int firstTextEntryIndex = GetFirstTextEntryIndex();
            string statusText = null;

            if ((client.selectedItem >= 0 || client.selectedSpell >= 0) &&
                client.menuOptionsCount == 1)
            {
                statusText = ChooseTargetText;
            }
            else if ((client.selectedItem >= 0 || client.selectedSpell >= 0) &&
                client.menuOptionsCount > 1)
            {
                statusText =
                    "@whi@" +
                    client.menuText1[client.menuIndexes[0]] +
                    " " +
                    client.menuText2[client.menuIndexes[0]];
            }
            else if (firstTextEntryIndex != -1)
            {
                statusText =
                    client.menuText2[client.menuIndexes[firstTextEntryIndex]] +
                    ": @whi@" +
                    client.menuText1[client.menuIndexes[0]];
            }

            if (client.menuOptionsCount == 2 && statusText is not null)
            {
                statusText += "@whi@ / 1 more option";
            }

            if (client.menuOptionsCount > 2 && statusText is not null)
            {
                statusText += "@whi@ / " + (client.menuOptionsCount - 1) + " more options";
            }

            return statusText;
        }

        private void ClampMenuBounds()
        {
            if (client.menuX < 0)
            {
                client.menuX = 0;
            }

            if (client.menuY < 0)
            {
                client.menuY = 0;
            }

            if (client.menuX + client.menuWidth > MenuWidthLimit)
            {
                client.menuX = MenuWidthLimit - client.menuWidth;
            }

            if (client.menuY + client.menuHeight > MenuHeightLimit)
            {
                client.menuY = MenuHeightLimit - client.menuHeight;
            }
        }

        private void DrawMenuStatusText()
        {
            string statusText = BuildStatusText();

            if (statusText is not null)
            {
                client.GameGraphics.DrawString(
                    statusText,
                    MenuDisplayX,
                    MenuDisplayY,
                    MenuDisplayFontIndex,
                    MenuDisplayColour);
            }
        }

        private int GetFirstTextEntryIndex()
        {
            for (int menuSearchIndex = 0;
                menuSearchIndex < client.menuOptionsCount;
                menuSearchIndex += 1)
            {
                string menuText = client.menuText2[client.menuIndexes[menuSearchIndex]];

                if (menuText is null || menuText.Length <= 0)
                {
                    continue;
                }

                return menuSearchIndex;
            }

            return -1;
        }

        private void OpenMenu()
        {
            client.menuHeight = (client.menuOptionsCount + 1) * MenuItemHeight;
            client.menuWidth =
                client.GameGraphics.TextWidth(ChooseOptionTitle, MenuDisplayFontIndex) +
                MenuPadding;
            UpdateMenuWidth();
            client.menuX = client.mouseX - (client.menuWidth / 2);
            client.menuY = client.mouseY - MenuHeightOffset;
            client.menuShow = true;
            ClampMenuBounds();
            client.mouseButtonClick = 0;
        }

        private void PopulateMenuIndexes()
        {
            for (int menuIndex = 0; menuIndex < client.menuOptionsCount; menuIndex += 1)
            {
                client.menuIndexes[menuIndex] = menuIndex;
            }
        }

        private void SortMenuIndexes()
        {
            bool isSorted = false;

            while (!isSorted)
            {
                isSorted = true;

                for (int menuIndex = 0;
                    menuIndex < client.menuOptionsCount - 1;
                    menuIndex += 1)
                {
                    if (!ShouldSwapMenuIndexes(menuIndex))
                    {
                        continue;
                    }

                    SwapMenuIndexes(menuIndex);
                    isSorted = false;
                }
            }
        }

        private bool ShouldSwapMenuIndexes(int menuIndex)
        {
            int firstIndex = client.menuIndexes[menuIndex];
            int secondIndex = client.menuIndexes[menuIndex + 1];
            return client.menuActionID[firstIndex] > client.menuActionID[secondIndex];
        }

        private void SwapMenuIndexes(int menuIndex)
        {
            int firstIndex = client.menuIndexes[menuIndex];
            client.menuIndexes[menuIndex] = client.menuIndexes[menuIndex + 1];
            client.menuIndexes[menuIndex + 1] = firstIndex;
        }

        private bool TryExecutePrimaryMenuAction()
        {
            if ((!client.configOneMouseButton && client.mouseButtonClick == 1) ||
                (client.configOneMouseButton &&
                client.mouseButtonClick == 1 &&
                client.menuOptionsCount == 1))
            {
                client.MenuClick(client.menuIndexes[0]);
                client.mouseButtonClick = 0;
                return true;
            }

            return false;
        }

        private void TrimMenuOptions()
        {
            if (client.menuOptionsCount > MaximumDisplayedMenuOptions)
            {
                client.menuOptionsCount = MaximumDisplayedMenuOptions;
            }
        }

        private void TryOpenMenu()
        {
            if ((!client.configOneMouseButton && client.mouseButtonClick == 2) ||
                (client.configOneMouseButton && client.mouseButtonClick == 1))
            {
                OpenMenu();
            }
        }

        private void UpdateMenuWidth()
        {
            for (int menuIndex = 0; menuIndex < client.menuOptionsCount; menuIndex += 1)
            {
                int entryTextWidth =
                    client.GameGraphics.TextWidth(
                        client.menuText1[menuIndex] + " " + client.menuText2[menuIndex],
                        MenuDisplayFontIndex) +
                    MenuPadding;

                if (entryTextWidth > client.menuWidth)
                {
                    client.menuWidth = entryTextWidth;
                }
            }
        }
    }
}