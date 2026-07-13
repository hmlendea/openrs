using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class ElevationEntity : EntityBase
    {
        public int Roof { get; set; }

        public int Unknown { get; set; }
    }
}
