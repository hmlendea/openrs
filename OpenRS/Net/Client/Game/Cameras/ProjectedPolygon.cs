namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class ProjectedPolygon
    {
        internal int[] X { get; }

        internal int[] Y { get; }

        internal ProjectedPolygon(int[] x, int[] y)
        {
            X = x;
            Y = y;
        }
    }
}
