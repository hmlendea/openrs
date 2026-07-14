using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRS.Net.Client.Game
{
    public sealed class StringDraw
    {
        public string text { get; set; }
        public Vector2 drawPosition { get; set; }

        public Color forecolor = new(255, 0, 0, 255);

        public SpriteFont font { get; set; }
    }
}
