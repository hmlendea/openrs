using OpenRSC.Models.Enumerations;
using OpenRSC.Primitives;

namespace OpenRSC.Models
{
    public class GameObjectLocation
    {
        public Point2D Location { get; set; }

        public int Direction { get; set; }

        public GameObjectType Type { get; set; }
    }
}
