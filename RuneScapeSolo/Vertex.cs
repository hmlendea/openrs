using Microsoft.Xna.Framework;

namespace RuneScapeSolo
{
    /// <summary>
    /// Vertex.
    /// </summary>
	public class Vertex
    {
        /// <summary>
        /// Gets or sets the local point.
        /// </summary>
        /// <value>The local point.</value>
        public Vector3 LocalPoint { get; set; }

        /// <summary>
        /// Gets or sets the world point.
        /// </summary>
        /// <value>The world point.</value>
        public Vector3 WorldPoint { get; set; }

        /// <summary>
        /// Gets or sets the aligned point.
        /// </summary>
        /// <value>The aligned point.</value>
        public Vector3 AlignedPoint { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public Vertex(double x, double y, double z)
        {
            LocalPoint = new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="localPoint">Local point.</param>
        public Vertex(Vector3 localPoint)
        {
            LocalPoint = localPoint;
        }
    }
}
