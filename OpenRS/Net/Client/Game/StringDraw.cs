using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRS.Net.Client.Game
{
    public sealed class StringDraw
    {
        public string Text { get; set; }

        public Vector2 DrawPosition { get; set; }

        public Color ForeColour { get; set; } = new(255, 0, 0, 255);

        public SpriteFont Font { get; set; }
    }
}
