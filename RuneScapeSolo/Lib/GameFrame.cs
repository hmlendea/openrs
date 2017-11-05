using System;

using Microsoft.Xna.Framework.Graphics;

namespace RuneScapeSolo.Lib
{
    public class GameFrame
    {
        public GameFrame(GameApplet gameApplet, int width, int height, string title, bool isResizable, bool doTranslation)
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

            //setTitle(title);
            //setResizable(resizable);
            //show();
            //toFront();
            Resize(FrameWidth, FrameHeight);

            //addWindowListener(this);
        }

        //public GraphicsDevice getGraphics()
        //{
        //    GraphicsDevice g = gameApplet.graphics; //super.getGraphics();
        //    //if (fej == 0)
        //        //g.translate(0, 24);
        //        //g.Viewport = new Viewport(0, 24, ); = 24;
        //    //else
        //        //g.translate(-5, 0);
        //        //g.Viewport.X -= 5;
        //    return g;
        //}

        public void Resize(int i, int j)
        {
            //super.resize(i, j + yOffset);
        }

        public void Paint(GraphicsDevice g)
        {
            GameApplet.paint(g);
        }

        public void WindowClosed(EventArgs evt)
        {
            if (GameApplet.runStatus != -1)
            {
                GameApplet.Destroy();
            }
        }

        public void WindowClosing(EventArgs evt)
        {
            if (GameApplet.runStatus != -1)
            {
                GameApplet.Destroy();
            }
        }

        public GameApplet GameApplet { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int OffsetY { get; set; }
    }
}
