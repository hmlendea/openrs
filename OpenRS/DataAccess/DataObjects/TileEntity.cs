using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class TileEntity : EntityBase
    {
        public int V1Id { get; set; }

        public int Colour { get; set; }

        public int Unknown { get; set; }

        public int Type { get; set; }
    }
}
