using OpenRS.Net.Client.Game;
using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class CombatRenderer(GameClient client)
    {
        public void DrawCombatStyleBox()
        {
            int boxOffsetX = 7;
            int boxOffsetY = 15;
            int boxWidth = 175;

            if (client.mouseButtonClick != 0)
            {
                for (int rowIndex = 0; rowIndex < 5; rowIndex += 1)
                {
                    if (rowIndex <= 0 ||
                        client.mouseX <= boxOffsetX ||
                        client.mouseX >= boxOffsetX + boxWidth ||
                        client.mouseY <= boxOffsetY + rowIndex * 20 ||
                        client.mouseY >= boxOffsetY + rowIndex * 20 + 20)
                    {
                        continue;
                    }

                    client.combatStyle = rowIndex - 1;
                    client.mouseButtonClick = 0;
                    client.streamClass.CreatePacket(42);
                    client.streamClass.AddByte(client.combatStyle);
                    client.streamClass.FormatPacket();
                    break;
                }
            }

            for (int rowIndex = 0; rowIndex < 5; rowIndex += 1)
            {
                int rowColour = GameImage.RgbToInt(190, 190, 190);

                if (rowIndex == client.combatStyle + 1)
                {
                    rowColour = GameImage.RgbToInt(255, 0, 0);
                }

                client.gameGraphics.DrawBoxAlpha(boxOffsetX, boxOffsetY + rowIndex * 20, boxWidth, 20, rowColour, 128);
                client.gameGraphics.DrawLineX(boxOffsetX, boxOffsetY + rowIndex * 20, boxWidth, 0);
                client.gameGraphics.DrawLineX(boxOffsetX, boxOffsetY + rowIndex * 20 + 20, boxWidth, 0);
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.select_style"), boxOffsetX + boxWidth / 2, boxOffsetY + 16, 3, 0xffffff);
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.style_controlled"), boxOffsetX + boxWidth / 2, boxOffsetY + 36, 3, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.style_aggressive"), boxOffsetX + boxWidth / 2, boxOffsetY + 56, 3, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.style_accurate"), boxOffsetX + boxWidth / 2, boxOffsetY + 76, 3, 0);
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.style_defensive"), boxOffsetX + boxWidth / 2, boxOffsetY + 96, 3, 0);
        }

        public void DrawWildernessAlertBox()
        {
            int textY = 97;
            client.gameGraphics.DrawBox(86, 77, 340, 180, 0);
            client.gameGraphics.DrawBoxEdge(86, 77, 340, 180, 0xffffff);
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_title"), 256, textY, 4, 0xff0000);
            textY += 26;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line1"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line2"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line3"), 256, textY, 1, 0xffffff);
            textY += 22;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line4"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line5"), 256, textY, 1, 0xffffff);
            textY += 22;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line6"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line7"), 256, textY, 1, 0xffffff);
            textY += 22;
            int closeLinkColour = 0xffffff;

            if (client.mouseY > textY - 12 && client.mouseY <= textY && client.mouseX > 181 && client.mouseX < 331)
            {
                closeLinkColour = 0xff0000;
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.close_window"), 256, textY, 1, closeLinkColour);

            if (client.mouseButtonClick != 0)
            {
                if (client.mouseY > textY - 12 && client.mouseY <= textY && client.mouseX > 181 && client.mouseX < 331)
                {
                    client.wildType = 2;
                }

                if (client.mouseX < 86 || client.mouseX > 426 || client.mouseY < 77 || client.mouseY > 257)
                {
                    client.wildType = 2;
                }

                client.mouseButtonClick = 0;
            }
        }
    }
}
