using System;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRS.Net.Client
{
    public sealed class GameFrame
    {
        public GameFrame(GameApplet applet, int width, int height, string title, bool resizable, bool translate)
        {
            yOffset = 28;
            frameWidth = width;
            frameHeight = height;
            gameApplet = applet;
            if (translate)
            {
                yOffset = 48;
            }
            else
            {
                yOffset = 28;
            }

            gameApplet.mouseYOffset = 0;// 24;
            //setTitle(title);
            //setResizable(resizable);
            //show();
            //toFront();
            Resize(frameWidth, frameHeight);

            //addWindowListener(this);
        }

        //public GraphicsDevice GetGraphics()
        //{
        //    GraphicsDevice g = gameApplet.graphics; //super.GetGraphics();
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
            //super.Resize(i, j + yOffset);
        }

        public void Paint(GraphicsDevice graphicsDevice)
        {
            gameApplet.Paint(graphicsDevice);
        }

        public void WindowClosed(EventArgs evt)
        {
            if (gameApplet.runStatus != -1)
            {
                gameApplet.Destroy();
            }
        }

        public void WindowClosing(EventArgs evt)
        {
            if (gameApplet.runStatus != -1)
            {
                gameApplet.Destroy();
            }
        }

        public int frameWidth;
        public int frameHeight;
        public int yOffset;
        public GameApplet gameApplet;
    }
}
