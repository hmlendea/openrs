using Microsoft.Xna.Framework;

namespace OpenRS.Net.Client.Game
{
    public sealed class Face
    {
        public int Image { get; } = NoImage;

        public int[] Points { get; }

        public Color FaceColour { get; set; }

        private static int NoImage => -1;

        public Face(int[] points)
        {
            Points = points;
            FaceColour = Color.Red;
        }

        public Face(Color colour, int[] points)
        {
            Points = points;
            FaceColour = colour;
        }

        public Face(int image, int[] points)
        {
            Points = points;
            Image = image;
        }
    }
}
