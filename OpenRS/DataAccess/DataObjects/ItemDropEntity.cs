using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class ItemDropEntity : EntityBase
    {
        public string ItemId { get; set; }
        public int Amount { get; set; }
        public int Weight { get; set; }
    }
}
