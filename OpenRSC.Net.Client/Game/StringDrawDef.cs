using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRSC.Net.Client.Game
{
    public class StringDraw
    {
        public string Text { get; set; }

        public Vector2 Location { get; set; }

        public Color Colour { get; set; }

        public SpriteFont Font { get; set; }

        public StringDraw()
        {
            Colour = new Color(255, 0, 0, 255);
        }
    }
}
