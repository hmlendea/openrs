using OpenRS.Localisation;

namespace OpenRS.Net.Client.Rendering
{
    public sealed class LoginRenderer(GameClient client)
    {

        public void DrawWelcomeBox()
        {
            int boxHeight = 65;

            if (client.lastLoginAddress != "0.0.0.0")
            {
                boxHeight += 30;
            }

            if (client.subDaysLeft > 0)
            {
                boxHeight += 15;
            }

            if (client.lastLoginDays >= 0)
            {
                boxHeight += 15;
            }

            int yPosition = 167 - boxHeight / 2;
            client.gameGraphics.DrawBox(56, yPosition, 400, boxHeight, 0);
            client.gameGraphics.DrawBoxEdge(56, yPosition, 400, boxHeight, 0xffffff);
            yPosition += 20;
            client.gameGraphics.DrawText(LocalisationManager.GetString("login.welcome_title") + client.loginUsername, 256, yPosition, 4, 0xffff00);
            yPosition += 30;
            string lastLoginDescription = client.lastLoginDays + " days ago";

            if (client.lastLoginDays == 0)
            {
                lastLoginDescription = LocalisationManager.GetString("login.last_login_today");
            }
            else if (client.lastLoginDays == 1)
            {
                lastLoginDescription = LocalisationManager.GetString("login.last_login_yesterday");
            }

            if (client.lastLoginAddress != "0.0.0.0")
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("login.last_login_prefix") + lastLoginDescription, 256, yPosition, 1, 0xffffff);
                yPosition += 15;
                client.gameGraphics.DrawText(LocalisationManager.GetString("login.last_login_from_prefix") + client.lastLoginAddress, 256, yPosition, 1, 0xffffff);
                yPosition += 15;
            }

            if (client.subDaysLeft > 0)
            {
                client.gameGraphics.DrawText(LocalisationManager.GetString("login.subscription_prefix") + client.subDaysLeft + " days", 256, yPosition, 1, 0xffffff);
                yPosition += 15;
            }

            int closeButtonColour = 0xffffff;

            if (client.mouseY > yPosition - 12 && client.mouseY <= yPosition && client.mouseX > 106 && client.mouseX < 406)
            {
                closeButtonColour = 0xff0000;
            }

            client.gameGraphics.DrawText(LocalisationManager.GetString("combat.close_window"), 256, yPosition, 1, closeButtonColour);

            if (client.mouseButtonClick == 1)
            {
                if (closeButtonColour == 0xff0000)
                {
                    client.showWelcomeBox = false;
                }

                if ((client.mouseX < 86 || client.mouseX > 426) &&
                    (client.mouseY < 167 - boxHeight / 2 || client.mouseY > 167 + boxHeight / 2))
                {
                    client.showWelcomeBox = false;
                }
            }

            client.mouseButtonClick = 0;
        }


        public void LoginScreenPrint(string statusLine1, string statusLine2)
        {
            if (client.loginScreen == 2 && client.loginMenuLogin is not null)
            {
                client.loginMenuLogin.UpdateText(client.loginMenuStatusText, statusLine1 + " " + statusLine2);
            }

            DrawLoginScreens();
            client.ResetTimings();
        }


        public void DrawLoginScreens()
        {
            client.loginScreenShown = false;

            if (client.gameGraphics is null)
            {
                return;
            }

            client.gameGraphics.interlace = false;
            client.gameGraphics.ClearScreen();

            if (client.loginScreen == 0 ||
                client.loginScreen == 1 ||
                client.loginScreen == 2 ||
                client.loginScreen == 3)
            {
                int animationFrame = client.tick * 2 % 3072;

                if (animationFrame < 1024)
                {
                    client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic);

                    if (animationFrame > 768)
                    {
                        client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic + 1, animationFrame - 768);
                    }
                }
                else if (animationFrame < 2048)
                {
                    client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic + 1);

                    if (animationFrame > 1792)
                    {
                        client.gameGraphics.DrawPicture(0, 10, client.baseInventoryPic + 10, animationFrame - 1792);
                    }
                }
                else
                {
                    client.gameGraphics.DrawPicture(0, 10, client.baseInventoryPic + 10);

                    if (animationFrame > 2816)
                    {
                        client.gameGraphics.DrawPicture(0, 10, client.baseLoginScreenBackgroundPic, animationFrame - 2816);
                    }
                }
            }

            if (client.loginMenuFirst is null)
            {
                return;
            }

            if (client.loginScreen == 0)
            {
                client.loginMenuFirst.DrawMenu();
            }

            if (client.loginScreen == 1)
            {
                client.loginNewUser.DrawMenu();
            }

            if (client.loginScreen == 2)
            {
                client.loginMenuLogin.DrawMenu();
            }

            client.gameGraphics.DrawPicture(0, client.windowHeight, client.baseInventoryPic + 22);
            client.OnDrawDone();
        }


    }
}
