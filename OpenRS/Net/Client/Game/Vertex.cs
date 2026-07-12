// -----------------------------------------------------------------------
// <copyright file="Vertex.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace OpenRS
{
    /// <summary>
	/// TODO: Update summary.
	/// </summary>
	public sealed class Vertex
	{

		private Vector3 localPoint;
		private Vector3 worldPoint;
		private Vector3 alignedPoint;

		public Vertex(double d, double e, double f)
		{
			localPoint = new Vector3((float)d, (float)e, (float)f);
		}

		public Vertex(Vector3 localPoint)
		{
			this.localPoint = localPoint;
		}

		public Vector3 GetLocalPoint()
		{
			return localPoint;
		}

		public void SetLocalPoint(Vector3 localPoint)
		{
			this.localPoint = localPoint;
		}

		public Vector3 GetWorldPoint()
		{
			return worldPoint;
		}

		public void SetWorldPoint(Vector3 worldPoint)
		{
			this.worldPoint = worldPoint;
		}

		public Vector3 GetAlignedPoint()
		{
			return alignedPoint;
		}

		public void SetAlignedPoint(Vector3 alignedPoint)
		{
			this.alignedPoint = alignedPoint;
		}
	}
}
