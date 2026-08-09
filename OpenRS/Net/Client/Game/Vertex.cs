using Microsoft.Xna.Framework;

namespace OpenRS.Net.Client.Game
{
    public sealed class Vertex
    {
        private Vector3 localPoint;
        private Vector3 worldPoint;
        private Vector3 alignedPoint;

        public Vertex(double localPointX, double localPointY, double localPointZ)
        {
            localPoint = new Vector3(
                (float)localPointX,
                (float)localPointY,
                (float)localPointZ);
        }

        public Vertex(Vector3 localPoint)
        {
            this.localPoint = localPoint;
        }

        public Vector3 GetLocalPoint() => localPoint;

        public void SetLocalPoint(Vector3 localPoint) => this.localPoint = localPoint;

        public Vector3 GetWorldPoint() => worldPoint;

        public void SetWorldPoint(Vector3 worldPoint) => this.worldPoint = worldPoint;

        public Vector3 GetAlignedPoint() => alignedPoint;

        public void SetAlignedPoint(Vector3 alignedPoint) => this.alignedPoint = alignedPoint;
    }
}
