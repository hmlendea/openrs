using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class ElevationEntity : EntityBase
    {
        public int V1Id { get; set; }

        public int Roof { get; set; }

        public int Colour { get; set; }
    }
}
