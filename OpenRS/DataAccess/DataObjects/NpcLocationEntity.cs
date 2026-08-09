using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class NpcLocationEntity : EntityBase
    {
        public int InitialXCoordinate { get; set; }

        public int InitialYCoordinate { get; set; }

        public int MinimumXCoordinate { get; set; }

        public int MinimumYCoordinate { get; set; }

        public int MaximumXCoordinate { get; set; }

        public int MaximumYCoordinate { get; set; }
    }
}
