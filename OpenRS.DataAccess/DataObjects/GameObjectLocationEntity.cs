using NuciXNA.DataAccess.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public class GameObjectLocationEntity : EntityBase
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Direction { get; set; }

        public int Type { get; set; }
    }
}
