// -----------------------------------------------------------------------
// <copyright file="Face.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace OpenRS
{
    /// <summary>
	/// TODO: Update summary.
	/// </summary>
	public sealed class Face
	{
		private int[] points;
		private Color faceColor;
		private int image = -1;
		public Face(Color c, int[] points)
		{
			this.points = points;
			faceColor = c;
		}

		public Face(int image, int[] points)
		{
			this.points = points;
			this.image = image;
		}

		public Face(int[] points)
		{
			this.points = points;
			faceColor = Color.Red;
		}

		public int GetImage()
		{
			return image;
		}

		public int[] GetPoints()
		{
			return points;
		}

		public Color GetFaceColour()
		{
			return faceColor;
		}

		public void SetFaceColor(Color faceColor)
		{
			this.faceColor = faceColor;
		}
	}
}
