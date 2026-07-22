using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class GameObjectLocationEntity : EntityBase
    {
        public int XCoordinate { get; set; }

        public int YCoordinate { get; set; }

        public int Direction { get; set; }

        public int Type { get; set; }
    }
}
