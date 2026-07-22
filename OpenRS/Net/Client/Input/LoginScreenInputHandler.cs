namespace OpenRS.Net.Client.Input
{
    internal sealed class LoginScreenInputHandler(GameClient client)
    {
        private static int ExistingUserScreen => 2;

        private static int IntroLoginScreen => 0;

        private static string LoginPromptText =>
            "Please enter your username and password";

        private static int NewUserScreen => 1;

        internal void CheckLoginScreenInputs()
        {
            UpdateSocketTimeout();

            if (client.loginScreen == IntroLoginScreen)
            {
                HandleIntroLoginScreen();
                return;
            }

            if (client.loginScreen == NewUserScreen)
            {
                HandleNewUserScreen();
                return;
            }

            if (client.loginScreen == ExistingUserScreen)
            {
                HandleExistingUserScreen();
            }
        }

        private void HandleExistingUserScreen()
        {
            if (client.loginMenuLogin is null)
            {
                return;
            }

            client.loginMenuLogin.MouseClick(
                client.mouseX,
                client.mouseY,
                client.lastMouseButton,
                client.mouseButton);

            if (client.loginMenuLogin.IsClicked(client.loginMenuCancelButton))
            {
                client.loginScreen = IntroLoginScreen;
            }

            if (client.loginMenuLogin.IsClicked(client.loginMenuUserText))
            {
                client.loginMenuLogin.SetFocus(client.loginMenuPasswordText);
            }

            if (client.loginMenuLogin.IsClicked(client.loginMenuPasswordText) ||
                client.loginMenuLogin.IsClicked(client.loginMenuOkLoginButton))
            {
                SubmitExistingUserLogin();
            }
        }

        private void HandleIntroLoginScreen()
        {
            if (client.loginMenuFirst is null)
            {
                return;
            }

            client.loginMenuFirst.MouseClick(
                client.mouseX,
                client.mouseY,
                client.lastMouseButton,
                client.mouseButton);

            if (client.loginMenuFirst.IsClicked(client.loginButtonNewUser))
            {
                client.loginScreen = NewUserScreen;
            }

            if (client.loginMenuFirst.IsClicked(client.loginMenuLoginButton))
            {
                OpenExistingUserScreen();
            }
        }

        private void HandleNewUserScreen()
        {
            if (client.loginNewUser is null)
            {
                return;
            }

            client.loginNewUser.MouseClick(
                client.mouseX,
                client.mouseY,
                client.lastMouseButton,
                client.mouseButton);

            if (client.loginNewUser.IsClicked(client.loginMenuOkButton))
            {
                client.loginScreen = IntroLoginScreen;
            }
        }

        private void OpenExistingUserScreen()
        {
            client.loginScreen = ExistingUserScreen;
            client.loginMenuLogin.UpdateText(client.loginMenuStatusText, LoginPromptText);
            client.loginMenuLogin.UpdateText(client.loginMenuUserText, string.Empty);
            client.loginMenuLogin.UpdateText(client.loginMenuPasswordText, string.Empty);
            client.loginMenuLogin.SetFocus(client.loginMenuUserText);
        }

        private void SubmitExistingUserLogin()
        {
            client.loginUsername = client.loginMenuLogin.GetText(client.loginMenuUserText);
            client.loginPassword = client.loginMenuLogin.GetText(
                client.loginMenuPasswordText);
            client.Connect(client.loginUsername, client.loginPassword, false);
        }

        private void UpdateSocketTimeout()
        {
            if (client.socketTimeout > 0)
            {
                client.socketTimeout -= 1;
            }
        }
    }
}