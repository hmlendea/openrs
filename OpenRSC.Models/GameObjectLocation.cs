using NuciXNA.Primitives;

using OpenRSC.Models.Enumerations;

namespace OpenRSC.Models
{
    public class GameObjectLocation
    {
        public Point2D Location { get; set; }

        public int Direction { get; set; }

        public GameObjectType Type { get; set; }
    }
}
