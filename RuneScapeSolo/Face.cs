using Microsoft.Xna.Framework;

namespace RuneScapeSolo
{
    /// <summary>
    /// Face.
    /// </summary>
    public class Face
    {
        /// <summary>
        /// Gets or sets the colour of the face.
        /// </summary>
        /// <value>The colour.</value>
        public Color Colour { get; set; }

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>The image.</value>
        public int Image { get; private set; }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <value>The points.</value>
        public int[] Points { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="colour">Colour.</param>
        /// <param name="points">Points.</param>
        public Face(Color colour, int[] points)
        {
            Colour = colour;
            Image = -1;
            Points = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="points">Points.</param>
        public Face(int image, int[] points)
        {
            Image = image;
            Points = points;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Face"/> class.
        /// </summary>
        /// <param name="points">Points.</param>
        public Face(int[] points)
        {
            Colour = Color.Red;
            Image = -1;
            Points = points;
        }
    }
}
