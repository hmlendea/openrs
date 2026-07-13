using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class ItemEntity : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Command { get; set; }
        public int BasePrice { get; set; }
        public int SpriteId { get; set; }
        public int InventoryPicture { get; set; }
        public int PictureMask { get; set; }
        public int IsEquipable { get; set; }
        public int IsPremium { get; set; }
        public int IsSpecial { get; set; }
        public int IsStackable { get; set; }
        public int IsUnused { get; set; }
    }
}
