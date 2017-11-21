using RuneScapeSolo.Models.Enumerations;
using RuneScapeSolo.Primitives;

namespace RuneScapeSolo.Models
{
    public class GameObjectLocation
    {
        public Point2D Location { get; set; }

        public int Direction { get; set; }

        public GameObjectType Type { get; set; }
    }
}
