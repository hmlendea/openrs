using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class ItemLocationEntity : EntityBase
    {
        public int XCoordinate { get; set; }

        public int YCoordinate { get; set; }

        public int Amount { get; set; }

        public int RespawnTime { get; set; }
    }
}
