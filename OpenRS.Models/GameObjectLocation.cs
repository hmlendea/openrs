using NuciXNA.Primitives;

using OpenRS.Models.Enumerations;

namespace OpenRS.Models
{
    public class GameObjectLocation
    {
        public Point2D Location { get; set; }

        public int Direction { get; set; }

        public GameObjectType Type { get; set; }
    }
}
