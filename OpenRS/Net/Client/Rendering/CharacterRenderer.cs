using System;

using OpenRS.Models;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class CharacterRenderer(GameClient client, Action<int, int, int, int, Item> recordItemSprite)
    {

        public void DrawPrayerMagicMenu(bool canClick)
        {
            int menuX = client.gameGraphics.gameWidth - 199;
            int menuY = 36;
            client.gameGraphics.DrawPicture(menuX - 49, 3, client.baseInventoryPic + 4);
            int menuWidth = 196;
            int menuHeight = 182;
            int magicTabColour = GameImage.RgbToInt(160, 160, 160);
            int prayerTabColour = GameImage.RgbToInt(160, 160, 160);

            if (client.menuMagicPrayersSelected == 0)
            {
                magicTabColour = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                prayerTabColour = GameImage.RgbToInt(220, 220, 220);
            }

            client.gameGraphics.DrawBoxAlpha(menuX, menuY, menuWidth / 2, 24, magicTabColour, 128);
            client.gameGraphics.DrawBoxAlpha(menuX + menuWidth / 2, menuY, menuWidth / 2, 24, prayerTabColour, 128);
            client.gameGraphics.DrawBoxAlpha(menuX, menuY + 24, menuWidth, 90, GameImage.RgbToInt(220, 220, 220), 128);
            client.gameGraphics.DrawBoxAlpha(menuX, menuY + 24 + 90, menuWidth, menuHeight - 90 - 24, GameImage.RgbToInt(160, 160, 160), 128);
            client.gameGraphics.DrawLineX(menuX, menuY + 24, menuWidth, 0);
            client.gameGraphics.DrawLineY(menuX + menuWidth / 2, menuY, 24, 0);
            client.gameGraphics.DrawLineX(menuX, menuY + 113, menuWidth, 0);
            client.gameGraphics.DrawText("Magic", menuX + menuWidth / 4, menuY + 16, 4, 0);
            client.gameGraphics.DrawText("Prayers", menuX + menuWidth / 4 + menuWidth / 2, menuY + 16, 4, 0);

            if (client.menuMagicPrayersSelected == 0)
            {
                client.spellMenu.ClearList(client.spellMenuHandle);
                int spellListPosition = 0;

                for (int spellIndex = 0; spellIndex < client.entityManager.SpellCount; spellIndex += 1)
                {
                    string spellColourPrefix = "@yel@";

                    for (int runeIndex = 0; runeIndex < client.entityManager.GetSpell(spellIndex).RuneCount; runeIndex += 1)
                    {
                        int runeItemId = client.entityManager.GetSpell(spellIndex).RequiredRunesIds[runeIndex];

                        if (client.HasRequiredRunes(runeItemId, client.entityManager.GetSpell(spellIndex).RequiredRunesCounts[runeIndex]))
                        {
                            continue;
                        }

                        spellColourPrefix = "@whi@";
                        break;
                    }

                    int currentMagicLevel = client.playerStatCurrent[6];

                    if (client.entityManager.GetSpell(spellIndex).RequiredLevel > currentMagicLevel)
                    {
                        spellColourPrefix = "@normalZ@";
                    }

                    client.spellMenu.AddListItem(client.spellMenuHandle, spellListPosition, spellColourPrefix + "Level " + client.entityManager.GetSpell(spellIndex).RequiredLevel + ": " + client.entityManager.GetSpell(spellIndex).Name);
                    spellListPosition += 1;
                }

                client.spellMenu.DrawMenu();
                int highlightedSpellIndex = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);

                if (highlightedSpellIndex != -1)
                {
                    client.gameGraphics.DrawString("Level " + client.entityManager.GetSpell(highlightedSpellIndex).RequiredLevel + ": " + client.entityManager.GetSpell(highlightedSpellIndex).Name, menuX + 2, menuY + 124, 1, 0xffff00);
                    client.gameGraphics.DrawString(client.entityManager.GetSpell(highlightedSpellIndex).Description, menuX + 2, menuY + 136, 0, 0xffffff);

                    for (int runeDisplayIndex = 0; runeDisplayIndex < client.entityManager.GetSpell(highlightedSpellIndex).RuneCount; runeDisplayIndex += 1)
                    {
                        int runeId = client.entityManager.GetSpell(highlightedSpellIndex).RequiredRunesIds[runeDisplayIndex];
                        recordItemSprite(menuX + 2 + runeDisplayIndex * 44, menuY + 150, 48, 32, client.entityManager.GetItem(runeId));
                        int runeInventoryCount = client.GetInventoryItemTotalCount(runeId);
                        int requiredRuneCount = client.entityManager.GetSpell(highlightedSpellIndex).RequiredRunesCounts[runeDisplayIndex];
                        string runeCountColour = "@red@";

                        if (client.HasRequiredRunes(runeId, requiredRuneCount))
                        {
                            runeCountColour = "@gre@";
                        }

                        client.gameGraphics.DrawString(runeCountColour + runeInventoryCount + "/" + requiredRuneCount, menuX + 2 + runeDisplayIndex * 44, menuY + 150, 1, 0xffffff);
                    }
                }
                else
                {
                    client.gameGraphics.DrawString("Point at a spell for a description", menuX + 2, menuY + 124, 1, 0);
                }
            }

            if (client.menuMagicPrayersSelected == 1)
            {
                client.spellMenu.ClearList(client.spellMenuHandle);
                int prayerListPosition = 0;

                for (int prayerIndex = 0; prayerIndex < client.entityManager.PrayerCount; prayerIndex += 1)
                {
                    Prayer prayer = client.entityManager.GetPrayer(prayerIndex);
                    string prayerColourPrefix = "@whi@";

                    if (prayer.RequiredLevel > client.playerStatBase[5])
                    {
                        prayerColourPrefix = "@normalZ@";
                    }

                    if (client.prayerOn[prayerIndex])
                    {
                        prayerColourPrefix = "@gre@";
                    }

                    client.spellMenu.AddListItem(client.spellMenuHandle, prayerListPosition, prayerColourPrefix + "Level " + prayer.RequiredLevel + ": " + prayer.Name);
                    prayerListPosition += 1;
                }

                client.spellMenu.DrawMenu();
                int highlightedPrayerIndex = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);

                if (highlightedPrayerIndex != -1)
                {
                    Prayer highlightedPrayer = client.entityManager.GetPrayer(highlightedPrayerIndex);
                    client.gameGraphics.DrawText("Level " + highlightedPrayer.RequiredLevel + ": " + highlightedPrayer.Name, menuX + menuWidth / 2, menuY + 130, 1, 0xffff00);
                    client.gameGraphics.DrawText(highlightedPrayer.Description, menuX + menuWidth / 2, menuY + 145, 0, 0xffffff);
                    client.gameGraphics.DrawText("Drain rate: " + highlightedPrayer.DrainRate, menuX + menuWidth / 2, menuY + 160, 1, 0);
                }
                else
                {
                    client.gameGraphics.DrawString("Point at a prayer for a description", menuX + 2, menuY + 124, 1, 0);
                }
            }

            if (!canClick)
            {
                return;
            }

            int clickOffsetX = client.mouseX - (client.gameGraphics.gameWidth - 199);
            int clickOffsetY = client.mouseY - 36;

            if (clickOffsetX >= 0 && clickOffsetY >= 0 && clickOffsetX < 196 && clickOffsetY < 182)
            {
                client.spellMenu.MouseClick(clickOffsetX + (client.gameGraphics.gameWidth - 199), clickOffsetY + 36, client.lastMouseButton, client.mouseButton);

                if (clickOffsetY <= 24 && client.mouseButtonClick == 1)
                {
                    if (clickOffsetX < 98 && client.menuMagicPrayersSelected == 1)
                    {
                        client.menuMagicPrayersSelected = 0;
                        client.spellMenu.SwitchList(client.spellMenuHandle);
                    }
                    else if (clickOffsetX > 98 && client.menuMagicPrayersSelected == 0)
                    {
                        client.menuMagicPrayersSelected = 1;
                        client.spellMenu.SwitchList(client.spellMenuHandle);
                    }
                }

                if (client.mouseButtonClick == 1 && client.menuMagicPrayersSelected == 0)
                {
                    int clickedSpellIndex = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);

                    if (clickedSpellIndex != -1)
                    {
                        int currentMagicStat = client.playerStatCurrent[6];

                        if (client.entityManager.GetSpell(clickedSpellIndex).RequiredLevel > currentMagicStat)
                        {
                            client.DisplayMessage("Your magic ability is not high enough for this spell", 3);
                        }
                        else
                        {
                            int runeCheckIndex;

                            for (runeCheckIndex = 0; runeCheckIndex < client.entityManager.GetSpell(clickedSpellIndex).RuneCount; runeCheckIndex += 1)
                            {
                                int checkRuneId = client.entityManager.GetSpell(clickedSpellIndex).RequiredRunesIds[runeCheckIndex];

                                if (client.HasRequiredRunes(checkRuneId, client.entityManager.GetSpell(clickedSpellIndex).RequiredRunesCounts[runeCheckIndex]))
                                {
                                    continue;
                                }

                                client.DisplayMessage("You don't have all the reagents you need for this spell", 3);
                                runeCheckIndex = -1;
                                break;
                            }

                            if (runeCheckIndex == client.entityManager.GetSpell(clickedSpellIndex).RuneCount)
                            {
                                client.selectedSpell = clickedSpellIndex;
                                client.selectedItem = -1;
                            }
                        }
                    }
                }

                if (client.mouseButtonClick == 1 && client.menuMagicPrayersSelected == 1)
                {
                    int clickedPrayerIndex = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);

                    if (clickedPrayerIndex != -1)
                    {
                        int currentPrayerBase = client.playerStatBase[5];

                        if (client.entityManager.GetPrayer(clickedPrayerIndex).RequiredLevel > currentPrayerBase)
                        {
                            client.DisplayMessage("Your prayer ability is not high enough for this prayer", 3);
                        }
                        else if (client.playerStatCurrent[5] == 0)
                        {
                            client.DisplayMessage("You have Run out of prayer points. Return to a church to recharge", 3);
                        }
                        else if (client.prayerOn[clickedPrayerIndex])
                        {
                            client.streamClass.CreatePacket(248);
                            client.streamClass.AddByte(clickedPrayerIndex);
                            client.streamClass.FormatPacket();
                            client.prayerOn[clickedPrayerIndex] = false;
                            client.PlaySound("prayeroff");
                        }
                        else
                        {
                            client.streamClass.CreatePacket(56);
                            client.streamClass.AddByte(clickedPrayerIndex);
                            client.streamClass.FormatPacket();
                            client.prayerOn[clickedPrayerIndex] = true;
                            client.PlaySound("prayeron");
                        }
                    }
                }

                client.mouseButtonClick = 0;
            }
        }


        public void DrawAppearanceWindow()
        {
            client.gameGraphics.interlace = false;
            client.gameGraphics.ClearScreen();
            client.appearanceMenu.DrawMenu();
            int previewX = 140 + 116;
            int previewY = 50 - 25;
            client.gameGraphics.DrawCharacterLegs(previewX - 32 - 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).Number, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(previewX - 32 - 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).Number, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(previewX - 32 - 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).Number, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawCharacterLegs(previewX - 32, previewY, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).Number + 6, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(previewX - 32, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).Number + 6, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(previewX - 32, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).Number + 6, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawCharacterLegs(previewX - 32 + 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).Number + 12, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(previewX - 32 + 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).Number + 12, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(previewX - 32 + 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).Number + 12, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawPicture(0, client.windowHeight, client.baseInventoryPic + 22);
            client.OnDrawDone();
        }


        public void DrawStatsQuestsMenu(bool canClick)
        {
            int menuX = client.gameGraphics.gameWidth - 199;
            int menuY = 36;
            client.gameGraphics.DrawPicture(menuX - 49, 3, client.baseInventoryPic + 3);
            int menuWidth = 196;
            int menuHeight = 275;
            int statsTabColour = GameImage.RgbToInt(160, 160, 160);
            int questsTabColour = GameImage.RgbToInt(160, 160, 160);

            if (client.questMenuSelected == 0)
            {
                statsTabColour = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                questsTabColour = GameImage.RgbToInt(220, 220, 220);
            }

            client.gameGraphics.DrawBoxAlpha(menuX, menuY, menuWidth / 2, 24, statsTabColour, 128);
            client.gameGraphics.DrawBoxAlpha(menuX + menuWidth / 2, menuY, menuWidth / 2, 24, questsTabColour, 128);
            client.gameGraphics.DrawBoxAlpha(menuX, menuY + 24, menuWidth, menuHeight - 24, GameImage.RgbToInt(220, 220, 220), 128);
            client.gameGraphics.DrawLineX(menuX, menuY + 24, menuWidth, 0);
            client.gameGraphics.DrawLineY(menuX + menuWidth / 2, menuY, 24, 0);
            client.gameGraphics.DrawText("Stats", menuX + menuWidth / 4, menuY + 16, 4, 0);
            client.gameGraphics.DrawText("Quests", menuX + menuWidth / 4 + menuWidth / 2, menuY + 16, 4, 0);

            if (client.questMenuSelected == 0)
            {
                int textY = 72;
                int hoveredSkillIndex = -1;
                client.gameGraphics.DrawString("Skills", menuX + 5, textY, 3, 0xffff00);
                textY += 13;

                for (int skillRowIndex = 0; skillRowIndex < 9; skillRowIndex += 1)
                {
                    int skillColour = 0xffffff;

                    if (client.mouseX > menuX + 3 && client.mouseY >= textY - 11 && client.mouseY < textY + 2 && client.mouseX < menuX + 90)
                    {
                        skillColour = 0xff0000;
                        hoveredSkillIndex = skillRowIndex;
                    }

                    client.gameGraphics.DrawString(client.skillName[skillRowIndex] + ":@yel@" + client.playerStatCurrent[skillRowIndex] + "/" + client.playerStatBase[skillRowIndex], menuX + 5, textY, 1, skillColour);
                    skillColour = 0xffffff;

                    if (client.mouseX >= menuX + 90 && client.mouseY >= textY - 13 - 11 && client.mouseY < textY - 13 + 2 && client.mouseX < menuX + 196)
                    {
                        skillColour = 0xff0000;
                        hoveredSkillIndex = skillRowIndex + 9;
                    }

                    client.gameGraphics.DrawString(client.skillName[skillRowIndex + 9] + ":@yel@" + client.playerStatCurrent[skillRowIndex + 9] + "/" + client.playerStatBase[skillRowIndex + 9], menuX + menuWidth / 2 - 5, textY - 13, 1, skillColour);
                    textY += 13;
                }

                client.gameGraphics.DrawString("Quest Points:@yel@" + client.questPoints, menuX + menuWidth / 2 - 5, textY - 13, 1, 0xffffff);
                textY += 12;
                client.gameGraphics.DrawString("Fatigue: @yel@" + client.fatigue * 100 / 750 + "%", menuX + 5, textY - 13, 1, 0xffffff);
                textY += 8;
                client.gameGraphics.DrawString("Equipment Status", menuX + 5, textY, 3, 0xffff00);
                textY += 12;

                for (int gearRowIndex = 0; gearRowIndex < 3; gearRowIndex += 1)
                {
                    client.gameGraphics.DrawString(client.gearStats[gearRowIndex] + ":@yel@" + client.equipmentStatus[gearRowIndex], menuX + 5, textY, 1, 0xffffff);

                    if (gearRowIndex < 2)
                    {
                        client.gameGraphics.DrawString(client.gearStats[gearRowIndex + 3] + ":@yel@" + client.equipmentStatus[gearRowIndex + 3], menuX + menuWidth / 2 + 25, textY, 1, 0xffffff);
                    }

                    textY += 13;
                }

                textY += 6;
                client.gameGraphics.DrawLineX(menuX, textY - 15, menuWidth, 0);

                if (hoveredSkillIndex != -1)
                {
                    client.gameGraphics.DrawString(client.skillNameVerb[hoveredSkillIndex] + " skill", menuX + 5, textY, 1, 0xffff00);
                    textY += 12;
                    int nextLevelXp = client.experienceList[0];

                    for (int expTableIndex = 0; expTableIndex < 98; expTableIndex += 1)
                    {
                        if (client.playerStatExp[hoveredSkillIndex] >= client.experienceList[expTableIndex])
                        {
                            nextLevelXp = client.experienceList[expTableIndex + 1];
                        }
                    }

                    client.gameGraphics.DrawString("Total xp: " + client.playerStatExp[hoveredSkillIndex], menuX + 5, textY, 1, 0xffffff);
                    textY += 12;
                    client.gameGraphics.DrawString("Next level at: " + nextLevelXp, menuX + 5, textY, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString("Overall levels", menuX + 5, textY, 1, 0xffff00);
                    textY += 12;
                    int totalSkillLevels = 0;

                    for (int skillSumIndex = 0; skillSumIndex < 18; skillSumIndex += 1)
                    {
                        totalSkillLevels += client.playerStatBase[skillSumIndex];
                    }

                    client.gameGraphics.DrawString("Skill total: " + totalSkillLevels, menuX + 5, textY, 1, 0xffffff);
                    textY += 12;
                    client.gameGraphics.DrawString("Combat level: " + client.ourPlayer.level, menuX + 5, textY, 1, 0xffffff);
                }
            }

            if (client.questMenuSelected == 1)
            {
                client.questMenu.ClearList(client.questMenuHandle);
                client.questMenu.AddListItem(client.questMenuHandle, 0, "@whi@Quest-list (green=completed)");

                for (int questIndex = 0; questIndex < client.usedQuestName.Length; questIndex += 1)
                {
                    string questColour;

                    if (client.questStage[questIndex] == 0)
                    {
                        questColour = "@red@";
                    }
                    else if (client.questStage[questIndex] == 1)
                    {
                        questColour = "@yel@";
                    }
                    else
                    {
                        questColour = "@gre@";
                    }

                    client.questMenu.AddListItem(client.questMenuHandle, questIndex + 1, questColour + client.usedQuestName[questIndex]);
                }

                client.questMenu.DrawMenu();
            }

            if (!canClick)
            {
                return;
            }

            int clickOffsetX = client.mouseX - (client.gameGraphics.gameWidth - 199);
            int clickOffsetY = client.mouseY - 36;

            if (clickOffsetX >= 0 && clickOffsetY >= 0 && clickOffsetX < menuWidth && clickOffsetY < menuHeight)
            {
                if (client.questMenuSelected == 1)
                {
                    client.questMenu.MouseClick(clickOffsetX + (client.gameGraphics.gameWidth - 199), clickOffsetY + 36, client.lastMouseButton, client.mouseButton);
                }

                if (clickOffsetY <= 24 && client.mouseButtonClick == 1)
                {
                    if (clickOffsetX < 98)
                    {
                        client.questMenuSelected = 0;
                        return;
                    }

                    if (clickOffsetX > 98)
                    {
                        client.questMenuSelected = 1;
                    }
                }
            }
        }

   
    }
}
