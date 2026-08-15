using Microsoft.Xna.Framework.Input;

namespace OpenRS.Net.Client
{
    internal static class GameAppletKeyboardInputHandler
    {
        private static int InputTextMaximumLength => 20;

        private static int PrivateMessageMaximumLength => 80;

        internal static void HandleKeyDown(
            GameApplet applet,
            Keys key,
            char character)
        {
            if (key == Keys.Left)
            {
                applet.keyLeftDown = true;
            }

            if (key == Keys.Right)
            {
                applet.keyRightDown = true;
            }

            if (key == Keys.Up)
            {
                applet.keyUpDown = true;
            }

            if (key == Keys.Down)
            {
                applet.keyDownDown = true;
            }

            if (key == Keys.Space)
            {
                applet.keySpaceDown = true;
            }

            if (key == Keys.N || key == Keys.M)
            {
                applet.keyNMDown = true;
            }

            if (key == Keys.F1)
            {
                applet.keyF1Toggle = !applet.keyF1Toggle;
            }

            bool characterIsAllowed = IsCharacterAllowed(key, character);

            if (characterIsAllowed &&
                applet.inputText.Length < InputTextMaximumLength)
            {
                applet.inputText += character;
            }

            if (characterIsAllowed &&
                applet.pmText.Length < PrivateMessageMaximumLength)
            {
                applet.pmText += character;
            }

            if (key == Keys.Back && applet.inputText.Length > 0)
            {
                applet.inputText = applet.inputText[..^1];
            }

            if (key == Keys.Back && applet.pmText.Length > 0)
            {
                applet.pmText = applet.pmText[..^1];
            }

            if (key == Keys.Enter)
            {
                applet.enteredInputText = applet.inputText;
                applet.enteredPMText = applet.pmText;
            }
        }

        internal static void HandleKeyUp(GameApplet applet, Keys key)
        {
            if (key == Keys.Left)
            {
                applet.keyLeftDown = false;
            }

            if (key == Keys.Right)
            {
                applet.keyRightDown = false;
            }

            if (key == Keys.Up)
            {
                applet.keyUpDown = false;
            }

            if (key == Keys.Down)
            {
                applet.keyDownDown = false;
            }

            if (key == Keys.Space)
            {
                applet.keySpaceDown = false;
            }

            if (key == Keys.N || key == Keys.M)
            {
                applet.keyNMDown = false;
            }
        }

        private static bool IsCharacterAllowed(Keys key, char character)
        {
            for (int characterIndex = 0;
                characterIndex < GameApplet.AllowedChars.Length;
                characterIndex += 1)
            {
                if (character != GameApplet.AllowedChars[characterIndex] &&
                    key != Keys.Left &&
                    key != Keys.Right &&
                    key != Keys.Up &&
                    key != Keys.Down)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}