namespace OpenRS.Net.Client
{
    internal static class GameAppletMouseInputHandler
    {
        private static int PrimaryButton => 1;

        private static int SecondaryButton => 2;

        internal static void Move(GameApplet applet, int x, int y)
        {
            applet.mouseX = x;
            applet.mouseY = y - applet.mouseYOffset;
            applet.mouseButton = 0;
        }

        internal static void Press(
            GameApplet applet,
            int x,
            int y,
            bool isMetaDown)
        {
            applet.mouseX = x;
            applet.mouseY = y - applet.mouseYOffset;
            applet.mouseButton = PrimaryButton;

            if (isMetaDown)
            {
                applet.mouseButton = SecondaryButton;
            }

            applet.lastMouseButton = applet.mouseButton;
            applet.HandleMouseDown(applet.mouseButton, x, y);
        }

        internal static void Drag(
            GameApplet applet,
            int x,
            int y,
            bool isMetaDown)
        {
            applet.mouseX = x;
            applet.mouseY = y - applet.mouseYOffset;
            applet.mouseButton = PrimaryButton;

            if (isMetaDown)
            {
                applet.mouseButton = SecondaryButton;
            }
        }
    }
}