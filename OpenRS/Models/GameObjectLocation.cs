using NuciXNA.Primitives;

namespace OpenRS.Models
{
    public sealed class GameObjectLocation
    {
        public Point2D Location { get; set; }

        public int Direction { get; set; }

        public GameObjectType Type { get; set; }
    }
}
