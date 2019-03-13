namespace OpenRSC.Net.Client
{
    public class GameFrame
    {
        public GameApplet GameApplet { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int OffsetY { get; set; }

        public GameFrame(GameApplet gameApplet, int width, int height, bool doTranslation)
        {
            GameApplet = gameApplet;
            FrameWidth = width;
            FrameHeight = height;
            OffsetY = 28;

            if (doTranslation)
            {
                OffsetY = 48;
            }
            else
            {
                OffsetY = 28;
            }
        }

        public void Paint()
        {
            GameApplet.paint();
        }
    }
}
