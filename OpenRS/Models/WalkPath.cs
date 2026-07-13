using NuciXNA.Primitives;

namespace OpenRS.Models
{
    public sealed class WalkPath
    {
        private readonly Point2D[] waypointOffsets;
        public Point2D StartLocation { get; set; }
        public int Length => waypointOffsets.Length;
        public WalkPath(Point2D startLocation, Point2D[] waypointOffsets)
        {
            StartLocation = startLocation;
            this.waypointOffsets = waypointOffsets;
        }
        public WalkPath(Point2D location, Point2D destination)
        {
            StartLocation = destination;
            waypointOffsets = [];
        }
        public Point2D GetWaypoint(int waypointIndex) => StartLocation + waypointOffsets[waypointIndex];
        public Point2D GetWaypointOffset(int waypointIndex)
        {
            if (waypointIndex >= Length)
            {
                return Point2D.Empty;
            }

            return waypointOffsets[waypointIndex];
        }
    }
}
