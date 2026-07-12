using NuciXNA.Primitives;

namespace OpenRS.Models
{
    public sealed class NpcLocation
    {
        public string Id { get; set; }

        public Point2D InitialCoordinates { get; set; }

        public Point2D MinimumCoordinates { get; set; }

        public Point2D MaximumCoordinates { get; set; }
    }
}
