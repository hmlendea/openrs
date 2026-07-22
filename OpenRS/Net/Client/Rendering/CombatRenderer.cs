using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class CombatRenderer(GameClient client)
    {
        public void DrawWildernessAlertBox()
        {
            int textY = 97;
            client.GameGraphics.DrawBox(86, 77, 340, 180, 0);
            client.GameGraphics.DrawBoxEdge(86, 77, 340, 180, 0xffffff);
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_title"), 256, textY, 4, 0xff0000);
            textY += 26;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line1"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line2"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line3"), 256, textY, 1, 0xffffff);
            textY += 22;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line4"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line5"), 256, textY, 1, 0xffffff);
            textY += 22;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line6"), 256, textY, 1, 0xffffff);
            textY += 13;
            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.wilderness_warning_line7"), 256, textY, 1, 0xffffff);
            textY += 22;
            int closeLinkColour = 0xffffff;

            if (client.mouseY > textY - 12 && client.mouseY <= textY && client.mouseX > 181 && client.mouseX < 331)
            {
                closeLinkColour = 0xff0000;
            }

            client.GameGraphics.DrawText(LocalisationManager.GetString("combat.close_window"), 256, textY, 1, closeLinkColour);

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
