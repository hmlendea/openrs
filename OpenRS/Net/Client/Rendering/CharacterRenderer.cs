using System;

using OpenRS.Models;
using OpenRS.Net.Client.Game;
using OpenRS.Localisation;

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
            client.gameGraphics.DrawText(LocalisationManager.GetString("player.tab_magic"), menuX + menuWidth / 4, menuY + 16, 4, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("player.tab_prayers"), menuX + menuWidth / 4 + menuWidth / 2, menuY + 16, 4, 0);

            if (client.menuMagicPrayersSelected == 0)
            {
                client.spellMenu.ClearList(client.spellMenuHandle);
                int spellListPosition = 0;

                for (int spellIndex = 0; spellIndex < client.entityManager.SpellCount; spellIndex += 1)
                {
                    string spellColourPrefix = "@yel@";

                    foreach ((int runeItemId, int requiredRuneCount) in
                        client.entityManager.GetSpell(spellIndex).RequiredRunes)
                    {
                        if (client.HasRequiredRunes(runeItemId, requiredRuneCount))
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

                    client.spellMenu.AddListItem(client.spellMenuHandle, spellListPosition, spellColourPrefix + LocalisationManager.GetString("player.spell_level_prefix") + client.entityManager.GetSpell(spellIndex).RequiredLevel + ": " + client.entityManager.GetSpell(spellIndex).Name);
                    spellListPosition += 1;
                }

                client.spellMenu.DrawMenu();
                int highlightedSpellIndex = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);

                if (highlightedSpellIndex != -1)
                {
                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.spell_level_prefix") + client.entityManager.GetSpell(highlightedSpellIndex).RequiredLevel + ": " + client.entityManager.GetSpell(highlightedSpellIndex).Name, menuX + 2, menuY + 124, 1, 0xffff00);
                    client.gameGraphics.DrawString(client.entityManager.GetSpell(highlightedSpellIndex).Description, menuX + 2, menuY + 136, 0, 0xffffff);

                    int runeDisplayIndex = 0;

                    foreach ((int runeId, int requiredRuneCount) in
                        client.entityManager.GetSpell(highlightedSpellIndex).RequiredRunes)
                    {
                        recordItemSprite(
                            menuX + 2 + runeDisplayIndex * 44,
                            menuY + 150,
                            48,
                            32,
                            client.entityManager.GetItem(runeId));

                        int runeInventoryCount = client.GetInventoryItemTotalCount(runeId);
                        string runeCountColour = "@red@";

                        if (client.HasRequiredRunes(runeId, requiredRuneCount))
                        {
                            runeCountColour = "@gre@";
                        }

                        client.gameGraphics.DrawString(
                            runeCountColour + runeInventoryCount + "/" + requiredRuneCount,
                            menuX + 2 + runeDisplayIndex * 44,
                            menuY + 150,
                            1,
                            0xffffff);

                        runeDisplayIndex += 1;
                    }
                }
                else
                {
                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.spell_no_description"), menuX + 2, menuY + 124, 1, 0);
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

                    client.spellMenu.AddListItem(client.spellMenuHandle, prayerListPosition, prayerColourPrefix + LocalisationManager.GetString("player.spell_level_prefix") + prayer.RequiredLevel + ": " + prayer.Name);
                    prayerListPosition += 1;
                }

                client.spellMenu.DrawMenu();
                int highlightedPrayerIndex = client.spellMenu.GetEntryHighlighted(client.spellMenuHandle);

                if (highlightedPrayerIndex != -1)
                {
                    Prayer highlightedPrayer = client.entityManager.GetPrayer(highlightedPrayerIndex);
                    client.gameGraphics.DrawText(LocalisationManager.GetString("player.spell_level_prefix") + highlightedPrayer.RequiredLevel + ": " + highlightedPrayer.Name, menuX + menuWidth / 2, menuY + 130, 1, 0xffff00);
                    client.gameGraphics.DrawText(highlightedPrayer.Description, menuX + menuWidth / 2, menuY + 145, 0, 0xffffff);
                    client.gameGraphics.DrawText(LocalisationManager.GetString("player.prayer_drain_rate_prefix") + highlightedPrayer.DrainRate, menuX + menuWidth / 2, menuY + 160, 1, 0);
                }
                else
                {
                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.prayer_no_description"), menuX + 2, menuY + 124, 1, 0);
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
                            client.DisplayMessage(LocalisationManager.GetString("player.spell_level_too_low"), 3);
                        }
                        else
                        {
                            bool hasAllRunes = true;

                            foreach ((int runeId, int requiredCount) in
                                client.entityManager.GetSpell(clickedSpellIndex).RequiredRunes)
                            {
                                if (client.HasRequiredRunes(runeId, requiredCount))
                                {
                                    continue;
                                }

                                client.DisplayMessage(LocalisationManager.GetString(
                                    "player.spell_missing_reagents"), 3);

                                hasAllRunes = false;
                                break;
                            }

                            if (hasAllRunes)
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
                            client.DisplayMessage(LocalisationManager.GetString("player.prayer_level_too_low"), 3);
                        }
                        else if (client.playerStatCurrent[5] == 0)
                        {
                            client.DisplayMessage(LocalisationManager.GetString("player.prayer_no_points"), 3);
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
            client.gameGraphics.DrawCharacterLegs(previewX - 32 - 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).SpriteIndex, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(previewX - 32 - 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).SpriteIndex, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(previewX - 32 - 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).SpriteIndex, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawCharacterLegs(previewX - 32, previewY, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).SpriteIndex + 6, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(previewX - 32, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).SpriteIndex + 6, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(previewX - 32, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).SpriteIndex + 6, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawCharacterLegs(previewX - 32 + 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearance2Colour).SpriteIndex + 12, client.appearanceTopBottomColours[client.appearanceBottomColour]);
            client.gameGraphics.DrawImage(previewX - 32 + 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceBodyGender).SpriteIndex + 12, client.appearanceTopBottomColours[client.appearanceTopColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
            client.gameGraphics.DrawImage(previewX - 32 + 55, previewY, 64, 102, client.entityManager.GetAnimation(client.appearanceHeadType).SpriteIndex + 12, client.appearanceHairColours[client.appearanceHairColour], client.appearanceSkinColours[client.appearanceSkinColour], 0, false);
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
            client.gameGraphics.DrawText(LocalisationManager.GetString("player.tab_stats"), menuX + menuWidth / 4, menuY + 16, 4, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("player.tab_quests"), menuX + menuWidth / 4 + menuWidth / 2, menuY + 16, 4, 0);

            if (client.questMenuSelected == 0)
            {
                int textY = 72;
                int hoveredSkillIndex = -1;
                client.gameGraphics.DrawString(LocalisationManager.GetString("player.skills_heading"), menuX + 5, textY, 3, 0xffff00);
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

                client.gameGraphics.DrawString(LocalisationManager.GetString("player.quest_points_prefix") + client.questPoints, menuX + menuWidth / 2 - 5, textY - 13, 1, 0xffffff);
                textY += 12;
                client.gameGraphics.DrawString(LocalisationManager.GetString("player.fatigue_prefix") + client.fatigue * 100 / 750 + "%", menuX + 5, textY - 13, 1, 0xffffff);
                textY += 8;
                client.gameGraphics.DrawString(LocalisationManager.GetString("player.equipment_status"), menuX + 5, textY, 3, 0xffff00);
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
                    client.gameGraphics.DrawString(client.skillNameVerb[hoveredSkillIndex] + LocalisationManager.GetString("player.skill_hover_suffix"), menuX + 5, textY, 1, 0xffff00);
                    textY += 12;
                    int nextLevelXp = client.experienceList[0];

                    for (int expTableIndex = 0; expTableIndex < 98; expTableIndex += 1)
                    {
                        if (client.playerStatExp[hoveredSkillIndex] >= client.experienceList[expTableIndex])
                        {
                            nextLevelXp = client.experienceList[expTableIndex + 1];
                        }
                    }

                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.total_xp_prefix") + client.playerStatExp[hoveredSkillIndex], menuX + 5, textY, 1, 0xffffff);
                    textY += 12;
                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.next_level_prefix") + nextLevelXp, menuX + 5, textY, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.overall_levels"), menuX + 5, textY, 1, 0xffff00);
                    textY += 12;
                    int totalSkillLevels = 0;

                    for (int skillSumIndex = 0; skillSumIndex < 18; skillSumIndex += 1)
                    {
                        totalSkillLevels += client.playerStatBase[skillSumIndex];
                    }

                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.skill_total_prefix") + totalSkillLevels, menuX + 5, textY, 1, 0xffffff);
                    textY += 12;
                    client.gameGraphics.DrawString(LocalisationManager.GetString("player.combat_level_prefix") + client.ourPlayer.level, menuX + 5, textY, 1, 0xffffff);
                }
            }

            if (client.questMenuSelected == 1)
            {
                client.questMenu.ClearList(client.questMenuHandle);
                client.questMenu.AddListItem(client.questMenuHandle, 0, LocalisationManager.GetString("player.quest_list_header"));

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
