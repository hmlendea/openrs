using System;

using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class OverlayRenderer(GameClient client)
    {

        private static readonly string[] AbuseRuleDescriptions =
        [
            "1: Offensive language",
            "2: Item scamming",
            "3: Password scamming",
            "4: Bug abuse",
            "5: Jagex Staff impersonation",
            "6: Account sharing/trading",
            "7: Macroing",
            "8: Mutiple logging in",
            "9: Encouraging others to break rules",
            "10: Misuse of customer support",
            "11: Advertising / website",
            "12: Real world item trading",
        ];

        public void DrawReportAbuseBox1()
        {
            client.reportAbuseOptionSelected = 0;
            int yOffset = 135;

            for (int ruleIndex = 0; ruleIndex < 12; ruleIndex += 1)
            {
                if (client.mouseX > 66 &&
                    client.mouseX < 446 &&
                    client.mouseY >= yOffset - 12 &&
                    client.mouseY < yOffset + 3)
                {
                    client.reportAbuseOptionSelected = ruleIndex + 1;
                }

                yOffset += 14;
            }

            if (client.mouseButtonClick != 0 && client.reportAbuseOptionSelected != 0)
            {
                client.mouseButtonClick = 0;
                client.showAbuseBox = 2;
                client.inputText = "";
                client.enteredInputText = "";

                return;
            }

            yOffset += 15;

            if (client.mouseButtonClick != 0)
            {
                client.mouseButtonClick = 0;

                if (client.mouseX < 56 || client.mouseY < 35 || client.mouseX > 456 || client.mouseY > 325)
                {
                    client.showAbuseBox = 0;

                    return;
                }

                if (client.mouseX > 66 &&
                    client.mouseX < 446 &&
                    client.mouseY >= yOffset - 15 &&
                    client.mouseY < yOffset + 5)
                {
                    client.showAbuseBox = 0;

                    return;
                }
            }

            client.gameGraphics.DrawBox(56, 35, 400, 290, 0);
            client.gameGraphics.DrawBoxEdge(56, 35, 400, 290, 0xffffff);
            yOffset = 50;
            client.gameGraphics.DrawText("This form is for reporting players who are breaking our rules", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            client.gameGraphics.DrawText("Using it sends a snapshot of the last 60 secs of activity to us", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            client.gameGraphics.DrawText("If you misuse this form, you will be banned.", 256, yOffset, 1, 0xff8000);
            yOffset += 25;
            client.gameGraphics.DrawText("First indicate which of our 12 rules is being broken. For a detailed", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            client.gameGraphics.DrawText("explanation of each rule please read the manual on our website.", 256, yOffset, 1, 0xffff00);
            yOffset += 15;

            for (int ruleIndex = 0; ruleIndex < AbuseRuleDescriptions.Length; ruleIndex += 1)
            {
                int selectedColour = 0xffffff;

                if (client.reportAbuseOptionSelected == ruleIndex + 1)
                {
                    client.gameGraphics.DrawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                    selectedColour = 0xff8000;
                }

                client.gameGraphics.DrawText(AbuseRuleDescriptions[ruleIndex], 256, yOffset, 1, selectedColour);
                yOffset += 14;
            }

            yOffset += 15;
            int cancelColour = 0xffffff;

            if (client.mouseX > 196 && client.mouseX < 316 && client.mouseY > yOffset - 15 && client.mouseY < yOffset + 5)
            {
                cancelColour = 0xffff00;
            }

            client.gameGraphics.DrawText("Click here to cancel", 256, yOffset, 1, cancelColour);
        }


        public void DrawOptionsMenu(bool canClick)
        {
            int menuX = client.gameGraphics.gameWidth - 199;
            int menuWidth = 196;
            client.gameGraphics.DrawPicture(menuX - 49, 3, client.baseInventoryPic + 6);
            client.gameGraphics.DrawBoxAlpha(menuX, 36, menuWidth, 62, GameImage.RgbToInt(181, 181, 181), 160);
            client.gameGraphics.DrawBoxAlpha(menuX, 98, menuWidth, 92, GameImage.RgbToInt(201, 201, 201), 160);
            client.gameGraphics.DrawBoxAlpha(menuX, 190, menuWidth, 90, GameImage.RgbToInt(181, 181, 181), 160);
            client.gameGraphics.DrawBoxAlpha(menuX, 280, menuWidth, 40, GameImage.RgbToInt(201, 201, 201), 160);
            int labelX = menuX + 3;
            int labelY = 36 + 15;
            client.gameGraphics.DrawString("Game options - click to toggle", labelX, labelY, 1, 0);
            labelY += 15;

            if (client.configCameraAutoAngle)
            {
                client.gameGraphics.DrawString("Camera angle mode - @gre@Auto", labelX, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Camera angle mode - @red@Manual", labelX, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.configOneMouseButton)
            {
                client.gameGraphics.DrawString("Mouse buttons - @red@One", labelX, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Mouse buttons - @gre@Two", labelX, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (Config.MembersFeatures)
            {
                if (client.configSoundOff)
                {
                    client.gameGraphics.DrawString("Sound effects - @red@off", labelX, labelY, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString("Sound effects - @gre@on", labelX, labelY, 1, 0xffffff);
                }
            }

            labelY += 15;
            client.gameGraphics.DrawString("Client assists - click to toggle", labelX, labelY, 1, 0);
            labelY += 15;

            if (client.showRoofs)
            {
                client.gameGraphics.DrawString("Roofs - @gre@show", labelX, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Roofs - @red@hide", labelX, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.showCombatWindow)
            {
                client.gameGraphics.DrawString("Fight mode window - @gre@show", labelX, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Fight mode window - @red@hide", labelX, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.fogOfWar)
            {
                client.gameGraphics.DrawString("Fog of war - @gre@show", labelX, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Fog of war - @red@hide", labelX, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.autoScreenshot)
            {
                client.gameGraphics.DrawString("Automatic screenshots - @gre@on", labelX, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Automatic screenshots - @red@off", labelX, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.useChatFilter)
            {
                client.gameGraphics.DrawString("Chat filter: @gre@<on>", menuX + 3, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Chat filter: @red@<off>", menuX + 3, labelY, 1, 0xffffff);
            }

            labelY += 15;
            client.gameGraphics.DrawString("Privacy settings. Will be applied to", labelX, labelY, 1, 0);
            labelY += 15;
            client.gameGraphics.DrawString("all people not on your friends list", labelX, labelY, 1, 0);
            labelY += 15;

            if (client.blockChat == 0)
            {
                client.gameGraphics.DrawString("Block chat messages: @red@<off>", menuX + 3, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Block chat messages: @gre@<on>", menuX + 3, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.blockPrivate == 0)
            {
                client.gameGraphics.DrawString("Block public messages: @red@<off>", menuX + 3, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Block public messages: @gre@<on>", menuX + 3, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (client.blockTrade == 0)
            {
                client.gameGraphics.DrawString("Block trade requests: @red@<off>", menuX + 3, labelY, 1, 0xffffff);
            }
            else
            {
                client.gameGraphics.DrawString("Block trade requests: @gre@<on>", menuX + 3, labelY, 1, 0xffffff);
            }

            labelY += 15;

            if (Config.MembersFeatures)
            {
                if (client.blockDuel == 0)
                {
                    client.gameGraphics.DrawString("Block duel requests: @red@<off>", menuX + 3, labelY, 1, 0xffffff);
                }
                else
                {
                    client.gameGraphics.DrawString("Block duel requests: @gre@<on>", menuX + 3, labelY, 1, 0xffffff);
                }
            }

            labelY += 20;
            client.gameGraphics.DrawString("Always logout when you finish", labelX, labelY, 1, 0);
            labelY += 15;
            int logoutColour = 0xffffff;

            if (client.mouseX > labelX && client.mouseX < labelX + menuWidth && client.mouseY > labelY - 12 && client.mouseY < labelY + 4)
            {
                logoutColour = 0xffff00;
            }

            client.gameGraphics.DrawString("Click here to logout", menuX + 3, labelY, 1, logoutColour);

            if (!canClick)
            {
                return;
            }

            int relativeMouseX = client.mouseX - (client.gameGraphics.gameWidth - 199);
            int relativeMouseY = client.mouseY - 36;

            if (relativeMouseX >= 0 && relativeMouseY >= 0 && relativeMouseX < 196 && relativeMouseY < 280)
            {
                int clickMenuX = client.gameGraphics.gameWidth - 199;
                int clickLabelX = clickMenuX + 3;
                int clickLabelY = 36 + 30;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.configCameraAutoAngle = !client.configCameraAutoAngle;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(0);
                    int configCameraAutoAngleByte = 0;

                    if (client.configCameraAutoAngle)
                    {
                        configCameraAutoAngleByte = 1;
                    }

                    client.streamClass.AddByte(configCameraAutoAngleByte);
                    client.streamClass.FormatPacket();
                }

                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.configOneMouseButton = !client.configOneMouseButton;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(2);
                    int configOneMouseButtonByte = 0;

                    if (client.configOneMouseButton)
                    {
                        configOneMouseButtonByte = 1;
                    }

                    client.streamClass.AddByte(configOneMouseButtonByte);
                    client.streamClass.FormatPacket();
                }

                clickLabelY += 15;

                if (Config.MembersFeatures &&
                    client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.configSoundOff = !client.configSoundOff;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(3);
                    int configSoundOffByte = 0;

                    if (client.configSoundOff)
                    {
                        configSoundOffByte = 1;
                    }

                    client.streamClass.AddByte(configSoundOffByte);
                    client.streamClass.FormatPacket();
                }

                clickLabelY += 30;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.showRoofs = !client.showRoofs;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(4);
                    int showRoofsByte = 0;

                    if (client.showRoofs)
                    {
                        showRoofsByte = 1;
                    }

                    client.streamClass.AddByte(showRoofsByte);
                    client.streamClass.FormatPacket();
                }

                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.showCombatWindow = !client.showCombatWindow;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(6);
                    int showCombatWindowByte = 0;

                    if (client.showCombatWindow)
                    {
                        showCombatWindowByte = 1;
                    }

                    client.streamClass.AddByte(showCombatWindowByte);
                    client.streamClass.FormatPacket();
                }

                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.fogOfWar = !client.fogOfWar;
                }

                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.autoScreenshot = !client.autoScreenshot;
                    client.streamClass.CreatePacket(157);
                    client.streamClass.AddByte(5);
                    int autoScreenshotByte = 0;

                    if (client.autoScreenshot)
                    {
                        autoScreenshotByte = 1;
                    }

                    client.streamClass.AddByte(autoScreenshotByte);
                    client.streamClass.FormatPacket();
                }

                bool isPrivacyChanged = false;
                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.useChatFilter = !client.useChatFilter;
                }

                clickLabelY += 45;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.blockChat = 1 - client.blockChat;
                    isPrivacyChanged = true;
                }

                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.blockPrivate = 1 - client.blockPrivate;
                    isPrivacyChanged = true;
                }

                clickLabelY += 15;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.blockTrade = 1 - client.blockTrade;
                    isPrivacyChanged = true;
                }

                clickLabelY += 15;

                if (Config.MembersFeatures &&
                    client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.blockDuel = 1 - client.blockDuel;
                    isPrivacyChanged = true;
                }

                if (isPrivacyChanged)
                {
                    client.CallSendUpdatedPrivacyInfo(client.blockChat, client.blockPrivate, client.blockTrade, client.blockDuel);
                }

                clickLabelY += 35;

                if (client.mouseX > clickLabelX &&
                    client.mouseX < clickLabelX + menuWidth &&
                    client.mouseY > clickLabelY - 12 &&
                    client.mouseY < clickLabelY + 4 &&
                    client.mouseButtonClick == 1)
                {
                    client.SendLogout();
                }

                client.mouseButtonClick = 0;
            }
        }


        public void DrawLogoutBox()
        {
            client.gameGraphics.DrawBox(126, 137, 260, 60, 0);
            client.gameGraphics.DrawBoxEdge(126, 137, 260, 60, 0xffffff);
            client.gameGraphics.DrawText("Logging out...", 256, 173, 5, 0xffffff);
        }


        public void DrawQuestionMenu()
        {
            if (client.mouseButtonClick != 0)
            {
                for (int answerIndex = 0; answerIndex < client.questionMenuCount; answerIndex += 1)
                {
                    if (client.mouseX >= client.gameGraphics.TextWidth(client.questionMenuAnswer[answerIndex], 1) ||
                        client.mouseY <= answerIndex * 12 ||
                        client.mouseY >= 12 + answerIndex * 12)
                    {
                        continue;
                    }

                    client.streamClass.CreatePacket(154);
                    client.streamClass.AddByte(answerIndex);
                    client.streamClass.FormatPacket();
                    break;
                }

                client.mouseButtonClick = 0;
                client.showQuestionMenu = false;

                return;
            }

            for (int answerDisplayIndex = 0; answerDisplayIndex < client.questionMenuCount; answerDisplayIndex += 1)
            {
                int answerColour = 65535;

                if (client.mouseX < client.gameGraphics.TextWidth(client.questionMenuAnswer[answerDisplayIndex], 1) &&
                    client.mouseY > answerDisplayIndex * 12 &&
                    client.mouseY < 12 + answerDisplayIndex * 12)
                {
                    answerColour = 0xff0000;
                }

                client.gameGraphics.DrawString(client.questionMenuAnswer[answerDisplayIndex], 6, 12 + answerDisplayIndex * 12, 1, answerColour);
            }
        }


        public void DrawReportAbuseBox2()
        {
            if (client.enteredInputText.Length > 0)
            {
                string trimmedName = client.enteredInputText.Trim();
                client.inputText = "";
                client.enteredInputText = "";

                if (trimmedName.Length > 0)
                {
                    long nameHash = DataOperations.NameToHash(trimmedName);
                    client.streamClass.CreatePacket(7);
                    client.streamClass.AddLong(nameHash);
                    client.streamClass.AddByte(client.reportAbuseOptionSelected);
                    client.streamClass.FormatPacket();
                }

                client.showAbuseBox = 0;

                return;
            }

            client.gameGraphics.DrawBox(56, 130, 400, 100, 0);
            client.gameGraphics.DrawBoxEdge(56, 130, 400, 100, 0xffffff);
            int textY = 160;
            client.gameGraphics.DrawText("Now type the name of the offending player, and press enter", 256, textY, 1, 0xffff00);
            textY += 18;
            client.gameGraphics.DrawText("Name: " + client.inputText + "*", 256, textY, 4, 0xffffff);
            textY = 222;
            int cancelLabelColour = 0xffffff;

            if (client.mouseX > 196 && client.mouseX < 316 && client.mouseY > textY - 13 && client.mouseY < textY + 2)
            {
                cancelLabelColour = 0xffff00;

                if (client.mouseButtonClick == 1)
                {
                    client.mouseButtonClick = 0;
                    client.showAbuseBox = 0;
                }
            }

            client.gameGraphics.DrawText("Click here to cancel", 256, textY, 1, cancelLabelColour);

            if (client.mouseButtonClick == 1 &&
                (client.mouseX < 56 || client.mouseX > 456 || client.mouseY < 130 || client.mouseY > 230))
            {
                client.mouseButtonClick = 0;
                client.showAbuseBox = 0;
            }
        }


        public void DrawRightClickMenu()
        {
            if (client.mouseButtonClick != 0)
            {
                for (int optionIndex = 0; optionIndex < client.menuOptionsCount; optionIndex += 1)
                {
                    int labelX = client.menuX + 2;
                    int labelY = client.menuY + 27 + optionIndex * 15;

                    if (client.mouseX <= labelX - 2 || client.mouseY <= labelY - 12 || client.mouseY >= labelY + 4 || client.mouseX >= labelX - 3 + client.menuWidth)
                    {
                        continue;
                    }

                    client.MenuClick(client.menuIndexes[optionIndex]);
                    break;
                }

                client.mouseButtonClick = 0;
                client.menuShow = false;
                return;
            }

            if (client.mouseX < client.menuX - 10 || client.mouseY < client.menuY - 10 || client.mouseX > client.menuX + client.menuWidth + 10 || client.mouseY > client.menuY + client.menuHeight + 10)
            {
                client.menuShow = false;
                return;
            }

            client.gameGraphics.DrawBoxAlpha(client.menuX, client.menuY, client.menuWidth, client.menuHeight, 0xd0d0d0, 160);
            client.gameGraphics.DrawString("Choose option", client.menuX + 2, client.menuY + 12, 1, 65535);

            for (int optionIndex = 0; optionIndex < client.menuOptionsCount; optionIndex += 1)
            {
                int labelX = client.menuX + 2;
                int labelY = client.menuY + 27 + optionIndex * 15;
                int labelColour = 0xffffff;

                if (client.mouseX > labelX - 2 && client.mouseY > labelY - 12 && client.mouseY < labelY + 4 && client.mouseX < labelX - 3 + client.menuWidth)
                {
                    labelColour = 0xffff00;
                }

                client.gameGraphics.DrawString(client.menuText1[client.menuIndexes[optionIndex]] + " " + client.menuText2[client.menuIndexes[optionIndex]], labelX, labelY, 1, labelColour);
            }
        }

   
    }
}
